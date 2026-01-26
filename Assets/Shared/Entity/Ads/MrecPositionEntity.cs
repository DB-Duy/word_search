using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Utils;
using UnityEngine;

namespace Shared.Entity.Ads
{
    [System.Serializable]
    public class MrecPositionEntity
    {
        [System.Serializable]
        public class Pos
        {
            [JsonProperty("x")] public float X { get; set; }
            [JsonProperty("y")] public float Y { get; set; }

            public Pos(float x, float y)
            {
                X = x;
                Y = y;
            }
            
            public Pos()
            {
            }
        }
        [JsonProperty("positions")] public Dictionary<string, Pos> _p;
        [JsonIgnore] public Dictionary<string, Pos> Positions => _p ??= new Dictionary<string, Pos>();
        
        

        public void AddPosition(string placement, Vector2 position)
        {
            var pos = Positions.GetValueOrDefault(placement) ?? new Pos(position.x, position.y);
            Positions.Upsert(placement, pos);
        }
        
        public Vector2 GetPosition(string placement)
        {
            if (Positions == null || !Positions.TryGetValue(placement, out var p)) return default;
            return new Vector2(p.X, p.Y);
        }
        
        public bool Contains(string placement)
        {
            return Positions != null && Positions.ContainsKey(placement);
        }
        
        public bool Delete(string placement)
        {
            return Positions != null && Positions.Remove(placement);
        }

        public bool IsEmpty()
        {
            return Positions == null || Positions.Count == 0;
        }
    }
}