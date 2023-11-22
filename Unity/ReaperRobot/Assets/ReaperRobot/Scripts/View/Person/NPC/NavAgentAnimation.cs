using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.ComponentModel;

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
        private Quaternion _previousRotation;
        private bool _isPlayingCollisionAnimation = false;

        void Awake()
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();

            _defaultSpeed = _agent.speed;
            _defaultAngularSpeed = _agent.angularSpeed;
            _previousRotation = transform.rotation;
        }

        void Update()
        {
            //移動アニメーションの計算
            var speed = new Vector2(_agent.velocity.x, _agent.velocity.z).magnitude;
            var deltaAngle = Mathf.DeltaAngle(_previousRotation.eulerAngles.y, transform.rotation.eulerAngles.y);
            var angleSpeed = Mathf.Abs(deltaAngle / Time.deltaTime);
            _previousRotation = transform.rotation;

            _animator.SetFloat("Speed", speed * _speedAnimationRate + angleSpeed * _angularAnimationRate, 0.05f, Time.deltaTime);

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

        async void OnCollisionEnter(Collision collision)
        {
            //衝突アニメーション中は無視
            if (_isPlayingCollisionAnimation) return;

            if (collision.gameObject == _collisionTarget
                || IsChildOfTarget(collision.gameObject, _collisionTarget))
            {
                //衝突対象に衝突したら一時停止
                _agent.speed = 0;
                _agent.angularSpeed = 0;

                //現在の向きを保存
                var rotationBeforeCollision = transform.rotation;

                //衝突対象の方を向く
                var direction = collision.transform.position - transform.position;
                var lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = lookRotation;

                //衝突アニメーション
                //フラグを立てる
                _isPlayingCollisionAnimation = true;

                //衝突対象の速度に応じてアニメーションを変化させる
                var speed = new Vector2(collision.relativeVelocity.x, collision.relativeVelocity.z).magnitude;
                float waitTime;
                if (speed > 0.1f)
                {
                    _animator.SetTrigger("DangerousCollision");
                    waitTime = 5.0f; //アニメーションの長さ(s)
                }
                else
                {
                    _animator.SetTrigger("Collision");
                    waitTime = 1.2f; //アニメーションの長さ(s)
                }
                await UniTask.Delay(TimeSpan.FromSeconds(waitTime));

                //衝突アニメーションが終わったら向きを戻す
                await RotateTarget(gameObject, rotationBeforeCollision);

                //停止を解除
                _agent.speed = _defaultSpeed;
                _agent.angularSpeed = _defaultAngularSpeed;

                //フラグを戻す
                _isPlayingCollisionAnimation = false;
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

        private async UniTask RotateTarget(GameObject obj, Quaternion targetRotation)
        {
            var rotationSpeed = 5f;
            var angleThreshold = 5f;
            while (Quaternion.Angle(obj.transform.rotation, targetRotation) > angleThreshold)
            {
                obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                await UniTask.Yield();
            }
        }
    }
}
