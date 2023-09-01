using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BetterComputeSpheres : MonoBehaviour
{
    [SerializeField, Range(0, 1000)] int sphereAmount = 1000;
    public ComputeShader shader;

    public Mesh mesh;
    public Material material;
    public float Scale = 1;

    ComputeBuffer resultBuffer;
    ComputeBuffer meshTriangles;
    ComputeBuffer meshPositions;
    int kernel;
    uint threadGroupSize;
    Bounds bounds;
    int threadGroups;

    bool increasing = true;

    void Start()
    {
        kernel = shader.FindKernel("Spheres");
        shader.GetKernelThreadGroupSizes(kernel, out threadGroupSize, out _, out _);

        threadGroups = (int)((sphereAmount + (threadGroupSize - 1)) / threadGroupSize);

        resultBuffer = new ComputeBuffer(sphereAmount, sizeof(float) * 3);

        int[] triangles = mesh.triangles;
        meshTriangles = new ComputeBuffer(triangles.Length, sizeof(int));
        meshTriangles.SetData(triangles);
        Vector3[] positions = mesh.vertices.Select(p => p * Scale).ToArray(); //adjust scale here
        meshPositions = new ComputeBuffer(positions.Length, sizeof(float) * 3);
        meshPositions.SetData(positions);

        shader.SetBuffer(kernel, "Result", resultBuffer);

        material.SetBuffer("SphereLocations", resultBuffer);
        material.SetBuffer("Triangles", meshTriangles);
        material.SetBuffer("Positions", meshPositions);

        bounds = new Bounds(Vector3.zero, Vector3.one * 20);
    }

    void Update()
    {
        shader.SetFloat("Time", Time.time);
        shader.Dispatch(kernel, threadGroups, 1, 1);

        Graphics.DrawProcedural(material, bounds, MeshTopology.Triangles, meshTriangles.count, sphereAmount);

        if (increasing)
        {
            sphereAmount++;

            if (sphereAmount >= 1000)
                increasing = false;
        }
        else
        {
            sphereAmount--;

            if (sphereAmount <= 0)
                increasing= true;
        }
    }

    void OnDestroy()
    {
        resultBuffer.Dispose();
        meshTriangles.Dispose();
        meshPositions.Dispose();
    }
}