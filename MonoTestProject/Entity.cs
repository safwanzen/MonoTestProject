using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoTestProject;

public class Entity
{
    public bool IsAlive = true; // mark for removal from list
    public Vector2 Position;
    public virtual void Update(float deltaTime) { }
    public virtual void Draw(SpriteBatch spriteBatch) { }
}
