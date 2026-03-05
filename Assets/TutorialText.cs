using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TutorialText: MonoBehaviour
{
    public GameObject tutorialText;
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            tutorialText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            tutorialText.SetActive(false);
        }
    }
}
