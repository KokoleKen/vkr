using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Test_Engine_3
{
    public class Scene
    {
        public Dictionary<string, Model> models = new Dictionary<string, Model>();
        public Dictionary<string, string> textures = new Dictionary<string, string>();

        public Scene() 
        {
            string file = File.ReadAllText("Objects/Scene.data");

            for (int i = 0; i < file.Length; i++)
            {
                if (file[i] == '$')
                {
                    string name = "";
                    string path = "";
                    int j = 0;
                    while (file[i + j] != '@')
                    {
                        if (file[i + j] != '$')
                            name += file[i + j];
                        j++;
                    }

                    if (file[i + j] == '@')
                        while (file[i + j] != '!')
                        {
                            if (file[i + j] != '@')
                                path += file[i + j];
                            j++;
                        }

                    models.Add(name, new Model("Objects/Models/" + path));
                }

                if (file[i] == '*')
                {
                    string name = "";
                    string path = "";
                    int j = 0;
                    while (file[i + j] != '@')
                    {
                        if (file[i + j] != '*')
                            name += file[i + j];
                        j++;
                    }

                    if (file[i + j] == '@')
                        while (file[i + j] != '!')
                        {
                            if (file[i + j] != '@')
                                path += file[i + j];
                            j++;
                        }

                    textures.Add(name, new string("Objects/Textures/" + path));
                }
            }
        }
    }
}
