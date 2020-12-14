using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;

public class GameManager : MonoBehaviour
{
    private List<float3> _PlottingData;
    [SerializeField, Range(0, 100)] float _FractalScale = 10f;

    [SerializeField] private RenderTexture _ExternPointDataTexture;
    private RenderTexture _InternalPointDataTexture;

    private ComputeBuffer _PointDataBuffer;

    [SerializeField]private ComputeShader _DataCopyCS;

    private int _KernelIndex;
    private string _KernelName = "CopyKernel";

    private FractalData[] _Examples;

    private FractalData _CurrentFractal;

    [SerializeField] private UnityEngine.UI.Slider _Slider;
    [SerializeField] private UnityEngine.VFX.VisualEffect _ExactEffect;

    // Start is called before the first frame update
    void Start()
    {
        _Examples = new FractalData[]
        {
            FractalExamples.KochCurve,
            FractalExamples.KochSnowFlake,
            FractalExamples.MapleLeaf,
            FractalExamples.Tree1,
        };

        _PlottingData = new List<float3>();

        _CurrentFractal = _Examples[1];
        SetFractal(1);

        InitializeTextures();
        InitializeCS();
        ComputeFractal();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        // Debug.Log($"Time spent copying: {watch.Elapsed.TotalMilliseconds}");
    }

    private void CopyData()
    {
        SetCSData(_FractalScale);
        _DataCopyCS.Dispatch(_KernelIndex, _InternalPointDataTexture.width / 8, _InternalPointDataTexture.height / 8, 1);   //Pray they are mod 8

        Graphics.Blit(_InternalPointDataTexture, _ExternPointDataTexture);
    }

    private void GenerateFractal(List<float3> initialSet, int iterations, List<System.Func<float3, float3>> ifs, float[] probabilities, bool useProbabilities, ref List<float3> fractal)
    {
        var _Processor = new FractalProcessor(initialSet, _ExternPointDataTexture.width * _ExternPointDataTexture.height);

        _Processor.SetFunctions(ifs);
        _Processor.SetProbabilities(probabilities);

        _Processor.CalculateForIterations(iterations, useProbabilities);

        _Processor.GetData(ref fractal);
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
            _KernelIndex = _DataCopyCS.FindKernel(_KernelName);

            _DataCopyCS.SetTexture(_KernelIndex, "Result", _InternalPointDataTexture);

            _DataCopyCS.SetInt("width", _InternalPointDataTexture.width);
            

            _DataCopyCS.SetFloat("maxValue", half.MaxValue);
        }
    }

    private void SetCSData(float scale)
    {
        try
        {
            _PointDataBuffer.Dispose();
        }
        catch { }
        _PointDataBuffer = new ComputeBuffer(_PlottingData.Count, 3 * sizeof(float));
        _DataCopyCS.SetBuffer(_KernelIndex, "_Data", _PointDataBuffer);
        _DataCopyCS.SetInt("arrayLenght", _PlottingData.Count);

        _PointDataBuffer.SetData(_PlottingData.ToArray());
        _DataCopyCS.SetFloat("scale", scale);
    }

    public void SetScale(float scale)
    {
        
        _FractalScale = scale;
    }

    public void SetFractal(int val)
    {
        var current = _CurrentFractal;
        try
        {
            current = _Examples[val];
        }
        catch {}

        _CurrentFractal = current;
        _FractalScale = _CurrentFractal.Scale;

        _Slider.value = _FractalScale;
    }

    public void ComputeFractal()
    {
        _PlottingData = new List<float3>();

        GenerateFractal(
            new List<float3>() { float3.zero }, 
            _CurrentFractal.Iterations, 
            _CurrentFractal.IFS, 
            _CurrentFractal.Probabilities, 
            _CurrentFractal.Probabilities == null? false : true, 
            ref _PlottingData);

        CopyData();

        _ExactEffect.Reinit();
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

public static class FractalExamples
{
    //! This is a horrible way of doing this, but i don't got too much time so this is it for now
    private static Matrix4x4 kMat1 = new Matrix4x4(
            new Vector4(1.0f / 3.0f, 0.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 1.0f / 3.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 0.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
            );

    private static Matrix4x4 kMat2 = new Matrix4x4(
            new Vector4(1.0f / 6.0f, Mathf.Sqrt(3.0f) / 6.0f, 0.0f, 0.0f),
            new Vector4(-Mathf.Sqrt(3.0f) / 6.0f, 1.0f / 6.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 0.0f, 0.0f, 0.0f),
            new Vector4(1.0f / 3.0f, 0.0f, 0.0f, 1.0f)
            );

    private static Matrix4x4 kMat3 = new Matrix4x4(
            new Vector4(1.0f / 6.0f, -Mathf.Sqrt(3.0f) / 6.0f, 0.0f, 0.0f),
            new Vector4(Mathf.Sqrt(3.0f) / 6.0f, 1.0f / 6.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 0.0f, 0.0f, 0.0f),
            new Vector4(1.0f / 2.0f, Mathf.Sqrt(3.0f) / 6.0f, 0.0f, 1.0f)
            );

    private static Matrix4x4 kMat4 = new Matrix4x4(
            new Vector4(1.0f / 3.0f, 0.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 1.0f / 3.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 0.0f, 0.0f, 0.0f),
            new Vector4(2.0f / 3.0f, 0.0f, 0.0f, 1.0f)
        );

    public static FractalData KochCurve = new FractalData(10, new List<float3>() { float3.zero }, null, new List<System.Func<float3, float3>>()
        {
            (float3 p) => kMat1.MultiplyPoint(p),
            (float3 p) => kMat2.MultiplyPoint(p),
            (float3 p) => kMat3.MultiplyPoint(p),
            (float3 p) => kMat4.MultiplyPoint(p),
        }, 7);

    private static Matrix4x4 ksMat1 = new Matrix4x4(
            new Vector4(1.0f / 2.0f, Mathf.Sqrt(3.0f) / 6.0f, 0.0f, 0.0f),
            new Vector4(-Mathf.Sqrt(3.0f) / 6.0f, 1.0f / 2.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 0.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
            );

    private static Matrix4x4 ksMat2 = new Matrix4x4(
            new Vector4(1.0f / 3.0f, 0.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 1.0f / 3.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 0.0f, 0.0f, 0.0f),
            new Vector4(1.0f / Mathf.Sqrt(3.0f), 1.0f / 3.0f, 0.0f, 1.0f)
            );

    private static Matrix4x4 ksMat3 = new Matrix4x4(
            new Vector4(1.0f / 3.0f, 0.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 1.0f / 3.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 0.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 2.0f / 3.0f, 0.0f, 1.0f)
            );

    private static Matrix4x4 ksMat4 = new Matrix4x4(
            new Vector4(1.0f / 3.0f, 0.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 1.0f / 3.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 0.0f, 0.0f, 0.0f),
            new Vector4(-1.0f / Mathf.Sqrt(3.0f), 1.0f/3.0f, 0.0f, 1.0f)
            );

    private static Matrix4x4 ksMat5 = new Matrix4x4(
            new Vector4(1.0f / 3.0f, 0.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 1.0f / 3.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 0.0f, 0.0f, 0.0f),
            new Vector4(-1.0f / Mathf.Sqrt(3.0f), -1.0f / 3.0f, 0.0f, 1.0f)
            );

    private static Matrix4x4 ksMat6 = new Matrix4x4(
            new Vector4(1.0f / 3.0f, 0.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 1.0f / 3.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 0.0f, 0.0f, 0.0f),
            new Vector4(0.0f, -2.0f / 3.0f, 0.0f, 1.0f)
            );

    private static Matrix4x4 ksMat7 = new Matrix4x4(
            new Vector4(1.0f / 3.0f, 0.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 1.0f / 3.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 0.0f, 0.0f, 0.0f),
            new Vector4(1.0f / Mathf.Sqrt(3.0f), -1.0f / 3.0f, 0.0f, 1.0f)
            );

    public static FractalData KochSnowFlake = new FractalData(7, new List<float3>() { float3.zero }, null, new List<System.Func<float3, float3>>()
        {
            (float3 p) => ksMat1.MultiplyPoint(p),
            (float3 p) => ksMat2.MultiplyPoint(p),
            (float3 p) => ksMat3.MultiplyPoint(p),
            (float3 p) => ksMat4.MultiplyPoint(p),
            (float3 p) => ksMat5.MultiplyPoint(p),
            (float3 p) => ksMat6.MultiplyPoint(p),
            (float3 p) => ksMat7.MultiplyPoint(p),
        }, 3);

    private static Matrix4x4 t1Mat1 = new Matrix4x4(
            new Vector4(0.01f, 0.00f, 0.00f, 0.00f),
            new Vector4(0.00f, 0.45f, 0.00f, 0.00f),
            new Vector4(0.00f, 0.00f, 0.00f, 0.00f),
            new Vector4(0.00f, 0.00f, 0.00f, 1.00f)
            );

    private static Matrix4x4 t1Mat2 = new Matrix4x4(
            new Vector4(-0.01f, 0.00f, 0.00f, 0.00f),
            new Vector4(0.00f, -0.45f, 0.00f, 0.00f),
            new Vector4(0.00f, 0.00f, 0.00f, 0.00f),
            new Vector4(0.00f, 0.40f, 0.00f, 1.00f)
            );

    private static Matrix4x4 t1Mat3 = new Matrix4x4(
            new Vector4(0.42f, 0.42f, 0.00f, 0.00f),
            new Vector4(-0.42f, 0.42f, 0.00f, 0.00f),
            new Vector4(0.00f, 0.00f, 0.00f, 0.00f),
            new Vector4(0.00f, 0.40f, 0.00f, 1.00f)
            );

    private static Matrix4x4 t1Mat4 = new Matrix4x4(
            new Vector4(0.42f, -0.42f, 0.00f, 0.00f),
            new Vector4(0.42f, 0.42f, 0.00f, 0.00f),
            new Vector4(0.00f, 0.00f, 0.00f, 0.00f),
            new Vector4(0.00f, 0.40f, 0.00f, 1.00f)
            );

    public static FractalData Tree1 = new FractalData(10, new List<float3>() { float3.zero}, null, new List<System.Func<float3, float3>>()
        {
            (float3 p) => t1Mat1.MultiplyPoint(p),
            (float3 p) => t1Mat2.MultiplyPoint(p),
            (float3 p) => t1Mat3.MultiplyPoint(p),
            (float3 p) => t1Mat4.MultiplyPoint(p),
        }, 5);

    private static Matrix4x4 mtMat1 = new Matrix4x4(
            new Vector4(0.14f, 0.00f, 0.00f, 0.00f),
            new Vector4(0.01f, 0.51f, 0.00f, 0.00f),
            new Vector4(0.00f, 0.00f, 0.00f, 0.00f),
            new Vector4(-0.08f, -1.31f, 0.00f, 1.00f)
            );

    private static Matrix4x4 mtMat2 = new Matrix4x4(
            new Vector4(0.43f, -0.45f, 0.00f, 0.00f),
            new Vector4(0.52f, 0.50f, 0.00f, 0.00f),
            new Vector4(0.00f, 0.00f, 0.00f, 0.00f),
            new Vector4(1.49f, -0.75f, 0.00f, 1.00f)
            );

    private static Matrix4x4 mtMat3 = new Matrix4x4(
            new Vector4(0.45f, 0.47f, 0.00f, 0.00f),
            new Vector4(-0.49f, 0.47f, 0.00f, 0.00f),
            new Vector4(0.00f, 0.00f, 0.00f, 0.00f),
            new Vector4(-1.62f, -0.74f, 0.00f, 1.00f)
            );

    private static Matrix4x4 mtMat4 = new Matrix4x4(
            new Vector4(0.49f, 0.00f, 0.00f, 0.00f),
            new Vector4(0.00f, 0.51f, 0.00f, 0.00f),
            new Vector4(0.00f, 0.00f, 0.00f, 0.00f),
            new Vector4(0.02f, 1.62f, 0.00f, 1.00f)
            );

    public static FractalData MapleLeaf = new FractalData(10, new List<float3>() { float3.zero }, null, new List<System.Func<float3, float3>>()
        {
            (float3 p) => mtMat1.MultiplyPoint(p),
            (float3 p) => mtMat2.MultiplyPoint(p),
            (float3 p) => mtMat3.MultiplyPoint(p),
            (float3 p) => mtMat4.MultiplyPoint(p),
        }, 1);
}

public class FractalData{
    public int Iterations { get; private set; }
    public List<float3> InitialSet { get; private set; }
    public float[] Probabilities { get; private set; }
    public List<System.Func<float3, float3>> IFS { get; private set; }
    public int Scale { get; private set; }

    public FractalData(int iterations, List<float3> initialSet, float[] probabilities, List<System.Func<float3, float3>> ifs, int scale)
    {
        Iterations = iterations;
        InitialSet = initialSet;
        Probabilities = probabilities;
        IFS = ifs;

        Scale = scale;
    }
}