using UnityEngine;
using UnityEngine.UI; // Button için gerekli
using TMPro; // TextMeshPro için gerekli

public class PuzzleManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_InputField codeInputField; // Kullanıcının yazdığı kod
    [SerializeField] private Button runButton; // Run butonu
    [SerializeField] private SingleLineOutput singleLineOutput; // Çıktıyı gösteren script
    [SerializeField] private CodeChecker codeChecker; // Çıktıyı kontrol eden script

    [Header("Compiler")]
    [SerializeField] private CCompiler compiler; // CCompiler script'i

    void Start()
    {
        if(runButton != null && compiler != null && codeInputField != null && singleLineOutput != null && codeChecker != null)
        {
            runButton.onClick.AddListener(OnRunButtonClicked);
        }
        else
        {
            Debug.LogError("PuzzleManager: Bir veya daha fazla referans eksik!");
        }
    }

    /// <summary>
    /// Run butonuna tıklandığında çalıştırılır.
    /// </summary>
    private void OnRunButtonClicked()
    {
        string code = codeInputField.text;
        if(string.IsNullOrWhiteSpace(code))
        {
            singleLineOutput.DisplayOutput("Kod alanı boş bırakılamaz.");
            return;
        }
        compiler.CompileAndRun(code);
    }
}
