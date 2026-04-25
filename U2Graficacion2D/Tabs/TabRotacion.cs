// ============================================================
//  2.1.3  ROTACIÓN
//  Girar un objeto un ángulo θ alrededor del origen.
//  Fórmula:
//    x' = x·cos(θ) - y·sin(θ)
//    y' = x·sin(θ) + y·cos(θ)
// ============================================================
using System.Drawing.Drawing2D;

namespace U2Graficacion2D;

public class TabRotacion : UserControl
{
    private readonly PointF[] _original = { new(0, -70), new(-50, 40), new(50, 40) };
    private float _angulo = 0f;
    private readonly TrackBar _tbAngulo;
    private readonly Label _lblInfo;
    private readonly Panel _canvas;

    public TabRotacion()
    {
        _canvas = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        _canvas.Paint += OnPaint;

        var panel = new Panel { Dock = DockStyle.Bottom, Height = 80, BackColor = Color.WhiteSmoke };

        _tbAngulo = new TrackBar { Minimum = 0, Maximum = 360, Value = 0, Width = 300, TickFrequency = 30 };
        _tbAngulo.ValueChanged += (_, _) => { _angulo = _tbAngulo.Value; Actualizar(); };

        _lblInfo = new Label { AutoSize = true, Font = new Font("Consolas", 9) };

        var ly = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        ly.Controls.Add(new Label { Text = "Ángulo (°):", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft });
        ly.Controls.Add(_tbAngulo);
        ly.Controls.Add(_lblInfo);
        panel.Controls.Add(ly);

        Controls.Add(_canvas);
        Controls.Add(panel);
        Actualizar();
    }

    private void Actualizar()
    {
        double rad = _angulo * Math.PI / 180.0;
        float x0 = _original[0].X, y0 = _original[0].Y;
        float xr = (float)(x0 * Math.Cos(rad) - y0 * Math.Sin(rad));
        float yr = (float)(x0 * Math.Sin(rad) + y0 * Math.Cos(rad));
        _lblInfo.Text = $"θ = {_angulo}°\n" +
                        $"Vértice [0]: ({x0},{y0}) → ({xr:F1},{yr:F1})";
        _canvas.Invalidate();
    }

    private void OnPaint(object? s, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        float cx = _canvas.Width / 2f, cy = _canvas.Height / 2f;

        using var penEje = new Pen(Color.LightGray, 1);
        g.DrawLine(penEje, 0, cy, _canvas.Width, cy);
        g.DrawLine(penEje, cx, 0, cx, _canvas.Height);

        // Original
        var orig = _original.Select(p => new PointF(cx + p.X, cy + p.Y)).ToArray();
        g.DrawPolygon(Pens.Gray, orig);

        // Rotado
        double rad = _angulo * Math.PI / 180.0;
        float cos = (float)Math.Cos(rad), sin = (float)Math.Sin(rad);
        var rot = _original.Select(p => new PointF(
            cx + p.X * cos - p.Y * sin,
            cy + p.X * sin + p.Y * cos)).ToArray();

        using var br = new SolidBrush(Color.FromArgb(80, 200, 0, 200));
        g.FillPolygon(br, rot);
        g.DrawPolygon(new Pen(Color.Purple, 2), rot);

        // Arco que muestra el ángulo
        using var penArc = new Pen(Color.OrangeRed, 1.5f);
        g.DrawArc(penArc, cx - 30, cy - 30, 60, 60, -90, -_angulo);

        g.DrawString("Original",  new Font("Segoe UI", 8), Brushes.Gray, 4, 4);
        g.DrawString($"Rotado {_angulo}°", new Font("Segoe UI", 8), Brushes.Purple, 4, 18);
    }
}
