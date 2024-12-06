using ElectricDrill.SimpleRpgCore.Health;
using ElectricDrill.SimpleRpgCore.Stats;
using UnityEngine;
using UnityEngine.Serialization;

namespace ElectricDrill.SimpleRpgCore.Health
{
    [CreateAssetMenu(fileName = "New Stat", menuName = "Simple RPG/DmgType")]
    public class DmgType : ScriptableObject
    {
        [SerializeField] private Stat dmgReducedBy;
        [SerializeField] private DmgReductionFn dmgReductionFn;
        [SerializeField] private Stat defensiveStatPiercedBy;
        [SerializeField] private DefReductionFn defReductionFn;
        [SerializeField] private bool ignoresBarrier = false;

        public Stat ReducedBy { get => dmgReducedBy; protected set => dmgReducedBy = value; }
        public Stat DefensiveStatPiercedBy => defensiveStatPiercedBy;
        public DefReductionFn DefReductionFn => defReductionFn;
        public DmgReductionFn DmgReductionFn { get => dmgReductionFn; protected set => dmgReductionFn = value; }
        public bool IgnoresBarrier => ignoresBarrier;

        public override string ToString() {
            return name;
        }
    }
}