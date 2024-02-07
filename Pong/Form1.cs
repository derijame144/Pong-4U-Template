/*
 * Description:     A basic PONG simulator
 * Author:           
 * Date:            
 */

#region libraries

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;
using System.Security.Cryptography;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values

        Random rng = new Random();
        int random;

        Image mushroom = Properties.Resources.mushroom;
        Rectangle shroom = new Rectangle(1000000, 1000000, 50, 50);
        bool mushr00m = false;

        Boolean bpUp = true;
        Boolean rpUp = false;
        Boolean portal = false;

        //graphics objects for drawing
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush blueBrush = new SolidBrush(Color.Blue);
        Font drawFont = new Font("Courier New", 10);

        // Sounds for game
        SoundPlayer scoreSound = new SoundPlayer(Properties.Resources.score);
        SoundPlayer collisionSound = new SoundPlayer(Properties.Resources.collision);

        //determines whether a key is being pressed or not
        Boolean wKeyDown, sKeyDown, upKeyDown, downKeyDown;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball values
        Boolean ballMoveRight = true;
        Boolean ballMoveDown = true;
        int BALL_SPEED = 4;
        const int BALL_WIDTH = 20;
        const int BALL_HEIGHT = 20;
        Rectangle ball;

        //player values
        const int PADDLE_SPEED = 20;
        const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle            
        const int PADDLE_WIDTH = 10;
        const int PADDLE_HEIGHT = 60;
        Rectangle player1, player2, bPortal, rPortal;

        //player and game scores
        int player1Score = 0;
        int player2Score = 0;
        int gameWinScore = 5;  // number of points needed to win game

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //check to see if a key is pressed and set is KeyDown value to true if it has
            switch (e.KeyCode)
            {
                case Keys.W:
                    wKeyDown = true;
                    break;
                case Keys.S:
                    sKeyDown = true;
                    break;
                case Keys.Up:
                    upKeyDown = true;
                    break;
                case Keys.Down:
                    downKeyDown = true;
                    break;
                case Keys.Y:
                case Keys.Space:
                    if (newGameOk)
                    {
                        SetParameters();
                    }
                    break;
                case Keys.Escape:
                    if (newGameOk)
                    {
                        Close();
                    }
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.W:
                    wKeyDown = false;
                    break;
                case Keys.S:
                    sKeyDown = false;
                    break;
                case Keys.Up:
                    upKeyDown = false;
                    break;
                case Keys.Down:
                    downKeyDown = false;
                    break;
            }
        }

        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void SetParameters()
        {
            if (newGameOk == true)
            {
                player1Score = player2Score = 0;
                newGameOk = false;
                startLabel.Visible = false;
                gameUpdateLoop.Start();
            }


            bPortal = new Rectangle(this.Width / 2 + 100, this.Height / 2, PADDLE_WIDTH, 130);
            rPortal = new Rectangle(this.Width / 2- 100, this.Height / 2, PADDLE_WIDTH, 130);

            player1ScoreLabel.Text = $"{player1Score}";
            player2ScoreLabel.Text = $"{player2Score}";

            //player start positions
            player1 = new Rectangle(PADDLE_EDGE, this.Height / 2 - PADDLE_HEIGHT / 2, PADDLE_WIDTH, PADDLE_HEIGHT);
            player2 = new Rectangle(this.Width - PADDLE_EDGE - PADDLE_WIDTH, this.Height / 2 - PADDLE_HEIGHT / 2, PADDLE_WIDTH, PADDLE_HEIGHT);

            BALL_SPEED = 4;
            ball = new Rectangle(this.Width / 2 - 100, this.Height / 2 - 100, 100, 100);
        }

        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            #region update ball position


            if (ballMoveRight == true)
            {
                ball.X = ball.X + BALL_SPEED;
            }
            else
            {
                ball.X = ball.X - BALL_SPEED;
            }

            if (ballMoveDown == true)
            {
                ball.Y = ball.Y + BALL_SPEED;
            }
            else
            {
                ball.Y = ball.Y - BALL_SPEED;
            }
            #endregion

            #region update paddle positions

            if (wKeyDown == true && player1.Y > 0)
            {
                player1.Y -= PADDLE_SPEED;
            }

            if (player1.Y < 0)
            {
                player1.Y = 0;
            }

            if (sKeyDown == true && player1.Y + player1.Height < this.Height)
            {
                player1.Y += PADDLE_SPEED;
            }

            if (player1.Y + player1.Height > this.Height)
            {
                player1.Y = this.Height - player1.Height;
            }

            if (upKeyDown == true && player2.Y > 0)
            {
                player2.Y -= PADDLE_SPEED;
            }

            if (player2.Y < 0)
            {
                player2.Y = 0;
            }

            if (downKeyDown == true && player2.Y + player2.Height < this.Height)
            {
                player2.Y += PADDLE_SPEED;
            }

            if (player2.Y + player2.Height > this.Height)
            {
                player2.Y = this.Height - player2.Height;
            }

            #endregion

            #region ball collision with top and bottom lines

            if (ball.Y < 0) // if ball hits top line
            {
                ball.Y = 0;
                ballMoveDown = true;
                // TODO play a collision sound
            }

            if (ball.Y + ball.Height > this.Height)
            {
                ball.Y = this.Height - ball.Height;
                ballMoveDown = false;
                // TODO play a collision sound
            }


            #endregion

            #region ball collision with paddles

            // TODO create if statment that checks if player1 collides with ball and if it does
            // --- play a "paddle hit" sound and
            if (ball.IntersectsWith(player1))
            {

                random = rng.Next(1, 4);

                if (random == 1 && mushr00m == false)
                {
                    mushr00m = true;
                    shroom.X = this.Width / 2 - shroom.Width;
                    shroom.Y = rng.Next(1, this.Height - shroom.Height);
                }

                ball.X = player1.X + player1.Width;
                ballMoveRight = true;

                if (BALL_SPEED < 10)
                {
                    BALL_SPEED += 2;
                }

                ball.Height -= 20;
                ball.Width -= 20;

                if (ball.Width <= 0)
                {
                    ball.Width = 5;
                    ball.Height = 5;
                }
            }


            // TODO create if statment that checks if player2 collides with ball and if it does
            // --- play a "paddle hit" sound and
            if (ball.IntersectsWith(player2))
            {
                random = rng.Next(1, 4);

                if (random == 1 && mushr00m == false) 
                {
                    mushr00m = true;
                    shroom.X = this.Width / 2 - shroom.Width;
                    shroom.Y = rng.Next(1, this.Height - shroom.Height);
                }

                ball.X = player2.X - ball.Width;
                ballMoveRight = false;

                if (BALL_SPEED < 10)
                {
                    BALL_SPEED += 2;
                }

                ball.Height -= 20;
                ball.Width -= 20;

                if (ball.Width <= 0)
                {
                    ball.Width = 5;
                    ball.Height = 5;
                }
            }

            /*  ENRICHMENT
             *  Instead of using two if statments as noted above see if you can create one
             *  if statement with multiple conditions to play a sound and change direction
             */

            #endregion

            #region ball collision with side walls (point scored)

            if (ball.X < 0)  // ball hits left wall logic
            {
                if (ball.Y >= player1.Y && ball.Y <= player1.Y + PADDLE_HEIGHT)
                {
                    ballMoveRight = true;
                    BALL_SPEED += 2;
                }

                else
                {
                    player2Score += 1;

                    if (player2Score == gameWinScore)
                    {
                        GameOver("player2");
                    }

                    else
                    {
                        SetParameters();
                    }

                }

            }

            if (ball.X + ball.Width > this.Width)  // ball hits right wall logic
            {
                if (ball.Y >= player2.Y && ball.Y <= player2.Y + PADDLE_HEIGHT)
                {
                    ballMoveRight = false;
                    BALL_SPEED += 2;
                }

                else
                {
                    player1Score += 1;

                    if (player1Score == gameWinScore)
                    {
                        GameOver("player1");
                    }
                    else
                    {
                        SetParameters();
                    }
                }
            }

            #endregion

            if (mushr00m == true && ball.IntersectsWith(shroom))
            {
                if (ballMoveRight == true)
                {
                    player1.Height += 10;
                }
                else
                {
                    player2.Height += 10;
                }

                mushr00m = false;

            }

            if (ball.Width == 5)
            {
                #region blue portal
                if (bpUp == true)
                {
                    bPortal.Y -= 5;
                }

                else
                {
                    bPortal.Y += 5;
                }

                if (bPortal.Y <= 0)
                {
                    bpUp = false;
                }

                if (bPortal.Y + bPortal.Height >= this.Height)
                {
                    bpUp = true;
                }

                #endregion

                #region red portal
                if (rpUp == true)
                {
                    rPortal.Y -= 5;
                }

                else
                {
                    rPortal.Y += 5;
                }

                if (rPortal.Y <= 0)
                {
                    rpUp = false;
                }

                if (rPortal.Y + bPortal.Height >= this.Height)
                {
                    rpUp = true;
                }

                #endregion

                #region portal phys
                if (ball.IntersectsWith(bPortal) && portal == false)
                {
                    ball.Y = rPortal.Y + 65;
                    ball.X = rPortal.X;
                    portal = true;
                }

                else if (ball.IntersectsWith(rPortal) && portal == false)
                {
                    ball.Y = bPortal.Y + 65;
                    ball.X = bPortal.X;
                    portal = true;
                }

                else
                {
                    portal = false;
                }

                #endregion
            }

            //refresh the screen, which causes the Form1_Paint method to run
            this.Refresh();
        }

        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        /// <param name="winner">The player name to be shown as the winner</param>
        private void GameOver(string winner)
        {
            newGameOk = true;
            startLabel.Text = $"{winner} wins\npress space to play again";
            startLabel.Visible = true;
            gameUpdateLoop.Stop();
            this.Refresh();
            // TODO create game over logic
            // --- stop the gameUpdateLoop
            // --- show a message on the startLabel to indicate a winner, (may need to Refresh).
            // --- use the startLabel to ask the user if they want to play again

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (newGameOk == false)
            {
                e.Graphics.FillRectangle(whiteBrush, player1);
                e.Graphics.FillRectangle(whiteBrush, player2);
                e.Graphics.FillRectangle(whiteBrush, ball);

                if (mushr00m == true)
                {
                    e.Graphics.DrawImage(mushroom, shroom);
                }

                if (ball.Width == 5)
                {
                    e.Graphics.FillRectangle(redBrush, rPortal);
                    e.Graphics.FillRectangle(blueBrush, bPortal);
                }

            }

        }

    }
}
