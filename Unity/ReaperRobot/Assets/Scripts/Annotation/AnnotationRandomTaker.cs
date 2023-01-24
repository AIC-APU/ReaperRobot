using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace smart3tene
{
    [RequireComponent(typeof(PictureTaker))]
    public class AnnotationRandomTaker : MonoBehaviour
    {
        #region Serialized Private Field
        [SerializeField] private string _fileName = "Annotation";
        [SerializeField] private Camera _camera;
        #endregion

        #region Private Fields
        private PictureTaker _pictureTaker;
        #endregion

        #region Readonly Fields
        readonly string DefaultDirectory = UnityEngine.Application.streamingAssetsPath + "/../../../AnnotationImages";
        readonly float LightRangeRadius = 10f;
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
        #endregion

        #region Public method
        public async UniTask RandomTake(int width, int height, int startNum, int EndNum, int minFov, int maxFov, List<Transform> rangeBoxes, Transform light)
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
                var (pos, angle) = GetRandomPosAndAngle(rangeBoxes);
                _camera.transform.SetPositionAndRotation(pos, Quaternion.Euler(angle));

                //カメラのfovを指定
                var random = new System.Random();
                var randomFov = random.Next(minFov, maxFov + 1);

                //ランダムに光源を配置
                light.transform.position = GetRandomLightPos(LightRangeRadius);

                //撮影
                _ = await _pictureTaker.TakeColorPicture(Camera.main, width, height, randomFov, _fileName, startNum, DefaultDirectory + $"/{now}/Images");
                _ = await _pictureTaker.TakeTagPicture(Camera.main, width, height, randomFov, _fileName, startNum, DefaultDirectory + $"/{now}/tags");       
            }

            //撮影完了の表示
            var filePath = Path.GetFullPath($"{DefaultDirectory}/{now}");
            ReaperEventManager.InvokeTextPopupEvent("撮影が完了しました\n" + "保存先:\n" + filePath);
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

        private Vector3 GetRandomLightPos(float radius)
        {
            //原点を中心とする半径radiusの天球上の座標を返す
            var posX = UnityEngine.Random.Range(-radius, radius);

            var rangeZ = Mathf.Sqrt(-posX * posX + radius * radius);
            var posZ = UnityEngine.Random.Range(-rangeZ, rangeZ);

            var posY = Mathf.Sqrt(-posX * posX - posZ * posZ + radius * radius);

            return new Vector3(posX, posY, posZ);
        }
        #endregion
    }
}