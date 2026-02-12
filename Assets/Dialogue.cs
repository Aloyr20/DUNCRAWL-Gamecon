using UnityEngine;

[System.Serializable]
public class Dialogue : MonoBehaviour
{
    public new string name;

    [TextArea(3, 10)]
    public string[] firstTimeSentences;

    [TextArea(3, 10)]
    public string[] openingLines;

    [TextArea(3, 10)]
    public string[] serviceLines;

    [TextArea(3, 10)]
    public string[] buyLines;

    [TextArea(3, 10)]
    public string[] sellLines;

    public int talkCount = 0;

    public string[] GetOpeningSet()
    {
        if (talkCount == 0)
        {
            return firstTimeSentences;
        }
        else
        {
            return new string[] { openingLines[Random.Range(0, openingLines.Length)] };

        }
    }

    public string[] GetServiceSet()
    {
        return new string[] { serviceLines[Random.Range(0, serviceLines.Length)] };
    }

    public string[] GetBuySet()
    {
        return new string[] { buyLines[Random.Range(0, buyLines.Length)] };
    }

    public string[] GetSellSet()
    {
        return new string[] { sellLines[Random.Range(0, sellLines.Length)] };
    }

    public void IncrementTalkCount()
    {
        talkCount++;
    }
}