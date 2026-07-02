using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Survivor;

namespace Hockey;

// graphic
public enum ObstacleType
{
    Crack,
    Hole,
    Trash
}

public class Obstacle : Base
{
    Sprite sprite;
    Vector2 WorldPosition;
    float scale = 1;

    public Rectangle Hitbox;

    public Obstacle(ContentManager contentManager, Vector2 position, ObstacleType obstacleType)
    {
        int s = 16;
        WorldPosition = position;
        sprite = new Sprite(
            contentManager.Load<Texture2D>("hockey/obstacle"),
            new Rectangle(16 * (int)obstacleType, 0, s, s),
            new Vector2(s/2));

        Hitbox = new Rectangle((int)WorldPosition.X - s / 2, (int)WorldPosition.Y - s / 2, s, s);
    }

    private void UpdateHitBoxPosition()
    {
        Hitbox.X = (int)WorldPosition.X - Hitbox.Width / 2;
        Hitbox.Y = (int)WorldPosition.Y - Hitbox.Height / 2;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        sprite.Draw(spriteBatch, WorldPosition, 0, Color.White, SpriteEffects.None, scale, scale);
    }

    public override void Update(GameTime gameTime)
    {
    }
}
