using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace smart3tene
{
    public static class MeshCreator
    {
        #region ReadOnly Fields
        readonly static int[] cubeTriangles =
        {
            0, 2, 1, //face front
			0, 3, 2,
            2, 3, 4, //face top
			2, 4, 5,
            1, 2, 5, //face right
			1, 5, 6,
            0, 7, 4, //face left
			0, 4, 3,
            5, 4, 7, //face back
			5, 7, 6,
            0, 6, 7, //face bottom
			0, 1, 6,
        };

        readonly static Vector3[] cubeVertices =
        {
                new Vector3 (-0.5f, -0.5f, -0.5f),
                new Vector3 (0.5f, -0.5f, -0.5f),
                new Vector3 (0.5f, 0.5f, -0.5f),
                new Vector3 (-0.5f, 0.5f, -0.5f),
                new Vector3 (-0.5f, 0.5f, 0.5f),
                new Vector3 (0.5f, 0.5f, 0.5f),
                new Vector3 (0.5f, -0.5f, 0.5f),
                new Vector3 (-0.5f, -0.5f, 0.5f),
        };
        #endregion

        #region Public method
        public static GameObject CreateCubeMesh(Vector3 centerPos, Material material, float size)
        {
            //必要なコンポーネントをadd(meshFilter等)
            var obj = new GameObject("CubeMesh");
            var meshFilter = obj.AddComponent<MeshFilter>();
            var meshRender = obj.AddComponent<MeshRenderer>();
            var mesh = meshFilter.mesh;

            //前処理
            mesh.Clear();

            //verticesとtrianglesを指定
            mesh.vertices = cubeVertices;
            mesh.triangles = cubeTriangles;

            //後処理
            mesh.Optimize();
            mesh.RecalculateNormals();

            //オブジェクトの設定
            obj.transform.position = centerPos;
            obj.transform.localScale = new Vector3(size, size, size);
            obj.isStatic = true;
            meshRender.material = material;
            meshRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRender.receiveShadows = false;

            //作成したGameObjectを返す
            return obj;          
        }
        #endregion

        #region Private method
        #endregion
    }
}