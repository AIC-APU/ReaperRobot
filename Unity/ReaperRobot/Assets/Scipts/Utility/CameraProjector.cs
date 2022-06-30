using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace smart3tene
{
    [RequireComponent(typeof(RawImage))]
    public class CameraProjector : MonoBehaviour
    {
        //�J�������B�e�����f����RawImage�ɓ��e���܂�
        //delay�̕b��,�\����x�点�邱�Ƃ��ł��܂�

        #region public Fields
        [System.NonSerialized] public float delay = 0f; //�P��: �b
        [System.NonSerialized] public Camera recordingCamera;
        #endregion

        #region private Fields
        private Queue<Texture2D> _savedTexQueue = new();
        private RawImage _rawImage;
        private Vector2Int _textureSize;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _rawImage = GetComponent<RawImage>();
            _textureSize = Vector2Int.RoundToInt(_rawImage.rectTransform.rect.size);
            _rawImage.texture = RecordTexture(recordingCamera, _textureSize);
        }

        private void FixedUpdate()
        {
            //�e�N�X�`���̐���
            _textureSize = Vector2Int.RoundToInt(_rawImage.rectTransform.rect.size);
            var recordedTexture = RecordTexture(recordingCamera, _textureSize);
            _savedTexQueue.Enqueue(recordedTexture);


            //delay�̂��߂ɕۑ����K�v�ȃe�N�X�`���̖������v�Z
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
            //camera�̉摜���e�N�X�`���Ƃ��Ď擾
            var texture = new Texture2D(textureSize.x, textureSize.y, TextureFormat.RGB24, false);
            var render = new RenderTexture(textureSize.x, textureSize.y, 0);

            camera.targetTexture = render;
            camera.Render();

            var cache = RenderTexture.active;
            RenderTexture.active = render;

            //ReadPixels�͏d�������炵���B
            //https://docs.unity3d.com/jp/current/ScriptReference/Texture2D.ReadPixels.html �ɑ�ֈĂ���
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


