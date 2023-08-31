using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Plusplus.ReaperRobot.Scripts.View.DelayProjector
{
    [RequireComponent(typeof(RawImage))]
    public class DelayedProjector : MonoBehaviour
    {
        //カメラが撮影した映像をRawImageに投影します
        //delayの秒数,表示を遅らせることができます

        //Build SettingsのプラットフォームがWindows・MacOSX・Linux・WebGLの時のみ上手く表示することができます
        //androidにしていた時、いくつかのオブジェクトが表示されませんでした

        #region public Fields
        public float Delay = 0f; //単位: 秒
        public Camera RecordingCamera;
        #endregion

        #region private Fields
        private Queue<Texture2D> _savedTexQueue = new();
        private RawImage _rawImage;
        private RenderTexture _renderTexture;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _rawImage = GetComponent<RawImage>();

            //RenderTextureを生成
            var textureSize = Vector2Int.RoundToInt(_rawImage.rectTransform.rect.size);
            _renderTexture = new RenderTexture(textureSize.x, textureSize.y, 24, RenderTextureFormat.ARGB32);
            RecordingCamera.targetTexture = _renderTexture;
        }

        void Start()
        {
            //カメラの準備ができたら、RenderTextureを生成
            var newTexture = RecordTexture(RecordingCamera, _rawImage);
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

            _renderTexture.Release();
            Destroy(_renderTexture);
            _renderTexture = null;

            _savedTexQueue.Clear();
        }

        private void FixedUpdate()
        {
            //テクスチャの生成と保存
            _savedTexQueue.Enqueue(RecordTexture(RecordingCamera, _rawImage));

            //delayのために保存が必要なテクスチャの枚数を計算
            var optimalTexQueueSize = (int)(Delay / Time.fixedDeltaTime);

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
        private Texture2D RecordTexture(Camera camera, RawImage rawImage)
        {
            //cameraの画像をテクスチャとして取得
            var textureSize = Vector2Int.RoundToInt(rawImage.rectTransform.rect.size);
            var texture = new Texture2D(textureSize.x, textureSize.y, TextureFormat.RGBA32, false);

            if (_renderTexture.texelSize.x != texture.texelSize.x ||
                _renderTexture.texelSize.y != texture.texelSize.y)
            {
                //前のRenderTextureを破棄
                _renderTexture.Release();
                Destroy(_renderTexture);
                _renderTexture = null;

                //新しくRenderTextureを生成
                _renderTexture = new RenderTexture(textureSize.x, textureSize.y, 24, RenderTextureFormat.ARGB32);
                RecordingCamera.targetTexture = _renderTexture;
                camera.Render();
            }

            Graphics.CopyTexture(_renderTexture, texture);

            return texture;
        }
        #endregion
    }
}


