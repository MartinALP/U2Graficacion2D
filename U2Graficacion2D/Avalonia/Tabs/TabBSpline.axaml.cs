using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;

namespace U2Graficacion2D.Tabs;

public partial class TabBSpline : UserControl
{
    public TabBSpline()
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
        LblInfo.Text = $"B-Spline: Grado {grado} (uniforme, no racional)\n" +
                       $"Ventaja: control local sobre la curva";
    }

    private void OnPaint(object? sender, DrawingContext g)
    {
        if (MainCanvas == null || SliderGrado == null) return;

        double cx = MainCanvas.Bounds.Width / 2;
        double cy = MainCanvas.Bounds.Height / 2;

        GraficoUtil.DibujarEjes(g, cx, cy, MainCanvas.Bounds.Width, MainCanvas.Bounds.Height);

        int grado = (int)SliderGrado.Value;
        
        // Puntos de control
        Point[] controlPoints = new[]
        {
            new Point(-120, -60),
            new Point(-60, 100),
            new Point(0, -80),
            new Point(60, 100),
            new Point(120, -60)
        };

        // Dibujar puntos de control
        var screenPts = GraficoUtil.ToScreen(controlPoints, cx, cy);
        foreach (var pt in screenPts)
        {
            g.DrawEllipse(Brushes.Lime, null, new Rect(new Point(pt.X - 4, pt.Y - 4), new Size(8, 8)));
        }

        // Dibujar líneas de control
        g.DrawGeometry(null, new Pen(Brushes.Gray, 1), new PolylineGeometry(screenPts, false));

        // Dibujar curva B-Spline (aproximación simple)
        var curvePoints = CalcularBSpline(controlPoints, grado, 100);
        var curvePts = GraficoUtil.ToScreen(curvePoints, cx, cy);
        g.DrawGeometry(null, new Pen(Brushes.Yellow, 2), new PolylineGeometry(curvePts, false));

        // Leyendas
        var typeface = new Typeface("Inter, Arial, Segoe UI");
        GraficoUtil.DrawText(g, "Control Points", new Point(4, 4), typeface, 12, Brushes.Lime);
        GraficoUtil.DrawText(g, "B-Spline", new Point(4, 20), typeface, 12, Brushes.Yellow);
    }

    private Point[] CalcularBSpline(Point[] P, int grado, int segments)
    {
        var points = new List<Point>();
        int n = P.Length - 1;
        int k = grado + 1; // Order = grado + 1

        // Nodos uniformes
        double[] nodos = new double[n + k + 1];
        for (int i = 0; i < nodos.Length; i++)
        {
            if (i < k) nodos[i] = 0;
            else if (i >= n + 1) nodos[i] = n - grado + 1;
            else nodos[i] = i - grado;
        }

        // Evaluar la curva
        for (int i = 0; i <= segments; i++)
        {
            double u = (double)i / segments * (n - grado + 1);
            Point pt = EvaluarBSpline(P, u, grado, nodos);
            points.Add(pt);
        }

        return points.ToArray();
    }

    private Point EvaluarBSpline(Point[] P, double u, int grado, double[] nodos)
    {
        int n = P.Length - 1;
        Point resultado = new Point(0, 0);

        for (int i = 0; i <= n; i++)
        {
            double ni = FuncionBasis(i, grado, u, nodos);
            resultado = new Point(resultado.X + ni * P[i].X, resultado.Y + ni * P[i].Y);
        }

        return resultado;
    }

    private double FuncionBasis(int i, int p, double u, double[] nodos)
    {
        if (p == 0)
        {
            return (u >= nodos[i] && u < nodos[i + 1]) ? 1.0 : 0.0;
        }

        double lizado = 0, derecho = 0;
        
        if (nodos[i + p] != nodos[i])
            lizado = (u - nodos[i]) / (nodos[i + p] - nodos[i]) * FuncionBasis(i, p - 1, u, nodos);
        
        if (nodos[i + p + 1] != nodos[i + 1])
            derecho = (nodos[i + p + 1] - u) / (nodos[i + p + 1] - nodos[i + 1]) * FuncionBasis(i + 1, p - 1, u, nodos);

        return lizado + derecho;
    }
}
