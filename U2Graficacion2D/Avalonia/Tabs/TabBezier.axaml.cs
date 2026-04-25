using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;

namespace U2Graficacion2D.Tabs;

public partial class TabBezier : UserControl
{
    public TabBezier()
    {
        InitializeComponent();
    }

    private void Slider_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Slider.ValueProperty)
        {
            ActualizarInfo();
            MainCanvas?.InvalidateVisual();
        }
    }

    private void ActualizarInfo()
    {
        if (LblInfo == null || SliderGrado == null) return;
        int grado = (int)SliderGrado.Value;
        LblInfo.Text = $"Curva de Bézier: Grado {grado} ({grado + 1} puntos de control)\n" +
                       $"Fórmula: B(t) = Σ(C(n,i) × (1-t)^(n-i) × t^i × P_i), donde t ∈ [0,1]";
    }

    private void OnPaint(object? sender, DrawingContext g)
    {
        if (MainCanvas == null || SliderGrado == null) return;

        double cx = MainCanvas.Bounds.Width / 2;
        double cy = MainCanvas.Bounds.Height / 2;

        GraficoUtil.DibujarEjes(g, cx, cy, MainCanvas.Bounds.Width, MainCanvas.Bounds.Height);

        int grado = (int)SliderGrado.Value;
        
        // Puntos de control predefinidos
        Point[] controlPoints = grado switch
        {
            1 => new[] { new Point(-100, -80), new Point(100, 80) },
            2 => new[] { new Point(-100, -80), new Point(0, 100), new Point(100, -80) },
            3 => new[] { new Point(-100, -80), new Point(-50, 100), new Point(50, 100), new Point(100, -80) },
            _ => new[] { new Point(-100, -80), new Point(-60, 100), new Point(60, 100), new Point(100, -80) }
        };

        // Dibujar puntos de control
        var screenPts = GraficoUtil.ToScreen(controlPoints, cx, cy);
        foreach (var pt in screenPts)
        {
            g.DrawEllipse(Brushes.Red, null, new Rect(new Point(pt.X - 4, pt.Y - 4), new Size(8, 8)));
        }

        // Dibujar líneas de control
        g.DrawGeometry(null, new Pen(Brushes.Gray, 1), new PolylineGeometry(screenPts, false));

        // Dibujar curva de Bézier
        var curvePoints = CalcularBezier(controlPoints, 100);
        var curvePts = GraficoUtil.ToScreen(curvePoints, cx, cy);
        g.DrawGeometry(null, new Pen(Brushes.Cyan, 2), new PolylineGeometry(curvePts, false));

        // Leyendas
        var typeface = new Typeface("Inter, Arial, Segoe UI");
        GraficoUtil.DrawText(g, "Control Points", new Point(4, 4), typeface, 12, Brushes.Red);
        GraficoUtil.DrawText(g, "Bézier Curve", new Point(4, 20), typeface, 12, Brushes.Cyan);
    }

    private Point[] CalcularBezier(Point[] controlPoints, int segments)
    {
        var points = new List<Point>();
        int n = controlPoints.Length - 1;

        for (int i = 0; i <= segments; i++)
        {
            double t = i / (double)segments;
            Point pt = EvaluarBezier(controlPoints, t);
            points.Add(pt);
        }

        return points.ToArray();
    }

    private Point EvaluarBezier(Point[] P, double t)
    {
        int n = P.Length - 1;
        Point result = new Point(0, 0);

        for (int i = 0; i <= n; i++)
        {
            double coef = BinomialCoefficient(n, i) * Math.Pow(1 - t, n - i) * Math.Pow(t, i);
            result = new Point(result.X + coef * P[i].X, result.Y + coef * P[i].Y);
        }

        return result;
    }

    private double BinomialCoefficient(int n, int k)
    {
        if (k > n) return 0;
        if (k == 0 || k == n) return 1;
        
        double result = 1;
        for (int i = 0; i < k; i++)
        {
            result = result * (n - i) / (i + 1);
        }
        return result;
    }
}
