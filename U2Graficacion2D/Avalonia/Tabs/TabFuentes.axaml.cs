using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;

namespace U2Graficacion2D.Tabs;

public partial class TabFuentes : UserControl
{
    public TabFuentes()
    {
        InitializeComponent();
    }

    private void TextBox_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        MainCanvas?.InvalidateVisual();
    }

    private void Slider_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Slider.ValueProperty)
        {
            MainCanvas?.InvalidateVisual();
        }
    }

    private void OnPaint(object? sender, DrawingContext g)
    {
        if (MainCanvas == null || TxtTexto == null || SliderTamano == null) return;

        double cx = MainCanvas.Bounds.Width / 2;
        double cy = MainCanvas.Bounds.Height / 2;
        double tamano = SliderTamano.Value;

        GraficoUtil.DibujarEjes(g, cx, cy, MainCanvas.Bounds.Width, MainCanvas.Bounds.Height);

        // Obtener el texto del input
        string texto = TxtTexto.Text ?? "¡Graficación!";
        
        // Dibujar el texto principal
        var typeface = new Typeface("Arial, Segoe UI");
        GraficoUtil.DrawText(g, texto, new Point(cx - 100, cy - 50), typeface, tamano, Brushes.Cyan);

        // Dibujar variaciones
        var typefaceBold = new Typeface("Arial, Segoe UI", FontStyle.Normal, FontWeight.Bold);
        GraficoUtil.DrawText(g, texto, new Point(cx - 100, cy + 50), typefaceBold, tamano * 0.7, Brushes.Yellow);

        var typefaceItalic = new Typeface("Arial, Segoe UI", FontStyle.Italic, FontWeight.Normal);
        GraficoUtil.DrawText(g, texto, new Point(cx - 100, cy + 100), typefaceItalic, tamano * 0.6, Brushes.LimeGreen);

        // Información
        var typeface_info = new Typeface("Arial, Segoe UI");
        GraficoUtil.DrawText(g, $"Tamaño: {tamano:F0}px", new Point(10, 10), typeface_info, 12, Brushes.Gray);
        GraficoUtil.DrawText(g, "Normal | Bold | Italic", new Point(10, 30), typeface_info, 10, Brushes.Gray);
    }
}
