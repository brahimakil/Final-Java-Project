using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using FontAwesome.Sharp;

namespace Project_College_App
{
    public partial class QuizForm : Form
    {
        private int quizId;
        private int studentId;
        private Guna2Panel mainPanel;
        private Label lblTitle;
        private Label lblTimeRemaining;
        private FlowLayoutPanel questionsPanel;
        private Guna2Button btnSubmit;
        private Timer quizTimer;
        private int timeRemaining;

        public QuizForm(int quizId, int studentId)
        {
            this.quizId = quizId;
            this.studentId = studentId;
            InitializeComponents();
            LoadQuizQuestions();
            StartQuizTimer();
        }

        private void InitializeComponents()
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
        }

        private void LoadQuizQuestions()
        {
            // TODO: Load questions from database
        }

        private void StartQuizTimer()
        {
            // TODO: Initialize and start quiz timer
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            // TODO: Submit quiz answers
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
} 