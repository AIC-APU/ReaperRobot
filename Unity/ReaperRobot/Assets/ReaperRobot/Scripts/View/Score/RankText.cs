using UnityEngine;
using TMPro;

namespace Plusplus.ReaperRobot.Scripts.View.Score
{
    public class RankText : MonoBehaviour
    {
        [SerializeField] private ScoreCalcurator _calculator;
        [SerializeField] private TMP_Text _text;

        public void UpdateText()
        {
            _text.text = _calculator.CalcRank();
        }
    }
}
