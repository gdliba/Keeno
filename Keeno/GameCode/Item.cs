using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Keeno
{
    class Item : Animated2D
    {
        public Rectangle Bounds { get { return _rect; } protected set { _rect = value; } }

        protected bool _isSelected;
        protected Texture2D _monochromaticTileset;
        protected Rectangle _blueprintScrRect;
        protected Rectangle _selectedScrRect;

        public Item(Texture2D spriteSheet, int fps, Rectangle rect)
            : base(spriteSheet, fps, rect)
        {
            _isSelected = false;
            _monochromaticTileset = Assets.MonochromaticTilesetTxr;
            _blueprintScrRect =
                new Rectangle(Globals.BlueprintIndex % Globals.TilemapColumns * Globals.Tile_Width_Height,
                                (Globals.BlueprintIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                                Globals.Tile_Width_Height, Globals.Tile_Width_Height);
            _selectedScrRect =
                new Rectangle(Globals.ItemSelectedIndex % Globals.TilemapColumns * Globals.Tile_Width_Height,
                                (Globals.ItemSelectedIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                                Globals.Tile_Width_Height, Globals.Tile_Width_Height);
        }
        public Item (Texture2D UISpriteSheet, int fps, Rectangle rect, Texture2D txr)
            : base(UISpriteSheet, fps, rect)
        {
            _txr = txr;
            _isSelected= false;
            _monochromaticTileset = Assets.MonochromaticTilesetTxr;
            _selectedScrRect =
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
                sb.Draw(_monochromaticTileset, _rect, _selectedScrRect,
                    Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            }
        }
    }
    class BuildingBlueprint : Item
    {
        protected Texture2D _blueprintTxr;
        protected Rectangle _srcRect2, _srcRect3;
        public BuildingBlueprint(Texture2D spriteSheet, int fps, Rectangle rect, Texture2D txr)
            : base(spriteSheet, fps, rect)
        {
            _blueprintTxr = txr;
            _srcRect2 = _srcRect3 = _srcRect;
            _srcRect2.X = _srcRect.X + _srcRect.Width;
            _srcRect3.X = _srcRect2.X + _srcRect.Width;
        }
        public override void Draw(SpriteBatch sb)
        {
            _rect.X = (int)_position.X;
            _rect.Y = (int)_position.Y;

            SelectedDraw(sb);
            sb.Draw(_txr, _rect, _srcRect, Color.Beige);
            sb.Draw(_txr, _rect, _srcRect2, Color.Beige);
            sb.Draw(_txr, _rect, _srcRect3, Color.Beige);
            sb.Draw(_blueprintTxr, _rect, Color.CornflowerBlue);
            //sb.Draw(_monochromaticTileset, _rect, _blueprintScrRect,
            //            Color.CornflowerBlue, 0, Vector2.Zero, SpriteEffects.None, 0);

        }
    }
}
