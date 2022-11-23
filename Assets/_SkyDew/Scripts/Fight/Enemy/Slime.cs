using System;
using System.Collections.Generic;
using System.Linq;
using _SkyDew.Scripts.System;
using AppBootstrap.Runtime;
using DG.Tweening;
using DrawerTools;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace _SkyDew.Scripts.Fight.Enemy
{
    public class Slime : MonoBehaviour, IDamageReceiver
    {
        public event Action<Slime, int> OnDamageTaken;

        [SerializeField] private SpriteRenderer view;

        public int hp;
        public float moveSpeed;
        public AttackConfig _attackConfig;

        private Tween _damageTween;

        public void TakeDamage(int damage)
        {
            OnDamageTaken?.Invoke(this, damage);
        }

        public string Fraction => "Enemy";
        
        
        public void DoTakeDamage()
        {
            _damageTween?.Kill(true);
            _damageTween = view.DOColor(Color.red, .2f)
                .SetLoops(2, LoopType.Yoyo)
                .SetTarget(this);
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }
    }

    public interface INpcSpawner
    {
        void Spawn(Vector2 position);
    }

    [Injectable]
    public class SlimeSpawner : INpcSpawner
    {
        [Inject] private IPlayerViewProvider _playerService;
        [Inject] private IAttackService _attackService;
        [Inject] private Slime _slimePrefab;

        private Transform _playerTr;
        private List<SlimeNode> _slimes = new();

        [Init(InitSteps.Postload)]
        private void Init()
        {
            _playerTr = _playerService.View.transform;
            Observable.EveryUpdate().Where(_ => Input.GetKeyUp(KeyCode.Keypad1)).Subscribe(_ =>
            {
               var pos = (Vector2)_playerTr.position + Random.insideUnitCircle.normalized * 4;
               Spawn(pos);
            });
        }

        public void Spawn(Vector2 position)
        {
            var slime = Object.Instantiate(_slimePrefab, position, quaternion.identity);
            slime.OnDamageTaken += AtDamageTaken;
            var node = new SlimeNode() {slime = slime};
            node.currentAction = GetFollowAction(node);
            _slimes.Add(node);
        }

        private void AtDamageTaken(Slime slime, int damage)
        {
            slime.hp -= damage;

            if (slime.hp > 0)
            {
                slime.DoTakeDamage();
                Debug.Log($"Slime: O_o! ({slime.hp })");
                return;
            }

            Debug.Log($"Slime: X_x");
            slime.OnDamageTaken -= AtDamageTaken;
            var node = _slimes.First(x => x.slime == slime);
            node.currentAction.Dispose();
            _slimes.Remove(node);
            Object.Destroy(slime.gameObject);
        }


        private FollowAction GetFollowAction(SlimeNode node)
        {
            var result = new FollowAction();
            result.target = _playerTr;
            result.source = node.slime.transform;
            result.treshold = 2;
            result.moveSpeed = 2f;
            result.completeAction = () =>
            {
                node.currentAction?.Dispose();
                node.currentAction = GetAttackAction(node);
            };
            result.Run();
            return result;
        }

        private AttackAction GetAttackAction(SlimeNode node)
        {
            var result = new AttackAction();
            result.timeToAttack = 1f;
            var tr = node.slime.transform;
            result.completeAction = () =>
            {
                var attackInfo = node.slime._attackConfig.GetAttackInfo();
                attackInfo.sender = node.slime;
                _attackService.Attack(attackInfo, tr.position, _playerTr.position);
                node.currentAction?.Dispose();
                node.currentAction = GetFollowAction(node);
            };
            result.Run();
            return result;
        }


        private class SlimeNode
        {
            public Slime slime;
            public INpcAction currentAction;
        }
    }


    public interface INpcAction : IDisposable
    {
        
    }

    public class FollowAction : INpcAction
    {
        public Transform target;
        public Transform source;
        public float treshold;
        public float moveSpeed;
        public Action completeAction;
        public CompositeDisposable _disposable = new();

        public void Run()
        {
            Observable.EveryUpdate().Subscribe(OnUpdate).AddTo(_disposable);
        }

        private void OnUpdate( long _)
        {
            var diff = target.position - source.transform.position;
            if (Vector3.SqrMagnitude(diff) < treshold)
            {
                completeAction?.Invoke();
                return;
            }

            var dir = diff.normalized;
            dir *= moveSpeed * Time.deltaTime;
            source.transform.position += dir;
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }

    public class AttackAction : INpcAction
    {
        public float timeToAttack;
        public Action completeAction;
        public CompositeDisposable _disposable = new();

        public void Run()
        {
            Observable.EveryUpdate().Subscribe(OnUpdate).AddTo(_disposable);
        }


        private void OnUpdate( long _)
        {
            if (timeToAttack < 0)
            {
                completeAction?.Invoke();
                return;
            }

            timeToAttack -= Time.deltaTime;
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}