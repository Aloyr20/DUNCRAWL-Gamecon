using UnityEngine;

public class LevelClearController : MonoBehaviour
{
    public int totalEnemiesWave1;
    public int totalEnemiesWave2;
    int enemiesKilled;
    int wave = 1;
    public GameObject Wave2Skeletons;
    public GameObject WinScreen;
    public GameObject LoseScreen;
    public GameObject Arrow;
    public LutController lutController;
    public TurnScript turn;
    public SceneController audioTrack;

    private void Start()
    {
        Time.timeScale = 1f;
        Wave2Skeletons.SetActive(false);

        WinScreen.SetActive(false);
        LoseScreen.SetActive(false);
        Arrow.SetActive(false);

    }

    void Update()
    {
        if (wave == 1)
        {
            if (enemiesKilled == totalEnemiesWave1)
            {
                Wave1Complete();
                audioTrack.music[0].Stop();
                audioTrack.music[1].Play();
                Arrow.SetActive(true);
            }
        }
        else if (wave == 2)
        {
            if (enemiesKilled == totalEnemiesWave2)
            {
                Wave2Complete();
                Arrow.SetActive(false);
                audioTrack.music[1].Stop();
                audioTrack.music[0].Play();
            }
        }
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
    }

    void Wave1Complete()
    {
        Debug.Log("Wave 1 complete");
        enemiesKilled = 0;
        wave = 2;

        lutController.SetHorror();

        Wave2Skeletons.SetActive(true);

    }

    void Wave2Complete()
    {
        Debug.Log("Wave 2 complete");
        enemiesKilled = 0;

        lutController.SetOff();

        Win();
    }

    public void Win()
    {
        Time.timeScale = 0f;
        turn.GetComponent<TurnScript>().enabled = false;
        Cursor.lockState = CursorLockMode.Confined;
        WinScreen.SetActive(true);
    }

    public void Lose()
    {
        Time.timeScale = 0f;
        turn.GetComponent<TurnScript>().enabled = false;
        Cursor.lockState = CursorLockMode.Confined;
        LoseScreen.SetActive(true);
    }
}
