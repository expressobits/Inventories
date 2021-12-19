using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExpressoBits.Inventories
{
    public class Container : MonoBehaviour, IContainer<Item>
    {
        //public List<Slot> Slots => slots;
        public int Count => slots.Count;
        public float Weight
        {
            get
            {
                float total = 0;
                foreach (Slot slot in slots) total += slot.Weight;
                return total;
            }
        }

        [SerializeField] private List<Slot> slots = new List<Slot>();
        [SerializeField] private bool limitedSlots;
        [SerializeField] private int limitedAmountOfSlots = 8;

        #region Actions
        public Action<Item, ushort> OnAdd;
        public Action<Item, ushort> OnRemove;
        /// <summary>
        /// Basic client received update event
        /// </summary>
        public Action OnChanged;
        #endregion

        private void Awake()
        {
            slots = new List<Slot>();
        }

        #region IContainer Functions
        public ushort Add(Item item, ushort amount = 1)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                Slot slot = slots[i];
                if (slot.ItemID == item.ID)
                {
                    amount = slot.Add(amount);
                    slots[i] = slot;
                    if (amount == 0)
                    {
                        OnAdd?.Invoke(item, amount);
                        OnChanged?.Invoke();
                        return 0;
                    }
                }
            }
            if (!limitedSlots || slots.Count < limitedAmountOfSlots)
            {
                slots.Add(new Slot(item.ID, amount) { });
                amount = 0;
            }
            OnAdd?.Invoke(item, amount);
            OnChanged?.Invoke();
            return amount;
        }

        public ushort RemoveInIndex(int index, ushort valueToRemove = 1)
        {
            ushort valueNoRemoved = valueToRemove;
            if (slots.Count > index)
            {
                Slot slot = slots[index];
                Item item = slot.Item;
                if (item != null)
                {
                    valueNoRemoved = slot.Remove(valueNoRemoved);
                    slots[index] = slot;
                    if (slot.IsEmpty)
                    {
                        slots.RemoveAt(index);
                    }
                }
                OnRemove?.Invoke(item, (ushort)(valueToRemove - valueNoRemoved));
                OnChanged?.Invoke();
            }
            return valueNoRemoved;
        }

        public ushort Remove(Item item, ushort valueToRemove = 1)
        {
            ushort valueNoRemoved = valueToRemove;
            for (int i = 0; i < slots.Count; i++)
            {
                Slot slot = slots[i];
                if (slot.Item == item.ID)
                {
                    valueNoRemoved = slot.Remove(valueNoRemoved);
                    slots[i] = slot;
                    if (slot.IsEmpty)
                    {
                        slots.RemoveAt(i);
                    }
                    if (valueNoRemoved == 0) return 0;
                }
            }
            OnRemove?.Invoke(item, (ushort)(valueToRemove - valueNoRemoved));
            OnChanged?.Invoke();
            return valueNoRemoved;
        }

        public bool Has(Item item)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                Slot slot = slots[i];
                if (slot.ItemID == item.ID && !slot.IsEmpty)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Has(Item item, ushort amount)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                Slot slot = slots[i];
                if (slot.ItemID == item.ID)
                {
                    amount = (ushort)Mathf.Max(amount - slot.Amount, 0);
                }
                if (amount <= 0) return true;
            }
            return false;
        }

        public Slot this[int index]
        {
            get { return slots[index]; }
            set
            {
                slots[index] = value;
                OnChanged?.Invoke();
            }
        }

        public int IndexOf(Slot slot)
        {
            return slots.IndexOf(slot);
        }

        public void Clear()
        {
            slots.Clear();
        }
        #endregion

    }
}