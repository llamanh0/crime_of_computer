// Assets/Scripts/UI/SingleLineOutput.cs
using TMPro;
using UnityEngine;

namespace MyGame.UI
{
    /// <summary>
    /// Tek satırlık çıktıyı ekranda gösterir.
    /// </summary>
    public class SingleLineOutput : MonoBehaviour
    {
        public static SingleLineOutput Instance { get; private set; }

        [SerializeField] private TMP_Text outputText; // UI Text referansı

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
        /// Gelen mesajı OutputText alanında gösterir.
        /// </summary>
        /// <param name="message">Gösterilecek mesaj.</param>
        public void DisplayOutput(string message)
        {
            if (outputText != null)
            {
                outputText.text = message;
            }
            else
            {
                Debug.LogError("SingleLineOutput: OutputText referansı eksik!");
            }
        }
    }
}
