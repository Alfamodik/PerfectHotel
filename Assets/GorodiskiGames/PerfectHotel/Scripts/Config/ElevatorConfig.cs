using UnityEngine;

namespace Game.Config
{
    [SerializeField]
    [CreateAssetMenu(menuName = "config/elevatorconfig")]
    public sealed class ElevatorConfig : ScriptableObject
    {
        [Min(1)] public int Area;
        public int PricePurchase;
    }
}
