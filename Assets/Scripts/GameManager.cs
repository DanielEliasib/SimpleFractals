using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;

public class GameManager : MonoBehaviour
{
    private FractalProcessor _Processor;
    private List<float2> _PlottingData;
    [SerializeField] float _PointSize = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        List<float2> A = new List<float2>();

        A.Add(float2.zero);
        A.Add(new float2(0.0f, 0.5f));
        A.Add(new float2(0.5f, 0.5f));
        A.Add(new float2(0.5f, 0.0f));

        _Processor = new FractalProcessor(A);

        //_Processor.SetFunctions(new List<System.Func<float2, float2>>()
        //{
        //    (float2 p) => 0.5f*p,
        //    (float2 p) => 0.5f*p + new float2(0.5f, 0.0f),
        //    (float2 p) => 0.5f*p + new float2(0.25f, 0.5f),
        //});

        _Processor.SetFunctions(new List<System.Func<float2, float2>>()
        {
            (float2 p) => 0.5f*p,
            (float2 p) => 0.5f*p + new float2(0.5f, 0.0f),
            (float2 p) => 0.5f*p + new float2(0.25f, 0.5f),
        });

        _Processor.CalculateForIterations(7);

        List<float2> B = new List<float2>();
        _Processor.GetData(ref B);

        _PlottingData = B;

        Debug.Log($"Number of points: {_PlottingData.Count}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (_PlottingData != null)
        {
            foreach(float2 p in _PlottingData)
            {
                Gizmos.DrawWireSphere(new Vector3(p.x, p.y, 0.0f), _PointSize);
            }
        }
    }
}
