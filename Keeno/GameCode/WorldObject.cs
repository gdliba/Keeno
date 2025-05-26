using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace Keeno
{
    enum ObjectState
    {
        AwaitingResourceDelivery,
        UnderConstruction,
        Neutral,
        Harvestable,
        Broken,
        Dead
    }
    enum BuildingType
    {
        Tent,
        House,
        ResourceStorage,
        FarmLand,
        Bridge
    }
    enum BuildingLevel { One, Two, Three,}
    class WorldObject
    {
        #region Variables
        public ObjectState State { get { return _state; } protected set { _state = value; } }
        protected ObjectState _state;
        protected ResourceType _resourceType;

        protected string _name;
        protected SpriteFont _descriptionFont;

        protected ButtonPrompt _buttonPrompt_E;
        protected ButtonPrompt _buttonPrompt_Q;
        protected ButtonPrompt _buttonPrompt_X;

        protected HourGlass _HGInteract;
        protected HourGlass _HGDropOff;
        protected HourGlass _HGDestroy;
        protected HourGlass _HGWorkProgress;
        protected HourGlass _HGCantInteract;

        protected Texture2D _txr;
        protected Texture2D _selectedTileTileset;
        protected Texture2D _testPixel;

        protected Rectangle _rect;
        protected Rectangle? _srcRect;
        protected Rectangle? _selectedTileSrcRect;
        protected Rectangle _coreRect;
        public Rectangle CoreRect { get { return _coreRect; } protected set { _coreRect = value; } }

        protected Point _tilePosition;
        public Point TilePosition { get { return _tilePosition; } protected set { _tilePosition = value; } }


        protected float _txrRotationDegrees;
        protected float _txrRotationRadians;

        protected int _tileWidth;
        protected int _tileHeight;
        protected int _tilesetColumns;

        protected int _health;
        public int Health { get { return _health; } protected set { _health = value; } }

        protected bool _flipped;
        protected bool _isSelected;
        protected bool _canBeSelectedWhenBroken;
        protected bool _canDropOff;
        protected bool _canUse;
        protected bool _cannotUse;
        protected bool _destroyMe;
        protected bool _impassable;
        protected bool _isDropOffPointActive;
        protected bool _specialInteraction;
        public bool Impassable { get { return _impassable;} protected set { _impassable = value; } }

        protected Color _tint;
        protected Color _fontColour;

        public Rectangle Bounds { get{ return _rect; } protected set { _rect = value; } }
        public Vector2 Position { get { return new Vector2(_rect.X + _tileWidth / 2, _rect.Y + _tileHeight / 2); } }
        #endregion
        public WorldObject(Point tilePosition, int globalTileIndex)
        {
            _fontColour = Color.White;
            _name = "name";
            _descriptionFont = Assets.MonogramDescriptionFont;
            _state = ObjectState.Harvestable;
            _impassable = true;
            _isSelected = false;
            _canDropOff = false;
            _destroyMe = false;
            _canUse = false;
            _cannotUse = false;
            _flipped = false;
            _canBeSelectedWhenBroken = true;
            _isDropOffPointActive = false;
            _specialInteraction = false;


            _testPixel = Assets.DebugPixelTxr; 
            _selectedTileTileset = Assets.MonochromaticTilesetTxr;
            _txr = Assets.TilesetTxr;

            _tilesetColumns = Globals.TilemapColumns;
            _tileWidth = _tileHeight = Globals.Tile_Width_Height;
            _tilePosition.X = tilePosition.X * _tileWidth;
            _tilePosition.Y = tilePosition.Y * _tileHeight;


            _rect = new Rectangle(_tilePosition.X,
                              _tilePosition.Y,
                              Globals.Tile_Width_Height,
                              Globals.Tile_Width_Height);
            _srcRect = new Rectangle(
                  (globalTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (globalTileIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height);
            _tint = Color.White;


            _selectedTileSrcRect = 
                new Rectangle   (Globals.TileSelectedIndex % _tilesetColumns * _tileWidth,
                                (Globals.TileSelectedIndex / _tilesetColumns) * _tileHeight,
                                _tileWidth, _tileHeight);


            _health = 1;
            _txrRotationDegrees = 0f;

            LoadingBarsAndPrompts();
        }
        protected virtual void TextDescription(SpriteBatch sb)
        {
            if (Globals.HidePromtsAndNames)
                return;
            Vector2 textSize = _descriptionFont.MeasureString(_name);
            sb.DrawString(_descriptionFont, _name, new Vector2(_rect.Center.X-textSize.X/2, _rect.Bottom-textSize.Y/4), Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, .099f);
        }
        protected virtual void LoadingBarsAndPrompts()
        {
            #region ButtonPrompts and HG
            _buttonPrompt_E = new ButtonPrompt(Assets.InputsTilesetTxr,
                new Rectangle(_tilePosition.X + _tileWidth / 2,
                _tilePosition.Y - _tileHeight,
                _tileWidth,
                _tileHeight), Globals.InputsTilesetIndex_E);

            _buttonPrompt_Q = new ButtonPrompt(Assets.InputsTilesetTxr,
                new Rectangle(_tilePosition.X - _tileWidth / 2,
                _tilePosition.Y - _tileHeight,
                _tileWidth,
                _tileHeight), Globals.InputsTilesetIndex_Q);

            _buttonPrompt_X = new ButtonPrompt(Assets.InputsTilesetTxr,
                new Rectangle(_tilePosition.X,
                _tilePosition.Y + _tileHeight + 8,
                _tileWidth,
                _tileHeight), Globals.InputsTilesetIndex_X);

            _HGInteract = new HourGlass(Assets.MonochromaticTilesetTxr,
                new Rectangle(_tilePosition.X + _tileWidth / 2,
                _tilePosition.Y - _tileHeight,
                _tileWidth,
                _tileHeight), Color.Yellow);

            _HGDropOff = new HourGlass(Assets.MonochromaticTilesetTxr,
                new Rectangle(_tilePosition.X - _tileWidth / 2,
                _tilePosition.Y - _tileHeight,
                _tileWidth,
                _tileHeight), Color.White);

            _HGWorkProgress = new HourGlass(Assets.MonochromaticTilesetTxr,
                new Rectangle(_tilePosition.X,
                _tilePosition.Y,
                _tileWidth+1,
                _tileHeight),
                Color.Yellow);

            _HGDestroy = new HourGlass(Assets.MonochromaticTilesetTxr,
                new Rectangle(_tilePosition.X,
                _tilePosition.Y + _tileHeight + 8,
                _tileWidth,
                _tileHeight), Color.Red);

            _HGCantInteract = new HourGlass(Assets.MonochromaticTilesetTxr,
                new Rectangle(_tilePosition.X + _tileWidth / 2,
                _tilePosition.Y - _tileHeight,
                _tileWidth,
                _tileHeight), Color.Red);
            #endregion
        }
        public float DistanceTo(Vector2 destination)
        {
            return (destination - Position).Length();
        }
        public bool GetDropOffPointState()
        {
            return _isDropOffPointActive;
        }
        public virtual void Update(GameTime gt)
        {
            // Change the HourGlass positions if propts are hidden
            if (Globals.HidePromtsAndNames)
            {
                _HGInteract.ChangePosition(_HGWorkProgress.Bounds);
                _HGDestroy.ChangePosition(_HGWorkProgress.Bounds);
                _HGDropOff.ChangePosition(_HGWorkProgress.Bounds);
            }
            else
            {
                _HGInteract.DefaultPosition();
                _HGDestroy.DefaultPosition();
                _HGDropOff.DefaultPosition();
            }
            if (!_isSelected)
            {
                _HGInteract.Reset();
                _HGDropOff.Reset();
                _HGDestroy.Reset();
                _HGCantInteract.Reset();
            }
            _isSelected = false;
            _txrRotationRadians = MathHelper.ToRadians(_txrRotationDegrees);

        }
        /// <summary>
        /// Called when the player “interacts” with this object
        /// </summary>
        public virtual void OnInteract()
        {

        }
        public virtual void DestroyMe()
        {
            
            _state = ObjectState.Dead;
        }
        public virtual void DestroyMeAndMyWorkers()
        {

            _state = ObjectState.Dead;
        }
        public virtual void SelectedDraw(SpriteBatch sb)
        {
            //Rectangle temp = new Rectangle(_rect.X + _rect.Width / 16, _rect.Y + _rect.Height / 16, 7 * _rect.Width / 8, 7 * _rect.Height / 8);
            switch (_state)
            {
                case ObjectState.Harvestable:
                    if (_isSelected)
                        sb.Draw(_selectedTileTileset,_rect , _selectedTileSrcRect, Color.White, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
                    break;
                case ObjectState.Neutral:
                    if (_isSelected)
                    {
                        TextDescription(sb);
                        sb.Draw(_selectedTileTileset, _rect, _selectedTileSrcRect, Color.White, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
                    }
                    break;
                case ObjectState.Broken:
                    if (_isSelected && _canBeSelectedWhenBroken)
                        sb.Draw(_selectedTileTileset, _rect, _selectedTileSrcRect, Color.White, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
                    break;
            }
        }
        public virtual void Draw(SpriteBatch sb)
        {
            //sb.Draw(_testPixel, new Vector2(Position.X, Position.Y), Color.Yellow);  // Draw Position


            Vector2 origin = new Vector2(_rect.Width / 2f, _rect.Height / 2f);

            // determine when to flip the sprite (making it look to the RIGHT)
            var flip = _flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            SelectedDraw(sb);
            if (_state != ObjectState.Dead)
            {
                //sb.Draw(_txr, _rect, _srcRect, Tint, _txrRotationRadians, origin, flip, Globals.WolrdObjectLD);
                sb.Draw(_txr, Position, _srcRect, _tint, _txrRotationRadians, origin, 1, flip, Globals.WolrdObjectLD);
                //sb.Draw(_testPixel, Bounds, Color.Red);

                _HGWorkProgress.Draw(sb);
            }
        }
    }
    class SelectableWorldObject : WorldObject
    {
        public SelectableWorldObject(Point position, int globalTileIndex)
            : base(position, globalTileIndex)
        {

        }

        public virtual void Selected()
        {
            _isSelected = true;
        }
    }
    class Item : SelectableWorldObject
    {
        protected bool _isEquipped;
        public Item(Point position, Texture2D txr, int index)
            : base(position,index)
        {
            _isEquipped = false;
            _txr = txr;

            //_tilePosition.X = position.X / _tileWidth;
            //_tilePosition.Y = position.Y / _tileHeight;
            //_rect = new Rectangle(position.X, position.Y, _tileWidth, _tileHeight);

            _impassable = false;
            _selectedTileSrcRect = _srcRect;
                LoadingBarsAndPrompts();
        }
        public virtual void Selected(bool IsConditionMet)
        {
            if(IsConditionMet)
                _isSelected = true;
        }
        public virtual void OnInteract(Point itemCarryPoint)
        {
            _rect.X = itemCarryPoint.X;
            _rect.Y = itemCarryPoint.Y;

        }
        public void FollowPlayer(Point itemCarryPoint)
        {
            _isEquipped = true;
            _rect.X = itemCarryPoint.X;
            _rect.Y = itemCarryPoint.Y;
        }
        public virtual void Place(Rectangle onThisTile)
        {
            _rect = onThisTile;
        }
        public override void Draw(SpriteBatch sb)
        {
            if (_isSelected)
                sb.Draw(_selectedTileTileset, _rect, _selectedTileSrcRect, _tint, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
            //sb.Draw(_testPixel, Bounds, Color.Red * .75f);
            //sb.Draw(_txr, _rect, Color.White);
            sb.Draw(_txr, _rect, null, Color.White, 0f,Vector2.Zero,SpriteEffects.None,.1f);

        }

    }
    class GoldCoin : Item
    {
        public GoldCoin(Point position)
            : base(position, null, Globals.ItemSelectedIndex)
        {
            _txr = Assets.TilesetTxr;

            _srcRect = new Rectangle(
                            (Globals.GoldCoinTileIndex % _tilesetColumns) * _tileWidth,
                            (Globals.GoldCoinTileIndex / _tilesetColumns) * _tileHeight,
                            _tileWidth,
                            _tileHeight);

            _coreRect = new Rectangle(_rect.X + _rect.Width / 4, _rect.Y + _rect.Height / 4, _rect.Width / 2, _rect.Height / 2);

        }
        public void GatherGoldCoin()
        {
            ResourceTracker.Add(ResourceType.Gold, 1);
            _state = ObjectState.Dead;
        }
        public override void Draw(SpriteBatch sb)
        {
            if (_isSelected)
                sb.Draw(_selectedTileTileset, _rect, _selectedTileSrcRect, _tint, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
            //sb.Draw(_testPixel, Bounds, Color.Red * .75f);
            //sb.Draw(_txr, _rect, Color.White);
            sb.Draw(_txr, _rect, _srcRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, .1f);

            //sb.Draw(Assets.DebugPixelTxr, CoreRect, Color.Red);


        }
    }
    class Blueprint : Item
    {
        protected Texture2D _blueprintTxr;
        public Blueprint(Point position, Texture2D txr)
            : base(position, txr, Globals.ItemSelectedIndex)
        {
            _blueprintTxr = Assets.BlueprintTxr;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            sb.Draw(_blueprintTxr, _rect, null, Color.CornflowerBlue, 0f, Vector2.Zero, SpriteEffects.None, .1f);
        }

    }
    class BuildingBlueprint : Blueprint
    {
        public event Action<Building> BuildingSpawned;

        protected List<Rectangle> _stageSrcRects;

        protected BuildingType _buildingType;

        protected Texture2D _buildingTxr;

        protected Building _building;

        public BuildingBlueprint(Point position, BuildingType type)
            : base(position, null)
        {
            _buildingType = type;

            switch (type)
            {
                case BuildingType.Tent:
                    _txr = Assets.TentsWhiteTxr;
                    break;
                case BuildingType.House:
                    _txr = Assets.HousesWhiteTxr;
                    break;
                case BuildingType.ResourceStorage:
                    _txr = Assets.MonochromaticTilesetTxr;
                    _srcRect = new Rectangle(
                  (Globals.ResourceStorageTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.ResourceStorageTileIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height);
                    break;
                case BuildingType.FarmLand:
                    _txr = Assets.MonochromaticTilesetTxr;
                    _srcRect = new Rectangle(
                  (Globals.FarmTileIndex1 % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.FarmTileIndex1/ Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height);
                    break;

            }
            // The BuildingSpritesheet has 3 stages of the building given
            // and is intended to be drawn as 3 sprites one on top of the other
            _stageSrcRects = new List<Rectangle>();
            _stageSrcRects.Add(new Rectangle(0, 0, _rect.Width, _rect.Height));
            _stageSrcRects.Add(new Rectangle(_stageSrcRects[0].X + _rect.Width, 0, _rect.Width, _rect.Height));
            _stageSrcRects.Add(new Rectangle(_stageSrcRects[1].X + _rect.Width, 0, _rect.Width, _rect.Height));
        }
        public override void Place(Rectangle onThisTile)
        {
            Assets.BuildingPlacedSFX.Play();

            switch (_buildingType)
            {
                case BuildingType.Tent:
                    _blueprintTxr = Assets.TentsTxr;
                    break;
                case BuildingType.House:
                    _blueprintTxr = Assets.HousesTxr;
                    break;
                default:
                    break;

            }

            base.Place(onThisTile);
            _building = new Building(new Point(onThisTile.X, onThisTile.Y), _blueprintTxr, _buildingType);
            BuildingSpawned?.Invoke(_building);
            // Remove Blueprint
            _state = ObjectState.Dead;
        }
        public override void Draw(SpriteBatch sb)
        {
            if (_state == ObjectState.Dead)
                return;
            if (_isSelected)
                sb.Draw(_selectedTileTileset, _rect, _selectedTileSrcRect, Color.DeepSkyBlue, 0, Vector2.Zero, SpriteEffects.None, Globals.ItemSelectedTxrLD);

            if(_buildingType == BuildingType.ResourceStorage || _buildingType == BuildingType.FarmLand)
            {
                
                var temp = new Rectangle(_rect.X + _rect.Width / 5, _rect.Y+ _rect.Height / 6, 2*_rect.Width / 3, 2*_rect.Height / 3);
                sb.Draw(_txr, temp, _srcRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.ItemTxrLD);
            }
            else
            {
                // Draw based on level
                for (int i = 0; i < 3; i++)
                {
                    sb.Draw(_txr, _rect, _stageSrcRects[i], Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.ItemTxrLD);
                }
            }
                sb.Draw(_blueprintTxr, _rect, null, Color.RoyalBlue, 0f, Vector2.Zero, SpriteEffects.None, Globals.BlueprintTxrLD);
        }
    }
    class ShopBuildingBlueprint : BuildingBlueprint
    {
        private Texture2D _swapIconTxr;
        private float _flashingFontTimer, _flashingFontTimerReset;
        private int _price;
        public event Action<BuildingBlueprint> BuildingBlueprintPurchaced;

        public ShopBuildingBlueprint(Point position, BuildingType type)
            : base(position, type)
        {
            _buildingType = type;
            _swapIconTxr = Assets.UISwapIconTxr;
            _flashingFontTimer = 0;
            _flashingFontTimerReset = .3f;

            // Set the price
            switch (_buildingType)
            {
                case BuildingType.Tent:
                    _price = Globals.TentBLGoldPrice;
                    break;
                case BuildingType.House:
                    _price = Globals.HouseBLGoldPrice;
                    break;
                case BuildingType.ResourceStorage:
                    _price = Globals.ResourceStorageBLGoldPrice;
                    break;

            }
        }
        public override void Update(GameTime gt)
        {
            // apply the appropriate effect
            if (_flashingFontTimer > 0)
            {
                _flashingFontTimer -= Globals.DeltaTime;
                _fontColour = Color.Red;
            }
            else
            {
                _fontColour = Color.White;
            }
            base.Update(gt);
        }
        public virtual void OnQInteract()
        {
            var sound = Assets.BlueprintShuffleSFX;
            var soundInst = sound.CreateInstance();
            soundInst.Volume = .8f;
            soundInst.Play();

            _buildingType = (BuildingType) ((int) (_buildingType + 1) % (int) BuildingType.Bridge);


            switch (_buildingType)
            {

                case BuildingType.Tent:
                    _txr = Assets.TentsWhiteTxr;
                    _price = Globals.TentBLGoldPrice;
                    break;
                case BuildingType.House:
                    _txr = Assets.HousesWhiteTxr; 
                    _price = Globals.HouseBLGoldPrice;
                    break;
                case BuildingType.ResourceStorage:
                    _txr = Assets.MonochromaticTilesetTxr;
                    _price = Globals.ResourceStorageBLGoldPrice;
                    _srcRect = new Rectangle(
                  (Globals.ResourceStorageTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.ResourceStorageTileIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height);
                    break;
                case BuildingType.FarmLand:
                    _txr = Assets.MonochromaticTilesetTxr;
                    _srcRect = new Rectangle(
                 (Globals.FarmTileIndex1 % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                 (Globals.FarmTileIndex1 / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                 Globals.Tile_Width_Height,
                 Globals.Tile_Width_Height);
                    _price = Globals.FarmLandBLGoldPrice;
                    break;

            }
            return;
        }
        public Item OnInteract(Point itemCarryPoint)
        {
            

            if (ResourceTracker.CanSpend(ResourceType.Gold, _price))
            {
                var sound = Assets.BuySFX;
                var soundInst = sound.CreateInstance();
                soundInst.Volume = .8f;
                soundInst.Play();

                ResourceTracker.Spend(ResourceType.Gold, _price);
                var bp = new BuildingBlueprint(Point.Zero, _buildingType);
                BuildingBlueprintPurchaced?.Invoke(bp);
                bp.OnInteract(itemCarryPoint);
                return bp;
            }
            _flashingFontTimer = _flashingFontTimerReset;
            return null;
        }
        protected void PurchaceUI(SpriteBatch sb)
        {
            string priceText = _price.ToString();
            Vector2 priceTextSize = _descriptionFont.MeasureString(priceText);
            Vector2 priceTextPos = new Vector2(_rect.Right + 16, _rect.Top - 16);

            
            sb.DrawString(_descriptionFont, priceText, priceTextPos, _fontColour, 0f, Vector2.Zero, 1, SpriteEffects.None, .099f);
            Vector2 goldUIPos = new Vector2(priceTextPos.X - 8, priceTextPos.Y + 4);
            sb.Draw(Assets.UIGoldIconTxr, goldUIPos, null, _fontColour, 0f, Vector2.Zero, 1, SpriteEffects.None, Globals.InGameUILD);
        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (_isSelected)
            {
                PurchaceUI(sb);
                if (!Globals.HidePromtsAndNames)
                {
                    _buttonPrompt_E.Draw(sb);
                    _buttonPrompt_Q.Draw(sb);
                    var temp = new Rectangle(_rect.X-6-_tileWidth, _rect.Y-_tileHeight, _rect.Width, _rect.Height);
                    sb.Draw(_swapIconTxr, temp, Color.White);

                }
            }
        } 
    }
    class Building : SelectableWorldObject, IDropOffPoint
    {
        protected Rectangle _riverSrcRect;
        public BuildingType Type { get { return _buildingType; } protected set { _buildingType = value; } }

        protected List<Keeno> _workers;
        protected float _workSpeed;
        protected int _workerSlots;
        protected float _workDuration;
        public Vector2 Position => base.Position;

        public event Action<WorkStation> WorkStationSpawned;
        protected Farm _farm;
        public Farm Farm { get { return _farm; } }

        protected BuildingType _buildingType;

        protected List<Rectangle> _stageSrcRects;
        protected Rectangle _buildingSrcRect;

        protected int _currLevel;

        protected Texture2D _defaultTxr;

        protected bool _canBeUpgraded;
        protected bool _canAffordUpgrade;
        protected bool _toggleUpgrade;
        protected bool _constructionComplete;
        protected bool _singleTxrDraw;

        protected int _populationCountExtention;
        protected int _woodCost, _stoneCost, _woodUpgradeCost, _stoneUpgradeCost;
        protected int _woodDelivered, _stoneDelivered;
        protected int _woodPromissed, _stonePromissed;
        protected int _totalWoodSpent, _totalStoneSpent;
        protected int _goldPrice;


        protected float _flashingFontTimer, _flashingFontTimerReset;

        public Building(Point position, Texture2D BuildingSpriteSheet, BuildingType type)
            :base(position, -1)
        {
            _defaultTxr = _txr = BuildingSpriteSheet;
            _state = ObjectState.AwaitingResourceDelivery;
            _buildingType = type;
            _woodCost = -1;
            _stoneCost = -1;
            _populationCountExtention = 0;
            _woodDelivered = _woodPromissed = 0;
            _stoneDelivered = _stonePromissed = 0;
            _woodUpgradeCost = _stoneUpgradeCost = -1;
            _totalWoodSpent = _totalWoodSpent = 0;
            _workerSlots = 10;
            _workSpeed = 0f; 
            _workDuration = 6f;

            _currLevel = 0;
            _impassable = false;
            _canAffordUpgrade = false;
            _toggleUpgrade = false;
            _canBeUpgraded = true;
            _constructionComplete = false;
            _isDropOffPointActive = false;
            _singleTxrDraw = false;

            _tilePosition.X = position.X;
            _tilePosition.Y = position.Y;

            _rect = new Rectangle(position.X, position.Y, _tileWidth, _tileHeight);
            _workers = new List<Keeno>();

            _stageSrcRects = new List<Rectangle>();
            _stageSrcRects.Add(new Rectangle(0, 0, _rect.Width, _rect.Height));
            _stageSrcRects.Add(new Rectangle(_stageSrcRects[0].X + _rect.Width, 0, _rect.Width, _rect.Height));
            _stageSrcRects.Add(new Rectangle(_stageSrcRects[1].X + _rect.Width, 0, _rect.Width, _rect.Height));

            _riverSrcRect = new Rectangle(
                  (Globals.RiverTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.RiverTileIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height);


            // Apply Resource costs appropriately
            switch (type)
            {
                case BuildingType.Tent:
                    _name = "Tent";
                    _goldPrice = Globals.TentBLGoldPrice;
                    _woodCost = Globals.TentWoodCost;
                    _stoneCost = Globals.TentStoneCost;
                    _populationCountExtention = Globals.TentPopulationAddition;
                    _woodUpgradeCost = Globals.TentUpgradeWoodCost;
                    _stoneUpgradeCost = Globals.TentUpgradeStoneCost;
                    break;
                case BuildingType.House:
                    _name = "House";
                    _goldPrice = Globals.HouseBLGoldPrice;
                    _woodCost = Globals.HouseWoodCost;
                    _stoneCost = Globals.HouseStoneCost;
                    _woodUpgradeCost = Globals.HouseUpgradeWoodCost;
                    _stoneUpgradeCost = Globals.HouseUpgradeStoneCost;
                    _populationCountExtention = Globals.HousePopulationAddition;
                    break;
                case BuildingType.ResourceStorage:
                    _name = "Storage";
                    _goldPrice = Globals.ResourceStorageBLGoldPrice;
                    _currLevel = 2;
                    _singleTxrDraw = true;
                    _txr = Assets.MonochromaticTilesetTxr;

                    _woodCost = Globals.ResourceStorageWoodCost;
                    _stoneCost = Globals.ResourceStorageStoneCost;
                    _woodUpgradeCost = Globals.ResourceStorageUpgradeWoodCost;
                    _stoneUpgradeCost = Globals.ResourceStorageUpgradeStoneCost;
                    _buildingSrcRect = new Rectangle(
                  (Globals.ResourceStorageTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.ResourceStorageTileIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height);
                    break;
                case BuildingType.Bridge:
                    _state = ObjectState.Neutral;
                    _name = "Broken Bridge";
                    _currLevel = 2;
                    _singleTxrDraw = true;
                    _txr = Assets.TilesetTxr;

                    _woodCost = Globals.BridgeWoodCost;
                    _stoneCost = Globals.BridgeStoneCost;
                    _woodUpgradeCost = Globals.BridgeUpgradeWoodCost;
                    _stoneUpgradeCost = Globals.BridgeStoneCost;
                    _buildingSrcRect = new Rectangle(
                  (Globals.BrokenBridgeTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.BrokenBridgeTileIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height);
                    break;
                case BuildingType.FarmLand:
                    _name = "Farm Land";
                    _goldPrice = Globals.FarmLandBLGoldPrice;
                    _currLevel = 2;
                    _singleTxrDraw = true;
                    _txr = Assets.TilesetTxr;

                    _woodCost = Globals.ResourceStorageWoodCost;
                    _stoneCost = Globals.ResourceStorageStoneCost;
                    _woodUpgradeCost = Globals.ResourceStorageUpgradeWoodCost;
                    _stoneUpgradeCost = Globals.ResourceStorageUpgradeStoneCost;
                    _buildingSrcRect = new Rectangle(
                  (Globals.FarmLandTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.FarmLandTileIndex/ Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height);
                    break;

            }
            LoadingBarsAndPrompts();

            _flashingFontTimer = 0f;
            _flashingFontTimerReset = .1f;
        }
        public override void Update(GameTime gt)
        {
            float deltaTime = (float)gt.ElapsedGameTime.TotalSeconds;
            _canBeUpgraded = _currLevel < 3 ? true : false;

            // apply the appropriate effect
            if (_flashingFontTimer > 0)
            {
                _flashingFontTimer -= Globals.DeltaTime;
                _fontColour = Color.Red;
            }
            else
            {
                _fontColour = Color.White;
            }

            _workSpeed = 0f;
            foreach (var worker in _workers)
            {
                if (!worker.IsWalking)  // Only apply when they have arrived at the Workstation
                {
                    // Get the worker's workspeed and apply it to the Workstation
                    float kWorkspeed = worker.GetWorkSpeed();
                    _workSpeed += kWorkspeed;
                }
            }

            // Worker Work Logic
            float workerFill = _workSpeed * (deltaTime / _workDuration);
            _constructionComplete = _HGWorkProgress.Update(true, workerFill);




            switch (_state)
            {
                case ObjectState.Neutral:

                    //ClearWorkerList();
                    if(_buildingType == BuildingType.ResourceStorage)
                        _isDropOffPointActive = true;
                    if (_canBeUpgraded)
                    {
                        if (ResourceTracker.CanSpend(ResourceType.Wood, _woodUpgradeCost)
                            && ResourceTracker.CanSpend(ResourceType.Stone, _stoneUpgradeCost))
                            _canAffordUpgrade = true;
                        else
                            _canAffordUpgrade = false;
                    }
                    if(_toggleUpgrade)
                    {
                        ResetDeliveredResources();
                        _woodCost = _woodUpgradeCost;
                        _stoneCost = _stoneUpgradeCost;
                        _HGInteract.Reset();
                        _HGWorkProgress.Reset();
                        _toggleUpgrade = false;
                        _state = ObjectState.AwaitingResourceDelivery;
                    }
                    if(_buildingType == BuildingType.Bridge && _currLevel == 3)
                    {
                        _name = "Bridge";
                        _impassable = false;
                        break;
                    }
                    // Player Work Logic
                    if (_isSelected)
                    {
                        if (_canBeUpgraded && _canAffordUpgrade)
                        {
                            // Interaction
                            _toggleUpgrade = _HGInteract.Update(Globals.E_KeyDown, Globals.UpgradeInteractSpeed);
                        }
                        else if (Globals.E_KeyDown)
                        {
                            _flashingFontTimer = _flashingFontTimerReset;
                        }
                        if (_buildingType != BuildingType.Bridge)
                            _destroyMe = _HGDestroy.Update(Globals.X_KeyDown, Globals.DestroyInteractSpeed);
                    }
                    if (_buildingType == BuildingType.FarmLand)
                    {
                        _impassable = false;
                        break;
                    }
                    _impassable = true;
                    break;
                case ObjectState.AwaitingResourceDelivery:
                    if (_buildingType != BuildingType.FarmLand)
                        _impassable = true;

                    if (_woodDelivered == _woodCost
                            && _stoneDelivered == _stoneCost)
                        _state = ObjectState.UnderConstruction;

                    // if farm is not null, destroy it
                    _farm?.DestroyMe();
                    _farm = null;
                    break;
                    case ObjectState.UnderConstruction:
                    if (_buildingType != BuildingType.FarmLand)
                        _impassable = true;

                    if (_constructionComplete)
                    {
                        var sound = Assets.BuildingUpgradedSFX;
                        var soundInst = sound.CreateInstance();
                        soundInst.Volume = .4f;
                        soundInst.Play();

                        _totalWoodSpent += _woodDelivered;
                        _totalStoneSpent += _stoneDelivered;
                        ResourceTracker.Add(ResourceType.Housing, _populationCountExtention);
                        ClearWorkerList();
                        _constructionComplete = false;
                        _state = ObjectState.Neutral;

                        if(_buildingType == BuildingType.FarmLand)
                        {
                            if (_farm is null)
                            {
                                int x, y;
                                x = _tilePosition.X / 16;
                                y = _tilePosition.Y / 16;

                                // FARMLAND SPECIFIFC INTERACTION
                                _farm = new Farm(new Point(x,y), Globals.FarmTileIndex1);
                                WorkStationSpawned?.Invoke(_farm);
                            }
                            break;
                        }
                        _currLevel++;
                    }
                    break;
                default:
                    _impassable = false;
                    break;
            }


            if (_destroyMe)
            {
                var sound = Assets.BuildingRemovedSFX;
                var soundInst = sound.CreateInstance();
                soundInst.Volume = .8f;
                soundInst.Play();

                ResourceTracker.Spend(ResourceType.Housing, _currLevel * _populationCountExtention);
                ResourceTracker.Add(ResourceType.Wood, _totalWoodSpent);
                ResourceTracker.Add(ResourceType.Stone, _totalStoneSpent);
                ResourceTracker.Add(ResourceType.Gold, _goldPrice);

                DestroyMe();
            }

            // Tell workers where to go
            foreach (Keeno keeno in _workers)
            {
                var temp = Position.ToPoint();
                temp.X -= 8;
                temp.Y -= 8;
                if (/*keeno.State == KeenoState.Working
                    || */keeno.State == KeenoState.Building)
                    keeno.MoveTo(temp);
            }
            base.Update(gt);

        }
        public override void DestroyMeAndMyWorkers()
        {
            for (int i = 0; i < _workers.Count; i++)
            {
                _workers.RemoveAt(i);
            }
            base.DestroyMeAndMyWorkers();
        }
        private void ResetDeliveredResources()
        {
            _woodDelivered = 0;
            _stoneDelivered = 0;
        }
        public void TakeThisResource(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Wood:
                    _woodDelivered++;
                    _woodPromissed--;
                        break;
                case ResourceType.Stone:
                    _stoneDelivered++;
                    _stonePromissed--;
                    break;
            }
        }
        public void DontTakeThisResource(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Wood:
                    _woodPromissed--;
                    break;
                case ResourceType.Stone:
                    _stonePromissed--;
                    break;
            }
        }
        public ResourceType CheckCosts()
        {
            lock (this)
            {
                // Check if the building requires Wood
                var temp = _woodCost - (_woodDelivered + _woodPromissed);
                if (temp > 0 && ResourceTracker.CanSpend(ResourceType.Wood, 1))
                {
                    _woodPromissed++;
                    return ResourceType.Wood;
                }
                // If no wood is required, Check the stone
                temp = _stoneCost - (_stoneDelivered + _stonePromissed);
                if (temp > 0 && ResourceTracker.CanSpend(ResourceType.Stone, 1))
                {
                    _stonePromissed++;
                    return ResourceType.Stone;
                }
            }
            return ResourceType.None;
        }
        public virtual void Selected(float playerWorkSpeed, bool condition)
        {
            if (_buildingType == BuildingType.Bridge && _currLevel == 3)
                return;
            // if there's a farm on this tile, or the farm is depleated
            if (_farm == null || _farm.State == ObjectState.Broken)
                base.Selected();
            else
                _farm.Selected(playerWorkSpeed, condition);
        }
        #region Workers
        public virtual void ReduceWorkerSlots()
        {
            if (_workerSlots > 0)
                _workerSlots--;
        }
        public virtual void IncreaseWorkerSlots()
        {
            _workerSlots++;
        }
        public virtual void TakeThisWorker(Keeno worker)
        {
            _workers.Add(worker);
            if(_state == ObjectState.UnderConstruction)
                worker.SwitchToBuilding();
            //else
                //worker.SwitchToWorking();
            ReduceWorkerSlots();
        }
        public void ClearWorkerList()
        {
            foreach (var worker in _workers)
            {
                worker.SwitchToWalkingToBuilderCabin();
                IncreaseWorkerSlots();
            }
            _canDropOff = false;
            //_canUse = false;
            _cannotUse = false;
            _HGDropOff.Reset();
            _HGInteract.Reset();
            _HGCantInteract.Reset();
            _HGWorkProgress.Reset();

            _workers.Clear();
        }
        public virtual bool CanDropOffWorker(Keeno worker)
        {
            if (_workerSlots > 0)
            {
                TakeThisWorker(worker);
                return true;
            }
            return false;
        }
        #endregion
        #region Draw Methods
        public override void SelectedDraw(SpriteBatch sb)
        {
            if (_isSelected)
            {

                switch (_state)
                {
                    case ObjectState.AwaitingResourceDelivery:
                        ResourcesNeededUI(sb);
                        break;
                    case ObjectState.Neutral:
                        TextDescription(sb);
                        _HGInteract.Draw(sb);
                        _HGDestroy.Draw(sb);
                        _buttonPrompt_X.Draw(sb);
                        // if the structure isn't upgradable
                        if (_woodUpgradeCost == -1 || _stoneUpgradeCost == -1)
                            break;
                        UpgradeUI(sb);
                        break;
                }

                // draw selected outline
                sb.Draw(_selectedTileTileset, _rect, _selectedTileSrcRect, _tint, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
            }

        }
        protected void ResourcesNeededUI(SpriteBatch sb)
        {
            string woodDelivered = _woodDelivered.ToString();
            Vector2 woodDeliveredTextSize = _descriptionFont.MeasureString(woodDelivered);
            Vector2 woodDeliveredTextPos = new Vector2(_rect.Right + 16, _rect.Top - 16);

            string stoneDeliveredText = _stoneDelivered.ToString();
            Vector2 stoneDeliveredTextSize = _descriptionFont.MeasureString(stoneDeliveredText);
            Vector2 stoneDeliveredTextPos = new Vector2(_rect.Right + 16, woodDeliveredTextPos.Y + woodDeliveredTextSize.Y / 2 + 2);

            if (_woodUpgradeCost > 0)
            {
                sb.DrawString(_descriptionFont, woodDelivered + "/" + _woodCost, woodDeliveredTextPos, _fontColour, 0f, Vector2.Zero, 1, SpriteEffects.None, .099f);
                Vector2 woodUIPos = new Vector2(woodDeliveredTextPos.X - 8, woodDeliveredTextPos.Y + 4);
                sb.Draw(Assets.UIWoodIconTxr, woodUIPos, null, _fontColour, 0f, Vector2.Zero, 1, SpriteEffects.None, Globals.InGameUILD);
            }
            if (_stoneUpgradeCost > 0)
            {
                sb.DrawString(_descriptionFont, stoneDeliveredText + "/" + _stoneCost, stoneDeliveredTextPos, _fontColour, 0f, Vector2.Zero, 1, SpriteEffects.None, .099f);
                Vector2 stoneUIPos = new Vector2(stoneDeliveredTextPos.X - 8, stoneDeliveredTextPos.Y + 5);
                sb.Draw(Assets.UIStoneIconTxr, stoneUIPos, null, _fontColour, 0f, Vector2.Zero, 1, SpriteEffects.None, Globals.InGameUILD);

            }
        }
        protected void UpgradeUI(SpriteBatch sb)
        {
            if (_canBeUpgraded)
            {
                _buttonPrompt_E.Draw(sb);
                string woodUpgradeText = _woodUpgradeCost.ToString();
                Vector2 woodUpgradetextSize = _descriptionFont.MeasureString(woodUpgradeText);
                Vector2 woodUpgradeTextPos = new Vector2(_rect.Right + 16, _rect.Top - 16);

                string stoneUpgradeText = _stoneUpgradeCost.ToString();
                Vector2 stoneUpgradetextSize = _descriptionFont.MeasureString(stoneUpgradeText);
                Vector2 stoneUpgradeTextPos = new Vector2(_rect.Right + 16, woodUpgradeTextPos.Y+ woodUpgradetextSize.Y/2+2);
                
                if(_woodUpgradeCost > 0)
                {
                    sb.DrawString(_descriptionFont, woodUpgradeText, woodUpgradeTextPos, _fontColour, 0f, Vector2.Zero, 1, SpriteEffects.None, .099f);
                    Vector2 woodUIPos = new Vector2(woodUpgradeTextPos.X - 8, woodUpgradeTextPos.Y + 4);
                    sb.Draw(Assets.UIWoodIconTxr, woodUIPos, null, _fontColour, 0f, Vector2.Zero, 1,SpriteEffects.None,Globals.InGameUILD);
                }
                if(_stoneUpgradeCost > 0)
                {
                    sb.DrawString(_descriptionFont, stoneUpgradeText, stoneUpgradeTextPos, _fontColour, 0f, Vector2.Zero, 1, SpriteEffects.None, .099f);
                    Vector2 stoneUIPos = new Vector2(stoneUpgradeTextPos.X - 8, stoneUpgradeTextPos.Y + 5);
                    sb.Draw(Assets.UIStoneIconTxr, stoneUIPos, null, _fontColour, 0f, Vector2.Zero, 1, SpriteEffects.None, Globals.InGameUILD);

                }
            }
        }
        public void UnderConstructionDraw(SpriteBatch sb)
        {
            _txr = Assets.TilesetTxr;
            _srcRect = new Rectangle(
              (Globals.ConstructionSiteTileIndex % _tilesetColumns) * _tileWidth,
              (Globals.ConstructionSiteTileIndex / _tilesetColumns) * _tileHeight,
              _tileWidth,
              _tileHeight);
            sb.Draw(_txr, _rect, _srcRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.BuildingLD);
        }
        public void CurrLevelDraw(SpriteBatch sb)
        {
            _txr = _defaultTxr;
            // Draw based on level
            for (int i = 0; i < _currLevel; i++)
            {
                sb.Draw(_txr, _rect, _stageSrcRects[i], Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.BuildingLD);
            }
        }
        public void SingleTxrDraw(SpriteBatch sb)
        {
            // Draw the River texture
            if (_buildingType == BuildingType.Bridge)
                sb.Draw(_txr, _rect, _riverSrcRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.RiverLD);

            // Draw the fixed bridge
            if (_buildingType == BuildingType.Bridge && _currLevel == 3)
            {
                _buildingSrcRect= new Rectangle(
                  (Globals.FixedBridgeTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.FixedBridgeTileIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height);
                sb.Draw(_txr, _rect, _buildingSrcRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.BuildingLD);
                return;
            }

            sb.Draw(_txr, _rect, _buildingSrcRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.BuildingLD);

        }
        public override void Draw(SpriteBatch sb)
        {
            // Don't draw if dead
            if(_state == ObjectState.Dead)
                return;

            SelectedDraw(sb);
    

            // Draw according to Current Building Type
            switch (_state)
            {
                case ObjectState.AwaitingResourceDelivery:
                    UnderConstructionDraw(sb);
                    break;
                case ObjectState.UnderConstruction:
                    UnderConstructionDraw(sb);
                    _HGWorkProgress.Draw(sb);
                    break;
                default:
                    if (_singleTxrDraw)
                        SingleTxrDraw(sb);
                    else
                        CurrLevelDraw(sb);
                    break;
            }
        }
        #endregion
    }
    class WorkStation : SelectableWorldObject
    {
        #region Variables
        protected List<Keeno> _workers;

        protected SoundEffect _workSFX;
        protected SoundEffect? _workstationDepletedSFX;


        protected Texture2D _tilesetTxr;
        protected Texture2D _whiteTxr;
        protected Texture2D _defaultTxr;

        protected float _workSpeed;
        protected float _workDuration;
        protected float _playerWorkSpeed;
        protected float _flashingTxrTimer;
        protected float _flashingTxrTimerReset;



        protected int _workerSlots;
        protected int _resourceAmount;

        protected bool _diesWhenBroken;
        protected bool _playerHarvestedResource;
        protected bool _workerHarvestedResource;
        protected bool _hasToBeCollected;
        protected bool _selectedCondition;      // in most cases checking if player has followers
        protected bool _flashesWhenHarvested;
        protected bool _brokenByPlayer;         // checking if the player broke the resource
        protected bool _hasPlayedDepletedSFX;






        #endregion
        public WorkStation(Point tilePosition, int globalTileIndex)
            : base
            (tilePosition, globalTileIndex)
        {

            _workers = new List<Keeno>();
            // Default values
            _resourceType = ResourceType.None;
            _resourceAmount = 0;
            _workSpeed = 0f;
            _workerSlots = 1;
            _workDuration = 10f;
            _health = 1;

            _defaultTxr = _txr;
            _whiteTxr = Assets.MonochromaticTilesetTxr;
            _tilesetTxr = Assets.TilesetTxr;

            _playerHarvestedResource = false;
            _canDropOff = false;
            _selectedCondition = false;
            _canBeSelectedWhenBroken = true;
            _flashesWhenHarvested = true;
            _hasToBeCollected = false;
            _diesWhenBroken = false;
            _brokenByPlayer = false;
            _hasPlayedDepletedSFX = false;

            _coreRect = new Rectangle(_rect.X+_rect.Width/4,_rect.Y+_rect.Height/4, _rect.Width/2, _rect.Height/2);


            _flashingTxrTimer = 0;
            _flashingTxrTimerReset = .02f;
        }
        public virtual void Selected(float playerWorkSpeed, bool condition)
        {
            base.Selected();
            _playerWorkSpeed = playerWorkSpeed;
            // in most cases checking if player has followers
            _selectedCondition = condition;
        }
        public override void Update(GameTime gt)
        {
            float deltaTime = (float)gt.ElapsedGameTime.TotalSeconds;

            // if this WorkStation flashes when a unit of resource is harvested
            if (_flashesWhenHarvested)
            {
                // apply the appropriate effect
                if (_flashingTxrTimer > 0)
                {
                    _flashingTxrTimer -= deltaTime;
                    _txr = _whiteTxr;
                }
                else
                {
                    _txr = _defaultTxr;
                    _txrRotationDegrees = 0f;
                }
            }

            _workSpeed = 0f;
            foreach (var worker in _workers)
            {
                if (!worker.IsWalking)  // Only apply when they have arrived at the Workstation
                {
                    // Get the worker's workspeed and apply it to the Workstation
                    float kWorkspeed = worker.GetWorkSpeed();
                    _workSpeed += kWorkspeed;
                }
            }

            switch (_state)
            {
                case ObjectState.Broken:
                    ClearWorkerList();
                    if (_diesWhenBroken)
                    {
                        _state = ObjectState.Dead;
                        break;
                    }
                    if(!_canBeSelectedWhenBroken)
                        break;
                    if (_isSelected)
                    {
                        _destroyMe = _HGDestroy.Update(Globals.X_KeyDown, Globals.DestroyInteractSpeed);
                        _playerHarvestedResource = false;
                        _workerHarvestedResource = false;
                    }
                    break;
                case ObjectState.Harvestable:
                    if (_isSelected)
                    {
                        // Player Work Logic
                        float playerFill = _playerWorkSpeed * (deltaTime / _workDuration);
                        // Interaction
                        _playerHarvestedResource = _HGInteract.Update(Globals.E_KeyDown, playerFill);
                        // worker DropOff
                        if (_workerSlots > 0 && _selectedCondition)
                        {
                            _canDropOff = _HGDropOff.Update(Globals.Q_KeyDown, Globals.DropOffKeenoSpeed);
                        }
                    }
                    // Worker Work Logic
                    float workerFill = _workSpeed * (deltaTime / _workDuration);

                    _workerHarvestedResource = _HGWorkProgress.Update(true, workerFill);

                    // Tell workers where to go
                    foreach (Keeno keeno in _workers)
                    {
                        if(keeno.State == KeenoState.Working)
                            keeno.MoveTo(_tilePosition);
                    }
                    // Harvest Resource
                    if (_playerHarvestedResource)
                    {
                        if (!_hasToBeCollected)
                            PlayerHarvestedResource(_resourceType, _resourceAmount);
                        else
                            PlayerHarvestedResource(ResourceType.None, 0);
                    }
                    // Worker Harvested Resource
                    if (_workerHarvestedResource)
                    {
                        if (!_hasToBeCollected)
                            WorkerHarvestedResource(_resourceType, _resourceAmount);
                        else
                            WorkerBrokeResource();
                    }

                    break;
                    case ObjectState.Neutral:
                    if (_isSelected)
                    {
                        if (_specialInteraction)
                        {
                            DoSpecialInteraction();
                            _canUse = _HGInteract.Update(Globals.E_KeyDown, Globals.NeutralInteractSpeed);

                        }

                        // worker DropOff
                        if (_workerSlots > 0 && _selectedCondition && _state != ObjectState.Broken)
                        {
                            _canDropOff = _HGDropOff.Update(Globals.Q_KeyDown, Globals.DropOffKeenoSpeed);
                        }
                    }

                    // Tell workers where to go
                    foreach (Keeno keeno in _workers)
                    {
                        if (keeno.State == KeenoState.ReadyToBuild)
                            keeno.MoveTo(_tilePosition);
                    }
                    break;
            }
            if (_destroyMe)
                DestroyMe();
            // Health Check
            if (_health == 0)
            {
                if(_workstationDepletedSFX != null)
                {
                    var temp = _workstationDepletedSFX.CreateInstance();
                    temp.Play();
                }
                _hasPlayedDepletedSFX = true;
                _health--;
                _state = ObjectState.Broken;
            }
            // Set selected to false;
            // Reset all HG
            base.Update(gt);
        }
        public virtual void DoSpecialInteraction(){}
        public override void DestroyMeAndMyWorkers()
        {
            for (int i = 0; i < _workers.Count; i++)
            {
                _workers[i].Die();
            }

            base.DestroyMeAndMyWorkers();
        }
        #region Resources/Workers
        public virtual void WorkerHarvestedResource(ResourceType type, int amount)
        {
            for (int i = 0; i < _workers.Count; i++)
            {
                if (!_workers[i].IsWalking)
                {
                    _workers[i].DropOffResources(type, amount);
                    break;
                }
            }

            ApplyHitEffect();
            _health--;
            //ResourceTracker.Add(type, amount);
            _HGWorkProgress.Reset();
            _HGInteract.Reset();

        }
        public virtual void WorkerBrokeResource()
        {
            for (int i = 0; i < _workers.Count; i++)
            {
                if (!_workers[i].IsWalking)
                {
                    _workers[i].SwitchWalkingToIdleSpot();
                    break;
                }
            }

            ApplyHitEffect();
            _health--;
            //ResourceTracker.Add(type, amount);
            _HGWorkProgress.Reset();
            _HGInteract.Reset();

        }
        public virtual void PlayerHarvestedResource(ResourceType type, int amount)
        {
            ApplyHitEffect();
            _health--;
            if (_health == 0)
                _brokenByPlayer = true;
            ResourceTracker.Add(type, amount);
            _HGWorkProgress.Reset();
            _HGInteract.Reset();

        }
        public virtual void ApplyHitEffect()
        {
            if (_flashesWhenHarvested)
            {
                // Apply a slight rotation and reset the flashing timer
                // so that the Workstation flashes when hit
                float rand = Globals.RNG.Next(0, 2) == 0 ? 3 : -3;
                _txrRotationDegrees = rand;
                _flashingTxrTimer = _flashingTxrTimerReset;
            }
        }
        public virtual void ReduceWorkerSlots()
        {
            if (_workerSlots > 0)
                _workerSlots--;
        }
        public virtual void IncreaseWorkerSlots()
        {
            _workerSlots++;
        }
        public virtual void TakeThisWorker(Keeno worker)
        {
            worker.TakeWorkSoundEffect(_workSFX);

            _workers.Add(worker);
            if (_state == ObjectState.Harvestable)
                worker.SwitchToWorking();
            else if(_state == ObjectState.Neutral)
                worker.SwitchToReadyToBuild();
            ReduceWorkerSlots();
        }
        public void ClearWorkerList()
        {
            foreach (var keeno in _workers)
            {
                if(_hasToBeCollected)
                    keeno.SwitchWalkingToIdleSpot();
                else if(keeno.State == KeenoState.Working && !_brokenByPlayer)
                {
                    if(keeno.IsWalking)
                        keeno.DropOffAndIdle(_resourceType, 0);
                    else
                        keeno.DropOffAndIdle(_resourceType, _resourceAmount);
                }
                else if (keeno.State == KeenoState.DroppingOff && !_brokenByPlayer)
                    keeno.DropOffAndIdle(_resourceType, _resourceAmount);
                else if(_brokenByPlayer)
                    keeno.SwitchWalkingToIdleSpot();
                // in case this workstation still exists and can be harvested once more
                // reset the total amount of worker slots to the default
                IncreaseWorkerSlots();
            }
            _canDropOff = false;
            _canUse = false;
            _cannotUse = false;
            _HGDropOff.Reset();
            _HGInteract.Reset();
            _HGCantInteract.Reset();
            _HGWorkProgress.Reset();

            _workers.Clear();
        }
        public virtual bool CanDropOffWorker(Keeno worker)
        {
            if (_canDropOff && _workerSlots > 0
                && _state !=ObjectState.Broken && _state != ObjectState.Dead)
            {
                TakeThisWorker(worker);
                _HGDropOff.Reset();
                return true;
            }
            return false;
        }
        #endregion
        public override void DestroyMe()
        {
            ClearWorkerList();
            // Set state to Dead
            base.DestroyMe();
        }
        public virtual void SetHealthTo(int healthTotal)
        {
            _health = healthTotal;
            _state = ObjectState.Harvestable;
            ChangeTextureBackToDefault();
        }
        public virtual void ChangeTextureToBroken()
        {
      
        }
        public virtual void ChangeTextureBackToDefault()
        {

        }
        public override void Draw(SpriteBatch sb)
        {
            //sb.Draw(_testPixel, _coreRect, Color.Green);
            switch (_state)
            {
                case ObjectState.Dead:
                    return;
                case ObjectState.Harvestable:
                    if (_isSelected)
                    {
                        // HourGlasses
                        _HGInteract.Draw(sb);
                        _HGDropOff.Draw(sb);
                        // Input Promts
                        _buttonPrompt_E.Draw(sb);
                        if(_workerSlots > 0 && _selectedCondition)
                            _buttonPrompt_Q.Draw(sb);
                    }
                    break;
                case ObjectState.Neutral:
                    if (_isSelected)
                    {
                        _HGDropOff.Draw(sb);

                        if (_workerSlots > 0 && _selectedCondition)
                            _buttonPrompt_Q.Draw(sb);
                    }
                    break;
                case ObjectState.Broken:
                    ChangeTextureToBroken();
                    if (!_canBeSelectedWhenBroken)
                        break;
                    if (_isSelected)
                    {
                        _buttonPrompt_X.Draw(sb);
                        _HGDestroy.Draw(sb);
                    }
                    break;
            }
            //sb.Draw(_txr, _rect, _farmLandSrc, Color.White);
            base.Draw(sb);
        }
    }
    #region Resources / Breakables
    class Tree : WorkStation
    {
        private Texture2D _choppedTreeTxr;

        public Tree(Point tilePosition, int globalTileIndex, bool isBroken)
            : base(tilePosition, globalTileIndex)
        {
            _resourceType = ResourceType.Wood;
            _resourceAmount = Globals.TreeWoodAmount;
            _health = Globals.TreeHealth;
            _workerSlots = Globals.TreeWorkerSlots;
            _workDuration = Globals.TreeWorkAmount;

            _choppedTreeTxr = Assets.ChoppedTreeTxr;
            _impassable = true;

            _workSFX = Assets.WoodCuttingSFX;

            if (isBroken)
                Health = 0;
        }
        public override void ChangeTextureToBroken()
        {
            _srcRect = null;
            _txr = _choppedTreeTxr;
        }
    }
    class Farm : WorkStation
    {
        private Rectangle _farmLandSrc;
        public Farm(Point tilePosition, int globalTileIndex) 
            : base(tilePosition, globalTileIndex)
        {
            _workSFX = Assets.WorkingOnFarmSFX;
            _resourceType = ResourceType.Food;
            _resourceAmount = Globals.FarmFoodAmount;
            _health = Globals.FarmHealth;
            _workerSlots = Globals.FarmWorkerSlots;
            _workDuration = Globals.FarmWorkAmount;

            _txr = Assets.TilesetTxr;
            _impassable = false;

            _farmLandSrc = new Rectangle(
                  (Globals.FarmLandTileIndex % _tilesetColumns) * _tileWidth,
                  (Globals.FarmLandTileIndex / _tilesetColumns) * _tileHeight,
                  _tileWidth,
                  _tileHeight);
        }
        public override void Draw(SpriteBatch sb)
        {
            //sb.Draw(_txr, _rect, _farmLandSrc, Color.White, _txrRotationRadians, Vector2.Zero, SpriteEffects.None, Globals.WolrdObjectLD - .001f);

            base.Draw(sb);
        }
        public override void ChangeTextureToBroken()
        {

            _srcRect = new Rectangle(
                  (Globals.HarvestedFarmTileIndex % _tilesetColumns) * _tileWidth,
                  (Globals.HarvestedFarmTileIndex / _tilesetColumns) * _tileHeight,
                  _tileWidth,
                  _tileHeight);
        }
        public override void ChangeTextureBackToDefault()
        {

            _srcRect = new Rectangle(
                  (Globals.FarmTileIndex1 % _tilesetColumns) * _tileWidth,
                  (Globals.FarmTileIndex1 / _tilesetColumns) * _tileHeight,
                  _tileWidth,
                  _tileHeight);
        }
    }
    class RockFormation : WorkStation
    {
        public RockFormation(Point tilePosition, int globalTileIndex, bool isBroken)
            : base(tilePosition, globalTileIndex)
        {
            _workstationDepletedSFX = Assets.RockBrokenSFX;
            _resourceType = ResourceType.Stone;
            _resourceAmount = Globals.RockStoneAmount;
            _health = Globals.RockHealth;
            _workerSlots = Globals.RockWorkerSlots;
            _workDuration = Globals.RockWorkAmount;
            _workSFX = Assets.StoneCuttingSFX;

            _impassable = true;

            _srcRect = null;
            _txr = Assets.RockTxr;
            _defaultTxr = _txr;
            _whiteTxr = Assets.WhiteRockTxr;

            if (isBroken)
                _state = ObjectState.Broken;
        }
        public override void ChangeTextureToBroken()
        {
            _impassable = false ;
            _txr = _tilesetTxr;
            _srcRect = new Rectangle(
                  (Globals.HarvestedRockTileIndex % _tilesetColumns) * _tileWidth,
                  (Globals.HarvestedRockTileIndex / _tilesetColumns) * _tileHeight,
                  _tileWidth,
                  _tileHeight);
        }
    }
    class GoldFromation : WorkStation
    {
        public GoldFromation(Point tilePosition, int globalTileIndex)
            : base(tilePosition, globalTileIndex)
        {
            _workstationDepletedSFX = Assets.RockBrokenSFX;
            _resourceType = ResourceType.Gold;
            _resourceAmount = Globals.GoldGoldAmount;
            _health = Globals.GoldHealth;
            _workerSlots = Globals.GoldWorkerSlots;
            _workDuration = Globals.GoldWorkAmount;
            _workSFX = Assets.StoneCuttingSFX;

            _impassable = true;
            _canBeSelectedWhenBroken = false;
            _flashesWhenHarvested = false;
            _hasToBeCollected = true;

            _txr = Assets.GoldOreTxr;
            _srcRect = null;
        }
        public override void ChangeTextureToBroken()
        {
            _impassable = false;
            _txr = _tilesetTxr;
            _srcRect = new Rectangle(
                  (Globals.GoldCoinTileIndex % _tilesetColumns) * _tileWidth,
                  (Globals.GoldCoinTileIndex / _tilesetColumns) * _tileHeight,
                  _tileWidth,
                  _tileHeight);
        }
        public void GatherGoldCoin()
        {
            PlayerHarvestedResource(_resourceType, _resourceAmount);
            _state = ObjectState.Dead;
        }
    }
    class BreakableWall : WorkStation
    {
        public BreakableWall(Point tilePosition, int globalTileIndex)
            : base(tilePosition, globalTileIndex)
        {
            _workstationDepletedSFX = Assets.RockBrokenSFX;
            _workDuration = Globals.BreakableWallWorkAmount;
            _workerSlots = Globals.BreakableWallWorkerSlots;
            _resourceType = ResourceType.None;
            _resourceAmount = 0;
            _health = Globals.BreakableWallHealth;

            _impassable = true;
            _diesWhenBroken = true;
        }
    }
    interface IDropOffPoint
    {
        public Vector2 Position { get; }
    }
    #endregion
    class TownCentre : SelectableWorldObject, IDropOffPoint 
    {
        private Map _map;
        private List<Keeno> _keenosISpawned;
        public event Action<Keeno> KeenoSpawned;
        public List<Keeno> KeenosISpwaned { get { return _keenosISpawned; } }
        public Vector2 Position => base.Position;

        private float _flashingFontTimer, _flashingFontTimerReset;

        public TownCentre(Point tilePosition, int globalTileIndex, Map map) 
            : base(tilePosition, globalTileIndex)
        {
            _name = "Town Centre";
            _state = ObjectState.Neutral;
            _keenosISpawned = new List<Keeno>();
            _isDropOffPointActive = true;
            _map = map;
            _flashingFontTimer = 0f;
            _flashingFontTimerReset = .1f;

            ResourceTracker.Add(ResourceType.Housing, 5);
        }
        public override void Update(GameTime gt)
        {
            // apply the appropriate effect
            if (_flashingFontTimer > 0)
            {
                _flashingFontTimer -= Globals.DeltaTime;
                _fontColour = Color.Red;
            }
            else
            {
                _fontColour = Color.White;
            }
            
            if (_isSelected)
            {
                if (ResourceTracker.CanSpend(ResourceType.Food,
                ResourceTracker.KeenoCost) && ResourceTracker.HasHousingSpace(1))
                    _canUse = _HGInteract.Update(Globals.E_KeyDown, Globals.NeutralInteractSpeed);
                else if (Globals.E_KeyDown)
                    _flashingFontTimer = _flashingFontTimerReset;
                else 
                    _canUse = false; 
            }
            if (_canUse)
            {
                if (ResourceTracker.CanSpend(ResourceType.Food,
                               ResourceTracker.KeenoCost) && ResourceTracker.HasHousingSpace(1))
                {
                    ResourceTracker.Spend(ResourceType.Food,
                    ResourceTracker.KeenoCost);
                    SpawnKeeno();
                    _fontColour = Color.White;
                }
                _HGInteract.Reset();
            }
            base.Update(gt);
        }
        public override void OnInteract()
        {

        }
        private void SpawnKeeno()
        {
            var SFX = Assets.KeenoSpawnSFX.CreateInstance();
            SFX.Play();


            Rectangle temp = new Rectangle(_rect.X-_rect.Width/2, _rect.Y , 16, 16);

            var newKeeno = new Keeno(Assets.KeenoTxr, 5, temp, Assets.DebugPixelTxr, _map, false);
            _keenosISpawned.Add(newKeeno);
            //Debug.WriteLine("Spawning Keeno: firing event");
            KeenoSpawned?.Invoke(newKeeno);
            ResourceTracker.Add(ResourceType.Keeno, 1);
        }
        public override void Draw(SpriteBatch sb)
        {
            //sb.Draw(_testPixel, Bounds, Color.Red * .75f);

            base.Draw(sb);
            if (_isSelected)
            {
                _HGCantInteract.Draw(sb);
                _HGInteract.Draw(sb);
                KeenoCostDisplay(sb);
            }
        }
        private void KeenoCostDisplay(SpriteBatch sb)
        {
            _buttonPrompt_E.Draw(sb);
            string keenoFoodCostText = ResourceTracker.KeenoCost.ToString();
            Vector2 KeenoFoodCostTextSize = _descriptionFont.MeasureString(keenoFoodCostText);
            Vector2 keenoFoodCostTextPos = new Vector2(_rect.Right + KeenoFoodCostTextSize.X + 10, _rect.Top - 16);
            
            sb.DrawString(_descriptionFont, keenoFoodCostText, keenoFoodCostTextPos, _fontColour, 0f, Vector2.Zero, 1, SpriteEffects.None, .099f);
            Vector2 foodUIPos = new Vector2(keenoFoodCostTextPos.X - 8, keenoFoodCostTextPos.Y + 4);
            sb.Draw(Assets.UIFoodIconTxr, foodUIPos, _fontColour);

            string keenoHousingCostText = "1";
            Vector2 keenoHousingCostTextSize = _descriptionFont.MeasureString(keenoHousingCostText);
            Vector2 keenoHousingCostTextPos = new Vector2(_rect.Right + keenoHousingCostTextSize.X + 10, _rect.Top - 16+ KeenoFoodCostTextSize.Y);

            sb.DrawString(_descriptionFont, keenoHousingCostText, keenoHousingCostTextPos, _fontColour, 0f, Vector2.Zero, 1, SpriteEffects.None, .099f);
            Vector2 housingUIPos = new Vector2(keenoHousingCostTextPos.X - 8, keenoHousingCostTextPos.Y + 5);
            sb.Draw(Assets.UIHousingIconTxr, housingUIPos, _fontColour);

        }
    }
    class BuilderCabin : WorkStation
    {
        public BuilderCabin(Point position, int globalTileIndex)
            : base(position, globalTileIndex)
        {
            _state = ObjectState.Neutral;
            _workerSlots = 10;
            _name = "Builders Cabin";
        }
        public override void Update(GameTime gt)
        {
            foreach (var worker in _workers)
            {
                worker.RememberThisBuilderCabin(Position.ToPoint());
            }
            base.Update(gt);
        }
    }
    class Bell : WorkStation
    {
        public event Action BellRung;
        public Bell(Point position, int globalTileIndex)
            : base(position, globalTileIndex)
        {
            _state = ObjectState.Neutral;
            _workerSlots = 0;
            _name = "Bell";
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
        public override void OnInteract()
        {
            BellRung.Invoke();
        }
        public override void Draw(SpriteBatch sb)
        {
            if (_isSelected)
                _buttonPrompt_E.Draw(sb);
            base.Draw(sb);
        }
    }
    class Shop : SelectableWorldObject
    {
        public Shop(Point position, int globalTileIndex)
            : base(position, globalTileIndex)
        {
            _state = ObjectState.Neutral;
            _name = "Shop";

        }
    }
    #region Tile Property Related
    class Door : WorldObject
    {
        public Door(Point position, int globalTileIndex)
            : base(position, globalTileIndex)
        {
            _impassable = false;
        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            //sb.Draw(_testPixel, Bounds, Color.Red);

        }
    }
    class EmptyTile : SelectableWorldObject
    {
        private bool _rngFoliage;
        public EmptyTile(Point tilePosition, int globalTileIndex) 
            : base(tilePosition, globalTileIndex)
        {
            _impassable = false;


            if (_tilePosition.Y > 11* _tileHeight)
            {
                _rngFoliage = Globals.RNG.Next(3) == 0;
            }
            if (_rngFoliage)
            {
                _tint = _tint * .4f;
                int temp = Globals.RNG.Next(3);
                if (temp == 0)
                    _srcRect = new Rectangle(
                      (Globals.FoliageTileIndex % _tilesetColumns) * _tileWidth,
                      (Globals.FoliageTileIndex / _tilesetColumns) * _tileHeight,
                      _tileWidth,
                      _tileHeight);
                else if (temp == 1)
                    _srcRect = new Rectangle(
                      (Globals.FoliageTileIndex2 % _tilesetColumns) * _tileWidth,
                      (Globals.FoliageTileIndex2 / _tilesetColumns) * _tileHeight,
                      _tileWidth,
                      _tileHeight);
                else if (temp == 2)
                    _srcRect = new Rectangle(
                      (Globals.FoliageTileIndex3 % _tilesetColumns) * _tileWidth,
                      (Globals.FoliageTileIndex3 / _tilesetColumns) * _tileHeight,
                      _tileWidth,
                      _tileHeight);
            }
            else
            { 
                _srcRect = new Rectangle(
                      (Globals.EmptyTileIndex % _tilesetColumns) * _tileWidth,
                      (Globals.EmptyTileIndex / _tilesetColumns) * _tileHeight,
                      _tileWidth,
                      _tileHeight);
            }
        }
        public override void OnInteract()
        {

        }
        public void Die()
        {
            _state = ObjectState.Dead;
            _srcRect = new Rectangle(
                 (Globals.EmptyTileIndex% _tilesetColumns) * _tileWidth,
                 (Globals.EmptyTileIndex/ _tilesetColumns) * _tileHeight,
                 _tileWidth,
                 _tileHeight);
        }
    }
    class OccupiedTile : EmptyTile
    {

        public OccupiedTile(Point tilePosition, int globalTileIndex)
            : base(tilePosition, globalTileIndex)
        {
            _impassable = false;
        }
        public override void OnInteract()
        {

        }
    }
    #endregion
}
