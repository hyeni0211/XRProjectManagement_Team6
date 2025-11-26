local util = require 'xlua.util'

function start()
    self:StartCoroutine(util.cs_generator(RunTest))
end

function RunTest()
    ScreenRecording.StartRecording()

    ScreenRecording.PauseRecording()

    coroutine.yield(WaitForSeconds(5))

    ScreenRecording.ResumeRecording()

    coroutine.yield(WaitForSeconds(10))

    ScreenRecording.StopRecording()

    ScreenRecording.SetFrameRate(30)
    print(ScreenRecording.GetFrameRate())

    ScreenRecording.SetOutputPath("C:/Users/USER/Desktop")
    print(ScreenRecording.GetOutputPath())

    ScreenRecording.SetOutputFileName("Test")
    print(ScreenRecording.GetOutputFileName())

    ScreenRecording.ClearOutputPaths()
    print(ScreenRecording.GetOutputPath())
    print(ScreenRecording.GetOutputPath())

    local deviceName = ScreenRecording.GetCurrentAudioInputDevice()
    ScreenRecording.SetAudioInputDevice(deviceName)
    print(ScreenRecording.GetCurrentAudioInputDevice())
end