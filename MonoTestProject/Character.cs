using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoTestProject;

public class Character : Entity
{
    public Texture2D Texture;
    public Vector2 Position;
    public Vector2 Speed;
    public float Rotation;

    private Sprite sprite;
    private AnimatedSprite bulletSprite;

    public Vector2 direction;
    public float speedMagnitude = 0;

    // circular queue implementation
    const int trailSize = 4;
    int count = 0;
    int start = 0;
    public (Vector2, float)[] trails = new (Vector2, float)[trailSize];
    
    const float trailSpawnTime = 0.05f;
    float currTrailSpawnTime = trailSpawnTime;

    Random random = new Random();

    bool characterFlashing = false;
    float flashTime = 0;
    // charged shot
    float maxChargeTime = .5f;
    float currChargeTime = 0f;
    bool fullyCharged = false;
    double shootParticleTimer = 0;

    bool u, d, l, r;

    public Character()
    {
        bulletSprite = new AnimatedSprite(MainGame.BulletSheet, new Rectangle(0, 0, 16, 16), new Vector2(8, 8), 3, 30, true);
        Texture = MainGame.handTexture;
        sprite = new Sprite(
            MainGame.handTexture,
            new Rectangle(0, 0, MainGame.handTexture.Width, MainGame.handTexture.Height),
            new Vector2(16, 22));
    }


    public override void Update(float deltaTime)
    {
        var mousestate = Mouse.GetState();
        var facingDirection = mousestate.Position.ToVector2() - Position;
        if (facingDirection.Length() > 0) facingDirection.Normalize();
        Rotation = (float)Math.Atan2(facingDirection.Y, facingDirection.X);

        

        shootParticleTimer -= deltaTime;
        //if (MainGame.PrevLMBState == ButtonState.Released && mousestate.LeftButton == ButtonState.Pressed && shootParticleTimer <= 0)
        if (InputManager.IsPressed(MouseButtons.LeftButton) && shootParticleTimer <= 0)
        {
            shootParticleTimer = 0.05f;

            var randAngle = Rotation + (float)random.NextDouble() * 0.3 - 0.15;
            var dirFluctuation = new Vector2((float)Math.Cos(randAngle), (float)Math.Sin(randAngle));
            var newDirection = facingDirection + dirFluctuation;
            newDirection.Normalize();

            MainGame.Bullets.Add(new Bullet(Position, facingDirection, 0,
                new AnimatedSprite(MainGame.BulletSheet, new Rectangle(0, 0, 16, 16), new Vector2(8, 8), 3, 30, true), 
                12, 12)
            {
                Speed = 100,
                Damage = 4
            });

            MainGame.Sounds[2].Play();

        }

        //if (mousestate.LeftButton == ButtonState.Pressed)
        if (InputManager.IsDown(MouseButtons.LeftButton))
        {
            currChargeTime += deltaTime;
            flashTime -= deltaTime;
            if (flashTime < 0)
            {
                characterFlashing = !characterFlashing;
                if (currChargeTime >= maxChargeTime) flashTime = 0.03f;
                else flashTime = 0.1f;
                //else flashTime = 0.5f;
            }

            fullyCharged = currChargeTime > maxChargeTime;
        }

        //if (mousestate.LeftButton == ButtonState.Released)
        if (InputManager.IsUp(MouseButtons.LeftButton))
        {
            // charged shot
            if (fullyCharged)
            {
                fullyCharged = false;
                shootParticleTimer = 0.2;
                var chargedspeed = 100f;
                var bulletpos = Position + facingDirection * 20;
                //MainGame.Bullets.Add(new Bullet(bulletpos, facingDirection, 0, 28, 28, MainGame.BulletTextureXLarge)
                MainGame.Bullets.Add(new Bullet(Position, facingDirection, 0,
                    new AnimatedSprite(MainGame.BulletSheet, new Rectangle(0, 0, 16, 16), new Vector2(8, 8), 3, 30, true), 
                    12, 12)
                {
                    Speed = chargedspeed,
                    Damage = 12,
                    BulletType = BulletType.Charged
                });

                var b1 = new Bullet(Position, facingDirection, 0,
                    new AnimatedSprite(MainGame.BulletSheet, new Rectangle(0, 0, 16, 16), new Vector2(8, 8), 3, 30, true), 
                    12, 12)
                {
                    Speed = chargedspeed,
                    Damage = 4,
                    Wavy = true,
                };
                b1.Phase = MathHelper.PiOver2;

                var b2 = new Bullet(Position, facingDirection, 0,
                    new AnimatedSprite(MainGame.BulletSheet, new Rectangle(0, 0, 16, 16), new Vector2(8, 8), 3, 30, true), 
                    12, 12)
                {
                    Speed = chargedspeed,
                    Damage = 4,
                    Wavy = true,
                };
                b2.Phase = MathHelper.PiOver2 + MathHelper.Pi;

                MainGame.Bullets.Add(b1);
                MainGame.Bullets.Add(b2);

                for (int i = 0; i < 10; i++)
                {
                    var p = new Particle(Position, Rotation + (float)random.NextDouble() * MathHelper.Pi - MathHelper.PiOver2, 0.3f)
                    {
                        Speed = (float)random.NextDouble() * 400 + 100
                    };
                    MainGame.Entities.Add(p);
                }
                MainGame.Sounds[6].Play();
            }
            else
            {
                flashTime = 0.3f;
                characterFlashing = false;
                currChargeTime = 0;
            }
        }

        // physics
        //Speed.Y += MainGame.GravityAcceleration * deltaTime;
        //Position.Y += Speed.Y;

        #region movement
        u = InputManager.IsDown(Keys.W);
        d = InputManager.IsDown(Keys.S);
        l = InputManager.IsDown(Keys.A);
        r = InputManager.IsDown(Keys.D);

        if (u && d) { direction.Y = 0; }
        else if (u) { direction.Y = -1; }
        else if (d) { direction.Y = 1; }
        else { direction.Y = 0; }

        if (l && r) { direction.X = 0; }
        else if (l) { direction.X = -1; }
        else if (r) { direction.X = 1; }
        else { direction.X = 0; }

        if (direction.Length() > 0) { direction.Normalize(); }

        float accel = 0f;
        if (u || d || l || r) accel = 8000f;

        float maxSpeed = 800f;
        //speedMagnitude += direction.Length() * accel * deltaTime;
        if (Speed.Length() < maxSpeed) 
            Speed += direction * accel * deltaTime;
        else
        {
            Speed = Vector2.Normalize(Speed) * maxSpeed;
        }
        Position += Speed * deltaTime;
        //if (Speed.Length() > 0) { Speed *= 0.95f; }
        Speed *= 0.95f;
        if (Math.Abs(Speed.X) < 3f) Speed.X = 0;
        if (Math.Abs(Speed.Y) < 3f) Speed.Y = 0;
        if (Speed.Length() < 9f) Speed = Vector2.Zero;
        //direction = Vector2.Zero;
        
 #endregion

        // check for boundary collision
        if (Position.X > MainGame.ScreenWidth - Texture.Width / 2)
        {
            Position.X = MainGame.ScreenWidth - Texture.Width / 2;
            Speed.X = 0;
        }
        else if (Position.X < Texture.Width / 2)
        {
            Position.X = Texture.Width / 2;
            Speed.X = 0;
        }

        if (Position.Y > MainGame.ScreenHeight - Texture.Height / 2)
        {
            Position.Y = MainGame.ScreenHeight - Texture.Height / 2;
            Speed.Y = 0;
        }
        else if (Position.Y < Texture.Height / 2)
        {
            Position.Y = Texture.Height / 2;
            Speed.Y = 0;
        }

        // trail array
        //for (int i = 0; i < trails.Length; i++)
        //{
        //    (var pos, var life) = trails[i];
        //    if (life <= 0) continue;
        //    trails[i] = (pos, life - 0.05f);
        //}

        currTrailSpawnTime += deltaTime;
        if (currTrailSpawnTime > trailSpawnTime)
        {
            currTrailSpawnTime = 0;
            trails[start] = (Position, Rotation);
            start++;
            if (count < trailSize - 1) count++;
            if (start > trailSize - 1) start = 0;
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < trails.Length; i++)
        {
            if (random.NextDouble() > 0.5) continue;
            (Vector2 pos, float rot) = trails[i];
            spriteBatch.Draw(MainGame.ParticleTrailTexture, pos, null, 
                Color.White * 0.5f, rot + MathHelper.PiOver2,
                new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);
        }
        // need to rotate sprite because it points up
        // if it points to the right no need to rotate
        //spriteBatch.Draw(Texture, Position, null,
        //    !characterFlashing ? Color.White : Color.OrangeRed, Rotation + MathHelper.PiOver2,
        //    new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);
        sprite.Draw(spriteBatch, Position, Rotation + MathHelper.PiOver2, !characterFlashing ? Color.White : Color.OrangeRed);
    }
}
