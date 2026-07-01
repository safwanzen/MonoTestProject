using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Survivor;

namespace Hockey;

public class GoalPost : Base
{
    public int Width;
    public int Height;
    public Vector2 WorldPosition = Vector2.Zero;

    public GoalPost()
    {
        WorldPosition = new Vector2(HockeyGame.ScreenWidth, HockeyGame.ScreenHeight / 2);
        Width = 60;
        Height = 200;
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawRect(new Vector2(WorldPosition.X - Width / 2, WorldPosition.Y - Height / 2), Width, Height, Color.Blue);
    }
}
