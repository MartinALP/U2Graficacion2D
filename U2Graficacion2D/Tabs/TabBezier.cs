// ============================================================
//  2.3.1  CURVAS DE BÉZIER
//
//  Una curva de Bézier de grado n está definida por n+1
//  puntos de control P0..Pn usando los polinomios de Bernstein:
//
//       B(t) = Σ C(n,i) · (1-t)^(n-i) · t^i · Pi    t ∈ [0,1]
//
//  Ejemplo: Bézier cúbica (4 puntos de control):
//  B(t) = (1-t)³P0 + 3(1-t)²tP1 + 3(1-t)t²P2 + t³P3
//
//  Los puntos de control se arrastran con el ratón.
// ============================================================
using System.Drawing.Drawing2D;

namespace U2Graficacion2D;

public class TabBezier : UserControl
{
    private PointF[] _ctrl = { new(80, 350), new(200, 80), new(450, 80), new(570, 350) };
    private int _drag = -1;
    private readonly Label _lblInfo;

    public TabBezier()
    {
        BackColor = Color.White;
        _lblInfo = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 40,
            Font = new Font("Consolas", 9),
            Text = "Arrastra los puntos de control (■) para modificar la curva."
        };
        Controls.Add(_lblInfo);

        MouseDown += (_, e) => { _drag = PuntoMasCercano(e.Location); Invalidate(); };
        MouseMove += (_, e) => { if (_drag >= 0) { _ctrl[_drag] = e.Location; Actualizar(); } };
        MouseUp   += (_, _) => _drag = -1;
    }

    private void Actualizar()
    {
        _lblInfo.Text = string.Join("   ", _ctrl.Select((p, i) => $"P{i}=({p.X:F0},{p.Y:F0})"));
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Polígono de control (líneas punteadas)
        using var penCtrl = new Pen(Color.LightBlue, 1) { DashStyle = DashStyle.Dash };
        g.DrawLines(penCtrl, _ctrl);

        // Curva de Bézier cúbica
        const int pasos = 300;
        var pts = new PointF[pasos + 1];
        for (int i = 0; i <= pasos; i++)
        {
            float t = i / (float)pasos;
            pts[i] = Bezier4(t, _ctrl[0], _ctrl[1], _ctrl[2], _ctrl[3]);
        }
        g.DrawLines(new Pen(Color.DarkBlue, 2), pts);

        // Puntos de control
        for (int i = 0; i < _ctrl.Length; i++)
        {
            var c = i == _drag ? Color.Red : Color.DodgerBlue;
            g.FillRectangle(new SolidBrush(c), _ctrl[i].X - 6, _ctrl[i].Y - 6, 12, 12);
            g.DrawString($"P{i}", new Font("Segoe UI", 8), Brushes.Black, _ctrl[i].X + 8, _ctrl[i].Y - 10);
        }

        // Punto en t=0.5 (demostración)
        var mid = Bezier4(0.5f, _ctrl[0], _ctrl[1], _ctrl[2], _ctrl[3]);
        g.FillEllipse(Brushes.OrangeRed, mid.X - 5, mid.Y - 5, 10, 10);
        g.DrawString("t=0.5", new Font("Segoe UI", 8), Brushes.OrangeRed, mid.X + 6, mid.Y - 6);
    }

    private static PointF Bezier4(float t, PointF p0, PointF p1, PointF p2, PointF p3)
    {
        float u = 1 - t;
        float x = u * u * u * p0.X + 3 * u * u * t * p1.X + 3 * u * t * t * p2.X + t * t * t * p3.X;
        float y = u * u * u * p0.Y + 3 * u * u * t * p1.Y + 3 * u * t * t * p2.Y + t * t * t * p3.Y;
        return new PointF(x, y);
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
