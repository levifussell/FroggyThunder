using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MathUtils
{
    namespace Editor
    {
#if UNITY_EDITOR
        public static class E_BezierCurves
        {
            public static void EditorSceneDrawBezierCurve3(BezierCurve3 curve3, int steps = 100, float sphereRadius = 0.03f)
            {
                Vector3[] points = new Vector3[steps];
                for (int i = 0; i < steps; ++i)
                {
                    float t = (float)i / (float)steps;
                    points[i] = curve3.Evaluate(t);
                }

                Handles.DrawLines(points);

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(curve3.p0, sphereRadius);
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(curve3.p1, sphereRadius);
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(curve3.p2, sphereRadius);
            }
        }
#endif
    }
}
