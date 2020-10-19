using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Popcron.Settings
{
    public class Settings : ScriptableObject
    {
        private static Settings current;

        public const string SettingsAssetName = "Settings Settings.asset";

        /// <summary>
        /// The current settings data being used.
        /// </summary>
        public static Settings Current
        {
            get
            {
                if (!current)
                {
                    current = GetOrCreate();
                }

                return current;
            }
        }

        public string pathToClass = "Assets/Settings.cs";
        public TextAsset classTemplate;
        public Property[] properties = { };

        /// <summary>
        /// Returns an existing console settings asset, or creates a new one if none exist.
        /// </summary>
        public static Settings GetOrCreate()
        {
            //find from resources
            string name = Path.GetFileNameWithoutExtension(SettingsAssetName);
            Settings settings = Resources.Load<Settings>(name);
            bool exists = settings;
            if (!exists)
            {
                //no console settings asset exists yet, so create one
                settings = CreateInstance<Settings>();
                settings.name = name;
            }

#if UNITY_EDITOR
            if (!exists)
            {
                //ensure the resources folder exists
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }

                //make a file here
                string path = $"Assets/Resources/{SettingsAssetName}";
                AssetDatabase.CreateAsset(settings, path);
                AssetDatabase.Refresh();
            }
#endif

            return settings;
        }
    }

    [Serializable]
    public class Property
    {
        public string name;
        public string description;
        public string type;
        public string defaultValue;
        public bool isExpanded;
    }
}