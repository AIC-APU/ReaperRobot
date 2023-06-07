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

            var newTexture = RecordTexture(recordingCamera, _textureSize);
            _rawImage.texture = newTexture;
            newTexture = null;
        }

        void OnDestroy()
        {
            while (_savedTexQueue.Count > 0)
            {
                var dequeueTex = _savedTexQueue.Dequeue();
                Destroy(dequeueTex);
                dequeueTex = null;
            }

            Destroy(_rawImage.texture);
            _rawImage.texture = null;

            _savedTexQueue.Clear();
        }

        private void FixedUpdate()
        {
            //テクスチャの生成と保存
            _textureSize = Vector2Int.RoundToInt(_rawImage.rectTransform.rect.size);

            var newTexture = RecordTexture(recordingCamera, _textureSize);
            _savedTexQueue.Enqueue(newTexture);
            newTexture = null;

            //delayのために保存が必要なテクスチャの枚数を計算
            var optimalTexQueueSize = (int)(delay / Time.fixedDeltaTime);

            //保存しているテクスチャの枚数がdelayのために必要な枚数に達していない場合は、何もしない
            if (_savedTexQueue.Count < optimalTexQueueSize + 1) return;

            //保存しているテクスチャの枚数がdelayのために必要な枚数を超えたら、古いテクスチャを削除
            while (_savedTexQueue.Count > optimalTexQueueSize + 1)
            {
                var dequeueTex = _savedTexQueue.Dequeue();
                Destroy(dequeueTex);
                dequeueTex = null;
            }

            //保存しているテクスチャの枚数がdelayのために必要な枚数に達したら、RawImageのテクスチャを更新
            if (_savedTexQueue.Count == optimalTexQueueSize + 1)
            {
                Destroy(_rawImage.texture);
                _rawImage.texture = null;

                var dequeueTex = _savedTexQueue.Dequeue();
                _rawImage.texture = dequeueTex;
            }
        }
        #endregion

        #region private method
        private Texture2D RecordTexture(Camera camera, Vector2Int textureSize)
        {
            //cameraの画像をテクスチャとして取得
            var texture = new Texture2D(textureSize.x, textureSize.y, TextureFormat.RGBA32, false);
            var render = new RenderTexture(textureSize.x, textureSize.y, 24, RenderTextureFormat.ARGB32);

            camera.targetTexture = render;
            camera.Render();

            //var cache = RenderTexture.active;
            //RenderTexture.active = render;

            Graphics.CopyTexture(render, texture);

            //RenderTexture.active = cache;
            camera.targetTexture = null;

            render.Release();
            Destroy(render);
            render = null;

            return texture;
        }
        #endregion
    }
}


