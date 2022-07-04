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
        void FollowPath(float3[] path);

        bool isPathing { get; }

        Vector3 CurrentPos();
    }
}