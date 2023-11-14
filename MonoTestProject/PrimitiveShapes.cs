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

    public static void DrawRect(this SpriteBatch spriteBatch, Rectangle rect, Color color)
        => DrawRect(spriteBatch, rect.X, rect.Y, rect.Width, rect.Height, color);


    public static void DrawRect(this SpriteBatch spriteBatch, Vector2 pos, int width, int height, Color color, float scalex = 1, float scaley = 1)
        => DrawRect(spriteBatch, (int)pos.X, (int)pos.Y, width, height, color, scalex, scaley);

    public static void DrawRect(this SpriteBatch spriteBatch, int x, int y, int width, int height, Color color, float scalex = 1, float scaley = 1)
    {
        var pixel = GetTexture(spriteBatch);
        spriteBatch.Draw(pixel, new Rectangle(x, y, (int)(width * scalex), (int)(height * scaley)), color);
    }

    public static void DrawRectWireframe(this SpriteBatch spriteBatch, Vector2 pos, int width, int height, Color color, float scalex = 1, float scaley = 1, int linethickness = 1)
        => DrawRectWireframe(spriteBatch, (int)pos.X, (int)pos.Y, width, height, color, scalex, scaley, linethickness);

    public static void DrawRectWireframe(this SpriteBatch spriteBatch, int x, int y, int width, int height, Color color, float scalex = 1, float scaley = 1, int linethickness = 1)
    {
        var pixel = GetTexture(spriteBatch);
        spriteBatch.DrawLineP(pixel, x, y, (int)(x + width * scalex), y, color);
        spriteBatch.DrawLineP(pixel, (int)(x + width * scalex), y, (int)(x + width * scalex), (int)(y + height * scaley), color);
        spriteBatch.DrawLineP(pixel, (int)(x + width * scalex), (int)(y + height * scaley), x, (int)(y + height * scaley), color);
        spriteBatch.DrawLineP(pixel, x, (int)(y + height * scaley), x, y, color);
    }

    public static void DrawLine(this SpriteBatch spriteBatch, Vector2 from, Vector2 to, Color color)
        => DrawLine(spriteBatch, (int)from.X, (int)from.Y, (int)to.X, (int)to.Y, color);
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

    public static void DrawPolygonWireFrame(this SpriteBatch spriteBatch, Vector2 position, Vector2[] points, Color color, float scalex = 1, float scaley = 1)
    {
        if (points == null && points.Length <= 0) return;
        int limit = points.Length;
        for (int i = 0; i < points.Length; i++)
        {
            var p1 = new Vector2(points[i].X * scalex, points[i].Y * scaley) + position;
            var p2 = new Vector2(points[(i+1)%limit].X * scalex, points[(i+1)%limit].Y * scaley) + position;
            spriteBatch.DrawLine(p1, p2, color);
        }
    }
}
