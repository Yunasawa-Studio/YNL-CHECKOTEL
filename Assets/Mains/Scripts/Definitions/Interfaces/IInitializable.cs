namespace YNL.Checkotel
{
    public interface IInitializable
    {
        void Initialize();
    }

    public interface IInitializable<T>
    {
        void Initialize(T input);
    }

    public interface IInitializable<TIn, TOut>
    {
        TOut Initialize(TIn input);
    }
}