namespace ElectricDrill.SimpleRpgCore.Events
{
    public interface IRaisable<T>
    {
        void Raise(T context);
    }
    
    public interface IRaisable<T, U>
    {
        void Raise(T context1, U context2);
    }
    
    public interface IRaisable<T, U, V>
    {
        void Raise(T context1, U context2, V context3);
    }
    
    public interface IRaisable<T, U, V, W>
    {
        void Raise(T context1, U context2, V context3, W context4);
    }
}