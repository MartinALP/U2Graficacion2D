using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;

namespace U2Graficacion2D.Tabs;

public partial class TabMatrices : UserControl
{
    public TabMatrices()
    {
        InitializeComponent();
        ActualizarInfo();
    }

    private void ActualizarInfo()
    {
        if (LblInfo == null) return;
        LblInfo.Text = "Matrices de Transformación 2D Homogéneas:\n\n" +
                       "Traslación:  [1 0 tx]    Escalamiento: [sx  0 0]    Rotación: [cos θ -sin θ 0]\n" +
                       "             [0 1 ty]                  [ 0 sy 0]               [sin θ  cos θ 0]\n" +
                       "             [0 0  1]                  [ 0  0 1]               [  0      0    1]\n\n" +
                       "Compósición: M_resultante = M₁ × M₂ × M₃ × ... × P";
    }

    private void OnPaint(object? sender, DrawingContext g)
    {
        if (MainCanvas == null) return;

        double cx = MainCanvas.Bounds.Width / 2;
        double cy = MainCanvas.Bounds.Height / 2;

        GraficoUtil.DibujarEjes(g, cx, cy, MainCanvas.Bounds.Width, MainCanvas.Bounds.Height);

        // Ejemplos visuales de diferentes transformaciones
        var typeface = new Typeface("Inter, Arial, Segoe UI", FontStyle.Normal, FontWeight.Bold);
        
        GraficoUtil.DrawText(g, "Matrices de Transformación 2D", new Point(10, 10), typeface, 14, Brushes.Cyan);
        
        double y = 30;
        GraficoUtil.DrawText(g, "Traslación (tx, ty) | Escalamiento (sx, sy) | Rotación (θ)", new Point(10, y), typeface, 11, Brushes.Yellow);
        GraficoUtil.DrawText(g, "Sesgado (shx, shy) | Composición de transformaciones", new Point(10, y + 20), typeface, 11, Brushes.LimeGreen);
    }
}
