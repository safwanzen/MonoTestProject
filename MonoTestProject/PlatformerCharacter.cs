using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTestProject;

public class PlatformerCharacter : Entity
{
    public AnimatedSprite runningSprite;
    public Sprite standingSprite;
    private AnimationPlayer animationPlayer;
    private Sprite sprite;

    private Vector2 size = new(8, 15);
    private int sizex = 8;
    private int sizey = 15;

    private float gravity = 30f;
    private Vector2 speed;

    private bool facingRight = true;
    private bool running = false;

    private float shootTimer = 0.05f;

    private Weapon weapon = new();
    private bool isOnGround = false;
    private bool isJumping = false;

    private float jumpVelocity = 50f;

    private int tilex, tiley;

    #region controls
    private const Keys dpadUp = Keys.W;
    private const Keys dpadDown = Keys.S;
    private const Keys dpadLeft = Keys.A;
    private const Keys dpadRight = Keys.D;
    private const Keys ABtn = Keys.I;
    private const Keys BBtn = Keys.O;
    private const Keys LBtn = Keys.P;
    private const Keys RBtn = Keys.OemQuotes;
    #endregion

    #region collisions
    bool collideTop;
    bool collideBottom;
    bool collideLeft;
    bool collideRight;
    #endregion

    public PlatformerCharacter() 
    {
    }

    public override void Update(float deltaTime)
    {
        #region handle input
        if (InputManager.IsDown(dpadRight))
        {
            speed.X = 180;
            facingRight = true;
        }
        else if (InputManager.IsDown(dpadLeft))
        {
            speed.X = -180;
            facingRight = false;
        }
        else
        {
            speed.X = 0;
            runningSprite.ResetAnimation();
        }

        if (InputManager.IsPressed(BBtn))
        {
            var dir = new Vector2(facingRight ? 1 : -1, 0);
            weapon.Attack(WorldPosition + dir * 8, dir, 0);
        }

        if (InputManager.IsReleased(BBtn))
        {
            var dir = new Vector2(facingRight ? 1 : -1, 0);
            weapon.ReleaseCharge(WorldPosition + dir * 8, dir, 0);
            weapon.Charge(false);
        }

        if (InputManager.IsDown(BBtn))
        {
            weapon.Charge(true);
        }

        if (InputManager.IsPressed(dpadUp) && isOnGround && !isJumping)
        {
            Console.WriteLine("jumping");
            isOnGround = false;
            isJumping = true;
            speed.Y = -500f; // -jumpVelocity
        }

        if (InputManager.IsReleased(dpadUp) && isJumping)
        {
            Console.WriteLine("jump released");
            isJumping = false;
            if (speed.Y < 0) speed.Y *= 0.3f;
        }
        #endregion

        #region movement

        WorldPosition += speed * deltaTime;

        if (isOnGround)
        {
            isJumping = false;
            speed.Y = 0;
        }
        else
        {
            speed.Y += /*gravity*/ 1500f * deltaTime;
        }
        #endregion

        #region collision
        CheckMapCollision();
        #endregion

        running = Math.Abs(speed.X) > 0 && isOnGround;
        sprite = running ? runningSprite : standingSprite;

        MainGame.WorldToTile(WorldPosition, 32, out tilex, out tiley);

        sprite.Update(deltaTime);
        //animationPlayer.Update(deltaTime);
        weapon.Update(deltaTime);
        base.Update(deltaTime);
    }

    private Color debugColor = Color.Cyan * .5f;
    private Color debugColorHit = Color.Yellow;
    private Rectangle PlayerRect;

    public override void Draw(SpriteBatch spriteBatch)
    {
        sprite.Draw(spriteBatch, ScreenPosition, 0, Color.White, facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally, scaleX, scaleY);
        //spriteBatch.DrawRect((int)World.WorldToScreen(WorldPosition.X - 8, 0).X, (int)World.WorldToScreen(0, WorldPosition.Y - 14).Y, (int)(World.scaleX * 16), (int)(World.scaleY * 30), Color.Cyan);
        spriteBatch.DrawRect(World.WorldToScreen(new Vector2(PlayerRect.X, PlayerRect.Y)), (int)(sizex * World.scaleX * 2), (int)(sizey * World.scaleY * 2), debugColor);
        //spriteBatch.DrawRect(World.WorldToScreen(new Vector2(tilex * 32, tiley * 32)), (int)(World.scaleX * 32), (int)(World.scaleY * 32), debugColor);
        
        spriteBatch.DrawRect(World.WorldToScreen(new Vector2((tilex - 1) * 32, tiley * 32)), (int)(World.scaleX * 32), (int)(World.scaleY * 32), collideLeft ? debugColorHit : debugColor); // left
        spriteBatch.DrawRect(World.WorldToScreen(new Vector2((tilex + 1) * 32, tiley * 32)), (int)(World.scaleX * 32), (int)(World.scaleY * 32), collideRight ? debugColorHit : debugColor); // right
        spriteBatch.DrawRect(World.WorldToScreen(new Vector2(tilex * 32, (tiley - 1) * 32)), (int)(World.scaleX * 32), (int)(World.scaleY * 32), collideTop ? debugColorHit : debugColor); // top
        spriteBatch.DrawRect(World.WorldToScreen(new Vector2(tilex * 32, (tiley + 1) * 32)), (int)(World.scaleX * 32), (int)(World.scaleY * 32), collideBottom ? debugColorHit : debugColor); // bottom
        
        spriteBatch.DrawRectWireframe(World.WorldToScreen(new Vector2((tilex - 1) * 32, (tiley - 1) * 32)), (int)(World.scaleX * 32), (int)(World.scaleY * 32), debugColor);
        spriteBatch.DrawRectWireframe(World.WorldToScreen(new Vector2((tilex - 1) * 32, (tiley + 1) * 32)), (int)(World.scaleX * 32), (int)(World.scaleY * 32), debugColor);
        spriteBatch.DrawRectWireframe(World.WorldToScreen(new Vector2((tilex + 1) * 32, (tiley - 1) * 32)), (int)(World.scaleX * 32), (int)(World.scaleY * 32), debugColor);
        spriteBatch.DrawRectWireframe(World.WorldToScreen(new Vector2((tilex + 1) * 32, (tiley + 1) * 32)), (int)(World.scaleX * 32), (int)(World.scaleY * 32), debugColor);

        base.Draw(spriteBatch);
    }

    private void CheckMapCollision()
    {
        // tile collision

        PlayerRect = new Rectangle((int)WorldPosition.X - 8, (int)WorldPosition.Y - 14, sizex * 2, sizey * 2);

        // 0, 0
        var idx = GetTileIndex(1, 0);
        if (idx != -1)
        {
            var tilerect = new Rectangle((tilex + 1) * 32, tiley * 32, 32, 32);
            collideRight = PlayerRect.Right >= tilerect.Left && MainGame.tiles[idx] == TileType.Wall;
            if (collideRight)
            {
                WorldPosition.X -= PlayerRect.Right - tilerect.Left;
            }
        }

        idx = GetTileIndex(-1, 0);
        if (idx != -1)
        {
            var tilerect = new Rectangle((tilex - 1) * 32, tiley * 32, 32, 32);
            collideLeft = tilerect.Right >= PlayerRect.Left && MainGame.tiles[idx] == TileType.Wall;
            if (collideLeft)
            {
                WorldPosition.X -= PlayerRect.Left - tilerect.Right;
            }
        }

        idx = GetTileIndex(0, 1);
        if (idx != -1)
        {
            var tilerect = new Rectangle(tilex * 32, (tiley + 1) * 32, 32, 32);
            collideBottom = tilerect.Top <= PlayerRect.Bottom && MainGame.tiles[idx] == TileType.Wall;
            if (collideBottom)
            {
                WorldPosition.Y -= PlayerRect.Bottom - tilerect.Top;
                speed.Y = 0;
            }
        }

        isOnGround = collideBottom;

        // check for boundary collision
        if (PlayerRect.Left < 0)
        {
            WorldPosition.X += -PlayerRect.Left;
            speed.X = 0;
        }
        if (WorldPosition.X <= size.X / 2)
        {
            WorldPosition.X = size.X / 2;
            speed.X = 0;
        }
        if (PlayerRect.Bottom >= MainGame.ScreenHeight)
        {
            WorldPosition.Y -= PlayerRect.Bottom - MainGame.ScreenHeight;
            speed.Y = 0;
            isOnGround = true;
        }
        if (WorldPosition.Y <= size.Y / 2)
        {
            WorldPosition.Y = size.Y / 2;
            speed.Y = 0;
        }

        //for (int x = -1; x <= 1; x++)
        //{
        //    for (int y = -1; y <= 1; y++)
        //    {
        //        //if (Math.Abs(x) + Math.Abs(y) > 1) continue;
        //        var tileindex = (tiley + y) * MainGame.worldTileWidth + tilex + x;
        //        if (tileindex >= 0 && tileindex < MainGame.tiles.Length)
        //        {
        //            if (MainGame.tiles[tileindex] == TileType.Wall)
        //            {
        //                var intersects = CheckTileIntersect(x, y, playerRect);
        //                //if (intersects) break;
        //            }
        //        }
        //    }
        //}
    }

    private int GetTileIndex(int x, int y)
    {
        var idx = (tiley + y) * MainGame.worldTileWidth + tilex + x;
        if (idx >= 0 && idx < MainGame.tiles.Length) return idx;
        return -1;
    }

    private Rectangle GetTileRect(int x, int y)
    {
        return new Rectangle((tilex + x) * 32, (tiley + y) * 32, 32, 32);
    }

    private bool CheckTileIntersect(int x, int y, Rectangle playerRect)
    {
        var tilerect = GetTileRect(x, y);
        var intersects = playerRect.Intersects(tilerect);

        // check right
        collideRight = y == 0 && x == 1 && playerRect.Right >= tilerect.Left;

        // check left
        collideLeft = y == 0 && x == -1 && playerRect.Left <= tilerect.Right;
        
        // check bottom
        collideBottom = x == 0 && y == 1 && playerRect.Bottom >= tilerect.Top;

        // check top
        collideTop = x == 0 && y == -1 && playerRect.Top <= tilerect.Bottom;

        // check diagonals

        return false;
    }
}
