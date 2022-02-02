using System;

using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.WinForms;
using OpenTK.Core;


namespace Fractals
{
    public partial class Form1 : Form
    {
        private Timer _timer = null!;
        int VBO; //Vertex buffer object
        int VAO; //Vertex array object
        int frame;
        Shader shader;
        System.Diagnostics.Stopwatch stopwatch;
        bool paused = false;
        string currentShader;

        public Form1()
        {

            this.FormClosed += Form1_FormClosed;
            InitializeComponent();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            shader.Dispose();
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VBO);
        }

        void UpdateShader(string s)
        {
            currentShader = s;

            if (stopwatch.IsRunning)
                stopwatch.Restart();
            if (shader != null)
                shader.Dispose();//Dispose first
            shader = new Shader(s);
        }

        private void glControl_Load(object? sender, EventArgs e)
        {
            stopwatch = new System.Diagnostics.Stopwatch();
            frame = 0;
            UpdateShader("../../../Res/shader.frag");
            VBO = GL.GenBuffer();
            VAO = GL.GenVertexArray();
            //Init shader


            //Init vbo
            //Set up vertices
            float[] Vertices = {
                -1.0f, -1.0f, 0.0f,
                1.0f, -1.0f, 0.0f,
                1.0f,  1.0f, 0.0f,
            -1.0f,  1.0f, 0.0f};

            //Set up VBO and VAO
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            glControl.Resize += glControl_Resize;
            glControl.Paint += glControl_Paint;

            // Redraw the screen every 1/60 of a second.
            _timer = new Timer();
            _timer.Tick += (sender, e) =>
            {
                Render();
            };
            _timer.Interval = 15;   // 1000 ms per sec / 15 ms per frame = 60 FPS
            _timer.Start();
            stopwatch.Start();

            glControl_Resize(glControl, EventArgs.Empty);
        }

        private void glControl_Resize(object? sender, EventArgs e)
        {
            glControl.MakeCurrent();
            if (glControl.ClientSize.Height == 0)
                glControl.ClientSize = new System.Drawing.Size(glControl.ClientSize.Width, 1);
            GL.Viewport(0, 0, glControl.ClientSize.Width, glControl.ClientSize.Height);
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }
        private void Render()
        {
            glControl.MakeCurrent();
            GL.ClearColor(Color4.Black);
            //Clear the back buffer
            GL.Clear(ClearBufferMask.ColorBufferBit);

            //Set shader
            shader.Use();

            //Set uniforms
            GL.Uniform3(1, new Vector3(glControl.ClientSize.Width, glControl.ClientSize.Height, 1.0f));
            float f = (float)stopwatch.Elapsed.TotalSeconds;
            GL.Uniform1(0, f);
            //f = 
            GL.Uniform1(2, frame);

            //Draw
            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);

            glControl.SwapBuffers();
            if (!paused)
                frame++;
        }
        void Restart()
        {
            frame = 0;
            if (paused)
                stopwatch.Reset();
            else
                stopwatch.Restart();
        }
        private void calibrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateShader("../../../Res/Calibration.frag");
            Restart();
        }
        private void mainShaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateShader("../../../Res/shader.frag");
            Restart();
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e) => UpdateShader(currentShader);
        private void toolStripMenuItem2_Click(object sender, EventArgs e) => Restart();
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (paused)
                stopwatch.Start();
            else
                stopwatch.Stop();
            paused = !paused;
        }
    }
}
