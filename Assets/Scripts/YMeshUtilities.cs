using UnityEngine;
using System;
using System.Collections.Generic;
using YTU.Banks;

/// <summary>
/// Originally made by Austin Yarger, a few minor, insignificant edits by me--Ben Okun
/// </summary>
public class YMeshUtilities
{
    static Dictionary<Texture, Dictionary<float, Mesh>> _texture_mesh_cache = new Dictionary<Texture, Dictionary<float, Mesh>>();

    static Dictionary<Mesh, Dictionary<float, Mesh>> _2d_mesh_cache = new Dictionary<Mesh, Dictionary<float, Mesh>>();

    static Dictionary<Sprite, Dictionary<float, Mesh>> _sprite_mesh_cache = new Dictionary<Sprite, Dictionary<float, Mesh>>();

    public static void GetAndApplyMeshFromSprite(Sprite s, float depth, MeshRenderer mr, MeshFilter mf)
    {
        Mesh new_mesh = GetMeshFromSprite(s, depth);
        Material new_mat = new Material(ShaderCache.GetShader("Unlit/Texture"));
        mr.material = new_mat;
        mr.material.mainTexture = s.texture;
        mf.mesh = new_mesh;
    }

    public static void GetAndApplyMeshFromTexture(Texture t, float depth, MeshRenderer mr, MeshFilter mf, float pixels_per_unit=100)
    {
        Mesh new_mesh = GetMeshFromTexture(t, depth, pixels_per_unit);
        Material new_mat = new Material(ShaderCache.GetShader("Unlit/Texture"));
        mr.material = new_mat;
        mr.material.mainTexture = t;
        mf.mesh = new_mesh;
    }

    public static Mesh GetMeshFromTexture(Texture t, float depth, float pixels_per_unit=100)
    {
        if (!_texture_mesh_cache.ContainsKey(t))
        {
            _texture_mesh_cache[t] = new Dictionary<float, Mesh>();
        }

        if (!_texture_mesh_cache[t].ContainsKey(depth))
        {
            Mesh mesh = new Mesh();

            Sprite s = Sprite.Create(
                (Texture2D)t,
                new Rect(0, 0, t.width, t.height),
                new Vector2(0.5f, 0.5f),
                pixels_per_unit,
                0,
                SpriteMeshType.Tight
            );

            mesh.vertices = GetVertices(s.vertices, depth).ToArray();
            mesh.uv = GetUVs(s.uv).ToArray();
            mesh.triangles = GetTris(s.triangles, s.vertices).ToArray();

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            _texture_mesh_cache[t][depth] = mesh;
        }

        return _texture_mesh_cache[t][depth];
    }

    public static Mesh GetMeshFrom2DMesh(Mesh m, float depth)
    {
        if (!_2d_mesh_cache.ContainsKey(m))
        {
            _2d_mesh_cache[m] = new Dictionary<float, Mesh>();
        }

        if (!_2d_mesh_cache[m].ContainsKey(depth))
        {
            Mesh mesh = new Mesh();

            int num_vertices = m.vertices.Length;
            Vector2[] vertices = new Vector2[m.vertices.Length];
            for (int i = 0; i < num_vertices; i++)
            {
                vertices[i] = new Vector2(m.vertices[i].x, m.vertices[i].y);
            }

            int num_uvs = m.vertices.Length;
            Vector2[] uvs = new Vector2[m.uv.Length];
            for (int i = 0; i < num_uvs; i++)
            {
                uvs[i] = new Vector2(m.uv[i].x, m.uv[i].y);
            }

            int num_triangles = m.triangles.Length;
            ushort[] tris = new ushort[num_triangles];
            for (int i = 0; i < num_triangles; i++)
            {
                tris[i] = (ushort)m.triangles[i];
            }

            mesh.vertices = GetVertices(vertices, depth).ToArray();
            mesh.uv = GetUVs(uvs).ToArray();
            mesh.triangles = GetTris(tris, vertices).ToArray();

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            _2d_mesh_cache[m][depth] = mesh;
        }

        return _2d_mesh_cache[m][depth];
    }

    public static Mesh GetMeshFromSprite(Sprite s, float depth)
    {
        if (!_sprite_mesh_cache.ContainsKey(s))
        {
            _sprite_mesh_cache[s] = new Dictionary<float, Mesh>();
        }

        if (!_sprite_mesh_cache[s].ContainsKey(depth))
        {
            Mesh mesh = new Mesh
            {
                vertices = GetVertices(s.vertices, depth).ToArray(),
                uv = GetUVs(s.uv).ToArray(),
                triangles = GetTris(s.triangles, s.vertices).ToArray()
            };

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            _sprite_mesh_cache[s][depth] = mesh;
        }

        return _sprite_mesh_cache[s][depth];
    }

    static List<Vector3> GetVertices(Vector2[] vertices, float depth)
    {
        List<Vector3> result = new List<Vector3>();

        // Near Vertices
        foreach (Vector2 v in vertices)
        {
            Vector3 near_vertex = (Vector3)v - Vector3.forward * depth * 0.5f;
            result.Add(near_vertex);
        }

        // Far Vertices
        foreach (Vector2 v in vertices)
        {
            Vector3 near_vertex = (Vector3)v;
            Vector3 far_vertex = near_vertex + Vector3.forward * depth * 0.5f;

            result.Add(far_vertex);
        }

        return result;
    }

    

    /*static List<Vector3> GetVerticesFromSprite2(Sprite s, float depth)
    {
        List<Vector3> result = new List<Vector3>();

        // Near Vertices
        foreach (Vector2 v in s.vertices)
        {
            Vector3 near_vertex = (Vector3)v - Vector3.forward * depth * 0.5f;
            result.Add(near_vertex);
        }

        // Far Vertices
        foreach (Vector2 v in s.vertices)
        {
            Vector3 near_vertex = (Vector3)v;
            Vector3 far_vertex = near_vertex + Vector3.forward * depth * 0.5f;

            result.Add(far_vertex);
        }

        return result;
    }*/



    static List<Vector2> GetUVs(Vector2[] uv)
    {
        List<Vector2> result = new List<Vector2>();
        int num_uvs = uv.Length;

        // Near Vertices
        for (int i = 0; i < num_uvs; i++)
        {
            result.Add(uv[i]);
        }

        // Far Vertices
        for (int i = 0; i < num_uvs; i++)
        {
            result.Add(uv[i]);
        }

        return result;
    }

    static List<int> GetTris(ushort[] triangles, Vector2[] vertices)
    {
        List<int> result = new List<int>();

        // Near Face
        int num_triangles = triangles.Length;
        for (int i = 0; i < num_triangles; i++)
        {
            result.Add(triangles[i]);
        }

        // Far Face
        for (int i = 0; i < num_triangles; i += 3)
        {
            result.Add(triangles[i + 2] + vertices.Length);
            result.Add(triangles[i + 1] + vertices.Length);
            result.Add(triangles[i] + vertices.Length);
        }

        // Sides of mesh
        int[] tris = Array.ConvertAll(triangles, i => (int)i);
        List<EdgeHelpers.Edge> edges = EdgeHelpers.GetEdges(tris);
        List<EdgeHelpers.Edge> boundary = EdgeHelpers.FindBoundary(edges).SortEdges();

        for (int i = 0; i < boundary.Count; i++)
        {
            EdgeHelpers.Edge edge = boundary[i];

            int vertex1 = edge.v1; // Near #1
            int vertex2 = edge.v2; // Near #2
            int vertex3 = edge.v1 + vertices.Length; // Far #1
            int vertex4 = edge.v2 + vertices.Length; // Far #2

            // Triangle 1
            result.Add(vertex1);
            result.Add(vertex3);
            result.Add(vertex2);

            // Triangle 2
            result.Add(vertex2);
            result.Add(vertex3);
            result.Add(vertex4);
        }

        return result;
    }
}

public static class EdgeHelpers
{
    public struct Edge
    {
        public int v1;
        public int v2;
        public int triangleIndex;
        public Edge(int aV1, int aV2, int aIndex)
        {
            v1 = aV1;
            v2 = aV2;
            triangleIndex = aIndex;
        }
    }

    public static List<Edge> GetEdges(int[] aIndices)
    {
        List<Edge> result = new List<Edge>();
        for (int i = 0; i < aIndices.Length; i += 3)
        {
            int v1 = aIndices[i];
            int v2 = aIndices[i + 1];
            int v3 = aIndices[i + 2];
            result.Add(new Edge(v1, v2, i));
            result.Add(new Edge(v2, v3, i));
            result.Add(new Edge(v3, v1, i));
        }
        return result;
    }

    public static List<Edge> FindBoundary(this List<Edge> aEdges)
    {
        List<Edge> result = new List<Edge>(aEdges);
        for (int i = result.Count - 1; i > 0; i--)
        {
            for (int n = i - 1; n >= 0; n--)
            {
                if (result[i].v1 == result[n].v2 && result[i].v2 == result[n].v1)
                {
                    // shared edge so remove both
                    result.RemoveAt(i);
                    result.RemoveAt(n);
                    i--;
                    break;
                }
            }
        }
        return result;
    }
    public static List<Edge> SortEdges(this List<Edge> aEdges)
    {
        List<Edge> result = new List<Edge>(aEdges);
        for (int i = 0; i < result.Count - 2; i++)
        {
            Edge E = result[i];
            for (int n = i + 1; n < result.Count; n++)
            {
                Edge a = result[n];
                if (E.v2 == a.v1)
                {
                    // in this case they are already in order so just continoue with the next one
                    if (n == i + 1)
                        break;
                    // if we found a match, swap them with the next one after "i"
                    result[n] = result[i + 1];
                    result[i + 1] = a;
                    break;
                }
            }
        }
        return result;
    }
}