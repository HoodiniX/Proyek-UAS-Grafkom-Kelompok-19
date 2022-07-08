﻿using LearnOpenTK.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using System;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Text;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ProyekUAS
{
    internal class Asset3d
    {
        private readonly string path = "../../../";

        List<Vector3> _vertices = new List<Vector3>();
        List<uint> _indices = new List<uint>();
        private List<Vector3> normals = new List<Vector3>();
        private List<Vector3> texCoords = new List<Vector3>();

        int _vertexBufferObject;
        int _vertexArrayObject;
        int _elementBufferObject;
        public Vector4 _color, _lightColor;

        Shader _shader;
        private Texture _texture;

        private string vertName;
        private string fragName;

        Matrix4 _view;
        Matrix4 _projection;
        Matrix4 _model = Matrix4.Identity;
        private Matrix4 normalMat = Matrix4.Identity;

        public Vector3 _centerPosition = Vector3.Zero;
        public List<Vector3> _euler = new List<Vector3>();
        public List<Asset3d> Child = new List<Asset3d>();

        public Asset3d(List<Vector3> vertices, List<uint> indices)
        {
            _vertices = vertices;
            _indices = indices;
        }

        public Asset3d(string vertName, string fragName, Vector3 color, float alpha = 1)
        {
            _color = new Vector4(color, alpha);
            this.vertName = vertName;
            this.fragName = fragName;
            _euler.Add(Vector3.UnitX);
            _euler.Add(Vector3.UnitY);
            _euler.Add(Vector3.UnitZ);
        }

        public Asset3d(Vector4 color)
        {
            _color = color;
            /*this._lightColor = lightColor;*/
        }

        public Asset3d()
        {
            _vertices = new List<Vector3>();
            setdefault();
        }

        public void setdefault()
        {
            //sumbu X
            _euler.Add(new Vector3(1, 0, 0));
            //sumbu y
            _euler.Add(new Vector3(0, 1, 0));
            //sumbu z
            _euler.Add(new Vector3(0, 0, 1));
        }

        public void load(string shadervert, string shaderfrag, float Size_x, float Size_y)
        {
            //Buffer
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count
                * Vector3.SizeInBytes, _vertices.ToArray(), BufferUsageHint.StaticDraw);
            //VAO
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            //kalau mau bikin object settingannya beda dikasih if
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float,
                false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            //ada data yang disimpan di _indices
            if (_indices.Count != 0)
            {
                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Count
                    * sizeof(uint), _indices.ToArray(), BufferUsageHint.StaticDraw);
            }

            _shader = new Shader(shadervert, shaderfrag);
            _shader.Use();

            // For the view, we don't do too much here. Next tutorial will be all about a Camera class that will make it much easier to manipulate the view.
            // For now, we move it backwards three units on the Z axis.
            _view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);

            // For the matrix, we use a few parameters.
            //   Field of view. This determines how much the viewport can see at once. 45 is considered the most "realistic" setting, but most video games nowadays use 90
            //   Aspect ratio. This should be set to Width / Height.
            //   Near-clipping. Any vertices closer to the camera than this value will be clipped.
            //   Far-clipping. Any vertices farther away from the camera than this value will be clipped.
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size_x / (float)Size_y, 0.1f, 100.0f);

            foreach (var item in Child)
            {
                item.load(shadervert, shaderfrag, Size_x, Size_y);
            }
        }

        public void load2()
        {
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            if (texCoords.Count == 0 && normals.Count == 0)
            {
                GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vector3.SizeInBytes, _vertices.ToArray(), BufferUsageHint.StaticDraw);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);
            }
            else if (texCoords.Count > 0)
            {
                var combinedData = new List<Vector3>();
                for (int i = 0; i < _vertices.Count; i++)
                {
                    combinedData.Add(_vertices[i]);
                    combinedData.Add(texCoords[i]);
                }

                GL.BufferData(BufferTarget.ArrayBuffer, combinedData.Count * Vector3.SizeInBytes, combinedData.ToArray(), BufferUsageHint.StaticDraw);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);
            }
            else if (normals.Count > 0)
            {
                var combinedData = new List<Vector3>();
                for (int i = 0; i < _vertices.Count; i++)
                {
                    combinedData.Add(_vertices[i]);
                    combinedData.Add(normals[i]);
                }

                GL.BufferData(BufferTarget.ArrayBuffer, combinedData.Count * Vector3.SizeInBytes, combinedData.ToArray(), BufferUsageHint.StaticDraw);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);
            }

            if (_indices.Count != 0)
            {
                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Count * sizeof(uint), _indices.ToArray(), BufferUsageHint.StaticDraw);
            }

            _shader = new Shader(path + "Shaders/" + vertName, path + "Shaders/" + fragName);
            _shader.Use();

            if (texCoords.Count > 0)
            {
                _texture = Texture.LoadFromFile(path + "textures/top.png");
                _texture.Use(TextureUnit.Texture0);
            }

            foreach (var i in Child)
            {
                i.load2();
            }
        }

        public void render(Vector4 color, int _lines, double time, Matrix4 temp, Matrix4 camera_view, Matrix4 camera_projection)
        {
            _shader.Use();

            int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "objColor");

            GL.Uniform4(vertexColorLocation, color);

            GL.BindVertexArray(_vertexArrayObject);
            // Finally, we have the model matrix. This determines the position of the model.
            var model = Matrix4.Identity /** Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(time))*/;
            model = temp;
            //// To combine two matrices, you multiply them. Here, we combine the transform matrix with another one created by OpenTK to rotate it by 20 degrees.
            //// Note that all Matrix4.CreateRotation functions take radians, not degrees. Use MathHelper.DegreesToRadians() to convert to radians, if you want to use degrees.
            //model = model * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(20f));

            //// Next, we scale the matrix. This will make the rectangle slightly larger.
            //model = model * Matrix4.CreateScale(1.1f);

            //// Then, we translate the matrix, which will move it slightly towards the top-right.
            //// Note that we aren't using a full coordinate system yet, so the translation is in normalized device coordinates.
            //// The next tutorial will be about how to set one up so we can use more human-readable numbers.
            //model = model * Matrix4.CreateTranslation(0.1f, 0.1f, 0.0f);
            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", camera_view);
            _shader.SetMatrix4("projection", camera_projection);


            if (_indices.Count != 0)
            {
                GL.DrawElements(PrimitiveType.Triangles, _indices.Count, DrawElementsType.UnsignedInt, 0);
            }
            else
            {

                if (_lines == 0)
                {
                    GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count);
                }
                else if (_lines == 1)
                {
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, _vertices.Count);
                }
                else if (_lines == 2)
                {

                }
                else if (_lines == 3)
                {
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, _vertices.Count);
                }
            }
            foreach (var item in Child)
            {
                item.render(new Vector4(0.66274509803f, 0.662745098039f, 0.66274509803f, 1.0f), _lines, time, temp, camera_view, camera_projection);
            }
        }

        public void render2(int line, Matrix4 camera_view, Matrix4 camera_projection, Vector3 cameraPos, Asset3d light)
        {
            _shader.Use();

            _shader.SetMatrix4("model", _model);
            _shader.SetMatrix4("view", camera_view);
            _shader.SetMatrix4("projection", camera_projection);

            GL.BindVertexArray(_vertexArrayObject);

            if (texCoords.Count > 0)
            {
                _texture.Use(TextureUnit.Texture0);
            }
            else
            {
                _shader.SetVector4("objColor", _color);
            }

            if (normals.Count > 0)
            {
                _shader.SetMatrix4("normalMat", normalMat);

                _shader.SetVector3("lightPos", light._centerPosition);
                _shader.SetVector3("lightColor", light._color.Xyz);
                _shader.SetVector3("viewPos", cameraPos);
            }

            if (_indices.Count != 0)
            {
                switch (line)
                {
                    case 1:
                        GL.DrawElements(PrimitiveType.Triangles, _indices.Count, DrawElementsType.UnsignedInt, 0);
                        break;
                    case -1:
                        GL.DrawElements(PrimitiveType.LineStrip, _indices.Count, DrawElementsType.UnsignedInt, 0);
                        break;
                }
            }
            else
            {
                switch (line)
                {
                    case 1:
                        GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count);
                        break;
                    case -1:
                        GL.DrawArrays(PrimitiveType.LineStrip, 0, _vertices.Count);
                        break;
                }
            }

            foreach (var i in Child)
            {
                i.render2(line, camera_view, camera_projection, cameraPos, light);
            }
        }

        public void createTrapezoidHeadVertices(float x, float y, float z, float length, bool useNormals)
        {
            var tempVertices = new List<Vector3>();
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 4.0f;
            temp_vector.Y = y + length / 8.0f;
            temp_vector.Z = z - length / 4.0f;
            tempVertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 4.0f;
            temp_vector.Y = y + length / 8.0f;
            temp_vector.Z = z - length / 4.0f;
            tempVertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z - length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z - length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 4.0f;
            temp_vector.Y = y + length / 8.0f;
            temp_vector.Z = z + length / 4.0f;
            tempVertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 4.0f;
            temp_vector.Y = y + length / 8.0f;
            temp_vector.Z = z + length / 4.0f;
            tempVertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z + length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z + length / 2.0f;
            tempVertices.Add(temp_vector);

            var tempIndices = new List<uint>
            {
				//Back
				1, 2, 0,
                2, 1, 3,
				
				//Top
				5, 0, 4,
                0, 5, 1,

				//Right
				5, 3, 1,
                3, 5, 7,

				//Left
				0, 6, 4,
                6, 0, 2,

				//Front
				4, 7, 5,
                7, 4, 6,

				//Bottom
				3, 6, 2,
                6, 3, 7
            };

            if (useNormals)
            {
                for (int i = 0; i < tempIndices.Count; i++)
                {
                    _vertices.Add(tempVertices[(int)tempIndices[i]]);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitZ);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitY);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitX);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitX);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitZ);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitY);
                }
            }
        }

        public void createTrapezoidBodyVertices(float x, float y, float z, float length, bool useNormals)
        {
            var tempVertices = new List<Vector3>();
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length;
            tempVertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length;
            tempVertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length;
            tempVertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length;
            tempVertices.Add(temp_vector);

            var tempIndices = new List<uint>
            {
				//Back
				1, 2, 0,
                2, 1, 3,
				
				//Top
				5, 0, 4,
                0, 5, 1,

				//Right
				5, 3, 1,
                3, 5, 7,

				//Left
				0, 6, 4,
                6, 0, 2,

				//Front
				4, 7, 5,
                7, 4, 6,

				//Bottom
				3, 6, 2,
                6, 3, 7
            };

            if (useNormals)
            {
                for (int i = 0; i < tempIndices.Count; i++)
                {
                    _vertices.Add(tempVertices[(int)tempIndices[i]]);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitZ);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitY);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitX);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitX);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitZ);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitY);
                }
            }
        }

        public void createRock(float x, float y, float z, float length, bool useNormals)
        {
            var tempVertices = new List<Vector3>();
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 6.0f;
            temp_vector.Z = z - length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z - length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 1.5f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 1.5f;
            tempVertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 1.5f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 1.5f;
            tempVertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 1.5f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 1.5f;
            tempVertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 1.5f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 1.5f;
            tempVertices.Add(temp_vector);

            var tempIndices = new List<uint>
            {
				//Back
				1, 2, 0,
                2, 1, 3,
				
				//Top
				5, 0, 4,
                0, 5, 1,

				//Right
				5, 3, 1,
                3, 5, 7,

				//Left
				0, 6, 4,
                6, 0, 2,

				//Front
				4, 7, 5,
                7, 4, 6,

				//Bottom
				3, 6, 2,
                6, 3, 7
            };

            if (useNormals)
            {
                for (int i = 0; i < tempIndices.Count; i++)
                {
                    _vertices.Add(tempVertices[(int)tempIndices[i]]);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitZ);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitY);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitX);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitX);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitZ);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitY);
                }
            }
        }

        public void createLongBoxVertices(float x, float y, float z, float length, bool useNormals)
        {
            var tempVertices = new List<Vector3>();
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length * 2.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 4.0f;
            tempVertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length * 2.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 4.0f;
            tempVertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length * 2.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z - length / 4.0f;
            tempVertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length * 2.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z - length / 4.0f;
            tempVertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length * 2.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 4.0f;
            tempVertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length * 2.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 4.0f;
            tempVertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length * 2.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z + length / 4.0f;
            tempVertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length * 2.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z + length / 4.0f;
            tempVertices.Add(temp_vector);

            var tempIndices = new List<uint>
            {
				//Back
				1, 2, 0,
                2, 1, 3,
				
				//Top
				5, 0, 4,
                0, 5, 1,

				//Right
				5, 3, 1,
                3, 5, 7,

				//Left
				0, 6, 4,
                6, 0, 2,

				//Front
				4, 7, 5,
                7, 4, 6,

				//Bottom
				3, 6, 2,
                6, 3, 7
            };

            if (useNormals)
            {
                for (int i = 0; i < tempIndices.Count; i++)
                {
                    _vertices.Add(tempVertices[(int)tempIndices[i]]);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitZ);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitY);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitX);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitX);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitZ);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitY);
                }
            }
        }

        public void createVerticalLongBoxVertices(float x, float y, float z, float length, bool useNormals)
        {
            var tempVertices = new List<Vector3>();
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 4.0f;
            temp_vector.Y = y + length * 2.0f;
            temp_vector.Z = z - length / 4.0f;
            tempVertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 4.0f;
            temp_vector.Y = y + length * 2.0f;
            temp_vector.Z = z - length / 4.0f;
            tempVertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 4.0f;
            temp_vector.Y = y - length * 2.0f;
            temp_vector.Z = z - length / 4.0f;
            tempVertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 4.0f;
            temp_vector.Y = y - length * 2.0f;
            temp_vector.Z = z - length / 4.0f;
            tempVertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 4.0f;
            temp_vector.Y = y + length * 2.0f;
            temp_vector.Z = z + length / 4.0f;
            tempVertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 4.0f;
            temp_vector.Y = y + length * 2.0f;
            temp_vector.Z = z + length / 4.0f;
            tempVertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 4.0f;
            temp_vector.Y = y - length * 2.0f;
            temp_vector.Z = z + length / 4.0f;
            tempVertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 4.0f;
            temp_vector.Y = y - length * 2.0f;
            temp_vector.Z = z + length / 4.0f;
            tempVertices.Add(temp_vector);

            var tempIndices = new List<uint>
            {
				//Back
				1, 2, 0,
                2, 1, 3,
				
				//Top
				5, 0, 4,
                0, 5, 1,

				//Right
				5, 3, 1,
                3, 5, 7,

				//Left
				0, 6, 4,
                6, 0, 2,

				//Front
				4, 7, 5,
                7, 4, 6,

				//Bottom
				3, 6, 2,
                6, 3, 7
            };

            if (useNormals)
            {
                for (int i = 0; i < tempIndices.Count; i++)
                {
                    _vertices.Add(tempVertices[(int)tempIndices[i]]);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitZ);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitY);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitX);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitX);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitZ);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitY);
                }
            }
        }

        public void createFlatLongBoxVertices(float x, float y, float z, float length)
        {
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length * 2.0f;
            temp_vector.Y = y + length / 16.0f;
            temp_vector.Z = z - length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length * 2.0f;
            temp_vector.Y = y + length / 16.0f;
            temp_vector.Z = z - length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length * 2.0f;
            temp_vector.Y = y - length / 16.0f;
            temp_vector.Z = z - length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length * 2.0f;
            temp_vector.Y = y - length / 16.0f;
            temp_vector.Z = z - length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length * 2.0f;
            temp_vector.Y = y + length / 16.0f;
            temp_vector.Z = z + length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length * 2.0f;
            temp_vector.Y = y + length / 16.0f;
            temp_vector.Z = z + length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length * 2.0f;
            temp_vector.Y = y - length / 16.0f;
            temp_vector.Z = z + length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length * 2.0f;
            temp_vector.Y = y - length / 16.0f;
            temp_vector.Z = z + length / 4.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }

        public void createFlatBoxVertices(float x, float y, float z, float length, bool useNormals)
        {
            var tempVertices = new List<Vector3>();
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 16.0f;
            temp_vector.Z = z - length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 16.0f;
            temp_vector.Z = z - length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 6.0f;
            temp_vector.Z = z - length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 6.0f;
            temp_vector.Z = z - length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 16.0f;
            temp_vector.Z = z + length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 16.0f;
            temp_vector.Z = z + length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 6.0f;
            temp_vector.Z = z + length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 6.0f;
            temp_vector.Z = z + length / 2.0f;
            tempVertices.Add(temp_vector);

            var tempIndices = new List<uint>
            {
				//Back
				1, 2, 0,
                2, 1, 3,
				
				//Top
				5, 0, 4,
                0, 5, 1,

				//Right
				5, 3, 1,
                3, 5, 7,

				//Left
				0, 6, 4,
                6, 0, 2,

				//Front
				4, 7, 5,
                7, 4, 6,

				//Bottom
				3, 6, 2,
                6, 3, 7
            };

            if (useNormals)
            {
                for (int i = 0; i < tempIndices.Count; i++)
                {
                    _vertices.Add(tempVertices[(int)tempIndices[i]]);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitZ);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitY);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitX);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitX);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitZ);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitY);
                }
            }
        }

        public void createBackPrism(float x, float y, float z, float length)
        {
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 64.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 8.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 64.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 8.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 64.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z - length / 8.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 64.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z - length / 8.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 64.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 8.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 64.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 8.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 64.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z + length / 8.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 64.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z + length / 8.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }

        public void createFrontPrism(float x, float y, float z, float length)
        {
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 64.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 8.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 64.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 8.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 64.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z - length / 8.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 64.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z - length / 8.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 64.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 8.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 64.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 8.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 64.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z + length / 8.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 64.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z + length / 8.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }

        public void createRightPrism(float x, float y, float z, float length)
        {
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 8.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 64.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x - length / 8.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 64.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 8.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z - length / 64.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 8.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z - length / 64.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 8.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 64.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x - length / 8.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 64.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 8.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z + length / 64.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 8.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z + length / 64.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }

        public void createLeftPrism(float x, float y, float z, float length)
        {
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x + length / 8.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 64.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 8.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 64.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 8.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z - length / 64.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 8.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z - length / 64.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x + length / 8.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 64.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 8.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 64.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 8.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z + length / 64.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 8.0f;
            temp_vector.Y = y - length / 4.0f;
            temp_vector.Z = z + length / 64.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }

        public void createBoxVertices(float x, float y, float z, float length, bool useNormals)
        {
            var tempVertices = new List<Vector3>();
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            tempVertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            tempVertices.Add(temp_vector);

            var tempIndices = new List<uint>
            {
				//Back
				1, 2, 0,
                2, 1, 3,
				
				//Top
				5, 0, 4,
                0, 5, 1,

				//Right
				5, 3, 1,
                3, 5, 7,

				//Left
				0, 6, 4,
                6, 0, 2,

				//Front
				4, 7, 5,
                7, 4, 6,

				//Bottom
				3, 6, 2,
                6, 3, 7
            };

            if (useNormals)
            {
                for (int i = 0; i < tempIndices.Count; i++)
                {
                    _vertices.Add(tempVertices[(int)tempIndices[i]]);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitZ);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitY);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitX);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitX);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitZ);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitY);
                }
            }
        }

        public void createBoxVertices2(float x, float y, float z, float length)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //FRONT FACE

            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, 0.0f, -1.0f));

            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, 0.0f, -1.0f));

            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, 0.0f, -1.0f));

            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, 0.0f, -1.0f));

            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, 0.0f, -1.0f));

            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, 0.0f, -1.0f));

            //BACK FACE
            //TITIK 5
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, 0.0f, 1.0f));

            //TITIK 6
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, 0.0f, 1.0f));
            //TITIK 7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, 0.0f, 1.0f));

            //TITIK 6
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, 0.0f, 1.0f));
            //TITIK 7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, 0.0f, 1.0f));

            //TITIK 8
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, 0.0f, 1.0f));

            //LEFT FACE
            //TITIK 1
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(-1.0f, 0.0f, 0.0f));
            //TITIK 3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(-1.0f, 0.0f, 0.0f));
            //TITIK 5
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(-1.0f, 0.0f, 0.0f));
            //TITIK 3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(-1.0f, 0.0f, 0.0f));
            //TITIK 5
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(-1.0f, 0.0f, 0.0f));
            //TITIK 7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(-1.0f, 0.0f, 0.0f));

            //RIGHT FACE
            //TITIK 2
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(1.0f, 0.0f, 0.0f));
            //TITIK 4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(1.0f, 0.0f, 0.0f));
            //TITIK 6
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(1.0f, 0.0f, 0.0f));
            //TITIK 4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(1.0f, 0.0f, 0.0f));
            //TITIK 6
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(1.0f, 0.0f, 0.0f));
            //TITIK 8
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(1.0f, 0.0f, 0.0f));

            //BOTTOM FACES
            //TITIK 3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, -1.0f, 0.0f));
            //TITIK 4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, -1.0f, 0.0f));
            //TITIK 7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, -1.0f, 0.0f));
            //TITIK 4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, -1.0f, 0.0f));
            //TITIK 7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, -1.0f, 0.0f));
            //TITIK 8
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, -1.0f, 0.0f));

            //TOP FACES
            //TITIK 1
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, 1.0f, 0.0f));
            //TITIK 2
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, 1.0f, 0.0f));
            //TITIK 5
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, 1.0f, 0.0f));
            //TITIK 2
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, 1.0f, 0.0f));
            //TITIK 5
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, 1.0f, 0.0f));
            //TITIK 6
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            _vertices.Add(new Vector3(0.0f, 1.0f, 0.0f));
        }

        public void createCuboid(float x_, float y_, float z_, float length, bool useNormals, bool useTextures)
        {
            _centerPosition = new Vector3(x_, y_, z_);

            var tempVertices = new List<Vector3>();
            Vector3 temp_vector;

            //Titik 1
            temp_vector.X = x_ - length / 2.0f;
            temp_vector.Y = y_ + length / 2.0f;
            temp_vector.Z = z_ - length / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 2
            temp_vector.X = x_ + length / 2.0f;
            temp_vector.Y = y_ + length / 2.0f;
            temp_vector.Z = z_ - length / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 3
            temp_vector.X = x_ - length / 2.0f;
            temp_vector.Y = y_ - length / 2.0f;
            temp_vector.Z = z_ - length / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 4
            temp_vector.X = x_ + length / 2.0f;
            temp_vector.Y = y_ - length / 2.0f;
            temp_vector.Z = z_ - length / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 5
            temp_vector.X = x_ - length / 2.0f;
            temp_vector.Y = y_ + length / 2.0f;
            temp_vector.Z = z_ + length / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 6
            temp_vector.X = x_ + length / 2.0f;
            temp_vector.Y = y_ + length / 2.0f;
            temp_vector.Z = z_ + length / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 7
            temp_vector.X = x_ - length / 2.0f;
            temp_vector.Y = y_ - length / 2.0f;
            temp_vector.Z = z_ + length / 2.0f;
            tempVertices.Add(temp_vector);

            //Titik 8
            temp_vector.X = x_ + length / 2.0f;
            temp_vector.Y = y_ - length / 2.0f;
            temp_vector.Z = z_ + length / 2.0f;
            tempVertices.Add(temp_vector);

            var tempIndices = new List<uint>
            {
				//Back
				1, 2, 0,
                2, 1, 3,
				
				//Top
				5, 0, 4,
                0, 5, 1,

				//Right
				5, 3, 1,
                3, 5, 7,

				//Left
				0, 6, 4,
                6, 0, 2,

				//Front
				4, 7, 5,
                7, 4, 6,

				//Bottom
				3, 6, 2,
                6, 3, 7
            };

            if (useNormals)
            {
                for (int i = 0; i < tempIndices.Count; i++)
                {
                    _vertices.Add(tempVertices[(int)tempIndices[i]]);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitZ);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitY);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitX);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitX);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(Vector3.UnitZ);
                }

                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-Vector3.UnitY);
                }
            }

            if (useTextures)
            {
                for (int i = 0; i < tempIndices.Count; i++)
                {
                    _vertices.Add(tempVertices[(int)tempIndices[i]]);
                }

                texCoords = new List<Vector3>()
                {
                    (0, 1.0f, 0),
                    (1.0f, 0, 0),
                    (1.0f, 1.0f, 0),
                    (1.0f, 0, 0),
                    (0, 1.0f, 0),
                    (0, 0, 0),

                    (1.0f, 0, 0),
                    (0, 1.0f, 0),
                    (0, 0, 0),
                    (0, 1.0f, 0),
                    (1.0f, 0, 0),
                    (1.0f, 1.0f, 0),

                    (0, 1.0f, 0),
                    (1.0f, 0, 0),
                    (1.0f, 1.0f, 0),
                    (1.0f, 0, 0),
                    (0, 1.0f, 0),
                    (0, 0, 0),

                    (0, 1.0f, 0),
                    (1.0f, 0, 0),
                    (1.0f, 1.0f, 0),
                    (1.0f, 0, 0),
                    (0, 1.0f, 0),
                    (0, 0, 0),

                    (0, 1.0f, 0),
                    (1.0f, 0, 0),
                    (1.0f, 1.0f, 0),
                    (1.0f, 0, 0),
                    (0, 1.0f, 0),
                    (0, 0, 0),

                    (1.0f, 0, 0),
                    (0, 1.0f, 0),
                    (0, 0, 0),
                    (0, 1.0f, 0),
                    (1.0f, 0, 0),
                    (1.0f, 1.0f, 0)
                };
            }

            if (!useNormals && !useTextures)
            {
                _vertices = tempVertices;
                _indices = tempIndices;
            }
        }

        public void createEllipsoid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for (float u = -pi; u<=pi; u+= pi / 300)
            {
                for (float v = -pi / 2; v <= pi / 2; v += pi / 300)
                {
                    temp_vector.X = _x + (float)Math.Cos(v) * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + (float)Math.Cos(v) * (float)Math.Sin(u) * radiusY;
                    temp_vector.Z = _z + (float)Math.Sin(v) * radiusZ;
                    _vertices.Add(temp_vector);
                }
            }
        }

        public void createWheels(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z, int sectorCount, int stackCount)
        {
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            float sectorStep = 2 * (float)Math.PI / sectorCount;
            float stackStep = (float)Math.PI / stackCount;
            float sectorAngle, StackAngle, x, y, z;

            for (int i = 0; i <= stackCount; ++i)
            {
                StackAngle = pi / 2 - i * stackStep;
                x = radiusX * (float)Math.Cos(StackAngle);
                y = radiusY * (float)Math.Cos(StackAngle);
                z = radiusZ * (i / 2);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;

                    temp_vector.X = _x + x * (float)Math.Cos(sectorAngle);
                    temp_vector.Y = _y + y * (float)Math.Sin(sectorAngle);
                    temp_vector.Z = _z + z;
                    _vertices.Add(temp_vector);
                }
            }

            uint k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);
                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        _indices.Add(k1);
                        _indices.Add(k2);
                        _indices.Add(k1 + 1);
                    }
                    if (i != stackCount - 1)
                    {
                        _indices.Add(k1 + 1);
                        _indices.Add(k2);
                        _indices.Add(k2 + 1);
                    }
                }
            }
        }

        public void createTube(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z, int sectorCount, int stackCount)
        {
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            float sectorStep = 2 * (float)Math.PI / sectorCount;
            float stackStep = (float)Math.PI / stackCount;
            float sectorAngle, StackAngle, x, y, z;

            for (int i = 0; i <= stackCount; ++i)
            {
                StackAngle = pi / 2 - i * stackStep;
                x = radiusX * (float)Math.Cos(StackAngle);
                y = radiusY * (i / 2);
                z = radiusZ * (float)Math.Cos(StackAngle);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;

                    temp_vector.X = _x + x * (float)Math.Cos(sectorAngle);
                    temp_vector.Y = _y + y;
                    temp_vector.Z = _z + z * (float)Math.Sin(sectorAngle);
                    _vertices.Add(temp_vector);
                }
            }

            uint k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);
                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        _indices.Add(k1);
                        _indices.Add(k2);
                        _indices.Add(k1 + 1);
                    }
                    if (i != stackCount - 1)
                    {
                        _indices.Add(k1 + 1);
                        _indices.Add(k2);
                        _indices.Add(k2 + 1);
                    }
                }
            }
        }

        public void createCone(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z, int sectorCount, int stackCount)
        {
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            float sectorStep = 2 * (float)Math.PI / sectorCount;
            float stackStep = (float)Math.PI / stackCount;
            float sectorAngle, StackAngle, x, y, z;

            for (int i = 0; i <= stackCount; ++i)
            {
                StackAngle = pi / 2 - i * stackStep;
                x = radiusX * (float)Math.Cos(StackAngle);
                y = radiusY * (float)Math.Cos(StackAngle);
                z = radiusZ * (float)Math.Cos(StackAngle);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;

                    temp_vector.X = _x + x * (float)Math.Cos(sectorAngle);
                    temp_vector.Y = _y - y;
                    temp_vector.Z = _z + z * (float)Math.Sin(sectorAngle);
                    _vertices.Add(temp_vector);
                }
            }

            uint k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);
                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        _indices.Add(k1);
                        _indices.Add(k2);
                        _indices.Add(k1 + 1);
                    }
                    if (i != stackCount - 1)
                    {
                        _indices.Add(k1 + 1);
                        _indices.Add(k2);
                        _indices.Add(k2 + 1);
                    }
                }
            }
        }

        public void createEllipsoid2(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z, int sectorCount, int stackCount)
        {
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            float sectorStep = 2 * (float)Math.PI / sectorCount;
            float stackStep = (float)Math.PI / stackCount;
            float sectorAngle, StackAngle, x, y, z;

            for (int i = 0; i <= stackCount; ++i)
            {
                StackAngle = pi / 2 - i * stackStep;
                x = radiusX * (float)Math.Cos(StackAngle);
                y = radiusY * (float)Math.Cos(StackAngle);
                z = radiusZ * (float)Math.Sin(StackAngle);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;

                    temp_vector.X = _x + x * (float)Math.Cos(sectorAngle);
                    temp_vector.Y = _y + y * (float)Math.Sin(sectorAngle);
                    temp_vector.Z = _z + z;
                    _vertices.Add(temp_vector);
                }
            }

            uint k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);
                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        _indices.Add(k1);
                        _indices.Add(k2);
                        _indices.Add(k1 + 1);
                    }
                    if (i != stackCount - 1)
                    {
                        _indices.Add(k1 + 1);
                        _indices.Add(k2);
                        _indices.Add(k2 + 1);
                    }
                }
            }
        }

        public void rotatede(Vector3 pivot, Vector3 vector, float angle)
        {
            var radAngle = MathHelper.DegreesToRadians(angle);

            var arbRotationMatrix = new Matrix4
                (
                new Vector4((float)(Math.Cos(radAngle) + Math.Pow(vector.X, 2.0f) * (1.0f - Math.Cos(radAngle))), (float)(vector.X * vector.Y * (1.0f - Math.Cos(radAngle)) + vector.Z * Math.Sin(radAngle)), (float)(vector.X * vector.Z * (1.0f - Math.Cos(radAngle)) - vector.Y * Math.Sin(radAngle)), 0),
                new Vector4((float)(vector.X * vector.Y * (1.0f - Math.Cos(radAngle)) - vector.Z * Math.Sin(radAngle)), (float)(Math.Cos(radAngle) + Math.Pow(vector.Y, 2.0f) * (1.0f - Math.Cos(radAngle))), (float)(vector.Y * vector.Z * (1.0f - Math.Cos(radAngle)) + vector.X * Math.Sin(radAngle)), 0),
                new Vector4((float)(vector.X * vector.Z * (1.0f - Math.Cos(radAngle)) + vector.Y * Math.Sin(radAngle)), (float)(vector.Y * vector.Z * (1.0f - Math.Cos(radAngle)) - vector.X * Math.Sin(radAngle)), (float)(Math.Cos(radAngle) + Math.Pow(vector.Z, 2.0f) * (1.0f - Math.Cos(radAngle))), 0),
                Vector4.UnitW
                );

            _model *= Matrix4.CreateTranslation(-pivot);
            _model *= arbRotationMatrix;
            _model *= Matrix4.CreateTranslation(pivot);

            for (int i = 0; i < 3; i++)
            {
                _euler[i] = Vector3.Normalize(getRotationResult(pivot, vector, radAngle, _euler[i], true));
            }

            _centerPosition = getRotationResult(pivot, vector, radAngle, _centerPosition);


            foreach (var i in Child)
            {
                i.rotate(pivot, vector, angle);
            }
        }

        public void rotate(Vector3 pivot, Vector3 vector, float angle)
        {
            //pivot -> mau rotate di titik mana
            //vector -> mau rotate di sumbu apa? (x,y,z)
            //angle -> rotatenya berapa derajat?

            angle = MathHelper.DegreesToRadians(angle);

            //mulai ngerotasi
            for (int i = 0; i < _vertices.Count; i++)
            {
                _vertices[i] = getRotationResult(pivot, vector, angle, _vertices[i]);
            }
            //rotate the euler direction
            for (int i = 0; i < 3; i++)
            {
                _euler[i] = getRotationResult(pivot, vector, angle, _euler[i], true);

                //NORMALIZE
                //LANGKAH - LANGKAH
                //length = akar(x^2+y^2+z^2)
                float length = (float)Math.Pow(Math.Pow(_euler[i].X, 2.0f) + Math.Pow(_euler[i].Y, 2.0f) + Math.Pow(_euler[i].Z, 2.0f), 0.5f);
                Vector3 temporary = new Vector3(0, 0, 0);
                temporary.X = _euler[i].X / length;
                temporary.Y = _euler[i].Y / length;
                temporary.Z = _euler[i].Z / length;
                _euler[i] = temporary;
            }
            _centerPosition = getRotationResult(pivot, vector, angle, _centerPosition);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vector3.SizeInBytes,
                _vertices.ToArray(), BufferUsageHint.StaticDraw);
        }

        Vector3 getRotationResult(Vector3 pivot, Vector3 vector, float angle, Vector3 point, bool isEuler = false)
        {
            Vector3 temp, newPosition;
            if (isEuler)
            {
                temp = point;
            }
            else
            {
                temp = point - pivot;
            }

            newPosition.X =
                temp.X * (float)(Math.Cos(angle) + Math.Pow(vector.X, 2.0f) * (1.0f - Math.Cos(angle))) +
                temp.Y * (float)(vector.X * vector.Y * (1.0f - Math.Cos(angle)) - vector.Z * Math.Sin(angle)) +
                temp.Z * (float)(vector.X * vector.Z * (1.0f - Math.Cos(angle)) + vector.Y * Math.Sin(angle));
            newPosition.Y =
                temp.X * (float)(vector.X * vector.Y * (1.0f - Math.Cos(angle)) + vector.Z * Math.Sin(angle)) +
                temp.Y * (float)(Math.Cos(angle) + Math.Pow(vector.Y, 2.0f) * (1.0f - Math.Cos(angle))) +
                temp.Z * (float)(vector.Y * vector.Z * (1.0f - Math.Cos(angle)) - vector.X * Math.Sin(angle));
            newPosition.Z =
                temp.X * (float)(vector.X * vector.Z * (1.0f - Math.Cos(angle)) - vector.Y * Math.Sin(angle)) +
                temp.Y * (float)(vector.Y * vector.Z * (1.0f - Math.Cos(angle)) + vector.X * Math.Sin(angle)) +
                temp.Z * (float)(Math.Cos(angle) + Math.Pow(vector.Z, 2.0f) * (1.0f - Math.Cos(angle)));

            if (isEuler)
            {
                temp = newPosition;
            }
            else
            {
                temp = newPosition + pivot;
            }
            return temp;
        }

        public void setDirectionalLight(Vector3 direction, Vector3 ambient, Vector3 diffuse, Vector3 specular)
        {
            _shader.SetVector3("dirLight.direction", direction);
            _shader.SetVector3("dirLight.ambient", ambient);
            _shader.SetVector3("dirLight.diffuse", diffuse);
            _shader.SetVector3("dirLight.specular", specular);
        }
        public void setPointLight(Vector3 position, Vector3 ambient, Vector3 diffuse, Vector3 specular, float constant, float linear, float quadratic)
        {
            _shader.SetVector3("pointLight.position", position);
            _shader.SetVector3("pointLight.ambient", ambient);
            _shader.SetVector3("pointLight.diffuse", diffuse);
            _shader.SetVector3("pointLight.specular", specular);
            _shader.SetFloat("pointLight.constant", constant);
            _shader.SetFloat("pointLight.linear", linear);
            _shader.SetFloat("pointLight.quadratic", quadratic);
        }

        public void setSpotLight(Vector3 position, Vector3 direction, Vector3 ambient, Vector3 diffuse, Vector3 specular, float constant, float linear, float quadratic, float cutOff, float outerCutOff)
        {
            _shader.SetVector3("spotLight.position", position);
            _shader.SetVector3("spotLight.direction", direction);
            _shader.SetVector3("spotLight.ambient", ambient);
            _shader.SetVector3("spotLight.diffuse", diffuse);

            _shader.SetVector3("spotLight.specular", specular);
            _shader.SetFloat("spotLight.constant", constant);
            _shader.SetFloat("spotLight.linear", linear);
            _shader.SetFloat("spotLight.quadratic", quadratic);
            _shader.SetFloat("spotLight.cutOff", cutOff);
            _shader.SetFloat("spotLight.outerCutOff", outerCutOff);
        }

        public void setPointLights(Vector3[] position, Vector3 ambient, Vector3 diffuse, Vector3 specular, float constant, float linear, float quadratic)
        {
            for (int i = 0; i < position.Length; i++)
            {
                _shader.SetVector3($"pointLight[{i}].position", position[i]);
                _shader.SetVector3($"pointLight[{i}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
                _shader.SetVector3($"pointLight[{i}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
                _shader.SetVector3($"pointLight[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
                _shader.SetFloat($"pointLight[{i}].constant", 1.0f);
                _shader.SetFloat($"pointLight[{i}].linear", 0.09f);
                _shader.SetFloat($"pointLight[{i}].quadratic", 0.032f);
            }

        }
        public void resetEuler()
        {
            _euler[0] = new Vector3(1, 0, 0);
            _euler[1] = new Vector3(0, 1, 0);
            _euler[2] = new Vector3(0, 0, 1);
        }

        public void addChild(float x, float y, float z, float length)
        {
            Asset3d newChild = new Asset3d();
        }
    }
}