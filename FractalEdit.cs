using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fractals
{
    public partial class FractalEdit : Form
    {
        public FractalView fractalView;
        Data data;
        public FractalEdit()
        {
            InitializeComponent();            
            propertyGrid1.PropertyValueChanged += PropertyGrid1_PropertyValueChanged;
        }
        private void PropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            fractalView.colors = new OpenTK.Mathematics.Vector4[data.colors.Length];
            for (int i = 0; i < fractalView.colors.Length; i++)            
                fractalView.colors[i] = new OpenTK.Mathematics.Vector4(data.colors[i].R, data.colors[i].G, data.colors[i].B, data.colors[i].A);
            fractalView.UpdateColors();            
        }
        public void GetValues() 
        {
            trackBar1.Value = fractalView.MaxIter;
            label1.Text = "Max iterations: " + trackBar1.Value.ToString();
            trackBar2.Value = fractalView.colorNum;
            label2.Text = "Number of color stripes: " + trackBar2.Value.ToString();
            data = new Data(fractalView.colors);
            propertyGrid1.SelectedObject = data;
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

    class Data
    {
        public Data(  OpenTK.Mathematics.Vector4[] cs) 
        {
            colors = new Color[cs.Length];
            for (int i = 0; i < cs.Length; i++)            
                colors[i] = Color.FromArgb((int)cs[i].X, (int)cs[i].Y, (int)cs[i].Z);            
        }
        public Color[] colors { get; set; }
    }
}
