using ElectricDrill.SimpleRpgCore.Health;

namespace ElectricDrill.SimpleRpgCore.Health
{
    public interface IDamageable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="preDmg"></param>
        public void TakeDamage(PreDmgInfo preDmg);
    }
}