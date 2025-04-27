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
        private Texture2D debugPixel;

        // TESTS
        private StaticSwarmPoint testSwarmPoint;
        private MobileSwarmPoint testMobileSwarmPoint;
        Map testMap;
        Texture2D tilesetTxr;
        Texture2D monochromaticTilesetTxr;
        Texture2D inputsTilesetTxr;


        HourGlass testHourGlass;

        //Camera
        Camera camera;

        // Keyboard
        //KeyboardState kb_curr;

        // Player
        Player testPlayer;
        List<WorldObject> objectsNearPlayer;
        List<Keeno> keenosNearPlayer;

        // Keeno
        List<Keeno> keenos;
        Texture2D keenoTexture;


        // WorldObjects
        Texture2D choppedTree;

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
            camera.Zoom = 8;


            #region List Initialisations
            keenos = new List<Keeno>();
            objectsNearPlayer = new List<WorldObject>();
            keenosNearPlayer = new List<Keeno>();
            #endregion

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _renderTarget = new RenderTarget2D(_spriteBatch.GraphicsDevice,
                _graphics.PreferredBackBufferWidth/4, _graphics.PreferredBackBufferHeight/4);


            debugPixel = Content.Load<Texture2D>("Pixel");

            //testSwarmPoint = new StaticSwarmPoint(Content.Load<Texture2D>("SpriteSheets\\color_t")
            //    , 2, 19, 16, 16, new Rectangle(100, 100, 16, 16), debugPixel);

            //testMobileSwarmPoint = new MobileSwarmPoint(Content.Load<Texture2D>("Characters\\Keeno"), 3, new Rectangle(200, 200, 16, 16), debugPixel);


            // Keeno
            keenoTexture = Content.Load<Texture2D>("Characters\\Keeno");

            // World Objects
            choppedTree = Content.Load<Texture2D>("WorldObjects\\Flora\\choppedTree2");

            // Fonts
#if DEBUG
            debugFont = Content.Load<SpriteFont>("Fonts\\debugFont");
#endif
            tilesetTxr = Content.Load<Texture2D>("SpriteSheets\\color_t");
            monochromaticTilesetTxr = Content.Load<Texture2D>("SpriteSheets\\mono_t");
            inputsTilesetTxr = Content.Load<Texture2D>("SpriteSheets\\inputs_t");

            testMap = new Map("Content/MapData/testLevel_Map.csv", tilesetTxr, monochromaticTilesetTxr, inputsTilesetTxr, 16, 16, 49, choppedTree, debugPixel);
            testPlayer = new Player(Content.Load<Texture2D>("Characters\\Keeno"), 5, new Rectangle(200, 200, 16, 16),
                debugPixel, testMap, keenos);

            testHourGlass = new HourGlass(tilesetTxr, new Rectangle(50, 50, 16, 16));
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
                    StartUpdate(gt, Globals.KbCurr);
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
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.getCam());


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

            _spriteBatch.End();


            //Completing Scale effect on the screen
            //GraphicsDevice.SetRenderTarget(null);
            //_spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            //_spriteBatch.Draw(_renderTarget, GraphicsDevice.Viewport.Bounds, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
            //_spriteBatch.End();

            base.Draw(gt);
        }
        #region STATE UPDATES
        private void StartUpdate(GameTime gt, KeyboardState kb)
        {
            testMap.Update(gt);

            // Keeno
            foreach (var keeno in keenos)
            {
                keeno.Update(gt);
            }
            #region Old Code
            // PLAYER - WorldObject INTERACTION
            //if (kb_curr.IsKeyDown(Keys.E))
            //{
            //    for(var i = 0; i < testMap.WorldObjects.Count; i++)
            //    {
            //        if (testPlayer.Bounds.Intersects(testMap.WorldObjects[i].Bounds))
            //        {
            //            testMap.WorldObjects[i].OnInteract();
            //        }
            //    }
            //}

            // Clear the List of worldObjects that the are in range with the player
            //objectsNearPlayer.Clear();
            //for (var i = 0; i < testMap.WorldObjects.Count; i++)
            //{
            //    if (testPlayer.InteractionRange.Intersects(testMap.WorldObjects[i].Bounds))
            //    {
            //        objectsNearPlayer.Add(testMap.WorldObjects[i]);
            //    }
            //}
            //if (player_objectDistances.Count > 0)
            //{
            //    Debug.WriteLine($"Sorting prox: {player_objectDistances.Count}");
            //    foreach (var i in player_objectDistances)
            //        Debug.WriteLine(i.DistanceTo(testPlayer.Position).ToString());
            //}

            // Sort the list
            //var sortedList = objectsNearPlayer.OrderBy(x => x.DistanceTo(testPlayer.Position)).ToList();

            //if (sortedList.Count > 0)
            //{
            //    Debug.WriteLine($"Now prox: {sortedList.Count}");
            //    foreach (var i in sortedList)
            //        Debug.WriteLine(i.DistanceTo(testPlayer.Position).ToString());
            //}

            //if (sortedList.Count > 0)
            //{
            //    sortedList[0].Selected();
            //    if (kb_curr.IsKeyDown(Keys.E))
            //        sortedList[0].OnInteract();
            //}

            //// Player - movement
            //if (testMap.IsWalkable(testPlayer.HandleInput()))
            //{
            //    testPlayer.MoveMe(testPlayer.Direction);
            //}
            //else
            //{
            //    testPlayer.MoveMe(Vector2.Zero);
            //}


            //keenosNearPlayer.Clear();
            //for (var i = 0; i < keenos.Count; i++) 
            //{
            //    if (testPlayer.InteractionRange.Intersects(keenos[i].Bounds))
            //        keenosNearPlayer.Add(keenos[i]);
            //}
            //var sortedKeenoList = keenosNearPlayer.OrderBy(x => x.DistanceTo(testPlayer.Position)).ToList();
            //if (sortedKeenoList.Count > 0)
            //{
            //    sortedKeenoList[0].Selected();
            //    if (kb_curr.IsKeyDown(Keys.Q))
            //    {
            //        Vector2 distanceToPlayer = new Vector2(testPlayer.Position.X - sortedKeenoList[0].Position.X, testPlayer.Position.Y - sortedKeenoList[0].Position.Y);
            //        distanceToPlayer.Normalize();
            //        sortedKeenoList[0].MoveMe(distanceToPlayer);
            //    }
            //}



            // Loop through the list of WoldObjects
            //for (int i = 0; i < testMap.WorldObjects.Count; i++)
            //{
            //    // If the player's next movement would intersect with the Object's bounds
            //    if (testPlayer._targetDestinationBounds.Intersects(testMap.WorldObjects[i].Bounds))
            //    {
            //        testMap.WorldObjects[i].Tint = Color.Red;
            //        testPlayer.MoveMe(Vector2.Zero);

            //    }
            //    else
            //    {
            //        testPlayer.MoveMe(testPlayer.HandleInput());
            //    }

            //}

            //     list = list.OrderBy(x => x.AVC ? 0 : 1)
            //.ToList();



            //for (var j = 0; j < keenos.Count; j++)
            //{
            //    for (var i = 0; i < testMap.WorldObjects.Count; i++)
            //    {
            //        if (keenos[j].Bounds.Intersects(testMap.WorldObjects[i].Bounds))
            //        {
            //            testMap.WorldObjects[i].OnInteract();

            //            //if (testMap.WorldObjects[i] is Tree)
            //            //{
            //            //    var treeObject = (Tree)testMap.WorldObjects[i];


            //            //}
            //        }
            //    }
            //}
            //for (var j = 0; j < keenos.Count; j++)
            //{
            //    keenos[j].
            //}
            #endregion
            testPlayer.Update(gt);

            // temp testing code
            if (Keyboard.GetState().IsKeyDown(Keys.K))
            {
                int x = Globals.RNG.Next(0, _renderTarget.Width);
                int y = Globals.RNG.Next(0, _renderTarget.Height);

                var newKeeno = new Keeno(keenoTexture,5,new Rectangle(x,y,16,16),debugPixel);
                keenos.Add(newKeeno);
            }


            testHourGlass.Update(Globals.Q_KeyDown);
            camera.Position.X = (-testPlayer.Bounds.X + _graphics.PreferredBackBufferWidth / (2 * camera.Zoom));
            camera.Position.Y = (-testPlayer.Bounds.Y + _graphics.PreferredBackBufferHeight / (2 * camera.Zoom));



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

            // TEST MAP
            testMap.Draw(_spriteBatch);

            // Keenos
            foreach (var keeno in keenos)
            {
                keeno.Draw(_spriteBatch);
            }

            testHourGlass.Draw(_spriteBatch);



            testPlayer.Draw(_spriteBatch);
#if DEBUG
            _spriteBatch.DrawString(debugFont, _renderTarget.Width + "x " + _renderTarget.Height
                + "\nKeenos: " + keenos.Count,
                new Vector2(10, 10), Color.White);
#endif
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
