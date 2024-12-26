// Assets/Scripts/UI/SingleLineOutput.cs
using MyGame.Core.Utilities;
using TMPro;
using UnityEngine;

namespace MyGame.UI
{
    /// <summary>
    /// Displays single-line output messages on the UI.
    /// </summary>
    public class SingleLineOutput : Singleton<SingleLineOutput>
    {
        [SerializeField] private TMP_Text outputText;

        protected override void Awake()
        {
            base.Awake();
            if (outputText == null)
            {
                Debug.LogError("SingleLineOutput: OutputText reference not assigned.");
            }
        }

        /// <summary>
        /// Displays a message on the UI.
        /// </summary>
        public void DisplayOutput(string message)
        {
            if (outputText != null)
            {
                outputText.text = message;
            }
            else
            {
                Debug.LogError("SingleLineOutput: OutputText reference is missing.");
            }
        }
    }
}
