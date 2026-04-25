using Avalonia;
using Avalonia.Media;
using System;
using System.Linq;
using System.Globalization;

namespace U2Graficacion2D;

internal static class GraficoUtil
{
    // ── Conversión de coordenadas ────────────────────────────
    public static Point ToScreen(Point math, double cx, double cy)
        => new(cx + math.X, cy - math.Y);

    public static Point[] ToScreen(Point[] pts, double cx, double cy)
        => pts.Select(p => ToScreen(p, cx, cy)).ToArray();

    // ── Dibujo de ejes con etiquetas y graduación ────────────
    public static void DibujarEjes(DrawingContext g, double cx, double cy, double ancho, double alto)
    {
        var penEje = new Pen(Brushes.Gray, 1.0);
        var penFlecha = new Pen(Brushes.DimGray, 1.5);

        // Líneas de eje
        g.DrawLine(penEje, new Point(5, cy), new Point(ancho - 5, cy));   // X
        g.DrawLine(penEje, new Point(cx, 5), new Point(cx, alto - 5));    // Y

        // Flechas (en Avalonia no hay EndCap simple como GDI+, se dibujan manualmente si es necesario)
        // Para este prototipo, usaremos líneas simples o pequeños triángulos si se requiere detalle.
        
        // Etiquetas de eje
        var typeface = new Typeface("Inter, Arial, Segoe UI", FontStyle.Normal, FontWeight.Bold);
        
        DrawText(g, "X+", new Point(ancho - 30, cy + 5), typeface, 12, Brushes.DimGray);
        DrawText(g, "X−", new Point(3, cy + 5), typeface, 12, Brushes.DimGray);
        DrawText(g, "Y+", new Point(cx + 5, 3), typeface, 12, Brushes.DimGray);
        DrawText(g, "Y−", new Point(cx + 5, alto - 20), typeface, 12, Brushes.DimGray);

        // Marcas de graduación cada 50 unidades
        var penTick = new Pen(Brushes.LightGray, 1);
        var typefaceTick = new Typeface("Inter, Arial, Segoe UI", FontStyle.Normal, FontWeight.Normal);

        for (int v = -500; v <= 500; v += 50)
        {
            if (v == 0) continue;

            float sx = (float)(cx + v);
            if (sx > 20 && sx < ancho - 20)
            {
                g.DrawLine(penTick, new Point(sx, cy - 4), new Point(sx, cy + 4));
                string lbl = v.ToString();
                DrawText(g, lbl, new Point(sx - 10, cy + 6), typefaceTick, 10, Brushes.DimGray);
            }

            float sy = (float)(cy - v);
            if (sy > 20 && sy < alto - 20)
            {
                g.DrawLine(penTick, new Point(cx - 4, sy), new Point(cx + 4, sy));
                DrawText(g, v.ToString(), new Point(cx + 6, sy - 7), typefaceTick, 10, Brushes.DimGray);
            }
        }
    }

    public static void DrawText(DrawingContext g, string text, Point pos, Typeface typeface, double fontSize, IBrush brush)
    {
        var formattedText = new FormattedText(
            text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            typeface,
            fontSize,
            brush
        );
        g.DrawText(formattedText, pos);
    }
}
