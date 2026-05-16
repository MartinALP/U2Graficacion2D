using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Linq;
using System.Collections.Generic;

namespace U2Graficacion2D.Tabs;

public partial class TabSesgado26 : UserControl
{
    public class Point3D
    {
        public double X, Y, Z;
        public Point3D(double x, double y, double z) { X=x; Y=y; Z=z; }
    }

    // Puntos del contorno de un rayo (lightning bolt)
    private readonly Point3D[] _lightningVertices = {
        // Frente (z = -10)
        new(10, -60, -10),  // 0: Punta superior
        new(-15, 5, -10),   // 1: Esquina interior izquierda
        new(15, 5, -10),    // 2: Esquina exterior derecha
        new(-15, 60, -10),  // 3: Punta inferior
        new(5, -5, -10),    // 4: Esquina interior derecha
        new(-25, -5, -10),  // 5: Esquina exterior izquierda
        
        // Atrás (z = 10)
        new(10, -60, 10),   // 6
        new(-15, 5, 10),    // 7
        new(15, 5, 10),     // 8
        new(-15, 60, 10),   // 9
        new(5, -5, 10),     // 10
        new(-25, -5, 10)    // 11
    };

    // Definición de las caras usando índices a los vértices
    private readonly int[][] _faces = {
        new[] {0, 1, 2, 3, 4, 5},       // Frontal
        new[] {11, 10, 9, 8, 7, 6},     // Trasera
        new[] {5, 0, 6, 11},            // Lado Superior Izquierdo
        new[] {0, 1, 7, 6},             // Lado Interior Superior
        new[] {1, 2, 8, 7},             // Lado Horizontal Derecho
        new[] {2, 3, 9, 8},             // Lado Interior Inferior
        new[] {3, 4, 10, 9},            // Lado Inferior Izquierdo
        new[] {4, 5, 11, 10}            // Lado Horizontal Izquierdo
    };

    // Colores para cada cara para darle aspecto 3D y volumen
    private readonly IBrush[] _faceBrushes = {
        new SolidColorBrush(Color.Parse("#FFE600")), // Frontal - Amarillo
        new SolidColorBrush(Color.Parse("#B3A100")), // Trasera - Amarillo oscuro
        new SolidColorBrush(Color.Parse("#FFD700")), // Lados...
        new SolidColorBrush(Color.Parse("#E6C200")), 
        new SolidColorBrush(Color.Parse("#CCA800")), 
        new SolidColorBrush(Color.Parse("#B39300")), 
        new SolidColorBrush(Color.Parse("#997E00")), 
        new SolidColorBrush(Color.Parse("#E6C200"))  
    };

    public TabSesgado26()
    {
        InitializeComponent();
        ActualizarInfo();
    }

    private void ActualizarInfo()
    {
        if (LblInfo == null || SliderShx == null || SliderShy == null || SliderShzX == null || SliderShzY == null) return;
        LblInfo.Text = $"Sesgado 3D: shx(XY)={SliderShx.Value:F2}  shy(YX)={SliderShy.Value:F2}  shzx(XZ)={SliderShzX.Value:F2}  shzy(YZ)={SliderShzY.Value:F2}";
    }

    private void Slider_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Slider.ValueProperty)
        {
            ActualizarInfo();
            MainCanvas?.InvalidateVisual();
        }
    }

    private void OnPaint(object? sender, DrawingContext g)
    {
        if (MainCanvas == null || SliderShx == null) return;

        double cx = MainCanvas.Bounds.Width / 2;
        double cy = MainCanvas.Bounds.Height / 2;

        GraficoUtil.DibujarEjes(g, cx, cy, MainCanvas.Bounds.Width, MainCanvas.Bounds.Height);

        double shx = SliderShx.Value;
        double shy = SliderShy.Value;
        double shzx = SliderShzX.Value;
        double shzy = SliderShzY.Value;

        // Intercambiamos los sliders para corregir la percepción visual:
        // "Rot X" moverá la figura de izquierda a derecha (rotación sobre Y)
        // "Rot Y" moverá la figura de arriba a abajo (rotación sobre X)
        double angleX = SliderRotY?.Value * Math.PI / 180.0 ?? 0;
        double angleY = SliderRotX?.Value * Math.PI / 180.0 ?? 0;

        double cosX = Math.Cos(angleX), sinX = Math.Sin(angleX);
        double cosY = Math.Cos(angleY), sinY = Math.Sin(angleY);

        var transformedVertices = new Point3D[_lightningVertices.Length];

        
        for (int i = 0; i < _lightningVertices.Length; i++)
        {
            var p = _lightningVertices[i];

            // 1. Aplicar matriz de Sesgado 3D (Skew 3D)
            double sx = p.X + (p.Y * shx) + (p.Z * shzx);
            double sy = p.Y + (p.X * shy) + (p.Z * shzy);
            double sz = p.Z;

            // 2. Rotación X
            double ry = sy * cosX - sz * sinX;
            double rz = sy * sinX + sz * cosX;

            // 3. Rotación Y
            double rx = sx * cosY + rz * sinY;
            double finalZ = -sx * sinY + rz * cosY;

            // Escala
            double scale = 1.8;
            transformedVertices[i] = new Point3D(rx * scale, ry * scale, finalZ * scale);
        }

        // Algoritmo de Pintor: Ordenar caras de la más lejana a la más cercana
        var faceZ = new List<(int faceIndex, double zDepth)>();
        for (int i = 0; i < _faces.Length; i++)
        {
            double sumZ = 0;
            foreach (var vIdx in _faces[i])
            {
                sumZ += transformedVertices[vIdx].Z;
            }
            faceZ.Add((i, sumZ / _faces[i].Length));
        }

        var sortedFaces = faceZ.OrderByDescending(f => f.zDepth).ToList();

        // Dibujar
        var pen = new Pen(Brushes.Black, 1.5);
        foreach (var face in sortedFaces)
        {
            int idx = face.faceIndex;
            var points = _faces[idx].Select(v => new Point(cx + transformedVertices[v].X, cy + transformedVertices[v].Y)).ToArray();
            
            var geometry = new PolylineGeometry(points, true);
            g.DrawGeometry(_faceBrushes[idx], pen, geometry);
        }

        var typeface = new Typeface("Inter, Arial, Segoe UI");
        GraficoUtil.DrawText(g, "Rayo 3D Sesgado Manual", new Point(10, 10), typeface, 14, Brushes.Yellow);
    }
}
