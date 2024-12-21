using UnityEngine;
using TMPro;

/// <summary>
/// Oyuncu adının girilmesini ve PlayerPrefs'e kaydedilmesini sağlar.
/// Boş bırakılırsa varsayılan bir isim atar.
/// </summary>
public class PlayerNameInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private string defaultName = "Cabbar"; // Eğer boş kalırsa kullanılacak

    public void SavePlayerName()
    {
        string playerName = inputField.text;

        if (string.IsNullOrWhiteSpace(playerName))
        {
            playerName = defaultName;
        }

        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
    }
}