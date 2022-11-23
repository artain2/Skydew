using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _SkyDew.Scripts.Fight
{
    public class FightEffect : MonoBehaviour
    {

        [SerializeField] private Collider2D collider;
        [SerializeField] private float lifetime=1f;

        public Collider2D Collider => collider;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(lifetime);
            Destroy(gameObject);
        }

        [Button]
        public void CheckOverlap()
        {
            var hits = new List<Collider2D>();
            Physics2D.OverlapCollider(collider, new ContactFilter2D(), hits);
            foreach (var hit in hits)
            {
                Debug.Log(hit.gameObject.name);
            }
        }
    }
}