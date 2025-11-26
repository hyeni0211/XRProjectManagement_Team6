-- 큐브가 매 프레임마다 앞으로 이동
function update()
    self.transform.position = self.transform.position + (Time.deltaTime * self.transform.forward * 2);
end

function onTriggerEnter(col)
    if col.gameObject.name == "Saber" then
        -- Saber와 닿으면 Saber의 Material을 가져옴
        local material = col.gameObject:GetComponent(typeof(Renderer)).material  
        
        -- 큐브와 Saber의 색깔이 같은지 비교하고 같으면 큐브를 삭제
        if color == material.color then
            GameObject.Destroy(self.gameObject)
        end
    end

    -- 벽에 닿으면 큐브를 삭제
    if col.gameObject.name == "Wall" then
        GameObject.Destroy(self.gameObject)
    end
end

