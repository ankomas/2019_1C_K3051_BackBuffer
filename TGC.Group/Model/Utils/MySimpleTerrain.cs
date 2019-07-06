using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Shaders;
using TGC.Core.Terrain;
using TGC.Core.Textures;

namespace TGC.Group.Model.UI
{
    public class MySimpleTerrain
    {
        protected Effect effect;
        protected string technique;
        private Texture terrainTexture;
        private int totalVertices;
        private VertexBuffer vbTerrain;
        private CustomVertex.PositionTextured[] data;
        private float yScale;

        public MySimpleTerrain()
        {
            this.Enabled = true;
            this.AlphaBlendEnable = false;
            this.effect = TGCShaders.Instance.VariosShader;
            this.technique = "PositionTextured";
        }

        /// <summary>
        ///     Devuelve la informacion de Custom Vertex Buffer del HeightMap cargado
        /// </summary>
        /// <returns>Custom Vertex Buffer de tipo PositionTextured</returns>
        public CustomVertex.PositionTextured[] getData()
        {
            return this.data;
        }

        /// <summary>Valor de Y para cada par (X,Z) del Heightmap</summary>
        public int[,] HeightmapData { get; private set; }


        public float scaled(int x, int y)
        {
            return HeightmapData[x, HeightmapData.GetLength(1)-1-y] * this.yScale;
        }
        public float[,] ScaledHeghtmapData
        {
            get
            {
                float[,] res = new float[HeightmapData.GetLength(0), HeightmapData.GetLength(1)];
                for (int x = 0; x < HeightmapData.GetLength(0); x++)
                {
                    for (int y = 0; y < HeightmapData.GetLength(1); y++)
                    {
                        res[x, y] = HeightmapData[x, HeightmapData.GetLength(1)-1-y] * this.yScale;
                    }
                }

                return res;
            }
        }

        /// <summary>
        ///     Indica si la malla esta habilitada para ser renderizada
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>Centro del terreno</summary>
        public TGCVector3 Center { get; private set; }

        /// <summary>Shader del mesh</summary>
        public Effect Effect
        {
            get { return this.effect; }
            set { this.effect = value; }
        }

        /// <summary>
        ///     Technique que se va a utilizar en el effect.
        ///     Cada vez que se llama a Render() se carga este Technique (pisando lo que el shader ya tenia seteado)
        /// </summary>
        public string Technique
        {
            get { return this.technique; }
            set { this.technique = value; }
        }

        public TGCVector3 Position
        {
            get { return this.Center; }
        }

        /// <summary>
        ///     Habilita el renderizado con AlphaBlending para los modelos
        ///     con textura o colores por vértice de canal Alpha.
        ///     Por default está deshabilitado.
        /// </summary>
        public bool AlphaBlendEnable { get; set; }

        /// <summary>Renderiza el terreno</summary>
        public void Render()
        {
            if (!this.Enabled)
                return;
            this.effect.SetValue((EffectHandle) "texDiffuseMap", (BaseTexture) this.terrainTexture);
            TexturesManager.Instance.clear(1);
            TGCShaders.Instance.SetShaderMatrix(this.effect, TGCMatrix.Identity);
            D3DDevice.Instance.Device.VertexDeclaration = TGCShaders.Instance.VdecPositionTextured;
            this.effect.Technique = (EffectHandle) this.technique;
            D3DDevice.Instance.Device.SetStreamSource(0, this.vbTerrain, 0);
            this.effect.Begin(FX.None);
            this.effect.BeginPass(0);
            D3DDevice.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, this.totalVertices / 3);
            this.effect.EndPass();
            this.effect.End();
        }

        /// <summary>Libera los recursos del Terreno</summary>
        public void Dispose()
        {
            if ((Resource) this.vbTerrain != (Resource) null)
                this.vbTerrain.Dispose();
            if (!(this.terrainTexture != (Texture) null))
                return;
            this.terrainTexture.Dispose();
        }

        /// <summary>
        ///     Crea la malla de un terreno en base a un Heightmap
        /// </summary>
        /// <param name="heightmapPath">Imagen de Heightmap</param>
        /// <param name="scaleXZ">Escala para los ejes X y Z</param>
        /// <param name="scaleY">Escala para el eje Y</param>
        /// <param name="center">Centro de la malla del terreno</param>
        public void loadHeightmap(
            Bitmap heightmap,
            float scaleXZ,
            float scaleY,
            TGCVector3 center)
        {
            this.Center = center;
            this.yScale = scaleY;
            
            if ((Resource) this.vbTerrain != (Resource) null && !this.vbTerrain.Disposed)
                this.vbTerrain.Dispose();
            
            this.HeightmapData = this.loadHeightMap(heightmap);
            
            float length1 = (float) this.HeightmapData.GetLength(0);
            float length2 = (float) this.HeightmapData.GetLength(1);
            
            this.totalVertices = 6 * (this.HeightmapData.GetLength(0) - 1) * (this.HeightmapData.GetLength(1) - 1);
            
            this.vbTerrain = new VertexBuffer(typeof(CustomVertex.PositionTextured), this.totalVertices,
                D3DDevice.Instance.Device, Usage.Dynamic | Usage.WriteOnly,
                VertexFormats.Texture1 | VertexFormats.Position, Pool.Default);
            
            int index1 = 0;
            
            this.data = new CustomVertex.PositionTextured[this.totalVertices];
            
            //center.X = (float) ((double) center.X * (double) scaleXZ - (double) length1 / 2.0 * (double) scaleXZ);
            //center.Z = (float) ((double) center.Z * (double) scaleXZ - (double) length2 / 2.0 * (double) scaleXZ);
            
            for (int index2 = 0; (double) index2 < (double) length1 - 1.0; ++index2)
            {
                for (int index3 = 0; (double) index3 < (double) length2 - 1.0; ++index3)
                {
                    TGCVector3 tgcVector3_1 = new TGCVector3(center.X + (float) index2 * scaleXZ,
                        center.Y + (float) this.HeightmapData[index2, index3] * scaleY,
                        center.Z + (float) index3 * scaleXZ);
                    TGCVector3 tgcVector3_2 = new TGCVector3(center.X + (float) index2 * scaleXZ,
                        center.Y + (float) this.HeightmapData[index2, index3 + 1] * scaleY,
                        center.Z + (float) (index3 + 1) * scaleXZ);
                    TGCVector3 tgcVector3_3 = new TGCVector3(center.X + (float) (index2 + 1) * scaleXZ,
                        center.Y + (float) this.HeightmapData[index2 + 1, index3] * scaleY,
                        center.Z + (float) index3 * scaleXZ);
                    TGCVector3 tgcVector3_4 = new TGCVector3(center.X + (float) (index2 + 1) * scaleXZ,
                        center.Y + (float) this.HeightmapData[index2 + 1, index3 + 1] * scaleY,
                        center.Z + (float) (index3 + 1) * scaleXZ);
                    TGCVector2 tgcVector2_1 = new TGCVector2((float) index2 / length1, (float) index3 / length2);
                    TGCVector2 tgcVector2_2 = new TGCVector2((float) index2 / length1, (float) (index3 + 1) / length2);
                    TGCVector2 tgcVector2_3 = new TGCVector2((float) (index2 + 1) / length1, (float) index3 / length2);
                    TGCVector2 tgcVector2_4 =
                        new TGCVector2((float) (index2 + 1) / length1, (float) (index3 + 1) / length2);
                    this.data[index1] =
                        new CustomVertex.PositionTextured((Vector3) tgcVector3_1, tgcVector2_1.X, tgcVector2_1.Y);
                    this.data[index1 + 1] =
                        new CustomVertex.PositionTextured((Vector3) tgcVector3_2, tgcVector2_2.X, tgcVector2_2.Y);
                    this.data[index1 + 2] =
                        new CustomVertex.PositionTextured((Vector3) tgcVector3_4, tgcVector2_4.X, tgcVector2_4.Y);
                    this.data[index1 + 3] =
                        new CustomVertex.PositionTextured((Vector3) tgcVector3_1, tgcVector2_1.X, tgcVector2_1.Y);
                    this.data[index1 + 4] =
                        new CustomVertex.PositionTextured((Vector3) tgcVector3_4, tgcVector2_4.X, tgcVector2_4.Y);
                    this.data[index1 + 5] =
                        new CustomVertex.PositionTextured((Vector3) tgcVector3_3, tgcVector2_3.X, tgcVector2_3.Y);
                    index1 += 6;
                }
            }

            this.vbTerrain.SetData((object) this.data, 0, LockFlags.None);
        }

        /// <summary>Carga la textura del terreno</summary>
        public void loadTexture(string path)
        {
            if (this.terrainTexture != (Texture) null && !this.terrainTexture.Disposed)
                this.terrainTexture.Dispose();
            Bitmap image = (Bitmap) Image.FromFile(path);
            image.RotateFlip(RotateFlipType.Rotate90FlipX);
            this.terrainTexture =
                Texture.FromBitmap(D3DDevice.Instance.Device, image, Usage.AutoGenerateMipMap, Pool.Managed);
        }

        public void loadTexture(Texture texture)
        {
            this.terrainTexture = texture;
        }

        /// <summary>Carga los valores del Heightmap en una matriz</summary>
        protected int[,] loadHeightMap(Bitmap bitmap)
        {
            int width = bitmap.Size.Width;
            int height = bitmap.Size.Height;
            int[,] numArray = new int[width, height];
            for (int y = 0; y < width; ++y)
            {
                for (int x = 0; x < height; ++x)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    float num = (float) ((double) pixel.R * 0.29899999499321 + (double) pixel.G * 0.587000012397766 +
                                         (double) pixel.B * (57.0 / 500.0));
                    numArray[y, x] = (int) num;
                }
            }

            return numArray;
        }
    }
}