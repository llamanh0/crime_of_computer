using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyGame.Puzzles;
using MyGame.UI;

/// <summary>
/// Puzzle yönetimini ve kullanıcı etkileşimlerini kontrol eder.
/// </summary>
public class PuzzleManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_InputField codeInputField; // Kullanıcının yazdığı kod
    [SerializeField] private Button runButton; // Run butonu

    private CCompiler compiler;

    private void Awake()
    {
        compiler = GetComponent<CCompiler>();

        if (compiler == null)
        {
            Debug.LogError("PuzzleManager: CCompiler script'i bulunamadı!");
        }

        if (runButton != null)
        {
            runButton.onClick.AddListener(OnRunButtonClicked);
        }
        else
        {
            Debug.LogError("PuzzleManager: RunButton referansı eksik!");
        }
    }

    /// <summary>
    /// Run butonuna tıklandığında çalıştırılır.
    /// </summary>
    private void OnRunButtonClicked()
    {
        string code = codeInputField.text;
        if (string.IsNullOrWhiteSpace(code))
        {
            SingleLineOutput.Instance.DisplayOutput("Kod alanı boş bırakılamaz.");
            return;
        }
        compiler.CompileAndRun(code);
    }
}
