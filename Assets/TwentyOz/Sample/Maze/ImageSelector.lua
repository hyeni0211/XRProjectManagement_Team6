local button1
local button2
local button3
local webView

function start()
    button1 = Button1:GetComponent(typeof(Button))
    button2 = Button2:GetComponent(typeof(Button))
    button3 = Button3:GetComponent(typeof(Button))
    webView = WebView:GetComponent(typeof(VivenWebView))

    button1.onClick:AddListener(function()
        webView:SetUrl("https://img.freepik.com/free-vector/halloween-background-in-flat-design_52683-43845.jpg?w=1800&t=st=1696966515~exp=1696967115~hmac=74d0cdec5d5db69beb87e8c0019de252f9349d3d046246fff43b725045e1a70e")
    end)

    button2.onClick:AddListener(function()
        webView:SetUrl("https://img.freepik.com/free-vector/watercolor-halloween-background_52683-43698.jpg?w=1800&t=st=1696967029~exp=1696967629~hmac=3bca63256ca2ec4d44dd8c408ea4a917c280453ca002ee3b1e71081898aff581")
    end)

    button3.onClick:AddListener(function()
        webView:SetUrl("https://img.freepik.com/free-vector/halloween-night-moon-composition-with-glowing-pumpkins-vintage-castle-and-bats-flying-over-cemetery-flat_1284-40781.jpg?w=1800&t=st=1696967071~exp=1696967671~hmac=a0e02dcd3fc940afd221f49b96543a6733870395f12a9f4acdf02d5a84ead105")
    end)
end