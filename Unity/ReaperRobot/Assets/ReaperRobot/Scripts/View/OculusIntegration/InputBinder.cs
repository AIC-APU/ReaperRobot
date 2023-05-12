using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Oculus;

namespace Plusplus.ReaperRobot.Scripts.View.OculusIntegration
{
    public class InputBinder : MonoBehaviour
    {
        #region Struct
        [System.Serializable]
        private struct ActionBinding
        {
            public enum InputType
            {
                Get,
                GetDown,
                GetUp,
            }

            public InputActionReference inputActionReference;
            public InputType inputType;
            public OVRInput.Button button;
        }
        #endregion

        #region Serialized Private Fields
        [SerializeField] private ActionBinding _bindings;
        #endregion

        #region Private Fields
        private PlayerInput _playerInput;
        private InputActionMap _reaperActionMap;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
        }

        void Update()
        {
            if(OVRInput.Get(OVRInput.RawButton.LHandTrigger))
            {
                
            }
        }
        #endregion

        #region Public method

        #endregion

        #region Private method

        #endregion
    }
}
