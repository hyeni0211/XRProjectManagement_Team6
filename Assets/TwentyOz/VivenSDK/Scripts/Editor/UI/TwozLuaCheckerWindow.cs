using TwentyOz.VivenSDK.Scripts.Core.Lua;
using TwentyOz.VivenSDK.Scripts.Editor.Lua;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityToolbarExtender;

namespace TwentyOz.VivenSDK.Scripts.Editor.UI
{
    public class TwozLuaCheckerWindow : EditorWindow
    {
        
        static TwozLuaCheckerWindow()
        {
            ToolbarExtender.LeftToolbarGUI.Add(() => {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("Check Lua Script", "Validate Lua Script")))
                {
                    ShowWindow();
                }
            });
        }
    
        [MenuItem("Window/UI Toolkit/TwozLuaCheckerWindow")]
        public static void ShowWindow()
        {
            var wnd = GetWindow<TwozLuaCheckerWindow>();
            wnd.titleContent = new GUIContent("TwozLuaCheckerWindow");
        }
    
        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;
        
            var luaScriptField = new ObjectField
            {
                name = "lua-script-field",
                label = "Target Lua Script",
                objectType = typeof(VivenLuaBehaviour),
                allowSceneObjects = true
            };
            root.Add(luaScriptField);

            var button = new Button
            {
                name = "check-button",
                text = "Check"
            };
            button.clicked += OnCheckButtonClicked;
            root.Add(button);
        
            var resultLabel = new Label
            {
                name = "result-label",
                text = "검사 결과"
            };
            root.Add(resultLabel);
        
            var resultText = new Label
            {
                name = "result-text"
            };

            root.Add(resultText);
            
            // Save Current VisualTreeAsset
            
        }

        private void OnCheckButtonClicked()
        {
            VisualElement root = rootVisualElement;
            ObjectField luaScriptField = root.Q<ObjectField>("lua-script-field");
            Label resultText = root.Q<Label>("result-text");
        
            VivenLuaBehaviour target = luaScriptField.value as VivenLuaBehaviour;
            if (!target) return;
        
            var validationResult = TwozLuaChecker.Check(target);
            resultText.text = validationResult ? "Lua Script에서 문제가 발견되지 않았습니다." : "Lua Script에서 문제가 발견되었습니다.";
        }
    }
}
