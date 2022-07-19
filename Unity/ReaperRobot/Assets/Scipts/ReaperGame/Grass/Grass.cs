using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using System;
using Photon.Pun;

namespace smart3tene.Reaper
{
    public class Grass : MonoBehaviourPun
    {
        //これをアタッチしたGameObjectに,Grassの形態を表す子オブジェクトを順に設定してください
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

        #region private Fields
        public IReadOnlyReactiveProperty<bool> IsCut => _isCut;
        private ReactiveProperty<bool> _isCut = new(false);

        readonly float _finishCutTime = 0.5f;
        private float _cutTime = 0;
        private int _nowStep = 0;
        private int _maxStep;
        private GameObject _cutEffectInstance;
        private ParticleSystem _particleSystem;
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

                ReshapeGrass();

                //草が切れるエフェクトを出す
                if (_cutEffectInstance == null)
                {
                    _cutEffectInstance = Instantiate(_cutEffectPrefab, transform.position, Quaternion.identity);
                    _particleSystem = _cutEffectInstance.GetComponent<ParticleSystem>();
                }
                else if(_particleSystem.isStopped)
                {
                    _particleSystem.Play(true);
                }

                //cutが完了したかの判定
                if (_nowStep == _maxStep)
                {
                    //カット完了
                    _isCut.Value = true;

                    //CutGrassに加算
                    if (PhotonNetwork.IsConnected)
                    {
                        photonView.RPC(nameof(AddCutGrass), RpcTarget.All);
                    }
                    else
                    {
                        AddCutGrass();
                    }

                    //パーティクルの停止と破棄
                    _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    _ = DelayAsync(3f, () => Destroy(_particleSystem));
                    _ = DelayAsync(3f, () => Destroy(_cutEffectInstance));
                }   
            }
            else
            {
                if (_particleSystem != null)
                {
                    //パーティクルを停止
                    _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_isCut.Value) return;

            if (other.CompareTag("Cutting"))
            {
                if(_particleSystem != null)
                {
                    //パーティクルを停止
                    _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
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
                if (PhotonNetwork.IsConnected)
                {
                    photonView.RPC(nameof(MinusCutGrass), RpcTarget.All);
                }
                else
                {
                    MinusCutGrass();
                }
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

            Destroy(_particleSystem);
            Destroy(_cutEffectInstance);
        }

        private async UniTask DelayAsync(float seconds, UnityAction callback)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(seconds));
            callback?.Invoke();
        }

        [PunRPC]
        private void AddCutGrass()
        {
            GameSystem.Instance.AddCutGrassCount(1);
        }

        [PunRPC]
        private void MinusCutGrass()
        {
            GameSystem.Instance.AddCutGrassCount(-1);
        }
        #endregion
    }

}
