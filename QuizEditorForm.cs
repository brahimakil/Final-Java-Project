using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Project_College_App
{
    public class QuizEditorForm : Form
    {
        private readonly string GEMINI_API_KEY = "AIzaSyBswCA-tmUntZ_Yj8SHCjyQ5UyuJ3HPBoI";
        private readonly string API_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent";

        private int? quizId;
        private int classId;
        private int teacherId;
        private string className;
        private Guna2TextBox txtTitle;
        private Guna2TextBox txtDescription;
        private Guna2NumericUpDown numTimeLimit;
        private RichTextBox txtGeneratedQuiz;
        private Guna2Button btnGenerate;
        private Guna2Button btnSave;
        private Guna2Button btnCancel;
        private Guna2CheckBox chkActive;

        public QuizEditorForm(int classId, string className, int teacherId, int? quizId = null)
        {
            this.quizId = quizId;
            this.classId = classId;
            this.className = className;
            this.teacherId = teacherId;
            InitializeComponents();

            if (quizId.HasValue)
            {
                LoadExistingQuiz(quizId.Value);
            }
        }

        private void InitializeComponents()
        {
            this.Size = new Size(800, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = quizId.HasValue ? "Edit Quiz" : "Add Quiz";

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                Padding = new Padding(20)
            };

            // Title
            txtTitle = new Guna2TextBox
            {
                PlaceholderText = "Enter quiz title",
                Size = new Size(400, 36),
                BorderRadius = 5
            };

            // Description
            txtDescription = new Guna2TextBox
            {
                PlaceholderText = "Enter quiz description",
                Multiline = true,
                Size = new Size(400, 60),
                BorderRadius = 5
            };

            // Time Limit
            numTimeLimit = new Guna2NumericUpDown
            {
                Minimum = 1,
                Maximum = 180,
                Value = 30,
                Size = new Size(120, 36),
                BorderRadius = 5
            };

            // Generated Quiz Content
            txtGeneratedQuiz = new RichTextBox
            {
                Size = new Size(700, 300),
                ReadOnly = true,
                Font = new Font("Consolas", 10)
            };

            // Buttons
            btnGenerate = new Guna2Button
            {
                Text = "Generate Quiz",
                Size = new Size(120, 36),
                BorderRadius = 5,
                FillColor = Color.FromArgb(94, 148, 255),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            btnSave = new Guna2Button
            {
                Text = "Save",
                Size = new Size(120, 36),
                BorderRadius = 5,
                FillColor = Color.FromArgb(94, 148, 255),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0, 0, 10, 0)
            };

            btnCancel = new Guna2Button
            {
                Text = "Cancel",
                Size = new Size(120, 36),
                BorderRadius = 5,
                FillColor = Color.FromArgb(255, 74, 74),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            // Active Checkbox
            chkActive = new Guna2CheckBox
            {
                Text = "Active",
                Size = new Size(120, 20),
                Checked = true
            };

            // Add controls to panel
            mainPanel.Controls.Add(new Label { Text = "Title:", Anchor = AnchorStyles.Left }, 0, 0);
            mainPanel.Controls.Add(txtTitle, 1, 0);
            mainPanel.Controls.Add(new Label { Text = "Description:", Anchor = AnchorStyles.Left }, 0, 1);
            mainPanel.Controls.Add(txtDescription, 1, 1);
            mainPanel.Controls.Add(new Label { Text = "Time Limit (mins):", Anchor = AnchorStyles.Left }, 0, 2);
            mainPanel.Controls.Add(numTimeLimit, 1, 2);
            mainPanel.Controls.Add(txtGeneratedQuiz, 1, 3);
            mainPanel.Controls.Add(chkActive, 1, 4);
            
            var buttonPanel = new FlowLayoutPanel 
            { 
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(0, 10, 0, 0),
                AutoSize = true
            };

            // Add buttons with proper spacing
            btnCancel.Margin = new Padding(10, 0, 0, 0);
            btnSave.Margin = new Padding(10, 0, 0, 0);
            btnGenerate.Margin = new Padding(10, 0, 0, 0);

            buttonPanel.Controls.AddRange(new Control[] { btnCancel, btnSave, btnGenerate });
            mainPanel.Controls.Add(buttonPanel, 1, 5);

            this.Controls.Add(mainPanel);

            // Events
            btnGenerate.Click += BtnGenerate_Click;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => this.Close();
        }

        private async void BtnGenerate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Please enter both title and description.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnGenerate.Enabled = false;
            txtGeneratedQuiz.Text = "Generating quiz...";

            try
            {
                string prompt = $@"Generate a multiple choice quiz about {txtTitle.Text}. Topic: {txtDescription.Text}.
                          Create exactly 5 questions.
                          Format the output exactly like this:
                          QUESTIONS:
                          Q1. [Question]
                          A) [Option]
                          B) [Option]
                          C) [Option]
                          D) [Option]

                          Q2. [Question]
                          A) [Option]
                          B) [Option]
                          C) [Option]
                          D) [Option]

                          [Continue for all 5 questions]

                          ANSWER KEY:
                          1. [A/B/C/D]
                          2. [A/B/C/D]
                          3. [A/B/C/D]
                          4. [A/B/C/D]
                          5. [A/B/C/D]";

                string generatedContent = await GenerateQuizContent(prompt);
                txtGeneratedQuiz.Text = generatedContent;
                btnSave.Enabled = true;
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

        private void LoadExistingQuiz(int quizId)
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Load quiz details
                string quizQuery = @"SELECT Title, Description, TimeLimit, IsActive FROM Quizzes WHERE QuizID = @QuizID";
                using (var cmd = new SqlCommand(quizQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizID", quizId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtTitle.Text = reader["Title"].ToString();
                            txtDescription.Text = reader["Description"].ToString();
                            numTimeLimit.Value = Convert.ToInt32(reader["TimeLimit"]);
                            chkActive.Checked = Convert.ToBoolean(reader["IsActive"]);
                        }
                    }
                }

                // Load questions and format them
                StringBuilder quizContent = new StringBuilder();
                quizContent.AppendLine("QUESTIONS:");

                string questionQuery = @"SELECT QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectAnswer, QuestionOrder 
                                       FROM Questions WHERE QuizID = @QuizID ORDER BY QuestionOrder";
                using (var cmd = new SqlCommand(questionQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizID", quizId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        Dictionary<int, string> answers = new Dictionary<int, string>();
                        
                        while (reader.Read())
                        {
                            int order = Convert.ToInt32(reader["QuestionOrder"]);
                            string questionText = reader["QuestionText"].ToString();
                            string optionA = reader["OptionA"].ToString();
                            string optionB = reader["OptionB"].ToString();
                            string optionC = reader["OptionC"].ToString();
                            string optionD = reader["OptionD"].ToString();
                            string correctAnswer = reader["CorrectAnswer"].ToString();

                            // Format question and options
                            quizContent.AppendLine($"Q{order}. {questionText}");
                            quizContent.AppendLine($"A) {optionA}");
                            quizContent.AppendLine($"B) {optionB}");
                            quizContent.AppendLine($"C) {optionC}");
                            quizContent.AppendLine($"D) {optionD}");
                            quizContent.AppendLine();
                            
                            answers[order] = correctAnswer;
                        }

                        if (answers.Count > 0)
                        {
                            quizContent.AppendLine("ANSWER KEY:");
                            foreach (var answer in answers.OrderBy(x => x.Key))
                            {
                                quizContent.AppendLine($"{answer.Key}. {answer.Value}");
                            }
                        }
                    }
                }

                txtGeneratedQuiz.Text = quizContent.ToString().Trim();
                btnSave.Enabled = true;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter a quiz title.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtGeneratedQuiz.Text))
            {
                MessageBox.Show("Please generate quiz questions first.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveQuiz();
        }

        private void SaveQuiz()
        {
            string connectionString = @"Server=desktop-hm9h3t3\sqlexpress;Database=QuizMakerDB;Integrated Security=True;TrustServerCertificate=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int currentQuizId;
                        if (quizId.HasValue)  // Update existing quiz
                        {
                            // Update quiz details
                            string updateQuery = @"UPDATE Quizzes 
                                                 SET Title = @Title, 
                                                     Description = @Description,
                                                     TimeLimit = @TimeLimit,
                                                     PassingScore = @PassingScore,
                                                     IsActive = @IsActive
                                                 WHERE QuizID = @QuizID";

                            using (var cmd = new SqlCommand(updateQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@QuizID", quizId.Value);
                                cmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim());
                                cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                                cmd.Parameters.AddWithValue("@TimeLimit", numTimeLimit.Value);
                                cmd.Parameters.AddWithValue("@PassingScore", 60);
                                cmd.Parameters.AddWithValue("@IsActive", chkActive.Checked);
                                cmd.ExecuteNonQuery();
                            }
                            currentQuizId = quizId.Value;

                            // Delete existing questions
                            string deleteQuery = "DELETE FROM Questions WHERE QuizID = @QuizID";
                            using (var cmd = new SqlCommand(deleteQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@QuizID", currentQuizId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else  // Insert new quiz
                        {
                            string insertQuery = @"INSERT INTO Quizzes (ClassID, Title, Description, TimeLimit, PassingScore, IsActive, CreatedAt, CreatedBy)
                                                 VALUES (@ClassID, @Title, @Description, @TimeLimit, @PassingScore, @IsActive, GETDATE(), @CreatedBy);
                                                 SELECT SCOPE_IDENTITY();";

                            using (var cmd = new SqlCommand(insertQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ClassID", classId);
                                cmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim());
                                cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                                cmd.Parameters.AddWithValue("@TimeLimit", numTimeLimit.Value);
                                cmd.Parameters.AddWithValue("@PassingScore", 60);
                                cmd.Parameters.AddWithValue("@IsActive", chkActive.Checked);
                                cmd.Parameters.AddWithValue("@CreatedBy", teacherId);
                                currentQuizId = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                        }

                        // Save questions
                        if (!string.IsNullOrWhiteSpace(txtGeneratedQuiz.Text))
                        {
                            ParseAndSaveQuestions(txtGeneratedQuiz.Text, currentQuizId, conn, transaction);
                        }

                        transaction.Commit();
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
            var parts = quizText.Split(new[] { "ANSWER KEY:" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                throw new Exception("Invalid quiz format");
            }

            string questionsText = parts[0].Replace("QUESTIONS:", "").Trim();
            string answersText = parts[1].Trim();

            // Parse answers first
            var answerMap = new Dictionary<int, string>();
            var answerLines = answersText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in answerLines)
            {
                var parts2 = line.Split('.');
                if (parts2.Length == 2)
                {
                    int qNum = int.Parse(parts2[0].Trim());
                    string answer = parts2[1].Trim();
                    answerMap[qNum] = answer;
                }
            }

            // Parse questions
            var currentQuestion = "";
            var options = new List<string>();
            var questionNumber = 0;

            var lines = questionsText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.StartsWith("Q"))
                {
                    if (currentQuestion != "" && options.Count == 4 && answerMap.ContainsKey(questionNumber))
                    {
                        SaveQuestion(quizId, currentQuestion, 
                                   options[0], options[1], options[2], options[3],
                                   answerMap[questionNumber], questionNumber, 
                                   conn, transaction);
                    }
                    questionNumber++;
                    currentQuestion = line.Substring(line.IndexOf('.') + 1).Trim();
                    options.Clear();
                }
                else if (line.Trim().Length > 2 && "ABCD".Contains(line[0].ToString()))
                {
                    options.Add(line.Substring(line.IndexOf(')') + 1).Trim());
                }
            }

            // Save the last question
            if (currentQuestion != "" && options.Count == 4 && answerMap.ContainsKey(questionNumber))
            {
                SaveQuestion(quizId, currentQuestion, 
                           options[0], options[1], options[2], options[3],
                           answerMap[questionNumber], questionNumber, 
                           conn, transaction);
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

        private async Task<string> GenerateQuizContent(string prompt)
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
    }
}