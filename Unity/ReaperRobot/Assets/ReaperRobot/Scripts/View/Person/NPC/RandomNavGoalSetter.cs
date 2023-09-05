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
        #endregion

        #region Private Fields
        private NavMeshAgent _agent;
        private Vector3 _goalPos;
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
        #endregion

        #region Private method
        private Vector3 GetRandomPos(IReadOnlyList<Transform> transforms)
        {
            var pos = new Vector3();

            if(transforms.Count > 1)
            {
                var randomIndex = Random.Range(0, transforms.Count - 1);
                pos = transforms[randomIndex].position;
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
