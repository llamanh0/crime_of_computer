// Assets/Scripts/Walls/WallController.cs
using UnityEngine;

namespace MyGame.Walls
{
    /// <summary>
    /// Duvarların hareketini kontrol eder.
    /// </summary>
    public class WallController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public Vector3 targetPosition; // Duvarın hareket edeceği hedef pozisyon
        public float moveDuration = 2f; // Hareket süresi

        private Vector3 initialPosition;
        private bool isMoving = false;

        private void Start()
        {
            initialPosition = transform.position;
        }

        /// <summary>
        /// Duvarı hedef pozisyona hareket ettirir.
        /// </summary>
        public void MoveToTarget()
        {
            if (!isMoving)
            {
                StartCoroutine(Move(transform.position, targetPosition, moveDuration));
            }
        }

        /// <summary>
        /// Duvarı başlangıç pozisyonuna geri döndürür.
        /// </summary>
        public void MoveToInitial()
        {
            if (!isMoving)
            {
                StartCoroutine(Move(transform.position, initialPosition, moveDuration));
            }
        }

        /// <summary>
        /// Hareket işlemini gerçekleştirir.
        /// </summary>
        private System.Collections.IEnumerator Move(Vector3 from, Vector3 to, float duration)
        {
            isMoving = true;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(from, to, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = to;
            isMoving = false;
        }
    }
}
