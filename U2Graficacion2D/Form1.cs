using U2Graficacion3D;

namespace U2Graficacion2D;

public partial class Form1 : Form
{
    public Form1()
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
<<<<<<< HEAD
        AgregarTab("2.5.8 Final",         new TabFuentes());
        AgregarTab("4D Rotación - Emmanuel Valenzuela (22540083)", new Tab4D());

=======
        AgregarTab("2.1.1.1 TabTraslacionWalas", new TabTraslacionWalas());
        AgregarTab("2.1.2.1 TabEscalamiento3d", new TabEscalamiento3D());
        AgregarTab("2.6 Traslación 3D", new TabTraslacion3dMendez());
>>>>>>> 104b355ccbc077a30fbeec600dbf5ce912794ce3
    }

    private void AgregarTab(string titulo, Control contenido)
    {
        var page = new TabPage(titulo);
        contenido.Dock = DockStyle.Fill;
        page.Controls.Add(contenido);
        tabControl.TabPages.Add(page);
    }
}
