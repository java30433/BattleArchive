using DG.Tweening;
using UnityEngine;

namespace Microlight.MicroBar
{
    // ****************************************************************************************************
    // Creates DOTween sequence from AnimationCommands in MicroBarAnimation
    // ****************************************************************************************************

    internal static class AnimBuilder
    {
        /// <summary>
        /// Builds whole sequence for the animation from the list of commands
        /// </summary>
        /// <param name="animInfo">Data about animation</param>
        /// <returns>Sequence for the animation</returns>
        internal static Sequence BuildAnimation(AnimationInfo animInfo)
        {
            Sequence sequence = DOTween.Sequence();

            foreach(AnimCommand x in animInfo.Commands)
            {
                InterpretCommand(sequence, x, animInfo);
            }

            return sequence;
        }

        /// <summary>
        /// Interprets how tween should be added to the sequence (appended or joined etc)
        /// Then calls for creation of the tween
        /// </summary>
        /// <param name="sequence">Sequence to which tween will be appended</param>
        /// <param name="command">Comand that should create tween</param>
        /// <param name="animInfo">Data about animation</param>
        static void InterpretCommand(Sequence sequence, AnimCommand command, AnimationInfo animInfo)
        {
            Tween tween;
            switch(command.Execution)
            {
                case AnimExecution.Sequence:
                    tween = InterpretEffect(command, animInfo);
                    if(tween != null)
                    {
                        sequence.Append(tween);
                    }
                    break;
                case AnimExecution.Parallel:
                    tween = InterpretEffect(command, animInfo);
                    if(tween != null)
                    {
                        sequence.Join(tween);
                    }
                    break;
                case AnimExecution.Wait:
                    sequence.AppendInterval(command.Duration);
                    break;
                default:
                    Debug.LogError($"[MicroBar] Unknown 'Execution': {command.Execution}");
                    break;
            }
        }

        /// <summary>
        /// Creates tween specified in command
        /// </summary>
        /// <param name="command">Command from which tween should be created</param>
        /// <param name="animInfo">Data about animation</param>
        /// <returns>Created tween for the sequence</returns>
        static Tween InterpretEffect(AnimCommand command, AnimationInfo animInfo)
        {
            Tween tween = null;

            // Values to use in switches
            float newFloat;
            float newFloatX;
            float newFloatY;
            float floatZ;
            float baseFloatValue;
            float defaultFloatValue;
            Vector2 baseVector2Value;
            Vector2 defaultVector2Value;
            Vector2 newVector2;
            Color newColor;

            switch(command.Effect)
            {
                case AnimEffect.Color:
                    if(animInfo.IsImage)
                    {
                        newColor = InterpretValueMode(animInfo.TargetImage.color, animInfo.Animation.DefaultValues.Color, command);
                        tween = animInfo.TargetImage.DOColor(newColor, command.Duration);
                    }
                    else
                    {
                        newColor = InterpretValueMode(animInfo.TargetSprite.color, animInfo.Animation.DefaultValues.Color, command);
                        tween = animInfo.TargetSprite.DOColor(newColor, command.Duration);
                    }
                    break;
                case AnimEffect.Fade:
                    if(animInfo.IsImage)
                    {
                        baseFloatValue = animInfo.TargetImage.color.a;
                    }
                    else
                    {
                        baseFloatValue = animInfo.TargetSprite.color.a;
                    }

                    defaultFloatValue = animInfo.Animation.DefaultValues.Fade;
                    if(command.ValueMode == ValueMode.Additive || command.ValueMode == ValueMode.Multiplicative)
                    {
                        newFloat = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                    }
                    else
                    {
                        newFloat = InterpretValueMode(baseFloatValue, defaultFloatValue, command, true);
                    }
                    if(animInfo.IsImage)
                    {
                        tween = animInfo.TargetImage.DOFade(newFloat, command.Duration);
                    }
                    else
                    {
                        tween = animInfo.TargetSprite.DOFade(newFloat, command.Duration);
                    }
                    break;
                case AnimEffect.Fill:
                    if(!animInfo.IsImage && animInfo.Animation.NotBar)
                    {
                        break;   // Fill is NOT supported for the sprite bars which are not considered bars
                    }

                    if(command.BoolValue)
                    {   // If fill will be custom value or fill to the bar value
                        if(animInfo.IsImage)
                        {
                            baseFloatValue = animInfo.TargetImage.fillAmount;
                        }
                        else
                        {
                            baseFloatValue = animInfo.TargetSpriteMask.localScale.x;
                        }
                        defaultFloatValue = animInfo.Animation.DefaultValues.Fill;
                        if(command.ValueMode == ValueMode.Additive || command.ValueMode == ValueMode.Multiplicative)
                        {
                            newFloat = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                        }
                        else
                        {
                            newFloat = InterpretValueMode(baseFloatValue, defaultFloatValue, command, true);
                        }
                        if(animInfo.IsImage)
                        {
                            tween = animInfo.TargetImage.DOFillAmount(newFloat, command.Duration);
                        }
                        else
                        {
                            tween = animInfo.TargetSpriteMask.DOScaleX(newFloat, command.Duration);
                        }
                    }
                    else
                    {
                        if(animInfo.IsImage)
                        {
                            tween = animInfo.TargetImage.DOFillAmount(animInfo.Bar.HPPercent, command.Duration);
                        }
                        else
                        {
                            tween = animInfo.TargetSpriteMask.DOScaleX(animInfo.Bar.HPPercent, command.Duration);
                        }
                    }
                    break;
                case AnimEffect.Move:
                    if(animInfo.IsImage)
                    {
                        floatZ = animInfo.TargetImage.rectTransform.localPosition.z;
                    }
                    else
                    {
                        floatZ = animInfo.TargetSprite.transform.localPosition.z;
                    }
                    switch(command.AnimAxis)
                    {
                        case AnimAxis.Uniform:
                            if(animInfo.IsImage)
                            {
                                baseFloatValue = animInfo.TargetImage.rectTransform.localPosition.x;
                                defaultFloatValue = animInfo.Animation.DefaultValues.Position.x;
                                newFloatX = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                                baseFloatValue = animInfo.TargetImage.rectTransform.localPosition.y;
                                defaultFloatValue = animInfo.Animation.DefaultValues.Position.y;
                                newFloatY = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                                tween = animInfo.TargetImage.rectTransform.DOLocalMove(new Vector3(newFloatX, newFloatY, floatZ), command.Duration);
                            }
                            else
                            {
                                baseFloatValue = animInfo.TargetSprite.transform.localPosition.x;
                                defaultFloatValue = animInfo.Animation.DefaultValues.Position.x;
                                newFloatX = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                                baseFloatValue = animInfo.TargetSprite.transform.localPosition.y;
                                defaultFloatValue = animInfo.Animation.DefaultValues.Position.y;
                                newFloatY = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                                tween = animInfo.TargetSprite.transform.DOLocalMove(new Vector3(newFloatX, newFloatY, floatZ), command.Duration);
                            }
                            break;
                        case AnimAxis.XY:
                            if(animInfo.IsImage)
                            {
                                baseVector2Value = animInfo.TargetImage.rectTransform.localPosition;
                                defaultVector2Value = animInfo.Animation.DefaultValues.Position;
                                newVector2 = InterpretValueMode(baseVector2Value, defaultVector2Value, command);
                                tween = animInfo.TargetImage.rectTransform.DOLocalMove(new Vector3(newVector2.x, newVector2.y, floatZ), command.Duration);
                            }
                            else
                            {
                                baseVector2Value = animInfo.TargetSprite.transform.localPosition;
                                defaultVector2Value = animInfo.Animation.DefaultValues.Position;
                                newVector2 = InterpretValueMode(baseVector2Value, defaultVector2Value, command);
                                tween = animInfo.TargetSprite.transform.DOLocalMove(new Vector3(newVector2.x, newVector2.y, floatZ), command.Duration);
                            }
                            break;
                        case AnimAxis.X:
                            if(animInfo.IsImage)
                            {
                                baseFloatValue = animInfo.TargetImage.rectTransform.localPosition.x;
                                defaultFloatValue = animInfo.Animation.DefaultValues.Position.x;
                                newFloat = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                                tween = animInfo.TargetImage.rectTransform.DOLocalMoveX(newFloat, command.Duration);
                            }
                            else
                            {
                                baseFloatValue = animInfo.TargetSprite.transform.localPosition.x;
                                defaultFloatValue = animInfo.Animation.DefaultValues.Position.x;
                                newFloat = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                                tween = animInfo.TargetSprite.transform.DOLocalMoveX(newFloat, command.Duration);
                            }
                            break;
                        case AnimAxis.Y:
                            if(animInfo.IsImage)
                            {
                                baseFloatValue = animInfo.TargetImage.rectTransform.localPosition.y;
                                defaultFloatValue = animInfo.Animation.DefaultValues.Position.y;
                                newFloat = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                                tween = animInfo.TargetImage.rectTransform.DOLocalMoveY(newFloat, command.Duration);
                            }
                            else
                            {
                                baseFloatValue = animInfo.TargetSprite.transform.localPosition.y;
                                defaultFloatValue = animInfo.Animation.DefaultValues.Position.y;
                                newFloat = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                                tween = animInfo.TargetSprite.transform.DOLocalMoveY(newFloat, command.Duration);
                            }
                            break;
                        default:
                            Debug.LogError($"[MicroBar] Unknown 'AnimAxis': {command.AnimAxis}");
                            return null;
                    }
                    break;
                case AnimEffect.Rotate:
                    defaultFloatValue = animInfo.Animation.DefaultValues.Rotation;
                    if(animInfo.IsImage)
                    {
                        baseFloatValue = animInfo.TargetImage.rectTransform.localRotation.eulerAngles.z;
                        newFloat = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                        tween = animInfo.TargetImage.rectTransform.DOLocalRotate(new Vector3(0f, 0f, newFloat), command.Duration);
                    }
                    else
                    {
                        baseFloatValue = animInfo.TargetSprite.transform.localRotation.eulerAngles.z;
                        newFloat = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                        newFloatX = animInfo.TargetSprite.transform.localRotation.eulerAngles.x;
                        newFloatY = animInfo.TargetSprite.transform.localRotation.eulerAngles.y;
                        tween = animInfo.TargetSprite.transform.DOLocalRotate(new Vector3(newFloatX, newFloatY, newFloat), command.Duration);
                    }
                    break;
                case AnimEffect.Scale:
                    if(animInfo.IsImage)
                    {
                        floatZ = animInfo.TargetImage.rectTransform.localScale.z;
                        switch(command.AnimAxis)
                        {
                            case AnimAxis.Uniform:
                                baseFloatValue = animInfo.TargetImage.rectTransform.localScale.x;
                                defaultFloatValue = animInfo.Animation.DefaultValues.Scale.x;
                                newFloatX = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                                baseFloatValue = animInfo.TargetImage.rectTransform.localScale.y;
                                defaultFloatValue = animInfo.Animation.DefaultValues.Scale.y;
                                newFloatY = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                                tween = animInfo.TargetImage.rectTransform.DOScale(new Vector3(newFloatX, newFloatY, floatZ), command.Duration);
                                break;
                            case AnimAxis.XY:
                                baseVector2Value = animInfo.TargetImage.rectTransform.localScale;
                                defaultVector2Value = animInfo.Animation.DefaultValues.Scale;
                                newVector2 = InterpretValueMode(baseVector2Value, defaultVector2Value, command);
                                tween = animInfo.TargetImage.rectTransform.DOScale(new Vector3(newVector2.x, newVector2.y, floatZ), command.Duration);
                                break;
                            case AnimAxis.X:
                                baseFloatValue = animInfo.TargetImage.rectTransform.localScale.x;
                                defaultFloatValue = animInfo.Animation.DefaultValues.Scale.x;
                                newFloat = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                                tween = animInfo.TargetImage.rectTransform.DOScaleX(newFloat, command.Duration);
                                break;
                            case AnimAxis.Y:
                                baseFloatValue = animInfo.TargetImage.rectTransform.localScale.y;
                                defaultFloatValue = animInfo.Animation.DefaultValues.Scale.y;
                                newFloat = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                                tween = animInfo.TargetImage.rectTransform.DOScaleY(newFloat, command.Duration);
                                break;
                            default:
                                Debug.LogError($"[MicroBar] Unknown 'AnimAxis': {command.AnimAxis}");
                                return null;
                        }
                    }
                    else
                    {
                        floatZ = animInfo.TargetSprite.transform.localScale.z;
                        switch(command.AnimAxis)
                        {
                            case AnimAxis.Uniform:
                                baseFloatValue = animInfo.TargetSprite.transform.localScale.x;
                                defaultFloatValue = animInfo.Animation.DefaultValues.Scale.x;
                                newFloatX = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                                baseFloatValue = animInfo.TargetSprite.transform.localScale.y;
                                defaultFloatValue = animInfo.Animation.DefaultValues.Scale.y;
                                newFloatY = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                                tween = animInfo.TargetSprite.transform.DOScale(new Vector3(newFloatX, newFloatY, floatZ), command.Duration);
                                break;
                            case AnimAxis.XY:
                                baseVector2Value = animInfo.TargetSprite.transform.localScale;
                                defaultVector2Value = animInfo.Animation.DefaultValues.Scale;
                                newVector2 = InterpretValueMode(baseVector2Value, defaultVector2Value, command);
                                tween = animInfo.TargetSprite.transform.DOScale(new Vector3(newVector2.x, newVector2.y, floatZ), command.Duration);
                                break;
                            case AnimAxis.X:
                                baseFloatValue = animInfo.TargetSprite.transform.localScale.x;
                                defaultFloatValue = animInfo.Animation.DefaultValues.Scale.x;
                                newFloat = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                                tween = animInfo.TargetSprite.transform.DOScaleX(newFloat, command.Duration);
                                break;
                            case AnimAxis.Y:
                                baseFloatValue = animInfo.TargetSprite.transform.localScale.y;
                                defaultFloatValue = animInfo.Animation.DefaultValues.Scale.y;
                                newFloat = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                                tween = animInfo.TargetSprite.transform.DOScaleY(newFloat, command.Duration);
                                break;
                            default:
                                Debug.LogError($"[MicroBar] Unknown 'AnimAxis': {command.AnimAxis}");
                                return null;
                        }
                    }
                    break;
                case AnimEffect.Punch:
                    switch(command.TransformProperty)
                    {
                        case TransformProperties.Position:
                            if(animInfo.IsImage)
                            {
                                tween = animInfo.TargetImage.rectTransform.DOPunchPosition(command.Vector2Value, command.Duration, command.Frequency, command.PercentValue);
                            }
                            else
                            {
                                tween = animInfo.TargetSprite.transform.DOPunchPosition(command.Vector2Value, command.Duration, command.Frequency, command.PercentValue);
                            }
                            break;
                        case TransformProperties.Rotation:
                            if(animInfo.IsImage)
                            {
                                tween = animInfo.TargetImage.rectTransform.DOPunchRotation(new Vector3(0f, 0f, command.FloatValue), command.Duration, command.Frequency, command.PercentValue);
                            }
                            else
                            {
                                tween = animInfo.TargetSprite.transform.DOPunchRotation(new Vector3(0f, 0f, command.FloatValue), command.Duration, command.Frequency, command.PercentValue);
                            }
                            break;
                        case TransformProperties.Scale:
                            if(animInfo.IsImage)
                            {
                                tween = animInfo.TargetImage.rectTransform.DOPunchScale(command.Vector2Value, command.Duration, command.Frequency, command.PercentValue);
                            }
                            else
                            {
                                tween = animInfo.TargetSprite.transform.DOPunchScale(command.Vector2Value, command.Duration, command.Frequency, command.PercentValue);
                            }
                            break;
                        case TransformProperties.AnchorPosition:
                            if(!animInfo.IsImage)
                                break;   // AnchorPosition is not supported for the sprites
                            tween = animInfo.TargetImage.rectTransform.DOPunchAnchorPos(command.Vector2Value, command.Duration, command.Frequency, command.PercentValue);
                            break;
                        default:
                            Debug.LogError($"[MicroBar] Unknown 'TransformProperty': {command.TransformProperty}");
                            return null;
                    }
                    break;
                case AnimEffect.Shake:
                    switch(command.TransformProperty)
                    {
                        case TransformProperties.Position:
                            if(animInfo.IsImage)
                            {
                                tween = animInfo.TargetImage.rectTransform.DOShakePosition(command.Duration, command.FloatValue, command.Frequency, 90f);
                            }
                            else
                            {
                                tween = animInfo.TargetSprite.transform.DOShakePosition(command.Duration, command.FloatValue, command.Frequency, 90f);
                            }
                            break;
                        case TransformProperties.Rotation:
                            if(animInfo.IsImage)
                            {
                                tween = animInfo.TargetImage.rectTransform.DOShakeRotation(command.Duration, new Vector3(0f, 0f, command.FloatValue), command.Frequency, 90f);
                            }
                            else
                            {
                                tween = animInfo.TargetSprite.transform.DOShakeRotation(command.Duration, new Vector3(0f, 0f, command.FloatValue), command.Frequency, 90f);
                            }
                            break;
                        case TransformProperties.Scale:
                            if(animInfo.IsImage)
                            {
                                tween = animInfo.TargetImage.rectTransform.DOShakeScale(command.Duration, command.FloatValue, command.Frequency, 90f);
                            }
                            else
                            {
                                tween = animInfo.TargetSprite.transform.DOShakeScale(command.Duration, command.FloatValue, command.Frequency, 90f);
                            }
                            break;
                        case TransformProperties.AnchorPosition:
                            if(!animInfo.IsImage)
                                break;   // AnchorPosition is not supported for the sprites
                            tween = animInfo.TargetImage.rectTransform.DOShakeAnchorPos(command.Duration, command.FloatValue, command.Frequency, 90f);
                            break;
                        default:
                            Debug.LogError($"[MicroBar] Unknown 'TransformProperty': {command.TransformProperty}");
                            return null;
                    }
                    break;
                case AnimEffect.AnchorMove:
                    if(!animInfo.IsImage)
                        break;   // AnchorPosition is not supported for the sprites
                    switch(command.AnimAxis)
                    {
                        case AnimAxis.Uniform:
                            baseFloatValue = animInfo.TargetImage.rectTransform.anchoredPosition.x;
                            defaultFloatValue = animInfo.Animation.DefaultValues.AnchorPosition.x;
                            newFloatX = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                            baseFloatValue = animInfo.TargetImage.rectTransform.anchoredPosition.y;
                            defaultFloatValue = animInfo.Animation.DefaultValues.AnchorPosition.y;
                            newFloatY = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                            tween = animInfo.TargetImage.rectTransform.DOAnchorPos(new Vector2(newFloatX, newFloatY), command.Duration);
                            break;
                        case AnimAxis.XY:
                            baseVector2Value = animInfo.TargetImage.rectTransform.anchoredPosition;
                            defaultVector2Value = animInfo.Animation.DefaultValues.AnchorPosition;
                            newVector2 = InterpretValueMode(baseVector2Value, defaultVector2Value, command);
                            tween = animInfo.TargetImage.rectTransform.DOAnchorPos(newVector2, command.Duration);
                            break;
                        case AnimAxis.X:
                            baseFloatValue = animInfo.TargetImage.rectTransform.anchoredPosition.x;
                            defaultFloatValue = animInfo.Animation.DefaultValues.AnchorPosition.x;
                            newFloat = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                            tween = animInfo.TargetImage.rectTransform.DOAnchorPosX(newFloat, command.Duration);
                            break;
                        case AnimAxis.Y:
                            baseFloatValue = animInfo.TargetImage.rectTransform.anchoredPosition.y;
                            defaultFloatValue = animInfo.Animation.DefaultValues.AnchorPosition.y;
                            newFloat = InterpretValueMode(baseFloatValue, defaultFloatValue, command, false);
                            tween = animInfo.TargetImage.rectTransform.DOAnchorPosY(newFloat, command.Duration);
                            break;
                        default:
                            Debug.LogError($"[MicroBar] Unknown 'AnimAxis': {command.AnimAxis}");
                            return null;
                    }
                    break;
                default:
                    Debug.LogError($"[MicroBar] Unknown 'Effect': {command.Effect}");
                    return null;
            }

            if(tween == null)
            {
                return null;   // Don't go further if tween is null, but this should not be true in any case
            }

            if(command.Delay > 0f)
            {
                tween.SetDelay(command.Delay);
            }
            tween.SetEase(command.Ease);
            return tween;
        }

        #region Value interpreter
        // ****************************************************************************************************
        static Color InterpretValueMode(Color baseValue, Color defaultValue, AnimCommand command)
        {
            if(command.ValueMode == ValueMode.Absolute)
            {
                return command.ColorValue;
            }
            else if(command.ValueMode == ValueMode.Additive)
            {
                return baseValue + command.ColorValue;
            }
            else if(command.ValueMode == ValueMode.Multiplicative)
            {
                return baseValue * command.ColorValue;
            }
            else if(command.ValueMode == ValueMode.StartingValue)
            {
                return baseValue;
            }
            else if(command.ValueMode == ValueMode.DefaultValue)
            {
                return defaultValue;
            }
            else
            {
                Debug.LogError($"[MicroBar] Unknown 'ValueMode': {command.ValueMode}");
                return baseValue;
            }
        }

        static float InterpretValueMode(float baseValue, float defaultValue, AnimCommand command, bool percent)
        {
            if(command.ValueMode == ValueMode.Absolute)
            {
                if(percent)
                {
                    return Mathf.Clamp01(command.PercentValue);
                }
                else
                {
                    return command.FloatValue;
                }
            }
            else if(command.ValueMode == ValueMode.Additive)
            {
                if(percent)
                {
                    return Mathf.Clamp01(baseValue + command.PercentValue);
                }
                else
                {
                    return baseValue + command.FloatValue;
                }
            }
            else if(command.ValueMode == ValueMode.Multiplicative)
            {
                if(percent)
                {
                    return Mathf.Clamp01(baseValue * command.PercentValue);
                }
                else
                {
                    return baseValue * command.FloatValue;
                }
            }
            else if(command.ValueMode == ValueMode.StartingValue)
            {
                if(percent)
                {
                    return Mathf.Clamp01(baseValue);
                }
                else
                {
                    return baseValue;
                }
            }
            else if(command.ValueMode == ValueMode.DefaultValue)
            {
                return defaultValue;
            }
            else
            {
                Debug.LogError($"[MicroBar] Unknown 'ValueMode': {command.ValueMode}");
                return baseValue;
            }
        }

        static int InterpretValueMode(int baseValue, int defaultValue, AnimCommand command)
        {
            if(command.ValueMode == ValueMode.Absolute)
            {
                return command.IntValue;
            }
            else if(command.ValueMode == ValueMode.Additive)
            {
                return baseValue + command.IntValue;
            }
            else if(command.ValueMode == ValueMode.Multiplicative)
            {
                return baseValue * command.IntValue;
            }
            else if(command.ValueMode == ValueMode.StartingValue)
            {
                return baseValue;
            }
            else if(command.ValueMode == ValueMode.DefaultValue)
            {
                return defaultValue;
            }
            else
            {
                Debug.LogError($"[MicroBar] Unknown 'ValueMode': {command.ValueMode}");
                return baseValue;
            }
        }

        static Vector3 InterpretValueMode(Vector3 baseValue, Vector3 defaultValue, AnimCommand command)
        {
            if(command.ValueMode == ValueMode.Absolute)
            {
                return command.Vector3Value;
            }
            else if(command.ValueMode == ValueMode.Additive)
            {
                return baseValue + command.Vector3Value;
            }
            else if(command.ValueMode == ValueMode.Multiplicative)
            {
                return new Vector3(baseValue.x * command.Vector3Value.x, baseValue.y * command.Vector3Value.y, baseValue.z * command.Vector3Value.z);
            }
            else if(command.ValueMode == ValueMode.StartingValue)
            {
                return baseValue;
            }
            else if(command.ValueMode == ValueMode.DefaultValue)
            {
                return defaultValue;
            }
            else
            {
                Debug.LogError($"[MicroBar] Unknown 'ValueMode': {command.ValueMode}");
                return baseValue;
            }
        }

        static Vector2 InterpretValueMode(Vector2 baseValue, Vector2 defaultValue, AnimCommand command)
        {
            if(command.ValueMode == ValueMode.Absolute)
            {
                return command.Vector2Value;
            }
            else if(command.ValueMode == ValueMode.Additive)
            {
                return baseValue + command.Vector2Value;
            }
            else if(command.ValueMode == ValueMode.Multiplicative)
            {
                return new Vector2(baseValue.x * command.Vector3Value.x, baseValue.y * command.Vector3Value.y);
            }
            else if(command.ValueMode == ValueMode.StartingValue)
            {
                return baseValue;
            }
            else if(command.ValueMode == ValueMode.DefaultValue)
            {
                return defaultValue;
            }
            else
            {
                Debug.LogError($"[MicroBar] Unknown 'ValueMode': {command.ValueMode}");
                return baseValue;
            }
        }
        #endregion
    }
}
