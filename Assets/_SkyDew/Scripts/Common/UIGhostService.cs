// using AppBootstrap.Runtime;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace _SkyDew.Scripts.Common
// {
//     public interface IUIGhostService
//     {
//         UIGhost CreateGhost(Sprite sprite, Transform root = null);
//     }
//
//     [Injectable]
//     public class UIGhostService : IUIGhostService
//     {
//         [Inject] private GameObject _prefab;
//
//         public UIGhost CreateGhost(Sprite sprite, Transform root = null)
//         {
//             var inst = GameObject.Instantiate(_prefab, root);
//             var ghost = inst.GetComponent<UIGhost>();
//             ghost.IconImage.sprite = sprite;
//             return ghost;
//         }
//     }
// }