using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.SpriteEngine;
namespace Platformer
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 748;
            this.IsFixedTimeStep = false;
           
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            base.Initialize();
            EngineFunc.Init("Images/", this.GraphicsDevice);
            GameFunc.CreateMap();
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

            EngineFunc.BackgroundEngine.Move((float)gameTime.ElapsedGameTime.TotalMilliseconds / 16.6f);
            EngineFunc.SpriteEngine.Move((float)gameTime.ElapsedGameTime.TotalMilliseconds / 16.6f);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            EngineFunc.BackgroundEngine.Draw();
            EngineFunc.SpriteEngine.Draw();
            EngineFunc.SpriteEngine.Dead();
            base.Draw(gameTime);
        }
    }
}