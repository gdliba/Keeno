using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace Keeno
{
    public abstract class WorldObject
    {
        protected Rectangle _rect;
        protected Texture2D _txr;
        protected Rectangle _srcRect;
        protected Rectangle _selectedTileSrcRect;
        protected Point _tilePosition;
        protected int _tileWidth;
        protected int _tileHeight;
        protected int _tilesetColumns;
        protected Texture2D _testPixel;

        protected bool _isSelected;
        protected bool _impassable;

        public Color Tint;
        public Rectangle Bounds { get{ return _rect; } }
        public Vector2 Position { get { return new Vector2(_tilePosition.X + _tileWidth / 2, _tilePosition.Y + _tileHeight / 2 - 3); } }


        protected WorldObject(Texture2D texture, Rectangle bounds, Rectangle sourceRect, int tilesetColumns, int tileWidth, int tileHeight, Texture2D testPixel)
        {
            _testPixel = testPixel; 
            _impassable = true;
            _isSelected = false;
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

        }
        public float DistanceTo(Vector2 destination)
        {
            return (destination - Position).Length();
        }

        public virtual void Update(GameTime gt)
        {
            _isSelected = false;
            //Tint = Color.White;
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
        public abstract void OnInteract();
        public virtual void Selected()
        {
            _isSelected = true;
            //Tint = Color.Red;
        }

    }

    class Tree : WorldObject
    {
        private HourGlass _hourglass;
        private ButtonPrompt _buttonPrompt_E;
        private bool _isChopped;
        private Texture2D _fallenTreeTxr;
        private Texture2D _choppedTreeTxr;


        public Tree(Texture2D tileset, int tileWidth, int tileHeight,
            int tilesetColumns, Point tilePosition, Texture2D choppedTree, Texture2D testpixel, Texture2D monochromaticTileset, Texture2D buttonsTileset)
            : base(
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
                  tileHeight), tilesetColumns, tileWidth, tileHeight, testpixel

              )
        {
            _isChopped = false;
            _tileHeight = tileHeight;
            _tileWidth = tileWidth;
            _tilePosition.X = tilePosition.X * tileWidth;
            _tilePosition.Y = tilePosition.Y * tileHeight;
            _tilesetColumns = tilesetColumns;
            _choppedTreeTxr = choppedTree;

            _hourglass = new HourGlass(monochromaticTileset,
                new Rectangle(_tilePosition.X, 
                _tilePosition.Y, 
                tileWidth, 
                tileHeight));

            _buttonPrompt_E = new ButtonPrompt(buttonsTileset,
                new Rectangle(_tilePosition.X,
                _tilePosition.Y,
                _tileWidth,
                _tileHeight),Globals.InputsTilesetIndex_E);

        }

        public override void Selected()
        {
            base.Selected();
            _hourglass.Update();


        }
        public override void OnInteract()
        {
            if (!_isChopped)
            {
                // play chop animation / sound
                _isChopped = true;
            }
        }

        public override void Draw(SpriteBatch sb)
        {

            if (_isChopped)
            {
                sb.Draw(_choppedTreeTxr, _rect, Color.White);
            }
            else
            {
                base.Draw(sb);

                if (_isSelected)
                {
                    _hourglass.Draw(sb);
                    _buttonPrompt_E.Draw(sb);
                }
            }
        }
    }
    class TownCentre : WorldObject
    {
        private Color _tint;
        public TownCentre(Texture2D tileset, int tileWidth, int tileHeight,
            int tilesetColumns, Point tilePosition, Texture2D testpixel)
            : base(
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
                  tileHeight), tilesetColumns, tileWidth, tileHeight, testpixel
              )
        {
            Tint = Color.White;
        }
        public override void OnInteract()
        {
            Tint = new Color(Game1.RNG.Next(0, 256), 
                Game1.RNG.Next(0, 256), Game1.RNG.Next(0, 256));
        }
    }
}
