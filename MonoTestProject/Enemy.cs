using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTestProject;

public class Enemy : Entity
{
    private float speed = 0;
    private Vector2 origin = new();

    public Texture2D Texture;
    public Vector2 Position = new();
    public Vector2 Direction = new(); // for now make it move slowly towards character
    public float Rotation = 0;

    public Rectangle Hitbox = new();

    public float HitPoints = 10;

    public bool Hit = false;
    
    private const float damageTime = 0.033f;
    private float damageFlashTimer = 0;

    public Enemy(Vector2 position)
    {
        Texture = MainGame.handTexture;
        var w = Texture.Width;
        var h = Texture.Width;
        Position = position;
        Hitbox = new Rectangle((int)Position.X - w / 2, (int)Position.Y - h / 2, w, h);
    }

    private void CheckParticleHit(Bullet particle)
    {
    }

    public void TakeDamage()
    {
        HitPoints -= 1;
        //Console.WriteLine("took damage");
        Hit = true;
        damageFlashTimer = damageTime;
        MainGame.Sounds[1].Play();
        if (HitPoints <= 0)
        {
            Random random = new();
            for (int a = 0; a < 10; a++)
            {
                var p = new Particle(Position, (float)(random.NextDouble() * MathHelper.Pi * 2), 0.5f)
                {
                    Speed = (float)random.NextDouble() * 500 + 50
                };
                MainGame.Sounds[5].Play(0.1f, 0, 0);
                MainGame.Particles.Add(p);
            }
            MainGame.Enemies.Remove(this);
        }
    }

    public override void Update(float deltaTime)
    {
        Position += Direction * speed * deltaTime;

        Hitbox.X = (int)Position.X - Texture.Width / 2;
        Hitbox.Y = (int)Position.Y - Texture.Height / 2;
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
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Hit ? MainGame.particleTexture : Texture, Position, null,
            Color.White, Rotation + MathHelper.PiOver2,
            new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);
    }
}
