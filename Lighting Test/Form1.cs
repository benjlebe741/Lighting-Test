using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lighting_Test
{
    public partial class Form1 : Form
    {
        //Shorthand
        int x = 0;
        int y = 1;
        int up = 0;
        int down = 1;
        int left = 2;
        int right = 3;


        //Lists to store the current loaded tiles, and current tiles Colour Pallette:
        List<Rectangle> currentTiles = new List<Rectangle>();
        List<Rectangle> currentTilePixels = new List<Rectangle>();
        List<int> currentTilePixelDepths = new List<int>();

        int tileWidth = 45;
        double[] rgb = { 0, 0, 0 };
        //storing current Level
        int currentLevel;



        //Key info
        bool[] WSAD = new bool[] { false, false, false, false };
        Keys[] keysToCheck = new Keys[] { Keys.W, Keys.S, Keys.A, Keys.D };

        //Player info
        Rectangle player;
        Rectangle playerXCheck;
        Rectangle playerYCheck;
        int[] xYDirection = new int[] { 0, 0 };
        int[] xYSpeed = new int[] { 6, 6 };

        Point playerSpawn = new Point(580, 326);
        int playerSpawnLevel = 0;
        int[] canMoveUpDownLeftRight = new int[4] { 0, 0, 0, 0 };

        int[][] determineDirectionsList = new int[][]
          {
      new int[]{  0,1,1}, //Up, Down, Affect Vertical Direction
      new int[]{  2,3,0}, //Left, Right, Affect Horizontal Direction
          };



        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush blueBrush = new SolidBrush(Color.DodgerBlue);
        SolidBrush whiteBrush = new SolidBrush(Color.White);

        //How large is a row of levels? (how many levels are in a row of levels?) 
        int levelLayoutWidth = 3;
        #region ALL LEVELS:
        int[][] levels = new int[][]
               {
                new int[]
                {
                30,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,
                4,4,1,1,4,4,4,4,4,4,4,4,4,4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,
                4,4,1,1,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,4,4,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                4,4,1,1,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,1,1,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
            },
                 new int[]
                {
                30,
                4,7,7,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
                7,4,1,7,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,4,4,4,4,4,4,4,4,
                4,4,7,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,4,
                7,7,7,7,7,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,1,1,1,1,1,1,1,1,1,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,1,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
            },

        new int[]
                {
                30,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
                4,4,1,1,7,1,1,1,1,1,1,1,1,1,4,4,1,1,1,1,1,1,4,4,4,4,4,4,4,4,
                4,4,1,1,7,1,1,7,7,7,1,1,1,1,4,4,1,1,1,1,1,1,1,1,1,4,4,4,4,4,
                4,4,7,7,7,1,1,7,7,7,1,1,1,1,4,4,1,1,1,1,1,1,1,1,1,1,1,4,4,4,
                4,4,1,1,7,1,7,1,1,1,7,1,1,1,4,4,1,1,1,1,1,1,1,1,1,1,1,4,4,4,
                4,4,1,1,7,1,7,1,1,1,7,1,1,1,4,4,4,4,1,1,1,1,1,1,1,1,1,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,1,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,1,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,1,1,1,1,1,1,1,1,1,1,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,
                1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,
                4,4,4,4,4,4,4,4,4,4,1,1,1,1,1,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
            },
        //NEXT LAYER
    new int[]
                {
                30,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                4,4,1,1,7,1,1,1,1,1,1,1,1,1,4,7,1,7,1,1,1,1,4,4,4,4,4,4,4,4,
                4,4,1,1,7,1,1,7,7,7,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,4,
                4,4,7,7,7,1,1,7,7,7,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,
                4,4,1,1,7,1,7,1,1,1,7,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,
                4,4,1,1,7,1,7,1,1,1,7,1,1,1,4,4,4,4,1,1,1,1,1,1,1,1,1,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,1,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                4,1,1,1,7,1,7,1,1,1,7,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                4,1,1,1,7,1,7,1,1,1,7,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
            },
     new int[]
                {
                30,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
                4,4,1,1,7,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,4,4,4,4,4,4,4,4,
                4,4,1,1,7,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,4,
                4,4,7,7,7,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,1,1,1,1,1,1,1,1,1,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,1,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                1,1,1,1,1,1,4,1,1,1,4,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,4,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
            },
new int[]
       {
                30,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                4,4,1,1,7,1,1,1,1,1,1,1,1,1,4,7,1,7,1,1,1,1,4,4,4,4,4,4,4,4,
                4,4,1,1,7,1,1,7,7,7,1,1,1,1,4,1,1,1,1,1,1,1,1,1,4,4,4,4,4,4,
                4,4,7,7,7,1,1,7,7,7,1,1,1,1,4,7,1,1,1,1,1,1,1,1,4,1,1,4,4,4,
                4,4,1,1,7,1,7,1,1,1,7,1,1,1,4,1,1,1,1,1,1,1,1,1,4,1,1,4,4,4,
                4,4,1,1,7,1,7,1,1,1,7,1,1,1,4,4,4,4,1,1,1,1,1,1,4,1,1,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,4,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,1,1,1,1,1,1,4,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,
                1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,
                4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
            },
};
        #endregion
        #region ALL TILE PATTERNS:
        int[][] tilePatterns = new int[][]
        {
        new int[]{
            3,
            10,10,10,
            16,10,16,
            10,10,100,
        },
        new int[]{
           3,
              10,10,10,
            16,100,16,
            10,10,10,
        },
         new int[]{
            3,
            1,0,1,
            1,1,1,
            1,0,1,
        },
             new int[]{
            3,
            1,0,0,
            0,1,0,
            0,0,1,
        },
        new int[]{
            3,
            1,0,0,
            0,1,0,
            1,0,0,
        },
          new int[]{
            3,
            1,0,0,
            0,1,0,
            1,0,0,
        },
            new int[]{
            3,
            1,0,0,
            0,1,0,
            1,0,0,
        },
              new int[]{
            3,
            1,0,0,
            0,1,0,
            1,0,0,
        },
        };
        #endregion
        public Form1()
        {
            InitializeComponent();
            player = new Rectangle(580, 320, 0, 0);
            playerXCheck = new Rectangle(0, 0, 0, 0);
            playerYCheck = new Rectangle(0, 0, 0, 0);

            createLevel(levels[0], tileWidth);
            currentLevel = 0;
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            #region Determing All Object Directions
            //Determening all player directions depending on what keys are pressed: (for example: if 'Up' is down, and 'Down' is not, player2's vertical direction is -1, reverse those to get 1, and if both/neither are down, get a direction of 0.)
            for (int i = 0; i < determineDirectionsList.Length; i++)
            {
                if (WSAD[determineDirectionsList[i][0]] == true && WSAD[determineDirectionsList[i][1]] == false)
                {
                    xYDirection[determineDirectionsList[i][2]] = -1;
                }
                else if (WSAD[determineDirectionsList[i][0]] == false && WSAD[determineDirectionsList[i][1]] == true)
                {
                    xYDirection[determineDirectionsList[i][2]] = 1;
                }
                else
                {
                    xYDirection[determineDirectionsList[i][2]] = 0;
                }
            }
            #endregion
            #region Move Player
            if (xYDirection[x] != canMoveUpDownLeftRight[left] && xYDirection[x] != canMoveUpDownLeftRight[right])
            {
                player.X += xYDirection[x] * xYSpeed[x];
            }
            if (xYDirection[y] != canMoveUpDownLeftRight[up] && xYDirection[y] != canMoveUpDownLeftRight[down])
            {
                player.Y += xYDirection[y] * xYSpeed[y];
            }

            canMoveUpDownLeftRight[up] = 0;
            canMoveUpDownLeftRight[down] = 0;
            canMoveUpDownLeftRight[left] = 0;
            canMoveUpDownLeftRight[right] = 0;

            for (int n = 0; n < currentTiles.Count; n++)
            {
                if (playerYCheck.IntersectsWith(currentTiles[n]) && levels[currentLevel][n] == 4)
                {
                    //TOP WALL
                    if (playerYCheck.Y >= currentTiles[n].Y && n + 30 <= currentTiles.Count && levels[currentLevel][n + 30] != 4)
                    {
                        canMoveUpDownLeftRight[up] = -1;
                    }
                    //BOTTOM WALL
                    if (playerYCheck.Y <= currentTiles[n].Y && n - 30 >= 0 && levels[currentLevel][n - 30] != 4)
                    {

                        canMoveUpDownLeftRight[down] = 1;

                    }
                }

                if (playerXCheck.IntersectsWith(currentTiles[n]) && levels[currentLevel][n] == 4)
                {
                    //LEFT WALL
                    if (playerXCheck.X >= currentTiles[n].X && n + 1 <= currentTiles.Count && levels[currentLevel][n + 1] != 4)
                    {
                        canMoveUpDownLeftRight[left] = -1;
                    }
                    //RIGHT WALL
                    if (playerXCheck.X <= currentTiles[n].X && n - 1 >= 0 && levels[currentLevel][n - 1] != 4)
                    {
                        canMoveUpDownLeftRight[right] = 1;
                    }
                }

                if (player.IntersectsWith(currentTiles[n]) && levels[currentLevel][n] == 4)
                {
                    //TOP WALL
                    if (player.Y >= currentTiles[n].Y && n + 30 <= currentTiles.Count && levels[currentLevel][n + 30] != 4)
                    {
                        player.Y += 1;
                    }
                    //BOTTOM WALL
                    if (player.Y <= currentTiles[n].Y && n - 30 >= 0 && levels[currentLevel][n - 30] != 4)
                    {
                        player.Y += -1;
                    }
                    //LEFT WALL
                    if (player.X >= currentTiles[n].X && n + 1 <= currentTiles.Count && levels[currentLevel][n + 1] != 4)
                    {
                        player.X += 1;
                    }
                    //RIGHT WALL
                    if (player.X <= currentTiles[n].X && n - 1 >= 0 && levels[currentLevel][n - 1] != 4)
                    {
                        player.X += -1;
                    }
                }
            }
            #endregion
            #region Passing Through Levels
            if (player.Y <= player.Height / 2)
            {
                player.Y = this.Height - player.Height;
                currentLevel += levelLayoutWidth * -1;
                createLevel(levels[currentLevel], tileWidth);
            }
            if (player.Y >= this.Height - player.Height / 2)
            {
                player.Y = player.Height;
                currentLevel += levelLayoutWidth;
                createLevel(levels[currentLevel], tileWidth);
            }
            if (player.X <= player.Height / 2)
            {
                player.X = this.Width - player.Height;
                currentLevel += -1;
                createLevel(levels[currentLevel], tileWidth);
            }
            if (player.X >= this.Width - player.Height / 2)
            {
                player.X = player.Height;
                currentLevel += 1;
                createLevel(levels[currentLevel], tileWidth);
            }
            #endregion

            //My info
            playerXCheck.Location = new Point(player.X - 1, player.Y);
            playerYCheck.Location = new Point(player.X, player.Y - 1);
            label1.Text = $"{WSAD[0]} {WSAD[1]} {WSAD[2]} {WSAD[3]}\n{xYDirection[0]} {xYDirection[1]}\n{canMoveUpDownLeftRight[up]} {canMoveUpDownLeftRight[down]} {canMoveUpDownLeftRight[left]} {canMoveUpDownLeftRight[right]} ";
            Refresh();
        }

        #region Key Checks
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //Check all keys and if they are down set the boolian value to true
            checkKey(true, e);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //Check all keys and if they are up set the boolian value to false
            checkKey(false, e);
        }

        void checkKey(bool trueOrFalse, KeyEventArgs e)
        {
            for (int i = 0; i < keysToCheck.Length; i++)
            {
                if (e.KeyCode == keysToCheck[i])
                {
                    WSAD[i] = trueOrFalse;
                }
            }
        }
        #endregion

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            for (int n = 0; n < currentTilePixels.Count; n++)
            {

                for (int num = 0; num < 3; num++)
                {
                    if (currentTilePixelDepths[n] > 0)
                    {
                        rgb[num] = (20000 / GetLength(player, currentTilePixels[n]) / currentTilePixelDepths[n]) - (num * 30) * (num + 1);//at the end here im just playing aroung with color at ( - (num * 30) * (num + 1); ), should change!
                    }
               
                    if (rgb[num] > 100) { rgb[num] += 20; }
                    if (rgb[num] > 220) { rgb[num] = 220; }
                    if (rgb[num] < 40) { rgb[num] -= 12; }
                    if (rgb[num] < 5) { rgb[num] = 0; }
                }

                try { e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(Convert.ToInt32(rgb[0]), Convert.ToInt32(rgb[1]), Convert.ToInt32(rgb[2]))), currentTilePixels[n]); }
                catch { }

            }


            e.Graphics.FillRectangle(whiteBrush, playerXCheck);
            e.Graphics.FillRectangle(whiteBrush, playerYCheck);
            e.Graphics.FillRectangle(redBrush, player);
        }

        #region Create Level
        private void createLevel(int[] level, int rectangleDimension)
        {
            this.Width = rectangleDimension * 30;
            this.Height = rectangleDimension * ((level.Length - 1) / 30);
            this.CenterToScreen();
            //Reset the current loaded level
            currentTiles.Clear();
            currentTilePixelDepths.Clear();
            currentTilePixels.Clear();

            //levelY keeps track of the 'Y' axis of the tile we are placing in the level
            int levelY = 0;
            //n represents the current tile
            for (int n = 0; n < level.Length; n++)
            {
                //levelX works like levelY but for the x coodinate; first it is set to tile value, and then has the board width subtracted from it until it fits from 1 to level[0]
                int levelX = n;
                while (levelX > level[0])
                {
                    levelX -= level[0];
                }
                int rectangleX = ((levelX - 1) * rectangleDimension);
                int rectangleY = ((levelY - 1) * rectangleDimension);

                //Now that we have the x and y point of the rectangle, put it into physical space
                currentTiles.Add(new Rectangle(rectangleX, rectangleY, rectangleDimension, rectangleDimension));

                //Now we have a boundary for where the player can go, we still need to place in the image
                if (n > 0)
                {
                    int thisTile = level[n];
                    int tilePixelY = 0;
                    int pixelDimension = rectangleDimension / tilePatterns[thisTile][0];

                    for (int m = 0; m < tilePatterns[0].Length; m++)
                    {
                        int tilePixelX = m;
                        while (tilePixelX > tilePatterns[0][0])
                        {
                            tilePixelX -= tilePatterns[0][0];
                        }

                        int pixelX = (tilePixelX - 1) * pixelDimension;
                        int pixelY = (tilePixelY - 1) * pixelDimension;
                        //Now that we have the x and y point of the rectangle, put it (one pixel of the rectangle) into physical space, aswell as adding the integer storing its color value into its list.
                        if (m > 0)
                        {
                            currentTilePixels.Add(new Rectangle(pixelX + rectangleX, pixelY + rectangleY, pixelDimension, pixelDimension));
                            currentTilePixelDepths.Add(tilePatterns[thisTile][m]);
                        }
                        if ((m) % (tilePatterns[0][0]) == 0)
                        {
                            //If we get to the end of a row, add one to levelY to move us down a row, and reset levelX to 1 (the begining of the row)
                            tilePixelY++;
                        }
                    }
                }
                if ((n) % (level[0]) == 0)
                {
                    //If we get to the end of a row, add one to levelY to move us down a row, and reset levelX to 1 (the begining of the row)
                    levelY++;
                }

            }
            int playerSize = Convert.ToInt32(rectangleDimension * 0.7);
            player.Size = new Size(playerSize, playerSize);
            playerXCheck = new Rectangle(0, 0, playerSize + 2, playerSize); ;
            playerYCheck = new Rectangle(0, 0, playerSize, playerSize + 2);

        }
        #endregion

        double GetLength(Rectangle rectangle1, Rectangle rectangle2)
        {

            double x1 = rectangle1.X + (rectangle1.Width / 2);
            double x2 = rectangle2.X + (rectangle2.Width / 2);
            double y1 = rectangle1.Y + (rectangle1.Height / 2);
            double y2 = rectangle2.Y + (rectangle2.Height / 2);

            //A^2 = B^2 + C^2
            double length = Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
            return length;
        }

    }
}
