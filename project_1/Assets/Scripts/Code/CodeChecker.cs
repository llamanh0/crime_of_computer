using UnityEngine;
using TMPro;
using UnityEditor.UI;

public class CodeChecker : MonoBehaviour
{
    public TMP_InputField codeInputField;
    public GameObject codePanel;
    public TMP_Text Message;

    // Bu fonksiyon butonun OnClick eventine bağlanacak
    public void CheckCode()
    {
        string userCode = codeInputField.text;
        
        // Örnek kontrol:
        // Oyuncudan "doorOpen = true;" ifadesini bekliyoruz diyelim.
        if (userCode.Contains("doorOpen = true;"))
        {
            Message.color = Color.green;
            Message.text = "KOD DOĞRU KAPI AÇILIYOR ...";

            // Burada ilgili kapıyı açan fonksiyonu çağırabilirsiniz.
            // codePanel.SetActive(false); diyerek paneli kapatabilirsiniz.
        }
        else
        {
            Message.color = Color.red;
            Message.text = "!!! HATALI KOD !!!";
        }
    }
}
