using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Project_College_App
{
    public class TeacherCourseForm : Form
    {
        private Guna2TextBox txtCourseName;
        private Guna2Button btnSave;
        private Guna2Button btnCancel;
        private int teacherId;
        private int? courseId;

        public TeacherCourseForm(int teacherId, int? editCourseId = null)
        {
            this.teacherId = teacherId;
            this.courseId = editCourseId;
            InitializeComponents();
            if (courseId.HasValue)
            {
                LoadCourseData();
                this.Text = "Edit Course";
            }
            else
            {
                this.Text = "Add Course";
            }
        }

        private void InitializeComponents()
        {
            this.Size = new Size(400, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                RowCount = 2,
                ColumnCount = 1
            };

            txtCourseName = new Guna2TextBox
            {
                PlaceholderText = "Course Name",
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
            panel.Controls.AddRange(new Control[] { txtCourseName, buttonPanel });
            this.Controls.Add(panel);

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => this.Close();
        }

        private void LoadCourseData()
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT ClassName FROM Classes WHERE ClassID = @ClassID AND TeacherID = @TeacherID", conn);
                    cmd.Parameters.AddWithValue("@ClassID", courseId);
                    cmd.Parameters.AddWithValue("@TeacherID", teacherId);
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        txtCourseName.Text = result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading course data: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCourseName.Text))
            {
                MessageBox.Show("Please enter a course name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = courseId.HasValue
                        ? "UPDATE Classes SET ClassName = @ClassName WHERE ClassID = @ClassID AND TeacherID = @TeacherID"
                        : "INSERT INTO Classes (ClassName, TeacherID) VALUES (@ClassName, @TeacherID)";

                    var cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ClassName", txtCourseName.Text);
                    cmd.Parameters.AddWithValue("@TeacherID", teacherId);
                    if (courseId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@ClassID", courseId);
                    }

                    cmd.ExecuteNonQuery();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving course: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
} 