// ============================================================
//  2.1.4  SESGADO (Shear)
//  Deforma el objeto inclinándolo en X o en Y.
//  Fórmula sesgado en X:   x' = x + shx·y    y' = y
//  Fórmula sesgado en Y:   x' = x             y' = y + shy·x
// ============================================================
using System.Drawing.Drawing2D;

namespace U2Graficacion2D;

public class TabSesgado : UserControl
{
    // Vértices en coordenadas MATEMÁTICAS: Y+ hacia arriba
    // Rectángulo: esquina superior-derecha=(60,50), inferior-izquierda=(-60,-50)
    private readonly PointF[] _original = { new(-60, -50), new(60, -50), new(60, 50), new(-60, 50) };
    private float _shx = 0f, _shy = 0f;
    private readonly TrackBar _tbShx, _tbShy;
    private readonly Label _lblInfo;
    private readonly Panel _canvas;

    public TabSesgado()
    {
        _canvas = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        _canvas.Paint += OnPaint;

        var panel = new Panel { Dock = DockStyle.Bottom, Height = 90, BackColor = Color.WhiteSmoke };

        // -20 a 20 (dividir entre 10) → -2.0 a 2.0
        _tbShx = CrearTrack(-20, 20, 0);
        _tbShy = CrearTrack(-20, 20, 0);
        _tbShx.ValueChanged += (_, _) => { _shx = _tbShx.Value / 10f; Actualizar(); };
        _tbShy.ValueChanged += (_, _) => { _shy = _tbShy.Value / 10f; Actualizar(); };

        _lblInfo = new Label { AutoSize = true, Font = new Font("Consolas", 9) };

        var ly = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        ly.Controls.Add(Etiqueta("shx (÷10):")); ly.Controls.Add(_tbShx);
        ly.Controls.Add(Etiqueta("shy (÷10):")); ly.Controls.Add(_tbShy);
        ly.Controls.Add(_lblInfo);
        panel.Controls.Add(ly);

        Controls.Add(_canvas);
        Controls.Add(panel);
        Actualizar();
    }

    private void Actualizar()
    {
        _lblInfo.Text = $"Sesgado:  shx = {_shx:F1}   shy = {_shy:F1}\n" +
                        $"Vértice [0]: ({_original[0].X},{_original[0].Y}) → " +
                        $"({_original[0].X + _shx * _original[0].Y:F1}, " +
                        $"{_original[0].Y + _shy * _original[0].X:F1})";
        _canvas.Invalidate();
    }

    private void OnPaint(object? s, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        float cx = _canvas.Width / 2f, cy = _canvas.Height / 2f;

        GraficoUtil.DibujarEjes(g, cx, cy, _canvas.Width, _canvas.Height);

        // Original (gris)
        var orig = GraficoUtil.ToScreen(_original, cx, cy);
        g.DrawPolygon(Pens.Gray, orig);

        // Sesgado: x'=x+shx·y   y'=y+shy·x  (luego a pantalla)
        var seg = GraficoUtil.ToScreen(
            _original.Select(p => new PointF(
                p.X + _shx * p.Y,
                p.Y + _shy * p.X)).ToArray(), cx, cy);

        using var br = new SolidBrush(Color.FromArgb(80, 220, 120, 0));
        g.FillPolygon(br, seg);
        g.DrawPolygon(new Pen(Color.DarkOrange, 2), seg);

        g.DrawString("Original",  new Font("Segoe UI", 8), Brushes.Gray, 4, 4);
        g.DrawString($"Sesgado shx={_shx:F1} shy={_shy:F1}", new Font("Segoe UI", 8), Brushes.DarkOrange, 4, 18);
    }

    private static TrackBar CrearTrack(int min, int max, int val)
        => new() { Minimum = min, Maximum = max, Value = val, Width = 200, TickFrequency = 5 };

    private static Label Etiqueta(string t)
        => new() { Text = t, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft };
}
