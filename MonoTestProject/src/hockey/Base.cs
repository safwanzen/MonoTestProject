using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hockey;

public abstract class Base
{
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SpriteBatch spriteBatch);
}
