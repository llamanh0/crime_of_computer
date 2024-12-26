// Assets/Scripts/Puzzles/CodeChecker.cs
using UnityEngine;
using MyGame.Walls;
using MyGame.Core.Utilities;
using MyGame.Managers;
using MyGame.UI;

namespace MyGame.Puzzles
{
    /// <summary>
    /// Program çıktısını kontrol eder ve bulmacanın çözülüp çözülmediğini belirler.
    /// </summary>
    public class CodeChecker : MonoBehaviour
    {
        [SerializeField] private PuzzleData currentPuzzle; // Şu anki bulmaca verisi

        [SerializeField] private WallController[] wallsToControl;

        /// <summary>
        /// Program çıktısını beklenen değerle karşılaştırır.
        /// </summary>
        public void CheckPuzzleOutput(string output)
        {
            if (currentPuzzle == null)
            {
                Debug.LogError("CodeChecker: currentPuzzle referansı atanmamış!");
                return;
            }

            Debug.Log($"CodeChecker: Alınan Çıktı: '{output}'");
            Debug.Log($"CodeChecker: Beklenen Çıktı: '{currentPuzzle.expectedOutput}'");

            if (output.Trim() == currentPuzzle.expectedOutput)
            {
                SingleLineOutput.Instance.DisplayOutput(currentPuzzle.successMessage);
                TriggerWallMovements();
                UnlockNextLevel();
            }
            else
            {
                SingleLineOutput.Instance.DisplayOutput(currentPuzzle.failMessage);
            }
        }

        private void TriggerWallMovements()
        {
            foreach (WallController wall in wallsToControl)
            {
                EventManager.Instance.EnqueueEvent(() => wall.MoveToTarget());
            }
        }

        private void UnlockNextLevel()
        {
            // LevelManager ile seviyeyi kilidi açma işlemini gerçekleştirin.
            Debug.Log("Sonraki seviye açıldı!");
            // Örneğin:
            // LevelManager.Instance.UnlockLevel(nextLevelID);
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
