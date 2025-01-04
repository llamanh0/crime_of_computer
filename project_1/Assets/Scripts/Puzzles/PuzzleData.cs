using UnityEngine;

namespace MyGame.Puzzles
{
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
        [Tooltip("Bulmaca tamamlandığında açılacak objeler (örn. kapılar).")]
        public GameObject unlockableObject;

        [Tooltip("Bulmaca için benzersiz kimlik.")]
        public string puzzleID;

        [Header("Code Template (isteğe bağlı)")]
        [TextArea]
        [Tooltip("Bu puzzle için kullanılacak C kod şablonu")]
        public string codeTemplate;

        /// <summary>
        /// Puzzle'a özel şablon kodu, kullanıcının yazdığı kod ile birleştirir.
        /// Template içinde '%USER_CODE%' placeholder'ı varsa, oraya userCode eklenir.
        /// </summary>
        public string GetFullCode(string userCode)
        {
            if (string.IsNullOrEmpty(codeTemplate))
                return userCode;

            return codeTemplate.Replace("%USER_CODE%", userCode);
        }
    }
}
