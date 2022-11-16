using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

namespace smart3tene
{
    [RequireComponent(typeof(PictureTaker))]
    public class AnnotationRandomTaker : MonoBehaviour
    {
        #region Serialized Private Field
        [SerializeField] private string _fileName = "Annotation";
        [SerializeField] private Camera _camera;

        //デバック用
        [SerializeField] private List<Transform> _rangeBoxes = new List<Transform>();
        [SerializeField] private Transform _light;
        #endregion

        #region Private Fields
        private PictureTaker _pictureTaker;
        #endregion

        #region Readonly Fields
        readonly string defaultDirectory = UnityEngine.Application.streamingAssetsPath + "/../../../AnnotationImages";
        #endregion

        #region private struct
        private struct Range
        {
            public float maxX;
            public float minX;
            public float maxY;
            public float minY;
            public float maxZ;
            public float minZ;

            public Range(float maxX, float minX, float maxY, float minY, float maxZ, float minZ)
            {
                this.maxX = maxX;
                this.minX = minX;
                this.maxY = maxY;
                this.minY = minY;
                this.maxZ = maxZ;
                this.minZ = minZ;
            }
        }
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            _pictureTaker= GetComponent<PictureTaker>();
        }

        private async void Update()
        {
            //デバック用
            if (Keyboard.current.bKey.wasPressedThisFrame)
            {
                await RandomTake(224, 224, 0, 10, _rangeBoxes, _light);
            }
        }
        #endregion

        #region Public method
        public async UniTask RandomTake(int width, int height, int startNum, int EndNum, List<Transform> rangeBoxes, Transform light)
        {
            //後でちゃんと例外処理行う
            if (startNum > EndNum)
            {
                Debug.LogError("error");
                return;
            }


            var now = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            for (int i = startNum; i <= EndNum; i++)
            {
                //ランダムにカメラを配置
                var PosAng = GetRandomPosAndAngle(_rangeBoxes);
                _camera.transform.SetPositionAndRotation(PosAng.pos, Quaternion.Euler(PosAng.angle));

                //ランダムに太陽を配置


                //撮影
                var colorFilePath = await _pictureTaker.TakeColorPicture(Camera.main, width, height, _fileName, startNum, defaultDirectory + $"/{now}/Images");
                var tagFilePath = await _pictureTaker.TakeTagPicture(Camera.main, width, height, _fileName, startNum, defaultDirectory + $"/{now}/tags");       
            }

            //撮影完了の表示
            Debug.Log("撮影が完了しました" + "\n" + $"保存先:{defaultDirectory}/{now}");
        }
        #endregion

        #region Private method
        private (Vector3 pos, Vector3 angle) GetRandomPosAndAngle(List<Transform> rangeBoxes)
        {
            var randomIndex = UnityEngine.Random.Range(0, rangeBoxes.Count);
            return GetRandomPosAndAngle(rangeBoxes[randomIndex]);
        }

        private (Vector3 pos, Vector3 angle) GetRandomPosAndAngle(Transform rangeBox)
        {
            //範囲を決める
            var range = GetRnage(rangeBox);

            //カメラの位置を範囲内でランダムで決定する
            var posX = UnityEngine.Random.Range(range.minX, range.maxX);
            var posY = UnityEngine.Random.Range(range.minY, range.maxY);
            var posZ = UnityEngine.Random.Range(range.minZ, range.maxZ);
            var pos = new Vector3(posX, posY, posZ);

            //カメラの向きを条件内でランダムに決定する
            var angleX = UnityEngine.Random.Range(-30f, 30f);
            var angleY = UnityEngine.Random.Range(0f, 359f);
            var angleZ = 0f;
            var angle = new Vector3(angleX, angleY, angleZ);

            return (pos, angle); 
        }

        private Range GetRnage(Transform rangeBox)
        {
            var maxPos = rangeBox.position
                + rangeBox.right * rangeBox.localScale.x / 2f
                + rangeBox.up * rangeBox.localScale.y / 2f
                + rangeBox.forward * rangeBox.localScale.z / 2f;

            var minPos = rangeBox.position
                - rangeBox.right * rangeBox.localScale.x / 2f
                - rangeBox.up * rangeBox.localScale.y / 2f
                - rangeBox.forward * rangeBox.localScale.z / 2f;

            return new Range(maxPos.x, minPos.x, maxPos.y, minPos.y, maxPos.z, minPos.z);
        }
        #endregion
    }
}