// Assets/Scripts/Puzzles/CodeChecker.cs
using UnityEngine;
using MyGame.Walls;
using MyGame.Managers;
using MyGame.UI;

namespace MyGame.Puzzles
{
    /// <summary>
    /// Checks the program's output and determines if the puzzle is solved.
    /// </summary>
    public class CodeChecker : MonoBehaviour
    {
        [SerializeField] private string correctOutput = "CrimeOfComputer";
        [SerializeField] private WallController[] wallsToControl;

        /// <summary>
        /// Validates the program output against the expected result.
        /// </summary>
        [System.Obsolete]
        public void CheckPuzzleOutput(string output)
        {
            Debug.Log($"CodeChecker: Received Output: '{output}'");
            Debug.Log($"CodeChecker: Expected Output: '{correctOutput}'");

            if (output.Trim() == correctOutput)
            {
                SingleLineOutput.Instance.DisplayOutput("Puzzle Solved Successfully!");
                TriggerWallMovements();
                UnlockNextLevel();
            }
            else
            {
                SingleLineOutput.Instance.DisplayOutput("Incorrect Output. Try Again.");
            }
        }

        [System.Obsolete]
        private void TriggerWallMovements()
        {
            foreach (WallController wall in wallsToControl)
            {
                EventManager.Instance.EnqueueEvent(() => wall.MoveToTarget());
            }
        }

        private void UnlockNextLevel()
        {
            // Implement level unlocking logic here.
            Debug.Log("Next level unlocked!");
            // Example:
            // LevelManager.Instance.UnlockLevel(nextLevelID);
        }

        [System.Obsolete]
        public void DisplayError(string error)
        {
            SingleLineOutput.Instance?.DisplayOutput("Error: " + error);
        }

        [System.Obsolete]
        public void DisplayOutput(string output)
        {
            SingleLineOutput.Instance?.DisplayOutput(output);
        }
    }
}
