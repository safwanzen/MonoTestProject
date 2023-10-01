using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTestProject;

public class DamageParticle : Entity
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
    private string text;
    private float initialLife;
    private float scale = 1f;

    FadeEffect fadeEffect = FadeEffect.FadeOut;

    public DamageParticle(Vector2 position, float rotation, float lifetime, FadeEffect fadeEffect)
    {
        Position = position;
        RotationRad = rotation;
        initialLife = lifetime;
        this.lifetime = lifetime;
        this.fadeEffect = fadeEffect;
    }

    public DamageParticle(Vector2 position, float rotation, float lifetime, string text)
    {
        Position = position;
        RotationRad = rotation;
        initialLife = lifetime;
        this.lifetime = lifetime;
        this.text = text;
    }

    public override void Update(float dt)
    {
        lifetime -= dt;
        if (lifetime > initialLife * 0.75) Position += Speed * direction * dt;
        IsAlive = lifetime > 0f;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(MainGame.Font, text, Position, Color.AntiqueWhite);
    }
}
