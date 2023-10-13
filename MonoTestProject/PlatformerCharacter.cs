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
    private int sizex = 5;
    private int sizey = 15;

    private float gravity = 1500f;
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
    private Vector2 newWorldPosition;
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

        if (InputManager.IsPressed(ABtn) && isOnGround && !isJumping)
        {
            Console.WriteLine("jumping");
            isOnGround = false;
            isJumping = true;
            speed.Y = -500f; // -jumpVelocity
        }

        if (InputManager.IsReleased(ABtn) && isJumping)
        {
            Console.WriteLine("jump released");
            isJumping = false;
            if (speed.Y < 0) speed.Y *= 0.3f;
        }
        #endregion

        #region movement

        speed.Y += 1500f * deltaTime;

        if (isOnGround)
        {
            isJumping = false;
            //speed.Y = 0;
        }
        else
        {
        }

        newWorldPosition = WorldPosition + speed * deltaTime;

        #endregion

        CheckMapCollision();

        running = Math.Abs(speed.X) > 0 && isOnGround;
        sprite = running ? runningSprite : standingSprite;

        WorldPosition = newWorldPosition;
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
        spriteBatch.DrawRect((int)World.WorldToScreen(WorldPosition.X - sizex, 0).X, (int)World.WorldToScreen(0, WorldPosition.Y - sizey).Y, (int)(World.scaleX * sizex * 2), (int)(World.scaleY * sizey * 2), debugColor);
        //spriteBatch.DrawRect(World.WorldToScreen(new Vector2(tilex * 32, tiley * 32)), (int)(World.scaleX * 32), (int)(World.scaleY * 32), debugColor);

        //spriteBatch.DrawRect(World.WorldToScreen(new Vector2(PlayerRect.X, PlayerRect.Y)), (int)(sizex * World.scaleX * 2), (int)(sizey * World.scaleY * 2), debugColor);
        //spriteBatch.DrawRect(World.WorldToScreen(new Vector2((tilex - 1) * 32, tiley * 32)), (int)(World.scaleX * 32), (int)(World.scaleY * 32), collideLeft ? debugColorHit : debugColor); // left
        //spriteBatch.DrawRect(World.WorldToScreen(new Vector2((tilex + 1) * 32, tiley * 32)), (int)(World.scaleX * 32), (int)(World.scaleY * 32), collideRight ? debugColorHit : debugColor); // right
        //spriteBatch.DrawRect(World.WorldToScreen(new Vector2(tilex * 32, (tiley - 1) * 32)), (int)(World.scaleX * 32), (int)(World.scaleY * 32), collideTop ? debugColorHit : debugColor); // top
        //spriteBatch.DrawRect(World.WorldToScreen(new Vector2(tilex * 32, (tiley + 1) * 32)), (int)(World.scaleX * 32), (int)(World.scaleY * 32), collideBottom ? debugColorHit : debugColor); // bottom

        //spriteBatch.DrawRectWireframe(World.WorldToScreen(new Vector2((tilex - 1) * 32, (tiley - 1) * 32)), (int)(World.scaleX * 32), (int)(World.scaleY * 32), debugColor);
        //spriteBatch.DrawRectWireframe(World.WorldToScreen(new Vector2((tilex - 1) * 32, (tiley + 1) * 32)), (int)(World.scaleX * 32), (int)(World.scaleY * 32), debugColor);
        //spriteBatch.DrawRectWireframe(World.WorldToScreen(new Vector2((tilex + 1) * 32, (tiley - 1) * 32)), (int)(World.scaleX * 32), (int)(World.scaleY * 32), debugColor);
        //spriteBatch.DrawRectWireframe(World.WorldToScreen(new Vector2((tilex + 1) * 32, (tiley + 1) * 32)), (int)(World.scaleX * 32), (int)(World.scaleY * 32), debugColor);

        base.Draw(spriteBatch);
    }

    private void CheckMapCollision()
    {
        PlayerRect = new Rectangle((int)WorldPosition.X - 8, (int)WorldPosition.Y - 14, sizex * 2, sizey * 2);

        isOnGround = false;
        
        if (speed.X <= 0)
        {
            if (GetTile(newWorldPosition.X - sizex, WorldPosition.Y - sizey * .9f) == TileType.Wall
                || GetTile(newWorldPosition.X - sizex, WorldPosition.Y + sizey * .9f) == TileType.Wall)
            {
                Console.WriteLine("hit left");
                newWorldPosition.X = (int)(newWorldPosition.X / 32) * 32 + sizex;
            }
        }
        else
        {
            if (GetTile(newWorldPosition.X + sizex, WorldPosition.Y - sizey * .9f) == TileType.Wall
                || GetTile(newWorldPosition.X + sizex, WorldPosition.Y + sizey * .9f) == TileType.Wall)
            {
                Console.WriteLine("hit right");
                newWorldPosition.X = (int)(newWorldPosition.X / 32 + 1) * 32 - sizex;
            }
        }
        
        if (speed.Y <= 0)
        {
            if (GetTile(newWorldPosition.X + sizex - 1, newWorldPosition.Y - sizey) == TileType.Wall
                || GetTile(newWorldPosition.X - sizex, newWorldPosition.Y - sizey) == TileType.Wall)
            {
                Console.WriteLine("hit top");
                newWorldPosition.Y = (int)(newWorldPosition.Y / 32) * 32 + sizey;
                speed.Y = 0;
            }
        }
        else
        {
            if (GetTile(newWorldPosition.X + sizex - 1, newWorldPosition.Y + sizey) == TileType.Wall
                || GetTile(newWorldPosition.X - sizex, newWorldPosition.Y + sizey) == TileType.Wall)
            {
                Console.WriteLine("hit bottom");
                newWorldPosition.Y = (int)(newWorldPosition.Y / 32 + 1) * 32 - sizey;
                speed.Y = 0;
                isOnGround = true;
            }

            // map lower bound
            if (newWorldPosition.Y + sizey > MainGame.ScreenHeight)
            {
                newWorldPosition.Y = MainGame.ScreenHeight - sizey;
                speed.Y = 0;
                isOnGround = true;
            }
        }

        // if player is completely inside a wall, push to top
        if (   GetTile(newWorldPosition.X - sizex + .9f, newWorldPosition.Y - sizey + .9f) == TileType.Wall
            && GetTile(newWorldPosition.X - sizex + .9f, newWorldPosition.Y + sizey - .9f) == TileType.Wall
            && GetTile(newWorldPosition.X + sizex - .9f, newWorldPosition.Y - sizey + .9f) == TileType.Wall
            && GetTile(newWorldPosition.X + sizex - .9f, newWorldPosition.Y + sizey - .9f) == TileType.Wall)
        {
            Console.WriteLine("player inside wall");
            newWorldPosition.Y = (int)(newWorldPosition.Y / 32) * 32 - sizey;
        }
    }


    private TileType GetTile(float x, float y)
    {
        int index = (int)(y / 32) * MainGame.worldTileWidth + (int)(x / 32);
        if (index >= 0 && index < MainGame.tiles.Length)
            return MainGame.tiles[index];
        return TileType.None;
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
}
