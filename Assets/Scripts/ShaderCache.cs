using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace YTU.Banks
{
    public class ShaderCache : MonoBehaviour
    {
        static Dictionary<string, Shader> _cache = new Dictionary<string, Shader>();

        public static Shader GetShader(string shader_path)
        {
            if (!_cache.ContainsKey(shader_path))
                _cache[shader_path] = Shader.Find(shader_path);

            if (_cache[shader_path] == null)
                Debug.LogError("[ShaderCache] Failed to locate shader at path [" + shader_path + "]");

            return _cache[shader_path];
        }
    }
}