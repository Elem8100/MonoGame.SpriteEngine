﻿namespace MonoGame.SpriteEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using WzComparerR2.Rendering;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.Windows.Forms;

public enum BlendMode
{
    Normal,
    LightMap,
    Multiply,
    AddtiveColor,
    Lighten,
    Multiply2x,
    LinearDodge,
    LinearBurn,
    Difference,
    Subtractive,
}
public class GameCanvas
{
    public GameCanvas(GraphicsDevice graphicGDevice)
    {


        blendState[0] = new BlendState
        {
            ColorSourceBlend = Blend.Zero,
            ColorDestinationBlend = Blend.SourceColor
        };
        //Light Map 
        blendState[1] = new BlendState
        {
            AlphaBlendFunction = BlendFunction.ReverseSubtract,
            AlphaSourceBlend = Blend.SourceAlpha,
            AlphaDestinationBlend = Blend.Zero,
            //deal with color
            ColorBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.DestinationColor,
            ColorDestinationBlend = Blend.Zero
        };
        // multiply 
        blendState[2] = new BlendState
        {
            ColorBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.DestinationColor,
            ColorDestinationBlend = Blend.Zero
        };

        //addtiveColor
        blendState[3] = new BlendState
        {
            ColorBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.One,
            AlphaSourceBlend = Blend.One,
            AlphaDestinationBlend = Blend.Zero,
            AlphaBlendFunction = BlendFunction.Add
        };
        //lighten
        blendState[4] = new BlendState
        {
            ColorSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.One,
            ColorBlendFunction = BlendFunction.Max,
            AlphaSourceBlend = Blend.One,
            AlphaDestinationBlend = Blend.One,
            AlphaBlendFunction = BlendFunction.Max
        };

        //Multiply2x
        blendState[5] = new BlendState
        {
            ColorSourceBlend = Blend.DestinationColor,
            ColorDestinationBlend = Blend.SourceColor,
            ColorBlendFunction = BlendFunction.Add
        };

        //LinearDodge
        blendState[6] = new BlendState
        {
            ColorSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.One,
            ColorBlendFunction = BlendFunction.Add
        };
        //LinearBurn
        blendState[7] = new BlendState
        {
            ColorSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.One,
            ColorBlendFunction = BlendFunction.ReverseSubtract
        };
        //Difference
        blendState[8] = new BlendState
        {
            ColorSourceBlend = Blend.InverseDestinationColor,
            ColorDestinationBlend = Blend.InverseSourceColor,
            ColorBlendFunction = BlendFunction.Add
        };
        //Subtractive
        blendState[9] = new BlendState
        {
            ColorSourceBlend = Blend.SourceAlpha,
            ColorDestinationBlend = Blend.One,
            ColorBlendFunction = BlendFunction.ReverseSubtract,
            AlphaSourceBlend = Blend.SourceAlpha,
            AlphaDestinationBlend = Blend.One,
            AlphaBlendFunction = BlendFunction.ReverseSubtract
        };
        //   blendState[4] = BlendState.Additive;
        this.GraphicsDevice = graphicGDevice;
        SpriteBatch = new SpriteBatchEx(this.GraphicsDevice);

    }
    public GraphicsDevice GraphicsDevice;
    private BlendState[] blendState = new BlendState[15];
    public SpriteBatchEx SpriteBatch;


    public void Draw(Texture2D Texture, float X, float Y, BlendMode BlendMode = BlendMode.Normal)
    {
        DrawEx(Texture, X, Y, 0, 0, 1, 1, 0, false, false, 255, 255, 255, 255, false, BlendMode);
    }
    public void Draw(Texture2D Texture, float X, float Y, bool FlipX, bool FlipY, BlendMode BlendMode = BlendMode.Normal)
    {
        DrawEx(Texture, X, Y, 0, 0, 1, 1, 0, FlipX, FlipY, 255, 255, 255, 255, false, BlendMode);
    }

    public void DrawColor(Texture2D Texture, float X, float Y, byte Red, byte Green, byte Blue, byte Alpha = 255, BlendMode BlendMode = BlendMode.Normal)
    {
        DrawEx(Texture, X, Y, 0, 0, 1, 1, 0, false, false, Red, Green, Blue, Alpha, false, BlendMode);
    }
    public void DrawScale(Texture2D Texture, float X, float Y, float ScaleX, float ScaleY, BlendMode BlendMode = BlendMode.Normal)
    {
        DrawEx(Texture, X, Y, 0, 0, ScaleX, ScaleY, 0, false, false, 255, 255, 255, 255, false, BlendMode);
    }


    private void SetBlendMode(BlendMode BlendMode)
    {
        if (BlendMode == BlendMode.Normal)
        {
            SpriteBatch.Begin();

        }
        else
        {
            switch (BlendMode)
            {
                case BlendMode.LightMap:
                    SpriteBatch.Begin(SpriteSortMode.Deferred, blendState[1]);
                    break;
                case BlendMode.Multiply:
                    SpriteBatch.Begin(SpriteSortMode.Deferred, blendState[2]);
                    break;
                case BlendMode.AddtiveColor:
                    SpriteBatch.Begin(SpriteSortMode.Deferred, blendState[3]);
                    break;
                case BlendMode.Lighten:
                    SpriteBatch.Begin(SpriteSortMode.Deferred, blendState[4]);
                    break;
                case BlendMode.Multiply2x:
                    SpriteBatch.Begin(SpriteSortMode.Deferred, blendState[5]);
                    break;
                case BlendMode.LinearDodge:
                    SpriteBatch.Begin(SpriteSortMode.Deferred, blendState[6]);
                    break;

                case BlendMode.LinearBurn:
                    SpriteBatch.Begin(SpriteSortMode.Deferred, blendState[7]);
                    break;
                case BlendMode.Difference:
                    SpriteBatch.Begin(SpriteSortMode.Deferred, blendState[8]);
                    break;

                case BlendMode.Subtractive:
                    SpriteBatch.Begin(SpriteSortMode.Deferred, blendState[9]);
                    break;

            }
        }

    }
    private SpriteEffects SetFlip(bool FlipX, bool FlipY)
    {
        SpriteEffects Flip;
        Flip = SpriteEffects.None;
        if (FlipX)
            Flip = SpriteEffects.FlipHorizontally;
        if (FlipY)
            Flip = SpriteEffects.FlipVertically;
        if (FlipX && FlipY)
            Flip = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
        return Flip;

    }
    public void DrawPattern(Texture2D Texture, float X, float Y, int PatternIndex, int PatternWidth, int PatternHeight,
      int OriginX, int OriginY, float ScaleX, float ScaleY, float Rotation, bool FlipX, bool FlipY,
      byte Red, byte Green, byte Blue, byte Alpha, bool DoCenter, BlendMode BlendMode = BlendMode.Normal)
    {

        var TexWidth = Texture.Width;
        var TexHeight = Texture.Height;

        var ColCount = TexWidth / PatternWidth;
        var RowCount = TexHeight / PatternHeight;
        var FPatternIndex = PatternIndex;
        if (FPatternIndex < 0)
            FPatternIndex = 0;
        if (FPatternIndex >= RowCount * ColCount)
            FPatternIndex = RowCount * ColCount - 1;
        var X1 = (FPatternIndex % ColCount) * PatternWidth;
        var Y1 = (FPatternIndex / ColCount) * PatternHeight;
        SpriteEffects Flip = SetFlip(FlipX, FlipY);
        if (DoCenter)
        {
            OriginX = PatternWidth / 2;
            OriginY = PatternHeight / 2;
        }

        SetBlendMode(BlendMode);
        SpriteBatch.Draw(Texture,
                        new Vector2(X, Y),
                        new Rectangle(X1, Y1, PatternWidth, PatternHeight),
                        new Microsoft.Xna.Framework.Color(Red, Green, Blue, Alpha),
                        Rotation,
                        new Vector2(OriginX, OriginY),
                        new Vector2(ScaleX, ScaleY),
                        Flip,
                        0);

        SpriteBatch.End();
    }

    public void DrawCropArea(Texture2D Texture, float X, float Y, Rectangle CropArea, int OriginX, int OriginY,
                float ScaleX, float ScaleY, float Rotation, bool FlipX, bool FlipY, byte Red, byte Green, byte Blue, byte Alpha,
                bool DoCenter, BlendMode BlendMode = BlendMode.Normal)
    {
        SpriteEffects Flip = SetFlip(FlipX, FlipY);
        if (DoCenter)
        {
            OriginX = CropArea.Width / 2;
            OriginY = CropArea.Height / 2;
        }


        SetBlendMode(BlendMode);
        SpriteBatch.Draw(Texture,
                         new Vector2(X, Y),
                         CropArea,
                         new Microsoft.Xna.Framework.Color(Red, Green, Blue, Alpha),
                         Rotation,
                         new Vector2(OriginX, OriginY),
                         new Vector2(ScaleX, ScaleY),
                         Flip,
                         0);

        SpriteBatch.End();
    }
    public void DrawCropArea(Texture2D Texture, float X, float Y, Rectangle CropArea)
    {
        DrawCropArea(Texture, X, Y, CropArea, 0, 0, 1, 1, 0, false, false, 255, 255, 255, 255, false);
    }
    public void DrawEx(Texture2D Texture, float X, float Y, int OriginX, int OriginY, float ScaleX, float ScaleY, float Rotation,
        bool FlipX, bool FlipY, byte Red, byte Green, byte Blue, byte Alpha, bool DoCenter, BlendMode BlendMode = BlendMode.Normal)
    {

        SetBlendMode(BlendMode);
        SpriteEffects Flip = SetFlip(FlipX, FlipY);
        if (DoCenter)
        {
            OriginX = Texture.Width / 2;
            OriginY = Texture.Height / 2;
        }

        SpriteBatch.Draw(Texture,
                      new Vector2(X, Y),
                      null,
                      new Microsoft.Xna.Framework.Color(Red, Green, Blue, Alpha),
                      Rotation,
                      new Vector2(OriginX, OriginY),
                      new Vector2(ScaleX, ScaleY),
                      Flip,
                      0);

        SpriteBatch.End();
    }

    public void DrawRotate(Texture2D Texture, float X, float Y, float Rotation, BlendMode BlendMode = BlendMode.Normal)
    {

        DrawEx(Texture, X, Y, 0, 0, 1, 1, Rotation, false, false, 255, 255, 255, 255, true, BlendMode);

    }

    public void DrawString(string KeyName, string Text, int X, int Y, Color Color)
    {
        SpriteBatch.Begin();
        SpriteBatch.DrawStringEx(EngineFunc.Fonts[KeyName], Text, new Vector2(X, Y), Color);
        SpriteBatch.End();
    }

}









