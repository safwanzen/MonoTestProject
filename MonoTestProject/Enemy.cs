using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTestProject;

public class Enemy
{
    private float speed = 0;
    private Vector2 origin = new();

    public Texture2D Texture;
    public Vector2 Position = new();
    public Vector2 Direction = new(); // for now make it move slowly towards character
    public float Rotation = 0;

    public Rectangle hitBox = new();

    public Enemy()
    {

    }

    private void CheckParticleHit(Particle particle)
    {

    }

    public void Update(float deltaTime)
    {
        Position += Direction * speed * deltaTime;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, null,
            Color.White, Rotation + MathHelper.PiOver2,
            new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);
    }
}
