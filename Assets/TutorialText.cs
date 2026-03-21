using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TutorialText: MonoBehaviour
{
    public GameObject tutorialText;
    public GameObject block;
    public string playerTag = "Player";
    public int textCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            tutorialText.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag(playerTag))
        {

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
