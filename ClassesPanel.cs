using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using FontAwesome.Sharp;

namespace Project_College_App
{
    public class ClassesPanel : Guna2Panel
    {
        private Guna2DataGridView dgvClasses;
        private Guna2TextBox txtSearch;
        private Guna2Button btnAdd;
        private Guna2Button btnEdit;
        private Guna2Button btnDelete;
        private Guna2Button btnManageStudents;
        private Guna2Panel actionPanel;

        public ClassesPanel()
        {
            InitializeComponents();
            LoadClasses();
        }

        private void InitializeComponents()
        {
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(30);
            this.BackColor = Color.FromArgb(240, 244, 247);

            actionPanel = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                Margin = new Padding(0, 0, 0, 40),
                FillColor = Color.White,
                ShadowDecoration = { Enabled = true, Depth = 1, Color = Color.FromArgb(10, 0, 0, 0) }
            };

            var controlsContainer = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(20, 25, 20, 25)
            };

            txtSearch = new Guna2TextBox
            {
                PlaceholderText = "Search classes...",
                Size = new Size(400, 36),
                BorderRadius = 5,
                Font = new Font("Segoe UI", 9),
                FillColor = Color.White,
                BorderColor = Color.FromArgb(213, 218, 223),
                PlaceholderForeColor = Color.FromArgb(125, 137, 149),
                Margin = new Padding(0, 0, 20, 0)
            };

            btnAdd = new Guna2Button
            {
                Text = "Add Class",
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
                Padding = new Padding(25, 80, 25, 25),
                BackColor = Color.White,
                Margin = new Padding(0, 40, 0, 0),
                ShadowDecoration = { Enabled = true, Depth = 1, Color = Color.FromArgb(10, 0, 0, 0) }
            };

            dgvClasses = new Guna2DataGridView
            {
                Dock = DockStyle.Fill,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = Color.FromArgb(230, 239, 255),
                    SelectionForeColor = Color.Black
                },
                ThemeStyle = { HeaderStyle = { Font = new Font("Segoe UI Semibold", 10) } },
                ColumnHeadersHeight = 50,
                RowTemplate = { Height = 45 },
                BackgroundColor = Color.White,
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

            gridContainer.Controls.Add(dgvClasses);
            this.Controls.AddRange(new Control[] { actionPanel, gridContainer });

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnManageStudents.Click += (s, e) =>
            {
                if (dgvClasses.SelectedRows.Count > 0)
                {
                    int classId = Convert.ToInt32(dgvClasses.SelectedRows[0].Cells["ClassID"].Value);
                    string className = dgvClasses.SelectedRows[0].Cells["ClassName"].Value.ToString();
                    using (var form = new ClassStudentsForm(classId, className))
                    {
                        form.ShowDialog();
                        LoadClasses(); // Refresh to update student count
                    }
                }
                else
                {
                    MessageBox.Show("Please select a class to manage students.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            this.Resize += (s, e) =>
            {
                controlsContainer.Location = new Point(
                    (actionPanel.Width - controlsContainer.Width) / 2,
                    (actionPanel.Height - controlsContainer.Height) / 2
                );
            };
        }

        private void LoadClasses()
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT c.ClassID, c.ClassName, u.Username as TeacherName, 
                                   c.CreatedAt, (SELECT COUNT(*) FROM StudentClasses sc 
                                   WHERE sc.ClassID = c.ClassID) as StudentCount
                                   FROM Classes c
                                   JOIN Users u ON c.TeacherID = u.UserID";
                    
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvClasses.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading classes: " + ex.Message);
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new ClassForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadClasses();
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvClasses.SelectedRows.Count > 0)
            {
                int classId = Convert.ToInt32(dgvClasses.SelectedRows[0].Cells["ClassID"].Value);
                using (var form = new ClassForm(classId))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadClasses();
                    }
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvClasses.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete this class?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    int classId = Convert.ToInt32(dgvClasses.SelectedRows[0].Cells["ClassID"].Value);
                    DeleteClass(classId);
                }
            }
        }

        private void DeleteClass(int classId)
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string deleteEnrollments = "DELETE FROM StudentClasses WHERE ClassID = @ClassID";
                            using (SqlCommand cmd = new SqlCommand(deleteEnrollments, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ClassID", classId);
                                cmd.ExecuteNonQuery();
                            }

                            string deleteClass = "DELETE FROM Classes WHERE ClassID = @ClassID";
                            using (SqlCommand cmd = new SqlCommand(deleteClass, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ClassID", classId);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            MessageBox.Show("Class deleted successfully!");
                            LoadClasses();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting class: " + ex.Message);
                }
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();
            if (dgvClasses.DataSource is DataTable dataTable)
            {
                try
                {
                    DataView dv = dataTable.DefaultView;
                    dv.RowFilter = string.IsNullOrEmpty(searchText) ? string.Empty :
                        $"ClassName LIKE '%{searchText}%' OR TeacherName LIKE '%{searchText}%'";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Search Error: {ex.Message}");
                }
            }
        }
    }
} 