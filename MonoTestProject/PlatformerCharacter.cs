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

    private Vector2 size = new(32, 32);

    private float gravity = 30f;
    private Vector2 speed;

    private bool facingRight = true;
    private bool running = false;

    public PlatformerCharacter() 
    {        
    }

    public override void Update(float deltaTime)
    {
        speed.Y += gravity * deltaTime;        

        if (InputManager.IsDown(Keys.D))
        {
            speed.X = 100;
            facingRight = true;
        }
        else if (InputManager.IsDown(Keys.A))
        {
            speed.X = -100;
            facingRight = false;
        }
        else
        {
            speed.X = 0;
            runningSprite.ResetAnimation();
        }

        Position += speed * deltaTime;
        running = speed.Length() > 0;
        if (running) runningSprite.Update(deltaTime);
        CheckMapCollision();
        base.Update(deltaTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (running)
        {
            runningSprite.Draw(spriteBatch, Position, 0, Color.White, facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
        }
        else
        {
            standingSprite.Draw(spriteBatch, Position, 0, Color.White, facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
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
