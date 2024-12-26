// Assets/Scripts/Puzzles/PuzzleData.cs
using UnityEngine;

namespace MyGame.Puzzles
{
    /// <summary>
    /// Stores data for individual puzzles.
    /// </summary>
    [CreateAssetMenu(fileName = "NewPuzzleData", menuName = "Puzzles/PuzzleData", order = 1)]
    public class PuzzleData : ScriptableObject
    {
        [TextArea]
        [Tooltip("Description or hints for the puzzle.")]
        public string puzzleDescription;

        [Tooltip("Expected code output.")]
        public string expectedAnswer;

        [Tooltip("Message displayed upon successful completion.")]
        public string successMessage;

        [Tooltip("Message displayed upon failure.")]
        public string failMessage;

        [Header("Optional Fields")]
        [Tooltip("Objects to unlock upon puzzle completion.")]
        public GameObject unlockableObject;

        [Tooltip("Unique identifier for the puzzle.")]
        public int puzzleID;
    }
}
