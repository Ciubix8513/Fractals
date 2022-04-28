using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Fractals
{
    public partial class FractalView : Form
    {
        private enum Fractal
        {
            Mandelbrot = 1,
            BurningShip = 2,
            Tricorn = 4,
            Feather = 8,
            Eye = 16
        }

        //openGL stuff
        //Vertex buffer object
        private int VBO; 
        //Vertex array object
        private int VAO;
        //Color buffer
        private int buf;


        private Shader shader = null!;

        //Uniforms
        public Vector4[] colors = new Vector4[] { new Vector4(85, 205, 252, 255), new Vector4(247, 168, 184, 255), new Vector4(255, 255, 255, 255), new Vector4(247, 168, 184, 255), new Vector4(85, 205, 252, 255) };

        private Timer timer = null!;
        private Fractal f = Fractal.Mandelbrot;

        private Fractal currentFractal
        { get => f; set { f = value; Restart(); } }

        public int maxIter = 1000;
        public int colorNum = 200;
        public int antiAliasing = 1;

        //Camera data
        private bool tracking = false;

        private Vector2 position;
        private Vector2 prevM;
        private float zoom = 1000.0f;

        private float zoom_dst;
        private Vector2 cam;
        private Vector2 cam_fp;
        private Vector2 cam_dst;
        private Vector2 prevDrag;

        //Other window data, make it nullable
        private FractalEdit? settings;

        //Shader path
#if DEBUG
        private const string MainShader = "../../../Res/shader.frag";
        private const string AboutFile = "../../../Res/About.txt";
#else
        private const string MainShader = "shader.frag";
        private const string AbotFile = "About.txt";
#endif

        //Load data into the color buffer
        public void UpdateColors()
        {
            //Bind the buffer for proccessing
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, buf);
            //Load new data in
            GL.BufferSubData(BufferTarget.ShaderStorageBuffer, IntPtr.Zero, colors.Length * sizeof(float) * 4, colors);
            //Unbind the buffer
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
        }

        //Converts screen coords to general coords
        private Vector2 Screen2p(Vector2 c)
        {
            return new Vector2(c.X - glControl.Width / 2, c.Y - glControl.Height / 2) / zoom - cam;
        }

        public FractalView()
        {
            this.FormClosed += Form1_FormClosed;
            position = new Vector2(0, 0);
            prevM = new Vector2(0, 0);

            InitializeComponent();
            //Add events to the glcontrol
            glControl.MouseWheel += GlControl_MouseWheel;
            glControl.MouseDown += GlControl_MouseDown;
            glControl.MouseUp += GlControl_MouseUp;
            glControl.MouseMove += GlControl_MouseMove;
        }

        private void GlControl_MouseWheel(object sender, MouseEventArgs e)
        {
            zoom_dst *= (float)Math.Pow(1.1f, e.Delta / 60);
            cam_fp = new Vector2(e.X, e.Y);
        }

        private void GlControl_MouseUp(object sender, MouseEventArgs e) => tracking = false;

        private void GlControl_MouseDown(object sender, MouseEventArgs e)
        { prevDrag = new Vector2(e.X, e.Y); tracking = true; }

        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (tracking)
            {
                Vector2 curDrag = new Vector2(e.X, e.Y);
                cam_dst += ((Vector2)curDrag - (Vector2)prevDrag) / zoom;
                prevDrag = curDrag;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Clean up
            shader.Dispose();
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(buf);
        }

        private void UpdateShader(string s)
        {
            if (shader != null)
                shader.Dispose();//Dispose first
            shader = new Shader(s);
            shader.Use();
        }

        private void glControl_Load(object sender, EventArgs e)
        {
            //Load shader
            UpdateShader(MainShader);

            VBO = GL.GenBuffer();
            VAO = GL.GenVertexArray();

            //Init VBO
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

            //Load initial data
            buf = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, buf);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, colors.Length * sizeof(float) * 4, colors, BufferUsageHint.StreamRead);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, buf);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

            glControl.Resize += glControl_Resize;
            glControl.Paint += glControl_Paint;

            Restart();
            // Use a timer to redraw the screen every 1/60 of a second.
            timer = new Timer();
            timer.Tick += (sender, e) => { Render(); };
            timer.Interval = 1000 / 60;   //1000 ms / fps
            timer.Start();

            GL.BindVertexArray(VAO);

            glControl_Resize(glControl, EventArgs.Empty);
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            //Change the opengl viewport size when resizing the window
            glControl.MakeCurrent();
            if (glControl.ClientSize.Height == 0)
                glControl.ClientSize = new System.Drawing.Size(glControl.ClientSize.Width, 1);
            GL.Viewport(0, 0, glControl.ClientSize.Width, glControl.ClientSize.Height);
        }

        private void glControl_Paint(object sender, PaintEventArgs e) => Render();

        private void Screenshot()
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
            //Get the data from the color buffer and save it to a bitmap
            using (Bitmap bitmap = new Bitmap(glControl.Width, glControl.Height))
            {
                BitmapData bits = bitmap.LockBits(new Rectangle(0, 0, glControl.Width, glControl.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
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
            fp = Screen2p(cam_fp);
            zoom = zoom * .8f + zoom_dst * .2f;
            cam_delta = Screen2p(cam_fp);
            cam_dst += cam_delta - fp;
            cam += cam_delta - fp;
            cam = cam * .8f + cam_dst * .2f;

            //Set uniforms
            GL.Uniform3(1, new Vector3(glControl.ClientSize.Width, glControl.ClientSize.Height, 1.0f));
            GL.Uniform4(3, new Vector4(cam.X, cam.Y, 1, 1));
            GL.Uniform1(4, (float)zoom);
            GL.Uniform1(5, colors.Length);

            GL.Uniform1(7, (int)currentFractal);
            GL.Uniform1(8, maxIter);
            GL.Uniform1(9, colorNum);
            GL.Uniform1(10, antiAliasing);
            //Draw
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);

            glControl.SwapBuffers();
        }

        //Reset camera
        private void Restart()
        {
            zoom = zoom_dst = 100.0f;
            cam = cam_dst = Vector2.Zero;
            position = Vector2.Zero;
            prevM = Vector2.Zero;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e) => Screenshot();

        //Fractal selection
        private void mandelbrotToolStripMenuItem_Click(object sender, EventArgs e) => currentFractal = Fractal.Mandelbrot;

        private void burningShipToolStripMenuItem_Click(object sender, EventArgs e) => currentFractal = Fractal.BurningShip;

        private void tricornToolStripMenuItem_Click(object sender, EventArgs e) => currentFractal = Fractal.Tricorn;

        private void featherToolStripMenuItem_Click(object sender, EventArgs e) => currentFractal = Fractal.Feather;

        private void eyeToolStripMenuItem_Click(object sender, EventArgs e) => currentFractal = Fractal.Eye;


        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Focus on the window if it's already opened
            if (settings != null)
            {
                settings.BringToFront(); return;
            }
            //If not open a new one
            settings = new FractalEdit();
            settings.fractalView = this;
            FormClosing += (s, args) => settings?.Close();
            settings.GetValues();
            settings.Show();
            settings.FormClosed += (s, e) => settings = null;
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e) => Restart();

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(AboutFile)) { MessageBox.Show("About file does not exist"); return; }
            //Open about file in notepad
            Process.Start("notepad.exe", AboutFile);
            /*var f = File.OpenText(AboutFile);
            MessageBox.Show(f.ReadToEnd());
            f.Close();*/
        }
    }
}