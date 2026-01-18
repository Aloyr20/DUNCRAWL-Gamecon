using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject _projectileWeaponPrefab;
    public GameObject _muzzlePosition;
    public float _speed = 10f;             
    public float _lifePeriod = 3f;
    public LayerMask _ignoreLayer;

    public bool _isChargingLaunch = false; 
    public float _maxChargeTime = 2f;      
    public float _maxChargeMultiplier = 3f;

    private float _currentChargeTime = 0f;
    private bool _isCharging = false;
    private GameObject _projectileBullet;

    float chargePercent = 0f;

    public Animator BowAnim;
    public GameObject Arrow;

    void Update()
    {
        if (_isChargingLaunch)
        {
            HandleChargingMode();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Fire(_muzzlePosition.transform.position, _projectileWeaponPrefab, _speed);
            }
        }
    }

    void HandleChargingMode()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isCharging = true;
            _currentChargeTime = 0f;

            Invoke("ArrowParticles", _maxChargeTime);
        }

        if (Input.GetMouseButton(0) && _isCharging)
        {
            _currentChargeTime += Time.deltaTime;
            _currentChargeTime = Mathf.Min(_currentChargeTime, _maxChargeTime);
            chargePercent = _currentChargeTime / _maxChargeTime;
        }

        if (Input.GetMouseButtonUp(0) && _isCharging)
        {
            _isCharging = false;
            chargePercent = _currentChargeTime / _maxChargeTime;
            float chargedSpeed = _speed * Mathf.Lerp(1f, _maxChargeMultiplier, chargePercent);


            if (chargePercent > 0.2f)
            {
                Fire(_muzzlePosition.transform.position, _projectileWeaponPrefab, chargedSpeed);
            }
            chargePercent = 0f;

            CancelInvoke("ArrowParticles");
        }

        //bow visuals
        BowAnim.Play("Draw", 0, chargePercent * 0.4f);

        if (chargePercent == 0)
        {
            Arrow.SetActive(false);
        }
        else
        {
            Arrow.SetActive(true);
            Vector3 arrowPos = Arrow.transform.localPosition;
            arrowPos.z = 0 + (-0.4f * chargePercent);
            Arrow.transform.localPosition = arrowPos;
        }
    }

    void Fire(Vector3 _Muzzle, GameObject _prefab, float _prefabSpeed)
    {
        _projectileBullet = Instantiate(_prefab, _Muzzle, transform.rotation);
        Rigidbody rb = _projectileBullet.GetComponent<Rigidbody>();
        _projectileBullet.GetComponent<ShotScript>().charge = chargePercent;

        Vector3 dir = Camera.main.transform.forward;

        if (rb != null)
        {
            rb.linearVelocity = dir.normalized * _prefabSpeed;
            Destroy(_projectileBullet, _lifePeriod);
        }
    }

    void ArrowParticles()
    {
        Arrow.GetComponent<ParticleSystem>().Play();
    }
}
