using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using Survivor;
using System;
using System.Collections.Generic;

namespace Hockey;

enum State
{
    Normal,
    Hurt
}

public class Snowman : Base
{
    public Vector2 WorldPosition;
    public Rectangle Hitbox;
    public Vector2 Speed;

    private Vector2 origin;
    private Vector2 facingDirection;
    Dictionary<State, AnimatedSprite> animatedSprites;
    AnimatedSprite currentSprite;

    private Texture2D sheet;
    private State state;

    private int swidth = 26, sheight = 32;

    const double hurtTime = 0.2;
    const double pushBackDist = 50; // pixels
    const float pushBackSpeed = (float)(pushBackDist / hurtTime); 
    private double hurtTimer = 0;

    private ContentManager contentManager;

    public Snowman(ContentManager cmanager)
    {
        contentManager = cmanager;
        sheet = cmanager.Load<Texture2D>("hockey/snowman");
        origin = new Vector2(13, 27);
        Hitbox = new Rectangle((int)WorldPosition.X - 8, (int)WorldPosition.Y - 8, 16, 16);
        // 6 20 15 15
        
        animatedSprites = new()
        {
            { State.Normal, new AnimatedSprite(sheet, new Rectangle(0, 0, swidth, sheight), new Vector2(13, 25), 2, 2, true) },
            { State.Hurt, new AnimatedSprite(sheet, new Rectangle(swidth * 3, 0, swidth, sheight), new Vector2(13, 25), 1, 1) },
        };

        state = State.Normal;
        currentSprite = animatedSprites[state];
    }
    
    private void UpdateHitBoxPosition()
    {
        Hitbox.X = (int)WorldPosition.X - Hitbox.Width / 2;
        Hitbox.Y = (int)WorldPosition.Y - Hitbox.Height / 2;
    }

    private void OnCollisionEnter(Base other)
    {
        if (other is Projectile proj
            && proj.Possessed
            && proj.Hitbox.Intersects(Hitbox))
        {
            Hit(proj);
        }
    }

    private void Hit(Projectile proj)
    {
        ChangeState(State.Hurt);
        hurtTimer = 0;
        Speed = Vector2.Divide(proj.Speed, 2);
        proj.Speed = Vector2.Divide(-proj.Speed, 2);
    }

    private void OnCollisionLeave(Base other)
    {

    }

    private void ChangeState(State st)
    {
        Log.Debug("Changed state from {0} to {1}", state, st);
        state = st;
    }

    private void CheckObjectCollision()
    {
        foreach (var e in EntityManager.Manager.Entities)
        {
            OnCollisionEnter(e);
        }
    }

    public override void Update(GameTime gameTime)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (state == State.Hurt)
        {
             WorldPosition += Speed * dt;

            hurtTimer += dt;
            if (hurtTimer > hurtTime)
            {
                ChangeState(State.Normal);
                Speed = Vector2.Zero;
            }
        }

        currentSprite = animatedSprites[state];
        UpdateHitBoxPosition();
        CheckObjectCollision();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var scale = HockeyGame.World.scaleX;
        var w = HockeyGame.World;
        currentSprite.Draw(spriteBatch, HockeyGame.World.WorldToScreen(WorldPosition), 0, Color.White, layerDepth: 0.3f, scaleX: scale, scaleY: scale);
        //spriteBatch.DrawRect(w.WorldToScreen(Hitbox.X, Hitbox.Y), Hitbox.Width, Hitbox.Height, Color.Red, scale, scale);
    }

}
