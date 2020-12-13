using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public struct IFSProcessorJOB : IJobParallelFor
{
    public NativeArray<float3> initialSet;
    public NativeList<float3>.ParallelWriter resultSet;

    public R3Func f;

    public void Execute(int index)
    {
        resultSet.AddNoResize(f(initialSet[index]));
    }

    public delegate float3 R3Func(float3 x);
}
