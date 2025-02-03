using UnityEngine;
using UnityEngine.EventSystems;
class PlayerController : MonoBehaviour
{
    public float WalkSpeed = 2f;
    public float SprintSpeed = 4.5f;
    public float AimSpeed = 1.7f;
    public float FireSpeed = 1.2f;
    public float ReloadingSpeed = 1.8f;
    public float SpeedChangeRate = 10f;
    public float RotationSmoothTime = 0.1f;
    public GameObject CameraFollow;
    public float CameraYMax = 50f;
    public float CameraYMin = -30f;

    public float CameraDistance = 2f;
    public float CameraHeight = 1f;
    public float CameraOffestRight = 0.2f;
    public float CameraAimScale = 2f;
    public float CameraFOV = 60;
    public float ScaleChangeRate = 10f;
    public bool IsGrounded;

    private static float _speedOffest = 0.1f;
    private bool _isSprinting;
    private PlayerStat _stat;
    private CharacterController _controller;
    private PlayerShooter _shooter;
    private Camera _camera;
    public float MouseSensitivity = 200f;
    private void Start()
    {
        _stat = GetComponent<PlayerStat>();
        _controller = GetComponent<CharacterController>();
        _shooter = GetComponent<PlayerShooter>();
        _camera = CameraFollow.GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public float GroundedOffset = -0.1f;
    public float GroundedRadius = 0.12f;
    public LayerMask GroundLayers;
    public float Gravity = -15f;
    private float _verticalVelocity;

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        IsGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }

    private void Update()
    {
        GroundedCheck();
        _verticalVelocity += Gravity * Time.deltaTime;
        if (IsGrounded)
        {
            _verticalVelocity = 0f;
        }
        //_stat.IsCrouch = Input.GetKey(KeyCode.LeftControl);
        _stat.IsAiming = Input.GetKey(KeyCode.Mouse1);
        _isSprinting = Input.GetKey(KeyCode.LeftShift);
        var targetSpeed = _stat.IsReloading ? ReloadingSpeed : _stat.IsFiring ? FireSpeed : _stat.IsAiming ? AimSpeed : _isSprinting ? SprintSpeed : WalkSpeed;
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        var currentSpeed = new Vector2(_controller.velocity.x, _controller.velocity.z).magnitude;
        if (currentSpeed < targetSpeed - _speedOffest || currentSpeed > targetSpeed + _speedOffest)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
        }
        else
        {
            currentSpeed = targetSpeed;
        }
        var targetRotation = 0f;
        if (input != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + CameraFollow.transform.eulerAngles.y;
            if (!_stat.IsAiming && !Input.GetKey(KeyCode.Mouse0))
            {
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _rotationVelocity, RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        } else {
            currentSpeed = 0;
        }
        var targetDirection = (Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward).normalized;
        _controller.Move(targetDirection * (currentSpeed * Time.deltaTime) - new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        _stat.Speed = currentSpeed;
    }
    private void FixedUpdate()
    {
        Client.Instance.SendMove(_stat);
    }
    public float LookPitchExtra;
    public float LookYawExtra;
    private float _lookPitch;
    private float _lookYaw;
    private float _rotationVelocity;
    private void LateUpdate()
    {
        var mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        var mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;
        _lookPitch = ClampAngle(_lookPitch - mouseY, CameraYMin, CameraYMax);
        _lookYaw += mouseX;
        var lookYaw = _lookYaw + LookYawExtra;
        if (_stat.IsAiming)
        {
            var target = CameraFOV / CameraAimScale;
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, target, Time.deltaTime * ScaleChangeRate);
        }
        else if (_camera.fieldOfView != CameraFOV)
        {
            _camera.fieldOfView = _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, CameraFOV, Time.deltaTime * ScaleChangeRate);
        }
        if (_stat.Speed < _speedOffest || _stat.IsAiming || Input.GetKey(KeyCode.Mouse0))
        {
            var targetRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, lookYaw, ref _rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, targetRotation, transform.eulerAngles.z);
        }
        CameraFollow.transform.rotation = Quaternion.Euler(_lookPitch + LookPitchExtra, lookYaw, 0.0f);
        ShelterTest();
        CameraFollow.transform.position = transform.position
            - CameraFollow.transform.forward * _cameraDistanceOverride
            + CameraFollow.transform.right * CameraOffestRight
            + Vector3.up * CameraHeight;
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    private float _cameraDistanceOverride;
    void ShelterTest()
	{
		float characterHeight = 1f;
        var cameraTrans = CameraFollow.transform;
		var targetHeadPos = new Vector3(transform.position.x, transform.position.y + characterHeight, transform.position.z);
 
		 Ray[] testRays = new Ray[5];
		 testRays[0] = new Ray(targetHeadPos, cameraTrans.position + 0.8f * cameraTrans.right + 0.5f * cameraTrans.up - targetHeadPos);
		 testRays[1] = new Ray(targetHeadPos, cameraTrans.position + 0.8f * cameraTrans.right - 0.5f * cameraTrans.up - targetHeadPos);
		 testRays[2] = new Ray(targetHeadPos, cameraTrans.position - 0.8f * cameraTrans.right + 0.5f * cameraTrans.up - targetHeadPos);
		 testRays[3] = new Ray(targetHeadPos, cameraTrans.position - 0.8f * cameraTrans.right - 0.5f * cameraTrans.up - targetHeadPos);
		 
		 testRays[4] = new Ray(transform.position, cameraTrans.position - targetHeadPos);
 
		float castDist = (cameraTrans.position - targetHeadPos).magnitude;
		float[] dists = new float[5]; 
		for (int i = 0; i < 5; i++)
		{
			if (Physics.Raycast(testRays[i], out RaycastHit result, castDist))
			{
				//Debug.DrawLine(targetHeadPos, result.point, Color.red);
				dists[i] = Vector3.Distance(result.point, targetHeadPos);
			}else
			{
				//Debug.DrawLine(targetHeadPos, targetHeadPos + castDist * testRays[i].direction, Color.blue);
				dists[i] = castDist;
			}
		}
 
		float minDist0 = Mathf.Min(dists[0], dists[1]);
		float minDist1 = Mathf.Min(dists[2], dists[3]);
		float minDist2 = Mathf.Min(minDist0, minDist1);
		float minDist = Mathf.Min(minDist2, dists[4]);
 
		_cameraDistanceOverride = Mathf.Min(minDist, CameraDistance);
	}
}