using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using static System.Formats.Asn1.AsnWriter;

namespace Test_Engine_3
{
    public partial class Inspector : Form
    {
        Inspector self;
        public float offset = 0.0f;
        public Engine engine;
        public bool object_reload = false;
        public string model_selected;
        public string texture_selected;

        public float transform_move_x = 0;
        public float transform_move_y = 0;
        public float transform_move_z = 0;

        public float transform_rotate_x = 0;
        public float transform_rotate_y = 0;
        public float transform_rotate_z = 0;

        public float transform_scale_x = 1;
        public float transform_scale_y = 1;
        public float transform_scale_z = 1;

        double height = SystemInformation.VirtualScreen.Height;
        double width = SystemInformation.VirtualScreen.Width;

        public float[] vertices =
        {
            0.0f, 0.0f, 0.0f, 0.0f, 0.0f
        };
        public uint[] indices =
        {
            0, 1, 2
        };

        public Inspector()
        {
            InitializeComponent();
        }

        private void Inspector_Load(object sender, EventArgs e)
        {
            timer1.Start();

            List<string> keyModels = new List<string>(engine.scene.models.Keys);
            foreach (string key in keyModels)
            {
                comboBoxModel.Items.Add(key);
            }

            List<string> keyImage = new List<string>(engine.scene.textures.Keys);
            foreach (string key in keyImage)
            {
                comboBoxTexture.Items.Add(key);
            }

            this.Location = new Point((int)width - this.Width, (int)height / 2 - (int)(this.Height / 2));
            self = this;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            offset += 0.05f;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox1.Text = Math.Round(engine.camera.Position.X, 5).ToString();
            textBox2.Text = Math.Round(engine.camera.Position.Y, 5).ToString();
            textBox3.Text = Math.Round(engine.camera.Position.Z, 5).ToString();

            textBox6.Text = Math.Round(engine.camera.Pitch, 5).ToString();
            textBox5.Text = Math.Round(engine.camera.Yaw, 5).ToString();

            textBox9.Text = Math.Round(engine.camera.Fov, 5).ToString();

            if (comboBoxModel.SelectedItem != null)
            {
                textboxDebug.Text = comboBoxModel.SelectedItem.ToString() + Environment.NewLine + engine.scene.models[comboBoxModel.SelectedItem.ToString()].vertices.ToString();
            }

            if (engine.initialised == true)
            {
                object_reload = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileObject.ShowDialog();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            openFileTexture.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            label10.Text = openFileObject.FileName;
        }

        private void openFileTexture_FileOk(object sender, CancelEventArgs e)
        {
            label1.Text = openFileTexture.FileName;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void listObjectsLoaded_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

        private void comboBoxModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void buttonLoadObject_Click(object sender, EventArgs e)
        {
            if (comboBoxModel.SelectedItem == null && comboBoxTexture.SelectedItem == null)
                MessageBox.Show("Не выбраны модель и текстура объекта");
            else
            if (comboBoxModel.SelectedItem == null)
                MessageBox.Show("Не выбрана модель объекта");
            else
            if (comboBoxTexture.SelectedItem == null)
                MessageBox.Show("Не выбрана текстура объекта");
            else
            {
                object_reload = true;
                model_selected = comboBoxModel.SelectedItem.ToString();
                texture_selected = comboBoxTexture.SelectedItem.ToString();
            }
        }

        private void buttonTransform_Click(object sender, EventArgs e)
        {
            if (textBox24.Text != "" && textBox23.Text != "" && textBox22.Text != "" && textBox21.Text != "" && textBox20.Text != "" && textBox19.Text != "" && textBox18.Text != "" && textBox17.Text != "" && textBox4.Text != "")
            {
                transform_move_x = float.Parse(textBox24.Text, CultureInfo.InvariantCulture.NumberFormat);
                transform_move_y = float.Parse(textBox23.Text, CultureInfo.InvariantCulture.NumberFormat);
                transform_move_z = float.Parse(textBox22.Text, CultureInfo.InvariantCulture.NumberFormat);

                transform_rotate_x = float.Parse(textBox21.Text, CultureInfo.InvariantCulture.NumberFormat);
                transform_rotate_y = float.Parse(textBox20.Text, CultureInfo.InvariantCulture.NumberFormat);
                transform_rotate_z = float.Parse(textBox19.Text, CultureInfo.InvariantCulture.NumberFormat);

                transform_scale_x = float.Parse(textBox18.Text, CultureInfo.InvariantCulture.NumberFormat);
                transform_scale_y = float.Parse(textBox17.Text, CultureInfo.InvariantCulture.NumberFormat);
                transform_scale_z = float.Parse(textBox4.Text, CultureInfo.InvariantCulture.NumberFormat);
            }
            else
                MessageBox.Show("Один или несколько параметров пусты");
        }
    }
}
