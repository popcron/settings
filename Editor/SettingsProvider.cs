using UnityEditor;
using UnityEngine.UIElements;

namespace Popcron.Settings
{
    public class SettingsSettingsProvider : SettingsProvider
    {
        private SerializedObject settings;

        public SettingsSettingsProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope)
        {

        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            settings = new SerializedObject(Settings.Current);
        }

        public override void OnGUI(string searchContext)
        {
            SettingsInspector.Show(settings);
        }

        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            SettingsSettingsProvider provider = new SettingsSettingsProvider("Project/Settings", SettingsScope.Project)
            {
                keywords = new string[] { "settings" }
            };

            return provider;
        }
    }
}
