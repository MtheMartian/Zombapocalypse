using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shoot_Out_Game
{
    public partial class Form1 : Form
    {
        bool goLeft, goRight, goUp, goDown, gameOver, reload = false;
        string facing = "up";
        int playerHealth = 100;
        int speed = 10;
        int ammo = 10;
        int zombieSpeed = 3;
        int score;
        private bool mouseDown = false;
        private Point lastLocation;
         
        Random randNum = new Random();

        List<PictureBox> zombieList = new List<PictureBox>();


        public Form1()
        {
            InitializeComponent();
            RestartGame();
        }



        private void MainTimerEvent(object sender, EventArgs e)
        {
            if (playerHealth > 1)
            {
                healthBar.Value = playerHealth;
            }
            else
            {
                gameOver = true;
                player.Image = Properties.Resources.dead;
                gameTimer.Stop();
            }


            txtAmmo.Text = "Ammo: " + ammo;
            txtScore.Text = "Kills: " + score;

            if (goLeft == true && player.Left > 0)
            {
                player.Left -= speed;
            }

            if (goRight == true && player.Left + player.Width < this.ClientSize.Width)
            {
                player.Left += speed;
            }

            if (goUp == true && player.Top > 40)
            {
                player.Top -= speed;
            }

            if (goDown == true && player.Top + player.Height < this.ClientSize.Height)
            {
                player.Top += speed;
            }

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "ammo")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        ((PictureBox)x).Dispose();
                        ammo += 5;
                    }
                }

                if (x is PictureBox && (string)x.Tag == "zombie")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        playerHealth -= 1;
                    }

                    if (x.Left > player.Left)
                    {
                        x.Left -= zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zleft;
                    }
                    if (x.Left < player.Left)
                    {
                        x.Left += zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zright;
                    }
                    if (x.Top > player.Top)
                    {
                        x.Top -= zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zup;
                    }
                    if (x.Top < player.Top)
                    {
                        x.Top += zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zdown;
                    }
                }

                foreach (Control j in this.Controls)
                {
                    if (j is PictureBox && (string)j.Tag == "bullet" && x is PictureBox && (string)x.Tag == "zombie")
                    {
                        if (x.Bounds.IntersectsWith(j.Bounds))
                        {
                            score++;

                            this.Controls.Remove(j);
                            ((PictureBox)j).Dispose();
                            this.Controls.Remove(x);
                            ((PictureBox)x).Dispose();
                            zombieList.Remove(((PictureBox)x));
                            MakeZombies();

                        }
                    }
                }
            }



        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (gameOver == true)
            {
                return;
            }

            if (e.KeyCode == Keys.A)
            {
                goLeft = true;
                facing = "left";
                player.Image = Properties.Resources.left;
            }

            if (e.KeyCode == Keys.D)
            {
                goRight = true;
                facing = "right";
                player.Image = Properties.Resources.right;
            }

            if (e.KeyCode == Keys.W)
            {
                goUp = true;
                facing = "up";
                player.Image = Properties.Resources.up;
            }

            if (e.KeyCode == Keys.S)
            {
                goDown = true;
                facing = "down";
                player.Image = Properties.Resources.down;
            }


        }

        private void MouseIsDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 1) mouseDown = true;
            lastLocation = e.Location;
        }

        

        private void mouseMove(object sender, MouseEventArgs e)
        {
          if (mouseDown)
            {
                this.Location = new Point
                    ((this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);
                this.Update();

            }
        }

        private void mouseIsUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                goLeft = false;

            }

            if (e.KeyCode == Keys.D)
            {
                goRight = false;

            }

            if (e.KeyCode == Keys.W)
            {
                goUp = false;

            }

            if (e.KeyCode == Keys.S)
            {
                goDown = false;


            }

            if (e.KeyCode == Keys.Space && ammo > 0 && gameOver == false)
            {
                ammo--;
                ShootBullet(facing);

            }

            if (e.KeyCode == Keys.R)
            {
                if (ammo < 1 && reload == false)
                {
                    DropAmmo();
                    reload = true;
                }
            }
            else if (ammo > 1)
            {
                reload = false;
            }

            if (e.KeyCode == Keys.Enter && gameOver == true)
            {
                RestartGame();
            }

            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }

        }

        private void ShootBullet(string direction)
        {
            Bullet shootBullet = new Bullet();
            shootBullet.direction = direction;
            shootBullet.bulletLeft = player.Left + (player.Width / 2);
            shootBullet.bulletTop = player.Top + (player.Height / 2);
            shootBullet.MakeBullet(this);
        }

        private void MakeZombies()
        {
            PictureBox zombie = new PictureBox();
            zombie.Tag = "zombie";
            zombie.Image = Properties.Resources.zdown;
            zombie.Left = randNum.Next(0, 900);
            zombie.Top = randNum.Next(0, 800);
            zombie.SizeMode = PictureBoxSizeMode.AutoSize;
            zombie.BackColor = Color.Transparent;
            zombieList.Add(zombie);
            this.Controls.Add(zombie);
            player.BringToFront();
        }


        
        private void DropAmmo()
        {
            PictureBox ammu = new PictureBox();
            ammu.Image = Properties.Resources.ammo_Image;
            ammu.SizeMode = PictureBoxSizeMode.AutoSize;
            ammu.Left = randNum.Next(10, this.ClientSize.Width - ammu.Width);
            ammu.Top = randNum.Next(60, this.ClientSize.Height - ammu.Height);
            ammu.Tag = "ammo";
            ammu.BackColor = Color.Transparent;
            this.Controls.Add(ammu);

            ammu.BringToFront();
            player.BringToFront();
        }

        private void RestartGame()
        {
            player.Image = Properties.Resources.up;

            foreach (PictureBox i in zombieList)
            {
                this.Controls.Remove(i);
            }

            zombieList.Clear();

            for (int i = 0; i < 3; i++)
            {
                MakeZombies();
            }

            goUp = false;
            goDown = false;
            goLeft = false;
            goRight = false;
            gameOver = false;

            playerHealth = 100;
            score = 0;
            ammo = 10;

            gameTimer.Start();
        }
    }

}
