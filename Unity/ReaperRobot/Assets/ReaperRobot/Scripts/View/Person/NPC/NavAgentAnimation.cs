using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
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

        void Awake()
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
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
            if (_animator.GetAnimatorTransitionInfo(0).IsName("Base Layer -> Collision")) return;

            //衝突対象に衝突したら一時停止
            if (collision.gameObject == _collisionTarget
                || IsChildOfTarget(collision.gameObject, _collisionTarget))
            {
                _defaultSpeed = _agent.speed;
                _defaultAngularSpeed = _agent.angularSpeed;

                _agent.speed = 0;
                _agent.angularSpeed = 0;
                _animator.SetTrigger("Collision");
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
                await UniTask.Delay(3000);
                _agent.speed = _defaultSpeed;
                _agent.angularSpeed = _defaultAngularSpeed;
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
    }
}
