using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Maniac.Utils
{
    [Serializable]
    public class Range
    {
        public int LowerBound;
        public int UpperBound;
        [JsonIgnore]
        public int Diff => UpperBound - LowerBound;

        public bool IsInRange(int value)
        {
            return LowerBound <= value && value <= UpperBound;
        }

        public bool IsAllInRange(IEnumerable<int> values)
        {
            return values.All(IsInRange);
        }
        
        public float GetRandom()
        {
            return UnityEngine.Random.Range(LowerBound, UpperBound + 1);
        }
    }

    [Serializable]
    public class FloatRange
    {
        public float LowerBound;
        public float UpperBound;
        [JsonIgnore]
        public float Diff => UpperBound - LowerBound;

        public bool IsInRange(float value)
        {
            return LowerBound <= value && value <= UpperBound;
        }
        
        public bool IsAllInRange(IEnumerable<float> values)
        {
            return values.All(IsInRange);
        }

        public float GetRandom()
        {
            return UnityEngine.Random.Range(LowerBound, UpperBound);
        }
    }
}