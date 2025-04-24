using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Keeno
{
    class HourGlass : StaticGraphic
    {
        private float _fill;

        private Rectangle _emptySrcRect;
        private Rectangle _fullSrcRect;

        private Texture2D _spritesheet;

        public HourGlass(Texture2D Spritesheet, Rectangle rect)
            : base(rect, Spritesheet)
        {
            _fill = 0f;

            _rect = rect;
            _spritesheet = Spritesheet;

            _emptySrcRect = new Rectangle(Globals.EmptyHourGlassIndex % Globals.TilemapColumns * Globals.TileWidth_andHeight,
                                (Globals.EmptyHourGlassIndex / Globals.TilemapColumns) * Globals.TileWidth_andHeight,
                                Globals.TileWidth_andHeight, Globals.TileWidth_andHeight);
            _fullSrcRect = new Rectangle(Globals.FullHourGlassIndex % Globals.TilemapColumns * Globals.TileWidth_andHeight,
                               (Globals.FullHourGlassIndex / Globals.TilemapColumns) * Globals.TileWidth_andHeight,
                               Globals.TileWidth_andHeight, Globals.TileWidth_andHeight);
        }
        public void Update()
        {
            if (_fill < 1f)
            {
                if (Globals.PickUpKeeno)
                    _fill += .01f;
            }
            else
                _fill = 1f;
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
                    updatedSrcRect, Color.White);
            // Draw the "Outline"
            sb.Draw(_spritesheet, _rect, 
                    _emptySrcRect, Color.White);

        }
    }
}
