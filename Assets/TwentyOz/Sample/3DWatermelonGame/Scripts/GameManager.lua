--과일의 종류
local fruitTypes = { Blueberry, Cherry, Lime, Banana, Persimmon, Apple, Peach, Coconut, Melon, Pineapple, Watermelon }
--현재 생성된 과일
local currentFruit = nil
--과일 생성 딜레이
local delay = 1.0
local deltaTime = 0
local isGameEnd = false
function update()
    -- 게임이 끝났다면 더 이상 게임을 진행하지 않습니다.
    if isGameEnd then
        return
    end
    
    --과일이 생성된 이후 다음 과일은 일정 딜레이 후에 생성됩니다.
    if deltaTime > delay then
        --설정된 딜레이가 지났고 현재 생성된 과일이 없다면 과일을 생성합니다.
        if currentFruit == nil then
            InstantiateFruit(fruitTypes[math.random(5)])
        --현재 생성된 과일이 있고 F키를 눌렀다면 과일을 아래로 떨어뜨립니다.
        elseif Keyboard.current[Key.F].wasPressedThisFrame then
            --과일의 물리적인 특성을 적용합니다.
            currentFruit:GetComponent(typeof(Rigidbody)).isKinematic = false
            --과일이 바로 내려갈 수 있도록 아래로 힘을 줍니다.
            currentFruit:GetComponent(typeof(Rigidbody)):AddForce(Vector3(math.random(-5,5), -100, math.random(-5,5)))
            --과일이 더 이상 플레이어의 조작을 따라가지 않도록 부모 관계를 해제합니다.
            currentFruit.transform:SetParent(nil)
            --현재 생성된 과일을 nil로 설정하여 다음 과일이 생성될 수 있도록 합니다.
            currentFruit = nil
            --딜레이를 0으로 설정하여 일정 시간동안 기다린 후 과일이 생성될 수 있도록 합니다.
            deltaTime = 0
        end
    else
        --딜레이가 지나지 않았다면 deltaTime에 Time.deltaTime을 더해줍니다.
        deltaTime = deltaTime + Time.deltaTime
    end
    
    --IJKL 키를 누르면 과일이 생성될 위치가 조정됩니다.
    --과일을 놓는 위치는 지정된 범위(4x4) 안에서만 가능합니다.
    
    -- I키를 누르면 -Z축으로 이동합니다.
    if Keyboard.current[Key.I].isPressed then
        local value = self.transform.position + Vector3(0, 0, -0.1)
        --과일의 Z축이 -4보다 작아지지 않도록 설정합니다.
        if value.z > -4 then
            self.transform.position = value
        end
        -- K키를 누르면 +Z축으로 이동
    elseif Keyboard.current[Key.K].isPressed then
        local value = self.transform.position + Vector3(0, 0, 0.1)
        --과일의 Z축이 4보다 커지지 않도록 설정합니다.
        if value.z < 4 then
            self.transform.position = value
        end
    -- J키를 누르면 +X축으로 이동
    elseif Keyboard.current[Key.J].isPressed then
        local value = self.transform.position + Vector3(0.1, 0, 0)
        --과일의 X축이 4보다 커지지 않도록 설정합니다.
        if value.x < 4 then
            self.transform.position = value
        end
    -- L키를 누르면 -X축으로 이동
    elseif Keyboard.current[Key.L].isPressed then
        local value = self.transform.position + Vector3(-0.1, 0, 0)
        --과일의 X축이 -4보다 작아지지 않도록 설정합니다.
        if value.x > -4 then
            self.transform.position = value
        end
    -- R키를 누르면 과일의 위치를 초기화 합니다.
    elseif Keyboard.current[Key.R].isPressed then
        self.transform.position = Vector3(0, 10, 0)
    elseif Keyboard.current[Key.T].isPressed then
        Player.Mine.FadeIn()
    elseif Keyboard.current[Key.Y].isPressed then
        Player.Mine.FadeOut()
    end
end

--과일을 생성합니다.
function InstantiateFruit(fruitType)
    --과일을 생성하여 현재 생성된 과일로 설정합니다.
    currentFruit = GameObject.Instantiate(fruitType)
    --과일의 물리적인 특성을 적용하지 않도록 설정합니다. (바로 떨어지지 않도록 하기 위해)
    currentFruit:GetComponent(typeof(Rigidbody)).isKinematic = true

    currentFruit:GetComponent(typeof(Rigidbody)).interpolation = CS.UnityEngine.RigidbodyInterpolation.None;
    
    --과일의 위치와 각도를 플레이어의 조작 위치와 각도로 설정합니다.
    currentFruit.transform.position = self.transform.position
    currentFruit.transform.rotation = self.transform.rotation
    --과일을 조작 오브젝트의 자식으로 설정하여 플레이어의 조작으로 과일의 위치가 변경되도록 합니다.
    currentFruit.transform.parent = self.transform
end

--게임이 클리어 되었을 경우 실행됩니다.
function GameClear()
    UIText:GetComponent("TMP_Text").text = "Game Clear !!!"
    isGameEnd = true
end

--게임이 끝났을 경우 실행됩니다.
function GameOver()
    UIText:GetComponent("TMP_Text").text = "Game Over !!!"
    isGameEnd = true
end