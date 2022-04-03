using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MathUtils
{
    public class BezierCurve3
    {
        public Vector3 p0;
        public Vector3 p1;
        public Vector3 p2;

        public BezierCurve3(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
        }

        public Vector3 Evaluate(float t)
        {
            Vector3 q0 = p0 * t + p1 * (1 - t);
            Vector3 q1 = p1 * t + p2 * (1 - t);
            Vector3 r0 = q0 * t + q1 * (1 - t);
            return r0;
        }
    }
}
