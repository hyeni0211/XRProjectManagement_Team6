using System;
using System.IO;
using TwentyOz.VivenSDK.Scripts.Editor.Build;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TwentyOz.VivenSDK.Scripts.Editor.UI.Build
{
    public class BuildResultWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;
        
        [SerializeField]
        private BuildResultSO m_BuildResultBindingSO = default;
        
        [SerializeField] 
        private StyleSheet m_StyleSheet;
        
        [MenuItem("Window/UI Toolkit/Build Result Window")]
        public static void ShowExample()
        {
            BuildResultWindow window = GetWindow<BuildResultWindow>();
            window.titleContent = new GUIContent("빌드 결과");
            window.Show();
        }
        
        public static void ShowWindow(BuildResultData buildResultData)
        {
            BuildResultWindow window = GetWindow<BuildResultWindow>();
            window.titleContent = new GUIContent("빌드 결과");
            window.m_BuildResultBindingSO.From(buildResultData);
            window.Init();
            window.Show();
        }
        
        private void OnEnable()
        {
            // USS 파일 로드
            m_StyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/TwentyOz/VivenSDK/Scripts/Editor/UI/Build/BuildResultWindow.uss");

            // 스타일시트가 로드되었는지 확인
            if (m_StyleSheet == null)
            {
                Debug.LogWarning("BuildResultWindow.uss 파일을 찾을 수 없습니다.");
            }
        }
        
        public void CreateGUI()
        {
            // 각 에디터 창에는 루트 VisualElement 객체가 포함됨
            VisualElement root = rootVisualElement;
            
            // 스타일시트 적용
            if (m_StyleSheet != null)
            {
                root.styleSheets.Add(m_StyleSheet);
            }
            
            if (m_VisualTreeAsset != null)
            {
                // UXML 인스턴스화
                VisualElement containerFromUXML = m_VisualTreeAsset.Instantiate();
                root.Add(containerFromUXML);
                
                // 빌드 성공/실패 상태 라벨 설정
                var buildLabel = containerFromUXML.Q<Label>("is-build-success");
                buildLabel.text = m_BuildResultBindingSO.IsBuildSuccess ? "빌드가 완료되었습니다." : "빌드에 실패했습니다.";
                buildLabel.AddToClassList("build-status-label");
                buildLabel.AddToClassList(m_BuildResultBindingSO.IsBuildSuccess ? "success-label" : "error-label");
                
                // 빌드 완료 시간 포맷팅
                var buildTimeField = containerFromUXML.Q<TextField>("build-time");
                if (buildTimeField != null)
                {
                    buildTimeField.value = FormatDateTime(m_BuildResultBindingSO._buildCompletionTime);
                }
                
                // 빌드 소요 시간 포맷팅
                var buildDurationField = containerFromUXML.Q<TextField>("build-duration");
                if (buildDurationField != null)
                {
                    buildDurationField.value = FormatDuration(m_BuildResultBindingSO._buildDuration);
                }
                
                // 출력 경로 필드 설정
                var outputPathField = containerFromUXML.Q<TextField>("output-path");
                if (outputPathField != null)
                {
                    outputPathField.value = m_BuildResultBindingSO._buildOutputPath;
                }
                
                // 폴더 열기 버튼
                var openFolderButton = containerFromUXML.Q<Button>("open-folder-button");
                if (openFolderButton != null)
                {
                    openFolderButton.clicked += OpenOutputFolder;
                    openFolderButton.SetEnabled(!string.IsNullOrEmpty(m_BuildResultBindingSO._buildOutputPath) && 
                                                Directory.Exists(m_BuildResultBindingSO._buildOutputPath));
                }
                
                // 닫기 버튼
                var closeButton = containerFromUXML.Q<Button>("close-button");
                closeButton.clicked += Close;
            }
            else
            {
                // UXML이 없는 경우 직접 UI 구성
                CreateUIWithoutUXML(root);
            }
        }
        
        private void CreateUIWithoutUXML(VisualElement root)
        {
            // 메인 컨테이너
            var mainContainer = new VisualElement();
            mainContainer.AddToClassList("main-container");
            root.Add(mainContainer);
            
            // 헤더 섹션
            var headerContainer = new VisualElement();
            headerContainer.AddToClassList("header-container");
            mainContainer.Add(headerContainer);
            
            // 빌드 상태 라벨
            var buildLabel = new Label(m_BuildResultBindingSO.IsBuildSuccess ? "빌드가 완료되었습니다." : "빌드에 실패했습니다.");
            buildLabel.AddToClassList("build-status-label");
            buildLabel.AddToClassList(m_BuildResultBindingSO.IsBuildSuccess ? "success-label" : "error-label");
            headerContainer.Add(buildLabel);
            
            // 빌드 정보 섹션
            var infoContainer = new VisualElement();
            infoContainer.AddToClassList("info-container");
            mainContainer.Add(infoContainer);
            
            // 타겟 이름 필드
            infoContainer.Add(CreateInfoField("콘텐츠 이름", m_BuildResultBindingSO._buildTarget));
            
            // 빌드 완료 시간
            infoContainer.Add(CreateInfoField("빌드 완료 시간", FormatDateTime(m_BuildResultBindingSO._buildCompletionTime)));
            
            // 빌드 소요 시간
            infoContainer.Add(CreateInfoField("빌드 소요 시간", FormatDuration(m_BuildResultBindingSO._buildDuration)));
            
            // 출력 경로
            infoContainer.Add(CreateInfoField("출력 경로", m_BuildResultBindingSO._buildOutputPath));
            
            // 빌드 메시지
            var messageContainer = new VisualElement();
            messageContainer.AddToClassList("message-container");
            mainContainer.Add(messageContainer);
            
            var messageLabel = new Label("빌드 메시지:");
            messageLabel.AddToClassList("message-title");
            messageContainer.Add(messageLabel);
            
            var messageContent = new Label(m_BuildResultBindingSO != null ? m_BuildResultBindingSO._buildMessage : "");
            messageContent.AddToClassList("message-content");
            messageContainer.Add(messageContent);
            
            // 버튼 컨테이너
            var buttonContainer = new VisualElement();
            buttonContainer.AddToClassList("button-container");
            mainContainer.Add(buttonContainer);
            
            // 폴더 열기 버튼
            var openFolderButton = new Button(OpenOutputFolder);
            openFolderButton.text = "폴더 열기";
            openFolderButton.AddToClassList("normal-button");
            openFolderButton.SetEnabled(!string.IsNullOrEmpty(m_BuildResultBindingSO._buildOutputPath) && 
                                        Directory.Exists(m_BuildResultBindingSO._buildOutputPath));
            buttonContainer.Add(openFolderButton);
            
            // 닫기 버튼
            var closeButton = new Button(Close);
            closeButton.text = "닫기";
            closeButton.AddToClassList("close-button");
            buttonContainer.Add(closeButton);
        }
        
        // 정보 필드 생성 헬퍼 함수
        private VisualElement CreateInfoField(string label, string value)
        {
            var container = new VisualElement();
            container.AddToClassList("field-container");
            
            var labelElement = new Label(label + ":");
            labelElement.AddToClassList("field-label");
            container.Add(labelElement);
            
            var valueElement = new Label(value);
            valueElement.AddToClassList("field-value");
            container.Add(valueElement);
            
            return container;
        }
        
        // 날짜 시간 포맷팅
        private string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        
        // 소요 시간 포맷팅
        private string FormatDuration(float seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            
            int hours = (int)timeSpan.TotalHours;
            int minutes = timeSpan.Minutes;
            int secs = timeSpan.Seconds;
            int milliseconds = timeSpan.Milliseconds;
            
            // 매우 짧은 시간 (0.5초 미만)인 경우 밀리초만 표시
            if (seconds < 0.5f)
            {
                return $"{milliseconds}밀리초";
            }
            
            string result = "";
            
            if (hours > 0)
            {
                result += $"{hours}시간 ";
            }
            
            if (hours > 0 || minutes > 0)
            {
                result += $"{minutes}분 ";
            }
            
            // 항상 초 단위는 표시하되, 밀리초 단위도 추가
            if (milliseconds > 0)
            {
                result += $"{secs}.{milliseconds:D3}초";
            }
            else
            {
                result += $"{secs}초";
            }
            
            return result;
        }
        
        // 출력 폴더 열기 기능
        private void OpenOutputFolder()
        {
            if (string.IsNullOrEmpty(m_BuildResultBindingSO._buildOutputPath))
            {
                Debug.LogWarning("유효한 출력 경로가 없습니다.");
                return;
            }
            
            string path = m_BuildResultBindingSO._buildOutputPath;
            
            if (Directory.Exists(path))
            {
                EditorUtility.RevealInFinder(path);
            }
            else
            {
                Debug.LogWarning($"폴더를 찾을 수 없습니다: {path}");
                
                // 만약 파일 경로라면 파일이 있는 디렉토리 열기
                string directory = Path.GetDirectoryName(path);
                if (Directory.Exists(directory))
                {
                    EditorUtility.RevealInFinder(directory);
                }
            }
        }

        public void Init()
        {
            if (m_BuildResultBindingSO == null)
                return;
                
            // VisualElement가 없거나 준비되지 않은 경우 처리
            if (rootVisualElement == null)
                return;
            
            var buildLabel = rootVisualElement.Q<Label>("is-build-success");
            if (buildLabel != null)
            {
                buildLabel.text = m_BuildResultBindingSO.IsBuildSuccess ? "빌드가 완료되었습니다." : "빌드에 실패했습니다.";
                buildLabel.RemoveFromClassList("success-label");
                buildLabel.RemoveFromClassList("error-label");
                buildLabel.AddToClassList(m_BuildResultBindingSO.IsBuildSuccess ? "success-label" : "error-label");
            }
            
            var buildMessageLabel = rootVisualElement.Q<Label>("build-message"); 
            if (buildMessageLabel != null)
            {
                buildMessageLabel.text = m_BuildResultBindingSO._buildMessage;
            }
            
            var targetNameField = rootVisualElement.Q<TextField>("target-name");
            if (targetNameField != null)
            {
                targetNameField.value = m_BuildResultBindingSO._buildTarget;
            }
            
            // 빌드 완료 시간 업데이트
            var buildTimeField = rootVisualElement.Q<TextField>("build-time");
            if (buildTimeField != null)
            {
                buildTimeField.value = FormatDateTime(m_BuildResultBindingSO._buildCompletionTime);
            }
            
            // 빌드 소요 시간 업데이트
            var buildDurationField = rootVisualElement.Q<TextField>("build-duration");
            if (buildDurationField != null)
            {
                buildDurationField.value = FormatDuration(m_BuildResultBindingSO._buildDuration);
            }
            
            // 출력 경로 업데이트
            var outputPathField = rootVisualElement.Q<TextField>("output-path");
            if (outputPathField != null)
            {
                outputPathField.value = m_BuildResultBindingSO._buildOutputPath;
            }
            
            // 폴더 열기 버튼 활성화/비활성화
            var openFolderButton = rootVisualElement.Q<Button>("open-folder-button");
            if (openFolderButton != null)
            {
                openFolderButton.clicked += OpenOutputFolder;
                openFolderButton.SetEnabled(!string.IsNullOrEmpty(m_BuildResultBindingSO._buildOutputPath) && 
                                           Directory.Exists(m_BuildResultBindingSO._buildOutputPath));
            }
        }
    }
}
