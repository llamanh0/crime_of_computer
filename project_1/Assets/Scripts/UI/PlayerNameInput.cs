using UnityEngine;
using TMPro;

public class PlayerNameInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private string defaultName = "Cabbar"; // Boş kalırsa varsayılan

    public void SavePlayerName()
    {
        string playerName = inputField.text;

        // Boş mu kontrolü
        if (string.IsNullOrWhiteSpace(playerName))
        {
            playerName = defaultName;
        }

        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
    }
}