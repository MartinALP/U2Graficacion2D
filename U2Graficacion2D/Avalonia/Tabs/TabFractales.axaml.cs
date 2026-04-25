using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;

namespace U2Graficacion2D.Tabs;

public partial class TabFractales : UserControl
{
    public TabFractales()
    {
        InitializeComponent();
    }

    private void Control_Changed(object? sender, SelectionChangedEventArgs e)
    {
        MainCanvas?.InvalidateVisual();
    }

    private void Slider_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Slider.ValueProperty)
        {
            MainCanvas?.InvalidateVisual();
        }
    }

    private void OnPaint(object? sender, DrawingContext g)
    {
        if (MainCanvas == null || CbTipo == null || SliderProf == null || SliderAng == null) return;

        if (CbTipo.SelectedIndex == 0)
            DibujarArbol(g, MainCanvas.Bounds.Width / 2.0, MainCanvas.Bounds.Height - 20, -90, 90, (int)SliderProf.Value);
        else
            DibujarKoch(g);
    }

    private void DibujarArbol(DrawingContext g, double x, double y, double ang, double longitud, int n)
    {
        if (n == 0) return;

        double rad = ang * Math.PI / 180.0;
        double x2 = x + longitud * Math.Cos(rad);
        double y2 = y + longitud * Math.Sin(rad);

        int profTotal = (int)SliderProf.Value;
        byte verde = (byte)(255 * (n / (float)profTotal));
        var color = Color.FromRgb(255, (byte)(255 - verde), 0);
        
        g.DrawLine(new Pen(new SolidColorBrush(color), Math.Max(1, n / 2.5)), new Point(x, y), new Point(x2, y2));

        double anguloDelta = SliderAng.Value;
        DibujarArbol(g, x2, y2, ang - anguloDelta, longitud * 0.7, n - 1);
        DibujarArbol(g, x2, y2, ang + anguloDelta, longitud * 0.7, n - 1);
    }

    private void DibujarKoch(DrawingContext g)
    {
        double lado = Math.Min(MainCanvas.Bounds.Width, MainCanvas.Bounds.Height) * 0.55;
        double cx = MainCanvas.Bounds.Width / 2.0, cy = MainCanvas.Bounds.Height / 2.0 - 30;
        double h = lado * Math.Sqrt(3) / 2.0;

        Point a = new(cx, cy - h * 2.0 / 3.0);
        Point b = new(cx - lado / 2.0, cy + h / 3.0);
        Point c = new(cx + lado / 2.0, cy + h / 3.0);

        var puntos = new List<Point>();
        Koch(a, b, (int)SliderProf.Value, puntos);
        Koch(b, c, (int)SliderProf.Value, puntos);
        Koch(c, a, (int)SliderProf.Value, puntos);
        puntos.Add(puntos[0]);

        if (puntos.Count > 1)
        {
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(puntos[0], false);
                for (int i = 1; i < puntos.Count; i++)
                    context.LineTo(puntos[i]);
                context.EndFigure(false);
            }
            g.DrawGeometry(null, new Pen(Brushes.Cyan, 1), geometry);
        }
    }

    private static void Koch(Point p1, Point p2, int n, List<Point> pts)
    {
        if (n == 0) { pts.Add(p1); return; }

        double dx = p2.X - p1.X, dy = p2.Y - p1.Y;
        Point a = new(p1.X + dx / 3.0, p1.Y + dy / 3.0);
        Point b = new(p1.X + 2 * dx / 3.0, p1.Y + 2 * dy / 3.0);
        Point m = new(
            (p1.X + p2.X) / 2.0 - dy * Math.Sqrt(3) / 6.0,
            (p1.Y + p2.Y) / 2.0 + dx * Math.Sqrt(3) / 6.0);

        Koch(p1, a, n - 1, pts);
        Koch(a, m, n - 1, pts);
        Koch(m, b, n - 1, pts);
        Koch(b, p2, n - 1, pts);
    }
}
