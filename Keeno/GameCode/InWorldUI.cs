using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Text;

namespace Keeno
{
    class HourGlass : StaticGraphic
    {
        private Rectangle _emptySrcRect;
        private Rectangle _fullSrcRect;
        private Texture2D _spritesheet;
        private Texture2D _emptyTxr;
        private Texture2D _fullTxr;
        private float _fill;

        public HourGlass(Texture2D Spritesheet, Rectangle rect)
            : base(rect, Spritesheet)
        {
            _rect = rect;
            _spritesheet = Spritesheet;

            _emptySrcRect = new Rectangle(Globals.EmptyHourGlassIndex % Globals.TilemapColumns * Globals.TileWidth_andHeight,
                                (Globals.EmptyHourGlassIndex / Globals.TilemapColumns) * Globals.TileWidth_andHeight,
                                Globals.TileWidth_andHeight, Globals.TileWidth_andHeight);


            _fullSrcRect = new Rectangle(Globals.FullHourGlassIndex % Globals.TilemapColumns * Globals.TileWidth_andHeight,
                               (Globals.FullHourGlassIndex / Globals.TilemapColumns) * Globals.TileWidth_andHeight,
                               Globals.TileWidth_andHeight, Globals.TileWidth_andHeight);

            _fill = 0f;
        }
        public void Update()
        {
            if (_fill < 1f)
            {
                if (Globals.PickUpKeeno)
                    _fill += .1f;
            }
            else
                _fill = 1f;
        }
        public override void Draw(SpriteBatch sb)
        {
            int yUsed;

            if (_fill > 0)
            {
                // move the in world Y AND HEIGHT accordingly

                yUsed = (int)(_rect.Y * _fill);

                Rectangle updatedRect = new Rectangle(_rect.X, _rect.Y+16,_rect.Width, 0+yUsed); 


                sb.Draw(_txr, updatedRect, _fullSrcRect, Color.White);
            }

            sb.Draw(_spritesheet, _rect, _emptySrcRect, Color.White);
        }
    }
}
