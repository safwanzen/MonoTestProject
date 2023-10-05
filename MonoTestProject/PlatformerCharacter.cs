using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTestProject;

public class PlatformerCharacter : Entity
{
    public AnimatedSprite runningSprite;
    public Sprite standingSprite;

    private Vector2 size = new(8, 32);

    private float gravity = 30f;
    private Vector2 speed;

    private bool facingRight = true;
    private bool running = false;

    private float shootTimer = 0.05f;

    private Weapon weapon = new();

    #region controls
    private const Keys dpadUp = Keys.W;
    private const Keys dpadDown = Keys.S;
    private const Keys dpadLeft = Keys.A;
    private const Keys dpadRight = Keys.D;
    private const Keys ABtn = Keys.I;
    private const Keys BBtn = Keys.O;
    private const Keys LBtn = Keys.P;
    private const Keys RBtn = Keys.OemQuotes;

    #endregion

    public PlatformerCharacter() 
    {        
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        #region handle input
        if (InputManager.IsDown(dpadRight))
        {
            speed.X = 100;
            facingRight = true;
        }
        else if (InputManager.IsDown(dpadLeft))
        {
            speed.X = -100;
            facingRight = false;
        }
        else
        {
            speed.X = 0;
            runningSprite.ResetAnimation();
        }

        if (InputManager.IsPressed(ABtn))
        {
            var dir = new Vector2(facingRight ? 1 : -1, 0);
            weapon.Attack(Position + dir * 8, dir, 0);
        }

        if (InputManager.IsReleased(ABtn))
        {
            var dir = new Vector2(facingRight ? 1 : -1, 0);
            weapon.ReleaseCharge(Position + dir * 8, dir, 0);
            weapon.Charge(false);
        }

        if (InputManager.IsDown(ABtn))
        {
            weapon.Charge(true);
        }
        #endregion

        #region movement
        Position += speed * deltaTime;
        //speed.Y += gravity * deltaTime;        
        #endregion

        #region collision
        CheckMapCollision();
        #endregion

        running = speed.Length() > 0;
        if (running) runningSprite.Update(deltaTime);
        weapon.Update(deltaTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (running)
        {
            runningSprite.Draw(spriteBatch, ScreenPosition, 0, Color.White, facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally, scaleX, scaleY);
        }
        else
        {
            standingSprite.Draw(spriteBatch, ScreenPosition, 0, Color.White, facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally, scaleX, scaleY);
        }

        base.Draw(spriteBatch);
    }

    private void CheckMapCollision()
    {
        // check for boundary collision
        if (Position.X > MainGame.ScreenWidth - size.X / 2)
        {
            Position.X = MainGame.ScreenWidth - size.X / 2;
            speed.X = 0;
        }
        else if (Position.X < size.X / 2)
        {
            Position.X = size.X / 2;
            speed.X = 0;
        }

        if (Position.Y > MainGame.ScreenHeight - size.Y / 2)
        {
            Position.Y = MainGame.ScreenHeight - size.Y / 2;
            speed.Y = 0;
        }
        else if (Position.Y < size.Y / 2)
        {
            Position.Y = size.Y / 2;
            speed.Y = 0;
        }
    }
}
