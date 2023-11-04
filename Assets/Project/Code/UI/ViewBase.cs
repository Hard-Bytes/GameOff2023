using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Code.UI
{
    public abstract class ViewBase : MonoBehaviour
    {
        protected List<IDisposable> _disposables = new List<IDisposable>();

        private void OnDestroy()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}