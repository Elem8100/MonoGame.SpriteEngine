using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Vector2 = Microsoft.Xna.Framework.Vector2;
namespace MonoGame.SpriteEngine;

public enum CollideMode
{
    Circle,
    Rect,
    Quadrangle,
    Polygon
}

public enum AnimPlayMode
{
    Forward,
    Backward,
    PingPong
}

public enum JumpState
{
    jsNone,
    jsJumping,
    jsFalling

}

public enum ImageMode
{
    SpriteSingle,
    SpriteSheet,
    CropRect
}

public enum TileMode
{
    Horizontal,
    Vertical,
    Full
}

public class MonoSpriteEngine : Sprite
{
    public MonoSpriteEngine(Sprite Parent) : base(Parent)
    {
        deadList = new();
        VisibleWidth = 1800;
        VisibleHeight = 1600;
    }
    protected int allCount;
    public int AllCount { get => allCount; set => allCount = value; }
    private List<Sprite> deadList;
    public List<Sprite> DeadList { get => deadList; }
    private int groupCount;
    private List<Sprite>[] groups;
    private int drawCount;
    private List<Sprite> currentSelected;
    public int DrawCount { get => drawCount; set => drawCount = value; }
    public Vector2 Camera;
    public bool ObjectsSelected { get; set; }
    public List<Sprite> CurrentSelected { get => currentSelected; }
    public int VisibleWidth { get; set; }
    public int VisibleHeight { get; set; }
    public GameCanvas Canvas;
    public List<Sprite> this[int index]
    {
        get
        {
            List<Sprite> Result;
            if ((index >= 0) || (index < groupCount))
                Result = groups[index];
            else
                Result = null;
            return Result;
        }
    }

    public void ClearCurrent()
    {
        while (CurrentSelected.Count != 0)
            ((SpriteEx)(CurrentSelected[CurrentSelected.Count - 1])).Selected = false;
        ObjectsSelected = false;
    }

    public void GroupSelect(Rect Area, bool Add = false, params Sprite[] Filter)
    {
        if (!Add)
            ClearCurrent();
        Sprite sprite;
        if (Filter.Length == 1)
        {
            for (int index = 0; index < Count; index++)
            {
                sprite = (SpriteEx)base[index];
                if (sprite.GetType().Equals(Filter[0]) && SpriteUtils.OverLapRect(((SpriteEx)(sprite)).GetBoundsRect(), Area))
                {
                    ((SpriteEx)(sprite)).Selected = true;
                }
            }
        }
        else
        {
            for (int index = 0; index < Count; index++)
            {
                sprite = base[index];
                for (int index2 = 0; index2 < Filter.Length; index2++)
                {
                    if (sprite.GetType().Equals(Filter[index2]) && SpriteUtils.OverLapRect(((SpriteEx)(sprite)).GetBoundsRect(), Area))
                    {
                        ((SpriteEx)(sprite)).Selected = true;
                        break;

                    }
                }
            }
        }
        ObjectsSelected = CurrentSelected.Count != 0;
    }

    public Sprite Select(Vector2 Point, bool Add = false, params Sprite[] Filter)
    {
        Sprite Result = null;
        if (!Add)
            ClearCurrent();
        if (Filter.Length == 1)
        {
            for (int index = DrawList.Count - 1; index >= 0; index--)
            {
                Result = DrawList[index];

                if (Result.GetType().Equals(Filter[0]) && SpriteUtils.PointInRect(Point, ((SpriteEx)(Result)).GetBoundsRect()))
                {
                    ((SpriteEx)(Result)).Selected = true;
                    ObjectsSelected = CurrentSelected.Count != 0;
                    goto End;
                }
                else
                {
                    Result = null;
                }
            }
        }
        else
        {
            for (int index = DrawList.Count - 1; index >= 0; index--)
            {
                Result = DrawList[index];

                for (int index2 = 0; index2 < Filter.Length; index2++)
                {
                    if (Result.GetType().Equals(Filter[index2]) && SpriteUtils.PointInRect(Point, ((SpriteEx)(Result)).GetBoundsRect()))
                    {
                        ((SpriteEx)(Result)).Selected = true;
                        ObjectsSelected = CurrentSelected.Count != 0;
                        goto End;
                    }
                    else
                    {
                        Result = null;
                    }
                }
            }
        }

    End:
        return Result;
    }

    public void ClearGroup(int GroupNumber)
    {
        var Group = this[GroupNumber];
        if (Group != null)
        {
            for (int Index = 0; Index <= Group.Count; Index++)
                ((SpriteEx)(Group[Index])).Selected = false;
        }
    }
    private void SetGroupCount(int GroupCount)
    {
        if (groupCount > GroupCount && GroupCount >= 0)
        {
            if (groupCount > GroupCount)
            {
                for (int Index = GroupCount; Index <= groupCount; Index++)
                {
                    ClearGroup(Index);
                    groups[Index] = null;
                }
                Array.Resize(ref groups, GroupCount);
            }
            else
            {
                Array.Resize(ref groups, GroupCount);
                for (int Index = groupCount; Index <= GroupCount; Index++)
                {
                    groups[Index] = new();
                }
            }
            groupCount = groups.Length;
        }
    }
    public int GroupCount
    {
        get => groupCount;
        set => SetGroupCount(value);
    }

    public void CurrentToGroup(int GroupNumber, bool Add = false)
    {
        List<Sprite> Group = groups[GroupNumber];
        if (Group == null)
            return;
        if (!Add)
            ClearGroup(GroupNumber);
        for (int index = 0; index < Group.Count; index++)
            ((SpriteEx)Group[index]).GroupNumber = GroupNumber;
    }

    public override void Draw()
    {
        drawCount = 0;
        base.Draw();
    }

    public void DrawEx(params string[] TypeList)
    {
        if (Visible)
        {
            if (Engine != null)
            {
                if ((X > Engine.Camera.X - Width) && (Y > Engine.Camera.Y - Height) && (X < Engine.Camera.X + Engine.VisibleWidth) && (Y < Engine.Camera.Y + Engine.VisibleHeight))
                {
                    DoDraw();
                    Engine.drawCount++;
                }
            }

            if (DrawList != null)
            {
                foreach (var I in DrawList)
                {
                    foreach (var i2 in TypeList)
                    {
                        if (((Sprite)I).GetType().ToString() == i2)
                            ((Sprite)I).Draw();
                    }
                }
            }
        }
    }
    public void Dead()
    {
        while (deadList.Count > 0)
        {
            ((Sprite)deadList[deadList.Count - 1]).Free();
        }
    }

}

public class Sprite
{
    public Sprite(Sprite Parent)
    {
        parent = Parent;
        if (parent != null)
        {
            parent.Add(this);
            if (parent is MonoSpriteEngine)
                Engine = (MonoSpriteEngine)parent;
            else
                Engine = parent.Engine;
            Engine.AllCount++;
        }
        Y = 200;
        Z = 0;
        if (Z == 0)
            Z = 1;
        Width = 100;
        Height = 100;
        Moved = true;
        Visible = true;
        TruncMove = true;
    }
    public Sprite(Sprite Parent, Dictionary<string, Microsoft.Xna.Framework.Graphics.Texture2D> ImageLib,
                  string ImageName, float X, float Y, int Z = 0, int Width = 100, int Height = 100)
    {
        parent = Parent;
        if (parent != null)
        {
            parent.Add(this);
            if (parent is MonoSpriteEngine)
                Engine = (MonoSpriteEngine)parent;
            else
                Engine = parent.Engine;
            Engine.AllCount++;
        }
        this.ImageLib = ImageLib;
        this.imageName = ImageName;
        this.x = X;
        this.y = Y;
        this.z = Z;
        if (Z == 0)
            Z = 1;
        this.Width = Width;
        this.Height = Height;
        Moved = true;
        Visible = true;
        TruncMove = true;
    }
    public void Init(Dictionary<string, Microsoft.Xna.Framework.Graphics.Texture2D> ImageLib,
                       string ImageName, float X, float Y, int Z = 0, int Width = 100, int Height = 100)
    {
        this.ImageLib = ImageLib;
        this.imageName = ImageName;
        this.x = X;
        this.y = Y;
        this.z = Z;
        this.Width = Width;
        this.Height = Height;
    }
    public MonoSpriteEngine Engine { get; set; }
    private List<Sprite> List;
    public List<Sprite> DrawList;
    private bool Deaded;
    public int Width, Height;
    public int Left, Top, Right, Bottom;
    private string imageName;
    private float x, y;
    private int z;
    public Vector2 Camera;
    public bool Visible;
    public bool CanCollision;
    int patternIndex;
    public int PatternIndex
    {
        get => patternIndex;
        set
        {
            patternIndex = value;
            if (imageName == " ")
                return;
        }
    }
    public int PatternCount
    {
        get
        {
            int Result = 0;
            if (PatternWidth != 0 && PatternHeight != 0)
            {
                Result = (ImageWidth / PatternWidth) * (ImageHeight / PatternHeight);
            }
            return Result;
        }
    }

    int imageIndex;
    public bool Moved { get; set; }
    public bool TruncMove { get; set; }
    public Vector2 CollidePos;
    public int CollideRadius;
    public Rect CollideRect;
    private Sprite parent;
    private int tag;
    public BlendMode BlendMode;
    public CollideMode CollideMode { get; set; }
    private bool ZSet;
    // public Wz_Node ImageEntry;
    // public Dictionary<Wz_Node, Microsoft.Xna.Framework.Graphics.Texture2D> ImageLib;
    public Dictionary<string, Microsoft.Xna.Framework.Graphics.Texture2D> ImageLib;
    public int ImageWidth
    {
        get
        {
            return ImageLib[imageName].Width;
        }
    }
    public int ImageHeight
    {
        get => ImageLib[imageName].Height;
    }
    public int PatternWidth;
    public int PatternHeight;
    public int Count
    {
        get
        {
            int Result;
            if (List != null)
                Result = List.Count;
            else
                Result = 0;
            return Result;
        }
    }

    public Sprite this[int index]
    {
        get
        {
            Sprite Result = null;
            if (List != null)
                Result = List[index];
            return Result;
        }
        set { }
    }
    private void Add(Sprite sprite)
    {
        if (List == null)
        {
            List = new List<Sprite>();
            DrawList = new List<Sprite>();
        }
        List.Add(sprite);
        // AddDrawList(sprite);
    }
    private void Remove(Sprite Sprite)
    {
        List.Remove(Sprite);
        DrawList.Remove(Sprite);
        if (List.Count == 0)
        {
            // List.Clear();
            List = null;
            // DrawList.Clear();
            DrawList = null;
        }
    }
    private void AddDrawList(Sprite sprite)
    {
        int L = 0;
        int H = DrawList.Count - 1;
        int I;
        while (L <= H)
        {
            I = (L + H) / 2;
            int C = (DrawList[I]).z - sprite.z - 1;
            if (C < 0)
                L = I + 1;
            else
                H = I - 1;
        }
        DrawList.Insert(L, sprite);
    }
    public void Clear()
    {
        while (Count > 0)
            this[Count - 1] = null;

    }
    public void Dead()
    {
        if ((Engine != null) && (!Deaded))
        {
            Deaded = true;
            Engine.DeadList.Add(this);
        }
    }
    public virtual void Free()
    {
        this.Clear();
        if (Parent != null)
        {
            Parent.Remove(this);
            Engine.DeadList.Remove(this);
            Engine.AllCount--;
        }
        List = null;
        DrawList = null;
    }
    public virtual void Draw()
    {
        if (Visible)
        {
            if (Engine != null)
            {
                if ((X > Engine.Camera.X - Width) && (Y > Engine.Camera.Y - Height) && (X < Engine.Camera.X + Engine.VisibleWidth) && (Y < Engine.Camera.Y + Engine.VisibleHeight))
                {
                    DoDraw();
                    Engine.DrawCount++;
                }
            }
            if (DrawList != null)
            {
                for (int i = 0; i < DrawList.Count; i++)
                    ((Sprite)DrawList[i]).Draw();
            }
        }

    }

    public void Move(float Delta)
    {
        if (Moved)
        {
            DoMove(Delta);
            for (int i = 0; i < Count; i++)
                this[i].Move(Delta);
        }
    }

    public virtual void DoMove(float Delta)
    {

    }
    public virtual void DoDraw()
    {
        if (!Visible || ImageLib == null)
            return;

        Engine.Canvas.DrawEx(ImageLib[imageName],
           X + Camera.X - Engine.Camera.X, Y + Camera.Y - Engine.Camera.Y, 0, 0, 1, 1, 0, false, false, 255, 255, 255, 255, false, BlendMode);
    }

    public string Name;
       
    public string ImageName
    {
        get => imageName;
        set
        {
            if (!string.Equals(imageName, value))
                imageName = value;
        }
    }
    public void SetPos(float X, float Y)
    {
        this.x = X;
        this.y = Y;
    }
    public void SetPos(float X, float Y, int Z)
    {
        this.x = X;
        this.y = Y;
        this.z = Z;
    }
    public void SetSize(int AWidth, int AHeight)
    {
        this.Width = AWidth;
        this.Height = AHeight;
    }

    public float X
    {
        get => x;
        set => x = value;
    }
    public float Y
    {
        get => y;
        set => y = value;

    }
    public int Z
    {
        get => z;
        set
        {
            if (z != value)
            {
                z = value;
                if (parent != null)
                {
                    // optimize load time
                    if (ZSet)
                        parent.DrawList.Remove(this);
                    parent.AddDrawList(this);
                    ZSet = true;
                }
            }
        }
    }

    public void SetCollideRect(int Left, int Top, int Right, int Bottom)
    {
        this.Left = (int)(X + Left);
        this.Top = (int)(Y + Top);
        this.Right = (int)(X + Right);
        this.Bottom = (int)(Y + Bottom);
        CollideRect = SpriteUtils.Rect(this.Left, this.Top, this.Right, this.Bottom);
    }
    public virtual void Collision(Sprite Other)
    {
        double Delta;
        bool IsCollide = false;
        if (CanCollision && Other.CanCollision && (!Deaded) && (!Other.Deaded))
        {
            switch (CollideMode)
            {
                case CollideMode.Circle:
                    Delta = Math.Sqrt(Math.Pow(CollidePos.X - Other.CollidePos.X, 2) + Math.Pow(CollidePos.Y - Other.CollidePos.Y, 2));
                    IsCollide = (Delta < (CollideRadius + Other.CollideRadius));
                    break;
                case CollideMode.Rect:
                    IsCollide = SpriteUtils.OverLapRect(CollideRect, Other.CollideRect);
                    break;
            }

            if (IsCollide)
            {
                OnCollision(Other);
                Other.OnCollision(this);
            }
        }

    }
    public virtual void Collision()
    {
        if ((Engine != null) && (!Deaded) && (CanCollision))
        {
            for (int i = 0; i < Engine.Count; i++)
            {
                this.Collision(Engine.List[i]);
            }
        }
    }

    public virtual void OnCollision(Sprite sprite)
    {

    }
    public List<Sprite> SpriteList
    {
        get => List;
        set
        {
            List = value;
        }
    }
    public Sprite Parent { get => parent; }
    public int Tag
    {
        get => tag; set { tag = value; }
    }
}

//SpriteEx
public class SpriteEx : Sprite
{
    public SpriteEx(Sprite Parent) : base(Parent)
    {
        Init();
    }
    public SpriteEx(Sprite Parent, Dictionary<string, Microsoft.Xna.Framework.Graphics.Texture2D> ImageLib,
                 string ImageName, float X, float Y, int Z = 0, int Width = 100, int Height = 100) : base(Parent)
    {
        Init();
        this.ImageLib = ImageLib;
        this.ImageName = ImageName;
        this.X = X;
        this.Y = Y;
        this.Z = Z;
        this.Width = Width;
        this.Height = Height;
    }

    private void Init()
    {
        GroupNumber = -1;
        OriginX = 0;
        OriginY = 0;
        ScaleX = 1;
        ScaleY = 1;
        Red = 255;
        Green = 255;
        Blue = 255;
        Alpha = 255;
        DrawMode = 1;
        FlipX = false;
        FlipY = false;
        PositionListX = new();
        PositionListY = new();
        CosTable256 = new double[256];
        for (int i = 0; i < 256; i++)
        {
            CosTable256[i] = Math.Cos(i * Math.PI / 128.0);
        }
    }


    public void Init(Dictionary<string, Microsoft.Xna.Framework.Graphics.Texture2D> ImageLib,
                     string ImageName, float X, float Y, int Z, int PatternWidth, int PatternHeight, int Width = 100, int Height = 100)
    {
        this.ImageLib = ImageLib;
        this.ImageName = ImageName;
        this.X = X;
        this.Y = Y;
        this.Z = Z;
        this.PatternWidth = PatternWidth;
        this.PatternHeight = PatternHeight;
        this.Width = Width;
        this.Height = Height;
    }
    public int OriginX { get; set; }
    public int OriginY { get; set; }
    public Vector2 Offset;
    public float ScaleX { get; set; }
    public float ScaleY { get; set; }
    public bool FlipX;
    public bool FlipY;
    public byte Red, Green, Blue, Alpha;

    public float X1, Y1, X2, Y2;
    public int DrawMode;
    public bool DoCenter;
    public int GoDirection;
    private bool selected;
    private int groupNumber;
    private Sprite AttachTo;
    public float Angle;
    
    private float SrcAngle, DestAngle;
    private List<float> PositionListX, PositionListY;
    public Microsoft.Xna.Framework.Rectangle CropRect;
    public ImageMode ImageMode;
    private const float PIConv256 = -40.743665431f; //-128.0 / PI;
    double[] CosTable256;

    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        
    }
    public int GetAngle256(int X, int Y)
    {
        return (int)(Math.Atan2(X, Y) * PIConv256) + 128;
    }
    public int GetAngle256(int SrcX, int SrcY, int DestX, int DestY)
    {
        return (int)(Math.Atan2(DestX - SrcX, DestY - SrcY) * PIConv256) + 128;
    }
    public double Cos256(int i)
    {
        return CosTable256[i & 255];
    }
    public double Sin256(int i)
    {
        return CosTable256[(i + 192) & 255];
    }
    public void LookAt(int TargetX, int TargetY)
    {
        Angle = GetAngle256((int)X, (int)Y, TargetX, TargetY) / -PIConv256;
    }
    public void TowardToAngle(int Direction256, float Speed, bool DoLookAt)
    {
        if (DoLookAt)
            Angle = Direction256 / PIConv256;
        X += (float)(Sin256(Direction256) * Speed);
        Y -= (float)(Cos256(Direction256) * Speed);
    }
    bool SameValue(float A, float B, float Epsilon)
    {
        if (Epsilon == 0)
            Epsilon = Math.Max(Math.Min(Math.Abs(A), Math.Abs(B)) * 0.0001f, 0.0001f);
        if (A > B)
            return (A - B) <= Epsilon;
        else
            return (B - A) <= Epsilon;
    }
    public void TowardToPos(int TargetX, int TargetY, float Speed, bool DoLookAt, bool Stop,float Delta)
    {
        if (DoLookAt)
            LookAt(TargetX, TargetY);
        int Direction256 = GetAngle256((int)X, (int)Y, TargetX, TargetY);
        GoDirection = Direction256;
        if (Stop)
        {
            if ((!SameValue(X, TargetX, Speed + 1)) || (!SameValue(Y, TargetY, Speed + 1)))
            {
                X += (float)(Sin256(Direction256) * Speed * Delta);
                Y -= (float)(Cos256(Direction256) * Speed * Delta);
            }
            else
            {
                X = TargetX;
                Y = TargetY;
            }
        }
        else
        {
            X += (float)(Sin256(Direction256) * Speed* Delta);
            Y -= (float)(Cos256(Direction256) * Speed * Delta);
        }
    }

    // toward(rotate self angle automation)(straight) move direction
    // and move by rotation speed(to destination angle)
    public void RotateToAngle(int Direction, float RotateSpeed, float MoveSpeed)
    {
        DestAngle = Direction;
        if (!SameValue(SrcAngle, DestAngle, RotateSpeed + 1))
        {
            if (SpriteUtils.AngleDiff(SrcAngle, DestAngle) > 0)
                SrcAngle = SrcAngle + RotateSpeed;
            if (SpriteUtils.AngleDiff(SrcAngle, DestAngle) < 0)
                SrcAngle = SrcAngle - RotateSpeed;
        }
        if (SrcAngle > 255)
            SrcAngle = SrcAngle - 255;
        if (SrcAngle < 0)
            SrcAngle = 255 + SrcAngle;
        Angle = SrcAngle / PIConv256;
        X += (float)(Sin256((int)SrcAngle) * MoveSpeed );
        Y -= (float)(Cos256((int)SrcAngle) * MoveSpeed);
    }

    // toward(rotate self angle automation)(straight) move  direction
    // and move by rotation speed(to destination position)
    public void RotateToPos(int TargetX, int TargetY, float RotateSpeed, float MoveSpeed,float Delta)
    {
        DestAngle = GetAngle256((int)X, (int)Y, TargetX, TargetY);
        if (!SameValue(SrcAngle, DestAngle, RotateSpeed + 1))
        {
            if (SpriteUtils.AngleDiff(SrcAngle, DestAngle) > 0)
                SrcAngle = SrcAngle + RotateSpeed;
            if (SpriteUtils.AngleDiff(SrcAngle, DestAngle) < 0)
                SrcAngle = SrcAngle - RotateSpeed;
        }
        if (SrcAngle > 255)
            SrcAngle = SrcAngle - 255;
        if (SrcAngle < 0)
            SrcAngle = 255 + SrcAngle;
        Angle = SrcAngle / PIConv256;
        X += (float)(Sin256((int)SrcAngle) * MoveSpeed * Delta);
        Y -= (float)(Cos256((int)SrcAngle) * MoveSpeed * Delta);
    }

    // move by rotation speed to destination angle,but not straight direction(no rotate self)
    // but can be custom angle
    public void CircleToAngle(int Direction, int LookAtX, int LookAtY, float RotateSpeed, float MoveSpeed, bool DoLookAt)
    {
        if (DoLookAt)
            LookAt(LookAtX, LookAtY);
        DestAngle = Direction;
        if (!SameValue(SrcAngle, DestAngle, RotateSpeed + 1))
        {
            if (SpriteUtils.AngleDiff(SrcAngle, DestAngle) > 0)
                SrcAngle = SrcAngle + RotateSpeed;
            if (SpriteUtils.AngleDiff(SrcAngle, DestAngle) < 0)
                SrcAngle = SrcAngle - RotateSpeed;
        }
        if (SrcAngle > 255)
            SrcAngle = SrcAngle - 255;
        if (SrcAngle < 0)
            SrcAngle = 255 + SrcAngle;
        X += (float)(Sin256((int)SrcAngle) * MoveSpeed);
        Y -= (float)(Cos256((int)SrcAngle) * MoveSpeed);
    }

    // move by rotation speed to destination position,but not straight direction(no rotae self)
    // but can be custom angle
    public void CircleToPos(int TargetX, int TargetY, int LookAtX, int LookAtY, float RotateSpeed, float MoveSpeed, bool DoLookAt)
    {
        if (DoLookAt)
            LookAt(LookAtX, LookAtY);
        DestAngle = GetAngle256((int)X, (int)Y, TargetX, TargetY);
        if (!SameValue(SrcAngle, DestAngle, RotateSpeed + 1))
        {
            if (SpriteUtils.AngleDiff(SrcAngle, DestAngle) > 0)
                SrcAngle = SrcAngle + RotateSpeed;
            if (SpriteUtils.AngleDiff(SrcAngle, DestAngle) < 0)
                SrcAngle = SrcAngle - RotateSpeed;
        }
        if (SrcAngle > 255)
            SrcAngle = SrcAngle - 255;
        if (SrcAngle < 0)
            SrcAngle = 255 + SrcAngle;
        X += (float)(Sin256((int)SrcAngle) * MoveSpeed);
        Y -= (float)(Cos256((int)SrcAngle) * MoveSpeed);
    }
    public void Attach(Sprite Sprite)
    {
        if (AttachTo == null)
            return;
        AttachTo = Sprite;
        float CurrentPositionX = AttachTo.X;
        float CurrentPositionY = AttachTo.Y;
        PositionListX.Add(CurrentPositionX);
        PositionListY.Add(CurrentPositionY);
        if (PositionListX.Count > 2)
        {
            float LastPositionX = PositionListX.Last();
            float LastPositionY = PositionListY.Last();
            float PredPositionX = PositionListX[1];
            float PredPositionY = PositionListY[1];
            X += (LastPositionX - PredPositionX);
            Y += (LastPositionY - PredPositionY);
            PositionListX.RemoveAt(0);
            PositionListY.RemoveAt(0);
        }
    }

    public static double Hypot(double x, double y)
    {
        return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
    }

    public double GetDistance(Sprite Sprite1, Sprite Sprite2)
    {
        return Hypot(Sprite1.X - Sprite2.X, Sprite1.Y - Sprite2.Y);
    }

    public void Detach()
    {
        AttachTo = null;
    }

    private void SetSelected(bool Selected)
    {
        if ((Selected != selected) && (Engine != null))
        {
            selected = Selected;
            if (Selected)
                Engine.CurrentSelected.Add(this);
            else
                Engine.CurrentSelected.Remove(this);
            Engine.ObjectsSelected = Engine.CurrentSelected.Count != 0;
        }
    }
    public bool Selected { get => selected; set => SetSelected(value); }
    public Rect GetBoundsRect()
    {
        Rect Result = new Rect();
        Result.Left = (int)X;
        Result.Top = (int)Y;
        Result.Right = (int)X + Width;
        Result.Bottom = (int)Y + Height;
        return Result;
    }

    private void SetGroupNumber(int AGroupNumber)
    {
        if ((AGroupNumber != groupNumber) && (Engine != null))
        {
            if (AGroupNumber >= 0)
                Engine[AGroupNumber].Remove(this);
            if (AGroupNumber >= 0)
                Engine[AGroupNumber].Add(this);
        }
    }
    public int GroupNumber { get => groupNumber; set => SetGroupNumber(value); }
    private void Collision_GetSpriteAt(float x, float y, SpriteEx Sprite)
    {
        if ((Sprite.Parent != null) && (!Sprite.Parent.Visible))
            return;
        Sprite.CanCollision = false;
        bool IsCollision;
        if (Sprite.CanCollision)
        {
            int SWidth = (int)Math.Round(Sprite.PatternWidth * Sprite.ScaleX);
            int SHeight = (int)Math.Round(Sprite.PatternHeight * Sprite.ScaleY);
            if (Sprite.DrawMode == 1)
            {
                X1 = X - Sprite.X - Sprite.Parent.X;
                Y1 = Y - Sprite.Y - Sprite.Parent.Y;
            }
            else
            if (Sprite.DoCenter)
            {
                X1 = X - Sprite.X - Sprite.Parent.X - Sprite.PatternWidth / 2;
                Y1 = Y - Sprite.Y - Sprite.Parent.Y - Sprite.PatternHeight / 2;
            }
            else
            {
                X1 = X - Sprite.X - Sprite.Parent.X - SWidth / 2;
                Y1 = Y - Sprite.Y - Sprite.Parent.Y - SHeight / 2;
            }
            X2 = Y1 * (float)Math.Sin(Sprite.Angle) + X1 * (float)Math.Cos(Sprite.Angle);
            Y2 = Y1 * (float)Math.Cos(Sprite.Angle) - X1 * (float)Math.Sin(Sprite.Angle);
            IsCollision = Sprite.Visible && SpriteUtils.PointInRect(new Vector2(X2, Y2), SpriteUtils.Rect(-SWidth / 2, -SHeight / 2, SWidth, SHeight));
            if (IsCollision)
            {
                //if ((Result = null) ||(Sprite.Z > Result.Z)) 
                //  Result = Sprite;
            }
        }
    }
    public Sprite GetSpriteAt(int X, int Y)
    {
        return null;
    }
    public override void DoDraw()
    {
        switch (ImageMode)
        {
            case ImageMode.SpriteSingle:
                Engine.Canvas.DrawEx(ImageLib[ImageName],
                                     X + Camera.X + Offset.X - Engine.Camera.X,
                                     Y + Camera.Y + Offset.Y - Engine.Camera.Y,
                                     OriginX, OriginY,
                                     ScaleX, ScaleY,
                                     Angle,
                                     FlipX, FlipY,
                                     Red, Green, Blue, Alpha,
                                     DoCenter,
                                     BlendMode);
                break;
            // 
            case ImageMode.SpriteSheet:

                Engine.Canvas.DrawPattern(ImageLib[ImageName],
                                          X + Camera.X + Offset.X - Engine.Camera.X,
                                          Y + Camera.Y + Offset.Y - Engine.Camera.Y,
                                          PatternIndex,
                                          PatternWidth, PatternHeight,
                                          OriginX, OriginY,
                                          ScaleX, ScaleY,
                                          Angle,
                                          FlipX, FlipY,
                                          Red, Green, Blue, Alpha,
                                          DoCenter,
                                          BlendMode);


                break;
            //
            case ImageMode.CropRect:
                Engine.Canvas.DrawCropArea(ImageLib[ImageName],
                                           X + Camera.X + Offset.X - Engine.Camera.X,
                                           Y + Camera.Y + Offset.Y - Engine.Camera.Y,
                                           CropRect,
                                           OriginX, OriginY,
                                           ScaleX, ScaleY,
                                           Angle,
                                           FlipX, FlipY,
                                           Red, Green, Blue, Alpha,
                                           DoCenter,
                                           BlendMode);

                break;
        }
    }

    public override void Draw()
    {
        if (Visible)
        {
            if (Engine != null)
            {
                if ((X + Offset.X > Engine.Camera.X - Width)
                 && (Y + Offset.Y > Engine.Camera.Y - Height)
                 && (X + Offset.X < Engine.Camera.X + Engine.VisibleWidth)
                 && (Y + Offset.Y < Engine.Camera.Y + Engine.VisibleHeight))
                {
                    DoDraw();
                    Engine.DrawCount++;
                }
            }
            if (DrawList != null)
            {
                for (int I = 0; I < DrawList.Count; I++)
                    ((SpriteEx)DrawList[I]).Draw();
            }
        }
    }

    public void SetColor(byte Red, byte Green, byte Blue, byte Alpha = 255)
    {
        this.Red = Red;
        this.Green = Green;
        this.Blue = Blue;
        this.Alpha = Alpha;
    }

    public void SetPattern(int Width, int Height)
    {
        this.PatternWidth = Width;
        this.PatternHeight = Height;
    }
}



public class AnimatedSprite : SpriteEx
{
    public AnimatedSprite(Sprite Parent) : base(Parent)
    {
        DoAnimate = true;
        PatternWidth = 64;
        PatternHeight = 64;
        PatternIndex = 0;
        Width = 64;
        Height = 64;
        ImageMode = ImageMode.SpriteSheet;
    }

    public AnimatedSprite(Sprite Parent, Dictionary<string, Microsoft.Xna.Framework.Graphics.Texture2D> ImageLib,
                string ImageName, float X, float Y, int Z = 0, int Width = 100, int Height = 100) : base(Parent)
    {
        DoAnimate = true;
        PatternWidth = 64;
        PatternHeight = 64;
        Width = 64;
        Height = 64;
        ImageMode = ImageMode.SpriteSheet;
        this.ImageLib = ImageLib;
        this.ImageName = ImageName;
        this.X = X;
        this.Y = Y;
        this.Z = Z;
        this.Width = Width;
        this.Height = Height;
    }

    public bool DoAnimate;
    public float AnimPos;
    private int animStart;
    public int AnimStart
    {
        get => animStart;
        set
        {
            if (animStart != value)
            {
                AnimPos = value;
                animStart = value;
            }
        }
    }
    public int AnimCount;
    public float AnimSpeed;
    public bool AnimLooped;
    // public bool AnimEnded;
    public AnimPlayMode AnimPlayMode;
    public int ImageIndex;
    private bool Flag1, Flag2;

    public bool AnimEnded()
    {
        bool Result;
        if ((int)AnimPos == (animStart + AnimCount - 1))
            Result = true;
        else
            Result = false;
        return Result;
    }
    public virtual void OnAnimStart()
    {
    }
    public virtual void OnAnimEnd()
    {
    }
    public override void DoMove(float Delta)
    {
        if (!DoAnimate)
            return;
        switch (AnimPlayMode)
        {
            case AnimPlayMode.Forward:
                AnimPos = AnimPos + AnimSpeed * Delta;
                if (AnimPos >= animStart + AnimCount)
                {
                    if ((int)AnimPos == animStart)
                        OnAnimStart();
                    if (AnimEnded())
                        OnAnimEnd();
                    if (AnimLooped)
                    {
                        AnimPos = animStart;
                    }
                    else
                    {
                        AnimPos = animStart + AnimCount - 1;
                        DoAnimate = false;
                    }
                }
                PatternIndex = (int)AnimPos;
                ImageIndex = (int)(AnimPos);
                break;
            //
            case AnimPlayMode.Backward:
                AnimPos = AnimPos - AnimSpeed * Delta;
                if (AnimPos < animStart)
                {
                    if (AnimLooped)
                    {
                        AnimPos = animStart + AnimCount;
                    }
                    else
                    {
                        // FAnimPos := FAnimStart;
                        AnimPos = animStart + AnimCount;
                        DoAnimate = false;
                    }
                }
                PatternIndex = (int)(AnimPos);
                ImageIndex = (int)(AnimPos);
                break;
            //
            case AnimPlayMode.PingPong:
                AnimPos = AnimPos + AnimSpeed * Delta;
                if (AnimLooped)
                {
                    if ((AnimPos > animStart + AnimCount - 1) || (AnimPos < animStart))
                        AnimSpeed = -AnimSpeed;
                }
                else
                {
                    if ((AnimPos > animStart + AnimCount) || (AnimPos < animStart))
                        AnimSpeed = -AnimSpeed;
                    if ((int)AnimPos == (animStart + AnimCount))
                        Flag1 = true;
                    if (((int)AnimPos == animStart) && (Flag1 = true))
                        Flag2 = true;
                    if ((Flag1 = true) && (Flag2 == true))
                    {
                        // FAnimPos := FAnimStart;
                        DoAnimate = false;
                        Flag1 = false;
                        Flag2 = false;
                    }
                }
                PatternIndex = (int)(AnimPos);
                ImageIndex = (int)(AnimPos);
                break;
        }
    }

    public void SetAnim(string AniImageName, int AniStart, int AniCount, float AniSpeed, bool AniLooped, bool DoFlipX, bool DoAnim, AnimPlayMode PlayMode = AnimPlayMode.Forward)
    {
        ImageName = AniImageName;
        animStart = AniStart;
        AnimCount = AniCount;
        AnimSpeed = AniSpeed;
        AnimLooped = AniLooped;
        FlipX = DoFlipX;
        DoAnimate = DoAnim;
        AnimPlayMode = PlayMode;
        if ((PatternIndex < animStart) || (PatternIndex >= AnimCount + animStart))
        {
            //PatternIndex = animStart % AnimCount;
            AnimPos = animStart;
            PatternIndex = (int)AnimPos;
        }
    }

    public void SetAnim(string AniImageName, int AniStart, int AniCount, float AniSpeed, bool AniLooped, AnimPlayMode PlayMode = AnimPlayMode.Forward)
    {
        ImageName = AniImageName;
        AnimStart = AniStart;
        AnimCount = AniCount;
        AnimSpeed = AniSpeed;
        AnimLooped = AniLooped;
        AnimPlayMode = PlayMode;
        if ((PatternIndex < animStart) || (PatternIndex >= AnimCount + AnimStart))
        {
            //PatternIndex = animStart % AnimCount;
            AnimPos = animStart;
        }
    }

}

public class ParticleSprite : AnimatedSprite
{
    public ParticleSprite(Sprite Parent) : base(Parent)
    {
        AccelX = 0;
        AccelY = 0;
        VelocityX = 0;
        VelocityY = 0;
        UpdateSpeed = 0;
        Decay = 0;
        LifeTime = 1;
    }
    public float AccelX;
    public float AccelY;
    public float VelocityX;
    public float VelocityY;
    public float UpdateSpeed;
    public float Decay;
    public float LifeTime;
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        X = X + VelocityX * UpdateSpeed * Delta;
        Y = Y + VelocityY * UpdateSpeed * Delta;
        VelocityX = VelocityX + AccelX * UpdateSpeed;
        VelocityY = VelocityY + AccelY * UpdateSpeed;
        LifeTime = LifeTime - Decay * Delta;
        if (LifeTime <= 0)
            Dead();
    }
}

public class PlayerSprite : AnimatedSprite
{
    public PlayerSprite(Sprite Parent) : base(Parent)
    {
    }
    private float speed;
    public float Acc;
    public float Dcc;
    public float MaxSpeed;
    public float MinSpeed;
    public float VelocityX;
    public float VelocityY;
    private int direction;
    public int Direction
    {
        get => direction;
        set
        {
            direction = value;
            VelocityX = (float)Cos256(direction + 192) * speed;
            VelocityY = (float)Sin256(direction + 192) * speed;
        }
    }
    public float Speed
    {
        get => speed;
        set
        {
            if (speed > MaxSpeed)
                speed = MaxSpeed;
            else if (speed < MinSpeed)
                speed = MinSpeed;
            speed = value;
            VelocityX = (float)Cos256(direction + 192) * speed;
            VelocityY = (float)Sin256(direction + 192) * speed;
        }
    }

    public void FlipXDirection()
    {
        if (direction >= 64)
            direction = 192 + (64 - direction);
        else if (direction > 0)
            direction = 256 - direction;
    }

    public void FlipYDirection()
    {
        if (direction > 128)
            direction = 128 + (256 - direction);
        else
            direction = 128 - direction;
    }

    public virtual void Accelerate()
    {
        if (speed != MaxSpeed)
        {
            speed = speed + Acc;
            if (speed > MaxSpeed)
                speed = MaxSpeed;
            VelocityX = (float)Cos256(direction + 192) * speed;
            VelocityY = (float)Sin256(direction + 192) * speed;
        }
    }

    public virtual void Deccelerate()
    {
        if (speed != MinSpeed)
        {
            speed = speed - Dcc;
            if (speed < MinSpeed)
                speed = MinSpeed;
            VelocityX = (float)Cos256(direction + 192) * speed;
            VelocityY = (float)Sin256(direction + 192) * speed;
        }
    }

    public float Acceleration
    {
        get => Acc; set => Acc = value;

    }
    public float Decceleration
    {
        get => Dcc; set => Dcc = value;

    }
    public void UpdatePos(float Delta)
    {
        base.DoMove(Delta);
        X += VelocityX * Delta;
        Y += VelocityY * Delta;
    }
}

public class JumperSprite : PlayerSprite
{
    public JumperSprite(Sprite Parent) : base(Parent)
    {
        VelocityX = 0;
        VelocityY = 0;
        //  MaxSpeed= MaxSpeed;
        Direction = 0;
        jumpState = JumpState.jsNone;
        JumpSpeed = 0.25f;
        JumpHeight = 8;
        Acceleration = 0.2f;
        Decceleration = 0.2f;
        MaxFallSpeed = 5;
        DoJump = false;
    }
    public int JumpCount;
    public float JumpSpeed;
    public float JumpHeight;
    public float MaxFallSpeed;
    public bool DoJump;
    private JumpState jumpState;
    public JumpState JumpState
    {
        get => jumpState;
        set
        {
            if (jumpState != value)
            {
                jumpState = value;
                switch (value)
                {
                    case JumpState.jsNone:
                    case JumpState.jsFalling:
                        VelocityY = 0;
                        break;
                }
            }
        }
    }

    public override void Accelerate()
    {
        if (Speed != MaxSpeed)
        {
            Speed += Acc;
            if (Speed > MaxSpeed)
                Speed = MaxSpeed;
            // VelocityX := Cos256(FDirection) * Speed;
        }
    }
    public override void Deccelerate()
    {
        if (Speed != MinSpeed)
        {
            Speed -= Dcc;
            if (Speed < MinSpeed)
                Speed = MinSpeed;
        }
    }
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        switch (jumpState)
        {
            case JumpState.jsNone:
                if (DoJump)
                {
                    jumpState = JumpState.jsJumping;
                    VelocityY = -JumpHeight;
                }
                break;

            case JumpState.jsJumping:

                Y += VelocityY * Delta;
                VelocityY += JumpSpeed;
                if (VelocityY > 0)
                    jumpState = JumpState.jsFalling;
                break;

            case JumpState.jsFalling:
                Y = Y + VelocityY * Delta;
                VelocityY = VelocityY + JumpSpeed;
                if (VelocityY > MaxFallSpeed)
                    VelocityY = MaxFallSpeed;
                break;
        }
    }
}

public class PathSprite : AnimatedSprite
{
    public PathSprite(Sprite Parent) : base(Parent)
    {
        Segment = 0;
        Distance = 0;
        Looped = false;
        MoveSpeed = 0.01f;
    }
    public bool Looped;
    public int Segment;
    public float Distance;
    public float MoveSpeed;
    public Vector2[] ControlPoints = new Vector2[10000];
    private int Calculate(int P0, int P1, int P2, int P3, float T)
    {
        return (int)((2 * P1 +
                   (-P0 + P2) * T +
                   (2 * P0 - 5 * P1 + 4 * P2 - P3) * T * T +
                   (-P0 + 3 * P1 - 3 * P2 + P3) * T * T * T) / 2);
    }
    private Vector2 CalculatePoint(Vector2 CP0, Vector2 CP1, Vector2 CP2, Vector2 CP3, float T)
    {
        Vector2 Result;
        Result.X = Calculate((int)CP0.X, (int)CP1.X, (int)CP2.X, (int)CP3.X, T);
        Result.Y = Calculate((int)CP0.Y, (int)CP1.Y, (int)CP2.Y, (int)CP3.Y, T);
        return Result;
    }

    public Vector2 GetPoint(int Index)
    {
        if (Index < 0)
        {
            return ControlPoints[ControlPoints.Length + Index];
        }
        else if (Index > ControlPoints.Length - 1)
        {
            return ControlPoints[Index - ControlPoints.Length];
        }
        else
        {
            return ControlPoints[Index];
        }
    }
    public Vector2 Position
    {
        get
        {
            if (Distance > 1.0)
            {
                Distance -= 1.0f;
                Segment++;
                if (Looped)
                {
                    if (Segment == (ControlPoints.Length))
                        Segment = 0;
                }
            }
            return CalculatePoint(GetPoint(Segment - 1), GetPoint(Segment),
                                GetPoint(Segment + 1), GetPoint(Segment + 2),
                                Distance);
        }
    }
    public void AddPoint(int X, int Y)
    {
        Vector2 Point;
        Point.X = X;
        Point.Y = Y;
        AddPoint(Point);
    }
    public void AddPoint(Vector2 Point)
    {
        Array.Resize(ref ControlPoints, ControlPoints.Length + 1);
        ControlPoints[ControlPoints.Length - 1] = Point;
    }

    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        Distance = Distance + MoveSpeed * Delta;
        X = Position.X;
        Y = Position.Y;
    }
}

public class NPathSprite : PathSprite
{
    public NPathSprite(Sprite Parent) : base(Parent)
    {
        Distance = 0;
        MoveSpeed = 0;
        UpdateSpeed = 0.01f;
        MaxParameter = 100;

    }
    public NURBSCurveEx Path;
    public float Distance;
    public int MaxParameter;
    public float Accel;
    public float UpdateSpeed;
    Vector2 Position
    {
        get => Path.GetXY(Distance / MaxParameter);
    }
    public void LookAt(float anAngle)
    {
        Angle = Path.GetTangent(Distance / MaxParameter) + anAngle;

    }
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        Distance = Distance + MoveSpeed * UpdateSpeed * Delta;
        MoveSpeed = MoveSpeed + Accel * UpdateSpeed * Delta;
        if (Distance > MaxParameter)
            Distance = MaxParameter;
        if (Distance < 0)
            Distance = 0;
        Vector2 Point = Position;
        X = Point.X;
        Y = Point.Y;
    }
}


public class BackgroundSprite : AnimatedSprite
{
    public BackgroundSprite(Sprite Parent) : base(Parent)
    {
        ImageMode = ImageMode.SpriteSingle;
        Tiled = true;
        TileMode = TileMode.Vertical;
    }
    public TileMode TileMode;
    public bool Tiled;
    private int MapW, MapH;
    private int mapWidth, mapHeight;

    public int MapWidth
    {
        get => mapWidth;
        set => SetMapSize(value, mapHeight);
    }
    public int MapHeight
    {
        get => mapHeight;
        set => SetMapSize(mapWidth, value);
    }
    public void SetMapSize(int MapWidth, int MapHeight)
    {
        MapW = Width * MapWidth;
        MapH = Height * MapHeight;
        if ((mapWidth != MapWidth) || (mapHeight != MapHeight))
        {
            if ((MapWidth <= 0) || (MapHeight <= 0))
            {
                MapWidth = 0;
                MapHeight = 0;
            }
            mapWidth = MapWidth;
            mapHeight = MapHeight;
        }
    }

    public override void Draw()
    {
        if (Visible)
        {
            if (Engine != null)
            {
                DoDraw();
                Engine.DrawCount++;
            }

            if (DrawList != null)
            {
                for (int i = 0; i < DrawList.Count; i++)
                    ((BackgroundSprite)DrawList[i]).Draw();
            }
        }
    }

    public override void DoDraw()
    {
        int ChipWidth = this.Width;
        int ChipHeight = this.Height;
        int dWidth = (Engine.VisibleWidth + ChipWidth) / ChipWidth + 1;
        int dHeight = (Engine.VisibleHeight + ChipHeight) / ChipHeight + 1;

        int _x = (int)(-Engine.Camera.X - X);
        int _y = (int)(-Engine.Camera.Y - Y);
        int OfsX = _x % ChipWidth;
        int OfsY = _y % ChipHeight;
        int StartX = _x / ChipWidth;
        int StartX_ = 0;
        if (StartX < 0)
        {
            StartX_ = -StartX;
            StartX = 0;
        }
        int StartY = _y / ChipHeight;
        int StartY_ = 0;
        if (StartY < 0)
        {
            StartY_ = -StartY;
            StartY = 0;
        }
        int EndX = Math.Min(StartX + mapWidth - StartX_, dWidth);
        int EndY = Math.Min(StartY + mapHeight - StartY_, dHeight);

        switch (TileMode)
        {
            case TileMode.Horizontal:
                dWidth = (Engine.VisibleWidth + ChipWidth) / ChipWidth + 1;
                dHeight = -1;
                break;
            case TileMode.Vertical:
                dWidth = -1;
                dHeight = (Engine.VisibleHeight + ChipHeight) / ChipHeight + 1;
                break;
            case TileMode.Full:
                dWidth = (Engine.VisibleWidth + ChipWidth) / ChipWidth + 1;
                dHeight = (Engine.VisibleHeight + ChipHeight) / ChipHeight + 1;
                break;
        }
        if (Tiled)
        {
            for (int cy = -1; cy <= dHeight; cy++)
            {
                for (int cx = -1; cx <= dWidth; cx++)
                {
                    switch (TileMode)
                    {
                        case TileMode.Horizontal:
                            Engine.Canvas.DrawEx(ImageLib[ImageName],
                                                 cx * ChipWidth + OfsX - Offset.X, _y - Offset.Y,
                                                 OriginX, OriginY,
                                                 ScaleX, ScaleY,
                                                 Angle,
                                                 FlipX, FlipY,
                                                 Red, Green, Blue, Alpha,
                                                 DoCenter,
                                                 BlendMode);
                            break;

                        case TileMode.Vertical:
                            Engine.Canvas.DrawEx(ImageLib[ImageName],
                                                 _x - Offset.X, cy * ChipHeight + OfsY - Offset.Y,
                                                 OriginX, OriginY,
                                                 ScaleX, ScaleY,
                                                 Angle,
                                                 FlipX, FlipY,
                                                 Red, Green, Blue, Alpha,
                                                 DoCenter,
                                                 BlendMode);
                            break;

                        case TileMode.Full:
                            Engine.Canvas.DrawEx(ImageLib[ImageName],
                                                cx * ChipWidth + OfsX - Offset.X, cy * ChipHeight + OfsY - Offset.Y,
                                                OriginX, OriginY,
                                                ScaleX, ScaleY,
                                                Angle,
                                                FlipX, FlipY,
                                                Red, Green, Blue, Alpha,
                                                DoCenter,
                                                BlendMode);

                            break;
                    }
                }
            }
        }
        else
        {
            for (int cy = StartY; cy < EndY; cy++)
            {
                for (int cx = StartX; cx < EndX; cx++)
                {
                    Engine.Canvas.DrawEx(ImageLib[ImageName],
                                               cx * ChipWidth + OfsX - Offset.X, cy * ChipHeight + OfsY - Offset.Y,
                                               OriginX, OriginY,
                                               ScaleX, ScaleY,
                                               Angle,
                                               FlipX, FlipY,
                                               Red, Green, Blue, Alpha,
                                               DoCenter,
                                               BlendMode);


                }
            }
        }


    }

}


