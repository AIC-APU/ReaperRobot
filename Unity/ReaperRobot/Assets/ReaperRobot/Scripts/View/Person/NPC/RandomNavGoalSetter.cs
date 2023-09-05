using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.View.Person
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class RandomNavGoalSetter : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private List<Transform> _goals = new List<Transform>();
        [SerializeField, Range(1, 30)] private int _interval = 20;
        [SerializeField] private bool _lookGoalWhenStop = true;
        #endregion

        #region Private Fields
        private NavMeshAgent _agent;
        private Vector3 _goalPos;
        private int _currentGoalIndex = 0;
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            _agent = GetComponent<NavMeshAgent>();

            //_interval秒ごとにランダムなゴールを設定する
            Observable
                .Timer(System.TimeSpan.Zero, System.TimeSpan.FromSeconds(_interval))
                .Subscribe(_ =>
                {
                    _goalPos = GetRandomPos(_goals);
                    _agent.SetDestination(_goalPos);
                })
                .AddTo(this);
        }

        void Update()
        {
            if(_lookGoalWhenStop && _agent.hasPath && _agent.remainingDistance < _agent.stoppingDistance)
            {
                //オブジェクトの方を向く
                float rotateSpeed = 8f;
                float step = rotateSpeed * Time.deltaTime;
                var direction = _goalPos - transform.position;
                direction.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, step);
            }
        }
        #endregion

        #region Private method
        private Vector3 GetRandomPos(IReadOnlyList<Transform> transforms)
        {
            var pos = new Vector3();

            if(transforms.Count > 1)
            {
                var randomIndex = Random.Range(0, transforms.Count);
                while (randomIndex == _currentGoalIndex)
                {
                    randomIndex = Random.Range(0, transforms.Count);
                }
                pos = transforms[randomIndex].position;
                _currentGoalIndex = randomIndex;
            }
            else if(transforms.Count == 1)
            {
                pos = transforms[0].position;
            }
            else
            {
                throw new System.NullReferenceException();
            }

            return pos;
        }
        #endregion
    }
}
