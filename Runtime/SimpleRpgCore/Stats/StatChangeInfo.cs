using System.Collections;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Stats;
using UnityEngine;

namespace ElectricDrill {
    public struct StatChangeInfo
    {
        public EntityStats EntityStats;
        public Stat Stat;
        public long OldValue;
        public long NewValue;

        public StatChangeInfo(EntityStats entity, Stat stat, long oldValue, long newValue) {
            EntityStats = entity;
            Stat = stat;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
