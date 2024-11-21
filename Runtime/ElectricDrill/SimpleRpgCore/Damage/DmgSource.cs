using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Damage {
    [CreateAssetMenu(fileName = "New Dmg Source", menuName = "Simple RPG/Dmg Source")]
    public class DmgSource : ScriptableObject
    {
        public override string ToString() {
            return name;
        }
    }
}
