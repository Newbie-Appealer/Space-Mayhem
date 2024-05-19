using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public enum NavMeshType
{
    OUTSIDE,
    INSIDE
}

public class NavMeshController : MonoBehaviour
{
    [SerializeField] NavMeshSurface[] _mapMeshSurfaces;

    public void F_NavMeshBake(NavMeshType v_type)
    {
        NavMeshSurface target = _mapMeshSurfaces[(int)v_type];

        target.collectObjects = CollectObjects.Children;
        target.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
        target.BuildNavMesh();
    }
}
