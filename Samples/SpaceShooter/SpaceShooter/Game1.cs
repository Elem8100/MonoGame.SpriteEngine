using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.SpriteEngine;
using Mouse=SpriteEngine.Mouse;
namespace SpaceShooter
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
          
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            EngineFunc.Init("Images/", this.GraphicsDevice);
            GameFunc.CreateGame();
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
            GameFunc.UpdataGame(1);
            base.Update(gameTime);

           
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            GameFunc.SpaceLayer.Draw();
            GameFunc.MistLayer1.Draw();
            GameFunc.MistLayer2.Draw();
            EngineFunc.SpriteEngine.Draw();
            var Angle = SpriteUtils.GetAngle256( Mouse.State.X - 512, Mouse.State.Y - 384) * 0.025f;
            EngineFunc.Canvas.DrawRotate(EngineFunc.ImageLib["cursor.png"], Mouse.State.X, Mouse.State.Y,Angle,BlendMode.AddtiveColor);

            base.Draw(gameTime);
        }
    }
}