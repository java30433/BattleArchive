using UnityEngine;

class HaloFollow : MonoBehaviour
{
    public GameObject PositionTarget;
    public GameObject ForwardTarget;
    public float SmoothSpeed = 3f;
    public float YOffset = 1f;
    public float Distance = 0.1f;
    private void Update()
    {
        var target = new Vector3(PositionTarget.transform.position.x, PositionTarget.transform.position.y + YOffset, PositionTarget.transform.position.z) - ForwardTarget.transform.forward * Distance;
        transform.forward = -ForwardTarget.transform.forward;
        transform.position = Vector3.Lerp(transform.position, target, SmoothSpeed * Time.deltaTime);
    }
}