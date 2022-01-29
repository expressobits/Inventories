using System;
using System.Collections.Generic;
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
        public UnityEvent<Item, ushort> OnAddUnityEvent;
        public UnityEvent<Container> OnOpenUnityEvent;
        public UnityEvent<Container> OnCloseUnityEvent;

        private List<Container> openedContainers = new List<Container>();

        /// <summary>
        /// Container default of itemHandler
        /// </summary>
        public Container DefaultContainer => defaultInventoryContainer;

        /// <summary>
        /// Drop a quantity of the item with ObjectDropItemComponent
        /// </summary>
        /// <param name="item">Item with ObjectDropItemComponent</param>
        /// <param name="amount">Amount of item</param>
        /// <returns>Return true if drop with success</returns>
        public bool Drop(Item item, ushort amount = 1)
        {
            if (item.TryGetComponent(out ObjectDropItemComponent dropItemComponent))
            {
                ItemObject itemObjectPrefab = dropItemComponent.itemObjectPrefab;
                for (int i = 0; i < amount; i++)
                {
                    ItemObject itemObject = Instantiate(itemObjectPrefab, transform.position, transform.rotation);
                    OnDrop?.Invoke(itemObject);
                    OnDropUnityEvent?.Invoke(itemObject);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add an amount of an item to a container, if this addition fails and the dropNotAddedValues is true then a drop of the unadded items occurs
        /// </summary>
        /// <param name="container">Container to add items</param>
        /// <param name="item">Type of item to be added</param>
        /// <param name="amount">Amount of item to be added</param>
        /// <param name="dropNotAddedValues">Drop items not added successfully</param>
        /// <returns>dropNotAddedValues is false returns amount not added, if dropNotAddedValues is true returns 0</returns>
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

        /// <summary>
        /// Drops a amount of items from a container
        /// </summary>
        /// <param name="container">Container to drop itens</param>
        /// <param name="index">Item slot Index</param>
        /// <param name="amount">Amount of item to drop</param>
        public void DropFromContainer(Container container, int index, ushort amount = 1)
        {
            if (container.Count <= index) return;
            Slot slot = container[index];
            Item item = slot.Item;
            ushort amountNotRemoved = container.RemoveItemAt(index, amount);
            ushort amountForDrop = (ushort)(amount - amountNotRemoved);
            Drop(item, amountForDrop);
        }

        /// <summary>
        /// Pick a itemObject to container
        /// </summary>
        /// <param name="container">Container to add item</param>
        /// <param name="itemObject">ItemObject to pick</param>
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
        public void MoveBetweenContainers(Container from, int index, ushort amount, Container to)
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

        /// <summary>
        /// Swap slot information from different or non-containers
        /// </summary>
        /// <param name="container"></param>
        /// <param name="index"></param>
        /// <param name="otherContainer"></param>
        /// <param name="otherIndex"></param>
        public void SwapBetweenContainers(Container container, int index, Container otherContainer, int otherIndex)
        {
            Slot slot = container[index];
            Slot otherSlot = otherContainer[otherIndex];
            container[index] = otherSlot;
            otherContainer[otherIndex] = slot;
        }

        public void SwapBetweenContainers(Container container, int index, Container otherContainer, int otherIndex, ushort amount)
        {
            Slot slot = container[index];
            Slot otherSlot = otherContainer[otherIndex];
            if (otherSlot.IsEmpty || slot.ItemID == otherSlot.ItemID)
            {
                Item item = slot.Item;
                ushort forTrade = otherSlot.IsEmpty ? amount : (ushort)Mathf.Min(otherSlot.Remaining, amount);
                ushort noRemove = container.RemoveItemAt(index, forTrade);
                otherContainer.AddItemAt(item, otherIndex, (ushort)(forTrade - noRemove));
            }
            else
            {   
                if(slot.Amount == amount)
                {
                    container[index] = otherSlot;
                    otherContainer[otherIndex] = slot;
                }
            }

        }

        #region Container Interactions
        /// <summary>
        /// Open Default Container
        /// </summary>
        /// <returns>Return true if container opened with success</returns>
        public bool OpenDefaultContainer()
        {
            return Open(defaultInventoryContainer);
        }

        /// <summary>
        /// Close Default Container
        /// </summary>
        /// <returns>Return true if container closed with success</returns>
        public bool CloseDefaultContainer()
        {
            return Close(defaultInventoryContainer);
        }

        /// <summary>
        /// Open a container
        /// </summary>
        /// <param name="container">Container to be opened, container cannot be opened</param>
        /// <returns>Return true if container opened with success</returns>
        public bool Open(Container container)
        {
            if (openedContainers.Contains(container)) return false;
            openedContainers.Add(container);
            if (container.IsOpen) return false;
            OnOpen?.Invoke(container);
            OnOpenUnityEvent?.Invoke(container);
            container.Open();
            return true;
        }

        /// <summary>
        /// Close a container
        /// </summary>
        /// <param name="container">Container to be closed, container cannot be closed</param>
        /// <returns>Return true if container closed with success</returns>
        public bool Close(Container container)
        {
            if (!openedContainers.Contains(container)) return false;
            openedContainers.Remove(container);
            if (!container.IsOpen) return false;
            OnClose?.Invoke(container);
            OnCloseUnityEvent?.Invoke(container);
            container.Close();
            return true;
        }

        public void CloseAllContainers()
        {
            for (int i = 0; i < openedContainers.Count; i++)
            {
                Container container = openedContainers[i];
                Close(container);
                i--;
            }
        }
        #endregion

    }
}

