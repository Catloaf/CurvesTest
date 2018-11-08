using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineComponent : MonoBehaviour {

    //Destroyed the 'closed' bool
    public List<Vector3> points = new List<Vector3>();
    public float? length;   //What does the '?' here mean?!?!
    
    public int ControlPointCount => points.Count;   //Not used enough for whatever reason, points.Count is used instead, so why even bother with this?!


    public void InsertControlPoint(int index, Vector3 position) {
        ResetIndex();
        if(index >= points.Count) {
            points.Add(position);
        }
        else {
            points.Insert(index, position);
        }
    }

    public void SetControlPoint(int index, Vector3 position) {
        ResetIndex();
        points[index] = position;
    }

    public Vector3 GetControlPoint(int index) {
        return points[index];
    }

    public Vector3 GetPoint(float t) => Index.GetPoint(t);

    public Vector3 GetNonUniformPoint(float t) {
        switch(points.Count) {
            case 0:
                return Vector3.zero;    //only point in a line of length 0 is (0,0,0)... wait what?
            case 1:
                return transform.TransformPoint(points[0]); //transform is a Vector 3 property of all GameObjects in Unity
            case 2:                                             //Returning the coordinates of the only point on the line... I think
                return transform.TransformPoint(Vector3.Lerp(points[0], points[1], t));     //returning the linear interpolation between the 2 points with magnitude(?) t
            case 3:
                return transform.TransformPoint(points[1]); //Returns the 2nd point's coordinates... I don't know why.
            default:
                return Hermite(t);
        }
    }

    public float GetLength(float stepSize = 0.001f) {
        float d = 0f;
        Vector3 a = GetNonUniformPoint(0);
        for(float t = 0f; t < 1f; t += stepSize) {
            Vector3 b = GetNonUniformPoint(t);
            Vector3 delta = (b - a);
            d += delta.magnitude;
            a = b;
        }
        return d;
    }

    Vector3 GetPointByIndex(int i) {
        if(i < 0) {                         //Why would the index ever be negative? but anyway...
            i += points.Count;
        }
        return points[i % points.Count];    //rolls over if too big, thus the %
    }

    //This is largely lacking in explaination...
    //"This is the function that looks up the control points for positioning along the spline 
        //then performs & returns the interpolated world position."
    Vector3 Hermite(float t) {  
        int count = points.Count - 3;   //because closed if never used, with my stuff, if this fucks this up, swap out 3 for 0
        int i = Mathf.Min(Mathf.FloorToInt(t * (float) count), count - 1);
        float u = t * (float) count - (float) i;
        Vector3 a = GetPointByIndex(i);     //Control Point 1 (?)
        Vector3 b = GetPointByIndex(i + 1); //Control Point 2 (?)
        Vector3 c = GetPointByIndex(i + 2); //Start point (?)
        Vector3 d = GetPointByIndex(i + 3); //End Point (?)
        return transform.TransformPoint(Interpolate(a, b, c, d, u));
    }

    internal static Vector3 Interpolate(Vector3 controlPoint1, Vector3 controlPoint2, Vector3 startPoint, Vector3 endPoint, float interpolationPosition) {
        return (
            0.5f *
            (
                (-controlPoint1 + 3f * controlPoint2 - 3f * startPoint + endPoint) *
                (interpolationPosition * interpolationPosition * interpolationPosition) +
                (2f * controlPoint1 - 5f * controlPoint2 + 4f * startPoint - endPoint) *
                (interpolationPosition * interpolationPosition) +
                (-controlPoint1 + startPoint) *
                interpolationPosition + 2f * controlPoint2
            )
        );
    }

    //LAZY INDEXING
        //Index is used to provide uniform point searching.
    //Poorly organized code here because setting aside like the tutorial.
    SplineIndex uniformIndex;
    SplineIndex Index {
        get {
            if(uniformIndex == null) {
                uniformIndex = new SplineIndex(this);
            }
            return uniformIndex;
        }
    }

    public void ResetIndex() {
        uniformIndex = null;
        length = null;
    }

   public void Reset() {  //Unity's reset... Not sure why these numbers are the way they are...
        points = new List<Vector3>() {  //So... defaults to ([0,0,3],[0,0,6],[0,0,9],[0,0,12])...?  I think I might want to swap this to Y instead of Z.
            Vector3.zero,    //So I just got rid of the mystery numbers
            Vector3.zero,
            Vector3.zero,
            Vector3.zero
        };
    }

    void OnValidate() { //Called whenever a component's values have been changed in UnityEditor
        if(uniformIndex != null) {
            uniformIndex.ReIndex();
        }
    }









    //Thowing all the useless crap to the end of the file:
    //Slightly Useless:

    public Vector3 GetUp(float t) {
        Vector3 A = GetPoint(t - 0.001f);
        Vector3 B = GetPoint(t + 0.001f);
        Vector3 delta = (B - A).normalized;
        return Vector3.Cross(delta, GetRight(t));
    }

    public Vector3 GetDown(float t) => -GetUp(t);

    public Vector3 GetForward(float t) {
        Vector3 a = GetPoint(t - 0.001f);
        Vector3 b = GetPoint(t + 0.001f);
        return (b - a).normalized;  //not sure what .normalized is/does or how/why the minus operand works here
    }

    public Vector3 GetBackward(float t) => -GetForward(t);

    public Vector3 GetRight(float t) {
        Vector3 a = GetPoint(t - 0.001f);
        Vector3 b = GetPoint(t + 0.001f);
        Vector3 delta = (b - a);
        return new Vector3(-delta.y, delta.x).normalized;
    }

    public Vector3 GetLeft(float t) => -GetRight(t);
  
    //COMPLETELY Useless
    public void RemoveControlPoint(int index) { //THIS DOES NOT WORK so just getting rid of it for now
        throw new System.NotImplementedException(); //by just putting default shit here so compiler doesn't get angry
    }
    public Vector3 GetDistance(float distance) {    //Apparenlty never actually used.
        if(length == null) {
            length = GetLength();
        }
        return uniformIndex.GetPoint(distance / length.Value);
    }
    //returns approx closest position on spline to given world point
    public Vector3 FindClosest(Vector3 worldPoint) {
        float smallestDelta = float.MaxValue;
        float step = 1f / 1024;
        Vector3 closestPoint = Vector3.zero;
        for(int i = 0; i <= 1024; i++) {
            Vector3 p = GetPoint(i * step);
            float delta = (worldPoint - p).sqrMagnitude;
            if(delta < smallestDelta) {
                closestPoint = p;
                smallestDelta = delta;
            }
        }
        return closestPoint;
    }
}