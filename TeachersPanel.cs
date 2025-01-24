using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using FontAwesome.Sharp;

namespace Project_College_App
{
    public class TeachersPanel : Guna2Panel
    {
        private Guna2DataGridView dgvTeachers;
        private Guna2TextBox txtSearch;
        private Guna2Button btnAdd;
        private Guna2Button btnEdit;
        private Guna2Button btnDelete;
        private Guna2Panel actionPanel;

        public TeachersPanel()
        {
            InitializeComponents();
            LoadTeachers();
        }

        private void InitializeComponents()
        {
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(30);
            this.BackColor = Color.FromArgb(240, 244, 247);

            // Action Panel with modern design
            actionPanel = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                Margin = new Padding(0, 0, 0, 40),
                FillColor = Color.White,
                ShadowDecoration = { Enabled = true, Depth = 1, Color = Color.FromArgb(10, 0, 0, 0) }
            };

            // Create a flow layout panel for centered controls
            var controlsContainer = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(20, 25, 20, 25)
            };

            // Modern Search Box
            txtSearch = new Guna2TextBox
            {
                PlaceholderText = "Search teachers...",
                Size = new Size(400, 36),
                BorderRadius = 5,
                Font = new Font("Segoe UI", 9),
                FillColor = Color.White,
                BorderColor = Color.FromArgb(213, 218, 223),
                PlaceholderForeColor = Color.FromArgb(125, 137, 149),
                Margin = new Padding(0, 0, 20, 0) // Add right margin
            };

            // CRUD Buttons with margins
            btnAdd = new Guna2Button
            {
                Text = "Add Teacher",
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

            // Add controls to the flow layout panel
            controlsContainer.Controls.AddRange(new Control[] { txtSearch, btnAdd, btnEdit, btnDelete });

            // Center the controls container in the action panel
            actionPanel.Controls.Add(controlsContainer);
            controlsContainer.Location = new Point(
                (actionPanel.Width - controlsContainer.Width) / 2,
                (actionPanel.Height - controlsContainer.Height) / 2
            );

            // Grid container with increased spacing
            var gridContainer = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(25, 80, 25, 25),
                BackColor = Color.White,
                Margin = new Padding(0, 40, 0, 0),
                ShadowDecoration = { Enabled = true, Depth = 1, Color = Color.FromArgb(10, 0, 0, 0) }
            };

            // DataGridView settings
            dgvTeachers = new Guna2DataGridView
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

            // Layout
            gridContainer.Controls.Add(dgvTeachers);
            this.Controls.Add(actionPanel);
            this.Controls.Add(gridContainer);

            // Events
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            txtSearch.TextChanged += TxtSearch_TextChanged;

            // Handle resizing
            this.Resize += (s, e) =>
            {
                controlsContainer.Location = new Point(
                    (actionPanel.Width - controlsContainer.Width) / 2,
                    (actionPanel.Height - controlsContainer.Height) / 2
                );
            };
        }

        private void LoadTeachers()
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Initial Catalog=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT UserID, Username, Email, CreatedAt, LastLogin 
                                   FROM Users WHERE Role = 'Teacher'";
                    
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvTeachers.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading teachers: " + ex.Message);
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new TeacherForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadTeachers();
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvTeachers.SelectedRows.Count > 0)
            {
                int userId = Convert.ToInt32(dgvTeachers.SelectedRows[0].Cells["UserID"].Value);
                using (var form = new TeacherForm(userId))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadTeachers();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a teacher to edit.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvTeachers.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete this teacher?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    int userId = Convert.ToInt32(dgvTeachers.SelectedRows[0].Cells["UserID"].Value);
                    DeleteTeacher(userId);
                }
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();
            if (dgvTeachers.DataSource is DataTable dataTable)
            {
                try
                {
                    DataView dv = dataTable.DefaultView;
                    if (string.IsNullOrEmpty(searchText))
                    {
                        dv.RowFilter = string.Empty;  // Clear the filter to show all records
                    }
                    else
                    {
                        dv.RowFilter = $"Username LIKE '%{searchText}%' OR Email LIKE '%{searchText}%'";
                    }
                    dgvTeachers.DataSource = dv.ToTable();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Search Error: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DeleteTeacher(int userId)
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Initial Catalog=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM Users WHERE UserID = @UserID AND Role = 'Teacher'";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        int result = cmd.ExecuteNonQuery();
                        
                        if (result > 0)
                        {
                            MessageBox.Show("Teacher deleted successfully!", "Success", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadTeachers(); // Refresh the grid
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete teacher.", "Error", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting teacher: " + ex.Message, "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
} 