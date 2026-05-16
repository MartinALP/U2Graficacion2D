using System; 

using System.Drawing; 

using System.Drawing.Drawing2D; 

using System.Linq; 

using System.Windows.Forms; 

 

namespace U2Graficacion2D 

{ 

    public class PuntoWalas3D 

    { 

        public float X { get; set; } 

        public float Y { get; set; } 

        public float Z { get; set; } 

        public PuntoWalas3D(float x, float y, float z) { X = x; Y = y; Z = z; } 

    } 

 

    public class TabTraslacionWalas : UserControl 

    { 

        private PuntoWalas3D[] _v; 

        private int[][] _caras; 

        private float _tx = 0, _ty = 0, _tz = 0; 

        private readonly TrackBar _tbTx, _tbTy, _tbTz; 

        private readonly Label _lblInfo; 

        private readonly Panel _canvas; 

 

        public TabTraslacionWalas() 

        { 

            this.Dock = DockStyle.Fill; 

            _canvas = new Panel { Dock = DockStyle.Fill, BackColor = Color.White }; 

            _canvas.Paint += OnPaint; 

 

         

            typeof(Panel).InvokeMember("DoubleBuffered", 

                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, 

                null, _canvas, new object[] { true }); 

 

            var pnlControl = new Panel { Dock = DockStyle.Bottom, Height = 140, BackColor = Color.WhiteSmoke }; 

             

            _tbTx = CrearTrack(-300, 300, 0); 

            _tbTy = CrearTrack(-300, 300, 0); 

            _tbTz = CrearTrack(-300, 300, 0); 

 

            _tbTx.ValueChanged += (_, _) => { _tx = _tbTx.Value; Actualizar(); }; 

            _tbTy.ValueChanged += (_, _) => { _ty = _tbTy.Value; Actualizar(); }; 

            _tbTz.ValueChanged += (_, _) => { _tz = _tbTz.Value; Actualizar(); }; 

 

            _lblInfo = new Label { AutoSize = true, Font = new Font("Consolas", 9), ForeColor = Color.DarkRed }; 

 

            var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) }; 

            flow.Controls.Add(new Label { Text = "TX:", AutoSize = true }); flow.Controls.Add(_tbTx); 

            flow.Controls.Add(new Label { Text = "TY:", AutoSize = true }); flow.Controls.Add(_tbTy); 

            flow.Controls.Add(new Label { Text = "TZ:", AutoSize = true }); flow.Controls.Add(_tbTz); 

            flow.Controls.Add(_lblInfo); 

 

            pnlControl.Controls.Add(flow); 

            this.Controls.Add(_canvas); 

            this.Controls.Add(pnlControl); 

 

            ModelarGemaCorazon(); 

            Actualizar(); 

        } 

 

        private void ModelarGemaCorazon() 

        { 

             

            _v = new PuntoWalas3D[] { 

                new PuntoWalas3D(0, 0, 30),       

                new PuntoWalas3D(0, -80, -10),    

                 

                 

                new PuntoWalas3D(-30, 45, 25), new PuntoWalas3D(30, 45, 25),   // 2, 3 

                new PuntoWalas3D(-60, 20, 25), new PuntoWalas3D(60, 20, 25),   // 4, 5 

                new PuntoWalas3D(0, 15, 25),                                   // 6 

                 

             

                new PuntoWalas3D(-50, 65, 0),  new PuntoWalas3D(50, 65, 0),    // 7, 8 

                new PuntoWalas3D(-100, 30, 0), new PuntoWalas3D(100, 30, 0),   // 9, 10 

                new PuntoWalas3D(-80, -20, 0), new PuntoWalas3D(80, -20, 0),   // 11, 12 

                new PuntoWalas3D(0, 30, 0)                                      

            }; 

 

             

            _caras = new int[][] { 

                 

                new int[] { 0, 6, 2 }, new int[] { 0, 6, 3 }, 

                new int[] { 0, 2, 4 }, new int[] { 0, 3, 5 }, 

                new int[] { 2, 7, 4 }, new int[] { 3, 8, 5 }, 

                new int[] { 4, 9, 11 }, new int[] { 5, 10, 12 }, 

                 

            

                new int[] { 1, 11, 9 }, new int[] { 1, 12, 10 }, 

                new int[] { 1, 9, 7 }, new int[] { 1, 10, 8 }, 

                new int[] { 1, 7, 13 }, new int[] { 1, 8, 13 } 

            }; 

        } 

 

        private void Actualizar() 

        { 

            _lblInfo.Text = $"A TI DENTRO DE 2,000 AÑos:| DE TI, HACE 2,000 AÑOS({_tx}, {_ty}, {_tz})\n" + 

                            $"LA CAIDA DE SHINGANSHINA P1: ({_v[0].X + _tx:F0}, {_v[0].Y + _ty:F0}, {_v[0].Z + _tz:F0})"; 

            _canvas.Invalidate(); 

        } 

 

        private void OnPaint(object sender, PaintEventArgs e) 

        { 

            var g = e.Graphics; 

            g.SmoothingMode = SmoothingMode.AntiAlias; 

            float cx = _canvas.Width / 2f, cy = _canvas.Height / 2f; 

 

           

            g.DrawLine(Pens.Gainsboro, 0, cy, _canvas.Width, cy); 

            g.DrawLine(Pens.Gainsboro, cx, 0, cx, _canvas.Height); 

 

            

            foreach (var cara in _caras) 

            { 

                PointF[] pts = cara.Select(i => Proyectar(_v[i], cx, cy)).ToArray(); 

 

                using (var path = new GraphicsPath()) 

                { 

                    path.AddPolygon(pts); 

                    using (var br = new PathGradientBrush(path)) 

                    { 

                        

                        br.CenterColor = Color.FromArgb(255, 245, 230); // Punto de luz 

                        br.SurroundColors = new Color[] { Color.FromArgb(183, 110, 121) };  

                        g.FillPolygon(br, pts); 

                    } 

                } 

               

                g.DrawPolygon(new Pen(Color.FromArgb(140, 90, 80), 0.8f), pts); 

            } 

 

            

            float vx = _tx + (_tz * 0.5f), vy = _ty - (_tz * 0.5f); 

            using var pF = new Pen(Color.Red, 2) { EndCap = LineCap.ArrowAnchor }; 

            g.DrawLine(pF, cx, cy, cx + vx, cy - vy); 

        } 

 

        private PointF Proyectar(PuntoWalas3D p, float cx, float cy) 

        { 

            float xt = p.X + _tx, yt = p.Y + _ty, zt = p.Z + _tz; 

           

            return new PointF(cx + (xt + zt * 0.5f), cy - (yt - zt * 0.5f)); 

        } 

 

        private static TrackBar CrearTrack(int min, int max, int v) =>  

            new() { Minimum = min, Maximum = max, Value = v, Width = 140, TickFrequency = 100 }; 

    } 

} 