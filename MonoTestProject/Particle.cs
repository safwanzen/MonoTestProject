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

    private float speed = 15;
    private float rotationSpeed;
    private float rotationAngle;
    private float gravity = 0;

    // circular queue implementation
    private static int trailSize = 5;
    int count = 0;
    int start = 0;
    (Vector2, float)[] trails = new (Vector2, float)[trailSize];
    int increment = 255 / trailSize;

    private static double frameTime = 0.0333;
    double currTime = 0;

    public Particle()
    {
        Random r = new Random();
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
    }

    float blinkTimer = 0;
    bool blink = false;

    public void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        //blinkTimer += deltaTime;
        //if (blinkTimer > .1) 
        //{
        //    blinkTimer = 0;
        //    blink = !blink;
        //}

        Direction.Y += gravity * deltaTime;
        Position += Direction * speed * deltaTime;
        rotationAngle += rotationSpeed * deltaTime;

        // trail array
        currTime += deltaTime;

        if (currTime > frameTime)
        {
            currTime = 0;
            trails[start] = (Position, rotationAngle);
            start++;
            if (count < trailSize - 1) count++;
            if (start > trailSize - 1) start = 0;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        int index = start;
        for (int i = 0; i < count; i++)
        {
            if (index > count) index = 0;
            (Vector2 pos, float rot) = trails[index];
            spriteBatch.Draw(MainGame.ParticleTrailTexture, pos, null,
                new Color(Color.Black, i * increment), rot + MathHelper.PiOver2,
                new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);
            index++;
        }

        spriteBatch.Draw(Texture, Position, null, Color.Wheat, rotationAngle + MathHelper.PiOver2,
            new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);
    }
}
