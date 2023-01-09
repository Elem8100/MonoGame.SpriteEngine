using MonoGame.SpriteEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Xml;
using Input = Microsoft.Xna.Framework.Input.Keys;
using Keyboard = SpriteEngine.Keyboard;
using System.Xml;
using Microsoft.Xna.Framework;
namespace SpriteSheet;
public enum State { Stand, WalkLeft, WalkRight, Attack }

public class Sprite2 : AnimatedSprite
{
    public Sprite2(Sprite Parent) : base(Parent)
    {
        SpriteSheetMode = SpriteSheetMode.VariableSize;
    }
    public int ID;
    State State;
    public override void DoMove(float Delta)
    {
        base.DoMove(Delta);
        Random Random = new Random();

        switch (Random.Next(0, 250))
        {
            case 50: case 200:
                FlipX = true;
                State = State.WalkRight;
                break;
            case 10:case 220:
                FlipX = false;
                State = State.WalkLeft;
                break;
            case 100:
                State = State.Stand;
                break;
            case 80: 
                State = State.Attack;
                break;
        }

        if (ID == 0)
        {
            switch (State)
            {
                case State.WalkRight:
                    PlayAnimation("EggWalk", 0.13f, true, FlipX, true);
                    X += 1.5f*Delta;
                    break;
                case State.WalkLeft:
                    X -= 1.5f*Delta;
                    PlayAnimation("EggWalk", 0.13f, true, FlipX, true);
                    break;
                case State.Stand:
                    X -= 0;
                    PlayAnimation("EggStand", 0.13f, true, FlipX, true);
                    break;
                case State.Attack:
                    X -= 0;
                    PlayAnimation("EggAttack", 0.13f, true, FlipX, true);
                    break;
            }
        }

        if (ID == 1)
        {
            switch (State)
            {
                case State.WalkRight:
                    PlayAnimation("TomatoWalk", 0.13f, true, FlipX, true);
                    X += 1.5f * Delta;
                    break;
                case State.WalkLeft:
                    X -= 1.5f * Delta;
                    PlayAnimation("TomatoWalk", 0.13f, true, FlipX, true);
                    break;
                case State.Stand:
                    X -= 0;
                    PlayAnimation("TomatoStand", 0.13f, true, FlipX, true);
                    break;
                case State.Attack:
                    X -= 0;
                    PlayAnimation("TomatoAttack", 0.18f, true, FlipX, true);
                    break;
            }
        }
        if (ID == 2)
        {
            switch (State)
            {
                case State.WalkRight:
                    PlayAnimation("RabbitWalk", 0.13f, true, FlipX, true);
                    X += 1.5f * Delta;
                    break;
                case State.WalkLeft:
                    X -= 1.5f * Delta;
                    PlayAnimation("RabbitWalk", 0.13f, true, FlipX, true);
                    break;
                case State.Stand:
                    X -= 0;
                    PlayAnimation("RabbitStand", 0.13f, true, FlipX, true);
                    break;
                case State.Attack:
                   State=State.Stand;
                    break;
            }
        }


        if (X<50)
        {  
            State=State.WalkRight;
            FlipX=true;
        }
        if (X >750)
        { 
            FlipX=false;
            State = State.WalkLeft;
        }
    }
}

public class GameFunc
{

    public static void LoadXml(string FileName, string ImageName, string AnimationName, string StandName, string WalkName, string AttackName)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(FileName);
        var node = doc.SelectSingleNode("sym/frame");
        foreach (XmlNode i in node.ChildNodes)
        {
            var Name = i.Name + "aaaaaaaaaaaa";
            int OriginX = Convert.ToInt32(i.Attributes["cx"].InnerText);
            int OriginY = Convert.ToInt32(i.Attributes["cy"].InnerText);
            int X = Convert.ToInt32(i.Attributes["x"].InnerText);
            int Y = Convert.ToInt32(i.Attributes["y"].InnerText);
            int Width = Convert.ToInt32(i.Attributes["sx"].InnerText);
            int Height = Convert.ToInt32(i.Attributes["sy"].InnerText);
            if (Name.Substring(0, StandName.Length) == StandName)
            {
                AnimatedSprite.AddFrame(AnimationName + "Stand", ImageName, OriginX, OriginY, new Rectangle(X, Y, Width, Height), 0);
            }

            if (Name.Substring(0, WalkName.Length) == WalkName)
            {
                AnimatedSprite.AddFrame(AnimationName + "Walk", ImageName, OriginX, OriginY, new Rectangle(X, Y, Width, Height), 0);
            }
            if (Name.Substring(0, AttackName.Length) == AttackName)
            {
                AnimatedSprite.AddFrame(AnimationName + "Attack", ImageName, OriginX, OriginY, new Rectangle(X, Y, Width, Height), 0);
            }
        }
    }
    public static void CeateGame()
    {
        LoadXml("Rabbit.xml", "Rabbit.png", "Rabbit", "stand2", "walk", "");
        LoadXml("Tomato.xml", "Tomato.png", "Tomato", "stand1", "walk1", "stand2");
        LoadXml("Egg.xml", "Egg.png", "Egg", "stand4", "walk1", "spellcast");
        AnimatedSprite.FinishFrames();
        for (int i = 0; i <= 2; i++)
        {
            var Sprite2 = new Sprite2(EngineFunc.SpriteEngine);
            Sprite2.Init(EngineFunc.ImageLib, "", 100, 140 + 140 * i);
            Sprite2.ID = i;
        }
    }

}
