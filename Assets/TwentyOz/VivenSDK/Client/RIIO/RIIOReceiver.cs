using System;
using UnityEngine;

namespace Twoz.Viven.RuntimeInteractionObject.Core
{
    /// <summary>
    /// RIIO에서 데이터를 받는 컴포넌트입니다.
    /// </summary>
    [AddComponentMenu("VivenSDK/RIIO/RIIO Receiver")]
    [RequireComponent(typeof(BoxCollider))]
    public class RIIOReceiver : MonoBehaviour
    {
        /// <summary>
        /// receiver의 이름입니다. 
        /// </summary>
        public string receiverName;  
        
        /// <summary>
        /// receiver의 설명입니다.
        /// </summary>
        public string description;
        
        /// <summary>
        /// 수신할 데이터의 자료형을 설정할 수 있습니다.
        /// </summary>
        public RIIODataType dataType;
        
    #if UNITY_EDITOR
        private void Reset()
        {
            var sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = 0.1f;

            if (String.IsNullOrEmpty(receiverName))
            {
                receiverName = this.gameObject.name;
            }
        }

        /// <summary>
        /// Gizmo 이벤트
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 0.1f);
        }
    #endif
        
        public void Disconnect() { }
    }
}