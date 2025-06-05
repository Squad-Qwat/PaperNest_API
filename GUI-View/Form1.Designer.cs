using View.Global;
using View.Lecturer;
using View.Student;
namespace GUI_View
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        /// <summary>
        ///  Display menu based on the GlobalView.cs, using text, buttons, and labels.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            GlobalView globalView = new GlobalView();
            // globalView.DrawMenu(e.Graphics, this.ClientSize.Width, this.ClientSize.Height);
        }
    }
}