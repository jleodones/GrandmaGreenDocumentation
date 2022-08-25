using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.Mathematics;

namespace GrandmaGreen
{
    public interface IPathfinderServicer : IServicer<Pathfinder>
    {

    }

    public interface IPathAgent : IServiceUser<Pathfinder, IPathfinderServicer>
    {
        float3[] RequestPath(int2 startPos, int2 endPos);

        void FollowPath(float3[] path);

        bool isPathing { get; }

        Vector3 CurrentPos();
    }
}