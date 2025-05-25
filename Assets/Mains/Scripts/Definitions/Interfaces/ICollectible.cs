namespace YNL.Checkotel
{
    public interface ICollectible
    {
        void Collect();
    }

    public interface ICollectible<T>
    {
        void Collect(T input);
    }

    public interface ICollectible<TIn, TOut>
    {
        TOut Collect(TIn input);
    }
}