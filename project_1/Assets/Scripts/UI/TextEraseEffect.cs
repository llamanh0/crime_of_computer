using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TextEraseEffect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private Button changeButton;
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private string newText;
    [SerializeField] private bool allowSkip = true; // Efekti skip etmek ister misiniz?

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
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(ReplaceTextCoroutine());
        }
        else if (allowSkip)
        {
            // Eğer skip edilebilir olsun diyorsanız
            // Mevcut korutini durdurup direkt final metnini yazabiliriz
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            targetText.text = newText;
        }
    }

    private IEnumerator ReplaceTextCoroutine()
    {
        if (targetText == null) yield break;

        string current = targetText.text;

        // 1. Aşama: Mevcut yazıyı sil
        for (int i = current.Length; i >= 0; i--)
        {
            targetText.text = current.Substring(0, i);
            yield return new WaitForSeconds(speed);
        }

        // 2. Aşama: Yeni yazıyı yaz
        for (int i = 0; i <= newText.Length; i++)
        {
            targetText.text = newText.Substring(0, i);
            yield return new WaitForSeconds(speed);
        }
    }
}