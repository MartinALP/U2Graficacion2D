// ============================================================
//  2.5  USO Y CREACIÓN DE FUENTES DE TEXTO
//
//  En GDI+ se usan las clases:
//  • Font       — define tipografía, tamaño y estilo
//  • StringFormat — alineación, dirección, recorte
//  • FontFamily  — familia de fuentes instaladas
//  • GraphicsPath.AddString — convierte texto en trazos
//                             (permite transformarlo como figura)
//
//  Este ejemplo muestra:
//  1. Texto con diferentes familias, tamaños y estilos
//  2. Texto convertido a trazos (path) y con transformaciones
//  3. Texto siguiendo una trayectoria curva (Bézier)
// ============================================================
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace U2Graficacion2D;

public class TabFuentes : UserControl
{
    private readonly ComboBox _cbFamilia;
    private readonly TrackBar _tbTamaño;
    private readonly CheckBox _chkBold, _chkItalic, _chkOutline, _chkCurva;
    private readonly TextBox _txtTexto;
    private readonly Panel _canvas;

    public TabFuentes()
    {
        _canvas = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        _canvas.Paint += OnPaint;

        var panel = new Panel { Dock = DockStyle.Bottom, Height = 100, BackColor = Color.WhiteSmoke };

        // Cargar familias instaladas
        _cbFamilia = new ComboBox { Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
        using (var col = new InstalledFontCollection())
            foreach (var ff in col.Families.Take(60))
                _cbFamilia.Items.Add(ff.Name);
        _cbFamilia.SelectedIndex = 0;
        _cbFamilia.SelectedIndexChanged += (_, _) => _canvas.Invalidate();

        _tbTamaño = new TrackBar { Minimum = 8, Maximum = 80, Value = 36, Width = 150, TickFrequency = 8 };
        _tbTamaño.ValueChanged += (_, _) => _canvas.Invalidate();

        _txtTexto = new TextBox { Text = "Graficación 2D", Width = 200 };
        _txtTexto.TextChanged += (_, _) => _canvas.Invalidate();

        _chkBold    = new CheckBox { Text = "Bold",    AutoSize = true };
        _chkItalic  = new CheckBox { Text = "Italic",  AutoSize = true };
        _chkOutline = new CheckBox { Text = "Contorno (Path)", AutoSize = true };
        _chkCurva   = new CheckBox { Text = "Texto en curva", AutoSize = true };

        foreach (Control c in new Control[] { _chkBold, _chkItalic, _chkOutline, _chkCurva })
            ((CheckBox)c).CheckedChanged += (_, _) => _canvas.Invalidate();

        var ly = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(6) };
        ly.Controls.Add(new Label { Text = "Texto:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft });
        ly.Controls.Add(_txtTexto);
        ly.Controls.Add(new Label { Text = "Familia:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft });
        ly.Controls.Add(_cbFamilia);
        ly.Controls.Add(new Label { Text = "Tamaño:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft });
        ly.Controls.Add(_tbTamaño);
        ly.Controls.Add(_chkBold); ly.Controls.Add(_chkItalic);
        ly.Controls.Add(_chkOutline); ly.Controls.Add(_chkCurva);
        panel.Controls.Add(ly);

        Controls.Add(_canvas);
        Controls.Add(panel);
    }

    private void OnPaint(object? s, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.AntiAlias;

        string texto = string.IsNullOrWhiteSpace(_txtTexto.Text) ? "Texto" : _txtTexto.Text;
        string familia = _cbFamilia.SelectedItem?.ToString() ?? "Arial";
        float tamaño = _tbTamaño.Value;

        var estilo = FontStyle.Regular;
        if (_chkBold.Checked) estilo |= FontStyle.Bold;
        if (_chkItalic.Checked) estilo |= FontStyle.Italic;

        // ── Tabla de estilos en la mitad superior ────────────
        int y = 10;
        foreach (var fs in new[] { FontStyle.Regular, FontStyle.Bold, FontStyle.Italic, FontStyle.Bold | FontStyle.Italic })
        {
            try
            {
                using var f = new Font(familia, 14, fs, GraphicsUnit.Pixel);
                g.DrawString($"{fs}: {texto}", f, Brushes.Black, 10, y);
                y += 22;
            }
            catch { /* familia no soporta el estilo */ }
        }

        // ── Texto principal ──────────────────────────────────
        float cy = _canvas.Height / 2f;

        if (_chkCurva.Checked)
        {
            // Texto siguiendo arco
            DibujarTextoEnCurva(g, texto, familia, tamaño, estilo, cy);
        }
        else if (_chkOutline.Checked)
        {
            // Texto como GraphicsPath (contorno + relleno)
            using var path = new GraphicsPath();
            using var ff = new FontFamily(familia);
            path.AddString(texto, ff, (int)estilo, tamaño, new PointF(20, cy), StringFormat.GenericTypographic);
            using var br = new LinearGradientBrush(
                new Rectangle(0, (int)cy, _canvas.Width, (int)tamaño + 10),
                Color.DodgerBlue, Color.OrangeRed, LinearGradientMode.Horizontal);
            g.FillPath(br, path);
            g.DrawPath(new Pen(Color.Black, 1), path);
        }
        else
        {
            using var font = new Font(familia, tamaño, estilo, GraphicsUnit.Pixel);
            using var br = new LinearGradientBrush(
                new Rectangle(0, (int)cy, _canvas.Width, (int)tamaño + 10),
                Color.DodgerBlue, Color.OrangeRed, LinearGradientMode.Horizontal);
            g.DrawString(texto, font, br, 20, cy);
        }

        // ── Leyenda de StringFormat ──────────────────────────
        int fy = _canvas.Height - 90;
        using var fSmall = new Font("Segoe UI", 8);
        var sf = new StringFormat { Alignment = StringAlignment.Center };
        g.DrawString("← Centrado →", fSmall, Brushes.Gray, new RectangleF(0, fy, _canvas.Width, 20), sf);
        sf.Alignment = StringAlignment.Far;
        g.DrawString("Derecha →", fSmall, Brushes.Gray, new RectangleF(0, fy + 20, _canvas.Width, 20), sf);
        sf.Alignment = StringAlignment.Near;
        g.DrawString("← Izquierda", fSmall, Brushes.Gray, new RectangleF(0, fy + 40, _canvas.Width, 20), sf);
    }

    private static void DibujarTextoEnCurva(Graphics g, string texto, string familia,
        float tamaño, FontStyle estilo, float cy)
    {
        // Evaluamos posición y ángulo de cada carácter sobre un arco
        float radio = 180f;
        float cx = 400, arcY = cy + radio;
        float anguloInicio = -160f;
        float paso = 12f;

        try
        {
            using var font = new Font(familia, tamaño * 0.6f, estilo, GraphicsUnit.Pixel);
            for (int i = 0; i < texto.Length; i++)
            {
                float ang = anguloInicio + i * paso;
                float rad = ang * MathF.PI / 180f;
                float x = cx + radio * MathF.Cos(rad);
                float y = arcY + radio * MathF.Sin(rad);

                var state = g.Save();
                g.TranslateTransform(x, y);
                g.RotateTransform(ang + 90);
                g.DrawString(texto[i].ToString(), font, Brushes.DarkBlue, 0, 0);
                g.Restore(state);
            }
        }
        catch { }
    }
}
