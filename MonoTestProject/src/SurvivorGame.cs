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

    const int MUL = 2; // screen is 4x world
    public static World World = new() { scaleX = MUL, scaleY = MUL };
    public static int ScreenWidth, ScreenHeight = 0;
    public static int WorldWidth, WorldHeight;
    private const int TileSize = 16;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    Random rand = new Random();
    const float maxSpawnTime = 5;
    double spawnTimer = 0;

    public HockeyGame()
    {

        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1024; // 4x NES resolution
        _graphics.PreferredBackBufferHeight = 896;
        ScreenWidth = _graphics.PreferredBackBufferWidth;
        ScreenHeight = _graphics.PreferredBackBufferHeight;

        WorldWidth = ScreenWidth / MUL;
        WorldHeight = ScreenHeight / MUL; // NES resolution

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
    }

    protected override void Initialize()
    {
        InputManager.SetWindow(Window);
        var manager = EntityManager.Manager;
        manager.AddObject(new Player(Content)
        {
            WorldPosition = new Vector2(20, WorldHeight / 2)
        });
        //manager.AddObject(new GoalPost()
        //{
        //    Width = 30,
        //    Height = 90,
        //    WorldPosition = new Vector2(WorldWidth, WorldHeight / 2)
        //});
        EntityManager.Manager.AddObject(new Projectile(Content)
        {
            WorldPosition = new Vector2(WorldWidth / 2, WorldHeight / 2)
        });

        //int count = 10;
        //for(int i = 0; i < count; i++)
        //{
        //    var o = new Obstacle(Content, new Vector2(
        //        (int)(rand.NextDouble() * WorldWidth / TileSize) * TileSize,
        //        (int)(rand.NextDouble() * WorldHeight / TileSize) * TileSize),
        //        (ObstacleType)(int)(rand.NextDouble() * Enum.GetNames(typeof(ObstacleType)).Length));

        //    EntityManager.Manager.AddObject(o);
        //}

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

    private void SpawnAmmoRandom()
    {
        EntityManager.Manager.AddObject(new Projectile(Content)
        {
            WorldPosition = new Vector2((float)rand.NextDouble() * WorldWidth, (float)rand.NextDouble() * WorldHeight)
        });
    }

    protected override void Update(GameTime gameTime)
    {
        InputManager.BeginFrame();
        EntityManager.Manager.Update(gameTime);
        spawnTimer += gameTime.ElapsedGameTime.TotalSeconds;
        if (spawnTimer > maxSpawnTime)
        {
            spawnTimer = 0;
            SpawnAmmoRandom();
        }
        InputManager.EndFrame();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Gainsboro);
        _spriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointClamp);
        
        EntityManager.Manager.Draw(_spriteBatch);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
