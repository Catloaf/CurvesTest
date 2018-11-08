using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace YTU.Banks
{
    public class SpriteBank : MonoBehaviour
    {
        static SpriteBank _instance;

        public List<SpriteBankEntry> sprites = new List<SpriteBankEntry>();
        static Dictionary<string, Sprite> _sprite_cache = new Dictionary<string, Sprite>();

        private void Awake()
        {
            _instance = this;
            Initialize();
        }

        static bool initialized = false;
        static void Initialize()
        {
            if (initialized)
                return;

            for (int i = 0; i < _instance.sprites.Count; i++)
            {
                _sprite_cache[_instance.sprites[i].id] = _instance.sprites[i].s;
            }

            initialized = true;
        }

        public static Sprite GetSprite(string id)
        {
            Initialize();

            return _sprite_cache[id];
        }
    }

    [Serializable]
    public class SpriteBankEntry
    {
        public Sprite s;
        public string id;
    }
}