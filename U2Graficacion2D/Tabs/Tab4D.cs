// ============================================================
//  4D HYPERCUBE (TESSERACT) - ROTACIÓN 4D
//  Proyección de un hipercubo 4D a 2D con rotación
//  Estudiante: Emmanuel Valenzuela
//  Matrícula: 22540083
// ============================================================
using System.Drawing.Drawing2D;

namespace U2Graficacion2D;

public class Tab4D : UserControl
{
    // 16 vértices de un hipercubo 4D (coordenadas: x, y, z, w)
    // Rango: ±1 en cada dimensión
    private readonly float[,] _vertices4D = new float[,]
    {
        // Cubo en z=0, w=0
        { -1, -1, -1, -1 }, { 1, -1, -1, -1 }, { 1, 1, -1, -1 }, { -1, 1, -1, -1 },
        // Cubo en z=1, w=0
        { -1, -1, 1, -1 }, { 1, -1, 1, -1 }, { 1, 1, 1, -1 }, { -1, 1, 1, -1 },
        // Cubo en z=0, w=1
        { -1, -1, -1, 1 }, { 1, -1, -1, 1 }, { 1, 1, -1, 1 }, { -1, 1, -1, 1 },
        // Cubo en z=1, w=1
        { -1, -1, 1, 1 }, { 1, -1, 1, 1 }, { 1, 1, 1, 1 }, { -1, 1, 1, 1 }
    };

    // Aristas del hipercubo 4D (conecta vértices que difieren en exactamente una coordenada)
    private readonly (int, int)[] _edges = new[]
    {
        // Aristas en z=0, w=0
        (0, 1), (1, 2), (2, 3), (3, 0),
        // Aristas en z=1, w=0
        (4, 5), (5, 6), (6, 7), (7, 4),
        // Aristas en z=0, w=1
        (8, 9), (9, 10), (10, 11), (11, 8),
        // Aristas en z=1, w=1
        (12, 13), (13, 14), (14, 15), (15, 12),
        // Aristas Z (conectan w=0 con w=0)
        (0, 4), (1, 5), (2, 6), (3, 7),
        // Aristas W (conectan w=0 con w=1)
        (0, 8), (1, 9), (2, 10), (3, 11),
        (4, 12), (5, 13), (6, 14), (7, 15)
    };

    private float _rotXY = 0f, _rotXW = 0f, _rotYW = 0f;
    private readonly Panel _canvas;
    private readonly TrackBar _tbRotXY, _tbRotXW, _tbRotYW;
    private readonly Label _lblInfo;

    public Tab4D()
    {
        _canvas = new Panel { Dock = DockStyle.Fill, BackColor = Color.Black };
        _canvas.Paint += OnPaint;

        var panelControls = new Panel { Dock = DockStyle.Bottom, Height = 140, BackColor = Color.DarkGray };

        _tbRotXY = CreateTrackBar("Rotación XY");
        _tbRotXW = CreateTrackBar("Rotación XW");
        _tbRotYW = CreateTrackBar("Rotación YW");

        _tbRotXY.ValueChanged += (_, _) => { _rotXY = _tbRotXY.Value * 360f / 360f; Actualizar(); };
        _tbRotXW.ValueChanged += (_, _) => { _rotXW = _tbRotXW.Value * 360f / 360f; Actualizar(); };
        _tbRotYW.ValueChanged += (_, _) => { _rotYW = _tbRotYW.Value * 360f / 360f; Actualizar(); };

        _lblInfo = new Label
        {
            AutoSize = true,
            Font = new Font("Consolas", 9),
            ForeColor = Color.White
        };

        var ly = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(8), AutoScroll = true };
        ly.Controls.Add(new Label { Text = "Rotación XY:", AutoSize = true, ForeColor = Color.White });
        ly.Controls.Add(_tbRotXY);
        ly.Controls.Add(new Label { Text = "Rotación XW:", AutoSize = true, ForeColor = Color.White });
        ly.Controls.Add(_tbRotXW);
        ly.Controls.Add(new Label { Text = "Rotación YW:", AutoSize = true, ForeColor = Color.White });
        ly.Controls.Add(_tbRotYW);
        ly.Controls.Add(_lblInfo);
        panelControls.Controls.Add(ly);

        Controls.Add(_canvas);
        Controls.Add(panelControls);
    }

    private TrackBar CreateTrackBar(string name)
    {
        return new TrackBar
        {
            Name = name,
            Minimum = 0,
            Maximum = 360,
            Value = 0,
            Width = 200,
            TickFrequency = 30
        };
    }

    private void Actualizar()
    {
        _lblInfo.Text = $"XY: {_rotXY:F1}°  |  XW: {_rotXW:F1}°  |  YW: {_rotYW:F1}°\n" +
                        $"Emmanuel Valenzuela - Mat: 22540083";
        _canvas.Invalidate();
    }

    private float[,] Rotar4D(float[,] vertices)
    {
        var resultado = new float[vertices.GetLength(0), 4];
        Array.Copy(vertices, resultado, vertices.Length);

        double radXY = _rotXY * Math.PI / 180.0;
        double radXW = _rotXW * Math.PI / 180.0;
        double radYW = _rotYW * Math.PI / 180.0;

        for (int i = 0; i < vertices.GetLength(0); i++)
        {
            float x = resultado[i, 0];
            float y = resultado[i, 1];
            float z = resultado[i, 2];
            float w = resultado[i, 3];

            // Rotación en plano XY
            float cosXY = (float)Math.Cos(radXY);
            float sinXY = (float)Math.Sin(radXY);
            float x1 = x * cosXY - y * sinXY;
            float y1 = x * sinXY + y * cosXY;

            // Rotación en plano XW
            float cosXW = (float)Math.Cos(radXW);
            float sinXW = (float)Math.Sin(radXW);
            float x2 = x1 * cosXW - w * sinXW;
            float w1 = x1 * sinXW + w * cosXW;

            // Rotación en plano YW
            float cosYW = (float)Math.Cos(radYW);
            float sinYW = (float)Math.Sin(radYW);
            float y2 = y1 * cosYW - w1 * sinYW;
            float w2 = y1 * sinYW + w1 * cosYW;

            resultado[i, 0] = x2;
            resultado[i, 1] = y2;
            resultado[i, 2] = z;
            resultado[i, 3] = w2;
        }

        return resultado;
    }

    private PointF Proyectar3D(float x, float y, float z, float w, float distancia = 3f)
    {
        // Proyección perspectiva 4D → 3D
        float escala = distancia / (distancia + w);
        float x3d = x * escala;
        float y3d = y * escala;
        float z3d = z * escala;

        // Proyección isométrica 3D → 2D
        float px = x3d - z3d;
        float py = y3d - z3d * 0.5f;

        return new PointF(px, py);
    }

    private void OnPaint(object? s, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Color.Black);

        float cx = _canvas.Width / 2f;
        float cy = _canvas.Height / 2f;

        // Rotar vértices en 4D
        var rotados = Rotar4D(_vertices4D);

        // Proyectar a 2D
        var proyectados = new PointF[rotados.GetLength(0)];
        for (int i = 0; i < rotados.GetLength(0); i++)
        {
            var p2d = Proyectar3D(rotados[i, 0], rotados[i, 1], rotados[i, 2], rotados[i, 3]);
            proyectados[i] = new PointF(cx + p2d.X * 80, cy + p2d.Y * 80);
        }

        // Dibujar aristas
        using (var pen = new Pen(Color.Cyan, 1.5f))
        {
            foreach (var (v1, v2) in _edges)
            {
                g.DrawLine(pen, proyectados[v1], proyectados[v2]);
            }
        }

        // Dibujar vértices
        for (int i = 0; i < proyectados.Length; i++)
        {
            g.FillEllipse(Brushes.LimeGreen, proyectados[i].X - 3, proyectados[i].Y - 3, 6, 6);
        }

        // Información
        g.DrawString("HIPERCUBO 4D (TESSERACT)", new Font("Segoe UI", 12, FontStyle.Bold), Brushes.White, 10, 10);
        g.DrawString("Emmanuel Valenzuela - Matrícula: 22540083", new Font("Segoe UI", 10), Brushes.Yellow, 10, 30);
        g.DrawString("Rotación en planos: XY, XW, YW", new Font("Segoe UI", 9), Brushes.Cyan, 10, 50);
    }
}
