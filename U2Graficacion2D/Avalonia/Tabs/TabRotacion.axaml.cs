using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Linq;

namespace U2Graficacion2D.Tabs;

public partial class TabRotacion : UserControl
{
    private readonly Point[] _original = { new(0, 60), new(-50, -40), new(50, -40) };

    public TabRotacion()
    {
        InitializeComponent();
        ActualizarInfo();
    }

    private void ActualizarInfo()
    {
        if (LblInfo == null || SliderAngulo == null) return;
        double angulo = SliderAngulo.Value;
        LblInfo.Text = $"Rotación:  θ = {angulo:F1}°\n" +
                        $"Vértice [0]:  ({_original[0].X:F0},{_original[0].Y:F0})";
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
        if (MainCanvas == null || SliderAngulo == null) return;

        double cx = MainCanvas.Bounds.Width / 2;
        double cy = MainCanvas.Bounds.Height / 2;

        GraficoUtil.DibujarEjes(g, cx, cy, MainCanvas.Bounds.Width, MainCanvas.Bounds.Height);

        // Triángulo original (gris)
        var orig = GraficoUtil.ToScreen(_original, cx, cy);
        g.DrawGeometry(null, new Pen(Brushes.Gray, 1), new PolylineGeometry(orig, true));

        // Triángulo rotado
        double anguloRad = SliderAngulo.Value * Math.PI / 180;
        var rotMath = _original.Select(p => RotarPunto(p, anguloRad)).ToArray();
        var rotated = GraficoUtil.ToScreen(rotMath, cx, cy);
        
        var brush = new SolidColorBrush(Color.FromArgb(80, 220, 100, 0));
        g.DrawGeometry(brush, new Pen(Brushes.Orange, 2), new PolylineGeometry(rotated, true));

        // Leyendas
        var typeface = new Typeface("Inter, Arial, Segoe UI");
        GraficoUtil.DrawText(g, "Original", new Point(4, 4), typeface, 12, Brushes.Gray);
        GraficoUtil.DrawText(g, "Rotado", new Point(4, 20), typeface, 12, Brushes.Orange);
    }

    private Point RotarPunto(Point p, double angulo)
    {
        double x = p.X * Math.Cos(angulo) - p.Y * Math.Sin(angulo);
        double y = p.X * Math.Sin(angulo) + p.Y * Math.Cos(angulo);
        return new Point(x, y);
    }
}
