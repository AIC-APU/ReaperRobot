using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
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
        }

        async void OnCollisionEnter(Collision collision)
        {
            if(_animator.GetAnimatorTransitionInfo(0).IsName("Base Layer -> Collision")) return;
            
            if (collision.gameObject == _collisionTarget 
                || IsChildOfTarget(collision.gameObject, _collisionTarget))
            {
                _agent.speed = 0;
                _animator.SetTrigger("Collision");

                await UniTask.Delay(1000);

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
