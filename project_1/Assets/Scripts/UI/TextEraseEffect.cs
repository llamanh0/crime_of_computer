using UnityEngine;
using TMPro; // TextMeshPro desteği
using UnityEngine.UI; // Button desteği
using System.Collections;

public class TextEraseEffect : MonoBehaviour
{
    public TextMeshProUGUI targetText; // Hedef TextMeshPro yazısı
    public Button changeButton; // Buton referansı
    public float speed = 0.1f; // Silme ve yazma hızı
    public string newText; // Yeni metin
    private bool hasTextBeenChanged = false; // Metnin değişip değişmediğini takip eder

    void Start()
    {
        // Butona tıklama olayını bağla
        changeButton.onClick.AddListener(StartReplace);
    }

    private void StartReplace()
    {
        if (!hasTextBeenChanged) // Eğer metin değiştirilmediyse
        {
            hasTextBeenChanged = true; // İşaretle
            StartCoroutine(ReplaceTextCoroutine());
        }
    }

    private IEnumerator ReplaceTextCoroutine()
    {
        // 1. Aşama: Mevcut yazıyı silme
        string currentText = targetText.text; // Mevcut metni al
        for (int i = currentText.Length; i >= 0; i--)
        {
            targetText.text = currentText.Substring(0, i); // Yazıyı kısalt
            yield return new WaitForSeconds(speed); // Bekleme süresi
        }

        // 2. Aşama: Yeni yazıyı yazma
        for (int i = 0; i <= newText.Length; i++)
        {
            targetText.text = newText.Substring(0, i); // Yeni yazıyı oluştur
            yield return new WaitForSeconds(speed); // Bekleme süresi
        }
    }
}
