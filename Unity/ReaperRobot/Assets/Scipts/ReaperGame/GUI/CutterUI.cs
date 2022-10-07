using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;


namespace smart3tene.Reaper 
{
    public class CutterUI : MonoBehaviour
    {
        [SerializeField] private ReaperManager _reaperManager;
        [SerializeField] private Image _cutterLamp;

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            if (_reaperManager == null)
            {
                _reaperManager = InstanceHolder.Instance.ReaperInstance.GetComponent<ReaperManager>();
            }

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
            }).AddTo(this);
        }
        #endregion

        #region Public Method for Button
        public void OnClickRotate()
        {
            _reaperManager.RotateCutter(true);
        }

        public void OnClickStop()
        {
            _reaperManager.RotateCutter(false);
        }
        #endregion
    }

}