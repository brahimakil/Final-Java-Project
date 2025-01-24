using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using FontAwesome.Sharp;

namespace Project_College_App
{
    public class TeacherCoursesPanel : Guna2Panel
    {
        private Guna2DataGridView dgvCourses;
        private Guna2TextBox txtSearch;
        private Guna2Button btnAdd;
        private Guna2Button btnEdit;
        private Guna2Button btnDelete;
        private Guna2Button btnManageStudents;
        private int teacherId;

        public TeacherCoursesPanel(int teacherId)
        {
            this.teacherId = teacherId;
            InitializeComponents();
            LoadCourses();
        }

        private void InitializeComponents()
        {
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(20);

            var actionPanel = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(0, 0, 0, 10)
            };

            var controlsContainer = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight
            };

            txtSearch = new Guna2TextBox
            {
                PlaceholderText = "Search courses...",
                Size = new Size(200, 36),
                BorderRadius = 5,
                Margin = new Padding(0, 0, 10, 0)
            };

            btnAdd = new Guna2Button
            {
                Text = "Add Course",
                Size = new Size(120, 36),
                FillColor = Color.FromArgb(94, 148, 255),
                BorderRadius = 5,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Image = IconChar.Plus.ToBitmap(Color.White, 16),
                ImageSize = new Size(16, 16),
                ImageAlign = HorizontalAlignment.Left,
                TextAlign = HorizontalAlignment.Left,
                ImageOffset = new Point(8, 0),
                TextOffset = new Point(3, 0),
                Margin = new Padding(0, 0, 10, 0)
            };

            btnEdit = new Guna2Button
            {
                Size = new Size(36, 36),
                FillColor = Color.FromArgb(94, 148, 255),
                BorderRadius = 5,
                Image = IconChar.Edit.ToBitmap(Color.White, 16),
                ImageSize = new Size(16, 16),
                ImageAlign = HorizontalAlignment.Center,
                Margin = new Padding(0, 0, 10, 0)
            };

            btnDelete = new Guna2Button
            {
                Size = new Size(36, 36),
                FillColor = Color.FromArgb(255, 74, 74),
                BorderRadius = 5,
                Image = IconChar.Trash.ToBitmap(Color.White, 16),
                ImageSize = new Size(16, 16),
                ImageAlign = HorizontalAlignment.Center
            };

            btnManageStudents = new Guna2Button
            {
                Size = new Size(36, 36),
                FillColor = Color.FromArgb(94, 148, 255),
                BorderRadius = 5,
                Image = IconChar.UserGroup.ToBitmap(Color.White, 16),
                ImageSize = new Size(16, 16),
                ImageAlign = HorizontalAlignment.Center,
                Margin = new Padding(0, 0, 10, 0)
            };

            controlsContainer.Controls.AddRange(new Control[] { txtSearch, btnAdd, btnEdit, btnDelete, btnManageStudents });
            actionPanel.Controls.Add(controlsContainer);
            controlsContainer.Location = new Point(
                (actionPanel.Width - controlsContainer.Width) / 2,
                (actionPanel.Height - controlsContainer.Height) / 2
            );

            var gridContainer = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 10, 0, 0)
            };

            dgvCourses = new Guna2DataGridView
            {
                Dock = DockStyle.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = Color.FromArgb(230, 239, 255),
                    SelectionForeColor = Color.Black,
                    Padding = new Padding(8)
                },
                ColumnHeadersHeight = 50,
                RowTemplate = { Height = 45 },
                BackgroundColor = Color.White,
                CellBorderStyle = DataGridViewCellBorderStyle.Single,
                GridColor = Color.FromArgb(231, 229, 255),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(242, 242, 242),
                    ForeColor = Color.FromArgb(66, 66, 66),
                    Font = new Font("Segoe UI Semibold", 11),
                    Padding = new Padding(10)
                },
                EnableHeadersVisualStyles = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ScrollBars = ScrollBars.Both
            };

            gridContainer.Controls.Add(dgvCourses);
            this.Controls.AddRange(new Control[] { actionPanel, gridContainer });

            // Events
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnManageStudents.Click += BtnManageStudents_Click;
            txtSearch.TextChanged += TxtSearch_TextChanged;
        }

        private void LoadCourses()
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT c.ClassID, c.ClassName, 
                                   (SELECT COUNT(*) FROM StudentClasses sc WHERE sc.ClassID = c.ClassID) as StudentCount 
                                   FROM Classes c 
                                   WHERE c.TeacherID = @TeacherID";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TeacherID", teacherId);
                        var adapter = new SqlDataAdapter(cmd);
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        dgvCourses.DataSource = dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading courses: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new TeacherCourseForm(teacherId))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadCourses();
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCourses.SelectedRows.Count > 0)
            {
                int courseId = Convert.ToInt32(dgvCourses.SelectedRows[0].Cells["ClassID"].Value);
                using (var form = new TeacherCourseForm(teacherId, courseId))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadCourses();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a course to edit.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCourses.SelectedRows.Count > 0)
            {
                int courseId = Convert.ToInt32(dgvCourses.SelectedRows[0].Cells["ClassID"].Value);
                string courseName = dgvCourses.SelectedRows[0].Cells["ClassName"].Value.ToString();

                var result = MessageBox.Show($"Are you sure you want to delete the course '{courseName}'?",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
                    using (var conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            var cmd = new SqlCommand("DELETE FROM Classes WHERE ClassID = @ClassID AND TeacherID = @TeacherID", conn);
                            cmd.Parameters.AddWithValue("@ClassID", courseId);
                            cmd.Parameters.AddWithValue("@TeacherID", teacherId);
                            cmd.ExecuteNonQuery();
                            LoadCourses();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error deleting course: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a course to delete.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnManageStudents_Click(object sender, EventArgs e)
        {
            if (dgvCourses.SelectedRows.Count > 0)
            {
                int courseId = Convert.ToInt32(dgvCourses.SelectedRows[0].Cells["ClassID"].Value);
                string courseName = dgvCourses.SelectedRows[0].Cells["ClassName"].Value.ToString();

                using (var form = new ClassStudentsForm(courseId, courseName))
                {
                    form.ShowDialog();
                    LoadCourses(); // Refresh to update student count
                }
            }
            else
            {
                MessageBox.Show("Please select a course to manage students.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (dgvCourses.DataSource is DataTable dt)
            {
                string searchText = txtSearch.Text.Trim().ToLower();
                dt.DefaultView.RowFilter = string.IsNullOrEmpty(searchText) ? "" 
                    : $"ClassName LIKE '%{searchText}%'";
            }
        }
    }
} 