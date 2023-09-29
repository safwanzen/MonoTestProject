using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTestProject;

public class ParticleTrail
{
    public Vector2 Position;
    public float Rotation;
    public bool IsAlive = true;

    private float lifetime = 0f;
    private float initialLife;

    public ParticleTrail(Vector2 position, float rotation, float lifetime)
    {
        Position = position;
        Rotation = rotation;
        initialLife = lifetime;
        this.lifetime = lifetime;

    }

    public void Update(float dt)
    {
        lifetime -= dt;
        IsAlive = lifetime > 0f;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(MainGame.ParticleTrailTexture, Position, null, Color.White * (lifetime / initialLife),
            Rotation + MathHelper.PiOver2, new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);
    }
}
