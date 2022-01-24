using System;
using UnityEngine;

namespace ExpressoBits.Inventories
{
    public class HotBar : MonoBehaviour
    {   
        [SerializeField] private Container container;
        [SerializeField] private int slotsInHotBar = 8;

        private int selectionIndex = 0;

        public Action OnChangeSelection;
        public int SelectionIndex => selectionIndex;
        public Container Container => container;
        public bool IsAnItemSelected => container.Count > selectionIndex;
        public Item SelectedItem => IsAnItemSelected ? container[selectionIndex].Item : null;

        public void ChangeSelection(int index)
        {
            if(index < 0 || index >= slotsInHotBar) return;
            selectionIndex = index;
            OnChangeSelection?.Invoke();
        }

        public void ScrollUp()
        {
            int nextIndex = selectionIndex + 1;
            if(nextIndex >= slotsInHotBar) nextIndex = 0;
            ChangeSelection(nextIndex);
        }

        public void ScrollDown()
        {
            int nextIndex = selectionIndex - 1;
            if(nextIndex < 0) nextIndex = slotsInHotBar - 1;
            ChangeSelection(nextIndex);
        }
    }
}

