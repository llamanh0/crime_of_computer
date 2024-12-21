using UnityEngine;

[CreateAssetMenu(fileName = "NewPuzzle", menuName = "Puzzle/New Puzzle", order = 0)]
public class PuzzleData : ScriptableObject
{
    [TextArea]
    [Tooltip("Oyuncuya gösterilecek puzzle açıklaması veya ipucu.")]
    public string puzzleDescription;

    [Tooltip("Oyuncunun girmesi beklenen kod (whitespace temizlenmiş hali).")]
    public string expectedAnswer;

    [Tooltip("Doğru çözüldüğünde verilecek başarı mesajı.")]
    public string successMessage;

    [Tooltip("Yanlış çözüldüğünde verilecek hata mesajı.")]
    public string failMessage;

    [Header("Opsiyonel Ek Alanlar")]
    [Tooltip("Bu puzzle tamamlanınca açılacak kapı/duvar vs. eğer özel logic yapacaksanız.")]
    public GameObject unlockableObject;

    [Tooltip("Bulmacanın zorluk derecesi veya ID'si gibi ek veriler tutabilirsiniz.")]
    public int puzzleID;
}