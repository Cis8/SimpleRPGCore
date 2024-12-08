namespace ElectricDrill.SimpleRpgCore
{
    public interface IExpSource {
        public bool Harvested { get; set; }
        
        long Exp { get; }
    }
}