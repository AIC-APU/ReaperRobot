using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace smart3tene
{

    public class CaptureFromCamera : MonoBehaviour
    {
        public IEnumerator Capture(int width, int height, Camera camera, TextureFormat format, string savePath)
        {
            // Debug.Log("Capture " + savePath);
            // var d_width = camera.targetTexture.width;
            // var d_height = camera.targetTexture.height;
            Texture2D tex = new Texture2D(width, height, format, false);
            if (camera.targetTexture != null)
                camera.targetTexture.Release();
            camera.targetTexture = new RenderTexture(width, height, 24);

            yield return new WaitForEndOfFrame();
            
            RenderTexture.active = camera.targetTexture;
            tex.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(savePath, bytes);
            Destroy(tex);
            if (camera.targetTexture != null)
                camera.targetTexture.Release();
            camera.targetTexture = null;
            yield break;
        }
    }
}
