using ElectricDrill.SimpleRpgCore.Attributes;
using ElectricDrill.SimpleRpgCore.Stats;

namespace ElectricDrill.SimpleRpgCore
{
    public interface IEntityCore
    {
        EntityLevel Level { get; }
        EntityStats Stats { get; }
        EntityAttributes Attributes { get; }
        string Name { get; }
    }
}