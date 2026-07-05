using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using Survivor;

namespace Hockey;

public class Projectile : Base
{
    private const float decell = 0.99f;
    private const float bounceDecell = 0.7f;
    private int width = 16, height = 16;
    private int hitsize = 11;
    private int playerRadius = 10;

    public Vector2 WorldPosition { get; set; }
    public Vector2 Speed { get; set; }

    private Texture2D puckTex;
    private Sprite puck;

    public Rectangle Hitbox;
    public Rectangle PlayerHitbox;
    public bool Possessed = false;
    public Base Owner;

    public Projectile(ContentManager contentManager)
    {
        puckTex = contentManager.Load<Texture2D>("hockey/hockey_puck");
        puck = new Sprite(puckTex, new Rectangle(0, 0, width, height), new Vector2(width/2, height/2));
        Hitbox = new Rectangle((int)WorldPosition.X - hitsize / 2, (int)WorldPosition.Y - height / 2, hitsize, hitsize);
    }

    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        WorldPosition += Speed * dt;
        Speed *= decell;

        Hitbox.X = (int)WorldPosition.X - hitsize / 2;
        Hitbox.Y = (int)WorldPosition.Y - hitsize / 2;

        CheckBoundaryCollision();
        CheckGoalCollision();
        //if (!Possessed) CheckObjectCollision();
    }

    private void CheckObjectCollision()
    {
        foreach (var e in EntityManager.Manager.Entities)
        {
            
            if (e is Snowman s)
            {
                if (Hitbox.Intersects(s.Hitbox))
                {
                    Log.Debug("hit {0}", nameof(Snowman), s.GetHashCode());
                    EntityManager.Manager.RemoveObject(this);
                }
            }
        }
    }

    private void CheckEnemyCollision(Snowman s)
    {

    }

    public void SetOwner(Base owner)
    {
        Owner = owner;
        Possessed = true;
    }

    public void Shoot(Vector2 direction, float speed)
    {
        Speed = direction * speed;
    }

    private void CheckBoundaryCollision()
    {
        float x = WorldPosition.X;
        float y = WorldPosition.Y;
        float sx = Speed.X;
        float sy = Speed.Y;

        bool outofBounds =
                    WorldPosition.X + hitsize / 2 > HockeyGame.WorldWidth
                    || WorldPosition.X - hitsize / 2 < 0
                    || WorldPosition.Y + hitsize / 2 > HockeyGame.WorldHeight
                    || WorldPosition.Y - hitsize / 2 < 0;

        if (WorldPosition.X + hitsize / 2 > HockeyGame.WorldWidth)
        {
            x = HockeyGame.WorldWidth - hitsize / 2;
            sx *= -1;
        }
        if (WorldPosition.X - hitsize / 2 < 0)
        {
            x = hitsize / 2;
            sx *= -1;
        }
        if (WorldPosition.Y + hitsize / 2 > HockeyGame.WorldHeight)
        {
            y = HockeyGame.WorldHeight - hitsize / 2;
            sy *= -1;
        }
        if (WorldPosition.Y - hitsize / 2 < 0)
        {
            y = hitsize / 2;
            sy *= -1;
        }

        WorldPosition = new Vector2(x, y);
        Speed = new Vector2(sx, sy);
        if (outofBounds) Speed *= bounceDecell;
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
        var screenpos = HockeyGame.World.WorldToScreen(WorldPosition);
        var scale = HockeyGame.World.scaleX;
        puck.Draw(spriteBatch, screenpos, 0, Color.White, layerDepth: 0.1f + screenpos.Y / HockeyGame.ScreenHeight * 0.2f, scaleX: scale, scaleY: scale);
        //spriteBatch.DrawRect(HockeyGame.World.WorldToScreen(new Vector2(Hitbox.X, Hitbox.Y)), hitsize, hitsize, Color.Red, scale, scale);
    }
}
