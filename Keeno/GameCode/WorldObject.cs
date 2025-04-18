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

        /// <summary> Called once per frame to update your object logic (e.g. animations). </summary>
        public virtual void Update(GameTime gt) { }

        /// <summary> Draw this object to the screen. </summary>
        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture, Bounds, SourceRect, Color.White);
        }

        /// <summary> Called when the player “interacts” with this object. </summary>
        public abstract void OnInteract();
    }

    public class Tree : WorldObject
    {
        public bool IsChopped { get; private set; }

        public Tree(Texture2D tileset, int tileWidth, int tileHeight, int tilesetColumns, Point tilePosition)
            : base(
                tileset,
                // world‐space bounds: tilePosition * tileSize
                new Rectangle(tilePosition.X * tileWidth,
                              tilePosition.Y * tileHeight,
                              tileWidth,
                              tileHeight),
                // sourceRect inside the tileset:
                new Rectangle(
                  (51 % tilesetColumns) * tileWidth,
                  (51 / tilesetColumns) * tileHeight,
                  tileWidth,
                  tileHeight)
              )
        {
            IsChopped = false;
        }

        public override void OnInteract()
        {
            if (!IsChopped)
            {
                // play chop animation / sound
                IsChopped = true;

                // swap SourceRect to a “stump” sprite here
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            if (IsChopped)
            {
                // draw stump (could be a different sourceRect or texture)
            }
            else
            {
                base.Draw(sb);
            }
        }
    }
}
