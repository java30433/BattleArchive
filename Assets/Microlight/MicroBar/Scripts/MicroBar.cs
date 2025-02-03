using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microlight.MicroBar
{
    // ****************************************************************************************************
    // Base script for MicroBar, holds base functionality of the health bar
    // Manager for other features added to the health bar
    // ****************************************************************************************************
    public class MicroBar : MonoBehaviour
    {
        bool isInitalized = false;   // Safety system that gives out warning if health bar has not been initialized

        #region Properties
        float _previousValue = 1f;   // Value of HP that was before last update to the HP
        public float PreviousValue
        {
            get => _previousValue;
            private set
            {
                _previousValue = value;
            }
        }
        float _currentValue = 1f;   // Current value of HP
        public float CurrentValue
        {
            get => _currentValue;
            private set
            {
                PreviousValue = _currentValue;
                _currentValue = Mathf.Clamp(value, 0f, MaxValue);
                OnCurrentValueChange?.Invoke(this);
            }
        }
        float _maxValue = 1f;   // Max value of HP
        public float MaxValue
        {
            get => _maxValue;
            private set
            {
                _maxValue = value;
                OnMaxValueChange?.Invoke(this);
            }
        }
        public float HPPercent
        {
            get => Mathf.Clamp01(CurrentValue / MaxValue);
        }
        static MaxHealthCalculation _maxHealthCalculation = MaxHealthCalculation.FollowIncrease;
        public static MaxHealthCalculation MaxHealthCalculation
        {
            get => _maxHealthCalculation;
            private set
            {
                _maxHealthCalculation = value;
            }
        }
        #endregion

        #region Events
        public event Action<MicroBar> OnMaxValueChange;
        public event Action<MicroBar> OnCurrentValueChange;

        internal event Action<bool, UpdateAnim> OnNewMax;
        internal event Action<bool, UpdateAnim> OnBarUpdate;
        internal event Action BarDestroyed;
        internal event Action OnDefaultValuesSnapshot;
        #endregion

        [SerializeField] SimpleMicroBar simpleBar;   // Data stored about simple version of the bar
        [SerializeField] MicroBarEditorMode editorMode;   // Mode in which micro bar is
        [SerializeField] List<MicroBarAnimation> animations;   // Stores all animations for the bar
        public List<MicroBarAnimation> Animations => animations;

        private void OnDestroy()
        {
            BarDestroyed?.Invoke();
        }

        #region API
        /// <summary>
        /// Initializes health bar with max value and makes it useable
        /// </summary>
        /// <param name="maxValue">Health bar max value</param>
        public void Initialize(float maxValue)
        {
            isInitalized = true;
            _currentValue = maxValue;
            SetNewMaxHP(maxValue);

            if(editorMode == MicroBarEditorMode.Advanced)
            {
                foreach(MicroBarAnimation x in animations)
                {
                    if(!x.Initialize(this))
                    {
                        isInitalized = false;
                        Debug.LogError("[MicroBar] 'MicroBarAnimation' initialization failed");
                        return;
                    }
                }
            }
            else
            {
                if(simpleBar == null)
                {
                    Debug.LogError("[MicroBar] editorMode set to 'Simple' but 'simpleBar' is null");
                    return;
                }

                if(!simpleBar.Initialize(this))
                {
                    Debug.LogError("[MicroBar] 'SimpleMicroBar' initialization failed");
                    return;
                }
            }
        }

        /// <summary>
        /// Changes the way how is max health calculated.
        /// For more info look in MaxHealthCalculation enum
        /// </summary>
        public static void ChangeMaxHealthCalculation(MaxHealthCalculation maxHealthCalculation)
        {
            MaxHealthCalculation = maxHealthCalculation;
        }

        /// <summary>
        /// Sets new max value for the health bar
        /// </summary>
        /// <param name="skipAnimation">If true, it will skip all animations for changing max health bar value</param>
        public void SetNewMaxHP(float newMaxValue, bool skipAnimation = false)
        {
            if(!isInitalized)
            {
                Debug.LogWarning("[MicroBar] Not initialization");
                return;
            }
            // Don't allow negative max HP
            if(newMaxValue < 1f)
            {
                newMaxValue = 1f;
            }

            float previousHealth = MaxValue;
            float previousProportion = CurrentValue / MaxValue;
            MaxValue = newMaxValue;
            float change = MaxValue - previousHealth;

            // Calculate new CurrentHealth
            float newHP;
            switch(MaxHealthCalculation)
            {
                case MaxHealthCalculation.Follow:
                    // Don't allow current HP to go lower than 0
                    newHP = CurrentValue + change;
                    if(newHP < 1)
                    {
                        newHP = 1;
                    }
                    CurrentValue = newHP;
                    break;
                case MaxHealthCalculation.FollowIncrease:
                    if(change > 0)
                    {
                        CurrentValue += change;
                    }
                    break;
                case MaxHealthCalculation.Proportional:
                    // Don't allow current HP to go lower than 0
                    newHP = MaxValue * previousProportion;
                    if(newHP < 1)
                    {
                        newHP = 1;
                    }
                    CurrentValue = newHP;
                    break;
                case MaxHealthCalculation.Keep:
                default:
                    break;
            }

            OnNewMax?.Invoke(skipAnimation, UpdateAnim.MaxHP);
            UpdateBar(CurrentValue, true);
        }

        /// <summary>
        /// Updates the bar's visuals to a new HP value
        /// </summary>
        /// <param name="newHP">New HP value for the bar</param>
        /// <param name="skipAnimation">Will animation be skipped and visual set right to the new value?</param>
        /// <param name="updateType">Type of the animation that will be played (Damage, Heal, etc...)</param>
        public void UpdateBar(float newHP, bool skipAnimation = false, UpdateAnim updateType = UpdateAnim.Damage)
        {
            if(!isInitalized)
            {
                Debug.LogWarning("[MicroBar] Not initialization");
                return;
            }
            CurrentValue = newHP;
            OnBarUpdate?.Invoke(skipAnimation, updateType);
        }

        /// <summary>
        /// Animates the bar's visuals to a new HP value
        /// </summary>
        /// <param name="newHP">New HP value for the bar</param>
        /// <param name="updateType">Type of the animation that will be played (Damage, Heal, etc...)</param>
        public void UpdateBar(float newHP, UpdateAnim updateType)
        {
            UpdateBar(newHP, false, updateType);
        }

        /// <summary>
        /// Animates the bar's visuals to a new HP value and always uses Damage animation
        /// </summary>
        /// <param name="newHP">New HP value for the bar</param>
        public void UpdateBar(float newHP)
        {
            UpdateBar(newHP, false, UpdateAnim.Damage);
        }

        /// <summary>
        /// Snapshots current values of the image and rect transform to be used as new default values
        /// </summary>
        public void SnapshotDefaultValues()
        {
            if(!isInitalized)
            {
                Debug.LogWarning("[MicroBar] Not initialization");
                return;
            }
            OnDefaultValuesSnapshot?.Invoke();
        }
        #endregion
    }
}