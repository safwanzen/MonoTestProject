using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoTestProject;

public class Character
{
    public Texture2D Texture;
    public Vector2 Position;
    public Vector2 Speed;
    public float Rotation;

    private const float speedMagnitude = 500;

    // circular queue implementation
    int trailSize = 5;
    int count = 0;
    int start = 0;
    public (Vector2, float)[] trails;
    
    const double frameTime = 0.050;
    double currTime = 0;

    int increment;

    public Character()
    {
        increment = 255 / trailSize;
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
        int index = start;
        for (int i = 0; i < count; i++)
        {
            if (index > count) index = 0;
            (Vector2 pos, float rot) = trails[index];
            spriteBatch.Draw(Texture, pos, null, 
                Color.White, rot + MathHelper.PiOver2,
                new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);
            index++;
        }
        spriteBatch.Draw(Texture, Position, null,
            Color.White, Rotation + MathHelper.PiOver2,
            new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);
    }
}
