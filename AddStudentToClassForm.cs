using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using FontAwesome.Sharp;

namespace Project_College_App
{
    public class AddStudentToClassForm : Form
    {
        private int classId;
        private Guna2DataGridView dgvAvailableStudents;
        private Guna2Button btnAdd;
        private Guna2Button btnCancel;
        private Guna2TextBox txtSearch;

        public AddStudentToClassForm(int classId)
        {
            this.classId = classId;
            InitializeComponents();
            LoadAvailableStudents();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Add Students to Class";

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                RowCount = 3,
                ColumnCount = 1
            };

            txtSearch = new Guna2TextBox
            {
                PlaceholderText = "Search students...",
                Size = new Size(300, 36),
                BorderRadius = 5,
                Margin = new Padding(0, 0, 0, 10)
            };

            dgvAvailableStudents = new Guna2DataGridView
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true
            };

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 0)
            };

            btnAdd = new Guna2Button
            {
                Text = "Add Selected",
                Size = new Size(120, 36),
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

            buttonPanel.Controls.AddRange(new Control[] { btnAdd, btnCancel });
            panel.Controls.AddRange(new Control[] { txtSearch, dgvAvailableStudents, buttonPanel });
            this.Controls.Add(panel);

            btnAdd.Click += BtnAdd_Click;
            btnCancel.Click += (s, e) => this.Close();
            txtSearch.TextChanged += TxtSearch_TextChanged;
        }

        private void LoadAvailableStudents()
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT u.UserID, u.Username, u.Email 
                                   FROM Users u
                                   WHERE u.Role = 'Student'
                                   AND u.UserID NOT IN (
                                       SELECT StudentID 
                                       FROM StudentClasses 
                                       WHERE ClassID = @ClassID)";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    adapter.SelectCommand.Parameters.AddWithValue("@ClassID", classId);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvAvailableStudents.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading students: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (dgvAvailableStudents.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select at least one student to add.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            AddSelectedStudents();
        }

        private void AddSelectedStudents()
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (DataGridViewRow row in dgvAvailableStudents.SelectedRows)
                            {
                                int studentId = Convert.ToInt32(row.Cells["UserID"].Value);
                                var cmd = new SqlCommand(
                                    "INSERT INTO StudentClasses (ClassID, StudentID) VALUES (@ClassID, @StudentID)",
                                    conn, transaction);
                                cmd.Parameters.AddWithValue("@ClassID", classId);
                                cmd.Parameters.AddWithValue("@StudentID", studentId);
                                cmd.ExecuteNonQuery();
                            }
                            transaction.Commit();
                            
                            // Set DialogResult before closing
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding students: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();
            if (dgvAvailableStudents.DataSource is DataTable dataTable)
            {
                try
                {
                    DataView dv = dataTable.DefaultView;
                    dv.RowFilter = string.IsNullOrEmpty(searchText) ? string.Empty :
                        $"Username LIKE '%{searchText}%' OR Email LIKE '%{searchText}%'";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Search Error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
} 