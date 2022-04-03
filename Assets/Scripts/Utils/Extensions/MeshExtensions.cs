using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class MeshExtensions
{
    #region data structures
    /* Data structures */

    public static Vector3[] GetMeshBoundPoints(this Mesh mesh)
    {
        Vector3[] meshAABBcorners = new Vector3[]
        {
            mesh.bounds.min,
            new Vector3(mesh.bounds.min.x, mesh.bounds.max.y, mesh.bounds.max.z),
            new Vector3(mesh.bounds.max.x, mesh.bounds.min.y, mesh.bounds.max.z),
            new Vector3(mesh.bounds.max.x, mesh.bounds.max.y, mesh.bounds.min.z),
            new Vector3(mesh.bounds.min.x, mesh.bounds.min.y, mesh.bounds.max.z),
            new Vector3(mesh.bounds.min.x, mesh.bounds.max.y, mesh.bounds.min.z),
            new Vector3(mesh.bounds.max.x, mesh.bounds.min.y, mesh.bounds.min.z),
            mesh.bounds.max
        };

        return meshAABBcorners;
    }
    #endregion

    #region vertex searches
    /* Vertex Searches */

    public static Vector3 NearestVertexToLocalPoint(this Mesh mesh, Vector3 localPoint)
    {
        Vector3 nearestPoint = Vector3.zero;
        float nearestDist = float.MaxValue;
        foreach(Vector3 vert in mesh.vertices)
        {
            float distToVert = (vert - localPoint).sqrMagnitude;
            if(distToVert < nearestDist)
            {
                distToVert = nearestDist;
                nearestPoint = localPoint;
            }
        }

        Debug.Assert(nearestPoint != Vector3.zero);

        return nearestPoint;
    }

    public static Vector3 FurthestVertexToLocalPoint(this Mesh mesh, Vector3 localPoint)
    {
        Vector3 furthestPoint = Vector3.zero;
        float furthestDist = float.MinValue;
        foreach(Vector3 vert in mesh.vertices)
        {
            float distToVert = (vert - localPoint).sqrMagnitude;
            if(distToVert > furthestDist)
            {
                distToVert = furthestDist;
                furthestPoint = localPoint;
            }
        }

        Debug.Assert(furthestPoint != Vector3.zero);

        return furthestPoint;
    }

    public static Vector3 FurthestBoundToLocalPoint(this Mesh mesh, Vector3 localPoint)
    {
        Vector3 furthestBound = Vector3.zero;
        float furthestDist = float.MinValue;
        foreach(Vector3 p in mesh.GetMeshBoundPoints())
        {
            float d = (p - localPoint).sqrMagnitude;
            if(d > furthestDist)
            {
                furthestDist = d;
                furthestBound = p;
            }
        }

        return furthestBound;
    }

    public static Vector3 NearestBoundToLocalPoint(this Mesh mesh, Vector3 localPoint)
    {
        Vector3 nearestBound = Vector3.zero;
        float nearestDist = float.MaxValue;
        foreach(Vector3 p in mesh.GetMeshBoundPoints())
        {
            float d = (p - localPoint).sqrMagnitude;
            if(d < nearestDist)
            {
                nearestDist = d;
                nearestBound = p;
            }
        }

        return nearestBound;
    }
    #endregion

    #region Modifications
	/// <summary>
	/// Returns a version of the marched mesh which has the vertices merged.
	/// </summary>
	/// <returns></returns>
	public static void MergeMesh(this Mesh mesh)
	{
		Dictionary<Vector3, int> meshVerts = new Dictionary<Vector3, int>();
		int[] mapping = new int[mesh.vertices.Length]; // max number of vertices.
		int vCount = 0;
		int tCount = 0;
        foreach(Vector3 v in mesh.vertices)
        {
            if (!meshVerts.ContainsKey(v))
            {
                meshVerts.Add(v, tCount);
                mapping[vCount] = tCount;
                tCount++;
            }
            else
            {
                mapping[vCount] = meshVerts[v];
            }
            vCount++;
        }

		//vCount = 0;
        //List<int> meshTris = new List<int>();
        int[] meshTris = mesh.triangles;
        for(int t = 0; t < meshTris.Length; ++t)
        {
            //meshTris.Add(mapping[vCount + 2]);
            //meshTris.Add(mapping[vCount + 1]);
            //meshTris.Add(mapping[vCount]);
            meshTris[t] = mapping[meshTris[t]];
            //vCount += 3;
        }

		List<Vector3> mVerts = new List<Vector3>();
		foreach (Vector3 v in meshVerts.Keys)
			mVerts.Add(v);

        mesh.Clear();
        mesh.SetVertices(mVerts);
        mesh.SetTriangles(meshTris, 0);
        mesh.RecalculateNormals();
		mesh.RecalculateBounds();

	}
    #endregion
}
