using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Project_College_App
{
    public class QuizTakingForm : Form
    {
        private int quizId;
        private int userId;
        private Guna2Panel mainPanel;
        private Label lblTitle;
        private Label lblTimeRemaining;
        private FlowLayoutPanel questionsPanel;
        private Guna2Button btnSubmit;
        private Timer quizTimer;
        private int timeRemaining;
        private Dictionary<int, Guna2TextBox> answerBoxes;

        public QuizTakingForm(int quizId, int userId)
        {
            // Verify user is a student
            if (!IsUserStudent(userId))
            {
                MessageBox.Show("Only students can take quizzes.", "Access Denied",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }

            this.quizId = quizId;
            this.userId = userId;
            this.answerBoxes = new Dictionary<int, Guna2TextBox>();
            InitializeComponent();
            LoadQuiz();
            StartQuizTimer();
        }

        private bool IsUserStudent(int userId)
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Role FROM Users WHERE UserID = @UserID";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    string role = cmd.ExecuteScalar()?.ToString();
                    return role == "Student";
                }
            }
        }

        private void InitializeComponent()
        {
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;

            mainPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.White
            };

            lblTitle = new Label
            {
                Text = "Quiz",
                Font = new Font("Segoe UI Semibold", 16),
                Dock = DockStyle.Top,
                Height = 40
            };

            lblTimeRemaining = new Label
            {
                Text = "Time Remaining: ",
                Font = new Font("Segoe UI", 12),
                Dock = DockStyle.Top,
                Height = 30
            };

            questionsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            btnSubmit = new Guna2Button
            {
                Text = "Submit Quiz",
                Size = new Size(120, 36),
                BorderRadius = 5,
                FillColor = Color.FromArgb(94, 148, 255),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Dock = DockStyle.Bottom
            };

            mainPanel.Controls.AddRange(new Control[] { lblTitle, lblTimeRemaining, questionsPanel, btnSubmit });
            this.Controls.Add(mainPanel);

            btnSubmit.Click += BtnSubmit_Click;

            // Initialize timer
            quizTimer = new Timer();
            quizTimer.Interval = 1000; // 1 second
            quizTimer.Tick += QuizTimer_Tick;
        }

        private void StartQuizTimer()
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT TimeLimit FROM Quizzes WHERE QuizID = @QuizID";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizID", quizId);
                    timeRemaining = Convert.ToInt32(cmd.ExecuteScalar()) * 60; // Convert minutes to seconds
                }
            }
            UpdateTimeLabel();
            quizTimer.Start();
        }

        private void QuizTimer_Tick(object sender, EventArgs e)
        {
            timeRemaining--;
            UpdateTimeLabel();

            if (timeRemaining <= 0)
            {
                quizTimer.Stop();
                MessageBox.Show("Time's up! Submitting quiz...", "Time's Up",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                SubmitQuiz();
            }
        }

        private void UpdateTimeLabel()
        {
            int minutes = timeRemaining / 60;
            int seconds = timeRemaining % 60;
            lblTimeRemaining.Text = $"Time Remaining: {minutes:D2}:{seconds:D2}";
        }

        private void LoadQuiz()
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                
                // Load quiz details
                string quizQuery = @"SELECT Title, TimeLimit FROM Quizzes WHERE QuizID = @QuizID";
                using (var cmd = new SqlCommand(quizQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizID", quizId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblTitle.Text = reader["Title"].ToString();
                            timeRemaining = Convert.ToInt32(reader["TimeLimit"]) * 60;
                        }
                    }
                }

                // Load questions with correct answers
                string questionQuery = @"SELECT QuestionID, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectAnswer 
                                       FROM Questions WHERE QuizID = @QuizID ORDER BY QuestionOrder";
                using (var cmd = new SqlCommand(questionQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizID", quizId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AddQuestionToPanel(
                                Convert.ToInt32(reader["QuestionID"]),
                                reader["QuestionText"].ToString(),
                                reader["OptionA"].ToString(),
                                reader["OptionB"].ToString(),
                                reader["OptionC"].ToString(),
                                reader["OptionD"].ToString()
                            );
                        }
                    }
                }
            }
        }

        private void AddQuestionToPanel(int questionId, string questionText, string optionA, string optionB, string optionC, string optionD)
        {
            var questionPanel = new Panel
            {
                Width = questionsPanel.Width - 40,
                AutoSize = true,
                Padding = new Padding(10)
            };

            var lblQuestion = new Label
            {
                Text = questionText,
                AutoSize = true,
                Font = new Font("Segoe UI", 11),
                Dock = DockStyle.Top
            };

            var lblOptions = new Label
            {
                Text = $"A) {optionA}\nB) {optionB}\nC) {optionC}\nD) {optionD}",
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Top
            };

            var txtAnswer = new Guna2TextBox
            {
                Width = 100,
                Height = 36,
                PlaceholderText = "Your answer (A/B/C/D)",
                Font = new Font("Segoe UI", 10)
            };

            answerBoxes[questionId] = txtAnswer;
            
            questionPanel.Controls.AddRange(new Control[] { lblQuestion, lblOptions, txtAnswer });
            questionsPanel.Controls.Add(questionPanel);
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to submit your answers?", "Confirm Submit",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                SubmitQuiz();
            }
        }

        private void SubmitQuiz()
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int totalQuestions = answerBoxes.Count;
                        int correctAnswers = 0;

                        foreach (var answer in answerBoxes)
                        {
                            string query = "SELECT CorrectAnswer FROM Questions WHERE QuestionID = @QuestionID";
                            using (var cmd = new SqlCommand(query, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@QuestionID", answer.Key);
                                string correctAnswer = cmd.ExecuteScalar()?.ToString();
                                
                                if (answer.Value.Text.Trim().ToUpper() == correctAnswer?.Trim().ToUpper())
                                {
                                    correctAnswers++;
                                }
                            }
                        }

                        double score = (double)correctAnswers / totalQuestions * 100;

                        // Save quiz result with UserID instead of StudentID
                        string insertResult = @"INSERT INTO QuizResults (QuizID, UserID, Score, TimeTaken, CompletedAt)
                                              VALUES (@QuizID, @UserID, @Score, @TimeTaken, GETDATE())";
                        
                        using (var cmd = new SqlCommand(insertResult, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@QuizID", quizId);
                            cmd.Parameters.AddWithValue("@UserID", userId);
                            cmd.Parameters.AddWithValue("@Score", (int)score);
                            cmd.Parameters.AddWithValue("@TimeTaken", timeRemaining);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show($"Quiz submitted successfully!\nYour score: {score:F1}%", "Quiz Complete",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Error submitting quiz: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ParseAndSaveQuestions(string quizText, int quizId, SqlConnection conn, SqlTransaction transaction)
        {
            string[] lines = quizText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string currentQuestion = "";
            List<string> options = new List<string>();
            string correctAnswer = "";
            int questionNumber = 1;

            foreach (string line in lines)
            {
                if (line.Trim().StartsWith("Q"))
                {
                    // Save previous question if exists
                    if (!string.IsNullOrEmpty(currentQuestion) && options.Count == 4)
                    {
                        SaveQuestion(quizId, currentQuestion, options[0], options[1], options[2], options[3], 
                                   correctAnswer, questionNumber, conn, transaction);
                        questionNumber++;
                        options.Clear();
                    }
                    currentQuestion = line.Trim();
                }
                else if (line.Trim().StartsWith("A) ") || line.Trim().StartsWith("B) ") || 
                        line.Trim().StartsWith("C) ") || line.Trim().StartsWith("D) "))
                {
                    options.Add(line.Trim());
                }
                else if (line.Trim().StartsWith("Correct Answer:"))
                {
                    correctAnswer = line.Replace("Correct Answer:", "").Trim();
                }
            }

            // Save the last question
            if (!string.IsNullOrEmpty(currentQuestion) && options.Count == 4)
            {
                SaveQuestion(quizId, currentQuestion, options[0], options[1], options[2], options[3], 
                            correctAnswer, questionNumber, conn, transaction);
            }
        }

        private void SaveQuestion(int quizId, string questionText, string optionA, string optionB, 
            string optionC, string optionD, string correctAnswer, int questionOrder, 
            SqlConnection conn, SqlTransaction transaction)
        {
            string query = @"INSERT INTO Questions (QuizID, QuestionText, OptionA, OptionB, OptionC, OptionD, 
                            CorrectAnswer, QuestionOrder) 
                            VALUES (@QuizID, @QuestionText, @OptionA, @OptionB, @OptionC, @OptionD, 
                            @CorrectAnswer, @QuestionOrder)";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@QuizID", quizId);
                cmd.Parameters.AddWithValue("@QuestionText", questionText);
                cmd.Parameters.AddWithValue("@OptionA", optionA);
                cmd.Parameters.AddWithValue("@OptionB", optionB);
                cmd.Parameters.AddWithValue("@OptionC", optionC);
                cmd.Parameters.AddWithValue("@OptionD", optionD);
                cmd.Parameters.AddWithValue("@CorrectAnswer", correctAnswer);
                cmd.Parameters.AddWithValue("@QuestionOrder", questionOrder);

                cmd.ExecuteNonQuery();
            }
        }
    }
} 