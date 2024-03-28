using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Drawing;

namespace MalmaCraft
{
    public class Game : GameWindow
    {
        private static readonly float[] _vertexBufferData = [
            -1 * 1, -1 * 1, -10,
             1 * 1, -1 * 1, -10,
            -1 * 1,  1 * 1, -10,
             1 * 1,  1 * 1, -10
        ];

        private static readonly short[] _elementBufferData = [
            0, 1, 2, 3
        ];

        private int _vertexBuffer, _elementBuffer, _program;

        public Game() : base(
            GameWindowSettings.Default,
            new NativeWindowSettings
            {
                ClientSize = new(800, 600),
                Title = "MalmaCraft"
            })
        { }

        protected override void OnLoad()
        {
            base.OnLoad();

            _vertexBuffer = Utils.MakeBuffer(BufferTarget.ArrayBuffer, _vertexBufferData);
            _elementBuffer = Utils.MakeBuffer(BufferTarget.ElementArrayBuffer, _elementBufferData);

            _program = Utils.MakeProgram(Utils.LoadShader(ShaderType.VertexShader, "vertex.glsl"),
                                         Utils.LoadShader(ShaderType.FragmentShader, "fragment.glsl"));
        }

        private void Set3D(out float[] matrix)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            Utils.PerspectiveMatrix(out matrix, 65, ClientSize.X / ClientSize.Y, 0.1f, 60);
        }

        private float[] _matrix = new float[16];

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.ClearColor(Color.LightSkyBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Set3D(out _matrix);

            GL.UniformMatrix4(GL.GetUniformLocation(_program, "matrix"), 1, false, _matrix);
            GL.UseProgram(_program);

            var positionIndex = GL.GetAttribLocation(_program, "position");
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
            GL.VertexAttribPointer(positionIndex, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);
            GL.EnableVertexAttribArray(positionIndex);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBuffer);
            GL.DrawElements(PrimitiveType.TriangleStrip, 4, DrawElementsType.UnsignedShort, 0);
            GL.EnableVertexAttribArray(positionIndex);

            SwapBuffers();
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBuffer);
            GL.DeleteVertexArray(_elementBuffer);

            GL.DeleteProgram(_program);

            base.OnUnload();
        }
    }
}
