using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Xml.Linq;
using OpenTKTesting;
using StbImageSharp;
using static StbImageSharp.StbImage;
using System.IO;
using System.Reflection.Metadata;

namespace Test_Engine_3
{
    public class Engine : GameWindow
    {
        float[] vertices2 =
        {
            0.0f, 0.0f, 0.0f, 0.0f, 0.0f,   //Ближний левый нижний угол     0
            1.0f, 0.0f, 0.0f, 1.0f, 0.0f,   //Ближний правый нижний угол    1
            0.0f, 1.0f, 0.0f, 0.0f, 1.0f,   //Ближний левый верхний угол    2
            1.0f, 1.0f, 0.0f, 1.0f, 1.0f,   //Ближний правый верхний угол   3
            0.0f, 0.0f, 1.0f, 1.0f, 0.0f,   //Дальний левый нижний угол     4
            1.0f, 0.0f, 1.0f, 0.0f, 0.0f,   //Дальний правый нижний угол    5
            0.0f, 1.0f, 1.0f, 1.0f, 1.0f,   //Дальний левый верхний угол    6
            2.0f, 1.0f, 1.0f, 0.0f, 1.0f    //Дальний правый верхний угол   7
        };

        uint[] indices2 = {  // note that we start from 0!
            0, 2, 1,
            1, 3, 2,
            5, 7, 4,
            4, 7, 6,
            1, 3, 5,
            3, 5, 7,
            4, 2, 0,
            4, 6, 2,
            2, 6, 3,
            3, 6, 7,
            1, 4, 0,
            1, 5, 4
        };

        float[] vertices = 
        {
            0.0f, 0.0f, 0.0f, 0.0f, 0.0f
        };
        uint[] indices =
        {
            0, 0, 0
        };

        public Engine self;

        private int VertexBufferObject;
        private int VertexArrayObject;
        private int ElementBufferObject;

        private Shader shader;
        private Texture texture;
        public Camera camera;
        public Scene scene = new Scene();

        public bool initialised = false;
        private bool firstMove = true;
        private Vector2 lastPos;
        private double time;

        static Inspector form = new Inspector();
        Thread inspector_running = new Thread(new ThreadStart(Run_Inspector));

        public Engine(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title })
        {

        }

        public static void Run_Inspector()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(form);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            shader = new Shader("shader.vert", "shader.frag");
            shader.Use();

            var vertexLocation = shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            texture = Texture.LoadFromFile("Objects/Textures/pop_cat2.png");
            texture.Use(TextureUnit.Texture0);

            shader.SetInt("texture0", 0);

            camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);

            this.WindowState = WindowState.Maximized;

            form.engine = self;
            inspector_running.ApartmentState = ApartmentState.STA;
            inspector_running.Start();
        }

        public void Initialize()
        {
            vertices = new float[scene.models[form.model_selected].vertices.Length];
            indices = new uint[scene.models[form.model_selected].indices.Length];

            vertices = scene.models[form.model_selected].GetVertices();
            indices = scene.models[form.model_selected].GetIndices();

            GL.Enable(EnableCap.DepthTest);

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            shader = new Shader("shader.vert", "shader.frag");
            shader.Use();

            var vertexLocation = shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            texture = Texture.LoadFromFile(scene.textures[form.texture_selected]);
            texture.Use(TextureUnit.Texture0);

            shader.SetInt("texture0", 0);

            initialised = true;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            time += 4.0 * e.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindVertexArray(VertexArrayObject);

            texture.Use(TextureUnit.Texture0);
            shader.Use();

            //Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(time))

            var model = Matrix4.Identity * Matrix4.CreateRotationX(form.transform_rotate_x) * Matrix4.CreateRotationY(form.transform_rotate_y) * Matrix4.CreateRotationZ(form.transform_rotate_z) * Matrix4.CreateTranslation(form.transform_move_x, form.transform_move_y, form.transform_move_z) * Matrix4.CreateScale(form.transform_scale_x, form.transform_scale_y, form.transform_scale_z);
            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (form.object_reload == true && initialised == false)
            {
                Initialize();
            }
            else if (form.object_reload == false)
                initialised = false;


            if (!IsFocused)
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape))
            {
                Close();
            }

            const float cameraSpeed = 1.5f;
            const float sensetivity = 0.2f;

            if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.W))
            {
                camera.Position += camera.Front * cameraSpeed * (float)e.Time;
            }

            if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.A))
            {
                camera.Position -= camera.Right * cameraSpeed * (float)e.Time;
            }

            if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.S))
            {
                camera.Position -= camera.Front * cameraSpeed * (float)e.Time;
            }

            if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D))
            {
                camera.Position += camera.Right * cameraSpeed * (float)e.Time;
            }

            if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Space))
            {
                camera.Position += new Vector3(0.0f, 1.0f, 0.0f) * cameraSpeed * (float)e.Time;
            }

            if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftControl))
            {
                camera.Position -= new Vector3(0.0f, 1.0f, 0.0f) * cameraSpeed * (float)e.Time;
            }

            var mouse = MouseState;
            if (mouse.WasButtonDown(MouseButton.Right))
            {
                CursorState = CursorState.Grabbed;

                if (firstMove)
                {
                    lastPos = new Vector2(mouse.X, mouse.Y);
                    firstMove = false;
                }
                else
                {
                    var deltaX = mouse.X - lastPos.X;
                    var deltaY = mouse.Y - lastPos.Y;
                    lastPos = new Vector2(mouse.X, mouse.Y);

                    camera.Yaw += deltaX * sensetivity;
                    camera.Pitch -= deltaY * sensetivity;
                }
            }
            else
            {
                firstMove = true;

                CursorState = CursorState.Normal;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            camera.Fov -= e.OffsetY * 3;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);

            camera.AspectRatio = Size.X / (float)Size.Y;
        }

        protected override void OnUnload()
        {
            base.OnUnload();


        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            Application.Exit();
        }

        protected virtual void OnInspectorClose(object sender, EventArgs e)
        {
            Close();
        }
    }
}
