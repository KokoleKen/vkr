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
        public float[] normals;
        float[] textures;

        public int progress = 0;
        public int progress_max = 0;

        public Model(string path)
        {
            string[] file = File.ReadAllLines(path);
            string file_stream = File.ReadAllText(path);
            int vertices_size = 0;
            int normals_size = 0;
            int textures_size = 0;
            int indices_size = 0;

            progress_max = file.Length * 3;

            int vertices_orig_size = 0;
            int textures_orig_size = 0;

            for (int i = 0; i < file.Length; i++)
            {
                if (file[i][0] == 'v' && file[i][1] == ' ')
                {
                    vertices_orig_size += 3;
                }

                if (file[i][0] == 'v' && file[i][1] == 'n' && file[i][2] == ' ')
                {
                    normals_size += 3;
                }

                if (file[i][0] == 'v' && file[i][1] == 't' && file[i][2] == ' ')
                {
                    textures_size += 2;
                }

                if (file[i][0] == 'f' && file[i][1] == ' ')
                {
                    indices_size += 3;
                    vertices_size += 24;
                }

                progress++;
            }

            vertices = new float[vertices_size];
            vertices_orig = new float[vertices_orig_size];
            normals = new float[normals_size];
            textures = new float[textures_size];
            indices = new uint[indices_size];

            int vertices_count = 0;
            int vertices_orig_count = 0;
            int normals_count = 0;
            int indices_count = 0;
            int textures_count = 0;

            for (int i = 0; i < file.Length; i++)
            {
                if (file[i][0] == 'v' && file[i][1] == ' ')
                {
                    string digit = "";
                    bool is_digit = false;

                    for (int j = 0; j < file[i].Length; j++)
                    {
                        if (is_digit == true)
                        {
                            if (file[i][j] == ' ')
                            {
                                vertices_orig[vertices_orig_count] = float.Parse(digit, CultureInfo.InvariantCulture.NumberFormat);
                                vertices_orig_count++;

                                digit = "";
                            }
                            else if (j == file[i].Length - 1)
                            {
                                vertices_orig[vertices_orig_count] = float.Parse(digit, CultureInfo.InvariantCulture.NumberFormat);
                                vertices_orig_count++;
                                digit = "";
                            }
                            else
                            {
                                digit += file[i][j];
                            }
                        }
                        else if (file[i][j] == ' ')
                        {
                            is_digit = true;
                        }
                    }
                }
                else if (file[i][0] == 'v' && file[i][1] == 'n' && file[i][2] == ' ')
                {
                    string digit = "";
                    bool is_digit = false;

                    for (int j = 0; j < file[i].Length; j++)
                    {
                        if (is_digit == true)
                        {
                            if (file[i][j] == ' ')
                            {
                                normals[normals_count] = float.Parse(digit, CultureInfo.InvariantCulture.NumberFormat);
                                normals_count++;

                                digit = "";
                            }
                            else if (j == file[i].Length - 1)
                            {
                                normals[normals_count] = float.Parse(digit, CultureInfo.InvariantCulture.NumberFormat);
                                normals_count++;
                                digit = "";
                            }
                            else
                            {
                                digit += file[i][j];
                            }
                        }
                        else if (file[i][j] == ' ')
                        {
                            is_digit = true;
                        }
                    }
                }
                else if (file[i][0] == 'v' && file[i][1] == 't' && file[i][2] == ' ')
                {
                    string digit = "";
                    bool is_digit = false;

                    for (int j = 0; j < file[i].Length; j++)
                    {
                        if (is_digit == true)
                        {
                            if (file[i][j] == ' ')
                            {
                                textures[textures_count] = float.Parse(digit, CultureInfo.InvariantCulture.NumberFormat);
                                textures_count++;

                                digit = "";
                            }
                            else if (j == file[i].Length - 1)
                            {
                                textures[textures_count] = float.Parse(digit, CultureInfo.InvariantCulture.NumberFormat);
                                textures_count++;
                                digit = "";
                            }
                            else
                            {
                                digit += file[i][j];
                            }
                        }
                        else if (file[i][j] == ' ')
                        {
                            is_digit = true;
                        }
                    }
                }

                progress++;
            }

            for (int i = 0; i < file.Length; i++)
            {
                if (file[i][0] == 'f' && file[i][1] == ' ')
                {
                    string digit = "";
                    int data_type = 0;

                    for (int j = 0; j < file[i].Length; j++)
                    {
                        if (data_type == 0)
                        {
                            if (file[i][j] == ' ')
                            {
                                data_type = 1;
                            }
                        }
                        else if (data_type == 1)
                        {
                            if (file[i][j] == '/')
                            {
                                vertices[vertices_count] = vertices_orig[(int.Parse(digit) - 1) * 3];
                                vertices_count++;

                                vertices[vertices_count] = vertices_orig[(int.Parse(digit) - 1) * 3 + 1];
                                vertices_count++;

                                vertices[vertices_count] = vertices_orig[(int.Parse(digit) - 1) * 3 + 2];
                                vertices_count++;


                                indices[indices_count] = uint.Parse(digit, CultureInfo.InvariantCulture.NumberFormat);
                                indices_count++;

                                digit = "";
                                data_type++;
                            }
                            else
                            {
                                digit += file[i][j];
                            }
                        }
                        else if (data_type == 2)
                        {
                            if (file[i][j] == '/')
                            {
                                vertices[vertices_count] = textures[(int.Parse(digit) - 1) * 2];
                                vertices_count++;

                                vertices[vertices_count] = textures[(int.Parse(digit) - 1) * 2 + 1];
                                vertices_count++;

                                digit = "";
                                data_type++;
                            }
                            else
                            {
                                digit += file[i][j];
                            }
                        }
                        else if (data_type == 3)
                        {
                            if (file[i][j] == ' ')
                            {
                                vertices[vertices_count] = normals[(int.Parse(digit) - 1) * 3];
                                vertices_count++;

                                vertices[vertices_count] = normals[(int.Parse(digit) - 1) * 3 + 1];
                                vertices_count++;

                                vertices[vertices_count] = normals[(int.Parse(digit) - 1) * 3 + 2];
                                vertices_count++;

                                digit = "";
                                data_type = 1;
                            }
                            else if (j == file[i].Length - 1)
                            {
                                digit += file[i][j];

                                vertices[vertices_count] = normals[(int.Parse(digit) - 1) * 3];
                                vertices_count++;

                                vertices[vertices_count] = normals[(int.Parse(digit) - 1) * 3 + 1];
                                vertices_count++;

                                vertices[vertices_count] = normals[(int.Parse(digit) - 1) * 3 + 2];
                                vertices_count++;

                                digit = "";
                                data_type = 1;
                            }
                            else
                            {
                                digit += file[i][j];
                            }
                        }
                    }
                }

                progress++;
            }

            for (int i = 0; i < indices_size; i++)
            {
                indices[i] = (uint)i;
            }
        }

        public float[] GetVertices()
        {
            //progress_max = vertices.Length + vertices_orig.Length;
            //string text = "Parsed vertices:\n";
            //int line = 0;
            //progress = 0;

            //for (int i = 0; i < vertices.Length; i++)
            //{
            //    text += vertices[i].ToString() + "/ ";

            //    line++;

            //    if (line == 8)
            //    {
            //        text += "\n";
            //        line = 0;
            //    }

            //    progress++;
            //}

            //line = 0;
            //text += "\nVertices:\n";

            //for (int i = 0; i < vertices_orig.Length; i++)
            //{
            //    text += vertices_orig[i].ToString() + "/ ";

            //    line++;

            //    if (line == 3)
            //    {
            //        text += "\n";
            //        line = 0;
            //    }

            //    progress++;
            //}

            //File.WriteAllText("vertices_log.txt", text);

            return vertices;
        }

        public uint[] GetIndices()
        {
            //string text = "";
            //int line = 0;

            //for (int i = 0; i < indices.Length; i++)
            //{
            //    text += indices[i].ToString() + "/ ";

            //    line++;

            //    if (line == 8)
            //    {
            //        text += "\n";
            //        line = 0;
            //    }
            //}

            //File.WriteAllText("indices_log.txt", text);

            return indices;
        }
    }
}
