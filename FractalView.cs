using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Fractals
{    
    public partial class FractalView : Form
    {        
        enum Fractal
        {
            Mandelbrot = 1,
            BurningShip = 2,
            Tricorn = 4,
            Feather = 8,
            Eye = 16
        }
        //openGL stuff
        int VBO; //Vertex buffer object
        int VAO; //Vertex array object
        int buf;
        Shader shader;        
        //Uniforms
        public Vector4[] colors = new Vector4[] { new Vector4(85, 205, 252, 255), new Vector4(247, 168, 184, 255), new Vector4(255, 255, 255, 255), new Vector4(247, 168, 184, 255), new Vector4(85, 205, 252, 255) };
        private Timer _timer = null!;
        Fractal f = Fractal.Mandelbrot;
        Fractal currentFractal { get { return f; } set { f = value; Restart(); } }
        int mI = 1000; 
        public int MaxIter { get { return mI; }set { mI = value; sameFrame = 0; } }
        int cNum = 200;
        public int colorNum { get { return cNum; } set { cNum = value;sameFrame = 0; } }
        //Camera
        bool tracking = false;
        Vector2 position;
        Vector2 prevM;
        float zoom = 1000.0f;
        int sameFrame = 0;
        float zoom_dst;
        Vector2 cam;
        Vector2i cam_fp;
        Vector2 cam_dst;
        Vector2i prevDrag;
#if DEBUG
        const string MainShader = "../../../Res/shader.frag";
        const string Calibration = "../../../Res/Calibration.frag";
#else
        const string MainShader = "shader.frag";
        const string Calibration = "Calibration.frag";
#endif
        public void UpdateColors()
        {           
            sameFrame = 0;
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, buf);
            GL.BufferSubData(BufferTarget.ShaderStorageBuffer, IntPtr.Zero, colors.Length * sizeof(float) * 4, colors);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
        }
        Vector2 screen2p(Vector2i c)
        {
            return new Vector2(c.X - glControl.Width / 2, c.Y - glControl.Height / 2) / zoom - cam;
        }
        public FractalView()
        {
            this.FormClosed += Form1_FormClosed;
            position = new Vector2(0, 0);
            prevM = new Vector2(0, 0);

            InitializeComponent();

            glControl.MouseWheel += GlControl_MouseWheel;
            glControl.MouseDown += GlControl_MouseDown;
            glControl.MouseUp += GlControl_MouseUp;
            glControl.MouseMove += GlControl_MouseMove;

        }
        private void GlControl_MouseWheel(object sender, MouseEventArgs e)
        {
            sameFrame = 0;
            zoom_dst *= MathF.Pow(1.1f, e.Delta / 60);
            cam_fp = new Vector2i(e.X, e.Y);
        }
        private void GlControl_MouseUp(object sender, MouseEventArgs e) => tracking = false;
        private void GlControl_MouseDown(object sender, MouseEventArgs e) { prevDrag = new Vector2i(e.X, e.Y); tracking = true; }
        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (tracking)
            {
                sameFrame = 0;
                Vector2i curDrag = new Vector2i(e.X, e.Y);
                cam_dst += ((Vector2)curDrag - (Vector2)prevDrag) / zoom;
                prevDrag = curDrag;
            }
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            shader.Dispose();
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(buf);
        }
        void UpdateShader(string s)
        {            
            sameFrame = 0;            
            if (shader != null)
                shader.Dispose();//Dispose first
            shader = new Shader(s);
            shader.Use();
        }
        private void glControl_Load(object? sender, EventArgs e)
        {            
            UpdateShader(MainShader);
            VBO = GL.GenBuffer();
            VAO = GL.GenVertexArray();

            //Init vbo
            //Set up vertices
            float[] Vertices = {
                -1.0f,-1.0f, 0.0f,
                 1.0f,-1.0f, 0.0f,
                 1.0f, 1.0f, 0.0f,
                -1.0f, 1.0f, 0.0f};
            //Set up VBO and VAO
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            buf = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, buf);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, colors.Length * sizeof(float) * 4, colors, BufferUsageHint.StreamRead);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, buf);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);


            glControl.Resize += glControl_Resize;
            glControl.Paint += glControl_Paint;

            Restart();
            // Redraw the screen every 1/60 of a second.
            _timer = new Timer();
            _timer.Tick += (sender, e) =>{Render();};
            _timer.Interval =1000/ 60;   //1000 ms / fps
            _timer.Start();       

            GL.BindVertexArray(VAO);

            glControl_Resize(glControl, EventArgs.Empty);
        }
        private void glControl_Resize(object? sender, EventArgs e)
        {
            glControl.MakeCurrent();
            if (glControl.ClientSize.Height == 0)
                glControl.ClientSize = new System.Drawing.Size(glControl.ClientSize.Width, 1);
            GL.Viewport(0, 0, glControl.ClientSize.Width, glControl.ClientSize.Height);
            sameFrame = 0;
        }
        private void glControl_Paint(object sender, PaintEventArgs e) => Render();
        void Screenshot()
        {
            string path = "screenshots";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            switch (currentFractal)
            {
                case Fractal.Mandelbrot:
                    path += "/Mandelbrot_";
                    break;
                case Fractal.Tricorn:
                    path += "/Tricorn_";
                    break;
                case Fractal.BurningShip:
                    path += "/BurningShip_";
                    break;
                case Fractal.Feather:
                    path += "/Feather_";
                    break;
                case Fractal.Eye:
                    path += "/Eye_";
                    break;
            }
            //screenshots/FRACTALNAME_TIME.png
            path += DateTime.Now.ToFileTime() + ".png";

            using (Bitmap bitmap = new Bitmap(glControl.Width, glControl.Height))
            {
                BitmapData bits = bitmap.LockBits(new Rectangle(0, 0, glControl.Width, glControl.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.ReadPixels(0, 0, glControl.Width, glControl.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bits.Scan0);
                bitmap.UnlockBits(bits);
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
        private void Render()
        {            
            //Camera stuff
            Vector2 fp, cam_delta;
            fp = screen2p(cam_fp);
            zoom = zoom * .8f + zoom_dst * .2f;
            cam_delta = screen2p(cam_fp);
            cam_dst += cam_delta - fp;
            cam += cam_delta - fp;
            cam = cam * .8f + cam_dst * .2f;

            //Set uniforms
            GL.Uniform3(1, new Vector3(glControl.ClientSize.Width, glControl.ClientSize.Height, 1.0f));
            
            GL.Uniform4(3, new Vector4(cam.X, cam.Y, 1, 1));
            GL.Uniform1(4, (float)zoom);
            GL.Uniform1(5, colors.Length);
            GL.Uniform1(6, sameFrame);
            GL.Uniform1(7, (int)currentFractal);
            GL.Uniform1(8, MaxIter);
            GL.Uniform1(9, colorNum);
            //Draw         
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);

            glControl.SwapBuffers();        
        }
        void Restart()
        {
            zoom = zoom_dst = 100.0f;
            cam = cam_dst = Vector2.Zero;
            position = Vector2.Zero;
            prevM = Vector2.Zero; ;
            sameFrame = 0;
         
        }      
        private void toolStripMenuItem4_Click(object sender, EventArgs e) => Screenshot();
        //Fractal changes
        private void mandelbrotToolStripMenuItem_Click(object sender, EventArgs e) => currentFractal = Fractal.Mandelbrot;
        private void burningShipToolStripMenuItem_Click(object sender, EventArgs e) => currentFractal = Fractal.BurningShip;
        private void tricornToolStripMenuItem_Click(object sender, EventArgs e) => currentFractal = Fractal.Tricorn;
        private void featherToolStripMenuItem_Click(object sender, EventArgs e) => currentFractal = Fractal.Feather;
        private void eyeToolStripMenuItem_Click(object sender, EventArgs e) => currentFractal = Fractal.Eye;
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FractalEdit edit = new FractalEdit();
            edit.fractalView = this;
            FormClosing += (s,args) => edit.Close();
            edit.GetValues();
            edit.Show();
        }
        private void resetToolStripMenuItem_Click(object sender, EventArgs e) => Restart();
        
    }
}
