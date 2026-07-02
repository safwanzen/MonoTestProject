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
    Charging,
    Swinging,
    //RunR,
    //RunUR,
    //RunDR
}

public class Player : Base
{
    private const float decell = 0.99f;
    private const int width = 16;
    private const int height = 16;
    private float health;
    private Vector2 WorldPosition;
    private Vector2 Speed;
    private Vector2 direction;
    private Vector2 facingDirection = new Vector2(1, 0);
    private float maxAccel = 2300f;
    private float maxSpeed = 800f;
    private float projectileSpeed = 1200f;
    private float brakeRate = 0.87f;
    private float minSpeed = 9f;

    // input
    private bool u, d, l, r;
    bool braking = false;
    bool shoot = false;
    bool charge = false;

    // graphics
    Dictionary<AnimState, AnimatedSprite> animatedSprites;
    Sprite standingSprite;
    AnimatedSprite currentSprite;
    Sprite shadow;
    float spriteScale = 1;

    AnimState animState;
    AnimState prevAnimState;

    Texture2D sheet;
    int spritesize = 16; // 16 x 16 px tile
    float animFps = 15;

    ContentManager contentManager;
    int hitRadius = 10;
    Vector2 origin;

    public Rectangle Hitbox;

    public Player(ContentManager cmanager)
    {
        sheet = cmanager.Load<Texture2D>("hockey/hockey_char");
        contentManager = cmanager;
        origin = new Vector2(spritesize / 2, spritesize - 2);
        Hitbox = new Rectangle((int)WorldPosition.X - width / 2, (int)WorldPosition.Y - height / 2, width, height);

        InitSprite();
        UpdateHitBoxPosition();
    }

    private void InitSprite()
    {

        animatedSprites = new()
        {
            { AnimState.RunU,   ExtractFrames(15, 2) },
            { AnimState.RunUL,  ExtractFrames(11, 2) },
            { AnimState.RunL,   ExtractFrames(0 , 2) },
            { AnimState.RunDL,  ExtractFrames(7 , 2) },
            { AnimState.RunD,   ExtractFrames(4 , 2) },
        };
        standingSprite = new Sprite(sheet, new Rectangle(spritesize, 0, spritesize, spritesize), origin);
        var shadowsprite = contentManager.Load<Texture2D>("hockey/player-shadow");
        shadow = new Sprite(shadowsprite, new Rectangle(0, 0, spritesize, spritesize), new Vector2(spritesize / 2, spritesize / 2));
    }

    private AnimatedSprite ExtractFrames(int position, int frames, bool loop = true)
    {
        return new AnimatedSprite(
                sheet,
                new Rectangle(position * spritesize, 0, spritesize, spritesize),
                origin,
                frames,
                fps: animFps,
                loop
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
        charge = InputManager.IsDown(Keys.O);

        bool moving = u || d || l || r;

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

        prevAnimState = animState;

        if (Math.Abs(direction.X) > 0 && direction.Y < 0) 
            animState = AnimState.RunUL;
        else if (Math.Abs(direction.X) > 0 && direction.Y > 0)
            animState = AnimState.RunDL;
        else if (Math.Abs(direction.X) > 0)
            animState = AnimState.RunL;
        else if (direction.Y < 0)
            animState = AnimState.RunU;
        else if (direction.Y > 0) 
            animState = AnimState.RunD;

        if (prevAnimState != animState) Log.Debug("changed {0}", animState.ToString());

        currentSprite = animatedSprites[animState];
        
        if (!moving) currentSprite.FrameIndex = 1;

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
            var b = new Projectile(contentManager)
            {
                Speed = Vector2.Normalize(facingDirection) * (projectileSpeed + Speed.Length()),
                WorldPosition = WorldPosition
            };
            EntityManager.Manager.AddObject(b);
        }

        if (Speed.Length() < minSpeed) Speed = Vector2.Zero;

        CheckBoundaryCollision();
        UpdateHitBoxPosition();
        

        // check for map collision
        // check for enemy collision
        foreach (var e in EntityManager.Manager.Entities)
        {
            if (e is Obstacle obs)
            {
                if (obs.Hitbox.Intersects(Hitbox))
                {
                    Log.Debug("hit obstacle {0}", obs.GetHashCode());
                    break;
                }
            }
        }

        currentSprite.Update(dt);
    }

    private void UpdateHitBoxPosition()
    {
        Hitbox.X = (int)WorldPosition.X - Hitbox.Width / 2;
        Hitbox.Y = (int)WorldPosition.Y - Hitbox.Height / 2;
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
        var w = WorldPosition;
        spritebatch.DrawRect(
            new Vector2(w.X - width / 2, w.Y - height / 2),
            width, height, Color.DarkGray);

        //spritebatch.DrawLine(w, w + Vector2.Multiply(facingDirection, 50), Color.YellowGreen);
        //spritebatch.DrawLine(w, w + Vector2.Multiply(direction, 50), Color.Red);
        //spritebatch.DrawPolygonWireFrame(w + Vector2.Multiply(facingDirection, 50),
        //    new Vector2[] { new(s, s), new(s, -s), new(-s, -s), new(-s, s) },
        //    Color.YellowGreen);

        shadow.Draw(spritebatch, new Vector2(w.X, w.Y), 0, Color.White, SpriteEffects.None, spriteScale, spriteScale);
        bool facingR = facingDirection.X >= 0;
        currentSprite.Draw(spritebatch, new Vector2(w.X, w.Y), 0, Color.White, facingR ? SpriteEffects.FlipHorizontally : SpriteEffects.None, spriteScale, spriteScale);

        // debug text
        //spritebatch.DrawString(Font, "(0, 0)", World.WorldToScreen(0, 0), Color.White, 0f, Vector2.Zero, new Vector2(xscale, yscale), SpriteEffects.None, 0);
    }
}
