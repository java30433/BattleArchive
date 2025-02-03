using DG.Tweening;
using UnityEngine;

namespace Microlight.MicroBar
{
    // ****************************************************************************************************
    // Command specifying the behavior and duration of a segment of an animation.
    // ****************************************************************************************************
    [System.Serializable]
    public class AnimCommand
    {
        internal AnimExecution Execution => execution;
        [SerializeField] AnimExecution execution = AnimExecution.Sequence;
        internal AnimEffect Effect => effect;
        [SerializeField] AnimEffect effect = AnimEffect.Scale;
        internal float Duration => duration;
        [SerializeField] float duration = 0f;
        internal float Delay => delay;
        [SerializeField] float delay = 0f;

        // Possible end values
        internal ValueMode ValueMode => valueMode;
        [SerializeField] ValueMode valueMode;
        internal float FloatValue => floatValue;
        [SerializeField] float floatValue;
        internal int IntValue => intValue;
        [SerializeField] int intValue;
        internal bool BoolValue => boolValue;
        [SerializeField] bool boolValue;
        internal Vector2 Vector2Value => vector2Value;
        [SerializeField] Vector2 vector2Value;
        internal Vector3 Vector3Value => vector3Value;
        [SerializeField] Vector3 vector3Value;
        internal Color ColorValue => colorValue;
        [SerializeField] Color colorValue;
        internal float PercentValue => percentValue;
        [SerializeField][Range(0f, 1f)] float percentValue;

        // Additional settings
        internal int Frequency => frequency;
        [SerializeField] int frequency = 10;
        internal Ease Ease => ease;
        [SerializeField] Ease ease = Ease.Linear;
        internal AnimAxis AnimAxis => animAxis;
        [SerializeField] AnimAxis animAxis;
        internal TransformProperties TransformProperty => transformProperty;
        [SerializeField] TransformProperties transformProperty;
    }
}
