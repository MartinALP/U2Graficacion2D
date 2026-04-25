using Avalonia.Controls;
using U2Graficacion2D.Tabs;

namespace U2Graficacion2D;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        AgregarTab("2.1.1 Traslación",    new TabTraslacion());
        AgregarTab("2.1.2 Escalamiento",  new TabEscalamiento());
        AgregarTab("2.1.3 Rotación",      new TabRotacion());
        AgregarTab("2.1.4 Sesgado",       new TabSesgado());
        AgregarTab("2.2 Matrices",        new TabMatrices());
        AgregarTab("2.3.1 Bézier",        new TabBezier());
        AgregarTab("2.3.2 B-Spline",      new TabBSpline());
        AgregarTab("2.4 Fractales",       new TabFractales());
        AgregarTab("2.5 Fuentes",         new TabFuentes());
    }

    private void AgregarTab(string titulo, Control contenido)
    {
        var item = new TabItem
        {
            Header = titulo,
            Content = contenido
        };
        MainTabControl.Items.Add(item);
    }
}
