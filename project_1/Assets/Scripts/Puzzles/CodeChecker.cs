// Assets/Scripts/Puzzles/CodeChecker.cs
using UnityEngine;
using MyGame.UI;

namespace MyGame.Puzzles
{
    /// <summary>
    /// Program çıktısını kontrol eder ve bulmacanın çözülüp çözülmediğini belirler.
    /// </summary>
    public class CodeChecker : MonoBehaviour, ICodeChecker
    {
        [Header("Puzzle Data")]
        [SerializeField] private PuzzleData currentPuzzle;

        [Header("Walls To Control")]
        [SerializeField] private MonoBehaviour[] wallsToControl;
        // Inspector'da WallController referansları eklenebilir
        // ya da IMovable[] direkt kullanabilirsiniz (Unity inspector arayüzü için biraz trick gerekebilir).

        /// <summary>
        /// Puzzle çözüldüğünde diğer sınıflara haber vermek için event.
        /// </summary>
        public static event System.Action<PuzzleData> OnPuzzleSolved;

        public void CheckPuzzleOutput(string output)
        {
            if (currentPuzzle == null)
            {
                Debug.LogError("CodeChecker: currentPuzzle is null!");
                return;
            }

            Debug.Log($"CodeChecker: Alınan Çıktı: '{output}'");
            Debug.Log($"CodeChecker: Beklenen Çıktı: '{currentPuzzle.expectedOutput}'");

            // Çıktı beklenenle eşleşiyorsa:
            if (output.Trim() == currentPuzzle.expectedOutput)
            {
                SingleLineOutput.Instance.DisplayOutput(currentPuzzle.successMessage);
                TriggerWallMovements();

                // Puzzle çözüldüğünü event ile duyuruyoruz
                OnPuzzleSolved?.Invoke(currentPuzzle);
            }
            else
            {
                SingleLineOutput.Instance.DisplayOutput(currentPuzzle.failMessage);
            }
        }

        private void TriggerWallMovements()
        {
            // IMovable arayüzü üzerinden çağırmak için:
            foreach (var wallObj in wallsToControl)
            {
                if (wallObj is MyGame.Walls.IMovable movableWall)
                {
                    movableWall.MoveToTarget();
                }
            }
        }

        public void DisplayError(string error)
        {
            SingleLineOutput.Instance?.DisplayOutput("Error: " + error);
        }

        public void DisplayOutput(string output)
        {
            SingleLineOutput.Instance?.DisplayOutput(output);
        }
    }
}
