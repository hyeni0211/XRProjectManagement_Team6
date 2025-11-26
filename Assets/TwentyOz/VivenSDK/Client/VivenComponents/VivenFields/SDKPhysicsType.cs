namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.VivenFields
{
    /// <summary>
    /// SDKPhysicsType은 물체와 상호작용할 때 물리 엔진의 영향을 결정합니다.
    /// </summary>
    /// <summary>
    /// SDKPhysicsType은 VR 상호작용 시 사용자 경험에 큰 영향을 끼칠 수 있습니다.
    /// Kinematic인 물체는 허공에서 놓더라도 물체가 떨어지지 않습니다. Physics는 물체가 중력의 영향을 받아 떨어질 수 있습니다.
    /// 마찬가지로, Kinematic인 물체는 던질 수 없습니다.
    ///
    /// <para>
    /// Kinematic인 물체도 Collision을 수행합니다. 배치 모드에서 Kinematic인 물체를 무시하고 겹치거나, 통과할 수 없습니다.
    /// </para>
    /// </summary>
    public enum SDKPhysicsType
    {
        /// <summary>
        /// 물리의 영향을 받지 않음
        /// </summary>
        Kinematic,
        /// <summary>
        /// Physics의 영향을 받음
        /// </summary>
        Physics
    }
}