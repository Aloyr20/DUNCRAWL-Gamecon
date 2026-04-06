using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    Collider[] _enemies;
    public float _radius = 5.0f;
    public LayerMask _enemyLayer;
    public float reach;

    public int damage = 20;

    public Transform PlayerTransform;

    public Animator SwordAnim;
    public float SwingDuration;
    string currentstate;

    bool stopSlash = true;

    public AudioSource sourceSwing;
    public AudioSource sourceHit;
    public AudioClip[] sound;


    void Start()
    {
        AnimatorStateInfo info = SwordAnim.GetCurrentAnimatorStateInfo(0);
        currentstate = GetCurrentStateName(info);
    }


    void Update()
    {
        AnimatorStateInfo info = SwordAnim.GetCurrentAnimatorStateInfo(0);

        if (Input.GetMouseButtonDown(0) && stopSlash)
        {
            stopSlash = false;
        }

        if (stopSlash)
        {
            SwordAnim.speed = 0f;
        }
        else
        {
            SwordAnim.speed = 1f;
        }

        if ((info.IsName(currentstate) == false) && GetCurrentStateName(info) == "Slash")
        {
            //just started slashing
            if (!sourceSwing.isPlaying)
            {
                sourceSwing.PlayOneShot(sound[0], 1.5f);
            }
            EnemyDetermine();

        }
        if ((info.IsName(currentstate) == false) && GetCurrentStateName(info) == "Windup")
        {
            //just started windup
            stopSlash = true;

        }

        currentstate = GetCurrentStateName(info);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + (PlayerTransform.forward * reach), _radius);
    }

    void EnemyDetermine()
    {
        if (_enemies == null)
        {
            _enemies = new Collider[10];
        }

        int hitCount = Physics.OverlapSphereNonAlloc(transform.position + (PlayerTransform.forward * reach), _radius, _enemies, _enemyLayer);

        for (int i = 0; i < hitCount; i++)
        {
            Collider enemy = _enemies[i];

            if (!sourceHit.isPlaying)
            {
                sourceHit.PlayOneShot(sound[1], 1.4f);
            }

            if (enemy.name == "Dummy")
            {
                enemy.GetComponent<DummyEnemy>().TakeDamage();
                return;
            }

            enemy.GetComponent<EnemyHP>().TakeDamage(damage);
        }
    }

    string GetCurrentStateName(AnimatorStateInfo info)
    {
        if (info.IsName("Windup")) return "Windup";
        if (info.IsName("Slash")) return "Slash";
        if (info.IsName("Recovery")) return "Recovery";
        return "Unknown";
    }
}
