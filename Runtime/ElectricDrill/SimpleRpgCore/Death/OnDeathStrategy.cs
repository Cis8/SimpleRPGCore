using UnityEngine;

namespace ElectricDrill.SimpleRpgCore
{
    public abstract class OnDeathStrategy : ScriptableObject
    {
        public abstract void Die(EntityHealth entityHealth);
    }
}