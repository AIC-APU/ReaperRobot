using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace smart3tene.Reaper
{
    public class Grass : MonoBehaviour
    {
        //これをアタッチしたGameObjectに,Grassの形態を表す子オブジェクトを順に設定してください
        //子オブジェクトの数は2つ以上であれば大丈夫です

        //例:
        // GrassObject(Grassクラスをアタッチ)
        //    L Step0(default)
        //    L Step1
        //    L Step2(カットが終了した状態)
        //(この時の_maxStep は 2 になります)


        #region private Fields
        public IReadOnlyReactiveProperty<bool> IsCut => _isCut;
        private ReactiveProperty<bool> _isCut = new(false);

        readonly float _finishCutTime = 0.5f;
        private float _cutTime = 0;
        private int _nowStep = 0;
        private int _maxStep;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _maxStep = transform.childCount - 1;

            //Step0のみをactiveに
            transform.GetChild(0).gameObject.SetActive(true);
            for(int i = 1; i <= _maxStep; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }

            GameSystem.Instance.ResetEvent += ResetGrass;
        }

        private void OnTriggerStay(Collider other)
        {
            if (_isCut.Value) return;

            if (other.CompareTag("Cutting"))
            {
                _cutTime += Time.deltaTime;

                //ここで草が切れる音やエフェクトを出す

                ReshapeGrass();

                if (_nowStep == _maxStep)
                {
                    _isCut.Value = true;
                    GameSystem.Instance.AddCutGrassCount(1);
                }
            }
        }

        private void OnDestroy()
        {
            GameSystem.Instance.ResetEvent -= ResetGrass;
        }
        #endregion

        #region private Method
        private void ReshapeGrass()
        {
            if (_nowStep >= _maxStep) return;

            //cutTimeに応じてStepを変化させます
            //finish cut timeにちょうど最後のStepに変化します
            if(_cutTime > _finishCutTime * (1 + _nowStep) / _maxStep) 
            {
                transform.GetChild(_nowStep).gameObject.SetActive(false);
                _nowStep++;
                transform.GetChild(_nowStep).gameObject.SetActive(true);
            }
        }

        private void ResetGrass()
        {
            //切られていたらcutGrassCountをもとに戻す
            if (_isCut.Value)
            {
                GameSystem.Instance.AddCutGrassCount(-1);
            }

            //パラメータと形状を初期化
            _isCut.Value = false;
            _cutTime = 0;
            _nowStep = 0;

            //Step0のみをactiveに
            transform.GetChild(0).gameObject.SetActive(true);
            for (int i = 1; i <= _maxStep; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        #endregion
    }

}
