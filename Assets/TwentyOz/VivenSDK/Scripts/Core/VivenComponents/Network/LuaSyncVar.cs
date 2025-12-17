using System;

namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.Network
{
    /// <summary>
    /// 네트워크를 통해 자동 동기화되는 변수를 나타내는 클래스입니다.
    /// </summary>
    /// <remarks>
    /// <para>LuaSyncVar는 VivenCustomSyncView에서 생성되는 동기화 변수 객체입니다.</para>
    /// <para>소유권을 가진 클라이언트에서 값을 변경하면 자동으로 모든 클라이언트에게 동기화됩니다.</para>
    /// <para>지원되는 타입: primitive types (int, float, bool 등), string, Vector3, Quaternion</para>
    /// <para>성능 최적화를 위해 Dirty Flag 패턴을 사용하여 변경된 값만 네트워크 전송합니다.</para>
    /// </remarks>
    /// <example>
    /// <code language="lua">
    /// -- awake()에서 SyncVar 생성
    /// function awake()
    ///     healthVar = SyncView:CreateLuaSyncVar("health", 100)
    ///     scoreVar = SyncView:CreateLuaSyncVar("score", 0)
    /// 
    ///     -- 콜백 등록
    ///     healthVar:AddListener(onHealthChanged)
    ///     scoreVar:AddListener(onScoreChanged)
    /// end
    /// 
    /// -- 값 변경 콜백
    /// function onHealthChanged(oldValue, newValue)
    ///     print("체력 변경: " .. oldValue .. " → " .. newValue)
    ///     updateHealthBar(newValue)
    /// end
    /// 
    /// -- 값 변경 (소유자만 가능)
    /// function takeDamage(damage)
    ///     if SyncView.IsMine then
    ///         local currentHealth = self.healthVar:Get()
    ///         healthVar:Set(math.max(0, currentHealth - damage))
    ///     end
    /// end
    /// 
    /// -- 즉시 동기화 (중요한 변경사항)
    /// function respawn()
    ///     if SyncView.IsMine then
    ///         healthVar:SetValueImmediately(100)
    ///         print("즉시 부활!")
    ///     end
    /// end
    /// </code>
    /// </example>
    public class LuaSyncVar
    {
        /// <summary>
        /// 이 SyncVar의 고유 식별자를 가져옵니다.
        /// </summary>
        /// <value>
        /// SyncVar 생성 시 지정한 고유 ID 문자열
        /// </value>
        /// <remarks>
        /// <para>네트워크 동기화 시 이 ID를 통해 올바른 SyncVar를 식별합니다.</para>
        /// <para>모든 클라이언트에서 동일한 ID로 SyncVar를 생성해야 합니다.</para>
        /// <para>디버깅이나 로깅 목적으로 사용할 수 있습니다.</para>
        /// </remarks>
        /// <example>
        /// <code language="lua">
        /// function awake()
        ///     healthVar = SyncView:CreateLuaSyncVar("player_health", 100)
        ///     scoreVar = SyncView:CreateLuaSyncVar("player_score", 0)
        /// end
        /// 
        /// function debugSyncVars()
        ///     print("체력 SyncVar ID: " .. self.healthVar.VarId)  -- "player_health"
        ///     print("점수 SyncVar ID: " .. self.scoreVar.VarId)   -- "player_score"
        /// end
        /// </code>
        /// </example>
        public string VarId { get; }

        /// <summary>
        /// 현재 SyncVar의 값을 가져옵니다.
        /// </summary>
        /// <returns>현재 저장된 값</returns>
        /// <remarks>
        /// <para>소유권과 관계없이 모든 클라이언트에서 호출 가능합니다.</para>
        /// <para>네트워크를 통해 동기화된 최신 값을 반환합니다.</para>
        /// </remarks>
        /// <example>
        /// <code language="lua">
        /// function checkHealth()
        ///     local currentHealth = healthVar:Get()
        ///     print("현재 체력: " .. currentHealth)
        ///     
        ///     if currentHealth <= 0 then
        ///         print("사망!")
        ///     end
        /// end
        /// </code>
        /// </example>
        public object Get()
        {
            return null;
        }


        /// <summary>
        /// SyncVar의 값을 변경하고 모든 클라이언트에게 동기화합니다.
        /// </summary>
        /// <param name="newValue">설정할 새로운 값</param>
        /// <remarks>
        /// <para>소유권을 가진 클라이언트에서만 호출 가능합니다.</para>
        /// <para>값이 실제로 변경된 경우에만 네트워크 동기화가 발생합니다.</para>
        /// <para>변경 시 비소유자 클라이언트에서 onValueChanged 콜백이 호출됩니다.</para>
        /// <para>지원되는 타입: primitive types, string, Vector3, Quaternion</para>
        /// </remarks>
        /// <example>
        /// <code language="lua">
        /// function increaseScore(points)
        ///     if SyncView.IsMine then
        ///         local currentScore = scoreVar:Get()
        ///         scoreVar:Set(currentScore + points)
        ///         print("점수 증가: " .. (currentScore + points))
        ///     else
        ///         print("소유권이 없어 점수를 변경할 수 없습니다")
        ///     end
        /// end
        /// 
        /// function movePlayer(newPosition)
        ///     if SyncView.IsMine then
        ///         positionVar:Set(newPosition)
        ///     end
        /// end
        /// </code>
        /// </example>
        /// <exception cref="System.ArgumentException">지원되지 않는 타입의 값을 설정할 때 발생</exception>
        public void Set(object newValue)
        {
        }
        
        /// <summary>
        /// 값 변경 시 호출될 콜백 함수를 등록합니다.
        /// </summary>
        /// <param name="callback">값 변경 시 호출될 콜백 함수 (oldValue, newValue)</param>
        /// <remarks>
        /// <para>기본적으로 비소유자 클라이언트에서만 콜백이 호출됩니다.</para>
        /// <para>alwaysInvokeOnChange=true로 생성된 SyncVar의 경우 소유자에서도 콜백이 호출됩니다.</para>
        /// <para>여러 콜백을 등록할 수 있으며, 등록된 순서대로 호출됩니다.</para>
        /// </remarks>
        /// <example>
        /// <code language="lua">
        /// function awake()
        ///     healthVar = SyncView:CreateLuaSyncVar("health", 100)
        ///     
        ///     -- 단일 콜백 등록
        ///     healthVar:AddListener(onHealthChanged)
        ///     
        ///     -- 익명 함수로 콜백 등록
        ///     healthVar:AddListener(function(oldValue, newValue)
        ///         print("체력 변경됨: " .. oldValue .. " → " .. newValue)
        ///     end)
        /// end
        /// 
        /// function onHealthChanged(oldHealth, newHealth)
        ///     updateHealthBar(newHealth)
        ///     
        ///     if newHealth <= 0 then
        ///         onPlayerDied()
        ///     elseif oldHealth > newHealth then
        ///         playDamageEffect()
        ///     end
        /// end
        /// </code>
        /// </example>
        public void AddListener(Action<object, object> callback)
        {
        }

        /// <summary>
        /// 등록된 값 변경 콜백 함수를 제거합니다.
        /// </summary>
        /// <param name="callback">제거할 콜백 함수</param>
        /// <remarks>
        /// <para>AddListener로 등록한 콜백과 정확히 동일한 함수 참조를 전달해야 합니다.</para>
        /// <para>등록되지 않은 콜백을 제거하려고 해도 오류가 발생하지 않습니다.</para>
        /// <para>익명 함수로 등록한 콜백은 참조를 저장해두지 않으면 제거할 수 없습니다.</para>
        /// </remarks>
        /// <example>
        /// <code language="lua">
        /// function awake()
        ///     healthVar = SyncView:CreateLuaSyncVar("health", 100)
        ///     healthVar:AddListener(onHealthChanged)
        /// end
        /// 
        /// function stopHealthMonitoring()
        ///     -- 등록된 콜백 제거
        ///     healthVar:RemoveListener(onHealthChanged)
        ///     print("체력 모니터링 중지")
        /// end
        /// 
        /// function onHealthChanged(oldValue, newValue)
        ///     print("체력: " .. newValue)
        /// end
        /// </code>
        /// </example>
        public void RemoveListener(Action<object, object> callback)
        {
        }
        
        /// <summary>
        /// SyncVar의 현재 값을 가져오거나 설정합니다.
        /// </summary>
        /// <value>
        /// getter: 현재 저장된 값을 반환합니다. (모든 클라이언트에서 호출 가능)
        /// setter: 새로운 값을 설정하고 동기화합니다. (소유자만 호출 가능)
        /// </value>
        /// <remarks>
        /// <para>Get() 및 Set() 메서드의 프로퍼티 버전입니다.</para>
        /// <para>Lua에서 더 직관적인 문법으로 사용할 수 있습니다.</para>
        /// </remarks>
        /// <example>
        /// <code language="lua">
        /// function updatePlayerStats()
        ///     -- 값 읽기 (getter)
        ///     local currentHealth = healthVar.Value
        ///     local currentScore = scoreVar.Value
        ///     
        ///     print("체력: " .. currentHealth .. ", 점수: " .. currentScore)
        ///     
        ///     -- 값 설정 (setter) - 소유자만 가능
        ///     if SyncView.IsMine then
        ///         healthVar.Value = 100  -- Set(100)과 동일
        ///         scoreVar.Value = currentScore + 10
        ///     end
        /// end
        /// </code>
        /// </example>
        public object Value
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// 값을 즉시 변경하고 네트워크를 통해 즉시 동기화합니다.
        /// </summary>
        /// <param name="newValue">설정할 새로운 값</param>
        /// <remarks>
        /// <para>Set() 메서드와 달리 값 변경 후 즉시 네트워크 전송을 수행합니다.</para>
        /// <para>중요한 상태 변경이나 긴급한 동기화가 필요한 경우에 사용합니다.</para>
        /// <para>일반적인 경우에는 Set() 메서드를 사용하는 것이 성능상 유리합니다.</para>
        /// <para>소유권을 가진 클라이언트에서만 호출 가능합니다.</para>
        /// </remarks>
        /// <example>
        /// <code language="lua">
        /// function respawnPlayer()
        ///     if SyncView.IsMine then
        ///         -- 즉시 체력 회복 및 동기화
        ///         healthVar:SetValueImmediately(100)
        ///         
        ///         -- 즉시 위치 리셋 및 동기화
        ///         local spawnPoint = Vector3(0, 0, 0)
        ///         positionVar:SetValueImmediately(spawnPoint)
        ///         
        ///         print("플레이어 즉시 부활!")
        ///     end
        /// end
        /// 
        /// function emergencyStop()
        ///     if SyncView.IsMine then
        ///         -- 긴급 정지 - 즉시 동기화 필요
        ///         speedVar:SetValueImmediately(0)
        ///         isMovingVar:SetValueImmediately(false)
        ///     end
        /// end
        /// </code>
        /// </example>
        /// <exception cref="System.ArgumentException">지원되지 않는 타입의 값을 설정할 때 발생</exception>
        public void SetValueImmediately(object newValue)
        {
        }
    }
}