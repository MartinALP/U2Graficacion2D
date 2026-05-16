using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

namespace U2Graficacion2D;

public class TabEscalamiento3D : UserControl
{
    private readonly Vector3[] _original =
    {
        new Vector3(0, 60, 0),
        new Vector3(-50, -40, -40),
        new Vector3(50, -40, -40),
        new Vector3(0, -40, 40)
    };

    private float _sx = 1f, _sy = 1f, _sz = 1f;
    private float _rotY = 25f, _rotX = 20f;

    private readonly TrackBar _tbSx, _tbSy, _tbSz, _tbRotY, _tbRotX;
    private readonly Label _lblInfo;
    private readonly Panel _canvas;

    public TabEscalamiento3D()
    {
        _canvas = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        _canvas.Paint += OnPaint;

        var panelControles = new Panel 
        { 
            Dock = DockStyle.Bottom, 
            Height = 170, 
            BackColor = Color.WhiteSmoke 
        };

        _tbSx = CrearTrack(5, 40, 10);
        _tbSy = CrearTrack(5, 40, 10);
        _tbSz = CrearTrack(5, 40, 10);
        _tbRotY = CrearTrack(0, 360, 25);
        _tbRotX = CrearTrack(0, 60, 20);

        _tbSx.ValueChanged += (_, _) => { _sx = _tbSx.Value / 10f; Actualizar(); };
        _tbSy.ValueChanged += (_, _) => { _sy = _tbSy.Value / 10f; Actualizar(); };
        _tbSz.ValueChanged += (_, _) => { _sz = _tbSz.Value / 10f; Actualizar(); };
        _tbRotY.ValueChanged += (_, _) => { _rotY = _tbRotY.Value; Actualizar(); };
        _tbRotX.ValueChanged += (_, _) => { _rotX = _tbRotX.Value; Actualizar(); };

        _lblInfo = new Label { AutoSize = true, Font = new Font("Consolas", 9.5f) };

        var ly = new FlowLayoutPanel 
        { 
            Dock = DockStyle.Fill, 
            Padding = new Padding(12),
            FlowDirection = FlowDirection.TopDown 
        };

        AgregarControl(ly, "sx:", _tbSx);
        AgregarControl(ly, "sy:", _tbSy);
        AgregarControl(ly, "sz:", _tbSz);
        AgregarControl(ly, "Rot Y:", _tbRotY);
        AgregarControl(ly, "Rot X:", _tbRotX);
        ly.Controls.Add(_lblInfo);

        panelControles.Controls.Add(ly);
        Controls.Add(_canvas);
        Controls.Add(panelControles);

        Actualizar();
    }

    private void AgregarControl(FlowLayoutPanel ly, string texto, Control control)
    {
        ly.Controls.Add(new Label { Text = texto, AutoSize = true });
        ly.Controls.Add(control);
    }

    private void Actualizar()
    {
        _lblInfo.Text = $"sx = {_sx:F1}  sy = {_sy:F1}  sz = {_sz:F1}\nRotX = {_rotX:F0}°  RotY = {_rotY:F0}°";
        _canvas.Invalidate();
    }

    private void OnPaint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Color.White);

        float cx = _canvas.Width / 2f;
        float cy = _canvas.Height / 2f - 30;

        // Ejes
        g.DrawLine(Pens.Gray, cx - 200, cy, cx + 200, cy);
        g.DrawLine(Pens.Gray, cx, cy - 150, cx, cy + 150);
        g.DrawString("X", new Font("Arial", 10), Brushes.Gray, cx + 205, cy);
        g.DrawString("Y", new Font("Arial", 10), Brushes.Gray, cx - 5, cy - 160);

        // Aplicar transformaciones
        var pts = _original.Select(v =>
        {
            var scaled = new Vector3(v.X * _sx, v.Y * _sy, v.Z * _sz);
            var rotated = RotarY(RotarX(scaled, _rotX), _rotY);
            return Proyectar(rotated, cx, cy);
        }).ToArray();

        using var pen = new Pen(Color.DarkBlue, 2.8f);
        using var brush1 = new SolidBrush(Color.FromArgb(110, 30, 100, 220));
        using var brush2 = new SolidBrush(Color.FromArgb(110, 0, 180, 100));

        DibujarCara(g, pts[0], pts[1], pts[2], brush1, pen);
        DibujarCara(g, pts[0], pts[2], pts[3], brush1, pen);
        DibujarCara(g, pts[0], pts[3], pts[1], brush1, pen);
        DibujarCara(g, pts[1], pts[2], pts[3], brush2, pen);

        g.DrawString("ESCALAMIENTO 3D", new Font("Segoe UI", 12, FontStyle.Bold), Brushes.Black, 15, 15);
    }

    private void DibujarCara(Graphics g, PointF a, PointF b, PointF c, Brush brush, Pen pen)
    {
        var p = new[] { a, b, c };
        g.FillPolygon(brush, p);
        g.DrawPolygon(pen, p);
    }

    private Vector3 RotarX(Vector3 v, float ang)
    {
        float rad = ang * MathF.PI / 180f;
        float cos = MathF.Cos(rad), sin = MathF.Sin(rad);
        return new Vector3(v.X, v.Y * cos - v.Z * sin, v.Y * sin + v.Z * cos);
    }

    private Vector3 RotarY(Vector3 v, float ang)
    {
        float rad = ang * MathF.PI / 180f;
        float cos = MathF.Cos(rad), sin = MathF.Sin(rad);
        return new Vector3(v.X * cos + v.Z * sin, v.Y, -v.X * sin + v.Z * cos);
    }

    private PointF Proyectar(Vector3 v, float cx, float cy)
    {
        float dist = 380f;
        float f = dist / (dist + v.Z);
        return new PointF(cx + v.X * f, cy - v.Y * f);
    }

    private static TrackBar CrearTrack(int min, int max, int val)
        => new() { Minimum = min, Maximum = max, Value = val, Width = 220, TickFrequency = 5 };
}