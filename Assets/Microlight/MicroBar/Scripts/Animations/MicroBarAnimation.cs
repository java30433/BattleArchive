using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Microlight.MicroBar
{
    // ****************************************************************************************************
    // Animation which holds commands on how should animation behave
    // Triggered by UpdateAnim definition in the bar update
    // *Class must be public because it needs to be used in editor scripts
    // ****************************************************************************************************
    [System.Serializable]
    public class MicroBarAnimation
    {
        [SerializeField] UpdateAnim animationType;
        [SerializeField] RenderType renderType;
        [SerializeField] Image targetImage;
        [SerializeField] SpriteRenderer targetSprite;
        Transform targetSpriteMask;
        [SerializeField] bool notBar;   // If turned on, when animation is skipped, will not fill image/sprite
        [SerializeField] List<AnimCommand> commands;

        MicroBar parentBar;
        GraphicsDefaultValues defaultValues;
        internal GraphicsDefaultValues DefaultValues => defaultValues;
        internal bool NotBar => notBar;

        Sequence sequence;   // Sequence for animations
        public Sequence Sequence => sequence;

        internal bool Initialize(MicroBar bar)
        {
            if(renderType == RenderType.Image && targetImage == null)
            {
                Debug.LogError("[MicroBar] RenderType set to 'Image' but 'targetImage' is null");
                return false;
            }
            if(renderType == RenderType.Sprite)
            {
                if(targetSprite == null)
                {
                    Debug.LogError("[MicroBar] RenderType set to 'Sprite' but 'targetSprite' is null");
                    return false;
                }

                // SpriteMask is checked only if component is not tagged as 'NotBar'
                if(!notBar)
                {
                    if(targetSprite.GetComponent<SortingGroup>() == null)
                    {
                        Debug.LogWarning("[MicroBar] Couldn't find the 'SortingGroup' for the 'targetSprite'." +
                            "While the game will still run and update the bar values, it might lead to unintended behaviour." +
                            "For more info, look into documentation under 'Render type' section to update your SpriteRenderer bar structure.");
                    }

                    // This is only improvization for the backwards compatibility for the older versions of the MicroBar
                    // Will use targetSprite as a target for the fill animations
                    SpriteMask spriteMask = targetSprite.GetComponentInChildren<SpriteMask>();
                    if(spriteMask == null)
                    {
                        targetSpriteMask = targetSprite.transform;
                        Debug.LogError("[MicroBar] Couldn't find the 'SpriteMask' for the 'targetSprite'." +
                            "While the game will still run and update the bar values, it might lead to unintended behaviour." +
                            "For more info, look into documentation under 'Render type' section to update your SpriteRenderer bar structure.");
                    }
                    else
                    {
                        targetSpriteMask = spriteMask.transform;
                    }
                }
            }

            parentBar = bar;
            bar.OnBarUpdate += Update;
            bar.BarDestroyed += Destroy;
            bar.OnDefaultValuesSnapshot += DefaultValuesSnapshot;

            DefaultValuesSnapshot();

            return true;
        }

        // When bar is updated, decide what this animation should do
        void Update(bool skipAnimation, UpdateAnim animationType)
        {
            // Always kill when bar is updating, because we dont want to have for example active damage animation if heal animation is active
            if(sequence.IsActive())
            {
                sequence.Kill();
                sequence = null;
            }
            if(animationType != this.animationType)
            {
                return;
            }
            if(skipAnimation)
            {
                if(!notBar)
                {
                    SilentUpdate();
                }
                return;
            }

            if(renderType == RenderType.Image)
            {
                AnimationInfo animInfo = new AnimationInfo(commands, targetImage, parentBar, this);
                sequence = AnimBuilder.BuildAnimation(animInfo);
            }
            else
            {
                AnimationInfo animInfo = new AnimationInfo(commands, targetSprite, targetSpriteMask, parentBar, this);
                sequence = AnimBuilder.BuildAnimation(animInfo);
            }
        }

        internal void DefaultValuesSnapshot()
        {
            if(renderType == RenderType.Image)
            {
                defaultValues = new GraphicsDefaultValues(targetImage);
            }
            else if(renderType == RenderType.Sprite)
            {
                defaultValues = new GraphicsDefaultValues(targetSprite);
            }
        }

        // When animation is skipped, bar can be updated silently
        void SilentUpdate()
        {
            if(parentBar == null)
            {
                Debug.LogError("[MicroBar] Missing reference to the 'ParentBar'");
                return;
            }
            if(renderType == RenderType.Image)
            {
                targetImage.fillAmount = parentBar.HPPercent;
            }
            else if(renderType == RenderType.Sprite)
            {
                targetSpriteMask.localScale = new Vector3(parentBar.HPPercent, targetSpriteMask.localScale.y, targetSpriteMask.localScale.z);
            }
        }

        // When health bar is being destroyed
        void Destroy()
        {
            if(sequence.IsActive())
            {
                sequence.Kill();
                sequence = null;
            }
        }
    }
}