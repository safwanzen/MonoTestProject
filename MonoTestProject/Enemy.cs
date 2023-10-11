using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoTestProject;

public class Enemy : Entity
{
    private float speed = 0;
    private Vector2 origin = new();

    public Texture2D Texture;
    public Vector2 Direction = new(); // for now make it move slowly towards character
    public float Rotation = 0;

    public Rectangle Hitbox = new();

    public float HitPoints = 10;

    public bool Hit = false;
    
    private const float damageTime = 0.033f;
    private float damageFlashTimer = 0;
    private bool oneHitKill = true;

    private const float immunityTime = .5f;
    private float immunityCounter = 0;

    public Enemy(Vector2 worldposition)
    {
        Texture = MainGame.handTexture;
        var w = Texture.Width;
        var h = Texture.Width;
        WorldPosition = worldposition;
        Hitbox = new Rectangle((int)worldposition.X - w / 2, (int)worldposition.Y - h / 2, w, h);
    }

    private void CheckParticleHit(Bullet particle)
    {
    }

    public bool TakeDamage(float damage)
    {
        //if (immunityCounter > 0) return false;
        //immunityCounter = 1f;
        HitPoints -= damage;
        //Console.WriteLine("took damage");
        Hit = true;
        damageFlashTimer = damageTime;
        //MainGame.Sounds[1].Play();
        MainGame.Entities.Add(new DamageParticle(WorldPosition, -MathHelper.PiOver2, .8f, damage.ToString()) { Speed = 300 });
        if (HitPoints <= 0)
        {
            Random random = new();

            MainGame.Entities.Add(
                new ExplosionParticle(
                    WorldPosition, 0f,
                    1f, MainGame.ExplosionBeginTexture, FadeEffect.None
                ));

            //for (int a = 0; a < 4; a++)
            //{
            //    var p = new Particle(WorldPosition, (float)(random.NextDouble() * MathHelper.Pi * 2), 0.5f)
            //    {
            //        Speed = (float)random.NextDouble() * 500 + 50
            //    };
            //    //MainGame.Sounds[5].Play(0.1f, 0, 0);
            //    MainGame.Entities.Add(p);
            //}
            MainGame.Enemies.Remove(this);
            return true;
        }
        oneHitKill = false;
        return false;
    }

    public override void Update(float deltaTime)
    {
        if (immunityCounter > 0)
            immunityCounter -= deltaTime;
        else
            immunityCounter = 0;

        WorldPosition += Direction * speed * deltaTime;

        Hitbox.X = (int)WorldPosition.X - Texture.Width / 2;
        Hitbox.Y = (int)WorldPosition.Y - Texture.Height / 2;
        Hitbox.Width = Texture.Width;
        Hitbox.Height = Texture.Height;

        if (Hit && damageFlashTimer > 0)
        {
            damageFlashTimer -= deltaTime;
        }
        else
        {
            Hit = false;
        }
        base.Update(deltaTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Hit ? MainGame.particleTexture : Texture, ScreenPosition, null,
            Color.White, Rotation + MathHelper.PiOver2,
            new Vector2(16, 22), new Vector2(scaleX, scaleY), SpriteEffects.None, 0f);
    }
}
