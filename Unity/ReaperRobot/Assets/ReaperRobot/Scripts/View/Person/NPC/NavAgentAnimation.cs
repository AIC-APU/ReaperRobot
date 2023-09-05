using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.View.Person
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavAgentAnimation : MonoBehaviour
    {
        private Animator _animator;
        private NavMeshAgent _navMeshAgent;

        void Awake()
        {
            _animator = GetComponent<Animator>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            //移動アニメーション
            var speed = new Vector2(_navMeshAgent.velocity.x, _navMeshAgent.velocity.z).magnitude;
            _animator.SetFloat("Speed", speed, 0.05f, Time.deltaTime);

            //衝突アニメーション
            //衝突フラグかなんかが立った時にアニメーションを再生する
        }
    }
}
