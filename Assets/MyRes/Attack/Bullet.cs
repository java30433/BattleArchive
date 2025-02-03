using UnityEngine;

class Bullet : MonoBehaviour
{
    public float Speed = 10f;
    private bool _isShooted;
    private Vector3 _targetPoint;
    private float _targetDistance;
    private float _currentDistance;

    public void Shoot(Vector3 firePoint, Vector3 targetPoint)
    {
        var bullet = Instantiate(gameObject);
        bullet.transform.position = firePoint;
        _targetPoint = targetPoint;
        bullet.transform.LookAt(targetPoint);
        var bulletScript = bullet.GetComponent<Bullet>();
        bulletScript._isShooted = true;
        bulletScript._targetDistance = Vector3.Distance(firePoint, targetPoint);
    }
    private void Update()
    {
        if (_isShooted)
        {
            var delta = Speed * Time.deltaTime;
            transform.Translate(Vector3.forward * delta);
            _currentDistance += delta;
            if (_currentDistance >= _targetDistance)
            {
                Destroy(gameObject);
            }
        }
    }
}