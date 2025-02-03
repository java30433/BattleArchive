namespace Microlight.MicroBar
{
#if UNITY_EDITOR
    using Microlight.MicroEditor;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Rendering;

    // ****************************************************************************************************
    // Property drawer for the SimpleMicroBar class
    // ****************************************************************************************************
    [CustomPropertyDrawer(typeof(SimpleMicroBar))]
    public class SimpleMicroBarDrawer : PropertyDrawer
    {
        public static float GetHeight(SerializedProperty property)
        {
            bool isAnimated = property.FindPropertyRelative("_isAnimated").boolValue;

            float totalHeight = 0;

            totalHeight += 20; // Container alignament
            totalHeight += RenderingHeight(property); // Rendering
            totalHeight += SpriteMaskWarningHeight(property);
            totalHeight += CanvasWarningHeight(property);
            totalHeight += FadeLineHeight();
            totalHeight += ColorsHeight(property);
            if(isAnimated)
            {
                totalHeight += FadeLineHeight();
                totalHeight += GhostBarHeight(property); // Ghost bar
                totalHeight += FadeLineHeight();
                totalHeight += DamageAnimationHeight(property);   // Bar damage animation
                totalHeight += FadeLineHeight();
                totalHeight += HealAnimationHeight(property);   // Bar heal animation
            }

            return totalHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = new Rect(position.x - 5, position.y + 10, position.width, position.height - 10);   // Prepare position for the drawing of the property
            EditorGUI.BeginProperty(position, label, property);

            MicroEditor_DrawUtility.DrawContainer(position);
            position = new Rect(position.x + 10, position.y + 5, position.width - 20, position.height + 10);   // Accomodate the container

            bool isAnimated = property.FindPropertyRelative("_isAnimated").boolValue;

            position = DrawRendering(position, property);
            position = DrawSpriteMaskWarning(position, property);
            position = DrawCanvasWarning(position, property);
            position = DrawFadeLine(position);
            position = DrawColors(position, property);
            if(isAnimated)
            {
                position = DrawFadeLine(position);
                position = DrawGhostBar(position, property);
                position = DrawFadeLine(position);
                position = DrawDamageAnimation(position, property);
                position = DrawFadeLine(position);
                position = DrawHealAnimation(position, property);
            }
            EditorGUI.EndProperty();
        }

        #region Building blocks
        Rect DrawRendering(Rect position, SerializedProperty property)
        {
            SerializedProperty renderTypeProperty = property.FindPropertyRelative("_renderType");
            SerializedProperty isAnimatedProperty = property.FindPropertyRelative("_isAnimated");
            SerializedProperty backgroundProperty;
            SerializedProperty primaryBarProperty;
            if(renderTypeProperty.enumValueIndex == (int)RenderType.Image)
            {
                backgroundProperty = property.FindPropertyRelative("_uiBackground");
                primaryBarProperty = property.FindPropertyRelative("_uiPrimaryBar");
            }
            else
            {
                backgroundProperty = property.FindPropertyRelative("_srBackground");
                primaryBarProperty = property.FindPropertyRelative("_srPrimaryBar");

            }

            position = MicroEditor_DrawUtility.DrawBoldLabel(position, "Rendering", true);
            position = MicroEditor_DrawUtility.DrawProperty(position, renderTypeProperty, new GUIContent("Rendering type"));
            position = MicroEditor_DrawUtility.DrawProperty(position, backgroundProperty, new GUIContent("Background"));
            position = MicroEditor_DrawUtility.DrawProperty(position, primaryBarProperty, new GUIContent("Bar"));
            position = MicroEditor_DrawUtility.DrawProperty(position, isAnimatedProperty, new GUIContent("Animated"));
            return position;
        }

        Rect DrawColors(Rect position, SerializedProperty property)
        {
            SerializedProperty adaptiveColorProperty = property.FindPropertyRelative("_adaptiveColor");
            SerializedProperty barPrimaryColorProperty = property.FindPropertyRelative("_barPrimaryColor");
            bool adaptiveColor = adaptiveColorProperty.boolValue;

            position = MicroEditor_DrawUtility.DrawBoldLabel(position, "Colors", true);
            position = MicroEditor_DrawUtility.DrawProperty(position, adaptiveColorProperty, new GUIContent("Adaptive color", "When on, health bar changes color based on % of fill"));
            if(adaptiveColor)
            {
                SerializedProperty barAdaptiveColorProperty = property.FindPropertyRelative("_barAdaptiveColor");

                position = MicroEditor_DrawUtility.DrawProperty(position, barPrimaryColorProperty, new GUIContent("Full color", "Color of the bar when HP is at 100%"));
                position = MicroEditor_DrawUtility.DrawProperty(position, barAdaptiveColorProperty, new GUIContent("Empty color", "Color of the bar when HP is at 0%"));
            }
            else
            {
                position = MicroEditor_DrawUtility.DrawProperty(position, barPrimaryColorProperty, new GUIContent("Bar color"));
            }
            return position;
        }

        Rect DrawGhostBar(Rect position, SerializedProperty property)
        {
            bool isAnimated = property.FindPropertyRelative("_isAnimated").boolValue;
            if(!isAnimated)
            {
                return position;
            }

            SerializedProperty useGhostBarProperty = property.FindPropertyRelative("_useGhostBar");
            bool useGhostBar = useGhostBarProperty.boolValue;

            position = MicroEditor_DrawUtility.DrawBoldLabel(position, "Ghost Bar", true);
            position = MicroEditor_DrawUtility.DrawProperty(position, useGhostBarProperty, new GUIContent("Ghost bar"));
            if(useGhostBar)
            {
                SerializedProperty renderTypeProperty = property.FindPropertyRelative("_renderType");
                SerializedProperty dualGhostBarsProperty = property.FindPropertyRelative("_dualGhostBars");
                SerializedProperty ghostBarDamageColorProperty = property.FindPropertyRelative("_ghostBarDamageColor");
                SerializedProperty ghostBarHealColorProperty = property.FindPropertyRelative("_ghostBarHealColor");
                SerializedProperty ghostBarProperty;
                bool dualGhostBars = dualGhostBarsProperty.boolValue;

                // Dual mode
                position = MicroEditor_DrawUtility.DrawProperty(
                    position,
                    dualGhostBarsProperty,
                    new GUIContent("Dual ghost bars", "Differently colored ghost bars based on healing or damaging effect"));

                // Renderer
                if(renderTypeProperty.enumValueIndex == (int)RenderType.Image)
                {
                    ghostBarProperty = property.FindPropertyRelative("_uiGhostBar");

                }
                else
                {
                    ghostBarProperty = property.FindPropertyRelative("_srGhostBar");

                }
                position = MicroEditor_DrawUtility.DrawProperty(position, ghostBarProperty, new GUIContent("Bar"));

                // Colors
                if(dualGhostBars)
                {
                    position = MicroEditor_DrawUtility.DrawProperty(position, ghostBarDamageColorProperty, new GUIContent("Hurt color", "Ghost bar color when getting hurt"));
                    position = MicroEditor_DrawUtility.DrawProperty(position, ghostBarHealColorProperty, new GUIContent("Heal color", "Ghost bar color when getting healed"));
                }
                else
                {
                    position = MicroEditor_DrawUtility.DrawProperty(position, ghostBarDamageColorProperty, new GUIContent("Color"));
                }
            }
            return position;
        }

        Rect DrawDamageAnimation(Rect position, SerializedProperty property)
        {
            bool isAnimated = property.FindPropertyRelative("_isAnimated").boolValue;
            if(!isAnimated)
            {
                return position;
            }

            SerializedProperty damageAnimProperty = property.FindPropertyRelative("_damageAnim");
            SerializedProperty damageAnimDurationProperty = property.FindPropertyRelative("_damageAnimDuration");
            SerializedProperty damageAnimDelayProperty = property.FindPropertyRelative("_damageAnimDelay");
            SerializedProperty damageFlashColorProperty = property.FindPropertyRelative("_damageFlashColor");
            SerializedProperty damageAnimStrengthProperty = property.FindPropertyRelative("_damageAnimStrength");

            position = MicroEditor_DrawUtility.DrawBoldLabel(position, "Damage animation", true);
            position = MicroEditor_DrawUtility.DrawProperty(position, damageAnimProperty, new GUIContent("Animation"));

            // None doesnt have any of the variables
            if(damageAnimProperty.enumValueIndex == (int)SimpleAnim.Fill)
            {
                position = MicroEditor_DrawUtility.DrawProperty(position, damageAnimDurationProperty, new GUIContent("Duration"));
                position = MicroEditor_DrawUtility.DrawProperty(position, damageAnimDelayProperty, new GUIContent("Delay"));
            }
            else if(damageAnimProperty.enumValueIndex == (int)SimpleAnim.Flash)
            {
                position = MicroEditor_DrawUtility.DrawProperty(position, damageAnimDurationProperty, new GUIContent("Duration"));
                position = MicroEditor_DrawUtility.DrawProperty(position, damageAnimDelayProperty, new GUIContent("Delay"));
                position = MicroEditor_DrawUtility.DrawProperty(position, damageFlashColorProperty, new GUIContent("Flash color"));
            }
            else if(damageAnimProperty.enumValueIndex == (int)SimpleAnim.Shake)
            {
                position = MicroEditor_DrawUtility.DrawProperty(position, damageAnimDurationProperty, new GUIContent("Duration"));
                position = MicroEditor_DrawUtility.DrawProperty(position, damageAnimStrengthProperty, new GUIContent("Shake strength"));
            }
            return position;
        }

        Rect DrawHealAnimation(Rect position, SerializedProperty property)
        {
            bool isAnimated = property.FindPropertyRelative("_isAnimated").boolValue;
            if(!isAnimated)
            {
                return position;
            }

            SerializedProperty healAnimProperty = property.FindPropertyRelative("_healAnim");
            SerializedProperty healAnimDurationProperty = property.FindPropertyRelative("_healAnimDuration");
            SerializedProperty healAnimDelayProperty = property.FindPropertyRelative("_healAnimDelay");
            SerializedProperty healFlashColorProperty = property.FindPropertyRelative("_healFlashColor");
            SerializedProperty healAnimStrengthProperty = property.FindPropertyRelative("_healAnimStrength");

            position = MicroEditor_DrawUtility.DrawBoldLabel(position, "Heal animation", true);
            position = MicroEditor_DrawUtility.DrawProperty(position, healAnimProperty, new GUIContent("Animation"));

            // None doesnt have any of the variables
            if(healAnimProperty.enumValueIndex == (int)SimpleAnim.Fill)
            {
                position = MicroEditor_DrawUtility.DrawProperty(position, healAnimDurationProperty, new GUIContent("Duration"));
                position = MicroEditor_DrawUtility.DrawProperty(position, healAnimDelayProperty, new GUIContent("Delay"));
            }
            else if(healAnimProperty.enumValueIndex == (int)SimpleAnim.Flash)
            {
                position = MicroEditor_DrawUtility.DrawProperty(position, healAnimDurationProperty, new GUIContent("Duration"));
                position = MicroEditor_DrawUtility.DrawProperty(position, healAnimDelayProperty, new GUIContent("Delay"));
                position = MicroEditor_DrawUtility.DrawProperty(position, healFlashColorProperty, new GUIContent("Flash color"));
            }
            else if(healAnimProperty.enumValueIndex == (int)SimpleAnim.Shake)
            {
                position = MicroEditor_DrawUtility.DrawProperty(position, healAnimDurationProperty, new GUIContent("Duration"));
                position = MicroEditor_DrawUtility.DrawProperty(position, healAnimStrengthProperty, new GUIContent("Shake strength"));
            }
            return position;
        }

        Rect DrawFadeLine(Rect position)
        {
            Rect fadeRect = new Rect(position.x, position.y + 5, position.width, position.height);
            MicroEditor_DrawUtility.DrawFadeLine(fadeRect);
            return new Rect(position.x, position.y + 13, position.width, position.height);
        }

        Rect DrawCanvasWarning(Rect position, SerializedProperty property)
        {
            bool hasCanvas = HasCanvasParent(property);
            bool needsCanvas = property.FindPropertyRelative("_renderType").enumValueIndex == (int)RenderType.Image;

            string message = "";
            if(!hasCanvas && needsCanvas)
            {
                message = "Rendering is set to 'Image' but there is no 'Canvas' parent.";
            }
            else if(hasCanvas && !needsCanvas)
            {
                message = "Rendering is set to 'Sprite' but there is 'Canvas' parent.";
            }

            if(message == "")
            {
                return position;
            }

            // Draw warning
            Rect elementRect = new Rect(
                position.x,
                position.y + MicroEditor_Utility.VerticalSpacing,
                position.width,
                MicroEditor_Utility.LineHeight * 2);
            EditorGUI.HelpBox(elementRect, message, MessageType.Warning);
            return new Rect(
                position.x,
                position.y + elementRect.height + MicroEditor_Utility.VerticalSpacing * 3,
                position.width,
                position.height);
        }

        Rect DrawSpriteMaskWarning(Rect position, SerializedProperty property)
        {
            SerializedProperty renderTypeProperty = property.FindPropertyRelative("_renderType");
            bool isAnimated = property.FindPropertyRelative("_isAnimated").boolValue;
            bool useGhostBar = property.FindPropertyRelative("_useGhostBar").boolValue;

            if((RenderType)renderTypeProperty.enumValueIndex == RenderType.Image)
            {
                return position;   // If its image, we don't need masks
            }

            // Primary bar
            SerializedProperty primaryBarProperty = property.FindPropertyRelative("_srPrimaryBar");
            if(primaryBarProperty.objectReferenceValue == null)
            {
                return position;
            }

            SpriteRenderer spriteRenderer = (SpriteRenderer)primaryBarProperty.objectReferenceValue;
            if(spriteRenderer == null)
            {
                return position;   // This should never happen but okay
            }

            SortingGroup spriteGroup = spriteRenderer.GetComponent<SortingGroup>();
            if(spriteGroup == null)
            {
                position = DrawSortingGroupWarning(spriteRenderer.gameObject.name);
            }

            SpriteMask spriteMask = spriteRenderer.GetComponentInChildren<SpriteMask>();
            if(spriteMask == null)
            {
                position = DrawSpriteMaskWarning(spriteRenderer.gameObject.name);
            }

            // Ghost bar
            if(isAnimated && useGhostBar)
            {
                SerializedProperty ghostBarProperty = property.FindPropertyRelative("_srGhostBar");
                if(ghostBarProperty.objectReferenceValue == null)
                {
                    return position;
                }

                SpriteRenderer ghostSpriteRenderer = (SpriteRenderer)ghostBarProperty.objectReferenceValue;
                if(ghostSpriteRenderer == null)
                {
                    return position;   // This should never happen but okay
                }

                SortingGroup ghostSpriteGroup = ghostSpriteRenderer.GetComponent<SortingGroup>();
                if(ghostSpriteGroup == null)
                {
                    position = DrawSortingGroupWarning(ghostSpriteRenderer.gameObject.name);
                }

                SpriteMask ghostSpriteMask = ghostSpriteRenderer.GetComponentInChildren<SpriteMask>();
                if(ghostSpriteMask == null)
                {
                    position = DrawSpriteMaskWarning(ghostSpriteRenderer.gameObject.name);
                }
            }

            return position;

            Rect DrawSortingGroupWarning(string gameObjectName)
            {
                Rect elementRect = new Rect(
                        position.x,
                        position.y + MicroEditor_Utility.VerticalSpacing,
                        position.width,
                        MicroEditor_Utility.LineHeight * 3);
                EditorGUI.HelpBox(elementRect,
                    $"'Sprite Renderer' bar should have 'SortingGroup' component, '{gameObjectName}' doesn't have it." +
                    "Check documentation under 'Render type' for more info",
                    MessageType.Warning);

                return new Rect(
                    position.x,
                    position.y + elementRect.height + MicroEditor_Utility.VerticalSpacing * 3,
                    position.width,
                    position.height);
            }
            Rect DrawSpriteMaskWarning(string gameObjectName)
            {
                Rect elementRect = new Rect(
                        position.x,
                        position.y + MicroEditor_Utility.VerticalSpacing,
                        position.width,
                        MicroEditor_Utility.LineHeight * 3);
                EditorGUI.HelpBox(elementRect,
                    $"'Sprite Renderer' bar should have 'SpriteMask' child, '{gameObjectName}' doesn't have it." +
                    "Check documentation under 'Render type' for more info",
                    MessageType.Warning);

                return new Rect(
                    position.x,
                    position.y + elementRect.height + MicroEditor_Utility.VerticalSpacing * 3,
                    position.width,
                    position.height);
            }
        }
        #endregion

        #region Heights

        static float RenderingHeight(SerializedProperty property)
        {
            return MicroEditor_Utility.DefaultFieldHeight * 5;
        }

        static float ColorsHeight(SerializedProperty property)
        {
            bool adaptiveColor = property.FindPropertyRelative("_adaptiveColor").boolValue;

            float height = MicroEditor_Utility.LineHeight * 3 + MicroEditor_Utility.VerticalSpacing * 3;   // Header label, adaptive color and primary color
            if(adaptiveColor)
            {
                height += MicroEditor_Utility.LineHeight + MicroEditor_Utility.VerticalSpacing;   // Secondary color
            }
            return height;
        }

        static float GhostBarHeight(SerializedProperty property)
        {
            bool isAnimated = property.FindPropertyRelative("_isAnimated").boolValue;
            if(!isAnimated)
            {
                return 0f;
            }

            bool useGhostBar = property.FindPropertyRelative("_useGhostBar").boolValue;

            float height = MicroEditor_Utility.DefaultFieldHeight;   // Ghost bar label
            height += MicroEditor_Utility.DefaultFieldHeight;   // Use ghost bar
            if(useGhostBar)
            {
                bool dualGhostBars = property.FindPropertyRelative("_dualGhostBars").boolValue;
                height += MicroEditor_Utility.DefaultFieldHeight;   // Dual mode
                height += MicroEditor_Utility.DefaultFieldHeight;   // Renderer

                if(dualGhostBars)
                {
                    height += MicroEditor_Utility.DefaultFieldHeight * 2;   // Dual mode, dual line
                }
                else
                {
                    height += MicroEditor_Utility.DefaultFieldHeight;   // Single color
                }
            }
            return height;
        }

        static float DamageAnimationHeight(SerializedProperty property)
        {
            bool isAnimated = property.FindPropertyRelative("_isAnimated").boolValue;
            if(!isAnimated)
            {
                return 0f;
            }

            SerializedProperty damageAnimProperty = property.FindPropertyRelative("_damageAnim");

            float height = MicroEditor_Utility.DefaultFieldHeight * 2; // Label and animation

            if(damageAnimProperty.enumValueIndex == (int)SimpleAnim.Fill)
            {
                height += MicroEditor_Utility.DefaultFieldHeight * 2; // Duration and delay
            }
            else if(damageAnimProperty.enumValueIndex == (int)SimpleAnim.Flash)
            {
                height += MicroEditor_Utility.DefaultFieldHeight * 3; // Duration, delay and flash color
            }
            else if(damageAnimProperty.enumValueIndex == (int)SimpleAnim.Shake)
            {
                height += MicroEditor_Utility.DefaultFieldHeight * 2; // Duration and shake strength
            }
            return height;
        }

        static float HealAnimationHeight(SerializedProperty property)
        {
            bool isAnimated = property.FindPropertyRelative("_isAnimated").boolValue;
            if(!isAnimated)
            {
                return 0f;
            }

            SerializedProperty healAnimProperty = property.FindPropertyRelative("_healAnim");

            float height = MicroEditor_Utility.DefaultFieldHeight * 2; // Label and animation

            if(healAnimProperty.enumValueIndex == (int)SimpleAnim.Fill)
            {
                height += MicroEditor_Utility.DefaultFieldHeight * 2; // Duration and delay
            }
            else if(healAnimProperty.enumValueIndex == (int)SimpleAnim.Flash)
            {
                height += MicroEditor_Utility.DefaultFieldHeight * 3; // Duration, delay and flash color
            }
            else if(healAnimProperty.enumValueIndex == (int)SimpleAnim.Shake)
            {
                height += MicroEditor_Utility.DefaultFieldHeight * 2; // Duration and shake strength
            }
            return height;
        }

        static float FadeLineHeight()
        {
            return 13;
        }

        static float CanvasWarningHeight(SerializedProperty property)
        {
            bool hasCanvas = HasCanvasParent(property);
            bool needsCanvas = property.FindPropertyRelative("_renderType").enumValueIndex == (int)RenderType.Image;

            if(!hasCanvas && needsCanvas)
            {
                return MicroEditor_Utility.LineHeight * 2 + MicroEditor_Utility.VerticalSpacing * 3;
            }
            else if(hasCanvas && !needsCanvas)
            {
                return MicroEditor_Utility.LineHeight * 2 + MicroEditor_Utility.VerticalSpacing * 3;
            }

            return 0f;
        }

        static float SpriteMaskWarningHeight(SerializedProperty property)
        {
            SerializedProperty renderTypeProperty = property.FindPropertyRelative("_renderType");
            bool isAnimated = property.FindPropertyRelative("_isAnimated").boolValue;
            bool useGhostBar = property.FindPropertyRelative("_useGhostBar").boolValue;
            float height = 0;

            if((RenderType)renderTypeProperty.enumValueIndex == RenderType.Image)
            {
                return height;   // If its image, we don't need masks
            }

            // Primary bar
            SerializedProperty primaryBarProperty = property.FindPropertyRelative("_srPrimaryBar");
            if(primaryBarProperty.objectReferenceValue == null)
            {
                return height;
            }

            SpriteRenderer spriteRenderer = (SpriteRenderer)primaryBarProperty.objectReferenceValue;
            if(spriteRenderer == null)
            {
                return height;   // This should never happen but okay
            }

            SortingGroup spriteGroup = spriteRenderer.GetComponent<SortingGroup>();
            if(spriteGroup == null)
            {
                height += ReturnSpace();
            }

            SpriteMask spriteMask = spriteRenderer.GetComponentInChildren<SpriteMask>();
            if(spriteMask == null)
            {
                height += ReturnSpace();
            }

            // Ghost bar
            if(isAnimated && useGhostBar)
            {
                SerializedProperty ghostBarProperty = property.FindPropertyRelative("_srGhostBar");
                if(ghostBarProperty.objectReferenceValue == null)
                {
                    return height;
                }

                SpriteRenderer ghostSpriteRenderer = (SpriteRenderer)ghostBarProperty.objectReferenceValue;
                if(ghostSpriteRenderer == null)
                {
                    return height;   // This should never happen but okay
                }

                SortingGroup ghostSpriteGroup = ghostSpriteRenderer.GetComponent<SortingGroup>();
                if(ghostSpriteGroup == null)
                {
                    height += ReturnSpace();
                }

                SpriteMask ghostSpriteMask = ghostSpriteRenderer.GetComponentInChildren<SpriteMask>();
                if(ghostSpriteMask == null)
                {
                    height += ReturnSpace();
                }
            }

            return height;

            float ReturnSpace() => MicroEditor_Utility.LineHeight * 3 + MicroEditor_Utility.VerticalSpacing * 3;
        }
        #endregion

        #region Utilities
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetHeight(property);
        }

        // Checks if transform has a parent with canvas component
        static bool HasCanvasParent(SerializedProperty property)
        {
            return Selection.activeGameObject && Selection.activeGameObject.GetComponentInParent<Canvas>();
        }
        #endregion
    }
#endif
}