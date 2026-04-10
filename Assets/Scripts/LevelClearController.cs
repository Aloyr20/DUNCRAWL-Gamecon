using UnityEngine;

public class LevelClearController : MonoBehaviour
{
    public int totalEnemiesWave1;
    public int totalEnemiesWave2;
    public int totalEnemiesWave3;
    public int totalEnemiesWave4;
    int enemiesKilled;
    int wave = 1;
    public GameObject Wave2Skeletons;
    public GameObject Wave3Skeletons;
    public GameObject Wave4Skeletons;
    public GameObject WinScreen;
    public GameObject LoseScreen;
    public GameObject door1;
    public GameObject door2;
    public LutController lutController;
    public TurnScript turn;
    public SceneController audioTrack;
    public PlayerHealth bleedPanel;


    private void Start()
    {
        Time.timeScale = 1f;
        Wave2Skeletons.SetActive(false);

        WinScreen.SetActive(false);
        LoseScreen.SetActive(false);
        door1.SetActive(true);
        door2.SetActive(true);
    }

    void Update()
    {
        if (wave == 1)
        {
            if (enemiesKilled == totalEnemiesWave1)
            {
                Wave1Complete();
            }
        }
        else if (wave == 2)
        {
            if (enemiesKilled == totalEnemiesWave2)
            {
                Wave2Complete();
            }
        }
        else if (wave == 3)
        {
            if (enemiesKilled == totalEnemiesWave3)
            {
                Wave3Complete();
            }
        }
        else if (wave == 4)
        {
            if (enemiesKilled == totalEnemiesWave4)
            {
                Wave4Complete();
            }
        }
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
    }

    void Wave1Complete()
    {
        enemiesKilled = 0;
        wave = 2;

        lutController.SetHorror();

        Wave2Skeletons.SetActive(true);

    }

    void Wave2Complete()
    {
        enemiesKilled = 0;
        wave = 3;

        lutController.SetOff();

        door1.SetActive(false);

        Wave3Skeletons.SetActive(true);
    }

    void Wave3Complete()
    {
        enemiesKilled = 0;
        wave = 4;

        door2.SetActive(false);

        Wave4Skeletons.SetActive(true);

        //Win();
    }

    void Wave4Complete()
    {
        enemiesKilled = 0;

        Win();
    }

    public void Win()
    {
        bleedPanel.StopAllCoroutines();
        bleedPanel.DestroyBleedPanels();
        turn.GetComponent<TurnScript>().enabled = false;
        WinScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }

    public void Lose()
    {
        bleedPanel.StopAllCoroutines();
        bleedPanel.DestroyBleedPanels();
        turn.GetComponent<TurnScript>().enabled = false;
        LoseScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }
}
