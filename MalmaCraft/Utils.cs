using OpenTK.Graphics.OpenGL;

namespace MalmaCraft
{
    public static class Utils
    {
        public static int MakeBuffer(BufferTarget target, float[] data)
        {
            GL.GenBuffers(1, out int buffer);
            GL.BindBuffer(target, buffer);
            GL.BufferData(target, sizeof(float) * data.Length, data, BufferUsageHint.StaticDraw);
            GL.BindBuffer(target, 0);
            return buffer;
        }

        public static int MakeBuffer(BufferTarget target, short[] data)
        {
            GL.GenBuffers(1, out int buffer);
            GL.BindBuffer(target, buffer);
            GL.BufferData(target, sizeof(short) * data.Length, data, BufferUsageHint.StaticDraw);
            GL.BindBuffer(target, 0);
            return buffer;
        }

        public static int MakeShader(ShaderType type, string source)
        {
            var shader = GL.CreateShader(type);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
            if (status == 0)
            {
                GL.GetShaderInfoLog(shader, out string info);
                Console.Error.WriteLine("glCompileShader failed:\n{0}", info);
            }
            return shader;
        }

        public static int LoadShader(ShaderType type, string path)
        {
            return MakeShader(type, File.ReadAllText(path));
        }

        public static int MakeProgram(int shader1, int shader2)
        {
            var program = GL.CreateProgram();
            GL.AttachShader(program, shader1);
            GL.AttachShader(program, shader2);
            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int status);
            if (status == 0)
            {
                GL.GetProgramInfoLog(program, out string info);
                Console.Error.WriteLine("glLinkProgram failed: {0}", info);
            }

            GL.DetachShader(program, shader1);
            GL.DetachShader(program, shader2);

            return program;
        }

        public static int LoadProgram(ShaderType shader1Type, string shader1Path, ShaderType shader2Type, string shader2Path)
        {
            return MakeProgram(LoadShader(shader1Type, shader1Path), LoadShader(shader2Type, shader2Path));
        }

        public static void IdentityMatrix(out float[] matrix)
        {
            matrix = [
                1, 0, 0, 0, 0,
                1, 0, 0, 0, 0,
                1, 0, 0, 0, 0,
                1
           ];
        }

        public static void FrustumMatrix(out float[] matrix,
                                         float left,
                                         float right,
                                         float bottom,
                                         float top,
                                         float znear,
                                         float zfar)
        {
            var temp = 2 * znear;
            var temp2 = right - left;
            var temp3 = top - bottom;
            var temp4 = zfar - znear;
            matrix = [
                temp / temp2,            0,  0, 0,                      0,
                temp / temp3,            0,  0, (right + left) / temp2, (top + bottom) / temp3,
                (-zfar - znear) / temp4, -1, 0, 0,                      -temp * zfar / temp4,
                0
            ];
        }

        public static void PerspectiveMatrix(out float[] matrix,
                                             float fov,
                                             float aspect,
                                             float znear,
                                             float zfar)
        {
            var ymax = znear * MathF.Tan(fov * MathF.PI / 360);
            var xmax = ymax * aspect;
            FrustumMatrix(out matrix, -xmax, xmax, -ymax, ymax, znear, zfar);
        }
    }
}
