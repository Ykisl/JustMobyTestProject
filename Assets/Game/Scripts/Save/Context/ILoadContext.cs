
namespace Game.Save.Context
{
    public interface ILoadContext
    {
        long SaveTimestamp { get; }

        T GetData<T>();
    }
}
