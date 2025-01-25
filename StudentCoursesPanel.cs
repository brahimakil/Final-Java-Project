using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using FontAwesome.Sharp;

namespace Project_College_App
{
    public class StudentCoursesPanel : Guna2Panel
    {
        private Guna2DataGridView dgvCourses;
        private Label lblCourses;
        private int studentId;
        private string connectionString;
        private int userId;

        public StudentCoursesPanel(int studentId)
        {
            this.studentId = studentId;
            connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            userId = studentId;
            InitializeComponents();
            LoadCourses();
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
                    new RowStyle(SizeType.Absolute, 60),
                    new RowStyle(SizeType.Percent, 100)
                }
            };

            // Header label panel
            var headerPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 0, 0, 10)
            };

            lblCourses = new Label
            {
                Text = "My Enrolled Courses",
                Font = new Font("Segoe UI Semibold", 14),
                ForeColor = Color.FromArgb(64, 64, 64),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(600, 10)  // Shifted to the right
            };

            headerPanel.Controls.Add(lblCourses);

            // Grid container with shadow
            var gridContainer = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(25, 25, 25, 25),
                BackColor = Color.White,
                Margin = new Padding(0, 20, 0, 0),
                ShadowDecoration = { Enabled = true, Depth = 1, Color = Color.FromArgb(10, 0, 0, 0) }
            };

            var gridWrapper = new Panel
            {
                Dock = DockStyle.Right,
                Width = 800,
                Padding = new Padding(0),
                AutoScroll = true
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
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ScrollBars = ScrollBars.Both
            };

            // Add controls in the correct hierarchy
            gridWrapper.Controls.Add(dgvCourses);
            gridContainer.Controls.Add(gridWrapper);
            mainContainer.Controls.Add(headerPanel, 0, 0);
            mainContainer.Controls.Add(gridContainer, 0, 1);
            this.Controls.Add(mainContainer);

            dgvCourses.CellClick += DgvCourses_CellClick;
        }

        private void LoadCourses()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT c.ClassID, c.ClassName, t.Username as Teacher,
                                   CASE WHEN EXISTS (
                                       SELECT 1 FROM Quizzes q 
                                       WHERE q.ClassID = c.ClassID AND q.IsActive = 1
                                   ) THEN 'Yes' ELSE 'No' END as HasActiveQuiz
                                   FROM StudentClasses sc
                                   JOIN Classes c ON sc.ClassID = c.ClassID
                                   JOIN Users t ON c.TeacherID = t.UserID
                                   WHERE sc.StudentID = @StudentID";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", studentId);
                        var adapter = new SqlDataAdapter(cmd);
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        // Add DoQuiz button column
                        var buttonColumn = new DataGridViewButtonColumn
                        {
                            Name = "DoQuiz",
                            Text = "Do Quiz",
                            UseColumnTextForButtonValue = true
                        };
                        dgvCourses.Columns.Add(buttonColumn);

                        dgvCourses.DataSource = dt;

                        // Configure button visibility based on HasActiveQuiz
                        foreach (DataGridViewRow row in dgvCourses.Rows)
                        {
                            var hasActiveQuiz = row.Cells["HasActiveQuiz"].Value.ToString();
                            if (hasActiveQuiz == "No")
                            {
                                row.Cells["DoQuiz"].Value = string.Empty;
                                row.Cells["DoQuiz"].ReadOnly = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading courses: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DgvCourses_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvCourses.Columns["DoQuiz"].Index)
            {
                int classId = Convert.ToInt32(dgvCourses.Rows[e.RowIndex].Cells["ClassID"].Value);
                
                // First check if student has already taken the quiz
                if (HasStudentTakenQuiz(classId))
                {
                    MessageBox.Show("You have already taken this quiz.", "Quiz Completed",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Then check if there's an active quiz
                if (!HasActiveQuiz(classId))
                {
                    MessageBox.Show("There is no active quiz available for this class.", "No Quiz",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (var conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "SELECT QuizID FROM Quizzes WHERE ClassID = @ClassID AND IsActive = 1";
                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@ClassID", classId);
                            var quizId = Convert.ToInt32(cmd.ExecuteScalar());

                            using (var quizForm = new QuizTakingForm(quizId, userId))
                            {
                                quizForm.ShowDialog();
                                if (quizForm.DialogResult == DialogResult.OK)
                                {
                                    LoadCourses();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading quiz: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private bool HasStudentTakenQuiz(int quizId)
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT COUNT(*) FROM QuizResults 
                                   WHERE QuizID = @QuizID AND UserID = @StudentID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@QuizID", quizId);
                        cmd.Parameters.AddWithValue("@StudentID", studentId);
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error checking quiz status: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        private bool HasActiveQuiz(int classId)
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Quizzes WHERE ClassID = @ClassID AND IsActive = 1";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ClassID", classId);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }
    }
} 