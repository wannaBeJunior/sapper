using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sapper
{
    public partial class Form1 : Form
    {
        public const int mapSize = 5;

        public int[,] map = new int[mapSize, mapSize];
        public Button[,] buttons = new Button[mapSize, mapSize];

        public static Image spriteSet;
        public static bool isFirstStep;
        private static int currentPictureToSet = 0;
        private static Point firstCoord;
        private static int bombsCount;
        public Form1()
        {
            InitializeComponent();

            initGame();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void initGame()
        {
            isFirstStep = true;
            spriteSet = new Bitmap(Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName.ToString(), "Sprites\\tiles.png"));
            initMap();
            initButtons();
        }

        private void initMap()
        {
            for(int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    map[i, j] = 0;
                }
            }
        }

        private void initButtons()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    Button button = new Button();
                    button.Location = new Point(j * 50, i * 50);
                    button.Size = new Size(50, 50);
                    button.MouseUp += new MouseEventHandler(OnButtonPressedMouse);
                    this.Controls.Add(button);
                    buttons[i, j] = button;
                }
            }
        }

        private void OnButtonPressedMouse(object sender, MouseEventArgs e)
        {
            Button pressedButton = sender as Button;
            MessageBox.Show(bombsCount.ToString());
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

        private void OnRightButtonPressed(Button pressedButton)
        {
            int iButton = pressedButton.Location.Y / 50;
            int jButton = pressedButton.Location.X / 50;
            if(map[iButton, jButton] == -1)
            {
                bombsCount--;
            }
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
            pressedButton.Image = FindNeededImage(posX, posY);
        }

        private void OnLeftButtonPressed(Button pressedButton)
        {
            if (bombsCount == 0 && !isFirstStep)
            {
                endOfGame();
            }
            pressedButton.Enabled = false;
            int iButton = pressedButton.Location.Y / 50;
            int jButton = pressedButton.Location.X / 50;
            if (isFirstStep)
            {
                firstCoord = new Point(jButton, iButton);
                SeedMap();
                CountCellBomb();
                isFirstStep = false;
            }
            OpenCells(iButton, jButton);

            if (map[iButton, jButton] == -1)
            {
                ShowAllBombs(iButton, jButton);
                MessageBox.Show("Поражение!");
                this.Controls.Clear();
                initGame();
            }
        }

        private void OpenCells(int i, int j)
        {
            OpenCell(i, j);

            if (map[i, j] > 0)
                return;

            for (int k = i - 1; k < i + 2; k++)
            {
                for (int l = j - 1; l < j + 2; l++)
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

        private void OpenCell(int i, int j)
        {
            buttons[i, j].Enabled = false;

            switch (map[i, j])
            {
                case 1:
                    buttons[i, j].Image = FindNeededImage(1, 0);
                    break;
                case 2:
                    buttons[i, j].Image = FindNeededImage(2, 0);
                    break;
                case 3:
                    buttons[i, j].Image = FindNeededImage(3, 0);
                    break;
                case 4:
                    buttons[i, j].Image = FindNeededImage(4, 0);
                    break;
                case 5:
                    buttons[i, j].Image = FindNeededImage(0, 1);
                    break;
                case 6:
                    buttons[i, j].Image = FindNeededImage(1, 1);
                    break;
                case 7:
                    buttons[i, j].Image = FindNeededImage(2, 1);
                    break;
                case 8:
                    buttons[i, j].Image = FindNeededImage(3, 1);
                    break;
                case -1:
                    buttons[i, j].Image = FindNeededImage(1, 2);
                    break;
                case 0:
                    buttons[i, j].Image = FindNeededImage(0, 0);
                    break;
            }
        }

        private void ShowAllBombs(int iBomb, int jBomb)
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (i == iBomb && j == jBomb)
                        continue;
                    if (map[i, j] == -1)
                    {
                        buttons[i, j].Image = FindNeededImage(3, 2);
                    }
                }
            }
        }

        private void CountCellBomb()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (map[i, j] == -1)
                    {
                        for (int k = i - 1; k < i + 2; k++)
                        {
                            for (int l = j - 1; l < j + 2; l++)
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

        private static bool IsInBorder(int i, int j)
        {
            if (i < 0 || j < 0 || j > mapSize - 1 || i > mapSize - 1)
            {
                return false;
            }
            return true;
        }

        public static Image FindNeededImage(int xPos, int yPos)
        {
            Image image = new Bitmap(50, 50);
            Graphics g = Graphics.FromImage(image);
            g.DrawImage(spriteSet, new Rectangle(new Point(0, 0), new Size(50, 50)), 0 + 32 * xPos, 0 + 32 * yPos, 33, 33, GraphicsUnit.Pixel);


            return image;
        }

        private void SeedMap()
        {
            Random r = new Random();
            int number = r.Next(1, 1);
            bombsCount = number;
            for (int i = 0; i < number; i++)
            {
                int posI = r.Next(0, mapSize - 1);
                int posJ = r.Next(0, mapSize - 1);

                while (map[posI, posJ] == -1 || (Math.Abs(posI - firstCoord.Y) <= 1 && Math.Abs(posJ - firstCoord.X) <= 1))
                {
                    posI = r.Next(0, mapSize - 1);
                    posJ = r.Next(0, mapSize - 1);
                }
                map[posI, posJ] = -1;
            }
        }

        private void endOfGame()
        {
            MessageBox.Show("Победа!");
            this.Controls.Clear();
            initGame();
        }
    }
}
