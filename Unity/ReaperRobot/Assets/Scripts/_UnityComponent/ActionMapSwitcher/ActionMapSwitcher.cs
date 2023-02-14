using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using ReaperRobot.Scripts.UnityComponent.Camera;
using ReaperRobot.Scripts.UnityComponent.Person;
using ReaperRobot.Scripts.UnityComponent.ReaperRobot;

namespace ReaperRobot.Scripts.UnityComponent.ActionMapSwitcher
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(CameraController))]
    public class ActionMapSwitcher : MonoBehaviour
    {
        #region Private Fields
        private PlayerInput _playerInput;
        private InputActionMap _reaperMap;
        private InputActionMap _personMap;
        private CameraController _cameraCon;
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            _playerInput = GetComponent<PlayerInput>();
            _reaperMap = _playerInput.actions.FindActionMap("Reaper");
            _personMap = _playerInput.actions.FindActionMap("Person");

            _cameraCon = GetComponent<CameraController>();

            _cameraCon
                .ActiveCamera
                .Subscribe(x =>
                {
                    if (x.Target.GetComponent<ReaperManager>())
                    {
                        _reaperMap.Enable();
                        _personMap.Disable();
                    }
                    else if (x.Target.GetComponent<PersonManager>())
                    {
                        _reaperMap.Disable();
                        _personMap.Enable();
                    }
                    else
                    {
                        _reaperMap.Disable();
                        _personMap.Disable();

                        Debug.LogWarning("TargetにReaperManagerもPersonManagerもありません。");
                    }
                })
                .AddTo(this);
        }
        #endregion
    }
}