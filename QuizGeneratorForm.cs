using System;
using System.Net.Http;
using System.Text.Json;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.Text;
using System.Threading.Tasks;

namespace Project_College_App
{
    public partial class QuizGeneratorForm : Form
    {
        private readonly string GEMINI_API_KEY = "AIzaSyBswCA-tmUntZ_Yj8SHCjyQ5UyuJ3HPBoI";
        private readonly string API_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent";
        
        private int teacherId;
        private Guna2Panel mainPanel;
        private Guna2TextBox txtTopic;
        private Guna2TextBox txtDescription;
        private Guna2NumericUpDown numQuestions;
        private Guna2NumericUpDown numTimeLimit;
        private Guna2NumericUpDown numPassingScore;
        private RichTextBox txtGeneratedQuiz;
        private Guna2Button btnGenerate;
        private Guna2Button btnSubmit;
        private Guna2Button btnCancel;

        public QuizGeneratorForm(int teacherId)
        {
            this.teacherId = teacherId;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(800, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Generate Quiz";

            mainPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            var inputPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 200,
                ColumnCount = 2,
                RowCount = 5
            };

            txtTopic = new Guna2TextBox
            {
                PlaceholderText = "Enter topic (e.g., 'JavaScript Arrays and Objects')",
                Font = new Font("Segoe UI", 10)
            };

            txtDescription = new Guna2TextBox
            {
                PlaceholderText = "Quiz description",
                Font = new Font("Segoe UI", 10),
                Multiline = true,
                Height = 60
            };

            numQuestions = new Guna2NumericUpDown
            {
                Minimum = 1,
                Maximum = 20,
                Value = 5,
                Font = new Font("Segoe UI", 10)
            };

            numTimeLimit = new Guna2NumericUpDown
            {
                Minimum = 5,
                Maximum = 180,
                Value = 30,
                Font = new Font("Segoe UI", 10)
            };

            numPassingScore = new Guna2NumericUpDown
            {
                Minimum = 1,
                Maximum = 100,
                Value = 60,
                Font = new Font("Segoe UI", 10)
            };

            txtGeneratedQuiz = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                ReadOnly = true
            };

            btnGenerate = new Guna2Button
            {
                Text = "Generate Quiz",
                Size = new Size(120, 36),
                BorderRadius = 5,
                FillColor = Color.FromArgb(94, 148, 255),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            btnSubmit = new Guna2Button
            {
                Text = "Submit Quiz",
                Size = new Size(120, 36),
                BorderRadius = 5,
                FillColor = Color.FromArgb(39, 174, 96),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Enabled = false
            };

            btnCancel = new Guna2Button
            {
                Text = "Cancel",
                Size = new Size(120, 36),
                BorderRadius = 5,
                FillColor = Color.FromArgb(255, 74, 74),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            // Add controls to input panel
            inputPanel.Controls.Add(new Label { Text = "Topic:", Font = new Font("Segoe UI", 10) }, 0, 0);
            inputPanel.Controls.Add(txtTopic, 1, 0);
            inputPanel.Controls.Add(new Label { Text = "Description:", Font = new Font("Segoe UI", 10) }, 0, 1);
            inputPanel.Controls.Add(txtDescription, 1, 1);
            inputPanel.Controls.Add(new Label { Text = "Number of Questions:", Font = new Font("Segoe UI", 10) }, 0, 2);
            inputPanel.Controls.Add(numQuestions, 1, 2);
            inputPanel.Controls.Add(new Label { Text = "Time Limit (minutes):", Font = new Font("Segoe UI", 10) }, 0, 3);
            inputPanel.Controls.Add(numTimeLimit, 1, 3);
            inputPanel.Controls.Add(new Label { Text = "Passing Score (%):", Font = new Font("Segoe UI", 10) }, 0, 4);
            inputPanel.Controls.Add(numPassingScore, 1, 4);

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(10)
            };

            buttonPanel.Controls.AddRange(new Control[] { btnCancel, btnSubmit, btnGenerate });

            mainPanel.Controls.AddRange(new Control[] { inputPanel, txtGeneratedQuiz, buttonPanel });
            this.Controls.Add(mainPanel);

            // Wire up events
            btnGenerate.Click += BtnGenerate_Click;
            btnSubmit.Click += BtnSubmit_Click;
            btnCancel.Click += (s, e) => this.Close();
        }

        private async void BtnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                btnGenerate.Enabled = false;
                txtGeneratedQuiz.Text = "Generating quiz...";

                string prompt = $@"Generate a multiple-choice quiz about {txtTopic.Text} with {numQuestions.Value} questions.
                                 Format: 
                                 Q1. [Question]
                                 A) [Option]
                                 B) [Option]
                                 C) [Option]
                                 D) [Option]
                                 Correct Answer: [Letter]

                                 Make sure questions test different aspects and difficulty levels.
                                 Include an answer key at the end.";

                var quiz = await GenerateQuizAsync(prompt);
                txtGeneratedQuiz.Text = quiz;
                btnSubmit.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating quiz: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnGenerate.Enabled = true;
            }
        }

        private async Task<string> GenerateQuizAsync(string prompt)
        {
            using (var client = new HttpClient())
            {
                var request = new
                {
                    contents = new[]
                    {
                        new { parts = new[] { new { text = prompt } } }
                    }
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Add("x-goog-api-key", GEMINI_API_KEY);

                var response = await client.PostAsync($"{API_URL}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Parse the response and extract the generated text
                using (JsonDocument document = JsonDocument.Parse(responseContent))
                {
                    var root = document.RootElement;
                    var candidates = root.GetProperty("candidates")[0];
                    var content_ = candidates.GetProperty("content");
                    var parts = content_.GetProperty("parts")[0];
                    return parts.GetProperty("text").GetString();
                }
            }
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            SaveQuizToDatabase();
        }

        private void SaveQuizToDatabase()
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Insert quiz
                        string quizQuery = @"INSERT INTO Quizzes (Title, Description, CreatedBy, TimeLimit, PassingScore, IsActive, CreatedAt)
                                           VALUES (@Title, @Description, @CreatedBy, @TimeLimit, @PassingScore, 1, GETDATE());
                                           SELECT SCOPE_IDENTITY();";

                        int quizId;
                        using (var cmd = new SqlCommand(quizQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Title", txtTopic.Text);
                            cmd.Parameters.AddWithValue("@Description", txtDescription.Text);
                            cmd.Parameters.AddWithValue("@CreatedBy", teacherId);
                            cmd.Parameters.AddWithValue("@TimeLimit", (int)numTimeLimit.Value);
                            cmd.Parameters.AddWithValue("@PassingScore", (int)numPassingScore.Value);

                            quizId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // Parse and save questions and options
                        ParseAndSaveQuestions(txtGeneratedQuiz.Text, quizId, conn, transaction);

                        transaction.Commit();
                        MessageBox.Show("Quiz saved successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Error saving quiz: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ParseAndSaveQuestions(string quizText, int quizId, SqlConnection conn, SqlTransaction transaction)
        {
            string[] lines = quizText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string currentQuestion = null;
            List<string> currentOptions = new List<string>();
            string correctAnswer = null;
            int questionOrder = 1;

            foreach (string line in lines)
            {
                if (line.StartsWith("Q"))
                {
                    // Save previous question if exists
                    if (currentQuestion != null)
                    {
                        SaveQuestion(quizId, currentQuestion, currentOptions, correctAnswer, questionOrder, conn, transaction);
                        questionOrder++;
                    }

                    // Start new question
                    currentQuestion = line.Substring(line.IndexOf(".") + 1).Trim();
                    currentOptions.Clear();
                    correctAnswer = null;
                }
                else if (line.StartsWith("Correct Answer:"))
                {
                    correctAnswer = line.Substring(line.IndexOf(":") + 1).Trim();
                }
                else if (line.StartsWith("A)") || line.StartsWith("B)") || line.StartsWith("C)") || line.StartsWith("D)"))
                {
                    currentOptions.Add(line.Substring(2).Trim());
                }
            }

            // Save last question
            if (currentQuestion != null)
            {
                SaveQuestion(quizId, currentQuestion, currentOptions, correctAnswer, questionOrder, conn, transaction);
            }
        }

        private void SaveQuestion(int quizId, string questionText, List<string> options, string correctAnswer, int order,
            SqlConnection conn, SqlTransaction transaction)
        {
            // Insert question
            string questionQuery = @"INSERT INTO Questions (QuizID, QuestionText, QuestionType, Points, OrderNumber)
                                   VALUES (@QuizID, @QuestionText, 'MultipleChoice', 1, @OrderNumber);
                                   SELECT SCOPE_IDENTITY();";

            int questionId;
            using (var cmd = new SqlCommand(questionQuery, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@QuizID", quizId);
                cmd.Parameters.AddWithValue("@QuestionText", questionText);
                cmd.Parameters.AddWithValue("@OrderNumber", order);
                questionId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // Insert options
            string optionQuery = @"INSERT INTO Options (QuestionID, OptionText, IsCorrect, OrderNumber)
                                 VALUES (@QuestionID, @OptionText, @IsCorrect, @OrderNumber)";

            for (int i = 0; i < options.Count; i++)
            {
                using (var cmd = new SqlCommand(optionQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@QuestionID", questionId);
                    cmd.Parameters.AddWithValue("@OptionText", options[i]);
                    cmd.Parameters.AddWithValue("@IsCorrect", correctAnswer == ((char)('A' + i)).ToString());
                    cmd.Parameters.AddWithValue("@OrderNumber", i + 1);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
} 