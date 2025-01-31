using System;
using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.Penalty
{
    public class PenaltyHandler : MonoBehaviour
    {
        public event Action PenaltyEvent;
        public bool ActivePenalty { get; set; } = true;
        
        public void TriggerPenalty()
        {
            if (!ActivePenalty) return;
            
            PenaltyEvent?.Invoke();
        }
    }
}
