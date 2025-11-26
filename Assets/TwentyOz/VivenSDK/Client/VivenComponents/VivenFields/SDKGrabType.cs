namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.VivenFields
{
    /// <summary>
    /// 물체를 잡았을 때, 해당 물체의 Rigidbody 상태를 결정합니다.
    /// </summary>
    /// <remarks>
    /// 물체를 잡았을 때, Kinematic은 물체의 물리적 상태를 무시하고, Interactor의 위치에 따라 물체가 이동합니다.
    /// Velocity는 물체의 물리적 상태가 유지됩니다. 물체를 잡고 이동할 때 다른 물체와 부딪히거나, 플레이어가 순간이동한다면 물체는 물리적 특성에 따라 움직입니다.
    /// </remarks>
    public enum SDKGrabType
    {
        /// <summary>
        /// 물체가 충돌 등 물리학의 영향을 받지 않습니다.    
        /// </summary>
        Kinematic,
        /// <summary>
        /// 물체가 물리학의 영향을 받습니다.
        /// </summary>
        Velocity
    }
}