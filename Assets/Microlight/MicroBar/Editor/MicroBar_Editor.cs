namespace Microlight.MicroBar
{
#if UNITY_EDITOR
    using Microlight.MicroEditor;
    using UnityEditor;
    using UnityEngine;

    // ****************************************************************************************************
    // Custom editor for the MicroBars
    // ****************************************************************************************************
    [CustomEditor(typeof(MicroBar))]
    public class MicroBar_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Store serialized properties
            SerializedProperty editorModeProperty = serializedObject.FindProperty("editorMode");
            SerializedProperty animationsProperty = serializedObject.FindProperty("animations");
            SerializedProperty simpleBarProperty = serializedObject.FindProperty("simpleBar");

            // Mode header label
            GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
            centeredStyle.alignment = TextAnchor.MiddleCenter;
            centeredStyle.fontStyle = FontStyle.Bold;
            centeredStyle.fontSize = 14;
            float modeHeaderWidth = 200;
            Rect modeHeaderRect = new Rect(EditorGUIUtility.currentViewWidth / 2 - 200 / 2,
                2,
                modeHeaderWidth,
                MicroEditor_Utility.LineHeight);
            EditorGUI.LabelField(modeHeaderRect, "Mode", centeredStyle);
            EditorGUILayout.Space(MicroEditor_Utility.LineHeight);

            // Mode selection buttons init
            float modeButtonWidth = 120;
            float modeButtonX = 0;
            float modeButtonY = GUILayoutUtility.GetLastRect().yMax + MicroEditor_Utility.VerticalSpacing;
            Rect simpleModeButtonRect;
            Rect advancedModeButtonRect;
            if(EditorGUIUtility.currentViewWidth > MicroEditor_Utility.SingleRowThreshold)
            {
                // 2 buttons, one next to another
                modeButtonX = EditorGUIUtility.currentViewWidth / 2 - modeButtonWidth;
                simpleModeButtonRect = new Rect(modeButtonX, modeButtonY, modeButtonWidth, MicroEditor_Utility.HeaderLineHeight);
                modeButtonX = EditorGUIUtility.currentViewWidth / 2;
                advancedModeButtonRect = new Rect(modeButtonX, modeButtonY, modeButtonWidth, MicroEditor_Utility.HeaderLineHeight);
            }
            else
            {
                // 2 buttons, one below another
                modeButtonX = EditorGUIUtility.currentViewWidth / 2 - modeButtonWidth / 2;
                simpleModeButtonRect = new Rect(modeButtonX, modeButtonY, modeButtonWidth, MicroEditor_Utility.HeaderLineHeight);
                advancedModeButtonRect = new Rect(modeButtonX, modeButtonY + MicroEditor_Utility.HeaderLineHeight + MicroEditor_Utility.VerticalSpacing, modeButtonWidth, MicroEditor_Utility.HeaderLineHeight);
            }
            centeredStyle = new GUIStyle(GUI.skin.label);
            centeredStyle.alignment = TextAnchor.MiddleCenter;

            // Simple mode button
            if((MicroBarEditorMode)editorModeProperty.enumValueIndex == MicroBarEditorMode.Simple)
            {
                MicroEditor_DrawUtility.DrawContainer(simpleModeButtonRect);
                EditorGUI.LabelField(simpleModeButtonRect, "Simple", centeredStyle);
            }
            else
            {
                if(GUI.Button(simpleModeButtonRect, "Simple"))
                {
                    ChangeMode(editorModeProperty, MicroBarEditorMode.Simple);
                }
            }

            // Advanced mode button
            if((MicroBarEditorMode)editorModeProperty.enumValueIndex == MicroBarEditorMode.Advanced)
            {
                MicroEditor_DrawUtility.DrawContainer(advancedModeButtonRect);
                EditorGUI.LabelField(advancedModeButtonRect, "Advanced", centeredStyle);
            }
            else
            {
                if(GUI.Button(advancedModeButtonRect, "Advanced"))
                {
                    ChangeMode(editorModeProperty, MicroBarEditorMode.Advanced);
                }
            }

            // Mode buttons spacing
            if(EditorGUIUtility.currentViewWidth > MicroEditor_Utility.SingleRowThreshold)
            {
                // 2 buttons, one next to another
                EditorGUILayout.Space(MicroEditor_Utility.HeaderLineHeight + MicroEditor_Utility.VerticalSpacing);
            }
            else
            {
                // 2 buttons, one below another
                EditorGUILayout.Space(MicroEditor_Utility.HeaderLineHeight * 2 + MicroEditor_Utility.VerticalSpacing * 2);
            }

            // If mode is set to advanced we draw the animations
            if((MicroBarEditorMode)editorModeProperty.enumValueIndex == MicroBarEditorMode.Advanced)
            {
                // Animations
                EditorGUILayout.LabelField("Animations", EditorStyles.boldLabel);

                int arraySize = animationsProperty.arraySize;
                for(int i = 0; i < arraySize; i++)
                {
                    SerializedProperty elementProperty = animationsProperty.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(elementProperty, true);

                    // Draw the remove button
                    GUI.backgroundColor = MicroBar_Theme.RemoveButtonColor;
                    if(GUI.Button(RemoveButtonRect(GUILayoutUtility.GetLastRect()), "-"))
                    {
                        RemoveItem(animationsProperty, i);
                        arraySize = animationsProperty.arraySize;
                        i--;
                    }
                    GUI.backgroundColor = Color.white;
                }

                // Add new animation
                float buttonWidth = EditorGUIUtility.currentViewWidth - 20;
                float buttonX = GUILayoutUtility.GetLastRect().x - 8;
                Rect buttonRect = new Rect(buttonX, GUILayoutUtility.GetLastRect().yMax + MicroEditor_Utility.VerticalSpacing, buttonWidth, MicroEditor_Utility.HeaderLineHeight);

                if(GUI.Button(buttonRect, "Add Animation"))
                {
                    AddNewItemToList(animationsProperty);
                }
                EditorGUILayout.Space(MicroEditor_Utility.HeaderLineHeight + MicroEditor_Utility.VerticalSpacing);
            }
            else   // Else we want to draw simple mode
            {
                EditorGUILayout.PropertyField(simpleBarProperty);
            }

            // To update hovering over elements instantly instead of the delay
            // And it works quite good, very precise
            // DISABLED BECAUSE OF PERFORMANCE
            //Rect checkRect = GUILayoutUtility.GetLastRect();
            //checkRect.height = checkRect.y + checkRect.height;
            //checkRect.y = checkRect.x;
            //if(checkRect.Contains(Event.current.mousePosition)) {
            //    Repaint();
            //    //EditorUtility.SetDirty(target);
            //}

            serializedObject.ApplyModifiedProperties();   // Apply changes
        }

        #region List control
        static void AddNewItemToList(SerializedProperty animationsProperty)
        {
            // Update the serialized property to reflect the change
            animationsProperty.arraySize++;
            animationsProperty.GetArrayElementAtIndex(animationsProperty.arraySize - 1).isExpanded = true;
            ResetAnimationState(animationsProperty.GetArrayElementAtIndex(animationsProperty.arraySize - 1));
            animationsProperty.serializedObject.ApplyModifiedProperties();
        }

        static void ChangeMode(SerializedProperty editorModeProperty, MicroBarEditorMode newEditorMode)
        {
            if(editorModeProperty.enumValueIndex != (int)newEditorMode)
            {
                editorModeProperty.enumValueIndex = (int)newEditorMode;
                editorModeProperty.serializedObject.ApplyModifiedProperties();
            }
        }

        static void RemoveItem(SerializedProperty animationsProperty, int index)
        {
            animationsProperty.DeleteArrayElementAtIndex(index);
            animationsProperty.serializedObject.ApplyModifiedProperties();
        }
        static void ResetAnimationState(SerializedProperty animationProperty)
        {
            animationProperty.FindPropertyRelative("animationType").enumValueIndex = (int)UpdateAnim.Damage;
            animationProperty.FindPropertyRelative("targetImage").objectReferenceValue = null;
            animationProperty.FindPropertyRelative("targetSprite").objectReferenceValue = null;
            animationProperty.FindPropertyRelative("notBar").boolValue = false;

            // Reset commands
            SerializedProperty commandsProperty = animationProperty.FindPropertyRelative("commands");
            commandsProperty.arraySize = 0;
        }
        #endregion

        #region Drawing utilites
        static Rect RemoveButtonRect(Rect position)
        {
            return new Rect(
                position.xMax - 6 - MicroEditor_Utility.HeaderLineHeight,
                position.y,
                MicroEditor_Utility.HeaderLineHeight,
                MicroEditor_Utility.HeaderLineHeight);
        }
        #endregion
    }
#endif
}