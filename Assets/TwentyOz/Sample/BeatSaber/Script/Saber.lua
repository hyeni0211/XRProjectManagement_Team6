local material

-- Saber의 Material을 가져옴
function awake()
    material = self:GetComponent(typeof(Renderer)).material
end

-- Saber를 잡았을 때 Spawnmer를 켬
function onGrab()
    spawner.gameObject:SetActive(true)
end

-- Saber를 놓았을 때 Spawnmer를 끔
function onRelease()
    spawner.gameObject:SetActive(false)
end

-- Saber를 잡고 왼쪽 마우스 버튼을 누르면 Saber의 색깔이 바뀜
function objectShortClickAction()
    if material.color == red then
        material.color = blue
        return
    elseif material.color == blue then
        material.color = red
    end
end