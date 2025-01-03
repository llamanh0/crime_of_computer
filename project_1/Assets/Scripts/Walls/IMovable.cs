// Assets/Scripts/Walls/IMovable.cs
namespace MyGame.Walls
{
    /// <summary>
    /// Duvar veya platform gibi hareket eden objeler için temel arayüz.
    /// </summary>
    public interface IMovable
    {
        void MoveToTarget();
        void MoveToInitial();
    }
}
