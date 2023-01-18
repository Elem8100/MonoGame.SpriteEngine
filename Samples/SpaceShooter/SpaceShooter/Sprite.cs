using System;
using MonoGame.SpriteEngine;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Input = Microsoft.Xna.Framework.Input.Keys;
using SharpDX.Direct3D11;
using System.Diagnostics.Metrics;
using System.Media;

namespace SpaceShooter;

public enum EnemyType
{
    Ship = 0, SquareShip = 1, AnimShip = 2, Mine = 3
}
public class Bullet : AnimatedSprite
{
    public Bullet(Sprite Parent) : base(Parent)
    {
        ImageLib = EngineFunc.ImageLib;
        SetPattern(40, 40);
        BlendMode = BlendMode.AddtiveColor;
        Z = 4000;
        CanCollision = true;
        CollideRadius = 12;

    }
    public float DestAngle;
    public int Counter;
    public float MoveSpeed;
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        CollidePos = new Vector2(X + 20, Y + 20);
        // TowardToPos()
        Counter++;
        Collision();
        if ((AnimPos >= 15) && (ImageName == "Explosion3"))
            Dead();
        if (Counter > 250)
            Dead();
    }

    public override void OnCollision(Sprite sprite)
    {

    }
}

public class PlayerBullet : AnimatedSprite
{
    public PlayerBullet(Sprite Parent) : base(Parent)
    {
        SetPattern(40, 40);
        BlendMode = BlendMode.AddtiveColor;
        Z = 4000;
        CanCollision = true;
        CollideRadius = 12;

    }
    private int Counter;
    float MoveSpeed;

    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        // TowardToPos()
        CollidePos = new Vector2(X + 24, Y + 38);
        if (Counter > 180)
            Dead();
        if (AnimPos >= 11)
            Dead();
        Collision();
    }

    public override void OnCollision(Sprite sprite)
    {
        if (sprite is Asteroids)
        {
            //PlaySound('Hit.wav');
            CanCollision = false;
            MoveSpeed = 0;
            SetPattern(64, 64);
            SetAnim("Explosions", 0, 12, 0.3f, false, false, true);
            var Asteroids = (Asteroids)sprite;
            if (AnimPos < 1)
                Asteroids.BlendMode = BlendMode.Subtractive;
            Asteroids.Life -= 1;
            if (Asteroids.Left < 1)
            {
                // PlaySound('Explode.wav');
                Asteroids.MoveSpeed = 0;
                var Random = new Random();
                for (int i = 0; i < 128; i++)
                {
                    Explosion Explosion = new(EngineFunc.SpriteEngine)
                    {
                        ImageLib = EngineFunc.ImageLib,
                        ImageName = "Particles",
                        //SetPattern(32, 32),
                        Width = PatternWidth,
                        Height = PatternHeight,
                        BlendMode = BlendMode.AddtiveColor,
                        X = Asteroids.X + -Random.Next(0, 60),
                        Y = Asteroids.Y - Random.Next(0, 60),
                        Z = 4850,
                        PatternIndex = 7,
                        ScaleX = 3,
                        ScaleY = 3,
                        //Red= 255;
                        // Green= 100;
                        // Blue= 101;
                        Acceleration = 0.0252f,
                        MinSpeed = 1,
                        MaxSpeed = -(0.31f + Random.Next(0, 2)),
                        Direction = i * 2

                    }; Explosion.SetPattern(32, 32);



                }

            }

        }

    }

}

public class Enemy : AnimatedSprite
{
    public Enemy(Sprite Parent) : base(Parent)
    {
    }

    public float MoveSpeed;
    public float TempMoveSpeed;
    public float RotateSpeed;
    public int DestX, DestY;
    public int DestAngle;
    public bool LookAt;
    public EnemyType Type;
    public int Life;
    public Bullet Bullet;

    private bool InOffScreen()
    {
        if ((X > Engine.Camera.X - 50) && (Y > Engine.Camera.Y - 50) && (X < Engine.Camera.X + 1124) && (Y < Engine.Camera.Y + 778))
            return true;
        else
            return false;
    }
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        if ((Life >= 1) && (ImageName != "Explosion2.png"))
            BlendMode = BlendMode.Normal;
        if ((InOffScreen()) && (ImageName != "Explosion2.png"))
            MoveSpeed = TempMoveSpeed;
        if ((Life <= 0) || (!InOffScreen()))
            MoveSpeed = 0;
        if (AnimPos >= 15)
            Dead();
        if ((AnimPos >= 1) && (ImageName == "Explosion2.png"))
            CanCollision = false;
        var Random = new Random();
        switch (Type)
        {
            case EnemyType.Ship:
                CollidePos = new Vector2(X + 64, Y + 64);
                switch (Random.Next(0, 100))
                {
                    case 40:
                    case 41:
                    case 42:
                    case 43:
                        DestAngle = Random.Next(0, 256);
                        break;
                    case 51:
                    case 52:
                        DestAngle = GetAngle256((int)GameFunc.PlayerShip.X - (int)this.X, (int)GameFunc.PlayerShip.Y - (int)this.Y);
                        break;

                }
                RotateToAngle(DestAngle, RotateSpeed, MoveSpeed);
                break;
            case EnemyType.SquareShip:
                CollidePos = new Vector2(X + 30, Y + 30);
                switch (Random.Next(0, 100))
                {
                    case 40:
                    case 41:
                    case 42:
                    case 43:
                    case 44:
                    case 45:
                        DestX = Random.Next(0, 10000);
                        DestY = Random.Next(0, 10000);
                        break;
                    case 51:
                    case 52:
                        DestX = (int)GameFunc.PlayerShip.X;
                        DestY = (int)GameFunc.PlayerShip.Y;
                        break;

                }
                CircleToPos(DestX, DestY, (int)GameFunc.PlayerShip.X, (int)GameFunc.PlayerShip.Y, RotateSpeed, MoveSpeed, LookAt);
                break;
            case EnemyType.AnimShip:
                CollidePos = new Vector2(X + 20, Y + 20);
                switch (Random.Next(0, 100))
                {
                    case 40:
                    case 41:
                    case 42:
                    case 43:
                    case 44:
                    case 45:
                        DestX = Random.Next(0, 10000);
                        DestY = Random.Next(0, 10000);
                        break;
                    case 51:
                    case 52:
                    case 53:
                    case 54:
                        DestX = (int)GameFunc.PlayerShip.X;
                        DestY = (int)GameFunc.PlayerShip.Y;
                        break;
                }
                RotateToPos(DestX, DestY, RotateSpeed, MoveSpeed, Delta);
                break;

            case EnemyType.Mine:
                CollidePos = new Vector2(X + 32, Y + 32);
                switch (Random.Next(0, 300))
                {
                    case 150:
                        DestX = (int)GameFunc.PlayerShip.X;
                        DestY = (int)GameFunc.PlayerShip.Y;
                        break;
                    case 200:
                    case 201:
                    case 202:
                        DestX = Random.Next(0, 10000);
                        DestY = Random.Next(0, 10000);
                        break;
                }
                TowardToPos(DestX, DestY, MoveSpeed, false, false, Delta);
                break;
        }
        // enemy shoot bullet
        if ((Type == EnemyType.Ship) || (Type == EnemyType.SquareShip))
        {
            if (InOffScreen())
            {
                if (Random.Next(0, 100) == 50)
                {
                    Bullet = new Bullet(EngineFunc.SpriteEngine);
                    {

                        ImageLib = EngineFunc.ImageLib;
                        ImageName = "bulletr.png";
                        MoveSpeed = 5;
                        X = this.X + 1;
                        Y = this.Y;
                        DestAngle = (int)Angle * 40;
                    };
                }
            }
        }


    }
}

public class Asteroids : AnimatedSprite
{
    public Asteroids(Sprite Parent) : base(Parent)
    {
    }
    public int Life;
    public float Step;
    public float Range;
    public float MoveSpeed;
    public int Seed;
    public int PosX, PosY;

    public override void DoMove(float Delta)
    {

        base.DoMove(Delta);
        X = PosX + (float)Math.Cos(Step / (30)) * Range - (float)(Math.Sin(Step / (20)) * Range);
        Y = PosY + (float)Math.Sin(Step / (30 + Seed)) * Range + (float)(Math.Cos(Step / (20)) * Range);
        Step += MoveSpeed * Delta;

    }
}


public class Bonus : AnimatedSprite
{
    public Bonus(Sprite Parent) : base(Parent)
    {
    }
    public float PX, PY;
    public float Step;
    public float MoveSpeed;
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        CollidePos = new Vector2(X + 24, Y + 24);
        X = PX + (float)Math.Cos(Step / (30)) * 60 - (float)(Math.Sin(Step / (20)) * 150);
        Y = PY + (float)Math.Sin(Step / (90)) * 130 + (float)(Math.Cos(Step / (20)) * 110);
        Step += MoveSpeed * Delta;
    }


}
public class Explosion : PlayerSprite
{
    public Explosion(Sprite Parent) : base(Parent)
    {
    }

    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        Accelerate();
        UpdatePos(1 * Delta);
        Alpha -= 1;
        if (Alpha < 2)
            Dead();
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
        UpdatePos(1 * Delta);
        Alpha -= 1;
        if (Alpha < 2)
            Dead();
    }
}
public class PlayerShip : PlayerSprite
{
    public PlayerShip(Sprite Parent) : base(Parent)
    {
    }

    bool DoAccelerate;
    bool DoDeccelerate;
    float Life;
    PlayerBullet Bullet;
    bool Ready;
    int ReadyTime;

    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        CollidePos = new Vector2(X + 20, Y + 20);
        Collision();
        if (DoAccelerate)
            Accelerate();
        if (DoDeccelerate)
            Deccelerate();
        if (ImageName == "PlayerShip.png")
        {
            UpdatePos(1 * Delta);
            LookAt(GameFunc.MouseState.X - 512, GameFunc.MouseState.Y - 384);
            //Angle:= Angle256(Trunc(MainForm.CursorX) - 512, Trunc(MainForm.CursorY) - 384) * 0.025;
            // Direction:= Trunc(Angle256(MainForm.CursorX - 512, MainForm.CursorY - 384));
        }
        if ((AnimPos >= 32) && (ImageName == "Explode.png"))
        {
            ImageName = "PlayerShip.png";
            BlendMode = BlendMode.Normal;
            ScaleX = 1.2f;
            ScaleY = 1.2f;
        }
        if (Ready)
            ReadyTime++;
        if (ReadyTime == 350)
        {
            Ready = false;
            CanCollision = true;
        }
        Engine.Camera.X = X - 512;
        Engine.Camera.Y = Y - 384;
    }

    public override void OnCollision(Sprite sprite)
    {
        if (sprite is Bonus)
        {
            var Bonus = (Bonus)sprite;
            switch (Bonus.ImageName)
            {
                case "Bonus0.png": GameFunc.Score += 100; break;
                case "Bonus1.png": GameFunc.Score += 200; break;
                case "Bonus2.png": GameFunc.Score += 300; break;
                case "Money.png": GameFunc.Score += 500; break;
            }
            GameFunc.CreateSpark(Bonus.X, Bonus.Y);
            Bonus.Dead();
        }
        if (sprite is Bullet)
        {
            GameFunc.PlayerShip.Life -= 0.25f;
            this.SetColor(255, 0, 0);
            var Bullet = (Bullet)sprite;
            Bullet.CanCollision = false;
            Bullet.MoveSpeed = 0;
            Bullet.SetPattern(64, 64);
            Bullet.SetAnim("Explosion3.png", 0, 12, 0.3f, false, false, true);
            Bullet.Z = 10000;
        }
        if ((sprite is Asteroids) || (sprite is Enemy))
        {
            Ready = true;
            ReadyTime = 0;
            GameFunc.PlayerShip.Life -= 0.25f;
            AnimPos = 0;
            SetPattern(64, 64);
            SetAnim("Explode.png", 0, 40, 0.25f, false, false, true);
            CanCollision = false;
            BlendMode = BlendMode.AddtiveColor;
            ScaleX = 1.5f;
            ScaleY = 1.5f;
        }

    }

}

public class Tail : PlayerSprite
{
    public Tail(Sprite Parent) : base(Parent)
    {
    }
    private int Counter;
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        Alpha -= 6;
        if (GameFunc.PlayerShip.Speed < 1.1f)
        {
            ScaleX += 0.01f;
            ScaleY += 0.01f;
        }
        else
        {
            ScaleX += 0.025f;
            ScaleY += 0.025f;
        }
        Angle += 0.125f;
        UpdatePos(1 * Delta);
        Accelerate();
        Counter += 1;
        if (Counter > 25)
            Dead();

    }
}

public class Fort : AnimatedSprite
{
    public Fort(Sprite Parent) : base(Parent)
    {
    }

    public int Life;
    public Bullet Bullet;


    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        SetColor(255, 255, 255);
        if (ImageName == "fort")
            LookAt((int)GameFunc.PlayerShip.X, (int)GameFunc.PlayerShip.Y);
        CollidePos = new Vector2(X + 22, Y + 36);
        if (AnimPos > 15)
            Dead();
        if (AnimPos >= 1 && ImageName == "Explosion2.png")
            CanCollision = false;
        var Random = new Random();
        if (Random.Next(0, 150) == 50)
        {
            if ((X > Engine.Camera.X) && (Y > Engine.Camera.Y) && (X < Engine.Camera.X + 800) && (Y < Engine.Camera.Y + 600))
            {
                var Bullet = new Bullet(EngineFunc.SpriteEngine);
                Bullet.ImageLib = EngineFunc.ImageLib;
                Bullet.ImageName = "BulletS.png";
                Bullet.SetSize(40, 40);

                Bullet.BlendMode = BlendMode.AddtiveColor;
                Bullet.MoveSpeed = 4;
                Bullet.Z = 4000;
                Bullet.Counter = 0;
                Bullet.X = this.X + 5;
                Bullet.Y = this.Y;
                Bullet.DestAngle = Angle * 40;


            }

        }

    }
}


public class GameFunc
{
    public static MouseState MouseState;
    public static int Score;
    public static PlayerShip PlayerShip;
    public static MonoSpriteEngine SpaceLayer;
    public static MonoSpriteEngine MistLayer1;
    public static MonoSpriteEngine MistLayer2;

    public static void CreateGame()
    {
        SpaceLayer = new MonoSpriteEngine(null);
        SpaceLayer.Canvas = EngineFunc.Canvas;
        MistLayer1 = new MonoSpriteEngine(null);
        MistLayer1.Canvas = EngineFunc.Canvas;
        MistLayer2 = new MonoSpriteEngine(null);
        MistLayer2.Canvas = EngineFunc.Canvas;
        //create enemy
        var Random = new Random();
        for (int i = 0; i < 400; i++)
        {
            var Enemy = new Enemy(EngineFunc.SpriteEngine);
            Enemy.ImageLib = EngineFunc.ImageLib;
            Enemy.Type = (EnemyType)Random.Next(Enum.GetNames(typeof(EnemyType)).Length);
            Enemy.X = Random.Next(0, 8000) - 2500;
            Enemy.Y = Random.Next(0, 8000) - 2500;
            Enemy.Z = 10000;
            Enemy.CanCollision = true;
            Enemy.MoveSpeed = 1 + (Random.Next(0, 4) * 0.5f);
            Enemy.RotateSpeed = 0.5f + (Random.Next(0, 4) * 0.4f);
            Enemy.Life = 4;
            switch (Enemy.Type)
            {
                case EnemyType.Ship:
                    Enemy.ImageName = "Ship" + (Random.Next(0, 2)).ToString() + ".png";
                    Enemy.SetPattern(128, 128);
                    Enemy.CollideRadius = 40;
                    Enemy.ScaleX = 0.7f;
                    Enemy.ScaleY = 0.8f;
                    break;
                case EnemyType.SquareShip:
                    Enemy.ImageName = "SquareShip" + Random.Next(0, 2).ToString() + ".png";
                    Enemy.CollideRadius = 30;
                    Enemy.LookAt = true;
                    if (Enemy.ImageName == "SquareShip0.png")
                        Enemy.SetPattern(60, 62);
                    else
                        Enemy.SetPattern(72, 62);
                    break;

                case EnemyType.AnimShip:
                    Enemy.ImageName = "AnimShip" + Random.Next(0, 2).ToString() + ".png";
                    Enemy.CollideRadius = 25;
                    if (Enemy.ImageName == "AnimShip1.png")
                    {
                        Enemy.SetPattern(64, 64);
                        Enemy.SetAnim(Enemy.ImageName, 0, 8, 0.2f, true, false, true);
                    }
                    if (Enemy.ImageName == "AnimShip0.png")
                    {
                        Enemy.SetPattern(48, 62);
                        Enemy.SetAnim(Enemy.ImageName, 0, 4, 0.08f, true, false, true);
                    }
                    break;
                case EnemyType.Mine:
                    Enemy.ImageName = "Mine0";
                    Enemy.SetPattern(64, 64);
                    Enemy.CollideRadius = 16;
                    Enemy.RotateSpeed = 0.04f;
                    break;


            }
            Enemy.TempMoveSpeed = Enemy.MoveSpeed;
            Enemy.Width = Enemy.PatternWidth;
            Enemy.Height = Enemy.PatternHeight;
        }
        //create asteroids
        float[] RndF = { -0.15f, 0.15f };
        for (int i = 0; i < 500; i++)
        {
            var Asteroids = new Asteroids(EngineFunc.SpriteEngine);
            Asteroids.ImageLib = EngineFunc.ImageLib;
            Asteroids.ImageName = "Roids" + Random.Next(0, 3).ToString() + ".png";
            Asteroids.PosX = Random.Next(0, 8000) - 2500;
            Asteroids.PosY = Random.Next(0, 8000) - 2500;
            Asteroids.Z = 4800;
            Asteroids.DoCenter = true;
            switch (Asteroids.ImageName)
            {
                case "Roids0.png": Asteroids.SetPattern(64, 64); Asteroids.AnimSpeed = 0.2f; Asteroids.CollideRadius = 32; break;
                case "Roids1.png": Asteroids.SetPattern(96, 96); Asteroids.AnimSpeed = 0.16f; Asteroids.CollideRadius = 48; break;
                case "Roids2.png": Asteroids.SetPattern(128, 128); Asteroids.AnimSpeed = 0.25f; Asteroids.CollideRadius = 50; break;
            }
            Asteroids.SetAnim(Asteroids.ImageName, 0, Asteroids.PatternCount, 0.15f, true, false, true);
            Asteroids.MoveSpeed = RndF[Random.Next(0, 2)];
            Asteroids.Range = 150 + Random.Next(0, 200);
            Asteroids.Step = Random.Next(0, 1512);
            Asteroids.Seed = 50 + Random.Next(0, 100);
            Asteroids.Life = 6;
            Asteroids.CanCollision = true;
            Asteroids.Width = Asteroids.PatternWidth;
            Asteroids.Height = Asteroids.PatternHeight;
        }
        // create player's ship
        PlayerShip = new PlayerShip(EngineFunc.SpriteEngine);
        PlayerShip.ImageLib = EngineFunc.ImageLib;
        PlayerShip.SetPattern(64, 64);
        PlayerShip.SetSize(64, 64);
        PlayerShip.DoCenter = true;
        PlayerShip.ScaleX = 1.2f;
        PlayerShip.ScaleY = 1.2f;
        PlayerShip.Acceleration = 0.02f;
        PlayerShip.Decceleration = 0.02f;
        PlayerShip.MinSpeed = 0;
        PlayerShip.MaxSpeed = 1.5f;
        PlayerShip.Z = 5000;
        PlayerShip.CanCollision = true;
        PlayerShip.CollideRadius = 25;


        // create planet
        for (int i = 0; i < 100; i++)
        {
            var Planet = new SpriteEx(SpaceLayer,
                EngineFunc.ImageLib, "planet" + Random.Next(0, 4).ToString() + ".png",
                0, 0,
                Random.Next(0, 25) * 300 - 2500, Random.Next(0, 25) * 300 - 2500, 100);
            Planet.SetSize(Planet.ImageWidth, Planet.ImageHeight);
        }
        // create a huge endless space
        var Back = new BackgroundSprite(SpaceLayer);
        Back.ImageLib = EngineFunc.ImageLib;
        Back.ImageName = "space.png";
        Back.SetPattern(512, 512);
        Back.SetSize(512, 512);
        Back.Tiled = true;
        Back.TileMode = TileMode.Full;
        // create mist layer1
        var mistLayer1 = new BackgroundSprite(MistLayer1);
        mistLayer1.ImageLib = EngineFunc.ImageLib;
        mistLayer1.ImageName = "Mist.png"
        mistLayer1.SetPattern(1024,1024);
        mistLayer1.SetSize(1024, 1024);





    }


    public static void CreateBonus(string BonusName, int PosX, int PosY)
    {
        var Random = new Random();
        if (Random.Next(0, 3) == 1 || Random.Next(0, 3) == 2)
        {
            var Bonus = new Bonus(EngineFunc.SpriteEngine);
            Bonus.ImageLib = EngineFunc.ImageLib;
            Bonus.ImageName = BonusName;
            Bonus.Width = Bonus.PatternWidth;
            Bonus.Height = Bonus.PatternHeight;
            Bonus.MoveSpeed = 0.251f;
            Bonus.PX = PosX - 50;
            Bonus.PY = PosY - 100;
            Bonus.Z = 12000;
            Bonus.ScaleX = 1.5f;
            Bonus.ScaleY = 1.5f;
            Bonus.DoCenter = true;
            Bonus.CanCollision = true;
            Bonus.CollideRadius = 24;
            Bonus.SetAnim(Bonus.ImageName, 0, Bonus.PatternCount, 0.25f, true, false, true);
        }
    }

    public static void CreateSpark(float PosX, float PosY)
    {
        var Random = new Random();
        for (int i = 0; i < 128; i++)
        {
            var Spark = new Spark(EngineFunc.SpriteEngine);
            Spark.ImageLib = EngineFunc.ImageLib;
            Spark.ImageName = "Particles" + Random.Next(0, 2).ToString() + ".png";
            Spark.BlendMode = BlendMode.AddtiveColor;
            Spark.SetPattern(32, 32);
            Spark.SetSize(32, 32);
            Spark.PatternIndex = Random.Next(0, 7);
            Spark.X = PosX + -Random.Next(0, 30);
            Spark.Y = PosY + Random.Next(0, 30);
            Spark.Z = 12000;
            Spark.ScaleX = 1.2f;
            Spark.ScaleY = 1.2f;
            Spark.Acceleration = 0.02f;
            Spark.MinSpeed = 0.8f;
            Spark.MaxSpeed = (float)-(0.4 + Random.Next(0, 2));
            Spark.Direction = i * 2;
        }
    }



}