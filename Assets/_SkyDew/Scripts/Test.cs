using Sirenix.OdinInspector;
using UnityEngine;

namespace _SkyDew.Scripts
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float time = 1f;

        [Button]
        void Move()
        {
            HandyTweens.DoJump(transform, target.position, time);
        }
    }
}