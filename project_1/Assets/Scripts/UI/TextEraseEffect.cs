using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Bir metni harf harf silip, ardından yeni bir metni harf harf yazma efektini uygular.
/// Tek bir seferlik veya butonla tekrar tetiklenebilir.
/// </summary>
public class TextEraseEffect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private Button changeButton;
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private string newText;
    [SerializeField] private bool allowSkip = true; // Efekti ikinci basışta hızlıca tamamlamak ister misin?

    private bool hasTextBeenChanged = false;
    private Coroutine currentCoroutine;

    private void Start()
    {
        if (changeButton != null)
        {
            changeButton.onClick.AddListener(StartReplace);
        }
    }

    private void StartReplace()
    {
        if (!hasTextBeenChanged)
        {
            hasTextBeenChanged = true;

            // Eski coroutine varsa durdur
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(ReplaceTextCoroutine());
        }
        else if (allowSkip)
        {
            // Eğer skip edilebilir olsun diyorsanız
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            targetText.text = newText; // Anında final metnini yaz
        }
    }

    private IEnumerator ReplaceTextCoroutine()
    {
        if (targetText == null) yield break;

        string currentText = targetText.text;

        // 1. Aşama: Mevcut yazıyı harf harf sil
        for (int i = currentText.Length; i >= 0; i--)
        {
            targetText.text = currentText.Substring(0, i);
            yield return new WaitForSeconds(speed);
        }

        // 2. Aşama: Yeni yazıyı harf harf yaz
        for (int i = 0; i <= newText.Length; i++)
        {
            targetText.text = newText.Substring(0, i);
            yield return new WaitForSeconds(speed);
        }
    }
}