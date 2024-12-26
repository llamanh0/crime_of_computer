// Assets/Scripts/Puzzles/CodeChecker.cs
using UnityEngine;
using MyGame.Walls;
using MyGame.Managers;
using MyGame.UI;

namespace MyGame.Puzzles
{
    /// <summary>
    /// Programın çıktısını kontrol eder ve doğru olup olmadığını belirler.
    /// </summary>
    public class CodeChecker : MonoBehaviour
    {
        [SerializeField] private string correctOutput = "CrimeOfComputer"; // Beklenen doğru çıktı
        [SerializeField] private WallController[] wallsToControl; // Kontrol edilecek duvarlar

        /// <summary>
        /// Program çıktısını kontrol eder ve puzzle'ın çözülüp çözülmediğini belirler.
        /// </summary>
        /// <param name="output">Program çıktısı.</param>
        public void CheckPuzzleOutput(string output)
        {
            Debug.Log($"CodeChecker: Gelen çıktı: '{output}'");
            Debug.Log($"CodeChecker: Beklenen çıktı: '{correctOutput}'");

            if (output.Trim() == correctOutput)
            {
                // Puzzle başarılı
                SingleLineOutput.Instance.DisplayOutput("Puzzle Başarıyla Çözüldü!");
                TriggerWallMovements();
                UnlockNextLevel();
            }
            else
            {
                // Yanlış çıktı
                SingleLineOutput.Instance.DisplayOutput("Yanlış Çıktı. Tekrar Deneyin.");
            }
        }

        /// <summary>
        /// Duvar hareketlerini tetikler.
        /// </summary>
        private void TriggerWallMovements()
        {
            foreach (WallController wall in wallsToControl)
            {
                EventManager.Instance.EnqueueEvent(() => wall.MoveToTarget());
            }
        }

        /// <summary>
        /// Puzzle çözüldüğünde bir sonraki seviyeyi açar.
        /// </summary>
        private void UnlockNextLevel()
        {
            // Seviye yönetimi için Event yayınlayın veya direkt olarak LevelManager'a erişin.
            Debug.Log("Bir sonraki seviye açıldı!");
            // Örneğin:
            // LevelManager.Instance.UnlockLevel(2);
        }

        /// <summary>
        /// Derleme veya çalıştırma hatalarını gösterir.
        /// </summary>
        /// <param name="error">Hata mesajı.</param>
        public void DisplayError(string error)
        {
            if (SingleLineOutput.Instance != null)
            {
                SingleLineOutput.Instance.DisplayOutput("Hata: " + error);
            }
            else
            {
                Debug.LogError("CodeChecker: SingleLineOutput referansı eksik!");
            }
        }

        /// <summary>
        /// Genel çıktıları gösterir.
        /// </summary>
        /// <param name="output">Çıktı mesajı.</param>
        public void DisplayOutput(string output)
        {
            if (SingleLineOutput.Instance != null)
            {
                SingleLineOutput.Instance.DisplayOutput(output);
            }
            else
            {
                Debug.LogError("CodeChecker: SingleLineOutput referansı eksik!");
            }
        }
    }
}
