using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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

        // Adding Render Target to scale the screen
        private RenderTarget2D _renderTarget;

        // RNG
        //public static readonly Random RNG = new Random();


        #region VARIABLES

        // Game State
        private GameState currentGameState;

        // Debug Pixel
        //private Texture2D debugPixel;

        // TESTS
        //private StaticSwarmPoint testSwarmPoint;
        //private MobileSwarmPoint testMobileSwarmPoint;
        Map testMap;
        //Texture2D tilesetTxr;
        //Texture2D monochromaticTilesetTxr;
        //Texture2D inputsTilesetTxr;
        ResourceType debugResource;


        //HourGlass testHourGlass;

        //Camera
        Camera camera;

        // Keyboard
        //KeyboardState kb_curr;

        // Player
        Player testPlayer;

        // Keeno
        List<Keeno> keenos;
        //Texture2D keenoTexture;

        // WorldObjects
        //Texture2D choppedTree;

        // Items
        //List<Item> items;


        // Fonts
#if DEBUG
        public static SpriteFont debugFont;
#endif

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
            camera.Position = Vector2.Zero;
            camera.Zoom = 6;


            #region List Initialisations
            keenos = new List<Keeno>();
            #endregion

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _renderTarget = new RenderTarget2D(_spriteBatch.GraphicsDevice,
                _graphics.PreferredBackBufferWidth/4, _graphics.PreferredBackBufferHeight/4);

            Assets.Load(this.Content);

            // Fonts
#if DEBUG
            debugFont = Content.Load<SpriteFont>("Fonts\\debugFont");
#endif

            //testMap = new Map("Content/MapData/testLevel_Map.csv");
            testMap = new Map("Content/MapData/testLevel3.csv");

            testPlayer = new Player(Content.Load<Texture2D>("Characters\\Keeno"), 5, new Rectangle(400, 300, 16, 16),
                Assets.DebugPixelTxr, testMap, keenos);

            //testHourGlass = new HourGlass(tilesetTxr, new Rectangle(50, 50, 16, 16));

            //testBuildingObject = new (Content.Load<Texture2D>
            //    ("WorldObjects\\Buildings\\Houses\\tents_w"), 0, new Rectangle(300, 200, 16, 16), Content.Load<Texture2D>("WorldObjects\\Items\\scroll"));

            // TODO: FIND OUT
            // Does this update according to the number of TCS???
            foreach (var townCentre in testMap.WorldObjects.OfType<TownCentre>())
            {
                townCentre.KeenoSpawned += keeno => keenos.Add(keeno);
                Debug.WriteLine("Subscribed to KeenoSpawned on TownCentre");
            }
        }

        protected override void Update(GameTime gt)
        {
            Globals.Update(gt);
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
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
            // screen scaling
            //GraphicsDevice.SetRenderTarget(_renderTarget);

            GraphicsDevice.Clear(Color.Black);

            //_spriteBatch.Begin();


            switch (currentGameState)
            {
                case GameState.Start:
                    StartDraw(gt);
                    break;
                case GameState.Playing:
                    PlayingDraw();
                    break;
                case GameState.GameOver:
                    GameOverDraw();
                    break;
            }

            //_spriteBatch.End();
            //_spriteBatch.Begin();
            //_spriteBatch.End();

            //Completing Scale effect on the screen
            //GraphicsDevice.SetRenderTarget(null);
            //_spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            //_spriteBatch.Draw(_renderTarget, GraphicsDevice.Viewport.Bounds, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
            //_spriteBatch.End();

            base.Draw(gt);
        }
        #region STATE UPDATES
        private void StartUpdate(GameTime gt)
        {
            testMap.Update(gt);

            // Keeno
            foreach (var keeno in keenos)
            {
                keeno.Update(gt);
            }

            testPlayer.Update(gt);

            // temp testing code
            if (Keyboard.GetState().IsKeyDown(Keys.K))
            {
                int x = Globals.RNG.Next(0, _renderTarget.Width);
                int y = Globals.RNG.Next(0, _renderTarget.Height);

                var newKeeno = new Keeno(Assets.KeenoTxr,5,new Rectangle(x,y,16,16),Assets.DebugPixelTxr);
                keenos.Add(newKeeno);
            }

            camera.Position.X = (-testPlayer.Bounds.X + _graphics.PreferredBackBufferWidth / (2 * camera.Zoom));
            camera.Position.Y = (-testPlayer.Bounds.Y + _graphics.PreferredBackBufferHeight / (2 * camera.Zoom));

            // Testing ResourceTracker
            //if (Globals.Q_KeyPress)
            //    ResourceTracker.Add(ResourceType.Wood, 10);
           
            switch (debugResource)
            {
                case ResourceType.None:
                    if (Globals.Tab_KeyPress)
                        debugResource = ResourceType.Food;
                    break;
                case ResourceType.Food:
                    if(Globals.Tab_KeyPress)
                        debugResource = ResourceType.Wood;
                    break;
                case ResourceType.Wood:
                    if (Globals.Tab_KeyPress)
                        debugResource = ResourceType.Gold;
                    break;
                case ResourceType.Gold:
                    if (Globals.Tab_KeyPress)
                        debugResource = ResourceType.Food;
                    break;
            }
            if (Globals.UpArrow_KeyPress)
                ResourceTracker.Add(debugResource, 10);
            if (Globals.DownArrow_KeyPress)
                ResourceTracker.Add(debugResource, -10);
        }

        private void PlayingUpdate()
        {

        }

        private void GameOverUpdate()
        {

        }
        #endregion
        #region STATE DRAWS
        private void StartDraw(GameTime gt)
        {
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.getCam());

            // TEST MAP
            testMap.Draw(_spriteBatch);

            // Keenos
            foreach (var keeno in keenos)
            {
                keeno.Draw(_spriteBatch);
            }
            testPlayer.Draw(_spriteBatch);

            _spriteBatch.End();
            _spriteBatch.Begin();
#if DEBUG
            _spriteBatch.DrawString(debugFont,
                _renderTarget.Width + "x " + _renderTarget.Height
                + "\nKeenos: " + keenos.Count
                + "\nWood: " + ResourceTracker.GetAmount(ResourceType.Wood)
                + "\nFood: " + ResourceTracker.GetAmount(ResourceType.Food)
                + "\nGold: " + ResourceTracker.GetAmount(ResourceType.Gold)
                + "\nStone: " + ResourceTracker.GetAmount(ResourceType.Stone)
                + "\nSelected Resource" + debugResource,
                
                new Vector2(10, 10), Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, .1f);
#endif
            _spriteBatch.End();
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
