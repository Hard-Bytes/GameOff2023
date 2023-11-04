using UnityEngine;

namespace Project.Code.Core.Installers.Scenes
{
    /// <summary>
    /// Implementación por defecto, solo hace lo que la clase padre
    /// </summary>
    
    [DefaultExecutionOrder(-1)]
    public class SceneInstallerBehaviourImpl : SceneInstallerBehaviour
    {
        protected override void InstallAdditionalDependencies() { }

        protected override void OnStart() { }

        protected override void OnDestroy() { }
    }
}