#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Maniac.LanguageTableSystem
{
    public class LanguageTableManager : OdinMenuEditorWindow
    {
        [MenuItem("Tools/Language Table")]
        private static void OpenEditor() => GetWindow<LanguageTableManager>();

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            var languageTable = LanguageTable.ActiveLanguageTable;
            var languageItems = languageTable.GetAllItems();

            tree.AddAllAssetsAtPath(languageTable.name, "Assets/Resources", languageTable.GetType(), true, true);

            Dictionary<string, List<LanguageItem>> subLanguageItems = new Dictionary<string, List<LanguageItem>>();
            foreach (var item in languageItems)
            {
                var itemName = item.name;
                var separatorIndex = itemName.IndexOf("_");
                if (separatorIndex == -1)
                {
                    subLanguageItems.Add(itemName,new List<LanguageItem>(){item});
                    continue;
                }

                var prefix = itemName.Substring(0, separatorIndex);
                if (!subLanguageItems.ContainsKey(prefix))
                {
                    subLanguageItems.Add(prefix,new List<LanguageItem>());
                }

                var subLanguageItemValues = subLanguageItems[prefix];
                subLanguageItemValues.Add(item);
            }

            OdinMenuItem bigTree = new OdinMenuItem(tree, "LanguageItems", null);
            foreach (var key in subLanguageItems.Keys)
            {
                var values = subLanguageItems[key];
                OdinMenuItem mainMenuItem;
                if (values.Count == 1)
                {
                    mainMenuItem =  new OdinMenuItem(tree, key,values.FirstOrDefault());
                }
                else
                {
                    mainMenuItem = new OdinMenuItem(tree, key, null);
                    foreach (var value in values)
                    {
                        List<OdinMenuItem> odinMenuItems = new List<OdinMenuItem>();
                        odinMenuItems.Add(new OdinMenuItem(tree,value.name,value));
                        mainMenuItem.ChildMenuItems.AddRange(odinMenuItems);
                        mainMenuItem.ChildMenuItems.SortMenuItemsByName();
                    }
                }
                
                bigTree.ChildMenuItems.Add(mainMenuItem);
            }
            
            tree.MenuItems.Add(bigTree);

            return tree;
        }
    }
}

#endif