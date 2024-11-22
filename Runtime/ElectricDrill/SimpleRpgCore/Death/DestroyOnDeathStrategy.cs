using UnityEngine;

namespace ElectricDrill.SimpleRpgCore
{
    [CreateAssetMenu(fileName = "Destroy On Death Strategy", menuName = "Simple RPG/Death strategies/Destroy")]
    public class DestroyOnDeathStrategy : OnDeathStrategy
    {
        public override void Die(EntityHealth entityHealth)
        {
            Destroy(entityHealth.gameObject);
        }
    }
}