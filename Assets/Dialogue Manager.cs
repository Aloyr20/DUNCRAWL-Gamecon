using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public Queue<string> sentences;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public GameObject dialogueUI;
    public GameObject otherUI;
    public GameObject canvas;
    public GameObject weapons;
    public TurnScript turn;

    public bool dialogueActive = false;
    public float dialogueSpeed = 0.03f;

    private bool isTyping = false;
    private string currentSentence = "";
    private bool skipTyping = false;
    private Dialogue currentDialogue;

    void Start()
    {
        sentences = new Queue<string>();
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false);
            otherUI.SetActive(true);
            canvas.SetActive(true);
            weapons.SetActive(true);
        }
    }

    void Update()
    {
        if (dialogueActive && Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                skipTyping = true;
            }
            else
            {
                ShowNextSentence();
            }
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        Time.timeScale = 0f;

        if (turn != null)
        {
            turn.enabled = false;
        }

        otherUI.SetActive(false);
        canvas.SetActive(false);
        weapons.SetActive(false);
        dialogueActive = true;
        nameText.text = dialogue.name;
        sentences.Clear();

        string[] currentSentences = dialogue.GetOpeningSet();

        foreach (string sentence in currentSentences)
        {
            sentences.Enqueue(sentence);
        }

        dialogue.IncrementTalkCount();

        if (dialogueUI != null)
        {
            dialogueUI.SetActive(true);
        }

        ShowNextSentence();
    }

    public void ShowNextSentence()
    {
        if (sentences.Count == 0)
        {
            StopDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        currentSentence = sentence;
        StopAllCoroutines();
        StartCoroutine(TypeByLetter(sentence));
    }

    System.Collections.IEnumerator TypeByLetter(string sentence)
    {
        isTyping = true;
        skipTyping = false;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            if (skipTyping)
            {
                dialogueText.text = sentence;
                break;
            }

            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(dialogueSpeed);
        }

        isTyping = false;
    }

    void StopDialogue()
    {
        dialogueActive = false;
        isTyping = false;

        Time.timeScale = 1.0f;

        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false);
        }

        EnableGame();
    }

    public void EnableGame()
    {
        if (turn != null)
        {
            turn.enabled = true;
        }

        otherUI.SetActive(true);
        canvas.SetActive(true);
        weapons.SetActive(true);
    }

    public bool IsDialogueActive()
    {
        return dialogueActive;
    }

    public void ShowServicePrompt()
    {
        if (currentDialogue != null)
        {
            sentences.Clear();
            foreach (string sentence in currentDialogue.GetServiceSet())
            {
                sentences.Enqueue(sentence);
            }
            ShowNextSentence();
        }
    }

    public void ShowBuyOptions()
    {
        if (currentDialogue != null)
        {
            sentences.Clear();
            foreach (string sentence in currentDialogue.GetBuySet())
            {
                sentences.Enqueue(sentence);
            }
            ShowNextSentence();
        }
    }

    public void ShowSellOptions()
    {
        if (currentDialogue != null)
        {
            sentences.Clear();
            foreach (string sentence in currentDialogue.GetSellSet())
            {
                sentences.Enqueue(sentence);
            }
            ShowNextSentence();
        }
    }

    public void ShowOpeningLine()
    {
        if (currentDialogue != null)
        {
            sentences.Clear();
            foreach (string sentence in currentDialogue.GetOpeningSet())
            {
                sentences.Enqueue(sentence);
            }
            ShowNextSentence();
        }
    }

    public void EndDialogue()
    {
        StopDialogue();
    }
}