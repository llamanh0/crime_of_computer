// Assets/Scripts/Puzzles/PuzzleManager.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

        [Header("Compiler and Checker")]
        [SerializeField] private CCompiler compiler;
        [SerializeField] private CodeChecker codeChecker;
        // Veya inspector'da ICodeChecker tipinde bir Reference alabilirsiniz (daha geliþmiþ)

        private void Awake()
        {
            // Event'e abone oluyoruz
            CodeChecker.OnPuzzleSolved += HandlePuzzleSolved;

            if (compiler == null)
                Debug.LogError("PuzzleManager: CCompiler is null!");
            if (codeChecker == null)
                Debug.LogError("PuzzleManager: CodeChecker is null!");
            if (runButton != null)
                runButton.onClick.AddListener(OnRunButtonClicked);
            else
                Debug.LogError("PuzzleManager: RunButton is null!");
        }

        private void OnDestroy()
        {
            // Scene deðiþirse veya bu obje yok olursa unsub olmak gerekir
            CodeChecker.OnPuzzleSolved -= HandlePuzzleSolved;
        }

        private void Start()
        {
            DisplayPuzzleDescription();
        }

        private void DisplayPuzzleDescription()
        {
            Debug.Log("Puzzle Açýklamasý: " + currentPuzzle.puzzleDescription);
            // Ör: puzzleDescriptionText.text = currentPuzzle.puzzleDescription;
        }

        private void OnRunButtonClicked()
        {
            if (compiler == null || codeChecker == null)
                return;

            string userCode = codeInputField.text;
            if (string.IsNullOrWhiteSpace(userCode))
            {
                codeChecker.DisplayOutput("Code input cannot be empty.");
                return;
            }

            // Derleme & Çalýþtýrma
            compiler.CompileAndRun(userCode, currentPuzzle);
        }

        private void HandlePuzzleSolved(PuzzleData solvedPuzzle)
        {
            Debug.Log($"PuzzleManager: Puzzle çözüldü => {solvedPuzzle.puzzleID}");
            // Burada puzzle çözülünce yapýlacak iþleri (seviye geçiþi, ödül vs.) ekleyebilirsiniz
        }
    }
}
