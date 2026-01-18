using UnityEngine;

public class ProjectileSpells : MonoBehaviour
{
    [Header("Projectile")]
    public Transform _muzzlePosition;
    public float _speed = 10f;
    public float _lifePeriod = 3f;

    [Header("Charging")]
    public bool _isChargingLaunch = false;
    public float _maxChargeTime = 2f;
    public float _maxChargeMultiplier = 3f;

    private float _currentChargeTime = 0f;
    private bool _isCharging = false;

    void Update()
    {
        if (_isChargingLaunch)
        {
            HandleChargingMode();
        }
    }

    void HandleChargingMode()
    {
        if (Input.GetMouseButton(1))
        {
            _isCharging = true;
            _currentChargeTime = 0f;
        }

        if (Input.GetMouseButton(1) && _isCharging)
        {
            _currentChargeTime += Time.deltaTime;
            _currentChargeTime = Mathf.Min(_currentChargeTime, _maxChargeTime);
        }

        if (Input.GetMouseButton(1) && _isCharging)
        {
            _isCharging = false;
            float chargePercent = _currentChargeTime / _maxChargeTime;
            float chargedSpeed = _speed * Mathf.Lerp(1f, _maxChargeMultiplier, chargePercent);
            Fire(_muzzlePosition.gameObject, chargedSpeed);
        }
    }

    public void Fire(GameObject projectilePrefab, float projectileSpeed)
    {
        if (projectilePrefab == null)
        {
            return;
        }

        GameObject projectileObj = Instantiate(projectilePrefab, _muzzlePosition.position, _muzzlePosition.rotation);
        Rigidbody rb = projectileObj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 dir = Camera.main.transform.forward;
            rb.linearVelocity = dir.normalized * projectileSpeed;
        }

        Destroy(projectileObj, _lifePeriod);
    }
}
