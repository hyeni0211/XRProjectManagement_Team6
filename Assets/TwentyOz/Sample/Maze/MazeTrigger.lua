local completeText

function start()
    completeText = CompleteText.gameObject:GetComponent(typeof(TMP_Text))
    completeText.text = ""
end


function onTriggerEnter(col)
    local characterController = col.gameObject:GetComponent(typeof(CharacterController))
    if characterController then
        completeText.text = "You Win!"
    end
end