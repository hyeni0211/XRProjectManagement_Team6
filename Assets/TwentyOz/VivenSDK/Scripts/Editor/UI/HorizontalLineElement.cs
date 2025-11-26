using UnityEngine;
using UnityEngine.UIElements;

namespace TwentyOz.VivenSDK.Scripts.Editor.UI
{
    [UxmlElement]
    internal partial class HorizontalLineElement : VisualElement
    {
        public HorizontalLineElement()
        {
            name = "horizontal-line";
            style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, 1));
            style.height = new Length(2, LengthUnit.Pixel);
            style.width = Length.Percent(100);
        }
    }
}