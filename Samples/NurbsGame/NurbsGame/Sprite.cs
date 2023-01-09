using System;
using MonoGame.SpriteEngine;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace NurbsGame;
public class Player : SpriteEx
{
    public Player(Sprite Parent) : base(Parent)
    {
        DoCenter = true;
    }
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        LookAt(GameFunc.MouseState.X, GameFunc.MouseState.Y);
    }
}

public class SpaceBall : NPathSprite
{
    public SpaceBall(Sprite Parent) : base(Parent)
    {
        SpriteSheetMode = SpriteSheetMode.NoneSingle;
    }
    public int ID;
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        CollidePos = new Vector2(X + 31, Y + 31);
        // if (Distance >= 100) Dead();
        LookAt(3.14159f / 2);
        if (Y > 450 && X > 375)
        {
            Dead();
        }
    }

}

public class ShotBall : PlayerSprite
{
    public ShotBall(Sprite Parent) : base(Parent)
    {
        Random Random = new Random();
        ImageLib = EngineFunc.ImageLib;
        switch (Random.Next(0, 4))
        {
            case 0: ImageName = "Ball0.png"; break;
            case 1: ImageName = "Ball1.png"; break;
            case 2: ImageName = "Ball2.png"; break;
            case 3: ImageName = "Ball3.png"; break;
        }
        CanCollision = true;
        CollideMode = CollideMode.Circle;
        CollideRadius = 30;
        Velocity = 15;
        LifeTime = Velocity;
        Decay = LifeTime / 100;
        Fired = false;
        Angle = GameFunc.Player.Angle;
        X = GameFunc.Player.X;
        Y = GameFunc.Player.Y;
        Z = 100;
        CanCollision = true;
        CollideMode = CollideMode.Circle;
        DoCenter = true;
    }

    public bool Fired;
    public float Velocity;
    public float Decay;
    public float LifeTime;
    public void MoveOut()
    {
        this.Dead();
        GameFunc.CanCharge = true;
    }
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        GameFunc.CanCharge = false;
        if (!Fired)
        {
            Angle = GameFunc.Player.Angle;
            X = GameFunc.Player.X;
            Y = GameFunc.Player.Y;
        }
        else
        {
            X += (float)Math.Cos(Angle - 3.14159 / 2) * Velocity * Delta;
            Y += (float)Math.Sin(Angle - 3.14159 / 2) * Velocity * Delta;

            CollidePos = new Vector2(X + 31, Y + 31);
            if ((X < -20) || (Y < -20) || (X > 900) || (Y > 720))
            {
                MoveOut();
            }

            Collision();
        }
    }
  

    public override void OnCollision(Sprite sprite)
    {
        if (sprite is SpaceBall)
        {
            var SpaceBall = (SpaceBall)sprite;
            if (SpaceBall.ImageName == this.ImageName)
            {
                SpaceBall.CanCollision = false;
                SpaceBall.Visible = false;
                this.CanCollision = false;
                this.MoveOut();
                GameFunc.CreateSpark(SpaceBall.X,SpaceBall.Y,SpaceBall.ImageName.Substring(4,1));
            }
            else
            {
                GameFunc.CanCharge = true;
                CanCollision = false;
                Velocity = 10;
                Direction = (int)(40.76f * Angle);

                if (Direction > 128 && Direction < 210)
                {

                    TowardToAngle(Direction, 0, true);
                }
                else
                {
                    FlipXDirection();
                    FlipYDirection();
                    TowardToAngle(Direction, 0, true);
                }
            }
        }
    }

    public void SwitchColor()
    {
        switch (ImageName)
        {
            case "Ball0.png": ImageName = "Ball1.png"; return; break;
            case "Ball1.png": ImageName = "Ball2.png"; return; break;
            case "Ball2.png": ImageName = "Ball3.png"; return; break;
            case "Ball3.png": ImageName = "Ball0.png"; return; break;
        }
    }
}

public class Spark : PlayerSprite
{
    public Spark(Sprite Parent) : base(Parent)
    {
    }
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        Accelerate();
        UpdatePos(Delta);
        Alpha -= 4;
        SetColor(Alpha, Alpha, Alpha, Alpha);
        if (Alpha < 10)
            Dead();
    }
}

public class GameFunc
{
    public static Player Player;
    public static ShotBall ShotBall;
    public static bool CanCharge;
    public static MouseState MouseState;
    public static float NextBallInterval;
    public static int GameBallCount;
    public static bool NextBallReady;
    public static NURBSCurveEx LevelPath;
    public static void ChargeShot()
    {
        ShotBall = new ShotBall(EngineFunc.SpriteEngine);
        ShotBall.Init(EngineFunc.ImageLib, ShotBall.ImageName, 0, 0);
        CanCharge = false;
    }
    public static void CreateSpark(float PosX, float PosY, string Num)
    {
        var Random = new Random();
        for (int i = 0; i < 128; i++)
        {
            var Spark = new Spark(EngineFunc.SpriteEngine);
            Spark.SpriteSheetMode = SpriteSheetMode.NoneSingle;
            Spark.Init(EngineFunc.ImageLib, "Spark" + Num + ".png", PosX + -Random.Next(0, 31),-20+ PosY + Random.Next(0, 31), 200);
            Spark.BlendMode=MonoGame.SpriteEngine.BlendMode.AddtiveColor;
            Spark.Acceleration = 0.02f;
            Spark.MinSpeed = 1.8f;
            Spark.MaxSpeed =(float)-(0.4 + Random.Next(0, 3));
            Spark.Direction = i * 2;
        }
    }

    public static void CreateGame()
    {
        Player = new Player(EngineFunc.SpriteEngine);
        Player.Init(EngineFunc.ImageLib, "Cannon.png", 820, 600);
        LevelPath = new NURBSCurveEx();
        LevelPath.FittingCurveType = FittingCurveType.ConstantSpeed;
        LevelPath.LoadCurve("NURBS.txt");
        NextBallReady = true;
        CanCharge = true;
    }
    public static void UpdateGame()
    {
        MouseState = Mouse.GetState();
        if (MouseState.LeftButton == ButtonState.Pressed)
        {
            if (!ShotBall.Fired)
            {
                ShotBall.Fired = true;
                ShotBall.Z = 0;
            }
        }

        if (MouseState.RightButton == ButtonState.Pressed)
        {
            if (!ShotBall.Fired)
                ShotBall.SwitchColor();
        }
        NextBallInterval = 1.8f;
        var SpriteList = EngineFunc.SpriteEngine.SpriteList;
        for (int i = 0; i < SpriteList.Count; i++)
        {
            if ((SpriteList[i] is SpaceBall) && ((SpaceBall)SpriteList[i]).ID == GameBallCount - 1)
            {
                if (((SpaceBall)SpriteList[i]).Distance >= NextBallInterval)
                {
                    NextBallReady = true;
                }
            }
        }
        if (NextBallReady)
        {
            var SpaceBall = new SpaceBall(EngineFunc.SpriteEngine);
            SpaceBall.Init(EngineFunc.ImageLib, "Ball0.png", 0, 0);
            SpaceBall.ID = GameBallCount;
            Random Random = new Random();
            switch (Random.Next(0, 4))
            {
                case 0: SpaceBall.ImageName = "Ball0.png"; break;
                case 1: SpaceBall.ImageName = "Ball1.png"; break;
                case 2: SpaceBall.ImageName = "Ball2.png"; break;
                case 3: SpaceBall.ImageName = "Ball3.png"; break;
            }
            SpaceBall.MoveSpeed = 1.5f;
            SpaceBall.Path = LevelPath;
            SpaceBall.X = SpaceBall.Path.GetXY(0).X;
            SpaceBall.Y = SpaceBall.Path.GetXY(0).Y;
            SpaceBall.Offset.X = 33;
            SpaceBall.Offset.Y = -33;
            SpaceBall.CanCollision = true;
            SpaceBall.CollideMode = CollideMode.Circle;
            SpaceBall.CollideRadius = 31;
            NextBallReady = false;
            GameBallCount++;
        }
        if (CanCharge)
            ChargeShot();

    }
}