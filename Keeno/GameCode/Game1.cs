using Keeno.GameCode;
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
        EndOfDay,
        Pause,
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


        // Managers
        TimeManager timeManager;
        UIManager uiManager;
        ResetManager resetManager;
        MusicPlayer musicPlayer;
        TextManager textManager;



        // Debug Pixel
        //private Texture2D debugPixel;

        // TESTS
        TypewriterText testText, firstKeenoTutorial;


        float brightness;
        Map testMap;
        ResourceType debugResource;

        //Camera
        Camera camera;


        // Player
        Player testPlayer;

        // Keeno
        List<Keeno> keenos;
        List<Keeno> startScreenkeenos, endOfDayScreenKeenos;

        


        // Fonts
#if DEBUG
        public static SpriteFont debugFont;

#endif

        #endregion

        public Game1()
        {
            Globals.Graphics = _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.Title = "Keeno";
            IsMouseVisible = true;
            Globals.ChangeResolution(1920, 1080);

        }

        protected override void Initialize()
        {
            // Match the resolution to the current display
            //_graphics.PreferredBackBufferWidth = 1920;
            //GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //_graphics.PreferredBackBufferHeight = 1080;
            //GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            // Set screen to Fullscreen
            //_graphics.IsFullScreen = false;
            //_graphics.ApplyChanges();


            currentGameState = GameState.Start;
            camera.Position = Vector2.Zero;
            camera.Zoom = 5;


            #region List Initialisations
            keenos = new List<Keeno>();
            startScreenkeenos = new List<Keeno>();
            endOfDayScreenKeenos = new List<Keeno>();

            #endregion

            brightness = 1;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _renderTarget = new RenderTarget2D(_spriteBatch.GraphicsDevice,
                _graphics.PreferredBackBufferWidth/4, _graphics.PreferredBackBufferHeight/4);

            Assets.Load(this.Content);

            textManager = new TextManager();



            // Fonts
#if DEBUG
            debugFont = Content.Load<SpriteFont>("Fonts\\debugFont");

#endif
            //monogramFont = Content.Load<SpriteFont>("Fonts\\monogram");

            //testMap = new Map("Content/MapData/testLevel_Map.csv");
            testMap = new Map("Content/MapData/MainMap1.csv");

            var player = new Player(Content.Load<Texture2D>("Characters\\Keeno"), 5, new Rectangle(400, 300, 16, 16),
                Assets.DebugPixelTxr, testMap, keenos);
            testPlayer = player;
            player.FirstInteraction += () =>
            {
                textManager.SetActive("Resource Interact");
            };
            player.FirstFollower += () =>
            {
                textManager.SetActive("First Follower");
            };
            player.FirstBluePrint += () =>
            {
                textManager.SetActive("Blueprint1");
            };
            testMap.BellRung += () =>
            {
                foreach (var keeno in keenos)
                {
                    keeno.PlayerRangBell();
                }
            };
            testMap.TownCentreSpawnedKeeno += keeno =>
            {
                keenos.Add(keeno);
                if (keenos.Count == 1)
                    textManager.SetActive("First Keeno");
                if (keenos.Count == 2)
                    textManager.SetActive("Housing");
                if (keenos.Count == 3)
                    textManager.SetActive("Population");
                if (keenos.Count == 5)
                    textManager.SetActive("BuildersCabin");
            };


            //testHourGlass = new HourGlass(tilesetTxr, new Rectangle(50, 50, 16, 16));

            //testBuildingObject = new (Content.Load<Texture2D>
            //    ("WorldObjects\\Buildings\\Houses\\tents_w"), 0, new Rectangle(300, 200, 16, 16), Content.Load<Texture2D>("WorldObjects\\Items\\scroll"));
            
            timeManager = new TimeManager();
            uiManager = new UIManager();
            uiManager.Load();
            musicPlayer = new MusicPlayer();

            resetManager = new ResetManager(testMap, timeManager, keenos, testPlayer, textManager);

            #region Button Presses
            uiManager.OnStartPressed = () =>
            {
                DoStartButtonPressed();
            };
            uiManager.OnContinuePressed = () =>
            {
                currentGameState = GameState.Playing;
                musicPlayer.ResumeMusic();
            };
            uiManager.OnRestartPressed = () =>
            {
                musicPlayer.PlayFirstRain();
                resetManager.ResetAll();
                currentGameState = GameState.Playing;
            };
            uiManager.OnMainMenuPressed = () =>
            {
                currentGameState = GameState.Start;
                startScreenkeenos.Clear();
                endOfDayScreenKeenos.Clear();

            };
            uiManager.OnExitPressed = () =>
            {
                Exit();
            };
            uiManager.OnNextDayPressed = () =>
            {
                //resetManager.NextDay();
                currentGameState = GameState.Playing;
                endOfDayScreenKeenos.Clear();
            };
            #endregion

        }
        private void DoStartButtonPressed()
        {
            musicPlayer.PauseMusic();
            currentGameState = GameState.Playing;
            musicPlayer.PlayFirstRain();
            textManager.CompleteReset();
        }

        protected override void Update(GameTime gt)
        {
            for (int i = keenos.Count - 1; i >= 0; i--)
            {
                if (keenos[i].State == KeenoState.Dead)
                    keenos.RemoveAt(i);
            }

            Globals.Update(gt);
            //if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            // GameState Switch
            switch (currentGameState)
            {
                case GameState.Start:
                    StartUpdate(gt);
                    break;
                case GameState.Playing:
                    PlayingUpdate(gt);
                    break;
                case GameState.EndOfDay:
                    EndOfDayUpdate(gt);
                    break;
                case GameState.Pause:
                    PauseUpdate();
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
                case GameState.EndOfDay:
                    EndOfDayDraw(gt);
                    break;
                case GameState.Pause:
                    PauseDraw();
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
            uiManager.StartUpdate();
            musicPlayer.PlayMainTheme();

            if (Globals.Enter_KeyPress)
            {
                DoStartButtonPressed();
            }

            // Random Keeno spawn
            foreach (var keeno in startScreenkeenos)
            {
                keeno.Update(gt);
            }
            for (int i = 0; i <= 500;)
            {
                if(startScreenkeenos.Count>=500)
                    break;
                int x = Globals.RNG.Next(0, Globals.ScreenWidth);
                int y = Globals.RNG.Next(0, Globals.ScreenHeight);

                var newKeeno = new Keeno(Assets.KeenoTxr, 5, new Rectangle(x, y, 16, 16), Assets.DebugPixelTxr, null, true);
                startScreenkeenos.Add(newKeeno);
                break;
            }
        }
        private void EndOfDayUpdate(GameTime gt)
        {
            uiManager.EndOfDayUpdate();
            if (Globals.X_KeyPress)
            {
                resetManager.ResetAll();
                currentGameState = GameState.Playing;
            }


            // Random Keeno spawn (1 Keeno per alive Keeno in Game)
            foreach (var keeno in endOfDayScreenKeenos)
            {
                keeno.Update(gt);
            }
            for (int i = 0; i <= keenos.Count;)
            {
                if (endOfDayScreenKeenos.Count == keenos.Count)
                    break;
                int x = Globals.RNG.Next(Globals.ScreenWidth/4, 3*Globals.ScreenWidth/4);
                int y = Globals.RNG.Next(Globals.ScreenHeight / 4, 3 * Globals.ScreenHeight / 4);

                var newKeeno = new Keeno(Assets.KeenoTxr, 5, new Rectangle(x, y, 16, 16), Assets.DebugPixelTxr, null, true);
                endOfDayScreenKeenos.Add(newKeeno);
                break;
            }
        }

        private void PlayingUpdate(GameTime gt)
        {
            #region Updates
            textManager.Update();
            timeManager.UpdateTime((float)gt.ElapsedGameTime.TotalSeconds);
            testMap.Update(gt);
            testPlayer.Update(gt);
            // Keeno
            foreach (var keeno in keenos)
            {
                keeno.Update(gt);
            }
            #endregion
            #region EndOfDay / Hunger
            // When the day ends
            if (brightness <= .1)
            {
                endOfDayScreenKeenos.Clear();
                resetManager.NextDay();

                // Each Keeno should Eat
                foreach ( var keeno in keenos)
                {
                    ResourceTracker.Spend(ResourceType.Food, 1);
                    keeno.StopConstructingSound();
                    keeno.StopWorkSound();
                }
                // If you don't have enough food for every Keeno
                if (ResourceTracker.GetAmount(ResourceType.Food) < 0)
                {
                    int starvingKeeno = ResourceTracker.GetAmount(ResourceType.Food);

                    if (keenos.Count>0)
                        for ( int i = 0; i < Math.Abs(starvingKeeno); i++)
                        {
                            ResourceTracker.Spend(ResourceType.Keeno, 1);
                            keenos.RemoveAt(0);
                        }
                    // Reset Food to 0 as it doesn't make sense to have negative resources
                    ResourceTracker.Add(ResourceType.Food, Math.Abs(starvingKeeno));
                }
                currentGameState = GameState.EndOfDay;
            }
            #endregion

            if (Globals.I_KeyPress)
                Globals.HidePromtsAndNames = !Globals.HidePromtsAndNames;

            camera.Position.X = (-testPlayer.Bounds.X + _graphics.PreferredBackBufferWidth / (2 * camera.Zoom));
            camera.Position.Y = (-testPlayer.Bounds.Y + _graphics.PreferredBackBufferHeight / (2 * camera.Zoom));

#if DEBUG
            switch (debugResource)
            {
                case ResourceType.None:
                    if (Globals.Tab_KeyPress)
                        debugResource = ResourceType.Food;
                    break;
                case ResourceType.Food:
                    if (Globals.Tab_KeyPress)
                        debugResource = ResourceType.Wood;
                    break;
                case ResourceType.Wood:
                    if (Globals.Tab_KeyPress)
                        debugResource = ResourceType.Gold;
                    break;
                case ResourceType.Gold:
                    if (Globals.Tab_KeyPress)
                        debugResource = ResourceType.Stone;
                    break;
                case ResourceType.Stone:
                    if (Globals.Tab_KeyPress)
                        debugResource = ResourceType.Food;
                    break;
            }
            //if (Globals.X_KeyPress)
            //    ResourceTracker.Reset();
            if (Globals.UpArrow_KeyPress)
                ResourceTracker.Add(debugResource, 10);
            if (Globals.DownArrow_KeyPress)
                ResourceTracker.Add(debugResource, -10);
#endif

            // Pause
            if (Globals.Escape_KeyPress)
            {
                musicPlayer.PauseMusic();
                currentGameState = GameState.Pause;
                Assets.PauseSFX.Play();

                foreach (var keeno in keenos)
                {
                    keeno.StopConstructingSound();
                    keeno.StopWorkSound();

                }
            }
        }
        private void PauseUpdate()
        {
            uiManager.UpdatePause();
            // UnPause
            if (Globals.Escape_KeyPress)
            {
                musicPlayer.ResumeMusic();
                currentGameState = GameState.Playing;
                Assets.ButtonPressSFX.Play();
            }

        }

        private void GameOverUpdate()
        {

        }
        #endregion
        #region STATE DRAWS
        private void StartDraw(GameTime gt)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            foreach (var keeno in startScreenkeenos)
            {
                keeno.Draw(_spriteBatch);
            }
            uiManager.StartDraw(_spriteBatch);
            _spriteBatch.End();
        }
        private void EndOfDayDraw(GameTime gt)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            foreach (var keeno in endOfDayScreenKeenos)
            {
                keeno.Draw(_spriteBatch);
            }
            uiManager.EndOfDaytDraw(_spriteBatch);
            _spriteBatch.End();
        }

        private void PlayingDraw()
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


            #region Night Time Overlay
            // Night Time Overlay
            float t = timeManager.TimeOfDay / timeManager.DayLengthSeconds;
            brightness = 0.5f + 0.5f * (float)Math.Cos(Math.PI * t);
            if (t < 0.75f)
                brightness = 1f;
            else
            {
                float ramp = (t - 0.75f) / 0.25f;
                ramp = MathHelper.Clamp(ramp, 0f, 1f);

                // Fade linearly
                brightness = 1f - ramp;
            }
            int screenwidth = _graphics.PreferredBackBufferWidth;
            int screenHeight = _graphics.PreferredBackBufferHeight;

            Rectangle worldRect = new Rectangle(-screenwidth / 2, -screenHeight / 2, 2 * screenwidth, 2 * screenHeight);
            Color tintColor = Color.Lerp(Color.Black * .75f, Color.Transparent, brightness);

            _spriteBatch.Draw(Assets.DebugPixelTxr, worldRect,
                null, tintColor, 0f, Vector2.Zero, SpriteEffects.None, .099f);
            #endregion
            _spriteBatch.End();

            // Draw the Hud
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            // Hud Test
            //_spriteBatch.Draw(Assets.UITest, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
            //uiManager.Draw(_spriteBatch);
            uiManager.DrawPlaying(_spriteBatch, keenos);
            textManager.Draw(_spriteBatch);

#if DEBUG
            //_spriteBatch.DrawString(Assets.MonogramFont,
            //    _renderTarget.Width + "x " + _renderTarget.Height
            //    + "\nTime Of Day: " + timeManager._timeOfDay,

            //    new Vector2(10, 10), Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, .1f);
#endif
            _spriteBatch.End();

        }
        private void PauseDraw()
        {
            PlayingDraw();
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            uiManager.DrawPause(_spriteBatch);
            _spriteBatch.End();
        }

        private void GameOverDraw()
        {

        }
        #endregion
    }
}
