using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AppBootstrap.Runtime
{
    public class ServiceLocator
    {
        private readonly Dictionary<string, object> _services = new Dictionary<string, object>();
        
        public static ServiceLocator Current { get; private set; }

        public static bool IsInited { get; set; }

        public static Action OnInited;

        public static void Initiailze()
        {
            Current = new ServiceLocator();
        }

        public void Register<T>(T service)
        {
            var key = typeof(T).FullName;
            if (_services.ContainsKey(key))
            {
                Debug.LogError($"Attempt to register service [{key}]. Already registered in {GetType().Name}.");
                return;
            }
            _services.Add(key, service);
        }
        
        public void Register(object service)
        {
            var key = service.GetType().FullName;
            if (_services.ContainsKey(key))
            {
                Debug.LogError($"Attempt to register service [{key}]. Already registered in {GetType().Name}.");
                return;
            }
            _services.Add(key, service);
        }

        public void Unregister<T>(object service)
        {
            var key = service.GetType().FullName;
            if (!_services.ContainsKey(key))
            {
                Debug.LogError($"Attempt to register service [{key}]. Already registered in {GetType().Name}.");
                return;
            }
            _services.Remove(key);
        }

        public bool TryGet<T>(out T result)
        {
            result = Get<T>();
            return result != null;
        }
        
        /// <summary>
        /// Try get Injectable at string Type
        /// </summary>
        /// <param name="key">is Type.FullName</param>
        public bool TryGet(string key, out object result)
        {
            result = Get(key);
            return result != null;
        }

        /// <summary>
        /// Get Injectable at string Type
        /// </summary>
        /// <param name="key">Type.FullName</param>
        public object Get(string key)
        {
            if (_services.ContainsKey(key))
                return _services[key];

            Debug.LogError($"{key} not registered with {GetType().Name}");
            throw new InvalidOperationException();
        }

        public T Get<T>() => (T)Get(typeof(T).FullName);

        public object Get(Type t) => Get(t.FullName);

        public IEnumerable<object> GetAll() 
            => _services.Select(x => x.Value);
        
        public IEnumerable<T> GetAll<T>() 
            => _services.Select(x => (T)x.Value);

        public IEnumerable<T> FindAllByType<T>(Type type) 
            => _services.Where(x => x.Value.GetType().IsAssignableFrom(type))
                .Select(x => (T)x.Value);
        
        public IEnumerable<T> FindAllByType<T>() 
            => _services.Where(x => x.Value is T).Select(x => (T)x.Value);
    }
}