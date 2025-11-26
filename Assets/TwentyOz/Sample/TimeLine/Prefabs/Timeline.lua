gift = nil

-- 택스트를 띄어주는 함수
function chat(val)
    local textAsset = TextAsset:GetComponent(typeof(TMP_Text))
    textAsset.text = val
end

-- 선물을 생성하는 함수
function spawn(go)
    gift = self:DoInstantiate(go)
    gift.transform.position = RightIndex.transform.position + Vector3.up * 0.05
end