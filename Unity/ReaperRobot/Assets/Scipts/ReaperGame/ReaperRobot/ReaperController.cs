using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(PlayerInput))]
    public class ReaperController : MonoBehaviour
    {
        #region private Fields
        [SerializeField, Tooltip("�}���`�v���C�̎���null�ɂ��Ă����Ă�������")] private ReaperManager _reaperManager;
        private InputActionMap _reaperAction;
        #endregion

        #region MonoBehaviour Callbacks

        void Awake()
        {
            if(_reaperManager == null)
            {
                _reaperManager = GameSystem.Instance.ReaperInstance.GetComponent<ReaperManager>();
            }
            _reaperAction = GetComponent<PlayerInput>().actions.FindActionMap("Reaper");
        }

        private void OnEnable()
        {
            _reaperAction["Move"].performed += Move;
            _reaperAction["Move"].canceled += Stop;
            _reaperAction["Brake"].started += Brake;
            _reaperAction["Brake"].canceled += OffBrake;
            _reaperAction["Lift"].started += MoveLift;
            _reaperAction["Cutter"].started += RotateCutter;
            _reaperAction["MoveCamera"].performed += MoveCamera;
            _reaperAction["ResetCamera"].started += ResetCamera;
            _reaperAction["ChangeMode"].started += ChangeMode;
        }

        private void OnDisable()
        {
            _reaperAction["Move"].performed -= Move;
            _reaperAction["Move"].canceled -= Stop;
            _reaperAction["Brake"].started -= Brake;
            _reaperAction["Brake"].canceled -= OffBrake;
            _reaperAction["Lift"].started -= MoveLift;
            _reaperAction["Cutter"].started -= RotateCutter;
            _reaperAction["MoveCamera"].performed -= MoveCamera;
            _reaperAction["ResetCamera"].started -= ResetCamera;
            _reaperAction["ChangeMode"].started -= ChangeMode;
        }

        private void LateUpdate()
        {
            var move = _reaperAction["RotateCamera"].ReadValue<Vector2>();
            var rotateSpeed = 0.5f;
            _reaperManager.RotateCamera(-1 * move.y * rotateSpeed, move.x * rotateSpeed, 0);
        }

        #endregion

        #region private method
        private void Move(InputAction.CallbackContext obj)
        {
            var move = obj.ReadValue<Vector2>();
            _ = _reaperManager.AsyncMove(move.x, move.y);
        }

        private void Stop(InputAction.CallbackContext obj)
        {
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
            var move = obj.ReadValue<Vector2>();
            var cameraSpeed = 0.1f;
            _reaperManager.MoveCamera(move.x * cameraSpeed, move.y * cameraSpeed, 0);
        }
        private void ResetCamera(InputAction.CallbackContext obj)
        {
            _reaperManager.ResetCameraPos();
        }
        private void ChangeMode(InputAction.CallbackContext obj)
        {
            if(GameSystem.Instance != null)
            {
                GameSystem.Instance.ChangeOperationMode();

                if(GameSystem.Instance.NowOperationMode.Value == GameSystem.OperationMode.tpv)
                {
                    GetComponent<PlayerInput>().SwitchCurrentActionMap("Person");
                }
            }
        }

        #endregion
    }

}