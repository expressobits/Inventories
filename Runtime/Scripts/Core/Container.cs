using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ExpressoBits.Inventories
{
    [AddComponentMenu("Inventories/" + nameof(Container))]
    public class Container : MonoBehaviour, IContainer<Item>, IEnumerable<Slot>
    {
        public Database Database => database;
        public int Count => slots.Count;
        public bool IsOpen => isOpen;
        public float Weight
        {
            get
            {
                float total = 0;
                foreach (Slot slot in slots) total += slot.Weight;
                return total;
            }
        }

        [SerializeField] private Database database;
        [SerializeField] private List<Slot> slots = new List<Slot>();
        [SerializeField] private bool limitedSlots;
        [SerializeField] private int limitedAmountOfSlots = 8;
        [SerializeField] private bool isOpen;

        #region Actions
        public delegate void ItemEvent(Item item, ushort amount);
        public ItemEvent OnItemAdd;
        public ItemEvent OnItemRemove;

        public Action<Slot> OnAdd;
        public Action<Slot> OnRemove;
        public Action<int> OnRemoveAt;
        public Action<int> OnUpdate;

        public Action OnOpen;
        public Action OnClose;
        /// <summary>
        /// Basic client received update event
        /// </summary>
        public Action OnChanged;
        #endregion

        #region Unity Events
        public UnityEvent<Item,ushort> OnItemAddUnityEvent;
        public UnityEvent<Item,ushort> OnItemRemoveUnityEvent;

        public UnityEvent<Slot> OnAddUnityEvent;
        public UnityEvent<Slot> OnRemoveUnityEvent;
        public UnityEvent<int> OnRemoveAtUnityEvent;
        public UnityEvent<int> OnUpdateUnityEvent;

        public UnityEvent OnOpenUnityEvent;
        public UnityEvent OnCloseUnityEvent;

        public UnityEvent OnChangedUnityEvent;
        #endregion

        private void Awake()
        {
            //slots = new List<Slot>();
        }

        #region IContainer Functions
        public ushort AddItemAt(Item item, int index, ushort amount = 1)
        {
            ushort valueToAdd = amount;
            if (slots.Count > index)
            {
                Slot slot = slots[index];
                if(slot.Item == item)
                {
                    valueToAdd = slot.Add(valueToAdd);
                    this[index] = slot;
                }
            }
            if ((!limitedSlots || slots.Count < limitedAmountOfSlots) && valueToAdd > 0)
            {
                Add(new Slot(item, amount) { });
                valueToAdd = 0;
            }
            OnItemAdd?.Invoke(item, (ushort) (amount - valueToAdd));
            OnItemAddUnityEvent?.Invoke(item, (ushort) (amount - valueToAdd));
            OnChanged?.Invoke();
            OnChangedUnityEvent?.Invoke();
            return valueToAdd;
        }

        public ushort AddItem(Item item, ushort amount = 1)
        {
            ushort valueToAdd = amount;
            for (int i = 0; i < slots.Count; i++)
            {
                Slot slot = slots[i];
                if (slot.ItemID == item.ID)
                {
                    valueToAdd = slot.Add(valueToAdd);
                    this[i] = slot;
                    if (valueToAdd == 0)
                    {
                        OnItemAdd?.Invoke(item, (ushort) (amount - valueToAdd));
                        OnItemAddUnityEvent?.Invoke(item, (ushort) (amount - valueToAdd));
                        OnChanged?.Invoke();
                        OnChangedUnityEvent?.Invoke();
                        return 0;
                    }
                }
            }
            if ((!limitedSlots || slots.Count < limitedAmountOfSlots) && valueToAdd > 0)
            {
                Add(new Slot(item, valueToAdd) { });
                valueToAdd = 0;
            }
            OnItemAdd?.Invoke(item, (ushort) (amount - valueToAdd));
            OnItemAddUnityEvent?.Invoke(item, (ushort) (amount - valueToAdd));
            OnChanged?.Invoke();
            OnChangedUnityEvent?.Invoke();
            return valueToAdd;
        }

        public void SetOpen(bool isOpen)
        {
            this.isOpen = isOpen;
        }

        public ushort RemoveItemAt(int index, ushort valueToRemove = 1)
        {
            ushort valueNoRemoved = valueToRemove;
            if (slots.Count > index)
            {
                Slot slot = slots[index];
                Item item = slot.Item;
                if (item != null)
                {
                    valueNoRemoved = slot.Remove(valueNoRemoved);
                    this[index] = slot;
                    if (slot.IsEmpty)
                    {
                        RemoveAt(index);
                    }
                }
                OnItemRemove?.Invoke(item, (ushort)(valueToRemove - valueNoRemoved));
                OnItemRemoveUnityEvent?.Invoke(item, (ushort)(valueToRemove - valueNoRemoved));
                OnChanged?.Invoke();
                OnChangedUnityEvent?.Invoke();
            }
            return valueNoRemoved;
        }

        public ushort RemoveItem(Item item, ushort valueToRemove = 1)
        {
            ushort valueNoRemoved = valueToRemove;
            for (int i = 0; i < slots.Count; i++)
            {
                Slot slot = slots[i];
                if (slot.Item == item.ID)
                {
                    valueNoRemoved = slot.Remove(valueNoRemoved);
                    this[i] = slot;
                    if (slot.IsEmpty)
                    {
                        RemoveAt(i);
                    }
                    if (valueNoRemoved == 0) return 0;
                }
            }
            OnItemRemove?.Invoke(item, (ushort)(valueToRemove - valueNoRemoved));
            OnItemRemoveUnityEvent?.Invoke(item, (ushort)(valueToRemove - valueNoRemoved));
            OnChanged?.Invoke();
            OnChangedUnityEvent?.Invoke();
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
                OnUpdate?.Invoke(index);
                OnUpdateUnityEvent?.Invoke(index);
                OnChanged?.Invoke();
            }
        }

        public void Add(Slot slot)
        {
            OnAdd?.Invoke(slot);
            OnAddUnityEvent?.Invoke(slot);
            slots.Add(slot);
            OnChanged?.Invoke();
            OnChangedUnityEvent?.Invoke();
        }

        public void RemoveAt(int index)
        {
            OnRemoveAt?.Invoke(index);
            OnRemoveAtUnityEvent?.Invoke(index);
            slots.RemoveAt(index);
            OnChanged?.Invoke();
            OnChangedUnityEvent?.Invoke();
        }

        public void Remove(Slot slot)
        {
            OnRemove?.Invoke(slot);
            OnRemoveUnityEvent?.Invoke(slot);
            slots.Remove(slot);
            OnChanged?.Invoke();
            OnChangedUnityEvent?.Invoke();
        }

        public int IndexOf(Slot slot)
        {
            return slots.IndexOf(slot);
        }

        public void Clear()
        {
            slots.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return slots.GetEnumerator();
        }

        IEnumerator<Slot> IEnumerable<Slot>.GetEnumerator()
        {
            return slots.GetEnumerator();
        }
        #endregion

        #region Utils
        public Slot ToSlot(uint s)
        {
            (ushort, ushort) split = Slot.SplitInt(s);
            ushort itemId = split.Item1;
            ushort amount = split.Item2;
            Item item = Database.GetItem(itemId);
            return new Slot(item, amount) { };
        }
        #endregion

        #region Storage Calls
        public void Open()
        {
            if(isOpen) return;
            isOpen = true;
            OnOpen?.Invoke();
            OnOpenUnityEvent?.Invoke();
        }

        public void Close()
        {
            if(!isOpen) return;
            isOpen = false;
            OnClose?.Invoke();
            OnCloseUnityEvent?.Invoke();
        }
        #endregion

    }
}