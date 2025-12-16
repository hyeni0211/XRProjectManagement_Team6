using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TwentyOz.VivenSDK.Scripts.Core.Lua;

namespace ConveyorGunGame.Editor
{
    /// <summary>
    /// Conveyor Gun Game 씬 자동 설정 에디터 도구
    /// VivenLuaBehaviour Injection 자동 설정
    /// 한글→영어 이름 변환, 공 종류별 풀링 지원
    /// </summary>
    public class ConveyorGameSetup : EditorWindow
    {
        #region Constants

        private const string MENU_PATH = "Conveyor Gun Game";
        private const string SCRIPTS_PATH = "Assets/1_ConveyorGunGame/Scripts";

        // 오브젝트 이름
        private const string GUN_NAME = "Gun_Cosmic_Retro_Blaster_1";
        private const string SHOOT_POINT_NAME = "Shoot Start Point";
        private const string SPAWN_POINT_NAME = "PropSpawnPoint";
        private const string GAME_MANAGER_NAME = "GameManager";
        private const string SPAWN_MANAGER_NAME = "SpawnManager";
        private const string TARGET_POOL_NAME = "TargetPool";
        private const string BULLET_POOL_NAME = "BulletPool";
        private const string GAME_CANVAS_NAME = "GameCanvas";

        // Lua 스크립트 이름
        private const string SHOOTING_GUN_SCRIPT = "ShootingGun";
        private const string BULLET_SCRIPT = "Bullet";
        private const string TARGET_SCRIPT = "Target";
        private const string GAME_MANAGER_SCRIPT = "ConveyorGameManager";
        private const string SPAWN_MANAGER_SCRIPT = "TargetSpawnManager";

        // 풀 설정
        private const int DEFAULT_TARGET_POOL_SIZE = 15;
        private const int DEFAULT_BULLET_POOL_SIZE = 25;
        private const int TARGETS_PER_BALL_TYPE = 10; // 공 종류별 풀 개수

        #endregion

        #region Korean to English Mapping

        // 한글→영어 이름 변환 딕셔너리
        private static readonly Dictionary<string, string> KoreanToEnglishMap = new Dictionary<string, string>
        {
            // 공 종류
            { "골프공", "GolfBall" },
            { "농구공", "BasketBall" },
            { "럭비골", "RugbyBall" },
            { "배구공", "VolleyBall" },
            { "볼링공", "BowlingBall" },
            { "비치볼", "BeachBall" },
            { "야구공", "BaseBall" },
            { "축구공", "SoccerBall" },
            { "테니스공", "TennisBall" },

            // UI 요소
            { "게임종료", "GameOver" },
            { "점수", "Score" },
            { "준비!", "Ready" },

            // 추가 UI (필요 시)
            { "시작", "Start" },
            { "재시작", "Restart" },
            { "타이머", "Timer" },
            { "메시지", "Message" }
        };

        // 공 종류 목록 (영어)
        private static readonly string[] BallTypes = new string[]
        {
            "GolfBall",
            "BasketBall",
            "RugbyBall",
            "VolleyBall",
            "BowlingBall",
            "BeachBall",
            "BaseBall",
            "SoccerBall",
            "TennisBall"
        };

        // 공 종류 한글 목록
        private static readonly string[] BallTypesKorean = new string[]
        {
            "골프공",
            "농구공",
            "럭비골",
            "배구공",
            "볼링공",
            "비치볼",
            "야구공",
            "축구공",
            "테니스공"
        };

        #endregion

        #region Editor Window

        private int targetPoolSize = DEFAULT_TARGET_POOL_SIZE;
        private int bulletPoolSize = DEFAULT_BULLET_POOL_SIZE;
        private int targetsPerBallType = TARGETS_PER_BALL_TYPE;
        private Vector2 scrollPosition;
        private bool showBallTypeStatus = true;

        [MenuItem(MENU_PATH + "/Setup Scene")]
        public static void ShowWindow()
        {
            var window = GetWindow<ConveyorGameSetup>("Conveyor Game Setup");
            window.minSize = new Vector2(450, 700);
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayout.Label("Conveyor Gun Game Setup", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            // === 한글 이름 변환 섹션 ===
            GUI.backgroundColor = new Color(1f, 0.9f, 0.7f);
            EditorGUILayout.BeginVertical("box");
            GUI.backgroundColor = Color.white;
            GUILayout.Label("🔤 Korean → English Name Converter", EditorStyles.boldLabel);

            if (GUILayout.Button("Rename All Korean Objects to English", GUILayout.Height(30)))
            {
                RenameKoreanToEnglish();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);

            // === 풀 크기 설정 ===
            GUILayout.Label("Pool Settings", EditorStyles.boldLabel);
            targetsPerBallType = EditorGUILayout.IntSlider("Targets Per Ball Type", targetsPerBallType, 5, 20);
            EditorGUILayout.LabelField($"  → Total Targets: {targetsPerBallType * BallTypes.Length} (9 types × {targetsPerBallType})");
            bulletPoolSize = EditorGUILayout.IntSlider("Bullet Pool Size", bulletPoolSize, 10, 50);
            EditorGUILayout.Space(10);

            // === 상태 확인 ===
            GUILayout.Label("Current Status", EditorStyles.boldLabel);
            DrawStatusCheck();
            EditorGUILayout.Space(5);

            // 공 종류별 상태
            showBallTypeStatus = EditorGUILayout.Foldout(showBallTypeStatus, "Ball Type Pool Status");
            if (showBallTypeStatus)
            {
                DrawBallTypeStatus();
            }
            EditorGUILayout.Space(10);

            // === 설정 버튼 ===
            GUILayout.Label("Setup Actions", EditorStyles.boldLabel);

            if (GUILayout.Button("1. Create Manager Objects", GUILayout.Height(25)))
            {
                CreateManagerObjects();
            }

            if (GUILayout.Button("2. Create Target Pool (By Ball Type)", GUILayout.Height(25)))
            {
                CreateTargetPoolByBallType(targetsPerBallType);
            }

            if (GUILayout.Button("3. Create Bullet Pool", GUILayout.Height(25)))
            {
                CreateBulletPool(bulletPoolSize);
            }

            if (GUILayout.Button("4. Setup Gun (ShootingGun.lua)", GUILayout.Height(25)))
            {
                SetupGun();
            }

            if (GUILayout.Button("5. Setup All Injections", GUILayout.Height(25)))
            {
                SetupAllInjections();
            }

            EditorGUILayout.Space(20);

            // === AUTO SETUP ALL ===
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("AUTO SETUP ALL", GUILayout.Height(50)))
            {
                AutoSetupAll();
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(10);

            // === 유틸리티 버튼 ===
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("Fix Null Injection Arrays", GUILayout.Height(25)))
            {
                FixAllNullInjectionArrays();
            }

            if (GUILayout.Button("Find & Setup UI Elements (GameCanvas)", GUILayout.Height(25)))
            {
                SetupUIElements();
            }

            // === UI 구조 정리 버튼 ===
            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("🔧 Organize UI Structure (Fix GameUIPanel)", GUILayout.Height(30)))
            {
                OrganizeUIStructure();
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndScrollView();
        }

        private void DrawBallTypeStatus()
        {
            EditorGUILayout.BeginVertical("box");

            GameObject targetPool = GameObject.Find(TARGET_POOL_NAME);
            if (targetPool == null)
            {
                EditorGUILayout.LabelField("  TargetPool not found");
            }
            else
            {
                // 각 공 종류별 개수 세기
                Dictionary<string, int> ballCounts = new Dictionary<string, int>();
                foreach (var ballType in BallTypes)
                {
                    ballCounts[ballType] = 0;
                }
                ballCounts["Unknown"] = 0;

                for (int i = 0; i < targetPool.transform.childCount; i++)
                {
                    string childName = targetPool.transform.GetChild(i).name;
                    bool found = false;

                    foreach (var ballType in BallTypes)
                    {
                        if (childName.StartsWith(ballType))
                        {
                            ballCounts[ballType]++;
                            found = true;
                            break;
                        }
                    }

                    if (!found) ballCounts["Unknown"]++;
                }

                // 표시
                foreach (var ballType in BallTypes)
                {
                    int count = ballCounts[ballType];
                    string status = count >= targetsPerBallType ? "✓" : "✗";
                    EditorGUILayout.LabelField($"  {status} {ballType}: {count}/{targetsPerBallType}");
                }

                if (ballCounts["Unknown"] > 0)
                {
                    EditorGUILayout.LabelField($"  ? Unknown: {ballCounts["Unknown"]}");
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawStatusCheck()
        {
            EditorGUILayout.BeginVertical("box");

            DrawObjectStatus(GUN_NAME, GameObject.Find(GUN_NAME));
            DrawObjectStatus(SPAWN_POINT_NAME, GameObject.Find(SPAWN_POINT_NAME));
            DrawObjectStatus(GAME_MANAGER_NAME, GameObject.Find(GAME_MANAGER_NAME));
            DrawObjectStatus(SPAWN_MANAGER_NAME, GameObject.Find(SPAWN_MANAGER_NAME));
            DrawObjectStatus(TARGET_POOL_NAME, GameObject.Find(TARGET_POOL_NAME));
            DrawObjectStatus(BULLET_POOL_NAME, GameObject.Find(BULLET_POOL_NAME));

            EditorGUILayout.EndVertical();
        }

        private void DrawObjectStatus(string name, GameObject obj)
        {
            EditorGUILayout.BeginHorizontal();

            if (obj != null)
            {
                EditorGUILayout.LabelField("✓ " + name, EditorStyles.boldLabel);

                var luaBehaviour = obj.GetComponent<VivenLuaBehaviour>();
                if (luaBehaviour != null && luaBehaviour.luaScript != null)
                {
                    EditorGUILayout.LabelField("[" + luaBehaviour.luaScript.name + "]", GUILayout.Width(150));
                }
                else if (luaBehaviour != null)
                {
                    EditorGUILayout.LabelField("[No Script]", GUILayout.Width(150));
                }
            }
            else
            {
                EditorGUILayout.LabelField("✗ " + name + " (Not Found)");
            }

            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Menu Items

        [MenuItem(MENU_PATH + "/Auto Setup All", false, 0)]
        public static void AutoSetupAllMenu()
        {
            AutoSetupAll();
        }

        [MenuItem(MENU_PATH + "/Create Manager Objects", false, 20)]
        public static void CreateManagerObjectsMenu()
        {
            CreateManagerObjects();
        }

        [MenuItem(MENU_PATH + "/Create Target Pool (15)", false, 21)]
        public static void CreateTargetPoolMenu()
        {
            CreateTargetPool(DEFAULT_TARGET_POOL_SIZE);
        }

        [MenuItem(MENU_PATH + "/Create Bullet Pool (25)", false, 22)]
        public static void CreateBulletPoolMenu()
        {
            CreateBulletPool(DEFAULT_BULLET_POOL_SIZE);
        }

        [MenuItem(MENU_PATH + "/Setup All Injections", false, 40)]
        public static void SetupAllInjectionsMenu()
        {
            SetupAllInjections();
        }

        [MenuItem(MENU_PATH + "/Fix Null Injection Arrays", false, 60)]
        public static void FixNullArraysMenu()
        {
            FixAllNullInjectionArrays();
        }

        [MenuItem(MENU_PATH + "/Setup Conveyor Belt", false, 30)]
        public static void SetupConveyorBeltMenu()
        {
            SetupConveyorBelt();
        }

        [MenuItem(MENU_PATH + "/Setup Target Collector (Dustbin)", false, 31)]
        public static void SetupTargetCollectorMenu()
        {
            SetupTargetCollector();
        }

        #endregion

        #region Auto Setup

        public static void AutoSetupAll()
        {
            Debug.Log("=== Conveyor Gun Game Auto Setup Started ===");

            // 0. 한글 이름을 영어로 변환
            RenameKoreanToEnglish();

            // 1. 매니저 오브젝트 생성
            CreateManagerObjects();

            // 2. 공 종류별 풀 생성 (9종 x 10개 = 90개)
            CreateTargetPoolByBallType(TARGETS_PER_BALL_TYPE);
            CreateBulletPool(DEFAULT_BULLET_POOL_SIZE);

            // 3. 총 설정
            SetupGun();

            // 4. 컨베이어 벨트 설정
            SetupConveyorBelt();

            // 4.5. 공 수거 영역 설정
            SetupTargetCollector();

            // 5. UI 요소 설정
            SetupUIElements();

            // 5. 모든 Injection 설정
            SetupAllInjections();

            // 6. Null 배열 수정
            FixAllNullInjectionArrays();

            // 씬 변경 표시
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            Debug.Log("=== Conveyor Gun Game Auto Setup Completed ===");
            EditorUtility.DisplayDialog("Setup Complete",
                "Conveyor Gun Game setup completed!\n\n" +
                "- Korean names converted to English\n" +
                "- Ball type pools created (9 types × 10 each)\n" +
                "- UI elements configured\n\n" +
                "Please save the scene (Ctrl+S).", "OK");
        }

        #endregion

        #region Korean to English Rename

        /// <summary>
        /// 씬 내 모든 한글 오브젝트 이름을 영어로 변환
        /// </summary>
        public static void RenameKoreanToEnglish()
        {
            Debug.Log("=== Renaming Korean objects to English ===");

            int renamedCount = 0;
            var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (var obj in allObjects)
            {
                // 씬에 있는 오브젝트만 처리
                if (!obj.scene.IsValid()) continue;

                string originalName = obj.name;

                // 한글 이름인지 확인하고 변환
                if (KoreanToEnglishMap.TryGetValue(originalName, out string englishName))
                {
                    obj.name = englishName;
                    Debug.Log($"Renamed: '{originalName}' → '{englishName}'");
                    EditorUtility.SetDirty(obj);
                    renamedCount++;
                }
            }

            Debug.Log($"=== Renamed {renamedCount} objects ===");
        }

        /// <summary>
        /// 오브젝트 이름을 한글에서 영어로 변환 (필요시)
        /// </summary>
        private static string ConvertKoreanToEnglish(string koreanName)
        {
            if (KoreanToEnglishMap.TryGetValue(koreanName, out string englishName))
            {
                return englishName;
            }
            return koreanName;
        }

        /// <summary>
        /// 영어 이름을 한글로 역변환 (검색용)
        /// </summary>
        private static string ConvertEnglishToKorean(string englishName)
        {
            foreach (var pair in KoreanToEnglishMap)
            {
                if (pair.Value == englishName)
                {
                    return pair.Key;
                }
            }
            return englishName;
        }

        #endregion

        #region UI Elements Setup

        /// <summary>
        /// GameCanvas 및 Canvas_left 내 UI 요소들을 찾아서 설정
        /// 없는 요소는 자동 생성
        /// </summary>
        public static void SetupUIElements()
        {
            Debug.Log("=== Setting up UI Elements ===");

            GameObject gameCanvas = GameObject.Find(GAME_CANVAS_NAME);
            GameObject canvasLeft = GameObject.Find("Canvas_left");

            if (gameCanvas == null)
            {
                Debug.LogWarning("GameCanvas not found!");
                return;
            }

            // UI 요소 이름 매핑 (한글 → 영어로 검색)
            var uiMappings = new Dictionary<string, string[]>
            {
                // Injection 이름 → 검색할 이름들 (영어 먼저, 한글 fallback)
                { "ScoreTextObject", new[] { "Score", "ScoreText", "점수" } },
                { "TimerTextObject", new[] { "Timer", "TimerText", "타이머" } },
                { "StartUIPanel", new[] { "StartUIPanel", "StartUI", "Ready", "ReadyText", "준비!" } },
                { "GameUIPanel", new[] { "GameUIPanel", "GameUI" } },
                { "GameOverUIPanel", new[] { "GameOverUIPanel", "GameOverUI", "GameOver", "GameOverText", "게임종료" } },
                { "StartButtonObject", new[] { "StartButton", "Start", "시작" } },
                { "RestartButtonObject", new[] { "RestartButton", "Restart", "재시작" } },
                { "FinalScoreTextObject", new[] { "FinalScore", "FinalScoreText", "GameOverText", "게임종료" } },
                { "GuideTextObject", new[] { "GuideText", "Guide", "가이드", "TargetGuide" } }
            };

            // 각 UI 요소 찾기 (GameCanvas와 Canvas_left 양쪽에서)
            foreach (var mapping in uiMappings)
            {
                GameObject foundObj = null;

                // 먼저 GameCanvas에서 찾기
                foreach (var searchName in mapping.Value)
                {
                    foundObj = FindChildRecursive(gameCanvas.transform, searchName);
                    if (foundObj != null) break;
                }

                // 못 찾았으면 Canvas_left에서 찾기
                if (foundObj == null && canvasLeft != null)
                {
                    foreach (var searchName in mapping.Value)
                    {
                        foundObj = FindChildRecursive(canvasLeft.transform, searchName);
                        if (foundObj != null) break;
                    }
                }

                if (foundObj != null)
                {
                    Debug.Log($"Found UI: {mapping.Key} = '{foundObj.name}'");
                }
                else
                {
                    Debug.LogWarning($"UI not found: {mapping.Key} (searched: {string.Join(", ", mapping.Value)})");
                }
            }

            // 필수 UI 요소가 없으면 자동 생성
            CreateMissingUIElements(gameCanvas);

            Debug.Log("=== UI Elements setup completed ===");
        }

        /// <summary>
        /// 누락된 필수 UI 요소 자동 생성
        /// </summary>
        private static void CreateMissingUIElements(GameObject gameCanvas)
        {
            // GuideText가 없으면 생성
            GameObject guideText = FindChildRecursive(gameCanvas.transform, "GuideText");
            if (guideText == null)
            {
                guideText = CreateTextElement(gameCanvas.transform, "GuideText", "맞춰야 할 공", new Vector2(0, 250));
                Debug.Log("Created missing UI: GuideText");
            }

            // TimerText가 없으면 생성
            GameObject timerText = FindInMultipleCanvases(gameCanvas, null, "Timer", "TimerText", "타이머");
            if (timerText == null)
            {
                timerText = CreateTextElement(gameCanvas.transform, "TimerText", "01:00", new Vector2(0, 300));
                Debug.Log("Created missing UI: TimerText");
            }

            // FinalScoreText가 없으면 생성 (GameOverText와 별도로)
            GameObject finalScoreText = FindChildRecursive(gameCanvas.transform, "FinalScoreText");
            GameObject gameOverText = FindChildRecursive(gameCanvas.transform, "GameOverText");
            if (finalScoreText == null && gameOverText == null)
            {
                finalScoreText = CreateTextElement(gameCanvas.transform, "FinalScoreText", "최종 점수: 0", new Vector2(0, -100));
                Debug.Log("Created missing UI: FinalScoreText");
            }

            // StartButton이 없으면 생성
            GameObject startButton = FindInMultipleCanvases(gameCanvas, null, "StartButton", "Start", "시작");
            if (startButton == null)
            {
                startButton = CreateButtonElement(gameCanvas.transform, "StartButton", "시작", new Vector2(0, 0));
                Debug.Log("Created missing UI: StartButton");
            }

            // RestartButton이 없으면 생성
            GameObject restartButton = FindInMultipleCanvases(gameCanvas, null, "RestartButton", "Restart", "재시작");
            if (restartButton == null)
            {
                restartButton = CreateButtonElement(gameCanvas.transform, "RestartButton", "재시작", new Vector2(0, -80));
                restartButton.SetActive(false); // 초기에는 비활성화
                Debug.Log("Created missing UI: RestartButton");
            }
        }

        /// <summary>
        /// 버튼 UI 요소 생성
        /// </summary>
        private static GameObject CreateButtonElement(Transform parent, string name, string buttonText, Vector2 position)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent);
            buttonObj.layer = LayerMask.NameToLayer("UI");

            // RectTransform 설정
            var rectTransform = buttonObj.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = new Vector2(200, 60);
            rectTransform.localScale = Vector3.one;

            // Image (버튼 배경)
            var image = buttonObj.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(0.2f, 0.6f, 0.9f, 1f);

            // Button 컴포넌트
            var button = buttonObj.AddComponent<UnityEngine.UI.Button>();
            button.targetGraphic = image;

            // 텍스트 자식 오브젝트 생성
            GameObject textChild = new GameObject("Text");
            textChild.transform.SetParent(buttonObj.transform);
            textChild.layer = LayerMask.NameToLayer("UI");

            var textRect = textChild.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            textRect.localScale = Vector3.one;

            // TextMeshProUGUI 추가 시도
            var tmpType = System.Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");
            if (tmpType != null)
            {
                var tmpComponent = textChild.AddComponent(tmpType);
                var textProperty = tmpType.GetProperty("text");
                if (textProperty != null)
                {
                    textProperty.SetValue(tmpComponent, buttonText);
                }

                var fontSizeProperty = tmpType.GetProperty("fontSize");
                if (fontSizeProperty != null)
                {
                    fontSizeProperty.SetValue(tmpComponent, 24f);
                }

                var alignmentProperty = tmpType.GetProperty("alignment");
                if (alignmentProperty != null)
                {
                    alignmentProperty.SetValue(tmpComponent, 514); // Center
                }

                var colorProperty = tmpType.GetProperty("color");
                if (colorProperty != null)
                {
                    colorProperty.SetValue(tmpComponent, Color.white);
                }
            }
            else
            {
                var textComponent = textChild.AddComponent<UnityEngine.UI.Text>();
                textComponent.text = buttonText;
                textComponent.fontSize = 24;
                textComponent.alignment = TextAnchor.MiddleCenter;
                textComponent.color = Color.white;
            }

            EditorUtility.SetDirty(buttonObj);
            return buttonObj;
        }

        /// <summary>
        /// TextMeshPro UI 요소 생성
        /// </summary>
        private static GameObject CreateTextElement(Transform parent, string name, string defaultText, Vector2 position)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent);
            textObj.layer = LayerMask.NameToLayer("UI");

            // RectTransform 설정
            var rectTransform = textObj.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = new Vector2(400, 100);
            rectTransform.localScale = Vector3.one;

            // TextMeshProUGUI 추가 시도 (TMPro가 있는 경우)
            var tmpType = System.Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");
            if (tmpType != null)
            {
                var tmpComponent = textObj.AddComponent(tmpType);
                var textProperty = tmpType.GetProperty("text");
                if (textProperty != null)
                {
                    textProperty.SetValue(tmpComponent, defaultText);
                }

                var fontSizeProperty = tmpType.GetProperty("fontSize");
                if (fontSizeProperty != null)
                {
                    fontSizeProperty.SetValue(tmpComponent, 36f);
                }

                var alignmentProperty = tmpType.GetProperty("alignment");
                if (alignmentProperty != null)
                {
                    // TextAlignmentOptions.Center = 514
                    alignmentProperty.SetValue(tmpComponent, 514);
                }
            }
            else
            {
                // 폴백: 기본 UI.Text
                var textComponent = textObj.AddComponent<UnityEngine.UI.Text>();
                textComponent.text = defaultText;
                textComponent.fontSize = 36;
                textComponent.alignment = TextAnchor.MiddleCenter;
                textComponent.color = Color.white;
            }

            EditorUtility.SetDirty(textObj);
            return textObj;
        }

        /// <summary>
        /// UI 구조를 정리하여 패널별로 분리
        /// StartUIPanel, GameUIPanel, GameOverUIPanel 생성 및 UI 요소 재배치
        /// </summary>
        [MenuItem(MENU_PATH + "/Organize UI Structure", false, 50)]
        public static void OrganizeUIStructure()
        {
            Debug.Log("=== Organizing UI Structure ===");

            GameObject gameCanvas = GameObject.Find(GAME_CANVAS_NAME);
            if (gameCanvas == null)
            {
                Debug.LogError("GameCanvas not found!");
                return;
            }

            // 1. 패널 오브젝트 생성 또는 찾기
            GameObject startUIPanel = FindOrCreateUIPanel(gameCanvas.transform, "StartUIPanel");
            GameObject gameUIPanel = FindOrCreateUIPanel(gameCanvas.transform, "GameUIPanel");
            GameObject gameOverUIPanel = FindOrCreateUIPanel(gameCanvas.transform, "GameOverUIPanel");

            // 2. UI 요소들을 적절한 패널로 이동
            // StartUIPanel: StartButton
            MoveUIElementToPanel(gameCanvas.transform, startUIPanel.transform, "StartButton", "Start", "시작");

            // GameUIPanel: Score, Timer, GuideText, BallGuide들
            MoveUIElementToPanel(gameCanvas.transform, gameUIPanel.transform, "Score", "ScoreText", "점수");
            MoveUIElementToPanel(gameCanvas.transform, gameUIPanel.transform, "Timer", "TimerText", "타이머");
            MoveUIElementToPanel(gameCanvas.transform, gameUIPanel.transform, "GuideText", "Guide", "가이드");

            // BallGuide 오브젝트들도 GameUIPanel로 이동
            foreach (var ballType in BallTypes)
            {
                MoveUIElementToPanel(gameCanvas.transform, gameUIPanel.transform, ballType);
            }
            foreach (var ballTypeKr in BallTypesKorean)
            {
                MoveUIElementToPanel(gameCanvas.transform, gameUIPanel.transform, ballTypeKr);
            }

            // GameOverUIPanel: GameOver, FinalScore, RestartButton
            MoveUIElementToPanel(gameCanvas.transform, gameOverUIPanel.transform, "GameOver", "GameOverText", "게임종료");
            MoveUIElementToPanel(gameCanvas.transform, gameOverUIPanel.transform, "FinalScore", "FinalScoreText");
            MoveUIElementToPanel(gameCanvas.transform, gameOverUIPanel.transform, "RestartButton", "Restart", "재시작");

            // 3. 초기 상태 설정
            startUIPanel.SetActive(true);
            gameUIPanel.SetActive(false);
            gameOverUIPanel.SetActive(false);

            // 4. GameManager Injection 업데이트
            UpdateGameManagerUIInjections(startUIPanel, gameUIPanel, gameOverUIPanel);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            Debug.Log("=== UI Structure organized ===");
            Debug.Log($"  StartUIPanel: {startUIPanel.transform.childCount} children");
            Debug.Log($"  GameUIPanel: {gameUIPanel.transform.childCount} children");
            Debug.Log($"  GameOverUIPanel: {gameOverUIPanel.transform.childCount} children");

            EditorUtility.DisplayDialog("UI Structure Organized",
                $"UI 구조가 정리되었습니다!\n\n" +
                $"• StartUIPanel: {startUIPanel.transform.childCount}개 요소\n" +
                $"• GameUIPanel: {gameUIPanel.transform.childCount}개 요소\n" +
                $"• GameOverUIPanel: {gameOverUIPanel.transform.childCount}개 요소\n\n" +
                "씬을 저장하세요 (Ctrl+S)", "OK");
        }

        /// <summary>
        /// UI 패널 찾기 또는 생성
        /// </summary>
        private static GameObject FindOrCreateUIPanel(Transform parent, string panelName)
        {
            // 이미 존재하는지 확인
            GameObject panel = FindChildRecursive(parent, panelName);
            if (panel != null)
            {
                Debug.Log($"Found existing panel: {panelName}");
                return panel;
            }

            // 새로 생성
            panel = new GameObject(panelName);
            panel.transform.SetParent(parent);
            panel.layer = LayerMask.NameToLayer("UI");

            // RectTransform 추가 (Canvas 자식이므로 필요)
            var rectTransform = panel.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = Vector3.zero;

            Debug.Log($"Created new panel: {panelName}");
            EditorUtility.SetDirty(panel);
            return panel;
        }

        /// <summary>
        /// UI 요소를 지정한 패널로 이동
        /// </summary>
        private static void MoveUIElementToPanel(Transform sourceParent, Transform targetPanel, params string[] searchNames)
        {
            foreach (var name in searchNames)
            {
                // sourceParent의 직접 자식에서만 찾기 (이미 패널 안에 있는 것 제외)
                for (int i = 0; i < sourceParent.childCount; i++)
                {
                    var child = sourceParent.GetChild(i);

                    // 이미 패널인 경우 스킵
                    if (child.name == "StartUIPanel" || child.name == "GameUIPanel" || child.name == "GameOverUIPanel")
                        continue;

                    if (child.name == name || child.name.Contains(name))
                    {
                        // 이미 대상 패널의 자식인지 확인
                        if (child.parent == targetPanel)
                            continue;

                        child.SetParent(targetPanel);
                        Debug.Log($"Moved '{child.name}' to {targetPanel.name}");
                        EditorUtility.SetDirty(child.gameObject);
                        return; // 첫 번째 매치만 이동
                    }
                }
            }
        }

        /// <summary>
        /// GameManager의 UI Injection을 업데이트
        /// </summary>
        private static void UpdateGameManagerUIInjections(GameObject startUIPanel, GameObject gameUIPanel, GameObject gameOverUIPanel)
        {
            GameObject gameManager = GameObject.Find(GAME_MANAGER_NAME);
            if (gameManager == null)
            {
                Debug.LogWarning("GameManager not found, skipping injection update");
                return;
            }

            var luaBehaviour = gameManager.GetComponent<VivenLuaBehaviour>();
            if (luaBehaviour == null)
            {
                Debug.LogWarning("VivenLuaBehaviour not found on GameManager");
                return;
            }

            EnsureInjectionInitialized(luaBehaviour);
            var injection = luaBehaviour.injection;

            // 패널 Injection 업데이트 (기존 값 덮어쓰기)
            UpdateOrAddGameObjectInjection(ref injection.gameObjectValues, "StartUIPanel", startUIPanel);
            UpdateOrAddGameObjectInjection(ref injection.gameObjectValues, "GameUIPanel", gameUIPanel);
            UpdateOrAddGameObjectInjection(ref injection.gameObjectValues, "GameOverUIPanel", gameOverUIPanel);

            // 버튼들도 새 위치에서 찾아서 업데이트
            GameObject startButton = FindChildRecursive(startUIPanel.transform, "StartButton");
            if (startButton == null) startButton = FindChildRecursive(startUIPanel.transform, "Start");
            if (startButton == null) startButton = FindChildRecursive(startUIPanel.transform, "시작");
            UpdateOrAddGameObjectInjection(ref injection.gameObjectValues, "StartButtonObject", startButton);

            GameObject restartButton = FindChildRecursive(gameOverUIPanel.transform, "RestartButton");
            if (restartButton == null) restartButton = FindChildRecursive(gameOverUIPanel.transform, "Restart");
            if (restartButton == null) restartButton = FindChildRecursive(gameOverUIPanel.transform, "재시작");
            UpdateOrAddGameObjectInjection(ref injection.gameObjectValues, "RestartButtonObject", restartButton);

            // Score, Timer, GuideText도 GameUIPanel에서 찾아서 업데이트
            GameObject scoreText = FindChildRecursive(gameUIPanel.transform, "Score");
            if (scoreText == null) scoreText = FindChildRecursive(gameUIPanel.transform, "ScoreText");
            UpdateOrAddGameObjectInjection(ref injection.gameObjectValues, "ScoreTextObject", scoreText);

            GameObject timerText = FindChildRecursive(gameUIPanel.transform, "Timer");
            if (timerText == null) timerText = FindChildRecursive(gameUIPanel.transform, "TimerText");
            UpdateOrAddGameObjectInjection(ref injection.gameObjectValues, "TimerTextObject", timerText);

            GameObject guideText = FindChildRecursive(gameUIPanel.transform, "GuideText");
            UpdateOrAddGameObjectInjection(ref injection.gameObjectValues, "GuideTextObject", guideText);

            // FinalScore도 GameOverUIPanel에서 찾기 (여러 이름 검색)
            GameObject finalScore = FindChildRecursive(gameOverUIPanel.transform, "FinalScore");
            if (finalScore == null) finalScore = FindChildRecursive(gameOverUIPanel.transform, "FinalScoreText");
            if (finalScore == null) finalScore = FindChildRecursive(gameOverUIPanel.transform, "GameOver");
            if (finalScore == null) finalScore = FindChildRecursive(gameOverUIPanel.transform, "GameOverText");
            if (finalScore == null) finalScore = FindChildRecursive(gameOverUIPanel.transform, "게임종료");
            UpdateOrAddGameObjectInjection(ref injection.gameObjectValues, "FinalScoreTextObject", finalScore);

            // BallGuide들도 GameUIPanel에서 찾아서 업데이트
            for (int i = 0; i < BallTypes.Length; i++)
            {
                string ballType = BallTypes[i];
                string koreanName = BallTypesKorean[i];

                GameObject ballGuide = FindChildRecursive(gameUIPanel.transform, ballType);
                if (ballGuide == null) ballGuide = FindChildRecursive(gameUIPanel.transform, koreanName);

                if (ballGuide != null)
                {
                    UpdateOrAddGameObjectInjection(ref injection.gameObjectValues, $"BallGuide_{ballType}", ballGuide);
                }
            }

            EditorUtility.SetDirty(luaBehaviour);
            Debug.Log("GameManager UI injections updated");
        }

        /// <summary>
        /// GameObject Injection 업데이트 또는 추가 (기존 값 덮어쓰기)
        /// </summary>
        private static void UpdateOrAddGameObjectInjection(ref GameObjectValue[] array, string name, GameObject value)
        {
            // 기존 항목 검색 및 업데이트
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].name == name)
                    {
                        array[i].value = value;
                        return;
                    }
                }
            }

            // 새 항목 추가
            var newEntry = new GameObjectValue { name = name, value = value };

            if (array == null)
            {
                array = new GameObjectValue[] { newEntry };
            }
            else
            {
                var list = new List<GameObjectValue>(array);
                list.Add(newEntry);
                array = list.ToArray();
            }
        }

        #endregion

        #region Create Objects

        public static void CreateManagerObjects()
        {
            // GameManager 생성
            GameObject gameManager = GameObject.Find(GAME_MANAGER_NAME);
            if (gameManager == null)
            {
                gameManager = new GameObject(GAME_MANAGER_NAME);
                Debug.Log("Created: " + GAME_MANAGER_NAME);
            }

            // SpawnManager 생성
            GameObject spawnManager = GameObject.Find(SPAWN_MANAGER_NAME);
            if (spawnManager == null)
            {
                spawnManager = new GameObject(SPAWN_MANAGER_NAME);
                Debug.Log("Created: " + SPAWN_MANAGER_NAME);
            }

            // VivenLuaBehaviour 추가 및 스크립트 연결
            SetupVivenLuaBehaviour(gameManager, GAME_MANAGER_SCRIPT, "Manager");
            SetupVivenLuaBehaviour(spawnManager, SPAWN_MANAGER_SCRIPT, "Manager");

            EditorUtility.SetDirty(gameManager);
            EditorUtility.SetDirty(spawnManager);
        }

        /// <summary>
        /// 공 종류별 TargetPool 설정
        /// 1. TargetPool이 비어있으면 씬에서 공 템플릿 찾아서 복제
        /// 2. 기존 공 오브젝트에 필요한 컴포넌트만 추가 (Rigidbody, VivenLuaBehaviour)
        /// 3. 기존 Mesh, Material, TMP_Text 등은 유지
        /// </summary>
        public static void CreateTargetPoolByBallType(int countPerType)
        {
            Debug.Log("=== Setting up Target Pool by Ball Type ===");

            // TargetPool 부모 오브젝트 찾기/생성
            GameObject targetPool = GameObject.Find(TARGET_POOL_NAME);
            if (targetPool == null)
            {
                targetPool = new GameObject(TARGET_POOL_NAME);
                Debug.Log("Created: " + TARGET_POOL_NAME);
            }

            // TargetPool이 비어있으면 씬에서 공 템플릿 찾아서 복제
            if (targetPool.transform.childCount == 0)
            {
                Debug.Log("TargetPool is empty, searching for ball templates in scene...");
                PopulateTargetPoolFromScene(targetPool, countPerType);
            }

            int setupCount = 0;
            int rigidbodyAdded = 0;

            // TargetPool의 모든 자식 오브젝트 처리
            for (int i = 0; i < targetPool.transform.childCount; i++)
            {
                GameObject target = targetPool.transform.GetChild(i).gameObject;
                SetupTargetObject(target, ref setupCount, ref rigidbodyAdded);
            }

            Debug.Log($"=== Target Pool Setup Complete ===");
            Debug.Log($"  Total targets: {targetPool.transform.childCount}");
            Debug.Log($"  Configured: {setupCount}");
            Debug.Log($"  Rigidbodies added: {rigidbodyAdded}");
            EditorUtility.SetDirty(targetPool);
        }

        /// <summary>
        /// TargetPool에 공 오브젝트 생성 (템플릿 없이 새로 생성)
        /// </summary>
        private static void PopulateTargetPoolFromScene(GameObject targetPool, int countPerType)
        {
            int totalCreated = 0;

            // 각 공 타입별로 오브젝트 생성
            foreach (var ballType in BallTypes)
            {
                for (int i = 0; i < countPerType; i++)
                {
                    string newName = $"{ballType}_{i}";

                    // 이미 존재하는지 확인
                    bool exists = false;
                    for (int j = 0; j < targetPool.transform.childCount; j++)
                    {
                        if (targetPool.transform.GetChild(j).name == newName)
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (exists) continue;

                    // 새 GameObject 생성
                    GameObject newTarget = new GameObject(newName);
                    newTarget.transform.SetParent(targetPool.transform);
                    newTarget.transform.localPosition = Vector3.zero;
                    newTarget.transform.localRotation = Quaternion.identity;
                    newTarget.transform.localScale = Vector3.one * GetBallScale(ballType);

                    totalCreated++;
                }

                Debug.Log($"Created {countPerType} instances of {ballType}");
            }

            Debug.Log($"Total targets created: {totalCreated}");
        }

        /// <summary>
        /// 공 타입별 크기 반환
        /// </summary>
        private static float GetBallScale(string ballType)
        {
            switch (ballType)
            {
                case "GolfBall":    return 0.3f;   // 골프공 - 작음
                case "TennisBall":  return 0.35f;  // 테니스공 - 작음
                case "BaseBall":    return 0.35f;  // 야구공 - 작음
                case "SoccerBall":  return 0.5f;   // 축구공 - 중간
                case "VolleyBall":  return 0.5f;   // 배구공 - 중간
                case "RugbyBall":   return 0.5f;   // 럭비공 - 중간
                case "BowlingBall": return 0.55f;  // 볼링공 - 큼
                case "BasketBall":  return 0.6f;   // 농구공 - 큼
                case "BeachBall":   return 0.6f;   // 비치볼 - 큼
                default:            return 0.4f;
            }
        }

        /// <summary>
        /// TextMeshPro 컴포넌트 제거 (자식 포함)
        /// 가이드 텍스트는 GameUIPanel에서 별도 관리하므로 공에서는 제거
        /// </summary>
        private static void RemoveTextMeshProComponents(GameObject target)
        {
            // TMP_Text 타입 찾기 (TMPro 네임스페이스)
            var tmpType = System.Type.GetType("TMPro.TMP_Text, Unity.TextMeshPro");
            var tmpUGUIType = System.Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");
            var tmp3DType = System.Type.GetType("TMPro.TextMeshPro, Unity.TextMeshPro");

            // 자식 오브젝트들도 포함해서 모든 TMP 컴포넌트 찾기
            var allComponents = target.GetComponentsInChildren<Component>(true);
            var toRemove = new List<Component>();

            foreach (var comp in allComponents)
            {
                if (comp == null) continue;
                var compType = comp.GetType();

                // TextMeshPro 관련 컴포넌트인지 확인
                if ((tmpType != null && tmpType.IsAssignableFrom(compType)) ||
                    (tmpUGUIType != null && tmpUGUIType.IsAssignableFrom(compType)) ||
                    (tmp3DType != null && tmp3DType.IsAssignableFrom(compType)) ||
                    compType.Name.Contains("TextMeshPro") ||
                    compType.Name.Contains("TMP_"))
                {
                    toRemove.Add(comp);
                }
            }

            // TMP 컴포넌트가 있는 자식 오브젝트는 통째로 삭제 (단, MeshRenderer가 있는 자식은 보호)
            var childrenToRemove = new List<GameObject>();
            for (int i = 0; i < target.transform.childCount; i++)
            {
                var child = target.transform.GetChild(i).gameObject;

                // MeshRenderer가 있는 자식은 삭제하지 않음 (공의 메시 보호)
                if (child.GetComponentInChildren<MeshRenderer>(true) != null ||
                    child.GetComponentInChildren<MeshFilter>(true) != null ||
                    child.GetComponentInChildren<SkinnedMeshRenderer>(true) != null)
                {
                    continue;
                }

                // 자식이 TMP 전용 오브젝트인지 확인 (Canvas나 Text 오브젝트)
                if (child.name.Contains("Text") || child.name.Contains("TMP") ||
                    child.name.Contains("Canvas") || child.name.Contains("Label"))
                {
                    var tmpComps = child.GetComponentsInChildren<Component>(true);
                    foreach (var tc in tmpComps)
                    {
                        if (tc != null && (tc.GetType().Name.Contains("TextMeshPro") ||
                            tc.GetType().Name.Contains("TMP_")))
                        {
                            childrenToRemove.Add(child);
                            break;
                        }
                    }
                }
            }

            // 자식 오브젝트 삭제
            foreach (var child in childrenToRemove)
            {
                Debug.Log($"Removing TMP child object: {child.name} from {target.name}");
                Object.DestroyImmediate(child);
            }

            // 남은 TMP 컴포넌트 직접 제거
            foreach (var comp in toRemove)
            {
                if (comp != null && comp.gameObject != null)
                {
                    Debug.Log($"Removing TMP component from {target.name}");
                    Object.DestroyImmediate(comp);
                }
            }
        }

        /// <summary>
        /// 개별 타겟 오브젝트 설정 (Rigidbody, Script, Injection 등)
        /// </summary>
        private static void SetupTargetObject(GameObject target, ref int setupCount, ref int rigidbodyAdded)
        {
            string targetName = target.name;

            // 공 타입 감지
            string detectedBallType = null;
            foreach (var ballType in BallTypes)
            {
                if (targetName.StartsWith(ballType) || targetName.Contains(ballType))
                {
                    detectedBallType = ballType;
                    break;
                }
            }

            // 한글 이름으로도 체크
            if (detectedBallType == null)
            {
                for (int j = 0; j < BallTypesKorean.Length; j++)
                {
                    if (targetName.Contains(BallTypesKorean[j]))
                    {
                        detectedBallType = BallTypes[j];
                        break;
                    }
                }
            }

            if (detectedBallType == null)
            {
                Debug.LogWarning($"Unknown ball type for: {targetName}");
                detectedBallType = "Unknown";
            }

            // VivenLuaBehaviour 설정 (없으면 추가)
            var luaBehaviour = target.GetComponent<VivenLuaBehaviour>();
            if (luaBehaviour == null)
            {
                SetupVivenLuaBehaviour(target, TARGET_SCRIPT, "Objects");
                luaBehaviour = target.GetComponent<VivenLuaBehaviour>();
            }

            // ballType Injection 설정
            if (luaBehaviour != null)
            {
                EnsureInjectionInitialized(luaBehaviour);
                EnsureStringInjection(ref luaBehaviour.injection.stringValue, "ballType", detectedBallType);
                EditorUtility.SetDirty(luaBehaviour);
            }

            // 태그 설정
            SetOrCreateTag(target, "Target");

            // Rigidbody 추가 (없으면) - 컨베이어 벨트 물리 이동용
            var rb = target.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = target.AddComponent<Rigidbody>();
                rigidbodyAdded++;
            }
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.mass = 1f;
            rb.linearDamping = 0.5f;
            rb.angularDamping = 0.5f;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            // Collider 설정 (기존 것 유지, isTrigger만 확인)
            var colliders = target.GetComponents<Collider>();
            if (colliders.Length > 0)
            {
                // 첫 번째 콜라이더는 물리용으로 설정
                colliders[0].isTrigger = false;
            }
            else
            {
                // 콜라이더가 없으면 추가
                var col = target.AddComponent<SphereCollider>();
                col.isTrigger = false;
                col.radius = 0.15f;
            }

            // MeshFilter/MeshRenderer 설정 (없으면 템플릿에서 복사)
            SetupMeshFromTemplate(target, detectedBallType);

            setupCount++;
            EditorUtility.SetDirty(target);
        }

        /// <summary>
        /// MeshFilter/MeshRenderer 추가 (없으면 기본 Sphere)
        /// </summary>
        private static void SetupMeshFromTemplate(GameObject target, string ballType)
        {
            // 이미 MeshRenderer가 있으면 스킵
            if (target.GetComponentInChildren<MeshRenderer>(true) != null ||
                target.GetComponentInChildren<SkinnedMeshRenderer>(true) != null)
            {
                return;
            }

            // 임시 Sphere 생성해서 mesh 가져오기
            var tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var sphereMesh = tempSphere.GetComponent<MeshFilter>().sharedMesh;
            var defaultMat = tempSphere.GetComponent<MeshRenderer>().sharedMaterial;
            Object.DestroyImmediate(tempSphere);

            // MeshFilter 추가
            var mf = target.GetComponent<MeshFilter>();
            if (mf == null)
            {
                mf = target.AddComponent<MeshFilter>();
            }
            mf.sharedMesh = sphereMesh;

            // MeshRenderer 추가
            var mr = target.GetComponent<MeshRenderer>();
            if (mr == null)
            {
                mr = target.AddComponent<MeshRenderer>();
            }
            mr.sharedMaterial = defaultMat;

            Debug.Log($"{target.name}: Added Sphere mesh");
        }

        /// <summary>
        /// 기존 방식의 단순 타겟 풀 생성 (fallback용)
        /// </summary>
        public static void CreateTargetPool(int poolSize)
        {
            // TargetPool 부모 오브젝트
            GameObject targetPool = GameObject.Find(TARGET_POOL_NAME);
            if (targetPool == null)
            {
                targetPool = new GameObject(TARGET_POOL_NAME);
                Debug.Log("Created: " + TARGET_POOL_NAME);
            }

            // 기존 자식 수 확인
            int existingCount = targetPool.transform.childCount;

            // 타겟 오브젝트 생성
            for (int i = existingCount; i < poolSize; i++)
            {
                GameObject target = CreatePoolObject("Target_" + i, targetPool.transform);
                SetupVivenLuaBehaviour(target, TARGET_SCRIPT, "Objects");

                // 태그 설정
                SetOrCreateTag(target, "Target");

                // Rigidbody 추가 (컨베이어 벨트 물리 이동용)
                var rb = target.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = target.AddComponent<Rigidbody>();
                }
                rb.useGravity = true;
                rb.isKinematic = false;
                rb.mass = 1f;
                rb.linearDamping = 0.5f;
                rb.angularDamping = 0.5f;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                // Collider 추가 (물리용 - isTrigger = false)
                var col = target.GetComponent<Collider>();
                if (col == null)
                {
                    var boxCol = target.AddComponent<BoxCollider>();
                    boxCol.isTrigger = false;  // 물리 충돌용
                    boxCol.size = new Vector3(0.3f, 0.3f, 0.3f);
                }
                else
                {
                    col.isTrigger = false;  // 기존 콜라이더도 물리용으로
                }

                // 시각적 표시용 큐브 (없으면)
                if (target.GetComponent<MeshFilter>() == null)
                {
                    var meshFilter = target.AddComponent<MeshFilter>();
                    meshFilter.sharedMesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

                    var meshRenderer = target.AddComponent<MeshRenderer>();
                    meshRenderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
                }

                target.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            }

            Debug.Log($"Target Pool: {targetPool.transform.childCount} targets ready");
            EditorUtility.SetDirty(targetPool);
        }

        public static void CreateBulletPool(int poolSize)
        {
            // BulletPool 부모 오브젝트
            GameObject bulletPool = GameObject.Find(BULLET_POOL_NAME);
            if (bulletPool == null)
            {
                bulletPool = new GameObject(BULLET_POOL_NAME);
                Debug.Log("Created: " + BULLET_POOL_NAME);
            }

            // 기존 자식 수 확인
            int existingCount = bulletPool.transform.childCount;

            // 총알 오브젝트 생성
            for (int i = existingCount; i < poolSize; i++)
            {
                GameObject bullet = CreatePoolObject("Bullet_" + i, bulletPool.transform);
                SetupVivenLuaBehaviour(bullet, BULLET_SCRIPT, "Objects");

                // 태그 설정
                SetOrCreateTag(bullet, "Bullet");

                // Rigidbody 추가
                var rb = bullet.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = bullet.AddComponent<Rigidbody>();
                }
                rb.useGravity = false;
                rb.isKinematic = false;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                // Collider 추가
                if (bullet.GetComponent<Collider>() == null)
                {
                    var col = bullet.AddComponent<SphereCollider>();
                    col.isTrigger = true;
                    col.radius = 0.05f;
                }

                // 시각적 표시용 구체
                if (bullet.GetComponent<MeshFilter>() == null)
                {
                    // CreatePrimitive로 Sphere mesh 가져오기
                    var tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    var sphereMesh = tempSphere.GetComponent<MeshFilter>().sharedMesh;
                    var defaultMat = tempSphere.GetComponent<MeshRenderer>().sharedMaterial;
                    Object.DestroyImmediate(tempSphere);

                    var meshFilter = bullet.AddComponent<MeshFilter>();
                    meshFilter.sharedMesh = sphereMesh;

                    var meshRenderer = bullet.AddComponent<MeshRenderer>();
                    meshRenderer.sharedMaterial = defaultMat;
                }

                bullet.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            }

            Debug.Log($"Bullet Pool: {bulletPool.transform.childCount} bullets ready");
            EditorUtility.SetDirty(bulletPool);
        }

        private static GameObject CreatePoolObject(string name, Transform parent)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            return obj;
        }

        #endregion

        #region Setup Gun

        public static void SetupGun()
        {
            GameObject gun = GameObject.Find(GUN_NAME);
            if (gun == null)
            {
                Debug.LogWarning($"Gun object '{GUN_NAME}' not found in scene!");
                return;
            }

            SetupVivenLuaBehaviour(gun, SHOOTING_GUN_SCRIPT, "Objects");

            // ShootSound 오브젝트가 없으면 생성
            GameObject shootSound = FindChildRecursive(gun.transform, "ShootSound");
            if (shootSound == null)
            {
                // Gun 하위에 AudioSource가 있는지 먼저 확인
                var existingAudio = gun.GetComponentInChildren<AudioSource>();
                if (existingAudio != null)
                {
                    shootSound = existingAudio.gameObject;
                    shootSound.name = "ShootSound";
                    Debug.Log("Renamed existing AudioSource to ShootSound");
                }
                else
                {
                    // 새로 생성
                    shootSound = new GameObject("ShootSound");
                    shootSound.transform.SetParent(gun.transform);
                    shootSound.transform.localPosition = Vector3.zero;

                    var audioSource = shootSound.AddComponent<AudioSource>();
                    audioSource.playOnAwake = false;
                    audioSource.spatialBlend = 1f; // 3D 사운드

                    Debug.Log("Created ShootSound AudioSource in Gun");
                }
            }

            Debug.Log("Gun setup completed: " + GUN_NAME);
            EditorUtility.SetDirty(gun);
        }

        #endregion

        #region Setup Conveyor Belt

        /// <summary>
        /// 컨베이어 벨트 설정
        /// </summary>
        public static void SetupConveyorBelt()
        {
            // 씬에서 컨베이어 벨트 오브젝트 찾기
            string[] possibleNames = { "ConveyorBelt", "Conveyor", "Belt", "컨베이어", "컨베이어벨트", "ConveyorBeltTrigger" };
            GameObject conveyorBelt = null;

            foreach (var name in possibleNames)
            {
                conveyorBelt = GameObject.Find(name);
                if (conveyorBelt != null) break;
            }

            // 못 찾으면 SpawnPoint 근처에 생성
            if (conveyorBelt == null)
            {
                GameObject spawnPoint = GameObject.Find(SPAWN_POINT_NAME);
                if (spawnPoint == null)
                {
                    Debug.LogWarning("SpawnPoint not found, cannot create ConveyorBelt!");
                    return;
                }

                // 컨베이어 벨트 트리거 영역 생성
                conveyorBelt = new GameObject("ConveyorBelt");
                conveyorBelt.transform.position = spawnPoint.transform.position + new Vector3(0, -0.5f, 2f);
                conveyorBelt.transform.rotation = Quaternion.identity;

                Debug.Log("Created ConveyorBelt object");
            }

            // Box Collider 추가 (isTrigger = true)
            var collider = conveyorBelt.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = conveyorBelt.AddComponent<BoxCollider>();
            }
            collider.isTrigger = true;
            collider.size = new Vector3(3f, 1f, 10f); // 넓은 트리거 영역
            collider.center = Vector3.zero;

            // VivenLuaBehaviour + ConveyorBelt 스크립트
            SetupVivenLuaBehaviour(conveyorBelt, "ConveyorBelt", "Objects");

            // Injection 설정
            var luaBehaviour = conveyorBelt.GetComponent<VivenLuaBehaviour>();
            if (luaBehaviour != null)
            {
                EnsureInjectionInitialized(luaBehaviour);
                EnsureFloatInjection(ref luaBehaviour.injection.floatValue, "speed", 0.8f);

                // direction Vector3 설정
                if (luaBehaviour.injection.vector3Values == null)
                    luaBehaviour.injection.vector3Values = new Vector3Value[0];

                bool found = false;
                for (int i = 0; i < luaBehaviour.injection.vector3Values.Length; i++)
                {
                    if (luaBehaviour.injection.vector3Values[i].name == "direction")
                    {
                        luaBehaviour.injection.vector3Values[i].value = new Vector3(0, 0, 1);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    var list = new List<Vector3Value>(luaBehaviour.injection.vector3Values);
                    list.Add(new Vector3Value { name = "direction", value = new Vector3(0, 0, 1) });
                    luaBehaviour.injection.vector3Values = list.ToArray();
                }

                EditorUtility.SetDirty(luaBehaviour);
            }

            Debug.Log($"ConveyorBelt setup completed: {conveyorBelt.name} (isTrigger={collider.isTrigger})");
            EditorUtility.SetDirty(conveyorBelt);
        }

        /// <summary>
        /// 공 수거 영역 설정 (컨베이어 벨트 끝)
        /// </summary>
        public static void SetupTargetCollector()
        {
            // 씬에서 찾기
            string[] possibleNames = { "Props_Dustbin", "Dustbin", "TargetCollector", "Collector", "EndZone", "수거함", "쓰레기통" };
            GameObject collector = null;

            foreach (var name in possibleNames)
            {
                collector = GameObject.Find(name);
                Debug.Log($"Searching for '{name}': {(collector != null ? "FOUND" : "not found")}");
                if (collector != null) break;
            }

            // 못 찾으면 컨베이어 벨트 끝에 생성
            if (collector == null)
            {
                GameObject conveyorBelt = GameObject.Find("ConveyorBelt");
                Vector3 spawnPos = Vector3.zero;

                if (conveyorBelt != null)
                {
                    // 컨베이어 벨트 끝 위치 (direction 방향으로 offset)
                    var boxCol = conveyorBelt.GetComponent<BoxCollider>();
                    float length = boxCol != null ? boxCol.size.z : 10f;
                    spawnPos = conveyorBelt.transform.position + conveyorBelt.transform.forward * (length / 2 + 1f);
                }
                else
                {
                    GameObject spawnPoint = GameObject.Find(SPAWN_POINT_NAME);
                    if (spawnPoint != null)
                    {
                        spawnPos = spawnPoint.transform.position + new Vector3(0, 0, 12f);
                    }
                }

                collector = new GameObject("TargetCollector");
                collector.transform.position = spawnPos;
                collector.transform.rotation = Quaternion.identity;

                Debug.Log("Created TargetCollector object");
            }

            // Box Collider 추가 (isTrigger = true)
            var collider = collector.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = collector.AddComponent<BoxCollider>();
            }
            collider.isTrigger = true;
            collider.size = new Vector3(4f, 3f, 2f); // 넓은 수거 영역
            collider.center = Vector3.zero;

            // VivenLuaBehaviour + TargetCollector 스크립트
            SetupVivenLuaBehaviour(collector, "TargetCollector", "Objects");

            // Injection 설정
            var luaBehaviour = collector.GetComponent<VivenLuaBehaviour>();
            if (luaBehaviour != null)
            {
                EnsureInjectionInitialized(luaBehaviour);

                // GameManager, SpawnManager 참조
                GameObject gameManager = GameObject.Find(GAME_MANAGER_NAME);
                GameObject spawnManager = GameObject.Find(SPAWN_MANAGER_NAME);

                EnsureGameObjectInjection(ref luaBehaviour.injection.gameObjectValues, "GameManagerObject", gameManager);
                EnsureGameObjectInjection(ref luaBehaviour.injection.gameObjectValues, "SpawnManagerObject", spawnManager);

                EditorUtility.SetDirty(luaBehaviour);
            }

            Debug.Log($"TargetCollector setup completed: {collector.name}");
            EditorUtility.SetDirty(collector);
        }

        #endregion

        #region Setup Injections

        public static void SetupAllInjections()
        {
            Debug.Log("Setting up all injections...");

            // 오브젝트 찾기
            GameObject gameManager = GameObject.Find(GAME_MANAGER_NAME);
            GameObject spawnManager = GameObject.Find(SPAWN_MANAGER_NAME);
            GameObject gun = GameObject.Find(GUN_NAME);
            GameObject shootPoint = FindChildRecursive(gun?.transform, SHOOT_POINT_NAME);
            GameObject spawnPoint = GameObject.Find(SPAWN_POINT_NAME);
            GameObject targetPool = GameObject.Find(TARGET_POOL_NAME);
            GameObject bulletPool = GameObject.Find(BULLET_POOL_NAME);
            GameObject gameCanvas = GameObject.Find(GAME_CANVAS_NAME);

            // GameManager Injection
            if (gameManager != null)
            {
                SetupGameManagerInjection(gameManager, spawnManager, gameCanvas);
            }

            // SpawnManager Injection
            if (spawnManager != null)
            {
                SetupSpawnManagerInjection(spawnManager, spawnPoint, targetPool, gameManager);
            }

            // Gun (ShootingGun) Injection
            if (gun != null)
            {
                SetupGunInjection(gun, shootPoint, bulletPool, gameManager);
            }

            // Target Injection (각 타겟)
            if (targetPool != null)
            {
                SetupTargetPoolInjections(targetPool, spawnManager, gameManager);
            }

            // Bullet Injection (각 총알)
            if (bulletPool != null)
            {
                SetupBulletPoolInjections(bulletPool, gameManager);
            }

            Debug.Log("All injections setup completed!");
        }

        private static void SetupGameManagerInjection(GameObject gameManager, GameObject spawnManager, GameObject gameCanvas)
        {
            var luaBehaviour = gameManager.GetComponent<VivenLuaBehaviour>();
            if (luaBehaviour == null) return;

            EnsureInjectionInitialized(luaBehaviour);
            var injection = luaBehaviour.injection;

            // GameObject Injections
            EnsureGameObjectInjection(ref injection.gameObjectValues, "SpawnManagerObject", spawnManager);

            // Canvas_left도 검색 대상에 포함
            GameObject canvasLeft = GameObject.Find("Canvas_left");

            // UI 관련 (GameCanvas + Canvas_left 하위에서 찾기) - 영어/한글 양쪽 검색
            if (gameCanvas != null)
            {
                // 점수 텍스트
                GameObject scoreText = FindInMultipleCanvases(gameCanvas, canvasLeft, "Score", "ScoreText", "점수");
                EnsureGameObjectInjection(ref injection.gameObjectValues, "ScoreTextObject", scoreText);

                // 타이머 텍스트
                GameObject timerText = FindInMultipleCanvases(gameCanvas, canvasLeft, "Timer", "TimerText", "타이머");
                EnsureGameObjectInjection(ref injection.gameObjectValues, "TimerTextObject", timerText);

                // 시작 UI 패널 (준비!)
                GameObject startUIPanel = FindInMultipleCanvases(gameCanvas, canvasLeft, "StartUIPanel", "StartUI", "Ready", "ReadyText", "준비!");
                EnsureGameObjectInjection(ref injection.gameObjectValues, "StartUIPanel", startUIPanel);

                // 게임 UI 패널
                GameObject gameUIPanel = FindInMultipleCanvases(gameCanvas, canvasLeft, "GameUIPanel", "GameUI");
                EnsureGameObjectInjection(ref injection.gameObjectValues, "GameUIPanel", gameUIPanel);

                // 게임 오버 UI 패널
                GameObject gameOverUIPanel = FindInMultipleCanvases(gameCanvas, canvasLeft, "GameOverUIPanel", "GameOverUI", "GameOver", "GameOverText", "게임종료");
                EnsureGameObjectInjection(ref injection.gameObjectValues, "GameOverUIPanel", gameOverUIPanel);

                // 시작 버튼
                GameObject startButton = FindInMultipleCanvases(gameCanvas, canvasLeft, "StartButton", "Start", "시작");
                EnsureGameObjectInjection(ref injection.gameObjectValues, "StartButtonObject", startButton);

                // 재시작 버튼
                GameObject restartButton = FindInMultipleCanvases(gameCanvas, canvasLeft, "RestartButton", "Restart", "재시작");
                EnsureGameObjectInjection(ref injection.gameObjectValues, "RestartButtonObject", restartButton);

                // 최종 점수 텍스트 (GameOverText도 검색)
                GameObject finalScoreText = FindInMultipleCanvases(gameCanvas, canvasLeft, "FinalScore", "FinalScoreText", "GameOverText", "게임종료");
                EnsureGameObjectInjection(ref injection.gameObjectValues, "FinalScoreTextObject", finalScoreText);

                // 가이드 텍스트 (현재 맞춰야 할 공 종류 표시)
                GameObject guideText = FindInMultipleCanvases(gameCanvas, canvasLeft, "GuideText", "Guide", "가이드", "TargetGuide");
                EnsureGameObjectInjection(ref injection.gameObjectValues, "GuideTextObject", guideText);

                // 공 종류별 가이드 텍스트 오브젝트들 (9종) - GameCanvas와 Canvas_left 양쪽에서 검색
                for (int i = 0; i < BallTypes.Length; i++)
                {
                    string englishName = BallTypes[i];
                    string koreanName = BallTypesKorean[i];

                    GameObject ballGuide = FindInMultipleCanvases(gameCanvas, canvasLeft, englishName, koreanName);
                    if (ballGuide != null)
                    {
                        EnsureGameObjectInjection(ref injection.gameObjectValues, $"BallGuide_{englishName}", ballGuide);
                        Debug.Log($"Found ball guide: {englishName} = {ballGuide.name}");
                    }
                }
            }

            // Float/Int Injections (기본값)
            EnsureFloatInjection(ref injection.floatValue, "gameTime", 60f);
            EnsureFloatInjection(ref injection.floatValue, "spawnInterval", 2f);
            EnsureIntInjection(ref injection.intValue, "maxTargetCount", 10);
            EnsureIntInjection(ref injection.intValue, "missedPenalty", 5);

            EditorUtility.SetDirty(luaBehaviour);
            Debug.Log("GameManager injection setup completed");
        }

        /// <summary>
        /// 여러 캔버스에서 오브젝트 검색
        /// </summary>
        private static GameObject FindInMultipleCanvases(GameObject canvas1, GameObject canvas2, params string[] names)
        {
            // 첫 번째 캔버스에서 찾기
            if (canvas1 != null)
            {
                var found = FindChildByMultipleNames(canvas1.transform, names);
                if (found != null) return found;
            }

            // 두 번째 캔버스에서 찾기
            if (canvas2 != null)
            {
                var found = FindChildByMultipleNames(canvas2.transform, names);
                if (found != null) return found;
            }

            return null;
        }

        private static void SetupSpawnManagerInjection(GameObject spawnManager, GameObject spawnPoint,
            GameObject targetPool, GameObject gameManager)
        {
            var luaBehaviour = spawnManager.GetComponent<VivenLuaBehaviour>();
            if (luaBehaviour == null) return;

            EnsureInjectionInitialized(luaBehaviour);
            var injection = luaBehaviour.injection;

            EnsureGameObjectInjection(ref injection.gameObjectValues, "SpawnPoint", spawnPoint);
            EnsureGameObjectInjection(ref injection.gameObjectValues, "TargetPool", targetPool);
            EnsureGameObjectInjection(ref injection.gameObjectValues, "GameManagerObject", gameManager);

            EditorUtility.SetDirty(luaBehaviour);
            Debug.Log("SpawnManager injection setup completed");
        }

        private static void SetupGunInjection(GameObject gun, GameObject shootPoint,
            GameObject bulletPool, GameObject gameManager)
        {
            var luaBehaviour = gun.GetComponent<VivenLuaBehaviour>();
            if (luaBehaviour == null) return;

            EnsureInjectionInitialized(luaBehaviour);
            var injection = luaBehaviour.injection;

            EnsureGameObjectInjection(ref injection.gameObjectValues, "ShootPoint", shootPoint);
            EnsureGameObjectInjection(ref injection.gameObjectValues, "BulletPool", bulletPool);
            EnsureGameObjectInjection(ref injection.gameObjectValues, "GameManagerObject", gameManager);

            // 발사 사운드 (Gun 하위에서 찾기)
            GameObject shootSound = FindChildRecursive(gun.transform, "ShootSound");
            if (shootSound == null)
            {
                // AudioSource가 있는 오브젝트 찾기
                var audioSource = gun.GetComponentInChildren<AudioSource>();
                if (audioSource != null)
                {
                    shootSound = audioSource.gameObject;
                }
            }
            EnsureGameObjectInjection(ref injection.gameObjectValues, "ShootSoundObject", shootSound);

            // Float Injections
            EnsureFloatInjection(ref injection.floatValue, "bulletSpeed", 30f);
            EnsureFloatInjection(ref injection.floatValue, "shootCooldown", 0.2f);

            EditorUtility.SetDirty(luaBehaviour);
            Debug.Log("Gun injection setup completed");
        }

        private static void SetupTargetPoolInjections(GameObject targetPool, GameObject spawnManager, GameObject gameManager)
        {
            for (int i = 0; i < targetPool.transform.childCount; i++)
            {
                var target = targetPool.transform.GetChild(i).gameObject;
                var luaBehaviour = target.GetComponent<VivenLuaBehaviour>();
                if (luaBehaviour == null) continue;

                EnsureInjectionInitialized(luaBehaviour);
                var injection = luaBehaviour.injection;

                EnsureGameObjectInjection(ref injection.gameObjectValues, "SpawnManagerObject", spawnManager);
                EnsureGameObjectInjection(ref injection.gameObjectValues, "GameManagerObject", gameManager);
                EnsureIntInjection(ref injection.intValue, "scoreValue", 10);

                EditorUtility.SetDirty(luaBehaviour);
            }

            Debug.Log($"Target pool injections setup: {targetPool.transform.childCount} targets");
        }

        private static void SetupBulletPoolInjections(GameObject bulletPool, GameObject gameManager)
        {
            for (int i = 0; i < bulletPool.transform.childCount; i++)
            {
                var bullet = bulletPool.transform.GetChild(i).gameObject;
                var luaBehaviour = bullet.GetComponent<VivenLuaBehaviour>();
                if (luaBehaviour == null) continue;

                EnsureInjectionInitialized(luaBehaviour);
                var injection = luaBehaviour.injection;

                EnsureGameObjectInjection(ref injection.gameObjectValues, "GameManagerObject", gameManager);
                EnsureFloatInjection(ref injection.floatValue, "autoReturnTime", 3f);

                EditorUtility.SetDirty(luaBehaviour);
            }

            Debug.Log($"Bullet pool injections setup: {bulletPool.transform.childCount} bullets");
        }

        #endregion

        #region VivenLuaBehaviour Helpers

        private static void SetupVivenLuaBehaviour(GameObject obj, string scriptName, string subfolder)
        {
            if (obj == null) return;

            // VivenLuaBehaviour 컴포넌트 추가/획득
            var luaBehaviour = obj.GetComponent<VivenLuaBehaviour>();
            if (luaBehaviour == null)
            {
                luaBehaviour = obj.AddComponent<VivenLuaBehaviour>();
                Debug.Log($"Added VivenLuaBehaviour to: {obj.name}");
            }

            // Lua 스크립트 찾기 및 연결
            string scriptPath = $"{SCRIPTS_PATH}/{subfolder}/{scriptName}.lua";
            var vivenScript = AssetDatabase.LoadAssetAtPath<VivenScript>(scriptPath);

            if (vivenScript == null)
            {
                // 다른 경로에서 찾기
                string[] guids = AssetDatabase.FindAssets($"{scriptName} t:VivenScript");
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (path.Contains(scriptName))
                    {
                        vivenScript = AssetDatabase.LoadAssetAtPath<VivenScript>(path);
                        if (vivenScript != null) break;
                    }
                }
            }

            if (vivenScript != null)
            {
                luaBehaviour.luaScript = vivenScript;
                Debug.Log($"Linked script '{scriptName}' to: {obj.name}");
            }
            else
            {
                Debug.LogWarning($"Script not found: {scriptName}.lua");
            }

            // Injection 초기화
            EnsureInjectionInitialized(luaBehaviour);

            EditorUtility.SetDirty(luaBehaviour);
        }

        private static void EnsureInjectionInitialized(VivenLuaBehaviour luaBehaviour)
        {
            if (luaBehaviour.injection == null)
            {
                luaBehaviour.injection = new Injection();
            }

            EnsureAllInjectionArraysInitialized(luaBehaviour.injection);
        }

        private static void EnsureAllInjectionArraysInitialized(Injection injection)
        {
            if (injection.objectValues == null) injection.objectValues = new ObjectValue[0];
            if (injection.gameObjectValues == null) injection.gameObjectValues = new GameObjectValue[0];
            if (injection.vector3Values == null) injection.vector3Values = new Vector3Value[0];
            if (injection.floatValue == null) injection.floatValue = new FloatValue[0];
            if (injection.intValue == null) injection.intValue = new IntValue[0];
            if (injection.boolValue == null) injection.boolValue = new BoolValue[0];
            if (injection.stringValue == null) injection.stringValue = new StringValue[0];
            if (injection.colorValue == null) injection.colorValue = new ColorValue[0];
            if (injection.vivenScriptValue == null) injection.vivenScriptValue = new VivenScriptValue[0];
        }

        #endregion

        #region Injection Helpers

        private static bool EnsureGameObjectInjection(ref GameObjectValue[] array, string name, GameObject value)
        {
            // 기존 항목 검색
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].name == name)
                    {
                        if (array[i].value == null && value != null)
                        {
                            array[i].value = value;
                            return true;
                        }
                        return false;
                    }
                }
            }

            // 새 항목 추가
            var newEntry = new GameObjectValue { name = name, value = value };

            if (array == null)
            {
                array = new GameObjectValue[] { newEntry };
            }
            else
            {
                var list = new List<GameObjectValue>(array);
                list.Add(newEntry);
                array = list.ToArray();
            }

            return true;
        }

        private static bool EnsureFloatInjection(ref FloatValue[] array, string name, float value)
        {
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].name == name)
                    {
                        return false; // 이미 존재
                    }
                }
            }

            var newEntry = new FloatValue { name = name, value = value };

            if (array == null)
            {
                array = new FloatValue[] { newEntry };
            }
            else
            {
                var list = new List<FloatValue>(array);
                list.Add(newEntry);
                array = list.ToArray();
            }

            return true;
        }

        private static bool EnsureIntInjection(ref IntValue[] array, string name, int value)
        {
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].name == name)
                    {
                        return false;
                    }
                }
            }

            var newEntry = new IntValue { name = name, value = value };

            if (array == null)
            {
                array = new IntValue[] { newEntry };
            }
            else
            {
                var list = new List<IntValue>(array);
                list.Add(newEntry);
                array = list.ToArray();
            }

            return true;
        }

        private static bool EnsureStringInjection(ref StringValue[] array, string name, string value)
        {
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].name == name)
                    {
                        // 값이 다르면 업데이트
                        if (array[i].value != value)
                        {
                            array[i].value = value;
                            return true;
                        }
                        return false;
                    }
                }
            }

            var newEntry = new StringValue { name = name, value = value };

            if (array == null)
            {
                array = new StringValue[] { newEntry };
            }
            else
            {
                var list = new List<StringValue>(array);
                list.Add(newEntry);
                array = list.ToArray();
            }

            return true;
        }

        #endregion

        #region Utility Helpers

        /// <summary>
        /// 여러 이름으로 자식 오브젝트 검색 (첫 번째 발견된 것 반환)
        /// </summary>
        private static GameObject FindChildByMultipleNames(Transform parent, params string[] names)
        {
            foreach (var name in names)
            {
                var found = FindChildRecursive(parent, name);
                if (found != null) return found;
            }
            return null;
        }

        private static GameObject FindChildRecursive(Transform parent, string name)
        {
            if (parent == null) return null;

            // 직접 자식에서 찾기
            var direct = parent.Find(name);
            if (direct != null) return direct.gameObject;

            // 재귀적으로 찾기
            for (int i = 0; i < parent.childCount; i++)
            {
                var result = FindChildRecursive(parent.GetChild(i), name);
                if (result != null) return result;
            }

            return null;
        }

        private static void SetOrCreateTag(GameObject obj, string tagName)
        {
            // 태그가 존재하는지 확인하고 없으면 생성
            if (!TagExists(tagName))
            {
                CreateTag(tagName);
            }

            try
            {
                obj.tag = tagName;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Failed to set tag '{tagName}': {e.Message}");
            }
        }

        private static bool TagExists(string tagName)
        {
            foreach (string tag in UnityEditorInternal.InternalEditorUtility.tags)
            {
                if (tag == tagName)
                    return true;
            }
            return false;
        }

        private static void CreateTag(string tagName)
        {
            // TagManager 에셋 로드
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var tagsProp = tagManager.FindProperty("tags");

            // 이미 존재하는지 다시 확인
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                if (tagsProp.GetArrayElementAtIndex(i).stringValue == tagName)
                {
                    return; // 이미 존재함
                }
            }

            // 새 태그 추가
            tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
            tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tagName;
            tagManager.ApplyModifiedProperties();

            Debug.Log($"Created new tag: {tagName}");
        }

        #endregion

        #region Fix Null Arrays

        public static void FixAllNullInjectionArrays()
        {
            int fixedCount = 0;

            // 씬의 모든 VivenLuaBehaviour 찾기
            var allBehaviours = Resources.FindObjectsOfTypeAll<VivenLuaBehaviour>();

            foreach (var behaviour in allBehaviours)
            {
                if (behaviour.gameObject.scene.IsValid())
                {
                    if (FixNullInjectionArrays(behaviour))
                    {
                        fixedCount++;
                        EditorUtility.SetDirty(behaviour);
                    }
                }
            }

            Debug.Log($"Fixed null injection arrays in {fixedCount} components");
        }

        private static bool FixNullInjectionArrays(VivenLuaBehaviour behaviour)
        {
            if (behaviour.injection == null)
            {
                behaviour.injection = new Injection();
            }

            var injection = behaviour.injection;
            bool modified = false;

            if (injection.objectValues == null) { injection.objectValues = new ObjectValue[0]; modified = true; }
            if (injection.gameObjectValues == null) { injection.gameObjectValues = new GameObjectValue[0]; modified = true; }
            if (injection.vector3Values == null) { injection.vector3Values = new Vector3Value[0]; modified = true; }
            if (injection.floatValue == null) { injection.floatValue = new FloatValue[0]; modified = true; }
            if (injection.intValue == null) { injection.intValue = new IntValue[0]; modified = true; }
            if (injection.boolValue == null) { injection.boolValue = new BoolValue[0]; modified = true; }
            if (injection.stringValue == null) { injection.stringValue = new StringValue[0]; modified = true; }
            if (injection.colorValue == null) { injection.colorValue = new ColorValue[0]; modified = true; }
            if (injection.vivenScriptValue == null) { injection.vivenScriptValue = new VivenScriptValue[0]; modified = true; }

            return modified;
        }

        #endregion

        #region Editor Callbacks

        [InitializeOnLoadMethod]
        private static void OnEditorLoad()
        {
            // 씬 열릴 때 자동 null 배열 수정
            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                EditorApplication.delayCall += () =>
                {
                    // 자동 수정 비활성화 (수동으로 실행하도록)
                    // FixAllNullInjectionArrays();
                };
            };
        }

        #endregion
    }
}
