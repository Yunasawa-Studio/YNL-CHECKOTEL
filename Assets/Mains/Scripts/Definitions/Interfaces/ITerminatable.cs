namespace YNL.Checkotel
{
    public interface ITerminatable
    {
        void Terminate();
    }

    public interface ITerminatable<T>
    {
        void Terminate(T input);
    }

    public interface ITerminatable<TIn, TOut>
    {
        TOut Terminate(TIn input);
    }
}