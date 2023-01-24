using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace smart3tene
{
    public class CameraForMovie : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _targetPos;
        [SerializeField] private float _rotateSpeed = 1.0f;
        #endregion

        #region MonoBehaviour Callbacks
        void Update()
        {
            //原点を中心にxz平面を回転
            var center = new Vector3(_targetPos.transform.position.x, _camera.transform.position.y, _targetPos.transform.position.z);
            _camera.transform.RotateAround(center, Vector3.up, _rotateSpeed * Time.deltaTime);

        }
        #endregion
    }

}