using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace smart3tene.Reaper 
{

    public class RepositionUI : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private ShadowReaperPlayer _shadowPlayer;
        [SerializeField] private RobotReaperPlayer _robotPlayer;

        [SerializeField] private TMP_InputField _posXField;
        [SerializeField] private TMP_InputField _posYField;
        [SerializeField] private TMP_InputField _posZField;
        [SerializeField] private TMP_InputField _angXField;
        [SerializeField] private TMP_InputField _angYField;
        [SerializeField] private TMP_InputField _angZField;
        #endregion

        #region Readonly Fields
        readonly Vector3 _defaultPos = Vector3.zero;
        readonly Vector3 _defaultAng = Vector3.zero;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _posXField.text = _defaultPos.x.ToString();
            _posYField.text = _defaultPos.y.ToString();
            _posZField.text = _defaultPos.z.ToString();
            _angXField.text = _defaultAng.x.ToString();
            _angYField.text = _defaultAng.y.ToString();
            _angZField.text = _defaultAng.z.ToString();

            _shadowPlayer.RepositionPos = _defaultPos;
            _shadowPlayer.RepositionAng = _defaultAng;
            _robotPlayer.RepositionPos = _defaultPos;
            _robotPlayer.RepositionAng = _defaultAng;
        }
        #endregion

        #region Public method
        public void OnEndEditPosX()
        {
            if (_posXField.text == "" || _posXField.text == "-")
            {
                _posXField.text = _shadowPlayer.RepositionPos.x.ToString();
                return;
            }

            var value = float.Parse(_posXField.text);
            _shadowPlayer.RepositionPos.x = value;
            _robotPlayer.RepositionPos.x = value;
            _posXField.text = value.ToString();
        }
        public void OnEndEditPosY()
        {
            if (_posYField.text == "" || _posYField.text == "-")
            {
                _posYField.text = _shadowPlayer.RepositionPos.y.ToString();
                return;
            }

            var value = float.Parse(_posYField.text);
            _shadowPlayer.RepositionPos.y = value;
            _robotPlayer.RepositionPos.y = value;
            _posYField.text = value.ToString();
        }

        public void OnEndEditPosZ()
        {
            if (_posZField.text == "" || _posZField.text == "-")
            {
                _posZField.text = _shadowPlayer.RepositionPos.z.ToString();
                return;
            }

            var value = float.Parse(_posZField.text);
            _shadowPlayer.RepositionPos.z = value;
            _robotPlayer.RepositionPos.z = value;
            _posZField.text = value.ToString();
        }
        public void OnEndEditAngX()
        {
            if (_angXField.text == "" || _angXField.text == "-")
            {
                _angXField.text = _shadowPlayer.RepositionAng.x.ToString();
                return;
            }

            var value = float.Parse(_angXField.text);
            _shadowPlayer.RepositionAng.x = value;
            _robotPlayer.RepositionAng.x = value;
            _angXField.text = value.ToString();
        }
        public void OnEndEditAngY()
        {
            if (_angYField.text == "" || _angYField.text == "-")
            {
                _angYField.text = _shadowPlayer.RepositionAng.y.ToString();
                return;
            }

            var value = float.Parse(_angYField.text);
            _shadowPlayer.RepositionAng.y = value;
            _robotPlayer.RepositionAng.y = value;
            _angYField.text = value.ToString();
        }
        public void OnEndEditAngZ()
        {
            if (_angZField.text == "" || _angZField.text == "-")
            {
                _angZField.text = _shadowPlayer.RepositionAng.z.ToString();
                return;
            }

            var value = float.Parse(_angZField.text);
            _shadowPlayer.RepositionAng.z = value;
            _robotPlayer.RepositionAng.z = value;
            _angZField.text = value.ToString();
        }
        #endregion
    }
}