using System;
using System.Collections.Generic;
using TwentyOz.VivenSDK.Scripts.Core.Common;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.Manifest
{
    [Serializable]
    public struct VivenSDKManifestData
    {
        private const string CttDataType = "Addressable"; //해당 Map의 DataType. SDK에서는 언제나 Addressable임.
        
        public string ToJson(VivenContentProperty[] cttProperties)
        {
            var json = "{";
            json += $"\"cttDataType\":\"{CttDataType}\"";
            // json += $"\"binval\":\"{binval}\",";
            // json += $"\"cttId\":\"{cttId}\"";

            if (!CheckCttPropertyValid(cttProperties)) throw new Exception("CttProperty의 Key값이 중복됩니다.");
            
            // CttProperties 추가
            if (cttProperties.Length > 0)
            {
                json += ",";
                json += "\"cttProps\":{";
                for (int i = 0; i < cttProperties.Length; i++)
                {
                    json += $"\"{cttProperties[i].propertyName}\":\"{cttProperties[i].propertyValue}\"";
                    if (i != cttProperties.Length - 1)
                    {
                        json += ",";
                    }
                }
                json += "}";
            }
            json += "}";
            return json;
        }
        
        /// <summary>
        /// mapProperty중 값은 key값이 중복되지 않도록 체크합니다.
        /// </summary>
        /// <returns></returns>
        private bool CheckCttPropertyValid(VivenContentProperty[] cttProperties)
        {
            var contentPropertyKeys = new List<string>();
            foreach (var contentProperty in cttProperties)
            {
                if (contentPropertyKeys.Contains(contentProperty.propertyName))
                {
                    return false;
                }
                contentPropertyKeys.Add(contentProperty.propertyName);
            }
            return true;
        }
    }
}