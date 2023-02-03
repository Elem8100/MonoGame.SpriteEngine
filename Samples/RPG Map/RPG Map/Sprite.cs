using MonoGame.SpriteEngine;
using System.IO;
using System.Text.RegularExpressions;
using Input = Microsoft.Xna.Framework.Input.Keys;
using Keyboard = SpriteEngine.Keyboard;
using Microsoft.Xna.Framework.Input;
using System.Windows.Forms;
namespace RPG_Map;
public class Player : AnimatedSprite
{
    public Player(Sprite Parent) : base(Parent)
    {
        CollideMode = CollideMode.Rect;
        CanCollision = true;
        SetPattern(32, 48);
    }
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        SetCollideRect(5, 32 , PatternWidth - 6, PatternHeight);
        Keyboard.GetState();
        DoAnimate = false;
        if (Keyboard.KeyDown(Input.Left))
        {
            X -= 3 * Delta;
            SetAnim("player.png", 4, 4, 0.15f, true, false, true);
        }

        if (Keyboard.KeyDown(Input.Right))
        {
            X += 3 * Delta;
            SetAnim("player.png", 8, 4, 0.15f, true, false, true);
        }

        if (Keyboard.KeyDown(Input.Up))
        {
            Y -= 3 * Delta;
            SetAnim("player.png", 12, 4, 0.15f, true, false, true);
        }

        if (Keyboard.KeyDown(Input.Down))
        {
            Y += 3 * Delta;
            SetAnim("player.png", 0, 4, 0.15f, true, false, true);
        }
        Z = (int)Y + PatternHeight + 20;
        Collision();
        Engine.Camera.X = X - 512;
        Engine.Camera.Y = Y - 384;
    }

    public override void OnCollision(Sprite sprite)
    {
        if (sprite is MapObj)
        {
            var Tile = (MapObj)sprite;
            Tile.Left = (int)Tile.X;
            Tile.Top = (int)Tile.Y;
            Tile.Right = (int)Tile.X + Tile.Width;
            Tile.Bottom = (int)Tile.Y + Tile.Height;
            if (Tile.ImageName == "Block1.png" || Tile.ImageName == "Block2.png")
            {
                if (Keyboard.KeyDown(Input.Left))
                {
                    if ((this.Left + 8 > Tile.Right)
                     && (this.Top + 5 < Tile.Bottom)
                     && (this.Bottom - 8 > Tile.Top))
                        this.X = Tile.Right-6;
                }

                if (Keyboard.KeyDown(Input.Right))
                {
                    if ((this.Right - 8 < Tile.Left)
                     && (this.Top + 5 < Tile.Bottom)
                     && (this.Bottom - 8 > Tile.Top))
                        this.X = Tile.Left - 25;
                }
                if (Keyboard.KeyDown(Input.Up))
                {
                    if ((this.Top + 5 > Tile.Bottom)
                     && (this.Right - 4 > Tile.Left)
                     && (this.Left + 3 < Tile.Right))
                        this.Y = Tile.Bottom-36;//- 36;
                }

                if (Keyboard.KeyDown(Input.Down))
                {
                    if ((this.Bottom - 4 < Tile.Top)
                     && (this.Right - 4 > Tile.Left)
                     && (this.Left + 3 < Tile.Right))
                        this.Y = Tile.Top - 45;
                }
            }
        }
    }

}

public class MapObj : Sprite
{
    public MapObj(Sprite Parent) : base(Parent)
    {
    }

    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        SetCollideRect(0,0, Width, Height);
    }
    public static void CreateMap()
    {
        string LeftStr(string s, int count)
        {
            if (count > s.Length)
                count = s.Length;
            return s.Substring(0, count);
        }
        string AllText = File.ReadAllText("Map1.txt");
        string[] Section = AllText.Split('/');
        int Length = Section.Length;
      
        for (int i = Length - 2; i > 0; i--)
        {
            var Str = Section[i].Split(',');
            int X = int.Parse(Regex.Replace(Str[0], @"\D", ""));
            int Y = int.Parse(Regex.Replace(Str[1], @"\D", ""));
            string ImageName = Regex.Replace(Str[2], "ImageName=", "").Trim();

            var MapObj = new MapObj(EngineFunc.SpriteEngine);
            MapObj.Init(EngineFunc.ImageLib, ImageName, X - 540, Y - 150, 0);
            MapObj.Width = MapObj.ImageWidth;
            MapObj.Height = MapObj.ImageHeight;
            MapObj.CollideMode = CollideMode.Rect;
            if (ImageName == "Block1.png" || ImageName == "Block2.png")
            {
                MapObj.Visible = false;
                MapObj.CanCollision = true;
            }

            if (LeftStr(ImageName, 4) == "Tree"
            || LeftStr(ImageName, 5) == "House"
            || ImageName == "Object4.png"
            || ImageName == "t22.png"
            || ImageName == "t23.png"
            || ImageName == "t24.png")
                MapObj.Z = (int)MapObj.Y + MapObj.Height;
            if (ImageName == "Tree4.png")
                MapObj.Z = (int)MapObj.Y + MapObj.Height - 60;
        }
    }
}