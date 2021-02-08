#if USE_ZENJECT

using Zenject;

namespace Framework.ZenjectExtension
{
    public class FrameworkInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            //App router
            Container.BindInterfacesAndSelfTo<ZenjectAppRouter>().AsSingle().Lazy();
            //TODO: ui root
        }
    }
}

#endif