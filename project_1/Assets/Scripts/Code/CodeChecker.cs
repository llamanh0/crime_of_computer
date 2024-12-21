using UnityEngine;
using TMPro;
using System.Collections;
using System.Text.RegularExpressions;

public class CodeChecker : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private GameObject codePanel;
    [SerializeField] private TMP_Text messageText;

    [Header("Wall Reference")]
    [SerializeField] private GameObject acilanWall; // Açılacak duvar

    [Header("Wall Opening Settings")]
    [SerializeField] private float wallMoveUpDistance = 15f;  // Duvarın yukarı gideceği mesafe
    [SerializeField] private float wallMoveDuration = 1.3f;    // Lerp süresi
    [SerializeField] private float postMoveWait = 0.5f;        // Duvar yerleşince bekleme süresi

    // Bu fonksiyon butonun OnClick eventine bağlanacak
    public void CheckCode()
    {
        // Tüm boşluk karakterlerini (tab, newline, space) kaldırmak için Regex
        string userCode = Regex.Replace(codeInputField.text, @"\s+", "");

        // Basit kod kontrolü
        if (userCode == "true;")
        {
            messageText.color = Color.green;
            messageText.text = "KOD DOĞRU, KAPI AÇILIYOR...";
            StartCoroutine(OpenWallAndHidePanel());
            EnablePlayerMovement();
        }
        else
        {
            messageText.color = Color.red;
            messageText.text = "!!! HATALI KOD !!!";
        }
    }

    private void EnablePlayerMovement()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                // Kod paneli kapalı, oyuncu hareket edebilir
                playerMovement.SetCodePanelState(false);
            }
        }
    }

    private IEnumerator OpenWallAndHidePanel()
    {
        // Duvar yoksa koroutini sonlandır
        if (acilanWall == null) yield break;

        float elapsedTime = 0f;
        Vector3 startPos = acilanWall.transform.position;
        Vector3 targetPos = startPos + new Vector3(0, wallMoveUpDistance, 0);

        // Duvarı yavaşça yukarı taşı
        while (elapsedTime < wallMoveDuration)
        {
            float t = elapsedTime / wallMoveDuration;
            acilanWall.transform.position = Vector3.Lerp(startPos, targetPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        acilanWall.transform.position = targetPos;

        // Biraz bekle
        yield return new WaitForSeconds(postMoveWait);

        // Duvarı yok et
        Destroy(acilanWall);
        // Kod panelini kapat
        codePanel.SetActive(false);
    }
}