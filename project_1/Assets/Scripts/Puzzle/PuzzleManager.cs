using UnityEngine;
using TMPro;
/*
public class PuzzleManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private Button runButton;
    [SerializeField] private TMP_Text outputText;

    [Header("Compiler")]
    [SerializeField] private CCompiler compiler;

    [Header("Puzzle Settings")]
    [SerializeField] private string correctOutput = "Hello, World!";

    void Start()
    {
        if(runButton != null && compiler != null && codeInputField != null && outputText != null)
        {
            runButton.onClick.AddListener(OnRunButtonClicked);
        }
        else
        {
            Debug.LogError("PuzzleManager: Bir veya daha fazla referans eksik!");
        }
    }

    private void OnRunButtonClicked()
    {
        string code = codeInputField.text;
        compiler.CompileAndRun(code);
    }

    // CCompiler'dan çağrılacak yöntem
    public void OnCompilationFinished(bool isSuccess, string output)
    {
        if(isSuccess)
        {
            if(output.Trim().Equals(correctOutput, System.StringComparison.OrdinalIgnoreCase))
            {
                outputText.text = "Puzzle Başarıyla Çözüldü!";
                UnlockNextLevel();
            }
            else
            {
                outputText.text = "Yanlış Çıktı. Tekrar Deneyin.";
            }
        }
        else
        {
            outputText.text = "Derleme Hatası:\n" + output;
        }
    }

    private void UnlockNextLevel()
    {
        // Burada bir sonraki seviyeyi açma veya puzzle'ı tamamlama işlemlerini gerçekleştirin.
        Debug.Log("Bir sonraki seviye açıldı!");
        // Örneğin:
        // LevelManager.Instance.UnlockLevel(2);
    }
}
*/
