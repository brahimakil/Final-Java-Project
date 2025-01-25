using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Project_College_App
{
    public partial class QuizResultForm : Form
    {
        private int quizId;
        private int studentId;
        private Guna2Panel mainPanel;
        private Label lblTitle;
        private Label lblScore;
        private Label lblPassing;
        private Label lblStatus;
        private Guna2DataGridView dgvAnswers;
        private Guna2Button btnClose;

        public QuizResultForm(int quizId, int studentId)
        {
            this.quizId = quizId;
            this.studentId = studentId;
            InitializeComponents();
            LoadQuizResults();
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
                Text = "Quiz Results",
                Font = new Font("Segoe UI Semibold", 16),
                Dock = DockStyle.Top,
                Height = 40
            };

            var resultsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 100,
                FlowDirection = FlowDirection.TopDown
            };

            lblScore = new Label { Font = new Font("Segoe UI", 12) };
            lblPassing = new Label { Font = new Font("Segoe UI", 12) };
            lblStatus = new Label { Font = new Font("Segoe UI Semibold", 12) };

            resultsPanel.Controls.AddRange(new Control[] { lblScore, lblPassing, lblStatus });

            dgvAnswers = new Guna2DataGridView
            {
                Dock = DockStyle.Fill,
                DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Segoe UI", 10) },
                ColumnHeadersHeight = 50,
                RowTemplate = { Height = 45 },
                BackgroundColor = Color.White,
                CellBorderStyle = DataGridViewCellBorderStyle.Single,
                GridColor = Color.FromArgb(231, 229, 255),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };

            btnClose = new Guna2Button
            {
                Text = "Close",
                Size = new Size(120, 36),
                BorderRadius = 5,
                FillColor = Color.FromArgb(94, 148, 255),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Dock = DockStyle.Bottom
            };

            mainPanel.Controls.AddRange(new Control[] { lblTitle, resultsPanel, dgvAnswers, btnClose });
            this.Controls.Add(mainPanel);

            btnClose.Click += (s, e) => this.Close();
        }

        private void LoadQuizResults()
        {
            // TODO: Load quiz results from database
        }
    }
}  