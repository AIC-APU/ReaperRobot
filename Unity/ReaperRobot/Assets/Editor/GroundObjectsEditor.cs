using UnityEngine;
using UnityEditor;
using Plusplus.ReaperRobot.Scripts.View.Grass;

[CustomEditor(typeof(GrassObject))]
[CanEditMultipleObjects]
public class GroundObjectsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10f);

        if (GUILayout.Button("Ground Objects"))
        {
            GroundSelectedObjects();
        }
    }

    private void GroundSelectedObjects()
    {
        // 選択されたオブジェクトのリストを取得
        GameObject[] selectedObjects = Selection.gameObjects;

        foreach (GameObject obj in selectedObjects)
        {
            // オブジェクトの位置を取得
            Vector3 objectPosition = obj.transform.position;

            // レイキャストを使用して地面に接地させる
            RaycastHit hit;
            if (Physics.Raycast(objectPosition, Vector3.down, out hit, 10f))
            {
                float distanceToGround = hit.distance;
                objectPosition.y -= distanceToGround;
                obj.transform.position = hit.point;

                // オブジェクトを地面上に垂直に配置
                Vector3 objectUpDirection = obj.transform.up;
                Quaternion rotation = Quaternion.FromToRotation(objectUpDirection, hit.normal);
                obj.transform.rotation = rotation * obj.transform.rotation;
            }
        }
    }
}