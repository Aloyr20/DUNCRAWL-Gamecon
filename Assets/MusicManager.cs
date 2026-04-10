using UnityEngine;

public class DynamicMusic : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource explorationSource;
    public AudioSource combatSource;

    [Header("Audio Clips")]
    public AudioClip explorationTrack;
    public AudioClip combatTrack;

    [Header("Volume Settings")]
    public float explorationMaxVolume = 0.4f;
    public float combatMaxVolume = 0.45f;

    [Header("Combat Detection")]
    public Transform player;
    public float horizontalRange = 20f;
    public float verticalRange = 10f;
    public LayerMask enemyLayer;

    [Header("Transition")]
    public float fadeSpeed = 2f;

    private float combatTargetVolume = 0f;
    private Collider[] detectedEnemies = new Collider[20];

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        explorationSource.clip = explorationTrack;
        explorationSource.loop = true;
        explorationSource.volume = explorationMaxVolume;
        explorationSource.Play();

        combatSource.clip = combatTrack;
        combatSource.loop = true;
        combatSource.volume = 0f;
        combatSource.Play();
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        bool inCombat = CheckForEnemies();

        if (inCombat)
        {
            combatTargetVolume = combatMaxVolume;
        }
        else
        {
            combatTargetVolume = 0f;
        }

        combatSource.volume = Mathf.MoveTowards(combatSource.volume, combatTargetVolume, fadeSpeed * Time.deltaTime);
    }

    private bool CheckForEnemies()
    {
        Vector3 halfExtents = new Vector3(horizontalRange, verticalRange, horizontalRange);
        int count = Physics.OverlapBoxNonAlloc(player.position, halfExtents, detectedEnemies, Quaternion.identity, enemyLayer);
        return count > 0;
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            Vector3 size = new Vector3(horizontalRange * 2f, verticalRange * 2f, horizontalRange * 2f);
            Gizmos.DrawWireCube(player.position, size);
        }
    }
}