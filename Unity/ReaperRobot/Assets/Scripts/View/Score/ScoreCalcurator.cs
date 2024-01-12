using UnityEngine;
using Plusplus.ReaperRobot.Scripts.View.Penalty;

namespace Plusplus.ReaperRobot.Scripts.View.Score
{
    public class ScoreCalcurator : MonoBehaviour
    {
        public int Score {get; private set;}
        public string Rank {get; private set;}

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
            Score = CalcScore();
            Rank = "None";

            if (Score <= _sRank)
            {
                Rank = "S";
            }
            else if (Score <= _aRank)
            {
                Rank = "A";
            }
            else if (Score <= _bRank)
            {
                Rank = "B";
            }
            else if (Score <= _cRank)
            {
                Rank = "C";
            }
            else
            {
                Rank = "D";
            }

            Debug.Log($"Score: {Score}, Rank: {Rank}");
            return Rank;
        }
    }
}
