using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Linq;

namespace U2Graficacion2D.Tabs;

public partial class TabTraslacion : UserControl
{
    private readonly Point[] _original = { new(0, 60), new(-50, -40), new(50, -40) };

    public TabTraslacion()
    {
        InitializeComponent();
        ActualizarInfo();
    }

    private void ActualizarInfo()
    {
        if (LblInfo == null || SliderTx == null || SliderTy == null) return;
        double tx = SliderTx.Value;
        double ty = SliderTy.Value;
        LblInfo.Text = $"Traslación:  tx = {tx:F0}   ty = {ty:F0}\n" +
                        $"Vértice [0]:  ({_original[0].X:F0},{_original[0].Y:F0})  →  " +
                        $"({_original[0].X + tx:F0},{_original[0].Y + ty:F0})";
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
        if (MainCanvas == null || SliderTx == null || SliderTy == null) return;

        double cx = MainCanvas.Bounds.Width / 2;
        double cy = MainCanvas.Bounds.Height / 2;

        GraficoUtil.DibujarEjes(g, cx, cy, MainCanvas.Bounds.Width, MainCanvas.Bounds.Height);

        // Triángulo original (gris)
        var orig = GraficoUtil.ToScreen(_original, cx, cy);
        g.DrawGeometry(null, new Pen(Brushes.Gray, 1), new PolylineGeometry(orig, true));

        // Triángulo trasladado
        double tx = SliderTx.Value;
        double ty = SliderTy.Value;
        var trasMath = _original.Select(p => new Point(p.X + tx, p.Y + ty)).ToArray();
        var tras = GraficoUtil.ToScreen(trasMath, cx, cy);
        
        var brush = new SolidColorBrush(Color.FromArgb(80, 0, 100, 220));
        g.DrawGeometry(brush, new Pen(Brushes.Blue, 2), new PolylineGeometry(tras, true));

        // Vector de traslación
        g.DrawLine(new Pen(Brushes.Red, 2), new Point(cx, cy), new Point(cx + tx, cy - ty));

        // Leyendas
        var typeface = new Typeface("Inter, Arial, Segoe UI");
        GraficoUtil.DrawText(g, "Original", new Point(4, 4), typeface, 12, Brushes.Gray);
        GraficoUtil.DrawText(g, "Trasladado", new Point(4, 20), typeface, 12, Brushes.Blue);
        GraficoUtil.DrawText(g, "Vector (tx,ty)", new Point(4, 36), typeface, 12, Brushes.Red);
    }
}
