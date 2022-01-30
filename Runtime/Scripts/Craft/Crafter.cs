using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ExpressoBits.Inventories
{
    [AddComponentMenu(menuName: "Expresso Bits/Inventories/Crafter")]
    [RequireComponent(typeof(Container))]
    public class Crafter : MonoBehaviour
    {
        /// <summary>
        /// List of possible recipes to craft
        /// </summary>
        public List<Recipe> Recipes => Database.Recipes;

        public List<CraftStation> NearCraftStations => nearCraftStations;

        private List<Crafting> craftings = new List<Crafting>();
        [SerializeField] private Database database;
        [SerializeField] private Container[] containers;
        [SerializeField] private bool trackNearStations = false;
        [SerializeField] private List<CraftStationObject> nearCraftStationObject = new List<CraftStationObject>();
        [SerializeField] private List<CraftStation> nearCraftStations = new List<CraftStation>();

        /// <summary>
        /// Defines whether or not the crafter can finish a craft that has passed its creation time
        /// </summary>
        [SerializeField] private bool canFinishCraft = true;

        /// <summary>
        /// Is the crafting list limited?
        /// </summary>
        [SerializeField] private bool isLimitCrafts = true;
        /// <summary>
        /// Maximum number of craftings if limit is used
        /// </summary>
        [SerializeField] private uint craftsLimit = 8;

        public Action<Recipe> OnCrafted;
        public Action<Recipe> OnRequestCraft;
        public Action<Crafting> OnAdd;
        public Action<int> OnRemoveAt;
        public Action<int> OnUpdate;

        public Action<CraftStation> OnAddNearCraftStation;
        public Action<CraftStation> OnRemoveNearCraftStation;

        public UnityEvent<int> OnUpdateUnityEvent;

        /// <summary>
        /// Is something currently being crafted?
        /// </summary>
        public bool IsCrafting => craftings.Count > 0f;
        public int CountOfCraftings => craftings.Count;
        public Container[] Containers => containers;
        public Database Database => database;

        private void Update()
        {
            if (IsCrafting)
            {
                for (int i = 0; i < craftings.Count; i++)
                {
                    Crafting crafting = craftings[i];
                    crafting.AddTimeElapsed(Time.deltaTime);
                    if (crafting.IsFinished && canFinishCraft)
                    {
                        Recipe recipe = Recipes[crafting.Index];
                        ushort toAdd = recipe.AmountOfProduct;
                        foreach (var container in containers)
                        {
                            if (toAdd <= 0) continue;
                            toAdd = container.AddItem(recipe.Product, toAdd);
                        }
                        OnCrafted?.Invoke(recipe);
                        RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        craftings[i] = crafting;
                    }
                }
            }
        }

        /// <summary>
        /// Defines whether or not the crafter can finish a craft that has passed its creation time
        /// </summary>
        /// <param name="canFinishCraft">Can finish a craft ?</param>
        public void SetCanFinishCraft(bool canFinishCraft)
        {
            this.canFinishCraft = canFinishCraft;
        }

        /// <summary>
        /// Check if it is possible to create this recipe
        /// It is checked if the crafts limit has been exceeded and then it is checked if the recipe items contain in the container
        /// </summary>
        /// <param name="recipe">Recipe to be checked</param>
        /// <returns>True if the recipe to be crafted is available</returns>
        public bool CanCraft(Recipe recipe)
        {
            if (isLimitCrafts && craftings.Count >= craftsLimit) return false;

            foreach (var needCraftStation in recipe.NeedCraftStations)
            {
                if (!nearCraftStations.Contains(needCraftStation)) return false;
            }

            foreach (var items in recipe.RequiredItems)
            {
                if (!HasItem(items.Item, items.Amount)) return false;
            }
            return true;
        }

        private bool HasItem(Item item, ushort amount)
        {
            foreach (var container in containers)
            {
                ushort value = container.GetAmountOf(item);
                if (value >= amount) return true;
                amount -= value;
            }
            return amount == 0;
        }

        private bool UseItems(Recipe recipe)
        {
            foreach (var items in recipe.RequiredItems)
            {
                ushort amount = items.Amount;
                foreach (var container in containers)
                {
                    amount -= container.RemoveItem(items.Item, items.Amount);
                }
                if (amount > 0) return false;
            }
            return true;
        }

        public Crafting this[int index]
        {
            get { return craftings[index]; }
            set
            {
                craftings[index] = value;
                OnUpdate?.Invoke(index);
                OnUpdateUnityEvent?.Invoke(index);
            }
        }

        public void Add(Crafting crafting)
        {
            craftings.Add(crafting);
            OnAdd?.Invoke(crafting);
        }

        public void RemoveAt(int index)
        {
            if (craftings.Count <= index) return;
            craftings.RemoveAt(index);
            OnRemoveAt?.Invoke(index);
        }

        public void Craft(Recipe recipe)
        {
            if (CanCraft(recipe))
            {
                if (UseItems(recipe))
                {
                    Crafting crafting = new Crafting(Recipes.IndexOf(recipe), recipe.TimeForCraft);
                    Add(crafting);
                    OnRequestCraft?.Invoke(recipe);
                }
            }
        }

        public void CancelCraft(Crafting crafting)
        {
            int index = craftings.IndexOf(crafting);
            CancelCraft(index);
        }

        public int IndexOf(Crafting crafting)
        {
            return craftings.IndexOf(crafting);
        }

        public void CancelCraft(int index)
        {
            if (index < 0) return;
            if (craftings.Count <= index) return;
            Crafting crafting = craftings[index];
            if (crafting.IsFinished) return;
            Recipe recipe = Recipes[crafting.Index];
            foreach (var requiredItem in recipe.RequiredItems)
            {
                ushort amount = requiredItem.Amount;
                foreach (var container in containers)
                {
                    if(amount == 0) continue;
                    amount = container.AddItem(requiredItem.Item,amount);
                }
            }
            RemoveAt(index);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!trackNearStations) return;
            if (!other.TryGetComponent(out CraftStationObject craftStationObject)) return;
            nearCraftStationObject.Add(craftStationObject);
            nearCraftStations.Add(craftStationObject.CraftStation);
            OnAddNearCraftStation?.Invoke(craftStationObject.CraftStation);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!trackNearStations) return;
            if (!other.TryGetComponent(out CraftStationObject craftStationObject)) return;
            if (nearCraftStationObject.Remove(craftStationObject))
            {
                nearCraftStations.Remove(craftStationObject.CraftStation);
                OnRemoveNearCraftStation?.Invoke(craftStationObject.CraftStation);
            }
        }

    }
}

