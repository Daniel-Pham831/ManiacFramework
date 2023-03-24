using UnityEngine;

namespace Maniac.Utils.Extension
{
    public static class Extension
    {
        private static readonly string colorFormat = "<color={1}>{0}</color>";

        public static string AddColor(this object origin, string colorInHex)
        {
            return string.Format(colorFormat, origin.ToString(), colorInHex);
        }
        
        public static string AddColor(this object origin, Color color)
        {
            return string.Format(colorFormat, origin.ToString(), Helper.ToRGBHex(color));
        }
    }
}