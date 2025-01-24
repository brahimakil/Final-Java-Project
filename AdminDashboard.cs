using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using FontAwesome.Sharp;

namespace Project_College_App
{
    public partial class AdminDashboard : Form
    {
        private Guna2Panel sidePanel;
        private Guna2Panel mainPanel;
        private Guna2Button btnTeachers;
        private Guna2Button btnStudents;
        private Guna2Button btnLogout;
        private Guna2Button btnClasses;
        private Label lblTitle;
        private Guna2ControlBox btnClose;
        private Guna2ControlBox btnMaximize;
        private Guna2ControlBox btnMinimize;
        private Guna2DragControl formDragControl;
        private IconPictureBox profilePicture;
        private Label lblUserName;
        private Guna2Panel headerPanel;
        private Guna2ShadowForm shadowForm;

        public AdminDashboard()
        {
            InitializeComponent();
            this.Load += new EventHandler(AdminDashboard_Load);
            CustomizeComponents();
        }

        private void AdminDashboard_Load(object sender, EventArgs e)
        {
            formDragControl = new Guna2DragControl(this)
            {
                DockIndicatorTransparencyValue = 0.6f,
                TargetControl = headerPanel,
                UseTransparentDrag = true
            };
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }

        private void CustomizeComponents()
        {
            // Form settings
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1200, 700);
            this.BackColor = Color.FromArgb(240, 244, 247);

            // Add shadow to form
            shadowForm = new Guna2ShadowForm(this);
            shadowForm.ShadowColor = Color.FromArgb(94, 148, 255);

            // Remove header panel completely
            // Side panel
            sidePanel = new Guna2Panel
            {
                Size = new Size(250, this.Height),
                BackColor = Color.FromArgb(94, 148, 255),
                Dock = DockStyle.Left,
                Padding = new Padding(10)
            };

            // Main panel
            mainPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 244, 247),
                Padding = new Padding(20)
            };

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
                Text = "Admin User",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Width = sidePanel.Width,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 120),
                BackColor = Color.Transparent
            };

            // Add profile controls to container
            profileContainer.Controls.AddRange(new Control[] { profilePicture, lblUserName });

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

            // Menu buttons with proper spacing
            btnClasses = CreateMenuButton("Classes", IconChar.School);
            btnTeachers = CreateMenuButton("Teachers", IconChar.ChalkboardTeacher);
            btnStudents = CreateMenuButton("Students", IconChar.GraduationCap);

            // Add buttons to nav container in order
            navContainer.Controls.Add(btnClasses);
            navContainer.Controls.Add(btnTeachers);
            navContainer.Controls.Add(btnStudents);

            // Logout at bottom
            btnLogout = CreateMenuButton("Logout", IconChar.SignOutAlt);
            btnLogout.FillColor = Color.FromArgb(220, 53, 69);
            btnLogout.Dock = DockStyle.Bottom;

            // Add all sections to side panel in correct order
            sidePanel.Controls.Clear(); // Clear existing controls first
            sidePanel.Controls.AddRange(new Control[] { 
                profileContainer,
                navContainer,
                btnLogout
            });
            this.Controls.AddRange(new Control[] { sidePanel, mainPanel });

            // Add event handlers
            btnLogout.Click += (sender, e) => 
            {
                try
                {
                    this.Hide();
                    var loginForm = new LoginForm();
                    loginForm.FormClosed += (closedSender, args) => Application.Exit();
                    loginForm.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Logout Error: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnTeachers.Click += (s, e) =>
            {
                mainPanel.Controls.Clear();
                var teachersPanel = new TeachersPanel();
                teachersPanel.Dock = DockStyle.Fill;
                mainPanel.Controls.Add(teachersPanel);
            };

            btnStudents.Click += (s, e) =>
            {
                mainPanel.Controls.Clear();
                mainPanel.Controls.Add(new StudentsPanel());
            };

            btnClasses.Click += (s, e) =>
            {
                mainPanel.Controls.Clear();
                mainPanel.Controls.Add(new ClassesPanel());
            };
        }

        private void RepositionControlButtons()
        {
            int rightPadding = 10;
            btnClose.Location = new Point(headerPanel.Width - btnClose.Width - rightPadding, 0);
            btnMaximize.Location = new Point(btnClose.Left - btnMaximize.Width, 0);
            btnMinimize.Location = new Point(btnMaximize.Left - btnMinimize.Width, 0);
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
                Width = sidePanel.Width - 20, // Full width minus padding
                Height = 45,
                Margin = new Padding(0, 5, 0, 5),
                Padding = new Padding(10, 0, 0, 0)
            };
        }
    }
} 