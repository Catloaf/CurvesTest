using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// This is to generate a path in the form of a 2D-spline where all control points are passed thru directly.
/// Will then attempt to put 2 circles (meshes) at the ends and extrude a tube along the spline and add all 3 to
///     a single mesh.
///     
/// 
/// </summary>
public class PathGen : MonoBehaviour {

    public Vector3 startPoint = new Vector3(0.0f, 0.0f);
    public Vector3 endPoint = new Vector3(10.0f, 10.0f, 10.0f);

    public int controlLength;   //number of control points
    public bool useRandomLength = false;
    public int minLength = 0;
    public int maxLength = 10;


    public string seed;
    public bool useRandomSeed = false;
    System.Random psuedoRandom;
    public SplineComponent splinePath;

    //stuff for Austin Yarger's YMeshUnitities
    public Sprite limonade;    //french for lemon (and sometimes other citrus) soda, so... Sprite
    public MeshFilter mf;
    public MeshRenderer mr;
    public Material mat;
    Mesh extrusionMesh;
    Extruder spriteExtruder;
    public int stepCount = 50;  //It doesn't seem to work below 50 so default to 50.
    public float leniency;  //Currently just a scale multiplier, might want to make it something else later...
    private static int leniencyAdjustmentCount; //can't figure out why things will continue getting bigger, so I'm doing this roundabout fix

    void Start() {
        mf = this.gameObject.AddComponent<MeshFilter>();

        mr = this.gameObject.AddComponent<MeshRenderer>();
        mat = mr.material;
        mat.color = Color.white;

        spriteExtruder = gameObject.AddComponent<Extruder>();
        leniencyAdjustmentCount = 0;
    }


    private void Update() {
        if(Input.GetMouseButtonDown(0)) {   //Just having it go on-click for now, maybe have an editor button later.
            GeneratePath();
        }
    }

    void GeneratePath() {
        
        extrusionMesh = GetExtrusionShape(limonade);
        Vector3[] adjustedVertices = extrusionMesh.vertices;
        for(int i = 0; i < adjustedVertices.Length; i++) {
            Vector3 adjustedVertex = adjustedVertices[i];
            adjustedVertex.x *= leniency;
            adjustedVertex.y *= leniency;
            adjustedVertices[i] = adjustedVertex;
        }
        leniencyAdjustmentCount += 1;
        if(leniencyAdjustmentCount == 1) {  //so this SHOULD only happen the 1st time....
            extrusionMesh.vertices = adjustedVertices;
        }
        

        if(useRandomLength || controlLength < 0) {
            if(minLength >= 0 && maxLength > minLength) {
                controlLength = UnityEngine.Random.Range(minLength, maxLength);
            }
        }
        if(useRandomSeed || seed == null || seed == "") {
            seed = UnityEngine.Random.Range(0, Int32.MaxValue).ToString("X"); //making it a sting in Hexadecimial to make it shorter in case user wants to save it for later.
        }
        psuedoRandom = new System.Random(seed.GetHashCode());

        //Currently just filling the spline with random points, will change this later
        //GetNextPointinPathFromMovementRange(); or something like that
        splinePath.points.Clear();
        splinePath.points.Add(startPoint);
        splinePath.points.Add(startPoint);  //The spline isn't drawn on the 1st and final points, so by doubleing them at the beginning and end, it makes it drawn on them
        int f = 0;
        for(int i = 2; i < controlLength; i++) {
            f = i - 2;
            float pX = psuedoRandom.Next(0, maxLength) * (float) psuedoRandom.NextDouble();
            float pY = psuedoRandom.Next(0, maxLength) * (float) psuedoRandom.NextDouble();
            splinePath.points.Add(new Vector3(pX, pY, f)); //having linear realtionship z=x...hopefully
        }
        endPoint.z = f + 1;
        splinePath.points.Add(endPoint);    //*See comment above the FOR loop*
        splinePath.points.Add(endPoint);

        mf.mesh = spriteExtruder.tubeMesh;

        if(stepCount <= splinePath.length) {    //This doesn't seem to work right... at least while Unity is running.
            stepCount = (int) splinePath.length + 50;
        }
        spriteExtruder.Extrude(extrusionMesh, splinePath, stepCount);
    }
    
    //getting a 2D mesh from either a sprite, an existing 2D mesh, or texture
    Mesh GetExtrusionShape(Sprite s) {  
        return YMeshUtilities.GetMeshFromSprite(s, 0.0f);
    }
    Mesh GetExtrusionShape(Mesh m2D) {  //not actually expecting to use the overloads, but not much effort to add them
        return YMeshUtilities.GetMeshFrom2DMesh(m2D, 0.0f);
    }
    Mesh GetExtrusionShape(Texture t){
        return YMeshUtilities.GetMeshFromTexture(t, 0.0f);
    }


    private void OnDrawGizmos() {   
         DrawGizmo(splinePath, 1024);
    }
    static void DrawGizmo(SplineComponent spline, int stepCount) {
        Gizmos.color = Color.white;
        if(spline.points.Count > 0) {
            float p = 0f;
            Vector3 start = spline.GetNonUniformPoint(0);
            float step = 1f / stepCount;
            do {
                p += step;
                Vector3 here = spline.GetNonUniformPoint(p);
                Gizmos.DrawLine(start, here);   
                start = here;
            } while(p + step <= 1);
            foreach(Vector3 item in spline.points) {
                Gizmos.DrawSphere(item, .25f);
            }
        }
    }
}
