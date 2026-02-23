using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUi : MonoBehaviour
{

    public Image icon;
    public TextMeshProUGUI numberText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Reset()
    {
        icon = transform.Find("icon")?.GetComponent<Image>();
        numberText = transform.Find("numberText")?.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetIcon(Sprite sprite, int NumberOwn)
    {
        if (sprite == null)
        {
            icon.enabled = false;
            icon.sprite = null;
            numberText.enabled = false;
            return;
        }

        icon.enabled = true;
        icon.sprite = sprite;

        numberText.enabled = true;
        numberText.text = NumberOwn.ToString();

    }

   
}
