local util = require 'xlua.util'

-- getAvatarProfile 함수는 Parameter가 없는 함수입니다.
-- public (static) void getAvatarProfile(Action<Texture2D> onFinished = null) <- 뒤에 콜백은 무시해도 됨. util.async_to_sync를 할때 포함되어야 하는 파라미터

-- Static 메소드의 경우 (매소드 직접 찾기)
--local API = util.async_to_sync(CS.TwentyOz.VivenSDK.Scripts.Core.Lua.VivenLuaBehaviour.GetAvatarProfile);

-- nonStatic 메소드의 경우 (메소드를 변수로 찾기)
local API = util.async_to_sync(self.GetAvatarProfile);

local SetMaterialByCharacterProfile = function()
    -- Static 메소드의 경우 (C# Reflection에서 Invoke시 Static 함수는 Instatnce 된 Class가 필요없어서 Parameter 없이 실행 가능함.)
    -- local texture = API()
    -- nonStatic 메소드의 경우 (일반 메소드는 Reflection으로 Invoke시 Instance 된 Class를 파라미터에 포함해줘야함. 따라서 함수 자체에는 파라미터가 존재하지 않지만, 자기자신을 넣어줘야 Invoke시킬 수 있음.)
    local texture = API(self)

    local meshRenderer = self:GetComponent(typeof(MeshRenderer))
    meshRenderer.material.mainTexture = texture;
end

function start()
    local logic = util.coroutine_call(SetMaterialByCharacterProfile);
    logic();
end
    