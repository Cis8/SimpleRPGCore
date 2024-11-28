namespace ElectricDrill.SimpleRpgCore.Health {
    public struct ReceivedHealInfo
    {
        public long Amount { get; }
        public Source Source { get; }
        public EntityCore Healer { get; }

        public EntityCore Target { get; }
        public bool IsCritical { get; }

        public static HealInfoAmount Builder => ReceivedHealInfoStepBuilder.Builder;

        public ReceivedHealInfo(long amount, PreHealInfo preHealInfo, EntityCore target) {
            Amount = amount;
            Source = preHealInfo.Source;
            Healer = preHealInfo.Healer;
            Target = target;
            IsCritical = preHealInfo.IsCritical;
        }

        private ReceivedHealInfo(long amount, Source source, EntityCore healer, EntityCore target, bool isCritical = false) {
            Amount = amount;
            Source = source;
            Healer = healer;
            Target = target;
            IsCritical = isCritical;
        }

        public interface HealInfoAmount
        {
            HealInfoSource WithAmount(long amount);
        }

        public interface HealInfoSource
        {
            HealInfoHealer WithSource(Source source);
        }

        public interface HealInfoHealer
        {
            HealInfoTarget WithHealer(EntityCore healer);
        }

        public interface HealInfoTarget
        {
            ReceivedHealInfoStepBuilder WithTarget(EntityCore target);
        }

        public sealed class ReceivedHealInfoStepBuilder : HealInfoAmount, HealInfoSource, HealInfoHealer, HealInfoTarget
        {
            private long amount;
            private Source source;
            private EntityCore healer;
            private EntityCore target;
            private bool isCritical;

            public static HealInfoAmount Builder => new ReceivedHealInfoStepBuilder();

            public HealInfoSource WithAmount(long amount)
            {
                this.amount = amount;
                return this;
            }

            public HealInfoHealer WithSource(Source source)
            {
                this.source = source;
                return this;
            }

            public HealInfoTarget WithHealer(EntityCore healer)
            {
                this.healer = healer;
                return this;
            }

            public ReceivedHealInfoStepBuilder WithTarget(EntityCore target)
            {
                this.target = target;
                return this;
            }

            public ReceivedHealInfoStepBuilder WithIsCritical(bool isCritical)
            {
                this.isCritical = isCritical;
                return this;
            }

            public ReceivedHealInfo Build()
            {
                return new ReceivedHealInfo(amount, source, healer, target, isCritical);
            }
        }
    }
}