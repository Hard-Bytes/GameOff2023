using Project.Code.Patterns.Events;
using Project.Code.Patterns.Services;

namespace Project.Code.Core.Installers.Singles
{
    public class EventDispatcherInstaller : InstallerBehaviour
    {
        public override void InstallDependency(ServiceLocator serviceLocator)
        {
            var locator = ServiceLocator.Instance;

            if (locator.HasService<EventDispatcher>()) return;
            
            var eventDispatcher = new EventDispatcherImpl();
            locator.RegisterService<EventDispatcher>(eventDispatcher);
        }
    }
}