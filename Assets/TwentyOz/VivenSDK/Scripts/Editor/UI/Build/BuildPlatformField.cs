using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace TwentyOz.VivenSDK.Scripts.Editor.UI.Build
{
    [UxmlElement]
    internal partial class BuildPlatformField : VisualElement
    {
        [UxmlAttribute]
        public string TargetPlatformName {
            get
            {
                if (UseToggleElement != null)
                {
                    return UseToggleElement.label;
                }
                return null;   
            }
            set
            {
                if (UseToggleElement != null)
                {
                    UseToggleElement.label = value;
                }
            }
        }
        
        public Toggle UseToggleElement { get; set; } 
        
        public ObjectField SceneField { get; set; }
        
        public string ScenePath { get; set; }
        
        public Button ManualBuildButton { get; set; }
        
        public Toggle ManualBuildToggle { get; set; }
        
        public BuildPlatformField()
        {
            UseToggleElement = new Toggle("Label")
            {
                name = "use-toggle"
            };
            SceneField = new ObjectField()
            {
                name = "scene-field",
                objectType = typeof(SceneAsset)
            };
            ManualBuildButton = new Button()
            {
                name = "manual-build-button",
                text = "Manual Build"
            };
            ManualBuildToggle = new Toggle("Is manually built")
            {
                name = "manual-build-toggle"
            };
            
            UseToggleElement.RegisterValueChangedCallback(evt =>
            {
                var isOn = evt.newValue;
                SceneField.SetEnabled(isOn);
                ManualBuildButton.SetEnabled(isOn);
            });
            Add(UseToggleElement);

            SceneField.SetEnabled(UseToggleElement.value);
            SceneField.RegisterValueChangedCallback(evt =>
            {
                ScenePath = AssetDatabase.GetAssetPath(evt.newValue);
            });
            Add(SceneField);

            ManualBuildButton.SetEnabled(UseToggleElement.value);
            Add(ManualBuildButton);
            
            ManualBuildToggle.SetEnabled(false);
            Add(ManualBuildToggle);
        }
    }
}