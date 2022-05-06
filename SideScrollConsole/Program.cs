using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace SideScrollConsole
{

    public class Vector2
    {
        public double x;
        public double y;

        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Cell
    {
        public static int counter = 0;
        public int id;
        public int x;
        public int y;
        public string defaultDisplayChars = " . ";
        public string displayCharacters = "";
        public int animIndex = 0;
        public ConsoleColor color;
        public static List<Cell> cells = new List<Cell>();

        public Cell(int x, int y)
        {
            counter++;
            this.id = counter;
            this.x = x;
            this.y = y;
            color = ConsoleColor.Blue;
            cells.Add(this);
        }

        public List<GameObject> GetOccupants(List<GameObject> gameObjects)
        {
            return gameObjects.Where(go => go.GetPosX() == x && go.GetPosY() == y).ToList();
        }

        public GameObject GetStrongestOccupant(List<GameObject> gameObjects)
        {
            List<GameObject> occupants = GetOccupants(gameObjects);
            GameObject winner = occupants.OrderByDescending(x => x.health).First();

            return winner;    
        }
    }

    public class GameObject
    {
        public Vector2 position;
        public bool isActive;
        public string displayCharacter = "";
        public ConsoleColor color;
        public int health = 1;

        public static List<GameObject> AllObjects = new List<GameObject>();

        public GameObject(Vector2 position)
        {
            color = ConsoleColor.Blue;
            this.position = position;
            isActive = true;
            AllObjects.Add(this);
        }

        public int GetPosX()
        {
            return (int)Math.Round(this.position.x);
        }

        public int GetPosY()
        {
            return (int)Math.Round(this.position.y);
        }
    }

    class Player : GameObject
    {
        public List<Trajectory> trajectories = new List<Trajectory>();

        public Player(Vector2 position) : base(position)
        {
            color = ConsoleColor.Cyan;
            health = 4;
            displayCharacter = "\\o/";
        }

        public void MoveTo(Vector2 targetPosition)
        {
            position.x = targetPosition.x;
            position.y = targetPosition.y;
        }

        public void Shoot(Vector2 targetPosition)
        {
            Trajectory trajectory = new Trajectory(new Vector2(this.position.x, this.position.y));
            trajectories.Add(trajectory);
        }
    }

    class Trajectory : GameObject
    {
        public Trajectory(Vector2 position) : base(position)
        {
            color = ConsoleColor.Yellow;
            health = 6;
            displayCharacter = " ^ ";
        }
    }

    class FallingObstacle : GameObject
    {
        public FallingObstacle(Vector2 position) : base(position)
        {
            color = ConsoleColor.White;
            health = 5;
            displayCharacter = " v ";
        }

    }

    class Program
    {

        static bool gameIsRunning = false;
        static ConsoleKey key;
        static int fpsTarget = 24;
        static double renderSpeedInMilliseconds => Math.Ceiling(1000.00 / fpsTarget);
        static int playAreaWidth = 30;
        static int playAreaHeight = 30;
        static Player player;
        static List<FallingObstacle> fallingObstacles = new List<FallingObstacle>();
        static double gravity = -0.20;
        static IEnumerable<GameObject> remainingObstacles;

        static void Main(string[] args)
        {
            Initialize();
            GameLoop();
            EndGame();
        }

        private static void EndGame()
        {
            Console.Clear();
            Console.WriteLine("YOU WIN!");
            Thread.Sleep(4000);
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
                remainingObstacles = GameObject.AllObjects.Where(o => o is FallingObstacle && o.isActive);


                if (remainingObstacles.Count() <= 0)
                {
                    gameIsRunning = false;
                }

                deltaTime = DateTime.Now - frameCompletedTimeStamp;

                if (deltaTime.TotalMilliseconds >= renderSpeedInMilliseconds)
                {
                    HandleInput();
                    HandlePhysics();
                    RenderScreen();

                    frameCompletedTimeStamp = DateTime.Now;
                }                
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

        private static void RenderScreen()
        {
            Console.Clear();

            Console.WriteLine(String.Format("{0,-10}", remainingObstacles.Count()));

            foreach (var cell in Cell.cells)
            {
                cell.displayCharacters = GetASCII(cell);

                Console.Write(cell.displayCharacters);

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