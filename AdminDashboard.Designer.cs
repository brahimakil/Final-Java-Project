namespace Project_College_App
{
    partial class AdminDashboard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                // Dispose of additional resources
                if (shadowForm != null) shadowForm.Dispose();
                if (formDragControl != null) formDragControl.Dispose();
                if (mainPanel != null) mainPanel.Dispose();
                if (sidePanel != null) sidePanel.Dispose();
                if (btnTeachers != null) btnTeachers.Dispose();
                if (btnStudents != null) btnStudents.Dispose();
                if (btnLogout != null) btnLogout.Dispose();
                if (profilePicture != null) profilePicture.Dispose();
                if (btnClasses != null) btnClasses.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AdminDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "AdminDashboard";
            this.Load += new System.EventHandler(this.AdminDashboard_Load);
            this.ResumeLayout(false);

        }

        #endregion
    }
} 