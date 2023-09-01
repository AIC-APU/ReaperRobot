using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using UniRx.Triggers;

namespace Plusplus.ReaperRobot.Scripts.View.Penalty
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavigationTest : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private List<Transform> _goals = new List<Transform>();
        [SerializeField, Range(1, 30)] private int _interval = 20;

        //for debug

        #endregion

        #region Private Fields
        private NavMeshAgent _agent;
        private Vector3 _nowGoal;
        private ReactiveProperty<bool> _isStop = new(false);
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            _agent = GetComponent<NavMeshAgent>();

            //10秒毎にゴールを変える
            var dueTime = System.TimeSpan.FromSeconds(0);
            var period = System.TimeSpan.FromSeconds(_interval);
            Observable
                .Timer(dueTime, period)
                .Subscribe(_ =>
                {
                    _nowGoal = GetRandomPos(_goals);
                    _agent.SetDestination(_nowGoal);
                })
                .AddTo(this);

        }
        #endregion

        private void Update()
        {
            _agent.isStopped = _agent.remainingDistance < _agent.stoppingDistance;

            //Agentが停止しているならゴールを見るよう回転する
            if (_agent.isStopped)
            {
                //ゆっくり回転
                float speed = 2f;
                float step = speed * Time.deltaTime;
                var direction = _nowGoal - transform.position;
                direction.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, step);
            }
        }

        #region Private method
        private Vector3 GetRandomPos(List<Transform> transforms)
        {
            var pos = new Vector3();

            if(transforms.Count == 0)
            {
                throw new System.NullReferenceException();
            }
            else if(transforms.Count == 1)
            {
                pos = transforms[0].position;
            }
            else
            {
                var randomIndex = UnityEngine.Random.Range(0, transforms.Count - 1);
                pos = transforms[randomIndex].position;
            }

            return pos;
        }
        #endregion
    }

}