#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build.Platform
{
    /// <summary>
    /// Unity 빌드 파이프라인에서 셰이더 전처리를 담당하는 클래스입니다.
    /// Windows 플랫폼이 아닌 경우 OpenXR 관련 셰이더 변형을 제거합니다.
    /// </summary>
    public class OpenXRShaderStripper : IPreprocessShaders
    {
        // 콜백 순서 - 낮은 값이 먼저 실행됩니다
        public int callbackOrder => 0;

        /// <summary>
        /// 셰이더 처리 과정에서 자동으로 호출되는 메서드입니다.
        /// Unity 빌드 프로세스 중에 각 셰이더와 그 변형에 대해 실행됩니다.
        /// </summary>
        /// <param name="shader">처리할 셰이더</param>
        /// <param name="snippet">셰이더 스니펫 데이터</param>
        /// <param name="data">셰이더 컴파일러 데이터 목록</param>
        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
        {
            // 현재 빌드 플랫폼 확인
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;

            // Windows 플랫폼이 아닌 경우 OpenXR 관련 셰이더 변형 제거
            if (buildTarget != BuildTarget.StandaloneWindows && buildTarget != BuildTarget.StandaloneWindows64)
            {
                // 리스트를 뒤에서부터 순회하여 안전하게 항목 제거
                for (int i = data.Count - 1; i >= 0; i--)
                {
                    ShaderCompilerData variant = data[i];
                    // 각 셰이더 변형의 키워드 확인
                    foreach (var keyword in variant.shaderKeywordSet.GetShaderKeywords())
                    {
                       // OPENXR 키워드가 포함된 셰이더 변형 제거
                        if (keyword.name.Contains("STEREO_INSTANCING_ON")       ||
                            keyword.name.Contains("STEREO_MULTIVIEW_ON")        ||
                            keyword.name.Contains("STEREO_CUBEMAP_RENDER_ON")   ||
                            keyword.name.Contains("UNITY_SINGLE_PASS_STEREO"))
                        {
                            data.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }
    }
}
#endif