using System.Linq;
using UnityEditor;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build.Platform
{
    public static class PlatformBuildConfigurator
    {
    
        /// <summary>
        /// 현재 프로젝트 내에 OpenXR 패키지가 설치되어 있는지 확인합니다.
        /// 추가로 OpenXR 옵션이 켜져있는지 확인합니다.
        /// Single Pass Instancing 옵션이 켜져있는지 확인합니다.
        /// </summary>
        public static bool ValidateOpenXRPackage()
        {
            // 현재 프로젝트 내에 OpenXR 패키지가 설치되어 있는지 확인
            var packageListRequest = UnityEditor.PackageManager.Client.List(true);
            // 패키지 리스트 요청이 완료될 때까지 대기합니다.
            while (!packageListRequest.IsCompleted) { } 
            
            // OpenXR 패키지 존재 여부를 확인하기 위한 변수
            bool hasOpenXR = false;
            
            // 모든 설치된 패키지를 순회하며 OpenXR 패키지가 있는지 확인합니다.
            foreach (var package in packageListRequest.Result)
            {
                // OpenXR 패키지의 이름과 일치하는지 확인합니다.
                if (package.name == "com.unity.xr.openxr")
                {
                    // OpenXR 패키지가 발견되면 true로 설정하고 루프를 종료합니다.
                    hasOpenXR = true;
                    break;
                }
            }
            
            // StereoRenderingPath 설정을 Single Pass로 변경합니다.
            if (hasOpenXR)
            {
                OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Standalone).renderMode =
                    OpenXRSettings.RenderMode.SinglePassInstanced;
            }

            // OpenXR 패키지 존재 여부를 반환합니다.
            return hasOpenXR;
        }

    }
}