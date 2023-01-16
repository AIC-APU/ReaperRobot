using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace smart3tene
{
    public class AnnotationUI : MonoBehaviour
    {
        #region Serialized Private Fields
        [Header("AnnotationRandomTaker")]
        [SerializeField] private AnnotationRandomTaker _randomTaker;

        [Header("UI")]
        [SerializeField] private TMP_InputField _startField;
        [SerializeField] private TMP_InputField _endField;
        [SerializeField] private TMP_InputField _widthField;
        [SerializeField] private TMP_InputField _heightField;
        [SerializeField] private TMP_InputField _minFovField;
        [SerializeField] private TMP_InputField _maxFovField;
        [SerializeField] private Button _startButton;

        [Header("Range Boxes")]
        [SerializeField] private List<GameObject> _rangeBoxObjects;

        [Header("Light")]
        [SerializeField] private GameObject _pointLight;
        #endregion

        #region Private Fields
        private int _startNum = 0;
        private int _endNum = 1;
        private int _width = 224;
        private int _height = 224;      
        private int _minFov = 30;
        private int _maxFov = 60;

        private List<Transform> _rangeBoxes = new List<Transform>();
        #endregion

        #region Readonly Fields
        readonly int MaxWidth = 1024;
        readonly int MinWidth = 32;
        readonly int MaxHeight = 1024;
        readonly int MinHeight = 32;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _startField.text = _startNum.ToString();
            _endField.text = _endNum.ToString();
            _widthField.text = _width.ToString();
            _heightField.text = _height.ToString();
            _minFovField.text = _minFov.ToString();
            _maxFovField.text = _maxFov.ToString();

            foreach (GameObject rangeBoxObj in _rangeBoxObjects)
            {
                //rangeBox は最終的には見えない状態で使う。
                rangeBoxObj.GetComponent<MeshRenderer>().enabled = false;
                _rangeBoxes.Add(rangeBoxObj.transform);
            }
        }
        #endregion

        #region Public method
        public async void OnClickStart()
        {
            await _randomTaker.RandomTake(_width, _height, _startNum, _endNum, _minFov, _maxFov, _rangeBoxes, _pointLight.transform);
        }

        public void OnEndEditStartNum()
        {
            if (_startField.text == "" || _startField.text == "-")
            {
                _startField.text = _startNum.ToString();
                return;
            }

            //正の数に変換
            var value = Math.Abs(int.Parse(_startField.text));

            //endNumより大きい数にはならない
            value = Math.Min(value, _endNum - 1);

            //反映
            _startField.text = value.ToString();
            _startNum = value;
        }

        public void OnEndEditEndNum()
        {
            if (_endField.text == "" || _endField.text == "-")
            {
                _endField.text = _endNum.ToString();
                return;
            }

            //正の数に変換
            var value = Math.Abs(int.Parse(_endField.text));

            //startNumより小さい数にはならない
            value = Math.Max(value, _startNum + 1);

            //反映
            _endField.text = value.ToString();
            _endNum = value;
        }

        public void OnEndEditWidth()
        {
            if (_widthField.text == "" || _widthField.text == "-")
            {
                _widthField.text = _width.ToString();
                return;
            }

            //正の数に変換
            var value = Math.Abs(int.Parse(_widthField.text));

            //指定範囲内に納める
            value = Math.Clamp(value, MinWidth, MaxWidth);

            //反映
            _widthField.text = value.ToString();
            _width = value;
        }

        public void OnEndEditHeight()
        {
            if (_heightField.text == "" || _heightField.text == "-")
            {
                _heightField.text = _height.ToString();
                return;
            }

            //正の数に変換
            var value = Math.Abs(int.Parse(_heightField.text));

            //指定範囲内に納める
            value = Math.Clamp(value, MinHeight, MaxHeight);

            //反映
            _heightField.text = value.ToString();
            _height = value;
        }

        public void OnEndEditMinFov()
        {
            if (_minFovField.text == "" || _minFovField.text == "-")
            {
                _minFovField.text = _minFov.ToString();
                return;
            }

            //正の数に変換
            var value = Math.Abs(int.Parse(_minFovField.text));

            //指定範囲内に納める
            //Cameraクラスのfovの範囲は0-179まで
            value = Math.Clamp(value, 0, _maxFov);

            //反映
            _minFovField.text = value.ToString();
            _minFov = value;
        }

        public void OnEndEditMaxFov()
        {
            if (_maxFovField.text == "" || _maxFovField.text == "-")
            {
                _maxFovField.text = _maxFov.ToString();
                return;
            }

            //正の数に変換
            var value = Math.Abs(int.Parse(_maxFovField.text));

            //指定範囲内に納める
            //Cameraクラスのfovの範囲は0-179まで
            value = Math.Clamp(value, _minFov, 179);

            //反映
            _maxFovField.text = value.ToString();
            _maxFov = value;
        }
        #endregion　
    }

}