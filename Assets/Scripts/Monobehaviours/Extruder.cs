using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Takes a (hopefully 2D) mesh m, and creates a adds to m an extrusion of m along a given Splinecomponent with a given number of divisions, stepCount
/// Note, does not match the Spline well if stepCount is less than 50 
/// </summary>
public class Extruder : MonoBehaviour {

    public Vector3 endPoint;   //coordinates of where shape is being extruded to
    public float divisions;
    public Mesh tubeMesh;
    public Mesh extrudeShape;

    public List<Vector3> tempTubeVertices;  //Vertices to be added to mesh later.
    public SplineComponent spline;

    //No constructor because it's a MonoBehaviour and it gets angry if I try to make one

    public void Extrude(Mesh m, SplineComponent splineComponent, int stepCount) {    //Should add 'leniency' later... could it be done with scale?

        tempTubeVertices = new List<Vector3>(); 
        spline = splineComponent;
        extrudeShape = m;

        for(int i = 0; i < extrudeShape.vertexCount; i++) { //converting Array to List, otherwise tempTubeVertices would be empty
            tempTubeVertices.Add(extrudeShape.vertices[i]);
        }

        divisions = (float) stepCount;

        CreateTubeMesh();

        MeshCollider tubeCollider;
        if(this.gameObject.GetComponent<MeshCollider>() == null) {
            tubeCollider = this.gameObject.AddComponent<MeshCollider>();
        }
        else {
            tubeCollider = this.gameObject.GetComponent<MeshCollider>();
        }
        tubeCollider.sharedMesh = tubeMesh;
    }

    private void CreateTubeMesh() { //HIGHLY edited stolen code... the  contents of the for loop in the do loop is really it at this point.

        List<Vector3> tubeVertices = new List<Vector3>();
        for(int i = 0; i < extrudeShape.vertexCount; i++) {     //tubeVeticies & tubeTriangles start with the input shape's mesh's verts/tris
            tubeVertices.Add(extrudeShape.vertices[i]);
        }
        List<int> tubeTriangles = new List<int>();
        for(int i = 0; i < extrudeShape.triangles.Length; i++) {
            tubeTriangles.Add(extrudeShape.triangles[i]);
        }

        tubeMesh = new Mesh();

        float p = 0f;
        Vector3 start = spline.GetNonUniformPoint(0);
        float step = 1f / divisions;
        do {            
            p += step;
            endPoint = spline.GetNonUniformPoint(p);
            for(int i = 0; i < tempTubeVertices.Count - 1; i++) {
                int startIndex = tubeVertices.Count;
                tubeVertices.Add(tempTubeVertices[i] + start);     //left vertex
                tubeVertices.Add(tempTubeVertices[i + 1] + start);   //right vertex
                tubeVertices.Add(tempTubeVertices[i] + endPoint);     //bottom left vertex
                tubeVertices.Add(tempTubeVertices[i + 1] + endPoint); //bottom right vertex

                tubeTriangles.Add(startIndex);
                tubeTriangles.Add(startIndex + 2);
                tubeTriangles.Add(startIndex + 3);

                tubeTriangles.Add(startIndex + 3);
                tubeTriangles.Add(startIndex + 1);
                tubeTriangles.Add(startIndex);
            }
            start = endPoint;

        } while(p + step <= 1);

        tubeMesh.vertices = tubeVertices.ToArray();
        tubeMesh.triangles = tubeTriangles.ToArray();

        this.gameObject.GetComponent<MeshFilter>().mesh = tubeMesh;
    }
}