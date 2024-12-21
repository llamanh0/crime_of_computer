using UnityEngine;
using TMPro;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

/// <summary>
/// PuzzleData üzerinden beklenecek cevabı alır, girilen kodu kontrol eder ve duvar/kapı açma işlemini yürütür.
/// </summary>
public class CodeChecker : MonoBehaviour
{
    [Header("Puzzle Data")]
    [Tooltip("Bu puzzle ile ilgili veriler (expectedAnswer, successMessage vb.)")]
    [SerializeField] private PuzzleData puzzleData;

    [Header("UI References")]
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private GameObject codePanel;
    [SerializeField] private TMP_Text messageText;

    [Header("Wall Opening")]
    [SerializeField] private GameObject acilanWall; // Eski sistemdeki duvar (opsiyonel)
    [SerializeField] private float wallMoveUpDistance = 15f;
    [SerializeField] private float wallMoveDuration = 1.3f;
    [SerializeField] private float postMoveWait = 0.5f;

    [SerializeField] private List<PuzzleData> puzzles;
    private int currentPuzzleIndex = 0;

    // Bu fonksiyon butonun OnClick eventine bağlanacak
    public void CheckCode()
    {
        var currentPuzzle = puzzles[currentPuzzleIndex];
        if (puzzleData == null)
        {
            Debug.LogWarning("PuzzleData atanmadı! Lütfen Inspector'dan bir PuzzleData referansı verin.");
            return;
        }

        // Tüm whitespace karakterlerini kaldırma
        string userCode = Regex.Replace(codeInputField.text, @"\s+", "");

        // Beklenen cevap puzzleData.expectedAnswer
        if (userCode == puzzleData.expectedAnswer)
        {
            currentPuzzleIndex++;
            messageText.color = Color.green;
            messageText.text = puzzleData.successMessage;

            // Puzzleda unlockableObject tanımlıysa, orayı açma logic'i. (Opsiyonel)
            if (puzzleData.unlockableObject != null)
            {
                // puzzleData.unlockableObject'i yok edebilir, aktif edebilir vs.
                // Destroy(puzzleData.unlockableObject);
                // codePanel.SetActive(false);
            }

            // Eski duvar açma yaklaşımı
            if (acilanWall != null)
            {
                StartCoroutine(OpenWallAndHidePanel());
            }
            else
            {
                // Duvar yoksa sadece paneli kapat ve player'ı serbest bırak
                codePanel.SetActive(false);
            }

            EnablePlayerMovement();
        }
        else
        {
            messageText.color = Color.red;
            messageText.text = puzzleData.failMessage;
        }
    }

    private IEnumerator OpenWallAndHidePanel()
    {
        float elapsedTime = 0f;
        Vector3 startPos = acilanWall.transform.position;
        Vector3 targetPos = startPos + new Vector3(0, wallMoveUpDistance, 0);

        while (elapsedTime < wallMoveDuration)
        {
            float t = elapsedTime / wallMoveDuration;
            acilanWall.transform.position = Vector3.Lerp(startPos, targetPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        acilanWall.transform.position = targetPos;

        yield return new WaitForSeconds(postMoveWait);

        // Duvarı yok et (veya kapıyı açma animasyonunuz varsa orada sonlandırın)
        Destroy(acilanWall);

        codePanel.SetActive(false);
    }

    private void EnablePlayerMovement()
    {
        // Panel kapandığında player hareket edebilsin
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.SetCodePanelState(false);
            }
        }
    }
}