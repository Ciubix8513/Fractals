namespace Fractals
{
    partial class FractalView
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FractalView));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mandelbrotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.burningShipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tricornToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.featherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.glControl = new OpenTK.WinForms.GLControl();
            this.eyeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reloadToolStripMenuItem,
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(919, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mandelbrotToolStripMenuItem,
            this.burningShipToolStripMenuItem,
            this.tricornToolStripMenuItem,
            this.featherToolStripMenuItem,
            this.eyeToolStripMenuItem});
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.reloadToolStripMenuItem.Text = "Fracrtals";
            // 
            // mandelbrotToolStripMenuItem
            // 
            this.mandelbrotToolStripMenuItem.Name = "mandelbrotToolStripMenuItem";
            this.mandelbrotToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.mandelbrotToolStripMenuItem.Text = "Mandelbrot";
            this.mandelbrotToolStripMenuItem.Click += new System.EventHandler(this.mandelbrotToolStripMenuItem_Click);
            // 
            // burningShipToolStripMenuItem
            // 
            this.burningShipToolStripMenuItem.Name = "burningShipToolStripMenuItem";
            this.burningShipToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.burningShipToolStripMenuItem.Text = "Burning ship";
            this.burningShipToolStripMenuItem.Click += new System.EventHandler(this.burningShipToolStripMenuItem_Click);
            // 
            // tricornToolStripMenuItem
            // 
            this.tricornToolStripMenuItem.Name = "tricornToolStripMenuItem";
            this.tricornToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.tricornToolStripMenuItem.Text = "Tricorn";
            this.tricornToolStripMenuItem.Click += new System.EventHandler(this.tricornToolStripMenuItem_Click);
            // 
            // featherToolStripMenuItem
            // 
            this.featherToolStripMenuItem.Name = "featherToolStripMenuItem";
            this.featherToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.featherToolStripMenuItem.Text = "Feather";
            this.featherToolStripMenuItem.Click += new System.EventHandler(this.featherToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(55, 20);
            this.toolStripMenuItem1.Text = "Reload";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(35, 20);
            this.toolStripMenuItem2.Text = "<<";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(28, 20);
            this.toolStripMenuItem3.Text = "| |";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(77, 20);
            this.toolStripMenuItem4.Text = "Screenshot";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // glControl
            // 
            this.glControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.glControl.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            this.glControl.APIVersion = new System.Version(3, 3, 0, 0);
            this.glControl.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            this.glControl.IsEventDriven = true;
            this.glControl.Location = new System.Drawing.Point(0, 27);
            this.glControl.Name = "glControl";
            this.glControl.Profile = OpenTK.Windowing.Common.ContextProfile.Compatability;
            this.glControl.Size = new System.Drawing.Size(919, 553);
            this.glControl.TabIndex = 2;
            this.glControl.Text = "glControl1";
            this.glControl.Load += new System.EventHandler(this.glControl_Load);
            // 
            // eyeToolStripMenuItem
            // 
            this.eyeToolStripMenuItem.Name = "eyeToolStripMenuItem";
            this.eyeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.eyeToolStripMenuItem.Text = "Eye";
            this.eyeToolStripMenuItem.Click += new System.EventHandler(this.eyeToolStripMenuItem_Click);
            // 
            // FractalView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(919, 579);
            this.Controls.Add(this.glControl);
            this.Controls.Add(this.menuStrip1);
            this.ForeColor = System.Drawing.SystemColors.Control;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FractalView";
            this.Text = "Fractals";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private OpenTK.WinForms.GLControl glControl;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem mandelbrotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem burningShipToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tricornToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem featherToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eyeToolStripMenuItem;
    }
}

