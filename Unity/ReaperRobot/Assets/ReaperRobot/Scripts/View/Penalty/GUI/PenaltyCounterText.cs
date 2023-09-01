using UnityEngine;
using TMPro;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.View.Penalty
{
    public class PenaltyCounterText : MonoBehaviour
    {
        [SerializeField] private PenaltyCounter _penaltyCounter;
        [SerializeField] private TMP_Text _penaltyNumText;

        private void Start()
        {
            _penaltyCounter.PenaltyCount.Subscribe(count => _penaltyNumText.text = count.ToString());
        }
    }
}
