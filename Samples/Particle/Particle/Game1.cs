using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.SpriteEngine;
using System.Drawing;

namespace Particle
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 768;
            this.IsFixedTimeStep = false;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            EngineFunc.Init("Images/", this.GraphicsDevice);
            Particle.CreateScanline();
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
            EngineFunc.FixedUpdate(gameTime, Particle.CreateParticle);
            EngineFunc.SpriteEngine.Move((float)gameTime.ElapsedGameTime.TotalMilliseconds/16.6f);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Azure);

            // TODO: Add your drawing code here
           
            EngineFunc.SpriteEngine.Draw();
            EngineFunc.SpriteEngine.Dead();

            base.Draw(gameTime);
        }
    }
}