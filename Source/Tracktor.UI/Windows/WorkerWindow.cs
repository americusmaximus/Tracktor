﻿#region License
/*
MIT License

Copyright (c) 2020, 2021 Americus Maximus

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion

using Tracktor.UI.Properties;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using Tracktor.UI.Controls;

namespace Tracktor.UI.Windows
{
    public partial class WorkerWindow : Form
    {
        public WorkerWindow()
        {
            InitializeComponent();
        }

        public Func<ExecutionResult> Action { get; set; }

        public Exception Exception { get; protected set; }

        public ExecutionResult Result { get; protected set; }

        protected virtual void MainBackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Result = Action?.Invoke();
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Exception = ex;
                DialogResult = DialogResult.Cancel;
            }
        }

        protected virtual void MainBackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Exception != default)
            {
                MessageBox.Show(Parent, Exception.ToString(), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Close();
        }

        protected virtual void WorkerWindowFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !MainBackgroundWorker.IsBusy;
        }

        protected virtual void WorkerWindowLoad(object sender, EventArgs e)
        {
            Icon = Resources.Icon;
        }

        protected virtual void WorkerWindowShown(object sender, EventArgs e)
        {
            if (Action != default)
            {
                MainBackgroundWorker.RunWorkerAsync(Action);
            }
        }
    }
}
