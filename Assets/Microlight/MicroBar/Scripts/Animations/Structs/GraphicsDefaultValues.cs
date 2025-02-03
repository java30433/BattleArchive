using UnityEngine;
using UnityEngine.UI;

namespace Microlight.MicroBar
{
    // ****************************************************************************************************
    // Stores default values for a graphics component
    // ****************************************************************************************************
    internal readonly struct GraphicsDefaultValues
    {
        readonly Color color;
        readonly float fade;
        readonly float fill;
        readonly Vector3 position;
        readonly float rotation;
        readonly Vector3 scale;
        readonly Vector2 anchorPosition;

        internal readonly Color Color => color;
        internal readonly float Fade => fade;
        internal readonly float Fill => fill;
        internal readonly Vector3 Position => position;
        internal readonly float Rotation => rotation;
        internal readonly Vector3 Scale => scale;
        internal readonly Vector2 AnchorPosition => anchorPosition;

        internal GraphicsDefaultValues(Image image)
        {
            color = image.color;
            fade = image.color.a;
            fill = image.fillAmount;
            position = image.rectTransform.localPosition;
            rotation = image.rectTransform.localRotation.eulerAngles.z;
            scale = image.rectTransform.localScale;
            anchorPosition = image.rectTransform.anchoredPosition;
        }
        internal GraphicsDefaultValues(SpriteRenderer sprite)
        {
            color = sprite.color;
            fade = sprite.color.a;
            fill = sprite.transform.localScale.x;
            position = sprite.transform.localPosition;
            rotation = sprite.transform.localRotation.eulerAngles.z;
            scale = sprite.transform.localScale;
            anchorPosition = Vector2.zero;
        }
    }
}
