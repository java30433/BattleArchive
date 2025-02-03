using System.Collections.Generic;
using UnityEngine;

class BulletHoleManager : MonoBehaviour
{
    public float LifeTime = 5f;
    public static BulletHoleManager Instance { get; private set;}
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public void Create(Vector3 targetPosition, Vector3 forward)
    {
        var clone = Instantiate(this);
        clone.transform.position = targetPosition;
        clone.transform.forward = forward;
        clone.transform.Translate(forward * -0.1f);
        Destroy(clone.gameObject, LifeTime);
    }
}