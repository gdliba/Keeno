using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace Keeno
{
    static class Globals
    {
        public static float DeltaTime { get; private set; }

        public static readonly Random RNG = new Random();

        public static bool HidePromtsAndNames = false;

        public static GraphicsDeviceManager Graphics;
        public static int ScreenWidth { get => Graphics.PreferredBackBufferWidth; }
        public static int ScreenHeight { get => Graphics.PreferredBackBufferHeight; }

        #region Input Properties
        public static Point MousePosition { get; private set; }

        public static MouseState MsCurr { get; private set; }
        public static MouseState MsOld { get; private set; }

        public static KeyboardState KbCurr { get; private set; }
        public static KeyboardState KbOld { get; private set; }

        public static bool LeftClick { get; private set; }
        public static bool RightClick { get; private set; }
        public static bool MiddleClick { get; private set; }

        public static bool W_KeyDown => KeyDown(Keys.W);
        public static bool A_KeyDown => KeyDown(Keys.A);
        public static bool S_KeyDown => KeyDown(Keys.S);
        public static bool D_KeyDown => KeyDown(Keys.D);
        public static bool Q_KeyDown => KeyDown(Keys.Q);
        public static bool E_KeyDown => KeyDown(Keys.E);
        public static bool X_KeyDown => KeyDown(Keys.X);


        public static bool Q_KeyPress => KeyPress(Keys.Q);
        public static bool E_KeyPress => KeyPress(Keys.E);
        public static bool X_KeyPress => KeyPress(Keys.X);
        public static bool I_KeyPress => KeyPress(Keys.I);
        public static bool UpArrow_KeyPress=> KeyPress(Keys.Up);
        public static bool DownArrow_KeyPress => KeyPress(Keys.Down);
        public static bool Tab_KeyPress => KeyPress(Keys.Tab);
        public static bool Escape_KeyPress => KeyPress(Keys.Escape);



        /// <summary>
        /// Key is newly pressed this frame.
        /// </summary>
        /// <param name="key">The key that was pressed</param>
        /// <returns>true in the frame it was pressed</returns>
        public static bool KeyPress(Keys key)
        {
            return KbCurr.IsKeyDown(key) && KbOld.IsKeyUp(key);
        }

        /// <summary>
        /// Key is currently pressed (no change information)
        /// </summary>
        /// <param name="key">The key that is pressed</param>
        /// <returns>true if the key is held down this frame</returns>
        public static bool KeyDown(Keys key)
        {
            return KbCurr.IsKeyDown(key);
        }
        public static void ChangeResolution(int width, int height)
        {
            Graphics.PreferredBackBufferWidth = width;
            Graphics.PreferredBackBufferHeight = height;
            Graphics.IsFullScreen = true;
            Graphics.ApplyChanges();
        }
        public static void Update(GameTime gt)
        {
            DeltaTime = (float)gt.ElapsedGameTime.TotalSeconds;

            MousePosition = Mouse.GetState().Position;

            MsOld = MsCurr;
            MsCurr = Mouse.GetState();

            KbOld = KbCurr;
            KbCurr = Keyboard.GetState();

            LeftClick = MsCurr.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && MsOld.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released;
            RightClick = MsCurr.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && MsOld.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released;
            MiddleClick = MsCurr.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && MsOld.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Released;
        }
        #endregion

        #region Tile Properties and Indexes
        // Tile Properties
        public const int TilemapColumns = 49;
        public const int Tile_Width_Height = 16;

        public const int OccupiedTileIndex = 0;
        public const int EmptyTileIndex = -1;
        public const int TileSelectedIndex = 624;
        public const int ItemSelectedIndex = 625;
        public const int BlueprintIndex = 767;

        // Folliage
        public const int TreeTileIndex = 51;
        public const int TreeChoppedTileIndex = 105;

        public const int FoliageTileIndex = 7;
        public const int FoliageTileIndex2 = 5;
        public const int FoliageTileIndex3 = 6;




        public const int FarmTileIndex1 = 309;
        public const int FarmTileIndex2 = 310;
        public const int HarvestedFarmTileIndex = 307;
        public const int FarmLandTileIndex = 306;

        public const int TownCentreTileIndex = 983;
        public const int BuilderCabinTileIndex = 985;
        public const int ShopBuildingTileIndex = 984;
        public const int ResourceStorageTileIndex = 980;


        public const int RockTileIndex = 103;
        public const int HarvestedRockTileIndex = 68;

        public const int GoldTileIndex = 522;
        public const int GoldCoinTileIndex = 218;

        public const int BreakableWallTileIndex = 885;

        public const int MineEntranceTileIndex = 300;
        public const int FarmEntranceTileIndex = 151;


        public const int BrokenBridgeTileIndex = 262;
        public const int FixedBridgeTileIndex = 261;
        public const int RiverTileIndex = 204;


        public const int ConstructionSiteTileIndex = 647;

        public const int BellTileIndex = 513;





        // HourGlass
        public const int EmptyHourGlassIndex = 628;
        //public const int FullHourGlassIndex = 630;
        public const int FullHourGlassIndex = 627;


        //Inputs Tileset
        public const int InputsTilesetColumns = 34;
        public const int InputsTileset_Width_Height = 16;

        public const int InputsTilesetIndex_E = 87;
        public const int InputsTilesetIndex_Q = 85;
        public const int InputsTilesetIndex_X = 156;

        //Left Click Index
        //public const int InputsTilesetIndex_E = 111;
        //Right Click Index
        //public const int InputsTilesetIndex_E = 112;


        #endregion

        #region UI
        public static readonly List<string> StartScreenButtons = new List<string>()
        {
            "Start",
            "Exit"
        };
        public static readonly List<string> PauseScreenButtons = new List<string>()
        {
            "Continue",
            "Restart",
            "Main Menu",
            "Quit"
        };
        public static readonly List<string> EndOfDayScreenButtons = new List<string>()
        {
            "Next Day",
            "Exit"
        };
        #endregion

        // Player
        public static int PlayerMovementSpeed = 50;
        public static float DropOffKeenoSpeed = .03f;
        public static float NeutralInteractSpeed = .02f;
        public static float DestroyInteractSpeed = .02f;
        public static float UpgradeInteractSpeed = .02f;

        // Keeno
        //public static float KeenoMovementSpeed = 20;

        // Tree
        public static int TreeHealth = 5;
        public static int TreeWoodAmount = 1;
        public static int TreeWorkerSlots = 1;
        public static float TreeWorkAmount = 12f;

        // Farm
        public static int FarmHealth = 10;
        public static int FarmFoodAmount = 1;
        public static int FarmWorkerSlots = 1;
        public static float FarmWorkAmount = 4f;


        // Rock / Stone
        public static int RockHealth = 5;
        public static int RockStoneAmount = 1;
        public static int RockWorkerSlots = 1;
        public static float RockWorkAmount = 22f;


        // Gold
        public static int GoldHealth = 1;
        public static int GoldGoldAmount = 1;
        public static int GoldWorkerSlots = 1;
        public static float GoldWorkAmount = 8f;

        // BreakableWall
        public static int BreakableWallHealth = 1;
        public static int BreakableWallWorkerSlots = 100;
        public static float BreakableWallWorkAmount = 6f;

        // Layer Depths
        public static float UIHighlightLD = .052f;
        public static float UIButtonLD = .051f;
        public static float UIPannelLD = .050f;
        public static float InGameUILD = .049f;
        public static float ItemSelectedTxrLD = .048f;
        public static float ItemTxrLD = .047f;
        public static float BlueprintTxrLD = .046f;
        public static float PlayerLD = .045f;
        public static float ButtonPromptLD = .044f;
        public static float ResourceBeingCarriedTxrLD = .043f;
        public static float HourGlassLD = .042f;
        public static float KeenoLD = .041f;
        public static float SelectedTxrLD = .040f;
        public static float WolrdObjectLD = .039f;
        public static float BuildingLD = .038f;
        public static float RiverLD = .037f;
        public static float EmptyTileLD = .001f;
        public static float MapLD = 0f;

        // Starting Housing Value 
        public static int StartingHousingValue = 5;

        
        // Buildings
        // Tent
        public static int TentWoodCost = 10;
        public static int TentStoneCost = 0;
        public static int TentUpgradeWoodCost = 5;
        public static int TentUpgradeStoneCost = 0;
        public static int TentPopulationAddition = 3;
        public static int TentBLGoldPrice = 1;

            // House
        public static int HouseWoodCost = 10;
        public static int HouseStoneCost = 5;
        public static int HouseUpgradeWoodCost = 5;
        public static int HouseUpgradeStoneCost = 5;
        public static int HousePopulationAddition = 5;
        public static int HouseBLGoldPrice = 2;

            // ResourceStorage
        public static int ResourceStorageWoodCost = 1;
        public static int ResourceStorageStoneCost = 0;
        public static int ResourceStorageUpgradeWoodCost = 1;
        public static int ResourceStorageUpgradeStoneCost = 0;
        public static int ResourceStorageBLGoldPrice = 2;
        
        // Bridge
        public static int BridgeWoodCost = 20;
        public static int BridgeStoneCost = 0;
        public static int BridgeUpgradeWoodCost = 1;
        public static int BridgeUpgradeStoneCost = 0;

        // FarmLand
        public static int FarmLandWoodCost = 10;
        public static int FarmLandStoneCost = 2;
        public static int FarmLandUpgradeWoodCost = 2;
        public static int FarmLandUpgradeStoneCost = 1;
        public static int FarmLandBLGoldPrice = 2;
    }
    static class Assets
    {
        #region Textures
        public static Texture2D KeenoTxr {  get; private set; }
        public static Texture2D WorkerKeenoTxr { get; private set; }
        public static Texture2D KeenoCarryingTxr { get; private set; }


        public static Texture2D DebugPixelTxr { get; private set; }
        public static Texture2D TilesetTxr { get; private set; }
        public static Texture2D MonochromaticTilesetTxr { get; private set; }
        public static Texture2D InputsTilesetTxr { get; private set; }
        public static Texture2D ChoppedTreeTxr {  get; private set; }
        public static Texture2D TentsTxr { get; private set; }
        public static Texture2D TentsWhiteTxr { get; private set; }

        public static Texture2D HousesTxr { get; private set; }
        public static Texture2D HousesWhiteTxr { get; private set; }
        public static Texture2D FarmLandWhiteTxr { get; private set; }



        public static Texture2D BlueprintTxr { get; private set; }
        public static Texture2D RockTxr { get; private set; }

        public static Texture2D WhiteRockTxr { get; private set; }
        public static Texture2D GoldOreTxr { get; private set; }


        // UI Objects
        public static Texture2D UIWoodTxr { get; private set; }
        public static Texture2D UIStoneTxr { get; private set; }
        public static Texture2D UIFoodTxr { get; private set; }

        // UI Icons
        public static Texture2D UIWoodIconTxr { get; private set; }
        public static Texture2D UIStoneIconTxr { get; private set; }
        public static Texture2D UIFoodIconTxr { get; private set; }
        public static Texture2D UIGoldIconTxr { get; private set; }
        public static Texture2D UIHousingIconTxr { get; private set; }

        public static Texture2D UISwapIconTxr { get; private set; }


        // UI Interface
        public static Texture2D UIPanelTxr { get; private set; }
        public static Texture2D UIHighlightTxr { get; private set; }
        public static Texture2D UIPanelBorderTxr { get; private set; }

        // Hud
        public static Texture2D ResourceHUD { get; private set; }
        #endregion
        // Fonts
        public static SpriteFont MonogramFont { get; private set; }
        public static SpriteFont MonogramDescriptionFont { get; private set; }

        #region Sound Effects
        // Buttons
        public static SoundEffect PauseSFX { get; private set; }
        public static SoundEffect ButtonHoverSFX { get; private set; }
        public static SoundEffect ButtonPressSFX { get; private set; }

        // Buildings
        public static SoundEffect BuildingPlacedSFX { get; private set; }
        public static SoundEffect KeenoSpawnSFX { get; private set; }
        public static SoundEffect BuildingUpgradedSFX { get; private set; }
        public static SoundEffect BuildingRemovedSFX { get; private set; }

        
            

        // UI
        public static SoundEffect BlueprintShuffleSFX { get; private set; }
        public static SoundEffect BuySFX { get; private set; }


        // Player
        public static SoundEffect PlayerAddingFollowerSFX { get; private set; }
        public static SoundEffect PlayerDroppingOffFollowerSFX { get; private set; }
        public static SoundEffect Footstep1Sfx { get; private set; }
        public static SoundEffect Footstep2Sfx { get; private set; }





        // Keeno work
        public static SoundEffect WoodCuttingSFX { get; private set; }
        public static SoundEffect StoneCuttingSFX { get; private set; }
        public static SoundEffect ConstructingSFX { get; private set; }
        public static SoundEffect WorkingOnFarmSFX { get; private set; }


        // Resources
        public static SoundEffect WoodDropOffSFX { get; private set; }
        public static SoundEffect FoodDropOffSFX { get; private set; }
        public static SoundEffect StoneDropOffSFX { get; private set; }
        public static SoundEffect ResourceDeliveredSFX { get; private set; }
        public static SoundEffect GoldCoinCollectSFX { get; private set; }
        public static SoundEffect RockBrokenSFX { get; private set; }



        // Music
        public static Song MainThemeSFX { get; private set; }
        public static Song FirstRainSFX { get; private set; }

        // Ambience
        public static SoundEffect ForestDay { get; private set; }
        public static SoundEffect ForestNight { get; private set; }





        #endregion

        public static void Load(ContentManager content)
        {
            #region Textures
            DebugPixelTxr = content.Load<Texture2D>("Pixel");

            // Keeno
            KeenoTxr = content.Load<Texture2D>("Characters\\Keeno");
            WorkerKeenoTxr = content.Load<Texture2D>("Characters\\WorkerKeeno");
            KeenoCarryingTxr = content.Load<Texture2D>("Characters\\KeenoCarrying");


            // TileSets
            TilesetTxr = content.Load<Texture2D>("SpriteSheets\\color_t2");
            MonochromaticTilesetTxr = content.Load<Texture2D>("SpriteSheets\\mono_t");
            InputsTilesetTxr = content.Load<Texture2D>("SpriteSheets\\inputs_t");

            // World Objects
            RockTxr = content.Load<Texture2D>("WorldObjects\\Minerals\\rock1");
            WhiteRockTxr = content.Load<Texture2D>("WorldObjects\\Minerals\\rock1_w");
            GoldOreTxr = content.Load<Texture2D>("WorldObjects\\Minerals\\gold");
            ChoppedTreeTxr = content.Load<Texture2D>("WorldObjects\\Flora\\choppedTree2");
            TentsTxr = content.Load<Texture2D>("WorldObjects\\Buildings\\Houses\\tents");
            TentsWhiteTxr = content.Load<Texture2D>("WorldObjects\\Buildings\\Houses\\tents_w");
            HousesTxr = content.Load<Texture2D>("WorldObjects\\Buildings\\Houses\\houses");
            HousesWhiteTxr = content.Load<Texture2D>("WorldObjects\\Buildings\\Houses\\houses_w");
            FarmLandWhiteTxr = content.Load<Texture2D>("WorldObjects\\Buildings\\FarmLand\\FarmLand");

            BlueprintTxr = content.Load<Texture2D>("WorldObjects\\Items\\scroll");


            // UI Objects
            UIWoodTxr= content.Load<Texture2D>("WorldObjects\\UI Objects\\Log");
            UIStoneTxr = content.Load<Texture2D>("WorldObjects\\UI Objects\\Stone");
            UIFoodTxr = content.Load<Texture2D>("WorldObjects\\UI Objects\\Bread");

            // UI Icons
            UIWoodIconTxr = content.Load<Texture2D>("UI\\Icons\\WoodIcon");
            UIStoneIconTxr = content.Load<Texture2D>("UI\\Icons\\StoneIcon");
            UIFoodIconTxr = content.Load<Texture2D>("UI\\Icons\\FoodIcon");
            UIGoldIconTxr = content.Load<Texture2D>("UI\\Icons\\GoldIcon");
            UIHousingIconTxr = content.Load<Texture2D>("UI\\Icons\\HousingIcon");
            UISwapIconTxr = content.Load<Texture2D>("WorldObjects\\UI Objects\\Swap");


            UIPanelTxr = content.Load<Texture2D>("UI\\Interface\\UIPanel");
            UIPanelBorderTxr = content.Load<Texture2D>("UI\\Interface\\UIPanelBorder");
            UIHighlightTxr = content.Load<Texture2D>("UI\\Interface\\UIHighlight");

            // Hud
            ResourceHUD = content.Load<Texture2D>("UI\\ResourceUI");

            #endregion
            // Font
            MonogramFont = content.Load<SpriteFont>("Fonts\\monogram");
            MonogramDescriptionFont = content.Load<SpriteFont>("Fonts\\monogramDescription");

            #region Sound Effects
            // Buttons
            PauseSFX = content.Load<SoundEffect>("Sounds\\UI\\Buttons\\PauseSFX");
            ButtonHoverSFX = content.Load<SoundEffect>("Sounds\\UI\\Buttons\\ButtonHoverSFX");
            ButtonPressSFX = content.Load<SoundEffect>("Sounds\\UI\\Buttons\\ButtonPressSFX");


            // Player
            PlayerAddingFollowerSFX = content.Load<SoundEffect>("Sounds\\Keeno\\PlayerAddingFollowerSFX");
            PlayerDroppingOffFollowerSFX = content.Load<SoundEffect>("Sounds\\Keeno\\PlayerDroppingOffFollowerSFX");

            // UI
            BlueprintShuffleSFX = content.Load<SoundEffect>("Sounds\\Keeno\\PickupBlueprintSFX");
            BuySFX = content.Load<SoundEffect>("Sounds\\UI\\BuySFX");


            // World Objects
            RockBrokenSFX = content.Load<SoundEffect>("Sounds\\WorldObjects\\RockBrokenSFX");

            //Buildings
            BuildingPlacedSFX = content.Load<SoundEffect>("Sounds\\WorldObjects\\BuildingPlacedSFX");
            KeenoSpawnSFX = content.Load<SoundEffect>("Sounds\\WorldObjects\\KeenoSpawnSFX");
            BuildingUpgradedSFX = content.Load<SoundEffect>("Sounds\\WorldObjects\\BuildingUpgradedSFX");
            BuildingRemovedSFX = content.Load<SoundEffect>("Sounds\\WorldObjects\\BuildingRemovedSFX");


            // Keeno
            WoodDropOffSFX = content.Load<SoundEffect>("Sounds\\Keeno\\WoodDropOffSFX");
            FoodDropOffSFX = content.Load<SoundEffect>("Sounds\\Keeno\\FoodDropOffSFX");
            StoneDropOffSFX = content.Load<SoundEffect>("Sounds\\Keeno\\StoneDropOffSFX");
            ResourceDeliveredSFX = content.Load<SoundEffect>("Sounds\\Keeno\\ResourceDeliveredSFX");
            GoldCoinCollectSFX = content.Load<SoundEffect>("Sounds\\Keeno\\GoldCoinCollectSFX");
            WorkingOnFarmSFX = content.Load<SoundEffect>("Sounds\\Keeno\\WorkingOnFarm");

            


            StoneCuttingSFX = content.Load<SoundEffect>("Sounds\\Keeno\\StoneCuttingLoopSFX");
            WoodCuttingSFX = content.Load<SoundEffect>("Sounds\\Keeno\\WoodCuttingLoopSFX");
            ConstructingSFX = content.Load<SoundEffect>("Sounds\\Keeno\\ConstructingLoopSFX");
            Footstep1Sfx = content.Load<SoundEffect>("Sounds\\Keeno\\Footstep1");
            Footstep2Sfx = content.Load<SoundEffect>("Sounds\\Keeno\\Footstep2");

            // Music
            MainThemeSFX = content.Load<Song>("Sounds\\Music\\MainTheme");
            FirstRainSFX = content.Load<Song>("Sounds\\Music\\FirstRain");

            // Ambience
            ForestDay = content.Load<SoundEffect>("Sounds\\AmbientSounds\\ForestDay");
            ForestNight = content.Load<SoundEffect>("Sounds\\AmbientSounds\\ForestNight");





            #endregion
        }
    }
    public enum ResourceType
    {
        None,
        Food,
        Wood,
        Stone,
        Housing,
        Keeno,
        Gold
    }

    static class ResourceTracker
    {
        public const int KeenoCost = 10;

        // Store resource Type and Amount
        private static readonly Dictionary<ResourceType, int> _amounts;

        //// fired whenever any resource changes
        //public static event Action<ResourceType, int> ResourceChanged;


        static ResourceTracker()
        {
            _amounts = Enum
                .GetValues<ResourceType>()
                .ToDictionary(rt => rt, rt => 0);
        }

        /// <summary>
        /// Returns the current amount of the given resource.
        /// </summary>
        public static int GetAmount(ResourceType type)
            => _amounts[type];

        /// <summary>
        /// Increases the given resource by a positive amount.
        /// </summary>
        public static void Add(ResourceType type, int amount)
        {
            // debugging
            //if (amount <= 0)
            //    throw new ArgumentException(
            //        "Must add a positive amount", nameof(amount));

            _amounts[type] += amount;
            //ResourceChanged?.Invoke(type, _amounts[type]);

            SoundEffectInstance sfx = null;
            switch (type)
            {
                case ResourceType.None:
                case ResourceType.Housing:
                case ResourceType.Keeno:
                    return;
                case ResourceType.Gold:
                    sfx = Assets.GoldCoinCollectSFX.CreateInstance();
                    break;
                case ResourceType.Food:
                    sfx = Assets.FoodDropOffSFX.CreateInstance();
                    sfx.Volume = .6f;
                    break;
                case ResourceType.Wood:
                    sfx = Assets.WoodDropOffSFX.CreateInstance();
                    break;
                case ResourceType.Stone:
                    sfx = Assets.StoneDropOffSFX.CreateInstance();
                    break;
            }
            sfx.Play();
        }

        /// <summary>
        /// Tries to spend (subtract) the given cost from the specified resource.
        /// Returns true if successful, false if insufficient funds.
        /// </summary>
        public static void Spend(ResourceType type, int cost)
        {
            // debugging
            //if (cost <= 0)
            //    throw new ArgumentException(
            //        "Cost must be positive", nameof(cost));

            _amounts[type] -= cost;
            //ResourceChanged?.Invoke(type, _amounts[type]);
        }
        public static bool CanSpend(ResourceType type, int cost)
        {
            if (_amounts[type] < cost)
                return false;

            return true;
        }
        public static bool HasHousingSpace(int cost)
        {
            if (_amounts[ResourceType.Housing] < cost + _amounts[ResourceType.Keeno])
                return false;

            return true;
        }

        /// <summary>
        /// Resets all resources back to zero.
        /// </summary>
        public static void Reset()
        {
            foreach (var key in _amounts.Keys.ToList())
            {
                _amounts[key] = 0;
                _amounts[ResourceType.Housing] = Globals.StartingHousingValue;
                //ResourceChanged?.Invoke(key, 0);
            }
        }
    }
}
