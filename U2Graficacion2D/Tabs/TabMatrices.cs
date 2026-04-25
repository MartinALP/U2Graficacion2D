// ============================================================
//  2.2  REPRESENTACIÓN MATRICIAL DE LAS TRANSFORMACIONES 2D
//
//  Usando coordenadas homogéneas (3x3) se unifican todas las
//  transformaciones en una sola multiplicación de matrices.
//
//  Traslación:           Escalamiento:         Rotación (θ):
//  [1  0  tx]            [sx  0  0]            [cos θ  -sin θ  0]
//  [0  1  ty]            [0  sy  0]            [sin θ   cos θ  0]
//  [0  0   1]            [0   0  1]            [0       0      1]
//
//  Punto: [x, y, 1]^T
//
//  La composición se logra multiplicando matrices en orden.
//  En este ejemplo se aplican T → S → R en secuencia.
// ============================================================
using System.Drawing.Drawing2D;

namespace U2Graficacion2D;

public class TabMatrices : UserControl
{
    // Vértices en coordenadas MATEMÁTICAS: Y+ hacia arriba
    private readonly PointF[] _original = { new(0, 50), new(-40, -35), new(40, -35) };

    private float _tx = 0, _ty = 0, _sx = 1f, _sy = 1f, _angulo = 0f;

    private readonly TrackBar _tbTx, _tbTy, _tbSx, _tbSy, _tbAng;
    private readonly RichTextBox _txtMatriz;
    private readonly Panel _canvas;

    public TabMatrices()
    {
        _canvas = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        _canvas.Paint += OnPaint;

        var panelDer = new Panel { Dock = DockStyle.Right, Width = 320, BackColor = Color.WhiteSmoke };

        _txtMatriz = new RichTextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Courier New", 9),
            ReadOnly = true,
            BackColor = Color.WhiteSmoke
        };

        var panelCtrl = new Panel { Dock = DockStyle.Top, Height = 170 };

        _tbTx = CrearTrack(-200, 200, 0); _tbTy = CrearTrack(-200, 200, 0);
        _tbSx = CrearTrack(1, 30, 10);   _tbSy = CrearTrack(1, 30, 10);
        _tbAng = CrearTrack(0, 360, 0);

        foreach (var tb in new[] { _tbTx, _tbTy, _tbSx, _tbSy, _tbAng })
            tb.ValueChanged += (_, _) => Actualizar();

        var ly = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(6) };
        ly.Controls.Add(Row("tx:", _tbTx)); ly.Controls.Add(Row("ty:", _tbTy));
        ly.Controls.Add(Row("sx(÷10):", _tbSx)); ly.Controls.Add(Row("sy(÷10):", _tbSy));
        ly.Controls.Add(Row("θ°:", _tbAng));
        panelCtrl.Controls.Add(ly);

        panelDer.Controls.Add(_txtMatriz);
        panelDer.Controls.Add(panelCtrl);

        Controls.Add(_canvas);
        Controls.Add(panelDer);
        Actualizar();
    }

    private void Actualizar()
    {
        _tx = _tbTx.Value; _ty = _tbTy.Value;
        _sx = _tbSx.Value / 10f; _sy = _tbSy.Value / 10f;
        _angulo = _tbAng.Value;

        double r = _angulo * Math.PI / 180.0;
        float cos = (float)Math.Cos(r), sin = (float)Math.Sin(r);

        // Matrices individuales
        float[,] T = { { 1, 0, _tx }, { 0, 1, _ty }, { 0, 0, 1 } };
        float[,] S = { { _sx, 0, 0 }, { 0, _sy, 0 }, { 0, 0, 1 } };
        float[,] R = { { cos, -sin, 0 }, { sin, cos, 0 }, { 0, 0, 1 } };

        // Composición: M = T · S · R
        var M = Mul(T, Mul(S, R));

        _txtMatriz.Text =
            "── Traslación T ──────────────\n" + MatrizStr(T) +
            "\n── Escalamiento S ────────────\n" + MatrizStr(S) +
            "\n── Rotación R ────────────────\n" + MatrizStr(R) +
            "\n── Compuesta M = T·S·R ───────\n" + MatrizStr(M) +
            "\n── Resultado vértice [0] ─────\n" + AplicarStr(M, _original[0]);

        _canvas.Invalidate();
    }

    private void OnPaint(object? s, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        float cx = _canvas.Width / 2f, cy = _canvas.Height / 2f;

        GraficoUtil.DibujarEjes(g, cx, cy, _canvas.Width, _canvas.Height);

        double r = _angulo * Math.PI / 180.0;
        float cos = (float)Math.Cos(r), sin = (float)Math.Sin(r);
        float[,] T = { { 1, 0, _tx }, { 0, 1, _ty }, { 0, 0, 1 } };
        float[,] S = { { _sx, 0, 0 }, { 0, _sy, 0 }, { 0, 0, 1 } };
        float[,] R = { { cos, -sin, 0 }, { sin, cos, 0 }, { 0, 0, 1 } };
        var M = Mul(T, Mul(S, R));

        // Original (gris) — a pantalla
        var orig = GraficoUtil.ToScreen(_original, cx, cy);
        g.DrawPolygon(Pens.Gray, orig);

        var trans = _original.Select(p => AplicarM(M, p, cx, cy)).ToArray();
        using var br = new SolidBrush(Color.FromArgb(80, 0, 150, 255));
        g.FillPolygon(br, trans);
        g.DrawPolygon(new Pen(Color.DodgerBlue, 2), trans);

        g.DrawString("Original", new Font("Segoe UI", 8), Brushes.Gray, 4, 4);
        g.DrawString("Transformado (T·S·R)", new Font("Segoe UI", 8), Brushes.DodgerBlue, 4, 18);
    }

    // ── Helpers matriciales ──────────────────────────────────
    private static float[,] Mul(float[,] A, float[,] B)
    {
        var C = new float[3, 3];
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                for (int k = 0; k < 3; k++)
                    C[i, j] += A[i, k] * B[k, j];
        return C;
    }

    // AplicarM: multiplica la matriz por el punto y convierte a pantalla (Y invertido)
    private static PointF AplicarM(float[,] M, PointF p, float ox, float oy)
    {
        float xn = M[0, 0] * p.X + M[0, 1] * p.Y + M[0, 2];
        float yn = M[1, 0] * p.X + M[1, 1] * p.Y + M[1, 2];
        return new PointF(ox + xn, oy - yn);  // oy - yn: Y+ hacia arriba
    }

    private static string AplicarStr(float[,] M, PointF p)
    {
        float xn = M[0, 0] * p.X + M[0, 1] * p.Y + M[0, 2];
        float yn = M[1, 0] * p.X + M[1, 1] * p.Y + M[1, 2];
        return $"  ({p.X},{p.Y}) → ({xn:F2},{yn:F2})\n";
    }

    private static string MatrizStr(float[,] M)
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < 3; i++)
            sb.AppendLine($"  [{M[i, 0],7:F2}  {M[i, 1],7:F2}  {M[i, 2],7:F2}]");
        return sb.ToString();
    }

    private static TrackBar CrearTrack(int min, int max, int val)
        => new() { Minimum = min, Maximum = max, Value = val, Width = 180, TickFrequency = 30 };

    private static Panel Row(string label, TrackBar tb)
    {
        var p = new Panel { Width = 280, Height = 30 };
        var lbl = new Label { Text = label, Width = 70, Dock = DockStyle.Left, TextAlign = ContentAlignment.MiddleRight };
        tb.Dock = DockStyle.Fill;
        p.Controls.Add(tb); p.Controls.Add(lbl);
        return p;
    }
}
