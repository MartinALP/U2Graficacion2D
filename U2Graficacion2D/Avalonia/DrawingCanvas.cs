using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering;
using System;

namespace U2Graficacion2D;

public class DrawingCanvas : Canvas
{
    public event EventHandler<DrawingContext>? Paint;
    private CustomVisual? _customVisual;

    public new static readonly StyledProperty<IBrush?> BackgroundProperty =
        AvaloniaProperty.Register<DrawingCanvas, IBrush?>(nameof(Background), Brushes.White);

    public new IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    public DrawingCanvas()
    {
        ClipToBounds = true;
        
        this.Loaded += (s, e) =>
        {
            _customVisual = new CustomVisual(this);
            VisualChildren.Add(_customVisual);
            DoDraw();
        };
        
        this.SizeChanged += (s, e) => DoDraw();
    }

    public new void InvalidateVisual()
    {
        DoDraw();
    }

    private void DoDraw()
    {
        _customVisual?.InvalidateVisual();
    }

    private class CustomVisual : Visual
    {
        private readonly DrawingCanvas _parent;

        public CustomVisual(DrawingCanvas parent)
        {
            _parent = parent;
        }

        public override void Render(DrawingContext context)
        {
            var bounds = _parent.Bounds;
            
            // Strictly clip drawing to bounds so shapes don't overlap controls
            using (context.PushClip(new Rect(bounds.Size)))
            {
                // Draw background
                if (_parent.Background != null)
                {
                    context.FillRectangle(_parent.Background, new Rect(bounds.Size));
                }

                // Invoke Paint event with the drawing context
                _parent.Paint?.Invoke(_parent, context);
            }
        }
    }
}
