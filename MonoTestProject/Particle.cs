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

    private Vector2 Speed;
    private float rotationSpeed;
    private float rotationAngle;

    public Particle()
    {
        Random r = new Random();
        Speed.Y = (float)r.NextDouble() * -10 - 5;
        Speed.X = (float)r.NextDouble() * 10 - 5;
        rotationSpeed = (float)r.NextDouble() * 90 - 45;
        //var angle = r.NextDouble() * Math.PI;
        //var speed = 5f;
        //Speed = new Vector2((float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed);
    }

    public void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Speed.Y += MainGame.GravityAcceleration * deltaTime;
        Position += Speed;
        rotationAngle += rotationSpeed * deltaTime;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, null, Color.White, rotationAngle,
            new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);
    }
}
