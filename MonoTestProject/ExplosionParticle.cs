using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTestProject;

public class ExplosionParticle : Particle
{
    AnimatedSprite sprite;
    public ExplosionParticle(Vector2 worldposition, float rotation, float lifetime, Texture2D texture, FadeEffect fadeEffect) : base(worldposition, rotation, lifetime, texture, fadeEffect)
    {
        sprite = new AnimatedSprite(texture, new Rectangle(0, 0, 32, 32), new Vector2(16, 16), 2, 30);
        sprite.OnAnimationEnd = () => { IsAlive = false; };
    }

    public override void Update(float dt)
    {
        sprite.Update(dt);
        base.Update(dt);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        sprite.Draw(spriteBatch, ScreenPosition, RotationRad, Color.White, SpriteEffects.None, scaleX, scaleY);
    }
}
