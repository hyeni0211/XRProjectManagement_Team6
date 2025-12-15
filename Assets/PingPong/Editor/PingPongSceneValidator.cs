using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace PingPong.Editor
{
    /// <summary>
    /// PingPong 씬의 VivenLuaBehaviour Injection 구조를 검증하는 에디터 도구
    /// </summary>
    public class PingPongSceneValidator : EditorWindow
    {
        private Vector2 scrollPosition;
        private List<ValidationResult> validationResults = new List<ValidationResult>();
        private bool showPassed = true;
        private bool showWarnings = true;
        private bool showErrors = true;

        [MenuItem("Viven/PingPong/🔍 씬 검증기 (Scene Validator)", false, 100)]
        public static void ShowWindow()
        {
            var window = GetWindow<PingPongSceneValidator>("PingPong 씬 검증기");
            window.minSize = new Vector2(600, 400);
        }

        [MenuItem("Viven/PingPong/▶ 빠른 검증 (Quick Validate)", false, 101)]
        public static void QuickValidate()
        {
            // EditorWindow를 직접 new하면 안됨 - 정적 메서드로 검증 수행
            var results = RunStaticValidation();
            PrintStaticResultsToConsole(results);
        }

        private static List<ValidationResult> RunStaticValidation()
        {
            var results = new List<ValidationResult>();

            // 1. VivenLuaBehaviour 컴포넌트 검증
            ValidateLuaBehavioursStatic(results);

            // 2. 필수 씬 오브젝트 검증
            ValidateRequiredObjectsStatic(results);

            // 3. 컴포넌트 구성 검증
            ValidateComponentSetupStatic(results);

            // 4. Lua 스크립트 파일 존재 확인
            ValidateLuaScriptsStatic(results);

            // 5. Injection 매핑 검증
            ValidateInjectionMappingStatic(results);

            Debug.Log($"[PingPong 검증기] 검증 완료: ✅ {results.Count(r => r.Type == ResultType.Pass)} | ⚠️ {results.Count(r => r.Type == ResultType.Warning)} | ❌ {results.Count(r => r.Type == ResultType.Error)}");

            return results;
        }

        private static void ValidateLuaBehavioursStatic(List<ValidationResult> results)
        {
            var luaBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .Where(mb => mb.GetType().Name == "VivenLuaBehaviour")
                .ToList();

            if (luaBehaviours.Count == 0)
            {
                results.Add(new ValidationResult
                {
                    Type = ResultType.Error,
                    Category = "VivenLuaBehaviour",
                    Message = "씬에 VivenLuaBehaviour 컴포넌트가 없습니다!",
                    Details = "PingPong 게임에는 최소 3개의 Lua 스크립트가 필요합니다."
                });
                return;
            }

            results.Add(new ValidationResult
            {
                Type = ResultType.Pass,
                Category = "VivenLuaBehaviour",
                Message = $"VivenLuaBehaviour {luaBehaviours.Count}개 발견",
                Details = string.Join(", ", luaBehaviours.Select(lb => lb.gameObject.name))
            });

            foreach (var lb in luaBehaviours)
            {
                ValidateSingleLuaBehaviourStatic(lb, results);
            }
        }

        private static void ValidateSingleLuaBehaviourStatic(MonoBehaviour luaBehaviour, List<ValidationResult> results)
        {
            var go = luaBehaviour.gameObject;
            var type = luaBehaviour.GetType();

            var luaScriptField = type.GetField("luaScript", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            object scriptValue = null;

            if (luaScriptField != null)
            {
                scriptValue = luaScriptField.GetValue(luaBehaviour);
            }
            else
            {
                var luaScriptProp = type.GetProperty("LuaScript", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (luaScriptProp != null)
                {
                    scriptValue = luaScriptProp.GetValue(luaBehaviour);
                }
            }

            if (scriptValue == null)
            {
                results.Add(new ValidationResult
                {
                    Type = ResultType.Error,
                    Category = $"{go.name}",
                    Message = "Lua 스크립트가 할당되지 않았습니다!",
                    Target = go
                });
            }
            else
            {
                var scriptPath = scriptValue.ToString();
                var scriptName = Path.GetFileNameWithoutExtension(scriptPath);
                results.Add(new ValidationResult
                {
                    Type = ResultType.Pass,
                    Category = $"{go.name}",
                    Message = $"Lua 스크립트: {scriptName}",
                    Details = scriptPath,
                    Target = go
                });
            }

            var injectionField = type.GetField("injection", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (injectionField != null)
            {
                var injection = injectionField.GetValue(luaBehaviour);
                ValidateInjectionStatic(go, injection, results);
            }
        }

        private static void ValidateInjectionStatic(GameObject go, object injection, List<ValidationResult> results)
        {
            if (injection == null) return;

            var injectionType = injection.GetType();

            var goValuesField = injectionType.GetField("gameObjectValues", BindingFlags.Public | BindingFlags.Instance);
            if (goValuesField != null)
            {
                var goValues = goValuesField.GetValue(injection) as System.Collections.IList;
                if (goValues != null)
                {
                    foreach (var item in goValues)
                    {
                        var nameField = item.GetType().GetField("name", BindingFlags.Public | BindingFlags.Instance);
                        var valueField = item.GetType().GetField("value", BindingFlags.Public | BindingFlags.Instance);

                        if (nameField != null && valueField != null)
                        {
                            var name = nameField.GetValue(item)?.ToString() ?? "Unknown";
                            var value = valueField.GetValue(item);

                            if (value == null)
                            {
                                results.Add(new ValidationResult
                                {
                                    Type = ResultType.Error,
                                    Category = $"{go.name}",
                                    Message = $"Injection '{name}'이 null입니다!",
                                    Details = "이 필드가 checkInject()를 사용하면 런타임 에러가 발생합니다.",
                                    Target = go
                                });
                            }
                            else
                            {
                                var valueGO = value as GameObject;
                                var valueName = valueGO != null ? valueGO.name : value.ToString();

                                results.Add(new ValidationResult
                                {
                                    Type = ResultType.Pass,
                                    Category = $"{go.name}",
                                    Message = $"Injection '{name}' → {valueName}",
                                    Target = go
                                });
                            }
                        }
                    }
                }
            }

            var stringValuesField = injectionType.GetField("stringValue", BindingFlags.Public | BindingFlags.Instance);
            if (stringValuesField != null)
            {
                var stringValues = stringValuesField.GetValue(injection) as System.Collections.IList;
                if (stringValues != null && stringValues.Count > 0)
                {
                    foreach (var item in stringValues)
                    {
                        var nameField = item.GetType().GetField("name", BindingFlags.Public | BindingFlags.Instance);
                        var valueField = item.GetType().GetField("value", BindingFlags.Public | BindingFlags.Instance);

                        if (nameField != null && valueField != null)
                        {
                            var name = nameField.GetValue(item)?.ToString() ?? "Unknown";
                            var value = valueField.GetValue(item)?.ToString() ?? "null";

                            results.Add(new ValidationResult
                            {
                                Type = ResultType.Pass,
                                Category = $"{go.name}",
                                Message = $"String Injection '{name}' = \"{value}\"",
                                Target = go
                            });
                        }
                    }
                }
            }
        }

        private static void ValidateRequiredObjectsStatic(List<ValidationResult> results)
        {
            var requiredObjects = new Dictionary<string, string>
            {
                { "PingPongGameManager", "게임 매니저" },
                { "Racket", "탁구채" },
                { "BallLauncher", "공 발사기" },
                { "LaunchPoint", "공 발사 위치" },
                { "Table", "탁구대" }
            };

            foreach (var kvp in requiredObjects)
            {
                var obj = GameObject.Find(kvp.Key);
                if (obj == null)
                {
                    results.Add(new ValidationResult
                    {
                        Type = ResultType.Error,
                        Category = "필수 오브젝트",
                        Message = $"'{kvp.Key}' ({kvp.Value})를 찾을 수 없습니다!",
                    });
                }
                else
                {
                    results.Add(new ValidationResult
                    {
                        Type = ResultType.Pass,
                        Category = "필수 오브젝트",
                        Message = $"'{kvp.Key}' ({kvp.Value}) 존재",
                        Target = obj
                    });
                }
            }
        }

        private static void ValidateComponentSetupStatic(List<ValidationResult> results)
        {
            var racket = GameObject.Find("Racket");
            if (racket != null)
            {
                ValidateRacketComponentsStatic(racket, results);
            }

            var launcher = GameObject.Find("BallLauncher");
            if (launcher != null)
            {
                ValidateBallLauncherComponentsStatic(launcher, results);
            }
        }

        private static void ValidateRacketComponentsStatic(GameObject racket, List<ValidationResult> results)
        {
            var requiredComponents = new[]
            {
                ("VObject", "Twoz.Viven.Interactions.VObject"),
                ("VivenGrabbableModule", "Twoz.Viven.Interactions.VivenGrabbableModule"),
                ("VivenRigidbodyControlModule", "Twoz.Viven.Interactions.VivenRigidbodyControlModule"),
                ("VivenGrabbableRigidView", "Twoz.Viven.Interactions.VivenGrabbableRigidView"),
                ("Collider", "UnityEngine.Collider")
            };

            foreach (var (displayName, typeName) in requiredComponents)
            {
                Component comp = null;

                if (typeName == "UnityEngine.Collider")
                {
                    comp = racket.GetComponent<Collider>();
                }
                else
                {
                    comp = racket.GetComponents<Component>()
                        .FirstOrDefault(c => c.GetType().FullName == typeName);
                }

                if (comp == null)
                {
                    results.Add(new ValidationResult
                    {
                        Type = ResultType.Error,
                        Category = "Racket 컴포넌트",
                        Message = $"'{displayName}' 컴포넌트가 없습니다!",
                        Details = "탁구채가 제대로 동작하려면 이 컴포넌트가 필요합니다.",
                        Target = racket
                    });
                }
                else
                {
                    results.Add(new ValidationResult
                    {
                        Type = ResultType.Pass,
                        Category = "Racket 컴포넌트",
                        Message = $"'{displayName}' 존재",
                        Target = racket
                    });
                }
            }
        }

        private static void ValidateBallLauncherComponentsStatic(GameObject launcher, List<ValidationResult> results)
        {
            var launchPoint = launcher.transform.Find("LaunchPoint");
            if (launchPoint == null)
            {
                results.Add(new ValidationResult
                {
                    Type = ResultType.Error,
                    Category = "BallLauncher",
                    Message = "LaunchPoint 자식 오브젝트가 없습니다!",
                    Details = "공이 발사될 위치를 지정하는 빈 오브젝트가 필요합니다.",
                    Target = launcher
                });
            }
            else
            {
                results.Add(new ValidationResult
                {
                    Type = ResultType.Pass,
                    Category = "BallLauncher",
                    Message = "LaunchPoint 자식 오브젝트 존재",
                    Target = launchPoint.gameObject
                });
            }
        }

        private static void ValidateLuaScriptsStatic(List<ValidationResult> results)
        {
            var luaScripts = new[]
            {
                "Assets/PingPong/Scripts/Manager/PingPongGameManager.lua",
                "Assets/PingPong/Scripts/Objects/BallLauncher.lua",
                "Assets/PingPong/Scripts/Objects/PingPongBall.lua",
                "Assets/PingPong/Scripts/Objects/PingPongRacket.lua"
            };

            foreach (var scriptPath in luaScripts)
            {
                var fullPath = Path.Combine(Application.dataPath, scriptPath.Replace("Assets/", ""));
                if (File.Exists(fullPath))
                {
                    results.Add(new ValidationResult
                    {
                        Type = ResultType.Pass,
                        Category = "Lua 스크립트",
                        Message = $"{Path.GetFileName(scriptPath)} 존재",
                        Details = scriptPath
                    });
                }
                else
                {
                    results.Add(new ValidationResult
                    {
                        Type = ResultType.Error,
                        Category = "Lua 스크립트",
                        Message = $"{Path.GetFileName(scriptPath)} 파일이 없습니다!",
                        Details = scriptPath
                    });
                }
            }
        }

        private static void ValidateInjectionMappingStatic(List<ValidationResult> results)
        {
            ValidateObjectInjectionStatic("PingPongGameManager", "Assets/PingPong/Scripts/Manager/PingPongGameManager.lua", results);
            ValidateObjectInjectionStatic("Racket", "Assets/PingPong/Scripts/Objects/PingPongRacket.lua", results);
            ValidateObjectInjectionStatic("BallLauncher", "Assets/PingPong/Scripts/Objects/BallLauncher.lua", results);
        }

        private static void ValidateObjectInjectionStatic(string objectName, string luaPath, List<ValidationResult> results)
        {
            var go = GameObject.Find(objectName);
            if (go == null) return;

            var luaBehaviour = go.GetComponents<Component>()
                .FirstOrDefault(c => c.GetType().Name == "VivenLuaBehaviour");

            if (luaBehaviour == null) return;

            var fullPath = Path.Combine(Application.dataPath, luaPath.Replace("Assets/", ""));
            if (!File.Exists(fullPath)) return;

            var luaContent = File.ReadAllText(fullPath);

            // checkInject 패턴 찾기
            var checkInjectPattern = @"(\w+)\s*=\s*checkInject\(\1\)";
            var requiredInjections = Regex.Matches(luaContent, checkInjectPattern)
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .ToList();

            // NullableInject 패턴 찾기
            var nullablePattern = @"(\w+)\s*=\s*NullableInject\(\1\)";
            var optionalInjections = Regex.Matches(luaContent, nullablePattern)
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .ToList();

            // 현재 injection 값들 가져오기
            var type = luaBehaviour.GetType();
            var injectionField = type.GetField("injection", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var currentInjections = new HashSet<string>();
            if (injectionField != null)
            {
                var injection = injectionField.GetValue(luaBehaviour);
                if (injection != null)
                {
                    var injectionType = injection.GetType();

                    var goValuesField = injectionType.GetField("gameObjectValues", BindingFlags.Public | BindingFlags.Instance);
                    if (goValuesField != null)
                    {
                        var goValues = goValuesField.GetValue(injection) as System.Collections.IList;
                        if (goValues != null)
                        {
                            foreach (var item in goValues)
                            {
                                var nameField = item.GetType().GetField("name", BindingFlags.Public | BindingFlags.Instance);
                                if (nameField != null)
                                {
                                    currentInjections.Add(nameField.GetValue(item)?.ToString() ?? "");
                                }
                            }
                        }
                    }

                    var stringValuesField = injectionType.GetField("stringValue", BindingFlags.Public | BindingFlags.Instance);
                    if (stringValuesField != null)
                    {
                        var stringValues = stringValuesField.GetValue(injection) as System.Collections.IList;
                        if (stringValues != null)
                        {
                            foreach (var item in stringValues)
                            {
                                var nameField = item.GetType().GetField("name", BindingFlags.Public | BindingFlags.Instance);
                                if (nameField != null)
                                {
                                    currentInjections.Add(nameField.GetValue(item)?.ToString() ?? "");
                                }
                            }
                        }
                    }
                }
            }

            // 필수 injection 누락 확인
            foreach (var required in requiredInjections)
            {
                if (!currentInjections.Contains(required))
                {
                    results.Add(new ValidationResult
                    {
                        Type = ResultType.Error,
                        Category = $"{objectName} Injection",
                        Message = $"필수 Injection '{required}'이 누락되었습니다!",
                        Details = "checkInject()로 선언된 변수는 반드시 Inspector에서 연결해야 합니다.",
                        Target = go
                    });
                }
            }

            // 선택적 injection 누락 확인
            foreach (var optional in optionalInjections)
            {
                if (!currentInjections.Contains(optional))
                {
                    results.Add(new ValidationResult
                    {
                        Type = ResultType.Warning,
                        Category = $"{objectName} Injection",
                        Message = $"선택적 Injection '{optional}'이 연결되지 않았습니다.",
                        Details = "NullableInject()로 선언된 변수입니다. 기능이 제한될 수 있습니다.",
                        Target = go
                    });
                }
            }

            // Lua에 없는 injection 확인
            var allLuaInjections = requiredInjections.Concat(optionalInjections).ToHashSet();
            foreach (var current in currentInjections)
            {
                if (!allLuaInjections.Contains(current))
                {
                    results.Add(new ValidationResult
                    {
                        Type = ResultType.Warning,
                        Category = $"{objectName} Injection",
                        Message = $"불필요한 Injection '{current}'이 있습니다.",
                        Details = "Lua 스크립트에서 사용하지 않는 변수입니다. 제거해도 됩니다.",
                        Target = go
                    });
                }
            }
        }

        private static void PrintStaticResultsToConsole(List<ValidationResult> results)
        {
            var sb = new StringBuilder();
            sb.AppendLine("\n═══════════════════════════════════════════════════════════");
            sb.AppendLine("                 🏓 PingPong 씬 검증 결과");
            sb.AppendLine("═══════════════════════════════════════════════════════════\n");

            var errors = results.Where(r => r.Type == ResultType.Error).ToList();
            var warnings = results.Where(r => r.Type == ResultType.Warning).ToList();
            var passes = results.Where(r => r.Type == ResultType.Pass).ToList();

            sb.AppendLine($"📊 요약: ✅ {passes.Count} 통과 | ⚠️ {warnings.Count} 경고 | ❌ {errors.Count} 오류\n");

            if (errors.Count > 0)
            {
                sb.AppendLine("❌ 오류 (반드시 수정 필요):");
                sb.AppendLine("───────────────────────────────────────────────────────────");
                foreach (var error in errors)
                {
                    sb.AppendLine($"  [{error.Category}] {error.Message}");
                    if (!string.IsNullOrEmpty(error.Details))
                        sb.AppendLine($"     └─ {error.Details}");
                }
                sb.AppendLine();
            }

            if (warnings.Count > 0)
            {
                sb.AppendLine("⚠️ 경고 (확인 권장):");
                sb.AppendLine("───────────────────────────────────────────────────────────");
                foreach (var warning in warnings)
                {
                    sb.AppendLine($"  [{warning.Category}] {warning.Message}");
                    if (!string.IsNullOrEmpty(warning.Details))
                        sb.AppendLine($"     └─ {warning.Details}");
                }
                sb.AppendLine();
            }

            sb.AppendLine($"✅ 통과 항목: {passes.Count}개 (상세 내용은 Editor 창에서 확인)");
            sb.AppendLine("\n═══════════════════════════════════════════════════════════\n");

            if (errors.Count > 0)
                Debug.LogError(sb.ToString());
            else if (warnings.Count > 0)
                Debug.LogWarning(sb.ToString());
            else
                Debug.Log(sb.ToString());
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            
            // 헤더
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("🏓 PingPong 씬 검증기", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            // 버튼 영역
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("🔍 전체 검증 실행", GUILayout.Height(30)))
            {
                RunValidation();
            }
            if (GUILayout.Button("📋 콘솔에 출력", GUILayout.Height(30)))
            {
                PrintResultsToConsole();
            }
            if (GUILayout.Button("🔧 자동 수정 시도", GUILayout.Height(30)))
            {
                TryAutoFix();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            
            // 필터
            EditorGUILayout.BeginHorizontal();
            showPassed = GUILayout.Toggle(showPassed, "✅ 통과", "Button");
            showWarnings = GUILayout.Toggle(showWarnings, "⚠️ 경고", "Button");
            showErrors = GUILayout.Toggle(showErrors, "❌ 오류", "Button");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // 결과 요약
            var errorCount = validationResults.Count(r => r.Type == ResultType.Error);
            var warningCount = validationResults.Count(r => r.Type == ResultType.Warning);
            var passCount = validationResults.Count(r => r.Type == ResultType.Pass);
            
            EditorGUILayout.LabelField($"결과: ✅ {passCount} | ⚠️ {warningCount} | ❌ {errorCount}", EditorStyles.boldLabel);

            EditorGUILayout.Space(5);

            // 결과 리스트
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            foreach (var result in validationResults)
            {
                if (result.Type == ResultType.Pass && !showPassed) continue;
                if (result.Type == ResultType.Warning && !showWarnings) continue;
                if (result.Type == ResultType.Error && !showErrors) continue;

                DrawResultItem(result);
            }
            
            EditorGUILayout.EndScrollView();
        }

        private void DrawResultItem(ValidationResult result)
        {
            var bgColor = result.Type switch
            {
                ResultType.Pass => new Color(0.2f, 0.6f, 0.2f, 0.2f),
                ResultType.Warning => new Color(0.8f, 0.6f, 0.1f, 0.2f),
                ResultType.Error => new Color(0.8f, 0.2f, 0.2f, 0.2f),
                _ => Color.gray
            };

            var icon = result.Type switch
            {
                ResultType.Pass => "✅",
                ResultType.Warning => "⚠️",
                ResultType.Error => "❌",
                _ => "❓"
            };

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            GUI.backgroundColor = bgColor;
            EditorGUILayout.BeginHorizontal();
            
            GUILayout.Label(icon, GUILayout.Width(25));
            EditorGUILayout.LabelField(result.Category, EditorStyles.boldLabel, GUILayout.Width(150));
            
            if (result.Target != null && GUILayout.Button("선택", GUILayout.Width(50)))
            {
                Selection.activeGameObject = result.Target;
                EditorGUIUtility.PingObject(result.Target);
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.LabelField(result.Message, EditorStyles.wordWrappedLabel);
            
            if (!string.IsNullOrEmpty(result.Details))
            {
                EditorGUILayout.LabelField(result.Details, EditorStyles.miniLabel);
            }
            
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(2);
        }

        private void RunValidation()
        {
            validationResults.Clear();
            
            // 1. VivenLuaBehaviour 컴포넌트 검증
            ValidateLuaBehaviours();
            
            // 2. 필수 씬 오브젝트 검증
            ValidateRequiredObjects();
            
            // 3. 컴포넌트 구성 검증
            ValidateComponentSetup();
            
            // 4. Lua 스크립트 파일 존재 확인
            ValidateLuaScripts();
            
            // 5. Injection 매핑 검증
            ValidateInjectionMapping();

            Debug.Log($"[PingPong 검증기] 검증 완료: ✅ {validationResults.Count(r => r.Type == ResultType.Pass)} | ⚠️ {validationResults.Count(r => r.Type == ResultType.Warning)} | ❌ {validationResults.Count(r => r.Type == ResultType.Error)}");
        }

        private void ValidateLuaBehaviours()
        {
            var luaBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .Where(mb => mb.GetType().Name == "VivenLuaBehaviour")
                .ToList();

            if (luaBehaviours.Count == 0)
            {
                validationResults.Add(new ValidationResult
                {
                    Type = ResultType.Error,
                    Category = "VivenLuaBehaviour",
                    Message = "씬에 VivenLuaBehaviour 컴포넌트가 없습니다!",
                    Details = "PingPong 게임에는 최소 3개의 Lua 스크립트가 필요합니다."
                });
                return;
            }

            validationResults.Add(new ValidationResult
            {
                Type = ResultType.Pass,
                Category = "VivenLuaBehaviour",
                Message = $"VivenLuaBehaviour {luaBehaviours.Count}개 발견",
                Details = string.Join(", ", luaBehaviours.Select(lb => lb.gameObject.name))
            });

            foreach (var lb in luaBehaviours)
            {
                ValidateSingleLuaBehaviour(lb);
            }
        }

        private void ValidateSingleLuaBehaviour(MonoBehaviour luaBehaviour)
        {
            var go = luaBehaviour.gameObject;
            var type = luaBehaviour.GetType();
            
            // luaScript 필드 확인
            var luaScriptField = type.GetField("luaScript", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (luaScriptField == null)
            {
                // 프로퍼티로 시도
                var luaScriptProp = type.GetProperty("LuaScript", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (luaScriptProp != null)
                {
                    var scriptObj = luaScriptProp.GetValue(luaBehaviour);
                    CheckLuaScriptAssignment(go, scriptObj);
                }
                return;
            }

            var scriptValue = luaScriptField.GetValue(luaBehaviour);
            CheckLuaScriptAssignment(go, scriptValue);
            
            // injection 필드 확인
            var injectionField = type.GetField("injection", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (injectionField != null)
            {
                var injection = injectionField.GetValue(luaBehaviour);
                ValidateInjection(go, injection, scriptValue?.ToString() ?? "Unknown");
            }
        }

        private void CheckLuaScriptAssignment(GameObject go, object scriptObj)
        {
            if (scriptObj == null)
            {
                validationResults.Add(new ValidationResult
                {
                    Type = ResultType.Error,
                    Category = $"{go.name}",
                    Message = "Lua 스크립트가 할당되지 않았습니다!",
                    Target = go
                });
            }
            else
            {
                var scriptPath = scriptObj.ToString();
                var scriptName = Path.GetFileNameWithoutExtension(scriptPath);
                validationResults.Add(new ValidationResult
                {
                    Type = ResultType.Pass,
                    Category = $"{go.name}",
                    Message = $"Lua 스크립트: {scriptName}",
                    Details = scriptPath,
                    Target = go
                });
            }
        }

        private void ValidateInjection(GameObject go, object injection, string scriptPath)
        {
            if (injection == null) return;

            var injectionType = injection.GetType();
            
            // gameObjectValues 확인
            var goValuesField = injectionType.GetField("gameObjectValues", BindingFlags.Public | BindingFlags.Instance);
            if (goValuesField != null)
            {
                var goValues = goValuesField.GetValue(injection) as System.Collections.IList;
                if (goValues != null)
                {
                    foreach (var item in goValues)
                    {
                        var nameField = item.GetType().GetField("name", BindingFlags.Public | BindingFlags.Instance);
                        var valueField = item.GetType().GetField("value", BindingFlags.Public | BindingFlags.Instance);
                        
                        if (nameField != null && valueField != null)
                        {
                            var name = nameField.GetValue(item)?.ToString() ?? "Unknown";
                            var value = valueField.GetValue(item);
                            
                            if (value == null)
                            {
                                validationResults.Add(new ValidationResult
                                {
                                    Type = ResultType.Error,
                                    Category = $"{go.name}",
                                    Message = $"Injection '{name}'이 null입니다!",
                                    Details = "이 필드가 checkInject()를 사용하면 런타임 에러가 발생합니다.",
                                    Target = go
                                });
                            }
                            else
                            {
                                // GameObject인지 확인
                                var valueGO = value as GameObject;
                                var valueName = valueGO != null ? valueGO.name : value.ToString();
                                
                                validationResults.Add(new ValidationResult
                                {
                                    Type = ResultType.Pass,
                                    Category = $"{go.name}",
                                    Message = $"Injection '{name}' → {valueName}",
                                    Target = go
                                });
                            }
                        }
                    }
                }
            }
            
            // stringValue 확인
            var stringValuesField = injectionType.GetField("stringValue", BindingFlags.Public | BindingFlags.Instance);
            if (stringValuesField != null)
            {
                var stringValues = stringValuesField.GetValue(injection) as System.Collections.IList;
                if (stringValues != null && stringValues.Count > 0)
                {
                    foreach (var item in stringValues)
                    {
                        var nameField = item.GetType().GetField("name", BindingFlags.Public | BindingFlags.Instance);
                        var valueField = item.GetType().GetField("value", BindingFlags.Public | BindingFlags.Instance);
                        
                        if (nameField != null && valueField != null)
                        {
                            var name = nameField.GetValue(item)?.ToString() ?? "Unknown";
                            var value = valueField.GetValue(item)?.ToString() ?? "null";
                            
                            validationResults.Add(new ValidationResult
                            {
                                Type = ResultType.Pass,
                                Category = $"{go.name}",
                                Message = $"String Injection '{name}' = \"{value}\"",
                                Target = go
                            });
                        }
                    }
                }
            }
        }

        private void ValidateRequiredObjects()
        {
            var requiredObjects = new Dictionary<string, string>
            {
                { "PingPongGameManager", "게임 매니저" },
                { "Racket", "탁구채" },
                { "BallLauncher", "공 발사기" },
                { "LaunchPoint", "공 발사 위치" },
                { "Table", "탁구대" }
            };

            foreach (var kvp in requiredObjects)
            {
                var obj = GameObject.Find(kvp.Key);
                if (obj == null)
                {
                    validationResults.Add(new ValidationResult
                    {
                        Type = ResultType.Error,
                        Category = "필수 오브젝트",
                        Message = $"'{kvp.Key}' ({kvp.Value})를 찾을 수 없습니다!",
                    });
                }
                else
                {
                    validationResults.Add(new ValidationResult
                    {
                        Type = ResultType.Pass,
                        Category = "필수 오브젝트",
                        Message = $"'{kvp.Key}' ({kvp.Value}) 존재",
                        Target = obj
                    });
                }
            }
        }

        private void ValidateComponentSetup()
        {
            // Racket 컴포넌트 검증
            var racket = GameObject.Find("Racket");
            if (racket != null)
            {
                ValidateRacketComponents(racket);
            }

            // BallLauncher 검증
            var launcher = GameObject.Find("BallLauncher");
            if (launcher != null)
            {
                ValidateBallLauncherComponents(launcher);
            }
        }

        private void ValidateRacketComponents(GameObject racket)
        {
            var requiredComponents = new[]
            {
                ("VObject", "Twoz.Viven.Interactions.VObject"),
                ("VivenGrabbableModule", "Twoz.Viven.Interactions.VivenGrabbableModule"),
                ("VivenRigidbodyControlModule", "Twoz.Viven.Interactions.VivenRigidbodyControlModule"),
                ("VivenGrabbableRigidView", "Twoz.Viven.Interactions.VivenGrabbableRigidView"),
                ("Collider", "UnityEngine.Collider")
            };

            foreach (var (displayName, typeName) in requiredComponents)
            {
                Component comp = null;
                
                if (typeName == "UnityEngine.Collider")
                {
                    comp = racket.GetComponent<Collider>();
                }
                else
                {
                    comp = racket.GetComponents<Component>()
                        .FirstOrDefault(c => c.GetType().FullName == typeName);
                }

                if (comp == null)
                {
                    validationResults.Add(new ValidationResult
                    {
                        Type = ResultType.Error,
                        Category = "Racket 컴포넌트",
                        Message = $"'{displayName}' 컴포넌트가 없습니다!",
                        Details = "탁구채가 제대로 동작하려면 이 컴포넌트가 필요합니다.",
                        Target = racket
                    });
                }
                else
                {
                    validationResults.Add(new ValidationResult
                    {
                        Type = ResultType.Pass,
                        Category = "Racket 컴포넌트",
                        Message = $"'{displayName}' 존재",
                        Target = racket
                    });
                }
            }
        }

        private void ValidateBallLauncherComponents(GameObject launcher)
        {
            // LaunchPoint 자식 확인
            var launchPoint = launcher.transform.Find("LaunchPoint");
            if (launchPoint == null)
            {
                validationResults.Add(new ValidationResult
                {
                    Type = ResultType.Error,
                    Category = "BallLauncher",
                    Message = "LaunchPoint 자식 오브젝트가 없습니다!",
                    Details = "공이 발사될 위치를 지정하는 빈 오브젝트가 필요합니다.",
                    Target = launcher
                });
            }
            else
            {
                validationResults.Add(new ValidationResult
                {
                    Type = ResultType.Pass,
                    Category = "BallLauncher",
                    Message = "LaunchPoint 자식 오브젝트 존재",
                    Target = launchPoint.gameObject
                });
            }
        }

        private void ValidateLuaScripts()
        {
            var luaScripts = new[]
            {
                "Assets/PingPong/Scripts/Manager/PingPongGameManager.lua",
                "Assets/PingPong/Scripts/Objects/BallLauncher.lua",
                "Assets/PingPong/Scripts/Objects/PingPongBall.lua",
                "Assets/PingPong/Scripts/Objects/PingPongRacket.lua"
            };

            foreach (var scriptPath in luaScripts)
            {
                var fullPath = Path.Combine(Application.dataPath, scriptPath.Replace("Assets/", ""));
                if (File.Exists(fullPath))
                {
                    validationResults.Add(new ValidationResult
                    {
                        Type = ResultType.Pass,
                        Category = "Lua 스크립트",
                        Message = $"{Path.GetFileName(scriptPath)} 존재",
                        Details = scriptPath
                    });
                }
                else
                {
                    validationResults.Add(new ValidationResult
                    {
                        Type = ResultType.Error,
                        Category = "Lua 스크립트",
                        Message = $"{Path.GetFileName(scriptPath)} 파일이 없습니다!",
                        Details = scriptPath
                    });
                }
            }
        }

        private void ValidateInjectionMapping()
        {
            // PingPongGameManager의 Injection 검증
            var gameManager = GameObject.Find("PingPongGameManager");
            if (gameManager != null)
            {
                var luaBehaviour = gameManager.GetComponents<Component>()
                    .FirstOrDefault(c => c.GetType().Name == "VivenLuaBehaviour");
                
                if (luaBehaviour != null)
                {
                    // Lua 파일을 읽어서 checkInject 변수들 추출
                    var luaPath = "Assets/PingPong/Scripts/Manager/PingPongGameManager.lua";
                    var fullPath = Path.Combine(Application.dataPath, luaPath.Replace("Assets/", ""));
                    
                    if (File.Exists(fullPath))
                    {
                        var luaContent = File.ReadAllText(fullPath);
                        ValidateLuaInjections(gameManager, luaBehaviour, luaContent, "PingPongGameManager");
                    }
                }
            }

            // Racket의 Injection 검증
            var racket = GameObject.Find("Racket");
            if (racket != null)
            {
                var luaBehaviour = racket.GetComponents<Component>()
                    .FirstOrDefault(c => c.GetType().Name == "VivenLuaBehaviour");
                
                if (luaBehaviour != null)
                {
                    var luaPath = "Assets/PingPong/Scripts/Objects/PingPongRacket.lua";
                    var fullPath = Path.Combine(Application.dataPath, luaPath.Replace("Assets/", ""));
                    
                    if (File.Exists(fullPath))
                    {
                        var luaContent = File.ReadAllText(fullPath);
                        ValidateLuaInjections(racket, luaBehaviour, luaContent, "Racket");
                    }
                }
            }

            // BallLauncher의 Injection 검증
            var launcher = GameObject.Find("BallLauncher");
            if (launcher != null)
            {
                var luaBehaviour = launcher.GetComponents<Component>()
                    .FirstOrDefault(c => c.GetType().Name == "VivenLuaBehaviour");
                
                if (luaBehaviour != null)
                {
                    var luaPath = "Assets/PingPong/Scripts/Objects/BallLauncher.lua";
                    var fullPath = Path.Combine(Application.dataPath, luaPath.Replace("Assets/", ""));
                    
                    if (File.Exists(fullPath))
                    {
                        var luaContent = File.ReadAllText(fullPath);
                        ValidateLuaInjections(launcher, luaBehaviour, luaContent, "BallLauncher");
                    }
                }
            }
        }

        private void ValidateLuaInjections(GameObject go, Component luaBehaviour, string luaContent, string objectName)
        {
            // checkInject 패턴 찾기: VariableName = checkInject(VariableName)
            var checkInjectPattern = @"(\w+)\s*=\s*checkInject\(\1\)";
            var checkInjectMatches = Regex.Matches(luaContent, checkInjectPattern);
            
            var requiredInjections = checkInjectMatches.Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .ToList();

            // NullableInject 패턴 찾기: VariableName = NullableInject(VariableName)
            var nullablePattern = @"(\w+)\s*=\s*NullableInject\(\1\)";
            var nullableMatches = Regex.Matches(luaContent, nullablePattern);
            
            var optionalInjections = nullableMatches.Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .ToList();

            // 현재 injection 값들 가져오기
            var type = luaBehaviour.GetType();
            var injectionField = type.GetField("injection", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            var currentInjections = new HashSet<string>();
            if (injectionField != null)
            {
                var injection = injectionField.GetValue(luaBehaviour);
                if (injection != null)
                {
                    var injectionType = injection.GetType();
                    
                    // gameObjectValues
                    var goValuesField = injectionType.GetField("gameObjectValues", BindingFlags.Public | BindingFlags.Instance);
                    if (goValuesField != null)
                    {
                        var goValues = goValuesField.GetValue(injection) as System.Collections.IList;
                        if (goValues != null)
                        {
                            foreach (var item in goValues)
                            {
                                var nameField = item.GetType().GetField("name", BindingFlags.Public | BindingFlags.Instance);
                                if (nameField != null)
                                {
                                    currentInjections.Add(nameField.GetValue(item)?.ToString() ?? "");
                                }
                            }
                        }
                    }
                    
                    // stringValue
                    var stringValuesField = injectionType.GetField("stringValue", BindingFlags.Public | BindingFlags.Instance);
                    if (stringValuesField != null)
                    {
                        var stringValues = stringValuesField.GetValue(injection) as System.Collections.IList;
                        if (stringValues != null)
                        {
                            foreach (var item in stringValues)
                            {
                                var nameField = item.GetType().GetField("name", BindingFlags.Public | BindingFlags.Instance);
                                if (nameField != null)
                                {
                                    currentInjections.Add(nameField.GetValue(item)?.ToString() ?? "");
                                }
                            }
                        }
                    }
                }
            }

            // 필수 injection 누락 확인
            foreach (var required in requiredInjections)
            {
                if (!currentInjections.Contains(required))
                {
                    validationResults.Add(new ValidationResult
                    {
                        Type = ResultType.Error,
                        Category = $"{objectName} Injection",
                        Message = $"필수 Injection '{required}'이 누락되었습니다!",
                        Details = "checkInject()로 선언된 변수는 반드시 Inspector에서 연결해야 합니다.",
                        Target = go
                    });
                }
            }

            // 선택적 injection 누락 확인
            foreach (var optional in optionalInjections)
            {
                if (!currentInjections.Contains(optional))
                {
                    validationResults.Add(new ValidationResult
                    {
                        Type = ResultType.Warning,
                        Category = $"{objectName} Injection",
                        Message = $"선택적 Injection '{optional}'이 연결되지 않았습니다.",
                        Details = "NullableInject()로 선언된 변수입니다. 기능이 제한될 수 있습니다.",
                        Target = go
                    });
                }
            }

            // Lua에 없는 injection 확인 (불필요한 injection)
            var allLuaInjections = requiredInjections.Concat(optionalInjections).ToHashSet();
            foreach (var current in currentInjections)
            {
                if (!allLuaInjections.Contains(current))
                {
                    validationResults.Add(new ValidationResult
                    {
                        Type = ResultType.Warning,
                        Category = $"{objectName} Injection",
                        Message = $"불필요한 Injection '{current}'이 있습니다.",
                        Details = "Lua 스크립트에서 사용하지 않는 변수입니다. 제거해도 됩니다.",
                        Target = go
                    });
                }
            }
        }

        private void PrintResultsToConsole()
        {
            if (validationResults.Count == 0)
            {
                RunValidation();
            }

            var sb = new StringBuilder();
            sb.AppendLine("\n═══════════════════════════════════════════════════════════");
            sb.AppendLine("                 🏓 PingPong 씬 검증 결과");
            sb.AppendLine("═══════════════════════════════════════════════════════════\n");

            var errors = validationResults.Where(r => r.Type == ResultType.Error).ToList();
            var warnings = validationResults.Where(r => r.Type == ResultType.Warning).ToList();
            var passes = validationResults.Where(r => r.Type == ResultType.Pass).ToList();

            sb.AppendLine($"📊 요약: ✅ {passes.Count} 통과 | ⚠️ {warnings.Count} 경고 | ❌ {errors.Count} 오류\n");

            if (errors.Count > 0)
            {
                sb.AppendLine("❌ 오류 (반드시 수정 필요):");
                sb.AppendLine("───────────────────────────────────────────────────────────");
                foreach (var error in errors)
                {
                    sb.AppendLine($"  [{error.Category}] {error.Message}");
                    if (!string.IsNullOrEmpty(error.Details))
                        sb.AppendLine($"     └─ {error.Details}");
                }
                sb.AppendLine();
            }

            if (warnings.Count > 0)
            {
                sb.AppendLine("⚠️ 경고 (확인 권장):");
                sb.AppendLine("───────────────────────────────────────────────────────────");
                foreach (var warning in warnings)
                {
                    sb.AppendLine($"  [{warning.Category}] {warning.Message}");
                    if (!string.IsNullOrEmpty(warning.Details))
                        sb.AppendLine($"     └─ {warning.Details}");
                }
                sb.AppendLine();
            }

            if (passes.Count > 0)
            {
                sb.AppendLine("✅ 통과:");
                sb.AppendLine("───────────────────────────────────────────────────────────");
                foreach (var pass in passes)
                {
                    sb.AppendLine($"  [{pass.Category}] {pass.Message}");
                }
                sb.AppendLine();
            }

            sb.AppendLine("═══════════════════════════════════════════════════════════\n");

            if (errors.Count > 0)
                Debug.LogError(sb.ToString());
            else if (warnings.Count > 0)
                Debug.LogWarning(sb.ToString());
            else
                Debug.Log(sb.ToString());
        }

        private void TryAutoFix()
        {
            var fixedCount = 0;
            var messages = new List<string>();

            // 1. Racket의 불필요한 gameManagerName 제거
            fixedCount += TryRemoveUnnecessaryInjection("Racket", "gameManagerName", messages);
            fixedCount += TryRemoveUnnecessaryInjection("Racket (1)", "gameManagerName", messages);

            // 2. 중복 Racket 오브젝트 삭제 제안
            var racket1 = GameObject.Find("Racket (1)");
            if (racket1 != null)
            {
                if (EditorUtility.DisplayDialog("중복 오브젝트 발견",
                    "'Racket (1)' 오브젝트가 발견되었습니다.\n이것은 중복된 탁구채입니다. 삭제하시겠습니까?",
                    "삭제", "유지"))
                {
                    Undo.DestroyObjectImmediate(racket1);
                    messages.Add("✅ 'Racket (1)' 중복 오브젝트 삭제됨");
                    fixedCount++;
                }
            }

            // 결과 표시
            if (fixedCount > 0)
            {
                var resultMsg = $"자동 수정 완료!\n\n수정된 항목 ({fixedCount}개):\n" + string.Join("\n", messages);
                EditorUtility.DisplayDialog("자동 수정 완료", resultMsg, "확인");

                // 씬 변경 표시
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                    UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

                // 검증 다시 실행
                RunValidation();

                Debug.Log($"[PingPong 검증기] 자동 수정 완료: {fixedCount}개 항목 수정됨\n" + string.Join("\n", messages));
            }
            else
            {
                EditorUtility.DisplayDialog("자동 수정", "수정할 항목이 없습니다.\n\n모든 설정이 올바릅니다!", "확인");
            }
        }

        private int TryRemoveUnnecessaryInjection(string objectName, string injectionName, List<string> messages)
        {
            // 정적 메서드 재사용
            return TryRemoveUnnecessaryInjectionStatic(objectName, injectionName, messages);
        }

        [MenuItem("Viven/PingPong/🔧 자동 수정 (Auto Fix)", false, 102)]
        public static void QuickAutoFix()
        {
            var fixedCount = 0;
            var messages = new List<string>();

            // 1. Racket의 불필요한 gameManagerName 제거
            fixedCount += TryRemoveUnnecessaryInjectionStatic("Racket", "gameManagerName", messages);
            fixedCount += TryRemoveUnnecessaryInjectionStatic("Racket (1)", "gameManagerName", messages);

            // 2. 중복 Racket 오브젝트 삭제 제안
            var racket1 = GameObject.Find("Racket (1)");
            if (racket1 != null)
            {
                if (EditorUtility.DisplayDialog("중복 오브젝트 발견",
                    "'Racket (1)' 오브젝트가 발견되었습니다.\n이것은 중복된 탁구채입니다. 삭제하시겠습니까?",
                    "삭제", "유지"))
                {
                    Undo.DestroyObjectImmediate(racket1);
                    messages.Add("✅ 'Racket (1)' 중복 오브젝트 삭제됨");
                    fixedCount++;
                }
            }

            // 결과 표시
            if (fixedCount > 0)
            {
                var resultMsg = $"자동 수정 완료!\n\n수정된 항목 ({fixedCount}개):\n" + string.Join("\n", messages);
                EditorUtility.DisplayDialog("자동 수정 완료", resultMsg, "확인");

                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                    UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

                Debug.Log($"[PingPong 검증기] 자동 수정 완료: {fixedCount}개 항목 수정됨\n" + string.Join("\n", messages));

                // 검증 다시 실행
                QuickValidate();
            }
            else
            {
                EditorUtility.DisplayDialog("자동 수정", "수정할 항목이 없습니다.\n\n모든 설정이 올바릅니다!", "확인");
            }
        }

        [MenuItem("Viven/PingPong/UI Structure Auto Setup", false, 103)]
        public static void SetupUIStructure()
        {
            var messages = new List<string>();
            var setupCount = 0;

            // StartCanvas 찾기
            var startCanvas = GameObject.Find("StartCanvas");
            if (startCanvas == null)
            {
                EditorUtility.DisplayDialog("오류", "StartCanvas를 찾을 수 없습니다!", "확인");
                return;
            }

            // 1. StartUIPanel 생성 (이미 없으면)
            var startUIPanel = startCanvas.transform.Find("StartUIPanel");
            if (startUIPanel == null)
            {
                var panelGO = new GameObject("StartUIPanel");
                panelGO.transform.SetParent(startCanvas.transform, false);

                // RectTransform 추가 및 설정
                var rectTransform = panelGO.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;

                startUIPanel = panelGO.transform;
                messages.Add("✅ StartUIPanel 생성됨");
                setupCount++;

                Undo.RegisterCreatedObjectUndo(panelGO, "Create StartUIPanel");
            }

            // 2. Dropdown을 StartUIPanel 아래로 이동
            var dropdown = startCanvas.transform.Find("Dropdown");
            if (dropdown != null && dropdown.parent != startUIPanel)
            {
                Undo.SetTransformParent(dropdown, startUIPanel, "Move Dropdown to StartUIPanel");
                messages.Add("✅ Dropdown → StartUIPanel 아래로 이동됨");
                setupCount++;
            }

            // 3. Button을 StartUIPanel 아래로 이동
            var button = startCanvas.transform.Find("Button");
            if (button != null && button.parent != startUIPanel)
            {
                Undo.SetTransformParent(button, startUIPanel, "Move Button to StartUIPanel");
                messages.Add("✅ Button → StartUIPanel 아래로 이동됨");
                setupCount++;
            }

            // 4. GameUIPanel 생성 또는 Container 사용
            var gameUIPanel = startCanvas.transform.Find("GameUIPanel");
            var container = startCanvas.transform.Find("Container");

            if (gameUIPanel == null)
            {
                if (container != null)
                {
                    // Container 이름 변경
                    Undo.RecordObject(container.gameObject, "Rename Container to GameUIPanel");
                    container.gameObject.name = "GameUIPanel";
                    gameUIPanel = container;
                    messages.Add("✅ Container → GameUIPanel로 이름 변경됨");
                    setupCount++;
                }
                else
                {
                    // 새 GameUIPanel 생성
                    var panelGO = new GameObject("GameUIPanel");
                    panelGO.transform.SetParent(startCanvas.transform, false);

                    var rectTransform = panelGO.AddComponent<RectTransform>();
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;

                    gameUIPanel = panelGO.transform;
                    messages.Add("✅ GameUIPanel 생성됨");
                    setupCount++;

                    Undo.RegisterCreatedObjectUndo(panelGO, "Create GameUIPanel");
                }
            }

            // 5. PingPongGameManager Injection 연결
            var gameManager = GameObject.Find("PingPongGameManager");
            if (gameManager != null)
            {
                var injectionSetupCount = SetupGameManagerInjections(gameManager, startCanvas.transform, messages);
                setupCount += injectionSetupCount;
            }

            // 결과 표시
            if (setupCount > 0)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                    UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

                var resultMsg = $"UI 구조 설정 완료!\n\n설정된 항목 ({setupCount}개):\n" + string.Join("\n", messages);
                EditorUtility.DisplayDialog("UI 구조 설정 완료", resultMsg, "확인");
                Debug.Log($"[PingPong] UI 구조 설정 완료: {setupCount}개 항목\n" + string.Join("\n", messages));

                // 검증 다시 실행
                QuickValidate();
            }
            else
            {
                EditorUtility.DisplayDialog("UI 구조 설정", "이미 모든 UI 구조가 올바르게 설정되어 있습니다!", "확인");
            }
        }

        private static int SetupGameManagerInjections(GameObject gameManager, Transform canvasTransform, List<string> messages)
        {
            var luaBehaviour = gameManager.GetComponents<Component>()
                .FirstOrDefault(c => c.GetType().Name == "VivenLuaBehaviour");

            if (luaBehaviour == null) return 0;

            var serializedObject = new SerializedObject(luaBehaviour);
            var injectionProp = serializedObject.FindProperty("injection");
            if (injectionProp == null) return 0;

            var goValuesProp = injectionProp.FindPropertyRelative("gameObjectValues");
            if (goValuesProp == null || !goValuesProp.isArray) return 0;

            int setupCount = 0;

            // UI 오브젝트 매핑
            var uiMappings = new Dictionary<string, string>
            {
                { "StartUIPanel", "StartUIPanel" },
                { "GameUIPanel", "GameUIPanel" },
                { "DifficultyDropdownObject", "StartUIPanel/Dropdown" },
                { "StartButtonObject", "StartUIPanel/Button" },
                { "ScoreTextObject", "GameUIPanel/Score Text" }
            };

            foreach (var mapping in uiMappings)
            {
                var injectionName = mapping.Key;
                var objectPath = mapping.Value;

                // 현재 injection에서 해당 이름 찾기
                int existingIndex = -1;
                bool isConnected = false;

                for (int i = 0; i < goValuesProp.arraySize; i++)
                {
                    var element = goValuesProp.GetArrayElementAtIndex(i);
                    var nameProp = element.FindPropertyRelative("name");
                    if (nameProp != null && nameProp.stringValue == injectionName)
                    {
                        existingIndex = i;
                        var valueProp = element.FindPropertyRelative("value");
                        isConnected = valueProp != null && valueProp.objectReferenceValue != null;
                        break;
                    }
                }

                // 오브젝트 찾기
                Transform targetTransform = canvasTransform.Find(objectPath);
                if (targetTransform == null)
                {
                    // 직접 자식에서 찾기
                    var parts = objectPath.Split('/');
                    targetTransform = canvasTransform.Find(parts[parts.Length - 1]);
                }

                if (targetTransform == null) continue;

                // injection이 없거나 연결이 안 되어 있으면 연결
                if (existingIndex == -1)
                {
                    // 새 injection 항목 추가
                    goValuesProp.InsertArrayElementAtIndex(goValuesProp.arraySize);
                    var newElement = goValuesProp.GetArrayElementAtIndex(goValuesProp.arraySize - 1);
                    var newNameProp = newElement.FindPropertyRelative("name");
                    var newValueProp = newElement.FindPropertyRelative("value");

                    if (newNameProp != null) newNameProp.stringValue = injectionName;
                    if (newValueProp != null) newValueProp.objectReferenceValue = targetTransform.gameObject;

                    messages.Add($"✅ '{injectionName}' Injection 추가 → {targetTransform.name}");
                    setupCount++;
                }
                else if (!isConnected)
                {
                    // 기존 injection 연결
                    var element = goValuesProp.GetArrayElementAtIndex(existingIndex);
                    var valueProp = element.FindPropertyRelative("value");
                    if (valueProp != null)
                    {
                        valueProp.objectReferenceValue = targetTransform.gameObject;
                        messages.Add($"✅ '{injectionName}' Injection 연결 → {targetTransform.name}");
                        setupCount++;
                    }
                }
            }

            if (setupCount > 0)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(luaBehaviour);
            }

            return setupCount;
        }

        [MenuItem("Viven/PingPong/🔧 자동 수정 (레거시)", false, 199)]
        public static void QuickAutoFixLegacy()
        {
            var fixedCount = 0;
            var messages = new List<string>();

            // 1. Racket의 불필요한 gameManagerName 제거
            fixedCount += TryRemoveUnnecessaryInjectionStatic("Racket", "gameManagerName", messages);
            fixedCount += TryRemoveUnnecessaryInjectionStatic("Racket (1)", "gameManagerName", messages);

            // 2. 중복 Racket 오브젝트 삭제 제안
            var racket1 = GameObject.Find("Racket (1)");
            if (racket1 != null)
            {
                if (EditorUtility.DisplayDialog("중복 오브젝트 발견",
                    "'Racket (1)' 오브젝트가 발견되었습니다.\n이것은 중복된 탁구채입니다. 삭제하시겠습니까?",
                    "삭제", "유지"))
                {
                    Undo.DestroyObjectImmediate(racket1);
                    messages.Add("✅ 'Racket (1)' 중복 오브젝트 삭제됨");
                    fixedCount++;
                }
            }

            // 결과 표시
            if (fixedCount > 0)
            {
                var resultMsg = $"자동 수정 완료!\n\n수정된 항목 ({fixedCount}개):\n" + string.Join("\n", messages);
                EditorUtility.DisplayDialog("자동 수정 완료", resultMsg, "확인");

                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                    UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

                Debug.Log($"[PingPong 검증기] 자동 수정 완료: {fixedCount}개 항목 수정됨\n" + string.Join("\n", messages));

                // 검증 다시 실행
                QuickValidate();
            }
            else
            {
                EditorUtility.DisplayDialog("자동 수정", "수정할 항목이 없습니다.\n\n모든 설정이 올바릅니다!", "확인");
            }
        }

        private static int TryRemoveUnnecessaryInjectionStatic(string objectName, string injectionName, List<string> messages)
        {
            var go = GameObject.Find(objectName);
            if (go == null) return 0;

            var luaBehaviour = go.GetComponents<Component>()
                .FirstOrDefault(c => c.GetType().Name == "VivenLuaBehaviour");

            if (luaBehaviour == null) return 0;

            // SerializedObject를 먼저 생성하여 직접 수정해야 함
            var serializedObject = new SerializedObject(luaBehaviour);
            var injectionProp = serializedObject.FindProperty("injection");

            if (injectionProp == null)
            {
                Debug.LogWarning($"[Auto Fix] '{objectName}'에서 injection 프로퍼티를 찾을 수 없습니다.");
                return 0;
            }

            // stringValue 배열 프로퍼티 찾기
            var stringValueProp = injectionProp.FindPropertyRelative("stringValue");
            if (stringValueProp == null || !stringValueProp.isArray)
            {
                Debug.LogWarning($"[Auto Fix] '{objectName}'에서 stringValue 배열을 찾을 수 없습니다.");
                return 0;
            }

            // 제거할 인덱스 찾기
            int indexToRemove = -1;
            for (int i = 0; i < stringValueProp.arraySize; i++)
            {
                var element = stringValueProp.GetArrayElementAtIndex(i);
                var nameProp = element.FindPropertyRelative("name");
                if (nameProp != null && nameProp.stringValue == injectionName)
                {
                    indexToRemove = i;
                    break;
                }
            }

            if (indexToRemove >= 0)
            {
                // Undo 등록
                Undo.RecordObject(luaBehaviour, $"Remove {injectionName} from {objectName}");

                // SerializedProperty의 DeleteArrayElementAtIndex 사용
                stringValueProp.DeleteArrayElementAtIndex(indexToRemove);

                // 변경사항 적용
                serializedObject.ApplyModifiedProperties();

                // Dirty 마킹
                EditorUtility.SetDirty(luaBehaviour);

                messages.Add($"✅ '{objectName}'에서 불필요한 '{injectionName}' Injection 제거됨");
                Debug.Log($"[Auto Fix] '{objectName}'에서 '{injectionName}' Injection 제거 완료");
                return 1;
            }

            return 0;
        }

        private enum ResultType
        {
            Pass,
            Warning,
            Error
        }

        private class ValidationResult
        {
            public ResultType Type { get; set; }
            public string Category { get; set; }
            public string Message { get; set; }
            public string Details { get; set; }
            public GameObject Target { get; set; }
        }

        #region Dropdown Style Setup

        [MenuItem("Viven/PingPong/Dropdown Style Setup")]
        public static void SetupDropdownStyle()
        {
            Debug.Log("[PingPong] Dropdown 스타일 설정 시작...");

            // Dropdown 찾기
            var dropdown = GameObject.Find("Dropdown");
            if (dropdown == null)
            {
                // Canvas 하위에서 찾기
                var canvas = GameObject.Find("Canvas");
                if (canvas != null)
                {
                    dropdown = FindInactiveChild(canvas.transform, "Dropdown")?.gameObject;
                }
            }

            if (dropdown == null)
            {
                Debug.LogError("[PingPong] Dropdown을 찾을 수 없습니다.");
                return;
            }

            // Template 찾기 (비활성 상태일 수 있음)
            Transform template = FindInactiveChild(dropdown.transform, "Template");
            if (template == null)
            {
                Debug.LogError("[PingPong] Dropdown의 Template을 찾을 수 없습니다.");
                return;
            }

            // 스프라이트 로드
            string basePath = "Assets/JetXR/UI Kit For Vision Pro OS/Runtime/Sprites";
            var tooltipSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/Tooltip/Tooltip.png");
            var highlightSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/Dropdown/Highlight.png");
            var checkmarkSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/Dropdown/ItemCheckmark.png");
            var scrollbarHandleSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/Dropdown/ScrollbarHandle.png");

            int changedCount = 0;

            // 1. Template 배경 스타일링
            var templateImage = template.GetComponent<UnityEngine.UI.Image>();
            if (templateImage != null && tooltipSprite != null)
            {
                Undo.RecordObject(templateImage, "Style Template Background");
                templateImage.sprite = tooltipSprite;
                templateImage.type = UnityEngine.UI.Image.Type.Sliced;
                templateImage.color = new Color(0.15f, 0.15f, 0.15f, 0.95f); // 어두운 반투명
                EditorUtility.SetDirty(templateImage);
                changedCount++;
                Debug.Log("[PingPong] Template 배경 스타일 적용");
            }

            // 2. Viewport 찾기
            Transform viewport = FindInactiveChild(template, "Viewport");

            // 3. Content 찾기
            Transform content = viewport != null ? FindInactiveChild(viewport, "Content") : FindInactiveChild(template, "Content");

            // 4. Item 찾기
            Transform item = content != null ? FindInactiveChild(content, "Item") : null;

            if (item != null)
            {
                // Item Background 스타일링
                Transform itemBackground = FindInactiveChild(item, "Item Background");
                if (itemBackground != null)
                {
                    var bgImage = itemBackground.GetComponent<UnityEngine.UI.Image>();
                    if (bgImage != null && highlightSprite != null)
                    {
                        Undo.RecordObject(bgImage, "Style Item Background");
                        bgImage.sprite = highlightSprite;
                        bgImage.type = UnityEngine.UI.Image.Type.Sliced;
                        bgImage.color = new Color(1f, 1f, 1f, 0.1f); // 연한 하이라이트
                        EditorUtility.SetDirty(bgImage);
                        changedCount++;
                        Debug.Log("[PingPong] Item Background 스타일 적용");
                    }
                }

                // Item Checkmark 스타일링
                Transform itemCheckmark = FindInactiveChild(item, "Item Checkmark");
                if (itemCheckmark != null)
                {
                    var checkImage = itemCheckmark.GetComponent<UnityEngine.UI.Image>();
                    if (checkImage != null && checkmarkSprite != null)
                    {
                        Undo.RecordObject(checkImage, "Style Item Checkmark");
                        checkImage.sprite = checkmarkSprite;
                        checkImage.color = Color.white;
                        EditorUtility.SetDirty(checkImage);
                        changedCount++;
                        Debug.Log("[PingPong] Item Checkmark 스타일 적용");
                    }
                }

                // Item Label 스타일링
                Transform itemLabel = FindInactiveChild(item, "Item Label");
                if (itemLabel != null)
                {
                    var tmpText = itemLabel.GetComponent<TMPro.TMP_Text>();
                    if (tmpText != null)
                    {
                        Undo.RecordObject(tmpText, "Style Item Label");
                        tmpText.color = new Color(0.9f, 0.9f, 0.9f, 1f); // 밝은 회색 텍스트
                        tmpText.fontSize = 14;
                        EditorUtility.SetDirty(tmpText);
                        changedCount++;
                        Debug.Log("[PingPong] Item Label 스타일 적용");
                    }
                }
            }

            // 5. Scrollbar 스타일링
            Transform scrollbar = FindInactiveChild(template, "Scrollbar");
            if (scrollbar != null)
            {
                // Scrollbar 배경
                var scrollbarImage = scrollbar.GetComponent<UnityEngine.UI.Image>();
                if (scrollbarImage != null)
                {
                    Undo.RecordObject(scrollbarImage, "Style Scrollbar Background");
                    scrollbarImage.color = new Color(0.1f, 0.1f, 0.1f, 0.5f); // 어두운 배경
                    EditorUtility.SetDirty(scrollbarImage);
                    changedCount++;
                }

                // Sliding Area > Handle
                Transform slidingArea = FindInactiveChild(scrollbar, "Sliding Area");
                Transform handle = slidingArea != null ? FindInactiveChild(slidingArea, "Handle") : FindInactiveChild(scrollbar, "Handle");

                if (handle != null)
                {
                    var handleImage = handle.GetComponent<UnityEngine.UI.Image>();
                    if (handleImage != null && scrollbarHandleSprite != null)
                    {
                        Undo.RecordObject(handleImage, "Style Scrollbar Handle");
                        handleImage.sprite = scrollbarHandleSprite;
                        handleImage.color = new Color(0.6f, 0.6f, 0.6f, 0.8f); // 밝은 회색 핸들
                        EditorUtility.SetDirty(handleImage);
                        changedCount++;
                        Debug.Log("[PingPong] Scrollbar Handle 스타일 적용");
                    }
                }
            }

            Debug.Log($"[PingPong] Dropdown 스타일 설정 완료: {changedCount}개 항목 변경됨");

            if (changedCount > 0)
            {
                // 씬 변경 표시
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                    UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            }
        }

        /// <summary>
        /// 비활성 오브젝트를 포함하여 자식에서 이름으로 찾기
        /// </summary>
        private static Transform FindInactiveChild(Transform parent, string childName)
        {
            if (parent == null) return null;

            foreach (Transform child in parent)
            {
                if (child.name == childName)
                    return child;
            }

            // 재귀적으로 찾기
            foreach (Transform child in parent)
            {
                var found = FindInactiveChild(child, childName);
                if (found != null)
                    return found;
            }

            return null;
        }

        #endregion
    }
}
