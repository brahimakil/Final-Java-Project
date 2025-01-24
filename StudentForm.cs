using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Project_College_App
{
    public class StudentForm : Form
    {
        private Guna2TextBox txtUsername;
        private Guna2TextBox txtEmail;
        private Guna2TextBox txtPassword;
        private Guna2Button btnSave;
        private Guna2Button btnCancel;
        private int? userId;

        public StudentForm(int? editUserId = null)
        {
            userId = editUserId;
            InitializeComponents();
            if (userId.HasValue)
            {
                LoadStudentData();
                this.Text = "Edit Student";
            }
            else
            {
                this.Text = "Add Student";
            }
        }

        private void InitializeComponents()
        {
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                RowCount = 4,
                ColumnCount = 1
            };

            txtUsername = new Guna2TextBox
            {
                PlaceholderText = "Username",
                Size = new Size(300, 36),
                BorderRadius = 5
            };

            txtEmail = new Guna2TextBox
            {
                PlaceholderText = "Email",
                Size = new Size(300, 36),
                BorderRadius = 5
            };

            txtPassword = new Guna2TextBox
            {
                PlaceholderText = "Password",
                Size = new Size(300, 36),
                BorderRadius = 5,
                UseSystemPasswordChar = true
            };

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true
            };

            btnSave = new Guna2Button
            {
                Text = "Save",
                Size = new Size(100, 36),
                BorderRadius = 5,
                FillColor = Color.FromArgb(94, 148, 255)
            };

            btnCancel = new Guna2Button
            {
                Text = "Cancel",
                Size = new Size(100, 36),
                BorderRadius = 5,
                FillColor = Color.FromArgb(255, 74, 74),
                Margin = new Padding(0, 0, 10, 0)
            };

            buttonPanel.Controls.AddRange(new Control[] { btnSave, btnCancel });
            panel.Controls.AddRange(new Control[] { txtUsername, txtEmail, txtPassword, buttonPanel });
            this.Controls.Add(panel);

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => this.Close();
        }

        private void LoadStudentData()
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT Username, Email FROM Users WHERE UserID = @UserID", conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtUsername.Text = reader["Username"].ToString();
                        txtEmail.Text = reader["Email"].ToString();
                        txtPassword.PlaceholderText = "Leave blank to keep current password";
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd;

                    if (userId.HasValue)
                    {
                        string query = "UPDATE Users SET Username = @Username, Email = @Email";
                        if (!string.IsNullOrEmpty(txtPassword.Text))
                            query += ", Password = HASHBYTES('SHA2_256', @Password)";
                        query += " WHERE UserID = @UserID";

                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                    }
                    else
                    {
                        cmd = new SqlCommand(@"INSERT INTO Users (Username, Email, Password, Role, CreatedAt) 
                                             VALUES (@Username, @Email, HASHBYTES('SHA2_256', @Password), 'Student', GETDATE())", conn);
                    }

                    cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    if (!string.IsNullOrEmpty(txtPassword.Text))
                        cmd.Parameters.AddWithValue("@Password", txtPassword.Text);

                    cmd.ExecuteNonQuery();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving student: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
} 