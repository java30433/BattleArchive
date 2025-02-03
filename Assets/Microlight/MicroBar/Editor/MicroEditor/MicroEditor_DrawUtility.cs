using UnityEditor;
using UnityEngine;

namespace Microlight.MicroEditor
{
    // ****************************************************************************************************
    // Utility class for drawing custom editor elements
    // ****************************************************************************************************
    internal static class MicroEditor_DrawUtility
    {
        #region Fields
        /// <summary>
        /// Draws property, label + field
        /// </summary>
        /// <returns>Returns rect with new x and y positions where should next element be</returns>
        internal static Rect DrawProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect fieldRect = new Rect(
                position.x,
                position.y,
                position.width,
                MicroEditor_Utility.LineHeight);

            EditorGUI.PropertyField(fieldRect, property, label);
            return new Rect(
                position.x,
                position.y + fieldRect.height + MicroEditor_Utility.VerticalSpacing,
                position.width,
                position.height);
        }
        /// <summary>
        /// Draws 2 fileds in order LFFL where width of the label is constant
        /// </summary>
        /// <returns>Returns rect with new x and y positions where should next element be</returns>
        internal static Rect DrawTwoPropertiesConstantLabel(Rect position, SerializedProperty property1, GUIContent label1, SerializedProperty property2, GUIContent label2, int labelWidth = 70)
        {
            Rect label1Rect = new Rect(
                position.x,
                position.y,
                labelWidth,
                MicroEditor_Utility.LineHeight);
            Rect field1Rect = new Rect(
                position.x + label1Rect.width,
                position.y,
                position.width / 2 - labelWidth,
                MicroEditor_Utility.LineHeight);
            Rect field2Rect = new Rect(
                field1Rect.x + field1Rect.width,
                position.y,
                position.width / 2 - labelWidth,
                MicroEditor_Utility.LineHeight);
            Rect label2Rect = new Rect(
                field2Rect.x + field2Rect.width,
                position.y,
                labelWidth,
                MicroEditor_Utility.LineHeight);

            GUIStyle rightStyle = new GUIStyle(GUI.skin.label);
            rightStyle.alignment = TextAnchor.MiddleRight;

            EditorGUI.LabelField(label1Rect, label1);
            EditorGUI.PropertyField(field1Rect, property1, GUIContent.none);
            EditorGUI.PropertyField(field2Rect, property2, GUIContent.none);
            EditorGUI.LabelField(label2Rect, label2, rightStyle);
            return new Rect(
                position.x,
                position.y + field1Rect.height + MicroEditor_Utility.VerticalSpacing,
                position.width,
                position.height);
        }
        /// <summary>
        /// Draws 2 fileds in order LFFL where width of the field is constant
        /// </summary>
        /// <returns>Returns rect with new x and y positions where should next element be</returns>
        internal static Rect DrawTwoPropertiesConstantField(Rect position, SerializedProperty property1, GUIContent label1, SerializedProperty property2, GUIContent label2, int fieldWidth = 70)
        {
            Rect label1Rect = new Rect(
                position.x,
                position.y,
                position.width / 2 - fieldWidth,
                MicroEditor_Utility.LineHeight);
            Rect field1Rect = new Rect(
                position.x + label1Rect.width,
                position.y,
                fieldWidth,
                MicroEditor_Utility.LineHeight);
            Rect field2Rect = new Rect(
                field1Rect.x + field1Rect.width,
                position.y,
                fieldWidth,
                MicroEditor_Utility.LineHeight);
            Rect label2Rect = new Rect(
                field2Rect.x + field2Rect.width,
                position.y,
                position.width / 2 - fieldWidth,
                MicroEditor_Utility.LineHeight);

            GUIStyle rightStyle = new GUIStyle(GUI.skin.label);
            rightStyle.alignment = TextAnchor.MiddleRight;

            EditorGUI.LabelField(label1Rect, label1);
            EditorGUI.PropertyField(field1Rect, property1, GUIContent.none);
            EditorGUI.PropertyField(field2Rect, property2, GUIContent.none);
            EditorGUI.LabelField(label2Rect, label2, rightStyle);
            return new Rect(
                position.x,
                position.y + field1Rect.height + MicroEditor_Utility.VerticalSpacing,
                position.width,
                position.height);
        }
        /// <summary>
        /// Draws bolded label
        /// </summary>
        internal static Rect DrawBoldLabel(Rect position, string text, bool center = false)
        {
            Rect fieldRect = new Rect(
                position.x,
                position.y,
                position.width,
                MicroEditor_Utility.LineHeight);

            // Define style
            GUIStyle boldStyle = new GUIStyle(EditorStyles.boldLabel);
            if(center)
            {
                boldStyle.alignment = TextAnchor.MiddleCenter;
            }
            else
            {
                boldStyle.alignment = TextAnchor.MiddleLeft;
            }

            EditorGUI.LabelField(fieldRect, text, boldStyle);
            return new Rect(
                position.x,
                position.y + fieldRect.height + MicroEditor_Utility.VerticalSpacing,
                position.width,
                position.height);
        }
        #endregion

        #region Container
        internal static void DrawContainer(Rect rect)
        {
            DrawContainer(rect, Color.white);
        }
        /// <summary>
        /// Draws container with default element color and overlay color applied
        /// </summary>
        /// <param name="rect">Size of the container</param>
        /// <param name="elementColorMultiplier">Color overlay, 0-1f, Color.white leaves same color</param>
        internal static void DrawContainer(Rect rect, Color elementColorMultiplier)
        {
            DrawContainer(rect, MicroEditor_Utility.ElementColor * elementColorMultiplier, MicroEditor_Utility.OutlineColor, MicroEditor_Utility.LightOutlineColor);
        }
        /// <summary>
        /// Draws container with colored background and outline with lighter corners
        /// </summary>
        /// <param name="rect">Area of container</param>
        /// <param name="elementColor">Background color</param>
        /// <param name="lightOutlineColor">Lighter color for the edges of the container</param>
        internal static void DrawContainer(Rect rect, Color elementColor, Color outlineColor, Color lightOutlineColor)
        {
            // Draw background
            EditorGUI.DrawRect(new Rect(rect.x + 1, rect.y + 1, rect.width - 2, rect.height - 2), elementColor);

            // Draw top border
            EditorGUI.DrawRect(new Rect(rect.x + 2, rect.y, rect.width - 4, 1), outlineColor);
            EditorGUI.DrawRect(new Rect(rect.x + 1, rect.y, 1, 2), lightOutlineColor);
            EditorGUI.DrawRect(new Rect(rect.xMax - 2, rect.y, 1, 2), lightOutlineColor);
            // Draw bottom border
            EditorGUI.DrawRect(new Rect(rect.x + 2, rect.yMax - 1, rect.width - 4, 1), outlineColor);
            EditorGUI.DrawRect(new Rect(rect.x + 1, rect.yMax - 2, 1, 2), lightOutlineColor);
            EditorGUI.DrawRect(new Rect(rect.xMax - 2, rect.yMax - 2, 1, 2), lightOutlineColor);
            // Draw left border
            EditorGUI.DrawRect(new Rect(rect.x, rect.y + 2, 1, rect.height - 4), outlineColor);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y + 1, 1, 1), lightOutlineColor);
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - 2, 1, 1), lightOutlineColor);
            // Draw right border
            EditorGUI.DrawRect(new Rect(rect.xMax - 1, rect.y + 2, 1, rect.height - 4), outlineColor);
            EditorGUI.DrawRect(new Rect(rect.xMax - 1, rect.y + 1, 1, 1), lightOutlineColor);
            EditorGUI.DrawRect(new Rect(rect.xMax - 1, rect.yMax - 2, 1, 1), lightOutlineColor);
        }
        /// <summary>
        /// Draw default glow effect over container
        /// </summary>
        /// <param name="rect">Area for of the container</param>
        internal static void DrawContainerGlow(Rect rect)
        {
            DrawContainerGlow(rect, Color.white);
        }
        /// <summary>
        /// Draws glow effect for the container but with color overlay
        /// </summary>
        /// <param name="rect">Area for of the container</param>
        /// <param name="colorMultiplier">Color overlay, 0-1f, Color.white leaves same color</param>
        internal static void DrawContainerGlow(Rect rect, Color colorMultiplier)
        {
            // Draw background
            EditorGUI.DrawRect(new Rect(rect.x + 2, rect.y + 1, rect.width - 4, rect.height - 2), MicroEditor_Utility.ElementHoverColor * colorMultiplier);
            EditorGUI.DrawRect(new Rect(rect.x + 1, rect.y + 2, 1, rect.height - 4), MicroEditor_Utility.ElementHoverColor * colorMultiplier);
            EditorGUI.DrawRect(new Rect(rect.xMax - 2, rect.y + 2, 1, rect.height - 4), MicroEditor_Utility.ElementHoverColor * colorMultiplier);
        }
        /// <summary>
        /// Draws light outline at the bottom of the contaner
        /// </summary>
        /// <param name="rect">Area of the container</param>
        internal static void DrawContainerBottomOutline(Rect rect)
        {
            DrawContainerBottomOutline(rect, MicroEditor_Utility.LightOutlineColor);
        }
        /// <summary>
        /// Draws outline at the bottom of the container
        /// </summary>
        /// <param name="rect">Area of the container</param>
        /// <param name="outlineColor">Color of the outline</param>
        internal static void DrawContainerBottomOutline(Rect rect, Color outlineColor, int outlineWidth = 1)
        {
            EditorGUI.DrawRect(new Rect(rect.x + 1, rect.yMax - outlineWidth, rect.width - 2, outlineWidth), outlineColor);
        }
        #endregion

        #region Effects
        /// <summary>
        /// Draws default fade line
        /// </summary>
        internal static void DrawFadeLine(Rect rect)
        {
            DrawFadeLine(rect, MicroEditor_Utility.ElementColor);
        }
        /// <summary>
        /// Draws line where edges are of specified color and towards middle color changes based on color brightness
        /// </summary>
        /// <param name="rect">Position for the line, starts at rect.x and rect.y - sidePadding</param>
        /// <param name="fadeColor">Color of the sides and colorBrightness modifies it towards middle</param>
        /// <param name="lineHeight">Thickness of the line</param>
        /// <param name="colorBrightness">Change in color brightness from 1.0f</param>
        /// <param name="gradientSample">How fine gradient is, lower number means less gradual change in color</param>
        /// <param name="sidePadding">In pixels, how much space on each side will be empty</param>
        internal static void DrawFadeLine(Rect rect, Color fadeColor, int lineHeight = 2, float colorBrightness = 0.3f, int gradientSample = 10, int sidePadding = 2)
        {
            // Initialization
            int fadeLinesWidth = Mathf.RoundToInt(rect.width * 0.2f / gradientSample);   // 0.2 is how much (in %) of the line will fading use (per side)
            float colorGradient = colorBrightness / (gradientSample + 1);

            Rect leftRect = new Rect(rect.x + sidePadding - fadeLinesWidth, rect.y, rect.width, rect.height);
            Rect rightRect = new Rect(rect.xMax - 1 - sidePadding, rect.y, rect.width, rect.height);
            float colorMultiplier = 1f;
            for(int i = 0; i < gradientSample; i++)
            {
                colorMultiplier += colorGradient;
                leftRect = new Rect(leftRect.x + fadeLinesWidth, rect.y, fadeLinesWidth, lineHeight);
                rightRect = new Rect(rightRect.x - fadeLinesWidth, rect.y, fadeLinesWidth, lineHeight);
                EditorGUI.DrawRect(leftRect, fadeColor * colorMultiplier);
                EditorGUI.DrawRect(rightRect, fadeColor * colorMultiplier);
            }

            // Base
            Rect baseRect = new Rect(leftRect.x + fadeLinesWidth, rect.y, rightRect.x - leftRect.x - fadeLinesWidth, lineHeight);
            EditorGUI.DrawRect(baseRect, fadeColor * (1 + colorBrightness));
        }
        #endregion
    }
}