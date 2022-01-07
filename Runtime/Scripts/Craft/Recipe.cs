using System.Collections.Generic;
using UnityEngine;

namespace ExpressoBits.Inventories
{
    /// <summary>
    /// Craft recipe stores required item information, craft time and craft product
    /// </summary>
    [CreateAssetMenu(fileName = "Recipe", menuName = "Expresso Bits/Inventories/Recipe")]
    public class Recipe : ScriptableObject
    {
        /// <summary>
        /// Item structure list required by craft, these items have required amount information
        /// </summary>
        public List<RequiredItem> RequiredItems => requiredItems;

        /// <summary>
        /// Time for this recipe to be crafted
        /// </summary>
        public float TimeForCraft => timeForCraft;

        /// <summary>
        /// Craft result item
        /// </summary>
        public Item Product => product;

        /// <summary>
        /// Amount of result item
        /// </summary>
        public ushort AmountOfProduct => amountOfProduct;

        [SerializeField] private List<RequiredItem> requiredItems;
        [SerializeField] private Item product;
        [SerializeField] private ushort amountOfProduct = 1;
        [SerializeField] private float timeForCraft = 4f;
    }
}

