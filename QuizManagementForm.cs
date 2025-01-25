using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using FontAwesome.Sharp;

namespace Project_College_App
{
    public class QuizManagementForm : Form
    {
        private int classId;
        private string className;
        private int teacherId;
        private Guna2DataGridView dgvQuizzes;
        private Guna2Button btnAdd;
        private Guna2Button btnEdit;
        private Guna2Button btnDelete;
        private Label lblTitle;

        public QuizManagementForm(int classId, string className, int teacherId)
        {
            this.classId = classId;
            this.className = className;
            this.teacherId = teacherId;
            InitializeComponents();
            LoadQuizzes();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = $"Manage Quizzes - {className}";

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(20)
            };

            lblTitle = new Label
            {
                Text = $"Quizzes for {className}",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(68, 88, 112),
                AutoSize = true
            };

            var buttonPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = new Padding(0, 10, 0, 10)
            };

            btnAdd = new Guna2Button
            {
                Text = "Add Quiz",
                Size = new Size(120, 36),
                BorderRadius = 5,
                FillColor = Color.FromArgb(94, 148, 255),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Image = IconChar.Plus.ToBitmap(Color.White, 16),
                ImageSize = new Size(16, 16),
                ImageAlign = HorizontalAlignment.Left,
                TextAlign = HorizontalAlignment.Left,
                ImageOffset = new Point(8, 0),
                TextOffset = new Point(3, 0)
            };

            btnEdit = new Guna2Button
            {
                Text = "Edit Quiz",
                Size = new Size(120, 36),
                BorderRadius = 5,
                FillColor = Color.FromArgb(94, 148, 255),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Enabled = false,
                Margin = new Padding(10, 0, 0, 0)
            };

            btnDelete = new Guna2Button
            {
                Text = "Delete Quiz",
                Size = new Size(120, 36),
                BorderRadius = 5,
                FillColor = Color.FromArgb(255, 74, 74),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Enabled = false,
                Margin = new Padding(10, 0, 0, 0)
            };

            dgvQuizzes = new Guna2DataGridView
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
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true
            };

            buttonPanel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });
            mainPanel.Controls.Add(lblTitle, 0, 0);
            mainPanel.Controls.Add(buttonPanel, 0, 1);
            mainPanel.Controls.Add(dgvQuizzes, 0, 2);

            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            this.Controls.Add(mainPanel);

            dgvQuizzes.SelectionChanged += DgvQuizzes_SelectionChanged;
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
        }

        private void LoadQuizzes()
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT q.QuizID, q.Title, q.TimeLimit, 
                                   q.PassingScore, q.IsActive,
                                   (SELECT COUNT(*) FROM Questions WHERE QuizID = q.QuizID) as QuestionCount
                                   FROM Quizzes q
                                   WHERE q.ClassID = @ClassID
                                   ORDER BY q.CreatedAt DESC";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ClassID", classId);
                        var dt = new DataTable();
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }

                        dgvQuizzes.DataSource = dt;

                        // Configure column headers
                        dgvQuizzes.Columns["QuizID"].Visible = false;
                        dgvQuizzes.Columns["Title"].HeaderText = "Quiz Title";
                        dgvQuizzes.Columns["TimeLimit"].HeaderText = "Time Limit (mins)";
                        dgvQuizzes.Columns["PassingScore"].HeaderText = "Passing Score";
                        dgvQuizzes.Columns["IsActive"].HeaderText = "Active";
                        dgvQuizzes.Columns["QuestionCount"].HeaderText = "Questions";
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
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new QuizEditorForm(classId, className, teacherId))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadQuizzes();
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvQuizzes.SelectedRows.Count > 0)
            {
                int quizId = Convert.ToInt32(dgvQuizzes.SelectedRows[0].Cells["QuizID"].Value);
                using (var form = new QuizEditorForm(classId, className, teacherId, quizId))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadQuizzes();
                    }
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvQuizzes.SelectedRows.Count > 0)
            {
                var result = MessageBox.Show("Are you sure you want to delete this quiz?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    int quizId = Convert.ToInt32(dgvQuizzes.SelectedRows[0].Cells["QuizID"].Value);
                    DeleteQuiz(quizId);
                }
            }
        }

        private void DeleteQuiz(int quizId)
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
                            // First delete quiz results
                            string deleteResults = "DELETE FROM QuizResults WHERE QuizID = @QuizID";
                            using (var cmd = new SqlCommand(deleteResults, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@QuizID", quizId);
                                cmd.ExecuteNonQuery();
                            }

                            // Then delete questions
                            string deleteQuestions = "DELETE FROM Questions WHERE QuizID = @QuizID";
                            using (var cmd = new SqlCommand(deleteQuestions, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@QuizID", quizId);
                                cmd.ExecuteNonQuery();
                            }

                            // Finally delete the quiz
                            string deleteQuiz = "DELETE FROM Quizzes WHERE QuizID = @QuizID";
                            using (var cmd = new SqlCommand(deleteQuiz, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@QuizID", quizId);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            LoadQuizzes();
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
                    MessageBox.Show($"Error deleting quiz: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
} 