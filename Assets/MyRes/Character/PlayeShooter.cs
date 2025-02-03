using System;
using System.Linq;
using UnityEngine;

class PlayerShooter : MonoBehaviour
{
    [Serializable]
    public struct AmmoRecoil
    {
        public int AmmoCount;
        public float UpForce;
        public float HorizontalForce;
        public float LeftProbability;
    }
    public AmmoRecoil[] AmmoRecoils;
    public float RecoilRecoveryRate = 1f;
    public float RecoilStability = 0.2f;
    public enum ReloadTypeEnum
    {
        Full, Single
    }
    public enum ShootModeEnum
    {
        Single, FullAuto
    }
    public ReloadTypeEnum ReloadType;
    public ShootModeEnum ShootMode;
    public float ReloadTime;
    public int MaxAmmo = 30;
    public float FireRate = 0.1f;
    private PlayerStat _stat;
    private float _lastFireTime;
    private PlayerController _controller;
    private Camera _camera;
    private void Start()
    {
        _stat = GetComponent<PlayerStat>();
        _controller = GetComponent<PlayerController>();
        _camera = _controller.CameraFollow.GetComponent<Camera>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _stat.CurrentAmmo != MaxAmmo)
        {
            _stat.IsReloading = true;
            Invoke(nameof(ReloadFinish), ReloadTime);
        }
        var isSignleShoot = ShootMode == ShootModeEnum.Single;
        var isPress = isSignleShoot ? Input.GetKeyDown(KeyCode.Mouse0) : Input.GetKey(KeyCode.Mouse0);
        if (isPress && _stat.CurrentAmmo > 0 &&
            (!_stat.IsReloading || ReloadType == ReloadTypeEnum.Single) &&
            Time.time - _lastFireTime > FireRate)
        {
            _stat.IsFiring = true;
            Fire();
            _lastFireTime = Time.time;
        }
        else if (!isPress || isSignleShoot)
        {
            _recoilAmmoCount = 0;
            _stat.IsFiring = false;
        }
    }
    private void LateUpdate()
    {
        _controller.LookPitchExtra = -_recoilCurrentUp;
        _controller.LookYawExtra = _recoilCurrentLeft;
        _recoilCurrentUp = Mathf.Lerp(_recoilCurrentUp, 0, Time.deltaTime * RecoilRecoveryRate);
        _recoilCurrentLeft = Mathf.Lerp(_recoilCurrentLeft, 0, Time.deltaTime * RecoilRecoveryRate);
    }
    private int _recoilAmmoCount;
    private float _recoilCurrentUp;
    private float _recoilCurrentLeft;
    private float RandomStability(float value, float stability)
    {
        return UnityEngine.Random.Range(value - stability, value + stability);
    }
    public int BulletDamage = 10;
    public float BaseSpreadRadius = 10f;
    private void Fire()
    {
        _stat.CurrentAmmo--;
        _recoilAmmoCount++;
        var recoilData = AmmoRecoils.First(data => data.AmmoCount > _recoilAmmoCount);
        _recoilCurrentUp += RandomStability(recoilData.UpForce, RecoilStability);
        var isLeft = UnityEngine.Random.Range(0, 1f) < recoilData.LeftProbability;
        _recoilCurrentLeft += RandomStability(isLeft ? recoilData.HorizontalForce : -recoilData.HorizontalForce, RecoilStability);
        var camera = _controller.CameraFollow.transform;
        var forward = GetDirectionInScreenCircle((_stat.Speed + 1) * (_stat.IsAiming ? 1 : BaseSpreadRadius));
        Client.Instance.SendFire(camera.position, forward);
        var (target, hit) = _stat.FireEffect(camera.position, forward);
        if (target != null)
        {
            var result = target.Damage(BulletDamage);
            var point = hit.point - forward * RandomStability(1f, 0.5f);
            Client.Instance.SendDamage(MultiPlayerManager.Instance.FindPlayerId(target.gameObject), point, (byte)result);
            WUDamageNumber.Instance.Create((byte)result, point);
        }
    }
    private Vector3 GetDirectionInScreenCircle(float radius)
    {
        var screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        var randomOffset = UnityEngine.Random.insideUnitCircle * radius;
        var screenPoint = new Vector3(
            screenCenter.x + randomOffset.x,
            screenCenter.y + randomOffset.y,
            _camera.nearClipPlane
        );
        Ray ray = _camera.ScreenPointToRay(screenPoint);
        return ray.direction.normalized;
    }
    private void ReloadFinish()
    {
        switch (ReloadType)
        {
            case ReloadTypeEnum.Full:
                _stat.CurrentAmmo = MaxAmmo;
                break;
            case ReloadTypeEnum.Single:
                _stat.CurrentAmmo++;
                if (_stat.CurrentAmmo < MaxAmmo && _stat.IsReloading)
                    Invoke(nameof(ReloadFinish), ReloadTime);
                break;
        }
        if (_stat.CurrentAmmo == MaxAmmo) _stat.IsReloading = false;
    }
}