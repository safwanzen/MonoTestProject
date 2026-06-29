using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Survivor.UI;

public interface IUIElement
{
    public void Update(GameTime gameTime);
    public void Draw(SpriteBatch spriteBatch);
}
