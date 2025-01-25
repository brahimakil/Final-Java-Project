using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using FontAwesome.Sharp;

namespace Project_College_App
{
    public partial class TeacherDashboard : Form
    {
        private Guna2Panel sidePanel;
        private Guna2Panel mainPanel;
        private Guna2Button btnCourses;
        private Guna2Button btnLogout;
        private Label lblTitle;
        private Guna2ControlBox btnClose;
        private Guna2ControlBox btnMaximize;
        private Guna2ControlBox btnMinimize;
        private Guna2DragControl formDragControl;
        private IconPictureBox profilePicture;
        private Label lblUserName;
        private Guna2Panel headerPanel;
        private Guna2ShadowForm shadowForm;
        private int teacherId;
        private Guna2Button btnClasses;

        public TeacherDashboard(int teacherId, string teacherName)
        {
            this.teacherId = teacherId;
            InitializeComponents();
            CustomizeComponents();
            lblUserName.Text = teacherName;
        }

        private void InitializeComponents()
        {
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;

            sidePanel = new Guna2Panel
            {
                Width = 280,
                Dock = DockStyle.Left,
                FillColor = Color.FromArgb(30, 41, 59),  // Darker blue theme
                Padding = new Padding(0, 0, 0, 20)
            };

            mainPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(241, 245, 249),  // Light gray background
                Padding = new Padding(30, 20, 30, 20)
            };

            // Add shadow effect
            shadowForm = new Guna2ShadowForm(this)
            {
                BorderRadius = 15
            };
            shadowForm.SetShadowForm(this);

            this.Controls.AddRange(new Control[] { sidePanel, mainPanel });
            
            var classesPanel = new ClassesPanel(teacherId)
            {
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(classesPanel);
        }

        private void CustomizeComponents()
        {
            // Profile section container
            var profileContainer = new Panel
            {
                Dock = DockStyle.Top,
                Height = 180,
                BackColor = Color.Transparent
            };

            profilePicture = new IconPictureBox
            {
                IconChar = IconChar.UserCircle,
                IconSize = 80,
                IconColor = Color.White,
                Size = new Size(80, 80),
                Location = new Point((sidePanel.Width - 80) / 2, 30),
                BackColor = Color.Transparent
            };

            lblUserName = new Label
            {
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Width = sidePanel.Width,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 120),
                BackColor = Color.Transparent
            };

            // Keep only Classes and Logout buttons
            btnClasses = CreateMenuButton("Classes", IconChar.ChalkboardTeacher);
            btnLogout = CreateMenuButton("Logout", IconChar.SignOutAlt);
            btnLogout.FillColor = Color.FromArgb(220, 53, 69);
            btnLogout.Dock = DockStyle.Bottom;

            profileContainer.Controls.AddRange(new Control[] { profilePicture, lblUserName });
            
            sidePanel.Controls.AddRange(new Control[] { 
                profileContainer,
                btnClasses,
                btnLogout
            });

            // Add event handlers
            btnClasses.Click += (s, e) =>
            {
                mainPanel.Controls.Clear();
                var classesPanel = new ClassesPanel(teacherId)
                {
                    Dock = DockStyle.Fill
                };
                mainPanel.Controls.Add(classesPanel);
            };

            btnLogout.Click += (s, e) =>
            {
                this.Hide();
                var loginForm = new LoginForm();
                loginForm.FormClosed += (closedSender, args) => Application.Exit();
                loginForm.Show();
            };
        }

        private Guna2Button CreateMenuButton(string text, IconChar icon)
        {
            return new Guna2Button
            {
                Text = text,
                Image = icon.ToBitmap(Color.White, 24),
                ImageSize = new Size(24, 24),
                FillColor = Color.FromArgb(75, 68, 83),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ImageAlign = HorizontalAlignment.Left,
                TextAlign = HorizontalAlignment.Left,
                Width = sidePanel.Width - 20,
                Height = 45,
                Margin = new Padding(0, 5, 0, 5),
                Padding = new Padding(10, 0, 0, 0)
            };
        }
    }
} 