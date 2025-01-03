using UnityEngine;
using System.Collections;
using UnityEditor;
using MyGame.Player;

/// <summary>
/// Oyuncu bu tetikleyiciye girince kameranın zoom (orthographic size) değerini değiştirir.
/// 2D projelerde ortographic size, 3D projelerde fieldOfView kullanılabilir.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class CameraZoomTrigger : MonoBehaviour
{
    [Header("Camera Zoom Settings")]
    [Tooltip("Hedef kamerayı atayın (örneğin MainCamera)")]
    [SerializeField] private Camera mainCamera;

    [Tooltip("Tetikleyiciye girildiğinde kameranın ulaşması gereken orthographic size değeri")]
    [SerializeField] private float targetOrthographicSize = 5f;

    [Tooltip("Geçişin süresi")]
    [SerializeField] private float transitionDuration = 1f;

    [Tooltip("Tetikleyiciden çıkınca kameranın eski boyutuna dönsün mü?")]
    [SerializeField] private bool revertOnExit = false;

    [Header("Nereye ?")]
    [SerializeField] private Vector3 targetPosition;

    // Yumuşak geçişte eski / yeni değerler
    private float originalSize;
    private Coroutine currentZoomCoroutine;
    private PlayerMovement playerMovementScript;
    private PlayerSettings currentSettings;
    private Coroutine cameraMoveCoroutine;

    private void Awake()
    {
        if (mainCamera == null)
        {
            // Sahnedeki ana kamerayı otomatik bulmak isterseniz:
            mainCamera = Camera.main;
        }
        // Oyun başlarken kameranın orijinal orthographic size değerini kaydediyoruz
        originalSize = mainCamera.orthographicSize;
        currentSettings = playerMovementScript.playerSettings;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Oyuncu bu trigger'a girince zoom geçişini başlat
        if (collision.CompareTag("Player"))
        {
            // Daha önce bir coroutine çalışıyorsa durdur
            if (currentZoomCoroutine != null)
            {
                StopCoroutine(currentZoomCoroutine);
            }
            currentZoomCoroutine = StartCoroutine(ChangeCameraSize(mainCamera.orthographicSize, targetOrthographicSize, transitionDuration));
            CameraTrigger trigger = collision.GetComponent<CameraTrigger>();
            if (trigger != null)
            {
                if (cameraMoveCoroutine != null)
                {
                    StopCoroutine(cameraMoveCoroutine);
                }
                cameraMoveCoroutine = StartCoroutine(MoveCamera(trigger.targetPosition));
            }
        }
    }

    private IEnumerator MoveCamera(Vector3 targetPosition)
    {
        while (Vector3.Distance(mainCamera.transform.position, targetPosition) > currentSettings.cameraStopDistance)
        {
            Vector3 newPosition = Vector3.Lerp(mainCamera.transform.position, targetPosition, currentSettings.cameraSmoothSpeed * Time.deltaTime);
            newPosition.z = mainCamera.transform.position.z;
            mainCamera.transform.position = newPosition;
            yield return null;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Tetikleyiciden çıkınca kamerayı eski boyutuna geri al (isteğe bağlı)
        if (revertOnExit && collision.CompareTag("Player"))
        {
            if (currentZoomCoroutine != null)
            {
                StopCoroutine(currentZoomCoroutine);
            }
            currentZoomCoroutine = StartCoroutine(ChangeCameraSize(mainCamera.orthographicSize, originalSize, transitionDuration));
        }
    }

    /// <summary>
    /// Kameranın boyutunu (orthographicSize) 'duration' sürede yumuşakça değiştirir.
    /// </summary>
    private IEnumerator ChangeCameraSize(float fromSize, float toSize, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            mainCamera.orthographicSize = Mathf.Lerp(fromSize, toSize, t);
            yield return null;
        }
        mainCamera.orthographicSize = toSize;
    }
}