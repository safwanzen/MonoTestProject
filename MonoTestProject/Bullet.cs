using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MonoTestProject;

public enum BulletType
{
    Normal,
    Charged
}

public class Bullet : Entity
{
    public Vector2 Position;
    public Texture2D Texture;
    public Vector2 Direction;
    public float Speed = 600;
    public float Damage = 1;
    public BulletType BulletType = BulletType.Normal;

    public bool Wavy = false;
        
    private float _angle;
    public float Angle
    {
        get => _angle;
        set
        {
            _angle = value;
            Direction = new Vector2((float)Math.Cos(value), (float)Math.Sin(value));
        }
    }

    private float rotationSpeed;
    private float rotationAngle;

    public Rectangle Hitbox = new();

    private static float frameTime = 0.05f;
    float currTime = frameTime;

    public bool WasHit = false;
    Random r = new Random();

    public float phase = MathHelper.PiOver2;
    float cosA;
    float sinA;

    public Bullet()
    {
        Random _r = new Random();
        Direction.Y = (float)r.NextDouble() * -.1f;
        Direction.X = (float)r.NextDouble() - .5f;
        rotationSpeed = (float)r.NextDouble() * 90 - 45;
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
        
        cosA = (float)Math.Cos(rotationAngle);
        sinA = (float)Math.Sin(rotationAngle);
    }

    private void CheckHit()
    {
        if (WasHit) return;
        if (CheckOutOfBounds())
        {
            MainGame.Sounds[0].Play();
            WasHit = true;
        }
        else
        {
            CheckEnemyCollision();
        }
    }

    private void CheckEnemyCollision()
    {
        for (int a = 0; a < MainGame.Enemies.Count; a++)
        {
            Enemy enemy = MainGame.Enemies[a];
            if (!Hitbox.Intersects(enemy.Hitbox)) continue;
            bool enemyDestroyed = enemy.TakeDamage(Damage);
            if (BulletType == BulletType.Charged) WasHit = !enemyDestroyed;
            else if (BulletType == BulletType.Normal) WasHit = true;
            //break;
        }
    }

    private bool CheckOutOfBounds()
    {
        return Position.X > MainGame.ScreenWidth || Position.X < 0
            || Position.Y < 0 || Position.Y > MainGame.ScreenHeight;
    }


    public override void Update(float deltaTime)
    {
        if (Wavy)
        {
            // wave trajectory
            phase += deltaTime * 20;
            //float t = Speed * deltaTime;
            var sinT = (float)Math.Sin(phase);
            //var cosT = (float)Math.Cos(distanceTravelled);
            var yAmp = 10f;
            var xAmp = 5f;
            Position.X += xAmp * cosA * sinT + yAmp * sinA * sinT + Speed * deltaTime * Direction.X;
            Position.Y += xAmp * sinA * sinT - yAmp * cosA * sinT + Speed * deltaTime * Direction.Y;
        }
        else
        {
            // straight course
            Position += Direction * Speed * deltaTime;
        }

        rotationAngle += rotationSpeed * deltaTime;

        currTime += deltaTime;
        if (currTime > frameTime)
        {
            currTime = 0;
            MainGame.Entities
                .Add(new Particle(Position, rotationAngle, 0.2f, FadeEffect.FadeOutScale)
                {
                    //Speed = Speed / 5
                });
        }

        Hitbox.X = (int)Position.X - Texture.Width / 2;
        Hitbox.Y = (int)Position.Y - Texture.Height / 2;
        Hitbox.Width = Texture.Width;
        Hitbox.Height = Texture.Height;

        CheckHit();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(MainGame.particleTexture, Position, null, Color.White, rotationAngle + MathHelper.PiOver2,
            new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);
    }
}
