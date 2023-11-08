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
    public event EventHandler Released;
    public event EventHandler Pressed;
    public event EventHandler PointerEnter;
    public event EventHandler PointerLeave;

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
            OnPointerEnter();
        }

        if (isHovering && !hit)
        {
            // pointer leave
            //Console.WriteLine("pointer left");
            OnPointerLeave();
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
                OnClick();
            }
        }

        if (hit && InputManager.IsReleased(MouseButtons.LeftButton))
        {
            // invoke click release event
            //Console.WriteLine("pointer release");
            OnPointerReleased();
        }

        if (buttonDebounceTime > 0) buttonDebounceTime -= (float)elapsedTime;
        if (doubleClickTimeWindow > 0)
        {
            doubleClickTimeWindow -= (float)elapsedTime;
            //if (doubleClickTimeWindow <= 0) Console.WriteLine("double click expired");
        }

        isHovering = hit;
    }

    protected void OnPointerEnter()
    {
        PointerEnter?.Invoke(this, EventArgs.Empty);
    }

    protected void OnPointerLeave()
    {
        PointerLeave?.Invoke(this, EventArgs.Empty);
    }

    protected void OnClick()
    {
        Click?.Invoke(this, EventArgs.Empty);
    }

    protected void OnPointerPressed()
    {
        Pressed?.Invoke(this, EventArgs.Empty);
    }

    protected void OnPointerReleased()
    {
        Released?.Invoke(this, EventArgs.Empty);
    }

    private bool HitCheckPoint()
    {
        return Bounds.Contains(InputManager.MousePosition);
    }
}
