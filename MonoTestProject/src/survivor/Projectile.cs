using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using Survivor.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survivor;

public class Projectile : Base
{
    private const float decell = 0.99f;
    private const float bounceDecell = 0.7f;
    private int width = 20, height = 20;

    public Vector2 WorldPosition { get; set; }
    public Vector2 Speed { get; set; }

    public override void Update(GameTime gameTime)
    {
        //Vector2 direction = Vector2.Normalize(Speed);

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        WorldPosition += Speed * dt;
        Speed *= decell;
        
        float x = WorldPosition.X;
        float y = WorldPosition.Y;
        float sx = Speed.X;
        float sy = Speed.Y;

        if (Speed.Length() < 9f)
        { 
            EntityManager.Manager.RemoveObject(this);
        }

        // collision
        // with screen edge
        bool outofBounds =
            WorldPosition.X + width / 2 > SurvivorGame.ScreenWidth
            || WorldPosition.X - width / 2 < 0
            || WorldPosition.Y + height / 2 > SurvivorGame.ScreenHeight
            || WorldPosition.Y - height / 2 < 0;

        if (WorldPosition.X + width / 2 > SurvivorGame.ScreenWidth)
        {
            x = SurvivorGame.ScreenWidth - width;
            sx *= -1;
        }
        if (WorldPosition.X - width / 2 < 0)
        {
            x = width;
            sx *= -1;
        }
        if (WorldPosition.Y + height / 2 > SurvivorGame.ScreenHeight)
        {
            y = SurvivorGame.ScreenHeight - height;
            sy *= -1;
        }
        if (WorldPosition.Y - height / 2 < 0)
        {
            y = height;
            sy *= -1;
        }

        WorldPosition = new Vector2(x, y);
        Speed = new Vector2(sx, sy);
        if (outofBounds) Speed *= bounceDecell;

        //Log.Debug("projectile {0} {1}", GetHashCode(), Speed.Length());
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawRect(
            new Vector2(WorldPosition.X - width / 2, WorldPosition.Y - height / 2),
            width, height, Color.Red);
    }
}
