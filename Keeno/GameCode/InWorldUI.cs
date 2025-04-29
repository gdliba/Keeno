using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Keeno
{
    class HourGlass : StaticGraphic
    {
        private float _fill;
        private Color _tint;
        private Color _defaultTint;


        private Rectangle _emptySrcRect;
        private Rectangle _fullSrcRect;

        private Texture2D _spritesheet;


        public HourGlass(Texture2D Spritesheet, Rectangle rect, Color tint)
            : base(rect, Spritesheet)
        {
            _fill = 0f;

            _rect = rect;
            _rect.X = rect.X - 1;
            _rect.Width = rect.Width+1;
            _spritesheet = Spritesheet;

            _emptySrcRect = new Rectangle(Globals.EmptyHourGlassIndex % Globals.TilemapColumns * Globals.Tile_Width_Height,
                                (Globals.EmptyHourGlassIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                                Globals.Tile_Width_Height, Globals.Tile_Width_Height);
            _fullSrcRect = new Rectangle(Globals.FullHourGlassIndex % Globals.TilemapColumns * Globals.Tile_Width_Height,
                               (Globals.FullHourGlassIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                               Globals.Tile_Width_Height, Globals.Tile_Width_Height);
            _tint = tint;
            _defaultTint = tint;
        }
        public bool Update(bool input, float deltaFill)
        {
            if (input)
                return Increment(deltaFill);

            Decrement(deltaFill);
            return false;

        }
        public bool Increment(float deltaFill)
        {
            _tint = _defaultTint;
            if (_fill < 1f)
            {
                _fill += deltaFill;
            }
            else
            {
                _fill = 1f;
                return true;
            }
            return false;
        }
        public void Decrement(float deltaFill)
        {
            _tint = _defaultTint;

            if (_fill > 0 && _fill < 1f)
            {
                _fill -= deltaFill;
            }
            else if (_fill <= 0)
            {
                _fill = 0f;
            }
            else
            {
                _fill = 1f;
            }

        }
        public void Reset()
        {
            _fill = 0f;
        }
        public override void Draw(SpriteBatch sb)
        {
            // Use "yUsed" to increment the appropriate Y-related
            // coordinates/Heights of the following rectanles
            int yUsed;

            Rectangle updatedDrawRect;
            Rectangle updatedSrcRect;

            // move the in world Y AND HEIGHT accordingly
            yUsed = (int)(_rect.Height * _fill);

            // Change the Position in which the "full Hourglass" sprite is drawn
            updatedDrawRect = new Rectangle(_rect.X, _rect.Bottom - yUsed,
                                            _rect.Width, yUsed); 
            // Change the position of the "full Hourglass" sprite's Source Rectangle to
            // draw from the bottom up
            updatedSrcRect = new Rectangle(_fullSrcRect.X, _fullSrcRect.Bottom - yUsed, 
                                            _fullSrcRect.Width, yUsed);
            // Draw the "Filling"
            sb.Draw(_spritesheet, updatedDrawRect, 
                    updatedSrcRect, _tint, 0, Vector2.Zero, SpriteEffects.None, 0.089f);
            // Draw the "Outline"
            //sb.Draw(_spritesheet, _rect, 
            //        _emptySrcRect, Color.White);

        }
    }
    class ButtonPrompt : StaticGraphic
    {

        public ButtonPrompt(Texture2D tileset, Rectangle rect, int tileIndex)
            : base(rect, tileset)
        {
            _rect = rect;
            _txr = tileset;
            _staticSrcRect = new Rectangle(tileIndex % Globals.InputsTilesetColumns * Globals.InputsTileset_Width_Height,
                                (tileIndex / Globals.InputsTilesetColumns) * Globals.InputsTileset_Width_Height,
                                Globals.InputsTileset_Width_Height, Globals.InputsTileset_Width_Height);

            //_emptySrcRect = new Rectangle(Globals.EmptyHourGlassIndex % Globals.TilemapColumns * Globals.Tile_Width_Height,
            //        (Globals.EmptyHourGlassIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
            //        Globals.Tile_Width_Height, Globals.Tile_Width_Height);
        }
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(_txr, new Vector2(_rect.X, _rect.Y), _staticSrcRect, 
                Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, .09f);
        }
    }
}
