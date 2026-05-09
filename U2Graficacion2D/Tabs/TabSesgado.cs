using System.Drawing.Drawing2D;

namespace U2Graficacion2D;

public class TabSesgado : UserControl
{
    // ⭐ Estrella en lugar de rectángulo
    private readonly PointF[] _original;

    private float _shx = 0f, _shy = 0f;

    private readonly TrackBar _tbShx, _tbShy, _tbAngulo;
    private readonly Label _lblInfo;
    private readonly Panel _canvas;

    public TabSesgado()
    {
        _original = CrearEstrella(0, 0, 60, 25, 5);

        _canvas = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        _canvas.Paint += OnPaint;

        var panel = new Panel { Dock = DockStyle.Bottom, Height = 110, BackColor = Color.WhiteSmoke };

        // sliders existentes
        _tbShx = CrearTrack(-20, 120, 0);
        _tbShy = CrearTrack(-20, 120, 0);

        // ✅ nuevo slider de ángulo
        _tbAngulo = CrearTrack(-60, 60, 0);

        _tbShx.ValueChanged += (_, _) => { _shx = _tbShx.Value / 10f; Actualizar(); };
        _tbShy.ValueChanged += (_, _) => { _shy = _tbShy.Value / 10f; Actualizar(); };

        // ✅ conversión de grados → tan(ángulo)
        _tbAngulo.ValueChanged += (_, _) =>
        {
            int grados = _tbAngulo.Value;
            double rad = grados * Math.PI / 180.0;
            _shx = (float)Math.Tan(rad);

            _tbShx.Value = Math.Max(_tbShx.Minimum,
                          Math.Min(_tbShx.Maximum, (int)(_shx * 10)));

            Actualizar();
        };

        _lblInfo = new Label { AutoSize = true, Font = new Font("Consolas", 9) };

        var ly = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(8) };

        ly.Controls.Add(Etiqueta("shx (÷10):")); ly.Controls.Add(_tbShx);
        ly.Controls.Add(Etiqueta("shy (÷10):")); ly.Controls.Add(_tbShy);
        ly.Controls.Add(Etiqueta("Ángulo (°):")); ly.Controls.Add(_tbAngulo);

        ly.Controls.Add(_lblInfo);
        panel.Controls.Add(ly);

        Controls.Add(_canvas);
        Controls.Add(panel);

        Actualizar();
    }

    private void Actualizar()
    {
        int ang = _tbAngulo.Value;

        _lblInfo.Text =
            $"Sesgado:\n" +
            $"shx = {_shx:F2}   shy = {_shy:F2}   ángulo = {ang}°\n" +
            $"V0 → (" +
            $"{_original[0].X + _shx * _original[0].Y:F1}, " +
            $"{_original[0].Y + _shy * _original[0].X:F1})";

        _canvas.Invalidate();
    }

    private void OnPaint(object? s, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        float cx = _canvas.Width / 2f, cy = _canvas.Height / 2f;

        GraficoUtil.DibujarEjes(g, cx, cy, _canvas.Width, _canvas.Height);

        // ⭐ original
        var orig = GraficoUtil.ToScreen(_original, cx, cy);
        g.DrawPolygon(Pens.Gray, orig);

        // ⭐ sesgado
        var seg = GraficoUtil.ToScreen(
            _original.Select(p => new PointF(
                p.X + _shx * p.Y,
                p.Y + _shy * p.X)).ToArray(), cx, cy);

        using var br = new SolidBrush(Color.FromArgb(180, 220, 220, 0));
        g.FillPolygon(br, seg);
        g.DrawPolygon(new Pen(Color.Orange, 2), seg);

        g.DrawString("Original", new Font("Segoe UI", 8), Brushes.Gray, 4, 4);
        g.DrawString($"Sesgado shx={_shx:F2} shy={_shy:F2}",
            new Font("Segoe UI", 8), Brushes.DarkOrange, 4, 18);
    }

    // ⭐ función para crear estrella
    private static PointF[] CrearEstrella(float cx, float cy, float rOuter, float rInner, int puntas)
    {
        PointF[] pts = new PointF[puntas * 2];
        double ang = -Math.PI / 2;
        double paso = Math.PI / puntas;

        for (int i = 0; i < pts.Length; i++)
        {
            double r = (i % 2 == 0) ? rOuter : rInner;

            pts[i] = new PointF(
                cx + (float)(r * Math.Cos(ang)),
                cy + (float)(r * Math.Sin(ang))
            );

            ang += paso;
        }

        return pts;
    }

    private static TrackBar CrearTrack(int min, int max, int val)
        => new() { Minimum = min, Maximum = max, Value = val, Width = 180, TickFrequency = 5 };

    private static Label Etiqueta(string t)
        => new() { Text = t, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft };
}