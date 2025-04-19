using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace Keeno
{
    public abstract class WorldObject
    {
        public Rectangle Bounds { get; protected set; }
        protected Texture2D Texture;
        protected Rectangle SourceRect;

        protected WorldObject(Texture2D texture, Rectangle bounds, Rectangle sourceRect)
        {
            Texture = texture;
            Bounds = bounds;
            SourceRect = sourceRect;
        }


        public virtual void Update(GameTime gt)
        {

        }

        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture, Bounds, SourceRect, Color.White);
        }

        /// <summary>
        /// Called when the player “interacts” with this object
        /// </summary>
        public abstract void OnInteract();
    }

    class Tree : WorldObject
    {
        private bool _isChopped;

        public Tree(Texture2D tileset, int tileWidth, int tileHeight,
            int tilesetColumns, Point tilePosition)
            : base(
                tileset,
                // world‐space bounds: tilePosition * tileSize
                new Rectangle(tilePosition.X * tileWidth,
                              tilePosition.Y * tileHeight,
                              tileWidth,
                              tileHeight),
                // sourceRect inside the tileset:
                new Rectangle(
                  (Globals.TreeTileIndex % tilesetColumns) * tileWidth,
                  (Globals.TreeTileIndex/ tilesetColumns) * tileHeight,
                  tileWidth,
                  tileHeight)
              )
        {
            _isChopped = false;
        }

        public override void OnInteract()
        {
            if (!_isChopped)
            {
                // play chop animation / sound
                _isChopped = true;

                // swap SourceRect to a “stump” sprite here
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            if (_isChopped)
            {
                // draw stump (could be a different sourceRect or texture)
            }
            else
            {
                base.Draw(sb);
            }
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
                // sourceRect inside the tileset:
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
            sb.Draw(Texture, Bounds, SourceRect, _tint);
        }
    }
}
