using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using Survivor;

namespace Hockey;

public class HockeyGame : Game
{
    public static int ScreenWidth, ScreenHeight = 0;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private double time = 0;

    private float expPoint = 0;
    private float nextExp = 100;
    private float nextExpMul = 0.1f;

    private SpriteFont Font;

    public HockeyGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.PreferredBackBufferWidth = 1280;
        ScreenWidth = _graphics.PreferredBackBufferWidth;
        ScreenHeight = _graphics.PreferredBackBufferHeight;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        InputManager.SetWindow(Window);
        
        EntityManager.Manager.AddObject(new Player(Content));
        EntityManager.Manager.AddObject(new GoalPost());

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
