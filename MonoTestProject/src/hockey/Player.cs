using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;
using Hockey;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System;

namespace Survivor;

// only to the left
enum AnimState
{
    RunL,
    RunUL,
    RunD,
    RunDL,
    RunU,
    //RunR,
    //RunUR,
    //RunDR
}

public class Player : Base
{
    private const float decell = 0.99f;
    private const int width = 40;
    private const int height = 40;
    private float health;
    private Vector2 WorldPosition;
    private Vector2 Speed;
    private Vector2 direction;
    private Vector2 facingDirection = new Vector2(1, 0);
    private float maxAccel = 2300f;
    private float maxSpeed = 800f;
    private bool u, d, l, r;
    private float projectileSpeed = 1200f;
    private float brakeRate = 0.87f;
    private float minSpeed = 9f;

    bool braking = false;
    bool shoot = false;

    // graphics
    Dictionary<AnimState, AnimatedSprite> animatedSprite;
    Sprite standingSprite;
    Sprite currentSprite;
    float spriteScale = 4;

    Texture2D sheet;
    int spritesize = 16; // 16 x 16 px tile
    float animFps = 15;

    public Player(ContentManager cmanager)
    {
        sheet = cmanager.Load<Texture2D>("hockey/hockey_char");
        InitSprite();
    }

    private void InitSprite()
    {
        animatedSprite = new()
        {
            { AnimState.RunU,   ExtractFrames(15) },
            { AnimState.RunUL,  ExtractFrames(11) },
            { AnimState.RunL,   ExtractFrames(0) },
            { AnimState.RunDL,  ExtractFrames(8) },
            { AnimState.RunD,   ExtractFrames(5) },
        };
        standingSprite = new Sprite(sheet, new Rectangle(spritesize, 0, spritesize, spritesize), new Vector2(spritesize / 2, spritesize / 2));

        currentSprite = standingSprite;
    }

    private AnimatedSprite ExtractFrames(int position)
    {
        return new AnimatedSprite(
                sheet,
                new Rectangle(position * spritesize, 0, spritesize, spritesize),
                new Vector2(spritesize / 2, spritesize / 2),
                2,
                fps: animFps,
                true
                );
    }

    public override void Update(GameTime gameTime)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        maxAccel = 2300;

        // input
        u = InputManager.IsDown(Keys.W);
        d = InputManager.IsDown(Keys.S);
        l = InputManager.IsDown(Keys.A);
        r = InputManager.IsDown(Keys.D);
        braking = InputManager.IsDown(Keys.I);
        shoot = InputManager.IsReleased(Keys.O);


        if (u && d) { direction.Y = 0; }
        else if (u) 
        { 
            direction.Y = -1;
        }
        else if (d) 
        { 
            direction.Y = 1;
        }
        else { direction.Y = 0; }

        if (l && r) { direction.X = 0; }
        else if (l)
        { 
            direction.X = -1;
        }
        else if (r)
        { 
            direction.X = 1;
        }
        else { direction.X = 0; }

        if (direction.Length() > 0)
        { 
            direction.Normalize();
            facingDirection = direction;
        }
        if (direction.Length() == 0) currentSprite = standingSprite;

        if (Math.Abs(direction.X) > 0 && direction.Y < 0) currentSprite = animatedSprite[AnimState.RunUL];
        else if (Math.Abs(direction.X) > 0 && direction.Y > 0) currentSprite = animatedSprite[AnimState.RunDL];
        else if (Math.Abs(direction.X) > 0) currentSprite = animatedSprite[AnimState.RunL];
        else if (direction.Y < 0) currentSprite = animatedSprite[AnimState.RunU];
        else if (direction.Y > 0) 
        {
            currentSprite = animatedSprite[AnimState.RunD];
        }

        float accel = 0f;
        if (u || d || l || r) accel = maxAccel;
        if (braking) accel = 0f;

        //speedMagnitude += direction.Length() * accel * deltaTime;
        if (Speed.Length() < maxSpeed)
        { 
            Speed += direction * accel * dt;
        }
        else
        {
            Speed = Vector2.Normalize(Speed) * maxSpeed;
        }
        //if (Speed.Length() > 0)
        //    facingDirection = Vector2.Normalize(Speed);

        WorldPosition += Speed * dt;

        // brake
        if (braking)
        {
            Speed *= brakeRate;
        }
        // decellerate
        else
        { 
            Speed *= decell;
        }

        if (shoot)
        {
            Log.Debug("dir {0} {1}", facingDirection, projectileSpeed);
            var b = new Projectile()
            {
                Speed = Vector2.Normalize(facingDirection) * (projectileSpeed + Speed.Length()),
                WorldPosition = WorldPosition
            };
            EntityManager.Manager.AddObject(b);
        }

        if (Speed.Length() < minSpeed) Speed = Vector2.Zero;

        CheckBoundaryCollision();

        // check for map collision
        // check for enemy collision

        currentSprite.Update(dt);
    }

    private void CheckBoundaryCollision()
    {
        float x = WorldPosition.X;
        float y = WorldPosition.Y;
        float sx = Speed.X;
        float sy = Speed.Y;

        // check for boundary collision
        bool outofBounds =
            WorldPosition.X + width / 2 > HockeyGame.ScreenWidth
            || WorldPosition.X - width / 2 < 0
            || WorldPosition.Y + height / 2 > HockeyGame.ScreenHeight
            || WorldPosition.Y - height / 2 < 0;

        if (WorldPosition.X + width / 2 > HockeyGame.ScreenWidth)
        {
            x = HockeyGame.ScreenWidth - width / 2;
            sx = 0;
        }
        if (WorldPosition.X - width / 2 < 0)
        {
            x = width / 2;
            sx = 0;
        }
        if (WorldPosition.Y + height / 2 > HockeyGame.ScreenHeight)
        {
            y = HockeyGame.ScreenHeight - height / 2;
            sy = 0;
        }
        if (WorldPosition.Y - height / 2 < 0)
        {
            y = height / 2;
            sy = 0;
        }

        WorldPosition = new Vector2(x, y);
        Speed = new Vector2(sx, sy);
    }

    public override void Draw(SpriteBatch spritebatch)
    {
        int s = 5;
        spritebatch.DrawRect(
            new Vector2(WorldPosition.X - width / 2, WorldPosition.Y - height / 2),
            width, height, Color.Black);
        spritebatch.DrawLine(WorldPosition, WorldPosition + Vector2.Multiply(facingDirection, 50), Color.YellowGreen);
        //spritebatch.DrawLine(WorldPosition, WorldPosition + Vector2.Multiply(direction, 50), Color.Red);
        spritebatch.DrawPolygonWireFrame(WorldPosition + Vector2.Multiply(facingDirection, 50),
            new Vector2[] { new(s, s), new(s, -s), new(-s, -s), new(-s, s) },
            Color.YellowGreen);

        bool facingR = facingDirection.X >= 0;
        currentSprite.Draw(spritebatch, new Vector2(WorldPosition.X, WorldPosition.Y - 20), 0, Color.White, facingR ? SpriteEffects.FlipHorizontally : SpriteEffects.None, spriteScale, spriteScale);

        // debug text
        //spritebatch.DrawString(Font, "(0, 0)", World.WorldToScreen(0, 0), Color.White, 0f, Vector2.Zero, new Vector2(xscale, yscale), SpriteEffects.None, 0);
    }
}
