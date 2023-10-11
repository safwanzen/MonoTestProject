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
    private bool isOnGround = true;
    private bool isJumping = false;

    private float jumpVelocity = 50f;

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
        #region handle input
        if (InputManager.IsDown(dpadRight))
        {
            speed.X = 180;
            facingRight = true;
        }
        else if (InputManager.IsDown(dpadLeft))
        {
            speed.X = -180;
            facingRight = false;
        }
        else
        {
            speed.X = 0;
            runningSprite.ResetAnimation();
        }

        if (InputManager.IsPressed(BBtn))
        {
            var dir = new Vector2(facingRight ? 1 : -1, 0);
            weapon.Attack(WorldPosition + dir * 8, dir, 0);
        }

        if (InputManager.IsReleased(BBtn))
        {
            var dir = new Vector2(facingRight ? 1 : -1, 0);
            weapon.ReleaseCharge(WorldPosition + dir * 8, dir, 0);
            weapon.Charge(false);
        }

        if (InputManager.IsDown(BBtn))
        {
            weapon.Charge(true);
        }

        if (InputManager.IsPressed(dpadUp) && isOnGround && !isJumping)
        {
            Console.WriteLine("jumping");
            isOnGround = false;
            isJumping = true;
            speed.Y = -500f; // -jumpVelocity
        }

        if (InputManager.IsReleased(dpadUp) && isJumping)
        {
            Console.WriteLine("jump released");
            isJumping = false;
            if (speed.Y < 0) speed.Y *= 0.1f;
        }
        #endregion

        #region movement

        WorldPosition += speed * deltaTime;

        if (isOnGround)
        {
            isJumping = false;
            speed.Y = 0;
        }
        else
        {
            speed.Y += /*gravity*/ 1500f * deltaTime;
        }
        #endregion

        #region collision
        CheckMapCollision();
        #endregion

        running = Math.Abs(speed.X) > 0 && isOnGround;
        if (running) runningSprite.Update(deltaTime);
        weapon.Update(deltaTime);
        base.Update(deltaTime);
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

        //spriteBatch.DrawRect((int)World.WorldToScreen(WorldPosition.X - 8, 0).X, (int)World.WorldToScreen(0, WorldPosition.Y - 14).Y, (int)(World.scaleX * 16), (int)(World.scaleY * 30), Color.Red * .5f);
        //spriteBatch.DrawRectWireframe((int)World.WorldToScreen(WorldPosition.X - 8, 0).X, (int)World.WorldToScreen(0, WorldPosition.Y - 14).Y, (int)(World.scaleX * 16), (int)(World.scaleY * 30), Color.Cyan);

        base.Draw(spriteBatch);
    }

    private void CheckMapCollision()
    {
        // check for boundary collision
        if (WorldPosition.X > MainGame.ScreenWidth - size.X / 2)
        {
            WorldPosition.X = MainGame.ScreenWidth - size.X / 2;
            speed.X = 0;
        }
        else if (WorldPosition.X < size.X / 2)
        {
            WorldPosition.X = size.X / 2;
            speed.X = 0;
        }

        if (WorldPosition.Y > MainGame.ScreenHeight - size.Y / 2)
        {
            WorldPosition.Y = MainGame.ScreenHeight - size.Y / 2;
            speed.Y = 0;
            isOnGround = true;
        }
        else if (WorldPosition.Y < size.Y / 2)
        {
            WorldPosition.Y = size.Y / 2;
            speed.Y = 0;
        }
    }
}
