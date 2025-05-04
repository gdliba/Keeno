using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Net.Mime.MediaTypeNames;

namespace Keeno
{

    class Map
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


        /// <summary>
        /// takes a CSV path and tile settings, loads the map
        /// </summary>
        /// <param name="csvPath"></param>
        /// <param name="tilesetTexture"></param>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
        /// <param name="tilesetColumns"></param>
        public Map(string csvPath)
        {
            _tileWidth = _tileHeight = Globals.Tile_Width_Height;
            _tilesetColumns = Globals.TilemapColumns;
            _tileset = Assets.TilesetTxr;


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
                            AddTree(x,y);
                            break;
                        case Globals.TownCentreTileIndex:
                            AddTownCentre(x,y);
                            break;
                        case Globals.FarmTileIndex1:
                            AddFarm(x,y);
                            break;
                        case Globals.FarmTileIndex2:
                                AddFarm(x,y);
                            break;
                        case Globals.EmptyTileIndex:
                            AddEmptyTile(x,y);
                            break;
                        case Globals.RockTileIndex:
                            AddRock(x, y);
                            break;
                        case Globals.GoldTileIndex:
                            AddGold(x, y);
                            break;
                        default:
                            AddWorldObject(x,y);
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
        /// <summary>
        /// Determines if the player can move into the specified area without colliding
        /// with any existing world objects that are tagged as Impassable.
        /// </summary>
        /// <param name="destinationRect"></param>
        /// <returns>
        /// TRUE if player movement is permitted
        /// otherwise, FALSE.
        /// </returns>
        public bool IsWalkable(Rectangle destinationRect)
        {    
            // Loop through WorldObjects
            for (int i = 0; i < _worldObjects.Count; i++)
            {
                // Check if destinationRect would interact
                if (destinationRect.Intersects(_worldObjects[i].Bounds)
                    && _worldObjects[i].Impassable)
                    return false;
            }
            return true;
        }
        private void AddWorldObject(int x, int y)
        {
            int index = _mapData[y, x];
            _worldObjects.Add(new WorldObject(new Point(x,y), index));

            _mapData[y, x] = Globals.OccupiedTileIndex;
        }
        private void AddTree(int x, int y)
        {
            _worldObjects.Add(new Tree(new Point(x, y), Globals.TreeTileIndex));

            _mapData[y, x] = Globals.OccupiedTileIndex;
        }
        private void AddFarm(int x, int y)
        {
            _worldObjects.Add(new Farm(new Point(x,y), Globals.FarmTileIndex1));

            _mapData[y, x] = Globals.OccupiedTileIndex;
        }
        private void AddRock(int x, int y)
        {
            _worldObjects.Add(new RockFormation(new Point(x, y), Globals.RockTileIndex));

            _mapData[y, x] = Globals.OccupiedTileIndex;
        }
        private void AddGold(int x, int y)
        {
            _worldObjects.Add(new GoldFromation(new Point(x, y), Globals.GoldTileIndex));

            _mapData[y, x] = Globals.OccupiedTileIndex;
        }
        private void AddTownCentre(int x, int y)
        {
            _worldObjects.Add(new TownCentre(new Point(x, y), Globals.TownCentreTileIndex));

            _mapData[y, x] = Globals.OccupiedTileIndex;
        }
        private void AddEmptyTile(int x, int y)
        {
            _worldObjects.Add(new EmptyTile(new Point(x, y), Globals.EmptyTileIndex));

            _mapData[y, x] = Globals.EmptyTileIndex;
        }

        public void Update(GameTime gt)
        {
            // Test Item spawn
            if (Globals.UpArrow_KeyPress)
                _worldObjects.Add(new BuildingBlueprint(new Point(100, 150), Assets.TentsTxr));


            for (int i = 0; i < _worldObjects.Count; i++)
            {
                // Update WorldObjects
                _worldObjects[i].Update(gt);

                // Remove Dead WorldObjects
                if (_worldObjects[i].State == ObjectState.Dead)
                {
                    int y = _worldObjects[i].TilePosition.Y / Globals.Tile_Width_Height;
                    int x = _worldObjects[i].TilePosition.X / Globals.Tile_Width_Height;

                    // Replace their tile with and Empty one so that the player can build on it
                    AddEmptyTile(x, y);
                    _worldObjects.RemoveAt(i);
                }
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
                    sb.Draw(_tileset, position, sourceRect, Color.White, 0, Vector2.Zero, 1f,SpriteEffects.None,Globals.MapLD);

                    //Draw the tileIndexes on screen
                    //string tempText = tileIndex.ToString();
                    //sb.DrawString(Game1.debugFont, tempText, position +
                    //new Vector2(0, (_tileHeight / 2)), Color.White);
                }
            }
        }
    }
}
