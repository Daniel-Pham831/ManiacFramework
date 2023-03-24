namespace Maniac.Utils.Extension
{
    public static class FloatExtension
    {
        public static string ToStringPercent(this float value)
        {
            return $"{value * 100}%";
        }

        public static string ToStringMultiplier(this float value)
        {
            return $"x{value}";
        }

        public static string ToStringDegree(this float value)
        {
            return $"{value}°";
        }
        
        public static string ToStringSecond(this float value)
        {
            return $"{value}s";
        }
        
        public static string ToStringCm(this float value)
        {
            return $"{value}cm";
        }
    }
}