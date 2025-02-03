using Microlight.MicroBar;
using TMPro;
using UnityEngine;

class WUPlayerStat : MonoBehaviour
{
    private Camera _camera;
    public GameObject PlayerHead;
    public GameObject PlayerNameObj;
    public GameObject HealthBarObj;
    public GameObject CanvasObj;
    private AttackTarget _stat;
    private TMP_Text _playerName;
    private MicroBar _healthBar;
    public float UpOffest = 0.5f;
    public float RightOffest = 0.1f;
    private void Start()
    {
        _camera = Camera.main;
        _stat = GetComponent<AttackTarget>();
        _playerName = PlayerNameObj.GetComponent<TMP_Text>();
        _healthBar = HealthBarObj.GetComponent<MicroBar>();
        _healthBar.Initialize(_stat.MaxHealth);
    }
    private void Update()
    {
        CanvasObj.transform.position = PlayerHead.transform.position + Vector3.up * UpOffest + _camera.transform.right * RightOffest;
        CanvasObj.transform.forward = _camera.transform.forward;
        if (_stat.Health != _healthBar.CurrentValue)
        {
            _healthBar.UpdateBar(_stat.Health);
        }
    }
}