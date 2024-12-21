using UnityEngine;
using TMPro;

public class PlayerNameInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    public void SavePlayerName()
    {
        string playerName = inputField.text;
        PlayerPrefs.SetString("PlayerName", playerName); // İsmi kaydet
        PlayerPrefs.Save();
    }
}
