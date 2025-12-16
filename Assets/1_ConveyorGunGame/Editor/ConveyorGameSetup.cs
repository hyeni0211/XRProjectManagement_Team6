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

        #endregion

        #region Editor Window

        private int targetPoolSize = DEFAULT_TARGET_POOL_SIZE;
        private int bulletPoolSize = DEFAULT_BULLET_POOL_SIZE;
        private Vector2 scrollPosition;

        [MenuItem(MENU_PATH + "/Setup Scene")]
        public static void ShowWindow()
        {
            var window = GetWindow<ConveyorGameSetup>("Conveyor Game Setup");
            window.minSize = new Vector2(400, 500);
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayout.Label("Conveyor Gun Game Setup", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            // 풀 크기 설정
            GUILayout.Label("Pool Settings", EditorStyles.boldLabel);
            targetPoolSize = EditorGUILayout.IntSlider("Target Pool Size", targetPoolSize, 5, 30);
            bulletPoolSize = EditorGUILayout.IntSlider("Bullet Pool Size", bulletPoolSize, 10, 50);
            EditorGUILayout.Space(10);

            // 상태 확인
            GUILayout.Label("Current Status", EditorStyles.boldLabel);
            DrawStatusCheck();
            EditorGUILayout.Space(10);

            // 설정 버튼
            GUILayout.Label("Setup Actions", EditorStyles.boldLabel);

            if (GUILayout.Button("1. Create Manager Objects", GUILayout.Height(30)))
            {
                CreateManagerObjects();
            }

            if (GUILayout.Button("2. Create Target Pool", GUILayout.Height(30)))
            {
                CreateTargetPool(targetPoolSize);
            }

            if (GUILayout.Button("3. Create Bullet Pool", GUILayout.Height(30)))
            {
                CreateBulletPool(bulletPoolSize);
            }

            if (GUILayout.Button("4. Setup Gun (ShootingGun.lua)", GUILayout.Height(30)))
            {
                SetupGun();
            }

            if (GUILayout.Button("5. Setup All Injections", GUILayout.Height(30)))
            {
                SetupAllInjections();
            }

            EditorGUILayout.Space(20);

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("AUTO SETUP ALL", GUILayout.Height(50)))
            {
                AutoSetupAll();
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(10);

            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("Fix Null Injection Arrays", GUILayout.Height(25)))
            {
                FixAllNullInjectionArrays();
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndScrollView();
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

        #endregion

        #region Auto Setup

        public static void AutoSetupAll()
        {
            Debug.Log("=== Conveyor Gun Game Auto Setup Started ===");

            // 1. 매니저 오브젝트 생성
            CreateManagerObjects();

            // 2. 풀 생성
            CreateTargetPool(DEFAULT_TARGET_POOL_SIZE);
            CreateBulletPool(DEFAULT_BULLET_POOL_SIZE);

            // 3. 총 설정
            SetupGun();

            // 4. 모든 Injection 설정
            SetupAllInjections();

            // 5. Null 배열 수정
            FixAllNullInjectionArrays();

            // 씬 변경 표시
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            Debug.Log("=== Conveyor Gun Game Auto Setup Completed ===");
            EditorUtility.DisplayDialog("Setup Complete",
                "Conveyor Gun Game setup completed!\n\nPlease save the scene (Ctrl+S).", "OK");
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

                // Collider 추가 (없으면)
                if (target.GetComponent<Collider>() == null)
                {
                    var col = target.AddComponent<BoxCollider>();
                    col.isTrigger = true;
                    col.size = new Vector3(0.5f, 0.5f, 0.5f);
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
                    var meshFilter = bullet.AddComponent<MeshFilter>();
                    meshFilter.sharedMesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");

                    var meshRenderer = bullet.AddComponent<MeshRenderer>();
                    meshRenderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
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

            Debug.Log("Gun setup completed: " + GUN_NAME);
            EditorUtility.SetDirty(gun);
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

            // UI 관련 (GameCanvas 하위에서 찾기)
            if (gameCanvas != null)
            {
                GameObject scoreText = FindChildRecursive(gameCanvas.transform, "ScoreText");
                GameObject timerText = FindChildRecursive(gameCanvas.transform, "TimerText");
                GameObject startUIPanel = FindChildRecursive(gameCanvas.transform, "StartUIPanel");
                GameObject gameUIPanel = FindChildRecursive(gameCanvas.transform, "GameUIPanel");
                GameObject gameOverUIPanel = FindChildRecursive(gameCanvas.transform, "GameOverUIPanel");
                GameObject startButton = FindChildRecursive(gameCanvas.transform, "StartButton");
                GameObject restartButton = FindChildRecursive(gameCanvas.transform, "RestartButton");

                EnsureGameObjectInjection(ref injection.gameObjectValues, "ScoreTextObject", scoreText);
                EnsureGameObjectInjection(ref injection.gameObjectValues, "TimerTextObject", timerText);
                EnsureGameObjectInjection(ref injection.gameObjectValues, "StartUIPanel", startUIPanel);
                EnsureGameObjectInjection(ref injection.gameObjectValues, "GameUIPanel", gameUIPanel);
                EnsureGameObjectInjection(ref injection.gameObjectValues, "GameOverUIPanel", gameOverUIPanel);
                EnsureGameObjectInjection(ref injection.gameObjectValues, "StartButtonObject", startButton);
                EnsureGameObjectInjection(ref injection.gameObjectValues, "RestartButtonObject", restartButton);
            }

            // Float/Int Injections (기본값)
            EnsureFloatInjection(ref injection.floatValue, "gameTime", 60f);
            EnsureFloatInjection(ref injection.floatValue, "spawnInterval", 2f);
            EnsureIntInjection(ref injection.intValue, "maxTargetCount", 10);
            EnsureIntInjection(ref injection.intValue, "missedPenalty", 5);

            EditorUtility.SetDirty(luaBehaviour);
            Debug.Log("GameManager injection setup completed");
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

        #endregion

        #region Utility Helpers

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
            // 태그가 존재하는지 확인
            try
            {
                obj.tag = tagName;
            }
            catch
            {
                Debug.LogWarning($"Tag '{tagName}' does not exist. Please create it manually in Tags & Layers.");
            }
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
