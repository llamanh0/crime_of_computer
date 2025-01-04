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
        // Veya inspector'da ICodeChecker tipinde bir Reference alabilirsiniz (daha geli�mi�)

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
            // Scene de�i�irse veya bu obje yok olursa unsub olmak gerekir
            CodeChecker.OnPuzzleSolved -= HandlePuzzleSolved;
        }

        private void Start()
        {
            DisplayPuzzleDescription();
        }

        private void DisplayPuzzleDescription()
        {
            Debug.Log("Puzzle A��klamas�: " + currentPuzzle.puzzleDescription);
            // �r: puzzleDescriptionText.text = currentPuzzle.puzzleDescription;
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

            // Derleme & �al��t�rma
            compiler.CompileAndRun(userCode, currentPuzzle);
        }

        private void HandlePuzzleSolved(PuzzleData solvedPuzzle)
        {
            Debug.Log($"PuzzleManager: Puzzle ��z�ld� => {solvedPuzzle.puzzleID}");
            // Burada puzzle ��z�l�nce yap�lacak i�leri (seviye ge�i�i, �d�l vs.) ekleyebilirsiniz
        }
    }
}
