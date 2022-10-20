using Cysharp.Threading.Tasks;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Localization.Settings;


namespace smart3tene.Reaper
{
    //デバック用です
    public class PictureController : MonoBehaviour
    {
        #region Serialized Private Fields
        [Header("FileName")]
        [SerializeField] private string _fileName = ""; //末に拡張子は付けない

        [Header("Save Text")]
        [SerializeField, TextArea(1, 4)] private string _ja = "は保存されました";
        [SerializeField, TextArea(1, 4)] private string _en = "was saved.";
        #endregion

        #region Private Fields
        private PictureTaker _pictureTaker;        
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            _pictureTaker = GetComponent<PictureTaker>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                _ = TakePicture();   
            }
        }
        #endregion

        private async UniTaskVoid TakePicture()
        {
            //撮影
            var colorFilePath = await _pictureTaker.TakeColorPicture(Camera.main, _fileName);
            var tagFilePath = await _pictureTaker.TakeTagPicture(Camera.main, _fileName);
            

            //color画像保存のテキスト表示
            var colorFileName = Path.GetFileName(colorFilePath);
            var colorText = GetText(LocalizationSettings.SelectedLocale.Identifier.Code, colorFileName);
            ReaperEventManager.InvokeTextPopupEvent(colorText);

            //数秒待つ(上のテキストを表示するため)
            await UniTask.Delay(TimeSpan.FromSeconds(2));

            //tag画像保存のテキスト表示
            var tagFileName = Path.GetFileName(tagFilePath);
            var tagText = GetText(LocalizationSettings.SelectedLocale.Identifier.Code, tagFileName);
            ReaperEventManager.InvokeTextPopupEvent(tagText);
        }

        private string GetText(string localeCode, string fileName)
        {
            var text = fileName + " ";
            switch (localeCode)
            {
                case "ja":
                    text += _ja;
                    break;

                case "en":
                    text += _en;
                    break;

                default:
                    break;
            }
            return text;
        }
    }

}
