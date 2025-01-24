using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Project_College_App
{
    public class ClassForm : Form
    {
        private Guna2TextBox txtClassName;
        private Guna2ComboBox cmbTeacher;
        private Guna2Button btnSave;
        private Guna2Button btnCancel;
        private int? classId;

        public ClassForm(int? editClassId = null)
        {
            classId = editClassId;
            InitializeComponents();
            LoadTeachers();
            if (classId.HasValue)
            {
                LoadClassData();
                this.Text = "Edit Class";
            }
            else
            {
                this.Text = "Add Class";
            }
        }

        private void InitializeComponents()
        {
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                RowCount = 3,
                ColumnCount = 1
            };

            txtClassName = new Guna2TextBox
            {
                PlaceholderText = "Class Name",
                Size = new Size(300, 36),
                BorderRadius = 5
            };

            cmbTeacher = new Guna2ComboBox
            {
                Text = "Select Teacher",
                Size = new Size(300, 36),
                BorderRadius = 5
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
            panel.Controls.AddRange(new Control[] { txtClassName, cmbTeacher, buttonPanel });
            this.Controls.Add(panel);

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => this.Close();
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
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    cmbTeacher.DisplayMember = "Username";
                    cmbTeacher.ValueMember = "UserID";
                    cmbTeacher.DataSource = dt;
                    cmbTeacher.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading teachers: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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
                            cmbTeacher.SelectedValue = reader["TeacherID"];
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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtClassName.Text) || cmbTeacher.SelectedValue == null)
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error",
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
                    SqlCommand cmd;

                    if (classId.HasValue)
                    {
                        cmd = new SqlCommand(@"UPDATE Classes 
                                             SET ClassName = @ClassName, TeacherID = @TeacherID 
                                             WHERE ClassID = @ClassID", conn);
                        cmd.Parameters.AddWithValue("@ClassID", classId);
                    }
                    else
                    {
                        cmd = new SqlCommand(@"INSERT INTO Classes (ClassName, TeacherID, CreatedAt, IsActive) 
                                             VALUES (@ClassName, @TeacherID, GETDATE(), 1)", conn);
                    }

                    cmd.Parameters.AddWithValue("@ClassName", txtClassName.Text);
                    cmd.Parameters.AddWithValue("@TeacherID", cmbTeacher.SelectedValue);

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