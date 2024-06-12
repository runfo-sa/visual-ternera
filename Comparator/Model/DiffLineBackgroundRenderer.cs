using ICSharpCode.AvalonEdit.Rendering;
using System.Windows.Media;

namespace Comparator.Model
{
    public class DiffLineBackgroundRenderer(List<int> diffLines, Color lineColor) : IBackgroundRenderer
    {
        public KnownLayer Layer => KnownLayer.Background;

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            foreach (var num in diffLines)
            {
                var line = textView.GetVisualLine(num);

                if (line != null)
                {
                    foreach (var rc in BackgroundGeometryBuilder.GetRectsFromVisualSegment(textView, line, 0, 10000))
                    {
                        drawingContext.DrawRectangle(
                            new SolidColorBrush(lineColor), null,
                            new System.Windows.Rect(0, rc.Top, textView.ActualWidth, rc.Height)
                        );
                    }
                }
            }
        }
    }
}
