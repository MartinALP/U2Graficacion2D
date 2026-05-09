// ============================================================
//  2.1.2  ESCALAMIENTO
//  Cambiar el tamaño multiplicando cada coordenada por un factor.
//  Fórmula:  x' = sx * x     y' = sy * y
//  (respecto al origen; si se quiere respecto a un pivote p:
//   trasladar al origen → escalar → trasladar de vuelta)
// ============================================================
using System.Drawing.Drawing2D;

namespace U2Graficacion2D;

public class TabEscalamiento : UserControl
{
    // Vértices en coordenadas MATEMÁTICAS: Y+ hacia arriba
<<<<<<< HEAD
    private readonly PointF[] _original = { new(0, 20), new(-50, -40), new(50, -40) };

    private readonly PointF[] _original = { new(0, 40), new(-50, -40), new(50, -40) };
>>>>>>> Walas

    private float _sx = 1f, _sy = 1f;
    private readonly TrackBar _tbSx, _tbSy;
    private readonly Label _lblInfo;
    private readonly Panel _canvas;

    public TabEscalamiento()
    {
        _canvas = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        _canvas.Paint += OnPaint;

        var panel = new Panel { Dock = DockStyle.Bottom, Height = 90, BackColor = Color.WhiteSmoke };

        // Factor: 10 → 0.1x ... 40 → 4.0x  (dividir entre 10)
        _tbSx = CrearTrack(1, 40, 10);
        _tbSy = CrearTrack(1, 40, 10);
        _tbSx.ValueChanged += (_, _) => { _sx = _tbSx.Value / 10f; Actualizar(); };
        _tbSy.ValueChanged += (_, _) => { _sy = _tbSy.Value / 10f; Actualizar(); };

        _lblInfo = new Label { AutoSize = true, Font = new Font("Consolas", 9) };

        var ly = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        ly.Controls.Add(Etiqueta("sx (÷10):")); ly.Controls.Add(_tbSx);
        ly.Controls.Add(Etiqueta("sy (÷10):")); ly.Controls.Add(_tbSy);
        ly.Controls.Add(_lblInfo);
        panel.Controls.Add(ly);

        Controls.Add(_canvas);
        Controls.Add(panel);
        Actualizar();
    }

    private void Actualizar()
    {
        _lblInfo.Text = $"Escalamiento:  sx = {_sx:F1}   sy = {_sy:F1}\n" +
                        $"Vértice [0]:  ({_original[0].X:F0},{_original[0].Y:F0})  →  " +
                        $"({_sx * _original[0].X:F0},{_sy * _original[0].Y:F0})";
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

        // Escalado: x'=sx·x  y'=sy·y  (luego a pantalla)
        var esc = GraficoUtil.ToScreen(
            _original.Select(p => new PointF(_sx * p.X, _sy * p.Y)).ToArray(), cx, cy);
        using var br = new SolidBrush(Color.FromArgb(80, 0, 180, 0));
        g.FillPolygon(br, esc);
        g.DrawPolygon(new Pen(Color.Green, 2), esc);

        g.DrawString("Original", new Font("Segoe UI", 8), Brushes.Gray, 4, 4);
        g.DrawString($"Escalado sx={_sx:F1} sy={_sy:F1}", new Font("Segoe UI", 8), Brushes.Green, 4, 18);
    }

    private static TrackBar CrearTrack(int min, int max, int val)
        => new() { Minimum = min, Maximum = max, Value = val, Width = 200, TickFrequency = 5 };

    private static Label Etiqueta(string t)
        => new() { Text = t, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft };
}
