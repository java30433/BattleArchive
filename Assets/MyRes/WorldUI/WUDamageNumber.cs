using System.Collections.Generic;
using TMPro;
using UnityEngine;

class WUDamageNumber : MonoBehaviour
{
    public static WUDamageNumber Instance { get; private set; }
    private TMP_Text _text;
    private bool _isShow;
    private Camera _camera;
    private List<EventPlayerDamage> _events = new();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        _text = GetComponent<TMP_Text>();
        _camera = Camera.main;
    }
    public void AddEvent(EventPlayerDamage e)
    {
        _events.Add(e);
    }
    public void Create(byte damage, Vector3 pos, bool isWeak = false)
    {
        var clone = Instantiate(gameObject, pos, Quaternion.identity);
        var cloneScript = clone.GetComponent<WUDamageNumber>();
        cloneScript._text.text = damage.ToString();
        cloneScript._isShow = true;
        Destroy(clone, 2f);
    }
    private void Update()
    {
        if (_isShow)
        {
            transform.forward = _camera.transform.forward;
            transform.position += 0.2f * Time.deltaTime * Vector3.up;
        }
        else
        {
            foreach (var e in _events)
            {
                if (e.SenderId == Client.Instance.SelfId) continue;
                Create(e.Number, e.Position);
            }
            _events.Clear();
        }
    }
}