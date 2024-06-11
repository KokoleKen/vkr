using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using System.Xml.Linq;

namespace Test_Engine_3
{
    public class Scene
    {
        public Dictionary<string, Tuple<Model, string>> objects = new Dictionary<string, Tuple<Model, string>>();
        //public List<string> models = new List<string>();
        public string[] models = new string[] { };
        public string[] textures = new string[] { };
        //public Dictionary<string, Model> models = new Dictionary<string, Model>();
        //public Dictionary<string, string> textures = new Dictionary<string, string>();

        public Scene() 
        {
            //string file = File.ReadAllText("Objects/Scene.data");
            string[] file_objects = File.ReadAllLines("Objects/Objects.data");
            string[] file_resources = File.ReadAllLines("Objects/Resources.data");

            // Новая система хранения объектов

            for (int i = 0; i < file_objects.Length; i++)
            {
                int datatype = 0;

                string name = "";
                string model = "";
                string texture = "";

                for (int j = 0; j < file_objects[i].Length; j++)
                {
                    if (datatype == 0 && file_objects[i][j] != '\\')
                    {
                        name += file_objects[i][j];
                    }
                    else if (datatype == 1 && file_objects[i][j] != '\\')
                    {
                        model += file_objects[i][j];
                    }
                    else if (datatype == 2 && file_objects[i][j] != '\\')
                    {
                        texture += file_objects[i][j];
                    }
                    else if (datatype == 0 && file_objects[i][j] == '\\')
                    {
                        datatype = 1;
                    }
                    else if (datatype == 1 && file_objects[i][j] == '\\')
                    {
                        datatype = 2;
                    }
                }

                objects.Add(name, Tuple.Create<Model, string>(new Model("Objects/Models/" + model), texture));
            }

            int textures_count = 0;
            int models_count = 0;

            int texture_number = 0;
            int model_number = 0;

            for (int j = 0; j < file_resources.Length; j++)
            {
                if (file_resources[j].Length > 0)
                {
                    if (file_resources[j][0] == '\\' && file_resources[j][1] == 'T' && file_resources[j][2] == 'E' && file_resources[j][3] == 'X')
                    {
                        textures_count++;
                    }

                    if (file_resources[j][0] == '\\' && file_resources[j][1] == 'M' && file_resources[j][2] == 'O' && file_resources[j][3] == 'D')
                    {
                        models_count++;
                    }
                }
            }

            models = new string[models_count];
            textures = new string[textures_count];

            for (int i = 0; i < file_resources.Length; i++)
            {
                string resource = "";

                if (file_resources[i].Length > 0)
                {
                    if (file_resources[i][0] == '\\' && file_resources[i][1] == 'T' && file_resources[i][2] == 'E' && file_resources[i][3] == 'X')
                    {
                        for (int j = 0; j < file_resources[i].Length; j++)
                        {
                            if (j > 8)
                            {
                                resource += file_resources[i][j];
                            }
                        }

                        textures[texture_number] = resource;
                        texture_number++;
                    }
                    else if (file_resources[i][0] == '\\' && file_resources[i][1] == 'M' && file_resources[i][2] == 'O' && file_resources[i][3] == 'D')
                    {
                        for (int j = 0; j < file_resources[i].Length; j++)
                        {
                            if (j > 6)
                            {
                                resource += file_resources[i][j];
                            }
                        }

                        //models.Add(resource);
                        models[model_number] = resource;
                        model_number++;
                    }
                }
            }

            //for (int i = 0; i < file.Length; i++)
            //{
            //    if (file[i] == '$')
            //    {
            //        string name = "";
            //        string path = "";
            //        int j = 0;
            //        while (file[i + j] != '@')
            //        {
            //            if (file[i + j] != '$')
            //                name += file[i + j];
            //            j++;
            //        }

                //        if (file[i + j] == '@')
                //            while (file[i + j] != '!')
                //            {
                //                if (file[i + j] != '@')
                //                    path += file[i + j];
                //                j++;
                //            }

                //        models.Add(name, new Model("Objects/Models/" + path));
                //    }

                //    if (file[i] == '*')
                //    {
                //        string name = "";
                //        string path = "";
                //        int j = 0;
                //        while (file[i + j] != '@')
                //        {
                //            if (file[i + j] != '*')
                //                name += file[i + j];
                //            j++;
                //        }

                //        if (file[i + j] == '@')
                //            while (file[i + j] != '!')
                //            {
                //                if (file[i + j] != '@')
                //                    path += file[i + j];
                //                j++;
                //            }

                //        textures.Add(name, new string("Objects/Textures/" + path));
                //    }
                //}

        }

        public void CreateObject(string name, string model_name, string texture_name)
        {
            string[] objects_name = File.ReadAllLines("Objects/Objects.data");
            string[] save_data = new string[objects_name.Length + 1];

            objects_name.CopyTo(save_data, 0);

            save_data[objects_name.Length] = $"{name}\\{model_name}\\{texture_name}";

            objects.Add(name, Tuple.Create<Model, string>(new Model("Objects/Models/" + model_name), texture_name));

            MessageBox.Show($"Объект {name} успешно создан!");

            File.WriteAllLines("Objects/Objects.data", save_data);
        }

        public void DeleteObject(string name)
        {
            string[] objects_name = File.ReadAllLines("Objects/Objects.data");
            string[] save_data;

            for (int i = 0; i < objects_name.Length; i++)
            {
                string object_name = "";

                for (int j = 0; j < name.Length; j++) 
                {
                    object_name += objects_name[i][j];
                }

                if (name == object_name)
                {
                    MessageBox.Show($"Объект {object_name} успешно удалён!");
                    objects_name[i] = "";
                }
            }

            int size = 0;
            int n = 0;

            for (int i = 0; i < objects_name.Length; i++)
            {
                if (objects_name[i] != "")
                {
                    size++;
                }
            }

            save_data = new string[size];

            for (int i = 0; i < objects_name.Length; i++)
            {
                if (objects_name[i] != "")
                {
                    save_data[n] = objects_name[i];
                    n++;
                }
            }

            File.WriteAllLines("Objects/Objects.data", save_data);
        }

        /// <summary>
        /// Удаляет объекты из проекта, имеющие отношение к файлу relate.
        /// </summary>
        /// <param name="relate"></param>
        /// <returns>Лист имён удаленных объектов.</returns>
        public List<string> DeleteRelatedObjects(string relate)
        {
            string[] objects_name = File.ReadAllLines("Objects/Objects.data");
            string[] save_data;
            List<string> names = new List<string>();

            for (int i = 0; i < objects_name.Length; i++)
            {
                if (objects_name[i].Contains(relate))
                {
                    string object_name = "";

                    for (int j = 0; objects_name[i][j] != '\\'; j++)
                    {
                        object_name += objects_name[i][j];
                    }

                    objects_name[i] = "";
                    names.Add(object_name);
                }
            }

            int size = 0;
            int n = 0;

            for (int i = 0; i < objects_name.Length; i++)
            {
                if (objects_name[i] != "")
                {
                    size++;
                }
            }

            save_data = new string[size];

            for (int i = 0; i < objects_name.Length; ++i)
            {
                if (objects_name[i] != "")
                {
                    save_data[n] = objects_name[i];
                    n++;
                }
            }

            File.WriteAllLines("Objects/Objects.data", save_data);

            string msg = $"Были удалены следующие объекты, содержащие в себе файл \"{relate}\":\n";

            foreach (string deleted_object in names)
            {
                msg += "Объект " + deleted_object + "\n";
            }

            if (names.Count != 0)
                MessageBox.Show(msg);
            return names;
        }

        /// <summary>
        /// Загружает в проект файл модели по указанному пути.
        /// </summary>
        /// <param name="origFile"></param>
        /// <returns>Результат того, был ли успешно записан (перезаписан) файл в проект.</returns>
        public bool AddResourceModel(string origFile)
        {
            string[] file_resources = File.ReadAllLines("Objects/Resources.data");
            string[] save_data = new string[file_resources.Length + 1];

            bool rewrite = false;
            bool write = false;

            string model_name = "";

            for (int i = origFile.Length - 1; origFile[i] != '\\'; i--)
            {
                model_name += origFile[i];
            }

            char[] name_reverse = model_name.ToCharArray();
            Array.Reverse(name_reverse);
            model_name = new string(name_reverse);

            if (File.Exists("Objects/Models/" + model_name))
            {
                DialogResult dialogResult = MessageBox.Show($"Файл {model_name} уже существует в проекте. Хотите перезаписать его?", "Внимание", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    write = true;
                }
                else if (dialogResult == DialogResult.No)
                {
                    return write;
                }
            }

            write = true;

            File.Copy(origFile, "Objects/Models/" + model_name, true);

            for (int i = 0; i < file_resources.Length; i++)
            {
                if (file_resources[i].Contains(model_name))
                    rewrite = true;
            }

            if (rewrite == false)
            {
                file_resources.CopyTo(save_data, 0);

                save_data[file_resources.Length] = $"\\MODEL\\{model_name}";

                File.WriteAllLines("Objects/Resources.data", save_data);
            }

            MessageBox.Show($"Файл {model_name} успешно загружен в проект!");

            return write;
        }

        public bool AddResourceTexture(string origFile)
        {
            string[] file_resources = File.ReadAllLines("Objects/Resources.data");
            string[] save_data = new string[file_resources.Length + 1];

            bool rewrite = false;
            bool write = false;

            string texture_name = "";

            for (int i = origFile.Length - 1; origFile[i] != '\\'; i--)
            {
                texture_name += origFile[i];
            }

            char[] name_reverse = texture_name.ToCharArray();
            Array.Reverse(name_reverse);
            texture_name = new string(name_reverse);

            if (File.Exists("Objects/Textures/" + texture_name))
            {
                DialogResult dialogResult = MessageBox.Show($"Файл {texture_name} уже существует в проекте. Хотите перезаписать его?", "Внимание", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    write = true;
                }
                else if (dialogResult == DialogResult.No)
                {
                    return write;
                }
            }

            write = true;

            File.Copy(origFile, "Objects/Textures/" + texture_name, true);

            for (int i = 0; i < file_resources.Length; i++)
            {
                if (file_resources[i].Contains(texture_name))
                    rewrite = true;
            }

            if (rewrite == false)
            {
                file_resources.CopyTo(save_data, 0);

                int offset = 0;
                for (int i = 0; i < file_resources.Length; i++)
                {
                    if (file_resources[i] != "")
                        if (file_resources[i][0] == '\\' && file_resources[i][1] == 'T')
                            offset++;
                }

                string[] ar_offset = new string[file_resources.Length + 1];
                file_resources.CopyTo(ar_offset, 0);

                save_data[offset] = $"\\TEXTURE\\{texture_name}";

                for (int i = offset + 1; i <  save_data.Length; i++)
                {
                    save_data[i] = ar_offset[i - 1];
                }

                File.WriteAllLines("Objects/Resources.data", save_data);
            }

            MessageBox.Show($"Файл {texture_name} успешно загружен в проект!");

            return write;
        }

        public void RemoveResource(string resName, string type)
        {
            string[] file_resources = File.ReadAllLines("Objects/Resources.data");
            string[] save_data;

            for (int i = 0; i < file_resources.Length; i++)
            {
                string model_name = "";

                if (file_resources[i] != "")
                {
                    if (file_resources[i][0] == '\\' && file_resources[i][1] == 'T')
                    {
                        for (int j = 9; j < file_resources[i].Length; j++)
                        {
                            model_name += file_resources[i][j];
                        }
                    }

                    if (file_resources[i][0] == '\\' && file_resources[i][1] == 'M')
                    {
                        for (int j = 7; j < file_resources[i].Length; j++)
                        {
                            model_name += file_resources[i][j];
                        }
                    }
                }

                if (resName == model_name)
                {
                    if (type == "model")
                        MessageBox.Show($"Файл модели {model_name} успешно удалён из проекта!");
                    else if (type == "texture")
                        MessageBox.Show($"Файл текстуры {model_name} успешно удалён из проекта!");
                    file_resources[i] = "\\";
                }
            }

            int size = 0;
            int n = 0;

            for (int i = 0; i < file_resources.Length; i++)
            {
                if (file_resources[i] != "\\")
                {
                    size++;
                }
            }

            save_data = new string[size];

            for (int i = 0; i < file_resources.Length; i++)
            {
                if (file_resources[i] != "\\")
                {
                    save_data[n] = file_resources[i];
                    n++;
                }
            }

            if (type == "model")
                File.Delete("Objects/Models/" + resName);
            else if (type == "texture")
                File.Delete("Objects/Textures/" + resName);

            File.WriteAllLines("Objects/Resources.data", save_data);
        }

        public string GetFileName(string origFile) 
        {
            string model_name = "";

            for (int i = origFile.Length - 1; origFile[i] != '\\'; i--)
            {
                model_name += origFile[i];
            }

            char[] name_reverse = model_name.ToCharArray();
            Array.Reverse(name_reverse);
            model_name = new string(name_reverse);

            return model_name;
        }
    }
}
