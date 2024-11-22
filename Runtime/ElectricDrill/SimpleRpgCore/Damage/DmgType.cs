using ElectricDrill.SimpleRpgCore.Damage;
using ElectricDrill.SimpleRpgCore.Stats;
using UnityEngine;
using UnityEngine.Serialization;

namespace ElectricDrill.SimpleRpgCore.Damage
{
    [CreateAssetMenu(fileName = "New Stat", menuName = "Simple RPG/DmgType")]
    public class DmgType : ScriptableObject
    {
        [SerializeField] private Stat dmgReducedBy;
        [FormerlySerializedAs("dmgReductionFn")] [SerializeField] private DmgReductionFn dmgReductionFnFn;
        [SerializeField] private Stat defensiveStatPiercedBy;
        [SerializeField] private DefReductionFn defReductionFn;
        [SerializeField] private bool ignoresBarrier = false;

        public Stat ReducedBy { get => dmgReducedBy; protected set => dmgReducedBy = value; }
        public Stat DefensiveStatPiercedBy => defensiveStatPiercedBy;
        public DefReductionFn DefReductionFn => defReductionFn;
        public DmgReductionFn DmgReductionFn { get => dmgReductionFnFn; protected set => dmgReductionFnFn = value; }
        public bool IgnoresBarrier => ignoresBarrier;

        public override string ToString() {
            return name;
        }
    }
}