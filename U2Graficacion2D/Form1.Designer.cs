namespace U2Graficacion2D;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        // 1. DECLARACIÓN DE INSTANCIAS (AGREGAR ESTO)
        this.tabControl = new TabControl();
        this.tabPage1 = new TabPage();
        this.canvasPanel = new Panel(); // Nuestro lienzo para dibujar

        // --- Configuración del TabControl ---
        this.tabControl.Dock = DockStyle.Fill;
        this.tabControl.Font = new Font("Segoe UI", 9.5f);
        // 2. AGREGAR LAS PESTAÑAS AL TABCONTROL (AGREGAR ESTO)
        this.tabControl.Controls.Add(this.tabPage1);

        // --- Configuración de la Pestaña 1 (AGREGAR ESTO)
        this.tabPage1.Text = "Pruebas de Dibujo";
        this.tabPage1.Controls.Add(this.canvasPanel);

        // --- Configuración del Lienzo/Panel de Dibujo (AGREGAR ESTO)
        this.canvasPanel.Dock = DockStyle.Fill;
        this.canvasPanel.BackColor = Color.White; // Fondo blanco para contrastar los dibujos

        // --- Configuración del Formulario Principal ---
        this.Controls.Add(this.tabControl);
        this.ClientSize = new System.Drawing.Size(1000, 650);
        this.Text = "U2 - Graficación 2D  |  Ejemplos de clase";
        this.StartPosition = FormStartPosition.CenterScreen;
    }

    // 3. DECLARACIÓN DE VARIABLES DE CONTROL (AGREGAR ESTO AL FINAL)
    private TabControl tabControl = null!;
    private TabPage tabPage1 = null!;
    private Panel canvasPanel = null!;
}