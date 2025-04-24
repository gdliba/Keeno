using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Net.Mime.MediaTypeNames;

namespace Keeno
{
    public class Map
    {

        private List<WorldObject> _worldObjects;
        public List<WorldObject> WorldObjects {  get { return _worldObjects; } }

        // 2D array storing tile indices for the map
        private int[,] _mapData;

        // Dimensions of each tile in pixels
        private int _tileWidth;
        private int _tileHeight;

        // Dimensions of the map in tiles
        private int _mapWidth;
        private int _mapHeight;

        // The tileset texture containing all tile sprites
        private Texture2D _tileset;

        // How many columns (tiles per row) are in the tileset image
        private int _tilesetColumns;

        /////////// TEST PIXEL
        private Texture2D _testPixel;


        /// <summary>
        /// takes a CSV path and tile settings, loads the map
        /// </summary>
        /// <param name="csvPath"></param>
        /// <param name="tilesetTexture"></param>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
        /// <param name="tilesetColumns"></param>
        public Map(string csvPath, Texture2D tilesetTexture, Texture2D monochromaticTilesetTxr, Texture2D inputsTileset, int tileWidth,
            int tileHeight, int tilesetColumns, Texture2D choppedTree, Texture2D testPixel)
        {
            _tileWidth = tileWidth;
            _tileHeight = tileHeight;
            _tileset = tilesetTexture;
            _tilesetColumns = tilesetColumns;
            _testPixel = testPixel;

            // Loads the map data from the CSV
            LoadMap(csvPath);

            _worldObjects = new List<WorldObject>();


            // after LoadMap has filled _mapData
            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    switch (_mapData[y, x])
                    {
                        case Globals.TreeTileIndex:
                            AddTree(x,y, choppedTree, monochromaticTilesetTxr, inputsTileset);
                            break;
                        case Globals.TownCentreTileIndex:
                            AddTownCentre(x,y);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Reads the CSV file and fills _mapData with tile indices
        /// </summary>
        /// <param name="path"></param>
        private void LoadMap(string path)
        {
            // Read entire CSV as a string
            string csv = File.ReadAllText(path);
            // Split into rows
            string[] rows = csv.Split(new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries);

            _mapHeight = rows.Length;                   // Number of tile rows
            _mapWidth = rows[0].Split(',').Length;      // Number of tile columns

            _mapData = new int[_mapHeight, _mapWidth];  // Initialize map array

            // Loop through each row
            for (int y = 0; y < _mapHeight; y++)
            {
                string[] cols = rows[y].Split(',');     // Split row into columns
                for (int x = 0; x < _mapWidth; x++)
                {
                    _mapData[y, x] = int.Parse(cols[x]);// Parse each tile index
                }
            }
        }
        public bool IsWalkable(Rectangle destinationRect)
        {
            // Loop through the map
            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    int tileIndex = _mapData[y, x];             //Store the tile index
                    if (tileIndex == Globals.OccupiedTileIndex)    // if the tile index is 0 (OccupiedTileIndex)
                    {
                        // create a rect to store the tile's bounds
                        Rectangle tileBounds = new Rectangle(x * _tileWidth, y * _tileHeight, _tileWidth, _tileHeight);
                        // check if they intersect
                        if(destinationRect.Intersects(tileBounds))
                            return false;
                    }
                }
            }
            // default to true (as in walkable)
            return true;
        }
        private void AddTree(int x, int y, Texture2D fallenTreeTxr, Texture2D monochromaticTileset, Texture2D inputsTileset)
        {
            _worldObjects.Add(new Tree(_tileset, _tileWidth, _tileHeight,
                                _tilesetColumns, new Point(x, y), fallenTreeTxr, _testPixel, monochromaticTileset, inputsTileset));

            _mapData[y, x] = Globals.OccupiedTileIndex;
        }
        private void AddTownCentre(int x, int y)
        {
            _worldObjects.Add(new TownCentre(_tileset, _tileWidth, _tileHeight,
                                _tilesetColumns, new Point(x, y), _testPixel));

            _mapData[y, x] = Globals.OccupiedTileIndex;
        }

        public void Update(GameTime gt)
        {
            for (int i = 0; i < _worldObjects.Count; i++)
            {
                _worldObjects[i].Update(gt);
            }
        }

        /// <summary>
        /// Draw Method for the class
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < _worldObjects.Count; i++)
            {
                _worldObjects[i].Draw(sb);
            }


            // Loop through the tiles to process their information
            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    int tileIndex = _mapData[y, x]; // Get the tile index
                    //if (tileIndex == Globals.EmptyTileIndex) 
                    //    continue;                   // skip "empty" tiles

                    int col = (tileIndex) % _tilesetColumns;    // X position in the tileset
                    int row = (tileIndex) / _tilesetColumns;    // Y position in the tileset

                    // Adjust source rectangle
                    Rectangle sourceRect = new Rectangle(col * _tileWidth, 
                        row * _tileHeight, _tileWidth, _tileHeight);

                    // Calculate screen position to draw the tile
                    Vector2 position = new Vector2(x * _tileWidth, y * _tileHeight);

                    // Draw the tile from tileset onto screen
                    sb.Draw(_tileset, position, sourceRect, Color.White);

                    //Draw the tileIndexes on screen
                    //string tempText = tileIndex.ToString();
                    //sb.DrawString(Game1.debugFont, tempText, position +
                    //new Vector2(0, (_tileHeight / 2)), Color.White);
                }
            }
        }
    }
}
