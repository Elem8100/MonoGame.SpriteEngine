using System;
using Input = Microsoft.Xna.Framework.Input.Keys;
using Keyboard = SpriteEngine.Keyboard;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using MonoGame.SpriteEngine;
using TiledCS;
namespace Tiled_Slope;

public enum Dir { Left, Right }

public class Player : JumperSprite
{
    public Player(Sprite Parent) : base(Parent)
    {
        JumpSpeed = 1f;
        JumpHeight = 7.5f;
        MaxFallSpeed = 8;
        JumpState = JumpState.jsJumping;
    }

    Dir Dir;
    bool FallFlag;
    int FallCounter;
    public Foothold FH;
    float SpeedL, SpeedR;
    public static float DestX, DestY;
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        Keyboard.GetState();
        int X1 = (int)FH.X1;
        int Y1 = (int)FH.Y1;
        int X2 = (int)FH.X2;
        int Y2 = (int)FH.Y2;

        if (Keyboard.KeyDown(Input.Space))
        {
            DoJump = true;
        }
        if (Keyboard.KeyDown(Input.Right))
            SetAnim("Player.png", 0, 5, 0.18f, true, false, true);
        if (Keyboard.KeyDown(Input.Left))
            SetAnim("Player.png", 0, 5, 0.18f, true, true, true);
        if (Keyboard.KeyUp(Input.Left))
        {
            AnimSpeed = 0;
            AnimPos = 0;
        }

        if (Keyboard.KeyUp(Input.Right))
        {
            AnimSpeed = 0;
            AnimPos = 0;
        }
        if (JumpState != JumpState.jsNone)
        {
            PatternIndex = 3;
            AnimSpeed = 0;
        }

        //left
        if (Keyboard.KeyDown(Input.Left) && SpeedR == 0)
        {
            Dir = Dir.Left;
            SpeedL += 1.5f;
            if (SpeedL > 3.5)
                SpeedL = 3.5f;
        }
        else
        {
            SpeedL -= 0.25f;
            if (SpeedL < 0)
                SpeedL = 0;
        }
        //right
        if (Keyboard.KeyDown(Input.Right) && SpeedL == 0)
        {
            Dir = Dir.Right;
            SpeedR += 1.5f;
            if (SpeedR > 3.5)
                SpeedR = 3.5f;
        }
        else
        {
            SpeedR -= 0.25f;
            if (SpeedR < 0)
                SpeedR = 0;
        }

        DestX = (1024 / 2) - X;
        if (Math.Abs(DestX + Engine.Camera.X) > 1)
            Engine.Camera.X -= 0.05f * (DestX + Engine.Camera.X);
        DestY = (768 / 2) + 60 - Y;
        if (Math.Abs(DestY + Engine.Camera.Y) > 1)
            Engine.Camera.Y -= 0.01f * (DestY + Engine.Camera.Y);
        if (Engine.Camera.Y > 1200)
            Engine.Camera.Y = 1200;
        if (Engine.Camera.X < 0)
            Engine.Camera.X = 0;
        if (Engine.Camera.X > 6100)
            Engine.Camera.X = 6100;

        Foothold BelowFH = null;
        Vector2 Below = new Vector2(0, 0);
        Foothold WallFH = null;
        //walk left
        if (Dir == Dir.Left && SpeedR == 0)
        {
            if (X < 10)
            {
                SpeedL = 0;
            }
            int Direction = GetAngle256(X2, Y2, X1, Y1);
            if (!FH.IsWall() && JumpState == JumpState.jsNone)
            {
                X += (float)(Sin256(Direction) * SpeedL);
                Y -= (float)(Cos256(Direction) * SpeedL);
            }
            else
            {
                X -= SpeedL;
            }

            float FallEdge = -999999;
            if (FH.Prev == null)
                FallEdge = FH.X1 - 10;

            // Wall down
            if ((FH.Prev != null) && (FH.Prev.IsWall()) && (FH.Prev.Y1 > Y))
                FallEdge = FH.X1;

            if ((JumpState == JumpState.jsNone) && (X < FallEdge))
                JumpState = JumpState.jsFalling;
            Below = FootholdTree.Instance.FindBelow(new Vector2(X + 10, Y - 5), ref BelowFH);
            WallFH = FootholdTree.Instance.FindWallR(new Vector2(X + 4, Y - 4));
            if ((WallFH != null) && (X <= WallFH.X1 + 20))
            {
                X = WallFH.X1 + 1 + 20;
                SpeedL = 0;
            }
            //walk left
            if ((X <= FH.X1) && (FH.PrevID != 0) && (!FH.IsWall()) && (!FH.Prev.IsWall()))
            {
                if (JumpState == JumpState.jsNone)
                {
                    FH = FH.Prev;
                    X = FH.X2;
                    Y = FH.Y2;
                }
            }
        }
        //walk right
        if ((Dir == Dir.Right) && (SpeedL == 0))
        {
            if (X > 7100)
            {
                SpeedR = 0;
            }

            int Direction = GetAngle256(X1, Y1, X2, Y2);
            if (!FH.IsWall() && JumpState == JumpState.jsNone)
            {
                X += (float)(Sin256(Direction) * SpeedR);
                Y -= (float)(Cos256(Direction) * SpeedR);
            }
            else
            {
                X += SpeedR;
            }

            float FallEdge = 999999;
            if (FH.Next == null)
                FallEdge = FH.X2 + 5;
            // Wall down
            if ((FH.Next != null) && (FH.Next.IsWall()) && (FH.Next.Y2 > Y))
                FallEdge = FH.X2;

            if ((JumpState == JumpState.jsNone) && (X > FallEdge))
                JumpState = JumpState.jsFalling;
            Below = FootholdTree.Instance.FindBelow(new Vector2(X - 10, Y - 5), ref BelowFH);
            WallFH = FootholdTree.Instance.FindWallL(new Vector2(X - 4, Y - 4));
            if ((WallFH != null) && (X >= WallFH.X1 - 20))
            {
                X = WallFH.X1 - 1 - 20;
                if (JumpState == JumpState.jsNone)
                    Y = WallFH.Y1;
                SpeedR = 0;
            }

            // walk right
            if ((X >= FH.X2) && (FH.NextID != 0) && (!FH.IsWall()) && (!FH.Next.IsWall()))
            {
                if (JumpState == JumpState.jsNone)
                {
                    FH = FH.Next;
                    X = FH.X1;
                    Y = FH.Y1;
                }
            }
        }

        if ((JumpState == JumpState.jsFalling) && (FallFlag))
        {
            Below = FootholdTree.Instance.FindBelow(new Vector2(X, Y - VelocityY - 6), ref BelowFH);

            if (Y >= Below.Y - 3)
            {
                Y = Below.Y;
                MaxFallSpeed = 8;
                JumpState = JumpState.jsNone;
                FH = BelowFH;
                DoJump = false;
            }
        }

        if (Keyboard.KeyDown(Input.Space))
        {
            if (Keyboard.KeyDown(Input.Down) && JumpState == JumpState.jsNone)
            {
                FallFlag = false;
                Below = FootholdTree.Instance.FindBelow(new Vector2(X, Y + 50), ref BelowFH);

                if (BelowFH.X2 > BelowFH.X1)
                    if (Below.Y != 99999)
                    {
                        JumpState = JumpState.jsFalling;
                    }
            }
        }

        if (!FallFlag)
            FallCounter += 1;
        if (FallCounter > VelocityY + 3)
        {
            FallFlag = true;
            FallCounter = 0;
        }

        if (JumpState == JumpState.jsJumping)
        {
            Below = FootholdTree.Instance.FindBelow(new Vector2(X, Y - 10), ref BelowFH);
            if (BelowFH.X2 < BelowFH.X1)
                JumpState = JumpState.jsFalling;
        }
    }

}

public class GameFunc
{
    public static void CreateGame()
    {

        var Map = new TiledMap("SlopeMap1.tmx");
        // var TileSet = new TiledTileset("0.tsx");
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
                Tile.SetPattern(48, 48);
                Tile.DoMove(1);
                Tile.Moved = false;
                Tile.IntMove = true;
                Tile.PatternIndex = Index - 1;
                Tile.SetSize(48, 48);
                Tile.X = (i % Map.Width) * Map.TileWidth;
                Tile.Y = (float)Math.Floor(i / (double)Map.Width) * Map.TileHeight;
            }
        }
        //
        FootholdTree.CreateFootholds("Footholds.txt");
        var Player = new Player(EngineFunc.SpriteEngine);
        Player.ImageLib = EngineFunc.ImageLib;
        Player.ImageName = "Player.png";
        Player.X = 1200;
        Player.Y = 1300;
        Player.OriginY = 64;
        Player.OriginX = 32;
        Player.SetPattern(64, 64);
        Player.JumpHeight = 18;
        Player.JumpSpeed = 1;
        Player.IntMove = true;
        Foothold BelowFH = null;
        var Below = FootholdTree.Instance.FindBelow(new Vector2(500, 500), ref BelowFH);
        Player.FH = new Foothold(new Vector2(0, 0), new Vector2(0, 0), 0);
        EngineFunc.SpriteEngine.Camera.X = 1200;
        EngineFunc.SpriteEngine.Camera.Y = 1400;
    }

}