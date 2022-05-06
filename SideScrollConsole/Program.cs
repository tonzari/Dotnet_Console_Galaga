using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace SideScrollConsole
{
    class Program
    {
        // Grid
        static int playAreaWidth = 30;
        static int playAreaHeight = 30;

        // Time
        static int fpsTarget = 24;
        static double renderSpeedInMilliseconds => Math.Ceiling(1000.00 / fpsTarget);
        static bool gameIsRunning = false;

        // Game Objects
        static Player player;
        static List<FallingObstacle> fallingObstacles = new List<FallingObstacle>();
        static IEnumerable<GameObject> remainingObstacles; // I don't remember why I made two collections here. Investigate... I think it was to add different types of game objects
        static List<Enemy> enemies = new List<Enemy>();
        static List<AnimatedTest> animatedTests = new List<AnimatedTest>();

        // Pretend Physics
        static double gravity = -0.20; // Determines the speed that falling objects move down on the grid.

        // Input
        static ConsoleKey key;

        // Util
        static Random random = new Random();

        static void Main(string[] args)
        {
            Initialize(); // Runs once. Sets up grid, player, and enemies.
            GameLoop(); // Runs repeatedly until player wins/quits the game.
            EndGame();
        }

        static void Initialize()
        {
            player = new Player(new Vector2(1,1));

            for (int row = playAreaWidth; row >= 1; row--)
            {
                for (int column = 1; column <= playAreaHeight; column++)
                {
                    new Cell(column, row);
                }
            }

            fallingObstacles.Add(new FallingObstacle(new Vector2(10, 45)));
            fallingObstacles.Add(new FallingObstacle(new Vector2(14, 33)));
            fallingObstacles.Add(new FallingObstacle(new Vector2(1, 19)));
            fallingObstacles.Add(new FallingObstacle(new Vector2(5, 20)));
            fallingObstacles.Add(new FallingObstacle(new Vector2(9, 22)));
            fallingObstacles.Add(new FallingObstacle(new Vector2(11, 32)));
            fallingObstacles.Add(new FallingObstacle(new Vector2(4, 25)));
            fallingObstacles.Add(new FallingObstacle(new Vector2(11, 38)));

            enemies.Add(new Enemy(new Vector2(15,25)));
            enemies.Add(new Enemy(new Vector2(20,25)));
            enemies.Add(new Enemy(new Vector2(2,25)));
            enemies.Add(new Enemy(new Vector2(9,25)));

            enemies.Add(new Enemy(new Vector2(15, 24)));
            enemies.Add(new Enemy(new Vector2(20, 24)));
            enemies.Add(new Enemy(new Vector2(2, 24)));
            enemies.Add(new Enemy(new Vector2(9, 24)));

            animatedTests.Add(
                new AnimatedTest(
                    new Vector2(10, 10),
                    new List<string>() { "-_-", "-_-", "*_*", "*_*", "*_*", "*_*", "*_*", "*_*", "^_-", "^_-", "^_-", "^_-" }
                )
            );

            Console.WriteLine("Press any key to start");
            Console.ReadKey();
            Console.Clear();

            gameIsRunning = true;
        }

        static void GameLoop()
        {
            DateTime frameCompletedTimeStamp = DateTime.Now;
            TimeSpan deltaTime;

            while (gameIsRunning)
            {
                remainingObstacles = GameObject.AllObjects.Where(o => o is FallingObstacle || o is Enemy && o.isActive);

                if (remainingObstacles.Count() <= 0)
                {
                    gameIsRunning = false;
                }

                deltaTime = DateTime.Now - frameCompletedTimeStamp;

                if (deltaTime.TotalMilliseconds >= renderSpeedInMilliseconds)
                {
                    HandleInput();
                    HandlePhysics();
                    ProcessEnemies();
                    ProcessAnimatedObjects();
                    RenderScreen();

                    frameCompletedTimeStamp = DateTime.Now;
                }                
            }
        }

        private static void ProcessEnemies()
        {
            foreach (Enemy item in enemies)
            {
                int newX = Math.Clamp(item.GetPosX() + random.Next(-1, 2),1, playAreaWidth);

                Vector2 newPosition = new Vector2(newX, item.GetPosY());

                item.MoveTo(newPosition);
            }
        }

        private static void ProcessAnimatedObjects()
        {
            foreach (AnimatedTest item in animatedTests)
            {
                item.Animate();
            }
        }

        private static void HandlePhysics()
        {
            // Apply gravity to falling obstacles
            if (fallingObstacles.Count > 0)
            {
                foreach (var obstacle in fallingObstacles)
                {
                    obstacle.position.y += gravity;

                    if (obstacle.GetPosY() <= 0)
                    {
                        obstacle.position.x = -1000;
                        obstacle.position.y = 1000;
                    }
                }
            }

            // Add force to trajectories
            if (player.trajectories.Count > 0)
            {
                foreach (Trajectory traj in player.trajectories)
                {
                    traj.position.y += 1;
                }
            }
        }

        private static void EndGame()
        {
            Console.Clear();

            if (remainingObstacles.Count() == 0)
            {
                Console.WriteLine("YOU WIN!");
            }

            Console.WriteLine("Exiting app...");

            Thread.Sleep(4000);
        }

        private static void RenderScreen()
        {
            Console.Clear();

            Console.WriteLine(String.Format("{0,-10}", remainingObstacles.Count(o => o.isActive == true)));

            foreach (var cell in Cell.cells)
            {
                cell.displayCharacters = GetASCII(cell);

                Console.Write(cell.displayCharacters);

                // Create a new row
                if (cell.x % playAreaWidth == 0)
                {
                    Console.WriteLine();
                }
            }
        }

        private static string GetASCII(Cell cell)
        {
            List<GameObject> occupants = cell.GetOccupants(GameObject.AllObjects);

            if (occupants.Any())
            {
                // this is a very crude way to handle "collisions" between game objects that occupy the same space on the grid, and needs to be reworked
                GameObject displayPriority = occupants.OrderByDescending(x => x.health).First();

                List<GameObject> losers = occupants.OrderBy(x => x.health).Take(occupants.Count - 1).ToList();

                foreach (var loser in losers)
                {
                    loser.position.x = -1;
                    loser.position.y = -1;
                }

                Console.ForegroundColor = displayPriority.color;
                return displayPriority.displayCharacter;
            }

            Console.ForegroundColor = cell.color;
            return String.Format("{0,3}", cell.defaultDisplayChars);
        }

        private static void HandleInput()
        {
            if (!Console.KeyAvailable) return;   

            key = Console.ReadKey(false).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    player.MoveTo(new Vector2(player.GetPosX(), player.GetPosY() + 1));
                    break;
                case ConsoleKey.RightArrow:
                    player.MoveTo(new Vector2(player.GetPosX() + 1, player.GetPosY()));
                    break;
                case ConsoleKey.DownArrow:
                    player.MoveTo(new Vector2(player.GetPosX(), player.GetPosY() - 1));
                    break;
                case ConsoleKey.LeftArrow:
                    player.MoveTo(new Vector2(player.GetPosX() - 1, player.GetPosY()));
                    break;
                case ConsoleKey.Spacebar:
                    player.Shoot(new Vector2(player.GetPosX(), player.GetPosY() + 6));
                    break;
                case ConsoleKey.Escape:
                    Console.WriteLine("...Exiting...");
                    gameIsRunning = false;
                    break;
            }
        }
    }
}