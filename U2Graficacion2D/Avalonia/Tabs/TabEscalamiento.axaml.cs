using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Linq;

namespace U2Graficacion2D.Tabs;

public partial class TabEscalamiento : UserControl
{
    private readonly Point[] _original = { new(0, 60), new(-50, -40), new(50, -40) };

    public TabEscalamiento()
    {
        InitializeComponent();
        ActualizarInfo();
    }

    private void ActualizarInfo()
    {
        if (LblInfo == null || SliderSx == null || SliderSy == null) return;
        double sx = SliderSx.Value;
        double sy = SliderSy.Value;
        LblInfo.Text = $"Escalamiento:  sx = {sx:F2}   sy = {sy:F2}\n" +
                        $"Vértice [0]:  ({_original[0].X:F0},{_original[0].Y:F0})  →  " +
                        $"({_original[0].X * sx:F0},{_original[0].Y * sy:F0})";
    }

    private void Slider_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Slider.ValueProperty)
        {
            ActualizarInfo();
            MainCanvas?.InvalidateVisual();
        }
    }

    private void OnPaint(object? sender, DrawingContext g)
    {
        if (MainCanvas == null || SliderSx == null || SliderSy == null) return;

        double cx = MainCanvas.Bounds.Width / 2;
        double cy = MainCanvas.Bounds.Height / 2;

        GraficoUtil.DibujarEjes(g, cx, cy, MainCanvas.Bounds.Width, MainCanvas.Bounds.Height);

        // Triángulo original (gris)
        var orig = GraficoUtil.ToScreen(_original, cx, cy);
        g.DrawGeometry(null, new Pen(Brushes.Gray, 1), new PolylineGeometry(orig, true));

        // Triángulo escalado
        double sx = SliderSx.Value;
        double sy = SliderSy.Value;
        var scaleMath = _original.Select(p => new Point(p.X * sx, p.Y * sy)).ToArray();
        var scaled = GraficoUtil.ToScreen(scaleMath, cx, cy);
        
        var brush = new SolidColorBrush(Color.FromArgb(80, 100, 220, 0));
        g.DrawGeometry(brush, new Pen(Brushes.LimeGreen, 2), new PolylineGeometry(scaled, true));

        // Leyendas
        var typeface = new Typeface("Inter, Arial, Segoe UI");
        GraficoUtil.DrawText(g, "Original", new Point(4, 4), typeface, 12, Brushes.Gray);
        GraficoUtil.DrawText(g, "Escalado", new Point(4, 20), typeface, 12, Brushes.LimeGreen);
    }
}
