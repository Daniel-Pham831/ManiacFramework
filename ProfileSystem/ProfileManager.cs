using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Maniac.Utils.Extension;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Maniac.ProfileSystem
{
    public class ProfileManager
    {
        const string PROFILE_DATA_FOLDER_NAME = "Profiles";
        const string PROFILE_DATA_FILE_NAME_SUFFIX = "json";

        public static Dictionary<string, ProfileRecord> recordsCache = new Dictionary<string, ProfileRecord>();
        public static bool ShouldBinaryFormat { get; set; } = false;

        public static T Get<T>() where T : ProfileRecord
        {
            if (recordsCache.TryGetValue(typeof(T).Name, out ProfileRecord value))
                return value as T;

            return Load<T>();
        }

        public static void Save(ProfileRecord record)
        {
            string typeName = record.GetType().Name;
            string savePath = $"{GetProfileFolderPath()}/{typeName}.{PROFILE_DATA_FILE_NAME_SUFFIX}";

            string json = JsonConvert.SerializeObject(record,Formatting.Indented);
            if (ShouldBinaryFormat)
            {

            }
            else
            {
                File.WriteAllText(savePath, json);
            }

            SaveCache(record);
        }

        public static T Load<T>() where T : ProfileRecord
        {
            ProfileRecord record = null;
            string savePath = $"{GetProfileFolderPath()}/{typeof(T).Name}.{PROFILE_DATA_FILE_NAME_SUFFIX}";
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                record = JsonConvert.DeserializeObject(json, typeof(T)) as T;
            }
            else
            {
                record = Activator.CreateInstance<T>();
            }

            SaveCache(record);

            return record as T;
        }

        private static void SaveCache(ProfileRecord record)
        {
            string typeName = record.GetType().Name;
            if (!recordsCache.ContainsKey(typeName))
            {
                recordsCache.Add(typeName, record);
            }

            recordsCache[typeName] = record;
        }

        public static void LoadAllProfileRecordsIntoCache()
        {
            Dictionary<string, Type> typeList = GetListTypeBaseOnProfile();

            string savePath = $"{GetProfileFolderPath()}";
            string[] filePaths = Directory.GetFiles(savePath, $"*.{PROFILE_DATA_FILE_NAME_SUFFIX}");
            foreach (string file in filePaths)
            {
                string json = File.ReadAllText(file);
                string fileName = Path.GetFileName(file);
                string fileType = fileName.Replace($".{PROFILE_DATA_FILE_NAME_SUFFIX}", "");
                try
                {
                    if (typeList[fileType] == null) continue;

                    ProfileRecord record = JsonConvert.DeserializeObject(json, typeList[fileType]) as ProfileRecord;
                    recordsCache.Add(record.GetType().Name, record);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogWarning(e);
                    File.Delete(file);
                }
            }
        }

        private static Dictionary<string, Type> GetListTypeBaseOnProfile()
        {
            Dictionary<string, Type> objects = new Dictionary<string, Type>();
            IEnumerable<Type> types = typeof(ProfileRecord).GetAllSubclasses();
            foreach (Type type in types)
            {
                objects.Add(type.Name, type);
            }
            return objects;
        }

        private static string GetProfileFolderPath()
        {
            string folderPath = $"{Application.persistentDataPath}/{PROFILE_DATA_FOLDER_NAME}";
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            return folderPath;
        }

#if UNITY_EDITOR
        [MenuItem("Tools/Clear Game Data")]
        public static void ClearAllRecords()
        {
            bool isClear = EditorUtility.DisplayDialog("Clear All Data?", "All Personal data will be clear. Do you want it?", "Yes", "No");
            if (isClear)
            {
                ClearGameData();
                UnityEngine.Debug.Log("Done");
            }
        }

        [MenuItem("Tools/Open Persistant folder")]
        public static void OpenPersistantFolder()
        {
            ProcessStartInfo StartInformation = new ProcessStartInfo();
            StartInformation.FileName = Application.persistentDataPath;

            Process.Start(StartInformation);
        }
#endif

        private static void ClearPersistantData()
        {
            //persitent folder
            DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }
        
        public static void ClearGameData()
        {
            ClearPersistantData();
            PlayerPrefs.DeleteAll();
        }
    }
}