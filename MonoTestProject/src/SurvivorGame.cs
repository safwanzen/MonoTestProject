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

    public static World World = new() { scaleX = 4, scaleY = 4 };
    public static int ScreenWidth, ScreenHeight = 0;
    public static int WorldWidth, WorldHeight;
    private const int TileSize = 16;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public HockeyGame()
    {
        int mul = 4; // screen is 4x world

        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1024; // 4x NES resolution
        _graphics.PreferredBackBufferHeight = 896;
        ScreenWidth = _graphics.PreferredBackBufferWidth;
        ScreenHeight = _graphics.PreferredBackBufferHeight;

        WorldWidth = ScreenWidth / mul;
        WorldHeight = ScreenHeight / mul; // NES resolution

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
    }

    protected override void Initialize()
    {
        InputManager.SetWindow(Window);
        
        EntityManager.Manager.AddObject(new Player(Content)
        {
            WorldPosition = new Vector2(20, WorldHeight / 2)
        });
        EntityManager.Manager.AddObject(new GoalPost()
        {
            Width = 30,
            Height = 90,
            WorldPosition = new Vector2(WorldWidth, WorldHeight / 2)
        });

        Random r = new Random();

        int count = 10;
        for(int i = 0; i < count; i++)
        {
            var o = new Obstacle(Content, new Vector2(
                (int)(r.NextDouble() * WorldWidth / TileSize) * TileSize,
                (int)(r.NextDouble() * WorldHeight / TileSize) * TileSize),
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

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
