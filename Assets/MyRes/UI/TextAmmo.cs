using TMPro;
using UnityEngine;

class TextAmmo : MonoBehaviour
{
    public GameObject PlayerObj;
    private PlayerStat _stat;
    private TMP_Text _text;
    private void Start()
    {
        _stat = PlayerObj.GetComponent<PlayerStat>();
        _text = GetComponent<TMP_Text>();
    }
    private void Update()
    {
        _text.text = _stat.CurrentAmmo.ToString();
    }
}