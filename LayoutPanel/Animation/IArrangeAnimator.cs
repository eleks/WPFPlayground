using System.Windows;

namespace Bornander.UI.Animations
{
    public interface IArrangeAnimator
    {
        Rect Arrange(double elapsedTime, Point desiredPosition, Size desiredSize, Point currentPosition, Size currentSize);
    }
}
