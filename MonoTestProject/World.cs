using Microsoft.Xna.Framework;

namespace MonoTestProject;

public class World
{
    public float offsetX;
    public float offsetY;

    public float scaleX = 1, scaleY = 1;

    public void SetOffset(Vector2 pos) => SetOffset(pos.X, pos.Y);
    public void SetOffset(float x, float y)
    {
        offsetX = x;
        offsetY = y;
    }

    public Vector2 GetOffset()
    {
        return new Vector2 (offsetX, offsetY);
    }

    public Vector2 WorldToScreen(Vector2 worldCoord) => WorldToScreen(worldCoord.X, worldCoord.Y);
    public Vector2 WorldToScreen(float x, float y)
    {
        var sx = (x + offsetX) * scaleX;
        var sy = (y + offsetY) * scaleY;
        return new Vector2(sx, sy);
    }

    public Vector2 ScreenToWorld(Vector2 screenCoord) => ScreenToWorld(screenCoord.X, screenCoord.Y);
    public Vector2 ScreenToWorld(float x, float y)
    {
        var wx = x / scaleX - offsetX;
        var wy = y / scaleY - offsetY;
        return new Vector2 (wx, wy);
    }
}
