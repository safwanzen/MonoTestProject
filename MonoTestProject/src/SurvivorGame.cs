using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using Survivor;
using System;

namespace Hockey;

public class HockeyGame : Game
{
    // since the sprites are tiny (16px * 16 px) we need to scale the world bigger
    // need world coordinate to screen coordinate conversion (i did 3 years ago)

    // tile based map 256 x 224 px (NES resolution) 
    // for 16x16px = 16x14 tiles

    public static World World;
    public static int ScreenWidth, ScreenHeight = 0;
    public static int WorldWidth, WorldHeight;
    private const int TileSize = 16;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public HockeyGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferHeight = 896;
        _graphics.PreferredBackBufferWidth = 1024;
        ScreenWidth = _graphics.PreferredBackBufferWidth;
        ScreenHeight = _graphics.PreferredBackBufferHeight;
        WorldWidth = ScreenWidth / TileSize;
        WorldHeight = ScreenHeight / TileSize;
        World = new();

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        InputManager.SetWindow(Window);
        
        EntityManager.Manager.AddObject(new Player(Content));
        EntityManager.Manager.AddObject(new GoalPost());

        Random r = new Random();

        int count = 10;
        for(int i = 0; i < count; i++)
        {
            var o = new Obstacle(Content, new Vector2(
                (float)r.NextDouble() * ScreenWidth,
                (float)r.NextDouble() * ScreenHeight),
                (ObstacleType)(int)(r.NextDouble() * Enum.GetNames(typeof(ObstacleType)).Length));

            EntityManager.Manager.AddObject(o);
        }

        Log.Information($"{nameof(HockeyGame)} initialized." +
            $" Width {_graphics.PreferredBackBufferWidth} Height {_graphics.PreferredBackBufferHeight}");
        base.Initialize();
    }

    protected override void LoadContent()
    {
        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // should i make a class for managing contents?
        Font = Content.Load<SpriteFont>("TextFont");

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        InputManager.BeginFrame();
        EntityManager.Manager.Update(gameTime);
        InputManager.EndFrame();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Gainsboro);
        _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
        EntityManager.Manager.Draw(_spriteBatch);

        base.Draw(gameTime);
        _spriteBatch.End();
    }
}
