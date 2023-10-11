using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoTestProject;

public static class SpriteBatchShapeExtension
{
    private static Texture2D _whitePixelTexture;

    private static Texture2D GetTexture(SpriteBatch spriteBatch)
    {
        if (_whitePixelTexture == null)
        {
            _whitePixelTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _whitePixelTexture.SetData(new[] { Color.White });
        }

        return _whitePixelTexture;
    }

    public static void DrawRect(this SpriteBatch spriteBatch, Vector2 pos, int width, int height, Color color)
        => DrawRect(spriteBatch, (int)pos.X, (int)pos.Y, width, height, color);

    public static void DrawRect(this SpriteBatch spriteBatch, int x, int y, int width, int height, Color color)
    {
        var pixel = GetTexture(spriteBatch);
        spriteBatch.Draw(pixel, new Rectangle(x, y, width, height), color);
    }

    public static void DrawRectWireframe(this SpriteBatch spriteBatch, Vector2 pos, int width, int height, Color color, int linethickness = 1)
        => DrawRectWireframe(spriteBatch, (int)pos.X, (int)pos.Y, width, height, color, linethickness);

    public static void DrawRectWireframe(this SpriteBatch spriteBatch, int x, int y, int width, int height, Color color, int linethickness = 1)
    {
        var pixel = GetTexture(spriteBatch);
        spriteBatch.DrawLineP(pixel, x, y, x + width, y, color);
        spriteBatch.DrawLineP(pixel, x + width, y, x + width, y + height, color);
        spriteBatch.DrawLineP(pixel, x + width, y + height, x, y + height, color);
        spriteBatch.DrawLineP(pixel, x, y + height, x, y, color);
    }

    public static void DrawLine(this SpriteBatch spriteBatch, /*Texture2D pixel,*/ int xFrom, int yFrom, int xTo, int yTo, Color color)
    {
        var pixel = GetTexture(spriteBatch);
        var distx = xTo - xFrom;
        var disty = yTo - yFrom;
        float angle = (float)Math.Atan2(disty, distx);
        int length = (int)Math.Sqrt(distx * distx + disty * disty);
        spriteBatch.Draw(pixel, new Rectangle(xFrom, yFrom, length, 1), null, color, angle, Vector2.Zero, SpriteEffects.None, 0f);
    }

    public static void DrawLineP(this SpriteBatch spriteBatch, Texture2D pixel, int xFrom, int yFrom, int xTo, int yTo, Color color)
    {
        var distx = xTo - xFrom;
        var disty = yTo - yFrom;
        float angle = (float)Math.Atan2(disty, distx);
        int length = (int)Math.Sqrt(distx * distx + disty * disty);
        spriteBatch.Draw(pixel, new Rectangle(xFrom, yFrom, length, 1), null, color, angle, Vector2.Zero, SpriteEffects.None, 0f);
    }
}
