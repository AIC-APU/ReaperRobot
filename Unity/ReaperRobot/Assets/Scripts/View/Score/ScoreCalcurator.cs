using UnityEngine;
using Plusplus.ReaperRobot.Scripts.View.Penalty;

namespace Plusplus.ReaperRobot.Scripts.View.Score
{
    public class ScoreCalcurator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ElapsedTimer.ElapsedTimer _timer;
        [SerializeField] private PenaltyCounter _penaltyCounter;

        [Header("Penalty Rate")]
        [SerializeField] private int _penaltyRate = 10;

        [Header("Evaluation")]
        [SerializeField] private int _sRank = 30;
        [SerializeField] private int _aRank = 60;
        [SerializeField] private int _bRank = 90;
        [SerializeField] private int _cRank = 120;

        public int CalcScore()
        {
            var sec = (int)_timer.Time;
            var penalty = _penaltyCounter.PenaltyCount * _penaltyRate;

            return sec + penalty;
        }

        public string CalcRank()
        {
            var score = CalcScore();
            var rank = "None";

            if (score <= _sRank)
            {
                rank = "S";
            }
            else if (score <= _aRank)
            {
                rank = "A";
            }
            else if (score <= _bRank)
            {
                rank = "B";
            }
            else if (score <= _cRank)
            {
                rank = "C";
            }
            else
            {
                rank = "D";
            }

            Debug.Log($"Score: {score}, Rank: {rank}");
            return rank;
        }
    }
}
