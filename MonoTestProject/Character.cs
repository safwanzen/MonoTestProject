using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTestProject;

public class Character
{
    public Texture2D Texture;
    public Vector2 Position;
    public Vector2 Speed;

    // circular queue implementation
    int size = 15;
    int count = 0;
    int start = 0;
    public Vector2[] trails;

    int increment;

    public Character()
    {
        increment = 255 / size;
        trails = new Vector2[size];
    }

    public void Update(GameTime gameTime)
    {
        var kstate = Keyboard.GetState();
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        if (kstate.IsKeyDown(Keys.W))
        {
            //Position.Y -= Speed.Y * deltaTime;
            Speed.Y = -20f;
        }

        if (kstate.IsKeyDown(Keys.S))
        {
            Position.Y += Speed.Y * deltaTime;
        }

        if (kstate.IsKeyDown(Keys.A))
        {
            Position.X -= Speed.X * deltaTime;
        }

        if (kstate.IsKeyDown(Keys.D))
        {
            Position.X += Speed.X * deltaTime;
        }

        // physics
        Speed.Y += MainGame.GravityAcceleration * deltaTime;
        Position.Y += Speed.Y;

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
            Speed.Y = -Speed.Y * 0.97f;
        }
        else if (Position.Y < Texture.Height / 2)
        {
            Position.Y = Texture.Height / 2;
            Speed.Y = -Speed.Y * 0.97f;
        }

        // trail array
        trails[start] = Position;
        start++;
        if (count < size - 1) count++;
        if (start > size - 1) start = 0;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        int index = start;
        for (int i = 0; i < count; i++)
        {
            if (index > count) index = 0;
            Vector2 pos = trails[index];
            spriteBatch.Draw(Texture, pos - new Vector2(Texture.Width / 2, Texture.Height / 2), new Color(Color.CornflowerBlue, i * 10 + 10));
            index++;
        }
        spriteBatch.Draw(Texture, Position, null, Color.White, 0f,
            new Vector2(Texture.Width / 2, Texture.Height / 2), Vector2.One, SpriteEffects.None, 0f);
    }
}
