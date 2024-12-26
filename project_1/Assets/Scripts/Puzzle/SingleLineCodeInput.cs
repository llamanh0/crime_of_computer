using TMPro;
using UnityEngine;

public class SingleLineOutput : MonoBehaviour
{
    [SerializeField] private TMP_Text outputText; // UI Text referansı

    public void DisplayOutput(string message)
    {
        if (outputText != null)
        {
            outputText.text = message;
        }
        else
        {
            Debug.LogError("SingleLineOutput: OutputText referansı eksik!");
        }
    }
}
