using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plusplus.ReaperRobot.Scripts.View.Penalty;
using Plusplus.ReaperRobot.Scripts.View.Score;


namespace Plusplus.ReaperRobot.Scripts.View.Web
{
    public class SendResult : BaseJsonSender
    {
        [SerializeField] private ElapsedTimer.ElapsedTimer _timer;
        [SerializeField] private PenaltyCounter _penaltyCounter;
        [SerializeField] private ScoreCalcurator _scoreCalcurator;

        public void SendScoreData()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            var id = QueryReaderUtility.GetValue("sid");
            if (string.IsNullOrEmpty(id)) return;

            var url = GetScoreRegisterURL();
            var scene = GetNowScene();
            var time = _timer.Time;
            var penalty = _penaltyCounter.PenaltyCount;
            var total = _scoreCalcurator.Score;
            var rank = _scoreCalcurator.Rank;

            var dataSet = new DataSet(id, scene, (int)time, penalty, total, rank);

            StartCoroutine(SendJsonData(url, dataSet.ToJson()));
#endif
        }

        private string GetScoreRegisterURL()
        {
            var uri = new Uri(Application.absoluteURL);
            var host = uri.Host;
            return "https://" + host + "/wp-json/myplugin/v1/user/register_score";
        }

        private string GetNowScene()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            if (scene.name.Contains("Field"))
            {
                return "Field";
            }
            else if (scene.name.Contains("Training"))
            {
                return "Training";
            }
            else if (scene.name.Contains("Slope"))
            {
                return "Slope";
            }
            else
            {
                return "Unknown";
            }
        }
    }
}
