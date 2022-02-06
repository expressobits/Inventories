using System;
using UnityEngine;

namespace ExpressoBits.Inventories
{
    public class HotBar : MonoBehaviour
    {   
        [SerializeField] private Container container;

        private int selectionIndex = 0;

        public Action OnChangeSelection;
        public int SelectionIndex => selectionIndex;
        public Container Container => container;
        public bool IsAnItemSelected => container.Count > selectionIndex;
        public Item SelectedItem => IsAnItemSelected ? container[selectionIndex].Item : null;

        public void ChangeSelection(int index)
        {
            if(index < 0 || index >= Container.Count) return;
            selectionIndex = index;
            OnChangeSelection?.Invoke();
        }

        public void ScrollUp()
        {
            int nextIndex = selectionIndex + 1;
            if(nextIndex >= Container.Count) nextIndex = 0;
            ChangeSelection(nextIndex);
        }

        public void ScrollDown()
        {
            int nextIndex = selectionIndex - 1;
            if(nextIndex < 0) nextIndex = Container.Count - 1;
            ChangeSelection(nextIndex);
        }
    }
}

