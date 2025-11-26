local timer = 0

function update()
    if timer > beat then
        local cube
        local color = math.random(2);
        
        -- 큐브 색상을 랜덤으로 해서 생성
        if color == 1 then
            cube = GameObject.Instantiate(cubeRed)
        else
            cube = GameObject.Instantiate(cubeBlue)
        end
        
        cube.transform.position = self.transform.position
        timer = timer - beat
    end
    timer = timer + Time.deltaTime
end
