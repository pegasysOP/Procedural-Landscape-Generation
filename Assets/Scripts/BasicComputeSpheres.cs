using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BasicComputeSpheres : MonoBehaviour
{
    [SerializeField] GameObject objectPrefab;
    [SerializeField] int sphereAmount;
    [SerializeField] ComputeShader shader;

    ComputeBuffer resultBuffer;
    int kernel;
    uint threadGroupSize;
    Vector3[] output;

    Transform[] instances;

    void Start()
    {
        kernel = shader.FindKernel("Spheres");
        shader.GetKernelThreadGroupSizes(kernel, out threadGroupSize, out _, out _);

        resultBuffer = new ComputeBuffer(sphereAmount, sizeof(float) * 3);
        output = new Vector3[sphereAmount];

        instances = new Transform[sphereAmount];
        for (int i = 0; i < sphereAmount; i++)
            instances[i] = Instantiate(objectPrefab, transform).transform;
    }

    void Update()
    {
        shader.SetBuffer(kernel, "Result", resultBuffer);

        int threadGroups = (int) ((sphereAmount + (threadGroupSize - 1)) / threadGroupSize);
        shader.Dispatch(kernel, threadGroups, 1, 1);

        resultBuffer.GetData(output);

        for (int i = 0; i < instances.Length; i++)
            instances[i].localPosition = output[i];
    }

    private void OnDestroy()
    {
        resultBuffer.Release();
    }
}
