using UnityEngine;
using UnityEngine.InputSystem;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(PlayerInput))]
    public class ReaperController : MonoBehaviour, ICameraController, IRobotController
    {
        #region Public Fields    
        public GameObject TargetRobot { get => _targetRobot; set => _targetRobot = value; }
        [SerializeField] private GameObject _targetRobot = null;

        public IControllableCamera CCamera 
        {
            get => _controllableCamera; 
            
            set
            {
                _controllableCamera = value;
                _controllableCamera.ResetCamera();
            }  
        }
        private IControllableCamera _controllableCamera;
        #endregion

        #region Serialized Private Fields
        [SerializeField, Tooltip("ここからIControllableCameraを設定することもできます（デバッグ用）")] private GameObject _controllableCameraObject;
        #endregion

        #region private Fields
        private ReaperManager _reaperManager;
        private PlayerInput _playerInput;
        private InputActionMap _reaperAction;
        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            _reaperManager      = _targetRobot.GetComponent<ReaperManager>();
            _playerInput        = GetComponent<PlayerInput>();

            if (_controllableCameraObject != null)
            {
                _controllableCamera = _controllableCameraObject.GetComponent<IControllableCamera>();
            }

            _reaperAction       = _playerInput.actions.FindActionMap("Reaper");

            _reaperAction["Move"].performed         += Move;
            _reaperAction["Move"].canceled          += Stop;
            _reaperAction["Brake"].started          += Brake;
            _reaperAction["Brake"].canceled         += OffBrake;
            _reaperAction["Lift"].started           += MoveLift;
            _reaperAction["Cutter"].started         += RotateCutter;
            _reaperAction["MoveCamera"].performed   += MoveCamera;
            _reaperAction["ResetCamera"].started    += ResetCamera;
        }

        private void OnDisable()
        {
            _reaperAction["Move"].performed                 -= Move;
            _reaperAction["Move"].canceled                  -= Stop;
            _reaperAction["Brake"].started                  -= Brake;
            _reaperAction["Brake"].canceled                 -= OffBrake;
            _reaperAction["Lift"].started                   -= MoveLift;
            _reaperAction["Cutter"].started                 -= RotateCutter;
            _reaperAction["MoveCamera"].performed           -= MoveCamera;
            _reaperAction["ResetCamera"].started            -= ResetCamera;
        }

        private void LateUpdate()
        {
            if (_playerInput.currentActionMap.name != "Reaper") return;
            if (_controllableCamera == null) return;

            _controllableCamera.FollowTarget();

            var vec = _reaperAction["RotateCamera"].ReadValue<Vector2>();
            _controllableCamera.RotateCamera(vec.x, vec.y);
        }
        #endregion


        #region private method
        private void Move(InputAction.CallbackContext obj)
        {
            var move = _reaperAction["Move"].ReadValue<Vector2>();
            _ = _reaperManager.AsyncMove(move.x, move.y);
        }
        private void Stop(InputAction.CallbackContext obj)
        {
            //Oculusコントローラでの操作時は以下の停止処理をさせない
            //この分岐がないと、なぜかOculusコントローラでは毎フレームこの停止処理をしてしまう
            if (obj.control.name == "thumbstick") return;

            _ = _reaperManager.AsyncMove(0, 0);
        }
        private void Brake(InputAction.CallbackContext obj)
        {
            _reaperManager.PutOnBrake();
        }
        private void OffBrake(InputAction.CallbackContext obj)
        {
            _reaperManager.ReleaseBrake();
        }
        private void MoveLift(InputAction.CallbackContext obj)
        {
            _reaperManager.MoveLift(!_reaperManager.IsLiftDown.Value);
        }
        private void RotateCutter(InputAction.CallbackContext obj)
        {
            _reaperManager.RotateCutter(!_reaperManager.IsCutting.Value);
        }

        private void MoveCamera(InputAction.CallbackContext obj)
        {
            var vec = obj.ReadValue<Vector2>();
            _controllableCamera.MoveCamera(vec.x, vec.y);
        }
        private void ResetCamera(InputAction.CallbackContext obj)
        {
            _controllableCamera.ResetCamera();
        }
        
        #endregion
    }

}
