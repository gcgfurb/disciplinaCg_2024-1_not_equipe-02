#define CG_OpenGL
#define CG_Debug
// #define CG_DirectX

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gcgcg
{
  internal class Objeto  //TODO: deveria ser uma class abstract ..??
  {
    // Objeto
    private readonly char rotulo;
    protected Objeto paiRef;
    private readonly List<Objeto> objetosLista = new List<Objeto>();
    private PrimitiveType primitivaTipo = PrimitiveType.LineLoop;
    public PrimitiveType PrimitivaTipo { get => primitivaTipo; set => primitivaTipo = value; }
    private float primitivaTamanho = 1;
    public float PrimitivaTamanho { get => primitivaTamanho; set => primitivaTamanho = value; }
    private Shader _shaderObjeto = new("Shaders/shader.vert", "Shaders/shaderBranca.frag");
    public Shader ShaderObjeto { set => _shaderObjeto = value; }

    protected List<Ponto4D> pontosLista = [];
    public int PontosListaTamanho { get => pontosLista.Count; }
    private int _vertexBufferObject;
    private int _vertexArrayObject;

    // BBox do objeto
    private readonly BBox bBox = new();
    public BBox Bbox()  //TODO: readonly
    {
      return bBox;
    }

    // Transformações do objeto
    private Transformacao4D matriz = new Transformacao4D();

    /// Matrizes temporarias que sempre sao inicializadas com matriz Identidade entao podem ser "static".
    private static Transformacao4D matrizTmpTranslacao = new Transformacao4D();
    private static Transformacao4D matrizTmpTranslacaoInversa = new Transformacao4D();
    private static Transformacao4D matrizTmpEscala = new Transformacao4D();
    private static Transformacao4D matrizTmpRotacao = new Transformacao4D();
    private static Transformacao4D matrizGlobal = new Transformacao4D();
    private char eixoRotacao = 'z';
    public void TrocaEixoRotacao(char eixo) => eixoRotacao = eixo;


    public Objeto(Objeto _paiRef, ref char _rotulo, Objeto objetoFilho = null)
    {
      this.paiRef = _paiRef;
      rotulo = _rotulo = Utilitario.CharProximo(_rotulo);
      if (_paiRef != null)
      {
        ObjetoAdicionar(objetoFilho);
      }
    }

    private void ObjetoAdicionar(Objeto objetoFilho)
    {
      if (objetoFilho == null)
      {
        paiRef.objetosLista.Add(this);
      }
      else
      {
        paiRef.FilhoAdicionar(objetoFilho);
      }
    }

    public void objetoRemover(Objeto objetoSelecionado)
    {
      Objeto objeto = GrafocenaBusca(objetoSelecionado.rotulo);
      objeto.paiRef.objetosLista.Remove(objeto);
    }

    public void ObjetoAtualizar()
    {
      float[] vertices = new float[pontosLista.Count * 3];
      int ptoLista = 0;
      for (int i = 0; i < vertices.Length; i += 3)
      {
        vertices[i] = (float)pontosLista[ptoLista].X;
        vertices[i + 1] = (float)pontosLista[ptoLista].Y;
        vertices[i + 2] = (float)pontosLista[ptoLista].Z;
        ptoLista++;
      }
      bBox.Atualizar(matriz, pontosLista);

      GL.PointSize(primitivaTamanho);

      _vertexBufferObject = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
      GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
      _vertexArrayObject = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);
    }

    public void Desenhar(Transformacao4D matrizGrafo)
    {
#if CG_OpenGL && !CG_DirectX
      GL.PointSize(primitivaTamanho);

      GL.BindVertexArray(_vertexArrayObject);

      if (paiRef != null)
      {
        matrizGrafo = matrizGrafo.MultiplicarMatriz(matriz);
        _shaderObjeto.SetMatrix4("transform", matrizGrafo.ObterDadosOpenTK());
        _shaderObjeto.Use();
        GL.DrawArrays(primitivaTipo, 0, pontosLista.Count);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
      }
      for (var i = 0; i < objetosLista.Count; i++)
      {
        objetosLista[i].Desenhar(matrizGrafo);
      }
    }

    #region Objeto: CRUD

    public void FilhoAdicionar(Objeto filho)
    {
      this.objetosLista.Add(filho);
    }

    public Ponto4D PontosId(int id)
    {
      return pontosLista[id];
    }

    public void PontosAdicionar(Ponto4D pto)
    {
      pontosLista.Add(pto);
      ObjetoAtualizar();
    }

    public void PontosAlterar(Ponto4D pto, int posicao)
    {
      pontosLista[posicao] = pto;
      ObjetoAtualizar();
    }

    public void PontosLimpar()
    {
      pontosLista.Clear();
      ObjetoAtualizar();
    }

    #endregion

    #region Objeto: Grafo de Cena

    public Objeto GrafocenaBusca(char _rotulo)
    {
      if (rotulo == _rotulo)
      {
        return this;
      }
      foreach (var objeto in objetosLista)
      {
        var obj = objeto.GrafocenaBusca(_rotulo);
        if (obj != null)
        {
          return obj;
        }
      }
      return null;
    }

    public Objeto GrafocenaBuscaProximo(Objeto objetoAtual)
    {
      objetoAtual = GrafocenaBusca(Utilitario.CharProximo(objetoAtual.rotulo));
      if (objetoAtual != null)
      {
        return objetoAtual;
      }
      else
      {
        return GrafocenaBusca(Utilitario.CharProximo('@'));
      }
    }

    public void GrafocenaImprimir(string idt)
    {
      Console.WriteLine(idt + rotulo);
      foreach (var objeto in objetosLista)
      {
        objeto.GrafocenaImprimir(idt + "  ");
      }
    }

    #endregion

    #region Objeto: Transformações Geométricas

    public void MatrizImprimir()
    {
      Console.WriteLine(matriz);
    }
    public void MatrizAtribuirIdentidade()
    {
      matriz.AtribuirIdentidade();
      ObjetoAtualizar();
    }
    public void MatrizTranslacaoXYZ(double tx, double ty, double tz)
    {
      Transformacao4D matrizTranslate = new Transformacao4D();
      matrizTranslate.AtribuirTranslacao(tx, ty, tz);
      matriz = matrizTranslate.MultiplicarMatriz(matriz);
      ObjetoAtualizar();
    }
    public void MatrizEscalaXYZ(double Sx, double Sy, double Sz)
    {
      Transformacao4D matrizScale = new Transformacao4D();
      matrizScale.AtribuirEscala(Sx, Sy, Sz);
      matriz = matrizScale.MultiplicarMatriz(matriz);
      ObjetoAtualizar();
    }

    public void MatrizEscalaXYZBBox(double Sx, double Sy, double Sz)
    {
      matrizGlobal.AtribuirIdentidade();
      Ponto4D pontoPivo = bBox.ObterCentro;

      matrizTmpTranslacao.AtribuirTranslacao(-pontoPivo.X, -pontoPivo.Y, -pontoPivo.Z); // Inverter sinal
      matrizGlobal = matrizTmpTranslacao.MultiplicarMatriz(matrizGlobal);

      matrizTmpEscala.AtribuirEscala(Sx, Sy, Sz);
      matrizGlobal = matrizTmpEscala.MultiplicarMatriz(matrizGlobal);

      matrizTmpTranslacaoInversa.AtribuirTranslacao(pontoPivo.X, pontoPivo.Y, pontoPivo.Z);
      matrizGlobal = matrizTmpTranslacaoInversa.MultiplicarMatriz(matrizGlobal);

      matriz = matriz.MultiplicarMatriz(matrizGlobal);

      ObjetoAtualizar();
    }
    public void MatrizRotacaoEixo(double angulo)
    {
      switch (eixoRotacao)  // TODO: ainda não uso no exemplo
      {
        case 'x':
          matrizTmpRotacao.AtribuirRotacaoX(Transformacao4D.DEG_TO_RAD * angulo);
          break;
        case 'y':
          matrizTmpRotacao.AtribuirRotacaoY(Transformacao4D.DEG_TO_RAD * angulo);
          break;
        case 'z':
          matrizTmpRotacao.AtribuirRotacaoZ(Transformacao4D.DEG_TO_RAD * angulo);
          break;
        default:
          Console.WriteLine("opção de eixoRotacao: ERRADA!");
          break;
      }
      ObjetoAtualizar();
    }
    public void MatrizRotacao(double angulo)
    {
      MatrizRotacaoEixo(angulo);
      matriz = matrizTmpRotacao.MultiplicarMatriz(matriz);
      ObjetoAtualizar();
    }
    public void MatrizRotacaoZBBox(double angulo)
    {
      matrizGlobal.AtribuirIdentidade();
      Ponto4D pontoPivo = bBox.ObterCentro;

      matrizTmpTranslacao.AtribuirTranslacao(-pontoPivo.X, -pontoPivo.Y, -pontoPivo.Z); // Inverter sinal
      matrizGlobal = matrizTmpTranslacao.MultiplicarMatriz(matrizGlobal);

      MatrizRotacaoEixo(angulo);
      matrizGlobal = matrizTmpRotacao.MultiplicarMatriz(matrizGlobal);

      matrizTmpTranslacaoInversa.AtribuirTranslacao(pontoPivo.X, pontoPivo.Y, pontoPivo.Z);
      matrizGlobal = matrizTmpTranslacaoInversa.MultiplicarMatriz(matrizGlobal);

      matriz = matriz.MultiplicarMatriz(matrizGlobal);

      ObjetoAtualizar();
    }

    #endregion

    public void OnUnload()
    {
      foreach (var objeto in objetosLista)
      {
        objeto.OnUnload();
      }

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
      GL.UseProgram(0);

      GL.DeleteBuffer(_vertexBufferObject);
      GL.DeleteVertexArray(_vertexArrayObject);

      GL.DeleteProgram(_shaderObjeto.Handle);
    }

#if CG_Debug
    protected string ImprimeToString()
    {
      string retorno;
      retorno = "__ Objeto: " + rotulo + "\n";
      for (var i = 0; i < pontosLista.Count; i++)
      {
        retorno += "P" + i + "[ " +
        string.Format("{0,10}", pontosLista[i].X) + " | " +
        string.Format("{0,10}", pontosLista[i].Y) + " | " +
        string.Format("{0,10}", pontosLista[i].Z) + " | " +
        string.Format("{0,10}", pontosLista[i].W) + " ]" + "\n";
      }
      retorno += bBox.ToString();
      return retorno;
    }
#endif

    //busca o objeto e remove ele do obj pai
    public bool removerObjeto(Objeto obj)
    {
      bool excluido = false;
      Objeto aux = GrafocenaBusca(obj.rotulo);
      //se tiver obj pai remove ele
      if (aux.paiRef != null)
      {
        aux.paiRef.objetosLista.Remove(aux);
        excluido = true;
      }

      return excluido;
    }

    //reotrna o indice mais proximo da posição do mouse
    public int ObterIndiceMaisProximo(Ponto4D localizacaoMouse)
    {
      int indiceMaisProximo = -1;
      double distanciaMaisProxima = double.MaxValue;

      for (int i = 0; i < pontosLista.Count; i++)
      {
        double distancia = Matematica.Distancia(localizacaoMouse, pontosLista[i]);

        if (distancia < distanciaMaisProxima)
        {
          distanciaMaisProxima = distancia;
          indiceMaisProximo = i;
        }
      }

      return indiceMaisProximo;
    }

    //remove o ponto mais proximo do mouse
    public void removerVerticePoligono(Ponto4D posMouse)
    {
      if (pontosLista.Count == 1)
        return;
      
      pontosLista.RemoveAt(ObterIndiceMaisProximo(posMouse));
      ObjetoAtualizar();
    }

    //retorna o tamanho da lista de pontos
    public int getTamanhoListaPontos()
    {
      return pontosLista.Count;
    }

    public Objeto isDentroBbox(Ponto4D pto)
    {
      // verifica se o ponto está dentro da bbox e se o ponto esta dentro do poligono com o scanline
      if (paiRef != null && bBox.Dentro(pto) && EstaDentroDoPoligono(pto))
          return this;

      // verifica os filhos recursivamente
      foreach (Objeto filho in objetosLista) {
        Objeto obj = filho.isDentroBbox(pto);

        if (obj != null)
          return obj;
      }

      return null;
    }

    // Método auxiliar para verificar se o ponto está dentro do polígono
    private bool EstaDentroDoPoligono(Ponto4D pontoClique)
    {
      //fecha o poligono
      var pontosPoligono = pontosLista.Append(pontosLista.First()).ToArray();
      var count = 0;

      for (var indicePonto = 0; indicePonto < pontosLista.Count; indicePonto++) {
        var pto1 = pontosPoligono[indicePonto];
        var pto2 = pontosPoligono[indicePonto + 1];

        if (Matematica.ScanLine(pontoClique, pto1, pto2))
          count++;
      }

      // O ponto está dentro do polígono se a contagem de interseções for ímpar
      return count % 2 != 0;
    }

  }
}