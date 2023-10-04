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

    public override void Update(float deltaTime)
    {
        shootTimer -= deltaTime;
        base.Update(deltaTime);
    }
}
