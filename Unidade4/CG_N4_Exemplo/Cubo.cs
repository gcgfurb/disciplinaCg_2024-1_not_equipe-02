//https://github.com/mono/opentk/blob/main/Source/Examples/Shapes/Old/Cube.cs

#define CG_Debug
using CG_Biblioteca;
using OpenTK.Mathematics;
using System.Drawing;

namespace gcgcg
{
  internal class Cubo : Objeto
  {
    Ponto4D[] vertices;
    // int[] indices;
    // Vector3[] normals;
    // int[] colors;

    public Cubo(Objeto paiRef, ref char _rotulo) :
      this(paiRef, ref _rotulo, new Ponto4D(-0.5, -0.5), new Ponto4D(0.5, 0.5))
    {
      // PrimitivaTipo = PrimitiveType.TriangleFan;
    }

    public Cubo(Objeto paiRef, ref char _rotulo, Ponto4D ptoInfEsq, Ponto4D ptoSupDir) : base(paiRef, ref _rotulo) 
    {

      float[] _vertices = {
        -0.3f, -0.3f,  0.3f,
         0.3f, -0.3f,  0.3f,
         0.3f,  0.3f,  0.3f,
        -0.3f,  0.3f,  0.3f,
        -0.3f, -0.3f, -0.3f,
         0.3f, -0.3f, -0.3f,
         0.3f,  0.3f, -0.3f,
        -0.3f,  0.3f, -0.3f
      };
      
      vertices = new Ponto4D[]
      {
        new Ponto4D(-0.3, -0.3,  0.3),
        new Ponto4D( 0.3, -0.3,  0.3),
        new Ponto4D( 0.3,  0.3,  0.3),
        new Ponto4D(-0.3,  0.3,  0.3),
        new Ponto4D(-0.3, -0.3, -0.3),
        new Ponto4D( 0.3, -0.3, -0.3),
        new Ponto4D( 0.3,  0.3, -0.3),
        new Ponto4D(-0.3,  0.3, -0.3)
      };
      
      base.PontosAdicionar(vertices[0]);
      base.PontosAdicionar(vertices[1]);
      base.PontosAdicionar(vertices[2]);
      base.PontosAdicionar(vertices[3]);

      base.PontosAdicionar(vertices[3]);
      base.PontosAdicionar(vertices[2]);
      base.PontosAdicionar(vertices[6]);
      base.PontosAdicionar(vertices[7]);

      base.PontosAdicionar(vertices[7]);
      base.PontosAdicionar(vertices[6]);
      base.PontosAdicionar(vertices[5]);
      base.PontosAdicionar(vertices[4]);

      base.PontosAdicionar(vertices[4]);
      base.PontosAdicionar(vertices[7]);
      base.PontosAdicionar(vertices[3]);
      base.PontosAdicionar(vertices[0]);

      base.PontosAdicionar(vertices[0]);
      base.PontosAdicionar(vertices[4]);
      base.PontosAdicionar(vertices[5]);
      base.PontosAdicionar(vertices[1]);
      
      base.PontosAdicionar(vertices[1]);
      base.PontosAdicionar(vertices[2]);
      base.PontosAdicionar(vertices[6]);
      base.PontosAdicionar(vertices[5]);


      // indices = new int[]
      // {
      //   0, 1, 2, 2, 3, 0, // front face
      //   3, 2, 6, 6, 7, 3, // top face
      //   7, 6, 5, 5, 4, 7, // back face
      //   4, 0, 3, 3, 7, 4, // left face
      //   0, 1, 5, 5, 4, 0, // bottom face  
      //   1, 5, 6, 6, 2, 1, // right face
      // };

      // normals = new Vector3[]
      // {
      //   new Vector3(-1.0f, -1.0f,  1.0f),
      //   new Vector3( 1.0f, -1.0f,  1.0f),
      //   new Vector3( 1.0f,  1.0f,  1.0f),
      //   new Vector3(-1.0f,  1.0f,  1.0f),
      //   new Vector3(-1.0f, -1.0f, -1.0f),
      //   new Vector3( 1.0f, -1.0f, -1.0f),
      //   new Vector3( 1.0f,  1.0f, -1.0f),
      //   new Vector3(-1.0f,  1.0f, -1.0f),
      // };
      
      // colors = new int[]
      // {
      //   ColorToRgba32(Color.DarkRed),
      //   ColorToRgba32(Color.DarkRed),
      //   ColorToRgba32(Color.Gold),
      //   ColorToRgba32(Color.Gold),
      //   ColorToRgba32(Color.DarkRed),
      //   ColorToRgba32(Color.DarkRed),
      //   ColorToRgba32(Color.Gold),
      //   ColorToRgba32(Color.Gold),
      // };

      // Sentido horário
      // base.PontosAdicionar(ptoInfEsq);
      // base.PontosAdicionar(new Ponto4D(ptoSupDir.X, ptoInfEsq.Y));
      // base.PontosAdicionar(ptoSupDir);
      // base.PontosAdicionar(new Ponto4D(ptoInfEsq.X, ptoSupDir.Y));
      Atualizar();
    }
    public Ponto4D[] getVertices(){
      return this.vertices;
    }
    public static int ColorToRgba32(Color c)
    {
      return (int)((c.A << 24) | (c.B << 16) | (c.G << 8) | c.R);
    }

    private void Atualizar()
    {

      base.ObjetoAtualizar();
    }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Cubo _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return (retorno);
    }
#endif

  }
}
