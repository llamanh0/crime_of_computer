using UnityEngine;
using TMPro;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

[System.Serializable]
public class WallData
{
    public GameObject wallObject;
    public Vector3 moveDirection = Vector3.up; 
    public float moveDistance = 15f; 
    public float moveDuration = 1.3f;
    public float postMoveWait = 0.5f; // duvar hareketi bittiğinde beklemek isterseniz
}

public class CodeChecker : MonoBehaviour
{
    [Header("Puzzle Data")]
    [SerializeField] private PuzzleData puzzleData;

    [Header("UI References")]
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private GameObject codePanel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text placeHolder;

    [Header("Walls (Each with its own params)")]
    [Tooltip("Her duvar için farklı MoveDirection, MoveDistance, MoveDuration belirleyebilirsiniz.")]
    [SerializeField] private List<WallData> acilanWalls; 

    [Header("Puzzle List (Opsiyonel)")]
    [SerializeField] private List<PuzzleData> puzzles;
    private int currentPuzzleIndex = 0;

    public void CheckCode(string userCode)
    {
        // PuzzleData seçimi
        var currentPuzzle = puzzles.Count > 0 ? puzzles[currentPuzzleIndex] : puzzleData;
        if (currentPuzzle == null)
        {
            Debug.LogWarning("PuzzleData atanmadı! Lütfen Inspector'dan bir PuzzleData referansı verin.");
            return;
        }

        // Boşlukları temizle
        userCode = Regex.Replace(codeInputField.text, @"\s+", "");

        // Kod karşılaştırma
        if (userCode == currentPuzzle.expectedAnswer)
        {
            placeHolder.color = Color.green;
            placeHolder.text = currentPuzzle.expectedAnswer;
            messageText.color = Color.green;
            messageText.text = currentPuzzle.successMessage;

            // Sonraki puzzle
            currentPuzzleIndex++;
            if (currentPuzzleIndex >= puzzles.Count)
            {
                currentPuzzleIndex = 0;
            }

            // unlockableObject veya benzeri mantık
            if (currentPuzzle.unlockableObject != null)
            {
                // Destroy(currentPuzzle.unlockableObject);
            }

            // Duvarları tek tek açma
            if (acilanWalls != null && acilanWalls.Count > 0)
            {
                StartCoroutine(OpenWallsOneByOne());
            }
            else
            {
                // Hiç duvar yoksa paneli kapat
                codePanel.SetActive(false);
            }

            EnablePlayerMovement();
        }
        else
        {
            placeHolder.color = Color.red;
            placeHolder.text = "<Tekrar Deneyin!>;";
            messageText.color = Color.red;
            messageText.text = currentPuzzle.failMessage;
        }
    }

    /// <summary>
    /// Duvarları tek tek, her bir WallData'ya özgü parametrelerle açar.
    /// </summary>
    private IEnumerator OpenWallsOneByOne()
    {
        // for döngüsüyle her duvarın hareketini sırayla yap
        for (int i = 0; i < acilanWalls.Count; i++)
        {
            WallData wallData = acilanWalls[i];
            if (wallData.wallObject == null) continue; // Geçerli bir duvar yoksa atla

            // Tek duvar için move
            yield return StartCoroutine(MoveOneWall(wallData));

            // Her duvar bittikten sonra 
            yield return new WaitForSeconds(wallData.postMoveWait);

            // İsteğe bağlı: Duvarı yok et
            Destroy(wallData.wallObject);
        }

        // Tüm duvarlar bitince listeyi boşalt
        acilanWalls.Clear();

        // Paneli kapat
        codePanel.SetActive(false);
    }

    /// <summary>
    /// Bir duvarı, kendi parametrelerine göre hareket ettirir (tek sefer).
    /// </summary>
    private IEnumerator MoveOneWall(WallData wallData)
    {
        Vector3 startPos = wallData.wallObject.transform.position;
        // Yön * mesafe
        Vector3 targetPos = startPos + wallData.moveDirection.normalized * wallData.moveDistance;

        float elapsedTime = 0f;
        while (elapsedTime < wallData.moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / wallData.moveDuration;

            // Lerp
            wallData.wallObject.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        // Final
        wallData.wallObject.transform.position = targetPos;
    }

    private void EnablePlayerMovement()
    {
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
