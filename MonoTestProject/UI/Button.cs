using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoTestProject.UI;

public class Button : IUIElement
{
    public string Text { get; set; }
    public Rectangle Bounds;
    
    private bool isHovering = false;
    private const float clickTimeWindow = 150;
    private float doubleClickTimeWindow = 0;
    private const int debounceTime = 300;
    private float buttonDebounceTime = 0;

    // click event
    // click function
    // hit checking

    // pointer enter
    // pointer leave
    // pointer pressed
    // pointer released

    public event EventHandler DoubleClick;
    public event EventHandler Click;
    public event EventHandler Release;

    public void Draw(SpriteBatch spriteBatch)
    {
        Color color = isHovering ? Color.Red : Color.White;
        spriteBatch.DrawRect(Bounds, color);
    }

    public void Update(GameTime gameTime)
    {
        bool hit = HitCheckPoint();
        var elapsedTime = gameTime.ElapsedGameTime.TotalMilliseconds;
        
        if (!isHovering && hit)
        {
            // pointer enter
            //Console.WriteLine("pointer entered");
        }

        if (isHovering && !hit)
        {
            // pointer leave
            //Console.WriteLine("pointer left");
        }

        if (hit && InputManager.IsPressed(MouseButtons.LeftButton))
        {
            // invoke click event
            //Console.WriteLine("pointer click");
            
            if (doubleClickTimeWindow > 0)
            {
                // invoke double click event
                //Console.WriteLine("double click");
                buttonDebounceTime = 50; //debounceTime;
                doubleClickTimeWindow = 0;
                DoubleClick?.Invoke(this, new EventArgs());
            }

            if (buttonDebounceTime <= 0)
            {
                doubleClickTimeWindow = 200; //clickTimeWindow;
                //Console.WriteLine("first click");
                Click?.Invoke(this, new EventArgs());
            }
        }

        if (hit && InputManager.IsReleased(MouseButtons.LeftButton))
        {
            // invoke click release event
            //Console.WriteLine("pointer release");
            Release?.Invoke(this, new EventArgs());
        }

        if (buttonDebounceTime > 0) buttonDebounceTime -= (float)elapsedTime;
        if (doubleClickTimeWindow > 0)
        {
            doubleClickTimeWindow -= (float)elapsedTime;
            //if (doubleClickTimeWindow <= 0) Console.WriteLine("double click expired");
        }

        isHovering = hit;
    }

    private bool HitCheckPoint()
    {
        return Bounds.Contains(InputManager.MousePosition);
    }
}
