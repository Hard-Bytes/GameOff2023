using UnityEngine;
using Project.Code.Patterns.Services;

namespace Project.Code.Core.Installers
{
    /// <summary>
    /// Clase base para configurar una única dependencia
    /// </summary>
    public abstract class InstallerBehaviour : MonoBehaviour
    {
        public abstract void InstallDependency(ServiceLocator serviceLocator);
    }
}