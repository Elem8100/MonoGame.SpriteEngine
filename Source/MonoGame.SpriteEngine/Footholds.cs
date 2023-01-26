using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
namespace MonoGame.SpriteEngine;

public class Foothold
{
    public Foothold(Vector2 P1, Vector2 P2, int ID)
    {
        p1 = P1;
        p2 = P2;
        id = ID;
    }
    private Vector2 p1, p2;
    private int id;
    public int ID { get => id; set => id = value; }
    public int PlatformID;
    public int Z;
    public int NextID;
    public int PrevID;
    public int X1 { get; }
    public int X2 { get; }
    public int Y1 { get; }
    public int Y2 { get; }
    public bool IsWall()
    {
        if (p1.X == p2.X)
            return true;
        return false;
    }
    public Foothold Prev;
    public Foothold Next;
}

public class FootholdTree
{
    private List<Foothold> footholds = new();
    private Vector2 P1, P2;
    public static FootholdTree Instance;
    public static List<int> MinX1, MaxX2;
    public List<Foothold> Footholds { get => footholds; }
    public FootholdTree(Vector2 P1, Vector2 P2)
    {
        this.P1 = P1;
        this.P2 = P2;
    }
    public Foothold GetPrev(Foothold FH)
    {
        Foothold Result = null;
        foreach (var F in footholds)
        {
            if (F.ID == FH.PrevID)
                Result = F;
        }
        return Result;
    }
    public Foothold GetNext(Foothold FH)
    {
        Foothold Result = null;
        foreach (var F in footholds)
        {
            if (F.ID == FH.NextID)
                Result = F;
        }
        return Result;
    }

    public Vector2? FindBelow(Vector2 Pos, ref Foothold FH)
    {
        bool First = true;
        double MaxY = 0, CMax = 0;
        Vector2? Result = null;
        Vector2 NewPos = new(0, 0);
        int X = (int)Pos.X;
        int Y = (int)Pos.Y;

        foreach (var F in footholds)
        {
            if (((X >= F.X1) && (X <= F.X2)) || ((X >= F.X2) && (X <= F.X1)))
            {
                if (First)
                {
                    if (F.X1 == F.X2)
                        continue;
                    MaxY = (F.Y1 - F.Y2) / (F.X1 - F.X2) * (X - F.X1) + F.Y1;
                    FH = F;
                    if (MaxY >= Y)
                        First = false;
                }
                else
                {
                    if (F.X1 == F.X2)
                        continue;
                    CMax = (F.Y1 - F.Y2) / (F.X1 - F.X2) * (X - F.X1) + F.Y1;
                    if ((CMax < MaxY) && (CMax >= Y))
                    {
                        FH = F;
                        MaxY = CMax;
                    }
                }
            }
        }

        if (!First)
        {
            NewPos.X = X;
            NewPos.Y = (float)MaxY;
            Result = NewPos;
        }
        else
        {
            Result = null;
        }
        return Result;
    }

    public Foothold FindWallR(Vector2 P)
    {
        Foothold Result = null;
        bool First = true;
        double MaxX = 0, CMax = 0;
        int X = (int)P.X;
        int Y = (int)P.Y;
        foreach (var F in footholds)
        {
            if ((F.IsWall()) && (F.X1 <= P.X) && (F.Y1 <= P.Y) && (F.Y2 >= P.Y))
            {
                if (First)
                {
                    MaxX = F.X1;
                    Result = F;
                    if (MaxX <= X)
                        First = false;
                }
                else
                {
                    CMax = F.X1;
                    if ((CMax > MaxX) && (CMax <= X))
                    {
                        MaxX = CMax;
                        Result = F;
                    }
                }
            }
        }
        return Result;
    }

    public Foothold FindWallL(Vector2 P)
    {
        Foothold Result = null;
        bool First = true;
        double MaxX = 0, CMax = 0;
        int X = (int)P.X;
        int Y = (int)P.Y;
        foreach (var F in footholds)
        {
            if ((F.IsWall()) && (F.X1 >= P.X) && (F.Y1 >= P.Y) && (F.Y2 <= P.Y))
            {
                if (First)
                {
                    MaxX = F.X1;
                    Result = F;
                    if (MaxX >= X)
                        First = false;
                }
                else
                {
                    CMax = F.X1;
                    if ((CMax < MaxX) && (CMax >= X))
                    {
                        MaxX = CMax;
                        Result = F;
                    }
                }
            }
        }
        return Result;
    }


    public bool ClosePlatform(Foothold FH)
    {
        int Count = 0;
        bool Result = false;
        foreach (var F in footholds)
        {
            if ((F.PlatformID == FH.PlatformID) && (F.IsWall()))
                Count += 1;
        }
        if (Count == 2)
            Result = true;
        return Result;
    }

    public void Insert(Foothold F)
    {
        footholds.Add(F);

    }
    public static void CreateFootholds()
    {
        if (Instance == null)
        {
            Instance = new FootholdTree(new Vector2(100, 10), new Vector2(-100, -100));
            MinX1 = new();
            MaxX2 = new();
        }
        else
        {
            Instance.Footholds.Clear();
            MinX1.Clear();
            MaxX2.Clear();
        }
        /*
         int X1=0,Y1=0,X2=0,Y2=0;
         foothold FH;
        foreach(var Iter in TMap.ImgFile.Child['foothold'].Children)
        {
           foreach(var Iter2 in Iter.Children )
           {
              foreach(var Iter2 in Iter.Children )
              {
                 X1 = Iter3.Get("x1", "0");
                 Y1 = Iter3.Get("y1", "0");
                 X2 = Iter3.Get("x2", "0");
                 Y2 = Iter3.Get("y2", "0");
                 FH = new(Point(X1, Y1), Point(X2, Y2), 0);
                 FH.NextID = Iter3.Get("next", "0");
                 FH.PrevID = Iter3.Get("prev", "0");
                 FH.PlatformID = Iter2.Name.ToString();
                 FH.ID := Iter3.Name.ToString();
                 FH.Z := Iter.Name.ToString();
                 This.Insert(FH);
                 MinX1.Add(X1);
                 MaxX2.Add(X2);
              }
           } 
        }

        */
        List<Foothold> FHs;
        MinX1.Sort();
        MaxX2.Sort();
        FHs = Instance.Footholds;
        for (int i = 0; i < FHs.Count; i++)
        {
            for (int j = 0; j < FHs.Count; j++)
            {
                if (i == j)
                    continue;
                if (FHs[j].ID == FHs[i].PrevID)
                    FHs[i].Prev = FHs[j];
                if (FHs[j].ID == FHs[i].NextID)
                    FHs[i].Next = FHs[j];
            }
        }
    }

}
