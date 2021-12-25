using UnityEngine;

namespace ExpressoBits.Inventories
{
    public class WeightItemComponent : ItemComponent
    {
        public float Value => weight;
        [SerializeField, Min(0.01f)]
        private float weight;
    }
}