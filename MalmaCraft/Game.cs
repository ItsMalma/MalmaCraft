using OpenTK.Graphics.OpenGL4;
using System.Drawing;

namespace MalmaCraft
{
    public class Game
    {
        private readonly Window window;

        public Game()
        {
            window = new(Init, Destroy, Tick, Update, Render);
        }

        private void Init(Window window)
        {

        }

        private void Destroy(Window window)
        {

        }

        private void Tick(Window window)
        {

        }

        private void Update(Window window)
        {

        }

        private void Render(Window window)
        {
            GL.ClearColor(Color.FromArgb(120, 167, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void Run()
        {
            window.Run();
        }
    }
}
