// Assets/Scripts/Core/Managers/EventManager.cs
using System;
using System.Collections.Generic;
using MyGame.Core.Utilities;
using UnityEngine;

namespace MyGame.Managers
{
    /// <summary>
    /// Singleton class that manages and executes game events in a queue.
    /// </summary>
    public class EventManager : Singleton<EventManager>
    {
        private readonly Queue<Action> eventQueue = new Queue<Action>();
        private bool isExecuting = false;

        /// <summary>
        /// Enqueues a new event to be executed.
        /// </summary>
        public void EnqueueEvent(Action action)
        {
            if (action == null) return;

            eventQueue.Enqueue(action);
            if (!isExecuting)
            {
                StartCoroutine(ExecuteEvents());
            }
        }

        /// <summary>
        /// Executes events in the queue sequentially with a delay.
        /// </summary>
        private System.Collections.IEnumerator ExecuteEvents()
        {
            isExecuting = true;
            while (eventQueue.Count > 0)
            {
                Action currentEvent = eventQueue.Dequeue();
                currentEvent?.Invoke();
                yield return new WaitForSeconds(0.5f); // Adjustable delay
            }
            isExecuting = false;
        }
    }
}
