using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Survivor;

namespace Hockey;

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

        // collision with screen edge
        bool outofBounds =
            WorldPosition.X + width / 2 > HockeyGame.ScreenWidth
            || WorldPosition.X - width / 2 < 0
            || WorldPosition.Y + height / 2 > HockeyGame.ScreenHeight
            || WorldPosition.Y - height / 2 < 0;

        if (WorldPosition.X + width / 2 > HockeyGame.ScreenWidth)
        {
            x = HockeyGame.ScreenWidth - width / 2;
            sx *= -1;
        }
        if (WorldPosition.X - width / 2 < 0)
        {
            x = width / 2;
            sx *= -1;
        }
        if (WorldPosition.Y + height / 2 > HockeyGame.ScreenHeight)
        {
            y = HockeyGame.ScreenHeight - height / 2;
            sy *= -1;
        }
        if (WorldPosition.Y - height / 2 < 0)
        {
            y = height / 2;
            sy *= -1;
        }

        WorldPosition = new Vector2(x, y);
        Speed = new Vector2(sx, sy);
        if (outofBounds) Speed *= bounceDecell;

        CheckGoalCollision();

        //Log.Debug("projectile {0} {1}", GetHashCode(), Speed.Length());
    }

    private void CheckGoalCollision()
    {
        Vector2 a = WorldPosition;
        int aw = width / 2; // because origin is at the center
        int ah = height / 2;
        bool collide = false;

        foreach (Base e in EntityManager.Manager.Entities)
        {
            if (e is GoalPost g)
            {
                Vector2 b = g.WorldPosition;
                int bw = g.Width / 2; // because origin is at the center
                int bh = g.Height / 2;

                collide = 
                    a.X + aw > b.X - bw
                    && a.X - aw < b.X + bw
                    && a.Y + ah > b.Y - bh
                    && a.Y - ah < b.Y + bh;

                if (collide)
                {
                    EntityManager.Manager.RemoveObject(this);
                    //Speed = Vector2.Zero;
                    break;
                }
            }
        }

    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawRect(
            new Vector2(WorldPosition.X - width / 2, WorldPosition.Y - height / 2),
            width, height, Color.Red);
    }
}
