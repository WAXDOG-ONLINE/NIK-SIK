using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;
using UnityEngine.AI;
using Unity.AI.Navigation;
[RequireComponent(typeof(SplineSampler))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode()]
public class SplineRoad : MonoBehaviour
{
    public NavMeshSurface navMesh;
    [SerializeField]
    private float wallHeight = 4;
    [SerializeField]
    private SplineSampler m_splineSampler;
    [SerializeField]
    private MeshFilter m_meshFilter;
    
    private MeshRenderer m_meshRenderer;
    
    private List<Vector3> m_vertsP1;
    private List<Vector3> m_vertsP2;

    [SerializeField, Min(5)]
    private int resolution = 10;
    [SerializeField, Range(0.1f, 100f)]
    private float tilingLength = 2;
    [SerializeField, Range(0.01f,1f)]
    private float m_curveStep = 0.1f;
    [SerializeField]
    private float m_width;

    [SerializeField]
    private List<Intersection> intersections;

    public List<Intersection> Intersections => intersections;

    //Draw mesh
    private void Awake()
    {
        
        
        m_meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
        GetSplineVerts();
    }

    private void OnSplineChanged(Spline arg1, int arg2, SplineModification arg3)
    {
        Rebuild();
    }

    public void Rebuild()
    {
        GetSplineVerts();
        BuildMesh();
    }

    private void OnValidate()
    {
        Rebuild();
    }

    private void BuildMesh()
    {
        Mesh m = new Mesh();

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        int offset = 0;

        int length = m_vertsP2.Count;

        
        float uvOffset = 0f;

        for (int currentSplineIndex = 0; currentSplineIndex < m_splineSampler.NumSplines; currentSplineIndex++)
        {
            int splineOffset = resolution * currentSplineIndex;
            splineOffset += currentSplineIndex;
            //Iterate verts and build a face
            for (int currentSplinePoint = 1; currentSplinePoint < resolution+1; currentSplinePoint++)
            {
                //get vertices for spline floor faces
                int vertoffset = splineOffset + currentSplinePoint;
                Vector3 p1 = m_vertsP1[vertoffset - 1];
                Vector3 p2 = m_vertsP2[vertoffset - 1];
                Vector3 p3 = m_vertsP1[vertoffset];
                Vector3 p4 = m_vertsP2[vertoffset];


                //make vertices for spline wall faces
                Vector3 p5h = m_vertsP1[vertoffset - 1];
                p5h = new Vector3(p5h.x,p5h.y + wallHeight,p5h.z);
                
                Vector3 p6h = m_vertsP2[vertoffset - 1];
                p6h = new Vector3(p6h.x,p6h.y + wallHeight,p6h.z);
                
                Vector3 p7h = m_vertsP1[vertoffset];
                p7h = new Vector3(p7h.x,p7h.y + wallHeight,p7h.z);
                
                Vector3 p8h = m_vertsP2[vertoffset];
                p8h = new Vector3(p8h.x,p8h.y + wallHeight,p8h.z);
        

                int num_verts = 12;

                offset = num_verts * resolution * currentSplineIndex;
                offset += num_verts * (currentSplinePoint - 1);
                // we want 3 quads, so 4 points for each quad
                // and then we want and each quad is split into two triangles
                // the quads share the same points, but seperate quads dont share
                // because we need seperate UVs for each quad
                verts.AddRange(new List<Vector3> { 
                  // left wall
                  p1, p3, p5h, p7h, // 0, 1, 2, 3

                  // floor
                  p1, p2, p3, p4, // 4, 5, 6, 7

                  // right wall
                  p2, p4, p6h, p8h, // 8, 9, 10, 11

                });

                //clock wise order ( when looking at the wall from inside the hallway)
                List<int> segment_triangles = new List<int> {
                  // Left wall
                  2, 1, 0,
                  2, 3, 1,

                  // Floor
                  4, 7, 5,
                  4, 6, 7,

                  // right wall
                  8, 9, 10,
                  9, 11, 10,
                };

                // add the offset to all the triangles
                for (int i = 0; i < segment_triangles.Count; i++)
                {
                  segment_triangles[i] += offset;
                }
                tris.AddRange(segment_triangles);

                float distance = Vector3.Distance(p1, p3) / tilingLength;
                List<Vector2> uv_list = new List<Vector2> {
                  // left wall
                  new Vector2(0, 0), // p1
                  new Vector2(1, 0), // p2
                  new Vector2(0, 1), // p3
                  new Vector2(1, 1), // p4

                  // floor
                  new Vector2(0, 1), // 
                  new Vector2(1, 1), // p4
                  new Vector2(0, 0), // p1
                  new Vector2(1, 0), // p2

                  // right wall
                  new Vector2(0, 0), // p1
                  new Vector2(1, 0), // p2
                  new Vector2(0, 1), // p3
                  new Vector2(1, 1), // p4
                };

                // multiply by the distance and then add the offset
                // but just to the u value
                for (int i = 0; i < uv_list.Count; i++)
                {
                  uv_list[i] = new Vector2(uv_list[i].x  * distance + uvOffset, uv_list[i].y);
                }

                uvs.AddRange(uv_list);

                uvOffset += distance;
            }
        }

        List<int> trisB = new List<int>();

        int numVerts = verts.Count;
        GetIntersectionVerts(verts, trisB, uvs);

        m.subMeshCount = 2;

        m.SetVertices(verts);

        m.SetTriangles(tris, 0);
        // m.SetTriangles(trisB, 1);

        m.SetUVs(0, uvs);

        m.SetNormals(normals);


       /*
        
        m.SetUVs(1, uvsB);
*/


        m_meshFilter.mesh = m;
        
        navMesh.BuildNavMesh();
    }

    private void GetSplineVerts()
    {
        m_vertsP1 = new List<Vector3>();
        m_vertsP2 = new List<Vector3>();

        float step = 1f / (float)resolution;
        Vector3 p1;
        Vector3 p2;
        for (int j = 0; j < m_splineSampler.NumSplines; j++)
        {
            for (int i = 0; i < resolution; i++)
            {
                float t = step * i;
                m_splineSampler.SampleSplineWidth(j, t, m_width, out p1, out p2);
                m_vertsP1.Add(p1);
                m_vertsP2.Add(p2);
            }

            m_splineSampler.SampleSplineWidth(j, 1f, m_width, out p1, out p2);
            m_vertsP1.Add(p1);
            m_vertsP2.Add(p2);
        }
    }

    struct JunctionEdge
    {
        public Vector3 left;
        public Vector3 right;

        public Vector3 Center => (left + right)/2;

        public JunctionEdge (Vector3 p1, Vector3 p2)
        {
            this.left = p1;
            this.right = p2;
        }
    }

    private void GetIntersectionVerts(List<Vector3> verts, List<int> tris, List<Vector2> uvs)
    {
        int offset = verts.Count;

        //Get intersection verts
        for (int i = 0; i < intersections.Count; i++)
        {
            Intersection intersection = intersections[i];
            int count = 0;
            //List<Vector3> points = new List<Vector3>();
            
            List<JunctionEdge> junctionEdges = new List<JunctionEdge>();

            Vector3 center = new Vector3();
            foreach (JunctionInfo junction in intersection.GetJunctions())
            {
                int splineIndex = junction.splineIndex;
                float t = junction.knotIndex == 0 ? 0f : 1f;
                m_splineSampler.SampleSplineWidth(splineIndex, t, m_width, out Vector3 p1, out Vector3 p2);
                //If knot index is 0 we're facing away from the junction
                //If we're more than zero we're facing the junction
                if (junction.knotIndex == 0)
                {
                    junctionEdges.Add(new JunctionEdge(p1, p2));
                }
                else
                {
                    junctionEdges.Add(new JunctionEdge(p2, p1));

                }
                center += p1;
                center += p2;
                count++;
            }
            center = center / (junctionEdges.Count * 2);
            

            //Sort the junctions based on their direction from the center
            junctionEdges.Sort((x, y) => SortPoints(center, x.Center, y.Center));


            List<Vector3> curvePoints = new List<Vector3>();
            //Add additional points
            Vector3 mid;
            Vector3 c;
            Vector3 b;
            Vector3 a;
            BezierCurve curve;
            for (int j = 1; j <= junctionEdges.Count; j++)
            {
                a = junctionEdges[j - 1].left;
                curvePoints.Add(a);
                b = (j < junctionEdges.Count) ? junctionEdges[j].right : junctionEdges[0].right;
                mid = Vector3.Lerp(a, b, 0.5f);
                Vector3 dir = center - mid;
                mid = mid - dir;
                c = Vector3.Lerp(mid, center, intersection.curves[j-1]);

                curve = new BezierCurve(a, c, b);
                for (float t = 0f; t < 1f; t += m_curveStep)
                {
                    Vector3 pos = CurveUtility.EvaluatePosition(curve, t);
                    curvePoints.Add(pos);
                }

                curvePoints.Add(b);
            }

            curvePoints.Reverse();

            int pointsOffset = verts.Count;

            for (int j = 1; j <= curvePoints.Count; j++)
            {

                Vector3 pointA = curvePoints[j - 1];
                Vector3 pointB;
                if (j == curvePoints.Count)
                {
                    pointB = curvePoints[0];
                }
                else
                {
                    pointB = curvePoints[j];
                }
                
                verts.Add(center);
                verts.Add(pointA);
                verts.Add(pointB);

                tris.Add(pointsOffset + ((j - 1) * 3) + 0);
                tris.Add(pointsOffset + ((j - 1) * 3) + 1);
                tris.Add(pointsOffset + ((j - 1) * 3) + 2);

                uvs.Add(new Vector2(center.z, center.x));
                uvs.Add(new Vector2(pointA.z, pointA.x));
                uvs.Add(new Vector2(pointB.z, pointB.x));

            }




            /*
            //We need to make a new list to add them into
            List<Vector3> curvePoints = new List<Vector3>();

            //Add additional points
            Vector3 mid;
            Vector3 c;
            Vector3 b;
            Vector3 a;
            BezierCurve curve;
            for (int j = 2; j < points.Count; j += 2)
            {
                a = points[j - 1];
                b = points[j];
                mid = Vector3.Lerp(a, b, 0.5f);
                c = Vector3.Lerp(mid, center, 0.3f);

                curve = new BezierCurve(a, c, b);
                for (float t = 0f; t < 1f; t += m_curveStep)
                {
                    Vector3 pos = CurveUtility.EvaluatePosition(curve, t);
                    curvePoints.Add(pos);
                }
            }

            a = points[points.Count - 1];
            b = points[0];
            mid = Vector3.Lerp(a, b, 0.5f);
            c = Vector3.Lerp(mid, center, 0.3f);
            curve = new BezierCurve(a, c, b);
            for (float t = 0f; t < 1f; t += m_curveStep)
            {
                Vector3 pos = CurveUtility.EvaluatePosition(curve, t);
                curvePoints.Add(pos);
            }

            points.AddRange(curvePoints);

            //Sort the points based on their direction from the center
            points.Sort((x, y) => SortPoints(center, x, y));

            int pointsOffset = verts.Count;

            for (int j = 1; j <= points.Count; j++)
            {
                verts.Add(center);
                verts.Add(points[j - 1]);
                if (j == points.Count)
                {
                    verts.Add(points[0]);
                }
                else
                {
                    verts.Add(points[j]);
                }

                tris.Add(pointsOffset + ((j-1) * 3) + 0);
                tris.Add(pointsOffset + ((j-1) * 3) + 1);
                tris.Add(pointsOffset + ((j-1) * 3) + 2);

            }*/

        }
    }

    private int SortPoints(Vector3 center, Vector3 x, Vector3 y)
    {
        Vector3 xDir = x - center;
        Vector3 yDir = y - center;

        float angleA = Vector3.SignedAngle(center.normalized, xDir.normalized, Vector3.up);
        float angleB = Vector3.SignedAngle(center.normalized, yDir.normalized, Vector3.up);

        if (angleA > angleB)
        {
            return -1;
        }
        if (angleA < angleB)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private void OnDisable()
    {
        Spline.Changed -= OnSplineChanged;
    }
    private void OnDrawGizmosSelected()
    {  
        /*
        for (int i = 0; i < m_vertsP1.Count; i++)
        {
            Vector3 p1 = m_vertsP1[i];
            Vector3 p2 = m_vertsP2[i];

            Handles.matrix = transform.localToWorldMatrix;
            Handles.SphereHandleCap(0, p1, Quaternion.identity, .2f, EventType.Repaint);
            Handles.SphereHandleCap(0, p2, Quaternion.identity, .2f, EventType.Repaint);
        }

        for (int i = 0; i < intersections.Count; i++)
        {
            Intersection intersection = intersections[i];
            int count = 0;
            //List<Vector3> points = new List<Vector3>();

            List<JunctionEdge> junctionEdges = new List<JunctionEdge>();

            Vector3 center = new Vector3();
            foreach (JunctionInfo junction in intersection.GetJunctions())
            {
                int splineIndex = junction.splineIndex;
                float t = junction.knotIndex == 0 ? 0f : 1f;
                m_splineSampler.SampleSplineWidth(splineIndex, t, m_width, out Vector3 p1, out Vector3 p2);
                //If knot index is 0 we're facing away from the junction
                //If we're more than zero we're facing the junction
                if (junction.knotIndex == 0)
                {
                    junctionEdges.Add(new JunctionEdge(p1, p2));
                }
                else
                {
                    junctionEdges.Add(new JunctionEdge(p2, p1));

                }
                center += p1;
                center += p2;
                count++;
            }

            //Get the center of all the points 
            center = center / (junctionEdges.Count * 2);

            //Sort the junctions based on their direction from the center
            junctionEdges.Sort((x, y) => SortPoints(center, x.Center, y.Center));

            foreach(JunctionEdge edge in junctionEdges)
            {
                Handles.color = Color.yellow;
                Handles.SphereHandleCap(0, edge.Center, Quaternion.identity, 0.3f, EventType.Repaint);
                Handles.color = Color.red;
                Handles.SphereHandleCap(0, edge.left, Quaternion.identity, 0.3f, EventType.Repaint);
                Handles.color = Color.blue;
                Handles.SphereHandleCap(0, edge.right, Quaternion.identity, 0.3f, EventType.Repaint);
            }
            Handles.color = Color.white;

            List<Vector3> curvePoints = new List<Vector3>();
            //Add additional points
            Vector3 mid;
            Vector3 c;
            Vector3 b;
            Vector3 a;
            BezierCurve curve;
            for (int j = 1; j <= junctionEdges.Count; j ++)
            {
                a = junctionEdges[j-1].left;
                b = (j < junctionEdges.Count) ? junctionEdges[j].right : junctionEdges[0].right;
                mid = Vector3.Lerp(a, b, 0.5f);
                c = Vector3.Lerp(mid, center, 0.3f);

                curve = new BezierCurve(a, c, b);
                for (float t = 0f; t < 1f; t += m_curveStep)
                {
                    Vector3 pos = CurveUtility.EvaluatePosition(curve, t);
                    curvePoints.Add(pos);
                }
            }


            foreach(Vector3 point in curvePoints)
            {
                Handles.SphereHandleCap(0, point, Quaternion.identity, 0.2f, EventType.Repaint);
            }
        }
        */
    }

        public void AddJunction(Intersection junction)
    {
        if(intersections == null)
        {
            intersections = new List<Intersection>();
        }
        
        intersections.Add(junction);
        
    }
}
