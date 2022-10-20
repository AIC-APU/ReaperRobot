using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;


namespace smart3tene.Reaper
{
    public class LiftUI : MonoBehaviour
    {
        [SerializeField] private ReaperManager _reaperManager;
        [SerializeField] private Image _liftLamp;

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            if (_reaperManager == null)
            {
                _reaperManager = InstanceHolder.Instance.ReaperInstance.GetComponent<ReaperManager>();
            }

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
            }).AddTo(this);

        }
        #endregion

        #region Public Method for Button
        public void OnClickDown()
        {
            _reaperManager.MoveLift(true);
        }

        public void OnClickUp()
        {
            _reaperManager.MoveLift(false);
        }
        #endregion
    }

}