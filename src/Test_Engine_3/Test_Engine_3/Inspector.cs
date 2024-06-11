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
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Test_Engine_3
{
    public partial class Inspector : Form
    {
        Inspector self;
        public float offset = 0.0f;
        public Engine engine;
        public bool object_reload = false;
        public string object_selected;
        //public string model_selected;
        //public string texture_selected;

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

        int progress = 0;

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

            List<string> keyObjects = new List<string>(engine.scene.objects.Keys);
            foreach (string key in keyObjects)
            {
                comboBoxObject.Items.Add(key);
            }

            foreach (string key in engine.scene.models)
            {
                comboBoxModel.Items.Add(key);
                listModelsLoaded.Items.Add(key);
            }

            foreach (string key in engine.scene.textures)
            {
                comboBoxTexture.Items.Add(key);
                listTexturesLoaded.Items.Add(key);
            }

            textBox_light_ambient.Text = engine.light_ambient.ToString();
            textBox_light_diffuse.Text = engine.light_diffuse.ToString();
            textBox_light_specular.Text = engine.light_specular.ToString();

            textBox_light_x.Text = engine._lightPos.X.ToString();
            textBox_light_y.Text = engine._lightPos.Y.ToString();
            textBox_light_z.Text = engine._lightPos.Z.ToString();

            this.Location = new Point((int)width - this.Width, 23);
            this.Size = new Size(this.Size.Width, engine.Size.Y);
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

            if (engine.model.progress_max != 0)
                progressBar.Value = (int)(engine.model.progress * 1000 / engine.model.progress_max);
            else
                progressBar.Value = 0;

            if (engine.model.progress_max != 0)
                labelProgress.Text = (engine.model.progress * 100 / engine.model.progress_max).ToString() + "%";
            else
                labelProgress.Text = "0%";

            progressBar.Update();

            if (comboBoxModel.SelectedItem != null)
            {
                //textboxDebug.Text = comboBoxModel.SelectedItem.ToString() + Environment.NewLine + engine.scene.models[comboBoxModel.SelectedItem.ToString()].vertices.ToString();
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


        private void openFileModel_Ok(object sender, CancelEventArgs e)
        {
            labelModelLoaded.Text = openFileObject.FileName;
        }

        private void openFileTexture_Ok(object sender, CancelEventArgs e)
        {
            labelTextureLoaded.Text = openFileTexture.FileName;
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

        private void buttonObjectLoad_Click(object sender, EventArgs e)
        {
            progressBar.Value = 0;

            if (comboBoxObject.SelectedItem == null)
                MessageBox.Show("Не выбран объект");
            //else
            //if (comboBoxModel.SelectedItem == null)
            //    MessageBox.Show("Не выбрана модель объекта");
            //else
            //if (comboBoxTexture.SelectedItem == null)
            //    MessageBox.Show("Не выбрана текстура объекта");
            else
            {
                object_reload = true;
                object_selected = comboBoxObject.SelectedItem.ToString();
                //model_selected = comboBoxModel.SelectedItem.ToString();
                //texture_selected = comboBoxTexture.SelectedItem.ToString();
            }
        }

        private void buttonTransform_Click(object sender, EventArgs e)
        {
            if (textBox24.Text != "" && textBox23.Text != "" && textBox22.Text != "" && textBox21.Text != "" && textBox20.Text != "" && textBox19.Text != "" && textBox18.Text != "" && textBox17.Text != "" && textBox4.Text != "")
            {
                try
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
                catch
                {
                    MessageBox.Show("Ошибка во входных данных трансформации!");
                }
            }
            else
                MessageBox.Show("Один или несколько параметров пусты");
        }

        private void button_lightpos_Click(object sender, EventArgs e)
        {
            try
            {
                engine._lightPos.X = float.Parse(textBox_light_x.Text);
                engine._lightPos.Y = float.Parse(textBox_light_y.Text);
                engine._lightPos.Z = float.Parse(textBox_light_z.Text);
            }
            catch
            {

            }
        }

        private void button_lighttype_Click(object sender, EventArgs e)
        {
            try
            {
                engine.light_ambient = float.Parse(textBox_light_ambient.Text);
                engine.light_diffuse = float.Parse(textBox_light_diffuse.Text);
                engine.light_specular = float.Parse(textBox_light_specular.Text);
            }
            catch
            {

            }
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            engine.color_reflection = (float)trackBar_colorReflection.Value / 10;
            textBox_colorReflection.Text = ((float)trackBar_colorReflection.Value / 10).ToString();
        }

        private void trackBar_colorRed_Scroll(object sender, EventArgs e)
        {
            // Textbox RGB
            textBox_colorRed.Text = trackBar_colorRed.Value.ToString();

            // Textbox HEX
            if (trackBar_colorRed.Value < 16)
                textBox_colorHex.Text = "0";
            else
                textBox_colorHex.Text = "";
            textBox_colorHex.Text += Convert.ToString(trackBar_colorRed.Value, 16).ToUpper();

            if (trackBar_colorGreen.Value < 16)
                textBox_colorHex.Text += "0";
            textBox_colorHex.Text += Convert.ToString(trackBar_colorGreen.Value, 16).ToUpper();

            if (trackBar_colorBlue.Value < 16)
                textBox_colorHex.Text += "0";
            textBox_colorHex.Text += Convert.ToString(trackBar_colorBlue.Value, 16).ToUpper();

            // Color preview
            label_colorPreview.BackColor = Color.FromArgb(trackBar_colorRed.Value, trackBar_colorGreen.Value, trackBar_colorBlue.Value);

            engine.color_rgb_red = float.Parse(textBox_colorRed.Text) / 255;
        }

        private void trackBar_colorGreen_Scroll(object sender, EventArgs e)
        {
            // Textbox RGB
            textBox_colorGreen.Text = trackBar_colorGreen.Value.ToString();

            //Textbox HEX
            if (trackBar_colorRed.Value < 16)
                textBox_colorHex.Text = "0";
            else
                textBox_colorHex.Text = "";
            textBox_colorHex.Text += Convert.ToString(trackBar_colorRed.Value, 16).ToUpper();

            if (trackBar_colorGreen.Value < 16)
                textBox_colorHex.Text += "0";
            textBox_colorHex.Text += Convert.ToString(trackBar_colorGreen.Value, 16).ToUpper();

            if (trackBar_colorBlue.Value < 16)
                textBox_colorHex.Text += "0";
            textBox_colorHex.Text += Convert.ToString(trackBar_colorBlue.Value, 16).ToUpper();

            // Color preview
            label_colorPreview.BackColor = Color.FromArgb(trackBar_colorRed.Value, trackBar_colorGreen.Value, trackBar_colorBlue.Value);

            engine.color_rgb_green = float.Parse(textBox_colorGreen.Text) / 255;
        }

        private void trackBar_colorBlue_Scroll(object sender, EventArgs e)
        {
            // Textbox RGB
            textBox_colorBlue.Text = trackBar_colorBlue.Value.ToString();

            //Textbox HEX
            if (trackBar_colorRed.Value < 16)
                textBox_colorHex.Text = "0";
            else
                textBox_colorHex.Text = "";
            textBox_colorHex.Text += Convert.ToString(trackBar_colorRed.Value, 16).ToUpper();

            if (trackBar_colorGreen.Value < 16)
                textBox_colorHex.Text += "0";
            textBox_colorHex.Text += Convert.ToString(trackBar_colorGreen.Value, 16).ToUpper();

            if (trackBar_colorBlue.Value < 16)
                textBox_colorHex.Text += "0";
            textBox_colorHex.Text += Convert.ToString(trackBar_colorBlue.Value, 16).ToUpper();

            // Color preview
            label_colorPreview.BackColor = Color.FromArgb(trackBar_colorRed.Value, trackBar_colorGreen.Value, trackBar_colorBlue.Value);

            engine.color_rgb_blue = float.Parse(textBox_colorBlue.Text) / 255;
        }

        private void buttonHexColor_Click(object sender, EventArgs e)
        {
            if (textBox_colorHex.Text != "")
            {
                try
                {
                    int red = Convert.ToInt16((textBox_colorHex.Text[0].ToString() + textBox_colorHex.Text[1].ToString()).ToString(), 16);
                    int green = Convert.ToInt16((textBox_colorHex.Text[2].ToString() + textBox_colorHex.Text[3].ToString()).ToString(), 16);
                    int blue = Convert.ToInt16((textBox_colorHex.Text[4].ToString() + textBox_colorHex.Text[5].ToString()).ToString(), 16);

                    if (red > 255) red = 255;
                    if (red < 0) red = 0;

                    if (green > 255) green = 255;
                    if (green < 0) green = 0;

                    if (blue > 255) blue = 255;
                    if (blue < 0) blue = 0;

                    trackBar_colorRed.Value = red;
                    trackBar_colorGreen.Value = green;
                    trackBar_colorBlue.Value = blue;

                    textBox_colorRed.Text = red.ToString();
                    textBox_colorGreen.Text = green.ToString();
                    textBox_colorBlue.Text = blue.ToString();

                    label_colorPreview.BackColor = Color.FromArgb(trackBar_colorRed.Value, trackBar_colorGreen.Value, trackBar_colorBlue.Value);

                    engine.color_rgb_red = float.Parse(textBox_colorRed.Text) / 255;
                    engine.color_rgb_green = float.Parse(textBox_colorGreen.Text) / 255;
                    engine.color_rgb_blue = float.Parse(textBox_colorBlue.Text) / 255;
                }
                catch
                {
                    trackBar_colorRed.Value = 255;
                    trackBar_colorBlue.Value = 255;
                    trackBar_colorGreen.Value = 255;

                    label_colorPreview.BackColor = Color.FromArgb(trackBar_colorRed.Value, trackBar_colorGreen.Value, trackBar_colorBlue.Value);

                    engine.color_rgb_red = float.Parse(textBox_colorRed.Text) / 255;
                    engine.color_rgb_green = float.Parse(textBox_colorGreen.Text) / 255;
                    engine.color_rgb_blue = float.Parse(textBox_colorBlue.Text) / 255;

                    textBox_colorHex.Text = "FFFFFF";
                }
            }
        }

        private void buttonClear(object sender, EventArgs e)
        {
            engine.ReloadScene();
            comboBoxObject.SelectedItem = null;
        }

        private void buttonObjectDelete_Click(object sender, EventArgs e)
        {
            if (comboBoxObject.SelectedItem != null)
            {
                engine.scene.DeleteObject(comboBoxObject.SelectedItem.ToString());
                comboBoxObject.Items.Remove(comboBoxObject.SelectedItem);
                comboBoxObject.SelectedItem = null;
                engine.ReloadScene();
            }
            else
                MessageBox.Show("Не выбран объект");
        }

        private void buttonObjectCreate_Click(object sender, EventArgs e)
        {
            if (textBoxObjectName.Text != "" && comboBoxModel.SelectedItem != null && comboBoxTexture.SelectedItem != null)
            {
                engine.scene.CreateObject(textBoxObjectName.Text, comboBoxModel.Text, comboBoxTexture.Text);

                comboBoxObject.Items.Add(textBoxObjectName.Text);

                textBoxObjectName.Text = "";
                comboBoxModel.SelectedItem = null;
                comboBoxTexture.SelectedItem = null;
            }
            else
            {
                string[] msg_error = new string[] { "", "", "" };

                if (textBoxObjectName.Text == "")
                    msg_error[0] = "Не указано имя объекта\n";

                if (comboBoxModel.SelectedItem == null)
                    msg_error[1] = "Не выбрана модель объекта\n";

                if (comboBoxTexture.SelectedItem == null)
                    msg_error[2] = "Не выбрана текстура объекта";

                MessageBox.Show($"Ошибка: \n{msg_error[0]}{msg_error[1]}{msg_error[2]}");
            }
        }

        private void buttonModelLoad_Click(object sender, EventArgs e)
        {
            if (openFileObject.FileName != null && openFileObject.FileName != "")
            {
                bool write = engine.scene.AddResourceModel(openFileObject.FileName);

                if (write == true)
                {
                    comboBoxModel.Items.Add(engine.scene.GetFileName(openFileObject.FileName));
                    listModelsLoaded.Items.Add(engine.scene.GetFileName(openFileObject.FileName));
                }
            }
            else
                MessageBox.Show("Файл не выбран");
        }

        private void buttonModelDelete_Click(object sender, EventArgs e)
        {
            if (listModelsLoaded.SelectedItem != null && listModelsLoaded.SelectedItem != "")
            {
                engine.scene.RemoveResource(listModelsLoaded.SelectedItem.ToString(), "model");

                List<string> deleted_objects = engine.scene.DeleteRelatedObjects(listModelsLoaded.SelectedItem.ToString());

                comboBoxModel.Items.Remove(listModelsLoaded.SelectedItem);
                listModelsLoaded.Items.Remove(listModelsLoaded.SelectedItem);

                foreach (string obj in deleted_objects)
                {
                    comboBoxObject.Items.Remove(obj);
                }

                listModelsLoaded.SelectedItem = null;
                engine.ReloadScene();
            }
            else
                MessageBox.Show("Ни одна модель в списке не выделена");
        }

        private void buttonTextureLoad_Click(object sender, EventArgs e)
        {
            if (openFileTexture.FileName != null && openFileTexture.FileName != "")
            {
                bool write = engine.scene.AddResourceTexture(openFileTexture.FileName);

                if (write == true)
                {
                    comboBoxTexture.Items.Add(engine.scene.GetFileName(openFileTexture.FileName));
                    listTexturesLoaded.Items.Add(engine.scene.GetFileName(openFileTexture.FileName));
                }
            }
            else
                MessageBox.Show("Файл не выбран");
        }

        private void buttonTextureDelete_Click(object sender, EventArgs e)
        {
            if (listTexturesLoaded.SelectedItem != null && listTexturesLoaded.SelectedItem != "")
            {
                engine.scene.RemoveResource(listTexturesLoaded.SelectedItem.ToString(), "texture");

                List<string> deleted_objects = engine.scene.DeleteRelatedObjects(listTexturesLoaded.SelectedItem.ToString());

                comboBoxTexture.Items.Remove(listTexturesLoaded.SelectedItem);
                listTexturesLoaded.Items.Remove(listTexturesLoaded.SelectedItem);

                foreach (string obj in deleted_objects)
                {
                    comboBoxObject.Items.Remove(obj);
                }

                listTexturesLoaded.SelectedItem = null;
                engine.ReloadScene();
            }
            else
                MessageBox.Show("Ни одна модель в списке не выделена");
        }
    }
}
