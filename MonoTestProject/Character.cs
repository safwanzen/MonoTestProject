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

    private const float speedMagnitude = 500;

    // circular queue implementation
    int trailSize = 4;
    int count = 0;
    int start = 0;
    public (Vector2, float)[] trails;
    
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

    public Character()
    {
        Texture = MainGame.handTexture;
        trails = new (Vector2, float)[trailSize];
    }


    public override void Update(float deltaTime)
    {
        var kstate = Keyboard.GetState();
        //var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        var mousestate = Mouse.GetState();
        var facingDirection = mousestate.Position.ToVector2() - Position;
        if (facingDirection.Length() > 0) facingDirection.Normalize();
        Rotation = (float)Math.Atan2(facingDirection.Y, facingDirection.X);

        if (InputManager.IsDown(Keys.W)) { Speed.Y = -1; }
        else if (InputManager.IsDown(Keys.S)) { Speed.Y = 1; }
        else { Speed.Y = 0; }

        if (InputManager.IsDown(Keys.A)) { Speed.X = -1; }
        else if (InputManager.IsDown(Keys.D)) { Speed.X = 1; }
        else { Speed.X = 0; }

        shootParticleTimer -= deltaTime;
        //if (MainGame.PrevLMBState == ButtonState.Released && mousestate.LeftButton == ButtonState.Pressed && shootParticleTimer <= 0)
        if (InputManager.IsPressed(MouseButtons.LeftButton) && shootParticleTimer <= 0)
        {
            shootParticleTimer = 0.05f;

            var randAngle = Rotation + (float)random.NextDouble() * 0.3 - 0.15;
            var dirFluctuation = new Vector2((float)Math.Cos(randAngle), (float)Math.Sin(randAngle));
            var newDirection = facingDirection + dirFluctuation;
            newDirection.Normalize();

            MainGame.Bullets.Add(new Bullet(direction: facingDirection, rotation: Rotation)
            {
                Position = Position, /*+ new Vector2(random.Next(20) - 10, random.Next(20) - 10) */
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
                MainGame.Bullets.Add(new Bullet(direction: facingDirection, rotation: Rotation)
                {
                    Position = Position, /*+ new Vector2(random.Next(20) - 10, random.Next(20) - 10) */
                    Speed = 1000,
                    Damage = 12,
                    BulletType = BulletType.Charged
                });
                MainGame.Bullets.Add(new Bullet(direction: facingDirection, rotation: Rotation)
                {
                    Position = Position - facingDirection * 5, /*+ new Vector2(random.Next(20) - 10, random.Next(20) - 10) */
                    Speed = 1000,
                    Damage = 4,
                    //BulletType = BulletType.Charged,
                    Wavy = true,
                    phase = MathHelper.PiOver2
                });
                MainGame.Bullets.Add(new Bullet(direction: facingDirection, rotation: Rotation)
                {
                    Position = Position - facingDirection * 5, /*+ new Vector2(random.Next(20) - 10, random.Next(20) - 10) */
                    Speed = 1000,
                    Damage = 4,
                    //BulletType = BulletType.Charged,
                    Wavy = true,
                    phase = -MathHelper.PiOver2
                });
                for (int i = 0; i < 10; i++)
                {
                    var p = new Particle(Position, Rotation + (float)random.NextDouble() * (float)MathHelper.Pi - (float)MathHelper.PiOver2, 0.3f)
                    {
                        Speed = (float)random.NextDouble() * 400 + 100
                    };
                    MainGame.Entities.Add(p);
                }
                MainGame.Sounds[6].Play();
            }
            else
            {
                flashTime = 0.1f;
                characterFlashing = false;
                currChargeTime = 0;
            }
        }

        // physics
        //Speed.Y += MainGame.GravityAcceleration * deltaTime;
        //Position.Y += Speed.Y;

        if (Speed.Length() > 0) Speed.Normalize();
        Position += Speed * speedMagnitude * deltaTime;

        // check for boundary collision
        if (Position.X > MainGame.ScreenWidth - Texture.Width / 2)
        {
            Position.X = MainGame.ScreenWidth - Texture.Width / 2;
        }
        else if (Position.X < Texture.Width / 2)
        {
            Position.X = Texture.Width / 2;
        }

        if (Position.Y > MainGame.ScreenHeight - Texture.Height / 2)
        {
            Position.Y = MainGame.ScreenHeight - Texture.Height / 2;
            //Speed.Y = -Speed.Y * 0.97f;
        }
        else if (Position.Y < Texture.Height / 2)
        {
            Position.Y = Texture.Height / 2;
            //Speed.Y = -Speed.Y * 0.97f;
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
        spriteBatch.Draw(Texture, Position, null,
            !characterFlashing ? Color.White : Color.OrangeRed, Rotation + MathHelper.PiOver2,
            new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);
    }
}
