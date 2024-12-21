using UnityEngine;
using TMPro;
using System.Collections;

public class TriggerTextChanger : MonoBehaviour
{
    [Header("Text Settings")]
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private string newText = "İlerle!!!";
    [SerializeField] private float eraseSpeed = 0.1f;
    [SerializeField] private float typeSpeed = 0.1f;

    private bool isTriggered = false;

    private void Start()
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "Cabbar");
        newText = newText.Replace("{name}", playerName);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isTriggered && collision.CompareTag("Player"))
        {
            isTriggered = true;
            StartCoroutine(HandleTextChange());
        }
    }

    private IEnumerator HandleTextChange()
    {
        if (targetText == null) yield break;

        // Metni harf harf sil
        for (int i = targetText.text.Length; i >= 0; i--)
        {
            targetText.text = targetText.text.Substring(0, i);
            yield return new WaitForSeconds(eraseSpeed);
        }

        // Yeni metni harf harf yaz
        for (int i = 0; i <= newText.Length; i++)
        {
            targetText.text = newText.Substring(0, i);
            yield return new WaitForSeconds(typeSpeed);
        }

        // İş bittikten sonra tetikleyiciyi yok et
        Destroy(gameObject);
    }
}