// Assets/Scripts/Managers/EventManager.cs
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Managers
{
    /// <summary>
    /// Oyun içi olayları yönetir.
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }

        private Queue<System.Action> eventQueue = new Queue<System.Action>();
        private bool isExecuting = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Yeni bir olayı kuyruğa ekler.
        /// </summary>
        /// <param name="action">Gerçekleştirilecek olay.</param>
        public void EnqueueEvent(System.Action action)
        {
            eventQueue.Enqueue(action);
            if (!isExecuting)
            {
                StartCoroutine(ExecuteEvents());
            }
        }

        /// <summary>
        /// Kuyruğun başındaki olayı sırayla gerçekleştirir.
        /// </summary>
        private System.Collections.IEnumerator ExecuteEvents()
        {
            isExecuting = true;
            while (eventQueue.Count > 0)
            {
                System.Action currentEvent = eventQueue.Dequeue();
                currentEvent?.Invoke();
                // Her olayın tamamlanmasını beklemek için bekleme süresi ekleyebilirsiniz.
                yield return new WaitForSeconds(0.5f); // Örneğin, 0.5 saniye bekleme
            }
            isExecuting = false;
        }
    }
}
