// Assets/Scripts/Puzzles/ICodeChecker.cs
namespace MyGame.Puzzles
{
    /// <summary>
    /// Kod çýktýsýný kontrol eden sýnýflar için ortak arayüz.
    /// </summary>
    public interface ICodeChecker
    {
        void CheckPuzzleOutput(string output);
        void DisplayError(string error);
        void DisplayOutput(string output);
    }
}
