using UniRx;
using UnityEngine;
using Plusplus.ReaperRobot.Scripts.View.ReaperRobot;

namespace Plusplus.ReaperRobot.Scripts.View.OculusIntegration
{
    public class OVRReaperController : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private ReaperManager _reaperRobot;

        [Header("Button Settings")]
        [SerializeField] private OVRInput.Axis2D _forwardBackStick = OVRInput.Axis2D.PrimaryThumbstick;
        [SerializeField] private OVRInput.Axis2D _turnStick = OVRInput.Axis2D.SecondaryThumbstick;
        [SerializeField] private OVRInput.Axis2D _moveStick = OVRInput.Axis2D.PrimaryThumbstick;
        [SerializeField] private OVRInput.Button _liftButton = OVRInput.Button.PrimaryIndexTrigger;
        [SerializeField] private OVRInput.Button _cutterButton = OVRInput.Button.SecondaryIndexTrigger;
        [SerializeField] private OVRInput.Button _brakeButton = OVRInput.Button.Two;
        

        [Header("Axis Settings")]
        [SerializeField] private bool _transmitterMode = true;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            //リフトの昇降
            this.ObserveEveryValueChanged(x => OVRInput.GetDown(_liftButton))
                .Where(x => x)
                .Subscribe(_ => _reaperRobot.MoveLift(!_reaperRobot.IsLiftDown.Value))
                .AddTo(this);
            
            //カッターを回転・停止
            this.ObserveEveryValueChanged(x => OVRInput.GetDown(_cutterButton))
                .Where(x => x)
                .Subscribe(_ => _reaperRobot.RotateCutter(!_reaperRobot.IsCutting.Value))
                .AddTo(this);
            
            //ブレーキをかける
            this.ObserveEveryValueChanged(x => OVRInput.GetDown(_brakeButton))
                .Where(x => x)
                .Subscribe(_ => _reaperRobot.PutOnBrake())
                .AddTo(this);

            //ブレーキを解除する
            this.ObserveEveryValueChanged(x => OVRInput.GetUp(_brakeButton))
                .Where(x => x)
                .Subscribe(_ => _reaperRobot.ReleaseBrake())
                .AddTo(this);

            //移動の処理(transmitterMode=true)
            this.ObserveEveryValueChanged(x => OVRInput.Get(_forwardBackStick))
                .Where(_ => _transmitterMode)
                .Subscribe(_ => _reaperRobot.Move(OVRInput.Get(_turnStick).x, OVRInput.Get(_forwardBackStick).y))
                .AddTo(this);
            
            //移動の処理(transmitterMode=false)
            this.ObserveEveryValueChanged(x => OVRInput.Get(_moveStick))
                .Where(_ => !_transmitterMode)
                .Subscribe(x => _reaperRobot.Move(x.x, x.y))
                .AddTo(this);
        }
        #endregion
    }
}
