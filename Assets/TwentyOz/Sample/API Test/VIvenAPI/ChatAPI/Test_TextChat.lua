--[[
VivenAPI/ChatAPI/Test_TextChat.lua
ChatAPI 테스트 코드

Author : 이준서
Date : 2025.01.06
]]
local util = require 'xlua.util'

-- 테스트 코드 시작
function start()
    self:StartCoroutine(util.cs_generator(RunTest))
    lastMessageUi = lastMessageUi:GetComponent(typeof(TMP_Text))
end

function RunTest()
    coroutine.yield(WaitForSeconds(10))

    Test_SendTextMessageToAll()
    Test_SentTextMessageToMyself()
    Test_SendTextMessageToCreator()
end

function Test_SendTextMessageToAll()
    TextChat.SendChannelTextMessage("Channel Message Send : Hello, World!")
end

function Test_SentTextMessageToMyself()
    TextChat.SendDirectTextMessage("Direct Message Send To Myself : Hello, World!", Player.Mine.UserID)
end

function Test_SendTextMessageToCreator()
    local creatorUserID = Room.GetCreatorUserID()
    TextChat.SendDirectTextMessage("Direct Message To Room Creator, From: " .. Player.Mine.UserID, creatorUserID)
end

function onChannelTextMessageReceived(senderUserId, message)
    print("onChannelTextMessageReceived: " .. senderUserId .. " Message: " .. message)
    lastMessageUi.text = "Last Message[".. senderUserId .. "]: " .. message
end

function onDirectTextMessageReceived(senderUserId, message)
    print("onDirectTextMessageReceived: " .. senderUserId .. " Message: " .. message)
    lastMessageUi.text = "Last Direct Message[".. senderUserId .. "]: " .. message
end
-- 테스트 코드 끝
