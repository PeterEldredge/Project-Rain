using UnityEngine;
using Opsive.UltimateCharacterController.Inventory;

namespace PixelCrushers.UCCSupport
{

    /// <summary>
    /// Utility functions that Pixel Crushers integrations use to work with
    /// Opsive's Ultimate Character Controllers.
    /// </summary>
    public static class UCCUtility
    {

        /// <summary>
        /// Searches for an ItemCollection within the scene.
        /// </summary>
        public static ItemCollection FindItemCollection(GameObject character)
        {
            var itemSetManager = character.GetComponent<ItemSetManager>();
            if (itemSetManager.ItemCollection != null)
            {
                return itemSetManager.ItemCollection;
            }

            var itemCollection = GameObject.FindObjectOfType<ItemCollection>();
            if (itemCollection != null)
            {
                return itemCollection;
            }

            return null;
        }

        /// <summary>
        /// Looks up an ItemType in an ItemCollection by ID.
        /// </summary>
        public static ItemType GetItemType(ItemCollection itemCollection, int id)
        {
            if (itemCollection != null && itemCollection.ItemTypes != null)
            {
                for (int i = 0; i < itemCollection.ItemTypes.Length; i++)
                {
                    var itemType = itemCollection.ItemTypes[i];
                    if (itemType != null && itemType.ID == id)
                    {
                        return itemType;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Looks up an ItemType in an ItemCollection by name.
        /// </summary>
        public static ItemType GetItemType(ItemCollection itemCollection, string itemTypeName)
        {
            if (itemCollection != null && itemCollection.ItemTypes != null)
            {
                for (int i = 0; i < itemCollection.ItemTypes.Length; i++)
                {
                    var itemType = itemCollection.ItemTypes[i];
                    if (itemType != null && string.Equals(itemType.name, itemTypeName))
                    {
                        return itemType;
                    }
                }
            }
            return null;
        }
    }
}
