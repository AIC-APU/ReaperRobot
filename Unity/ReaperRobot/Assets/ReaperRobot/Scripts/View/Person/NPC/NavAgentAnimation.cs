using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;

namespace Plusplus.ReaperRobot.Scripts.View.Person
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavAgentAnimation : MonoBehaviour
    {
        [SerializeField] private GameObject _collisionTarget;
        [SerializeField] private float _speedRate = 0.8f;

        private Animator _animator;
        private NavMeshAgent _agent;

        void Awake()
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            //移動アニメーション
            var speed = new Vector2(_agent.velocity.x, _agent.velocity.z).magnitude;
            _animator.SetFloat("Speed", speed * _speedRate, 0.05f, Time.deltaTime);


            //目標地点に到達したら待機アニメーション(未実装)
            // if (_agent.hasPath && _agent.remainingDistance < _agent.stoppingDistance)
            // {
            //     _animator.SetBool("hoge_motion", true);
            // }
            // else
            // {
            //     _animator.SetBool("hoge_motion", false);
            // }
        }

        async void OnCollisionEnter(Collision collision)
        {
            if(_animator.GetAnimatorTransitionInfo(0).IsName("Base Layer -> Collision")) return;
            
            //衝突対象に衝突したら一時停止
            if (collision.gameObject == _collisionTarget 
                || IsChildOfTarget(collision.gameObject, _collisionTarget))
            {
                _agent.speed = 0;
                _animator.SetTrigger("Collision");

                await UniTask.Delay(1500);

                _agent.speed = 1;
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
