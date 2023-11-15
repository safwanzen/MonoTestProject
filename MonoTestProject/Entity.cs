using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoTestProject;

public class Entity
{
    public World World;
    public bool IsAlive = true; // mark for removal from list
    public Vector2 WorldPosition;

    public float scaleX;
    public float scaleY;
    public Vector2 ScreenPosition { protected set; get; }

    public bool OnSlope = false;

    public Entity()
    {
        World = MainGame.World;
    }

    public virtual void Update(float deltaTime)
    {
        ScreenPosition = World.WorldToScreen(WorldPosition);
        scaleX = World.scaleX; 
        scaleY = World.scaleY;
    }
    public virtual void Draw(SpriteBatch spriteBatch) { }
}
