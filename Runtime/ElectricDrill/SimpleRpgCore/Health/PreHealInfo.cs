namespace ElectricDrill.SimpleRpgCore.Health {
    public class PreHealInfo
    {
        public long Amount { get; }
        public Source Source { get; }
        public EntityCore Healer { get; }
        public bool IsCritical { get; }

        public static HealInfoAmount Builder => new PreHealInfoStepBuilder();

        private PreHealInfo(long amount, Source source, EntityCore healer, bool isCritical = false) {
            Amount = amount;
            Source = source;
            Healer = healer;
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
            PreHealInfoStepBuilder WithHealer(EntityCore healer);
        }

        public sealed class PreHealInfoStepBuilder : HealInfoAmount, HealInfoSource, HealInfoHealer
        {
            private long amount;
            private Source source;
            private EntityCore healer;
            private bool isCritical;

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

            public PreHealInfoStepBuilder WithHealer(EntityCore healer)
            {
                this.healer = healer;
                return this;
            }

            public PreHealInfoStepBuilder WithIsCritical(bool isCritical)
            {
                this.isCritical = isCritical;
                return this;
            }

            public PreHealInfo Build()
            {
                return new PreHealInfo(amount, source, healer, isCritical);
            }
        }
    }
}