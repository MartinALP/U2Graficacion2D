using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Linq;

namespace U2Graficacion2D.Tabs;

public partial class TabSesgado : UserControl
{
    private readonly Point[] _original = { new(0, 60), new(-50, -40), new(50, -40) };

    public TabSesgado()
    {
        InitializeComponent();
        ActualizarInfo();
    }

    private void ActualizarInfo()
    {
        if (LblInfo == null || SliderShx == null || SliderShy == null) return;
        double shx = SliderShx.Value;
        double shy = SliderShy.Value;
        LblInfo.Text = $"Sesgado:  shx = {shx:F2}   shy = {shy:F2}\n" +
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
        if (MainCanvas == null || SliderShx == null || SliderShy == null) return;

        double cx = MainCanvas.Bounds.Width / 2;
        double cy = MainCanvas.Bounds.Height / 2;

        GraficoUtil.DibujarEjes(g, cx, cy, MainCanvas.Bounds.Width, MainCanvas.Bounds.Height);

        // Triángulo original (gris)
        var orig = GraficoUtil.ToScreen(_original, cx, cy);
        g.DrawGeometry(null, new Pen(Brushes.Gray, 1), new PolylineGeometry(orig, true));

        // Triángulo sesgado
        double shx = SliderShx.Value;
        double shy = SliderShy.Value;
        var skewMath = _original.Select(p => new Point(p.X + p.Y * shx, p.Y + p.X * shy)).ToArray();
        var skewed = GraficoUtil.ToScreen(skewMath, cx, cy);
        
        var brush = new SolidColorBrush(Color.FromArgb(80, 220, 0, 220));
        g.DrawGeometry(brush, new Pen(Brushes.Magenta, 2), new PolylineGeometry(skewed, true));

        // Leyendas
        var typeface = new Typeface("Inter, Arial, Segoe UI");
        GraficoUtil.DrawText(g, "Original", new Point(4, 4), typeface, 12, Brushes.Gray);
        GraficoUtil.DrawText(g, "Sesgado", new Point(4, 20), typeface, 12, Brushes.Magenta);
    }
}
