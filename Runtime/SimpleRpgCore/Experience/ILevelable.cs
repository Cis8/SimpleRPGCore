namespace ElectricDrill.SimpleRpgCore
{
    public interface ILevelable
    {
        int Level { get; set; }
        void AddExp(long amount);
        void SetTotalCurrentExp(long totalCurrentExperience);
        long CurrentTotalExperience { get; }
        long CurrentLevelTotalExperience();
        long NextLevelTotalExperience();
        void ValidateExperience();
    }
}