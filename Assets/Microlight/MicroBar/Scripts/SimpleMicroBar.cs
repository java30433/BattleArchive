using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Microlight.MicroBar
{
    // ****************************************************************************************************
    // Class for storing data about MicroBar animation in simple mode
    // ****************************************************************************************************

    [System.Serializable]
    public class SimpleMicroBar
    {
        // General
        [SerializeField] RenderType _renderType;
        internal RenderType RenderType => _renderType;
        [SerializeField] bool _isAnimated = true;   // Will the bar be animated
        internal bool IsAnimated => _isAnimated;

        // SpriteRenderer
        [SerializeField] SpriteRenderer _srBackground;
        internal SpriteRenderer SRBackground => _srBackground;
        [SerializeField] SpriteRenderer _srPrimaryBar;
        internal SpriteRenderer SRPrimaryBar => _srPrimaryBar;
        internal SpriteMask SRPrimaryBarMask { get; private set; }
        [SerializeField] SpriteRenderer _srGhostBar;
        internal SpriteRenderer SRGhostBar => _srGhostBar;
        internal SpriteMask SRGhostBarMask { get; private set; }

        // Image
        [SerializeField] Image _uiBackground;
        internal Image UIBackground => _uiBackground;
        [SerializeField] Image _uiPrimaryBar;
        internal Image UIPrimaryBar => _uiPrimaryBar;
        [SerializeField] Image _uiGhostBar;
        internal Image UIGhostBar => _uiGhostBar;

        // Colors
        [SerializeField] bool _adaptiveColor = true;   // Does health bar uses adaptive color based on current hp
        internal bool AdaptiveColor => _adaptiveColor;
        [SerializeField] Color _barPrimaryColor = new Color(1f, 1f, 1f);   // Color of the main health bar, also used as a color for full health in adaptive color
        internal Color BarPrimaryColor => _barPrimaryColor;
        [SerializeField] Color _barAdaptiveColor = new Color(1f, 0f, 0f);   // Color that health changes to as it gets lower
        internal Color BarAdaptiveColor => _barAdaptiveColor;

        // Ghost bar
        [SerializeField] bool _useGhostBar = true;   // Is ghost bar used
        internal bool UseGhostBar => _useGhostBar;
        [SerializeField] bool _dualGhostBars = false;   // Are ghost bars two separate bars for healing and damaging or single bar for both
        internal bool DualGhostBars => _dualGhostBars;
        [SerializeField] Color _ghostBarDamageColor = new Color(1f, 0f, 0f);   // Color of ghost bar in single mode and when hurt in dual mode
        internal Color GhostBarDamageColor => _ghostBarDamageColor;
        [SerializeField] Color _ghostBarHealColor = new Color(1f, 1f, 1f);   // Color of ghost bar when healed
        internal Color GhostBarHealColor => _ghostBarHealColor;

        // Duplicate fields are here to have more control over how they are displayed in editor
        // Putting them in separate class/field only makes things uglier
        // Damage Animation
        [SerializeField] SimpleAnim _damageAnim;   // Type of animation that will be played when the bar is damaged
        internal SimpleAnim DamageAnim => _damageAnim;
        [SerializeField][Range(0.01f, 1f)] float _damageAnimDuration = 0.5f;   // Duration of the animation
        internal float DamageAnimDuration => _damageAnimDuration;
        [SerializeField][Range(0f, 1f)] float _damageAnimDelay = 0f;   // How long will animation wait before following ghost bar
        internal float DamageAnimDelay => _damageAnimDelay;
        [SerializeField] Color _damageFlashColor = new Color(1f, 1f, 1f, 1f);
        internal Color DamageFlashColor => _damageFlashColor;
        [SerializeField][Range(0f, 1f)] float _damageAnimStrength = 0.5f;
        internal float DamageAnimStrength => _damageAnimStrength;

        // Heal Animation
        [SerializeField] SimpleAnim _healAnim;   // Type of animation that will be played when the bar is healed
        internal SimpleAnim HealAnim => _healAnim;
        [SerializeField][Range(0.01f, 1f)] float _healAnimDuration = 0.5f;   // Duration of the animation
        internal float HealAnimDuration => _healAnimDuration;
        [SerializeField][Range(0f, 1f)] float _healAnimDelay = 0f;   // Delay before animation starts playing
        internal float HealAnimDelay => _healAnimDelay;
        [SerializeField] Color _healFlashColor = new Color(1f, 1f, 1f, 1f);
        internal Color HealFlashColor => _healFlashColor;
        [SerializeField][Range(0f, 1f)] float _healAnimStrength = 0.5f;
        internal float HealAnimStrength => _healAnimStrength;

        // Variables
        internal MicroBar ParentBar { get; private set; }
        Sequence sequence;   // Sequence for animations
        public Sequence Sequence => sequence;

        internal bool Initialize(MicroBar bar)
        {
            if(RenderType == RenderType.Image)
            {
                if(UIBackground == null)
                {
                    Debug.LogError("[MicroBar] RenderType set to 'Image' but 'UIBackground' is null");
                    return false;
                }
                if(UIPrimaryBar == null)
                {
                    Debug.LogError("[MicroBar] RenderType set to 'Image' but 'UIPrimaryBar' is null");
                    return false;
                }
                if(IsAnimated && UseGhostBar && UIGhostBar == null)
                {
                    Debug.LogError("[MicroBar] RenderType set to 'Image' but 'UIGhostBar' is null");
                    return false;
                }
            }
            if(RenderType == RenderType.Sprite)
            {
                if(SRBackground == null)
                {
                    Debug.LogError("[MicroBar] RenderType set to 'Sprite' but 'SRBackground' is null");
                    return false;
                }
                if(SRPrimaryBar == null)
                {
                    Debug.LogError("[MicroBar] RenderType set to 'Sprite' but 'SRPrimaryBar' is null");
                    return false;
                }
                else
                {
                    if(SRPrimaryBar.GetComponent<SortingGroup>() == null)
                    {
                        Debug.LogError("[MicroBar] Couldn't find the 'SortingGroup' for the 'SRPrimaryBar'");
                        return false;
                    }
                    SRPrimaryBarMask = SRPrimaryBar.GetComponentInChildren<SpriteMask>();
                    if(SRPrimaryBarMask == null)
                    {
                        Debug.LogError("[MicroBar] Couldn't find the 'SpriteMask' for the 'SRPrimaryBar'");
                        return false;
                    }
                }
                if(IsAnimated && UseGhostBar && SRGhostBar == null)
                {
                    Debug.LogError("[MicroBar] RenderType set to 'Sprite' but 'SRGhostBar' is null");
                    return false;
                }
                else if(IsAnimated && UseGhostBar)
                {
                    if(SRGhostBar.GetComponent<SortingGroup>() == null)
                    {
                        Debug.LogError("[MicroBar] Couldn't find the 'SortingGroup' for the 'SRPrimaryBar'");
                        return false;
                    }
                    SRGhostBarMask = SRGhostBar.GetComponentInChildren<SpriteMask>();
                    if(SRGhostBarMask == null)
                    {
                        Debug.LogError("[MicroBar] Couldn't find the 'SpriteMask' for the 'SRGhostBar'");
                        return false;
                    }
                }
            }

            ParentBar = bar;
            InitializeValues();
            bar.OnBarUpdate += Update;
            bar.BarDestroyed += Destroy;

            return true;
        }

        // Sets some starting values
        void InitializeValues()
        {
            bool isSprite = RenderType == RenderType.Sprite;

            SilentUpdate();

            // Update colors
            if(AdaptiveColor)
            {
                SimpleAnimBuilder.SetAdaptiveBarColor(this, false);
            }
            else
            {
                if(isSprite)
                {
                    SRPrimaryBar.color = BarPrimaryColor;
                }
                else
                {
                    UIPrimaryBar.color = BarPrimaryColor;
                }
            }

            // Ghost bar
            if((IsAnimated && !UseGhostBar) || !IsAnimated)
            {
                if(SRGhostBar != null)
                {
                    SRGhostBar.gameObject.SetActive(false);
                }
                if(UIGhostBar != null)
                {
                    UIGhostBar.gameObject.SetActive(false);
                }
            }
            else if(IsAnimated && UseGhostBar)
            {
                if(isSprite)
                {
                    SRGhostBar.color = GhostBarDamageColor;
                }
                else
                {
                    UIGhostBar.color = GhostBarDamageColor;
                }
            }
        }

        // animationType is there only to connect to the event, but is not actually connected
        void Update(bool skipAnimation, UpdateAnim animationType)
        {
            // Always kill when bar is updating, because we dont want to have for example active damage animation if heal animation is active
            if(sequence.IsActive())
            {
                sequence.Kill();
                sequence = null;
            }

            // Decide if animation should be skipped
            bool isHealAnimation = ParentBar.CurrentValue > ParentBar.PreviousValue;
            SimpleAnim animToBePlayed = isHealAnimation ? HealAnim : DamageAnim;
            if(skipAnimation || !IsAnimated || animToBePlayed == SimpleAnim.None)
            {
                SilentUpdate();
                return;
            }

            sequence = SimpleAnimBuilder.BuildAnimation(this);
        }

        // Silently updates bars to the values without animating
        void SilentUpdate()
        {
            if(ParentBar == null)
            {
                Debug.LogError("[MicroBar] Missing reference to the 'ParentBar'");
                return;
            }

            if(RenderType == RenderType.Image)
            {
                UIPrimaryBar.fillAmount = ParentBar.HPPercent;
                if(UseGhostBar)
                {
                    UIGhostBar.fillAmount = ParentBar.HPPercent;
                }
            }
            else if(RenderType == RenderType.Sprite)
            {
                SRPrimaryBarMask.transform.localScale = new Vector3(ParentBar.HPPercent, SRPrimaryBarMask.transform.localScale.y, SRPrimaryBarMask.transform.localScale.z);
                if(UseGhostBar)
                {
                    SRGhostBarMask.transform.localScale = new Vector3(ParentBar.HPPercent, SRGhostBarMask.transform.localScale.y, SRGhostBarMask.transform.localScale.z);
                }
            }

            if(AdaptiveColor)
            {
                SimpleAnimBuilder.SetAdaptiveBarColor(this, false);
            }
        }

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
