using System;
using UnityEngine;
using UnityEngine.Events;

namespace ExpressoBits.Inventories
{
    /// <summary>
    /// Handles items by picking up and adding to the default container, dropping items from containers, or switching items between containers
    /// </summary>
    [RequireComponent(typeof(Container))]
    public class ItemHandler : MonoBehaviour
    {

        public Container DefaultContainer => defaultInventoryContainer;

        [SerializeField]
        private Container defaultInventoryContainer;

        public delegate void ItemObjectEvent(ItemObject itemObject);
        public ItemObjectEvent OnDrop;
        public ItemObjectEvent OnPick;
        public Container.ItemEvent OnAdd;

        public Action<Container> OnOpen;
        public Action<Container> OnClose;

        public UnityEvent<ItemObject> OnDropUnityEvent;
        public UnityEvent<ItemObject> OnPickUnityEvent;
        public UnityEvent<Item,ushort> OnAddUnityEvent;
        public UnityEvent<Container> OnOpenUnityEvent;
        public UnityEvent<Container> OnCloseUnityEvent;

        public void Drop(Item item, ushort amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                if (item.TryGetComponent(out ObjectDropItemComponent dropItemComponent))
                {
                    ItemObject itemObjectPrefab = dropItemComponent.itemObjectPrefab;
                    ItemObject itemObject = Instantiate(itemObjectPrefab, transform.position, transform.rotation);
                    OnDrop?.Invoke(itemObject);
                    OnDropUnityEvent?.Invoke(itemObject);
                }
            }
        }

        public ushort AddToContainer(Container container, Item item, ushort amount = 1, bool dropNotAddedValues = false)
        {
            ushort valueNoAdd = container.AddItem(item, amount);
            OnAdd?.Invoke(item, (ushort)(amount - valueNoAdd));
            OnAddUnityEvent?.Invoke(item, (ushort)(amount - valueNoAdd));
            if (dropNotAddedValues)
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
            if (AddToContainer(container, item) == 0)
            {
                OnPick?.Invoke(itemObject);
                OnPickUnityEvent?.Invoke(itemObject);
                Destroy(itemObject.gameObject);
            }
        }

        /// <summary>
        /// Exchanges a quantity of an item between containers.
        /// First remove from the "from" container, then the successfully removed value is added to the "to" container,
        ///  if any value is not successfully added, this value is added again to the "from" container at the same index,
        ///  if in this last task values are not added with successes they will be dropped
        /// </summary>
        /// <param name="from">Container origin of the item to be exchanged</param>
        /// <param name="index">Index of slot of Container from </param>
        /// <param name="amount">Amount of item exchanged of container from</param>
        /// <param name="to">Destination container of the item to be exchanged</param>
        public void SwapBetweenContainers(Container from, int index, ushort amount, Container to)
        {
            Slot slot = from[index];
            Item item = slot.Item;
            ushort amountNotRemoved = from.RemoveItemAt(index, amount);
            ushort amountForSwap = (ushort)(amount - amountNotRemoved);
            ushort amountNotSwaped = to.AddItem(item, amountForSwap);
            // TODO Implement Undo Tasks?
            ushort amountNotUndo = from.AddItemAt(item, index, amountNotSwaped);
            Drop(item, amountNotUndo);
        }

        #region Container Interactions
        public bool OpenDefaultContainer()
        {
            if(Open(defaultInventoryContainer))
            {
                return true;
            }
            return false;
        }

        public bool Open(Container container)
        {
            if (container.IsOpen) return false;
            OnOpen?.Invoke(container);
            OnOpenUnityEvent?.Invoke(container);
            container.Open();
            return true;
        }

        public bool Close(Container container)
        {
            if (!container.IsOpen) return false;
            OnClose?.Invoke(container);
            OnCloseUnityEvent?.Invoke(container);
            container.Close();
            return true;
        }
        #endregion

    }
}

