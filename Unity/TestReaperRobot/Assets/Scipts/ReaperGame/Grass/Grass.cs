using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace smart3tene.Reaper
{
    public class Grass : MonoBehaviour
    {
        //������A�^�b�`����GameObject��,Grass�̌`�Ԃ�\���q�I�u�W�F�N�g�����ɐݒ肵�Ă�������
        //�q�I�u�W�F�N�g�̐���2�ȏ�ł���Α��v�ł�

        //��:
        // GrassObject(Grass�N���X���A�^�b�`)
        //    L Step0(default)
        //    L Step1
        //    L Step2(�J�b�g���I���������)
        //(���̎���_maxStep �� 2 �ɂȂ�܂�)


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

            //Step0�݂̂�active��
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

                //�����ő����؂�鉹��G�t�F�N�g���o��

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

            //cutTime�ɉ�����Step��ω������܂�
            //finish cut time�ɂ��傤�ǍŌ��Step�ɕω����܂�
            if(_cutTime > _finishCutTime * (1 + _nowStep) / _maxStep) 
            {
                transform.GetChild(_nowStep).gameObject.SetActive(false);
                _nowStep++;
                transform.GetChild(_nowStep).gameObject.SetActive(true);
            }
        }

        private void ResetGrass()
        {
            //�؂��Ă�����cutGrassCount�����Ƃɖ߂�
            if (_isCut.Value)
            {
                GameSystem.Instance.AddCutGrassCount(-1);
            }

            //�p�����[�^�ƌ`���������
            _isCut.Value = false;
            _cutTime = 0;
            _nowStep = 0;

            //Step0�݂̂�active��
            transform.GetChild(0).gameObject.SetActive(true);
            for (int i = 1; i <= _maxStep; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        #endregion
    }

}
