using System;
using System.Collections.Generic;
using System.Linq;
using TwentyOz.VivenSDK.Scripts.Core.Common;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build.VMap
{
    /// <summary>
    /// VMap 빌드 시 사용할 데이터의 모음입니다.
    /// VMap은 플랫폼 별로 다른 맵을 빌드할 수 있으며 해당 맵들은 플랫폼에 따라 동적으로 로드됩니다.
    /// </summary>
    /// <remarks>
    /// CttId와 CttBinVal은 플랫폼에 상관없이 동일한 값을 사용합니다.
    /// 해당 값들은 Viven WebPage에서 맵을 생성할 때 사용됩니다.
    /// </remarks>
    [CreateAssetMenu(menuName = "VivenSDK/Build/Create VivenMapBuildData", fileName = "VivenMapBuildData", order = 0)]
    public class VivenMapBuildData : VivenBuildData
    {
        public override VivenBuildType BuildType => VivenBuildType.vmap;

        /// <summary>
        /// 빌드할 맵의 이름
        /// </summary>
        [SerializeField] public string mapName;

        //추가적인 Pack에서 빌드시 사용할 프로퍼티, Manifest에 추가됩니다.
        //ex) mapType을 추가할 때 사용합니다.
        /// <summary>
        /// 맵에서 공통으로 사용할 프로퍼티입니다.
        /// Manifest에 Dictionary 형태로 추가됩니다.
        /// </summary>
        /// <remarks>
        /// 플랫폼 별로 다른 프로퍼티를 설정하고 싶다면,
        /// 플랫폼 별로 별도의 맵을 생성한 후 <see cref="VivenMapEnvironment"/>의 mapProperties에 추가하십시오.
        /// </remarks>
        [SerializeField] public List<VivenContentProperty> vivenAdditionalProperty = new();

        [SerializeField] public List<VivenMapObject> mapObjects = new();

        private VivenContentProperty[] _currentMapProperties = Array.Empty<VivenContentProperty>();

        public override string GetBuildName()
        {
            return mapName;
        }

        /// <summary>
        /// 빌드할 MapEnvironment의 프로퍼티를 복사합니다.
        /// </summary>
        /// <param name="mapEnvironment"></param>
        public void SetMapProperties(VivenMapEnvironment mapEnvironment)
        {
            SetMapProperties(mapEnvironment.mapProperties);
        }
        
        /// <summary>
        /// 빌드할 Scene의 프로퍼티를 설정합니다.
        /// </summary>
        /// <param name="mapProperties"></param>
        public void SetMapProperties(VivenContentProperty[] mapProperties)
        {
            // 맵의 프로퍼티를 설정합니다.
            Array.Copy(mapProperties, _currentMapProperties, mapProperties.Length);
        }

        public override VivenContentProperty[] GetContentProperties()
        {
            return _currentMapProperties.Concat(vivenAdditionalProperty).ToArray();
        }

        /// <summary>
        /// MapObject를 VMap과 함께 빌드함
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public override BuildResultData BuildAdditionalObjects(AddressableAssetSettings settings)
        {
            return VivenBuildManager.BuildMapObjects(this, settings);
        }
    }
}