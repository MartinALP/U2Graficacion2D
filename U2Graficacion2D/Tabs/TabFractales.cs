// ============================================================
//  2.4  FRACTALES — Árbol fractal recursivo + Copo de Koch
//
//  Un fractal es una figura con auto-similaridad: cada parte
//  es una copia escalada del todo. Se generan con recursión.
//
//  Aquí se implementan dos fractales clásicos:
//  1. Árbol binario recursivo
//  2. Copo de nieve de Koch (curva fractal de línea)
// ============================================================
using System.Drawing.Drawing2D;

namespace U2Graficacion2D;

public class TabFractales : UserControl
{
    private int _profundidad = 6;
    private float _angulo = 25f;
    private readonly TrackBar _tbProf, _tbAng;
    private readonly ComboBox _cbTipo;
    private readonly Label _lblInfo;
    private readonly Panel _canvas;

    public TabFractales()
    {
        _canvas = new Panel { Dock = DockStyle.Fill, BackColor = Color.Black };
        _canvas.Paint += OnPaint;

        var panel = new Panel { Dock = DockStyle.Bottom, Height = 80, BackColor = Color.WhiteSmoke };

        _tbProf = new TrackBar { Minimum = 1, Maximum = 12, Value = _profundidad, Width = 200, TickFrequency = 1 };
        _tbAng = new TrackBar { Minimum = 5, Maximum = 60, Value = (int)_angulo, Width = 200, TickFrequency = 5 };

        _cbTipo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
        _cbTipo.Items.AddRange(new object[] { "Árbol fractal", "Copo de Koch" });
        _cbTipo.SelectedIndex = 0;

        _tbProf.ValueChanged += (_, _) => { _profundidad = _tbProf.Value; Actualizar(); };
        _tbAng.ValueChanged  += (_, _) => { _angulo = _tbAng.Value; Actualizar(); };
        _cbTipo.SelectedIndexChanged += (_, _) => Actualizar();

        _lblInfo = new Label { AutoSize = true, Font = new Font("Consolas", 9) };

        var ly = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(6) };
        ly.Controls.Add(new Label { Text = "Tipo:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft });
        ly.Controls.Add(_cbTipo);
        ly.Controls.Add(new Label { Text = "Prof:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft });
        ly.Controls.Add(_tbProf);
        ly.Controls.Add(new Label { Text = "Ángulo°:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft });
        ly.Controls.Add(_tbAng);
        ly.Controls.Add(_lblInfo);
        panel.Controls.Add(ly);

        Controls.Add(_canvas);
        Controls.Add(panel);
        Actualizar();
    }

    private void Actualizar()
    {
        _lblInfo.Text = $"Profundidad={_profundidad}  Ángulo={_angulo}°";
        _canvas.Invalidate();
    }

    private void OnPaint(object? s, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        if (_cbTipo.SelectedIndex == 0)
            DibujarArbol(g, _canvas.Width / 2f, _canvas.Height - 20, -90, 90, _profundidad);
        else
            DibujarKoch(g);
    }

    // ── Árbol fractal recursivo ──────────────────────────────
    private void DibujarArbol(Graphics g, float x, float y, float ang, float longitud, int n)
    {
        if (n == 0) return;

        float rad = ang * MathF.PI / 180f;
        float x2 = x + longitud * MathF.Cos(rad);
        float y2 = y + longitud * MathF.Sin(rad);

        // Color que va de verde a amarillo con la profundidad
        int verde = (int)(255 * (n / (float)_profundidad));
        using var pen = new Pen(Color.FromArgb(255, 255 - verde, verde, 0), Math.Max(1, n / 2.5f));
        g.DrawLine(pen, x, y, x2, y2);

        DibujarArbol(g, x2, y2, ang - _angulo, longitud * 0.7f, n - 1);
        DibujarArbol(g, x2, y2, ang + _angulo, longitud * 0.7f, n - 1);
    }

    // ── Copo de nieve de Koch ────────────────────────────────
    private void DibujarKoch(Graphics g)
    {
        float lado = Math.Min(_canvas.Width, _canvas.Height) * 0.55f;
        float cx = _canvas.Width / 2f, cy = _canvas.Height / 2f - 30;
        float h = lado * MathF.Sqrt(3) / 2f;

        PointF a = new(cx, cy - h * 2f / 3f);
        PointF b = new(cx - lado / 2f, cy + h / 3f);
        PointF c = new(cx + lado / 2f, cy + h / 3f);

        var puntos = new List<PointF>();
        Koch(a, b, _profundidad, puntos);
        Koch(b, c, _profundidad, puntos);
        Koch(c, a, _profundidad, puntos);
        puntos.Add(puntos[0]);

        if (puntos.Count > 1)
            g.DrawLines(new Pen(Color.Cyan, 1), puntos.ToArray());
    }

    private static void Koch(PointF p1, PointF p2, int n, List<PointF> pts)
    {
        if (n == 0) { pts.Add(p1); return; }

        float dx = p2.X - p1.X, dy = p2.Y - p1.Y;
        PointF a = new(p1.X + dx / 3f, p1.Y + dy / 3f);
        PointF b = new(p1.X + 2 * dx / 3f, p1.Y + 2 * dy / 3f);
        // Pico del triángulo
        PointF m = new(
            (p1.X + p2.X) / 2f - dy * MathF.Sqrt(3) / 6f,
            (p1.Y + p2.Y) / 2f + dx * MathF.Sqrt(3) / 6f);

        Koch(p1, a, n - 1, pts);
        Koch(a, m, n - 1, pts);
        Koch(m, b, n - 1, pts);
        Koch(b, p2, n - 1, pts);
    }
}
