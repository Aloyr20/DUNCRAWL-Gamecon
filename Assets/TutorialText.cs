using UnityEngine;

public class TutorialText : MonoBehaviour
{
    public GameObject tutorialText;
    public GameObject block;
    public string playerTag = "Player";
    public bool useMovementKeys = false;
    public KeyCode dismissKey = KeyCode.Space;

    private bool playerInside = false;
    private bool dismissed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (dismissed)
        {
            return;
        }

        if (!other.CompareTag(playerTag))
        {
            return;
        }

        playerInside = true;
        tutorialText.SetActive(true);

        if (block != null)
        {
            block.SetActive(true);
        }
    }

    private void Update()
    {
        if (!playerInside || dismissed)
        {
            return;
        }

        bool pressed = false;

        if (useMovementKeys)
        {
            pressed = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                      Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D);
        }
        else
        {
            pressed = Input.GetKeyDown(dismissKey);
        }

        if (pressed)
        {
            dismissed = true;
            playerInside = false;
            tutorialText.SetActive(false);

            if (block != null)
            {
                block.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        playerInside = false;
        tutorialText.SetActive(false);
    }
}