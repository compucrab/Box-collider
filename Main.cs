using Box_collider.Helpers;
using Box_collider.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Box_collider
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;

        private BasicEffect effect;
        private BasicEffect groundEffect;

        private Shape<VertexPositionColor> cube;
        private Shape<VertexPositionTexture> ground;

        float rotation = 0f;

        private Camera camera;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            GraphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.None };
            _graphics.PreferredBackBufferWidth = 960;
            _graphics.PreferredBackBufferHeight = 720;

            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            camera = new Camera(GraphicsDevice);

            cube = new Shape<VertexPositionColor>(GraphicsDevice, camera, ShapeData.maincube);
            ground = new Shape<VertexPositionTexture>(GraphicsDevice, camera, ShapeData.ground);

            effect = new BasicEffect(GraphicsDevice);
            effect.VertexColorEnabled = true;

            groundEffect = new BasicEffect(GraphicsDevice);
            groundEffect.TextureEnabled = true;
            groundEffect.Texture = Content.Load<Texture2D>("texture");
        }

        protected override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            ShapeData.DeltaTime = dt;
            rotation += dt * 2;

            effect.World = Matrix.CreateRotationZ(rotation);

            KeyboardManager.Update();
            MouseManager.Update();

            camera.Update();
            camera.UpdateView();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            cube.Draw(effect, PrimitiveType.TriangleList);
            ground.Draw(groundEffect, PrimitiveType.TriangleList);

            base.Draw(gameTime);
        }
    }
}
