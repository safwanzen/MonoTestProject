using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTestProject;

public class Weapon : Entity
{
    private float shootTimer = 0.5f;
    private float chargeTimer = 0;
    private float maxChargeTime = 1f;
    private bool isCharging = true;

    public bool IsAttacking
    {
        get; 
        private set;
    }

    public void Attack(Vector2 position, Vector2 direction, float rotation)
    {
        if (shootTimer <= 0)
        {
            shootTimer = 0.05f;
            MainGame.Bullets.Add(new(
                position, direction, rotation,
                new Sprite(MainGame.BulletTextureLarge, 
                    new Rectangle(0, 0, 16, 16), 
                    new Vector2(6, 6))
                )
            {
                Speed = 400,
                Damage = 4
            });
        }
    }

    public void ReleaseCharge(Vector2 position, Vector2 direction, float rotation)
    {
        if (chargeTimer > maxChargeTime)
        {
            chargeTimer = 0;
            var chargedspeed = 800f;
            //MainGame.Bullets.Add(new Bullet(bulletpos, facingDirection, 0, 28, 28, MainGame.BulletTextureXLarge)
            MainGame.Bullets.Add(new Bullet(position, direction, 0,
                new Sprite(MainGame.BulletTextureLarge,
                    new Rectangle(0, 0, 16, 16),
                    new Vector2(6, 6)),
                28, 28)
            {
                Speed = chargedspeed,
                Damage = 12,
                BulletType = BulletType.Charged
            });

            var b1 = new Bullet(position, direction, 0,
                new AnimatedSprite(MainGame.BulletSheet, new Rectangle(0, 0, 16, 16), new Vector2(8, 8), 3, 30, true),
                12, 12)
            {
                Speed = chargedspeed,
                Damage = 4,
                Wavy = true,
            };
            b1.Phase = MathHelper.PiOver2;

            var b2 = new Bullet(position, direction, 0,
                new AnimatedSprite(MainGame.BulletSheet, new Rectangle(0, 0, 16, 16), new Vector2(8, 8), 3, 30, true),
                12, 12)
            {
                Speed = chargedspeed,
                Damage = 4,
                Wavy = true,
            };
            b2.Phase = MathHelper.PiOver2 + MathHelper.Pi;

            MainGame.Bullets.Add(b1);
            MainGame.Bullets.Add(b2);
        }
    }

    public void Charge(bool charging)
    {
        isCharging = charging;
    }

    public override void Update(float deltaTime)
    {
        if (isCharging)
        {
            if (chargeTimer <= maxChargeTime)
                chargeTimer += deltaTime;
        }
        else chargeTimer = 0;

        shootTimer -= deltaTime;
        base.Update(deltaTime);
    }
}
