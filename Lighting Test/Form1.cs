using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
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

        //Current Level Renderer
        Action<bool> levelRenderer;

        //Random generator
        Random random = new Random();

        //Lists to store the current loaded tiles, and current tiles Colour Pallette:
        List<Rectangle> currentTiles = new List<Rectangle>();
        List<Rectangle> currentTilePixels = new List<Rectangle>();
        List<int> currentPixelDepths = new List<int>();
        List<int> currentTileDepths = new List<int>();
        List<Color> currentPixelColors = new List<Color>();


        //MOVING SPRITE GENERATION AND INFORMATOIN
        //A list to store currently moving sprites
        List<MovingSprite> movingSprites = new List<MovingSprite>();
        //A list to store places plants can spawn in
        List<Rectangle> soil = new List<Rectangle>();
        int maxSprites = 1;

        int largestSize = 50;
        int largestSubtraction = 1;
        int redAddition = 10;
        int greenAddition = 50;
        int blueAddition = 20;
        int bioluminescenceSize = 5;

        int sampleSuppressor = 4;

        int chanceOfSpawn = 45;
        int chanceOfRemoval = 90;


        int sampleSpriteSize;
        Color sampleColor;

        //all lights on screen
        List<Light> lightList = new List<Light>();

        double[] trueRgb = new double[3];

        int tileWidth = 45;

        Graphics e;


        //storing current Level
        int currentLevel = 0;

        //Key info
        bool[] WSAD = new bool[] { false, false, false, false };
        Keys[] keysToCheck = new Keys[] { Keys.Space, Keys.S, Keys.A, Keys.D };

        //Player info:
        //Player physical bodies
        Rectangle player;
        Rectangle playerXCheck;
        Rectangle playerYCheck;

        //Player movement:
        int[] xYDirection = new int[] { 0, 0 };
        int[] xYSpeed = new int[] { 6, 11 };
        int maxSpeedX = 11;
        int maxSpeedY = 35;

        string jumpState = "accelerating";

        int speedInteger = 8;
        int momentumGain = 3;
        int momentumLoss = 2;

        int previousJump = 0;
        int jumpInterval = 700;

        int[] canMoveUpDownLeftRight = new int[4] { 0, 0, 0, 0 };


        //Spawn points
        Point playerSpawn = new Point(580, 326);
        int playerSpawnLevel = 0;

        //Temperary brushes, I would like to have 0 brushes up here by the time the project is over.
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush blueBrush = new SolidBrush(Color.DodgerBlue);
        SolidBrush whiteBrush = new SolidBrush(Color.White);

        //How large is a row of levels? (how many levels are in a row of levels?) 
        int levelLayoutWidth = 3;

        Stopwatch stopwatch = new Stopwatch();
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
                0,0,1,1,7,1,8,6,5,5,1,1,1,1,0,1,2,3,4,5,6,7,0,0,0,0,0,0,0,0,
                0,0,1,3,7,9,9,7,7,7,1,1,1,1,1,3,4,4,6,7,8,8,1,1,1,0,0,0,0,0,
                0,0,7,7,7,9,9,7,7,7,5,5,1,1,0,3,4,5,6,7,8,9,1,1,1,1,1,0,0,0,
                0,0,1,3,7,9,7,4,5,5,7,5,1,1,0,2,2,4,5,4,7,1,1,1,1,1,1,0,0,0,
                0,0,1,3,7,9,7,4,1,1,7,1,1,1,0,0,0,0,5,5,6,1,1,1,1,1,1,0,0,0,
                0,0,1,3,3,3,3,4,1,1,1,1,1,1,0,1,2,3,2,4,5,1,1,1,1,1,0,0,0,0,
                0,0,1,2,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,4,1,1,1,1,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,3,1,1,1,1,1,1,1,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,1,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,
                0,1,1,2,3,3,7,1,1,1,7,1,1,1,0,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,
                0,1,2,3,4,4,7,1,1,1,7,1,1,1,1,1,1,1,1,1,1,1,4,3,2,1,1,1,1,1,
                0,0,3,4,5,6,1,1,1,1,1,1,1,1,1,1,1,1,1,1,5,6,5,4,3,1,0,0,0,0,
                0,0,2,3,4,5,1,1,1,1,1,1,1,1,0,1,1,1,1,1,4,5,4,3,2,1,0,0,0,0,
                0,0,1,2,3,4,1,1,1,1,1,1,1,1,0,1,1,1,1,1,3,4,3,2,1,1,1,1,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,3,0,0,0,0,0,0,0,0,
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
                4,4,9,10,10,10,10,10,10,10,10,10,10,10,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
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
                7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
                7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
                7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
                7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
                7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
                7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
                7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
                7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
                7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
                7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
                7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
                7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
                7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
                7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
                7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
                7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
                7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
                7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,

                //4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                //4,4,1,1,7,1,1,1,1,1,1,1,1,1,4,7,1,7,1,1,1,1,4,4,4,4,4,4,4,4,
                //4,4,1,1,7,1,1,7,7,7,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,4,
                //4,4,7,7,7,1,1,7,7,7,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,
                //4,4,1,1,7,1,7,1,1,1,7,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,
                //4,4,1,1,7,1,7,1,1,1,7,1,1,1,4,4,4,4,1,1,1,1,1,1,1,1,1,4,4,4,
                //4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,
                //4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,1,1,1,1,1,1,1,1,4,4,4,4,
                //4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,
                //4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                //4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                //4,1,1,1,7,1,7,1,1,1,7,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                //4,1,1,1,7,1,7,1,1,1,7,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                //4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,
                //4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,
                //4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,
                //4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1,1,4,4,4,4,4,4,4,4,
                //4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
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
              new int[]{ //7
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
            new int[]{ //8
            3,
            3,2,4,
            2,3,2,
            1,1,0,
        },
               new int[]{ // 9
            3,
            0,1,8,
            1,5,6,
            10,13,12,
        },
                    new int[]{ // 10
            3,
            1,1,1,
            7,8,7,
            10,8,12,
        },
        };
        #endregion

        #region Load
        public Form1()
        {
            InitializeComponent();
            player = new Rectangle(580, 320, 0, 0);
            lightList.Add(new Light(new Rectangle(580, 320, 0, 0), new int[] { 0, -60, -180 }, 100, 220, 40, 10, 0));
            lightList.Add(new Light(new Rectangle(580, 320, 0, 0), new int[] { 0, -60, -180 }, 10, 10, 10, 10, 0));
            playerXCheck = new Rectangle(0, 0, 0, 0);
            playerYCheck = new Rectangle(0, 0, 0, 0);

            lightList.Add(new Light(new Rectangle(980, 820, 0, 0), new int[] { -60, 0, -180 }, 100, 220, 40, 10, 0));
            lightList.Add(new Light(new Rectangle(380, 120, 0, 0), new int[] { 12, -6, 39 }, 80, 200, 50, 30, 0));
            lightList.Add(new Light(new Rectangle(680, 120, 0, 0), new int[] { 6, -126, -39 }, 80, 240, 50, 30, 0));
            lightList.Add(new Light(new Rectangle(980, 620, 0, 0), new int[] { -122, -6, -39 }, 70, 200, 40, 5, 0));

            createLevel(levelTiles[currentLevel], tileWidth);

            stopwatch.Start();
        }
        #endregion

        #region Game Timer
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            #region Adding Atmospheric Moving Sprites
            //Add rain or snow or nice effects!
            if (random.Next(0, 101) > chanceOfSpawn && movingSprites.Count < maxSprites)
            {
                sampleSpriteSize = random.Next(0, largestSize) / random.Next(1, largestSubtraction);
                int smallerSize = sampleSpriteSize / random.Next(2, 8);
                int red = random.Next(0, redAddition);
                int green = random.Next(0, greenAddition);
                int blue = random.Next(0, blueAddition);
                int yLoc = 0;

                movingSprites.Add(new MovingSprite(new Rectangle(random.Next(0, this.Width - sampleSpriteSize), yLoc, sampleSpriteSize, sampleSpriteSize), new int[] { 0, random.Next(1, 18) }, new int[] { 0, 1 }, Color.FromArgb(red, green, blue), random.Next(-3, 13)));
                movingSprites.Add(new MovingSprite(new Rectangle(random.Next(0, this.Width - smallerSize), yLoc - smallerSize, smallerSize, smallerSize), new int[] { 0, random.Next(1, 18) }, new int[] { 0, 1 }, Color.FromArgb(red / 2, green / 2, blue / 2), random.Next(-3, 13)));
            }

            if (random.Next(0, 101) > chanceOfRemoval && movingSprites.Count > 0)
            {
                if (movingSprites[0].body.Width > 16 && movingSprites.Count < maxSprites)
                {
                    movingSprites.Add(new MovingSprite(new Rectangle(movingSprites[0].body.X + movingSprites[0].body.Width / 2, movingSprites[0].body.Y, movingSprites[0].body.Width / 4, movingSprites[0].body.Width / 4), new int[] { 0, random.Next(1, 18) }, new int[] { 0, 1 }, movingSprites[0].color, random.Next(0, 3)));
                }
                movingSprites.RemoveAt(0);
            }
            #endregion

            //FOR SOME REASON PLAYER SPEED IS LACKING, LOOK INTO IT.
            if (stopwatch.ElapsedMilliseconds % speedInteger == 0)
            {
                //Find out how fast the player should be moving Horizontally!
                //gaining momentum based on walking time
                if (xYSpeed[x] < maxSpeedX && xYDirection[x] != 0) { xYSpeed[x] += momentumGain; }
                //losing momentum over time
                else if (xYSpeed[x] > momentumLoss && xYDirection[x] == 0) { xYSpeed[x] -= momentumLoss; }
            }


            if (WSAD[0] == true && canMoveUpDownLeftRight[down] == 1)
            {
                newJump();
            }
            //Resting: jump speed is at 0
            else if (canMoveUpDownLeftRight[down] == 1)
            {
                xYSpeed[y] = 3;
            }
            //Lifting: decrease speed
            if (jumpState == "decelerating")
            {
                xYDirection[y] = -1;
                if (xYSpeed[y] > 2)
                {
                    xYSpeed[y] -= 2;
                }

                if (WSAD[0] == false || stopwatch.ElapsedMilliseconds - previousJump > jumpInterval / 2)
                {
                    xYSpeed[y] = 0;
                    jumpState = "accelerating";
                }
            }
            //Falling: increase speed
            if (jumpState == "accelerating")
            {
                xYDirection[y] = 1;
                if (xYSpeed[y] < maxSpeedY)
                {
                    xYSpeed[y] += 2;
                }
            }




            #region Determing All Object Directions
            //Determening player directions depending on what keys are pressed:
            if (WSAD[left] == true && WSAD[right] == false)
            {
                xYDirection[x] = -1;
            }
            else if (WSAD[left] == false && WSAD[right] == true)
            {
                xYDirection[x] = 1;
            }
            else
            {
                xYDirection[x] = 0;
            }
            #endregion

            #region Move Player TRY 1
            //if (xYDirection[x] != canMoveUpDownLeftRight[left] && xYDirection[x] != canMoveUpDownLeftRight[right])
            //{
            //    player.X += xYDirection[x] * xYSpeed[x];
            //}

            //if (xYDirection[y] != canMoveUpDownLeftRight[up] && xYDirection[y] != canMoveUpDownLeftRight[down])
            //{
            //    player.Y += xYDirection[y] * xYSpeed[y];
            //}

            //canMoveUpDownLeftRight[up] = 0;
            //canMoveUpDownLeftRight[down] = 0;
            //canMoveUpDownLeftRight[left] = 0;
            //canMoveUpDownLeftRight[right] = 0;


            ////Keep track of the original players position before adjustments
            //int newYlocation = player.Y;
            //int newXlocation = player.X;

            //for (int n = 0; n < currentTiles.Count; n++)
            //{
            //    //CAN PLAYER GO UP DOWN LEFT RIGHT
            //    if (playerYCheck.IntersectsWith(currentTiles[n]) && levelDepths[currentLevel][n] == 0)
            //    {
            //        //TOP WALL
            //        if (playerYCheck.Y >= currentTiles[n].Y && n + 30 <= currentTiles.Count && levelDepths[currentLevel][n + 30] != 0)
            //        {
            //            canMoveUpDownLeftRight[up] = -1;
            //        }
            //        //BOTTOM WALL
            //        if (playerYCheck.Y <= currentTiles[n].Y && n - 30 >= 0 && levelDepths[currentLevel][n - 30] != 0)
            //        {

            //            canMoveUpDownLeftRight[down] = 1;

            //        }
            //    }
            //    if (playerXCheck.IntersectsWith(currentTiles[n]) && levelDepths[currentLevel][n] == 0)
            //    {
            //        //LEFT WALL
            //        if (playerXCheck.X >= currentTiles[n].X && n + 1 <= currentTiles.Count && levelDepths[currentLevel][n + 1] != 0)
            //        {
            //            canMoveUpDownLeftRight[left] = -1;
            //        }
            //        //RIGHT WALL
            //        if (playerXCheck.X <= currentTiles[n].X && n - 1 >= 0 && levelDepths[currentLevel][n - 1] != 0)
            //        {
            //            canMoveUpDownLeftRight[right] = 1;
            //        }
            //    }

            //    //PLAYER IN A TILE
            //    if (player.IntersectsWith(currentTiles[n]) && levelDepths[currentLevel][n] == 0)
            //    {
            //        //LEFT WALL
            //        if (player.X >= currentTiles[n].X && n + 1 <= currentTiles.Count && levelDepths[currentLevel][n + 1] != 0)
            //        {
            //            //player.X += 1;
            //            newXlocation = currentTiles[n].X + tileWidth;
            //        }
            //        //RIGHT WALL
            //        else if (player.X <= currentTiles[n].X && n - 1 >= 0 && levelDepths[currentLevel][n - 1] != 0)
            //        {
            //            //player.X += -1;
            //            newXlocation = currentTiles[n].X - player.Width;
            //        }
            //        //TOP WALL
            //        else if (player.Y >= currentTiles[n].Y && n + 30 <= currentTiles.Count && levelDepths[currentLevel][n + 30] != 0)
            //        {
            //            //player.Y += 1;
            //            newYlocation = currentTiles[n].Y + tileWidth;
            //        }
            //        //BOTTOM WALL
            //        else if (player.Y <= currentTiles[n].Y && n - 30 >= 0 && levelTiles[currentLevel][n - 30] != 0)
            //        {
            //            //player.Y += -1;
            //            newYlocation = currentTiles[n].Y - player.Width;
            //        }

            //        //Preform all adjustments AFTER checks have been made
            //        player.Y = newYlocation;
            //        player.X = newXlocation;
            //    }
            //}
            #endregion
            #region Move Player TRY 2

            canMoveUpDownLeftRight[up] = 0;
            canMoveUpDownLeftRight[down] = 0;
            canMoveUpDownLeftRight[left] = 0;
            canMoveUpDownLeftRight[right] = 0;

            //Rectangle to check the players next move
            Rectangle proposedMove = new Rectangle(player.X, player.Y, player.Width, player.Height);

            //Move the rectangle to be in players position
            int Xmove = xYDirection[x] * xYSpeed[x];
            int Ymove = xYDirection[y] * xYSpeed[y];

            proposedMove.X += Xmove;
            proposedMove.Y += Ymove;

            //For each tile, does the new move interstect with it?
            for (int n = 0; n < currentTiles.Count; n++)
            {
                #region UpDownLeftRight
                //CAN PLAYER GO UP DOWN LEFT RIGHT
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
                #endregion

                if (proposedMove.IntersectsWith(currentTiles[n]) && levelDepths[currentLevel][n] == 0)
                {
                    int Xdistance = Math.Abs((currentTiles[n].X + (currentTiles[n].Width / 2)) - (player.X + (player.Width / 2)));
                    int Ydistance = Math.Abs((currentTiles[n].Y + (currentTiles[n].Height / 2)) - (player.Y + (player.Height / 2)));

                    //If the player would not be where they are supposed to be.. find how far they can go!
                    //LEFT WALL
                    if (player.X >= currentTiles[n].X && n + 1 <= currentTiles.Count && levelDepths[currentLevel][n + 1] != 0 && Xdistance > Ydistance)
                    {
                        Xmove = (player.X - (currentTiles[n].Width + currentTiles[n].X)) * -1;
                    }
                    //RIGHT WALL
                    else if (player.X <= currentTiles[n].X && n - 1 >= 0 && levelDepths[currentLevel][n - 1] != 0 && Xdistance > Ydistance)

                    {
                        Xmove = currentTiles[n].X - player.X - player.Width;
                    }
                    //TOP WALL
                    else if (player.Y >= currentTiles[n].Y && n + 30 <= currentTiles.Count && levelDepths[currentLevel][n + 30] != 0 && Xdistance < Ydistance)
                    {
                        Ymove = (player.Y - (currentTiles[n].Y + currentTiles[n].Height)) * -1;
                    }
                    //BOTTOM WALL
                    else if (player.Y <= currentTiles[n].Y && n - 30 >= 0 && levelTiles[currentLevel][n - 30] != 0 && Xdistance < Ydistance)
                    {
                        Ymove = currentTiles[n].Y - player.Y - player.Height;
                    }
                }
            }
            //Finally move the player.

            player.X += Xmove;
            player.Y += Ymove;

            #endregion

            #region Passing Through Levels
            if (player.Y <= player.Height / 2)
            {
                player.Y = this.Height - player.Height;
                currentLevel += levelLayoutWidth * -1;
                createLevel(levelTiles[currentLevel], tileWidth);
                newJump();
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
            label1.Text = $"{WSAD[0]} {WSAD[1]} {WSAD[2]} {WSAD[3]}\n{xYDirection[0]} {xYDirection[1]}\n{canMoveUpDownLeftRight[up]} {canMoveUpDownLeftRight[down]} {canMoveUpDownLeftRight[left]} {canMoveUpDownLeftRight[right]} ";
            levelRenderer(false);

            //Move Moving Sprites
            for (int i = 0; i < movingSprites.Count; i++)
            {
                //move the sprite
                movingSprites[i].body.Y += movingSprites[i].xySpeed[y];
                //if the sprite is off the screen remove the sprite
                if (movingSprites[i].body.Y > this.Height - movingSprites[i].body.Height)
                {
                    movingSprites.RemoveAt(i);
                }
            }
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
            movingSprites.Clear();

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
                if (n > 0 && n <= 540)
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

            lightList[0].body.Location = new Point(player.X - playerSize / 2, player.Y - playerSize / 2);
            playerXCheck = new Rectangle(0, 0, playerSize + 2, playerSize); ;
            playerYCheck = new Rectangle(0, 0, playerSize, playerSize + 2);
            //Define Graphics
            e = this.CreateGraphics();

            //If we are in a large level
            if (currentTilePixels.Count > 70000)
            {
                //set the character light to something small
                lightList[0] = (new Light(new Rectangle(580, 320, 0, 0), new int[] { -400, -400, -400 }, 0, 0, 0, 0, 0));
                //set the current paint method to a large room renderer
                levelRenderer = largePaint;

            }
            //otherwise
            else
            {
                //set the character light to something fancier
                lightList[0] = (new Light(new Rectangle(580, 320, 0, 0), new int[] { 0, -60, -180 }, 170, 200, 130, 80, 0));
                //lightList[0] = (new Light(new Rectangle(580, 320, 0, 0), new int[] { 0, -60, -180 }, 100, 220, 40, 10, 0));
                //set the current paint method to a large room renderer
                levelRenderer = smallPaint;
            }

            levelRenderer(true);
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

        //This version of paint doesnt refresh the screen each time! instead of drawing 5000 pixels it only draws the ones that change! This took my 'Process Memory' panel output from 56 to 32!

        #region Large Paint
        void largePaint(bool loadWholeLevel)
        {
            //Setting up the pixel colors list
            if (currentPixelColors.Count != currentTilePixels.Count)
            {
                //clear the list, and for every pixel that should be on the screen, add in a replacable color.
                currentPixelColors.Clear();

                for (int n = 0; n < currentTilePixels.Count; n++)
                {
                    currentPixelColors.Add(Color.White);
                }
            }

            //For each rectangle on screen
            for (int n = 0; n < currentTilePixels.Count; n++)
            {
                //Finds if there is a spot in the Moving Sprites list where the pixel overlaps with a sprite
                int intersectsWith = checkList(currentTilePixels[n], movingSprites);
                if (intersectsWith != -1)
                {
                    //Redraws the pixel if outside the radius of the moving sprite
                    if (GetLength(movingSprites[intersectsWith].body, currentTilePixels[n]) > movingSprites[intersectsWith].body.Width / 1.9)
                    {
                        e.FillRectangle(new SolidBrush(currentPixelColors[n]), currentTilePixels[n]);
                    }
                    //draws the pixel as part of the moving sprite
                    else if (currentTileDepths[n] >= movingSprites[intersectsWith].depth)
                    {
                        try
                        {
                            if (movingSprites[intersectsWith].body.Width > bioluminescenceSize)
                            {
                                e.FillRectangle(new SolidBrush(Color.FromArgb(currentPixelColors[n].R + movingSprites[intersectsWith].color.R + sampleColor.R / sampleSuppressor, currentPixelColors[n].G + movingSprites[intersectsWith].color.G + sampleColor.G / sampleSuppressor, currentPixelColors[n].B + movingSprites[intersectsWith].color.B + sampleColor.B / sampleSuppressor)), currentTilePixels[n]);
                            }
                            else
                            {
                                sampleColor = Color.FromArgb(currentPixelColors[n].R + movingSprites[intersectsWith].color.R + sampleColor.R / 4, currentPixelColors[n].G + movingSprites[intersectsWith].color.G + sampleColor.G / 4, currentPixelColors[n].B + movingSprites[intersectsWith].color.B + sampleColor.B / 4);
                                if (currentPixelColors[n] != sampleColor)
                                {
                                    currentPixelColors[n] = sampleColor;

                                    e.FillRectangle(new SolidBrush(sampleColor), currentTilePixels[n]);
                                }
                            }
                        }
                        catch { movingSprites.RemoveAt(intersectsWith); }
                    }
                }
                else if (GetLength(currentTilePixels[n], player) < 103 || loadWholeLevel == true)
                {
                    if (currentTilePixels[n].IntersectsWith(player))
                    {
                        sampleColor = Color.White;
                        if (currentPixelColors[n] != sampleColor)
                        {
                            currentPixelColors[n] = sampleColor;

                            e.FillRectangle(new SolidBrush(sampleColor), currentTilePixels[n]);
                        }

                    }
                    else
                    {
                        //Reset the trueRgb array
                        trueRgb = new double[3] { 0, 0, 0 };

                        //For each light that can affect the rectangle (lights on screen)
                        for (int i = Convert.ToInt32(loadWholeLevel); i < lightList.Count; i++)
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
                        sampleColor = (Color.FromArgb(Convert.ToInt32(trueRgb[0]), Convert.ToInt32(trueRgb[1]), Convert.ToInt32(trueRgb[2])));

                        //To avoid drawing things over again (This is reducing lag!)
                        if (currentPixelColors[n] != sampleColor)
                        {
                            currentPixelColors[n] = sampleColor;
                            try { e.FillRectangle(new SolidBrush(currentPixelColors[n]), currentTilePixels[n]); }
                            catch { }
                        }
                    }
                }
            }
        }
        #endregion

        #region Small Paint
        void smallPaint(bool loadWholeLevel)
        {
            if (currentPixelColors.Count != currentTilePixels.Count)
            {
                currentPixelColors.Clear();

                for (int n = 0; n < currentTilePixels.Count; n++)
                {
                    currentPixelColors.Add(Color.White);
                }
            }
            //For each rectangle on screen
            for (int n = 0; n < currentTilePixels.Count; n++)
            {
                if (currentTilePixels[n].IntersectsWith(player))
                {
                    sampleColor = Color.White;
                    if (currentPixelColors[n] != sampleColor)
                    {
                        currentPixelColors[n] = sampleColor;
                        e.FillRectangle(new SolidBrush(sampleColor), currentTilePixels[n]);
                    }
                }
                else
                {
                    //Reset the trueRgb array
                    trueRgb = new double[3] { 0, 0, 0 };

                    //For each light that can affect the rectangle (lights on screen)
                    for (int i = Convert.ToInt32(loadWholeLevel); i < lightList.Count; i++)
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
                    sampleColor = (Color.FromArgb(Convert.ToInt32(trueRgb[0]), Convert.ToInt32(trueRgb[1]), Convert.ToInt32(trueRgb[2])));

                    //To avoid drawing things over again (This is reducing lag!)
                    if (currentPixelColors[n] != sampleColor)
                    {
                        currentPixelColors[n] = sampleColor;
                        try { e.FillRectangle(new SolidBrush(currentPixelColors[n]), currentTilePixels[n]); }
                        catch { }
                    }
                }
            }
        }
        #endregion

        void newJump()
        {
            xYDirection[y] = -1;
            xYSpeed[y] = maxSpeedY;
            previousJump = Convert.ToInt32(stopwatch.ElapsedMilliseconds);
            jumpState = "decelerating";
        }

        int checkList(Rectangle rectangle, List<MovingSprite> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (rectangle.IntersectsWith(list[i].body))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
