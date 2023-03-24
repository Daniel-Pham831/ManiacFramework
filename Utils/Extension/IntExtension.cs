namespace Maniac.Utils.Extension
{
    public static class IntExtension
    {
        public static string ToStringDegree(this int value)
        {
            return $"{value}°";
        }
        
        public static string ToStringSecond(this int value)
        {
            return $"{value}s";
        }
        
        public static string ToStringCm(this int value)
        {
            return $"{value}cm";
        }
        
        public static string ToStringMoney(this int value)
        {
            return $"{value}$";
        }
    }
}