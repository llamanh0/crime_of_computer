using UnityEngine;
using TMPro;

public class SingleLineCodeInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField codeInputField;

    // İsteğe bağlı, bir mesaj/hata text alanı
    [SerializeField] private TMP_Text feedbackText;

    [System.Obsolete]
    private void Awake()
    {
        // Eğer Inspector'dan atanmadıysa otomatik bulmaya çalışıyoruz
        if (codeInputField == null)
        {
            codeInputField = GetComponent<TMP_InputField>();
        }

        // onSubmit veya onEndEdit event'ine abone olalım
        // Unity 2020+ versiyonlarında: codeInputField.onSubmit.AddListener(HandleOnSubmit);
        // Daha eski versiyonlarda "onEndEdit" Enter sayılabiliyor. Deneyin hangisi sizin input sisteminde doğru çalışıyor.

        codeInputField.onSubmit.AddListener(HandleOnSubmit);
        // veya codeInputField.onEndEdit.AddListener(HandleOnEndEdit);
    }

    // Enter tuşuna basıldığında tetiklenecek
    [System.Obsolete]
    private void HandleOnSubmit(string userCode)
    {
        CodeChecker codeChecker = FindObjectOfType<CodeChecker>();
        if (codeChecker != null)
        {
            codeChecker.CheckCode(userCode);
            codeInputField.text = string.Empty;
        }
        // Tek satır input geldi, 'userCode' parametresiyle ne yapacağınız size bağlı.
        if (feedbackText != null)
        {
            feedbackText.text = $"KULLANICI GIRISI : {userCode}";
        }

    }

    // Eğer "onEndEdit" kullanırsanız:
    [System.Obsolete]
    private void HandleOnEndEdit(string userCode)
    {

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // Enter'a basılmışsa
            HandleOnSubmit(userCode);
        }
    }
}