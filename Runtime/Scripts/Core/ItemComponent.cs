using UnityEngine;

namespace ExpressoBits.Inventories
{
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp")]
    [System.Serializable]
    public abstract class ItemComponent
    {
        [HideInInspector]
        public Item item;
    }
}