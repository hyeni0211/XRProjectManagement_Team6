using System.Collections.Generic;
using System.Linq;
// using Newtonsoft.Json;
using TwentyOz.VivenSDK.Scripts.Editor.Build.VAvatar;
using TwentyOz.VivenSDK.Scripts.Editor.Build.VMap;
using TwentyOz.VivenSDK.Scripts.Editor.Core;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build
{
    /// <summary>
    /// Viven 빌드 데이터를 JSON 형식으로 직렬화하는 유틸리티 클래스입니다.
    /// VMap, VObject, VAvatar의 빌드 데이터를 JSON 형식으로 변환하는 기능을 제공합니다.
    /// </summary>
    public static class VivenBuildDataWriter
    {
        /// <summary>
        /// 맵 빌드 데이터를 JSON 형식으로 변환합니다.
        /// 빌드 시간, 생성자 정보, 맵 이름, 지원 플랫폼 등의 정보를 포함합니다.
        /// </summary>
        /// <param name="buildData">변환할 맵 빌드 데이터</param>
        /// <returns>JSON 형식으로 직렬화된 맵 빌드 데이터</returns>
        public static string WriteMapBuildData(VivenMapBuildData buildData)
        {
            var availablePlatforms = VivenPlatformExtension.Platforms
                .Where(platform => buildData.GetPlatformSceneWrapper(platform).enabled)
                .Select(platform => platform.GetPlatformName())
                .ToArray();

            var dataObject = new Dictionary<string, object>
            {
                { "dateAndTime", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz") },
                { "creator_mbrId", VivenLauncher.GetUserInfo().mbrId },
                { "creator_nickname", VivenLauncher.GetUserInfo().nickname },
                { "buildType", buildData.BuildType.ToString() },
                { "mapName", buildData.GetBuildName() },
                { "availablePlatforms", availablePlatforms },
                { "SDKVersion", "0.0.0" } // TODO: SDK 버전 추가
            };

            return "";//JsonConvert.SerializeObject(dataObject);
        }

        /// <summary>
        /// 오브젝트 빌드 데이터를 JSON 형식으로 변환합니다.
        /// 빌드 시간, 생성자 정보, 오브젝트 이름, 지원 플랫폼 등의 정보를 포함합니다.
        /// </summary>
        /// <param name="objectBuildData">변환할 오브젝트 빌드 데이터</param>
        /// <returns>JSON 형식으로 직렬화된 오브젝트 빌드 데이터</returns>
        public static string WriteVObjectBuildData(VivenObjectBuildData objectBuildData)
        {
            var availablePlatforms = VivenPlatformExtension.Platforms
                .Where(platform => objectBuildData.GetPlatformSceneWrapper(platform).enabled)
                .Select(platform => platform.GetPlatformName())
                .ToArray();

            var dataObject = new Dictionary<string, object>
            {
                { "dateAndTime", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz") },
                { "creator_mbrId", VivenLauncher.GetUserInfo().mbrId },
                { "creator_nickname", VivenLauncher.GetUserInfo().nickname },
                { "buildType", objectBuildData.BuildType.ToString() },
                { "objectName", objectBuildData.GetBuildName() },
                { "availablePlatforms", availablePlatforms },
                { "SDKVersion", "0.0.0" } // TODO: SDK 버전 추가
            };

            return "";//JsonConvert.SerializeObject(dataObject);
        }

        /// <summary>
        /// 아바타 빌드 데이터를 JSON 형식으로 변환합니다.
        /// 빌드 시간, 생성자 정보, 아바타 이름, 지원 플랫폼 등의 정보를 포함합니다.
        /// </summary>
        /// <param name="avatarBuildData">변환할 아바타 빌드 데이터</param>
        /// <returns>JSON 형식으로 직렬화된 아바타 빌드 데이터</returns>
        public static string WriteVAvatarBuildData(VivenAvatarBuildData avatarBuildData)
        {
            var availablePlatforms = VivenPlatformExtension.Platforms
                .Where(platform => avatarBuildData.GetPlatformSceneWrapper(platform).enabled)
                .Select(platform => platform.GetPlatformName())
                .ToArray();

            var dataObject = new Dictionary<string, object>
            {
                { "dateAndTime", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz") },
                { "creator_mbrId", VivenLauncher.GetUserInfo().mbrId },
                { "creator_nickname", VivenLauncher.GetUserInfo().nickname },
                { "buildType", avatarBuildData.BuildType.ToString() },
                { "avatarName", avatarBuildData.GetBuildName() },
                { "availablePlatforms", availablePlatforms },
                { "SDKVersion", "0.0.0" } // TODO: SDK 버전 추가
            };

            return "";//JsonConvert.SerializeObject(dataObject);
        }

        /// <summary>
        /// Dictionary를 JsonUtility로 직렬화하기 위한 도우미 클래스입니다.
        /// 제네릭 타입 T를 사용하여 다양한 타입의 Dictionary를 직렬화할 수 있습니다.
        /// </summary>
        /// <typeparam name="T">직렬화할 Dictionary의 값 타입</typeparam>
        [System.Serializable]
        private class Serializable<T>
        {
            /// <summary>
            /// 직렬화할 Dictionary 데이터
            /// </summary>
            public Dictionary<string, T> values;

            /// <summary>
            /// Serializable 클래스의 생성자
            /// </summary>
            /// <param name="values">직렬화할 Dictionary 데이터</param>
            public Serializable(Dictionary<string, T> values)
            {
                this.values = values;
            }
        }
    }
}