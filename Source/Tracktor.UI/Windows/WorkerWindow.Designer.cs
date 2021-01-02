﻿
namespace Tracktor.UI.Windows
{
    partial class WorkerWindow
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.MainBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.MainProgressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // MainBackgroundWorker
            // 
            this.MainBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.MainBackgroundWorkerDoWork);
            this.MainBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.MainBackgroundWorkerRunWorkerCompleted);
            // 
            // MainProgressBar
            // 
            this.MainProgressBar.Location = new System.Drawing.Point(12, 25);
            this.MainProgressBar.Name = "MainProgressBar";
            this.MainProgressBar.Size = new System.Drawing.Size(760, 50);
            this.MainProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.MainProgressBar.TabIndex = 0;
            // 
            // WorkerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 112);
            this.Controls.Add(this.MainProgressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WorkerWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tracktor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WorkerWindowFormClosing);
            this.Load += new System.EventHandler(this.WorkerWindowLoad);
            this.Shown += new System.EventHandler(this.WorkerWindowShown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker MainBackgroundWorker;
        private System.Windows.Forms.ProgressBar MainProgressBar;
    }
}