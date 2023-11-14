using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoTestProject.UI;
using System;
using System.Collections.Generic;

namespace MonoTestProject;

public class MainGame : Game
{
    public static int ScreenWidth, ScreenHeight = 0;
    public static float GravityAcceleration = 50f;

    Texture2D ballTexture;
    Vector2 ballPosition;
    Vector2 ballSpeed;
    float ballRotation;

    UIContainer UIContainer = new();
    Button testButton;

    public static Texture2D handTexture;
    public static Texture2D particleTexture;
    public static Texture2D ParticleTrailTexture;
    public static Texture2D BulletTextureMedium;
    public static Texture2D BulletTextureLarge;
    public static Texture2D BulletTextureXLarge;
    public static Texture2D BulletSheet;
    public static Texture2D ExplosionBeginTexture;
    public Texture2D CaveStoryCharSheet;

    public static Texture2D Pixel;

    public static SoundEffect BulletHitSound;
    public static SoundEffect BulletFireSound;
    public static List<SoundEffect> Sounds = new();

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public static List<Bullet> Bullets = new();
    public static List<Enemy> Enemies = new();
    public static List<Entity> Entities = new();

    public static ButtonState PrevLMBState = ButtonState.Released;
    public static ButtonState PrevRMBState = ButtonState.Released;

    public static SpriteFont Font;

    public static World World = new();

    PlatformerCharacter character;

    VertexPositionColor[] _vertexPositionColors;
    BasicEffect _basicEffect;

    Random random = new Random();

    private float xoff = 0, yoff = 0;
    private float xscale = 1, yscale = 1;

    private float targetScale = 1;
    private float maxtargetscale = 8f;
    private float mintargetscale = 0.5f;

    private float xscreencenter, yscreencenter;
    Vector2 lastMousePosition;
    Vector2 mouseWorldPosTile;
    Vector2 mouseScreenPosTile;
    private int lastScrollWheel;
    
    private const float tileWidth = 32;
    private const float tileWidthHalf = tileWidth / 2;
    public static int worldTileWidth, worldTileHeight;
    
    public static TileType[] tiles;
    private bool focusCharacter = false;

    public MainGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        ScreenWidth = _graphics.PreferredBackBufferWidth;
        ScreenHeight = _graphics.PreferredBackBufferHeight;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        //Console.WriteLine("initialize called");
        Console.WriteLine("Welcome");
        Console.WriteLine("Controls are: \n" +
            "- [A], [D] to move left and right \n" +
            "- [W] to jump \n" +
            "- [O] to shoot. Hold [O] for one second and release for charged shot" +
            "- [T] to add block at cursor \n" +
            "- [R] to remove block at cursor \n" +
            "- [Right mouse button] to add enemy");

        base.Initialize();
        SoundEffect.MasterVolume = .0f;

        worldTileWidth = (int)(ScreenWidth / tileWidth);
        worldTileHeight = (int)(ScreenHeight / tileWidth);
        tiles = new TileType[worldTileWidth * worldTileHeight];

        InputManager.SetWindow(Window);

        testButton = new Button()
        {
            Text = "Hello Button",
            Bounds = new Rectangle(0, 50, 50, 20)
        };

        testButton.Click += (s, e) =>
        {
            Console.WriteLine("click event invoked");
        };

        UIContainer.AddElement(testButton);
    }

    protected override void LoadContent()
    {
        //Console.WriteLine("loadcontent called");

        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Pixel = new Texture2D(GraphicsDevice, 1, 1);

        // TODO: use this.Content to load your game content here
        Font = Content.Load<SpriteFont>("TextFont");
        handTexture = Content.Load<Texture2D>("hand");
        ballTexture = handTexture; //Content.Load<Texture2D>("ball");        
        BulletTextureMedium = Content.Load<Texture2D>("ms_bullet_round_medium");
        BulletTextureLarge = Content.Load<Texture2D>("ms_bullet_round_large");
        BulletTextureXLarge = Content.Load<Texture2D>("ms_bullet_round_xlarge");
        BulletSheet = Content.Load<Texture2D>("ms_bullet_16x48");
        CaveStoryCharSheet = Content.Load<Texture2D>("cave-story-wii-sprite-sheet-transparent");
        ExplosionBeginTexture = Content.Load<Texture2D>("explosion-start-32x32");

        Pixel.SetData(new Color[] { Color.White });

        BulletHitSound = Content.Load<SoundEffect>("Audio/MMX3_SE_00044");
        BulletFireSound = Content.Load<SoundEffect>("Audio/ST01_00_00002");

        Sounds.Add(Content.Load<SoundEffect>("Audio/MMX3_SE_00044"));
        Sounds.Add(Content.Load<SoundEffect>("Audio/PL00_U_00027"));
        Sounds.Add(Content.Load<SoundEffect>("Audio/PL02_U_00020"));
        Sounds.Add(Content.Load<SoundEffect>("Audio/ST01_00_00002"));
        Sounds.Add(Content.Load<SoundEffect>("Audio/ST01_00_00003"));
        Sounds.Add(Content.Load<SoundEffect>("Audio/PL01_U_00024"));
        Sounds.Add(Content.Load<SoundEffect>("Audio/PL00_U_00016"));

        var data = new Color[handTexture.Width * handTexture.Height];
        handTexture.GetData(data);
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i].A == 0) continue;
            data[i] = Color.Orange;
        }

        particleTexture = new Texture2D(GraphicsDevice, handTexture.Width, handTexture.Height);
        particleTexture.SetData(data);

        for (int i = 0; i < data.Length; i++)
        {
            if (data[i].A == 0) continue;
            data[i] = Color.AliceBlue;
        }
        ParticleTrailTexture = new Texture2D(GraphicsDevice, handTexture.Width, handTexture.Height);
        ParticleTrailTexture.SetData(data);

        // init objects
        ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
            _graphics.PreferredBackBufferHeight / 2);
        ballSpeed = new Vector2();

        int sqw = 32;
        character = new PlatformerCharacter()
        {
            WorldPosition = new Vector2(ScreenWidth / 2, ScreenHeight / 2),
            runningSprite = new AnimatedSprite(
                CaveStoryCharSheet,
                new Rectangle(0 * sqw, 1 * sqw, sqw, sqw),
                new Vector2(sqw / 2, 17),
                3,
                durations: new float[] { .12f, .05f, .12f, .05f },
                true,
                sequence: new int[] { 1, 0, 2, 0 }),
            standingSprite = new Sprite(
                CaveStoryCharSheet,
                new Rectangle(0 * sqw, 1 * sqw, sqw, sqw),
                new Vector2(sqw / 2, 17)
                ),
        };

        //enemy = new Enemy(new Vector2(500, 200));
        //Entities.Add(enemy);
        _vertexPositionColors = new[]
        {
            new VertexPositionColor(new Vector3(0, 0, 1), Color.White),
            new VertexPositionColor(new Vector3(10, 0, 1), Color.White),
            new VertexPositionColor(new Vector3(10, 10, 1), Color.White),
            new VertexPositionColor(new Vector3(0, 10, 1), Color.White)
        };
        _basicEffect = new BasicEffect(GraphicsDevice);
        _basicEffect.World = Matrix.CreateOrthographicOffCenter(
            0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, 1);
    }

    protected override void Update(GameTime gameTime)
    {
        InputManager.BeginFrame();
        //Console.WriteLine("update \t {0}", gameTime.TotalGameTime);
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        var mousestate = Mouse.GetState(Window);

        //xoff = World.ScreenToWorld(character.ScreenPosition.X - ScreenWidth / 2, 0).X;

        if (InputManager.IsPressed(Keys.Q))
        {
            focusCharacter = !focusCharacter;
        }

        //if (PrevRMBState == ButtonState.Released && mousestate.RightButton == ButtonState.Pressed)
        if (InputManager.IsPressed(MouseButtons.RightButton))
        {
            //for (int i = 0; i < 5000; i++) Enemies.Add(new Enemy(mousestate.Position.ToVector2()));
            var e = new Enemy(World.ScreenToWorld(mousestate.Position.ToVector2()));
            Console.WriteLine("Enemy added {0}", e.GetHashCode());
            Enemies.Add(e);
        }

        var currmouseposition = mousestate.Position.ToVector2();

        // drag to change world offset
        if (!focusCharacter
            && InputManager.IsDown(MouseButtons.LeftButton)
            && currmouseposition.X < ScreenWidth && currmouseposition.Y < ScreenHeight
            && currmouseposition.X > 0 && currmouseposition.Y > 0)
        {
            var diff = currmouseposition - lastMousePosition;
            xoff += diff.X / xscale;
            yoff += diff.Y / yscale;
            World.SetOffset(xoff, yoff);
        }

        var oldmouseworldcoord = World.ScreenToWorld(currmouseposition);

        if (lastScrollWheel - mousestate.ScrollWheelValue > 0)
        {
            targetScale *= 2f;
            if (targetScale > maxtargetscale) targetScale = maxtargetscale;
            Console.WriteLine("target scale: {0}", targetScale);
        }

        if (lastScrollWheel - mousestate.ScrollWheelValue < 0)
        {
            targetScale *= .5f;
            if (targetScale < mintargetscale) targetScale = mintargetscale;
            Console.WriteLine("target scale: {0}", targetScale);
        }

        if (targetScale != xscale)
        {
            float scalediff = targetScale - xscale;
            //float scalediffabs = Math.Abs(scalediff);
            if (scalediff > .001f || scalediff < -.001f)
            {
                //Console.WriteLine(scalediff);
                xscale += scalediff / 4;
                yscale = xscale;
                World.scaleX = xscale;
                World.scaleY = yscale;
            }
            else
            {
                xscale = yscale = targetScale;
            }
        }
        
        lastMousePosition = mousestate.Position.ToVector2();
        var mousewpos = World.ScreenToWorld(lastMousePosition);
        WorldToTile(mousewpos, (int)tileWidth, out int x, out int y);
        mouseWorldPosTile.X = x;
        mouseWorldPosTile.Y = y;
        mouseScreenPosTile = World.WorldToScreen(mouseWorldPosTile * tileWidth);
        lastScrollWheel = mousestate.ScrollWheelValue;

        // add wall tile
        if (InputManager.IsDown(Keys.T))
        {
            if (x >= 0 && x < worldTileWidth && y >= 0 && y < worldTileHeight)
            {
                var tileindex = y * worldTileWidth + x;
                //if (tiles[tileindex] == TileType.None)
                tiles[tileindex] = TileType.Wall;
            }
        }
        // add left slope tile
        if (InputManager.IsDown(Keys.F))
        {
            if (x >= 0 && x < worldTileWidth && y >= 0 && y < worldTileHeight)
            {
                var tileindex = y * worldTileWidth + x;
                //if (tiles[tileindex] == TileType.None)
                tiles[tileindex] = TileType.SlopeL;
            }
        }
        // add right slope tile
        if (InputManager.IsDown(Keys.G))
        {
            if (x >= 0 && x < worldTileWidth && y >= 0 && y < worldTileHeight)
            {
                var tileindex = y * worldTileWidth + x;
                //if (tiles[tileindex] == TileType.None)
                tiles[tileindex] = TileType.SlopeR;
            }
        }
        // remove tile
        if (InputManager.IsDown(Keys.R))
        {
            if (x >= 0 && x < worldTileWidth && y >= 0 && y < worldTileHeight)
            {
                var tileindex = y * worldTileWidth + x;
                if (tiles[tileindex] != TileType.None)
                    tiles[tileindex] = TileType.None;
            }
        }

        if (!focusCharacter)
        {
            // drag to change world offset, also makes zooming centered to cursor
            var newmouseworldcoord = World.ScreenToWorld(currmouseposition);
            var mousediff = newmouseworldcoord - oldmouseworldcoord;
            xoff += mousediff.X;
            yoff += mousediff.Y;
            World.SetOffset(xoff, yoff);
        }
        else
        {
            // make camera follow character but add limit
            var wsw = ScreenWidth / xscale;
            var wsh = ScreenHeight / yscale;
            xoff = -character.WorldPosition.X + wsw / 2;
            yoff = -character.WorldPosition.Y + wsh / 2;
            if (xoff > 0) xoff = 0;
            else if (xoff < wsw - ScreenWidth) xoff = wsw - ScreenWidth;
            if (yoff < wsh - ScreenHeight) yoff = wsh - ScreenHeight;
            else if (yoff > 0) yoff = 0;
            World.SetOffset(xoff, yoff);
        }

        character.Update(deltaTime);

        foreach (var enemy in Enemies)
        {
            enemy.Update(deltaTime);
        }

        for (int i = 0; i < Entities.Count;)
        {
            if (!Entities[i].IsAlive) Entities.RemoveAt(i);
            else
            {
                Entities[i].Update(deltaTime);
                i++;
            }
        }

        for (int i = 0; i < Bullets.Count;)
        {
            Bullets[i].Update(deltaTime);
            if (Bullets[i].WasHit)
            {
                Bullets.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }

        UIContainer.Update(gameTime);

        InputManager.EndFrame();
        base.Update(gameTime);
    }


    protected override void Draw(GameTime gameTime)
    {
        Vector2[] triangle = { new(60, 60), new(60, 120), new(0, 60) };
        //Console.WriteLine("draw \t {0}", gameTime.TotalGameTime);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

        for (int y = 0; y < worldTileHeight; y++)
        {
            for (int x = 0;  x < worldTileWidth; x++)
            {
                var tileindex = (y * worldTileWidth) + x;
                var tile = tiles[tileindex];
                if (tile == TileType.Wall)
                    _spriteBatch.DrawRect(World.WorldToScreen(x * tileWidth, y * tileWidth), (int)(32 * xscale), (int)(32 * yscale), Color.Red * 0.5f);
                if (tile == TileType.SlopeR)
                    _spriteBatch.DrawPolygonWireFrame(World.WorldToScreen(x * tileWidth, y * tileWidth),
                        new Vector2[] { new(tileWidth, 0), new(tileWidth, tileWidth), new(0, tileWidth) },
                        Color.Red * 0.5f,
                        xscale, yscale);
                if (tile == TileType.SlopeL)
                    _spriteBatch.DrawPolygonWireFrame(World.WorldToScreen(x * tileWidth, y * tileWidth),
                        new Vector2[] { new(0, 0), new(tileWidth, tileWidth), new(0, tileWidth) },
                        Color.Red * 0.5f,
                        xscale, yscale);
                else continue;
            }
        }

        character.Draw(_spriteBatch);

        foreach (var entity in Enemies)
        {
            entity.Draw(_spriteBatch);
        }

        foreach (var trail in Entities)
        {
            trail.Draw(_spriteBatch);
        }

        foreach (var particle in Bullets)
        {
            particle.Draw(_spriteBatch);
        }


        //_spriteBatch.Draw(ballTexture, ballPosition, null, Color.White, ballRotation + MathHelper.PiOver2,
        //    new Vector2(16, 22), Vector2.One, SpriteEffects.None, 0f);

        // draw triangle
        _spriteBatch.DrawPolygonWireFrame(mouseScreenPosTile, triangle, Color.White, xscale, yscale);

        _spriteBatch.DrawLine((int)character.ScreenPosition.X, (int)character.ScreenPosition.Y, (int)(mouseScreenPosTile.X + tileWidthHalf * xscale), (int)(mouseScreenPosTile.Y + tileWidthHalf * xscale), Color.White);
        _spriteBatch.DrawRectWireframe(mouseScreenPosTile, 32, 32, Color.White, xscale, yscale);
        _spriteBatch.DrawRectWireframe(World.WorldToScreen(0, 0), ScreenWidth, ScreenHeight, Color.White, xscale, yscale);
        _spriteBatch.DrawString(Font, "(0, 0)", World.WorldToScreen(0, 0), Color.White, 0f, Vector2.Zero, new Vector2(xscale, yscale), SpriteEffects.None, 0);

        //_spriteBatch.DrawString(Font, $"Bullets: {Bullets.Count}", new Vector2(10, 10), Color.Black);
        _spriteBatch.DrawString(Font, string.Format("Camera mode: {0}", focusCharacter ? "Character" : "Click and drag"), new Vector2(10, 10), Color.Black);
        _spriteBatch.DrawString(Font, $"Enemies: {Enemies.Count}", new Vector2(10, 30), Color.Black);
        _spriteBatch.DrawString(Font, $"Player wpx: {character.WorldPosition.X}", new Vector2(10, 50), Color.Black);
        _spriteBatch.DrawString(Font, $"Player wpy: {character.WorldPosition.Y}", new Vector2(10, 70), Color.Black);
        //_spriteBatch.DrawString(Font, $"Speed magnitude: {character.speedMagnitude}", new Vector2(10, 90), Color.Black);
        //_spriteBatch.DrawString(Font, $"Direction: {character.direction}", new Vector2(10, 110), Color.Black);
        _spriteBatch.DrawString(Font, $"MouseXY: {lastMousePosition.X} {lastMousePosition.Y}", new Vector2(10, 90), Color.Black);
        //_spriteBatch.DrawString(Font, $"MouseXY: {World.ScreenToWorld)}", new Vector2(10, 110), Color.Black);
        //_spriteBatch.DrawString(Font, $"world offset XY: {xoff} {yoff}", new Vector2(10, 130), Color.Black);
        _spriteBatch.DrawString(Font, $"mouse world pos: {mouseWorldPosTile.X} {mouseWorldPosTile.Y}", new Vector2(10, 130), Color.Black);

        UIContainer.Draw(_spriteBatch);

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    public static void WorldToTile(Vector2 worldpos, int tilewidth, out int col, out int row)
    {
        col = (int)Math.Floor(worldpos.X / tilewidth);
        row = (int)Math.Floor(worldpos.Y / tilewidth);
    }
}