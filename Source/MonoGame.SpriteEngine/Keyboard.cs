using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SpriteEngine;

public class Keyboard
{
    static KeyboardState currentKeyState;
    static KeyboardState previousKeyState;

    public static KeyboardState GetState()
    {
        previousKeyState = currentKeyState;
        currentKeyState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        return currentKeyState;
    }

    public static bool KeyDown(Microsoft.Xna.Framework.Input.Keys key)
    {
        return currentKeyState.IsKeyDown(key);
    }
    public static bool KeyUp(Microsoft.Xna.Framework.Input.Keys key)
    {
        return !previousKeyState.IsKeyUp(key) && currentKeyState.IsKeyUp(key);
    }

    public static bool KeyPressed(Microsoft.Xna.Framework.Input.Keys key)
    {
        return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
    }

}
