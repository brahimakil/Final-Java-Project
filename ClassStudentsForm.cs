using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using FontAwesome.Sharp;

namespace Project_College_App
{
    public class ClassStudentsForm : Form
    {
        private int classId;
        private string className;
        private Guna2DataGridView dgvStudents;
        private Guna2Button btnAdd;
        private Guna2Button btnRemove;
        private Label lblTitle;

        public ClassStudentsForm(int classId, string className)
        {
            this.classId = classId;
            this.className = className;
            InitializeComponents();
            LoadStudents();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = $"Manage Students - {className}";

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(20)
            };

            lblTitle = new Label
            {
                Text = $"Students in {className}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };

            btnAdd = new Guna2Button
            {
                Text = "Add Students",
                Size = new Size(120, 36),
                BorderRadius = 5,
                FillColor = Color.FromArgb(94, 148, 255),
                Image = IconChar.UserPlus.ToBitmap(Color.White, 16),
                ImageSize = new Size(16, 16),
                ImageAlign = HorizontalAlignment.Left,
                TextAlign = HorizontalAlignment.Left,
                ImageOffset = new Point(8, 0),
                TextOffset = new Point(3, 0)
            };

            btnRemove = new Guna2Button
            {
                Text = "Remove",
                Size = new Size(100, 36),
                BorderRadius = 5,
                FillColor = Color.FromArgb(255, 74, 74),
                Margin = new Padding(10, 0, 0, 0),
                Image = IconChar.UserMinus.ToBitmap(Color.White, 16),
                ImageSize = new Size(16, 16),
                ImageAlign = HorizontalAlignment.Left,
                TextAlign = HorizontalAlignment.Left,
                ImageOffset = new Point(8, 0),
                TextOffset = new Point(3, 0)
            };

            dgvStudents = new Guna2DataGridView
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White
            };

            buttonPanel.Controls.AddRange(new Control[] { btnAdd, btnRemove });
            mainPanel.Controls.Add(lblTitle, 0, 0);
            mainPanel.Controls.Add(buttonPanel, 0, 1);
            mainPanel.Controls.Add(dgvStudents, 0, 2);
            
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            this.Controls.Add(mainPanel);

            btnAdd.Click += BtnAdd_Click;
            btnRemove.Click += BtnRemove_Click;
        }

        private void LoadStudents()
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT DISTINCT 
                                   u.UserID,
                                   u.Username as 'Student Name',
                                   u.Email,
                                   u.CreatedAt as 'Join Date'
                                   FROM Users u
                                   INNER JOIN StudentClasses sc ON u.UserID = sc.StudentID
                                   WHERE sc.ClassID = @ClassID
                                   ORDER BY u.Username";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ClassID", classId);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvStudents.DataSource = dt;

                        // Configure columns
                        if (dgvStudents.Columns.Contains("UserID"))
                            dgvStudents.Columns["UserID"].Visible = false;
                        if (dgvStudents.Columns.Contains("Student Name"))
                            dgvStudents.Columns["Student Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        if (dgvStudents.Columns.Contains("Email"))
                            dgvStudents.Columns["Email"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        if (dgvStudents.Columns.Contains("Join Date"))
                        {
                            dgvStudents.Columns["Join Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            dgvStudents.Columns["Join Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
                        }
                    }
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
            using (var form = new AddStudentToClassForm(classId))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadStudents();
                }
            }
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (dgvStudents.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to remove the selected students from this class?",
                    "Confirm Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    RemoveSelectedStudents();
                }
            }
        }

        private void RemoveSelectedStudents()
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
                            foreach (DataGridViewRow row in dgvStudents.SelectedRows)
                            {
                                int studentId = Convert.ToInt32(row.Cells["UserID"].Value);
                                var cmd = new SqlCommand(
                                    "DELETE FROM StudentClasses WHERE ClassID = @ClassID AND StudentID = @StudentID",
                                    conn, transaction);
                                cmd.Parameters.AddWithValue("@ClassID", classId);
                                cmd.Parameters.AddWithValue("@StudentID", studentId);
                                cmd.ExecuteNonQuery();
                            }
                            transaction.Commit();
                            LoadStudents();
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
                    MessageBox.Show($"Error removing students: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}