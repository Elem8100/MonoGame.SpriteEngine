using System;
using MonoGame.SpriteEngine;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Input = Microsoft.Xna.Framework.Input.Keys;
namespace Z_Order;

public class Sprite1 : Sprite
{
    public Sprite1(Sprite Parent) : base(Parent)
    {
        X = 120;
        Y = 500;
    }
    public float Speed = 1.5f;
    public override void DoMove(float MoveCount)
    {
        base.DoMove(MoveCount);
        Y += Speed;
        if (Y < 10 || Y > 600)
            Speed = -Speed;
        //dynamic change Z
        Z = (int)Y - 50;
    }
}
public class Sprite2 : Sprite
{
    public Sprite2(Sprite Parent) : base(Parent)
    {
    }
    public float Speed = 0.8f;
    public override void DoMove(float MoveCount)
    {
        base.DoMove(MoveCount);
        Y += Speed;
        if (Y < 250 || Y > 370)
            Speed = -Speed;
        //dynamic change Z
        Z = (int)Y - 50;
    }
}

public class Sprite3 : AnimatedSprite
{

    public Sprite3(Sprite Parent) : base(Parent)
    {
    }

    public int Counter;
    public override void DoMove(float MoveCount)
    {
        base.DoMove(MoveCount);
        Counter += 1;
        if ((Counter % 100) == 0)
            //dynamic change Z
            Z = -Z;
    }
}
public class GameFunc
{
    public static void CreateGame()
    {
        for (int i = 0; i < 4; i++)
        {
            var Sprite = new Sprite(EngineFunc.SpriteEngine, EngineFunc.ImageLib, "Tree6.png", 100, 50 + i * 150);
            Sprite.Z = (int)Sprite.Y;
        }
        Sprite1 Sprite1 = new Sprite1(EngineFunc.SpriteEngine);
        Sprite1.ImageLib = EngineFunc.ImageLib;
        Sprite1.ImageName = "Dog.png";
        //
        for (int i = 1; i <= 2; i++)
        {
            var Sprite = new Sprite2(EngineFunc.SpriteEngine);
            Sprite.ImageLib = EngineFunc.ImageLib;
            if (i == 1)
            {
                Sprite.X = 400;
                Sprite.Y = 255;
                Sprite.ImageName = "Dog.png";
            }
            else
            {
                Sprite.X = 410;
                Sprite.Y = 369;
                Sprite.ImageName = "Cat.png";
            }
            Sprite.Z = (int)Sprite.Y;
        }
        //
        for (int i = 0; i < 10; i++)
        {
            var Sprite = new Sprite3(EngineFunc.SpriteEngine);
            Sprite.DoMove(1);
            Sprite.Init(EngineFunc.ImageLib, "NumberSet.png", 650 + i * 25, 100 + i * 45, i, 88, 88);
            Sprite.PatternIndex = 9 - i;

        }

    }



}