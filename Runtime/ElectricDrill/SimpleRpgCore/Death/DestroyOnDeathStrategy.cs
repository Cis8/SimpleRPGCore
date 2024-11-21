using UnityEngine;

namespace ElectricDrill.SimpleRpgCore
{
    [CreateAssetMenu(fileName = "Destroy On Death Strategy", menuName = "Simple RPG/Death strategies/Destroy")]
    public class DestroyOnDeathStrategy : OnDeathStrategy
    {
        public override void Die(EntityHealth entityHealth)
        {
            Debug.Log("Destroying " + entityHealth.gameObject.name);
            Destroy(entityHealth.gameObject);
        }
    }
}