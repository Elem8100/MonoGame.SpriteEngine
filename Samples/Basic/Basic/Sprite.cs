using System;
using MonoGame.SpriteEngine;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Input = Microsoft.Xna.Framework.Input.Keys;
namespace Basic;
public class PlayerSprite : Sprite
{
    public PlayerSprite(Sprite Parent) : base(Parent)
    {
    }
    bool KeyDown(Input Key)
    {
        return (Keyboard.GetState().IsKeyDown(Key));
    }
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        CollidePos = new Vector2(X + 50, Y + 50);

        if (KeyDown(Input.Up))
            Y -= 3*Delta;
        if (KeyDown(Input.Down))
            Y += 3*Delta;
        if (KeyDown(Input.Left))
            X -= 3*Delta;
        if (KeyDown(Input.Right))
            X += 3*Delta;
        Collision();
        Engine.Camera.X = X - 452;
        Engine.Camera.Y = Y - 300;
    }

    public override void OnCollision(Sprite sprite)
    {
        if (sprite is BallSprite)
        {
            ((BallSprite)(sprite)).ImageName = "img1-2.png";
            ((BallSprite)(sprite)).CanCollision= false;
            ((BallSprite)(sprite)).Hit = true;
        }
    }
}

public class BallSprite : SpriteEx
{
    public BallSprite(Sprite Parent) : base(Parent)
    {
    }
    public int Counter;
    public int Life;
    public bool Hit;
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        CollidePos = new Vector2(X + 80, Y + 80);
        Counter += 1;
        X += (float)(Sin256(Counter)) * 3*Delta;
        Y += (float)(Cos256(Counter)) * 3*Delta;
        if (Hit)
        {
            Life -= 1;
            if (Life < 0)
                Dead();
        }
    }
}

public class GameFunc
{
    public static void CreateGame()
    {
        //create tiles
        Sprite[,] Tiles = new Sprite[80, 80];
        Random Rnd = new Random();
        for (int i = 0; i < 80; i++)
        {
            for (int j = 0; j < 80; j++)
            {
                Tiles[i, j] = new Sprite(EngineFunc.SpriteEngine);
                Tiles[i, j].ImageLib = EngineFunc.ImageLib;
                Tiles[i, j].ImageName = "t" + Rnd.Next(0, 19) + ".png";
                Tiles[i, j].SetSize(64, 64);
                Tiles[i, j].X = i * 64;
                Tiles[i, j].Y = j * 64;
                Tiles[i, j].Z = 1;
            }
        }
        
        //Create Ball
        for (int i = 0; i < 200; i++)
        {
            BallSprite ballSprite = new BallSprite(EngineFunc.SpriteEngine);
            ballSprite.ImageLib = EngineFunc.ImageLib;
            ballSprite.ImageName = "img1.png";
            ballSprite.SetSize(200, 200);
            ballSprite.X = Rnd.Next(0, 5000);
            ballSprite.Y = Rnd.Next(0, 5000);
            ballSprite.CanCollision = true;
            ballSprite.CollideRadius = 80;
            ballSprite.Counter = Rnd.Next(0, 1000);
            ballSprite.Life = 15;
            ballSprite.Hit = false;
        }

        //Create Player
        PlayerSprite PlayerSprite1 = new PlayerSprite(EngineFunc.SpriteEngine);
        PlayerSprite1.ImageLib = EngineFunc.ImageLib;
        PlayerSprite1.ImageName = "img2.png";
        PlayerSprite1.X = 2500;
        PlayerSprite1.Y = 2500;
        PlayerSprite1.Z = 10;
        PlayerSprite1.CanCollision = true;
        PlayerSprite1.CollideRadius = 50;

    }



}