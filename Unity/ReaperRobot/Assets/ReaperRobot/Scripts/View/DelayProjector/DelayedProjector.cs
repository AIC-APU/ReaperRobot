using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace Plusplus.ReaperRobot.Scripts.View.DelayProjector
{
    [RequireComponent(typeof(RawImage))]
    public class DelayedProjector : MonoBehaviour
    {
        //カメラが撮影した映像をRawImageに投影します
        //delayの秒数,表示を遅らせることができます

        //Build SettingsのプラットフォームがWindows・MacOSX・Linuxの時のみ上手く表示することができます
        //androidにしていた時、いくつかのオブジェクトが表示されませんでした

        #region public Fields
        public float delay = 0f; //単位: 秒
        public Camera recordingCamera;
        #endregion

        #region private Fields
        private Queue<Texture2D> _savedTexQueue = new();
        private RawImage _rawImage;
        private Vector2Int _textureSize;
        #endregion

        #region MonoBehaviour Callbacks
        private async void Awake()
        {
            _rawImage = GetComponent<RawImage>();
            _textureSize = Vector2Int.RoundToInt(_rawImage.rectTransform.rect.size);

            await UniTask.WaitUntil(() => recordingCamera != null);
            _rawImage.texture = RecordTexture(recordingCamera, _textureSize);
        }

        private void FixedUpdate()
        {
            //テクスチャの生成
            _textureSize = Vector2Int.RoundToInt(_rawImage.rectTransform.rect.size);
            var recordedTexture = RecordTexture(recordingCamera, _textureSize);
            _savedTexQueue.Enqueue(recordedTexture);


            //delayのために保存が必要なテクスチャの枚数を計算
            var optimalTexQueueSize = (int)(delay / Time.fixedDeltaTime);

            if (_savedTexQueue.Count < optimalTexQueueSize + 1) return;

            while (_savedTexQueue.Count > optimalTexQueueSize + 1)
            {
                Destroy(_savedTexQueue.Dequeue());
            }

            if (_savedTexQueue.Count == optimalTexQueueSize + 1)
            {
                Destroy(_rawImage.texture);
                _rawImage.texture = _savedTexQueue.Dequeue();
            }
        }
        #endregion

        #region private method
        private Texture2D RecordTexture(Camera camera, Vector2Int textureSize)
        {
            //cameraの画像をテクスチャとして取得
            var texture = new Texture2D(textureSize.x, textureSize.y, TextureFormat.RGB24, false);
            var render = new RenderTexture(textureSize.x, textureSize.y, 0);

            camera.targetTexture = render;
            camera.Render();

            var cache = RenderTexture.active;
            RenderTexture.active = render;

            //ReadPixelsは重い処理らしい。
            //https://docs.unity3d.com/jp/current/ScriptReference/Texture2D.ReadPixels.html に代替案あり
            texture.ReadPixels(new Rect(0, 0, textureSize.x, textureSize.y), 0, 0);
            texture.Apply();

            RenderTexture.active = cache;
            camera.targetTexture = null;
            Destroy(render);

            return texture;
        }
        #endregion
    }
}


