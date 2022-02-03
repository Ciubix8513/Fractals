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
        bool tracking = false;
        Vector2 position;
        Vector2 prevM;
        float sensetiity = 2.0f;
        float zoom = 1000.0f;



        public Form1()
        {
            this.FormClosed += Form1_FormClosed;
            position = new Vector2(0, 0);
            prevM = new Vector2(0,0);
            
            InitializeComponent();

            glControl.MouseWheel += GlControl_MouseWheel;
            glControl.MouseDown += GlControl_MouseDown;
            glControl.MouseUp += GlControl_MouseUp;
            glControl.MouseMove += GlControl_MouseMove;
        }

        private void GlControl_MouseWheel(object sender, MouseEventArgs e)
        {
            zoom += e.Delta;
        }

        private void GlControl_MouseUp(object sender, MouseEventArgs e)=>tracking = false;
        private void GlControl_MouseDown(object sender, MouseEventArgs e) { tracking = true; }
        

        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
                Vector2 p = new Vector2(e.X, e.Y);
            if (tracking) 
            {
                var delta = prevM - p;
                position += delta * sensetiity;
            }
                prevM = p;
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
            shader.Use();
        }

        private void glControl_Load(object? sender, EventArgs e)
        {
            stopwatch = new System.Diagnostics.Stopwatch();
            frame = 0;
            UpdateShader("../../../Res/shader.frag");
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
            
            GL.BindVertexArray(VAO);

            glControl_Resize(glControl, EventArgs.Empty);
        }

        private void glControl_Resize(object? sender, EventArgs e)
        {
            glControl.MakeCurrent();
            if (glControl.ClientSize.Height == 0)
                glControl.ClientSize = new System.Drawing.Size(glControl.ClientSize.Width, 1);
            GL.Viewport(0, 0, glControl.ClientSize.Width, glControl.ClientSize.Height);
        }

        private void glControl_Paint(object sender, PaintEventArgs e)=>Render();
        
        private void Render()
        {
            glControl.MakeCurrent();
            //Set uniforms
            GL.Uniform3(1, new Vector3(glControl.ClientSize.Width, glControl.ClientSize.Height, 1.0f));
            if (!paused)//Updated uniforms only when not paused
            {
                GL.Uniform1(0, (float)stopwatch.Elapsed.TotalSeconds);
                GL.Uniform1(2, frame);
            }
            GL.Uniform4(3,new Vector4( position.X,position.Y,1,1));
            GL.Uniform1(4, (float)zoom);

            //Draw         
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);

            glControl.SwapBuffers();
            if (!paused)
                frame++;
        }
        void Restart()
        {
            position = new Vector2(0, 0);
            prevM = new Vector2(0, 0);
            zoom = 1000;
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

        private void shaderDataDisplayToolStripMenuItem_Click(object sender, EventArgs e)=>UpdateShader("../../../Res/data.frag");
        
    }
}
