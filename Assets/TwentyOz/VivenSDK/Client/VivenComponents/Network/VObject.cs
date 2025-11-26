using System;
using TwentyOz.VivenSDK.Scripts.Core.VivenComponents.VivenFields;
using UnityEditor;
using UnityEngine;

namespace Twoz.Viven.Interactions
{
    /// <summary>
    /// Object를 동기화를 하거나, 상호작용이 가능할 수 있도록 사용하는 Component입니다.
    /// </summary>
    /// <remarks>
    /// Viven에서 오브젝트와 상호작용하기 위해선 오브젝트가 네트워크에 동기화되어있어야 합니다. TwozVObject 컴포넌트는 오브젝트를 서버와 동기화하는 역할을 수행합니다.
    ///
    /// <para>
    /// 서로 다른 클라이언트에 존재하는 VObject들은 동일한 Network ID를 가지고 있습니다. 이를 기반으로 서버는 클라이언트에게 동기화된 데이터를 전송합니다.
    /// RPC를 통해 함수를 실행하거나, interactor가 상호작용을 수행하는 등 플레이어가 상호작용을 수행하는 등 서버를 통해 모든 동기화 작업은 TwozVObject를 통해 이루어집니다.
    /// </para>
    ///
    /// <para> VObject가 서버에 연결되지 못하면 상호작용을 수행할 수 없습니다. 중복된 Network ID가 존재하는 경우에도 서버와 연결이 끊어집니다.
    /// Network Id를 찾을 수 없거나 Network Id가 중복됐다는 에러가 발생하면, VObject가 잘못 빌드되었거나, VivenBehaviour에서 오브젝트를 생성한 것일 수 있습니다.
    /// 동적으로 VObject를 생성하는 기능은 현재 제공되고 있지 않습니다.  
    /// </para>
    ///
    /// <para>
    /// 맵에 오브젝트를 포함해 빌드하는 경우, <see cref="contentType"/>을 Prepared로 설정해야 합니다.
    /// Prepared로 설정된 오브젝트는 맵과 함께 로딩되며, Network 정보가 자동으로 설정됩니다.
    /// Network ID가 고유한 지는 빌드 과정에서 검증하고 있지 않습니다. 중복된 Network ID가 존재한다는 에러가 발생하면, 맵에 포함된 VObject를 확인해야 합니다.
    /// </para>
    ///
    /// 자세한 사용법은 <see href="https://wiki.viven.app/developer"/>를 참고하세요.
    /// </remarks>
    /// 
    [AddComponentMenu("VivenSDK/Network/Viven VObject")]
    public class VObject : MonoBehaviour
    {
        /// <summary>
        /// 해당 오브젝트의 이름입니다.
        /// </summary>
        public string displayName;

        /// <summary>
        /// Networking할 때 사용되는 Object의 ID입니다.
        /// </summary>
        public string objectId;

        /// <summary>
        /// 해당 Object의 Content Type입니다. 
        /// </summary>
        public SDKContentType contentType = SDKContentType.Prepared;

        /// <summary>
        /// Object의 동기화 방식입니다.
        /// </summary>
        public SDKSyncType objectSyncType = SDKSyncType.Continuous;

    #if UNITY_EDITOR
        /// <summary>
        /// NetworkId 를 초기화시키기 위한 변수들, Editor에서만 사용됩니다.
        /// 한번이라도 초기화되었다면 처리하지 않습니다.
        /// </summary>
        [SerializeField, HideInInspector] private bool isInitialized = false;

        [SerializeField, HideInInspector] private bool isPrefab = false;

        /// <summary>
        /// Editor에서 SDKNetworkObject를 생성하거나 변경할 때, ObjectID를 생성합니다.
        /// </summary>
        private void OnValidate()
        {
            var newIsPrefab = PrefabUtility.IsPartOfPrefabAsset(this);

            if (isPrefab != newIsPrefab)
            {
                // 만약 Prefab이 Scene에서 Instance로 생성되었다면
                if (isPrefab)
                {
                    isPrefab = newIsPrefab;
                    // GUID를 초기화한다.
                    OnReset();
                }
                else
                {
                    isPrefab = newIsPrefab;
                }
            }

            // objectId가 비어있거나 Guid로 변환할 수 없는 경우
            if (string.IsNullOrEmpty(objectId) || !Guid.TryParse(objectId, out _))
            {
                isInitialized = false;
            }

            // 초기화가 되지 않은 경우
            if (!isInitialized)
            {
                OnReset();
                return;
            }

            // Scene에 있는 Instance일 때
            if (!isPrefab)
            {
                // 다른 Instance에서 복사한 경우
                if (Event.current != null)
                {
                    switch (Event.current.type)
                    {
                        case EventType.ExecuteCommand when Event.current.commandName == "Duplicate":
                        case EventType.ExecuteCommand when Event.current.commandName == "Paste":
                            OnReset();
                            break;
                    }
                }
            }
        }

        private void Reset()
        {
            OnReset();
        }

        // TODO : Batch 실행으로 변경해야 함 (https://docs.unity3d.com/ScriptReference/GlobalObjectId.GlobalObjectIdentifiersToInstanceIDsSlow.html)
        private void OnReset()
        {
            Debug.Log("Reset");
            isPrefab = PrefabUtility.IsPartOfPrefabAsset(this);

            // GlobalObjectId : Object의 고유 ID, 씬이 reload되어도 변하지 않음.
            // <see cref="https://docs.unity3d.com/ScriptReference/GlobalObjectId.html"/>
            if (isPrefab)
                // Prefab은 GlobalObjectId가 존재하지 않음. 임의의 Guid 생성, Persistent하게 바꾸려면 AssetDatabase의 Guid로 생성 필요
                objectId = Guid.NewGuid().ToString();
            else
                objectId = ParseHashcodeToGuid(GlobalObjectId.GetGlobalObjectIdSlow(this).GetHashCode());

            isInitialized = true;
        }

        private string ParseHashcodeToGuid(int hashCode)
        {
            var bytes = new byte[16];
            BitConverter.GetBytes(hashCode).CopyTo(bytes, 0);
            BitConverter.GetBytes(hashCode).CopyTo(bytes, 4);
            BitConverter.GetBytes(hashCode).CopyTo(bytes, 8);
            BitConverter.GetBytes(hashCode).CopyTo(bytes, 12);
            return new Guid(bytes).ToString();
        }
    #endif
    }
}