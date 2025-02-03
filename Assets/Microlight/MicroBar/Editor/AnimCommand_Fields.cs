using Microlight.MicroEditor;
using UnityEditor;
using UnityEngine;

namespace Microlight.MicroBar {
#if UNITY_EDITOR
    // ****************************************************************************************************
    // Stores instructions on how to draw the AnimCommand drawer
    // ****************************************************************************************************
    internal class AnimCommand_Fields {
        internal bool header = false;
        internal bool execution = false;
        internal bool effect = false;
        internal bool execAndEffect = false;
        internal bool duration = false;
        internal bool delay = false;
        internal bool durationAndDelay = false;

        // Values
        internal bool valueMode = false;
        internal bool intValue = false;
        internal string intLabel = "";
        internal string intTooltip = "";
        internal bool floatValue = false;
        internal string floatLabel = "";
        internal string floatTooltip = "";
        internal bool boolValue = false;
        internal string boolLabel = "";
        internal string boolTooltip = "";
        internal bool vector2Value = false;
        internal bool vector2Value2Row = false;
        internal string vector2Label = "";
        internal string vector2Tooltip = "";
        internal bool vector3Value = false;
        internal bool vector3Value2Row = false;
        internal string vector3Label = "";
        internal string vector3Tooltip = "";
        internal bool colorValue = false;
        internal string colorLabel = "";
        internal string colorTooltip = "";
        internal bool percentValue = false;
        internal string percentLabel = "";
        internal string percentTooltip = "";

        // Additional settings
        internal bool frequencyValue = false;
        internal string frequencyLabel = "";
        internal string frequencyTooltip = "";
        internal bool ease = false;
        internal bool animAxis = false;
        internal bool transformProperty = false;
        internal bool valuesFadeLine = false;
        internal bool extraFadeLine = false;

        internal AnimCommand_Fields(SerializedProperty property) {
            header = true;
            execution = true;
            duration = true;

            #region Execution
            if(execution) {
                SerializedProperty executionProperty = property.FindPropertyRelative("execution");
                switch((AnimExecution)executionProperty.enumValueIndex) {
                    case AnimExecution.Sequence:
                        effect = true;
                        delay = true;
                        valuesFadeLine = true;
                        extraFadeLine = true;
                        break;
                    case AnimExecution.Parallel:
                        effect = true;
                        delay = true;
                        valuesFadeLine = true;
                        extraFadeLine = true;
                        break;
                    case AnimExecution.Wait:
                        return;
                    default:
#if UNITY_EDITOR
                        Debug.LogWarning($"MicroBar: Unknown execution value: {(AnimExecution)executionProperty.enumValueIndex}");
#endif
                        return;
                }
            }
            #endregion

            #region Effects
            if(effect) {
                delay = true;
                ease = true;
                valueMode = true;

                SerializedProperty effectProperty = property.FindPropertyRelative("effect");
                SerializedProperty animAxisProperty = property.FindPropertyRelative("animAxis");
                SerializedProperty transformPropertyProperty = property.FindPropertyRelative("transformProperty");
                SerializedProperty boolValueProperty = property.FindPropertyRelative("boolValue");
                SerializedProperty valueModeProperty = property.FindPropertyRelative("valueMode");

                bool startingOrDefault = (ValueMode)valueModeProperty.enumValueIndex == ValueMode.StartingValue || (ValueMode)valueModeProperty.enumValueIndex == ValueMode.DefaultValue;

                switch((AnimEffect)effectProperty.enumValueIndex) {
                    case AnimEffect.Color:
                        if(!startingOrDefault) {
                            colorValue = true;
                            colorLabel = "Color";
                        }
                        break;
                    case AnimEffect.Fade:
                        if((ValueMode)valueModeProperty.enumValueIndex == ValueMode.Absolute) {
                            percentValue = true;
                            percentLabel = "Fade";
                        }
                        else if(!startingOrDefault) {
                            floatValue = true;
                            floatLabel = "Fade";
                        }
                        break;
                    case AnimEffect.Fill:
                        boolValue = true;
                        boolLabel = "Custom";
                        boolTooltip = "If turned off, image will animate to the current health value\nTurning custom on, let's user choose fill amount manually";
                        valueMode = false;
                        if(boolValueProperty.boolValue) {
                            valueMode = true;
                            if((ValueMode)valueModeProperty.enumValueIndex == ValueMode.Absolute) {
                                percentValue = true;
                                percentLabel = "Fill";
                            }
                            else if(!startingOrDefault) {
                                floatValue = true;
                                floatLabel = "Fill";
                            }
                        }
                        break;
                    case AnimEffect.Move:
                        animAxis = true;
                        if(!startingOrDefault) {
                            if((AnimAxis)animAxisProperty.enumValueIndex == AnimAxis.XY) {
                                vector2Value = true;
                                vector2Label = "Axis";
                            }
                            else if((AnimAxis)animAxisProperty.enumValueIndex == AnimAxis.X) {
                                floatValue = true;
                                floatLabel = "X";
                            }
                            else if((AnimAxis)animAxisProperty.enumValueIndex == AnimAxis.Y) {
                                floatValue = true;
                                floatLabel = "Y";
                            }
                            else {
                                floatValue = true;
                                floatLabel = "XY";
                            }
                        }
                        break;
                    case AnimEffect.Rotate:
                        if(!startingOrDefault) {
                            floatValue = true;
                            floatLabel = "Angle";
                            floatTooltip = "Animate to the desired angle on Z axis (degrees)";
                        }
                        break;
                    case AnimEffect.Scale:
                        animAxis = true;
                        if(!startingOrDefault) {
                            if((AnimAxis)animAxisProperty.enumValueIndex == AnimAxis.XY) {
                                vector2Value = true;
                                vector2Label = "Axis";
                            }
                            else if((AnimAxis)animAxisProperty.enumValueIndex == AnimAxis.X) {
                                floatValue = true;
                                floatLabel = "X";
                            }
                            else if((AnimAxis)animAxisProperty.enumValueIndex == AnimAxis.Y) {
                                floatValue = true;
                                floatLabel = "Y";
                            }
                            else {
                                floatValue = true;
                                floatLabel = "XY";
                            }
                        }
                        break;
                    case AnimEffect.Punch:
                        transformProperty = true;
                        valueMode = false;
                        if((TransformProperties)transformPropertyProperty.enumValueIndex == TransformProperties.Rotation) {
                            floatValue = true;
                            floatLabel = "Strength";
                        }
                        else {
                            vector2Value = true;
                            vector2Label = "Strength";
                        }
                        frequencyValue = true;
                        frequencyLabel = "Freq.";
                        frequencyTooltip = "Frequency of animation, bigger number means object will be faster (default = 10)";
                        percentValue = true;
                        percentLabel = "Elasticity";
                        percentTooltip = "How much inertia has effect on object (0-1)";
                        break;
                    case AnimEffect.Shake:
                        valueMode = false;
                        transformProperty = true;
                        floatValue = true;
                        floatLabel = "Strength";
                        floatTooltip = "Strength of the shake (default = 1, 100 for anchor transform)";
                        frequencyValue = true;
                        frequencyLabel = "Freq.";
                        frequencyTooltip = "Frequency of animation, bigger number means object will be faster (default = 10)";
                        break;
                    case AnimEffect.AnchorMove:
                        animAxis = true;
                        if(!startingOrDefault) {
                            if((AnimAxis)animAxisProperty.enumValueIndex == AnimAxis.XY) {
                                vector2Value = true;
                                vector2Label = "Axis";
                            }
                            else if((AnimAxis)animAxisProperty.enumValueIndex == AnimAxis.X) {
                                floatValue = true;
                                floatLabel = "X";
                            }
                            else if((AnimAxis)animAxisProperty.enumValueIndex == AnimAxis.Y) {
                                floatValue = true;
                                floatLabel = "Y";
                            }
                            else {
                                floatValue = true;
                                floatLabel = "XY";
                            }
                        }
                        break;
                    default:
                        Debug.LogWarning($"MicroBar: Unknown effect value: {(AnimEffect)effectProperty.enumValueIndex}");
                        return;
                }
            }
            #endregion

            #region Combine fields
            if(EditorGUIUtility.currentViewWidth > MicroEditor_Utility.SingleRowThreshold && execution && effect) {
                execution = false;
                effect = false;
                execAndEffect = true;
            }
            if(EditorGUIUtility.currentViewWidth > MicroEditor_Utility.SingleRowThreshold && duration && delay) {
                duration = false;
                delay = false;
                durationAndDelay = true;
            }
            if(EditorGUIUtility.currentViewWidth <= MicroEditor_Utility.UnityTwoRowsThreshold && vector2Value) {
                vector2Value = false;
                vector2Value2Row = true;
            }
            if(EditorGUIUtility.currentViewWidth <= MicroEditor_Utility.UnityTwoRowsThreshold && vector3Value) {
                vector3Value = false;
                vector3Value2Row = true;
            }
            #endregion
        }
    }
#endif
}