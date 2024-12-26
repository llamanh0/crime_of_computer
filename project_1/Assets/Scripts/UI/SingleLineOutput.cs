// Assets/Scripts/UI/SingleLineOutput.cs
using TMPro;
using UnityEngine;
using MyGame.Core.Utilities; // Singleton namespace'i

namespace MyGame.UI
{
    /// <summary>
    /// Tek satırlık mesajları UI'da görüntüler.
    /// </summary>
    public class SingleLineOutput : Singleton<SingleLineOutput>
    {
        [SerializeField] private TMP_Text outputText;

        protected override void Awake()
        {
            base.Awake();
            if (outputText == null)
            {
                Debug.LogError("SingleLineOutput: OutputText referansı atanmamış!");
            }
        }

        /// <summary>
        /// UI'da bir mesaj görüntüler.
        /// </summary>
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
