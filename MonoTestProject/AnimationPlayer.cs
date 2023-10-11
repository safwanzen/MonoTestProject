using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoTestProject;

public class AnimationPlayer
{
    private Sprite sprite;

    public void Update(float deltaTime)
    {
        sprite.Update(deltaTime);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation, Color color, SpriteEffects spriteEffects, float scaleX, float scaleY)
    {
        sprite.Draw(spriteBatch, position, rotation, color, spriteEffects, scaleX, scaleY);
    }
}
