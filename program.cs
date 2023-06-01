using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TetrisGame
{
    class Program
    {
        static int width = 10;
        static int height = 20;
        static int score = 0;
        static int speed = 100;
        static int currentX = width / 2;
        static int currentY = 0;
        static bool[,] playfield = new bool [width, height];
        static List <int[]> currentShape = new List<int[]>();
        static Random random = new Random();

        static void Main(string[] args)
        {
            Console.Title = "Tetris";
            Console.CursorVisible = false;

            InitPlayfield();
            GenerateShape();

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.LeftArrow)
                    {
                        MoveShape(-1, 0);
                    } else if (key == ConsoleKey.RightArrow)
                    {
                        MoveShape(1,0);
                    } else if (key == ConsoleKey.DownArrow)
                    {
                        MoveShape(0,1);
                    } else if (key == ConsoleKey.Spacebar)
                    {
                        RotateShape();
                    }
                }

                if (DateTime.Now.Substract(TimeSpan.FromMilliseconds(speed)) > lastActionTime)
                {
                    MoveShape(0,1);
                }

                DrawPlayfield();
                Thread.Sleep(20);
            }
        }

        static void InitPlayfield()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    playfield[x,y] = false;
                }
            }
        }

        static void GenerateShape()
        {
            int shapeIndex = random.Next(7);
            currentX = width / 2;
            currentY = 0;

            switch (shapeIndex)
            {
                case 0: //I shape
                    currentShape = new List<int[]>
                    {
                        new int[] {0,1},
                        new int[] {1,1},
                        new int[] {2,1},
                        new int[] {3,1}
                    };
                    break;
                case 1: // J Shape
                    currentShape = new List<int[]>
                    {
                        new int[] {0,1},
                        new int[] {0,2},
                        new int[] {1,2},
                        new int[] {2,2}
                    };
                    break;
                case 2: //L Shape
                    currentShape = new List<int[]>
                    {
                        new int[] {2,1},
                        new int[] {0,2},
                        new int[] {1,2},
                        new int[] {2,2}
                    };
                    break;
                case 3: //O shape
                    currentShape = new List<int[]>
                    {
                        new int[] {0,0},
                        new int[] {0,1},
                        new int[] {1,0},
                        new int[] {1,1}
                    };
                    break;
                case 4: // S shape
                    currentShape = new List<int[]>
                    {
                        new int[] {1,1},
                        new int[] {2,1},
                        new int[] {0,2},
                        new int[] {1,2}
                    };
                    break;
                case 5: //T shape
                    currentShape = new List<int[]>
                    {
                        new int[] {1,1},
                        new int[] {0,2},
                        new int[] {1,2},
                        new int[] {2,2}
                    };
                    break;
                case 6: //Z shape 
                    currentShape = new List<int[]>
                    {
                        new int[] {0,1},
                        new int[] {1,1},
                        new int[] {1,2},
                        new int[] {2,2}
                    };
                    break;
            }
        }

        static DateTime lastActionTime = DateTime.Now;

        static void MoveShape(int moveX, int moveY)
        {
            if (IsValidMove(currentShape, currentX + moveX, currentY + moveY))
            {
                currentX += moveX;
                currentY += moveY;
            }
            else if (moveY > 0) //Lock the shape in place if it cannot move down anymore
            {
                foreach (var block in currentShape)
                {
                    playfield[currentX + block[0], currentY + block[1]] = true;
                }

                CheckLines();
                GenerateShape();

                if (!IsValidMove(currentShape, currentX, currentY))
                {
                    //Game over
                    Console.Clear();
                    Console.SetCursorPosition(width / 2 - 4, height / 2);
                    Console.WriteLine("Game Over");
                    Console.SetCursorPosition(width / 2 - 4, height / 2 + 1);
                    Console.WriteLine("Score: "+ score);
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }

            lastActionTime = DateTime.Now;
        }

        static void RotateShape()
        {
            List<int[]> rotatedShape = new List<int[]>();

            foreach (var block in currentShape)
            {
                int x = block[0];
                int y = block[1];
                int newX = -y;
                int newY = x;

                rotatedShape.Add(new int[] {newX, newY});
            }

            if (IsValidMove(rotatedShape, currentX, currentY))
            {
                currentShape = rotatedShape;
            }
        }

        static bool IsValidMove(List<int[]> shape, int x, int y)
        {
            foreach (var block in shape)
            {
                int newX = x + block[0];
                int newY = y + block[1];

                if (newX < 0 || newX >= width || newY < 0 || newY >= height || playfield[newX, newY])
                {
                    return false;
                }
            }

            return true;
        }

        static void CheckLines()
        {
            for (int y = height -1; y >= 0; y--)
            {
                bool isLineFull = true;

                for (int x = 0; x < width; x++)
                {
                    if (!playfield[x,y])
                    {
                        isLineFull = false;
                        break;
                    }
                }

                if (isLineFull)
                {
                    score++;
                    if (speed < 10)
                        speed -=10;
                    
                    for (int y2 = y; y2 > 0; y2--)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            playfield[x, y2] = playfield[x, y2 - 1];
                        }
                    }

                    for (int x = 0; x < width; x++)
                    {
                        playfield[x, 0] = false;
                    }

                    y++;
                }
            }
        }

        static void DrawPlayfield()
        {
            Console.Clear();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (playfield[x,y])
                    {
                        Console.SetCursorPosition(x,y);
                        Console.Write("■");
                    }
                }
            }

            foreach (var block in currentShape)
            {
                int x = currentX + block[0];
                int y = currentY + block[1];

                if (y >= 0)
                {
                    Console.SetCursorPosition(x,y);
                    Console.Write("■");
                }
            }

            Console.SetCursorPosition(width + 2, 0);
            Console.WriteLine("Score: " + score);
        }
    }
}
