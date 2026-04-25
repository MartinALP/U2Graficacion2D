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
        this.tabControl = new TabControl();
        this.tabControl.Dock = DockStyle.Fill;
        this.tabControl.Font = new Font("Segoe UI", 9.5f);
        this.Controls.Add(this.tabControl);
        this.ClientSize = new System.Drawing.Size(1000, 650);
        this.Text = "U2 - Graficación 2D  |  Ejemplos de clase";
        this.StartPosition = FormStartPosition.CenterScreen;
    }

    private TabControl tabControl = null!;
}
