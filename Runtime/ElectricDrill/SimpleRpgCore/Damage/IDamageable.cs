using ElectricDrill.SimpleRpgCore.Damage;

namespace ElectricDrill.SimpleRpgCore.Damage
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