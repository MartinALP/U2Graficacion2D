// ============================================================
//  2.1.1  TRASLACIÓN
//  Mover un objeto sumando un vector (tx, ty) a cada vértice.
//  Fórmula:  x' = x + tx     y' = y + ty
// ============================================================
using System.Drawing.Drawing2D;

namespace U2Graficacion2D;

public class TabTraslacion : UserControl
{
    // Triángulo original (centrado en origen lógico)
    private readonly PointF[] _original = { new(0, -60), new(-50, 40), new(50, 40) };

    private float _tx = 150, _ty = 100;
    private readonly TrackBar _tbTx, _tbTy;
    private readonly Label _lblInfo;
    private readonly Panel _canvas;

    public TabTraslacion()
    {
        // ── Panel de dibujo ──────────────────────────────────
        _canvas = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        _canvas.Paint += OnPaint;

        // ── Controles ───────────────────────────────────────
        var panel = new Panel { Dock = DockStyle.Bottom, Height = 90, BackColor = Color.WhiteSmoke };

        _tbTx = CrearTrack(-300, 300, (int)_tx);
        _tbTy = CrearTrack(-300, 300, (int)_ty);
        _tbTx.ValueChanged += (_, _) => { _tx = _tbTx.Value; Actualizar(); };
        _tbTy.ValueChanged += (_, _) => { _ty = _tbTy.Value; Actualizar(); };

        _lblInfo = new Label { AutoSize = true, Font = new Font("Consolas", 9) };

        var ly = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        ly.Controls.Add(Etiqueta("tx:")); ly.Controls.Add(_tbTx);
        ly.Controls.Add(Etiqueta("ty:")); ly.Controls.Add(_tbTy);
        ly.Controls.Add(_lblInfo);
        panel.Controls.Add(ly);

        Controls.Add(_canvas);
        Controls.Add(panel);
        Actualizar();
    }

    private void Actualizar()
    {
        _lblInfo.Text = $"Traslación:  tx = {_tx:F0}   ty = {_ty:F0}\n" +
                        $"Vértice [0]:  ({_original[0].X:F0},{_original[0].Y:F0})  →  " +
                        $"({_original[0].X + _tx:F0},{_original[0].Y + _ty:F0})";
        _canvas.Invalidate();
    }

    private void OnPaint(object? s, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Origen visual en centro del canvas
        float cx = _canvas.Width / 2f, cy = _canvas.Height / 2f;

        // Ejes
        using var penEje = new Pen(Color.LightGray, 1);
        g.DrawLine(penEje, 0, cy, _canvas.Width, cy);
        g.DrawLine(penEje, cx, 0, cx, _canvas.Height);

        // Triángulo original (gris)
        var orig = Trasladar(_original, cx, cy);
        g.DrawPolygon(Pens.Gray, orig);

        // Triángulo trasladado (azul)
        var tras = Trasladar(_original, cx + _tx, cy + _ty);
        using var br = new SolidBrush(Color.FromArgb(80, 0, 100, 220));
        g.FillPolygon(br, tras);
        g.DrawPolygon(new Pen(Color.Blue, 2), tras);

        // Vector de traslación (flecha roja)
        using var penFlecha = new Pen(Color.Red, 2) { EndCap = LineCap.ArrowAnchor };
        g.DrawLine(penFlecha, cx, cy, cx + _tx, cy + _ty);

        // Leyenda
        g.DrawString("Original", new Font("Segoe UI", 8), Brushes.Gray, 4, 4);
        g.DrawString("Trasladado", new Font("Segoe UI", 8), Brushes.Blue, 4, 18);
        g.DrawString("Vector (tx,ty)", new Font("Segoe UI", 8), Brushes.Red, 4, 32);
    }

    private static PointF[] Trasladar(PointF[] pts, float dx, float dy)
        => pts.Select(p => new PointF(p.X + dx, p.Y + dy)).ToArray();

    private static TrackBar CrearTrack(int min, int max, int val)
        => new() { Minimum = min, Maximum = max, Value = val, Width = 200, TickFrequency = 50 };

    private static Label Etiqueta(string t)
        => new() { Text = t, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft };
}
