using System;
using Project.Code.Patterns;
using Project.Code.Patterns.Services;
using UnityEngine;

namespace Project.Code.Core.Installers
{
    /// <summary>
    /// Clase que ejecuta la instalación de dependencias de varios InstallerBehaviour
    /// </summary>
    public abstract class SceneInstallerBehaviour : MonoBehaviour
    {
        [Header("--- Base Class Configurations ---")]

        [SerializeField] private InstallerBehaviour[] dependencyInstallers;

        private void Awake() => InstallDependencies();

        private void Start() => OnStart();
        
        private void InstallDependencies()
        {
            ServiceLocator serviceLocator = ServiceLocator.Instance;
            
            foreach (var dependencyInstaller in dependencyInstallers)
            {
                dependencyInstaller.InstallDependency(serviceLocator);
            }

            InstallAdditionalDependencies();
        }
        
        protected abstract void InstallAdditionalDependencies();
        protected abstract void OnStart();
        protected abstract void OnDestroy();
    }
}