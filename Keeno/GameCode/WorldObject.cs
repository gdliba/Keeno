using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace Keeno
{
    public abstract class WorldObject
    {
        public Rectangle Bounds { get{ return _rect; } }
        protected Rectangle _rect;
        protected Texture2D _txr;
        protected Rectangle _srcRect;
        protected Point _tilePosition;
        protected int _tileWidth;
        protected int _tileHeight;
        protected int _tilesetColumns;


        protected WorldObject(Texture2D texture, Rectangle bounds, Rectangle sourceRect)
        {
            _txr = texture;
            _rect = bounds;
            _srcRect = sourceRect;
        }


        public virtual void Update(GameTime gt)
        {

        }

        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(_txr, _rect, _srcRect, Color.White);
        }

        /// <summary>
        /// Called when the player “interacts” with this object
        /// </summary>
        public abstract void OnInteract();
    }

    class Tree : WorldObject
    {
        private bool _isChopped;
        private Texture2D _fallenTreeTxr;

        public Tree(Texture2D tileset, int tileWidth, int tileHeight,
            int tilesetColumns, Point tilePosition, Texture2D fallenTreeTxr)
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
                  tileHeight)
              )
        {
            _isChopped = false;
            _tileHeight = tileHeight;
            _tileWidth = tileWidth;
            _tilePosition = tilePosition;
            _tilesetColumns = tilesetColumns;
            _fallenTreeTxr = fallenTreeTxr;
        }

        public override void OnInteract()
        {
            if (!_isChopped)
            {

                // play chop animation / sound
                _isChopped = true;

                _srcRect.X = (Globals.ChoppedTreeTileIndex % _tilesetColumns) * _tileWidth;
                _srcRect.Y = (Globals.ChoppedTreeTileIndex / _tilesetColumns) * _tileHeight;
                // swap SourceRect to a “stump” sprite here
            }
        }

        public override void Draw(SpriteBatch sb)
        {

            if (_isChopped)
            {
                sb.Draw(_fallenTreeTxr, _rect, Color.White);
            }
            else
                base.Draw(sb);
        }
    }
    class TownCentre : WorldObject
    {
        private Color _tint;
        public TownCentre(Texture2D tileset, int tileWidth, int tileHeight,
            int tilesetColumns, Point tilePosition)
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
                  tileHeight)
              )
        {
            _tint = Color.White;
        }
        public override void OnInteract()
        {
            _tint = new Color(Game1.RNG.Next(0, 256), 
                Game1.RNG.Next(0, 256), Game1.RNG.Next(0, 256));
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(_txr, _rect, _srcRect, _tint);
        }
    }
}
