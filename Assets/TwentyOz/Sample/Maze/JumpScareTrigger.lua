local initialPosition

local scaryAudioSource
local scaryLight

local moveTween

function start()
    initialPosition = ScaryImage.transform.position
    scaryAudioSource = ScaryImage:GetComponent(typeof(AudioSource))
    scaryLight = ScaryImage:GetComponentInChildren(typeof(Light))

    -- at first audio is not playing
    scaryAudioSource.playOnAwake = false
    scaryAudioSource.loop = false
    scaryAudioSource.volume = 1

    -- at first light is not playing
    scaryLight.enabled = false

    -- set image hide at first (lower than floor)
    ScaryImage.transform.position = Vector3(initialPosition.x, -4, initialPosition.z)
    -- set image gameObject to inactive
    ScaryImage.gameObject:SetActive(false)
end

function onTriggerEnter(col)
    local characterController = col.gameObject:GetComponent(typeof(CharacterController))
    if characterController then
        ScaryImage.gameObject:SetActive(true)

        -- Play Tween Move
        if moveTween ~= nil then
            self:KillTween(moveTween, true)
            ScaryImage.transform.position = Vector3(initialPosition.x, -4, initialPosition.z)
        end
        moveTween = self:DoMoveTarget(ScaryImage.transform, initialPosition, 1, 0.5)
        self:OnTweenComplete(moveTween, function()
            moveTween = self:DoMoveTarget(ScaryImage.transform, Vector3(initialPosition.x, -4, initialPosition.z), 1, 0.5)
            self:DelayTween(moveTween, 5)
        end)

        -- Play Scary Sound
        scaryAudioSource:Play()

        -- Find ParticleSystem and play it
        local particleSystem = ScaryImage:GetComponentInChildren(typeof(ParticleSystem))
        particleSystem:Play()

        -- Play Scary Light
        scaryLight.enabled = true
    end
end