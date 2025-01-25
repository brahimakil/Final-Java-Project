using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using FontAwesome.Sharp;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace Project_College_App
{
    public partial class LoginForm : Form
    {
        // Constants for window snapping
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;
        private const int HTLEFT = 0x0A;
        private const int HTRIGHT = 0x0B;
        private const int HTTOP = 0x0C;
        private const int HTTOPLEFT = 0x0D;
        private const int HTTOPRIGHT = 0x0E;
        private const int RESIZE_HANDLE_SIZE = 10;

        private Guna2Panel mainPanel;
        private Guna2TextBox txtUsername;
        private Guna2TextBox txtPassword;
        private Guna2Button btnLogin;
        private Guna2Button btnExit;
        private IconPictureBox iconPictureBox;
        private Label lblTitle;
        private Guna2ControlBox btnClose;
        private Guna2ControlBox btnMaximize;
        private Guna2ControlBox btnMinimize;
        private Guna2DragControl formDragControl;
        private Guna2RadioButton radioAdmin;
        private Guna2RadioButton radioTeacher;
        private Guna2RadioButton radioStudent;

        public LoginForm()
        {
            InitializeComponent();
            CustomizeComponents();
            this.FormClosing += LoginForm_FormClosing;
        }

        private void CustomizeComponents()
        {
            // Form settings
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(900, 600);
            this.BackColor = Color.FromArgb(240, 244, 247);

            // Add window control buttons
            btnClose = new Guna2ControlBox
            {
                Size = new Size(45, 29),
                Location = new Point(this.Width - 45, 0),
                FillColor = Color.FromArgb(240, 244, 247),
                IconColor = Color.Gray,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // Add maximize button
            Guna2ControlBox btnMaximize = new Guna2ControlBox
            {
                Size = new Size(45, 29),
                Location = new Point(this.Width - 90, 0),
                FillColor = Color.FromArgb(240, 244, 247),
                IconColor = Color.Gray,
                ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MaximizeBox,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            btnMinimize = new Guna2ControlBox
            {
                Size = new Size(45, 29),
                Location = new Point(this.Width - 135, 0), // Adjusted position
                FillColor = Color.FromArgb(240, 244, 247),
                IconColor = Color.Gray,
                ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // Enable form dragging
            formDragControl = new Guna2DragControl(this)
            {
                TargetControl = this,
                UseTransparentDrag = true,
                DockIndicatorTransparencyValue = 0.6f,
                DragStartTransparencyValue = 0.5f
            };

            // Add form resizing capability
            this.ResizeRedraw = true;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;

            // Add window snapping
            this.Resize += (s, e) =>
            {
                if (this.WindowState == FormWindowState.Maximized)
                {
                    mainPanel.BorderRadius = 0;
                    mainPanel.Location = new Point((this.Width - mainPanel.Width) / 2, (this.Height - mainPanel.Height) / 2);
                }
                else
                {
                    mainPanel.BorderRadius = 15;
                    mainPanel.Location = new Point((this.Width - mainPanel.Width) / 2, (this.Height - mainPanel.Height) / 2);
                }
            };

            // Add state change animation
            this.SizeChanged += (s, e) =>
            {
                mainPanel.Refresh();
            };

            // Main panel
            mainPanel = new Guna2Panel
            {
                Size = new Size(400, 550),
                Location = new Point((this.Width - 400) / 2, (this.Height - 550) / 2),
                FillColor = Color.White,
                BorderRadius = 15,
                ShadowDecoration = { Enabled = true, Depth = 5 }
            };

            // Icon
            iconPictureBox = new IconPictureBox
            {
                Size = new Size(80, 80),
                Location = new Point((mainPanel.Width - 80) / 2, 40),
                IconChar = IconChar.UserGraduate,
                IconColor = Color.FromArgb(94, 148, 255),
                IconSize = 80,
                BackColor = Color.White,
                SizeMode = PictureBoxSizeMode.Normal
            };

            // Title
            lblTitle = new Label
            {
                Text = "Quiz Maker Login",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(94, 148, 255),
                AutoSize = true,
                Location = new Point((mainPanel.Width - 220) / 2, 140),
                BackColor = Color.White
            };

            // Username textbox
            txtUsername = new Guna2TextBox
            {
                Size = new Size(300, 45),
                Location = new Point(50, 200),
                PlaceholderText = "Username",
                BorderRadius = 8,
                Font = new Font("Segoe UI", 10),
                FillColor = Color.FromArgb(240, 244, 247),
                BorderColor = Color.FromArgb(94, 148, 255)
            };

            // Password textbox
            txtPassword = new Guna2TextBox
            {
                Size = new Size(300, 45),
                Location = new Point(50, 290),
                PlaceholderText = "Password",
                BorderRadius = 8,
                Font = new Font("Segoe UI", 10),
                FillColor = Color.FromArgb(240, 244, 247),
                BorderColor = Color.FromArgb(94, 148, 255),
                PasswordChar = '●',
                UseSystemPasswordChar = true
            };

            // Role selection
            var rolePanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                WrapContents = false
            };

            radioAdmin = new Guna2RadioButton
            {
                Text = "Admin",
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(0, 0, 20, 0)
            };

            radioTeacher = new Guna2RadioButton
            {
                Text = "Teacher",
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(0, 0, 20, 0)
            };

            radioStudent = new Guna2RadioButton
            {
                Text = "Student",
                Font = new Font("Segoe UI", 10),
                Checked = true
            };

            rolePanel.Controls.AddRange(new Control[] { radioAdmin, radioTeacher, radioStudent });

            // Login button
            btnLogin = new Guna2Button
            {
                Size = new Size(300, 45),
                Location = new Point(50, 400),
                Text = "LOGIN",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FillColor = Color.FromArgb(94, 148, 255),
                BorderRadius = 8,
                Cursor = Cursors.Hand
            };

            // Exit button
            btnExit = new Guna2Button
            {
                Size = new Size(300, 45),
                Location = new Point(50, 470),
                Text = "EXIT",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FillColor = Color.FromArgb(255, 74, 74),
                BorderRadius = 8,
                Cursor = Cursors.Hand
            };

            // Add controls to panel
            mainPanel.Controls.AddRange(new Control[] { 
                iconPictureBox, 
                lblTitle,
                txtUsername, 
                txtPassword, 
                rolePanel,
                btnLogin,
                btnExit
            });

            // Add controls to form
            this.Controls.AddRange(new Control[] {
                mainPanel,
                btnClose,
                btnMaximize,
                btnMinimize
            });

            // Add event handlers
            btnLogin.Click += BtnLogin_Click;
            btnExit.Click += BtnExit_Click;
            
            // Add hover effects
            btnLogin.MouseEnter += (s, e) => btnLogin.FillColor = Color.FromArgb(73, 128, 234);
            btnLogin.MouseLeave += (s, e) => btnLogin.FillColor = Color.FromArgb(94, 148, 255);
            
            btnExit.MouseEnter += (s, e) => btnExit.FillColor = Color.FromArgb(235, 54, 54);
            btnExit.MouseLeave += (s, e) => btnExit.FillColor = Color.FromArgb(255, 74, 74);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    Point pos = new Point(m.LParam.ToInt32());
                    pos = this.PointToClient(pos);
                    if (pos.Y < RESIZE_HANDLE_SIZE)
                    {
                        m.Result = (IntPtr)HTTOP;
                        return;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter both username and password.", "Login Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string role = radioAdmin.Checked ? "Admin" : 
                         radioTeacher.Checked ? "Teacher" : "Student";

            if (ValidateLogin(txtUsername.Text, txtPassword.Text, role))
            {
                MessageBox.Show("Login successful!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                // TODO: Open main form
                // this.Hide();
                // new MainForm().Show();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Application.OpenForms.Count <= 1)
            {
                Application.Exit();
            }
        }

        private bool ValidateLogin(string username, string password, string role)
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Initial Catalog=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT UserID, Username FROM Users WHERE Username = @Username AND Password = @Password AND Role = @Role";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", HashPassword(password));
                        cmd.Parameters.AddWithValue("@Role", role);
                        
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userId = reader.GetInt32(0);
                                string dbUsername = reader.GetString(1);

                                this.Hide();
                                Form dashboard = null;

                                switch (role)
                                {
                                    case "Admin":
                                        dashboard = new AdminDashboard();
                                        break;
                                    case "Teacher":
                                        dashboard = new TeacherDashboard(userId, dbUsername);
                                        break;
                                    case "Student":
                                        dashboard = new StudentDashboard(userId, dbUsername);
                                        break;
                                }

                                if (dashboard != null)
                                {
                                    dashboard.FormClosed += (s, args) => this.Close();
                                    dashboard.Show();
                                }
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Database error: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.UserClosing && Application.OpenForms.Count == 1)
            {
                Application.Exit();
            }
        }

        private bool ValidateUserInput(string username, string email)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
                return false;
            
            // Add email format validation
            try {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch {
                return false;
            }
        }
    }
} 