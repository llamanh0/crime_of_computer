// Assets/Scripts/Walls/WallController.cs
using UnityEngine;

namespace MyGame.Walls
{
    /// <summary>
    /// Duvarların hareketini kontrol eder.
    /// </summary>
    public class WallController : MonoBehaviour, IMovable
    {
        [Header("Movement Settings")]
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private float moveDuration = 2f;

        private Vector3 initialPosition;
        private bool isMoving;

        private void Start()
        {
            initialPosition = transform.position;
        }

        public void MoveToTarget()
        {
            if (!isMoving)
            {
                StartCoroutine(MoveCoroutine(transform.position, targetPosition, moveDuration));
            }
        }

        public void MoveToInitial()
        {
            if (!isMoving)
            {
                StartCoroutine(MoveCoroutine(transform.position, initialPosition, moveDuration));
            }
        }

        private System.Collections.IEnumerator MoveCoroutine(Vector3 from, Vector3 to, float duration)
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
