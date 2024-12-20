using UnityEngine;
using TMPro;
using System.Collections;

public class SelectionColorChanger : MonoBehaviour
{
    public TMP_InputField inputField;
    public Color normalColor = Color.green;
    public Color selectedColor = Color.black;

    private string originalText = "";

    void Start()
    {
        // Input field başlarken normal rengi uygula
        inputField.textComponent.color = normalColor;

        // Olaylara abone ol
        inputField.onTextSelection.AddListener(OnTextSelection);
        inputField.onEndTextSelection.AddListener(OnEndTextSelection);
    }

    void OnTextSelection(string text, int start, int length)
    {
        // Halen bir seçim yapıldığında bu çalışır
        UpdateSelectedTextColor(start, length);
    }

    void OnEndTextSelection(string text, int start, int length)
    {
        // Seçim bittiğinde de güncelleyebilirsiniz
        UpdateSelectedTextColor(start, length);
    }

    void UpdateSelectedTextColor(int start, int length)
    {
        string fullText = inputField.text;
        int textLength = fullText.Length;

        // Uzunluk kontrolleri
        if (start < 0) start = 0;
        if (length < 0) length = 0;
        if (start > textLength) start = textLength;
        
        int end = start + length;
        if (end > textLength)
        {
            end = textLength;
            length = textLength - start; // length'i de güncelle
        }

        // Seçim yoksa
        if (length == 0)
        {
            inputField.text = $"<color=#{ColorUtility.ToHtmlStringRGB(normalColor)}>{fullText}</color>";
            return;
        }

        string before = fullText.Substring(0, start);
        string selected = fullText.Substring(start, length);
        string after = fullText.Substring(end);

        string beforeColored = $"<color=#{ColorUtility.ToHtmlStringRGB(normalColor)}>{before}</color>";
        string selectedColored = $"<color=#{ColorUtility.ToHtmlStringRGB(selectedColor)}>{selected}</color>";
        string afterColored = $"<color=#{ColorUtility.ToHtmlStringRGB(normalColor)}>{after}</color>";

        // Bu noktada text'i direkt değiştirmek yerine bir sonraki frame'de değiştirmeyi deneyebilirsiniz.
        // Çünkü anında değiştirmek grafik yenileme döngüsünü bozabilir.
        StartCoroutine(ApplyTextNextFrame(beforeColored + selectedColored + afterColored));
    }

    IEnumerator ApplyTextNextFrame(string newText)
    {
        yield return null; // Bir frame bekle
        inputField.text = newText;
    }

}
