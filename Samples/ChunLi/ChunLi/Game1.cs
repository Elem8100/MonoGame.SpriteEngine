using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.SpriteEngine;

namespace ChunLi
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 720;
            this.IsFixedTimeStep = false;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            EngineFunc.Init("Images/", this.GraphicsDevice);
            GameFunc.CreateGame();
            EngineFunc.AddFont(this.GraphicsDevice, "Cou10", "Courier New", 12);
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
            EngineFunc.Canvas.Draw(EngineFunc.ImageLib["back.png"], 0, -50);
            EngineFunc.SpriteEngine.Draw();
            EngineFunc.Canvas.DrawString("Cou10","A,S,D,F,G: Hand Attack",50,50,Color.Aqua);
            EngineFunc.Canvas.DrawString("Cou10", "Z,X,C,V,B,N: Foot Attack", 50, 80, Color.Aqua);
            base.Draw(gameTime);
        }
    }
}