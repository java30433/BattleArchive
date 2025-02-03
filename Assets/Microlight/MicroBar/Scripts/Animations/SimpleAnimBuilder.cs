using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Microlight.MicroBar
{
    // ****************************************************************************************************
    // Class for creating animation out of SimpleMicroBar
    // ****************************************************************************************************
    internal static class SimpleAnimBuilder
    {
        /// <summary>
        /// Builds whole sequence for the animation from the bar
        /// </summary>
        /// <param name="bar">Bar for which animation should be built</param>
        /// <returns>Sequence for the animation</returns>
        internal static Sequence BuildAnimation(SimpleMicroBar bar)
        {
            Sequence sequence = DOTween.Sequence();

            // Decide to take heal or damage animation
            SimpleAnim anim = bar.DamageAnim;
            bool isHealAnimation = bar.ParentBar.CurrentValue > bar.ParentBar.PreviousValue;
            if(isHealAnimation)
            {
                anim = bar.HealAnim;
            }

            // Decide which animation to create
            switch(anim)
            {
                case SimpleAnim.None:
                    break;
                case SimpleAnim.Fill:
                    FillAnim(bar, sequence);
                    break;
                case SimpleAnim.Flash:
                    FlashAnim(bar, sequence);
                    break;
                case SimpleAnim.Shake:
                    ShakeAnim(bar, sequence);
                    break;
                default:
                    Debug.LogError($"[MicroBar] Unknown 'SimpleAnim': {anim}");
                    break;
            }

            return sequence;
        }

        #region Animation
        // **************************************************
        // Animations
        // **************************************************

        // Creates animation which just fills the bar to the value
        static void FillAnim(SimpleMicroBar bar, Sequence sequence)
        {
            bool isHealAnimation = bar.ParentBar.CurrentValue > bar.ParentBar.PreviousValue;
            bool isSprite = bar.RenderType == RenderType.Sprite;

            // Prepare the bar values before the animation
            if(bar.UseGhostBar)
            {
                if(isHealAnimation)
                {
                    SetGhostBarValue(bar);
                }
                else
                {
                    SetBarValue(bar);
                }
            }
            SetGhostBarColor(bar);

            // Store some values to control the flow more easily
            Transform srBarToAnimate;
            Image uiBarToAnimate;
            float animDuration;
            float animDelay;

            // Decide timings
            if(isHealAnimation)
            {
                animDuration = bar.HealAnimDuration;
                animDelay = bar.HealAnimDelay;
            }
            else
            {
                animDuration = bar.DamageAnimDuration;
                animDelay = bar.DamageAnimDelay;
            }

            // Decide which graphics to control and create animation
            if(isSprite)
            {
                if(isHealAnimation)
                {
                    srBarToAnimate = bar.SRPrimaryBarMask.transform;
                }
                else
                {
                    if(bar.UseGhostBar)
                    {
                        srBarToAnimate = bar.SRGhostBarMask.transform;
                    }
                    else
                    {
                        srBarToAnimate = bar.SRPrimaryBarMask.transform;
                    }
                }

                sequence.Append(srBarToAnimate.DOScaleX(bar.ParentBar.HPPercent, animDuration).SetDelay(animDelay));
            }
            else
            {
                if(isHealAnimation)
                {
                    uiBarToAnimate = bar.UIPrimaryBar;
                }
                else
                {
                    if(bar.UseGhostBar)
                    {
                        uiBarToAnimate = bar.UIGhostBar;
                    }
                    else
                    {
                        uiBarToAnimate = bar.UIPrimaryBar;
                    }
                }

                sequence.Append(uiBarToAnimate.DOFillAmount(bar.ParentBar.HPPercent, animDuration).SetDelay(animDelay));
            }

            // Adaptive color support
            if(bar.AdaptiveColor)
            {
                sequence.OnUpdate(() => SetAdaptiveBarColor(bar));
            }
        }

        // Creates animation which first flashes the bar and then fills it to the value
        static void FlashAnim(SimpleMicroBar bar, Sequence sequence)
        {
            bool isHealAnimation = bar.ParentBar.CurrentValue > bar.ParentBar.PreviousValue;
            bool isSprite = bar.RenderType == RenderType.Sprite;

            // Prepare the bar values before the animation
            if(bar.UseGhostBar)
            {
                if(isHealAnimation)
                {
                    SetGhostBarValue(bar);
                }
                else
                {
                    SetBarValue(bar);
                }
            }

            // Decide the color to which bars will flash and colors to which they should return
            Color flashColor;
            Color barColor = SetAdaptiveBarColor(bar, false);
            Color ghostBarColor = SetGhostBarColor(bar);

            // Decide colors
            if(isHealAnimation)
            {
                flashColor = bar.HealFlashColor;
            }
            else
            {
                flashColor = bar.DamageFlashColor;
            }

            // Set bars to the flash color
            if(bar.RenderType == RenderType.Sprite)
            {
                if(bar.UseGhostBar)
                {
                    bar.SRGhostBar.color = flashColor;
                }
                bar.SRPrimaryBar.color = flashColor;
            }
            else
            {
                if(bar.UseGhostBar)
                {
                    bar.UIGhostBar.color = flashColor;
                }
                bar.UIPrimaryBar.color = flashColor;
            }

            Transform srBarToAnimate;
            Image uiBarToAnimate;
            float flashAnimDuration;
            float fillAnimDuration;
            float fillAnimDelay;
            float animDelay;

            if(isHealAnimation)
            {
                flashAnimDuration = bar.HealAnimDuration * 0.7f;
                fillAnimDuration = bar.HealAnimDuration * 0.7f;
                fillAnimDelay = bar.HealAnimDuration * 0.3f;
                animDelay = bar.HealAnimDelay;
            }
            else
            {
                flashAnimDuration = bar.DamageAnimDuration * 0.7f;
                fillAnimDuration = bar.DamageAnimDuration * 0.7f;
                fillAnimDelay = bar.DamageAnimDuration * 0.3f;
                animDelay = bar.DamageAnimDelay;
            }

            if(isSprite)
            {
                if(isHealAnimation)
                {
                    srBarToAnimate = bar.SRPrimaryBarMask.transform;
                }
                else
                {
                    if(bar.UseGhostBar)
                    {
                        srBarToAnimate = bar.SRGhostBarMask.transform;
                    }
                    else
                    {
                        srBarToAnimate = bar.SRPrimaryBarMask.transform;
                    }
                }

                sequence.Append(bar.SRPrimaryBar.DOColor(barColor, flashAnimDuration).SetDelay(animDelay));
                if(bar.UseGhostBar)
                {
                    sequence.Join(bar.SRGhostBar.DOColor(ghostBarColor, flashAnimDuration));
                }
                sequence.Join(srBarToAnimate.DOScaleX(bar.ParentBar.HPPercent, fillAnimDuration).SetDelay(fillAnimDelay));
            }
            else
            {
                if(isHealAnimation)
                {
                    uiBarToAnimate = bar.UIPrimaryBar;
                }
                else
                {
                    if(bar.UseGhostBar)
                    {
                        uiBarToAnimate = bar.UIGhostBar;
                    }
                    else
                    {
                        uiBarToAnimate = bar.UIPrimaryBar;
                    }
                }

                sequence.Append(bar.UIPrimaryBar.DOColor(barColor, flashAnimDuration).SetDelay(animDelay));
                if(bar.UseGhostBar)
                {
                    sequence.Join(bar.UIGhostBar.DOColor(ghostBarColor, flashAnimDuration));
                }
                sequence.Join(uiBarToAnimate.DOFillAmount(bar.ParentBar.HPPercent, fillAnimDuration).SetDelay(fillAnimDelay));
            }
        }

        // Creates animation that shakes the background and fill bar quickly fills in the middle of the animation
        static void ShakeAnim(SimpleMicroBar bar, Sequence sequence)
        {
            bool isHealAnimation = bar.ParentBar.CurrentValue > bar.ParentBar.PreviousValue;
            bool isSprite = bar.RenderType == RenderType.Sprite;

            // Prepare the bar values before the animation
            if(bar.UseGhostBar)
            {
                if(isHealAnimation)
                {
                    SetGhostBarValue(bar);
                }
                else
                {
                    SetBarValue(bar);
                }
            }
            SetGhostBarColor(bar);

            // Store some values to control the flow more easily
            Transform srFillBar;
            Image uiFillBar;
            float animDuration;
            float fillDelay;
            float fillDuration;
            float animStrength;

            // Decide timings
            if(isHealAnimation)
            {
                animDuration = bar.HealAnimDuration;
                if(isSprite)
                {
                    animStrength = bar.HealAnimStrength * 0.2f;
                }
                else
                {
                    animStrength = bar.HealAnimStrength * 30;
                }
            }
            else
            {
                animDuration = bar.DamageAnimDuration;
                if(isSprite)
                {
                    animStrength = bar.DamageAnimStrength * 0.2f;
                }
                else
                {
                    animStrength = bar.DamageAnimStrength * 30;
                }
            }
            fillDelay = animDuration * 0.2f;
            fillDuration = animDuration * 0.6f;

            // Create shake animation
            if(isSprite)
            {
                bar.SRBackground.transform.localPosition = Vector3.zero;
                sequence.Append(bar.SRBackground.transform.DOShakePosition(animDuration, animStrength, 40));
            }
            else
            {
                bar.UIBackground.transform.localPosition = Vector3.zero;
                sequence.Append(bar.UIBackground.transform.DOShakePosition(animDuration, animStrength, 40));
            }

            // Decide which graphics to control and create animation
            if(isSprite)
            {
                if(isHealAnimation)
                {
                    srFillBar = bar.SRPrimaryBarMask.transform;
                }
                else
                {
                    if(bar.UseGhostBar)
                    {
                        srFillBar = bar.SRGhostBarMask.transform;
                    }
                    else
                    {
                        srFillBar = bar.SRPrimaryBarMask.transform;
                    }
                }

                sequence.Join(srFillBar.DOScaleX(bar.ParentBar.HPPercent, fillDuration).SetDelay(fillDelay));
            }
            else
            {
                if(isHealAnimation)
                {
                    uiFillBar = bar.UIPrimaryBar;
                }
                else
                {
                    if(bar.UseGhostBar)
                    {
                        uiFillBar = bar.UIGhostBar;
                    }
                    else
                    {
                        uiFillBar = bar.UIPrimaryBar;
                    }
                }

                sequence.Join(uiFillBar.DOFillAmount(bar.ParentBar.HPPercent, fillDuration).SetDelay(fillDelay));
            }

            if(bar.AdaptiveColor)
            {
                sequence.OnUpdate(() => SetAdaptiveBarColor(bar));
            }
        }
        #endregion

        #region Utility
        // **************************************************
        // Utility
        // **************************************************

        // Sets primary bar based on values
        static void SetBarValue(SimpleMicroBar bar)
        {
            if(bar.RenderType == RenderType.Image)
            {
                bar.UIPrimaryBar.fillAmount = bar.ParentBar.HPPercent;
            }
            else if(bar.RenderType == RenderType.Sprite)
            {
                bar.SRPrimaryBarMask.transform.localScale = new Vector3(bar.ParentBar.HPPercent, bar.SRPrimaryBarMask.transform.localScale.y, bar.SRPrimaryBarMask.transform.localScale.z);
            }
        }

        // Sets ghost bar based on values
        static void SetGhostBarValue(SimpleMicroBar bar)
        {
            if(bar.RenderType == RenderType.Image)
            {
                bar.UIGhostBar.fillAmount = bar.ParentBar.HPPercent;
            }
            else if(bar.RenderType == RenderType.Sprite)
            {
                bar.SRGhostBarMask.transform.localScale = new Vector3(bar.ParentBar.HPPercent, bar.SRGhostBarMask.transform.localScale.y, bar.SRGhostBarMask.transform.localScale.z);
            }
        }

        // Sets ghost bar color if ghost bar uses dual colors
        static Color SetGhostBarColor(SimpleMicroBar bar)
        {
            if(!bar.UseGhostBar)
            {
                return Color.white;
            }
            if(!bar.DualGhostBars)
            {
                return bar.GhostBarDamageColor;   // Also used for ghost bar color when not using dual ghost bars
            }

            bool isDamage = bar.ParentBar.CurrentValue < bar.ParentBar.PreviousValue;

            Color ghostBarColor;
            if(isDamage)
            {
                ghostBarColor = bar.GhostBarDamageColor;
            }
            else
            {
                ghostBarColor = bar.GhostBarHealColor;
            }

            if(bar.RenderType == RenderType.Sprite)
            {
                bar.SRGhostBar.color = ghostBarColor;
            }
            else
            {
                bar.UIGhostBar.color = ghostBarColor;
            }

            return ghostBarColor;
        }

        // Sets the bar color to the adaptive value
        // useBarValue will set the adaptive color value based on the bar current value in graphics terms,
        // false will use bar current numerical value
        internal static Color SetAdaptiveBarColor(SimpleMicroBar bar, bool useBarValue = true)
        {
            if(!bar.AdaptiveColor)
            {
                return bar.BarPrimaryColor;
            }

            Color barColor;
            if(bar.RenderType == RenderType.Sprite)
            {
                if(useBarValue)
                {
                    barColor = Color.Lerp(bar.BarAdaptiveColor, bar.BarPrimaryColor, bar.SRPrimaryBarMask.transform.localScale.x);
                }
                else
                {
                    barColor = Color.Lerp(bar.BarAdaptiveColor, bar.BarPrimaryColor, bar.ParentBar.HPPercent);
                }
                bar.SRPrimaryBar.color = barColor;
            }
            else
            {
                if(useBarValue)
                {
                    barColor = Color.Lerp(bar.BarAdaptiveColor, bar.BarPrimaryColor, bar.UIPrimaryBar.fillAmount);
                }
                else
                {
                    barColor = Color.Lerp(bar.BarAdaptiveColor, bar.BarPrimaryColor, bar.ParentBar.HPPercent);
                }
                bar.UIPrimaryBar.color = barColor;
            }

            return barColor;
        }
        #endregion
    }
}