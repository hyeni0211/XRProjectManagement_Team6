using System;
using UnityEngine;

namespace Twoz.Viven.RuntimeInteractionObject.Core
{
    /// <summary>
    /// RIIO에서 데이터를 보내는 컴포넌트입니다. 
    /// </summary>
    [AddComponentMenu("VivenSDK/RIIO/RIIO Sender")]
    [RequireComponent(typeof(BoxCollider))]
    public class RIIOSender : MonoBehaviour
    {
        /// <summary>
        /// sender의 이름입니다.
        /// </summary>
        public string senderName;
        
        /// <summary>
        /// sender의 설명입니다.
        /// </summary>
        public string description; 
        
        /// <summary>
        /// 송신할 데이터의 자료형을 설정할 수 있습니다.
        /// </summary>
        public RIIODataType dataType;
        // public RIIOReceiver[] pre_connected_receivers;
        
    #if UNITY_EDITOR
        private void Reset()
        {
            var sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = 0.1f;
            
            
            if (String.IsNullOrEmpty(senderName))
            {
                senderName = this.gameObject.name;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.1f);
        }
    #endif
    }
}