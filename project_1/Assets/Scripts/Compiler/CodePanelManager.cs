using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CodePanelManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private Button runButton;
    [SerializeField] private TMP_Text outputText;
    [SerializeField] private CCompiler compiler;

    void Start()
    {
        if (runButton != null && compiler != null && codeInputField != null)
        {
            runButton.onClick.AddListener(() =>
            {
                string code = codeInputField.text;
                compiler.CompileAndRun(code);
            });
        }
        else
        {
            UnityEngine.Debug.LogError("Script referansları eksik! Lütfen tüm alanları doldurun.");
        }
    }

    // CCompiler script'inden çağrılacak bir yöntem
    public void DisplayOutput(string output)
    {
        outputText.text = output;
    }

    public void DisplayError(string error)
    {
        outputText.text = "Hata: " + error;
    }
}

