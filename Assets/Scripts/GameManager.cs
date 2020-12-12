using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;

public class GameManager : MonoBehaviour
{
    private FractalProcessor _Processor;
    private List<float3> _PlottingData;
    [SerializeField] float _PointSize = 0.01f;

    [SerializeField] private RenderTexture _ExternPointDataTexture;
    private RenderTexture _InternalPointDataTexture;

    private ComputeBuffer _PointDataBuffer;

    [SerializeField]private ComputeShader _DataCopyCS;

    private int _KernelIndex;
    private string _KernelName = "CSMain";

    // Start is called before the first frame update
    void Start()
    {
        List<float3> A = new List<float3>();

        A.Add(float3.zero);
        //A.Add(new float2(0.0f, 0.5f));
        //A.Add(new float2(0.5f, 0.5f));
        //A.Add(new float2(0.5f, 0.0f));

        _Processor = new FractalProcessor(A);

        _Processor.SetFunctions(new List<System.Func<float3, float3>>()
        {
            (float3 p) => 0.5f*p,
            (float3 p) => 0.5f*p + new float3(0.5f, 0.0f, 0.0f),
            (float3 p) => 0.5f*p + new float3(0.25f, 0.5f, 0.0f),
        });

        _Processor.CalculateForIterations(10);

        List<float3> B = new List<float3>();
        _Processor.GetData(ref B);

        _PlottingData = B;

        Debug.Log($"Number of points: {_PlottingData.Count}");

        _InternalPointDataTexture = new RenderTexture(_ExternPointDataTexture.width, _ExternPointDataTexture.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
        _InternalPointDataTexture.enableRandomWrite = true;
        _InternalPointDataTexture.Create();

        _PointDataBuffer = new ComputeBuffer(_PlottingData.Count, 3*sizeof(float));

        _PointDataBuffer.SetData(_PlottingData.ToArray());

        _KernelIndex = _DataCopyCS.FindKernel(_KernelName);
        _DataCopyCS.SetBuffer(_KernelIndex, "_Data", _PointDataBuffer);
        _DataCopyCS.SetTexture(_KernelIndex, "Result", _InternalPointDataTexture);
        _DataCopyCS.SetInt("width", _InternalPointDataTexture.width);
        _DataCopyCS.SetInt("arrayLenght", _PlottingData.Count);
        _DataCopyCS.SetFloat("maxValue", half.MaxValue);
        _DataCopyCS.Dispatch(_KernelIndex, _InternalPointDataTexture.width / 8, _InternalPointDataTexture.height /8, 1);   //Pray they are mod 8

        Graphics.Blit(_InternalPointDataTexture, _ExternPointDataTexture);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        //if (_PlottingData != null)
        //{
        //    foreach(float3 p in _PlottingData)
        //    {
        //        Gizmos.DrawWireSphere(p, _PointSize);
        //    }
        //}
    }

}
