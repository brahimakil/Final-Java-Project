using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using FontAwesome.Sharp;

namespace Project_College_App
{
    public class StudentCoursesPanel : Guna2Panel
    {
        private Guna2DataGridView dgvCourses;
        private Guna2DataGridView dgvQuizzes;
        private Guna2TextBox txtSearch;
        private Guna2Button btnTakeQuiz;
        private Guna2Button btnViewResults;
        private Label lblCourses;
        private Label lblQuizzes;
        private int studentId;

        public StudentCoursesPanel(int studentId)
        {
            this.studentId = studentId;
            InitializeComponents();
            LoadCourses();
            LoadQuizzes();
        }

        private void InitializeComponents()
        {
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(20);

            var mainContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(0),
                RowStyles = 
                {
                    new RowStyle(SizeType.AutoSize),
                    new RowStyle(SizeType.Percent, 45),
                    new RowStyle(SizeType.AutoSize),
                    new RowStyle(SizeType.Percent, 55)
                }
            };

            // Courses section
            lblCourses = new Label
            {
                Text = "My Enrolled Courses",
                Font = new Font("Segoe UI Semibold", 14),
                ForeColor = Color.FromArgb(64, 64, 64),
                Margin = new Padding(0, 0, 0, 10)
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
                BorderStyle = BorderStyle.None,
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
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // Quizzes section
            lblQuizzes = new Label
            {
                Text = "Available Quizzes",
                Font = new Font("Segoe UI Semibold", 14),
                ForeColor = Color.FromArgb(64, 64, 64),
                Margin = new Padding(0, 20, 0, 10)
            };

            var quizButtonPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = new Padding(0, 0, 0, 10)
            };

            btnTakeQuiz = new Guna2Button
            {
                Text = "Take Quiz",
                Image = IconChar.Edit.ToBitmap(Color.White, 20),
                ImageAlign = HorizontalAlignment.Left,
                ImageSize = new Size(20, 20),
                Size = new Size(120, 36),
                BorderRadius = 5,
                FillColor = Color.FromArgb(94, 148, 255),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Enabled = false
            };

            btnViewResults = new Guna2Button
            {
                Text = "View Results",
                Image = IconChar.ChartBar.ToBitmap(Color.White, 20),
                ImageAlign = HorizontalAlignment.Left,
                ImageSize = new Size(20, 20),
                Size = new Size(120, 36),
                BorderRadius = 5,
                FillColor = Color.FromArgb(75, 68, 83),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(10, 0, 0, 0),
                Enabled = false
            };

            quizButtonPanel.Controls.AddRange(new Control[] { btnTakeQuiz, btnViewResults });

            dgvQuizzes = new Guna2DataGridView
            {
                Dock = DockStyle.Fill,
                DefaultCellStyle = dgvCourses.DefaultCellStyle.Clone(),
                ColumnHeadersHeight = 50,
                RowTemplate = { Height = 45 },
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.Single,
                GridColor = Color.FromArgb(231, 229, 255),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersDefaultCellStyle = dgvCourses.ColumnHeadersDefaultCellStyle.Clone(),
                EnableHeadersVisualStyles = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            mainContainer.Controls.Add(lblCourses, 0, 0);
            mainContainer.Controls.Add(dgvCourses, 0, 1);
            mainContainer.Controls.Add(lblQuizzes, 0, 2);
            mainContainer.Controls.Add(quizButtonPanel, 0, 2);
            mainContainer.Controls.Add(dgvQuizzes, 0, 3);

            this.Controls.Add(mainContainer);

            // Event handlers
            dgvQuizzes.SelectionChanged += DgvQuizzes_SelectionChanged;
            btnTakeQuiz.Click += BtnTakeQuiz_Click;
            btnViewResults.Click += BtnViewResults_Click;
        }

        private void LoadCourses()
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT c.ClassID, c.ClassName, u.Username as TeacherName
                                   FROM Classes c
                                   INNER JOIN StudentClasses sc ON c.ClassID = sc.ClassID
                                   INNER JOIN Users u ON c.TeacherID = u.UserID
                                   WHERE sc.StudentID = @StudentID";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", studentId);
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

        private void LoadQuizzes()
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT q.QuizID, q.Title, q.Description, q.TimeLimit, q.PassingScore,
                                   CASE WHEN qr.ResultID IS NULL THEN 'Not Attempted' 
                                        ELSE 'Completed' END as Status
                                   FROM Quizzes q
                                   LEFT JOIN QuizResults qr ON q.QuizID = qr.QuizID 
                                   AND qr.UserID = @StudentID
                                   WHERE q.IsActive = 1";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", studentId);
                        var adapter = new SqlDataAdapter(cmd);
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        dgvQuizzes.DataSource = dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading quizzes: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DgvQuizzes_SelectionChanged(object sender, EventArgs e)
        {
            bool hasSelection = dgvQuizzes.SelectedRows.Count > 0;
            if (hasSelection)
            {
                string status = dgvQuizzes.SelectedRows[0].Cells["Status"].Value.ToString();
                btnTakeQuiz.Enabled = status == "Not Attempted";
                btnViewResults.Enabled = status == "Completed";
            }
            else
            {
                btnTakeQuiz.Enabled = false;
                btnViewResults.Enabled = false;
            }
        }

        private void BtnTakeQuiz_Click(object sender, EventArgs e)
        {
            if (dgvQuizzes.SelectedRows.Count > 0)
            {
                int quizId = Convert.ToInt32(dgvQuizzes.SelectedRows[0].Cells["QuizID"].Value);
                // TODO: Implement QuizForm
                using (var form = new QuizForm(quizId, studentId))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadQuizzes();
                    }
                }
            }
        }

        private void BtnViewResults_Click(object sender, EventArgs e)
        {
            if (dgvQuizzes.SelectedRows.Count > 0)
            {
                int quizId = Convert.ToInt32(dgvQuizzes.SelectedRows[0].Cells["QuizID"].Value);
                // TODO: Implement QuizResultForm
                using (var form = new QuizResultForm(quizId, studentId))
                {
                    form.ShowDialog();
                }
            }
        }
    }
} 