using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.SpriteEngine;

namespace Basic
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 900;
            _graphics.PreferredBackBufferHeight = 671;
            this.IsFixedTimeStep = false;

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            EngineFunc.Init("Images/", this.GraphicsDevice);
            GameFunc.CreateGame();
            EngineFunc.AddFont(this.GraphicsDevice, "Cou15", "Courier New", 15);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            EngineFunc.SpriteEngine.Move((float)gameTime.ElapsedGameTime.TotalMilliseconds / 16.6f);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            EngineFunc.SpriteEngine.Draw();
            EngineFunc.SpriteEngine.Dead();
            EngineFunc.Canvas.DrawString("Cou15", "Camera.X=" + EngineFunc.SpriteEngine.Camera.X.ToString(), 50, 50, Color.Azure);
            EngineFunc.Canvas.DrawString("Cou15", "Camera.Y=" + EngineFunc.SpriteEngine.Camera.Y.ToString(), 50, 80, Color.Azure);
            EngineFunc.Canvas.DrawString("Cou15", "Sprite Count=" +(EngineFunc.SpriteEngine.SpriteList.Count-6401).ToString(), 50, 110, Color.Azure);
            base.Draw(gameTime);
        }
    }
}