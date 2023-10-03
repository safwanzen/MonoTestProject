using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTestProject;

public enum FadeEffect
{
    None,
    FadeOut,
    FadeOutScale
}

public class Particle : Entity
{
    public Vector2 Position = Vector2.Zero;
    public float Speed = 0;

    private float _rotationRad = 0;

    public float RotationRad
    { 
        get => _rotationRad;
        set
        {
            _rotationRad = value;
            direction = new Vector2((float)Math.Cos(value), (float)Math.Sin(value));
            direction.Normalize();
        } 
    }

    private Vector2 direction;
    private float lifetime = 0f;
    private float initialLife;
    private float scale = 1f;
    private Texture2D texture;
    private Vector2 textureOrigin;

    FadeEffect fadeEffect = FadeEffect.FadeOut;

    public Particle(Vector2 position, float rotation, float lifetime, Texture2D texture, FadeEffect fadeEffect)
    {
        Position = position;
        RotationRad = rotation;
        initialLife = lifetime;
        this.lifetime = lifetime;
        this.fadeEffect = fadeEffect;
        this.texture = texture;
        textureOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
    }

    public Particle(Vector2 position, float rotation, float lifetime)
    {
        Position = position;
        RotationRad = rotation;
        initialLife = lifetime;
        this.lifetime = lifetime;
        texture = MainGame.particleTexture;
        textureOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
    }

    public override void Update(float dt)
    {
        lifetime -= dt;
        Position += Speed * direction * dt;
        if (fadeEffect == FadeEffect.FadeOutScale)
        {
            scale += dt * 2;
        }
        IsAlive = lifetime > 0f;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Color color = Color.White;

        if (fadeEffect == FadeEffect.FadeOut || fadeEffect == FadeEffect.FadeOutScale)
            color = Color.White * (lifetime / initialLife);

        spriteBatch.Draw(texture, Position, null, color,
            RotationRad, textureOrigin, new Vector2(scale), SpriteEffects.None, 0f);
    }
}
