using System;
using UnityEngine;

namespace ExpressoBits.Inventories
{
    [Serializable]
    public struct Slot : IEquatable<Slot>, ISlot<Item>
    {
        public Item Item => item;
        public ushort ItemID => item.ID;
        public ushort Amount => amount;
        public ushort MaxStack => Item.MaxStack;
        public ushort Remaining => (ushort)(MaxStack - Amount);
        public bool IsEmpty => Amount <= 0;
        public bool IsSpace => Amount < MaxStack;
        public float Weight
        {
            get
            {
                if(Item.TryGetComponent(out WeightItemComponent weight))
                {
                    return weight.Value * amount;
                }
                return 0f;
            }
        }

        [SerializeField] private Item item;
        [SerializeField] private ushort amount;
        private readonly int id;

        public Slot(Item item, ushort amount = 0)
        {
            this.item = item;
            this.amount = amount;
            id = UnityEngine.Random.Range(0, int.MaxValue);
        }

        public bool Equals(Slot other)
        {
            if (other.item.ID == item.ID && other.amount == amount && other.id == id) return true;
            return false;
        }

        public ushort Add(ushort value)
        {
            ushort valueToAdd = (ushort)Mathf.Min(value, Remaining);
            amount += valueToAdd;
            return (ushort)(value - valueToAdd);
        }

        public ushort Remove(ushort value)
        {
            ushort valueToRemove = (ushort)Mathf.Min(value, Amount);
            amount -= valueToRemove;
            return (ushort)(value - valueToRemove);
        }

        public static implicit operator uint(Slot slot)
        {
            byte[] recbytes = new byte[4];
            byte[] b1 = BitConverter.GetBytes(slot.Item.ID);
            recbytes[0] = b1[0];
            recbytes[1] = b1[1];
            byte[] b2 = BitConverter.GetBytes(slot.amount);
            recbytes[2] = b2[0];
            recbytes[3] = b2[1];
            uint reconstituted = (uint)BitConverter.ToInt32(recbytes, 0);
            return reconstituted;
        }

        public static (ushort, ushort) SplitInt(uint s)
        {
            byte[] bytes = BitConverter.GetBytes(s);
            ushort item1 = (ushort)BitConverter.ToInt16(bytes, 0);
            ushort item2 = (ushort)BitConverter.ToInt16(bytes, 2);
            return (item1, item2);
        }

    }
}