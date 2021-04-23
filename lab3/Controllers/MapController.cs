using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;

namespace lab3.Controllers
{
    public static class MapController
    {
        public const int MapSize = 9;
        public const int CellSize = 50;
        private static int currentPictureToSet = 0;
        public static int[,] map = new int[MapSize, MapSize];
        public static Button[,] buttons = new Button[MapSize, MapSize];
        public static Image spriteSet;
        private static bool IsFirstStep;
        private static Point FirstCoordinate;
        public static Form form;

        private static void ConfigureMapSize(Form current)
        {
            current.Width = MapSize * CellSize + 20;
            current.Height = (MapSize + 1) * CellSize;
        }

        private static void InitMap()
        {
            for (int x = 0; x < MapSize; x++)
            {
                for (int y = 0; y < MapSize; y++)
                {
                    map[x, y] = 0;
                }
            }
        }


        public static void Init(Form current)
        {
            form = current;
            currentPictureToSet = 0;
            IsFirstStep = true;
            spriteSet = new Bitmap(Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName.ToString(), "sprites\\tiles.png"));
            ConfigureMapSize(current);
            InitMap();
            InitButtons(current);
        }


        private static void InitButtons(Form current)
        {
            for (int x = 0; x < MapSize; x++)
            {
                for (int y = 0; y < MapSize; y++)
                {
                    Button button = new Button();
                    button.Location = new Point(y * CellSize + 10, x * CellSize + 30);
                    button.Size = new Size(CellSize, CellSize);
                    button.Image = FindNeededImage(0, 0);
                    button.MouseUp += new MouseEventHandler(OnButtonPressedMouse);
                    current.Controls.Add(button);
                    buttons[x, y] = button;
                }
            }
        }

        private static void OnButtonPressedMouse(object sender, MouseEventArgs e)
        {
            Button pressedButton = sender as Button;
            switch (e.Button.ToString())
            {
                case "Right":
                    OnRightButtonPressed(pressedButton);
                    break;
                case "Left":
                    OnLeftButtonPressed(pressedButton);
                    break;
            }
        }

        private static void OnRightButtonPressed(Button PressedButton)
        {
            currentPictureToSet++;
            currentPictureToSet %= 3;
            int posX = 0;
            int posY = 0;
            switch (currentPictureToSet)
            {
                case 0:
                    posX = 0;
                    posY = 0;
                    break;
                case 1:
                    posX = 0;
                    posY = 2;
                    break;
                case 2:
                    posX = 2;
                    posY = 2;
                    break;
            }
            PressedButton.Image = FindNeededImage(posX, posY);
        }


        private static void OnLeftButtonPressed(Button PressedButton)
        {
            PressedButton.Enabled = false;
            int xButton = PressedButton.Location.Y / CellSize;
            int yButton = PressedButton.Location.X / CellSize;
            if (IsFirstStep)
            {
                FirstCoordinate = new Point(xButton, yButton);
                SeedMap();
                CountCellBomb();
                IsFirstStep = false;
            }
            OpenCells(xButton, yButton);
            if (map[xButton, yButton] == -1)
            {
                ShowAllBombs(xButton, yButton);
                MessageBox.Show("Поражение!");
                form.Controls.Clear();
                Init(form);
            }
        }

        private static void ShowAllBombs(int xBomb, int yBomb)
        {
            for (int x = 0; x < MapSize; x++)
            {
                for (int y = 0; y < MapSize; y++)
                {
                    if (x == xBomb && y == yBomb)
                        continue;
                    if (map[x, y] == -1)
                    {
                        buttons[x, y].Image = FindNeededImage(3, 2);
                    }
                }
            }
        }

        public static Image FindNeededImage(int xPos, int yPos)
        {
            Image image = new Bitmap(CellSize, CellSize);
            Graphics g = Graphics.FromImage(image);
            g.DrawImage(spriteSet, new Rectangle(new Point(0, 0), new Size(CellSize, CellSize)), 0 + 32 * xPos, 0 + 32 * yPos, 33, 33, GraphicsUnit.Pixel);
            return image;
        }

        private static void SeedMap()
        {
            Random random = new Random();
            int number = random.Next(5, 10);
            for (int x = 0; x < number; x++)
            {
                int posX = random.Next(0, MapSize - 1);
                int posY = random.Next(0, MapSize - 1);
                while (map[posX, posY] == -1 || (Math.Abs(posY - FirstCoordinate.Y) <= 1 && Math.Abs(posX - FirstCoordinate.X) <= 1))
                {
                    posX = random.Next(0, MapSize - 1);
                    posY = random.Next(0, MapSize - 1);
                }
                map[posX, posY] = -1;
            }
        }

        private static void CountCellBomb()
        {
            for (int x = 0; x < MapSize; x++)
            {
                for (int y = 0; y < MapSize; y++)
                {
                    if (map[x, y] == -1)
                    {
                        for (int k = x - 1; k < x + 2; k++)
                        {
                            for (int l = y - 1; l < y + 2; l++)
                            {
                                if (!IsInBorder(k, l) || map[k, l] == -1)
                                    continue;
                                map[k, l] = map[k, l] + 1;
                            }
                        }
                    }
                }
            }
        }

        private static void OpenCell(int x, int y)
        {
            buttons[x, y].Enabled = false;

            switch (map[x, y])
            {
                case 1:
                    buttons[x, y].Image = FindNeededImage(1, 0);
                    break;
                case 2:
                    buttons[x, y].Image = FindNeededImage(2, 0);
                    break;
                case 3:
                    buttons[x, y].Image = FindNeededImage(3, 0);
                    break;
                case 4:
                    buttons[x, y].Image = FindNeededImage(4, 0);
                    break;
                case 5:
                    buttons[x, y].Image = FindNeededImage(0, 1);
                    break;
                case 6:
                    buttons[x, y].Image = FindNeededImage(1, 1);
                    break;
                case 7:
                    buttons[x, y].Image = FindNeededImage(2, 1);
                    break;
                case 8:
                    buttons[x, y].Image = FindNeededImage(3, 1);
                    break;
                case -1:
                    buttons[x, y].Image = FindNeededImage(1, 2);
                    break;
                case 0:
                    buttons[x, y].Image = FindNeededImage(0, 0);
                    break;
            }
        }

        private static void OpenCells(int x, int y)
        {
            OpenCell(x, y);

            if (map[x, y] > 0)
                return;

            for (int k = x - 1; k < x + 2; k++)
            {
                for (int l = y - 1; l < y + 2; l++)
                {
                    if (!IsInBorder(k, l))
                        continue;
                    if (!buttons[k, l].Enabled)
                        continue;
                    if (map[k, l] == 0)
                        OpenCells(k, l);
                    else if (map[k, l] > 0)
                        OpenCell(k, l);
                }
            }
        }

        private static bool IsInBorder(int x, int y)
        {
            if (x < 0 || y < 0 || y > MapSize - 1 || x > MapSize - 1)
            {
                return false;
            }
            return true;
        }
    }
}
