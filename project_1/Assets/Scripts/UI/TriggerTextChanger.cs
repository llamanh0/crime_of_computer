using UnityEngine;
using TMPro;
using System.Collections;

public class TriggerTextChanger : MonoBehaviour
{
    [Header("Text Settings")]
    [SerializeField] private TextMeshProUGUI text; // TMP Text referansı
    [SerializeField] private string newText = "İlerle!!!"; // Yeni yazılacak metin
    [SerializeField] private float eraseSpeed = 0.1f; // Harf silme hızı
    [SerializeField] private float typeSpeed = 0.1f; // Yeni yazıyı yazma hızı

    private bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isTriggered && collision.CompareTag("Player")) // Player tag'i kontrolü
        {
            isTriggered = true;
            StartCoroutine(HandleTextChange());
        }
    }

    private IEnumerator HandleTextChange()
    {
        // Metni harf harf sil
        for (int i = text.text.Length; i >= 0; i--)
        {
            text.text = text.text.Substring(0, i);
            yield return new WaitForSeconds(eraseSpeed);
        }

        // Yeni metni harf harf yaz
        for (int i = 0; i <= newText.Length; i++)
        {
            text.text = newText.Substring(0, i);
            yield return new WaitForSeconds(typeSpeed);
        }

        // Trigger objesini yok et
        Destroy(gameObject);
    }
}
