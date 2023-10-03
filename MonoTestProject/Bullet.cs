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
    public float Speed = 0;
    public float Damage = 0;
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
    private Vector2 textureOrigin;

    private static float particleSpawnTime = 0.02f;
    float currTime = particleSpawnTime;

    public bool WasHit = false;
    Random r = new Random();

    public float phase = MathHelper.PiOver2;
    public float Phase
    {
        get => phase;
        set
        {
            phase = value;
            //cosA =  Direction.X + (float)Math.Cos(phase);
            //sinA =  Direction.Y + (float)Math.Sin(phase);
        }
    }

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

    public Bullet(Vector2 position, Vector2 direction, float rotation, float hitboxWidth = 0, float hitboxHeight = 0, Texture2D texture = null)
    {
        //Random r = new Random();
        //rotationSpeed = (float)r.NextDouble() * 90 - 45;
        Position = position;
        Direction = direction;
        rotationAngle = rotation;

        direction.Normalize();
        cosA = direction.X;// (float)Math.Cos(rotationAngle);
        sinA = direction.Y;// (float)Math.Sin(rotationAngle);

        Hitbox = new Rectangle();
        Hitbox.Width = (int)hitboxWidth;
        Hitbox.Height = (int)hitboxHeight;
        UpdateHitBox();

        if (texture != null)
        {
            Texture = texture;
        }
        else
        {
            Texture = MainGame.particleTexture;
        }
        textureOrigin = new Vector2(Texture.Width / 2, Texture.Height / 2);
    }

    private void UpdateHitBox()
    {
        Hitbox.X = (int)Position.X - Hitbox.Width / 2;
        Hitbox.Y = (int)Position.Y - Hitbox.Height / 2;
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
            var timescale = 10;
            phase += deltaTime * timescale;
            //float t = Speed * deltaTime;
            var sinT = (float)Math.Sin(phase);
            //var cosT = (float)Math.Cos(distanceTravelled);
            var yAmp = 30f * timescale;
            var xAmp = 15f * timescale;
            Position.X += (xAmp * cosA * sinT + yAmp * sinA * sinT + Speed * Direction.X) * deltaTime;
            Position.Y += (xAmp * sinA * sinT - yAmp * cosA * sinT + Speed * Direction.Y) * deltaTime;
        }
        else
        {
            // straight course
            Position += Direction * Speed * deltaTime;
        }

        rotationAngle += rotationSpeed * deltaTime;

        currTime += deltaTime;
        //if (currTime > particleSpawnTime)
        //{
        //    currTime = 0;
        //    MainGame.Entities
        //        .Add(new Particle(Position, rotationAngle, 0.06f, Texture, FadeEffect.FadeOutScale)
        //        {
        //            //Speed = Speed / 5
        //        });
        //}

        UpdateHitBox();
        CheckHit();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, null, Color.White, rotationAngle,
            textureOrigin, Vector2.One, SpriteEffects.None, 0f);
    }
}
