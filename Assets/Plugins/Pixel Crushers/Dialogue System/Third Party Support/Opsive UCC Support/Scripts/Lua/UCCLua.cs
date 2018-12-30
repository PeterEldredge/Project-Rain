using UnityEngine;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Traits;
using Opsive.UltimateCharacterController.Inventory;
using PixelCrushers.UCCSupport;

namespace PixelCrushers.DialogueSystem.OpsiveUCCSupport
{

    /// <summary>
    /// Adds Lua functions to the Dialogue System that control Opsive UCC characters.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Dialogue System/Third Party/Opsive/UCC Lua")]
    public class UCCLua : MonoBehaviour
    {
        protected static bool registered = false;

        private bool didIRegister = false;

        void OnEnable()
        {
            if (registered)
            {
                didIRegister = false;
            }
            else
            {
                didIRegister = true;
                registered = true;
                Lua.RegisterFunction("uccGetAttribute", this, SymbolExtensions.GetMethodInfo(() => uccGetAttribute(string.Empty, string.Empty)));
                Lua.RegisterFunction("uccSetAttribute", this, SymbolExtensions.GetMethodInfo(() => uccSetAttribute(string.Empty, string.Empty, (double)0)));
                Lua.RegisterFunction("uccGetItemCount", this, SymbolExtensions.GetMethodInfo(() => uccGetItemCount(string.Empty, string.Empty)));
                Lua.RegisterFunction("uccAddItem", this, SymbolExtensions.GetMethodInfo(() => uccAddItem(string.Empty, string.Empty, (double)0)));
                Lua.RegisterFunction("uccRemoveItem", this, SymbolExtensions.GetMethodInfo(() => uccRemoveItem(string.Empty, string.Empty, (double)0)));
            }
        }

        void OnDisable()
        {
            if (didIRegister)
            {
                didIRegister = false;
                registered = false;
                Lua.UnregisterFunction("uccGetAttribute");
                Lua.UnregisterFunction("uccSetAttribute");
                Lua.UnregisterFunction("uccGetItemCount");
                Lua.UnregisterFunction("uccAddItem");
                Lua.UnregisterFunction("uccRemoveItem");
            }
        }

        /// <summary>
        /// Finds an UltimateCharacterLocomotion by GameObject name. If the
        /// GameObject name is blank, finds the GameObject tagged 'Player'.
        /// </summary>
        public static UltimateCharacterLocomotion FindCharacter(string characterName)
        {
            if (string.IsNullOrEmpty(characterName))
            {
                var gos = GameObject.FindGameObjectsWithTag("Player");
                for (int i = 0; i < gos.Length; i++)
                {
                    var character = gos[i].GetComponent<UltimateCharacterLocomotion>();
                    if (character != null) return character;
                }
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Can't find Ultimate Character Locomotion player.");
            }
            else
            {
                var go = Tools.GameObjectHardFind(characterName);
                if (go != null)
                {
                    var character = go.GetComponent<UltimateCharacterLocomotion>();
                    if (character != null) return character;
                }
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Can't find Ultimate Character Locomotion GameObject named '" + characterName + "'.");
            }
            return null;
        }

        /// <summary>
        /// Finds an UltimateCharacterLocomotion and its Inventory and ItemCollection by GameObject name.
        /// If the name is blank, finds the GameObject tagged 'Player'.
        /// </summary>
        public static bool FindCharacterWithInventory(string characterName, out UltimateCharacterLocomotion character, out InventoryBase inventory, out ItemCollection itemCollection)
        {
            inventory = null;
            itemCollection = null;
            character = FindCharacter(characterName);
            if (character != null)
            {
                inventory = character.GetComponent<InventoryBase>();
                itemCollection = UCCUtility.FindItemCollection(character.gameObject);
                if (inventory == null)
                {
                    if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Character '" + character.name + "' doesn't have an Inventory.", character);
                }
                else if (itemCollection == null)
                {
                    if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Character '" + character.name + "' doesn't have access to an Item Set Manager or Item Collection.", character);
                }
            }
            return character != null && inventory != null && itemCollection != null;
        }

        /// <summary>
        /// Finds an ItemType by name in an ItemCollection.
        /// </summary>
        protected static ItemType FindItemType(ItemCollection itemCollection, string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Item name is blank.");
                return null;
            }
            var itemType = UCCUtility.GetItemType(itemCollection, itemName);
            if (itemType == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Item Collection doesn't contain an item type named '" + itemName + "'.");
            }
            return itemType;
        }

        /// <summary>
        /// Returns the value of an attribute on a character.
        /// </summary>
        /// <param name="characterName">GameObject name of character, or blank for player.</param>
        /// <param name="attributeName">Attribute name.</param>
        public static double uccGetAttribute(string characterName, string attributeName)
        {
            try
            { 
            var character = FindCharacter(characterName);
            if (character != null)
            {
                var attributeManager = character.GetComponent<AttributeManager>();
                if (attributeManager != null)
                {
                    for (int i = 0; i < attributeManager.Attributes.Length; i++)
                    {
                        if (string.Equals(attributeName, attributeManager.Attributes[i].Name))
                        {
                            return attributeManager.Attributes[i].Value;
                        }
                    }
                }
            }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            return 0;
        }

        /// <summary>
        /// Sets the value of an attribute on a character.
        /// </summary>
        /// <param name="characterName">GameObject name of character, or blank for player.</param>
        /// <param name="attributeName">Attribute name.</param>
        /// <param name="value">New value of attribute.</param>
        public static void uccSetAttribute(string characterName, string attributeName, double value)
        {
            try
            {
                var character = FindCharacter(characterName);
                if (character != null)
                {
                    var attributeManager = character.GetComponent<AttributeManager>();
                    if (attributeManager != null)
                    {
                        for (int i = 0; i < attributeManager.Attributes.Length; i++)
                        {
                            if (string.Equals(attributeName, attributeManager.Attributes[i].Name))
                            {
                                attributeManager.Attributes[i].Value = (float)value;
                                return;
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Returns the amount of an item that a character has in its inventory.
        /// </summary>
        /// <param name="characterName">GameObject name of character, or blank for player.</param>
        /// <param name="itemName">Item type name.</param>
        public static double uccGetItemCount(string characterName, string itemName)
        {
            try
            {
                UltimateCharacterLocomotion character;
                InventoryBase inventory;
                ItemCollection itemCollection;
                if (FindCharacterWithInventory(characterName, out character, out inventory, out itemCollection))
                {
                    var itemType = FindItemType(itemCollection, itemName);
                    if (itemType != null)
                    {
                        return inventory.GetItemTypeCount(itemType);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            return 0;
        }

        /// <summary>
        /// Adds an amount of an item to a character's inventory.
        /// </summary>
        /// <param name="characterName">GameObject name of character, or blank for player.</param>
        /// <param name="itemName">Item type name.</param>
        public static void uccAddItem(string characterName, string itemName, double amount)
        {
            try
            {
                UltimateCharacterLocomotion character;
                InventoryBase inventory;
                ItemCollection itemCollection;
                if (FindCharacterWithInventory(characterName, out character, out inventory, out itemCollection))
                {
                    var itemType = FindItemType(itemCollection, itemName);
                    if (itemType != null)
                    {
                        inventory.PickupItemType(itemType, (float)amount, -1, true, false);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Removes an item from a character's inventory.
        /// </summary>
        /// <param name="characterName">GameObject name of character, or blank for player.</param>
        /// <param name="itemName">Item type name.</param>
        public static void uccRemoveItem(string characterName, string itemName, double amount)
        {
            try
            {
                UltimateCharacterLocomotion character;
                InventoryBase inventory;
                ItemCollection itemCollection;
                if (FindCharacterWithInventory(characterName, out character, out inventory, out itemCollection))
                {
                    var itemType = FindItemType(itemCollection, itemName);
                    if (itemType != null)
                    {
                        for (int i = 0; i < (int)amount; i++)
                        {
                            var items = inventory.GetAllItems();
                            for (int j = 0; j < items.Count; j++)
                            {
                                var item = items[j];
                                if (item.ItemType == itemType)
                                {
                                    inventory.RemoveItem(itemType, item.SlotID, false);
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

    }
}