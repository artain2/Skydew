using System.Collections.Generic;
using System.Linq;
using AppBootstrap.Runtime;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace _SkyDew.Scripts.Common.UI
{
    public interface IUIService
    {
        void AddElement(GameObject gameObject);
        void AddElements(IEnumerable<GameObject> gameObjects);
        T GetElement<T>() where T : MonoBehaviour;
        T GetElement<T>(string id) where T : MonoBehaviour;
        GameObject GetElement(string id);
        List<T> GetElements<T>() where T : MonoBehaviour;
        List<T> GetElements<T>(string id) where T : MonoBehaviour;
        List<GameObject> GetElements(string id);
    }

    [Injectable]
    public class UIService : IUIService
    {
        private List<UIElement> _elements = new List<UIElement>();

        public void AddElement(GameObject gameObject)
        {
            var allElements = GetAllUIElements(gameObject.transform);
            foreach (var element in allElements)
            {
                Register(element);
            }
        }

        public void AddElements(IEnumerable<GameObject> gameObjects)
        {
            foreach (var go in gameObjects)
                AddElement(go);
        }

        public T GetElement<T>() where T : MonoBehaviour
        {
            foreach (var element in _elements)
                if (element.TryGetComponent<T>(out var result))
                    return result;

            return null;
        }

        public T GetElement<T>(string id) where T : MonoBehaviour
        {
            var item = _elements.FirstOrDefault(x => x.id == id);
            return item == null ? null : item.GetComponent<T>();
        }

        public GameObject GetElement(string id)
        {
            var item = _elements.FirstOrDefault(x => x.id == id);
            return item == null ? null : item.gameObject;
        }

        public List<T> GetElements<T>() where T : MonoBehaviour
        {
            var result = new List<T>();
            foreach (var element in _elements)
                if (element.TryGetComponent<T>(out var comp))
                    result.Add(comp);
            return result;
        }

        public List<T> GetElements<T>(string id) where T : MonoBehaviour
        {
            var result = _elements.Where(x => x.id == id)
                .Select(x => x.GetComponent<T>())
                .ToList();
            return result;
        }

        public List<GameObject> GetElements(string id)
        {
            var result = _elements.Where(x => x.id == id)
                .Select(x => x.gameObject)
                .ToList();
            return result;
        }

        private IEnumerable<UIElement> GetAllUIElements(Transform parent)
        {
            return parent.GetComponentsInChildren<UIElement>(true);
        }

        private void Register(UIElement element)
        {
            _elements.Add(element);
            element.OnDestroyAsObservable().Subscribe(_ => Unregister(element.gameObject));
        }

        private void Unregister(GameObject gameObject)
        {
            var allElements = GetAllUIElements(gameObject.transform);
            foreach (var element in allElements)
            {
                _elements.Remove(element);
            }
        }
    }
}