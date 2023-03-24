using Maniac.DataBaseSystem;
using Maniac.LanguageTableSystem;
using Maniac.UISystem;
using Maniac.Utils.Extension;
using UnityEditor;
using UnityEngine;

namespace Maniac.Bootstrap.Scripts
{
    public class BootstrapEditor
    {
        [MenuItem("Bootstrap/Start/1: Create DataBase")]
        public static void CreateDataBase()
        {
            DataBase.CreateDataBase();
            Debug.Log($"Step {"1".AddColor(Color.green)}: {"Completed".AddColor(Color.yellow)}");
        }
        
        [MenuItem("Bootstrap/Start/2: Create LanguageTable")]
        public static void CreateLanguageTable()
        {
            LanguageTable.CreateLanguageTable();
            Debug.Log($"Step {"2".AddColor(Color.green)}: {"Completed".AddColor(Color.yellow)}");
        }
        
        [MenuItem("Bootstrap/Start/3: Create UIData")]
        public static void CreateUIData()
        {
            UIData.CreateUIData();
            Debug.Log($"Step {"3".AddColor(Color.green)}: {"Completed".AddColor(Color.yellow)}");
        }
    }
}