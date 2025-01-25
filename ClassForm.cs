using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Project_College_App
{
    public partial class ClassForm : Form
    {
        private bool isAdminMode;
        private int? teacherId;
        private int? classId;
        private Guna2TextBox txtClassName;
        private Guna2ComboBox cmbTeachers;
        private Guna2Button btnSave;
        private Guna2Button btnCancel;

        // Constructor for teacher mode
        public ClassForm(int teacherId, int? classId = null)
        {
            this.isAdminMode = false;
            this.teacherId = teacherId;
            this.classId = classId;
            InitializeComponents();
            if (classId.HasValue)
            {
                LoadClassData();
            }
        }

        // Constructor for admin mode
        public ClassForm()
        {
            this.isAdminMode = true;
            this.teacherId = null;
            InitializeComponents();
            LoadTeachers();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(400, isAdminMode ? 250 : 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Text = classId.HasValue ? "Edit Class" : "Add New Class";

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                RowCount = 3,
                ColumnCount = 1
            };

            txtClassName = new Guna2TextBox
            {
                PlaceholderText = "Enter class name",
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                Size = new Size(350, 36)
            };

            cmbTeachers = new Guna2ComboBox
            {
                BorderRadius = 5,
                Font = new Font("Segoe UI", 10),
                Size = new Size(350, 36),
                Visible = isAdminMode
            };

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0, 20, 0, 0)
            };

            btnSave = new Guna2Button
            {
                Text = "Save",
                Size = new Size(100, 36),
                BorderRadius = 5,
                FillColor = Color.FromArgb(94, 148, 255),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0, 0, 10, 0)
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
            panel.Controls.AddRange(new Control[] { txtClassName, cmbTeachers, buttonPanel });
            this.Controls.Add(panel);

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => this.Close();
        }

        private void LoadClassData()
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT ClassName, TeacherID FROM Classes WHERE ClassID = @ClassID", conn);
                    cmd.Parameters.AddWithValue("@ClassID", classId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtClassName.Text = reader["ClassName"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading class data: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadTeachers()
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT UserID, Username FROM Users WHERE Role = 'Teacher'";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        var adapter = new SqlDataAdapter(cmd);
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        cmbTeachers.DataSource = dt;
                        cmbTeachers.DisplayMember = "Username";
                        cmbTeachers.ValueMember = "UserID";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading teachers: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtClassName.Text))
            {
                MessageBox.Show("Please enter a class name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (isAdminMode && cmbTeachers.SelectedValue == null)
            {
                MessageBox.Show("Please select a teacher.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveClass();
        }

        private void SaveClass()
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query;
                    SqlCommand cmd;

                    if (classId.HasValue)
                    {
                        query = "UPDATE Classes SET ClassName = @ClassName WHERE ClassID = @ClassID";
                        if (isAdminMode)
                        {
                            query = "UPDATE Classes SET ClassName = @ClassName, TeacherID = @TeacherID WHERE ClassID = @ClassID";
                        }
                    }
                    else
                    {
                        query = "INSERT INTO Classes (ClassName, TeacherID) VALUES (@ClassName, @TeacherID)";
                    }

                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ClassName", txtClassName.Text.Trim());
                    cmd.Parameters.AddWithValue("@TeacherID", isAdminMode ? Convert.ToInt32(cmbTeachers.SelectedValue) : teacherId.Value);

                    if (classId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@ClassID", classId.Value);
                    }

                    cmd.ExecuteNonQuery();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving class: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
} 