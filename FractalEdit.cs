using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Fractals
{
    public partial class FractalEdit : Form
    {
        public FractalView fractalView;

        public FractalEdit()
        {
            InitializeComponent();           
            
        }
        public void GetValues() 
        {
            trackBar1.Value = fractalView.MaxIter;
            label1.Text = "Max iterations: " + trackBar1.Value.ToString();
            trackBar2.Value = fractalView.colorNum;
            label2.Text = "Number of color stripes: " + trackBar2.Value.ToString();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            fractalView.MaxIter = trackBar1.Value;
            label1.Text = "Max iterations: " + trackBar1.Value.ToString();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            fractalView.colorNum = trackBar2.Value;
            label2.Text = "Number of color stripes: " + trackBar2.Value.ToString();
        }

    }
}
