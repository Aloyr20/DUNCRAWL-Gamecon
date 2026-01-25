using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        Object.FindFirstObjectByType<DialogueManager>().StartDialogue(dialogue);
    }
}
