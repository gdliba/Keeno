using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Keeno
{
    class StaticSwarmPoint : StaticGraphic
    {

        private Texture2D _testPixel;
        private Rectangle _testRectangle;

        public StaticSwarmPoint(Texture2D txr, int xTile, int yTile, int tileWidth, int tileHeight, Rectangle destinationRect, Texture2D pixel)
            : base(txr, xTile, yTile, tileWidth, tileHeight, destinationRect)
        {
            _testPixel = pixel;
            _testRectangle = destinationRect;
        }

        public override void drawme(SpriteBatch sb)
        {
            base.drawme(sb);
            sb.Draw(_testPixel, _testRectangle, Color.Red * .75f);

        }

    }

    class MobileSwarPoints
    {

    }
}
