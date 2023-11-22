using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using Cysharp.Threading.Tasks;

namespace Plusplus.ReaperRobot.Scripts.View.Grass
{
    public class GrassObject : MonoBehaviour
    {
        //これをアタッチしたGameObjectに,Grassの形態を表すオブジェクトを順に子に設定してください
        //子オブジェクトの数は2つ以上であれば大丈夫です

        //例:
        // GrassObject(Grassクラスをアタッチ)
        //    L Step0(default)
        //    L Step1
        //    L Step2(カットが終了した状態)
        //(この時の_maxStep は 2 になります)

        #region Serialized Private Fields
        [SerializeField] private GameObject _cutEffectPrefab;
        #endregion

        #region public Fields
        public IReadOnlyReactiveProperty<bool> IsCut => _isCut;
        #endregion

        #region Private Fields
        private ReactiveProperty<bool> _isCut = new(false);
        private ReactiveProperty<float> _cutTime = new(0);
        private int _nowStep = 0;
        private int _maxStep;
        private GameObject _cutEffectInstance;
        private ParticleSystem _particleSystem;
        #endregion

        #region Readonly Fields
        readonly float _finishCutTime = 1f;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _maxStep = transform.childCount - 1;

            //cutTimeが増えたら草の形状を判定・変更する
            _cutTime
                .Subscribe(x => ReshapeGrass(x))
                .AddTo(this);

            //カットされた時パーティクルを破棄する
            _isCut
                .Skip(1)
                .Where(x => x)
                .Subscribe(_ => DestroyParticle())
                .AddTo(this);
        }

        private void OnTriggerStay(Collider other)
        {
            if (_isCut.Value) return;

            if (other.CompareTag("Cutting"))
            {
                //刃が回転しているReaperRobotが接触した時、cutTimeを増やす
                _cutTime.Value += Time.deltaTime;

                //切られている時にパーティクルを生成・再生する
                if (_cutEffectInstance == null)
                {
                    _cutEffectInstance = Instantiate(_cutEffectPrefab, transform.position, Quaternion.identity);
                    _particleSystem = _cutEffectInstance.GetComponent<ParticleSystem>();
                }
                else if (_particleSystem.isStopped)
                {
                    _particleSystem.Play(true);
                }

                //cutが完了したかの判定
                if (_cutTime.Value >= _finishCutTime)
                {
                    //カット完了
                    _isCut.Value = true;
                }
            }
            else
            {
                //刃が回転していないReaperRobotが接触した時、パーティクルを停止
                if (_particleSystem != null)
                {
                    _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_isCut.Value) return;

            if (other.CompareTag("Cutting") && _particleSystem != null)
            {
                //パーティクルを停止
                _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
        #endregion

        #region Public method
        public void CutThisGrass()
        {
            if (_isCut.Value) return;

            //マルチの同期などどうしてもスクリプトから草を刈ったことにしたい時に使う。
            _cutTime.Value = _finishCutTime;
            _isCut.Value = true;
        }
        #endregion

        #region Private method
        private void ReshapeGrass(float cutTime)
        {
            if (_nowStep >= _maxStep) return;

            //cutTimeに応じてStepを変化させます
            //finish cut timeにちょうど最後のStepに変化します
            for (int i = 0; i < _maxStep; i++)
            {
                if (cutTime >= i * _finishCutTime / _maxStep &&
                    cutTime < (i + 1) * _finishCutTime / _maxStep)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                    _nowStep = i;
                }
                else
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }

            if (cutTime >= _finishCutTime)
            {
                transform.GetChild(_maxStep).gameObject.SetActive(true);
                _nowStep = _maxStep;
            }
            else
            {
                transform.GetChild(_maxStep).gameObject.SetActive(false);
            }
        }

        private void ResetGrass()
        {
            //パラメータと形状を初期化
            _isCut.Value = false;
            _cutTime.Value = 0;
            _nowStep = 0;

            //Step0のみをactiveに
            transform.GetChild(0).gameObject.SetActive(true);
            for (int i = 1; i <= _maxStep; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }

            Destroy(_particleSystem);
            Destroy(_cutEffectInstance);
        }

        private async void DestroyParticle()
        {
            //isCut = true になった後の挙動
            if (_cutEffectInstance != null)
            {
                //パーティクルの停止と破棄
                _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);

                await DelayAsync(0.1f, () => Destroy(_particleSystem));
                _particleSystem = null;

                await DelayAsync(0.1f, () => Destroy(_cutEffectInstance));
                _cutEffectInstance = null;
            }
        }

        private async UniTask DelayAsync(float seconds, UnityAction callback)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(seconds));
            callback?.Invoke();
        }
        #endregion
    }
}