using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Keeno
{
    enum ObjectState
    {
        Default,
        Dead
    }
    class WorldObject
    {
        public ObjectState State { get { return _state; } protected set { _state = value; } }
        protected ObjectState _state;

        protected ButtonPrompt _buttonPrompt_E;
        protected ButtonPrompt _buttonPrompt_Q;
        protected ButtonPrompt _buttonPrompt_X;

        protected HourGlass _HGInteract;
        protected HourGlass _HGDropOff;
        protected HourGlass _HGDestroy;


        protected List<Keeno> _workers;

        protected Texture2D _txr;
        protected Texture2D _testPixel;

        protected Rectangle _rect;
        protected Rectangle _srcRect;
        protected Rectangle _selectedTileSrcRect;

        protected Point _tilePosition;

        protected int _tileWidth;
        protected int _tileHeight;
        protected int _tilesetColumns;

        protected int _health;
        protected int _workerSlots;

        protected bool _isSelected;
        protected bool _canDropOff;
        protected bool _canUse;
        protected bool _destroyMe;
        protected bool _impassable;
        public bool Impassable { get { return _impassable;} protected set { _impassable = value; } }

        protected float _destroySpeed;

        public Color Tint;
        public Rectangle Bounds { get{ return _rect; } protected set { _rect = value; } }
        public Vector2 Position { get { return new Vector2(_tilePosition.X + _tileWidth / 2, _tilePosition.Y + _tileHeight / 2 - 3); } }


        protected WorldObject(Texture2D texture,
            Rectangle bounds,
            Rectangle sourceRect, 
            int tilesetColumns,
            int tileWidth, 
            int tileHeight,
            Texture2D monochromaticTileset,
            Texture2D testPixel)
        {
            _state = ObjectState.Default;
            _testPixel = testPixel; 
            _impassable = true;
            _isSelected = false;
            _canDropOff = false;
            _destroyMe = false;
            _canUse = false;

            _txr = texture;
            _rect = bounds;
            _srcRect = sourceRect;
            Tint = Color.White;
            _tilesetColumns = tilesetColumns;
            _tileWidth = tileWidth;
            _tileHeight = tileHeight;

            _selectedTileSrcRect = 
                new Rectangle   (Globals.TileSelectedIndex % _tilesetColumns * _tileWidth,
                                (Globals.TileSelectedIndex / _tilesetColumns) * _tileHeight,
                                _tileWidth, _tileHeight);

            _workers = new List<Keeno>();
            _workerSlots = 3;
            _health = 1;

            _HGInteract = new HourGlass(monochromaticTileset,
                new Rectangle(_tilePosition.X,
                _tilePosition.Y,
                tileWidth,
                tileHeight));
        }
        public float DistanceTo(Vector2 destination)
        {
            return (destination - Position).Length();
        }
        public virtual void TakeAHit()
        {
            _health--;
        }

        public virtual void Update(GameTime gt)
        {
            _isSelected = false;
            //Tint = Color.White;

            foreach (Keeno keeno in _workers)
            {
                keeno.MoveTo(_tilePosition);
            }

        }

        public virtual void Draw(SpriteBatch sb)
        {
            //sb.Draw(_testPixel, Bounds, Color.Red*.75f);
            if (_isSelected )
                sb.Draw(_txr, _rect, _selectedTileSrcRect, Tint);
            sb.Draw(_txr, _rect, _srcRect, Tint);
        }

        /// <summary>
        /// Called when the player “interacts” with this object
        /// </summary>
        public virtual void OnInteract()
        {

        }
        
        public virtual void Selected(bool IsConditionMet,
            float playerWorkSpeed,
            float dropOffSpeed)
        {
            _isSelected = true;
            //Tint = Color.Red;
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
            _workerSlots--;
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
    }

    //class WorkStation : WorldObject
    //{
    //    protected List<Keeno> _workers;
    //    protected int _workerSlots;

    //    public WorkStation(
    //        Texture2D texture,
    //        Rectangle bounds,
    //        Rectangle sourceRect,
    //        int tilesetColumns,
    //        int tileWidth,
    //        int tileHeight,
    //        Texture2D testPixel
    //    ) : base(texture, bounds, sourceRect, tilesetColumns, tileWidth, tileHeight, testPixel)
    //    {
    //        _workers = new List<Keeno>();
    //        _workerSlots = 5;
    //        _health = 5;
    //    }

    //    public override void OnInteract()
    //    {
    //        Console.WriteLine("WorkStation interacted with!");
    //    }
    //    public virtual void ReduceWorkerSlots()
    //    {
    //        if (_workerSlots > 0)
    //            _workerSlots--;
    //    }
    //    public virtual void IncreaseWorkerSlots()
    //    {
    //        _workerSlots++;
    //    }
    //    public virtual void TakeWorker(Keeno worker)
    //    {
    //        _workers.Add(worker);
    //    }
    //}


    class Tree : WorldObject
    {
        private Texture2D _fallenTreeTxr;
        private Texture2D _choppedTreeTxr;

        private bool _isChopped;
        private bool _canTakeHit;
        private bool _canChop;


        public Tree(Texture2D tileset,
            int tileWidth,
            int tileHeight,
            int tilesetColumns,
            Point tilePosition,
            Texture2D choppedTree, 
            Texture2D testpixel, 
            Texture2D monochromaticTileset,
            Texture2D buttonsTileset
            ): base(
                tileset,
                // world‐space bounds: tilePosition * tileSize
                new Rectangle(tilePosition.X * tileWidth,
                              tilePosition.Y * tileHeight,
                              tileWidth,
                              tileHeight),
                // sourceRect inside the tileset
                new Rectangle(
                  (Globals.TreeTileIndex % tilesetColumns) * tileWidth,
                  (Globals.TreeTileIndex/ tilesetColumns) * tileHeight,
                  tileWidth,
                  tileHeight), tilesetColumns, tileWidth, tileHeight, monochromaticTileset, testpixel

              )
        {
            _state = ObjectState.Default;
            _workerSlots = 1;
            _destroySpeed = .01f;
            _health = Globals.TreeHealth;
            _canChop = false;
            _canTakeHit = false;
            _isChopped = false;
            _canDropOff = false;
            _impassable = true;


            _tileHeight = tileHeight;
            _tileWidth = tileWidth;
            _tilePosition.X = tilePosition.X * tileWidth;
            _tilePosition.Y = tilePosition.Y * tileHeight;
            _tilesetColumns = tilesetColumns;
            _choppedTreeTxr = choppedTree;

            _buttonPrompt_E = new ButtonPrompt(buttonsTileset,
                new Rectangle(_tilePosition.X+_tileWidth/2,
                _tilePosition.Y - _tileHeight,
                _tileWidth,
                _tileHeight), Globals.InputsTilesetIndex_E);

            _buttonPrompt_Q = new ButtonPrompt(buttonsTileset,
                new Rectangle(_tilePosition.X-_tileWidth/2,
                _tilePosition.Y - _tileHeight,
                _tileWidth,
                _tileHeight), Globals.InputsTilesetIndex_Q);

            _buttonPrompt_X = new ButtonPrompt(buttonsTileset,
                new Rectangle(_tilePosition.X,
                _tilePosition.Y + _tileHeight,
                _tileWidth,
                _tileHeight), Globals.InputsTilesetIndex_X);

            _HGInteract = new HourGlass(monochromaticTileset,
                new Rectangle(_tilePosition.X + _tileWidth / 2,
                _tilePosition.Y - _tileHeight,
                _tileWidth,
                _tileHeight));

            _HGDropOff = new HourGlass(monochromaticTileset,
                new Rectangle(_tilePosition.X - _tileWidth / 2,
                _tilePosition.Y - _tileHeight,
                _tileWidth,
                _tileHeight));

            _HGDestroy = new HourGlass(monochromaticTileset,
                new Rectangle(_tilePosition.X,
                _tilePosition.Y + _tileHeight,
                _tileWidth,
                _tileHeight));

            
        }

        public override void Selected(bool playerHasFollowers,
            float playerWorkSpeed,
            float dropOffSpeed)
        {
            base.Selected(playerHasFollowers, playerWorkSpeed, dropOffSpeed);

            _canTakeHit = _HGInteract.Update(Globals.E_KeyDown, playerWorkSpeed);

            if (_isChopped)
                _destroyMe = _HGDestroy.Update(Globals.X_KeyDown, _destroySpeed);
            if (_destroyMe) 
                _state=ObjectState.Dead;
            // Check if player has followers
            // And if there are available workerSlots
            if (playerHasFollowers && _workerSlots > 0)
            {
                _canDropOff = _HGDropOff.Update(Globals.Q_KeyDown, dropOffSpeed);
            }

        }
        public override void OnInteract()
        {
            if (!_isChopped)
            {
                if (_canTakeHit)
                    TakeAHit();
                if (_health == 0)
                    _isChopped = true;
                // play chop animation / sound
            }
        }
        public override void TakeAHit()
        {
            base.TakeAHit();
            _HGInteract.Reset();
        }

        public override void Draw(SpriteBatch sb)
        {

            if (_isChopped)
            {
                sb.Draw(_choppedTreeTxr, _rect, Color.White);
                _HGDestroy.Draw(sb);
                _buttonPrompt_X.Draw(sb);

            }
            else
            {
                base.Draw(sb);

                if (_isSelected)
                {
                    // HourGlasses
                    _HGInteract.Draw(sb);
                    _HGDropOff.Draw(sb);
                    // Input Promts
                    _buttonPrompt_E.Draw(sb);
                    _buttonPrompt_Q.Draw(sb);
                }
            }
        }
    }
    class TownCentre : WorldObject
    {
        private Color _tint;
        private List<Keeno> _keenoInGame;
        public event Action<Keeno> KeenoSpawned;
        public List<Keeno> KeenoInGame { get { return _keenoInGame; } }

        public TownCentre(Texture2D tileset,
            Texture2D monochromaticTileset,
            int tileWidth, 
            int tileHeight,
            int tilesetColumns,
            Point tilePosition,
            Texture2D testpixel,
            Texture2D buttonsTileset
            ) : base(
                tileset,
                // world‐space bounds: tilePosition * tileSize
                new Rectangle(tilePosition.X * tileWidth,
                              tilePosition.Y * tileHeight,
                              tileWidth,
                              tileHeight),
                // sourceRect inside the tileset
                new Rectangle(
                  (Globals.TownCentreTileIndex % tilesetColumns) * tileWidth,
                  (Globals.TownCentreTileIndex / tilesetColumns) * tileHeight,
                  tileWidth,
                  tileHeight), tilesetColumns, tileWidth, tileHeight, monochromaticTileset, testpixel
              )
        {
            _keenoInGame = new List<Keeno>();
            _impassable = true;
            _canUse = false;
            _state = ObjectState.Default;
            Tint = Color.White;

            _tileHeight = tileHeight;
            _tileWidth = tileWidth;
            _tilePosition.X = tilePosition.X * tileWidth;
            _tilePosition.Y = tilePosition.Y * tileHeight;
            _tilesetColumns = tilesetColumns;

            _buttonPrompt_E = new ButtonPrompt(buttonsTileset,
                new Rectangle(_tilePosition.X + _tileWidth / 2,
                _tilePosition.Y - _tileHeight,
                _tileWidth,
                _tileHeight), Globals.InputsTilesetIndex_E);

            _HGInteract = new HourGlass(monochromaticTileset,
                new Rectangle(_tilePosition.X + _tileWidth / 2,
                _tilePosition.Y - _tileHeight,
                _tileWidth,
                _tileHeight));
        }
        public override void Selected(bool playerHasFollowers,
            float playerWorkSpeed,
            float dropOffSpeed)
        {
            base.Selected(playerHasFollowers, playerWorkSpeed, dropOffSpeed);

            _canUse = _HGInteract.Update(Globals.E_KeyDown, playerWorkSpeed);
        }
        public override void OnInteract()
        {
            if (_canUse)
            {
                SpawnKeeno();
                _HGInteract.Reset();
            }
        }
        private void SpawnKeeno()
        {
            var newKeeno = new Keeno(Assets.KeenoTxr, 5, new Rectangle(_tilePosition.X, _tilePosition.Y, 16, 16), Assets.DebugPixel);
            _keenoInGame.Add(newKeeno);
            Debug.WriteLine("Spawning Keeno: firing event");
            KeenoSpawned?.Invoke(newKeeno);
        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if (_isSelected)
            {
                _HGInteract.Draw(sb);
                _buttonPrompt_E.Draw(sb);
            }
        }
    }
}
