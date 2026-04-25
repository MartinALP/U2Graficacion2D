// ============================================================
//  Utilidades compartidas de graficación
//
//  GDI+ usa coordenadas de PANTALLA:  Y positivo = abajo
//  Las transformaciones 2D usan coordenadas MATEMÁTICAS: Y positivo = arriba
//
//  ToScreen convierte:  (x_mat, y_mat)  →  (cx + x_mat,  cy − y_mat)
//    cx, cy = coordenadas de pantalla del origen (0,0) matemático
// ============================================================
using System.Drawing.Drawing2D;

namespace U2Graficacion2D;

internal static class GraficoUtil
{
    // ── Conversión de coordenadas ────────────────────────────
    public static PointF ToScreen(PointF math, float cx, float cy)
        => new(cx + math.X, cy - math.Y);

    public static PointF[] ToScreen(PointF[] pts, float cx, float cy)
        => pts.Select(p => ToScreen(p, cx, cy)).ToArray();

    // ── Dibujo de ejes con etiquetas y graduación ────────────
    public static void DibujarEjes(Graphics g, float cx, float cy, float ancho, float alto)
    {
        using var penEje  = new Pen(Color.Gray, 1f);
        using var penFlecha = new Pen(Color.DimGray, 1.5f)
            { EndCap = LineCap.ArrowAnchor };

        // Líneas de eje
        g.DrawLine(penEje, 5, cy, ancho - 5, cy);   // X
        g.DrawLine(penEje, cx, 5, cx, alto - 5);    // Y

        // Flechas de dirección positiva
        g.DrawLine(penFlecha, cx, cy, ancho - 6, cy);  // X+
        g.DrawLine(penFlecha, cx, cy, cx, 6);           // Y+  (pantalla hacia arriba = Y matemático +)

        // Etiquetas de eje
        using var fntEje = new Font("Segoe UI", 9, FontStyle.Bold);
        g.DrawString("X+", fntEje, Brushes.DimGray, ancho - 30, cy + 5);
        g.DrawString("X−", fntEje, Brushes.DimGray, 3,           cy + 5);
        g.DrawString("Y+", fntEje, Brushes.DimGray, cx + 5,      3);
        g.DrawString("Y−", fntEje, Brushes.DimGray, cx + 5,      alto - 20);

        // Marcas de graduación cada 50 unidades
        using var fntTick = new Font("Segoe UI", 7);
        using var penTick = new Pen(Color.LightGray, 1);

        for (int v = -500; v <= 500; v += 50)
        {
            if (v == 0) continue;

            // Eje X: v positivo → derecha (cx + v)
            float sx = cx + v;
            if (sx > 20 && sx < ancho - 20)
            {
                g.DrawLine(penTick, sx, cy - 4, sx, cy + 4);
                string lbl = v.ToString();
                float lw = lbl.Length * 3.5f;
                g.DrawString(lbl, fntTick, Brushes.DimGray, sx - lw, cy + 6);
            }

            // Eje Y: v positivo → arriba (cy − v en pantalla)
            float sy = cy - v;
            if (sy > 20 && sy < alto - 20)
            {
                g.DrawLine(penTick, cx - 4, sy, cx + 4, sy);
                g.DrawString(v.ToString(), fntTick, Brushes.DimGray, cx + 6, sy - 7);
            }
        }
    }
}
