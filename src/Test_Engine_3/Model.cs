using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace Test_Engine_3
{
    public class Model
    {
        public float[] vertices;
        public uint[] indices;
        float[] vertices_orig;
        float[] textures;
        
        public Model(string path)
        {
            string file = File.ReadAllText(path);
            int vertices_size = 0;
            int indices_size = 0;
            int textures_size = 0;

            int vertices_orig_size = 0;
            int textures_orig_size = 0;

            for (int i = 0; i< file.Length; i++)
            {
                if (file[i] == 'v' && file[i + 1] == ' ')
                {
                    vertices_orig_size += 3;
                }

                if (file[i] == 'v' && file[i + 1] == 't' && file[i + 2] == ' ')
                {
                    textures_size += 2;
                }

                if (file[i] == 'f' && file[i + 1] == ' ')
                {
                    indices_size += 3;
                    vertices_size += 15;
                }
            }

            vertices = new float[vertices_size];
            vertices_orig = new float[vertices_orig_size];
            textures = new float[textures_size];
            indices = new uint[indices_size];

            int vertices_count = 0;
            int vertices_orig_count = 0;
            int indices_count = 0;
            int textures_count = 0;

            for (int i = 0; i < file.Length; i++)
            {
                if (file[i] == 'v' && file[i + 1] == ' ')
                {
                    int offset1 = 0;
                    int offset2 = 0;
                    int offset3 = 0;

                    if (file[i + 2] == '-')
                        offset1 = 1;

                    if (file[i + 12] == '-' || file[i + 11] == '-')
                        offset2 = 1;

                    if (file[i + 20] == '-' || file[i + 21] == '-' || file[i + 22] == '-')
                        offset3 = 1;

                    vertices_orig[vertices_orig_count] = float.Parse(file.Substring(i + 2, 4 + offset1), CultureInfo.InvariantCulture.NumberFormat);
                    vertices_orig_count++;

                    vertices_orig[vertices_orig_count] = float.Parse(file.Substring(i + 11 + offset1, 4 + offset2), CultureInfo.InvariantCulture.NumberFormat);
                    vertices_orig_count++;

                    vertices_orig[vertices_orig_count] = float.Parse(file.Substring(i + 20 + offset1 + offset2, 4 + offset3), CultureInfo.InvariantCulture.NumberFormat);
                    vertices_orig_count++;
                }

                if (file[i] == 'v' && file[i + 1] == 't' && file[i + 2] == ' ')
                {
                    textures[textures_count] = float.Parse(file.Substring(i + 3, 4), CultureInfo.InvariantCulture.NumberFormat);
                    textures_count++;

                    textures[textures_count] = float.Parse(file.Substring(i + 12, 4), CultureInfo.InvariantCulture.NumberFormat);
                    textures_count++;
                }
            }

            for (int i = 0; i < file.Length; i++)
            {
                if (file[i] == 'f' && file[i + 1] == ' ')
                {
                    int j = 0;
                    string vert1 = "";

                    while (file[i + 2 + j] != '/')
                    {
                        vert1 += file[i + 2 + j];
                        j++;
                    }

                    vertices[vertices_count] = vertices_orig[(int.Parse(vert1) - 1) * 3];
                    vertices_count++;

                    vertices[vertices_count] = vertices_orig[(int.Parse(vert1) - 1) * 3 + 1];
                    vertices_count++;

                    vertices[vertices_count] = vertices_orig[(int.Parse(vert1) - 1) * 3 + 2];
                    vertices_count++;

                    //indices[indices_count] = uint.Parse(vert1, CultureInfo.InvariantCulture.NumberFormat);
                    //indices_count++;

                    vert1 = "";

                    while (file[i + 3 + j] != '/')
                    {
                        vert1 += file[i + 3 + j];
                        j++;
                    }

                    vertices[vertices_count] = textures[(int.Parse(vert1) - 1) * 2];
                    vertices_count++;

                    vertices[vertices_count] = textures[(int.Parse(vert1) - 1) * 2 + 1];
                    vertices_count++;

                    vert1 = "";

                    while (file[i + 4 + j] != ' ')
                    {
                        j++;
                    }



                    while (file[i + 5 + j] != '/')
                    {
                        vert1 += file[i + 5 + j];
                        j++;
                    }

                    vertices[vertices_count] = vertices_orig[(int.Parse(vert1) - 1) * 3];
                    vertices_count++;

                    vertices[vertices_count] = vertices_orig[(int.Parse(vert1) - 1) * 3 + 1];
                    vertices_count++;

                    vertices[vertices_count] = vertices_orig[(int.Parse(vert1) - 1) * 3 + 2];
                    vertices_count++;

                    //indices[indices_count] = uint.Parse(vert1, CultureInfo.InvariantCulture.NumberFormat);
                    //indices_count++;

                    vert1 = "";

                    while (file[i + 6 + j] != '/')
                    {
                        vert1 += file[i + 6 + j];
                        j++;
                    }

                    vertices[vertices_count] = textures[(int.Parse(vert1) - 1) * 2];
                    vertices_count++;

                    vertices[vertices_count] = textures[(int.Parse(vert1) - 1) * 2 + 1];
                    vertices_count++;

                    vert1 = "";

                    while (file[i + 7 + j] != ' ')
                    {
                        j++;
                    }



                    while (file[i + 8 + j] != '/')
                    {
                        vert1 += file[i + 8 + j];
                        j++;
                    }

                    vertices[vertices_count] = vertices_orig[(int.Parse(vert1) - 1) * 3];
                    vertices_count++;

                    vertices[vertices_count] = vertices_orig[(int.Parse(vert1) - 1) * 3 + 1];
                    vertices_count++;

                    vertices[vertices_count] = vertices_orig[(int.Parse(vert1) - 1) * 3 + 2];
                    vertices_count++;

                    //indices[indices_count] = uint.Parse(vert1, CultureInfo.InvariantCulture.NumberFormat);
                    //indices_count++;

                    vert1 = "";

                    while (file[i + 9 + j] != '/')
                    {
                        vert1 += file[i + 9 + j];
                        j++;
                    }

                    vertices[vertices_count] = textures[(int.Parse(vert1) - 1) * 2];
                    vertices_count++;

                    vertices[vertices_count] = textures[(int.Parse(vert1) - 1) * 2 + 1];
                    vertices_count++;
                }
            }

            for (int i = 0; i < indices_size; i++)
            {
                indices[i] = (uint)i;
            }
        }

        public float[] GetVertices()
        {
            return vertices;
        }

        public uint[] GetIndices()
        {
            return indices;
        }
    }
}
