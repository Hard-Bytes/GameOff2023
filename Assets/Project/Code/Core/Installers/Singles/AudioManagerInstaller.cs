using Project.Code.Domain.Services;
using Project.Code.Patterns;
using Project.Code.Patterns.Services;
using UnityEngine;

namespace Project.Code.Core.Installers.Singles
{
    public class AudioManagerInstaller : InstallerBehaviour
    {
        [SerializeField] private AudioManager audioManager;
        public override void InstallDependency(ServiceLocator serviceLocator)
        {
            DontDestroyOnLoad(audioManager);
            serviceLocator.RegisterService(audioManager);
        }
    }
}