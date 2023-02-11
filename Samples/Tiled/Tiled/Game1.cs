using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using MonoGame.SpriteEngine;
using TiledCS;

namespace Tiled;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 800;
        _graphics.PreferredBackBufferHeight = 640;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        var Map = new TiledMap("map1.tmx");

       // var TileSet = new TiledTileset("0.tsx");
        EngineFunc.Init("Images/", this.GraphicsDevice);
        for (var i = 0; i < Map.Layers[0].data.Length; i++)
        {
            int Index = Map.Layers[0].data[i];
            // Empty tile, do nothing
            if (Index == 0)
            {

            }
            else
            {
                var Tile = new SpriteEx(EngineFunc.SpriteEngine);
                Tile.ImageLib = EngineFunc.ImageLib;
                Tile.ImageName = "Tileset.png";
                Tile.SpriteSheetMode = SpriteSheetMode.FixedSize;
                Tile.SetPattern(16, 16);
                Tile.PatternIndex = Index - 1;
                Tile.DoMove(1);
                Tile.X = (i % Map.Width) * Map.TileWidth;
                Tile.Y = (float)Math.Floor(i / (double)Map.Width) * Map.TileHeight;
            }
        }
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
       
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(85,180,255));
        EngineFunc.SpriteEngine.Draw();
        base.Draw(gameTime);
    }
}