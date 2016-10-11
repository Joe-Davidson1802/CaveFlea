using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace CaveFlea
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        int p1Wins = 0;
        int p2Wins = 0;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private MapHandler map;
        private Texture2D ground;
        private Texture2D wall;
        private Texture2D player;
        private Texture2D player2;
        private Vector2 PlayerPos;
        private Vector2 Player2Pos;
        private int JumpCount = 0;
        private int JumpCount2 = 0;
        private SpriteFont pixelFont;
        private List<Keys> keysPressed = new List<Keys>();

        public Game1()
        {

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            map = new MapHandler();
            map.MakeCaverns();
            map.MakeCaverns();
            map.MakeCaverns();
            map.MakeCaverns();
            graphics.PreferredBackBufferHeight = map.MapHeight * 5;
            graphics.PreferredBackBufferWidth = map.MapWidth * 5;
            graphics.ApplyChanges();


            Random r = new Random();

            var x = r.Next(2, map.MapWidth - 2);
            var y = map.MapHeight - 1;

            while (map.Map[x, y] != 0)
            {
                x = r.Next(2, map.MapWidth - 2);
            }
            PlayerPos = new Vector2(x, y);
            map.Map[(int)PlayerPos.X, (int)PlayerPos.Y] = 2;
            do
            {
                x = r.Next(2, map.MapWidth - 2);
            }
            while (map.Map[x, y] != 0 || map.Map[x, y] == 2);
                Player2Pos = new Vector2(x, y);
            map.Map[(int)Player2Pos.X, (int)Player2Pos.Y] = 3;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            player = Content.Load<Texture2D>("player");
            player2 = Content.Load<Texture2D>("player2");
            ground = Content.Load<Texture2D>("ground");
            wall = Content.Load<Texture2D>("wall");
            pixelFont = Content.Load<SpriteFont>("Pixel");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            player.Dispose();
            player2.Dispose();
            ground.Dispose();
            wall.Dispose();
        }

        void Regen()
        {
            map = new MapHandler();
            map.MakeCaverns();
            map.MakeCaverns();
            map.MakeCaverns();
            map.MakeCaverns();

            Random r = new Random();

            var x = r.Next(2, map.MapWidth - 2);
            var y = map.MapHeight - 1;

            while (map.Map[x, y] != 0)
            {
                x = r.Next(2, map.MapWidth - 2);
            }
            PlayerPos = new Vector2(x, y);
            map.Map[(int)PlayerPos.X, (int)PlayerPos.Y] = 2;

            do
            {
                x = r.Next(2, map.MapWidth - 2);
            }
            while (map.Map[x, y] != 0 || map.Map[x, y] == 2);
            Player2Pos = new Vector2(x, y);
            map.Map[(int)Player2Pos.X, (int)Player2Pos.Y] = 3;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            keysPressed = keysPressed.Distinct().ToList();
            KeyboardState state = Keyboard.GetState();
            for (int i = 0; i < keysPressed.Count; i++)
            {
                if (state.IsKeyUp(keysPressed[i]))
                {
                    keysPressed.Remove(keysPressed[i]);
                }
            }
            foreach (var key in state.GetPressedKeys())
            {
                keysPressed.Add(key);
            }
            if (PlayerPos.Y < 30 || Player2Pos.Y < 30)
            {
                if (PlayerPos.Y < 30)
                    p1Wins++;
                if (Player2Pos.Y < 30)
                    p2Wins++;
                Regen();
            }
            map.Map[(int)PlayerPos.X, (int)PlayerPos.Y] = 0;
            map.Map[(int)Player2Pos.X, (int)Player2Pos.Y] = 0;
            if (JumpCount != 0)
            {
                JumpCount -= 1;
                if (map.Map[(int)PlayerPos.X, (int)PlayerPos.Y - 1] == 0)
                {
                    PlayerPos.Y -= 1f;
                }
                else
                {
                    JumpCount = 0;
                }
            }
            else if (PlayerPos.Y + 1 < map.MapHeight && map.Map[(int)PlayerPos.X, (int)PlayerPos.Y + 1] != 1 && PlayerPos.Y < map.MapHeight - 1)
            {
                PlayerPos.Y++;
            }

            if (JumpCount2 != 0)
            {
                JumpCount2 -= 1;
                if (map.Map[(int)Player2Pos.X, (int)Player2Pos.Y - 1] == 0)
                {
                    Player2Pos.Y -= 1f;
                }
                else
                {
                    JumpCount2 = 0;
                }
            }
            else if (Player2Pos.Y + 1 < map.MapHeight && map.Map[(int)Player2Pos.X, (int)Player2Pos.Y + 1] == 0 && Player2Pos.Y < map.MapHeight - 1)
            {
                Player2Pos.Y++;
            }
            if (keysPressed.Contains(Keys.Back) || keysPressed.Contains(Keys.Escape))
                Exit();
            if (keysPressed.Contains(Keys.Enter))
            {
                Regen();
            }
            if ((PlayerPos.X + 1 < map.MapWidth && keysPressed.Contains(Keys.Right) && map.Map[(int)PlayerPos.X + 1, (int)PlayerPos.Y] == 0))
            {
                PlayerPos.X++;
            }
            if ((Player2Pos.X + 1 < map.MapWidth && keysPressed.Contains(Keys.D) && map.Map[(int)Player2Pos.X + 1, (int)Player2Pos.Y] == 0))
            {
                Player2Pos.X++;
            }

            if (PlayerPos.X - 1 >= 0 && keysPressed.Contains(Keys.Left) && map.Map[(int)PlayerPos.X - 1, (int)PlayerPos.Y] == 0)
            {
                PlayerPos.X--;
            }
            if (Player2Pos.X - 1 >= 0 && keysPressed.Contains(Keys.A) && map.Map[(int)Player2Pos.X - 1, (int)Player2Pos.Y] == 0)
            {
                Player2Pos.X--;
            }


            if ((keysPressed.Contains(Keys.Up) || keysPressed.Contains(Keys.Decimal)))
            {
                if (JumpCount == 0 && (PlayerPos.Y == map.MapHeight - 1 || map.Map[(int)PlayerPos.X, (int)PlayerPos.Y + 1] != 0))
                {
                    JumpCount = 10;
                }
            }
            if ((keysPressed.Contains(Keys.W) || keysPressed.Contains(Keys.V)))
            {
                if (JumpCount2 == 0 && (Player2Pos.Y == map.MapHeight - 1 || map.Map[(int)Player2Pos.X, (int)Player2Pos.Y + 1] != 0))
                {
                    JumpCount2 = 10;
                }
            }

            if ((PlayerPos.X + 1 < map.MapWidth && (keysPressed.Contains(Keys.Right) && (keysPressed.Contains(Keys.RightShift) || keysPressed.Contains(Keys.NumPad0)))) && map.Map[(int)PlayerPos.X + 1, (int)PlayerPos.Y] == 1)
            {
                map.Map[(int)PlayerPos.X + 1, (int)PlayerPos.Y] = 0;
            }
            if ((Player2Pos.X + 1 < map.MapWidth && (keysPressed.Contains(Keys.D) && keysPressed.Contains(Keys.Space))) && map.Map[(int)Player2Pos.X + 1, (int)Player2Pos.Y] == 1)
            {
                map.Map[(int)Player2Pos.X + 1, (int)Player2Pos.Y] = 0;
            }

            if (PlayerPos.X - 1 >= 0 && (keysPressed.Contains(Keys.Left) && (keysPressed.Contains(Keys.RightShift) || keysPressed.Contains(Keys.NumPad0))) && map.Map[(int)PlayerPos.X - 1, (int)PlayerPos.Y] == 1)
            {
                map.Map[(int)PlayerPos.X - 1, (int)PlayerPos.Y] = 0;
            }
            if (Player2Pos.X - 1 >= 0 && (keysPressed.Contains(Keys.A) && keysPressed.Contains(Keys.Space)) && map.Map[(int)Player2Pos.X - 1, (int)Player2Pos.Y] == 1)
            {
                map.Map[(int)Player2Pos.X - 1, (int)Player2Pos.Y] = 0;
            }

            if (PlayerPos.Y + 1 < map.MapHeight && (keysPressed.Contains(Keys.Down) && (keysPressed.Contains(Keys.RightShift) || keysPressed.Contains(Keys.NumPad0)))
                && map.Map[(int)PlayerPos.X, (int)PlayerPos.Y + 1] == 1)
            {
                map.Map[(int)PlayerPos.X, (int)PlayerPos.Y + 1] = 0;
            }
            if (Player2Pos.Y + 1 < map.MapHeight && (keysPressed.Contains(Keys.S) && keysPressed.Contains(Keys.Space))
                && map.Map[(int)Player2Pos.X, (int)Player2Pos.Y + 1] == 1)
            {
                map.Map[(int)Player2Pos.X, (int)Player2Pos.Y + 1] = 0;
            }

            map.Map[(int)PlayerPos.X, (int)PlayerPos.Y] = 2;
            map.Map[(int)Player2Pos.X, (int)Player2Pos.Y] = 3;
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            for (int i = 0; i < map.MapWidth; i++)
            {
                for (int x = 0; x < map.MapHeight; x++)
                {
                    Vector2 position = new Vector2(i * 5, x * 5);
                    Color col = x < 20 ? Color.LimeGreen : Color.White;
                    if (Math.Abs(i - PlayerPos.X) + Math.Abs(x - PlayerPos.Y) < 30 || Math.Abs(i - Player2Pos.X) + Math.Abs(x - Player2Pos.Y) < 30 || x < 30)
                    {
                        int grades = 0;
                        float grading = 0;
                        float grading2 = 0;
                        float grading3 = 0;
                        if (x < 30)
                        {
                            grading3 = (255 / 30) * x;
                            grades++;
                        }
                        if (Math.Abs(i - PlayerPos.X) + Math.Abs(x - PlayerPos.Y) < 30)
                        {
                            grading = (255 / 30) * (Math.Abs(i - PlayerPos.X) + Math.Abs(x - PlayerPos.Y));
                            grades++;
                        }
                        if (Math.Abs(i - Player2Pos.X) + Math.Abs(x - Player2Pos.Y) < 30)
                        {
                            grading2 = (255 / 30) * (Math.Abs(i - Player2Pos.X) + Math.Abs(x - Player2Pos.Y));
                            grades++;
                        }

                        grading = grading + grading2 + grading3;
                        grading = grading / grades;
                        col = new Color()
                        {
                            R = (byte)(255 - grading),
                            G = (byte)(255 - grading),
                            B = (byte)(255 - grading),
                            A = 255
                        };
                        switch (map.Map[i, x])
                        {
                            case 0:
                                if (x > 0)
                                    if (map.Map[i, x - 1] == 1 || map.Map[i, x - 1] == 2 || map.Map[i, x - 1] == 3)
                                        col = Color.Black;
                                if (i > 0)
                                    if (map.Map[i - 1, x] == 1 || map.Map[i - 1, x] == 2 || map.Map[i - 1, x] == 3)
                                        col = Color.Black;
                                if (i > 0 && x > 0)
                                    if (map.Map[i - 1, x - 1] == 1 || map.Map[i - 1, x - 1] == 2 || map.Map[i - 1, x - 1] == 3)
                                        col = Color.Black;
                                spriteBatch.Draw(wall, position, col);
                                break;
                            case 1:
                                spriteBatch.Draw(ground, position, col);
                                break;
                            case 2:
                                if (PlayerPos == position / 5)
                                {
                                    spriteBatch.Draw(player, position);
                                }
                                else
                                {
                                    map.Map[(int)position.X / 5, (int)position.Y / 5] = 0;
                                }
                                break;
                            case 3:
                                if (Player2Pos == position / 5)
                                {
                                    spriteBatch.Draw(player2, position);
                                }
                                else
                                {
                                    map.Map[(int)position.X / 5, (int)position.Y / 5] = 0;
                                }
                                break;
                        }
                    }
                    else
                    {
                        spriteBatch.Draw(wall, position, Color.Black);
                    }
                }
            }
            Vector2 stringl1 = pixelFont.MeasureString("PLAYER 1 WINS 0 | PLAYER 2 WINS 0");
            Vector2 stringl2 = pixelFont.MeasureString("GET HERE TO WIN");

            float paddingLeft = (GraphicsDevice.DisplayMode.Width / 2) - (stringl1.X / 2);
            float paddingLeft2 = (GraphicsDevice.DisplayMode.Width / 2) - (stringl2.X / 2);

            spriteBatch.DrawString(pixelFont, string.Format("PLAYER 1 WINS {0} | PLAYER 2 WINS {1}", p2Wins, p1Wins), new Vector2(paddingLeft + 5, 15), new Color(Color.Black, 253));
            spriteBatch.DrawString(pixelFont, string.Format("PLAYER 1 WINS {0} | PLAYER 2 WINS {1}", p2Wins, p1Wins), new Vector2(paddingLeft, 10), new Color(Color.White, 253));
            spriteBatch.DrawString(pixelFont, string.Format("GET HERE TO WIN"), new Vector2(paddingLeft2 + 5, 75), new Color(Color.Black, 253));
            spriteBatch.DrawString(pixelFont, string.Format("GET HERE TO WIN"), new Vector2(paddingLeft2, 70), new Color(Color.White, 253));


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    public class MapHandler
    {
        Random rand = new Random();

        public int[,] Map;

        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public int PercentAreWalls { get; set; }

        public MapHandler()
        {
            MapWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 5 - 20;
            MapHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 5 - 20;
            PercentAreWalls = 50;

            RandomFillMap();
        }

        public void MakeCaverns()
        {
            for (int column = 0, row = 0; row <= MapHeight - 1; row++)
            {
                for (column = 0; column <= MapWidth - 1; column++)
                {
                    Map[column, row] = PlaceWallLogic(column, row);
                    if (row == MapHeight - 1)
                        Map[column, row] = 0;
                }
            }
        }

        public int PlaceWallLogic(int x, int y)
        {
            int numWalls = GetAdjacentWalls(x, y, 1, 1);


            if (Map[x, y] == 1)
            {
                if (numWalls >= 4)
                {
                    return 1;
                }
                if (numWalls < 2)
                {
                    return 0;
                }

            }
            else
            {
                if (numWalls >= 5)
                {
                    return 1;
                }
            }
            return 0;
        }

        public int GetAdjacentWalls(int x, int y, int scopeX, int scopeY)
        {
            int startX = x - scopeX;
            int startY = y - scopeY;
            int endX = x + scopeX;
            int endY = y + scopeY;

            int iX = startX;
            int iY = startY;

            int wallCounter = 0;

            for (iY = startY; iY <= endY; iY++)
            {
                for (iX = startX; iX <= endX; iX++)
                {
                    if (!(iX == x && iY == y))
                    {
                        if (IsWall(iX, iY))
                        {
                            wallCounter += 1;
                        }
                    }
                }
            }
            return wallCounter;
        }

        bool IsWall(int x, int y)
        {
            if (IsOutOfBounds(x, y))
            {
                return true;
            }

            if (Map[x, y] == 1)
            {
                return true;
            }

            if (Map[x, y] == 0)
            {
                return false;
            }
            return false;
        }

        bool IsOutOfBounds(int x, int y)
        {
            if (x < 0 || y < 0)
            {
                return true;
            }
            else if (x > MapWidth - 1 || y > MapHeight - 1)
            {
                return true;
            }
            return false;
        }

        public void PrintMap()
        {
            Console.Clear();
            Console.Write(MapToString());
        }

        string MapToString()
        {
            string returnString = string.Join(" ", // Seperator between each element
                                              "Width:",
                                              MapWidth.ToString(),
                                              "\tHeight:",
                                              MapHeight.ToString(),
                                              "\t% Walls:",
                                              PercentAreWalls.ToString(),
                                              Environment.NewLine
                                             );

            List<string> mapSymbols = new List<string>();
            mapSymbols.Add(".");
            mapSymbols.Add("#");
            mapSymbols.Add("+");

            for (int column = 0, row = 0; row < MapHeight; row++)
            {
                for (column = 0; column < MapWidth; column++)
                {
                    returnString += mapSymbols[Map[column, row]];
                }
                returnString += Environment.NewLine;
            }
            return returnString;
        }

        public void BlankMap()
        {
            for (int column = 0, row = 0; row < MapHeight; row++)
            {
                for (column = 0; column < MapWidth; column++)
                {
                    Map[column, row] = 0;
                }
            }
        }

        public void RandomFillMap()
        {
            Map = new int[MapWidth, MapHeight];

            int mapMiddle = 0;
            for (int column = 0, row = 0; row < MapHeight; row++)
            {
                for (column = 0; column < MapWidth; column++)
                {
                    if (column == 0)
                    {
                        Map[column, row] = 1;
                    }
                    else if (row == 0)
                    {
                        Map[column, row] = 1;
                    }
                    else if (column == MapWidth - 1)
                    {
                        Map[column, row] = 1;
                    }
                    else if (row == MapHeight - 1)
                    {
                        Map[column, row] = 1;
                    }
                    else
                    {
                        mapMiddle = (MapHeight / 2);

                        if (row == mapMiddle)
                        {
                            Map[column, row] = 0;
                        }
                        else
                        {
                            Map[column, row] = RandomPercent(PercentAreWalls);
                        }
                    }
                }
            }
        }

        int RandomPercent(int percent)
        {
            if (percent >= rand.Next(1, 101))
            {
                return 1;
            }
            return 0;
        }

        public MapHandler(int mapWidth, int mapHeight, int[,] map, int percentWalls = 40)
        {
            this.MapWidth = mapWidth;
            this.MapHeight = mapHeight;
            this.PercentAreWalls = percentWalls;
            this.Map = new int[this.MapWidth, this.MapHeight];
            this.Map = map;
        }
    }
}
