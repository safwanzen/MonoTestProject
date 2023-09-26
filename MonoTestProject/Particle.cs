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
    public Vector2 Speed;

    private float rotationSpeed;
    private float rotationAngle;
    private float gravity = 0;

    public Particle()
    {
        Random r = new Random();
        Speed.Y = (float)r.NextDouble() * -10 - 5;
        Speed.X = (float)r.NextDouble() * 10 - 5;
        rotationSpeed = (float)r.NextDouble() * 90 - 45;
        gravity = MainGame.GravityAcceleration;
        //var angle = r.NextDouble() * Math.PI;
        //var speed = 5f;
        //Speed = new Vector2((float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed);
    }

    public Particle(Vector2 speed, float rotation)
    {
        //Random r = new Random();
        //rotationSpeed = (float)r.NextDouble() * 90 - 45;
        Speed = speed;
        rotationAngle = rotation;
    }

    float blinkTimer = 0;
    bool blink = false;

    public void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        blinkTimer += deltaTime;
        if (blinkTimer > .1) 
        {
            blinkTimer = 0;
            blink = !blink;
        }

        Speed.Y += gravity * deltaTime;
        Position += Speed;
        rotationAngle += rotationSpeed * deltaTime;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, null, Color.Wheat, rotationAngle + MathHelper.PiOver2,
            new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);
    }
}
