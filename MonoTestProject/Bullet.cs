using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoTestProject;

public enum BulletType
{
    Normal,
    Charged
}

public class Bullet : Entity
{
    public Vector2 Direction;
    public float Speed = 0;
    public float Damage = 0;
    public BulletType BulletType = BulletType.Normal;

    public bool Wavy = false;
        
    private Sprite Sprite;

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
    private readonly Sprite sprite;
    public Rectangle Hitbox = new();
    private Vector2 spriteOrigin;

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

    const int trailSize = 6;
    int trailIndex = 0;
    int trailCount = 0;
    Vector2[] trails = new Vector2[trailSize];

    Vector2 Trail
    {
        //get
        //{
        //    var index = trailIndex;
        //    trailIndex++;
        //    if (trailIndex > trailCount) { trailIndex = 0; }
        //    return trails[index];
        //}
        set
        {
            trails[trailIndex] = value;
            trailIndex++;
            if (trailCount < trailSize - 1) trailCount++;
            if (trailIndex > trailCount) { trailIndex = 0; }
        }
    }

    public Bullet(Vector2 position, Vector2 direction, float rotation, Sprite sprite, float hitboxWidth = 0, float hitboxHeight = 0)
    {
        //Random r = new Random();
        //rotationSpeed = (float)r.NextDouble() * 90 - 45;
        Position = position;
        Direction = direction;
        rotationAngle = rotation;
        this.sprite = sprite;
        direction.Normalize();
        cosA = direction.X;// (float)Math.Cos(rotationAngle);
        sinA = direction.Y;// (float)Math.Sin(rotationAngle);

        Hitbox = new Rectangle();
        Hitbox.Width = (int)hitboxWidth;
        Hitbox.Height = (int)hitboxHeight;
        UpdateHitBox();

        Sprite = sprite;
        spriteOrigin = new Vector2(8, 8);

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
        base.Update(deltaTime);
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

        //currTime += deltaTime;
        //if (currTime > particleSpawnTime)
        //{
        //    currTime = 0;
        //}
        //MainGame.Entities.Add(new Particle(Position, rotationAngle, 0.04f, Texture, FadeEffect.None));

        //currTime += deltaTime;
        //if (currTime > particleSpawnTime)
        //{
        //    currTime = 0;
        //    Trail = Position;
        //}

        sprite.Update(deltaTime);
        UpdateHitBox();
        CheckHit();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        //for (int i = trailIndex; i < trailCount; i++)
        //{
        //    spriteBatch.Draw(Texture, trails[i], null, Color.White, rotationAngle,
        //    textureOrigin, Vector2.One, SpriteEffects.None, 0f);
        //}
        //spriteBatch.Draw(Sprite, Position, null, Color.White, rotationAngle,
        //    spriteOrigin, Vector2.One, SpriteEffects.None, 0f);
        sprite.Draw(spriteBatch, ScreenPosition, rotationAngle, Color.White, SpriteEffects.None, scaleX, scaleY);
    }
}
