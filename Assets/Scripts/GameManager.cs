using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;

public class GameManager : MonoBehaviour
{
    private FractalProcessor _Processor;
    private List<float3> _PlottingData;
    [SerializeField, Range(0, 100)] float _FractalScale = 10f;

    [SerializeField] private RenderTexture _ExternPointDataTexture;
    private RenderTexture _InternalPointDataTexture;

    private ComputeBuffer _PointDataBuffer;

    [SerializeField]private ComputeShader _DataCopyCS;

    private int _KernelIndex;
    private string _KernelName = "CopyKernel";

    private System.Diagnostics.Stopwatch watch;

    // Start is called before the first frame update
    void Start()
    {
        watch = new System.Diagnostics.Stopwatch();

        //_Processor.SetFunctions(new List<System.Func<float3, float3>>()
        //{
        //    (float3 p) => 0.5f*p,
        //    (float3 p) => 0.5f*p + new float3(0.5f, 0.0f, 0.0f),
        //    (float3 p) => 0.5f*p + new float3(0.25f, 0.5f, 0.0f),
        //});

        //_Processor.SetFunctions(new List<System.Func<float3, float3>>()
        //{
        //    (float3 p) => new float3(0.14f*p.x + 0.01f*p.y - 0.08f, 0.51f*p.y - 1.31f, 0.0f),
        //    (float3 p) => new float3(0.43f*p.x + 0.52f*p.y + 1.49f, -0.45f*p.x + 0.5f*p.y - 0.75f, 0.0f),
        //    (float3 p) => new float3(0.45f*p.x - 0.49f*p.y - 1.62f, 0.47f*p.x + 0.47f*p.y - 0.74f, 0.0f),
        //    (float3 p) => new float3(0.49f*p.x + 0.02f, 0.51f*p.y + 1.62f, 0.0f),
        //});

        watch.Reset();
        watch.Start();

        float[] par1 = new float[] { 0.0f, 0.0f, 0.0f, 0.16f, 0.0f, 0.0f };
        float[] par2 = new float[] { 0.2f, -0.26f, 0.23f, 0.22f, 0.0f, 1.6f };
        float[] par3 = new float[] { -0.15f, 0.28f, 0.26f, 0.24f, 0.44f, 0.0f };
        float[] par4 = new float[] { 0.85f, 0.04f, -0.04f, 0.85f, 0.0f, 1.6f };

        var ifs = new List<System.Func<float3, float3>>()
        {
            (float3 p) => new float3(par1[0]*p.x + par1[1]*p.y + par1[4],par1[2]*p.x + par1[3]*p.y + par1[4] , 0.0f),
            (float3 p) => new float3(par2[0]*p.x + par2[1]*p.y + par2[4],par2[2]*p.x + par2[3]*p.y + par2[4] , 0.0f),
            (float3 p) => new float3(par3[0]*p.x + par3[1]*p.y + par3[4],par3[2]*p.x + par3[3]*p.y + par3[4] , 0.0f),
            (float3 p) => new float3(par4[0]*p.x + par4[1]*p.y + par4[4],par4[2]*p.x + par4[3]*p.y + par4[4] , 0.0f)
        };

        GenerateFractal(new List<float3>() { float3.zero }, 10, ifs, out _PlottingData);

        watch.Stop();

        Debug.Log($"Number of points: {_PlottingData.Count} with time: {watch.ElapsedMilliseconds}");

        InitializeTextures();
        InitializeCS();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        watch.Reset();
        watch.Start();

        SetCSData(_FractalScale);
        _DataCopyCS.Dispatch(_KernelIndex, _InternalPointDataTexture.width / 8, _InternalPointDataTexture.height / 8, 1);   //Pray they are mod 8

        Graphics.Blit(_InternalPointDataTexture, _ExternPointDataTexture);

        watch.Stop();
        Debug.Log($"Time spent copying: {watch.Elapsed.TotalMilliseconds}");
    }

    private void GenerateFractal(List<float3> initialSet, int iterations, List<System.Func<float3, float3>> ifs, out List<float3> fractal)
    {
        var _Processor = new FractalProcessor(initialSet);

        _Processor.SetFunctions(ifs);

        _Processor.CalculateForIterations(iterations);

        List<float3> B = new List<float3>();
        _Processor.GetData(ref B);
        fractal = B;
    }

    private void InitializeTextures()
    {
        _InternalPointDataTexture = new RenderTexture(_ExternPointDataTexture.width, _ExternPointDataTexture.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
        _InternalPointDataTexture.enableRandomWrite = true;
        _InternalPointDataTexture.Create();
    }

    private void InitializeCS()
    {
        if(_PlottingData.Count <= _ExternPointDataTexture.width* _ExternPointDataTexture.height)
        {
            _PointDataBuffer = new ComputeBuffer(_PlottingData.Count, 3 * sizeof(float));
            _KernelIndex = _DataCopyCS.FindKernel(_KernelName);

            _DataCopyCS.SetBuffer(_KernelIndex, "_Data", _PointDataBuffer);
            _DataCopyCS.SetTexture(_KernelIndex, "Result", _InternalPointDataTexture);

            _DataCopyCS.SetInt("width", _InternalPointDataTexture.width);
            _DataCopyCS.SetInt("arrayLenght", _PlottingData.Count);

            _DataCopyCS.SetFloat("maxValue", half.MaxValue);
        }
    }

    private void SetCSData(float scale)
    {
        _PointDataBuffer.SetData(_PlottingData.ToArray());
        _DataCopyCS.SetFloat("scale", scale);
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
