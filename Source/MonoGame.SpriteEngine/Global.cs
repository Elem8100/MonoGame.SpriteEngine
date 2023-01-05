using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WzComparerR2.Rendering;
using System.Drawing;
namespace MonoGame.SpriteEngine;

public  class EngineFunc
{
    public static MonoSpriteEngine SpriteEngine;
    public static MonoSpriteEngine BackgroundEngine;
    public static GameCanvas Canvas;
    public static Dictionary<string, Texture2D> ImageLib;
    public static Dictionary<string ,XnaFont> Fonts=new();
    private static float FixedUpdateDelta = 0.016666f;
    // helper variables for the fixed update
    private static float PreviousTime = 0;
    private static float Accumulator = 0.0f;
    private static float ALPHA = 0;


   
    public static void FixedUpdate(GameTime gameTime,  params Action[] FuncArray)
    {
        if (PreviousTime == 0)
        {
            PreviousTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
        }

        float Now = (float)gameTime.TotalGameTime.TotalMilliseconds;
        float FrameTime = Now - PreviousTime;
        if (FrameTime > 0.016666f)
        {
            FrameTime = 0.016666f;
        }

        PreviousTime = Now;
        Accumulator += FrameTime;
        while (Accumulator >= FixedUpdateDelta)
        {
            
            for (int i = 0; i < FuncArray.Length; i++)
                FuncArray[i]();
            Accumulator -= FixedUpdateDelta;
        }
    }

    public static void Init(string LoadImagePath, GraphicsDevice graphicsDevice)
    {
        SpriteEngine = new MonoSpriteEngine(null);
        BackgroundEngine = new MonoSpriteEngine(null);
        Canvas = new GameCanvas(graphicsDevice);
        SpriteEngine.Canvas = Canvas;
        BackgroundEngine.Canvas = Canvas;
        ImageLib = new Dictionary<string, Texture2D>();
        ImageLib.LoadTextures(LoadImagePath, graphicsDevice);
    }

    public static void AddFont(GraphicsDevice GraphicsDevice,string KeyName,string FontName,int Size) 
    {
        var Font=new XnaFont(GraphicsDevice, new Font(FontName, Size));
        Fonts.Add(KeyName, Font);
    }
}
