using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTestProject;

public class Bullet : Entity
{
    public Vector2 Position;
    public Texture2D Texture;
    public Vector2 Direction;
    public float Speed = 600;

    private float rotationSpeed;
    private float rotationAngle;
    private float gravity = 0;

    public Rectangle Hitbox = new();

    // circular queue implementation
    private static int trailSize = 5;
    int count = 0;
    int start = 0;
    (Vector2, float)[] trails = new (Vector2, float)[trailSize];
    int increment = 255 / trailSize;

    private static float frameTime = 0.05f;
    float currTime = frameTime;

    public bool WasHit = false;
    Random r = new Random();

    public Bullet()
    {
        Random _r = new Random();
        Direction.Y = (float)r.NextDouble() * -.1f;
        Direction.X = (float)r.NextDouble() - .5f;
        rotationSpeed = (float)r.NextDouble() * 90 - 45;
        gravity = MainGame.GravityAcceleration;
        //var angle = r.NextDouble() * Math.PI;
        //var speed = 5f;
        //Speed = new Vector2((float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed);
    }

    public Bullet(Vector2 direction, float rotation)
    {
        //Random r = new Random();
        //rotationSpeed = (float)r.NextDouble() * 90 - 45;
        Direction = direction;
        rotationAngle = rotation;
        Texture = MainGame.handTexture;
        var w = Texture.Width;
        var h = Texture.Height;
        Hitbox = new Rectangle((int)Position.X - w / 2, (int)Position.Y - h / 2, w, h);
    }

    float blinkTimer = 0;
    bool blink = false;

    public void CheckHit(Enemy enemy)
    {
        if (WasHit) return;
        if (Hitbox.Intersects(enemy.Hitbox))
        {
            enemy.TakeDamage();
            WasHit = true;
        }
        else if (CheckOutOfBounds())
        {
            MainGame.Sounds[0].Play();
            WasHit = true;
        }
        /*
        var randomangle = r.NextDouble() * MathHelper.Pi;
        var randomdir = new Vector2((float)Math.Cos(randomangle), (float)Math.Sin(randomangle));
        randomdir.Normalize();
        Direction = -Direction;// + randomdir;
        //Direction.Normalize();
        rotationAngle -= MathHelper.Pi;
        */
    }

    private bool CheckOutOfBounds()
    {
        return Position.X > MainGame.ScreenWidth || Position.X < 0
            || Position.Y < 0 || Position.Y > MainGame.ScreenHeight;
    }

    public override void Update(float deltaTime)
    {
        Direction.Y += gravity * deltaTime;
        Position += Direction * Speed * deltaTime;
        rotationAngle += rotationSpeed * deltaTime;

        //DecayTrail();
        //AddTrail(deltaTime);

        currTime += deltaTime;
        if (currTime > frameTime)
        {
            currTime = 0;
            MainGame.Particles
                .Add(new Particle(Position, rotationAngle, 0.2f, FadeEffect.FadeOutScale)
                {
                    Speed = Speed / 5
                });
        }

        Hitbox.X = (int)Position.X - Texture.Width / 2;
        Hitbox.Y = (int)Position.Y - Texture.Height / 2;
        Hitbox.Width = Texture.Width;
        Hitbox.Height = Texture.Height;
    }

    private void DecayTrail()
    {
        // trail array
        for (int i = 0; i < trails.Length; i++)
        {
            (var pos, var life) = trails[i];
            if (life <= 0) continue;
            trails[i] = (pos, life - 0.05f);
        }
    }

    private void AddTrail(float deltaTime)
    {
        currTime += deltaTime;
        if (currTime > frameTime)
        {
            currTime = 0;
            trails[start] = (Position, .8f);
            start++;
            if (count < trailSize - 1) count++;
            if (start > trailSize - 1) start = 0;
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // draw trail
        /*
        int index = start;
        for (int i = 0; i < trailSize; i++)
        {
            if (index > count) index = 0;
            (Vector2 pos, float life) = trails[index];
            spriteBatch.Draw(MainGame.ParticleTrailTexture, pos, null,
                Color.White * life, rotationAngle + MathHelper.PiOver2,
                new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);
            index++;
        }
        */

        spriteBatch.Draw(MainGame.particleTexture, Position, null, Color.White, rotationAngle + MathHelper.PiOver2,
            new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);
    }
}
