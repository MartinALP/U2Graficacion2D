// ============================================================
//  2.3.2  CURVAS B-SPLINE (cúbica uniforme)
//
//  Las B-Splines son generalizaciones de Bézier con la ventaja
//  de control local: mover un punto solo afecta segmentos cercanos.
//
//  Para una B-Spline cúbica uniforme, cada segmento usa 4 puntos
//  de control consecutivos y la base está dada por la matriz M:
//
//       M = (1/6) · [-1  3 -3  1]
//                   [ 3 -6  3  0]
//                   [-3  0  3  0]
//                   [ 1  4  1  0]
//
//  Q(t) = [t³ t² t 1] · M · [Pi Pi+1 Pi+2 Pi+3]^T
//
//  Los puntos de control son arrastrables.
// ============================================================
using System.Drawing.Drawing2D;

namespace U2Graficacion2D;

public class TabBSpline : UserControl
{
    private PointF[] _ctrl =
    {
        new(60, 350), new(150, 100), new(280, 300), new(400, 80),
        new(520, 300), new(620, 150), new(700, 350)
    };
    private int _drag = -1;
    private readonly Label _lblInfo;

    public TabBSpline()
    {
        BackColor = Color.White;
        _lblInfo = new Label
        {
            Dock = DockStyle.Bottom, Height = 40,
            Font = new Font("Consolas", 9),
            Text = "Arrastra los puntos de control (■). Cada grupo de 4 genera un segmento."
        };
        Controls.Add(_lblInfo);

        MouseDown += (_, e) => { _drag = PuntoMasCercano(e.Location); Invalidate(); };
        MouseMove += (_, e) => { if (_drag >= 0) { _ctrl[_drag] = e.Location; Invalidate(); } };
        MouseUp   += (_, _) => _drag = -1;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Polígono de control
        using var penCtrl = new Pen(Color.LightGray, 1) { DashStyle = DashStyle.Dash };
        g.DrawLines(penCtrl, _ctrl);

        // Segmentos B-Spline cúbica uniforme
        Color[] colores = { Color.Blue, Color.DarkGreen, Color.DarkRed, Color.Purple, Color.Teal };
        for (int i = 0; i + 3 < _ctrl.Length; i++)
        {
            var seg = BSplineSegmento(_ctrl[i], _ctrl[i + 1], _ctrl[i + 2], _ctrl[i + 3], 200);
            var col = colores[i % colores.Length];
            g.DrawLines(new Pen(col, 2), seg);
        }

        // Puntos de control
        for (int i = 0; i < _ctrl.Length; i++)
        {
            var c = i == _drag ? Color.Red : Color.DarkSlateGray;
            g.FillRectangle(new SolidBrush(c), _ctrl[i].X - 5, _ctrl[i].Y - 5, 10, 10);
            g.DrawString($"P{i}", new Font("Segoe UI", 8), Brushes.Black, _ctrl[i].X + 7, _ctrl[i].Y - 9);
        }
    }

    private static PointF[] BSplineSegmento(PointF p0, PointF p1, PointF p2, PointF p3, int pasos)
    {
        var pts = new PointF[pasos + 1];
        for (int i = 0; i <= pasos; i++)
        {
            float t = i / (float)pasos;
            float t2 = t * t, t3 = t2 * t;
            // Polinomios base B-Spline cúbica uniforme
            float b0 = (-t3 + 3 * t2 - 3 * t + 1) / 6f;
            float b1 = (3 * t3 - 6 * t2 + 4) / 6f;
            float b2 = (-3 * t3 + 3 * t2 + 3 * t + 1) / 6f;
            float b3 = t3 / 6f;
            pts[i] = new PointF(
                b0 * p0.X + b1 * p1.X + b2 * p2.X + b3 * p3.X,
                b0 * p0.Y + b1 * p1.Y + b2 * p2.Y + b3 * p3.Y);
        }
        return pts;
    }

    private int PuntoMasCercano(Point mouse)
    {
        int idx = -1; float minD = 15;
        for (int i = 0; i < _ctrl.Length; i++)
        {
            float d = (float)Math.Sqrt(Math.Pow(_ctrl[i].X - mouse.X, 2) + Math.Pow(_ctrl[i].Y - mouse.Y, 2));
            if (d < minD) { minD = d; idx = i; }
        }
        return idx;
    }
}
