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

    public Particle(Vector2 worldposition, float rotation, float lifetime, Texture2D texture, FadeEffect fadeEffect)
    {
        WorldPosition = worldposition;
        RotationRad = rotation;
        initialLife = lifetime;
        this.lifetime = lifetime;
        this.fadeEffect = fadeEffect;
        this.texture = texture;
        textureOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
    }

    public Particle(Vector2 worldposition, float rotation, float lifetime)
    {
        WorldPosition = worldposition;
        RotationRad = rotation;
        initialLife = lifetime;
        this.lifetime = lifetime;
        texture = MainGame.particleTexture;
        textureOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
    }

    public override void Update(float dt)
    {
        if (!IsAlive) return;
        lifetime -= dt;
        WorldPosition += Speed * direction * dt;
        if (fadeEffect == FadeEffect.FadeOutScale)
        {
            scale += dt * 2;
        }
        IsAlive = lifetime > 0f;
        base.Update(dt);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Color color = Color.White;

        if (fadeEffect == FadeEffect.FadeOut || fadeEffect == FadeEffect.FadeOutScale)
            color = Color.White * (lifetime / initialLife);

        spriteBatch.Draw(texture, ScreenPosition, null, color,
            RotationRad, textureOrigin, new Vector2(scaleX, scaleY), SpriteEffects.None, 0f);
    }
}
