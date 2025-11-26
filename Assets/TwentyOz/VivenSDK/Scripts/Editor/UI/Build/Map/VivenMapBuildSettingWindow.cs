using System.Collections.Generic;
using System.IO;
using TwentyOz.VivenSDK.Scripts.Editor.Build;
using TwentyOz.VivenSDK.Scripts.Editor.Build.VMap;
using TwentyOz.VivenSDK.Scripts.Editor.Core;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.Serialization;

namespace TwentyOz.VivenSDK.Scripts.Editor.UI.Build.Map
{
    /// <summary>
    /// Viven Map 빌드를 위한 설정창 (UI Toolkit 기반)
    /// 
    /// 이 클래스는 Viven Map 빌드를 위한 UI를 제공합니다. 크게 두 가지 빌드 방식을 지원합니다:
    /// 1. 통합 빌드: 모든 타겟 플랫폼에 대해 한 번에 빌드를 진행합니다.
    /// 2. 메뉴얼 빌드: 각 플랫폼별로 개별 빌드를 진행하고, 나중에 패키징합니다.
    /// </summary>
    public class VivenMapBuildSettingWindow : EditorWindow
    {
        [SerializeField] public VMapBuildSetting vMapBuildSetting;
        [SerializeField] private VivenMapBuildData mVivenMapBuildData;

        // USS 파일 로드
        [SerializeField] private StyleSheet mStyleSheet;

        private const string IntegratedBuildTabName = "통합 빌드";
        private const string ManualBuildTabName = "메뉴얼 빌드";

        private int _mSelectedConfigIndex;
        private VisualElement _mRootElement;
        private VisualElement _mCurrentTabContent;
        private ScrollView _mMainScrollView;

        private ToolbarButton _mIntegratedBuildToggle;
        private ToolbarButton _mManualBuildToggle;

        // Addressables 설정 관련 변수
        private bool _mOriginalDebugBuildLayout;

        public static void ShowWindow()
        {
            var wnd = GetWindow<VivenMapBuildSettingWindow>();
            wnd.titleContent = new GUIContent("Viven Map Build Setting");
            wnd.minSize = new Vector2(700, 700);
        }

        private void OnEnable()
        {
            // 스타일시트가 로드되었는지 확인
            if (!mStyleSheet)
            {
                Debug.LogWarning("VivenMapBuildSettingWindow.uss 파일을 찾을 수 없습니다.");
            }
        }

        private void CreateGUI()
        {
            // 루트 요소 생성
            _mRootElement = rootVisualElement;
            _mRootElement.AddToClassList("root-element");

            // USS 스타일시트 적용
            if (mStyleSheet)
            {
                _mRootElement.styleSheets.Add(mStyleSheet);
            }

            // 초기 데이터 확인
            if (vMapBuildSetting == null || mVivenMapBuildData == null)
            {
                CreateMissingDataUI();
                return;
            }

            // 현재 씬을 각 플랫폼의 타겟 씬으로 자동 설정
            SetCurrentSceneAsTargetForAllPlatforms();

            // 메인 스크롤뷰 생성 (전체 UI를 스크롤 가능하게)
            _mMainScrollView = new ScrollView(ScrollViewMode.Vertical);
            _mMainScrollView.AddToClassList("main-scroll-view");
            _mRootElement.Add(_mMainScrollView);

            // 기본 설정 섹션 (공통)
            CreateBasicSettingsSection();

            // 탭 UI 생성
            CreateTabBar();

            // 탭 콘텐츠 영역 생성
            _mCurrentTabContent = new VisualElement();
            _mCurrentTabContent.name = "tab-content";
            _mCurrentTabContent.AddToClassList("tab-content");
            _mMainScrollView.Add(_mCurrentTabContent);

            // 기본적으로 통합 빌드 탭 선택
            ShowIntegratedBuildTab();
        }

        private void CreateMissingDataUI()
        {
            _mRootElement.Clear();

            // 경고 메시지 추가
            var warningBox = new HelpBox("VMapBuildSetting 또는 VivenMapBuildData가 설정되지 않았습니다.",
                HelpBoxMessageType.Error);
            _mRootElement.Add(warningBox);

            var spacer = new VisualElement();
            spacer.style.height = 10;
            _mRootElement.Add(spacer);

            // VMapBuildSetting 필드
            var settingField = new ObjectField("Viven Map Build Setting");
            settingField.objectType = typeof(VMapBuildSetting);
            settingField.value = vMapBuildSetting;
            settingField.RegisterValueChangedCallback(evt =>
            {
                vMapBuildSetting = evt.newValue as VMapBuildSetting;
                if (vMapBuildSetting != null && mVivenMapBuildData != null)
                {
                    CreateGUI();
                }
            });
            _mRootElement.Add(settingField);

            // VivenMapBuildData 필드
            var dataField = new ObjectField("Viven Map Build Data");
            dataField.objectType = typeof(VivenMapBuildData);
            dataField.value = mVivenMapBuildData;
            dataField.RegisterValueChangedCallback(evt =>
            {
                mVivenMapBuildData = evt.newValue as VivenMapBuildData;
                if (vMapBuildSetting != null && mVivenMapBuildData != null)
                {
                    CreateGUI();
                }
            });
            _mRootElement.Add(dataField);
        }

        private void CreateTabBar()
        {
            // 탭 바 생성
            var tabBar = new Toolbar();
            tabBar.AddToClassList("unity-toolbar");

            // 통합 빌드 탭
            _mIntegratedBuildToggle = new ToolbarButton();
            _mIntegratedBuildToggle.AddToClassList("toolbar-button");
            _mIntegratedBuildToggle.text = IntegratedBuildTabName;
            _mIntegratedBuildToggle.clicked += () =>
            {
                {
                    _mIntegratedBuildToggle.AddToClassList("toolbar-button--selected");
                    _mManualBuildToggle.RemoveFromClassList("toolbar-button--selected");
                    ShowIntegratedBuildTab();
                }
            };
            tabBar.Add(_mIntegratedBuildToggle);

            // 메뉴얼 빌드 탭
            _mManualBuildToggle = new ToolbarButton();
            _mManualBuildToggle.AddToClassList("toolbar-button");
            _mManualBuildToggle.text = ManualBuildTabName;
            _mManualBuildToggle.clicked += () =>
            {
                {
                    _mManualBuildToggle.AddToClassList("toolbar-button--selected");
                    _mIntegratedBuildToggle.RemoveFromClassList("toolbar-button--selected");
                    ShowManualBuildTab();
                }
            };
            tabBar.Add(_mManualBuildToggle);

            // 기본적으로 통합 빌드 탭 선택
            _mIntegratedBuildToggle.AddToClassList("toolbar-button--selected");
            _mManualBuildToggle.RemoveFromClassList("toolbar-button--selected");

            _mMainScrollView.Add(tabBar);
        }

        private void ShowIntegratedBuildTab()
        {
            // 콘텐츠 영역 초기화
            _mCurrentTabContent.Clear();

            // 플랫폼 설정 섹션
            var platformSettingsSection = CreatePlatformSettingsSection();
            _mCurrentTabContent.Add(platformSettingsSection);

            // 빌드 버튼 컨테이너
            var buildButtonContainer = new VisualElement();
            buildButtonContainer.AddToClassList("build-button-container");

            // 빌드 버튼 - 중요 버튼
            var buildButton = CreateButton("통합 빌드 시작", OnBuildPlatformBundleClicked, true);

            buildButtonContainer.Add(buildButton);
            _mCurrentTabContent.Add(buildButtonContainer);
        }

        private void ShowManualBuildTab()
        {
            // 콘텐츠 영역 초기화
            _mCurrentTabContent.Clear();

            // 플랫폼별 메뉴얼 빌드 카드
            var cardsContainer = CreateManualBuildCardsContainer();
            _mCurrentTabContent.Add(cardsContainer);

            // 버튼 컨테이너
            var buttonContainer = CreateManualBuildButtonsContainer();
            _mCurrentTabContent.Add(buttonContainer);
        }

        // 메뉴얼 빌드 카드 컨테이너 생성 함수
        private VisualElement CreateManualBuildCardsContainer()
        {
            var container = new VisualElement();
            container.AddToClassList("section");

            container.Add(CreateManualBuildPlatformCard("Windows", VivenPlatform.WIN));
            container.Add(CreateManualBuildPlatformCard("MacOS", VivenPlatform.MAC));
            container.Add(CreateManualBuildPlatformCard("Android", VivenPlatform.AOS));
            container.Add(CreateManualBuildPlatformCard("iOS", VivenPlatform.IOS));

            return container;
        }

        // 메뉴얼 빌드 버튼 컨테이너 생성 함수
        private VisualElement CreateManualBuildButtonsContainer()
        {
            var container = new VisualElement();
            container.AddToClassList("manual-buttons-container");

            // 패키지 빌드 버튼 - 중요 버튼
            var packageButton = CreateButton("패키지 메뉴얼 빌드", PackageManualBuild, true);
            packageButton.AddToClassList("flex-grow");
            packageButton.AddToClassList("margin-right");

            // 폴더 정리 버튼 - 일반 버튼
            var clearButton = CreateButton("메뉴얼 빌드 폴더 정리", ClearManualBuildFolders, false);
            clearButton.AddToClassList("flex-grow");
            clearButton.AddToClassList("margin-left");

            container.Add(packageButton);
            container.Add(clearButton);

            return container;
        }

        // 버튼 생성 함수 (재사용 가능)
        private Button CreateButton(string text, System.Action clickAction, bool isPrimary)
        {
            var button = new Button(clickAction) { text = text };
            button.AddToClassList(isPrimary ? "primary-build-button" : "normal-button");
            return button;
        }

        private void CreateBasicSettingsSection()
        {
            // 섹션 헤더
            var header = CreateSectionHeader("Viven 맵 빌드 설정");
            _mMainScrollView.Add(header);

            // 섹션 컨테이너
            var sectionContainer = new VisualElement();
            sectionContainer.AddToClassList("section");
            sectionContainer.AddToClassList("section-container");

            // 맵 이름 필드
            sectionContainer.Add(CreateMapNameField());

            // 빌드 설정 선택
            if (vMapBuildSetting.configList.Count > 0)
            {
                // Config 설정 UI 생성
                var configSectionContainer = new VisualElement();
                configSectionContainer.AddToClassList("config-section-container");


                var configField = CreateBuildConfigField(configSectionContainer);
                sectionContainer.Add(configField);

                sectionContainer.Add(configSectionContainer);

                // Config 설정 UI 생성
                vMapBuildSetting.configList[_mSelectedConfigIndex].BuildTabUi(configSectionContainer);
            }

            // 맵 오브젝트 목록 섹션
            sectionContainer.Add(CreateMapObjectsSection());

            _mMainScrollView.Add(sectionContainer);
        }

        // 섹션 헤더 생성 함수
        private Label CreateSectionHeader(string headerText)
        {
            var header = new Label(headerText);
            header.AddToClassList("header");
            return header;
        }

        // 맵 이름 필드 생성 함수
        private TextField CreateMapNameField()
        {
            var mapNameField = new TextField("맵 이름");
            mapNameField.value = string.IsNullOrEmpty(mVivenMapBuildData.mapName)
                ? SceneManager.GetActiveScene().name
                : mVivenMapBuildData.mapName;
            mapNameField.RegisterValueChangedCallback(evt => { mVivenMapBuildData.mapName = evt.newValue; });
            return mapNameField;
        }

        // 빌드 설정 필드 생성 함수
        private PopupField<string> CreateBuildConfigField(VisualElement configSectionContainer)
        {
            var configField = new PopupField<string>("빌드 설정",
                GetConfigNameList(),
                _mSelectedConfigIndex);

            configField.RegisterValueChangedCallback(_ =>
            {
                // 인덱스가 실제로 변경된 경우에만 업데이트
                var newIndex = configField.index;
                if (_mSelectedConfigIndex == newIndex)
                {
                    return;
                }

                _mSelectedConfigIndex = newIndex;
                configSectionContainer.Clear();
                vMapBuildSetting.configList[_mSelectedConfigIndex].BuildTabUi(configSectionContainer);
            });

            return configField;
        }

        // 맵 오브젝트 섹션 생성 함수
        private VisualElement CreateMapObjectsSection()
        {
            var container = new VisualElement();

            // 맵 오브젝트 목록 섹션 헤더
            var mapObjectsHeader = new Label("맵 오브젝트");
            mapObjectsHeader.AddToClassList("map-objects-header");
            container.Add(mapObjectsHeader);

            // SerializedObject를 통한 맵 오브젝트 목록 바인딩
            var so = new SerializedObject(mVivenMapBuildData);
            var mapObjectsProperty = so.FindProperty("mapObjects");

            var propertyField = new PropertyField(mapObjectsProperty);
            propertyField.bindingPath = "mapObjects";
            propertyField.Bind(so);

            container.Add(propertyField);
            return container;
        }

        // 플랫폼 아이콘 생성
        private Image CreatePlatformIcon(string platformName)
        {
            var platformIcon = new Image();
            platformIcon.scaleMode = ScaleMode.ScaleToFit;
            platformIcon.AddToClassList("platform-icon");

            // 플랫폼별 아이콘 설정
            platformIcon.image = platformName switch
            {
                "Windows" => EditorGUIUtility.IconContent("BuildSettings.Metro.Small").image,
                "MacOS" => EditorGUIUtility.IconContent("BuildSettings.Standalone.Small").image,
                "Android" => EditorGUIUtility.IconContent("BuildSettings.Android.Small").image,
                "iOS" => EditorGUIUtility.IconContent("BuildSettings.iPhone.Small").image,
                _ => EditorGUIUtility.IconContent("BuildSettings.DefaultIcon.Small").image
            };

            return platformIcon;
        }

        // 플랫폼 아이콘 컨테이너 생성
        private VisualElement CreatePlatformIconContainer(string platformName)
        {
            var iconContainer = new VisualElement();
            iconContainer.AddToClassList("platform-icon-container");

            // 플랫폼 아이콘 생성 및 추가
            var platformIcon = CreatePlatformIcon(platformName);
            iconContainer.Add(platformIcon);

            return iconContainer;
        }

        private VisualElement CreatePlatformSettingsSection()
        {
            // 콘텐츠
            var content = new VisualElement();
            content.AddToClassList("section");

            // 플랫폼 카드 추가
            content.Add(CreatePlatformCard("Windows", mVivenMapBuildData.WIN));
            content.Add(CreatePlatformCard("MacOS", mVivenMapBuildData.MAC));
            content.Add(CreatePlatformCard("Android", mVivenMapBuildData.AOS));
            content.Add(CreatePlatformCard("iOS", mVivenMapBuildData.IOS));

            return content;
        }

        private VisualElement CreatePlatformCard(string platformName, PlatformWrapper platformData)
        {
            // 카드 컨테이너
            var card = new VisualElement();
            card.AddToClassList("platform-card");

            // 한 줄 레이아웃 컨테이너
            var rowContainer = new VisualElement();
            rowContainer.AddToClassList("row-container");

            // 플랫폼 아이콘 컨테이너
            var iconContainer = CreatePlatformIconContainer(platformName);

            // 플랫폼 이름 레이블
            var platformLabel = CreatePlatformLabel(platformName);

            // 씬 선택 필드 (라벨 없이) - 먼저 선언
            var sceneField = CreateSceneField(platformData);

            // 활성화 토글 (레이블 없이)
            var toggle = CreatePlatformToggle(platformData, sceneField);

            // 컨트롤이 활성화 상태일 때만 씬 필드 활성화
            sceneField.SetEnabled(platformData.enabled);

            // 한 줄에 모든 요소 추가
            rowContainer.Add(iconContainer);
            rowContainer.Add(platformLabel);
            rowContainer.Add(toggle);
            rowContainer.Add(sceneField);

            card.Add(rowContainer);

            return card;
        }

        // 플랫폼 레이블 생성
        private Label CreatePlatformLabel(string platformName)
        {
            var platformLabel = new Label(platformName);
            platformLabel.AddToClassList("platform-label");
            return platformLabel;
        }

        // 씬 선택 필드 생성
        private ObjectField CreateSceneField(PlatformWrapper platformData)
        {
            var sceneField = new ObjectField
            {
                label = null, // 라벨 제거
                objectType = typeof(SceneAsset)
            };
            sceneField.AddToClassList("platform-scene-field");
            sceneField.value = string.IsNullOrEmpty(platformData.targetPath)
                ? null
                : AssetDatabase.LoadAssetAtPath<SceneAsset>(platformData.targetPath);
            sceneField.RegisterValueChangedCallback(evt =>
            {
                platformData.targetPath = evt.newValue != null
                    ? AssetDatabase.GetAssetPath(evt.newValue)
                    : string.Empty;
            });
            return sceneField;
        }

        // 플랫폼 토글 생성
        private Toggle CreatePlatformToggle(PlatformWrapper platformData, ObjectField sceneField)
        {
            var toggle = new Toggle();
            toggle.value = platformData.enabled;
            toggle.AddToClassList("platform-toggle");
            toggle.RegisterValueChangedCallback(evt =>
            {
                platformData.enabled = evt.newValue;
                // 씬 선택 필드 활성화/비활성화
                sceneField.SetEnabled(evt.newValue);
            });
            return toggle;
        }

        private List<string> GetConfigNameList()
        {
            var result = new List<string>();
            foreach (var config in vMapBuildSetting.configList)
            {
                result.Add(config.ConfigName);
            }

            return result;
        }

        /// <summary>
        /// Addressables의 빌드 보고서 자동 표시 기능을 비활성화합니다.
        /// 빌드 시 Addressables 레포트 창이 자동으로 열리는 것을 방지하고,
        /// 빌드 레이아웃 보고서 생성도 비활성화합니다.
        /// </summary>
        /// <param name="disable">비활성화할지 여부(true: 비활성화, false: 원래 설정으로 복원)</param>
        private void DisableAddressablesReport(bool disable)
        {
            try
            {
                // 현재 설정을 저장 (처음 비활성화할 때만 저장)
                if (disable)
                {
                    // GenerateBuildLayout은 ProjectConfigData를 통해 접근 가능
                    _mOriginalDebugBuildLayout = ProjectConfigData.GenerateBuildLayout;
                }

                if (disable)
                {
                    // Debug Build Layout 비활성화 - 빌드 레이아웃 보고서를 생성하지 않음
                    ProjectConfigData.GenerateBuildLayout = false;

                    // 에디터 설정에 변경 사항 저장
                    EditorPrefs.SetBool("Addressables.BuildAddressablesWithPlayerBuild", false);
                    EditorPrefs.SetBool("Addressables.ReportViewerEnabled", false);
                }
                else
                {
                    // 원래 설정으로 복원
                    ProjectConfigData.GenerateBuildLayout = _mOriginalDebugBuildLayout;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Addressables 설정 변경 중 오류가 발생했습니다: {e.Message}");
            }
        }

        /// <summary>
        /// 특정 플랫폼에 대한 메뉴얼 빌드를 실행합니다.
        /// 선택된 빌드 설정의 이벤트 핸들러를 VivenBuildManager에 등록한 후 빌드를 시작합니다.
        /// </summary>
        /// <param name="platform">빌드할 타겟 플랫폼</param>
        private void OnManualBuildClicked(VivenPlatform platform)
        {
            if (mVivenMapBuildData == null)
            {
                Debug.LogError("VivenMapBuildData is null");
                return;
            }

            var currentConfig = vMapBuildSetting.configList[_mSelectedConfigIndex];

            // Addressables 레포트 창 자동 표시 비활성화
            DisableAddressablesReport(true);

            try
            {
                // 빌드 이벤트 설정
                VivenBuildManager.OnBuildVMapCondition = currentConfig.Condition;
                VivenBuildManager.OnBuildVMapStart = currentConfig.Start;
                VivenBuildManager.OnBuildVMapEnd = currentConfig.End;

                var mVivenMapBuildProfiles = vMapBuildSetting.contentBuildProfiles;
                
                // ContentProperty 추가 (기본)
                mVivenMapBuildData.vivenAdditionalProperty = currentConfig.GetContentProperties();

                // Viven Map 빌드 실행
                var result = VivenBuildManager.ManualBuildVMap(
                    VivenBuildType.vmap,
                    mVivenMapBuildData,
                    mVivenMapBuildProfiles,
                    platform
                );
                
                // 패키징 완료 후 VivenMapBuildWindow 열기
                BuildResultWindow.ShowWindow(result);

                // 빌드 탭 갱신
                ShowManualBuildTab();
            }
            finally
            {
                // 설정 복원
                DisableAddressablesReport(false);
            }
        }

        /// <summary>
        /// 플랫폼별 빌드 상태 아이콘을 생성합니다. 
        /// 이미 빌드된 플랫폼은 체크 아이콘이 표시됩니다.
        /// </summary>
        /// <param name="platform">대상 플랫폼</param>
        /// <returns>빌드 상태를 표시하는 이미지 요소</returns>
        private Image CreateBuildStatusIcon(VivenPlatform platform)
        {
            var isBuildExists = VivenBuildManager.ManualBuildFolderExists(platform);
            var statusIcon = new Image();
            statusIcon.scaleMode = ScaleMode.ScaleToFit;
            statusIcon.AddToClassList("build-status-icon");
            statusIcon.image = EditorGUIUtility.IconContent(isBuildExists ? "TestPassed" : "TestNormal").image;
            statusIcon.tooltip = isBuildExists ? "빌드 완료" : "빌드 필요";

            return statusIcon;
        }

        /// <summary>
        /// 플랫폼별 빌드 버튼을 생성합니다.
        /// </summary>
        /// <param name="platform">대상 플랫폼</param>
        /// <returns>생성된 빌드 버튼</returns>
        private Button CreateManualBuildButton(VivenPlatform platform)
        {
            var buildButton = CreateButton("빌드", () => OnManualBuildClicked(platform), false);
            buildButton.AddToClassList("build-button");

            return buildButton;
        }

        private VisualElement CreateManualBuildPlatformCard(string platformName, VivenPlatform platform)
        {
            // 타겟 씬 정보 얻기
            var platformData = GetPlatformData(platform);

            // 카드 컨테이너
            var card = new VisualElement();
            card.AddToClassList("platform-card");

            // 한 줄 레이아웃 컨테이너
            var rowContainer = new VisualElement();
            rowContainer.AddToClassList("manual-card-row");

            // 플랫폼 아이콘 컨테이너
            var iconContainer = CreatePlatformIconContainer(platformName);

            // 플랫폼 이름 레이블
            var platformLabel = CreatePlatformLabel(platformName);

            // 씬 선택 필드 (라벨 없이)
            var sceneField = CreateSceneField(platformData);

            // 빌드 상태 표시 (아이콘)
            var statusIcon = CreateBuildStatusIcon(platform);

            // 빌드 버튼
            var buildButton = CreateManualBuildButton(platform);

            // 요소 순서대로 추가
            rowContainer.Add(iconContainer);
            rowContainer.Add(platformLabel);
            rowContainer.Add(sceneField);
            rowContainer.Add(statusIcon);
            rowContainer.Add(buildButton);

            card.Add(rowContainer);

            return card;
        }

        /// <summary>
        /// 메뉴얼 빌드 폴더를 정리합니다.
        /// 모든 플랫폼에 대한 메뉴얼 빌드 폴더를 제거하고 UI를 갱신합니다.
        /// </summary>
        private void ClearManualBuildFolders()
        {
            VivenBuildManager.ClearManualBuildFolder(VivenPlatform.WIN);
            VivenBuildManager.ClearManualBuildFolder(VivenPlatform.MAC);
            VivenBuildManager.ClearManualBuildFolder(VivenPlatform.AOS);
            VivenBuildManager.ClearManualBuildFolder(VivenPlatform.IOS);
            VivenBuildManager.ClearManualBuildFolder(VivenPlatform.WEB);

            // UI 갱신을 위해 창 다시 그리기
            ShowManualBuildTab();
        }

        /// <summary>
        /// 메뉴얼 빌드 결과물을 패키징합니다.
        /// 개별 플랫폼별 메뉴얼 빌드 결과물을 하나의 패키지로 만듭니다.
        /// </summary>
        private void PackageManualBuild()
        {
            if (mVivenMapBuildData == null)
            {
                Debug.LogError("VivenMapBuildData is null");
                return;
            }

            var result = VivenBuildManager.ManualBuildBundle(VivenBuildType.vmap, mVivenMapBuildData);
            Debug.Log("패키징 완료: " + result);
            ShowManualBuildTab();

            // 해당 Window 닫기
            Close();

            // 패키징 완료 후 VivenMapBuildWindow 열기
            BuildResultWindow.ShowWindow(result);
        }

        /// <summary>
        /// 현재 씬을 모든 플랫폼의 타겟 씬으로 설정하는 함수입니다.
        /// 이 함수는 창이 처음 열릴 때 자동으로 호출되어 현재 씬을 모든 플랫폼 빌드의 대상으로 설정합니다.
        /// 맵 이름이 설정되지 않은 경우 현재 씬 이름을 맵 이름으로 사용합니다.
        /// </summary>
        private void SetCurrentSceneAsTargetForAllPlatforms()
        {
            var currentScenePath = SceneManager.GetActiveScene().path;

            if (string.IsNullOrEmpty(currentScenePath))
            {
                Debug.LogWarning("현재 씬이 저장되지 않았습니다. 씬을 저장한 후 다시 시도해주세요.");
                return;
            }

            // 각 플랫폼에 현재 씬 할당
            mVivenMapBuildData.WIN.targetPath = currentScenePath;
            mVivenMapBuildData.MAC.targetPath = currentScenePath;
            mVivenMapBuildData.AOS.targetPath = currentScenePath;
            mVivenMapBuildData.IOS.targetPath = currentScenePath;

            // 맵 이름이 없으면 현재 씬 이름으로 설정
            if (string.IsNullOrEmpty(mVivenMapBuildData.mapName))
            {
                mVivenMapBuildData.mapName = SceneManager.GetActiveScene().name;
            }
        }

        /// <summary>
        /// 주어진 플랫폼 열거형 값에 해당하는 PlatformWrapper 객체를 반환합니다.
        /// 이 함수는 VivenPlatform 열거형을 사용하여 mVivenMapBuildData에서 
        /// 해당 플랫폼에 대한 설정을 가져옵니다.
        /// </summary>
        /// <param name="platform">대상 플랫폼</param>
        /// <returns>플랫폼에 대한 PlatformWrapper 객체</returns>
        private PlatformWrapper GetPlatformData(VivenPlatform platform)
        {
            return platform switch
            {
                VivenPlatform.WIN => mVivenMapBuildData.WIN,
                VivenPlatform.MAC => mVivenMapBuildData.MAC,
                VivenPlatform.AOS => mVivenMapBuildData.AOS,
                VivenPlatform.IOS => mVivenMapBuildData.IOS,
                _ => mVivenMapBuildData.WIN
            };
        }

        /// <summary>
        /// 모든 통합 빌드를 실행합니다. 이 함수는 다음과 같은 과정을 수행합니다:
        /// 1. 빌드 이벤트 핸들러 등록
        /// 2. 맵 웹 빌드 비활성화 (맵은 웹 빌드를 지원하지 않음)
        /// 3. 추가 프로퍼티 설정
        /// 4. Addressables 레포트 자동 표시 비활성화
        /// 5. 빌드 시작/종료 시간 기록 및 로깅
        /// 6. VivenBuildManager를 통한 빌드 실행
        /// </summary>
        private void OnBuildPlatformBundleClicked()
        {
            if (mVivenMapBuildData == null)
            {
                Debug.LogError("VivenMapBuildData is null");
                return;
            }

            var currentConfig = vMapBuildSetting.configList[_mSelectedConfigIndex];

            // Addressables 레포트 창 자동 표시 비활성화
            DisableAddressablesReport(true);

            try
            {
                // 빌드 이벤트 설정
                VivenBuildManager.OnBuildVMapCondition = currentConfig.Condition;
                VivenBuildManager.OnBuildVMapStart = currentConfig.Start;
                VivenBuildManager.OnBuildVMapEnd = currentConfig.End;

                var mVivenMapBuildProfiles = vMapBuildSetting.contentBuildProfiles;

                // 맵의 경우 WEB 빌드 비활성화
                mVivenMapBuildData.GetPlatformSceneWrapper(VivenPlatform.WEB).enabled = false;

                // ContentProperty 추가 (기본)
                mVivenMapBuildData.vivenAdditionalProperty = currentConfig.GetContentProperties();

                // Viven Map 빌드 실행
                var result = VivenBuildManager.TryBuildBundle(
                    VivenBuildType.vmap,
                    mVivenMapBuildData,
                    mVivenMapBuildProfiles
                );

                // 패키징 완료 후 VivenMapBuildWindow 열기
                BuildResultWindow.ShowWindow(result);
            }
            finally
            {
                // 설정 복원
                DisableAddressablesReport(false);
            }
        }
    }
}