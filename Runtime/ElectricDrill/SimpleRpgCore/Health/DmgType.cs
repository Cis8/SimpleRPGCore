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
        public Stat DefensiveStatPiercedBy {
            get => defensiveStatPiercedBy;
            protected set => defensiveStatPiercedBy = value;
        }
        public DefReductionFn DefReductionFn { get => defReductionFn; protected set => defReductionFn = value; }
        public DmgReductionFn DmgReductionFn { get => dmgReductionFn; protected set => dmgReductionFn = value; }
        public bool IgnoresBarrier { 
            get =>ignoresBarrier;
            protected set => ignoresBarrier = value;
        }

        public override string ToString() {
            return name;
        }
    }
}