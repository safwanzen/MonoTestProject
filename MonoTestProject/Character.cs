using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoTestProject;

public class Character
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
    
    const float frameTime = 0.05f;
    float currTime = frameTime;

    Random r = new Random();

    public Character()
    {
        trails = new (Vector2, float)[trailSize];
    }


    public void Update(GameTime gameTime)
    {
        var kstate = Keyboard.GetState();
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (kstate.IsKeyDown(Keys.W)) { Speed.Y = -1; }
        else if (kstate.IsKeyDown(Keys.S)) { Speed.Y = 1; }
        else { Speed.Y = 0; }

        if (kstate.IsKeyDown(Keys.A)) { Speed.X = -1; }
        else if (kstate.IsKeyDown(Keys.D)) { Speed.X = 1; }
        else { Speed.X = 0; }

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

        currTime += deltaTime;
        if (currTime > frameTime)
        {
            currTime = 0;
            trails[start] = (Position, Rotation);
            start++;
            if (count < trailSize - 1) count++;
            if (start > trailSize - 1) start = 0;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < trails.Length; i++)
        {
            if (r.NextDouble() > 0.5) continue;
            (Vector2 pos, float rot) = trails[i];
            spriteBatch.Draw(MainGame.ParticleTrailTexture, pos, null, 
                Color.White * 0.8f, rot + MathHelper.PiOver2,
                new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);
        }
        // need to rotate sprite because it points up
        // if it points to the right no need to rotate
        spriteBatch.Draw(Texture, Position, null,
            Color.White, Rotation + MathHelper.PiOver2,
            new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);
    }
}
