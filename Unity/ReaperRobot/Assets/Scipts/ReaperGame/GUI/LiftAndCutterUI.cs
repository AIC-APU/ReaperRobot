using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace smart3tene.Reaper
{
    public class LiftAndCutterUI : MonoBehaviour
    {
        [SerializeField] private Image _liftLamp;
        [SerializeField] private Image _cutterLamp;

        private ReaperManager _reaperManager;

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _reaperManager = ReaperGameSystem.Instance.ReaperInstance.GetComponent<ReaperManager>();

            //Liftのランプ
            _reaperManager.IsLiftDown.Subscribe(isDown =>
            {
                if (isDown)
                {
                    _liftLamp.color = new Color32(255, 90, 0, 255);
                }
                else
                {
                    _liftLamp.color = new Color32(196, 196, 196, 255);
                }
            });


            //Cutterのランプ
            _reaperManager.IsCutting.Subscribe(isCutting =>
            {
                if (isCutting)
                {
                    _cutterLamp.color = new Color32(255, 90, 0, 255);
                }
                else
                {
                    _cutterLamp.color = new Color32(196, 196, 196, 255);
                }
            });
        }
        #endregion

        #region Public Method for Button
        public void DownButtonClick()
        {
            _reaperManager.MoveLift(true);
        }

        public void UpButtonClick()
        {
            _reaperManager.MoveLift(false);
        }

        public void RotateButtonClick()
        {
            _reaperManager.RotateCutter(true);
        }

        public void StopButtonClick()
        {
            _reaperManager.RotateCutter(false);
        }
        #endregion
    }
}