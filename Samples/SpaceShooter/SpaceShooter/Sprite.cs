using System;
using MonoGame.SpriteEngine;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using System.Text.RegularExpressions;
using Mouse = SpriteEngine.Mouse;
namespace SpaceShooter;

public enum EnemyType
{
    Ship, SquareShip, AnimShip, Mine
}
public class Bullet : AnimatedSprite
{
    public Bullet(Sprite Parent) : base(Parent)
    {
        SpriteSheetMode = SpriteSheetMode.NoneSingle;
        BlendMode = BlendMode.AddtiveColor;
        Z = 4000;
        CanCollision = true;
        CollideRadius = 12;
        DoCenter = true;
    }
    public int Counter;
    public float MoveSpeed = 6;
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        CollidePos = new Vector2(X + 20, Y + 20);
        TowardToAngle((int)(Angle * 40.74), MoveSpeed, true, Delta);
        Counter++;
        Collision();
        if (ImageName == "Explosion3.png" && AnimEnded())
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
    public float MoveSpeed;

    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        TowardToAngle((int)(Angle * 40.7), MoveSpeed, true, Delta);
        CollidePos = new Vector2(X + 24, Y + 38);
        Counter += 1;
        if (Counter > 100)
            Dead();
        if (AnimPos >= 11)
            Dead();
        Collision();
    }



    public override void OnCollision(Sprite sprite)
    {
        var Random = new Random();
        if (sprite is Asteroids)
        {
            CanCollision = false;
            BassSound.Play("Hit.wav");
            MoveSpeed = 0;
            SetPattern(64, 64);
            SetAnim("Explosions.png", 0, 12, 0.3f, false, false, true);
            var Asteroids = (Asteroids)sprite;

            Asteroids.SetColor(250, 0, 0);
            Asteroids.Life -= 1;
            if (Asteroids.Life <= 0)
            {
                BassSound.Play("Explode.wav");
                Asteroids.MoveSpeed = 0;
                for (int i = 0; i < 128; i++)
                {
                    var Explosion = new Explosion(EngineFunc.SpriteEngine);
                    Explosion.ImageLib = EngineFunc.ImageLib;
                    Explosion.ImageName = "Particle2.png";
                    Explosion.SetPattern(32, 32);
                    Explosion.SetSize(32, 32);
                    Explosion.BlendMode = BlendMode.AddtiveColor;
                    Explosion.X = Asteroids.X + -Random.Next(60);
                    Explosion.Y = Asteroids.Y - Random.Next(60);
                    Explosion.Z = 4850;
                    Explosion.ScaleX = 3;
                    Explosion.ScaleY = 3;
                    Explosion.Acceleration = 0.252f;
                    Explosion.MinSpeed = 1;
                    Explosion.MaxSpeed = -(0.31f + Random.Next(2)) + 2;
                    Explosion.Direction = i * 2;
                }
                GameFunc.CreateBonus("Money.png", (int)Asteroids.X, (int)Asteroids.Y);
                Asteroids.Dead();
            }
        }


        if (sprite is Enemy)
        {
            BassSound.Play("Hit.wav");
            CanCollision = false;
            MoveSpeed = 0;
            SetPattern(64, 64);
            SetAnim("Explosion3.png", 0, 12, 0.3f, false, false, true);
            var Enemy = (Enemy)sprite;
            if (AnimPos < 1)
            {
                Enemy.BlendMode = BlendMode.AddtiveColor;
                Enemy.Life -= 1;
            }
            if (Enemy.Life <= 0)
            {
                Enemy.MoveSpeed = 0;
                Enemy.RotateSpeed = 0;
                Enemy.DestAngle = 0;
                Enemy.LookAt = false;
                Enemy.BlendMode = BlendMode.AddtiveColor;
                Enemy.ScaleX = 2.5f;
                Enemy.ScaleY = 2.5f;
                Enemy.SpriteSheetMode = SpriteSheetMode.FixedSize;
                Enemy.SetPattern(64, 64);
                Enemy.SetAnim("Explosion2.png", 0, 16, 0.15f, false, false, true);
                GameFunc.CreateBonus("Bonus" + Random.Next(3).ToString() + ".png", (int)Enemy.X, (int)Enemy.Y);
            }
        }

        if (sprite is Fort)
        {
            BassSound.Play("Hit.wav");
            CanCollision = false;
            MoveSpeed = 0;
            SetPattern(64, 64);
            SetAnim("Explosion3.png", 0, 12, 0.3f, false, false, true);
            var Fort = (Fort)sprite;
            if (AnimPos < 3)
                Fort.SetColor(255, 0, 0);
            Fort.Life -= 1;
            if (Fort.Life <= 1)
            {
                Fort.BlendMode = BlendMode.AddtiveColor;
                Fort.SpriteSheetMode = SpriteSheetMode.FixedSize;
                Fort.ScaleX = 3;
                Fort.ScaleY = 3;
                Fort.SetPattern(64, 64);
                Fort.SetAnim("Explosion2.png", 0, 16, 0.15f, false, false, true);
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
        if ((X > Engine.Camera.X - 50) && (Y > Engine.Camera.Y - 50) && (X < Engine.Camera.X + 1124) && (Y < Engine.Camera.Y + 878))
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
                switch (Random.Next(100))
                {
                    case 40:
                    case 41:
                    case 42:
                    case 43:
                        DestAngle = Random.Next(256);
                        break;
                    case 51:
                    case 52:
                        DestAngle = GetAngle256((int)GameFunc.PlayerShip.X - (int)this.X, (int)GameFunc.PlayerShip.Y - (int)this.Y);
                        break;
                }
                RotateToAngle(DestAngle, RotateSpeed, MoveSpeed, Delta);
                break;

            case EnemyType.SquareShip:
                CollidePos = new Vector2(X + 30, Y + 30);
                switch (Random.Next(100))
                {
                    case 40:
                    case 41:
                    case 42:
                    case 43:
                    case 44:
                    case 45:
                        DestX = Random.Next(10000);
                        DestY = Random.Next(10000);
                        break;
                    case 51:
                    case 52:
                        DestX = (int)GameFunc.PlayerShip.X;
                        DestY = (int)GameFunc.PlayerShip.Y;
                        break;
                }
                CircleToPos(DestX, DestY, (int)GameFunc.PlayerShip.X, (int)GameFunc.PlayerShip.Y, RotateSpeed, MoveSpeed, LookAt, Delta);
                break;

            case EnemyType.AnimShip:
                CollidePos = new Vector2(X + 20, Y + 20);
                switch (Random.Next(100))
                {
                    case 40:
                    case 41:
                    case 42:
                    case 43:
                    case 44:
                    case 45:
                        DestX = Random.Next(8000);
                        DestY = Random.Next(6000);
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
                switch (Random.Next(300))
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
                Angle += RotateSpeed;
                TowardToPos(DestX, DestY, MoveSpeed, false, false, Delta);
                break;
        }
        // enemy shoot bullet
        if ((Type == EnemyType.Ship) || (Type == EnemyType.SquareShip))
        {
            if (InOffScreen())
            {
                if (Random.Next(100) == 50)
                {
                    Bullet = new Bullet(EngineFunc.SpriteEngine);
                    Bullet.ImageLib = EngineFunc.ImageLib;
                    Bullet.ImageName = "bulletr.png";
                    Bullet.SetSize(40, 40);
                    Bullet.X = this.X + 1;
                    Bullet.Y = this.Y;
                    Bullet.Angle = this.Angle;
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

        SetColor(255, 255, 255);
        X = PosX + (float)Math.Cos(Step / (30)) * Range - (float)(Math.Sin(Step / (20)) * Range);
        Y = PosY + (float)Math.Sin(Step / (30 + Seed)) * Range + (float)(Math.Cos(Step / (20)) * Range);
        Step += MoveSpeed * Delta;

        switch (ImageName)
        {
            case "Roids0.png": CollidePos = new Vector2(X + 32, Y + 32); break;
            case "Roids1.png": CollidePos = new Vector2(X + 30, Y + 30); break;
            case "Roids2.png": CollidePos = new Vector2(X + 34, Y + 34); Angle += 0.02f; break;
        }

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
        //base.DoMove(Delta);
        Accelerate();
        UpdatePos(1 * Delta);
        Alpha -= 2;
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
        Alpha -= 2;
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
    public float Life=16;
    public PlayerBullet Bullet;
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
            Angle = GetAngle256(Mouse.State.X - 512, Mouse.State.Y - 384) / 40.7f;
            Direction = GetAngle256(Mouse.State.X - 512, Mouse.State.Y - 384);
            if (Mouse.RightPressed())
            {
                DoAccelerate = true;
                DoDeccelerate = false;
            }
            else
            {
                DoAccelerate = false;
                DoDeccelerate = true;
            }
        }
        if ((AnimPos >= 32) && (ImageName == "Explode.png"))
        {
            ImageName = "PlayerShip.png";
            SpriteSheetMode = SpriteSheetMode.NoneSingle;
            BlendMode = BlendMode.Normal;
            ScaleX = 1.2f;
            ScaleY = 1.2f;
        }
        if (Ready)
        {
            ReadyTime++;
            if (ImageName == "PlayerShip.png")
                Alpha = 80;

        }
        if (ReadyTime == 300)
        {
            Alpha = 255;
            Ready = false;
            CanCollision = true;
        }

        Engine.Camera.X = X - 512;
        Engine.Camera.Y = Y - 384;

        if (Mouse.LeftClick)
        {
            if (ImageName == "PlayerShip.png")
            {
                BassSound.Play("Shoot.wav");
                Bullet = new PlayerBullet(EngineFunc.SpriteEngine);
                Bullet.ImageLib = EngineFunc.ImageLib;
                Bullet.ImageName = "bb.png";
                Bullet.SetPattern(24, 36);
                Bullet.SetSize(24, 36);
                Bullet.BlendMode = BlendMode.AddtiveColor;
                Bullet.DoCenter = true;
                Bullet.MoveSpeed = 9;
                Bullet.Angle = GameFunc.PlayerShip.Angle + 0.05f;
                Bullet.X = GameFunc.PlayerShip.X;
                Bullet.Y = GameFunc.PlayerShip.Y;
                Bullet.Z = 11000;
                Bullet.CanCollision = true;
                Bullet.CollideRadius = 10;
            }
        }
    }

    public override void OnCollision(Sprite sprite)
    {
        if (sprite is Bonus)
        {
            BassSound.Play("GetBonus.wav");
            var Bonus = (Bonus)sprite;
            switch (Bonus.ImageName)
            {
                case "Bonus0.png": GameFunc.Score += 100; break;
                case "Bonus1.png": GameFunc.Score += 200; break;
                case "Bonus2.png": GameFunc.Score += 300; break;
                case "Money.png": GameFunc.Score += 500; break;
            }
            var Random = new Random();
            if (Random.Next(2) == 0)
                GameFunc.CreateSpark("Star1.png", Bonus.X, Bonus.Y);
            else
                GameFunc.CreateSpark("Star2.png", Bonus.X, Bonus.Y);
            Bonus.Dead();
        }
        if (sprite is Bullet)
        {
            GameFunc.PlayerShip.Life -= 0.25f;
            var Bullet = (Bullet)sprite;
            Bullet.CanCollision = false;
            Bullet.MoveSpeed = 0;
            Bullet.SpriteSheetMode = SpriteSheetMode.FixedSize;
            Bullet.SetPattern(64, 64);
            Bullet.ScaleX = 1.5f;
            Bullet.ScaleY = 1.5f;
            Bullet.SetAnim("Explosion3.png", 0, 12, 0.3f, false, false, true);
            Bullet.Z = 10000;
        }
        if ((sprite is Asteroids) || (sprite is Enemy))
        {
            BassSound.Play("Explode.wav");
            Ready = true;
            ReadyTime = 0;
            GameFunc.PlayerShip.Life -= 0.25f;
            SpriteSheetMode = SpriteSheetMode.FixedSize;
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
        if (GameFunc.PlayerShip.Alpha == 80)
            Alpha = 50;
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
        SpriteSheetMode = SpriteSheetMode.NoneSingle;
        DoCenter = true;
        CanCollision = true;
        CollideRadius = 24;
        Life = 5;
    }
    public int Life;
    public Bullet Bullet;
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        SetColor(255, 255, 255);
        if (ImageName == "fort.png")
            LookAt((int)GameFunc.PlayerShip.X, (int)GameFunc.PlayerShip.Y);
        CollidePos = new Vector2(X + 22, Y + 36);
        if (AnimPos > 15)
            Dead();
        if (AnimPos >= 1 && ImageName == "Explosion2.png")
            CanCollision = false;
        var Random = new Random();
        if (Random.Next(150) == 50)
        {
            if ((X > Engine.Camera.X) && (Y > Engine.Camera.Y) && (X < Engine.Camera.X + 800) && (Y < Engine.Camera.Y + 600))
            {
                var Bullet = new Bullet(EngineFunc.SpriteEngine);
                Bullet.ImageLib = EngineFunc.ImageLib;
                Bullet.ImageName = "BulletS.png";
                Bullet.SetSize(40, 40);
                Bullet.BlendMode = BlendMode.AddtiveColor;
                Bullet.MoveSpeed = 5;
                Bullet.Z = 4000;
                Bullet.Counter = 0;
                Bullet.X = this.X + 5;
                Bullet.Y = this.Y;
                Bullet.Angle = this.Angle;
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
    public static int Counter;
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
            Enemy.X = Random.Next(8000) - 2500;
            Enemy.Y = Random.Next(8000) - 2500;
            Enemy.Z = 10000;
            Enemy.CanCollision = true;
            Enemy.MoveSpeed = 1 + (Random.Next(4) * 0.5f);
            Enemy.RotateSpeed = 0.5f + (Random.Next(4) * 0.4f);
            Enemy.DoCenter = true;
            Enemy.Life = 2;
            switch (Enemy.Type)
            {
                case EnemyType.Ship:
                    Enemy.ImageName = "Ship" + (Random.Next(2)).ToString() + ".png";
                    Enemy.SpriteSheetMode = SpriteSheetMode.NoneSingle;
                    Enemy.SetSize(128, 128);
                    Enemy.CollideRadius = 40;
                    Enemy.ScaleX = 0.7f;
                    Enemy.ScaleY = 0.8f;
                    break;
                case EnemyType.SquareShip:
                    Enemy.ImageName = "SquareShip" + Random.Next(2).ToString() + ".png";
                    Enemy.CollideRadius = 30;
                    Enemy.SpriteSheetMode = SpriteSheetMode.NoneSingle;
                    Enemy.LookAt = true;
                    if (Enemy.ImageName == "SquareShip0.png")
                        Enemy.SetSize(60, 62);
                    else
                        Enemy.SetSize(72, 62);
                    break;

                case EnemyType.AnimShip:
                    Enemy.ImageName = "AnimShip" + Random.Next(2).ToString() + ".png";
                    Enemy.CollideRadius = 25;

                    if (Enemy.ImageName == "AnimShip1.png")
                    {
                        Enemy.SetPattern(64, 64);
                        Enemy.SetSize(64, 64);
                        Enemy.SetAnim(Enemy.ImageName, 0, 8, 0.35f, true, false, true);
                    }
                    if (Enemy.ImageName == "AnimShip0.png")
                    {
                        Enemy.SetPattern(48, 62);
                        Enemy.SetSize(48, 62);
                        Enemy.SetAnim(Enemy.ImageName, 0, 4, 0.1f, true, false, true);
                    }
                    break;


                case EnemyType.Mine:
                    Enemy.ImageName = "Mine0.png";
                    Enemy.SpriteSheetMode = SpriteSheetMode.NoneSingle;
                    Enemy.SetSize(64, 64);
                    Enemy.CollideRadius = 16;
                    Enemy.RotateSpeed = 0.04f;
                    break;
            }
            Enemy.TempMoveSpeed = Enemy.MoveSpeed;
        }

        //create asteroids
        float[] RndF = { -0.15f, 0.15f };
        for (int i = 0; i < 500; i++)
        {
            var Asteroids = new Asteroids(EngineFunc.SpriteEngine);
            Asteroids.ImageLib = EngineFunc.ImageLib;
            Asteroids.ImageName = "Roids" + Random.Next(3).ToString() + ".png";
            Asteroids.PosX = Random.Next(8000) - 2500;
            Asteroids.PosY = Random.Next(8000) - 2500;
            Asteroids.Z = 4800;
            Asteroids.DoCenter = true;
            switch (Asteroids.ImageName)
            {
                case "Roids0.png": Asteroids.SetPattern(64, 64); Asteroids.AnimSpeed = 0.2f; Asteroids.CollideRadius = 32; break;
                case "Roids1.png": Asteroids.SetPattern(96, 96); Asteroids.AnimSpeed = 0.16f; Asteroids.CollideRadius = 48; break;
                case "Roids2.png": Asteroids.SetPattern(128, 128); Asteroids.AnimSpeed = 0.25f; Asteroids.CollideRadius = 50; break;
            }
            Asteroids.SetAnim(Asteroids.ImageName, 0, Asteroids.PatternCount, 0.15f, true, false, true);
            Asteroids.MoveSpeed = RndF[Random.Next(2)];
            Asteroids.Range = 150 + Random.Next(200);
            Asteroids.Step = Random.Next(1512);
            Asteroids.Seed = 50 + Random.Next(100);
            Asteroids.Life = 6;
            Asteroids.CanCollision = true;
            Asteroids.Width = Asteroids.PatternWidth;
            Asteroids.Height = Asteroids.PatternHeight;
            Asteroids.Angle = Random.Next(628) * 0.01f;
        }

        // create player's ship
        PlayerShip = new PlayerShip(EngineFunc.SpriteEngine);
        PlayerShip.ImageLib = EngineFunc.ImageLib;
        PlayerShip.ImageName = "PlayerShip.png";
        PlayerShip.SpriteSheetMode = SpriteSheetMode.NoneSingle;
        PlayerShip.SetSize(64, 64);
        PlayerShip.DoCenter = true;
        PlayerShip.ScaleX = 1.2f;
        PlayerShip.ScaleY = 1.2f;
        PlayerShip.Acceleration = 0.05f;
        PlayerShip.Decceleration = 0.02f;
        PlayerShip.MinSpeed = 0;
        PlayerShip.MaxSpeed = 3.5f;
        PlayerShip.Z = 5000;
        PlayerShip.CanCollision = true;
        PlayerShip.CollideRadius = 25;

        //create map
        string AllText = System.IO.File.ReadAllText("Map1.txt");
        string[] Section = AllText.Split('/');
        int Length = Section.Length;
        int X = 0, Y = 0, Z = 0;
        string ImageName = null;

        for (int i = Length - 2; i > 0; i--)
        {
            var Str = Section[i].Split(',');
            X = int.Parse(Regex.Replace(Str[0], @"\D", ""));
            Y = int.Parse(Regex.Replace(Str[1], @"\D", ""));
            Z = int.Parse(Regex.Replace(Str[2], @"\D", ""));
            ImageName = Regex.Replace(Str[3], "ImageName=", "").Trim();
            if (ImageName.Substring(0, 4) == "Tile")
            {
                var Tile = new Sprite(EngineFunc.SpriteEngine);
                Tile.ImageLib = EngineFunc.ImageLib;
                Tile.ImageName = ImageName.ToLower();
                Tile.Width = Tile.ImageWidth;
                Tile.Height = Tile.ImageHeight;
                Tile.X = X - 2000;
                Tile.Y = Y - 2000;
                Tile.Z = Z;
            }
            if (ImageName.Substring(0, 4) == "Fort")
            {
                var Fort = new Fort(EngineFunc.SpriteEngine);
                Fort.ImageLib = EngineFunc.ImageLib;
                Fort.ImageName = ImageName.ToLower();
                Fort.Width = Fort.ImageWidth;
                Fort.Height = Fort.ImageHeight;
                Fort.X = X - 2000 + 22;
                Fort.Y = Y - 2000 + 40;
                Fort.Z = Z;
            }
        }

        // create planet
        for (int i = 0; i < 100; i++)
        {
            var Planet = new SpriteEx(SpaceLayer,
                EngineFunc.ImageLib, "planet" + Random.Next(4).ToString() + ".png",
                0, 0,
                Random.Next(0, 25) * 300 - 2500, Random.Next(25) * 300 - 2500, 100);
            Planet.SetSize(Planet.ImageWidth, Planet.ImageHeight);
        }
        // create a huge endless space
        var Back = new BackgroundSprite(SpaceLayer);
        Back.ImageLib = EngineFunc.ImageLib;
        Back.ImageName = "Space.jpg";
        Back.SetPattern(512, 512);
        Back.SetSize(512, 512);
        Back.Tiled = true;
        Back.TileMode = TileMode.Full;
        Back.Z = 1;

        // create mist layer1
        var mistLayer1 = new BackgroundSprite(MistLayer1);
        mistLayer1.ImageLib = EngineFunc.ImageLib;
        mistLayer1.ImageName = "Mist.png";
        mistLayer1.SetPattern(1024, 1024);
        mistLayer1.SetSize(1024, 1024);
        mistLayer1.Tiled = true;
        mistLayer1.TileMode = TileMode.Full;
        mistLayer1.BlendMode = BlendMode.AddtiveColor;
        mistLayer1.Z = 2;
        // create mist layer2
        var mistLayer2 = new BackgroundSprite(MistLayer1);
        mistLayer2.ImageLib = EngineFunc.ImageLib;
        mistLayer2.ImageName = "Mist.png";
        mistLayer2.SetPattern(1024, 1024);
        mistLayer2.SetSize(1024, 1024);
        mistLayer2.Tiled = true;
        mistLayer2.TileMode = TileMode.Full;
        mistLayer2.X = 200;
        mistLayer2.Y = 200;
        mistLayer2.BlendMode = BlendMode.AddtiveColor;
        mistLayer2.Z = 3;
        BassSound.Init();
        BassSound.LoadSounds("Sounds/");
        MidiSound.Play("Sounds/Music1.mid");
    }

    public static void UpdataGame(float Delta)
    {
        Mouse.GetState();
        if(GameFunc.PlayerShip.Life<=0) 
            return;

        Counter += 1;
        if (Counter % 16000 == 0)
            MidiSound.RePlay();
        if (Counter % 4 == 0)
        {
            if (PlayerShip.ImageName == "PlayerShip.png")
            {
                var Tail = new Tail(EngineFunc.SpriteEngine);
                Tail.ImageLib = EngineFunc.ImageLib;
                Tail.ImageName = "tail.png";
                Tail.SetPattern(64, 64);
                Tail.SetSize(64, 64);
                Tail.BlendMode = BlendMode.AddtiveColor;
                Tail.ScaleX = 0.1f;
                Tail.ScaleY = 0.1f;
                Tail.X = 510 + Tail.Engine.Camera.X;
                Tail.Y = 382 + Tail.Engine.Camera.Y;
                Tail.Z = 4000;
                Tail.DoCenter = true;
                Tail.Acceleration = 2.51f;
                Tail.MinSpeed = 1;
                if (PlayerShip.Speed < 1)
                    Tail.MaxSpeed = 2;
                else
                    Tail.MaxSpeed = 0.5f;
                Tail.Direction = -128 + PlayerShip.Direction;
            }
        }

        EngineFunc.SpriteEngine.Dead();
        EngineFunc.SpriteEngine.Move(Delta);
        SpaceLayer.Camera.X = EngineFunc.SpriteEngine.Camera.X * 0.71f * Delta;
        SpaceLayer.Camera.Y = EngineFunc.SpriteEngine.Camera.Y * 0.71f * Delta;
        MistLayer1.Camera.X = EngineFunc.SpriteEngine.Camera.X * 1.1f * Delta;
        MistLayer1.Camera.Y = EngineFunc.SpriteEngine.Camera.Y * 1.1f * Delta;
        MistLayer2.Camera.X = EngineFunc.SpriteEngine.Camera.X * 1.3f * Delta;
        MistLayer2.Camera.Y = EngineFunc.SpriteEngine.Camera.Y * 1.3f * Delta;
    }

    public static void CreateBonus(string BonusName, int PosX, int PosY)
    {
        var Random = new Random();
        if (Random.Next(2) == 1)
        {
            var Bonus = new Bonus(EngineFunc.SpriteEngine);
            Bonus.ImageLib = EngineFunc.ImageLib;
            Bonus.ImageName = BonusName;
            Bonus.Width = Bonus.PatternWidth;
            Bonus.Height = Bonus.PatternHeight;
            Bonus.SetPattern(32, 32);
            Bonus.MoveSpeed = 0.251f;
            Bonus.PX = PosX - 50;
            Bonus.PY = PosY - 100;
            Bonus.Z = 12000;
            Bonus.ScaleX = 1.5f;
            Bonus.ScaleY = 1.5f;
            Bonus.DoCenter = true;
            Bonus.CanCollision = true;
            Bonus.CollideRadius = 24;
            if (BonusName == "Money.png")
                Bonus.SetAnim(Bonus.ImageName, 0, Bonus.PatternCount, 0.25f, true, false, true);
            else
                Bonus.SetAnim(Bonus.ImageName, 0, Bonus.PatternCount, 0.5f, true, false, true);
        }
    }

    public static void CreateSpark(string ImageName, float PosX, float PosY)
    {
        var Random = new Random();
        for (int i = 0; i < 128; i++)
        {
            var Spark = new Spark(EngineFunc.SpriteEngine);
            Spark.ImageLib = EngineFunc.ImageLib;
            Spark.ImageName = ImageName;
            Spark.BlendMode = BlendMode.AddtiveColor;
            Spark.SetPattern(32, 32);
            Spark.DoMove(1);
            Spark.PatternIndex = Random.Next(7);
            Spark.SetSize(32, 32);
            Spark.X = PosX + -Random.Next(30);
            Spark.Y = PosY + Random.Next(30);
            Spark.Z = 12000;
            Spark.ScaleX = 1.2f;
            Spark.ScaleY = 1.2f;
            Spark.Acceleration = 0.02f;
            Spark.MinSpeed = 1.8f;
            Spark.MaxSpeed = (float)-(0.4 + Random.Next(3));
            Spark.Direction = i * 2;
        }
    }

}