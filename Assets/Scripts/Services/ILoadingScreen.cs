using Cysharp.Threading.Tasks;

namespace Services
{
    public interface ILoadingScreen
    {
        UniTask Load();
    }
}