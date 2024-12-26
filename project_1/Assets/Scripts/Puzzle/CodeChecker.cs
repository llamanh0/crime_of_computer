using System;
using UnityEngine;

public class CodeChecker : MonoBehaviour
{
    [SerializeField] private SingleLineOutput singleLineOutput; // SingleLineOutput referansı
    [SerializeField] private string correctOutput = "Hello, World!"; // Beklenen doğru çıktı

    /// <summary>
    /// Derleme veya çalıştırma hatalarını gösterir.
    /// </summary>
    /// <param name="error">Hata mesajı.</param>
    public void DisplayError(string error)
    {
        if(singleLineOutput != null)
        {
            singleLineOutput.DisplayOutput("Hata: " + error);
        }
        else
        {
            Debug.LogError("CodeChecker: SingleLineOutput referansı eksik!");
        }
    }

    /// <summary>
    /// Genel çıktıları gösterir.
    /// </summary>
    /// <param name="output">Çıktı mesajı.</param>
    public void DisplayOutput(string output)
    {
        if(singleLineOutput != null)
        {
            singleLineOutput.DisplayOutput(output);
        }
        else
        {
            Debug.LogError("CodeChecker: SingleLineOutput referansı eksik!");
        }
    }

    /// <summary>
    /// Programın çıktısını kontrol eder ve puzzle'ın çözülüp çözülmediğini belirler.
    /// </summary>
    /// <param name="output">Program çıktısı.</param>
    public void CheckPuzzleOutput(string output)
    {
        if (output.Equals(correctOutput, StringComparison.OrdinalIgnoreCase))
        {
            // Puzzle başarılı
            if(singleLineOutput != null)
            {
                singleLineOutput.DisplayOutput("Puzzle Başarıyla Çözüldü!");
            }
            UnlockNextLevel();
        }
        else
        {
            // Yanlış çıktı
            if(singleLineOutput != null)
            {
                singleLineOutput.DisplayOutput("Yanlış Çıktı. Tekrar Deneyin.");
            }
        }
    }

    /// <summary>
    /// Puzzle çözüldüğünde bir sonraki seviyeyi açar.
    /// </summary>
    private void UnlockNextLevel()
    {
        // Bir sonraki seviyeyi açma veya puzzle'ı tamamlama işlemleri
        Debug.Log("Bir sonraki seviye açıldı!");
        // Örneğin:
        // LevelManager.Instance.UnlockLevel(2);
    }
}
