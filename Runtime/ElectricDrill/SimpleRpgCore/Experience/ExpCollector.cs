using System;
using ElectricDrill.SimpleRpgCore.Health;
using ElectricDrill.SimpleRpgCore.Events;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore
{
    [RequireComponent(typeof(EntityDiedGameEventListener))]
    public class ExpCollector : MonoBehaviour
    {
        private EntityCore _entityCore;

        private void Start() {
            _entityCore = GetComponentInParent<EntityCore>();
        }

        public void CheckCollectKillExp(EntityHealth entityHealth, TakenDmgInfo takenDmgInfo) {
            // return if the entity that performed the kill is not the entity that this script is attached to
            if (takenDmgInfo.Dealer != _entityCore) {
                return;
            }
            
            // check if the entity that died has an exp source
            if (entityHealth.TryGetComponent<IExpSource>(out var expSource)) {
                CollectExp(expSource);
            }
        }
        
        public void CollectExp(IExpSource expSource) {
            _entityCore.Level.AddExp(expSource.Exp);
        }
    }
}