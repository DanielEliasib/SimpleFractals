using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Unity.Mathematics;

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class FractalProcessor
{
    private List<float3> _A;
    private List<Func<float3, float3>> _IFS;
    private float[] _Probs;
    private bool _Init;
    private int _MaxPoints;

    public FractalProcessor(List<float3> initialSet, int maxPoints)
    {
        if (initialSet.Count > 0)
            _A = initialSet;
        else
            throw new Exception("The initial set should have at least one point.");

        _MaxPoints = maxPoints;

        _Init = false;
    }

    public void SetFunctions(List<Func<float3, float3>> functions)
    {
        if(functions.Count > 0)
        {
            _IFS = functions;
            _Init = true;
        }
        else
            throw new Exception("There should be at least one function.");
    }

    public void SetProbabilities(float[] probs)
    {
        _Probs = probs;
    }

    public void CalculateForIterations(int n, bool useProbabilities = false, bool randomSampling = false)
    {
        if (_Init)
        {
            for (int i = 0; i < n; i++)
            {
                ApplySystem(useProbabilities, randomSampling);
            }
        }
    }

    public void GetData(ref List<float3> data)
    {
        //! This could cause problems but this array will be too big and will probabli be problematic to copy it
        data = _A;
    }

    private void ApplySystem(bool useProbabilities, bool random = false)
    {
        List<float3> T = new List<float3>();

        if (useProbabilities && _Probs.Length != _IFS.Count)
            throw new Exception("There must be a probabilitie for every function");

        bool terminate = false;

        if (!random)
        {
            int k = 0;
            foreach(Func<float3, float3> f in _IFS)
            {
                for(int i = 0; i < _A.Count; i++)
                {
                    if (useProbabilities)
                    {
                        if (UnityEngine.Random.value <= _Probs[k])
                            T.Add(f(_A[i]));
                    }
                    else
                    {
                        T.Add(f(_A[i]));
                    }

                    if (T.Count + _A.Count >= _MaxPoints)
                    {
                        terminate = true;
                        break;
                    }
                        
                }
                k++;

                if (terminate)
                    break;
            }
        }
        else
        {
            throw new NotImplementedException("Random Sampling is not implemented yet");
        }

        if (!useProbabilities)
            _A = T;
        else
            _A.AddRange(T);
    }

    private void ApplyParallelSystem(bool useProbabilities, bool random = false)
    {
        ConcurrentBag<float3> fractalBag = new ConcurrentBag<float3>();

        Parallel.ForEach(_IFS, f =>
        {
            for (int i = 0; i < _A.Count; i++)
            {

                fractalBag.Add(f(_A[i]));
            }
        });

        _A = new List<float3>(fractalBag);
    }
}
