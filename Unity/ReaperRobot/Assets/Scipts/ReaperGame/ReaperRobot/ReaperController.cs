using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(PlayerInput))]
    public class ReaperController : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private GameObject _reaperRobot = null;
        #endregion

        #region private Fields
        private ReaperManager _reaperManager;
        private FPVCameraManager _fpvCameraManager;
        private AroundViewCameraManager _aroundViewCameraManager;
        private BirdViewCameraManager _birdViewCameraManager;
        private IRobotCamera _robotCamera;

        private InputActionMap _reaperAction;

        private bool _isReaperOperatable = true;
        private bool _isCameraOperatable = true;
        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            if(_reaperRobot == null)
            {
                _reaperRobot = GameSystem.Instance.ReaperInstance;
            }

            _reaperManager           = _reaperRobot.GetComponent<ReaperManager>();
            _fpvCameraManager        = _reaperRobot.GetComponent<FPVCameraManager>();
            _birdViewCameraManager   = _reaperRobot.GetComponent<BirdViewCameraManager>();
            _aroundViewCameraManager = _reaperRobot.GetComponent<AroundViewCameraManager>();

            _reaperAction = GetComponent<PlayerInput>().actions.FindActionMap("Reaper");


            _reaperAction["Move"].performed         += Move;
            _reaperAction["Move"].canceled          += Stop;
            _reaperAction["Brake"].started          += Brake;
            _reaperAction["Brake"].canceled         += OffBrake;
            _reaperAction["Lift"].started           += MoveLift;
            _reaperAction["Cutter"].started         += RotateCutter;
            _reaperAction["MoveCamera"].performed   += MoveCamera;
            _reaperAction["ResetCamera"].started    += ResetCamera;
            _reaperAction["ChangeMode"].started     += ChangeViewMode;
            _reaperAction["CloseApp"].started       += CloseApp;
            _reaperAction["Menu"].started           += InvokeMenuEvent;
            _reaperAction["ChangeReaperAndPerson"].started += ChangeReaperAndPerson;


            if (GameSystem.Instance == null) return;
            GameSystem.Instance.NowViewMode.Subscribe(mode =>
            {
                _ = _reaperManager.AsyncMove(0, 0);
                var playerInput = GetComponent<PlayerInput>();

                switch (mode)
                {
                    case GameSystem.ViewMode.REAPER_FPV:
                        if(playerInput.currentActionMap.name != "Reaper") playerInput.SwitchCurrentActionMap("Reaper");
                        _isCameraOperatable = true;
                        _robotCamera = _fpvCameraManager; 
                        break;
                    case GameSystem.ViewMode.REAPER_FromPERSON:
                        if (playerInput.currentActionMap.name != "Reaper") playerInput.SwitchCurrentActionMap("Reaper");
                        _isCameraOperatable = false;
                        break;
                    case GameSystem.ViewMode.REAPER_BIRDVIEW:
                        if (playerInput.currentActionMap.name != "Reaper") playerInput.SwitchCurrentActionMap("Reaper");
                        _isCameraOperatable = true;
                        _robotCamera = _birdViewCameraManager;                        
                        break;
                    case GameSystem.ViewMode.REAPER_AROUND:
                        if (playerInput.currentActionMap.name != "Reaper") playerInput.SwitchCurrentActionMap("Reaper");
                        _isCameraOperatable = true;
                        _robotCamera = _aroundViewCameraManager;
                        break;
                    case GameSystem.ViewMode.REAPER_VR:
                        if (playerInput.currentActionMap.name != "Reaper") playerInput.SwitchCurrentActionMap("Reaper");
                        _isCameraOperatable = false;
                        break;
                    default:
                        _isCameraOperatable = false;
                        break;
                }
            });
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
            _reaperAction["ChangeMode"].started             -= ChangeViewMode;
            _reaperAction["CloseApp"].started               -= CloseApp;
            _reaperAction["Menu"].started                   -= InvokeMenuEvent;
            _reaperAction["ChangeReaperAndPerson"].started  -= ChangeReaperAndPerson;
        }

        private void LateUpdate()
        {
            if (!_isCameraOperatable) return;
            
            _robotCamera.FollowRobot();

            var vec = _reaperAction["RotateCamera"].ReadValue<Vector2>();
            if(vec != Vector2.zero)
            {
                _robotCamera.RotateCamera(vec.x, vec.y);
            }
        }
        #endregion


        #region private method
        private void Move(InputAction.CallbackContext obj)
        {
            if (!_isReaperOperatable) return;
            var move = _reaperAction["Move"].ReadValue<Vector2>();
            _ = _reaperManager.AsyncMove(move.x, move.y);
        }
        private void Stop(InputAction.CallbackContext obj)
        {
            if (!_isReaperOperatable) return;

            //Oculusコントローラでの操作時は以下の停止処理をさせない
            //この分岐がないと、なぜかOculusコントローラでは毎フレームこの停止処理をしてしまう
            if (obj.control.name == "thumbstick") return;

            _ = _reaperManager.AsyncMove(0, 0);
        }
        private void Brake(InputAction.CallbackContext obj)
        {
            if (!_isReaperOperatable) return;
            _reaperManager.PutOnBrake();
        }
        private void OffBrake(InputAction.CallbackContext obj)
        {
            if (!_isReaperOperatable) return;
            _reaperManager.ReleaseBrake();
        }
        private void MoveLift(InputAction.CallbackContext obj)
        {
            if (!_isReaperOperatable) return;
            _reaperManager.MoveLift(!_reaperManager.IsLiftDown.Value);
        }
        private void RotateCutter(InputAction.CallbackContext obj)
        {
            if (!_isReaperOperatable) return;
            _reaperManager.RotateCutter(!_reaperManager.IsCutting.Value);
        }

        private void MoveCamera(InputAction.CallbackContext obj)
        {
            if (!_isCameraOperatable) return;
            var vec = obj.ReadValue<Vector2>();
            _robotCamera.MoveCamera(vec.x, vec.y);
        }
        private void ResetCamera(InputAction.CallbackContext obj)
        {
            if (!_isCameraOperatable) return;
            _robotCamera.ResetCamera();
        }
        private void ChangeViewMode(InputAction.CallbackContext obj)
        {
            if(GameSystem.Instance != null)
            {
                GameSystem.Instance.ChangeViewMode();
            }
        }
        private void ChangeReaperAndPerson(InputAction.CallbackContext obj)
        {
            if (GameSystem.Instance != null)
            {
                GameSystem.Instance.ChangeReaperAndPerson();
            }
        }
        private void CloseApp(InputAction.CallbackContext obj)
        {
            //SceneTransitionManagerがシーンにないとCloseAppできません
            if (SceneTransitionManager.Instantiated)
            {
                SceneTransitionManager.Instance.CloseApp();
            }
        }

        private void InvokeMenuEvent(InputAction.CallbackContext obj)
        {
            if (GameSystem.Instance != null)
            {
                GameSystem.Instance.InvokeMenuEvent();
            }  
        }
        #endregion
    }

}
