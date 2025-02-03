using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microlight.MicroBar
{
    // ****************************************************************************************************
    // Stores data about animation for easier passing of the data
    // ****************************************************************************************************
    internal readonly struct AnimationInfo
    {
        readonly IReadOnlyList<AnimCommand> commands;
        readonly bool isImage;
        readonly Image targetImage;
        readonly SpriteRenderer targetSprite;
        readonly Transform targetSpriteMask;   // Used only for the fill functionalities of the SpriteRenderer bars
        readonly MicroBar bar;
        readonly MicroBarAnimation animation;

        public readonly IReadOnlyList<AnimCommand> Commands => commands;
        public readonly bool IsImage => isImage;
        public readonly Image TargetImage => targetImage;
        public readonly SpriteRenderer TargetSprite => targetSprite;
        public readonly Transform TargetSpriteMask => targetSpriteMask;
        public readonly MicroBar Bar => bar;
        public readonly MicroBarAnimation Animation => animation;

        internal AnimationInfo(IReadOnlyList<AnimCommand> commands, Image target, MicroBar bar, MicroBarAnimation animation)
        {
            this.commands = commands;
            isImage = true;
            targetImage = target;
            targetSprite = null;
            targetSpriteMask = null;
            this.bar = bar;
            this.animation = animation;
        }

        internal AnimationInfo(IReadOnlyList<AnimCommand> commands, SpriteRenderer target, Transform targetMask, MicroBar bar, MicroBarAnimation animation)
        {
            this.commands = commands;
            isImage = false;
            targetImage = null;
            targetSprite = target;
            targetSpriteMask = targetMask;
            this.bar = bar;
            this.animation = animation;
        }
    }
}
