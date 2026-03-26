using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TutorialText: MonoBehaviour
{
    public GameObject tutorialText;
    public GameObject block;
    public string playerTag = "Player";
    public int textCount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            tutorialText.SetActive(true);
            block.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(playerTag) && textCount == 1 && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
        {
            tutorialText.SetActive(false);
            block.SetActive(false);
            textCount++;
        }
        if (other.CompareTag(playerTag) && textCount == 2 && Input.GetKeyDown(KeyCode.Space))
        {
            tutorialText.SetActive(false);
            block.SetActive(false);
            textCount++;
        }
        if (other.CompareTag(playerTag) && textCount == 3 && Input.GetKeyDown(KeyCode.LeftShift))
        {
            tutorialText.SetActive(false);
            block.SetActive(false);
            textCount++;
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
