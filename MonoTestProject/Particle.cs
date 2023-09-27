using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTestProject;

public class Particle
{
    public Vector2 Position;
    public Texture2D Texture;
    public Vector2 Direction;

    private float speed = 600;
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

    public Particle()
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

    public Particle(Vector2 direction, float rotation)
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
        if (!Hitbox.Intersects(enemy.Hitbox)) return;
        if (!CheckOutOfBounds()) return;
        WasHit = true;
        /*
        var randomangle = r.NextDouble() * MathHelper.Pi;
        var randomdir = new Vector2((float)Math.Cos(randomangle), (float)Math.Sin(randomangle));
        randomdir.Normalize();
        Direction = -Direction;// + randomdir;
        //Direction.Normalize();
        rotationAngle -= MathHelper.Pi;
        */
        enemy.TakeDamage();
    }

    private bool CheckOutOfBounds()
    {
        return Position.X > 500 || Position.X < -10
            || Position.Y < -10 || Position.Y > 500;
    }

    public void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Direction.Y += gravity * deltaTime;
        Position += Direction * speed * deltaTime;
        rotationAngle += rotationSpeed * deltaTime;

        // trail array
        for (int i = 0; i < trails.Length; i++)
        {
            (var pos, var life) = trails[i];
            if (life <= 0) continue;
            trails[i] = (pos, life - 0.05f);
        }

        currTime += deltaTime;
        if (currTime > frameTime)
        {
            currTime = 0;
            trails[start] = (Position, .8f);
            start++;
            if (count < trailSize - 1) count++;
            if (start > trailSize - 1) start = 0;
        }

        Hitbox.X = (int)Position.X - Texture.Width / 2;
        Hitbox.Y = (int)Position.Y - Texture.Height / 2;
        Hitbox.Width = Texture.Width;
        Hitbox.Height = Texture.Height;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
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

        spriteBatch.Draw(Texture, Position, null, Color.Wheat, rotationAngle + MathHelper.PiOver2,
            new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);
    }
}
