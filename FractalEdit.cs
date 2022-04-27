using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fractals
{
    public partial class FractalEdit : Form
    {
        public FractalView fractalView;
        private Data data;

        public FractalEdit()
        {
            InitializeComponent();
            propertyGrid1.PropertyValueChanged += PropertyGrid1_PropertyValueChanged;
        }

        private void PropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            //Update the colors array when data is changed
            fractalView.colors = new OpenTK.Vector4[data.colors.Length];
            for (int i = 0; i < fractalView.colors.Length; i++)
                fractalView.colors[i] = new OpenTK.Vector4(data.colors[i].R, data.colors[i].G, data.colors[i].B, data.colors[i].A);
            fractalView.UpdateColors();
        }

        //Get data from the main form
        public void GetValues()
        {
            trackBar1.Value = fractalView.maxIter;
            label1.Text = "Max iterations: " + trackBar1.Value.ToString();
            trackBar2.Value = fractalView.colorNum;
            label2.Text = "Number of color stripes: " + trackBar2.Value.ToString();
            trackBar3.Value = fractalView.antiAliasing;
            label3.Text = "MSAA: " + trackBar3.Value.ToString();

            data = new Data(fractalView.colors);
            propertyGrid1.SelectedObject = data;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            fractalView.maxIter = trackBar1.Value;
            label1.Text = "Max iterations: " + trackBar1.Value.ToString();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            fractalView.colorNum = trackBar2.Value;
            label2.Text = "Number of color stripes: " + trackBar2.Value.ToString();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            fractalView.antiAliasing = trackBar3.Value;
            label3.Text = "MSAA: " + trackBar3.Value.ToString();
        }
    }

    //Class for property grid to work
    internal class Data
    {
        public Data(OpenTK.Vector4[] cs)
        {
            colors = new Color[cs.Length];
            for (int i = 0; i < cs.Length; i++)
                colors[i] = Color.FromArgb((int)cs[i].X, (int)cs[i].Y, (int)cs[i].Z);
        }

        public Color[] colors { get; set; }
    }
}