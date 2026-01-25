using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Queue<string> sentences;
    public Text nameText;
    public Text dialogueText;


    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("Starting conversation with " +  dialogue.name);

        nameText.text = dialogue.name;  

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
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
        dialogueText.text = sentence;
        StopAllCoroutines();
        StartCoroutine(TypeByLetter(sentence));
    }

    IEnumerator TypeByLetter (string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    void StopDialogue()
    {
        Debug.Log("End of conversation.");
    }
}
