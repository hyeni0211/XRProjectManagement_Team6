local util = require 'xlua.util'

function start()
    self:StartCoroutine(util.cs_generator(RunTest))
end

function RunTest()
    coroutine.yield(WaitForSeconds(5))

    Test_GetLocale()
    Test_SetLocaleToKorean()
    Test_SetLocaleToEnglish()
end

function Test_GetLocale()
    print(Locale.GetLocale())
end


function Test_SetLocaleToKorean()
    Locale.SetLocale("Korean")
    print("Test_SetLocaleToKorean: " .. Locale.GetLocale())
end

function Test_SetLocaleToEnglish()
    Locale.SetLocale("English")
    print("Test_SetLocaleToEnglish: " .. Locale.GetLocale())
end

