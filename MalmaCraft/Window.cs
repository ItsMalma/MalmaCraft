using MalmaCraft.Extensions;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ErrorCode = OpenTK.Windowing.GraphicsLibraryFramework.ErrorCode;
using Handle = OpenTK.Windowing.GraphicsLibraryFramework.Window;

namespace MalmaCraft
{
    public struct Button
    {
        public bool Down;
        public bool Last;
        public bool LastTick;
        public bool Pressed;
        public bool PressedTick;
    }

    public struct Mouse()
    {
        public Button[] Buttons = new Button[(int)MouseButton.Last];
        public Vector2d Position;
        public Vector2d Delta;

        public readonly void Tick()
        {
            for (var i = 0; i < Buttons.Length; i++)
            {
                Buttons[i].PressedTick = Buttons[i].Down && !Buttons[i].LastTick;
                Buttons[i].LastTick = Buttons[i].Down;
            }
        }

        public readonly void Update()
        {
            for (var i = 0; i < Buttons.Length; i++)
            {
                Buttons[i].Pressed = Buttons[i].Down && !Buttons[i].Last;
                Buttons[i].Last = Buttons[i].Down;
            }
        }
    }

    public struct Keyboard()
    {
        public Button[] Keys = new Button[(int)OpenTK.Windowing.GraphicsLibraryFramework.Keys.LastKey];

        public readonly void Tick()
        {
            for (var i = 0; i < Keys.Length; i++)
            {
                Keys[i].PressedTick = Keys[i].Down && !Keys[i].LastTick;
                Keys[i].LastTick = Keys[i].Down;
            }
        }

        public readonly void Update()
        {
            for (var i = 0; i < Keys.Length; i++)
            {
                Keys[i].Pressed = Keys[i].Down && !Keys[i].Last;
                Keys[i].Last = Keys[i].Down;
            }
        }
    }

    public delegate void WindowEventHandler(Window window);

    public sealed unsafe class Window
    {
        private readonly Handle*  handle;

        private Mouse    mouse = new();
        private Keyboard keyboard = new();

        private long lastSecond, frames, fps, lastFrame, frameDelta, ticks, tps, tickRemainder;
        private readonly WindowEventHandler init, destroy, tick, update, render;
        private readonly GLFWCallbacks.FramebufferSizeCallback framebufferSizeCallback;
        private readonly GLFWCallbacks.CursorPosCallback cursorPosCallback;
        private readonly GLFWCallbacks.KeyCallback keyCallback;
        private readonly GLFWCallbacks.MouseButtonCallback mouseButtonCallback;

        public Vector2i Size { get; private set; } = new(1280, 720);
        public string Title { get; private set; } = "MalmaCraft";

        public Window(WindowEventHandler init,
                       WindowEventHandler destroy,
                       WindowEventHandler tick,
                       WindowEventHandler update,
                       WindowEventHandler render)
        {
            this.init = init;
            this.destroy = destroy;
            this.tick = tick;
            this.update = update;
            this.render = render;

            lastFrame = DateTime.UtcNow.Nanosecond;
            lastSecond = DateTime.UtcNow.Nanosecond;

            GLFW.SetErrorCallback(OnError);

            if (!GLFW.Init())
            {
                Console.Error.WriteLine("Failed to initialize GLFW");
                Environment.Exit(1);
            }

            GLFW.WindowHint(WindowHintBool.Resizable, true);
            GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 3);
            GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 3);
            GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
            GLFW.WindowHint(WindowHintBool.OpenGLForwardCompat, true);

            handle = GLFW.CreateWindow(Size.X, Size.Y, Title, null, null);
            if (handle == null)
            {
                Console.Error.WriteLine("Failed to create GLFW's window");
                GLFW.Terminate();
                Environment.Exit(1);
            }

            GLFW.MakeContextCurrent(handle);

            framebufferSizeCallback = OnResize;
            GLFW.SetFramebufferSizeCallback(handle, framebufferSizeCallback);

            cursorPosCallback = OnMouseMove;
            GLFW.SetCursorPosCallback(handle, cursorPosCallback);

            keyCallback = OnKeyPress;
            GLFW.SetKeyCallback(handle, keyCallback);

            mouseButtonCallback = OnMouseClick;
            GLFW.SetMouseButtonCallback(handle, mouseButtonCallback);

            GL.LoadBindings(new GLFWBindingsContext());

            GLFW.SwapInterval(1);
        }

        ~Window()
        {
            destroy(this);
            GLFW.Terminate();
        }

        private void Init()
        {
            init(this);
        }

        private void Tick()
        {
            ticks++;
            mouse.Tick();
            keyboard.Tick();
            tick(this);
        }

        private void Update()
        {
            mouse.Update();
            keyboard.Update();
            update(this);

            mouse.Delta = Vector2d.Zero;
        }

        private void Render()
        {
            frames++;
            render(this);
        }

        public void Run()
        {
            Init();

            while (!GLFW.WindowShouldClose(handle))
            {
                long now = DateTime.UtcNow.Nanosecond;

                frameDelta = now - lastFrame;
                lastFrame = now;

                if (now - lastSecond >= TimeUtil.NanosecondPerSecond)
                {
                    fps = frames;
                    tps = ticks;
                    frames = 0;
                    ticks = 0;
                    lastSecond = now;

                    Console.WriteLine($"FPS: {fps} | TPS: {tps}");
                }

                var nanosecondPerTick = TimeUtil.NanosecondPerSecond / 60;
                var tickTime = frameDelta + tickRemainder;
                while (tickTime >= nanosecondPerTick)
                {
                    Tick();
                    tickTime -= nanosecondPerTick;
                }
                tickRemainder = Math.Max(tickTime, 0);

                Update();
                Render();

                GLFW.SwapBuffers(handle);
                GLFW.PollEvents();
            }
        }

        public bool MouseGrabbed
        {
            get => GLFW.GetInputMode(handle, CursorStateAttribute.Cursor) == CursorModeValue.CursorDisabled;
            set => GLFW.SetInputMode(handle, CursorStateAttribute.Cursor, value ? CursorModeValue.CursorDisabled : CursorModeValue.CursorNormal);
        }

        private static void OnError(ErrorCode code, string description)
        {
            Console.Error.WriteLine($"GLFW error {code}: {description}");
        }

        private void OnResize(Handle* window, int width, int height)
        {
            GL.Viewport(0, 0, width, height);
            Size = new(width, height);
        }

        private void OnMouseMove(Handle* window, double x, double y)
        {
            Vector2d newPosition = new(x, y);

            mouse.Delta = newPosition - mouse.Position;
            mouse.Delta.X = Math.Clamp(mouse.Delta.X, -100, 100);
            mouse.Delta.Y = Math.Clamp(mouse.Delta.Y, -100, 100);

            mouse.Position = newPosition;
        }

        private void OnKeyPress(Handle* window, Keys key, int scancode, InputAction action, KeyModifiers mods)
        {
            if (key < 0)
                return;

            keyboard.Keys[(int)key].Down = action switch
            {
                InputAction.Press => true,
                InputAction.Release => false,
                _ => keyboard.Keys[(int)key].Down
            };
        }

        private void OnMouseClick(Handle* window, MouseButton button, InputAction action, KeyModifiers mods)
        {
            if (button < 0)
                return;

            mouse.Buttons[(int)button].Down = action switch
            {
                InputAction.Press => true,
                InputAction.Release => false,
                _ => mouse.Buttons[(int)button].Down
            };
        }
    }
}
