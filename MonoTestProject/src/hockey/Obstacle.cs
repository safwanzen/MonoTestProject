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
    public Vector2 WorldPosition;
    int tilesize = 16;

    public ObstacleType ObstacleType;
    public Rectangle Hitbox;

    public Obstacle(ContentManager contentManager, Vector2 position, ObstacleType obstacleType)
    {
        ObstacleType = obstacleType;
        WorldPosition = position;
        sprite = new Sprite(
            contentManager.Load<Texture2D>("hockey/obstacle"),
            new Rectangle(16 * (int)obstacleType, 0, tilesize, tilesize),
            Vector2.Zero);

        Hitbox = new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, tilesize, tilesize);
    }

    private void UpdateHitBoxPosition()
    {
        Hitbox.X = (int)WorldPosition.X;
        Hitbox.Y = (int)WorldPosition.Y;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var scale = HockeyGame.World.scaleX;
        sprite.Draw(spriteBatch, HockeyGame.World.WorldToScreen(WorldPosition), 0, Color.White, scaleX: scale, scaleY: scale);
    }

    public override void Update(GameTime gameTime)
    {
    }
}
