using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace bSlideshow
{
    public partial class frmBSlideshow : Form
    {
        const string _dirFile = "directories.txt";

        List<PictureBox> _pictures = new List<PictureBox>();
        private List<string> _files = new List<string>();
        private List<string> _directories;

        private bool _random = false;
        private float _speed = 0;
        private bool _fullscreen = false;

        private int _currentFile = 0;

        Random _randGen = new Random();

        public frmBSlideshow()
        {
            InitializeComponent();

            try
            {
                string[] array;
                array = File.ReadAllLines(_dirFile);
                _directories = new List<string>(array);
            }
            catch (Exception e)
            {
                string msg = "The file \"" + _dirFile + "\" could not be read:\n " + e.Message;
                MessageBox.Show(msg, "bSlideshow", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                Environment.Exit(0);
            }

            foreach (string dir in _directories)
            {
                foreach (string f in Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories)
                    .Where(s => s.EndsWith(".jpg") || s.EndsWith(".jpeg") || s.EndsWith(".png")))
                {
                    _files.Add(f);
                }
            }

            FillPictures();
        }

        private void FillPictures()
        {
            int totalWidth = 0;
            while (totalWidth < panel.Width)
            {
                CreatePictureBox();
                totalWidth = _pictures.Last().Location.X + _pictures.Last().Width;
            }
        }

        private void CheckAndCreatePictureBox()
        {
            PictureBox last = _pictures.Last();
            if (last.Location.X < panel.Width && last.Location.X + last.Width > panel.Width)
            {
                CreatePictureBox();
            }
        }

        private void CreatePictureBox()
        {
            PictureBox pb = new PictureBox();
            Controls.Add(pb);
            Image img = Image.FromFile(_files[_currentFile]);
            pb.Tag = _files[_currentFile];
            pb.Image = img;
            pb.SizeMode = PictureBoxSizeMode.Zoom;

            Point newLoc;

            if (_pictures.Count == 0)
            {
                newLoc = new Point(0, 0);
            }
            else
            {
                PictureBox last = _pictures.Last();
                newLoc = new Point(last.Location.X + last.Width, 0);
            }

            pb.Location = newLoc;
            pb.Height = panel.Height;
            pb.Width = (int)((float)img.Width / img.Height * pb.Height);
            pb.DoubleClick += new System.EventHandler(this.OnImageDoubleClick);

            panel.Controls.Add(pb);

            _pictures.Add(pb);

            if (_random)
            {
                _currentFile = _randGen.Next(_files.Count);
            }
            else
            {
                _currentFile++;
            }
        }

        private void OnImageDoubleClick(object sender, EventArgs e)
        {
            Control pb = (Control)sender;
            System.Diagnostics.Process.Start(pb.Tag.ToString());
        }

        private void CheckAndDestroyRightmostPictureBox()
        {
            PictureBox pb = _pictures.First();
            if (_pictures.Count > 1 && pb.Location.X + pb.Width < 0)
            {
                panel.Controls.Remove(pb);
                _pictures.Remove(pb);
            }
        }

        private void MoveImages()
        {
            CheckAndCreatePictureBox();
            foreach (PictureBox pb in _pictures)
            {
                Point loc = pb.Location;
                loc.X -= (int)_speed;
                pb.Location = loc;
            }
            CheckAndDestroyRightmostPictureBox();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            MoveImages();
            this.Text = "bSlideshow  speed: " + _speed + "  random: " + _random;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                _speed++;
            }
            else if (e.KeyCode == Keys.Down)
            {
                _speed--;
            }
            else if (e.KeyCode == Keys.R)
            {
                _random = !_random;
            }
            else if (e.KeyCode == Keys.Space)
            {
                _speed = 0;
                timer.Interval = 100;
            }
            else if (e.KeyCode == Keys.Left)
            {
                timer.Interval--;
            }
            else if (e.KeyCode == Keys.Right)
            {
                timer.Interval++;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                _fullscreen = !_fullscreen;
                if (_fullscreen)
                {
                    FormBorderStyle = FormBorderStyle.None;
                    WindowState = FormWindowState.Maximized;
                }
                else
                {
                    FormBorderStyle = FormBorderStyle.Sizable;
                    WindowState = FormWindowState.Normal;
                }
            }
        }
    }
}
