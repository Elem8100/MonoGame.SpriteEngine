using MonoGame.SpriteEngine;
using System.IO;
using System.Text.RegularExpressions;
using Input = Microsoft.Xna.Framework.Input.Keys;
using Keyboard = SpriteEngine.Keyboard;
using Microsoft.Xna.Framework.Input;
using System;


namespace Platformer;
public enum State
{
    StandLeft, StandRight, WalkLeft, WalkRight, Die
}

public class Player : JumperSprite
{
    public Player(Sprite Parent) : base(Parent)
    {
        CollideMode = CollideMode.Rect;
        CanCollision = true;
        SetPattern(110, 118);
        JumpSpeed = 1f;
        JumpHeight = 7.5f;
        MaxFallSpeed = 8;
        JumpState = JumpState.jsJumping;
        State = State.StandRight;
    }

    State State;
    public static int LeftEdge, RightEdge;
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        if (Y > 1200)
        {
            X = 100;
            Y = 300;
            JumpState = JumpState.jsFalling;
            CanCollision = true;
            State = State.StandRight;
        }

        SetCollideRect(45, 45, 60, 110);
        if ((this.Right + 3 < LeftEdge) || (this.Left > RightEdge + 1))
        {
            if (JumpState != JumpState.jsJumping)
                JumpState = JumpState.jsFalling;
        }
        Keyboard.GetState();
        if (Keyboard.KeyDown(Input.Right) && State != State.Die)
        {
            State = State.WalkRight;
            X += 3f*Delta;
            switch (JumpState)
            {
                case JumpState.jsNone: SetAnim("Walk.png", 0, 12, 0.4f, true, false, true); break;
                case JumpState.jsJumping: SetAnim("Jump.png", 0, 3, 0.06f, false, false, true); break;
                case JumpState.jsFalling: SetAnim("Jump.png", 2, 2, 0, false, false, true); break;
            }
        }

        if (Keyboard.KeyDown(Input.Left) && State != State.Die)
        {
            State = State.WalkLeft;
            X -= 3f*Delta;
            switch (JumpState)
            {
                case JumpState.jsNone: SetAnim("Walk.png", 0, 12, 0.4f, true, true, true); break;
                case JumpState.jsJumping: SetAnim("Jump.png", 0, 3, 0.06f, false, true, true); break;
                case JumpState.jsFalling: SetAnim("Jump.png", 2, 2, 0, false, true, true); break;
            }
        }

        if (Keyboard.KeyUp(Input.Right) && State != State.Die)
        {
            State = State.StandRight;
            if (JumpState == JumpState.jsNone)
                SetAnim("Idle.png", 0, 12, 0.25f, true, false, true);
        }
        if (Keyboard.KeyUp(Input.Left) && State != State.Die)
        {
            State = State.StandLeft;
            if (JumpState == JumpState.jsNone)
                SetAnim("Idle.png", 0, 12, 0.25f, true, true, true);
        }

        if (JumpState == JumpState.jsNone && State != State.Die)
        {
            if ((Keyboard.KeyDown(Input.LeftControl)) || (Keyboard.KeyDown(Input.Space)))
            {
                DoJump = true;
                AnimPos = 0;
                PatternIndex = 0;
                switch (State)
                {
                    case State.StandRight: SetAnim("Jump.png", 0, 3, 0.06f, true, false, true); break;
                    case State.StandLeft: SetAnim("Jump.png", 0, 3, 0.06f, true, true, true); break;
                }
            }
        }
        if (Engine.Camera.X < (X - 350))
            Engine.Camera.X = X - 350;

        if (Engine.Camera.X > (X - 345))
            Engine.Camera.X = X - 345;

       
        Collision();
        Engine.Camera.X = X - 512;
    }

    public override void OnCollision(Sprite sprite)
    {
        if (sprite is Tile)
        {
            var Tile = (Tile)sprite;
            var ImageName = Tile.ImageName;
            //only those tile can collision
            if ((ImageName == "Ground1.png")
             || (ImageName == "Rock2.png")
             || (ImageName == "Rock1.png")
             || (ImageName == "Box1.png")
             || (ImageName == "Box2.png")
             || (ImageName == "Box3.png")
             || (ImageName == "Box4.png")
             || (ImageName == "Spring1.png"))
            {
                Tile.CanCollision = true;
                Tile.Left = (int)Tile.X;
                Tile.Top = (int)Tile.Y;
                Tile.Right = (int)Tile.X + Tile.Width;
                Tile.Bottom = (int)Tile.Y + Tile.Height;
                LeftEdge = Tile.Left;
                RightEdge = Tile.Right;
            }

            //Falling-- land at ground
            if (JumpState == JumpState.jsFalling)
            {
                if ((ImageName == "Ground1.png")
                 || (ImageName == "Rock1.png")
                 || (ImageName == "Rock2.png")
                 || (ImageName == "Box1.png")
                 || (ImageName == "Box2.png")
                 || (ImageName == "Box3.png")
                 || (ImageName == "Box4.png"))
                {
                    if ((this.Right - 4 > Tile.Left)
                    && (this.Left + 3 < Tile.Right)
                    && (this.Bottom - 12 < Tile.Top))
                    {
                        JumpState = JumpState.jsNone;
                        DoJump = false;
                        this.Y = Tile.Top - 102;
                        JumpSpeed = 0.35f;
                        JumpHeight = 10.5f;
                        MaxFallSpeed = 8.5f;
                        switch (State)
                        {
                            case State.StandLeft: SetAnim("Idle.png", 0, 12, 0.25f, true, true, true); break;
                            case State.StandRight: SetAnim("Idle.png", 0, 12, 0.25f, true, false, true); break;
                            case State.WalkLeft: SetAnim("Walk.png", 0, 12, 0.2f, true, true, true); break;
                            case State.WalkRight: SetAnim("Walk.png", 0, 12, 0.2f, true, false, true); break;
                        }
                    }
                }
            }

            // jumping-- touch top tiles
            if (JumpState == JumpState.jsJumping)
            {
                if ((ImageName == "Rock1.png")
                 || (ImageName == "Rock2.png")
                 || (ImageName == "Box1.png")
                 || (ImageName == "Box2.png")
                 || (ImageName == "Box3.png")
                 || (ImageName == "Box4.png"))
                {
                    if ((this.Right - 4 > Tile.Left)
                     && (this.Left + 3 < Tile.Right)
                     && (this.Top < Tile.Bottom - 5)
                     && (this.Bottom > Tile.Top + 8))
                    {
                        JumpState = JumpState.jsFalling;
                        if ((ImageName == "Box1.png")
                         || (ImageName == "Box2.png")
                         || (ImageName == "Box3.png")
                         || (ImageName == "Box4.png"))
                        {
                            Tile.Dead();
                            GameFunc.SprayBox(Tile.X, Tile.Y);
                            GreenApple.Create(Tile.X, Tile.Y);
                        }
                    }
                }
            }
            //collision with tile
            if ((ImageName == "Rock1.png")
             || (ImageName == "Rock2.png")
             || (ImageName == "Box1.png")
             || (ImageName == "Box2.png")
             || (ImageName == "Box3.png")
             || (ImageName == "Box4.png"))
            {
                if (State == State.WalkLeft)
                {
                    if ((this.Left + 8 > Tile.Right)
                     && (this.Top + 10 < Tile.Bottom)
                     && (this.Bottom - 8 > Tile.Top))
                        this.X = Tile.X + (Tile.Width - 45) - 3;
                }
                if (State == State.WalkRight)
                {
                    if ((this.Right - 8 < Tile.Left)
                     && (this.Top + 10 < Tile.Bottom)
                     && (this.Bottom - 8 > Tile.Top))
                        this.X = Tile.X - (this.PatternWidth - 45) + 3 + 3;
                }
            }
            //get fruit
            if (ImageName == "Fruit1.png" || ImageName == "Fruit4.png")
            {
                Tile.Dead();
                GameFunc.SprayFruit(Tile.X + 10, Tile.Y + 10);
            }
            //when falling and touch spring
            if (ImageName == "Spring1.png" && JumpState == JumpState.jsFalling)
            {
                this.Y = Tile.Top - 85;
                JumpState = JumpState.jsNone;
                DoJump = true;
                JumpSpeed = 0.2f;
                JumpHeight = 13;
                MaxFallSpeed = 8.5f;
                AnimPos = 0;
                PatternIndex = 0;
                switch (State)
                {
                    case State.WalkLeft: SetAnim("Jump.png", 0, 3, 0.06f, false, false, true); break;
                    case State.WalkRight: SetAnim("Jump.png", 0, 3, 0.06f, false, true, true); break;
                }
                Tile.AnimPos = 0;
                Tile.PatternIndex = 0;
                Tile.SetAnim("Spring1.png", 0, 6, 0.2f, false, false, true);
            }
        }
        // get green Apple
        if (sprite is GreenApple)
        {
            if (((GreenApple)sprite).JumpState == JumpState.jsNone)
                ((GreenApple)sprite).Dead();
        }
        // jump fall and kill enemy
        if (sprite is Enemy)
        {
            var Enemy = (Enemy)sprite;

            if (Y + 80 > Enemy.Y)
            {
                if (JumpState == JumpState.jsNone || JumpState == JumpState.jsJumping)
                {

                    State = State.Die;
                    CanCollision = false;
                    DoJump = true;
                    JumpHeight= 8;
                    JumpSpeed=0.2f;
                    AnimPos = 0;
                    SetAnim("Dead.png", 0, 6, 0.1f, true, false, true);
                }
            }

            if (JumpState == JumpState.jsFalling)
            {
                JumpState = JumpState.jsNone;
                JumpSpeed = 0.1f;
                JumpHeight = 7;
                JumpSpeed = 0.2f;
                DoJump = true;
                Enemy.State = State.Die;
                Enemy.CanCollision = false;
                Enemy.DoAnimate = false;
                Enemy.DoJump = true;
                Enemy.FlipY = true;
                Enemy.MaxFallSpeed = 7;
                Enemy.JumpHeight = 7;
                //SetAnim("Dead.png", 0, 6, 0.1f, true, false, true);
            }
        }
    }
}

public class GreenApple : JumperSprite
{
    public GreenApple(Sprite Parent) : base(Parent)
    {
       SpriteSheetMode =  SpriteSheetMode.NoneSingle;
    }
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        SetCollideRect(0, 0, 28, 30);
        Collision();
    }
    public override void OnCollision(Sprite sprite)
    {
        if (sprite is Tile)
        {
            var Tile = (Tile)sprite;
            if (Tile.ImageName == "Ground1.png" || Tile.ImageName == "Rock1.png" || Tile.Name == "Rock2.png")
            {
                JumpState = JumpState.jsNone;
                Y = Tile.Y - 28;
            }
        }
    }
    public static void Create(float PosX, float PosY)
    {
        Random Random = new Random();
        if (Random.Next(0, 2) == 1)
        {
            var GreenApple = new GreenApple(EngineFunc.SpriteEngine);
            GreenApple.Init(EngineFunc.ImageLib, "Fruit" + (Random.Next(2, 4)).ToString() + ".png", PosX, PosY, 5);
            GreenApple.CollideMode = CollideMode.Rect;
            GreenApple.CanCollision = true;
            GreenApple.DoJump = true;
        }
    }
}

public class Tile : AnimatedSprite
{
    public Tile(Sprite Parent) : base(Parent)
    {
        CanCollision = true;
    }
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        SetCollideRect(0, 0, Width, Height);
    }
}

public class Enemy : JumperSprite
{
    public Enemy(Sprite Parent) : base(Parent)
    {
        CanCollision = true;
        CollideMode = CollideMode.Rect;
        State = State.WalkLeft;
        JumpState = JumpState.jsFalling;
    }
    public State State;
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        SetCollideRect(0, 0, Width, Height);

        if (ImageName == "Enemy2.png") SetCollideRect(31, 18, 71, 78);
        if (State != State.Die)
        {
            switch (State)
            {
                case State.WalkLeft: X -= Speed * Delta; SetAnim(ImageName, 0, AnimCount, AnimSpeed, true, false, true); break;
                case State.WalkRight: X += Speed * Delta; SetAnim(ImageName, 0, AnimCount, AnimSpeed, true, true, true); break;
            }
        }
        if (Y > 900)
            Dead();
        Collision();
    }
    public override void OnCollision(Sprite sprite)
    {
        if (sprite is Tile)
        {
            var Tile = (Tile)(sprite);
            if (JumpState == JumpState.jsFalling)
            {
                if (Tile.ImageName == "Ground1.png" || Tile.ImageName == "Rock2.png")
                {
                    JumpState = JumpState.jsNone;
                    Y = Tile.Y - 40;
                    if (ImageName == "Enemy3.png")
                        Y = Tile.Y - 55;
                    if (ImageName == "Enemy2.png")
                        Y = Tile.Y - 75;
                    if (ImageName == "Enemy1.png")
                        Y = Tile.Y - 49;

                }
            }
            if (Tile.ImageName == "Test1.png")
            {
                if (State == State.WalkLeft)
                {
                    X += 5;
                    State = State.WalkRight;
                }
                else
                {
                    X -= 5;
                    State = State.WalkLeft;
                }
            }
        }
    }
}

public class Background : BackgroundSprite
{
    public Background(Sprite Parent) : base(Parent)
    {
       SpriteSheetMode= SpriteSheetMode.NoneSingle;
        TileMode = TileMode.Horizontal;
    }

    public int Layer;
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        switch (Layer)
        {
            case 1: X = EngineFunc.SpriteEngine.Camera.X * 0.5f; break;
            case 2: X = EngineFunc.SpriteEngine.Camera.X * 0.3f; break;
            case 3: X = EngineFunc.SpriteEngine.Camera.X * 0.1f; break;
        }
    }
}

public class GameFunc
{
    public static void SprayBox(float PosX, float PosY)
    {
        Random Random = new Random();
        float Rnd()
        {
            return Random.Next(0, 100) * 0.01f;
        }

        for (int i = 0; i <= 6; i++)
        {
            var Particle = new ParticleSprite(EngineFunc.SpriteEngine);
            Particle.Init(EngineFunc.ImageLib, "Star.png", PosX + Random.Next(0, 21), PosY + Random.Next(0, 21), 20, 13, 14);
            Particle.SpriteSheetMode= SpriteSheetMode.NoneSingle;
            Particle.LifeTime = 150;
            Particle.Decay = 1;
            Particle.ScaleX = 1.2f;
            Particle.ScaleY = 1.2f;
            Particle.UpdateSpeed = 0.5f;
            Particle.VelocityX = -4 + Rnd() * 8;
            Particle.VelocityY = -Rnd() * 7;
            Particle.AccelX = 0;
            Particle.AccelY = 0.2f + Rnd() / 2;
        }
    }

    class Spray : PlayerSprite
    {
        public Spray(Sprite Parent) : base(Parent)
        {
           SpriteSheetMode= SpriteSheetMode.NoneSingle;
        }
        byte Alpha = 255;
        public override void DoMove(float Delta)
        {
            base.DoMove(Delta);
            Accelerate();
            UpdatePos(Delta);
            SetColor(Alpha, Alpha, Alpha, Alpha);
            Alpha -= 3;
            if (Alpha < 10)
                Dead();
        }
    }

    public static void SprayFruit(float PosX, float PosY)
    {
        for (int i = 0; i <= 15; i++)
        {
            var Spray = new Spray(EngineFunc.SpriteEngine);
            Spray.Init(EngineFunc.ImageLib, "flare.png", PosX, PosY, 20);
            Spray.ScaleX = 0.1f;
            Spray.ScaleY = 0.1f;
            Spray.Acceleration = 0.08f;
            Spray.MinSpeed = 0.8f;
            Spray.MaxSpeed = 1;
            Spray.Direction = i * 16;
            Spray.BlendMode = BlendMode.AddtiveColor;
        }
    }

    public static void CreateMap()
    {
        for (int i = 3; i >= 1; i--)
        {
            var Background = new Background(EngineFunc.BackgroundEngine);
            Background.Init(EngineFunc.ImageLib, "back" + i.ToString() + ".png", 0, 0, -100, 0, 0, 1024, 240);
            Background.Layer = i;
            switch (i)
            {
                case 1: Background.Y = -450; break;
                case 2: Background.Y = -400; break;
                case 3: Background.Y = 0; Background.SetSize(1600, 240); Background.Offset.X = 500; break;
            }
        }

        var Player = new Player(EngineFunc.SpriteEngine);
        Player.Init(EngineFunc.ImageLib, "Idle.png", 800, 400, 100);

        string AllText = System.IO.File.ReadAllText("Map1.txt");
        string[] Section = AllText.Split('/');
        int Length = Section.Length;
        int X = 0, Y = 0;
        string ImageName = null;

        for (int i = Length - 2; i > 0; i--)
        {
            var Str = Section[i].Split(',');
            X = int.Parse(Regex.Replace(Str[0], @"\D", ""));
            Y = int.Parse(Regex.Replace(Str[1], @"\D", ""));
            ImageName = Regex.Replace(Str[2], "ImageName=", "").Trim();
            //create Map tile
            if ((ImageName != "Enemy1.png") && (ImageName != "Enemy2.png") && (ImageName != "Enemy3.png"))
            {
                var Tile = new Tile(EngineFunc.SpriteEngine);
                Tile.Init(EngineFunc.ImageLib, ImageName, X - 540, Y-20, -10);
                Tile.SetSize(Tile.ImageWidth, Tile.ImageHeight);
                Tile.SetPattern(Tile.Width, Tile.Height);
                if (ImageName == "Spring1.png")
                {
                    Tile.SetPattern(48, 48);
                    Tile.SetSize(48, 48);
                }
            }
            //Create Enemy
            else
            {
                var Enemy = new Enemy(EngineFunc.SpriteEngine);
                Enemy.Init(EngineFunc.ImageLib, ImageName, X - 540, Y - 10, 150);
                switch (ImageName)
                {
                    case "Enemy1.png": Enemy.SetSize(81, 52); Enemy.AnimCount = 6; Enemy.AnimSpeed = 0.25f; Enemy.Speed = 1.7f; break;
                    case "Enemy2.png": Enemy.SetSize(68, 84); Enemy.AnimCount = 10; Enemy.AnimSpeed = 0.27f; Enemy.Speed = 1.7f; break;
                    case "Enemy3.png": Enemy.SetSize(56, 58); Enemy.AnimCount = 4; Enemy.AnimSpeed = 0.18f; Enemy.Speed = 1.4f; break;
                }
                Enemy.SetPattern(Enemy.Width, Enemy.Height);
            }
        }
    }
}



