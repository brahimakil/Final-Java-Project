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
        private Guna2Button btnAdd;
        private Guna2Button btnEdit;
        private Guna2Button btnDelete;
        private Guna2Button btnManageStudents;
        private Guna2TextBox txtSearch;
        private int? teacherId;
        private Label instructionLabel;

        public ClassesPanel(int? teacherId = null)
        {
            this.teacherId = teacherId;
            InitializeComponents();
            LoadClasses();
        }

        private void InitializeComponents()
        {
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(40);

            var mainContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                RowStyles = {
                    new RowStyle(SizeType.Absolute, 100),
                    new RowStyle(SizeType.Percent, 100)
                }
            };

            // Action panel for buttons
            var actionPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                Height = 80,
                Padding = new Padding(0, 20, 0, 20)
            };

            var buttonContainer = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                Anchor = AnchorStyles.Right
            };

            txtSearch = new Guna2TextBox
            {
                PlaceholderText = "Search classes...",
                Size = new Size(200, 36),
                BorderRadius = 5,
                Margin = new Padding(0, 0, 10, 0)
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
                Text = "Edit",
                Size = new Size(100, 36),
                BorderRadius = 5,
                FillColor = Color.FromArgb(75, 68, 83),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(10, 0, 0, 0),
                Enabled = false
            };

            btnDelete = new Guna2Button
            {
                Text = "Delete",
                Size = new Size(100, 36),
                BorderRadius = 5,
                FillColor = Color.FromArgb(255, 74, 74),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(10, 0, 0, 0),
                Enabled = false
            };

            btnManageStudents = new Guna2Button
            {
                Text = "Manage Students",
                Size = new Size(140, 36),
                FillColor = Color.FromArgb(94, 148, 255),
                BorderRadius = 5,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Image = IconChar.UserGroup.ToBitmap(Color.White, 16),
                ImageSize = new Size(16, 16),
                ImageAlign = HorizontalAlignment.Left,
                TextAlign = HorizontalAlignment.Left,
                ImageOffset = new Point(8, 0),
                TextOffset = new Point(3, 0),
                Enabled = false
            };

            buttonContainer.Controls.AddRange(new Control[] { txtSearch, btnAdd, btnEdit, btnDelete, btnManageStudents });
            actionPanel.Controls.Add(buttonContainer);
            buttonContainer.Location = new Point(
                actionPanel.Width - buttonContainer.Width - 30,
                (actionPanel.Height - buttonContainer.Height) / 2
            );

            var gridContainer = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(25, 80, 25, 25),
                BackColor = Color.White,
                Margin = new Padding(0, 40, 0, 0),
                ShadowDecoration = { Enabled = true, Depth = 1, Color = Color.FromArgb(10, 0, 0, 0) }
            };

            var gridWrapper = new Panel
            {
                Dock = DockStyle.Right,
                Width = 800,  // Reduced width
                Padding = new Padding(0),
                AutoScroll = true  // Enable scrolling
            };

            dgvClasses = new Guna2DataGridView
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
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(242, 242, 242),
                    ForeColor = Color.FromArgb(66, 66, 66),
                    Font = new Font("Segoe UI Semibold", 11),
                    Padding = new Padding(10)
                },
                EnableHeadersVisualStyles = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ScrollBars = ScrollBars.Both  // Enable both scrollbars
            };

            gridWrapper.Controls.Add(dgvClasses);
            gridContainer.Controls.Add(gridWrapper);
            mainContainer.Controls.Add(actionPanel, 0, 0);
            mainContainer.Controls.Add(gridContainer, 0, 1);
            this.Controls.Add(mainContainer);

            // Create instruction label once
            instructionLabel = new Label
            {
                Text = "Click on a row to manage students",
                ForeColor = Color.Gray,
                AutoSize = true
            };
            this.Controls.Add(instructionLabel);

            // Events
            dgvClasses.CellClick += DgvClasses_CellClick;
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnManageStudents.Click += BtnManageStudents_Click;
            txtSearch.TextChanged += TxtSearch_TextChanged;
            dgvClasses.SelectionChanged += DgvClasses_SelectionChanged;
        }

        private void DgvClasses_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == dgvClasses.Columns["ManageQuiz"].Index)
                {
                    int classId = Convert.ToInt32(dgvClasses.Rows[e.RowIndex].Cells["ClassID"].Value);
                    string className = dgvClasses.Rows[e.RowIndex].Cells["ClassName"].Value.ToString();
                    
                    using (var form = new QuizManagementForm(classId, className, teacherId ?? 0))
                    {
                        form.ShowDialog();
                    }
                }
            }
        }

        private void LoadClasses()
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query;
                    
                    if (teacherId.HasValue)
                    {
                        query = @"SELECT c.ClassID, c.ClassName, 
                                 (SELECT COUNT(*) FROM StudentClasses sc WHERE sc.ClassID = c.ClassID) as StudentCount 
                                 FROM Classes c 
                                 WHERE c.TeacherID = @TeacherID";
                    }
                    else
                    {
                        query = @"SELECT c.ClassID, c.ClassName, 
                                 u.Username as TeacherName,
                                 (SELECT COUNT(*) FROM StudentClasses sc WHERE sc.ClassID = c.ClassID) as StudentCount,
                                 c.CreatedAt,
                                 c.IsActive
                                 FROM Classes c 
                                 LEFT JOIN Users u ON c.TeacherID = u.UserID
                                 ORDER BY c.CreatedAt DESC";
                    }

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        if (teacherId.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@TeacherID", teacherId);
                        }
                        
                        var adapter = new SqlDataAdapter(cmd);
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        dgvClasses.DataSource = dt;

                        ConfigureGridColumns();

                        // Add Manage Quiz button column if it doesn't exist
                        if (!dgvClasses.Columns.Contains("ManageQuiz"))
                        {
                            var manageQuizColumn = new DataGridViewButtonColumn
                            {
                                Name = "ManageQuiz",
                                HeaderText = "Quiz",
                                Text = "Manage Quiz",
                                UseColumnTextForButtonValue = true,
                                FlatStyle = FlatStyle.Flat,
                                DefaultCellStyle = new DataGridViewCellStyle
                                {
                                    BackColor = Color.FromArgb(94, 148, 255),
                                    ForeColor = Color.White,
                                    SelectionBackColor = Color.FromArgb(94, 148, 255),
                                    SelectionForeColor = Color.White,
                                    Padding = new Padding(5)
                                }
                            };
                            dgvClasses.Columns.Add(manageQuizColumn);
                        }

                        // Update instruction label position
                        instructionLabel.Location = new Point(10, dgvClasses.Bottom + 5);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading classes: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ConfigureGridColumns()
        {
            if (dgvClasses.Columns.Contains("ClassID"))
                dgvClasses.Columns["ClassID"].Visible = false;
            if (dgvClasses.Columns.Contains("ClassName"))
                dgvClasses.Columns["ClassName"].HeaderText = "Class Name";
            if (dgvClasses.Columns.Contains("TeacherName"))
                dgvClasses.Columns["TeacherName"].HeaderText = "Teacher";
            if (dgvClasses.Columns.Contains("StudentCount"))
                dgvClasses.Columns["StudentCount"].HeaderText = "Students";
            if (dgvClasses.Columns.Contains("CreatedAt"))
                dgvClasses.Columns["CreatedAt"].HeaderText = "Created Date";
            if (dgvClasses.Columns.Contains("IsActive"))
                dgvClasses.Columns["IsActive"].HeaderText = "Active";
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var classForm = teacherId.HasValue 
                ? new ClassForm(teacherId.Value)  // Teacher mode
                : new ClassForm();                // Admin mode
            
            if (classForm.ShowDialog() == DialogResult.OK)
            {
                LoadClasses();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvClasses.SelectedRows.Count > 0)
            {
                int classId = Convert.ToInt32(dgvClasses.SelectedRows[0].Cells["ClassID"].Value);
                var classForm = teacherId.HasValue
                    ? new ClassForm(teacherId.Value, classId)  // Teacher mode
                    : new ClassForm();                         // Admin mode
                
                if (classForm.ShowDialog() == DialogResult.OK)
                {
                    LoadClasses();
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
            if (dgvClasses.DataSource is DataTable dt)
            {
                string searchText = txtSearch.Text.Trim().ToLower();
                string filterExpression;

                if (teacherId.HasValue)
                {
                    filterExpression = string.IsNullOrEmpty(searchText) ? "" 
                        : $"ClassName LIKE '%{searchText}%'";
                }
                else
                {
                    filterExpression = string.IsNullOrEmpty(searchText) ? "" 
                        : $"ClassName LIKE '%{searchText}%' OR TeacherName LIKE '%{searchText}%'";
                }

                dt.DefaultView.RowFilter = filterExpression;
            }
        }

        private void BtnManageStudents_Click(object sender, EventArgs e)
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
        }

        private void DgvClasses_SelectionChanged(object sender, EventArgs e)
        {
            bool hasSelection = dgvClasses.SelectedRows.Count > 0;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
            btnManageStudents.Enabled = hasSelection;
        }
    }
} 