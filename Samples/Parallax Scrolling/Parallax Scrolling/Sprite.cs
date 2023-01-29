using MonoGame.SpriteEngine;
namespace Parallax_Scrolling;
public class Background : BackgroundSprite
{
    public Background(Sprite Parent) : base(Parent)
    {
        
    }
    public float Speed;
    public override void DoMove(float MoveCount)
    {
        base.DoMove(MoveCount);
        X += Speed;
    }
    public static void CreateLayers()
    {

        int X = 0, Y = 0, W = 0, H = 0; float speed = 0;
        for (int i = 0; i <= 5; i++)
        {
            var Background = new Background(EngineFunc.SpriteEngine);
            switch (i)
            {
                case 0: Y = -40; W = 800; H = 452; speed = 1; break;
                case 1: Y = -40; W = 800; H = 452; speed = 1.5f; break;
                case 2: Y = -140; W = 1598; H = 491; speed = 2; break;
                case 3: Y = -490; W = 902; H = 300; speed = 2.5f; break;
                case 4: Y = -590; W = 802; H = 300; speed = 3f; break;
                case 5: Y = -520; W = 500; H = 300; speed = 3.5f; break;
            }
            Background.Init(EngineFunc.ImageLib, "Layer" + i.ToString() + ".png", X, Y, i, 0, 0, W, H);
            Background.Speed = speed;
            Background.TileMode = TileMode.Horizontal;
         }
    }

}
