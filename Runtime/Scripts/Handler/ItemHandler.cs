using System;
using UnityEngine;

namespace ExpressoBits.Inventories
{
    [RequireComponent(typeof(Container))]
    public class ItemHandler : MonoBehaviour
    {

        public Container DefaultContainer => defaultInventoryContainer;

        [SerializeField]
        private Container defaultInventoryContainer;

        public delegate void ItemObjectEvent(ItemObject itemObject);
        public ItemObjectEvent OnDrop;
        public ItemObjectEvent OnPick;

        public void Drop(Item item, ushort amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                if (item.TryGetComponent(out ObjectDropItemComponent dropItemComponent))
                {
                    ItemObject itemObjectPrefab = dropItemComponent.itemObjectPrefab;
                    ItemObject itemObject = Instantiate(itemObjectPrefab, transform.position, transform.rotation);
                    OnDrop?.Invoke(itemObject);
                }
            }
        }

        public ushort AddToContainer(Container container, Item item, ushort amount = 1, bool dropNotAddedValues = false)
        {
            ushort valueNoAdd = container.AddItem(item, amount);
            if(dropNotAddedValues)
            {
                Drop(item, valueNoAdd);
                return 0;
            }
            return valueNoAdd;
        }

        public void DropFromContainer(Container container, int index, ushort amount = 1)
        {
            Slot slot = container[index];
            Item item = slot.Item;
            ushort amountNotRemoved = container.RemoveItemAt(index, amount);
            ushort amountForDrop = (ushort)(amount - amountNotRemoved);
            Drop(item, amountForDrop);
        }

        public void PickToContainer(Container container, ItemObject itemObject)
        {
            if (!itemObject.IsPickable) return;
            Item item = itemObject.Item;
            if (AddToContainer(container,item) == 0)
            {
                OnPick?.Invoke(itemObject);
                Destroy(itemObject.gameObject);
            }
        }
    }
}

