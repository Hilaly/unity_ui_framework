#if USE_ZENJECT

using System.Collections.Generic;
using Framework.Flow;
using Zenject;

namespace Framework.ZenjectExtension
{
    class ZenjectAppRouter : AppRouter
    {
        [Inject]
        private void InjectControllers(IEnumerable<BaseController> controllers)
        {
            foreach (var controller in controllers) 
                RegisterController(controller);
        }
    }
}

#endif