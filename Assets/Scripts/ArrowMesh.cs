using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Mesh theMesh = GetComponent<MeshFilter>().mesh;   // get the mesh component
        theMesh.Clear();    // delete whatever is there!!   
        Vector3[] v = new Vector3[14];   // 2x2 mesh needs 3x3 vertices
        int[] t = new int[60];         // Number of triangles: 2x2 mesh and 2x triangles on each mesh-unit
        Vector3[] n = new Vector3[14];   // MUST be the same as number of vertices
        Vector2[] uv = new Vector2[14];

        v[0] = new Vector3(0, 1, 1);
        v[1] = new Vector3(-1, 1, 0);
        v[2] = new Vector3(1, 1, 0);
        v[3] = new Vector3(-0.5f, 1, 0);
        v[4] = new Vector3(0.5f, 1, 0);
        v[5] = new Vector3(-0.5f, 1, -1);
        v[6] = new Vector3(0.5f, 1, -1);
        v[7] = new Vector3(0, 0, 1);
        v[8] = new Vector3(-1, 0, 0);
        v[9] = new Vector3(1, 0, 0);
        v[10] = new Vector3(-0.5f, 0, 0);
        v[11] = new Vector3(0.5f, 0, 0);
        v[12] = new Vector3(-0.5f, 0, -1);
        v[13] = new Vector3(0.5f, 0, -1);

        n[0] = new Vector3(0, 1, 0);
        n[1] = new Vector3(0, 1, 0);
        n[2] = new Vector3(0, 1, 0);
        n[3] = new Vector3(0, 1, 0);
        n[4] = new Vector3(0, 1, 0);
        n[5] = new Vector3(0, 1, 0);
        n[6] = new Vector3(0, 1, 0);
        n[7] = new Vector3(0, 1, 0);
        n[8] = new Vector3(0, 1, 0);
        n[9] = new Vector3(0, 1, 0);
        n[10] = new Vector3(0, 1, 0);
        n[11] = new Vector3(0, 1, 0);
        n[12] = new Vector3(0, 1, 0);
        n[13] = new Vector3(0, 1, 0);

        t[0] = 1; t[1] = 0; t[2] = 2;
        t[3] = 5; t[4] = 3; t[5] = 4;
        t[6] = 5; t[7] = 4; t[8] = 6;
        t[9] = 7; t[10] = 0; t[11] = 1;
        t[12] = 7; t[13] = 1; t[14] = 8;
        t[15] = 10; t[16] = 3; t[17] = 5;
        t[18] = 10; t[19] = 5; t[20] = 12;
        t[21] = 9; t[22] = 7; t[23] = 8;
        t[24] = 13; t[25] = 11; t[26] = 10;
        t[27] = 13; t[28] = 10; t[29] = 12;
        t[30] = 9; t[31] = 2; t[32] = 0;
        t[33] = 9; t[34] = 0; t[35] = 7;
        t[36] = 13; t[37] = 6; t[38] = 4;
        t[39] = 13; t[40] = 4; t[41] = 11;
        t[42] = 8; t[43] = 1; t[44] = 3;
        t[45] = 8; t[46] = 3; t[47] = 10;
        t[48] = 12; t[49] = 5; t[50] = 6;
        t[51] = 12; t[52] = 6; t[53] = 13;
        t[54] = 11; t[55] = 4; t[56] = 2;
        t[57] = 11; t[58] = 2; t[59] = 9;

        uv[0] = new Vector2(0.5f, 1);
        uv[1] = new Vector2(0, 0.5f);
        uv[2] = new Vector2(1, 0.5f);
        uv[3] = new Vector2(0.25f, 0.5f);
        uv[4] = new Vector2(0.75f, 0.5f);
        uv[5] = new Vector2(0.25f, 0);
        uv[6] = new Vector2(0.75f, 0);
        uv[7] = new Vector2(0.5f, 1);
        uv[8] = new Vector2(0, 0.5f);
        uv[9] = new Vector2(1, 0.5f);
        uv[10] = new Vector2(0.25f, 0.5f);
        uv[11] = new Vector2(0.75f, 0.5f);
        uv[12] = new Vector2(0.25f, 0);
        uv[13] = new Vector2(0.75f, 0);

        theMesh.vertices = v; //  new Vector3[3];
        theMesh.triangles = t; //  new int[3];

        n = ComputeNormals(v, n);

        theMesh.normals = n;
        theMesh.uv = uv;
        theMesh.uv2 = uv;

    }

    Vector3[] ComputeNormals(Vector3[] v, Vector3[] n)
    {
        Vector3[] triNormal = new Vector3[20];
        triNormal[0] = FaceNormal(v, 1, 0, 2);
        triNormal[1] = FaceNormal(v, 5, 3, 4);
        triNormal[2] = FaceNormal(v, 5, 4, 6);
        triNormal[3] = FaceNormal(v, 7, 0, 1);
        triNormal[4] = FaceNormal(v, 7, 1, 8);
        triNormal[5] = FaceNormal(v, 10, 3, 5);
        triNormal[6] = FaceNormal(v, 10, 5, 12);
        triNormal[7] = FaceNormal(v, 9, 7, 8);
        triNormal[8] = FaceNormal(v, 13, 11, 10);
        triNormal[9] = FaceNormal(v, 13, 10, 12);
        triNormal[10] = FaceNormal(v, 9, 2, 0);
        triNormal[11] = FaceNormal(v, 9, 0, 7);
        triNormal[12] = FaceNormal(v, 13, 6, 4);
        triNormal[13] = FaceNormal(v, 13, 4, 11);
        triNormal[14] = FaceNormal(v, 8, 1, 3);
        triNormal[15] = FaceNormal(v, 8, 3, 10);
        triNormal[16] = FaceNormal(v, 12, 5, 6);
        triNormal[17] = FaceNormal(v, 12, 6, 13);
        triNormal[18] = FaceNormal(v, 11, 4, 2);
        triNormal[19] = FaceNormal(v, 11, 2, 9);

        n[0] = (triNormal[0] + triNormal[3] + triNormal[10] + triNormal[11]).normalized;
        n[1] = (triNormal[0] + triNormal[3] + triNormal[4] + triNormal[14]).normalized;
        n[2] = (triNormal[0] + triNormal[10] + triNormal[18] + triNormal[19]).normalized;
        n[3] = (triNormal[1] + triNormal[5] + triNormal[14] + triNormal[15]).normalized;
        n[4] = (triNormal[1] + triNormal[2] + triNormal[12] + triNormal[13] + triNormal[18]).normalized;
        n[5] = (triNormal[1] + triNormal[2] + triNormal[5] + triNormal[6] + triNormal[16]).normalized;
        n[6] = (triNormal[2] + triNormal[12] + triNormal[16] + triNormal[17]).normalized;
        n[7] = (triNormal[3] + triNormal[4] + triNormal[7] + triNormal[11]).normalized;
        n[8] = (triNormal[4] + triNormal[7] + triNormal[14] + triNormal[15]).normalized;
        n[9] = (triNormal[7] + triNormal[10] + triNormal[11] + triNormal[19]).normalized;
        n[10] = (triNormal[5] + triNormal[6] + triNormal[8] + triNormal[9] + triNormal[15]).normalized;
        n[11] = (triNormal[8] + triNormal[13] + triNormal[18] + triNormal[19]).normalized;
        n[12] = (triNormal[6] + triNormal[9] + triNormal[16] + triNormal[17]).normalized;
        n[13] = (triNormal[8] + triNormal[9] + triNormal[12] + triNormal[13] + triNormal[17]).normalized;

        return n;
    }

    Vector3 FaceNormal(Vector3[] v, int i0, int i1, int i2)
    {
        Vector3 a = v[i1] - v[i0];
        Vector3 b = v[i2] - v[i0];
        return Vector3.Cross(a, b).normalized;
    }
}
