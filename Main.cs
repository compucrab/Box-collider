using Box_collider.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;

namespace Box_collider
{
    public class Main : Game
    {
        GraphicsDeviceManager _graphics;

        BasicEffect effect;
        BasicEffect groundEffect;

        Shape<VertexPositionColor> cube;
        Shape<VertexPositionTexture> ground;

        Camera camera;
        DebugUI debugUI;

        Label fpscounter;

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
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 960;

            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            camera = new Camera(GraphicsDevice);

            cube = new Shape<VertexPositionColor>(GraphicsDevice, camera, Globals.maincube);
            ground = new Shape<VertexPositionTexture>(GraphicsDevice, camera, Globals.ground);

            effect = new BasicEffect(GraphicsDevice) { VertexColorEnabled = true };

            groundEffect = new BasicEffect(GraphicsDevice)
            {
                TextureEnabled = true,
                Texture = Content.Load<Texture2D>("texture"),
            };
            debugUI = new DebugUI(this);
            fpscounter = debugUI.AddLabel("FPS count : 0");

            debugUI.AddSlider(
                "Cube Scale",
                1,
                5,
                value =>
                {
                    cube.Scale = Vector3.One * value;
                }
            );

            debugUI.AddButton("Reset", () => cube.Scale = Vector3.One);
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

            camera.Update();
            camera.UpdateView();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;

            GraphicsDevice.Clear(Color.CornflowerBlue);
            cube.Draw(effect, PrimitiveType.TriangleList);

            ground.Draw(groundEffect, PrimitiveType.TriangleList);
            debugUI.Draw();

            base.Draw(gameTime);
        }
    }
}
