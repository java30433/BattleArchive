using UnityEngine;

class PlayerStat : MonoBehaviour
{
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    public string PlayerName;
    public bool IsFiring;
    public float Speed;
    public bool IsAiming;
    public bool IsGrounded;
    public bool IsReloading;
    public int CurrentAmmo;
    public bool IsCrouch;

    public Animator Animator;
    private float _lastFootstepTime;
    public float MinDelayBetweenFootsteps = 0.3f;
    public float MaxDelayBetweenFootsteps = 0.6f;


    public GameObject FirePoint;
    public GameObject FireEffectRoot;
    public GameObject FireSmokeEffectRoot;
    public GameObject BulletObject;
    private Bullet _bulletScript;
    private ParticleSystem _gunFire;
    private ParticleSystem _smoke;
    public AttackTarget Target;

    private void Start()
    {
        Animator = GetComponent<Animator>();
        _gunFire = FireEffectRoot.GetComponent<ParticleSystem>();
        _smoke = FireSmokeEffectRoot.GetComponent<ParticleSystem>();
        _bulletScript = BulletObject.GetComponent<Bullet>();
        Target = GetComponent<AttackTarget>();
    }

    private void Update()
    {
        Animator.SetBool("IsFiring", IsFiring);
        Animator.SetBool("IsAiming", IsAiming);
        Animator.SetBool("IsReloading", IsReloading);
        Animator.SetBool("IsCrouch", IsCrouch);
        Animator.SetFloat("Speed", Speed);
        if (Time.time - _lastFootstepTime > Random.Range(MinDelayBetweenFootsteps, MaxDelayBetweenFootsteps) && Speed > 0)
        {
            _lastFootstepTime = Time.time;
            var index = Random.Range(0, FootstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.position, FootstepAudioVolume);
        }
    }

    public (AttackTarget target, RaycastHit hit) FireEffect(Vector3 startPoint, Vector3 forward)
    {
        _gunFire.Play();
        _smoke.Play();
        var targetPoint = startPoint + forward * 300;
        if (Physics.Raycast(startPoint, forward, out var hit))
        {
            targetPoint = hit.point;
        }
        _bulletScript.Shoot(FirePoint.transform.position, targetPoint);
        if (hit.collider != null)
        {
            var hitObj = hit.collider.gameObject;
            if (!hitObj.TryGetComponent<AttackTarget>(out var attackTarget))
            {
                BulletHoleManager.Instance.Create(targetPoint, hit.normal);
            }
            return (attackTarget, hit);
        } else {
            BulletHoleManager.Instance.Create(targetPoint, forward);
        }
        return (null, hit);
    }
}