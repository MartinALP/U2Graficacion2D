using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace U2Graficacion3D
{
    public class TabTraslacion3dMendez : UserControl
    {
        // =========================
        // CUBO 3D (8 VÉRTICES)
        // =========================
        private readonly Point3D[] _original =
        {
            new(-80, -80, -80), // 0
            new( 80, -80, -80), // 1
            new( 80,  80, -80), // 2
            new(-80,  80, -80), // 3

            new(-80, -80,  80), // 4
            new( 80, -80,  80), // 5
            new( 80,  80,  80), // 6
            new(-80,  80,  80)  // 7
        };

        private readonly int[,] _edges =
        {
            {0,1},{1,2},{2,3},{3,0},
            {4,5},{5,6},{6,7},{7,4},
            {0,4},{1,5},{2,6},{3,7}
        };

        // =========================
        // TRASLACIÓN
        // =========================
        private float _tx = 0;
        private float _ty = 0;
        private float _tz = 0;

        private readonly TrackBar _tbTx;
        private readonly TrackBar _tbTy;
        private readonly TrackBar _tbTz;

        public TabTraslacion3dMendez()
        {
            DoubleBuffered = true;

            // Panel inferior (controles)
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 80
            };

            _tbTx = CrearTrack(-200, 200, 0);
            _tbTy = CrearTrack(-200, 200, 0);
            _tbTz = CrearTrack(-200, 200, 0);

            _tbTx.ValueChanged += (s, e) => { _tx = _tbTx.Value; Invalidate(); };
            _tbTy.ValueChanged += (s, e) => { _ty = _tbTy.Value; Invalidate(); };
            _tbTz.ValueChanged += (s, e) => { _tz = _tbTz.Value; Invalidate(); };

            panel.Controls.Add(new Label { Text = "TX" });
            panel.Controls.Add(_tbTx);
            panel.Controls.Add(new Label { Text = "TY" });
            panel.Controls.Add(_tbTy);
            panel.Controls.Add(new Label { Text = "TZ" });
            panel.Controls.Add(_tbTz);

            Controls.Add(panel);
        }

        // =========================
        // TRACKBAR
        // =========================
        private TrackBar CrearTrack(int min, int max, int val)
        {
            return new TrackBar
            {
                Minimum = min,
                Maximum = max,
                Value = val,
                Width = 120
            };
        }

        // =========================
        // PROYECCIÓN 3D → 2D
        // =========================
        private Point Proyectar(float x, float y, float z)
        {
            float d = 300;

            float xp = (x * d) / (z + d);
            float yp = (y * d) / (z + d);

            return new Point((int)xp, (int)yp);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int cx = Width / 2;
            int cy = Height / 2 - 40;

            Point[] p2d = new Point[_original.Length];

            // =========================
            // APLICAR TRASLACIÓN
            // =========================
            for (int i = 0; i < _original.Length; i++)
            {
                float x = _original[i].X + _tx;
                float y = _original[i].Y + _ty;
                float z = _original[i].Z + _tz;

                Point p = Proyectar(x, y, z);

                p2d[i] = new Point(p.X + cx, p.Y + cy);
            }

            // =========================
            // DIBUJAR CUBO
            // =========================
            using var pen = new Pen(Color.Blue, 2);

            for (int i = 0; i < _edges.GetLength(0); i++)
            {
                Point p1 = p2d[_edges[i, 0]];
                Point p2 = p2d[_edges[i, 1]];

                g.DrawLine(pen, p1, p2);
            }

            // =========================
            // VECTOR DE TRASLACIÓN
            // =========================
            using var penVec = new Pen(Color.Red, 2)
            {
                EndCap = LineCap.ArrowAnchor
            };

            g.DrawLine(penVec, cx, cy, cx + _tx, cy - _ty);

            // =========================
            // TEXTO
            // =========================
            g.DrawString($"Traslación: tx={_tx}, ty={_ty}, tz={_tz}",
                new Font("Consolas", 9),
                Brushes.Black,
                10, 10);
        }
    }

    // =========================
    // ESTRUCTURA 3D
    // =========================
    public struct Point3D
    {
        public float X, Y, Z;

        public Point3D(float x, float y, float z)
        {
            X = x; Y = y; Z = z;
        }
    }
}