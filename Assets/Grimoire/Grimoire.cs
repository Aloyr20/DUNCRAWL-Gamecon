using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrimoireUI : MonoBehaviour
{
    [Header("Grimoire Panels")]
    public GameObject grimoirePanel;
    public Image grimoireBackground;

    [Header("Grimoire Icon (HUD)")]
    public GameObject grimoireIcon;
    public Image grimoireIconImage;
    public Sprite iconNormal;
    public Sprite iconNotification;

    [Header("Navigation Arrows")]
    public Button leftArrowButton;
    public Button rightArrowButton;

    [Header("Left Page (Text)")]
    public TMP_Text spellNameText;
    public TMP_Text spellDescriptionText;

    [Header("Right Page (Image)")]
    public Image spellImage;

    [Header("Potions Page")]
    public Transform potionGridParent;
    public GameObject potionSlotPrefab;

    [Header("Spell Page Data")]
    public List<SpellPageData> spellPages = new List<SpellPageData>();

    [Header("Potion Data")]
    public List<PotionData> allPotions = new List<PotionData>();

    private int currentPageIndex = 0;
    private bool isOpen = false;
    private bool hasNewUnlock = false;

    private HashSet<string> unlockedSpells = new HashSet<string>();
    private HashSet<string> unlockedPotions = new HashSet<string>();
    private List<PageContent> activePages = new List<PageContent>();

    public static GrimoireUI Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        grimoirePanel.SetActive(false);
        UpdateIconState();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleGrimoire();
        }

        if (isOpen)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                PreviousPage();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                NextPage();
            }
        }
    }

    public void ToggleGrimoire()
    {
        if (isOpen)
            CloseGrimoire();
        else
            OpenGrimoire();
    }

    public void OpenGrimoire()
    {
        isOpen = true;
        grimoirePanel.SetActive(true);
        Time.timeScale = 0f;

        hasNewUnlock = false;
        UpdateIconState();

        RebuildActivePages();
        currentPageIndex = 0;
        DisplayCurrentPage();
    }

    public void CloseGrimoire()
    {
        isOpen = false;
        grimoirePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void NextPage()
    {
        if (activePages.Count == 0) return;

        currentPageIndex++;
        if (currentPageIndex >= activePages.Count)
            currentPageIndex = 0;

        DisplayCurrentPage();
    }

    public void PreviousPage()
    {
        if (activePages.Count == 0) return;

        currentPageIndex--;
        if (currentPageIndex < 0)
            currentPageIndex = activePages.Count - 1;

        DisplayCurrentPage();
    }

    private void DisplayCurrentPage()
    {
        if (activePages.Count == 0)
        {
            spellNameText.text = "Grimoire";
            spellDescriptionText.text = "No spells unlocked yet...";
            spellImage.gameObject.SetActive(false);
            potionGridParent.gameObject.SetActive(false);
            leftArrowButton.gameObject.SetActive(false);
            rightArrowButton.gameObject.SetActive(false);
            return;
        }

        PageContent page = activePages[currentPageIndex];

        leftArrowButton.gameObject.SetActive(activePages.Count > 1);
        rightArrowButton.gameObject.SetActive(activePages.Count > 1);

        if (page.isSpellPage)
        {
            spellNameText.text = page.title;
            spellDescriptionText.text = page.description;

            spellImage.gameObject.SetActive(true);
            spellImage.sprite = page.image;

            potionGridParent.gameObject.SetActive(false);
        }
        else
        {
            spellNameText.text = page.title;
            spellDescriptionText.text = page.description;

            spellImage.gameObject.SetActive(false);
            potionGridParent.gameObject.SetActive(true);

            PopulatePotionGrid();
        }
    }

    private void PopulatePotionGrid()
    {
        foreach (Transform child in potionGridParent)
        {
            Destroy(child.gameObject);
        }

        foreach (PotionData potion in allPotions)
        {
            if (unlockedPotions.Contains(potion.potionID))
            {
                GameObject slot = Instantiate(potionSlotPrefab, potionGridParent);
                Image slotImage = slot.GetComponent<Image>();
                if (slotImage != null)
                {
                    slotImage.sprite = potion.potionSprite;
                }

                Text slotText = slot.GetComponentInChildren<Text>();
                if (slotText != null)
                {
                    slotText.text = potion.potionName;
                }
            }
        }
    }

    public void UnlockSpell(string spellID)
    {
        if (unlockedSpells.Contains(spellID)) return;

        unlockedSpells.Add(spellID);
        hasNewUnlock = true;
        UpdateIconState();
    }

    public void UnlockPotion(string potionID)
    {
        if (unlockedPotions.Contains(potionID)) return;

        unlockedPotions.Add(potionID);
        hasNewUnlock = true;
        UpdateIconState();
    }

    public bool IsSpellUnlocked(string spellID)
    {
        return unlockedSpells.Contains(spellID);
    }

    private void RebuildActivePages()
    {
        activePages.Clear();

        foreach (SpellPageData spell in spellPages)
        {
            if (unlockedSpells.Contains(spell.spellID))
            {
                activePages.Add(new PageContent
                {
                    isSpellPage = true,
                    title = spell.spellName,
                    description = spell.description,
                    image = spell.spellSprite
                });
            }
        }

        if (unlockedPotions.Count > 0)
        {
            activePages.Add(new PageContent
            {
                isSpellPage = false,
                title = "Potions",
                description = "Your discovered potions.",
                image = null
            });
        }
    }

    private void UpdateIconState()
    {
        if (grimoireIconImage == null) return;

        grimoireIconImage.sprite = hasNewUnlock ? iconNotification : iconNormal;
    }
}

[System.Serializable]
public class SpellPageData
{
    public string spellID;
    public string spellName;
    [TextArea(3, 6)]
    public string description;
    public Sprite spellSprite;
}

[System.Serializable]
public class PotionData
{
    public string potionID;
    public string potionName;
    public Sprite potionSprite;
}

public class PageContent
{
    public bool isSpellPage;
    public string title;
    public string description;
    public Sprite image;
}