
-- 버튼에 타임라인 시작 이벤트 연결
function awake()
    Btn:GetComponent(typeof(Button)).onClick:AddListener(StartTimeline)
end


function StartTimeline()
    --Timeline LuaScript 가져오기
    local timelineComponent = Amy:GetLuaComponent("Timeline")
    
    -- 선물 오브젝트가 이미 존재하면 삭제
    if(timelineComponent ~= nil) then
        local gift = timelineComponent.gift
        if (gift ~= nil) then
            self:DoDestroy(gift)
            gift = nil
        end    
    end
    
    -- 타임라인 초기화 및 시작
    local DirectorComponent = Director:GetComponent(typeof(PlayableDirector))
    DirectorComponent:Stop()
    DirectorComponent.time = 0
    DirectorComponent:Play()
end