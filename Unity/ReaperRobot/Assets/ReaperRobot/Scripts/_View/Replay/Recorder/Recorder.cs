using UnityEngine;
using UnityEngine.Events;
using UniRx;
using System;
using Plusplus.ReaperRobot.Scripts.View.ReaperRobot;
using Plusplus.ReaperRobot.Scripts.Data;

namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public class Recorder : MonoBehaviour
    {
        #region Public Properties
        public float Time => _timer;
        #endregion

        #region Serialized Fields
        [SerializeField] private ReaperManager _reaperManager;
        [SerializeField] private Transform _reaperTransform;
        [SerializeField] private string _fileName = "ReaperLog";
        #endregion

        #region Unity Events
        [SerializeField] private UnityEvent OnStartRecording;
        [SerializeField] private UnityEvent OnStopRecording;
        #endregion

        #region Reactive Properties
        public IReadOnlyReactiveProperty<DataSet> NowData => _nowData;
        private ReactiveProperty<DataSet> _nowData = new(null);
        public IReadOnlyReactiveProperty<bool> IsRecording => _isRecording;
        private ReactiveProperty<bool> _isRecording = new(false);
        #endregion

        #region Private Fields
        private string _csvData = DataSet.CSVLabel() + "\n";
        private float _timer = 0f;
        #endregion

        #region MonoBehaviour Callbacks

        void Update()
        {
            if (!_isRecording.Value) return;

            //データの記録
            var time = _timer;
            var inputH = _reaperManager.InputH.Value;
            var inputV = _reaperManager.InputV.Value;
            var lift = _reaperManager.IsLiftDown.Value;
            var cutter = _reaperManager.IsCutting.Value;
            var positionX = _reaperTransform.position.x;
            var positionY = _reaperTransform.position.y;
            var positionZ = _reaperTransform.position.z;
            var angleY = _reaperTransform.eulerAngles.y;

            _nowData.Value = new DataSet(time, inputH, inputV, lift, cutter, positionX, positionY, positionZ, angleY);

            _csvData += _nowData.Value.CSVLine() + "\n";

            _timer += UnityEngine.Time.deltaTime;
        }

        void OnDestroy()
        {
            //記録の途中でゲームが終わったならそこでExportする
            if (_isRecording.Value) ExportCSV(_csvData);
        }
        #endregion

        #region Public method
        public void StartRecording()
        {
            //記録開始
            _isRecording.Value = true;
            OnStartRecording?.Invoke();
        }
        public void StopRecording()
        {
            //記録停止
            _isRecording.Value = false;
            OnStopRecording?.Invoke();

            //CSV書き出し
            ExportCSV(_csvData);

            //初期化
            _nowData.Value = null;
            _csvData = DataSet.CSVLabel() + "\n";
            _timer = 0;
        }
        #endregion

        #region Private method
        private void ExportCSV(string csvData)
        {
            //ファイル名作成
            var now = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var fileName = $"{_fileName}_{now}.csv";

            //ファイル書き出し
            CSVUtility.Write(fileName, csvData);

            Debug.Log($"Exported: {fileName}");
        }
        #endregion
    }
}
