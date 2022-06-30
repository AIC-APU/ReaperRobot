using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace smart3tene.Reaper
{
    //�v���|��InputSystem(�V�^)�ɑΉ����Ă��Ȃ����߁AInputMangaer(���^)�Ŏ���
    //GamePad�Ƃ̊�������邽�߁A�v���|�g�p���͑��̃R���g���[����ڑ����Ȃ��ł�������
    //(�L�[�{�[�h�ł̓��͂ƕ��p���邱�Ƃ͂ł��܂�)

    public class ReaperControllerWithPropo : MonoBehaviour
    {
        #region private Fields
        [SerializeField, Tooltip("�}���`�v���C�̎���null�ɂ��Ă����Ă�������")] private ReaperManager _reaperManager;
        private bool _isUsingPropo = false;
        private bool _isNoVerticalLast = false;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            if (_reaperManager == null)
            {
                _reaperManager = GameSystem.Instance.ReaperInstance.GetComponent<ReaperManager>();
            }
        }

        void Update()
        {
            //�v���|���g���Ă��邩�ǂ����̔��f
            //�W���C�X�e�B�b�N�̐ڑ��E�ؒf�������̃R�[���o�b�N�Ƃ�����Ζ{���͂����ŏ�����������
            //�������Ȃ������̂�Update�ł���Ă�
            if (Input.GetJoystickNames()[0] == "WSC-1")
            {
                _isUsingPropo = true;
            }
            else
            {
                _isUsingPropo = false;
            }

            if (!_isUsingPropo) return;
        }

        private void FixedUpdate()
        {
            if (!_isUsingPropo) return;

            //�ړ��̏���
            var horizontal = Input.GetAxis("WSC-1_CH1");
            var vertical = Input.GetAxis("WSC-1_CH2");
            var speed = Mathf.Abs(Input.GetAxis("WSC-1_CH4"));

            //���@�̋����ɕ킢�A���i���Ȃ��Ȃ��]�����Ȃ��悤�ɂ��Ă���
            if (vertical == 0)
            {   
                if (_isNoVerticalLast)
                {
                    return;
                }
                else
                {
                    _isNoVerticalLast = true;
                    horizontal = 0;
                }
            }
            else
            {
                _isNoVerticalLast = false;
            }

            _ = _reaperManager.AsyncMove(horizontal * speed, vertical * speed);


        }

        private void OnDisable()
        {
            _isUsingPropo = false;
        }
        #endregion
    }
}