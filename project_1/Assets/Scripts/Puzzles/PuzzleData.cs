// Assets/Scripts/Puzzles/PuzzleData.cs
using UnityEngine;

namespace MyGame.Puzzles
{
    /// <summary>
    /// Her bir bulmaca için gerekli verileri saklar.
    /// </summary>
    [CreateAssetMenu(fileName = "NewPuzzleData", menuName = "Puzzles/PuzzleData", order = 1)]
    public class PuzzleData : ScriptableObject
    {
        [TextArea]
        [Tooltip("Bulmacanın açıklaması veya ipuçları.")]
        public string puzzleDescription;

        [Tooltip("Beklenen program çıktısı.")]
        public string expectedOutput;

        [Tooltip("Bulmaca başarı mesajı.")]
        public string successMessage;

        [Tooltip("Bulmaca başarısızlık mesajı.")]
        public string failMessage;

        [Header("Optional Fields")]
        [Tooltip("Bulmaca tamamlandığında açılacak objeler.")]
        public GameObject unlockableObject;

        [Tooltip("Bulmaca için benzersiz kimlik.")]
        public string puzzleID;
    }
}
