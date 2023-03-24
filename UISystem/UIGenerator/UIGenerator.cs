#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Compilation;
using UnityEngine;

namespace Maniac.UISystem.UIGenerator
{
    public class UIDialogCreator
    {
        [MenuItem("Assets/Create/UI/Create New UI", priority = 81)]
        public static void CreatePlainClass()
        {
            string clickedAssetGuid = Selection.assetGUIDs[0];
            string clickedPath = AssetDatabase.GUIDToAssetPath(clickedAssetGuid);
            DialogInputNameDialog.ShowWithPathAsset("Input name of new UI", clickedPath, OnCreateDialogAction);
        }
        private static bool ValidateString(string value)
        {
            //detect string value is valid
            if (value.Length == 0 || value.Contains(' ') || value.Contains('.'))
            {
                return false;
            }
            return true;
        }
        private static bool OnCreateScriptObjectWithName(string nameNamespace, string nameAsset, string pathScriptAsset)
        {

            string copyPath = $"{pathScriptAsset}/{nameAsset}.cs";
            UnityEngine.Debug.Log("Creating Class File: " + copyPath);
            if (!Directory.Exists(pathScriptAsset))
            {
                Directory.CreateDirectory(pathScriptAsset);
            }
            if (File.Exists(copyPath) == false)
            {
                string templatePath = Path.Combine(Application.dataPath, "Maniac/UISystem/UIGenerator/BaseUITemplate.txt");
                string dataTemplateContent = File.ReadAllText(templatePath);
                dataTemplateContent = dataTemplateContent.Replace("@ScriptName", nameAsset);
                dataTemplateContent = dataTemplateContent.Replace("@NamespaceName", nameNamespace);
                // do not overwrite
                using (StreamWriter outfile = File.CreateText(copyPath))
                {
                    outfile.WriteLine(dataTemplateContent);
                } //File written
                AssetDatabase.Refresh();
                return true;
            }
            return false;
        }

        private static bool OnCreateDialogObjectWithName(string nameNamespace, string nameAsset, string pathAsset)
        {
            string path = $"{pathAsset}/{nameAsset}.prefab";
            Debug.Log("Creating Dialog File: " + path);
            //make sure the path not exist to create one
            if (File.Exists(path) == true)
            {
                return false;
            }

            if (!Directory.Exists(pathAsset))
            {
                Directory.CreateDirectory(pathAsset);
            }
            Type type = System.Type.GetType($"{nameNamespace}.{nameAsset}, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            if (type == null)
            {
                return false;
            }

            //copy the template dialog into the path
            System.IO.File.Copy(Path.Combine(Application.dataPath, "Maniac/UISystem/UIGenerator/UIPrefabTemplate/UITemplate.prefab"), path);
            //
            GameObject go = PrefabUtility.LoadPrefabContents(path);
            go.AddComponent(type);

            EditorUtility.SetDirty(go);
            PrefabUtility.SaveAsPrefabAsset(go, path);
            //refresh & save
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            return true;
        }
        private static int OnCreateDialogAction(InfoClassDialogPath input, int state)
        {
            string nameAsset = input.InputName;
            string pathAsset = input.InputAssetPath;
            string pathScriptAsset = input.InputScriptPath;
            List<string> lsNamespaces = new List<string>();
            bool containNamespace = nameAsset.Contains(".");
            string nameNamespace = "Game";
            if (containNamespace)
            {
                string[] ls = nameAsset.Split(".");
                for (int i = 0; i < ls.Length - 1; i++)
                {
                    lsNamespaces.Add(ls[i]);
                    nameNamespace += ("." + ls[i]);
                }
                nameAsset = ls[ls.Length - 1];
            }

            pathScriptAsset += $"/{nameAsset}";
            pathAsset += $"/{nameAsset}";


            //detect value string input is valid or not
            if (!ValidateString(nameAsset) || !ValidateString(pathAsset) || !ValidateString(pathScriptAsset))
            {
                return -1;
            }
            if (state == 0) //create scrip object
            {
                bool res = OnCreateScriptObjectWithName(nameNamespace, nameAsset, pathScriptAsset);
                if (!res)
                {
                    string path = $"{pathScriptAsset}/{nameAsset}.asset";
                    if (File.Exists(path))
                    {
                        return -2;
                    }
                    else
                    {
                        state++;
                    }
                }
            }

            if (state == 1) // create asset object
            {
                //create object
                bool result = OnCreateDialogObjectWithName(nameNamespace, nameAsset, pathAsset);
                if (!result)
                {
                    return -2;
                }
                //refresh & save
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }

            return state;
        }

        [DidReloadScripts]
        private static void OnReloadScriptFinish()
        {
            InfoClassDialogPath tempInfoClass = InfoClassDialogPath.CreateFromSaved();
            //continue to state 2
            if (tempInfoClass == null)
            {
                return;
            }
            OnCreateDialogAction(tempInfoClass, 1);
        }
    }

    internal class InfoClassDialogPath
    {
        public String InputName;
        public String InputAssetPath;
        public String InputScriptPath;
        public void Save()
        {
            PlayerPrefs.SetString("Game.InfoClassDialogPath.InputName", InputName);
            PlayerPrefs.SetString("Game.InfoClassDialogPath.InputAssetPath", InputAssetPath);
            PlayerPrefs.SetString("Game.InfoClassDialogPath.InputScriptPath", InputScriptPath);
            PlayerPrefs.Save();
        }

        public static InfoClassDialogPath CreateFromSaved()
        {
            string inputName = PlayerPrefs.GetString("Game.InfoClassDialogPath.InputName", "");
            string inputAssetPath = PlayerPrefs.GetString("Game.InfoClassDialogPath.InputAssetPath", "");
            string inputScriptPath = PlayerPrefs.GetString("Game.InfoClassDialogPath.InputScriptPath", "");
            if (inputName.Length == 0)
            {
                return null;
            }
            InfoClassDialogPath ret = new InfoClassDialogPath()
            {
                InputName = inputName,
                InputAssetPath = inputAssetPath,
                InputScriptPath = inputScriptPath,
            };
            //clear the temp
            PlayerPrefs.DeleteKey("Game.InfoClassDialogPath.InputName");
            PlayerPrefs.DeleteKey("Game.InfoClassDialogPath.InputAssetPath");
            PlayerPrefs.DeleteKey("Game.InfoClassDialogPath.InputScriptPath");
            PlayerPrefs.Save();
            return ret;
        }

        public static InfoClassDialogPath Create(string name)
        {
            InfoClassDialogPath ret = new InfoClassDialogPath()
            {
                InputName = name
            };
            return ret;
        }
    }
    internal class DialogInputNameDialog : UnityEditor.EditorWindow
    {
        private string _description;
        private string _okButton;
        private string _cancelButton;
        //
        private InfoClassDialogPath _infoClass;
        private Func<InfoClassDialogPath, int, int> _createAction = null;
        //
        private int _currentState = -1;
        private bool _initializedPosition = false;
        private bool _shouldClose = false;
        private bool _showErrorIssue = false;
        //
        private string _pathAsset = "";

        private void OnCompileFinish(object obj)
        {
            //refresh
            AssetDatabase.Refresh();
            //reload script
            EditorUtility.RequestScriptReload();
            //remove event handler
            CompilationPipeline.compilationFinished -= OnCompileFinish;
        }

        private void CompileScript()
        {
            //compile script
            CompilationPipeline.RequestScriptCompilation();
            //set call back
            CompilationPipeline.compilationFinished += OnCompileFinish;
            //save the info class
            _infoClass.Save();
            //close to force reload
            Close();
        }

        private void HandleOkButton(int state)
        {
            int res = 0;
            if (_createAction != null)
            {
                res = _createAction(_infoClass, state);
            }

            if (res >= 0)
            {
                _showErrorIssue = false;
            }
            else
            {
                _showErrorIssue = true;
                if (res == -1)
                {
                    _description = "ERROR : there is space or not formatted in name, re-correct name and try again.";
                }
                else if (res == -2)
                {
                    _description =
                        "ERROR : there is UI with same name in game, please use another name.";
                }
            }

            if (res == 0)
            {
                CompileScript();
            }

            _currentState = res;
        }

        private void OnGUI()
        {
            // Check if Esc/Return have been pressed
            Event e = UnityEngine.Event.current;
            if (e.type == EventType.KeyDown)
            {
                switch (e.keyCode)
                {
                    // Escape pressed
                    case KeyCode.Escape:
                        _shouldClose = true;
                        break;

                    // Enter pressed
                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                        HandleOkButton(0);
                        break;
                }
            }

            if (_shouldClose)
            {  // Close this dialog
                Close();
            }

            // Draw our control
            Rect rect = EditorGUILayout.BeginVertical();

            if (_showErrorIssue)
            {
                EditorGUILayout.Space(12);
                EditorGUILayout.LabelField(_description);
            }

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Type the name of Dialog");
            EditorGUILayout.Space(2);
            if (_infoClass != null)
            {
                _infoClass.InputName = EditorGUILayout.TextField("", _infoClass.InputName);
                if (_pathAsset.Length == 0)
                {
                    EditorGUILayout.Space(4);
                    EditorGUILayout.LabelField("This is will be the path of the Dialog Asset");
                    EditorGUILayout.Space(2);
                    _infoClass.InputAssetPath = EditorGUILayout.TextField("", _infoClass.InputName);
                    EditorGUILayout.Space(4);
                    EditorGUILayout.LabelField("This is will be the path of the Dialog Script");
                    EditorGUILayout.Space(2);
                    _infoClass.InputScriptPath = EditorGUILayout.TextField("", _infoClass.InputName);
                    EditorGUILayout.Space(12);
                }
                else
                {
                    _infoClass.InputAssetPath = _pathAsset;
                    _infoClass.InputScriptPath = _pathAsset;
                }
            }

            if (_currentState == -1)
            {
                // Draw OK / Cancel buttons
                Rect r = EditorGUILayout.GetControlRect();
                r.width /= 2;
                if (UnityEngine.GUI.Button(r, _okButton))
                {
                    HandleOkButton(0);
                }

                r.x += r.width;
                if (UnityEngine.GUI.Button(r, _cancelButton))
                {
                    _infoClass = null;
                    _shouldClose = true;
                    _showErrorIssue = false;
                    _currentState = -1;
                }
            }

            EditorGUILayout.Space(8);
            EditorGUILayout.EndVertical();

            // Force change size of the window
            if (rect.width != 0 && minSize != rect.size)
            {
                minSize = maxSize = rect.size;
            }

            // Set dialog position next to mouse position
            if (!_initializedPosition)
            {
                int additionWidth = 50;
                Rect main = EditorGUIUtility.GetMainWindowPosition();
                position = new Rect(
                    main.x + (main.width - position.width - additionWidth) / 2,
                    main.y + (main.height - position.height) / 2,
                    position.width + additionWidth,
                    position.height);
                _initializedPosition = true;
            }
        }

        /// <summary>
        /// Returns text player entered, or null if player cancelled the dialog.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static DialogInputNameDialog Show(string title, Func<InfoClassDialogPath, int, int> okAction)
        {
            DialogInputNameDialog window = CreateInstance<DialogInputNameDialog>();
            window.titleContent = new GUIContent(title);
            window._okButton = "Create";
            window._cancelButton = "Cancel";
            //
            window._infoClass = InfoClassDialogPath.Create("NameDialog");
            window._createAction += okAction;
            //
            window.ShowModal();
            return window;
        }

        public static DialogInputNameDialog ShowWithPathAsset(string title, string pathAsset, Func<InfoClassDialogPath, int, int> okAction)
        {
            DialogInputNameDialog window = CreateInstance<DialogInputNameDialog>();
            window.titleContent = new GUIContent(title);
            window._okButton = "Create";
            window._cancelButton = "Cancel";
            window._pathAsset = pathAsset;
            //
            string[] ls = pathAsset.Split("/");
            //
            window._infoClass = InfoClassDialogPath.Create(ls[ls.Length - 1]);
            window._createAction += okAction;
            //
            window.ShowModal();
            return window;
        }
    }
}
#endif