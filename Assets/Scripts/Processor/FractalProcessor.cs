using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Unity.Mathematics;

public class FractalProcessor
{
    private List<float2> _A;
    private List<Func<float2, float2>> _IFS;
    private bool _Init;

    public FractalProcessor(List<float2> initialSet)
    {
        if (initialSet.Count > 0)
            _A = initialSet;
        else
            throw new Exception("The initial set should have at least one point.");

        _Init = false;
    }

    public void SetFunctions(List<Func<float2, float2>> functions)
    {
        if(functions.Count > 0)
        {
            _IFS = functions;
            _Init = true;
        }
        else
            throw new Exception("There should be at least one function.");
    }

    public void CalculateForIterations(int n, bool randomSampling = false)
    {
        if (_Init)
        {
            for(int i = 0; i < n; i++)
            {
                ApplySystem(randomSampling);
            }
        }
    }

    public void GetData(ref List<float2> data)
    {
        //! This could cause problems but this array will be too big and will probabli be problematic to copy it
        data = _A;
    }

    private void ApplySystem(bool random = false)
    {
        List<float2> T = new List<float2>();
        if (!random)
        {
            foreach(Func<float2, float2> f in _IFS)
            {
                for(int i = 0; i < _A.Count; i++)
                {
                    T.Add(f(_A[i]));
                }
            }
        }
        else
        {
            throw new NotImplementedException("Random Sampling is not implemented yet");
        }

        _A = T;
    }
}
