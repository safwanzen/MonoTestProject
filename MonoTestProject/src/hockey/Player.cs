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
    Fall
    //RunR,
    //RunUR,
    //RunDR
}

enum ActionState
{
    Normal,
    Fall
}

public class Player : Base
{
    private const float decell = 0.99f;
    private const int width = 8;
    private const int height = 8;
    public Vector2 WorldPosition;
    private Vector2 Speed;
    private Vector2 inputDirection;
    private Vector2 facingDirection = new Vector2(1, 0);
    private float maxAccel = 575f;
    private float maxSpeed = 200f;
    private float projectileSpeed = 300f;
    private float brakeRate = 0.87f;
    private float minSpeed = 9f;

    // input
    private bool u, d, l, r;
    bool braking = false;
    bool shoot = false;
    bool lockDir = false;
    bool charge = false;

    // graphics
    Dictionary<AnimState, AnimatedSprite> animatedSprites;
    AnimatedSprite currentSprite;
    Sprite shadow;
    Sprite ammo;

    AnimState animState;
    AnimState prevAnimState;
    ActionState actionState = ActionState.Normal;

    Texture2D sheet;
    int spritesize = 16; // 16 x 16 px tile
    float animFps = 15;

    ContentManager contentManager;
    Vector2 origin;

    const float fallTime = 1f;
    float fallTimer = 0;

    float maxShootTime = .07f;
    float shootTime = 0;

    public Rectangle Hitbox;
    List<Obstacle> overlaps = new();
    int ammoCount = 0;
    private float ammoDist = 6;

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
            { AnimState.Fall,   ExtractFrames(19, 1) },
        };
        var shadowsprite = contentManager.Load<Texture2D>("hockey/player-shadow");

        shadow = new Sprite(shadowsprite, 
            new Rectangle(0, 0, spritesize, spritesize),
            new Vector2(spritesize / 2, spritesize / 2));

        ammo = new Sprite(contentManager.Load<Texture2D>("hockey/hockey_puck"),
            new Rectangle(0, 0, spritesize, spritesize),
            new Vector2(spritesize / 2, spritesize / 2));
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

        if (actionState == ActionState.Normal) Input();
        if (actionState == ActionState.Fall)
        {
            fallTimer += dt;
            if (fallTimer > fallTime)
            {
                fallTimer = 0;
                actionState = ActionState.Normal;
                animState = prevAnimState;
                //stateRunning = stateRunning.Previous;
            }
        }

        bool moving = u || d || l || r;

        if (inputDirection.Length() > 0)
        {
            inputDirection.Normalize();
            if (!lockDir) facingDirection = inputDirection;
        }

        if (actionState == ActionState.Fall)
        {
            inputDirection = Vector2.Zero;
        }
        else if (Math.Abs(facingDirection.X) > 0 && facingDirection.Y < 0)
            animState = AnimState.RunUL;
        else if (Math.Abs(facingDirection.X) > 0 && facingDirection.Y > 0)
            animState = AnimState.RunDL;
        else if (Math.Abs(facingDirection.X) > 0)
            animState = AnimState.RunL;
        else if (facingDirection.Y < 0)
            animState = AnimState.RunU;
        else if (facingDirection.Y > 0)
            animState = AnimState.RunD;

        currentSprite = animatedSprites[animState];

        if (!moving && actionState == ActionState.Normal)
        {
            currentSprite.FrameIndex = 1;
        }

        float accel = 0f;
        if (u || d || l || r) accel = maxAccel;
        if (braking) accel = 0f;

        //speedMagnitude += direction.Length() * accel * deltaTime;
        if (Speed.Length() < maxSpeed)
        {
            Speed += inputDirection * accel * dt;
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

        if (shootTime < maxShootTime) shootTime += dt;

        if (shoot && ammoCount > 0 && shootTime >= maxShootTime)
        {
            shootTime = 0;
            Log.Debug("dir {0} {1}", facingDirection, projectileSpeed);
            var b = new Projectile(contentManager)
            {
                Speed = Vector2.Normalize(facingDirection) * (projectileSpeed + Speed.Length()),
                WorldPosition = WorldPosition + facingDirection * 10
            };
            EntityManager.Manager.AddObject(b);
            ammoCount--;
            Log.Debug("ammo count = {0}", ammoCount);
        }

        if (Speed.Length() < minSpeed) Speed = Vector2.Zero;

        UpdateHitBoxPosition();
        CheckObstacleCollision();
        CheckBoundaryCollision();

        currentSprite.Update(dt);
    }

    private void Input()
    {
        // input
        u = InputManager.IsDown(Keys.W);
        d = InputManager.IsDown(Keys.S);
        l = InputManager.IsDown(Keys.A);
        r = InputManager.IsDown(Keys.D);
        braking = InputManager.IsDown(Keys.I);
        shoot = InputManager.IsReleased(Keys.O);
        charge = InputManager.IsDown(Keys.O);
        lockDir = InputManager.IsDown(Keys.P);

        if (u && d) { inputDirection.Y = 0; }
        else if (u)
        {
            inputDirection.Y = -1;
        }
        else if (d)
        {
            inputDirection.Y = 1;
        }
        else { inputDirection.Y = 0; }

        if (l && r) { inputDirection.X = 0; }
        else if (l)
        {
            inputDirection.X = -1;
        }
        else if (r)
        {
            inputDirection.X = 1;
        }
        else { inputDirection.X = 0; }
    }

    private void CheckObstacleCollision()
    {
        foreach (var e in EntityManager.Manager.Entities)
        {
            if (e is Obstacle obs)
            {
                bool hit = obs.Hitbox.Intersects(Hitbox);
                bool contains = overlaps.Contains(obs);

                if (hit && !contains)
                {
                    OnCollisionEnter(obs);
                }
                if (!hit && contains)
                {
                    OnCollisionLeave(obs);
                }
            }
        }
    }

    private void OnCollisionEnter(Obstacle obs)
    {
        overlaps.Add(obs);

        Log.Debug("{0} collision enter {1}", typeof(Obstacle), obs.GetHashCode());
        if (actionState == ActionState.Normal)
        {
            actionState = ActionState.Fall;
            prevAnimState = animState;
            animState = AnimState.Fall;
        }
        // resolve collision
        else if (actionState == ActionState.Fall)
        {
            var oh = obs.Hitbox;
            var h = Hitbox;
            var w = WorldPosition;

            // which side to solve first? vertical or horizontal?
            // solve which overlap is smaller

            var i = Rectangle.Intersect(oh, h);
            if (Math.Abs(i.Width) < Math.Abs(i.Height))
            {
                // solve X axis
                Speed.X = 0;
                if (w.X < obs.WorldPosition.X) WorldPosition.X = (int)WorldPosition.X - i.Width + 1;
                else WorldPosition.X = (int)WorldPosition.X + i.Width;
            }
            else
            {
                // solve Y axis
                Speed.Y = 0;
                if (w.Y < obs.WorldPosition.Y) WorldPosition.Y = (int)WorldPosition.Y - i.Height + 1;
                else WorldPosition.Y = (int)WorldPosition.Y + i.Height;
            }
        }
    }

    private void OnCollisionLeave(Obstacle obs)
    {
        overlaps.Remove(obs);
        Log.Debug("{0} collision leave {1}", typeof(Obstacle), obs.GetHashCode());
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
            WorldPosition.X + width / 2 > HockeyGame.WorldWidth
            || WorldPosition.X - width / 2 < 0
            || WorldPosition.Y + height / 2 > HockeyGame.WorldHeight
            || WorldPosition.Y - height / 2 < 0;

        if (WorldPosition.X + width / 2 > HockeyGame.WorldWidth)
        {
            x = HockeyGame.WorldWidth - width / 2;
            sx = 0;
        }
        if (WorldPosition.X - width / 2 < 0)
        {
            x = width / 2;
            sx = 0;
        }
        if (WorldPosition.Y + height / 2 > HockeyGame.WorldHeight)
        {
            y = HockeyGame.WorldHeight - height / 2;
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

    public void AddAmmo()
    {
        ammoCount++;
        Log.Debug("ammo count = {0}", ammoCount);
    }

    public override void Draw(SpriteBatch spritebatch)
    {
        var w = WorldPosition;
        var screenpos = HockeyGame.World.WorldToScreen(new Vector2(w.X, w.Y));
        var scale = HockeyGame.World.scaleX;


        //spritebatch.DrawLine(w, w + Vector2.Multiply(facingDirection, 50), Color.YellowGreen);
        //spritebatch.DrawLine(w, w + Vector2.Multiply(direction, 50), Color.Red);
        //spritebatch.DrawPolygonWireFrame(w + Vector2.Multiply(facingDirection, 50),
        //    new Vector2[] { new(s, s), new(s, -s), new(-s, -s), new(-s, s) },
        //    Color.YellowGreen);

        shadow.Draw(spritebatch, screenpos, 0, Color.White, spriteEffects: SpriteEffects.None, scaleX: scale, scaleY: scale);
        bool facingR = facingDirection.X >= 0;
        currentSprite.Draw(spritebatch, screenpos, 0, Color.White, layerDepth: .5f, spriteEffects: facingR ? SpriteEffects.FlipHorizontally : SpriteEffects.None, scaleX: scale, scaleY: scale);

        if (ammoCount > 0)
        {
            ammo.Draw(spritebatch,
                HockeyGame.World.WorldToScreen(WorldPosition + facingDirection * ammoDist),
                0, Color.White, 0.4f, scaleX: scale, scaleY: scale);
        }

        //spritebatch.DrawRect(
        //    HockeyGame.World.WorldToScreen(new Vector2(w.X - width / 2, w.Y - height / 2)),
        //    width, height, Color.DarkGray, scale);

        // debug text
        //spritebatch.DrawString(Font, "(0, 0)", World.WorldToScreen(0, 0), Color.White, 0f, Vector2.Zero, new Vector2(xscale, yscale), SpriteEffects.None, 0);
    }

    
}
