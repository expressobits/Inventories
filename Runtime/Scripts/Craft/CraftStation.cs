using UnityEngine;

namespace ExpressoBits.Inventories
{
    [CreateAssetMenu(fileName = "Craft Station", menuName = "Expresso Bits/Inventories/Craft Station")]
    public class CraftStation : ScriptableObject
    {
        // TODO Implement auto craft (like campfire)
        public Sprite Icon => icon;
        [SerializeField] private Sprite icon;
    }
}

