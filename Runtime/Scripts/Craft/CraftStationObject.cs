using UnityEngine;

namespace ExpressoBits.Inventories
{
    public class CraftStationObject : MonoBehaviour
    {
        public CraftStation CraftStation => station;
        [SerializeField] private CraftStation station;
    }
}

