using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace Lighting_Test
{
    internal class Entity
    {
        //Shorthand
        int x = 0;
        int y = 1;
        int up = 0;
        int down = 1;
        int left = 2;
        int right = 3;

        public MovingSprite sprite;
        Rectangle oldSprite;

        Rectangle entityXCheck;
        Rectangle entityYCheck;

        int maxSpeedX = 5;
        int maxSpeedY = 25;
        int momentumGain = 3;
        int momentumLoss = 2;

        int[] canMoveUpDownLeftRight = new int[4] { 0, 0, 0, 0 };

        int speedInteger = 8;

        string jumpState;
        int previousJump = 0;
        int jumpInterval = 700;
        public int lifespan = 0;
        public int birth = 0;
        public bool immortal = true;

        //'Immortal' entities; enemies, player etc!
        public Entity(MovingSprite _sprite)
        {
            sprite = _sprite;
            entityXCheck = new Rectangle(0, 0, sprite.body.Width + 2, sprite.body.Height); ;
            entityYCheck = new Rectangle(0, 0, sprite.body.Width, sprite.body.Height + 2);
        }

        //'dying' entities; attacks, particles? elements that dont pass through levels and dissapear eventually, these also can hold a specific direction they travel in, not automous
        public Entity(MovingSprite _sprite, int _lifeSpan, int _birth)
        {
            sprite = _sprite;
            entityXCheck = new Rectangle(0, 0, sprite.body.Width + 2, sprite.body.Height); ;
            entityYCheck = new Rectangle(0, 0, sprite.body.Width, sprite.body.Height + 2);
            lifespan = _lifeSpan;
            birth = _birth;

            immortal = false;

        }

        public void adjustSpeeds(int time, Rectangle player)
        {
            if (immortal == true)
            {
                int directionOfPlayer = (player.X - sprite.body.X) / Math.Abs(player.X - sprite.body.X);
                int playerYdistance = (Math.Abs(player.Y - sprite.body.Y));

                if (directionOfPlayer == sprite.xyDirection[x] && playerYdistance < 1000)
                {
                    sprite.xySpeed[x] = maxSpeedX * 2;
                }
                else if (sprite.xySpeed[x] >= maxSpeedX) { sprite.xySpeed[x] = 3; }

                //Find out how fast the entity should be moving Horizontally!
                //gaining momentum based on walking time
                if (sprite.xySpeed[x] < maxSpeedX && sprite.xyDirection[x] != 0) { sprite.xySpeed[x] += momentumGain; }
                //losing momentum over time
                else if (sprite.xySpeed[x] > momentumLoss && sprite.xyDirection[x] == 0) { sprite.xySpeed[x] -= momentumLoss; }

                if (canMoveUpDownLeftRight[down] == 1 && ((canMoveUpDownLeftRight[right] == 1 && sprite.xyDirection[x] == 1) || (canMoveUpDownLeftRight[left] == -1 && sprite.xyDirection[x] == -1)) && time - previousJump > jumpInterval)
                {
                    newJump(time);
                    sprite.xyDirection[x] = directionOfPlayer;
                }
                //Resting: jump speed is at 0
                else if (canMoveUpDownLeftRight[down] == 1)
                {
                    sprite.xySpeed[y] = 3;
                    jumpState = "resting";
                }
                else if (time - previousJump > jumpInterval / 2 || (canMoveUpDownLeftRight[up] == -1 && time - previousJump > jumpInterval / 3))
                {
                    jumpState = "accelerating";
                }
                else { jumpState = "decelerating"; }

                //Lifting: decrease speed
                if (jumpState == "decelerating")
                {
                    sprite.xyDirection[y] = -1;
                    if (sprite.xySpeed[y] > 2)
                    {
                        sprite.xySpeed[y] -= 1;
                    }
                }
                //Falling: increase speed
                if (jumpState == "accelerating")
                {
                    sprite.xyDirection[y] = 1;
                    if (sprite.xySpeed[y] < maxSpeedY)
                    {
                        sprite.xySpeed[y] += 2;
                    }
                }
            }
        }
        public void moveEntity(List<Rectangle> currentTiles, int currentLevelY, int currentLevelX, Level[][] allLevels)
        {
            oldSprite = sprite.body;

            #region Move Sprite

            canMoveUpDownLeftRight[up] = 0;
            canMoveUpDownLeftRight[down] = 0;
            canMoveUpDownLeftRight[left] = 0;
            canMoveUpDownLeftRight[right] = 0;

            //Rectangle to check the entities next move
            Rectangle proposedMove = new Rectangle(sprite.body.X, sprite.body.Y, sprite.body.Width, sprite.body.Height);

            //Move the rectangle to be in entities position
            int Xmove = sprite.xyDirection[x] * sprite.xySpeed[x];
            int Ymove = sprite.xyDirection[y] * sprite.xySpeed[y];

            proposedMove.X += Xmove;
            proposedMove.Y += Ymove;

            //For each tile, does the new move interstect with it?
            for (int n = 0; n < currentTiles.Count; n++)
            {
                #region UpDownLeftRight
                //CAN ENTITY GO UP? DOWN? LEFT? RIGHT?
                if (entityYCheck.IntersectsWith(currentTiles[n]) && allLevels[currentLevelY][currentLevelX].depths[n] == 0)
                {
                    //TOP WALL
                    if (entityYCheck.Y >= currentTiles[n].Y && n + 30 <= currentTiles.Count && allLevels[currentLevelY][currentLevelX].depths[n + 30] != 0)
                    {
                        canMoveUpDownLeftRight[up] = -1;
                    }
                    //BOTTOM WALL
                    if (entityYCheck.Y <= currentTiles[n].Y && n - 30 >= 0 && allLevels[currentLevelY][currentLevelX].depths[n - 30] != 0)
                    {

                        canMoveUpDownLeftRight[down] = 1;

                    }
                }
                if (entityXCheck.IntersectsWith(currentTiles[n]) && allLevels[currentLevelY][currentLevelX].depths[n] == 0)
                {
                    //LEFT WALL
                    if (entityXCheck.X >= currentTiles[n].X && n + 1 <= currentTiles.Count && allLevels[currentLevelY][currentLevelX].depths[n + 1] != 0)
                    {
                        canMoveUpDownLeftRight[left] = -1;
                    }
                    //RIGHT WALL
                    if (entityXCheck.X <= currentTiles[n].X && n - 1 >= 0 && allLevels[currentLevelY][currentLevelX].depths[n - 1] != 0)
                    {
                        canMoveUpDownLeftRight[right] = 1;
                    }
                }
                #endregion

                if (proposedMove.IntersectsWith(currentTiles[n]) && allLevels[currentLevelY][currentLevelX].depths[n] == 0)
                {
                    int Xdistance = Math.Abs((currentTiles[n].X + (currentTiles[n].Width / 2)) - (sprite.body.X + (sprite.body.Width / 2)));
                    int Ydistance = Math.Abs((currentTiles[n].Y + (currentTiles[n].Height / 2)) - (sprite.body.Y + (sprite.body.Height / 2)));

                    //If the entity would not be where they are supposed to be.. find how far they can go! (if entity is in a wall, how far could they go from their current position before they hit the wall?)
                    //LEFT WALL
                    if (sprite.body.X >= currentTiles[n].X && n + 1 <= currentTiles.Count && allLevels[currentLevelY][currentLevelX].depths[n + 1] != 0 && Xdistance > Ydistance)
                    {
                        Xmove = (sprite.body.X - (currentTiles[n].Width + currentTiles[n].X)) * -1;
                    }
                    //RIGHT WALL
                    else if (sprite.body.X <= currentTiles[n].X && n - 1 >= 0 && allLevels[currentLevelY][currentLevelX].depths[n - 1] != 0 && Xdistance > Ydistance)

                    {
                        Xmove = currentTiles[n].X - sprite.body.X - sprite.body.Width;
                    }
                    //TOP WALL  
                    else if (sprite.body.Y >= currentTiles[n].Y && n + 30 <= currentTiles.Count && allLevels[currentLevelY][currentLevelX].depths[n + 30] != 0 && Xdistance < Ydistance)
                    {
                        Ymove = (sprite.body.Y - (currentTiles[n].Y + currentTiles[n].Height)) * -1;
                    }
                    //BOTTOM WALL
                    else if (sprite.body.Y <= currentTiles[n].Y && n - 30 >= 0 && allLevels[currentLevelY][currentLevelX].depths[n - 30] != 0 && Xdistance < Ydistance)
                    {
                        Ymove = currentTiles[n].Y - sprite.body.Y - sprite.body.Height;
                    }
                }
            }
            //Finally move the entity to adjusted position.

            if (sprite.xyDirection[x] != canMoveUpDownLeftRight[left] && sprite.xyDirection[x] != canMoveUpDownLeftRight[right])
            {
                sprite.body.X += Xmove;
            }

            if (sprite.xyDirection[y] != canMoveUpDownLeftRight[up] && sprite.xyDirection[y] != canMoveUpDownLeftRight[down])
            {
                sprite.body.Y += Ymove;
            }

            #endregion

            //Tracking entity accessories
            entityXCheck.Location = new Point(sprite.body.X - 1, sprite.body.Y);
            entityYCheck.Location = new Point(sprite.body.X, sprite.body.Y - 1);
        }
        void newJump(int time)
        {
            sprite.xyDirection[y] = -1;
            sprite.xySpeed[y] = maxSpeedY;
            previousJump = time;
            jumpState = "decelerating";
        }


        #region Passing Through Levels

        public Tuple<int, int, int, int> levelPassCheck(int height, int width)
        {
            int levelXChange = 0;
            int levelYChange = 0;
            int xCoord = sprite.body.X;
            int yCoord = sprite.body.Y;

            if (immortal == false) { return Tuple.Create(levelXChange, levelYChange, xCoord, yCoord); }

            //Entity passes through:
            //Top wall
            if (sprite.body.Y <= (sprite.body.Height / 2))
            {
                levelYChange = -1;
                yCoord = height - sprite.body.Height;
            }
            //Bottom Wall
            else if (sprite.body.Y >= (height - sprite.body.Height / 2))
            {
                levelYChange = 1;
                yCoord = sprite.body.Height;
            }
            //Left wall
            else if (sprite.body.X <= (sprite.body.Height / 2))
            {
                levelXChange = -1;
                xCoord = width - sprite.body.Height;
            }
            //Right wall
            else if (sprite.body.X >= (width - sprite.body.Height / 2))
            {
                levelXChange = 1;
                xCoord = sprite.body.Height;
            }
            return Tuple.Create(levelXChange, levelYChange, xCoord, yCoord);
        }
        #endregion

    }
}
