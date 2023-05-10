using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using Plusplus.ReaperRobot.Scripts.View.Camera;
using Plusplus.ReaperRobot.Scripts.View.Person;
using Plusplus.ReaperRobot.Scripts.View.ReaperRobot;

namespace Plusplus.ReaperRobot.Scripts.View.ActionMapSwitcher
{
    [RequireComponent(typeof(PlayerInput))]
    public class ActionMapSwitcher : MonoBehaviour
    {
        #region Private Fields
        private PlayerInput _playerInput;
        private InputActionMap _reaperMap;
        private InputActionMap _personMap;
        [SerializeField] private CameraManager _cameraManager;
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            _playerInput = GetComponent<PlayerInput>();
            _reaperMap = _playerInput.actions.FindActionMap("Reaper");
            _personMap = _playerInput.actions.FindActionMap("Person");

            _cameraManager
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