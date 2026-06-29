using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;
using Survivor.Manager;

namespace Survivor;

public class Player : Base
{
    private const float decell = 0.99f;
    private const int width = 40;
    private const int height = 40;
    private float health;
    private Vector2 WorldPosition;
    private Vector2 Speed;
    private Vector2 direction;
    private Vector2 facingDirection = new Vector2(1, 0);
    private float maxAccel = 2300f;
    private float maxSpeed = 800f;
    private bool u, d, l, r;
    private float projectileSpeed = 800f;
    private float brakeRate = 0.87f;
    private float minSpeed = 9f;

    public Player() { }

    public override void Update(GameTime gameTime)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        maxAccel = 2300;

        // input
        u = InputManager.IsDown(Keys.W);
        d = InputManager.IsDown(Keys.S);
        l = InputManager.IsDown(Keys.A);
        r = InputManager.IsDown(Keys.D);
        bool braking = InputManager.IsDown(Keys.I);
        bool shoot = InputManager.IsReleased(Keys.O);

        if (u && d) { direction.Y = 0; }
        else if (u) 
        { 
            direction.Y = -1;
            facingDirection.Y = -1;
        }
        else if (d) 
        { 
            direction.Y = 1;
            facingDirection.Y = 1;
        }
        else { direction.Y = 0; }

        if (l && r) { direction.X = 0; }
        else if (l)
        { 
            direction.X = -1;
            facingDirection.X = -1;
        }
        else if (r)
        { 
            direction.X = 1;
            facingDirection.X = 1;
        }
        else { direction.X = 0; }

        if (direction.Length() > 0) { direction.Normalize(); }

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
            Log.Debug("dir {0} {1}", direction, projectileSpeed);
            var b = new Projectile()
            {
                Speed = Vector2.Normalize(facingDirection) * projectileSpeed,
                WorldPosition = WorldPosition
            };
            EntityManager.Manager.AddObject(b);
        }

        if (Speed.Length() < minSpeed) Speed = Vector2.Zero;

        // check for boundary
        // check for map collision
        // check for enemy collision
    }

    public override void Draw(SpriteBatch spritebatch)
    {
        spritebatch.DrawRect(
            new Vector2(WorldPosition.X - width / 2, WorldPosition.Y - height / 2),
            width, height, Color.WhiteSmoke);
    }
}
