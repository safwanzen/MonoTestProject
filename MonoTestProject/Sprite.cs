﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTestProject;

public class Sprite
{
    protected Texture2D texture;
    protected Vector2 origin;
    
    protected Rectangle sourceRect;

    public Sprite(Texture2D texture, Rectangle sourceRect, Vector2 origin)
    {
        this.texture = texture;
        this.sourceRect = sourceRect;
        this.origin = origin;
    }

    public virtual void Update(float deltaTime)
    {

    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation, Color color)
    {
        spriteBatch.Draw(texture, position, sourceRect, color, rotation,
            origin, Vector2.One, SpriteEffects.None, 0);
    }
}
