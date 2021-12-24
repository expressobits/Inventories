using System.Collections.Generic;
using UnityEngine;

namespace ExpressoBits.Inventories
{
    public class Item : ScriptableObject, IItem
    {
        public Database Database => database;
        public ushort ID => id;
        public string Name => name;
        public string Description => description;
        public Sprite Icon => icon;
        public Category Category => category;
        public ushort MaxStack => maxStack;
        //public ItemObject ItemObjectPrefab => itemObjectPrefab;
        public float Weight => weight;

        [SerializeField] private Database database;
        [SerializeField] private ushort id = 0;
        [SerializeField, TextArea] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField, Min(0.01f)] private float weight = 0.1f;
        [SerializeField, Min(1)] private ushort maxStack = 64;
        //[SerializeField] private ItemObject itemObjectPrefab;
        [SerializeField] private Category category;
        
        [SerializeReference]
        public List<ItemComponent> components = new List<ItemComponent>();

        internal void Setup(Database database, ushort id)
        {
            this.database = database;
            this.id = id;
        }

        public static implicit operator ushort(Item item)
        {
            return item.ID;
        }

        #region Components
        /// <summary>Get an existing component of a specific type from the item component.</summary>
        /// <typeparam name="T">The type of component to get</typeparam>
        /// <returns>The component if it's present, or null</returns>
        public T GetComponent<T>() where T : ItemComponent
        {
            if (components != null)
            {
                foreach (var c in components)
                {
                    if (c is T t) return t;
                }
            }
            return default(T);
        }

        public bool TryGetComponent<T>(out T component) where T : ItemComponent
        {
            if (components != null)
            {
                foreach (var c in components)
                {
                    if (c is T t)
                    {
                        component = t;
                        return true;
                    }
                }
            }
            component = default(T);
            return false;
        }
        #endregion

    }
}