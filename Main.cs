using System.Collections.Generic;
using Box_collider.Helpers;
using Box_collider.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;

namespace Box_collider
{
    public class Main : Game
    {
        GraphicsDeviceManager _graphics;

        /*
         * BasicEffect is basically a ready-made shader/effect provided by MonoGame.
         * The thing that tells the GPU how to turn vertex data into visible pixels.
         */
        BasicEffect effect;
        BasicEffect groundEffect;

        BasicEffect selectionEffect;

        List<PhysicsObject> objects = new();
        Mesh<VertexPositionTexture> ground;

        PhysicsObject selectedObject;

        Camera camera;

        Label fpscounter;

        private PhysicsWorld physics;
        private PlaneCollider groundCollider;

        private float fpsTimer = 0f;
        private int frameCount = 0;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);

            // for uncapped fps enable the following 👇
            //_graphics.SynchronizeWithVerticalRetrace = false;
            //IsFixedTimeStep = false;

            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;

            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            camera = new Camera(GraphicsDevice);
            ground = new Mesh<VertexPositionTexture>(GraphicsDevice, camera, Globals.ground)
            {
                Position = new Vector3(0, 0, -0.001f),
            };

            effect = new BasicEffect(GraphicsDevice) { VertexColorEnabled = true };

            groundEffect = new BasicEffect(GraphicsDevice)
            {
                TextureEnabled = true,
                Texture = Content.Load<Texture2D>("texture"),
            };

            selectionEffect = new BasicEffect(GraphicsDevice) { VertexColorEnabled = true };

            groundCollider = new PlaneCollider(Vector3.Zero, Vector3.UnitZ, new Vector2(20, 20));
            physics = new PhysicsWorld(groundCollider);

            #region Debug UI
            DebugUI.Initialize(this);
            fpscounter = DebugUI.AddLabel("FPS count : 0");

            DebugUI.AddSlider(
                "Ground position",
                -10,
                10,
                value =>
                {
                    ground.Position.Z = value - 0.001f;
                    groundCollider.Position.Z = value;
                }
            );

            DebugUI.AddSlider(
                "Selected Cube X",
                -11,
                11,
                value =>
                {
                    if (selectedObject != null)
                        selectedObject.Body.Shape.Position.X = value;
                }
            );

            DebugUI.AddSlider(
                "Selected Cube Y",
                -11,
                11,
                value =>
                {
                    if (selectedObject != null)
                        selectedObject.Body.Shape.Position.Y = value;
                }
            );

            DebugUI.AddButton(
                "Add cube",
                () =>
                {
                    AddCube();
                }
            );

            #endregion
        }

        protected override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            fpsTimer += dt;
            frameCount++;

            if (fpsTimer >= 1f)
            {
                float fps = frameCount / fpsTimer;

                fpscounter.Text = $"FPS: {fps:0}";

                fpsTimer = 0f;
                frameCount = 0;
            }

            Globals.DeltaTime = dt;

            KeyboardManager.Update();
            MouseManager.Update();

            physics.Update(objects, dt);

            camera.Update();
            camera.UpdateView();

            if (MouseManager.LeftClicked)
            {
                SelectCube();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (PhysicsObject obj in objects)
            {
                if (obj.Body.IsActive)
                {
                    obj.Mesh.Draw(effect);
                }
            }
            if (selectedObject != null && selectedObject.Body.IsActive)
            {
                DrawSelectionBox(selectedObject);
            }

            ground.Draw(groundEffect);

            DebugUI.Draw();

            base.Draw(gameTime);
        }

        private void AddCube()
        {
            int id = objects.Count + 1;

            var mesh = new Mesh<VertexPositionColor>(GraphicsDevice, camera, Globals.maincube)
            {
                Position = new Vector3(0, 0, 10),
            };

            var body = new RigidBody(mesh, new BoxCollider(new Vector3(2, 2, 2)));

            objects.Add(new PhysicsObject($"Cube {id}", mesh, body));
        }

        private void SelectCube()
        {
            Ray ray = camera.GetMouseRay(MouseManager.Position);

            PhysicsObject closestObject = null;
            float closestDistance = float.MaxValue;

            foreach (PhysicsObject obj in objects)
            {
                if (!obj.Body.IsActive)
                    continue;

                Vector3 min = obj.Body.Collider.GetMin(
                    obj.Body.Shape.Position,
                    obj.Body.Shape.Scale
                );

                Vector3 max = obj.Body.Collider.GetMax(
                    obj.Body.Shape.Position,
                    obj.Body.Shape.Scale
                );

                BoundingBox box = new BoundingBox(min, max);

                float? distance = ray.Intersects(box);

                if (distance.HasValue && distance.Value < closestDistance)
                {
                    closestDistance = distance.Value;
                    closestObject = obj;
                }
            }

            if (closestObject != null)
            {
                selectedObject = closestObject;
            }
        }

        private void DrawSelectionBox(PhysicsObject obj)
        {
            Vector3 min = obj.Body.Collider.GetMin(obj.Body.Shape.Position, obj.Body.Shape.Scale);

            Vector3 max = obj.Body.Collider.GetMax(obj.Body.Shape.Position, obj.Body.Shape.Scale);

            Color bordercolor = Color.Red;

            Vector3[] corners =
            {
                new Vector3(min.X, min.Y, min.Z),
                new Vector3(max.X, min.Y, min.Z),
                new Vector3(max.X, max.Y, min.Z),
                new Vector3(min.X, max.Y, min.Z),
                new Vector3(min.X, min.Y, max.Z),
                new Vector3(max.X, min.Y, max.Z),
                new Vector3(max.X, max.Y, max.Z),
                new Vector3(min.X, max.Y, max.Z),
            };

            VertexPositionColor[] vertices =
            {
                // Bottom
                new(corners[0], bordercolor),
                new(corners[1], bordercolor),
                new(corners[1], bordercolor),
                new(corners[2], bordercolor),
                new(corners[2], bordercolor),
                new(corners[3], bordercolor),
                new(corners[3], bordercolor),
                new(corners[0], bordercolor),
                // Top
                new(corners[4], bordercolor),
                new(corners[5], bordercolor),
                new(corners[5], bordercolor),
                new(corners[6], bordercolor),
                new(corners[6], bordercolor),
                new(corners[7], bordercolor),
                new(corners[7], bordercolor),
                new(corners[4], bordercolor),
                // Vertical edges
                new(corners[0], bordercolor),
                new(corners[4], bordercolor),
                new(corners[1], bordercolor),
                new(corners[5], bordercolor),
                new(corners[2], bordercolor),
                new(corners[6], bordercolor),
                new(corners[3], bordercolor),
                new(corners[7], bordercolor),
            };

            selectionEffect.World = Matrix.Identity;
            selectionEffect.View = camera.View;
            selectionEffect.Projection = camera.Projection;

            foreach (EffectPass pass in selectionEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 12);
            }
        }
    }
}
