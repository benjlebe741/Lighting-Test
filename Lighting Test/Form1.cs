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
        #region GLOBAL VARIABLES
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
        List<int> currentPixelDepths = new List<int>();
        List<int> currentTileDepths = new List<int>();


        //all lights on screen
        List<Light> lightList = new List<Light>();

        double[] trueRgb = new double[3];

        int tileWidth = 45;
        SolidBrush changingBrush;

        //storing current Level
        int currentLevel;



        //Key info
        bool[] WSAD = new bool[] { false, false, false, false };
        Keys[] keysToCheck = new Keys[] { Keys.W, Keys.S, Keys.A, Keys.D };

        //Player info:
        //Player physical bodies
        Rectangle player;
        Rectangle playerXCheck;
        Rectangle playerYCheck;

        //Player movement:
        int[] xYDirection = new int[] { 0, 0 };
        int[] xYSpeed = new int[] { 6, 6 };

        int[] canMoveUpDownLeftRight = new int[4] { 0, 0, 0, 0 };

        int[][] determineDirectionsList = new int[][]
         {
      new int[]{  0,1,1}, //Up, Down, Affect Vertical Direction
      new int[]{  2,3,0}, //Left, Right, Affect Horizontal Direction
         };


        //Spawn points
        Point playerSpawn = new Point(580, 326);
        int playerSpawnLevel = 0;

        //Temperary brushes, I would like to have 0 brushes up here by the time the project is over.
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush blueBrush = new SolidBrush(Color.DodgerBlue);
        SolidBrush whiteBrush = new SolidBrush(Color.White);

        //How large is a row of levels? (how many levels are in a row of levels?) 
        int levelLayoutWidth = 3;
        #endregion

        #region LEVEL DEPTHS:
        int[][] levelDepths = new int[][]
               {
                new int[]
                {
                30,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,
                0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,
                0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                0,0,2,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,1,1,0,0,0,
                0,0,2,3,3,3,3,2,2,3,3,4,5,6,5,4,3,2,1,1,1,1,1,1,1,1,1,1,0,0,
                0,0,1,2,3,3,2,2,1,2,3,3,4,5,4,3,2,1,1,1,1,1,1,1,1,1,1,1,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
            },
                 new int[]
                {
                30,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                0,0,7,7,7,1,1,10,10,100,1,1,1,1,0,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,
                0,0,7,7,7,1,1,10,10,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
                0,0,7,7,7,40,100,100,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,
                0,0,1,-30,-1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,
                0,0,10,20,30,40,50,60,70,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                0,0,1,13,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
                0,0,7,13,13,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
                0,0,7,7,7,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            },

        new int[]
                {
                30,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,
                0,0,1,1,1,1,1,7,7,7,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
                0,0,1,1,1,1,1,7,7,7,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,
                0,0,1,1,7,1,7,1,1,1,7,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,
                0,0,1,1,7,1,7,1,1,1,7,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,
                1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,
                0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
            },
        //NEXT LAYER
    new int[]
                {
                30,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                0,0,1,1,7,1,1,1,1,1,1,1,1,1,0,7,1,7,1,1,1,1,0,0,0,0,0,0,0,0,
                0,0,1,1,7,1,1,7,7,7,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
                0,0,7,7,7,1,1,7,7,7,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,
                0,0,1,1,7,1,7,1,1,1,7,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,
                0,0,1,1,7,1,7,1,1,1,7,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                0,1,1,1,7,1,7,1,1,1,7,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                0,1,1,1,7,1,7,1,1,1,7,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            },
     new int[]
                {
                30,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                0,0,1,1,7,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,
                0,0,1,1,7,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
                0,0,7,7,7,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                1,1,1,1,1,1,0,1,1,1,0,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,0,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            },
new int[]
       {
                30,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                0,0,1,1,7,1,1,1,1,1,1,1,1,1,0,7,1,7,1,1,1,1,0,0,0,0,0,0,0,0,
                0,0,1,1,7,1,1,7,7,7,1,1,1,1,0,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,
                0,0,7,7,7,1,1,7,7,7,1,1,1,1,0,7,1,1,1,1,1,1,1,1,0,1,1,0,0,0,
                0,0,1,1,7,1,7,1,1,1,7,1,1,1,0,1,1,1,1,1,1,1,1,1,0,1,1,0,0,0,
                0,0,1,1,7,1,7,1,1,1,7,1,1,1,0,0,0,0,1,1,1,1,1,1,0,1,1,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,0,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,0,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,
                1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,

            },
};
        #endregion

        #region LEVEL TILES
        int[][] levelTiles = new int[][]
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
                4,5,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,
                4,6,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,4,4,
                4,5,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,3,3,7,1,1,1,4,4,4,4,4,4,4,
                4,4,4,4,4,4,4,4,4,3,3,3,3,4,4,4,4,4,4,7,1,1,4,4,4,4,4,4,4,4,
                4,4,9,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                4,6,3,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                4,4,3,2,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,1,1,4,4,4,
                4,6,3,3,3,2,3,3,3,3,6,6,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,
                4,4,3,2,2,3,2,2,3,3,6,6,6,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,
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
                4,4,7,7,7,7,7,7,7,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,
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
              6,6,6,
            10,100,10,
            6,6,6,
        },
         new int[]{
            3,
            2,2,2,
            1,1,2,
            1,2,2,
        },
             new int[]{
            3,
            1,0,0,
            0,1,0,
            0,0,1,
        },
        new int[]{
            3,
            1,1,1,
            1,0,1,
            1,1,1,
        },
          new int[]{
            3,
     1,0,1,
     1,3,1,
     1,1,1,
        },
              new int[]{
            3,
            1,0,1,
            2,3,2,
            3,2,0,
        },
              new int[]{
            15,
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            1,2,3,3,1,1,2,3,3,1,1,2,3,3,1,
            1,3,1,2,1,1,3,1,2,1,1,3,1,2,1,
            1,2,3,2,1,1,2,3,2,1,1,2,3,2,1,
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            1,2,3,3,1,1,2,3,3,1,1,2,3,3,1,
            1,3,1,2,1,1,3,1,2,1,1,3,1,2,1,
            1,2,3,2,1,1,2,3,2,1,1,2,3,2,1,
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            1,2,3,3,1,1,2,3,3,1,1,2,3,3,1,
            1,3,1,2,1,1,3,1,2,1,1,3,1,2,1,
            1,2,3,2,1,1,2,3,2,1,1,2,3,2,1,
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
        },
            new int[]{
            3,
            3,2,4,
            2,3,2,
            1,1,0,
        },
               new int[]{
            3,
            0,1,8,
            1,5,6,
            10,13,12,
        },
        };
        #endregion

        #region Load
        public Form1()
        {
            InitializeComponent();
            player = new Rectangle(580, 320, 0, 0);
            lightList.Add(new Light(new Rectangle(580, 320, 0, 0), new int[] { 0, -60, -180 }, 100, 220, 40, 10, 100, 0));
            playerXCheck = new Rectangle(0, 0, 0, 0);
            playerYCheck = new Rectangle(0, 0, 0, 0);

            //     lightList.Add(new Light(new Rectangle(980, 820, 0, 0), new int[] { -60, 0, -180 }, 100, 220, 40, 10, 100, 0));
            //     lightList.Add(new Light(new Rectangle(380, 120, 0, 0), new int[] { 12, -6, 39 }, 80, 200, 50, 30, 100, 0));
            lightList.Add(new Light(new Rectangle(980, 620, 0, 0), new int[] { -122, -6, -39 }, 70, 200, 40, 5, 100, 0));

            createLevel(levelTiles[0], tileWidth);
            currentLevel = 0;
        }
        #endregion

        #region Game Timer
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
                if (playerYCheck.IntersectsWith(currentTiles[n]) && levelDepths[currentLevel][n] == 0)
                {
                    //TOP WALL
                    if (playerYCheck.Y >= currentTiles[n].Y && n + 30 <= currentTiles.Count && levelDepths[currentLevel][n + 30] != 0)
                    {
                        canMoveUpDownLeftRight[up] = -1;
                    }
                    //BOTTOM WALL
                    if (playerYCheck.Y <= currentTiles[n].Y && n - 30 >= 0 && levelDepths[currentLevel][n - 30] != 0)
                    {

                        canMoveUpDownLeftRight[down] = 1;

                    }
                }

                if (playerXCheck.IntersectsWith(currentTiles[n]) && levelDepths[currentLevel][n] == 0)
                {
                    //LEFT WALL
                    if (playerXCheck.X >= currentTiles[n].X && n + 1 <= currentTiles.Count && levelDepths[currentLevel][n + 1] != 0)
                    {
                        canMoveUpDownLeftRight[left] = -1;
                    }
                    //RIGHT WALL
                    if (playerXCheck.X <= currentTiles[n].X && n - 1 >= 0 && levelDepths[currentLevel][n - 1] != 0)
                    {
                        canMoveUpDownLeftRight[right] = 1;
                    }
                }

                if (player.IntersectsWith(currentTiles[n]) && levelDepths[currentLevel][n] == 0)
                {
                    //TOP WALL
                    if (player.Y >= currentTiles[n].Y && n + 30 <= currentTiles.Count && levelDepths[currentLevel][n + 30] != 0)
                    {
                        player.Y += 1;
                    }
                    //BOTTOM WALL
                    if (player.Y <= currentTiles[n].Y && n - 30 >= 0 && levelTiles[currentLevel][n - 30] != 0)
                    {
                        player.Y += -1;
                    }
                    //LEFT WALL
                    if (player.X >= currentTiles[n].X && n + 1 <= currentTiles.Count && levelDepths[currentLevel][n + 1] != 0)
                    {
                        player.X += 1;
                    }
                    //RIGHT WALL
                    if (player.X <= currentTiles[n].X && n - 1 >= 0 && levelDepths[currentLevel][n - 1] != 0)
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
                createLevel(levelTiles[currentLevel], tileWidth);
            }
            if (player.Y >= this.Height - player.Height / 2)
            {
                player.Y = player.Height;
                currentLevel += levelLayoutWidth;
                createLevel(levelTiles[currentLevel], tileWidth);
            }
            if (player.X <= player.Height / 2)
            {
                player.X = this.Width - player.Height;
                currentLevel += -1;
                createLevel(levelTiles[currentLevel], tileWidth);
            }
            if (player.X >= this.Width - player.Height / 2)
            {
                player.X = player.Height;
                currentLevel += 1;
                createLevel(levelTiles[currentLevel], tileWidth);
            }
            #endregion

            //Tracking player accessories
            playerXCheck.Location = new Point(player.X - 1, player.Y);
            playerYCheck.Location = new Point(player.X, player.Y - 1);
            lightList[0].body.Location = new Point(player.X + (player.Width / 2), player.Y + (player.Width / 2));

            //My info
            //label1.Text = $"{WSAD[0]} {WSAD[1]} {WSAD[2]} {WSAD[3]}\n{xYDirection[0]} {xYDirection[1]}\n{canMoveUpDownLeftRight[up]} {canMoveUpDownLeftRight[down]} {canMoveUpDownLeftRight[left]} {canMoveUpDownLeftRight[right]} ";
            Refresh();
        }
        #endregion

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

        #region Paint
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (currentLevel == 0)
            {
                int x = 8;
            }
            //For each rectangle on screen
            for (int n = 0; n < currentTilePixels.Count; n++)
            {
                //Reset the trueRgb array
                trueRgb = new double[3] { 0, 0, 0 };

                //For each light that can affect the rectangle (lights on screen)
                for (int i = 0; i < lightList.Count; i++)
                {
                    //For each Red, Green, Or blue value of the rectangle's color
                    for (int num = 0; num < 3; num++)
                    {
                        //If the rectangle is in range of the light (rectangle would not be siloughetted by the light in 3D space)
                            if (currentTileDepths[n] >= lightList[i].depth && currentPixelDepths[n] > lightList[i].depth)
                            {
                                lightList[i].rgbStorage[num] = (20000 / GetLength(lightList[i].body, currentTilePixels[n]) / (currentPixelDepths[n] - (lightList[i].depth))) + lightList[i].rgbAffectors[num];
                            }
                            if (lightList[i].rgbStorage[num] > lightList[i].lightenPoint) { lightList[i].rgbStorage[num] += 20; }
                            if (lightList[i].rgbStorage[num] > lightList[i].maxBright) { lightList[i].rgbStorage[num] = lightList[i].maxBright; }
                            if (lightList[i].rgbStorage[num] < lightList[i].darkenPoint) { lightList[i].rgbStorage[num] -= 12; }
                            if (lightList[i].rgbStorage[num] < lightList[i].blackPoint) { lightList[i].rgbStorage[num] = 0; }
                    }
                    //Now that we have found the way the lights affect all pixels, find the brightest light per pixel
                    for (int j = 0; j < 3; j++)
                    {
                        if (lightList[i].rgbStorage[j] > trueRgb[j] && currentTileDepths[n] >= lightList[i].depth)
                        {
                            trueRgb[j] = lightList[i].rgbStorage[j];
                        }
                    }
                }
                try { e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(Convert.ToInt32(trueRgb[0]), Convert.ToInt32(trueRgb[1]), Convert.ToInt32(trueRgb[2]))), currentTilePixels[n]); }
                catch { }
            }
            e.Graphics.FillRectangle(whiteBrush, playerXCheck);
            e.Graphics.FillRectangle(whiteBrush, playerYCheck);
            e.Graphics.FillRectangle(redBrush, player);
        }
        #endregion

        #region Create Level
        private void createLevel(int[] level, int rectangleDimension)
        {
            this.Width = rectangleDimension * 30;
            this.Height = rectangleDimension * ((level.Length - 1) / 30);
            this.CenterToScreen();
            //Reset the current loaded level
            currentTiles.Clear();
            currentPixelDepths.Clear();
            currentTileDepths.Clear();
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

                    for (int m = 0; m < tilePatterns[thisTile].Length; m++)
                    {
                        int tilePixelX = m;
                        while (tilePixelX > tilePatterns[thisTile][0])
                        {
                            tilePixelX -= tilePatterns[thisTile][0];
                        }

                        int pixelX = (tilePixelX - 1) * pixelDimension;
                        int pixelY = (tilePixelY - 1) * pixelDimension;
                        //Now that we have the x and y point of the rectangle, put it (one pixel of the rectangle) into physical space, aswell as adding the integer storing its color value into its list.
                        if (m > 0)
                        {
                            currentTilePixels.Add(new Rectangle(pixelX + rectangleX, pixelY + rectangleY, pixelDimension, pixelDimension));
                            currentPixelDepths.Add(tilePatterns[thisTile][m] + (levelDepths[currentLevel][n]));
                            currentTileDepths.Add(levelDepths[currentLevel][n]);
                        }
                        if ((m) % (tilePatterns[thisTile][0]) == 0)
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
