local util = require 'xlua.util'

function start()
    ecanvas = self:GetComponentInChildren(typeof(ECanvas))
    Debug.Log("ecanvas: " .. ecanvas.gameObject.name)
    
    ecanvas:SetRole("Presenter")
    ecanvas:ApplyStreamingSetting("5yra-u2xm-af0b-4kwy-a3pj", "https://youtu.be/YGbzp2P9NGs")
    
    -- 전자칠판 초기화까지 대기
    self:StartCoroutine(util.cs_generator(WaitInitializedAndSetting))
end 

function WaitInitializedAndSetting()
    -- 전자칠판 초기화까지 대기
    coroutine.yield(WaitUntil(function() return ecanvas.IsECanvasInitialized end))

    Debug.Log("Blackboard is initialized")
    --ecanvas:StartStreaming()
end