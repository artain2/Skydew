using System;
using UnityEngine;

namespace _SkyDew.Scripts.Common
{
    public class ApplicationQuitObserver : MonoBehaviour
    {
        public event Action OnQuit;
        
        private void OnApplicationQuit()
        {
            OnQuit?.Invoke();
        }
    }
}