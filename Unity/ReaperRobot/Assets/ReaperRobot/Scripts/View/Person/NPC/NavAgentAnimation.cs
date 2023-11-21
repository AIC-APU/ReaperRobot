using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Plusplus.ReaperRobot.Scripts.View.Person
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavAgentAnimation : MonoBehaviour
    {
        [SerializeField] private GameObject _collisionTarget;
        [SerializeField] private float _speedAnimationRate = 0.8f;
        [SerializeField] private float _angularAnimationRate = 0.001f;

        private Animator _animator;
        private NavMeshAgent _agent;
        private float _defaultSpeed;
        private float _defaultAngularSpeed;
        private float _waitTime = 0.0f;

        private CancellationTokenSource _cts = new();

        private Quaternion _lastRotation;
        private bool _isPlayingAnimation = false;

        void Awake()
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();

            _defaultSpeed = _agent.speed;
            _defaultAngularSpeed = _agent.angularSpeed;
        }

        void OnDestroy()
        {
            _cts?.Cancel();
        }

        void Update()
        {
            //移動アニメーション
            var speed = new Vector2(_agent.velocity.x, _agent.velocity.z).magnitude;
            var angureSpeed = _agent.angularSpeed;
            _animator.SetFloat("Speed", speed * _speedAnimationRate + angureSpeed * _angularAnimationRate, 0.05f, Time.deltaTime);


            //目標地点に到達したら待機アニメーション
            if (_agent.remainingDistance < _agent.stoppingDistance)
            {
                _animator.SetBool("TreeCare", true);
            }
            else
            {
                _animator.SetBool("TreeCare", false);
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            //衝突アニメーション中は無視
            if (_isPlayingAnimation) return;

            if (collision.gameObject == _collisionTarget
                || IsChildOfTarget(collision.gameObject, _collisionTarget))
            {
                //衝突対象に衝突したら一時停止
                _cts?.Cancel();
                _agent.speed = 0;
                _agent.angularSpeed = 0;

                //現在の向きを保存
                _lastRotation = transform.rotation;

                //衝突対象の方を向く
                var direction = collision.transform.position - transform.position;
                var lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = lookRotation;

                //衝突アニメーション
                //衝突対象の速度に応じてアニメーションを変化させる
                _isPlayingAnimation = true;
                var speed = new Vector2(collision.relativeVelocity.x, collision.relativeVelocity.z).magnitude;
                if (speed > 0.1f)
                {
                    _animator.SetTrigger("DangerousCollision");
                    _waitTime = 5.0f; //アニメーションの長さ(s)
                }
                else
                {
                    _animator.SetTrigger("Collision");
                    _waitTime = 1.2f; //アニメーションの長さ(s)
                }
            }
        }

        void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject == _collisionTarget
                || IsChildOfTarget(collision.gameObject, _collisionTarget))
            {
                _agent.speed = 0;
                _agent.angularSpeed = 0;
            }
        }

        async void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject == _collisionTarget
                || IsChildOfTarget(collision.gameObject, _collisionTarget))
            {
                _cts = new CancellationTokenSource();

                await SetAgentSpeed(_agent, _defaultSpeed, _defaultAngularSpeed, _waitTime, _cts.Token);

                _isPlayingAnimation = false;
            }
        }

        private bool IsChildOfTarget(GameObject child, GameObject parent)
        {
            if (child.transform.parent == null)
            {
                return false;
            }
            else if (child.transform.parent.gameObject == parent)
            {
                return true;
            }
            else
            {
                return IsChildOfTarget(child.transform.parent.gameObject, parent);
            }
        }

        private async UniTask SetAgentSpeed(NavMeshAgent agent, float speed, float angularSpeed, float waitTime = 0f, CancellationToken token = default)
        {
            //衝突アニメーションが終わるまで待機
            if (waitTime > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(waitTime), false, PlayerLoopTiming.Update, token);
            }

            if (token.IsCancellationRequested) return;

            //衝突アニメーションが終わったら移動を再開
            agent.speed = speed;
            agent.angularSpeed = angularSpeed;
        }
    }
}
