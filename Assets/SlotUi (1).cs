using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUi : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI numberText;

    void Reset()
    {
        if (icon == null)
        {
            icon = transform.Find("icon")?.GetComponent<Image>();
        }
        if (numberText == null)
        {
            numberText = transform.Find("numberText")?.GetComponent<TextMeshProUGUI>();
        }
    }

    public void SetIcon(Sprite sprite, int numberOwn)
    {
        if (sprite == null)
        {
            if (icon != null) icon.enabled = false;
            if (icon != null) icon.sprite = null;
            if (numberText != null) numberText.enabled = false;
            return;
        }

        if (icon != null)
        {
            icon.enabled = true;
            icon.sprite = sprite;
        }

        if (numberText != null)
        {
            numberText.enabled = true;
            numberText.text = numberOwn.ToString();
        }
    }
}