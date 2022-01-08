using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExpressoBits.Inventories
{
    public class HandObject : MonoBehaviour
    {
        [SerializeField] private GameObject defaultGameObject;
        [SerializeField] private HotBar hotBar;
        private Dictionary<Item, GameObject> handObjects = new Dictionary<Item, GameObject>();
        private Item lastItem;

        private void OnEnable()
        {
            hotBar.OnChangeSelection += ChangeSelection;
            //hotBar.Container.OnChanged += ChangeSelection;
            defaultGameObject.SetActive(false);
            ChangeSelection();
        }

        private void OnDisable()
        {
            hotBar.OnChangeSelection -= ChangeSelection;
            //hotBar.Container.OnChanged -= ChangeSelection;
        }

        private void ChangeSelection()
        {
            ClearLastSelection();
            if (hotBar.IsAnItemSelected)
            {
                Item item = hotBar.SelectedItem;
                lastItem = item;
                if (!item) return;
                if (!item.TryGetComponent(out HandObjectItemComponent handObjectItemComponent))
                {
                    defaultGameObject.SetActive(true);
                }
                else if (handObjects.TryGetValue(item, out GameObject handObject))
                {
                    handObject.SetActive(true);
                }
                else
                {
                    GameObject prefab = handObjectItemComponent.handPrefab;
                    GameObject newHandObject = Instantiate(prefab, transform);
                    handObjects.Add(item, newHandObject);
                }
            }

        }

        private void ClearLastSelection()
        {
            defaultGameObject.SetActive(false);
            if (!lastItem) return;
            if (handObjects.TryGetValue(lastItem, out GameObject handObject))
            {
                handObject.SetActive(false);
            }
        }

    }
}

