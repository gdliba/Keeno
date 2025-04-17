using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Keeno
{
    public enum GameState
    {
        Start,
        Playing,
        GameOver
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        #region VARIABLES

        // Game State
        private GameState currentGameState;

        // Debug Pixel
        private Texture2D debugPixel;

        // TESTS
        private StaticSwarmPoint testSwarmPoint;
        private MobileSwarmPoint testMobileSwarmPoint;



        #endregion

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.Title = "Keeno";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Match the resolution to the current display
            _graphics.PreferredBackBufferWidth = 
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = 
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            // Set screen to Fullscreen
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();


            currentGameState = GameState.Start;


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            debugPixel = Content.Load<Texture2D>("Pixel");

            testSwarmPoint = new StaticSwarmPoint(Content.Load<Texture2D>("SpriteSheets\\color_t")
                , 2, 19, 16, 16, new Rectangle(100, 100, 16, 16), debugPixel);

            testMobileSwarmPoint = new MobileSwarmPoint(Content.Load<Texture2D>("Characters\\Keeno"), 3, new Rectangle(200, 200, 16, 16));
        }

        protected override void Update(GameTime gt)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            // GameState Switch
            switch (currentGameState)
            {
                case GameState.Start:
                    StartUpdate(gt);
                    break;
                case GameState.Playing:
                    PlayingUpdate();
                    break;
                case GameState.GameOver:
                    GameOverUpdate();
                    break;
            }
            base.Update(gt);
        }

        protected override void Draw(GameTime gt)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            switch (currentGameState)
            {
                case GameState.Start:
                    StartDraw();
                    break;
                case GameState.Playing:
                    PlayingDraw();
                    break;
                case GameState.GameOver:
                    GameOverDraw();
                    break;
            }

            _spriteBatch.End();


            base.Draw(gt);
        }
        #region STATE UPDATES
        private void StartUpdate(GameTime gt)
        {
            testMobileSwarmPoint.updateme(gt);
        }

        private void PlayingUpdate()
        {

        }

        private void GameOverUpdate()
        {

        }
        #endregion
        #region STATE DRAWS
        private void StartDraw()
        {
            testSwarmPoint.drawme(_spriteBatch);
            testMobileSwarmPoint.drawme(_spriteBatch);
        }

        private void PlayingDraw()
        {

        }

        private void GameOverDraw()
        {

        }
        #endregion
    }
}
