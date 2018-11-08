using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineIndex {
    public Vector3[] linearPoints;
    SplineComponent spline;

    public int ControlPointCount => spline.ControlPointCount;

    public SplineIndex(SplineComponent spline) {
        this.spline = spline;
        ReIndex();
    }

    public void ReIndex() {
        float searchStepSize = 0.00001f;
        float length = spline.GetLength(searchStepSize);
        int indexSize = Mathf.FloorToInt(length * 2);
        List<Vector3> linearPoints = new List<Vector3>(indexSize); //...what does the "_" @ the front mean? (Removed it because it bothered me)
        float t = 0f; //meaningless variable name leads me to believe I'm missing some math background info.

        float linearDistanceStep = length / 1024;
        float linearDistanceStep2 = Mathf.Pow(linearDistanceStep, 2);

        Vector3 start = spline.GetNonUniformPoint(0);
        linearPoints.Add(start);
        while(t <= 1f) {
            Vector3 current = spline.GetNonUniformPoint(t);
            while((current - start).sqrMagnitude <= linearDistanceStep2) {
                t += searchStepSize;
                current = spline.GetNonUniformPoint(t);
            }
            start = current;
            linearPoints.Add(current);
        }
        this.linearPoints = linearPoints.ToArray();
    }

    public Vector3 GetPoint(float t) {  //takes a float & converts it into an int, why not just take an int?
        int sections = linearPoints.Length - 3; //should never be closed, if I fucked up, swap 3 with 0
        int i = Mathf.Min(Mathf.FloorToInt(t * (float) sections), sections - 1);
        int count = linearPoints.Length;
        if(i < 0)
            i += count;
        float u = t * sections - i;   //Interpolation Position (?)
        Vector3 a = linearPoints[(i + 0) % count];      //Control Point (?)
        Vector3 b = linearPoints[(i + 1) % count];      //Control Point (?)
        Vector3 c = linearPoints[(i + 2) % count];      //Start Point (?)
        Vector3 d = linearPoints[(i + 3) % count];      //End Point (?)
        return SplineComponent.Interpolate(a, b, c, d, u);
    }
}
