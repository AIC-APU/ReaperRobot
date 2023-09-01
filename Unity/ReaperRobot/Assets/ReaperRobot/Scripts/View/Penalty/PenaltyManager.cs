using System;
using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.Penalty
{
    public class PenaltyManager : MonoBehaviour
    {
        public static PenaltyManager Instance { get; private set;}
        public event Action PenaltyEvent;
        public void TriggerPenalty() => PenaltyEvent?.Invoke();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("PenaltyManager is already exist.");
                Destroy(this);
            }
        }
    }
}
