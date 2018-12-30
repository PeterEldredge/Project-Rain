using UnityEngine;
using System;
using System.Collections.Generic;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Traits;
using Opsive.UltimateCharacterController.Inventory;
using Opsive.UltimateCharacterController.Items.Actions;

namespace PixelCrushers.UCCSupport
{

    /// <summary>
    /// Saves an Opsive Ultimate Character Controller's position, attributes, and/or inventory.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Common/Save System/Opsive/UCC Saver")]
    [RequireComponent(typeof(UltimateCharacterLocomotion))]
    public class UCCSaver : Saver
    {
        public bool savePosition = true;
        public bool saveAttributes = true;
        public bool saveInventory = true;
        public bool debug = false;

        [Serializable]
        public class Data // Holds the character's save data.
        {
            public Vector3 position;
            public Quaternion rotation;
            public List<float> attributes = new List<float>();
            public List<ItemData> items = new List<ItemData>();
        }

        [Serializable]
        public class ItemData // Holds the save data for an item in the character's inventory.
        {
            public int itemID;
            public int slot;
            public float count;
            public List<ItemActionData> itemActionData = new List<ItemActionData>();
        }

        [Serializable]
        public class ItemActionData // Holds the save data for an ItemAction.
        {
            public int id;
            public float count;
            public float consumableCount;

            public ItemActionData(int _id, float _count, float _consumableCount)
            {
                id = _id;
                count = _count;
                consumableCount = _consumableCount;
            }
        }

        public override string RecordData()
        {
            var data = new Data();

            // Save position:
            data.position = transform.position;
            data.rotation = transform.rotation;

            // Save attributes:
            if (saveAttributes)
            {
                var attributeManager = GetComponent<AttributeManager>();
                if (attributeManager == null)
                {
                    if (Debug.isDebugBuild) Debug.LogWarning("UCC Saver can't save attributes. " + name + " doesn't have an Attribute Manager.", this);
                }
                else
                {
                    for (int i = 0; i < attributeManager.Attributes.Length; i++)
                    {
                        data.attributes.Add(attributeManager.Attributes[i].Value);
                    }
                }
            }

            // Save inventory:
            if (saveInventory)
            {
                var inventory = GetComponent<InventoryBase>();
                if (inventory == null)
                {
                    if (Debug.isDebugBuild) Debug.LogWarning("UCC Saver can't save inventory. " + name + " doesn't have an Inventory component.", this);
                }
                else
                {
                    var items = inventory.GetAllItems();
                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        if (item != null && item.ItemType != null)
                        {
                            var itemData = new ItemData();
                            itemData.itemID = item.ItemType.ID;
                            itemData.slot = item.SlotID;
                            itemData.count = inventory.GetItemTypeCount(item.ItemType);
                            for (int j = 0; j < item.ItemActions.Length; j++)
                            {
                                var usableItem = item.ItemActions[j] as UsableItem;
                                if (usableItem != null)
                                {
                                    itemData.itemActionData.Add(new ItemActionData(usableItem.ID, inventory.GetItemTypeCount(usableItem.GetConsumableItemType()), usableItem.GetConsumableItemTypeCount()));
                                }
                            }
                            data.items.Add(itemData);
                        }
                    }
                }
            }
            var s = SaveSystem.Serialize(data);
            if (debug) Debug.Log("UCC Saver on " + name + " saving: " + s, this);
            return s;
        }

        public override void ApplyData(string s)
        {
            if (string.IsNullOrEmpty(s)) return;
            var data = SaveSystem.Deserialize<Data>(s);
            if (data == null)
            {
                Debug.LogWarning("UCC Saver on " + name + " received invalid data. Can't apply: " + s, this);
                return;
            }
            var character = GetComponent<UltimateCharacterLocomotion>();

            // Restore position:
            if (savePosition)
            {
                if (CompareTag("Player") && SaveSystem.playerSpawnpoint != null)
                {
                    if (debug) Debug.Log("UCC Saver on " + name + " moving character to spawnpoint " + SaveSystem.playerSpawnpoint, this);
                    character.SetPositionAndRotation(SaveSystem.playerSpawnpoint.transform.position, SaveSystem.playerSpawnpoint.transform.rotation);
                }
                else
                {
                    if (debug) Debug.Log("UCC Saver on " + name + " (tag=" + tag + ") restoring saved position " + data.position, this);
                    character.SetPositionAndRotation(data.position, data.rotation);
                }
            }

            // Restore attributes:
            if (saveAttributes)
            {
                var attributeManager = GetComponent<AttributeManager>();
                if (attributeManager == null)
                {
                    if (Debug.isDebugBuild) Debug.LogWarning("UCC Saver can't load attributes. " + name + " doesn't have an Attribute Manager.", this);
                }
                else
                {
                    if (debug) Debug.Log("UCC Saver on " + name + " restoring attributes", this);
                    var count = Mathf.Min(attributeManager.Attributes.Length, data.attributes.Count);
                    for (int i = 0; i < count; i++)
                    {
                        attributeManager.Attributes[i].Value = data.attributes[i];
                    }
                }
            }

            // Restore inventory:
            if (saveInventory)
            {
                var inventory = GetComponent<InventoryBase>();
                if (inventory == null)
                {
                    if (Debug.isDebugBuild) Debug.LogWarning("UCC Saver can't load inventory. " + name + " doesn't have an Inventory component.", this);
                }
                else
                {
                    // Clear inventory:
                    var items = inventory.GetAllItems();
                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        if (item != null && item.ItemType != null)
                        {
                            inventory.RemoveItem(item.ItemType, item.SlotID, false);
                        }
                    }

                    var itemCollection = UCCUtility.FindItemCollection(character.gameObject);
                    if (itemCollection == null)
                    {
                        Debug.LogError("Error: Unable to find ItemCollection.");
                        return;
                    }
                    // Add saved items:
                    for (int i = 0; i < data.items.Count; i++)
                    {
                        var itemData = data.items[i];
                        var itemType = UCCUtility.GetItemType(itemCollection, itemData.itemID);
                        inventory.PickupItemType(itemType, itemData.count, itemData.slot, true, false);
                        var item = inventory.GetItem(itemData.slot, itemType);
                        if (item != null)
                        {
                            for (int j = 0; j < item.ItemActions.Length; j++)
                            {
                                var usableItem = item.ItemActions[j] as UsableItem;
                                if (usableItem == null)
                                {
                                    continue;
                                }

                                for (int k = 0; k < itemData.itemActionData.Count; ++k)
                                {
                                    if (usableItem.ID != itemData.itemActionData[k].id)
                                    {
                                        continue;
                                    }

#if ULTIMATE_CHARACTER_CONTROLLER_SHOOTER
                                    var shootableWeapon = usableItem as ShootableWeapon;
                                    if (shootableWeapon != null)
                                    {
                                        // Temporarily fill clip so inventory.PickupItemType doesn't auto-reload:
                                        usableItem.SetConsumableItemTypeCount(shootableWeapon.ClipSize);
                                    }
#endif
                                    if (debug && itemData.itemActionData[k].count > 0) Debug.Log("UCC Saver on " + name + " restoring item: " + usableItem.name, this);
                                    inventory.PickupItemType(usableItem.GetConsumableItemType(), itemData.itemActionData[k].count, -1, true, false);
                                    usableItem.SetConsumableItemTypeCount(itemData.itemActionData[k].consumableCount);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
