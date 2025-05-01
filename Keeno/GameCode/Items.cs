using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Keeno
{
    class Items : Animated2D
    {
        protected bool _isSelected;
        protected Texture2D _selectedTileTileset;
        protected Rectangle _selectedTileSrcRect;

        public Items(Texture2D spriteSheet, int fps, Rectangle rect)
            : base(spriteSheet, fps, rect)
        {
            _isSelected = true;
            _selectedTileTileset = Assets.MonochromaticTilesetTxr;
            _selectedTileSrcRect =
                new Rectangle(Globals.ItemSelectedIndex % Globals.TilemapColumns * Globals.Tile_Width_Height,
                                (Globals.ItemSelectedIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                                Globals.Tile_Width_Height, Globals.Tile_Width_Height);
        }
        public virtual void Selected()
        {
            _isSelected = true;
        }
        public virtual void SelectedDraw(SpriteBatch sb)
        {
            if (_isSelected)
            {
                if (_isSelected)
                    sb.Draw(_selectedTileTileset, _rect, _selectedTileSrcRect, 
                        Color.White, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
            }
        }
    }
    class BuildingItem : Items
    {
        protected Rectangle _srcRect2, _srcRect3;
        public BuildingItem(Texture2D spriteSheet, int fps, Rectangle rect)
            : base(spriteSheet, fps, rect)
        {
            _srcRect2 = _srcRect3 = _srcRect;
            _srcRect2.X = _srcRect.X + _srcRect.Width;
            _srcRect3.X = _srcRect2.X + _srcRect.Width;
        }
        public override void Draw(SpriteBatch sb)
        {
            _rect.X = (int)_position.X;
            _rect.Y = (int)_position.Y;

            sb.Draw(_txr, _rect, _srcRect, Color.White);
            sb.Draw(_txr, _rect, _srcRect2, Color.White);
            sb.Draw(_txr, _rect, _srcRect3, Color.White);
            SelectedDraw(sb);

        }
    }
}
