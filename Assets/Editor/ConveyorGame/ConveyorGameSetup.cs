using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace ConveyorGame.Editor
{
    /// <summary>
    /// 컨베이어 게임을 Viven SDK로 변환하는 에디터 도구
    /// C# 스크립트를 Lua 스크립트로 교체하고 VObject 컴포넌트를 추가합니다.
    /// </summary>
    public class ConveyorGameSetup : EditorWindow
    {
        private const string LUA_SCRIPTS_PATH = "Assets/1_ConveyorGunGame/Scripts";
        private const string BALL_PREFABS_PATH = "Assets/1_ConveyorGunGame/Balls/Prefabs";

        private bool removeOldScripts = true;
        private bool addVivenComponents = true;
        private bool setupBallPrefabs = true;

        [MenuItem("Viven SDK/Conveyor Game Setup")]
        public static void ShowWindow()
        {
            GetWindow<ConveyorGameSetup>("Conveyor Game Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("컨베이어 게임 Viven SDK 변환 도구", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "이 도구는 기존 C# 스크립트를 Viven SDK Lua 스크립트로 변환합니다.\n" +
                "- BallSpawner → BallSpawner.lua\n" +
                "- ConveyorBelt → ConveyorBelt.lua\n" +
                "- DestroyOnContact → BallDestroyer.lua\n" +
                "- ConveyorScroll → ConveyorScroll.lua",
                MessageType.Info
            );

            EditorGUILayout.Space();

            removeOldScripts = EditorGUILayout.Toggle("기존 C# 스크립트 제거", removeOldScripts);
            addVivenComponents = EditorGUILayout.Toggle("Viven 컴포넌트 추가", addVivenComponents);
            setupBallPrefabs = EditorGUILayout.Toggle("공 프리팹 설정", setupBallPrefabs);

            EditorGUILayout.Space();

            if (GUILayout.Button("1. Lua 스크립트 확인", GUILayout.Height(30)))
            {
                CheckLuaScripts();
            }

            if (GUILayout.Button("2. 씬 오브젝트 검색", GUILayout.Height(30)))
            {
                FindSceneObjects();
            }

            EditorGUILayout.Space();

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("3. Viven SDK로 변환 실행", GUILayout.Height(40)))
            {
                ConvertToVivenSDK();
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space();

            if (GUILayout.Button("공 프리팹에 Rigidbody 추가", GUILayout.Height(25)))
            {
                SetupBallPrefabsRigidbody();
            }
        }

        private void CheckLuaScripts()
        {
            string[] luaFiles = new string[]
            {
                "Objects/BallSpawner.lua",
                "Objects/ConveyorBelt.lua",
                "Objects/BallDestroyer.lua",
                "Objects/ConveyorScroll.lua",
                "Manager/ConveyorGameManager.lua"
            };

            Debug.Log("=== Lua 스크립트 확인 ===");

            foreach (var file in luaFiles)
            {
                string fullPath = Path.Combine(LUA_SCRIPTS_PATH, file);
                bool exists = File.Exists(fullPath);

                if (exists)
                {
                    Debug.Log($"✓ {file} - 존재함");
                }
                else
                {
                    Debug.LogWarning($"✗ {file} - 없음! 먼저 Lua 스크립트를 생성하세요.");
                }
            }
        }

        private void FindSceneObjects()
        {
            Debug.Log("=== 씬 오브젝트 검색 ===");

            // BallSpawner 찾기
            var ballSpawners = FindObjectsOfType<BallSpawner>();
            Debug.Log($"BallSpawner: {ballSpawners.Length}개 발견");
            foreach (var obj in ballSpawners)
            {
                Debug.Log($"  - {obj.gameObject.name} (위치: {GetHierarchyPath(obj.gameObject)})");
            }

            // ConveyorBelt 찾기
            var conveyorBelts = FindObjectsOfType<ConveyorBelt>();
            Debug.Log($"ConveyorBelt: {conveyorBelts.Length}개 발견");
            foreach (var obj in conveyorBelts)
            {
                Debug.Log($"  - {obj.gameObject.name} (위치: {GetHierarchyPath(obj.gameObject)})");
            }

            // DestroyOnContact 찾기
            var destroyOnContacts = FindObjectsOfType<DestroyOnContact>();
            Debug.Log($"DestroyOnContact: {destroyOnContacts.Length}개 발견");
            foreach (var obj in destroyOnContacts)
            {
                Debug.Log($"  - {obj.gameObject.name} (위치: {GetHierarchyPath(obj.gameObject)})");
            }
        }

        private void ConvertToVivenSDK()
        {
            if (!EditorUtility.DisplayDialog(
                "Viven SDK 변환",
                "기존 C# 스크립트를 Viven SDK Lua 스크립트로 변환합니다.\n\n" +
                "이 작업은 되돌릴 수 없습니다. 계속하시겠습니까?",
                "변환 시작",
                "취소"))
            {
                return;
            }

            int convertedCount = 0;

            // 1. BallSpawner 변환
            var ballSpawners = FindObjectsOfType<BallSpawner>();
            foreach (var spawner in ballSpawners)
            {
                ConvertBallSpawner(spawner);
                convertedCount++;
            }

            // 2. ConveyorBelt 변환
            var conveyorBelts = FindObjectsOfType<ConveyorBelt>();
            foreach (var belt in conveyorBelts)
            {
                ConvertConveyorBelt(belt);
                convertedCount++;
            }

            // 3. DestroyOnContact 변환
            var destroyOnContacts = FindObjectsOfType<DestroyOnContact>();
            foreach (var destroyer in destroyOnContacts)
            {
                ConvertDestroyOnContact(destroyer);
                convertedCount++;
            }

            // 씬 저장
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
            );

            Debug.Log($"=== 변환 완료: {convertedCount}개 오브젝트 ===");
            EditorUtility.DisplayDialog("완료", $"{convertedCount}개 오브젝트가 변환되었습니다.", "확인");
        }

        private void ConvertBallSpawner(BallSpawner spawner)
        {
            GameObject go = spawner.gameObject;
            Debug.Log($"BallSpawner 변환: {go.name}");

            // 기존 데이터 저장
            var ballPrefabs = spawner.ballPrefabs;
            var spawnInterval = spawner.spawnInterval;
            var spawnPoint = spawner.spawnPoint;

            // 기존 스크립트 제거
            if (removeOldScripts)
            {
                DestroyImmediate(spawner);
            }

            // Viven 컴포넌트 추가
            if (addVivenComponents)
            {
                // VObject가 없으면 추가
                var vObject = go.GetComponent("VObject");
                if (vObject == null)
                {
                    Debug.Log($"  → VObject 추가 필요 (수동으로 추가하세요)");
                }

                // VivenLuaBehaviour 추가
                var luaBehaviour = go.GetComponent("VivenLuaBehaviour");
                if (luaBehaviour == null)
                {
                    Debug.Log($"  → VivenLuaBehaviour 추가 필요 (수동으로 추가하세요)");
                    Debug.Log($"  → Lua 스크립트: BallSpawner.lua");
                    Debug.Log($"  → 주입 변수:");
                    Debug.Log($"     - BallPrefabs: {ballPrefabs?.Length ?? 0}개 프리팹");
                    Debug.Log($"     - SpawnPoint: {(spawnPoint != null ? spawnPoint.name : "null")}");
                }
            }
        }

        private void ConvertConveyorBelt(ConveyorBelt belt)
        {
            GameObject go = belt.gameObject;
            Debug.Log($"ConveyorBelt 변환: {go.name}");

            // 기존 데이터 저장
            var speed = belt.speed;
            var direction = belt.direction;

            // 기존 스크립트 제거
            if (removeOldScripts)
            {
                DestroyImmediate(belt);
            }

            // Viven 컴포넌트 추가
            if (addVivenComponents)
            {
                var vObject = go.GetComponent("VObject");
                if (vObject == null)
                {
                    Debug.Log($"  → VObject 추가 필요 (수동으로 추가하세요)");
                }

                var luaBehaviour = go.GetComponent("VivenLuaBehaviour");
                if (luaBehaviour == null)
                {
                    Debug.Log($"  → VivenLuaBehaviour 추가 필요 (수동으로 추가하세요)");
                    Debug.Log($"  → Lua 스크립트: ConveyorBelt.lua");
                    Debug.Log($"  → 주입 변수:");
                    Debug.Log($"     - speed: {speed}");
                    Debug.Log($"     - direction: {direction}");
                }
            }
        }

        private void ConvertDestroyOnContact(DestroyOnContact destroyer)
        {
            GameObject go = destroyer.gameObject;
            Debug.Log($"DestroyOnContact 변환: {go.name}");

            // 기존 스크립트 제거
            if (removeOldScripts)
            {
                DestroyImmediate(destroyer);
            }

            // Viven 컴포넌트 추가
            if (addVivenComponents)
            {
                var vObject = go.GetComponent("VObject");
                if (vObject == null)
                {
                    Debug.Log($"  → VObject 추가 필요 (수동으로 추가하세요)");
                }

                var luaBehaviour = go.GetComponent("VivenLuaBehaviour");
                if (luaBehaviour == null)
                {
                    Debug.Log($"  → VivenLuaBehaviour 추가 필요 (수동으로 추가하세요)");
                    Debug.Log($"  → Lua 스크립트: BallDestroyer.lua");
                }
            }
        }

        private void SetupBallPrefabsRigidbody()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { BALL_PREFABS_PATH });
            int modifiedCount = 0;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab != null)
                {
                    // 프리팹 편집 시작
                    string prefabPath = AssetDatabase.GetAssetPath(prefab);
                    GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);

                    // Rigidbody 확인 및 추가
                    Rigidbody rb = prefabRoot.GetComponent<Rigidbody>();
                    if (rb == null)
                    {
                        rb = prefabRoot.AddComponent<Rigidbody>();
                        rb.mass = 0.5f;
                        rb.linearDamping = 0.5f;
                        rb.angularDamping = 0.5f;
                        rb.useGravity = true;

                        Debug.Log($"Rigidbody 추가: {prefab.name}");
                        modifiedCount++;
                    }

                    // Collider 확인
                    Collider col = prefabRoot.GetComponent<Collider>();
                    if (col == null)
                    {
                        SphereCollider sphereCol = prefabRoot.AddComponent<SphereCollider>();
                        sphereCol.radius = 0.5f;
                        Debug.Log($"SphereCollider 추가: {prefab.name}");
                    }

                    // 프리팹 저장
                    PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"=== 공 프리팹 설정 완료: {modifiedCount}개 수정됨 ===");
        }

        private string GetHierarchyPath(GameObject go)
        {
            string path = go.name;
            Transform parent = go.transform.parent;

            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
        }
    }
}
