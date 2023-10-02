using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTestProject;

public enum MouseButtonState
{
    Pressed,
    Released,
    Down,
    Up
}

public enum MouseButtons
{
    LeftButton,
    MiddleButton,
    RightButton,
    XButton1,
    XButton2,
}

public static class InputManager
{
    static readonly Dictionary<MouseButtons, ButtonState> PreviousMouseButtonState = new()
    {
        [MouseButtons.LeftButton    ] = ButtonState.Released,
        [MouseButtons.MiddleButton  ] = ButtonState.Released,
        [MouseButtons.RightButton   ] = ButtonState.Released,
        [MouseButtons.XButton1      ] = ButtonState.Released,
        [MouseButtons.XButton2      ] = ButtonState.Released,
    };

    static readonly Dictionary<MouseButtons, ButtonState> MouseButtonState = new()
    {
        [MouseButtons.LeftButton   ] = ButtonState.Released,
        [MouseButtons.MiddleButton ] = ButtonState.Released,
        [MouseButtons.RightButton  ] = ButtonState.Released,
        [MouseButtons.XButton1     ] = ButtonState.Released,
        [MouseButtons.XButton2     ] = ButtonState.Released,
    };

    static Keys[] pressedKeys = new Keys[] { Keys.None };
   
    public static void BeginFrame()
    {
        //pressedKeys = Keyboard.GetState().GetPressedKeys();
        MouseButtonState[MouseButtons.LeftButton    ] = Mouse.GetState().LeftButton;
        MouseButtonState[MouseButtons.MiddleButton  ] = Mouse.GetState().MiddleButton;
        MouseButtonState[MouseButtons.RightButton   ] = Mouse.GetState().RightButton;
        MouseButtonState[MouseButtons.XButton1      ] = Mouse.GetState().XButton1;
        MouseButtonState[MouseButtons.XButton2      ] = Mouse.GetState().XButton2;
    }

    public static void EndFrame()
    {
        pressedKeys = Keyboard.GetState().GetPressedKeys();
        PreviousMouseButtonState[MouseButtons.LeftButton    ] = Mouse.GetState().LeftButton;
        PreviousMouseButtonState[MouseButtons.MiddleButton  ] = Mouse.GetState().MiddleButton;
        PreviousMouseButtonState[MouseButtons.RightButton   ] = Mouse.GetState().RightButton;
        PreviousMouseButtonState[MouseButtons.XButton1      ] = Mouse.GetState().XButton1;
        PreviousMouseButtonState[MouseButtons.XButton2      ] = Mouse.GetState().XButton2;
    }

    public static bool IsPressed(Keys key)
    {
        return !pressedKeys.Contains(key) && IsDown(key);
    }

    public static bool IsPressed(MouseButtons mousebutton) => 
        PreviousMouseButtonState[mousebutton] == ButtonState.Released && IsDown(mousebutton);

    public static bool IsReleased(Keys key)
    {
        return pressedKeys.Contains(key) && IsUp(key);
    }

    public static bool IsReleased(MouseButtons mousebutton) =>
        PreviousMouseButtonState[mousebutton] == ButtonState.Pressed && IsUp(mousebutton);

    public static bool IsDown(Keys key)
    {
        return Keyboard.GetState().IsKeyDown(key);
    }

    public static bool IsDown(MouseButtons button)
    {
        return MouseButtonState[button] == ButtonState.Pressed;
    }

    public static bool IsUp(Keys key)
    {
        return Keyboard.GetState().IsKeyUp(key);
    }

    public static bool IsUp(MouseButtons button)
    {
        return MouseButtonState[button] == ButtonState.Released;
    }
}
