using System.IO;
using OpenTK.Graphics.OpenGL;
using System.Text;
using System;
using System.Windows.Forms;

namespace Fractals
{
    public class Shader : IDisposable
    {
        int Handle;
        int VertShader;
        int FragShader;
        bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);
                disposedValue = true;
            }
        }
        ~Shader()=>GL.DeleteProgram(Handle);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Shader(string vert, string frag)
        {
            VertShader = 0;
            FragShader = 0;
            Handle = 0;
            string vSrc;
            using (StreamReader reader = new StreamReader(vert, Encoding.UTF8))
            {
                vSrc = reader.ReadToEnd();
            }
            string fSrc;
            using (StreamReader reader = new StreamReader(frag, Encoding.UTF8))
            {
                fSrc = reader.ReadToEnd();
            }

            VertShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertShader, vSrc);
            FragShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragShader, fSrc);

            GL.CompileShader(VertShader);
            string LogV = GL.GetShaderInfoLog(VertShader);
            if (LogV.Length > 0)
            {
                System.Console.WriteLine($"Vertex shader at {vert} creation log: {LogV}");
                MessageBox.Show(LogV);
            }
            GL.CompileShader(FragShader);
            string LogF = GL.GetShaderInfoLog(FragShader);
            if (LogF.Length > 0)
            {
                System.Console.WriteLine($"Fragment shader at {frag} creation log: {LogF}");
                MessageBox.Show(LogF);
            }
            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertShader);
            GL.AttachShader(Handle, FragShader);
            GL.LinkProgram(Handle);

            GL.DetachShader(Handle, VertShader);
            GL.DetachShader(Handle, FragShader);
            GL.DeleteShader(VertShader);
            GL.DeleteShader(FragShader);
        }

        public Shader( string frag)
        {
            VertShader = 0;
            FragShader = 0;
            Handle = 0;
            string vSrc = "#version 330\nlayout (location = 0) in vec3 Pos;void main(){gl_Position = vec4(Pos,1.0);}";

            string fSrc;
            using (StreamReader reader = new StreamReader(frag, Encoding.UTF8))
            {
                fSrc = reader.ReadToEnd();
            }

            VertShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertShader, vSrc);
            FragShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragShader, fSrc);

            GL.CompileShader(VertShader);        

            GL.CompileShader(FragShader);
            string LogF = GL.GetShaderInfoLog(FragShader);
            if (LogF.Length > 0)
            {
                System.Console.WriteLine($"Fragment shader at {frag} creation log: {LogF}");
                MessageBox.Show(LogF);
            }
            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertShader);
            GL.AttachShader(Handle, FragShader);
            GL.LinkProgram(Handle);
            //Cleanup
            GL.DetachShader(Handle, VertShader);
            GL.DetachShader(Handle, FragShader);
            GL.DeleteShader(VertShader);
            GL.DeleteShader(FragShader);
        }
        public void Use()=>GL.UseProgram(Handle);        
    }
}
