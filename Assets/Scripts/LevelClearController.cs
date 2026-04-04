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
    public GameObject door;
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
        door.SetActive(true);

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
            }
        }
        else if (wave == 2)
        {
            if (enemiesKilled == totalEnemiesWave2)
            {
                Wave2Complete();
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
        enemiesKilled = 0;
        wave = 2;

        lutController.SetHorror();

        Wave2Skeletons.SetActive(true);

    }

    void Wave2Complete()
    {
        enemiesKilled = 0;

        lutController.SetOff();

        door.SetActive(false);

        //Win();
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
