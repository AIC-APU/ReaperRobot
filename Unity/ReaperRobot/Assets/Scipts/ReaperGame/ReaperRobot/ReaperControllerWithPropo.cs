﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace smart3tene.Reaper
{
    //プロポがInputSystem(新型)に対応していないため、InputMangaer(旧型)で実装
    //GamePadとの干渉を避けるため、プロポ使用時は他のコントローラを接続しないでください
    //(キーボードでの入力と併用することはできます)

    public class ReaperControllerWithPropo : MonoBehaviour
    {
        #region private Fields
        private ReaperManager _reaperManager;
        private bool _isUsingPropo = false;
        private bool _isNoVerticalLast = false;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _reaperManager = InstanceHolder.Instance.ReaperInstance.GetComponent<ReaperManager>();

            foreach (string controller in Input.GetJoystickNames())
            {
                Debug.Log(controller);
            }

        }

        void Update()
        {
            if (Input.GetJoystickNames() == null)
            {
                return;
            }

            var controllerList = new List<string>();
            controllerList.AddRange(Input.GetJoystickNames());

            //プロポを使っているかどうかの判断
            //ジョイスティックの接続・切断した時のコールバックとかあれば本当はそこで処理をしたい
            //見つけられなかったのでUpdateでやってる
            if (Input.GetJoystickNames().Length > 0 && controllerList.Contains("WSC-1"))
            {
                _isUsingPropo = true;
            }
            else
            {
                _isUsingPropo = false;
            }
        }

        private void FixedUpdate()
        {
            if (!_isUsingPropo) return;

            //移動の処理
            var horizontal = Input.GetAxis("WSC-1_CH1");
            var vertical = Input.GetAxis("WSC-1_CH2");
            var speed = Mathf.Abs(Input.GetAxis("WSC-1_CH4"));

            //実機の挙動に倣い、直進しないなら回転もしないようにしている
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

            _reaperManager.Move(horizontal * speed, vertical * speed);
        }

        private void OnDisable()
        {
            _isUsingPropo = false;
        }
        #endregion
    }
}