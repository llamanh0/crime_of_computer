// Assets/Scripts/Puzzles/PuzzleManager.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MyGame.UI;

namespace MyGame.Puzzles
{
    /// <summary>
    /// Manages puzzle interactions and user inputs.
    /// </summary>
    public class PuzzleManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_InputField codeInputField;
        [SerializeField] private Button runButton;

        [Header("Puzzle Data")]
        [SerializeField] private PuzzleData currentPuzzle;

        [Header("Compiler Settings")]
        [SerializeField] private CCompiler compiler;

        [System.Obsolete]
        private void Awake()
        {
            if (compiler == null)
            {
                Debug.LogError("PuzzleManager: CCompiler reference not assigned.");
            }

            if (runButton != null)
            {
                runButton.onClick.AddListener(OnRunButtonClicked);
            }
            else
            {
                Debug.LogError("PuzzleManager: RunButton reference not assigned.");
            }
        }

        private void Start()
        {
            DisplayPuzzleDescription();
        }

        private void DisplayPuzzleDescription()
        {
            // Assuming there's a UI element to display the puzzle description
            // For example:
            // puzzleDescriptionText.text = currentPuzzle.puzzleDescription;
        }

        [System.Obsolete]
        private void OnRunButtonClicked()
        {
            string userCode = codeInputField.text;
            if (string.IsNullOrWhiteSpace(userCode))
            {
                SingleLineOutput.Instance.DisplayOutput("Code input cannot be empty.");
                return;
            }

            compiler.CompileAndRun(userCode);
        }
    }
}
