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
    public Inventory inventory;

    [Header("Crosshair")]
    public Transform crosshairTransform;
    private Vector3 crosshairDefaultScale;
    public float crosshairMinScale = 0.5f;

    void Start()
    {
        if (crosshairTransform != null)
        {
            crosshairDefaultScale = crosshairTransform.localScale;
        }
    }

    void Update()
    {
        if (Inventory.IsDragging || (inventory != null && inventory.IsInventoryOpen()))
        {
            return;
        }

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

            if (chargePercent > 0.2f)
            {
                float chargedSpeed = _speed * Mathf.Lerp(0.5f, _maxChargeMultiplier, chargePercent);
                Fire(_muzzlePosition.transform.position, _projectileWeaponPrefab, chargedSpeed);
            }

            chargePercent = 0f;
            CancelInvoke("ArrowParticles");

            if (crosshairTransform != null)
            {
                crosshairTransform.localScale = crosshairDefaultScale;
            }
        }

        if (crosshairTransform != null && _isCharging)
        {
            float scale = Mathf.Lerp(1f, crosshairMinScale, chargePercent);
            crosshairTransform.localScale = crosshairDefaultScale * scale;
        }

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
        if (ArrowPool.Instance != null)
        {
            _projectileBullet = ArrowPool.Instance.Get(_Muzzle, transform.rotation);
        }
        else
        {
            _projectileBullet = Instantiate(_prefab, _Muzzle, transform.rotation);
        }

        if (_projectileBullet == null)
        {
            return;
        }

        Collider arrowCol = _projectileBullet.GetComponent<Collider>();
        Collider[] playerColliders = GetComponentsInParent<Collider>();
        foreach (Collider playerCol in playerColliders)
        {
            Physics.IgnoreCollision(arrowCol, playerCol);
        }

        Collider[] childColliders = GetComponentsInChildren<Collider>();
        foreach (Collider childCol in childColliders)
        {
            Physics.IgnoreCollision(arrowCol, childCol);
        }

        ShotScript shot = _projectileBullet.GetComponent<ShotScript>();
        shot.charge = chargePercent;

        Rigidbody rb = _projectileBullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = Camera.main.transform.forward;
            rb.linearVelocity = dir.normalized * _prefabSpeed;
        }
    }

    void ArrowParticles()
    {
        Arrow.GetComponent<ParticleSystem>().Play();
    }
}