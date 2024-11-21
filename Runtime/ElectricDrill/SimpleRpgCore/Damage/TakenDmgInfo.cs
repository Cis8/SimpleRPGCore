using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Damage {
    public struct TakenDmgInfo
    {
        public long Amount { get; }
        public DmgType Type { get; }
        public DmgSource Source { get; }
        public EntityCore Dealer { get; }
        
        public EntityCore Target { get; }
        public bool IsCritical { get; }

        public static DmgInfoAmount Builder => TakenDmgInfoStepBuilder.Builder;

        public TakenDmgInfo(long amount, PreDmgInfo preDmgInfo, EntityCore target) {
            Amount = amount;
            Type = preDmgInfo.Type;
            Source = preDmgInfo.Source;
            Dealer = preDmgInfo.Dealer;
            Target = target;
            IsCritical = preDmgInfo.IsCritical;
        }
        
        private TakenDmgInfo(long amount, DmgType type, DmgSource source, EntityCore dealer, EntityCore target, bool isCritical = false) {
            Amount = amount;
            Type = type;
            Source = source;
            Dealer = dealer;
            Target = target;
            IsCritical = isCritical;
        }

        public interface DmgInfoAmount
        {
            DmgInfoType WithAmount(long amount);
        }
        
         public interface DmgInfoType
        {
            DmgInfoSource WithType(DmgType type);
        }
        
         public interface DmgInfoSource
        {
            DmgInfoDealer WithSource(DmgSource source);
        }
        
         public interface DmgInfoDealer
        {
            DmgInfoTarget WithDealer(EntityCore dealer);
        }
         
        public interface DmgInfoTarget
        {
            TakenDmgInfoStepBuilder WithTarget(EntityCore target);
        }

        public sealed class TakenDmgInfoStepBuilder : DmgInfoAmount, DmgInfoType, DmgInfoSource, DmgInfoDealer, DmgInfoTarget
        {
            private long amount;
            private DmgType type;
            private DmgSource source;
            private EntityCore dealer;
            private EntityCore target;
            private bool isCritical;
            
            public static DmgInfoAmount Builder => new TakenDmgInfoStepBuilder();
            
            public DmgInfoType WithAmount(long amount)
            {
                this.amount = amount;
                return this;
            }

            public DmgInfoSource WithType(DmgType type)
            {
                this.type = type;
                return this;
            }

            public DmgInfoDealer WithSource(DmgSource source)
            {
                this.source = source;
                return this;
            }

            public DmgInfoTarget WithDealer(EntityCore dealer)
            {
                this.dealer = dealer;
                return this;
            }
            
            public TakenDmgInfoStepBuilder WithTarget(EntityCore target)
            {
                this.target = target;
                return this;
            }

            public TakenDmgInfoStepBuilder WithIsCritical(bool isCritical)
            {
                this.isCritical = isCritical;
                return this;
            }

            public TakenDmgInfo Build()
            {
                return new TakenDmgInfo(amount, type, source, dealer, target, isCritical);
            }

        }
    }
}
