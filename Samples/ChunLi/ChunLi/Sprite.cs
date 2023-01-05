using System;
using MonoGame.SpriteEngine;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Input = Microsoft.Xna.Framework.Input.Keys;
using Keyboard = SpriteEngine.Keyboard;
using System.Windows.Forms;

namespace ChunLi;

public enum State
{
    Stand,
    WalkLeft,
    WalkRight,
    Jump,
    PreCrouch,
    Crouch,
    HandAttack1,
    HandAttack2,
    HandAttack3,
    HandAttack4,
    HandAttack5,
    FootAttack1,
    FootAttack2,
    FootAttack3,
    FootAttack4,
    FootAttack5,
    FootAttack6
}

public class Bullet : AnimatedSprite
{
    public Bullet(Sprite Parent) : base(Parent)
    {
        PatternWidth = 69;
        PatternHeight = 46;
    }
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        X += 5*Delta;
        if (X > 950)
            Dead();
    }
}
public class Player : JumperSprite
{
    public Player(Sprite Parent) : base(Parent)
    {
        X = 420;
        Y = 485;
        WalkSpeed = 2f;
        JumpSpeed = 0.43f;
        JumpHeight = 11;
        ImageMode = ImageMode.SpriteSheet;
        PatternWidth = 180;
        PatternHeight = 150;
        PatternIndex = 0;
    }
    public static bool KeyPress;
    public State State;
    public float WalkSpeed;
    public bool DoFire;
    public AnimatedSprite Silhouette;
    public void DoAttack()
    {

        if (Keyboard.KeyDown(Input.A))
            State = State.HandAttack1;
        if (Keyboard.KeyDown(Input.S))
            State = State.HandAttack2;
        if (Keyboard.KeyDown(Input.D))
            State = State.HandAttack3;
        if (Keyboard.KeyDown(Input.F))
            State = State.HandAttack4;
        if (Keyboard.KeyDown(Input.G))
            State = State.HandAttack5;
        if (Keyboard.KeyDown(Input.Z))
            State = State.FootAttack1;
        if (Keyboard.KeyDown(Input.X))
            State = State.FootAttack2;
        if (Keyboard.KeyDown(Input.C))
            State = State.FootAttack3;
        if (Keyboard.KeyDown(Input.V))
            State = State.FootAttack4;
        if (Keyboard.KeyDown(Input.B))
            State = State.FootAttack5;
        if (Keyboard.KeyDown(Input.N))
            State = State.FootAttack6;
    }
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
       
        switch (State)
        {
            case State.Stand: SetAnim("Pic1.png", 0, 10, 0.23f, true, false, true); break;
            case State.WalkLeft: SetAnim("Pic1.png", 26, 16, 0.28f, true, false, true); break;
            case State.WalkRight: SetAnim("Pic1.png", 11, 16, 0.28f, true, false, true); break;
            case State.HandAttack1: SetAnim("Pic1.png", 62, 7, 0.23f, false, false, true); break;
            case State.HandAttack2: SetAnim("Pic1.png", 70, 14, 0.28f, false, false, true); break;
            case State.HandAttack3: SetAnim("Pic1.png", 98, 10, 0.25f, false, false, true); break;
            case State.HandAttack4: SetAnim("Pic2.png", 0, 15, 0.28f, false, false, true); break;
            case State.HandAttack5: SetAnim("Pic2.png", 32, 12, 0.28f, false, false, true); break;
            case State.FootAttack1: SetAnim("Pic2.png", 15, 7, 0.25f, false, false, true); break;
            case State.FootAttack2: SetAnim("Pic2.png", 23, 10, 0.23f, false, false, true); break;
            case State.FootAttack3: SetAnim("Pic2.png", 45, 25, 0.28f, false, false, true); break;
            case State.FootAttack4: SetAnim("Pic2.png", 69, 8, 0.23f, false, false, true); break;
            case State.FootAttack5: SetAnim("Pic2.png", 77, 13, 0.28f, false, false, true); break;
            case State.FootAttack6: SetAnim("Pic2.png", 90, 25, 0.28f, false, false, true); break;
            case State.PreCrouch: SetAnim("Pic2.png", 114, 3, 0.33f, false, false, true); break;
            case State.Crouch: SetAnim("Pic2.png", 117, 6, 0.23f, true, false, true); break;
            case State.Jump: SetAnim("Pic1.png", 59, 3, 0.12f, true, false, true); break;
        }
        if (Y > 455)
        {
            DoJump = false;
            Y = 455;
            JumpState = JumpState.jsNone;
            State = State.Stand;
        }
        Keyboard.GetState();
        if (JumpState == JumpState.jsNone)
        {
            if (Keyboard.KeyDown(Input.Up))
            {
                State = State.Jump;
                DoJump = true;
            }
        }

        if (Keyboard.KeyDown(Input.Right))
        {
            if (State == State.WalkRight || State == State.Jump)
                X += WalkSpeed*Delta;
        }

        if (Keyboard.KeyDown(Input.Left))
        {
            if (State == State.WalkLeft || State == State.Jump)
                X -= WalkSpeed*Delta;
        }
        // stand  attack
        if ((State == State.Stand) && (KeyPress = true))
        {
            DoAttack();
        }
        //when move and press attack
        if ((Keyboard.KeyDown(Input.Left)) || (Keyboard.KeyDown(Input.Right)))
        {
            if ((KeyPress) && ((State == State.WalkRight) || (State == State.WalkLeft)))
            {
                DoAttack();
            }
        }
        //
        if ((Keyboard.KeyDown(Input.Down)) && (KeyPress))
        {
            if (State == State.Stand)
            {
                State = State.PreCrouch;
                KeyPress = false;
            }
        }

        if (Keyboard.KeyPressed(Input.G))
            DoFire = true;
        if (Keyboard.KeyUp(Input.Down))
        {
            if (State == State.Crouch)
                State = State.Stand;
            WalkSpeed = 2f;
        }
        if (Keyboard.KeyUp(Input.Right) || Keyboard.KeyUp(Input.Left))
        {
            if ((State == State.WalkRight) || (State == State.WalkLeft))
                State = State.Stand;
        }
        if (AnimEnded())
        {
            switch (State)
            {
                case State.PreCrouch:
                    State = State.Crouch;
                    break;
                case State.FootAttack1:
                case State.FootAttack2:
                case State.FootAttack3:
                case State.FootAttack4:
                case State.FootAttack5:
                case State.FootAttack6:
                case State.HandAttack1:
                case State.HandAttack2:
                case State.HandAttack3:
                case State.HandAttack4:
                case State.HandAttack5:
                    {
                        State = State.Stand;
                        PatternIndex = 0;
                        WalkSpeed = 2f;
                    }
                    break;
            }
        }

        if (Keyboard.KeyDown(Input.Right))
        {
            if (State == State.Stand)
                State = State.WalkRight;
        }

        if (Keyboard.KeyDown(Input.Left))
        {
            if (State == State.Stand)
                State = State.WalkLeft;
        }

        if (DoFire && PatternIndex == 40)
        {
            var Bullet = new Bullet(this.Engine);
            Bullet.ImageLib = this.ImageLib;
            Bullet.X = this.X + 80;
            Bullet.Y = this.Y + 55;
            Bullet.SetAnim("Bullet.png", 1, 9, 0.28f, true, false, true);
            Bullet.BlendMode = BlendMode.Lighten;
            DoFire = false;
        }

        Silhouette.ImageName = this.ImageName;
        Silhouette.PatternIndex = this.PatternIndex;
        Silhouette.X = this.X;
        Silhouette.Y = -this.Y + 1045;
    }

}

public class GameFunc
{
    public static void CreateGame()
    {

        var Player = new Player(EngineFunc.SpriteEngine);
        Player.ImageLib = EngineFunc.ImageLib;
        Player.ImageName = "Pic1.png";
       
        //
        Player.Silhouette = new AnimatedSprite(EngineFunc.SpriteEngine);
        Player.Silhouette.ImageLib = Player.ImageLib;
        Player.Silhouette.ImageName = Player.ImageName;
        Player.Silhouette.FlipY = true;
        Player.Silhouette.PatternWidth = 180;
        Player.Silhouette.PatternHeight = 150;
        Player.Silhouette.SetColor(100, 255, 100, 100);
      // 
    }


}