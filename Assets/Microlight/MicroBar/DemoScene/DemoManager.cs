using UnityEngine;
using UnityEngine.UI;

namespace Microlight.MicroBar {
    public class DemoManager : MonoBehaviour {
        [Header("Prefabs")]
        [SerializeField] MicroBar Simple_MicroBar;
        [SerializeField] MicroBar Delayed_MicroBar;
        [SerializeField] MicroBar Disappear_MicroBar;
        [SerializeField] MicroBar Impact_MicroBar;
        [SerializeField] MicroBar Punch_MicroBar;
        [SerializeField] MicroBar Shake_MicroBar;

        [Header("Health Bar Holders")]
        [SerializeField] Transform leftBarHolder;
        [SerializeField] Transform rightBarHolder;

        [Header("Animators")]
        [SerializeField] Animator leftAnimator;
        [SerializeField] Animator rightAnimator;

        [Header("Type Text")]
        [SerializeField] Text typeText;

        [Header("Sounds")]
        [SerializeField] AudioClip hurtSound;
        [SerializeField] AudioClip healSound;
        [SerializeField] AudioSource soundSource;
        [SerializeField] Text soundButtonText;
        bool soundOn = false;

        MicroBar leftMicroBar;
        MicroBar rightMicroBar;

        const float MAX_HP = 100f;
        float hpLeft;
        float hpRight;

        int _currentType;
        int CurrentType {
            get => _currentType;
            set {
                _currentType = value;
                ChangeBars();
            }
        }

        private void Start() {
            CurrentType = 0;
        }

        #region Damage/Heal
        public void DamageLeft() {
            float damageAmount = Random.Range(5f, 15f);
            hpLeft -= damageAmount;
            if(hpLeft < 0f) hpLeft = 0f;
            soundSource.clip = hurtSound;
            if(soundOn) soundSource.Play();

            // Update HealthBar
            if(leftMicroBar != null) leftMicroBar.UpdateBar(hpLeft, false, UpdateAnim.Damage);
            leftAnimator.SetTrigger("Damage");
        }
        public void HealLeft() {
            float healAmount = Random.Range(5f, 15f);
            hpLeft += healAmount;
            if(hpLeft > MAX_HP) hpLeft = MAX_HP;
            soundSource.clip = healSound;
            if(soundOn) soundSource.Play();

            // Update HealthBar
            if(leftMicroBar != null) leftMicroBar.UpdateBar(hpLeft, false, UpdateAnim.Heal);
            leftAnimator.SetTrigger("Heal");
        }
        public void DamageRight() {
            float damageAmount = Random.Range(5f, 15f);
            hpRight -= damageAmount;
            if(hpRight < 0f) hpRight = 0f;
            soundSource.clip = hurtSound;
            if(soundOn) soundSource.Play();

            // Update HealthBar
            if(rightMicroBar != null) rightMicroBar.UpdateBar(hpRight, false, UpdateAnim.Damage);
            rightAnimator.SetTrigger("Damage");
        }
        public void HealRight() {
            float healAmount = Random.Range(5f, 15f);
            hpRight += healAmount;
            if(hpRight > MAX_HP) hpRight = MAX_HP;
            soundSource.clip = healSound;
            if(soundOn) soundSource.Play();

            // Update HealthBar
            if(rightMicroBar != null) rightMicroBar.UpdateBar(hpRight, false, UpdateAnim.Heal);
            rightAnimator.SetTrigger("Heal");
        }
        #endregion

        #region Choose Bar
        public void NextBarType() {
            if(CurrentType >= 5) {
                CurrentType = 0;
            }
            else {
                CurrentType++;
            }
        }
        public void PreviousBarType() {
            if(CurrentType <= 0) {
                CurrentType = 5;
            }
            else {
                CurrentType--;
            }
        }
        void ChangeBars() {
            hpLeft = MAX_HP;
            hpRight = MAX_HP;

            if(leftMicroBar != null) Destroy(leftMicroBar.gameObject);
            if(rightMicroBar != null) Destroy(rightMicroBar.gameObject);

            switch(CurrentType) {
                case 0:
                    leftMicroBar = Instantiate(Simple_MicroBar, leftBarHolder);
                    rightMicroBar = Instantiate(Simple_MicroBar, rightBarHolder);
                    typeText.text = "Simple";
                    break;
                case 1:
                    leftMicroBar = Instantiate(Delayed_MicroBar, leftBarHolder);
                    rightMicroBar = Instantiate(Delayed_MicroBar, rightBarHolder);
                    typeText.text = "Delayed";
                    break;
                case 2:
                    leftMicroBar = Instantiate(Disappear_MicroBar, leftBarHolder);
                    rightMicroBar = Instantiate(Disappear_MicroBar, rightBarHolder);
                    typeText.text = "Disappear";
                    break;
                case 3:
                    leftMicroBar = Instantiate(Impact_MicroBar, leftBarHolder);
                    rightMicroBar = Instantiate(Impact_MicroBar, rightBarHolder);
                    typeText.text = "Impact";
                    break;
                case 4:
                    leftMicroBar = Instantiate(Punch_MicroBar, leftBarHolder);
                    rightMicroBar = Instantiate(Punch_MicroBar, rightBarHolder);
                    typeText.text = "Punch";
                    break;
                case 5:
                    leftMicroBar = Instantiate(Shake_MicroBar, leftBarHolder);
                    rightMicroBar = Instantiate(Shake_MicroBar, rightBarHolder);
                    typeText.text = "Shake";
                    break;
            }

            if (leftMicroBar != null) leftMicroBar.Initialize(MAX_HP);
            if(rightMicroBar != null) rightMicroBar.Initialize(MAX_HP);
        }
        #endregion

        #region Sound
        public void ToggleSound() {
            soundOn = !soundOn;
            if(soundOn) soundButtonText.text = "On";
            else soundButtonText.text = "Off";
        }
        #endregion
    }
}