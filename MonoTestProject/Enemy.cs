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

    public Rectangle Hitbox = new();

    public float HitPoints = 10;

    public bool Hit = false;
    
    private const float damageTime = 0.033f;
    private float damageFlashTimer = 0;

    public Enemy()
    {
        Texture = MainGame.handTexture;
        var w = Texture.Width;
        var h = Texture.Width;
        Hitbox = new Rectangle((int)Position.X - w / 2, (int)Position.Y - h / 2, w, h);
    }

    private void CheckParticleHit(Particle particle)
    {
    }

    public void TakeDamage()
    {
        HitPoints -= 1;
        //Console.WriteLine("took damage");
        Hit = true;
        damageFlashTimer = damageTime;
        MainGame.BulletHitSound.Play();
    }

    public void Update(float deltaTime)
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

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Hit ? MainGame.particleTexture : Texture, Position, null,
            Color.White, Rotation + MathHelper.PiOver2,
            new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);
    }
}
