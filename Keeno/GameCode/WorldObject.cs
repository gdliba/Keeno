using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Keeno
{
    enum ObjectState
    {
        Harvestable,
        Broken,
        Dead
    }
    enum BuildingType
    {
        Tent
    }
    enum BuildingLevel { One, Two, Three,}
    class WorldObject
    {
        #region Variables
        public ObjectState State { get { return _state; } protected set { _state = value; } }
        protected ObjectState _state;
        protected ResourceType _resourceType;

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

        protected Point _tilePosition;
        public Point TilePosition { get { return _tilePosition; } protected set { _tilePosition = value; } }


        protected float _txrRotationDegrees;
        protected float _txrRotationRadians;

        protected int _tileWidth;
        protected int _tileHeight;
        protected int _tilesetColumns;

        protected int _health;

        protected bool _flipped;
        protected bool _isSelected;
        protected bool _canBeSelectedWhenBroken;
        protected bool _canDropOff;
        protected bool _canUse;
        protected bool _cannotUse;
        protected bool _destroyMe;
        protected bool _impassable;
        public bool Impassable { get { return _impassable;} protected set { _impassable = value; } }

        public Color Tint;
        public Rectangle Bounds { get{ return _rect; } protected set { _rect = value; } }
        public Vector2 Position { get { return new Vector2(_rect.X + _tileWidth / 2, _rect.Y + _tileHeight / 2); } }
        #endregion
        public WorldObject(Point tilePosition, int globalTileIndex)
        {
            _state = ObjectState.Harvestable;
            _impassable = true;
            _isSelected = false;
            _canDropOff = false;
            _destroyMe = false;
            _canUse = false;
            _cannotUse = false;
            _flipped = false;
            _canBeSelectedWhenBroken = true;
            

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
            Tint = Color.White;


            _selectedTileSrcRect = 
                new Rectangle   (Globals.TileSelectedIndex % _tilesetColumns * _tileWidth,
                                (Globals.TileSelectedIndex / _tilesetColumns) * _tileHeight,
                                _tileWidth, _tileHeight);


            _health = 1;
            _txrRotationDegrees = 0f;
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
                _tilePosition.Y + _tileHeight,
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
                _tileWidth,
                _tileHeight),
                Color.Yellow);

            _HGDestroy = new HourGlass(Assets.MonochromaticTilesetTxr,
                new Rectangle(_tilePosition.X,
                _tilePosition.Y + _tileHeight,
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
        public virtual void Update(GameTime gt)
        {
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
        public virtual void SelectedDraw(SpriteBatch sb)
        {

            //Rectangle temp = new Rectangle(_rect.X + _rect.Width / 16, _rect.Y + _rect.Height / 16, 7 * _rect.Width / 8, 7 * _rect.Height / 8);
            switch (_state)
            {
                case ObjectState.Harvestable:
                    if (_isSelected)
                        sb.Draw(_selectedTileTileset,_rect , _selectedTileSrcRect, Tint, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
                    break;
                case ObjectState.Broken:
                    if (_isSelected && _canBeSelectedWhenBroken)
                        sb.Draw(_selectedTileTileset, _rect, _selectedTileSrcRect, Tint, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
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
                sb.Draw(_txr, Position, _srcRect, Tint, _txrRotationRadians, origin, 1, flip, Globals.WolrdObjectLD);

                _HGWorkProgress.Draw(sb);
            }
        }
    }
    class Door : WorldObject
    {
        public Door(Point position, int globalTileIndex)
            : base(position, globalTileIndex)
        {
            _impassable = false;
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
            _rect = new Rectangle(position.X, position.Y, _tileWidth, _tileHeight);

            _impassable = false;
            _selectedTileSrcRect = _srcRect;
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
                sb.Draw(_selectedTileTileset, _rect, _selectedTileSrcRect, Tint, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
            //sb.Draw(_testPixel, Bounds, Color.Red * .75f);
            //sb.Draw(_txr, _rect, Color.White);
            sb.Draw(_txr, _rect, null, Color.White, 0f,Vector2.Zero,SpriteEffects.None,.1f);

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

        protected Rectangle _stage1SrcRect, _stage2SrcRect, _stage3SrcRect;
        protected Building _building;
        public BuildingBlueprint(Point position, Texture2D buildingSpritesheet)
            : base(position, buildingSpritesheet)
        {
            // The BuildingSpritesheet has 3 stages of the building given
            // and is intended to be drawn as 3 sprites one on top of the other
            _stage1SrcRect = _stage2SrcRect = _stage3SrcRect = new Rectangle(0, 0, _rect.Width, _rect.Height);
            _stage2SrcRect.X = _stage1SrcRect.X + _rect.Width;
            _stage3SrcRect.X = _stage2SrcRect.X + _rect.Width;
        }
        public override void Place(Rectangle onThisTile)
        {
            base.Place(onThisTile);
            _building = new Building(new Point(onThisTile.X, onThisTile.Y), Assets.TentsTxr);
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
            sb.Draw(_txr, _rect, _stage1SrcRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.ItemTxrLD);
            sb.Draw(_txr, _rect, _stage2SrcRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.ItemTxrLD);
            sb.Draw(_txr, _rect, _stage3SrcRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.ItemTxrLD);
            sb.Draw(_blueprintTxr, _rect, null, Color.RoyalBlue, 0f, Vector2.Zero, SpriteEffects.None, Globals.BlueprintTxrLD);
        }
    }
    class Building : SelectableWorldObject
    {

        protected List<Rectangle> _stageSrcRects;
        protected int _currLevel;

        public Building(Point position, Texture2D BuildingSpriteSheet)
            :base(position, -1)
        {
            _rect = new Rectangle(position.X, position.Y, _tileWidth, _tileHeight);

            _txr = BuildingSpriteSheet;
            _currLevel = 1;
            _stageSrcRects = new List<Rectangle>();
            _stageSrcRects.Add(new Rectangle(0, 0, _rect.Width, _rect.Height));
            _stageSrcRects.Add(new Rectangle(_stageSrcRects[0].X + _rect.Width, 0, _rect.Width, _rect.Height));
            _stageSrcRects.Add(new Rectangle(_stageSrcRects[1].X + _rect.Width, 0, _rect.Width, _rect.Height));

            // The BuildingSpritesheet has 3 stages of the building given
            //// and is intended to be drawn as 3 sprites one on top of the other
            //_stageSrcRects[0] = _stageSrcRects[1] = _stageSrcRects[2] = new Rectangle(0, 0, _rect.Width, _rect.Height);
            //_stageSrcRects[1] = new Rectangle (_stageSrcRects[0].X + _rect.Width, 0, _rect.Width, _rect.Height);
            //_stageSrcRects[2] = new Rectangle(_stageSrcRects[1].X + _rect.Width, 0, _rect.Width, _rect.Height);
        }
        public override void Draw(SpriteBatch sb)
        {
            SelectedDraw(sb);
            // Draw based on level
            for (int i = 0; i < _currLevel; i++)
            {
                sb.Draw(_txr, _rect, _stageSrcRects[i], Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.WolrdObjectLD);
            }
        }
    }

    class WorkStation : SelectableWorldObject
    {
        protected List<Keeno> _workers;

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



        protected Rectangle _coreRect;
        public Rectangle CoreRect { get { return _coreRect; } protected set { _coreRect = value; } }


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

            _coreRect = new Rectangle(_rect.X+_rect.Width/4,_rect.Y+_rect.Height/4, _rect.Width/2, _rect.Height/2);


            _flashingTxrTimer = 0;
            _flashingTxrTimerReset = .02f;

            #region ButtonPrompts and HG
            //_buttonPrompt_E = new ButtonPrompt(Assets.InputsTilesetTxr,
            //    new Rectangle(_tilePosition.X + _tileWidth / 2,
            //    _tilePosition.Y - _tileHeight,
            //    _tileWidth,
            //    _tileHeight), Globals.InputsTilesetIndex_E);

            //_buttonPrompt_Q = new ButtonPrompt(Assets.InputsTilesetTxr,
            //    new Rectangle(_tilePosition.X - _tileWidth / 2,
            //    _tilePosition.Y - _tileHeight,
            //    _tileWidth,
            //    _tileHeight), Globals.InputsTilesetIndex_Q);

            //_buttonPrompt_X = new ButtonPrompt(Assets.InputsTilesetTxr,
            //    new Rectangle(_tilePosition.X,
            //    _tilePosition.Y + _tileHeight,
            //    _tileWidth,
            //    _tileHeight), Globals.InputsTilesetIndex_X);

            //_HGInteract = new HourGlass(Assets.MonochromaticTilesetTxr,
            //    new Rectangle(_tilePosition.X + _tileWidth / 2,
            //    _tilePosition.Y - _tileHeight,
            //    _tileWidth,
            //    _tileHeight), Color.Yellow);

            //_HGDropOff = new HourGlass(Assets.MonochromaticTilesetTxr,
            //    new Rectangle(_tilePosition.X - _tileWidth / 2,
            //    _tilePosition.Y - _tileHeight,
            //    _tileWidth,
            //    _tileHeight), Color.White);

            //_HGDestroy = new HourGlass(Assets.MonochromaticTilesetTxr,
            //    new Rectangle(_tilePosition.X,
            //    _tilePosition.Y + _tileHeight,
            //    _tileWidth,
            //    _tileHeight), Color.Red);

            //_HGWorkProgress = new HourGlass(Assets.MonochromaticTilesetTxr,
            //    new Rectangle(_tilePosition.X,
            //    _tilePosition.Y,
            //    _tileWidth,
            //    _tileHeight),
            //    Color.Yellow);
            #endregion
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
                        if (_workerSlots > 0 && _selectedCondition && _state != ObjectState.Broken)
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
                            PlayerHarvestedResource(_resourceType, 0);
                    }
                    // Worker Harvested Resource
                    if (_workerHarvestedResource)
                    {
                        if (!_hasToBeCollected)
                            WorkerHarvestedResource(_resourceType, _resourceAmount);
                        else
                            WorkerHarvestedResource(_resourceType, 0);
                    }

                    break;
            }
            if (_destroyMe)
                DestroyMe();
            // Health Check
            if (_health == 0)
            {
                _health--;
                _state = ObjectState.Broken;
            }
            // Set selected to false;
            // Reset all HG
            base.Update(gt);
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
            _workers.Add(worker);
            worker.SwitchToWorking();
            ReduceWorkerSlots();
        }
        public void ClearWorkerList()
        {
            foreach (var keeno in _workers)
            {
                if(_hasToBeCollected)
                    keeno.DropOffAndIdle(_resourceType, 0);
                else if(keeno.State == KeenoState.Working && !_brokenByPlayer)
                {
                    if(keeno.IsWalking)
                        keeno.DropOffAndIdle(_resourceType, 0);
                    else
                        keeno.DropOffAndIdle(_resourceType, _resourceAmount);
                }
                else if (keeno.State == KeenoState.DroppingOff)
                    keeno.DropOffAndIdle(_resourceType, _resourceAmount);
                else if(_brokenByPlayer)
                    keeno.DropOffAndIdle(_resourceType, 0);
            }
            _workers.Clear();
        }
        public virtual bool CanDropOffWorker(Keeno worker)
        {
            if (_canDropOff && _workerSlots > 0)
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
        public virtual void ChangeTextureToBroken()
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

    class Tree : WorkStation
    {
        private Texture2D _choppedTreeTxr;

        public Tree(Point tilePosition, int globalTileIndex)
            : base(tilePosition, globalTileIndex)
        {
            _resourceType = ResourceType.Wood;
            _resourceAmount = Globals.TreeWoodAmount;
            _health = Globals.TreeHealth;
            _workerSlots = Globals.TreeWorkerSlots;
            _workDuration = Globals.TreeWorkAmount;

            _choppedTreeTxr = Assets.ChoppedTreeTxr;
            _impassable = true;
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
            _resourceType = ResourceType.Food;
            _resourceAmount = Globals.FarmFoodAmount;
            _health = Globals.FarmHealth;
            _workerSlots = Globals.FarmWorkerSlots;
            _workDuration = Globals.FarmWorkAmount;

            _txr = Assets.TilesetTxr;
            _impassable = false;
        }
        public override void ChangeTextureToBroken()
        {

            _srcRect = new Rectangle(
                  (Globals.HarvestedFarmTileIndex % _tilesetColumns) * _tileWidth,
                  (Globals.HarvestedFarmTileIndex / _tilesetColumns) * _tileHeight,
                  _tileWidth,
                  _tileHeight);
        }
    }
    class RockFormation : WorkStation
    {
        public RockFormation(Point tilePosition, int globalTileIndex)
            : base(tilePosition, globalTileIndex)
        {
            _resourceType = ResourceType.Stone;
            _resourceAmount = Globals.RockStoneAmount;
            _health = Globals.RockHealth;
            _workerSlots = Globals.RockWorkerSlots;
            _workDuration = Globals.RockWorkAmount;

            _impassable = true;

            _srcRect = null;
            _txr = Assets.RockTxr;
            _defaultTxr = _txr;
            _whiteTxr = Assets.WhiteRockTxr;
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
            _resourceType = ResourceType.Gold;
            _resourceAmount = Globals.GoldGoldAmount;
            _health = Globals.GoldHealth;
            _workerSlots = Globals.GoldWorkerSlots;
            _workDuration = Globals.GoldWorkAmount;

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
                  (Globals.HarvestedGoldTileIndex % _tilesetColumns) * _tileWidth,
                  (Globals.HarvestedGoldTileIndex / _tilesetColumns) * _tileHeight,
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
    class TownCentre : SelectableWorldObject, IDropOffPoint 
    {
        private Map _map;
        private List<Keeno> _keenosISpawned;
        public event Action<Keeno> KeenoSpawned;
        public List<Keeno> KeenosISpwaned { get { return _keenosISpawned; } }
        public Vector2 Position => base.Position;
            
        public TownCentre(Point tilePosition, int globalTileIndex, Map map) 
            : base(tilePosition, globalTileIndex)
        {
            _keenosISpawned = new List<Keeno>();
            _map = map;
        }
        public override void Update(GameTime gt)
        {

            if (_isSelected)
            {
                if (ResourceTracker.CanSpend(ResourceType.Food,
                ResourceTracker.KeenoCost))
                    _canUse = _HGInteract.Update(Globals.E_KeyDown, Globals.NeutralInteractSpeed);
                else
                    _canUse = _HGCantInteract.Update(Globals.E_KeyDown, Globals.NeutralInteractSpeed);
            }
            if (_canUse)
            {
                if (ResourceTracker.TrySpend(ResourceType.Food,
                ResourceTracker.KeenoCost))
                {
                    SpawnKeeno();
                }

                _HGInteract.Reset();
                _HGCantInteract.Reset();
            }
            base.Update(gt);
        }
        public override void OnInteract()
        {

        }
        private void SpawnKeeno()
        {
            Rectangle temp = new Rectangle(_rect.X-_rect.Width/2, _rect.Y , 16, 16);

            var newKeeno = new Keeno(Assets.KeenoTxr, 5, temp, Assets.DebugPixelTxr, _map);
            _keenosISpawned.Add(newKeeno);
            //Debug.WriteLine("Spawning Keeno: firing event");
            KeenoSpawned?.Invoke(newKeeno);
        }
        public override void Draw(SpriteBatch sb)
        {
            //sb.Draw(_testPixel, Bounds, Color.Red * .75f);

            base.Draw(sb);
            if (_isSelected)
            {
                _HGCantInteract.Draw(sb);
                _HGInteract.Draw(sb);
                _buttonPrompt_E.Draw(sb);
            }
        }
    }
    class EmptyTile : SelectableWorldObject
    {

        public EmptyTile(Point tilePosition, int globalTileIndex) 
            : base(tilePosition, globalTileIndex)
        {
            _impassable = false;
        }
        public override void OnInteract()
        {

        }
    }
}
