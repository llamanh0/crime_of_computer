using UnityEngine;

public class CodeChecker : MonoBehaviour
{
    [SerializeField] private SingleLineOutput singleLineOutput; // SingleLineOutput referansı
    [SerializeField] private string correctOutput = "Hello, World!"; // Beklenen doğru çıktı

    public void DisplayError(string error)
    {
        // Hataları SingleLineOutput'a ilet
        singleLineOutput.DisplayOutput("Hata: " + error);
    }

    public void DisplayOutput(string output)
    {
        // Genel çıktıları SingleLineOutput'a ilet
        singleLineOutput.DisplayOutput(output);
    }

    public void CheckPuzzleOutput(string output)
    {
        if (output.Equals(correctOutput, System.StringComparison.OrdinalIgnoreCase))
        {
            // Puzzle başarılı
            singleLineOutput.DisplayOutput("Puzzle Başarıyla Çözüldü!");
            UnlockNextLevel();
        }
        else
        {
            // Yanlış çıktı
            singleLineOutput.DisplayOutput("Yanlış Çıktı. Tekrar Deneyin.");
        }
    }

    private void UnlockNextLevel()
    {
        // Bir sonraki seviyeyi açma veya puzzle'ı tamamlama işlemleri
        Debug.Log("Bir sonraki seviye açıldı!");
        // Örneğin:
        // LevelManager.Instance.UnlockLevel(2);
    }
}
