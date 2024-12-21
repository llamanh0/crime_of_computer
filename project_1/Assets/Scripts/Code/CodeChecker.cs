using UnityEngine;
using TMPro;
using System.Collections;

public class CodeChecker : MonoBehaviour
{
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private GameObject codePanel;
    [SerializeField] private TMP_Text Message;
    [SerializeField] private GameObject acilanWall; // Açılan duvarın referansı

    private bool isCodeCorrect = false;

    // Bu fonksiyon butonun OnClick eventine bağlanacak
    public void CheckCode()
    {
        string userCode = codeInputField.text.Replace(" ", "");
        if (userCode == "true;")
        {
            Message.color = Color.green;
            Message.text = "KOD DOĞRU KAPI AÇILIYOR ...";
            isCodeCorrect = true;

            StartCoroutine(OpenWallAndHidePanel());
            PlayerLive();
        }
        else
        {
            Message.color = Color.red;
            Message.text = "!!! HATALI KOD !!!";
        }
    }

    private void PlayerLive()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.SetCodePanelState(false); // Oyuncuya hareket izni ver
            }
        }
    }

    private IEnumerator OpenWallAndHidePanel()
    {
        if (acilanWall == null)
        {
            yield break;
        }
        float duration = 1.3f;
        float elapsedTime = 0f;
        Vector3 startPos = acilanWall.transform.position;
        Vector3 targetPos = startPos + new Vector3(0, 15, 0);
        while (elapsedTime < duration)
        {
            acilanWall.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        acilanWall.transform.position = targetPos;
        yield return new WaitForSeconds(0.5f);
        Destroy(acilanWall);
        codePanel.SetActive(false);
    }
}
