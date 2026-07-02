using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Survivor;

namespace Hockey;

public class GoalPost : Base
{
    public int Width { get; set; }
    public int Height { get; set; }
    public Vector2 WorldPosition { get; set; }

    public GoalPost()
    {
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        float scale = HockeyGame.World.scaleX;
        spriteBatch.DrawRect(
            HockeyGame.World.WorldToScreen(new Vector2(WorldPosition.X - Width / 2, WorldPosition.Y - Height / 2)), Width, Height, Color.Blue, scale, scale);
    }
}
