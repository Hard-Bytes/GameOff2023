using Project.Code.Patterns.Events;
using Project.Code.Patterns.Services;

namespace Project.Code.Core.Installers.Singles
{
    public class EventDispatcherInstaller : InstallerBehaviour
    {
        public override void InstallDependency(ServiceLocator serviceLocator)
        {
            var eventDispatcher = new EventDispatcherImpl();
            serviceLocator.RegisterService<EventDispatcher>(eventDispatcher);
        }
    }
}