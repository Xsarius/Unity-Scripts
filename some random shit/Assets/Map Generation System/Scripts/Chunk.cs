using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    // public in

    public Vector3 chunkOffset;

    public ChunkSO data;

    // public out
    public Transform transform;

    public Mesh mesh;

    public GameObject[,] buildingGrid;


    // private
    private Vector3[] verticies;

    private int[] triangles;



    /////////////////////////////////////////

    // Initialization of Chunk class element
    public void GenerateChunk(float noiseScale)
    {

        GenerateMesh((int)data.chunkSize.x, (int)data.chunkSize.z, data.cellSize, noiseScale);

        GenerateBuildingGrid((int)data.chunkSize.x, (int)data.chunkSize.z);

    }

    // Mesh generation 
    private void GenerateMesh(int chunkWidth, int chunkLength, float cellSize, float noiseScale)
    {
        verticies = new Vector3[chunkWidth * chunkLength];
        triangles = new int[verticies.Length * 6];
        mesh = new Mesh();

        // Generating height map 
        float[] heightMap = Noise.GenerateHeightMap(chunkWidth, chunkLength, noiseScale);

        for (int z = 0, currentQuad = 0; z < chunkLength; z += 2)
        {
            for (int x = 0; x < chunkWidth; currentQuad++, x += 2)
            {
                int leftBottom = z * chunkWidth + x;
                int rightBottom = z * chunkWidth + x + 1;
                int leftTop = (z + 1) * chunkWidth + x;
                int rightTop = (z + 1) * chunkWidth + x + 1;

                verticies[leftBottom] = new Vector3(x * cellSize, heightMap[currentQuad], z * cellSize);
                verticies[rightBottom] = new Vector3((x + 1) * cellSize, heightMap[currentQuad], z * cellSize);
                verticies[leftTop] = new Vector3(x * cellSize, heightMap[currentQuad], (z + 1) * cellSize);
                verticies[rightTop] = new Vector3((x + 1) * cellSize, heightMap[currentQuad], (z + 1) * cellSize);
            }
            currentQuad++;
        }

        for (int z = 0, vert = 0, tris = 0; z < chunkLength - 1; z++, vert++)
        {

            for (int x = 0; x < chunkWidth - 1; x++, vert++, tris += 6)
            {
                triangles[tris] = vert;
                triangles[tris + 1] = vert + chunkWidth;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + chunkWidth;
                triangles[tris + 5] = vert + chunkWidth + 1;
            }
        }

        UpdateMesh();
    }

    // Building grid genertion
    private void GenerateBuildingGrid(int chunkWidth, int chunkLength)
    {
        buildingGrid = new GameObject[chunkWidth / 2, chunkLength / 2];
    }

    public void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = verticies;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    public void AdjustToOffset(Vector3 offset)
    {
        chunkOffset = offset;
    }
}