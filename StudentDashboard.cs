using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using FontAwesome.Sharp;

namespace Project_College_App
{
    public partial class StudentDashboard : Form
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
        private int studentId;

        public StudentDashboard(int studentId, string studentName)
        {
            this.studentId = studentId;
            InitializeComponents();
            CustomizeComponents();
            lblUserName.Text = studentName;
        }

        private void InitializeComponents()
        {
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;

            // Initialize main panels
            sidePanel = new Guna2Panel
            {
                Width = 250,
                Dock = DockStyle.Left,
                FillColor = Color.FromArgb(45, 45, 45),
                Padding = new Padding(0, 0, 0, 20)
            };

            mainPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 246, 247)
            };

            // Add controls to form
            this.Controls.AddRange(new Control[] { sidePanel, mainPanel });

            // Load initial panel
            mainPanel.Controls.Add(new StudentCoursesPanel(studentId));
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

            // Navigation container
            var navContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 20, 0, 0)
            };

            btnCourses = CreateMenuButton("My Courses", IconChar.GraduationCap);
            btnLogout = CreateMenuButton("Logout", IconChar.SignOutAlt);
            btnLogout.FillColor = Color.FromArgb(220, 53, 69);
            btnLogout.Dock = DockStyle.Bottom;

            // Add controls to containers
            profileContainer.Controls.AddRange(new Control[] { profilePicture, lblUserName });
            navContainer.Controls.Add(btnCourses);

            // Add all sections to side panel
            sidePanel.Controls.AddRange(new Control[] { 
                profileContainer,
                navContainer,
                btnLogout
            });

            // Add event handlers
            btnCourses.Click += (s, e) =>
            {
                mainPanel.Controls.Clear();
                mainPanel.Controls.Add(new StudentCoursesPanel(studentId));
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
                Margin = new Padding(10, 5, 10, 5),
                Padding = new Padding(10, 0, 0, 0)
            };
        }
    }
} 