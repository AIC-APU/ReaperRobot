using Cysharp.Threading.Tasks;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace smart3tene
{
    public static class CaptureFromCamera
    {
        public static async UniTask Capture(int width, int height, Camera camera, TextureFormat format, string savePath)
        {
            var texture = new Texture2D(width, height, format, false);
            var render  = new RenderTexture(width, height, 24);


            //cameraの映像を反映するRenderTextureを設定
            if (camera.targetTexture != null)
            {
                camera.targetTexture.Release();
            }
            camera.targetTexture = render;


            //1フレーム待つ
            await UniTask.Yield();


            //RenderTextureと同じ画像をTextureにコピー
            var cache = RenderTexture.active;
            RenderTexture.active = render;
            texture.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
            texture.Apply();


            //Textureの画像をPNGファイルとしてエンコード
            byte[] bytes = texture.EncodeToPNG();


            //エンコードしたPNGファイルを保存
            if (!savePath.EndsWith(".png"))
            {
                //拡張子のつけ忘れ防止
                savePath += ".png";
            }

            var directryName = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(directryName))
            {
                //savePathのディレクトリがなければ作る
                Directory.CreateDirectory(directryName);
            }
            File.WriteAllBytes(savePath, bytes);

            #if UNITY_EDITOR
            AssetDatabase.Refresh();
            #endif

            //テクスチャの破棄（メモリリーク防止）
            Object.Destroy(texture);
            Object.Destroy(render);


            //後処理
            RenderTexture.active = cache;
            if (camera.targetTexture != null)
            {
                camera.targetTexture.Release();
            }
            camera.targetTexture = null;

        }
    }
}
