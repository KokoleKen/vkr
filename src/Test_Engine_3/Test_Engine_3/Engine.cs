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
        //float[] vertices =
        //{
        //    0.0f, 0.0f, 0.0f, 0.0f, 0.0f,   //Ближний левый нижний угол     0
        //    1.0f, 0.0f, 0.0f, 1.0f, 0.0f,   //Ближний правый нижний угол    1
        //    0.0f, 1.0f, 0.0f, 0.0f, 1.0f,   //Ближний левый верхний угол    2
        //    1.0f, 1.0f, 0.0f, 1.0f, 1.0f,   //Ближний правый верхний угол   3
        //    0.0f, 0.0f, 1.0f, 1.0f, 0.0f,   //Дальний левый нижний угол     4
        //    1.0f, 0.0f, 1.0f, 0.0f, 0.0f,   //Дальний правый нижний угол    5
        //    0.0f, 1.0f, 1.0f, 1.0f, 1.0f,   //Дальний левый верхний угол    6
        //    2.0f, 1.0f, 1.0f, 0.0f, 1.0f    //Дальний правый верхний угол   7
        //};

        //uint[] indices = {
        //    0, 2, 1,
        //    1, 3, 2,
        //    5, 7, 4,
        //    4, 7, 6,
        //    1, 3, 5,
        //    3, 5, 7,
        //    4, 2, 0,
        //    4, 6, 2,
        //    2, 6, 3,
        //    3, 6, 7,
        //    1, 4, 0,
        //    1, 5, 4
        //};

        //  Positions               Texture coords   Normals
        //  -0.5f, -0.5f, -0.5f,    0.0f, 0.0f,      0.0f,  0.0f, -1.0f

        float[] vertices =
        {
            0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f
        };
        uint[] indices =
        {
            0, 0, 0
        };

        float[] vertices_light =
        {
            0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f
        };

        uint[] indices_light = 
        {
            0, 0, 0
        };

        float[] vertices_scene;
        uint[] indices_scene;


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
        public bool inspector_status = true;

        // Свет
        private Shader _lightingShader;
        private Shader _lampShader;
        private int _vaoLamp;
        private Texture _specularMap;
        public Vector3 _lightPos = new Vector3(1.2f, 3.0f, 3.5f);

        public float light_ambient = 0.35f;
        public float light_diffuse = 0.95f;
        public float light_specular = 1.0f;

        // Цвет освещения
        public float color_rgb_red = 1.0f;
        public float color_rgb_green = 1.0f;
        public float color_rgb_blue = 1.0f;

        public float color_reflection = 5.0f;

        public Model model = new Model("Objects/model.obj");


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

            // Объединение данных объектов сцены в один массив
            uint max_index = 0;
            for (int i = 0; i < indices.Length; i++) 
            {
                if (indices[i] > max_index)
                    max_index = indices[i];
            }

            for (int i = 0; i < indices_light.Length; i++)
            {
                if (max_index != 0)
                    indices_light[i] += max_index + 1;
            }

            vertices_scene = vertices.Concat(vertices_light).ToArray();
            indices_scene = indices.Concat(indices_light).ToArray();


            GL.ClearColor(0.3f, 0.5f, 0.5f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices_scene.Length * sizeof(float), vertices_scene, BufferUsageHint.StaticDraw);
            //GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices_scene.Length * sizeof(float), vertices_scene, BufferUsageHint.StaticDraw);
            //GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            _lightingShader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
            _lampShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            {
                VertexArrayObject = GL.GenVertexArray();
                GL.BindVertexArray(VertexArrayObject);

                var positionLocation = _lightingShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

                var texCoordLocation = _lightingShader.GetAttribLocation("aTexCoords");
                GL.EnableVertexAttribArray(texCoordLocation);
                GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

                var normalLocation = _lightingShader.GetAttribLocation("aNormal");
                GL.EnableVertexAttribArray(normalLocation);
                GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 5 * sizeof(float));
            }

            {
                _vaoLamp = GL.GenVertexArray();
                GL.BindVertexArray(_vaoLamp);

                var positionLocation = _lampShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            }

            // Our two textures are loaded in from memory, you should head over and
            // check them out and compare them to the results.
            texture = Texture.LoadFromFile("Objects/Textures/Rock_Texture.png");
            _specularMap = Texture.LoadFromFile("Objects/Textures/Specular/specular_map.png");

            camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);

            //base.OnLoad();
            //GL.ClearColor(0.3f, 0.5f, 0.5f, 1.0f);

            //GL.Enable(EnableCap.DepthTest);

            //VertexArrayObject = GL.GenVertexArray();
            //GL.BindVertexArray(VertexArrayObject);

            //VertexBufferObject = GL.GenBuffer();
            //GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            //GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

            //ElementBufferObject = GL.GenBuffer();
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            //GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            //shader = new Shader("shader.vert", "shader.frag");
            //shader.Use();

            //// Шейдер света
            //_lightingShader = new Shader("shader.vert", "lighting.frag");
            //_lightingShader.Use();

            //var vertexLocation = shader.GetAttribLocation("aPosition");
            //GL.EnableVertexAttribArray(vertexLocation);
            //GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            //int texCoordLocation = shader.GetAttribLocation("aTexCoord");
            //GL.EnableVertexAttribArray(texCoordLocation);
            //GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            //// Спекулярная текстура
            //_diffuseMap = Texture.LoadFromFile("Objects/Textures/Specular/specular_map.png");

            //texture = Texture.LoadFromFile("Objects/Textures/pop_cat2.png");
            //texture.Use(TextureUnit.Texture0);

            //shader.SetInt("texture0", 0);

            //camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);

            this.WindowState = WindowState.Maximized;

            form.engine = self;
            inspector_running.ApartmentState = ApartmentState.STA;
            inspector_running.Start();
        }

        public void Initialize()
        {
            vertices = new float[scene.objects[form.object_selected].Item1.vertices.Length];
            indices = new uint[scene.objects[form.object_selected].Item1.indices.Length];

            model = scene.objects[form.object_selected].Item1;
            vertices = model.GetVertices(); // 1300 мс процесса
            indices = model.GetIndices();
            //vertices = scene.objects[form.object_selected].Item1.GetVertices();
            //indices = scene.objects[form.object_selected].Item1.GetIndices();

            //vertices = new float[scene.models[form.model_selected].vertices.Length];
            //indices = new uint[scene.models[form.model_selected].indices.Length];

            //vertices = scene.models[form.model_selected].GetVertices();
            //indices = scene.models[form.model_selected].GetIndices();

            Model LightModel = new Model("Objects/sphere.obj");
            vertices_light = new float[LightModel.vertices.Length];
            indices_light = new uint[LightModel.indices.Length];

            vertices_light = LightModel.GetVertices();
            indices_light = LightModel.GetIndices();

            base.OnLoad();

            // Объединение данных объектов сцены в один массив
            uint max_index = 0;
            for (int i = 0; i < indices.Length; i++)
            {
                if (indices[i] > max_index)
                    max_index = indices[i];
            }

            for (int i = 0; i < indices_light.Length; i++)
            {
                if (max_index != 0)
                    indices_light[i] += max_index + 1;
            }

            vertices_scene = vertices.Concat(vertices_light).ToArray();
            indices_scene = indices.Concat(indices_light).ToArray();


            GL.Enable(EnableCap.DepthTest);

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices_scene.Length * sizeof(float), vertices_scene, BufferUsageHint.StaticDraw);
            //GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices_scene.Length * sizeof(float), vertices_scene, BufferUsageHint.StaticDraw);
            //GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            _lightingShader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
            _lampShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            {
                VertexArrayObject = GL.GenVertexArray();
                GL.BindVertexArray(VertexArrayObject);

                var positionLocation = _lightingShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

                var texCoordLocation = _lightingShader.GetAttribLocation("aTexCoords");
                GL.EnableVertexAttribArray(texCoordLocation);
                GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

                var normalLocation = _lightingShader.GetAttribLocation("aNormal");
                GL.EnableVertexAttribArray(normalLocation);
                GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 5 * sizeof(float));
            }

            {
                _vaoLamp = GL.GenVertexArray();
                GL.BindVertexArray(_vaoLamp);

                var positionLocation = _lampShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            }

            texture = Texture.LoadFromFile("Objects/Textures/" + scene.objects[form.object_selected].Item2);
            _specularMap = Texture.LoadFromFile("Objects/Textures/Specular/specular_map.png");

            //camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);

            //GL.Enable(EnableCap.DepthTest);

            //VertexArrayObject = GL.GenVertexArray();
            //GL.BindVertexArray(VertexArrayObject);

            //VertexBufferObject = GL.GenBuffer();
            //GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            //GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            //ElementBufferObject = GL.GenBuffer();
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            //GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            //// Шейдер света
            //_lightingShader = new Shader("shader.vert", "lighting.frag");
            //_lightingShader.Use();

            //shader = new Shader("shader.vert", "shader.frag");
            //shader.Use();

            //var vertexLocation = shader.GetAttribLocation("aPosition");
            //GL.EnableVertexAttribArray(vertexLocation);
            //GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            //int texCoordLocation = shader.GetAttribLocation("aTexCoord");
            //GL.EnableVertexAttribArray(texCoordLocation);
            //GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            //texture = Texture.LoadFromFile(scene.textures[form.texture_selected]);
            //texture.Use(TextureUnit.Texture0);

            //shader.SetInt("texture0", 0);

            initialised = true;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            time += 4.0 * e.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindVertexArray(VertexArrayObject);

            // The two textures need to be used, in this case we use the diffuse map as our 0th texture
            // and the specular map as our 1st texture.
            texture.Use(TextureUnit.Texture0);
            _specularMap.Use(TextureUnit.Texture1);
            _lightingShader.Use();

            var model = Matrix4.Identity * Matrix4.CreateRotationX(form.transform_rotate_x) * Matrix4.CreateRotationY(form.transform_rotate_y) * Matrix4.CreateRotationZ(form.transform_rotate_z) * Matrix4.CreateTranslation(form.transform_move_x, form.transform_move_y, form.transform_move_z) * Matrix4.CreateScale(form.transform_scale_x, form.transform_scale_y, form.transform_scale_z);
            _lightingShader.SetMatrix4("model", model);
            _lightingShader.SetMatrix4("view", camera.GetViewMatrix());
            _lightingShader.SetMatrix4("projection", camera.GetProjectionMatrix());

            _lightingShader.SetVector3("viewPos", camera.Position);

            // Here we specify to the shaders what textures they should refer to when we want to get the positions.
            _lightingShader.SetInt("material.diffuse", 0);
            _lightingShader.SetInt("material.specular", 1);
            _lightingShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            _lightingShader.SetFloat("material.shininess", 32.0f);

            _lightingShader.SetVector3("light.position", _lightPos);
            _lightingShader.SetVector3("light.color", new Vector3(color_rgb_red, color_rgb_green, color_rgb_blue));
            _lightingShader.SetFloat("light.color_reflection", color_reflection);
            _lightingShader.SetVector3("light.ambient", new Vector3(light_ambient));
            _lightingShader.SetVector3("light.diffuse", new Vector3(light_diffuse));
            _lightingShader.SetVector3("light.specular", new Vector3(light_specular));

            GL.DrawArrays(PrimitiveType.Triangles, 0, indices.Length);

            GL.BindVertexArray(_vaoLamp);

            _lampShader.Use();

            Matrix4 lampMatrix = Matrix4.Identity;
            lampMatrix *= Matrix4.CreateScale(0.2f);
            lampMatrix *= Matrix4.CreateTranslation(_lightPos);

            _lampShader.SetVector3("color", new Vector3(color_rgb_red, color_rgb_green, color_rgb_blue));

            _lampShader.SetMatrix4("model", lampMatrix);
            _lampShader.SetMatrix4("view", camera.GetViewMatrix());
            _lampShader.SetMatrix4("projection", camera.GetProjectionMatrix());

            GL.DrawArrays(PrimitiveType.Triangles, indices.Length, indices_scene.Length);

            SwapBuffers();

            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.BindVertexArray(VertexArrayObject);

            //texture.Use(TextureUnit.Texture0);
            //_diffuseMap.Use(TextureUnit.Texture1);
            //_lightingShader.Use();

            //_lightingShader.SetMatrix4("model", Matrix4.Identity);
            //_lightingShader.SetMatrix4("view", camera.GetViewMatrix());
            //_lightingShader.SetMatrix4("projection", camera.GetProjectionMatrix());

            //_lightingShader.SetVector3("viewPos", camera.Position);

            //_lightingShader.SetInt("material.diffuse", 0);
            //_lightingShader.SetInt("material.specular", 1);
            //_lightingShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            //_lightingShader.SetFloat("material.shininess", 32.0f);

            //_lightingShader.SetVector3("light.position", _lightPos);
            //_lightingShader.SetVector3("light.ambient", new Vector3(0.2f));
            //_lightingShader.SetVector3("light.diffuse", new Vector3(0.5f));
            //_lightingShader.SetVector3("light.specular", new Vector3(1.0f));

            //shader.Use();

            ////// Новая подпрограмма шейдеров
            ////Matrix4 lampMatrix = Matrix4.Identity;
            ////lampMatrix *= Matrix4.CreateScale(0.2f);
            ////lampMatrix *= Matrix4.CreateTranslation(_lightPos);

            ////shader.SetMatrix4("model", lampMatrix);
            ////shader.SetMatrix4("view", camera.GetViewMatrix());
            ////shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            ////Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(time))

            //var model = Matrix4.Identity * Matrix4.CreateRotationX(form.transform_rotate_x) * Matrix4.CreateRotationY(form.transform_rotate_y) * Matrix4.CreateRotationZ(form.transform_rotate_z) * Matrix4.CreateTranslation(form.transform_move_x, form.transform_move_y, form.transform_move_z) * Matrix4.CreateScale(form.transform_scale_x, form.transform_scale_y, form.transform_scale_z);
            //shader.SetMatrix4("model", model);
            //shader.SetMatrix4("view", camera.GetViewMatrix());
            //shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            //GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            //SwapBuffers();
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

            if (form.object_selected == null)
            {
                vertices = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
                indices = new uint[] { 0, 0, 0 };
            }
        }

        public void ReloadScene()
        {
            float[] vertices =
            {
                0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f
            };
            uint[] indices =
            {
                0, 0, 0
            };

            float[] vertices_light =
            {
                0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f
            };

            uint[] indices_light =
            {
                0, 0, 0
            };

            float[] vertices_scene;
            uint[] indices_scene;

            // Объединение данных объектов сцены в один массив
            uint max_index = 0;
            for (int i = 0; i < indices.Length; i++)
            {
                if (indices[i] > max_index)
                    max_index = indices[i];
            }

            for (int i = 0; i < indices_light.Length; i++)
            {
                if (max_index != 0)
                    indices_light[i] += max_index + 1;
            }

            vertices_scene = vertices.Concat(vertices_light).ToArray();
            indices_scene = indices.Concat(indices_light).ToArray();

            GL.ClearColor(0.3f, 0.5f, 0.5f, 1.0f);

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices_scene.Length * sizeof(float), vertices_scene, BufferUsageHint.StaticDraw);
            //GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices_scene.Length * sizeof(float), vertices_scene, BufferUsageHint.StaticDraw);
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
