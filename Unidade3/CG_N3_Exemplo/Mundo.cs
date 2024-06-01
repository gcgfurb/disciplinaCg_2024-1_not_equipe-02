#define CG_DEBUG
#define CG_Gizmo      
#define CG_OpenGL      
// #define CG_OpenTK
// #define CG_DirectX      
#define CG_Privado  

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using System.Linq;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace gcgcg
{
  public class Mundo : GameWindow
  {
    private static Objeto mundo = null;

    private char rotuloAtual = '?';
    private Objeto objetoSelecionado = null;

    private readonly float[] _sruEixos =
          {
            -0.5f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
            0.0f, -0.5f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
            0.0f,  0.0f, -0.5f, /* Z- */      0.0f,  0.0f,  0.5f, /* Z+ */
        };

    private int _vertexBufferObject_sruEixos;
    private int _vertexArrayObject_sruEixos;
    private int _vertexBufferObject_bbox;
    private int _vertexArrayObject_bbox;

    private Shader _shaderBranca;
    private Shader _shaderVermelha;
    private Shader _shaderVerde;
    private Shader _shaderAzul;
    private Shader _shaderCiano;
    private Shader _shaderMagenta;
    private Shader _shaderAmarela;

    bool controlePrimeiroClique = true;
    private List<Ponto4D> pontos;
    private Objeto novo;

    public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
      : base(gameWindowSettings, nativeWindowSettings)
    {
      mundo ??= new Objeto(null, ref rotuloAtual); //padrão Singleton
    }

    protected override void OnLoad()
    {
      base.OnLoad();

      Utilitario.Diretivas();
#if CG_DEBUG      
      Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif

      GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

      #region Cores
      _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
      _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
      _shaderMagenta = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
      _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
      #endregion

      #region Eixos: SRU  
      _vertexBufferObject_sruEixos = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
      GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
      _vertexArrayObject_sruEixos = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);
      #endregion

      #region iniciar variaveis
      pontos = new List<Ponto4D>();
      #endregion
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit);

      mundo.Desenhar(new Transformacao4D());

#if CG_Gizmo      
      Gizmo_Sru3D();
      Gizmo_BBox();
#endif
      SwapBuffers();
    }
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);

      #region Teclado
      var estadoTeclado = KeyboardState;
      if (estadoTeclado.IsKeyDown(Keys.Escape))
        Close();
        
      if (estadoTeclado.IsKeyPressed(Keys.Space))
      {
        if (objetoSelecionado == null)
          objetoSelecionado = mundo;
        objetoSelecionado = mundo.GrafocenaBuscaProximo(objetoSelecionado);
      }

      if (estadoTeclado.IsKeyPressed(Keys.Enter))
      {
        if (pontos.Count > 2)
        {
          objetoSelecionado = novo;
          pontos.Clear();
        }
      }

      if (estadoTeclado.IsKeyPressed(Keys.G))
        mundo.GrafocenaImprimir("");

      if (objetoSelecionado != null)
      {
        if (estadoTeclado.IsKeyPressed(Keys.M))
          objetoSelecionado.MatrizImprimir();

        if (estadoTeclado.IsKeyPressed(Keys.I))
          objetoSelecionado.MatrizAtribuirIdentidade();

        // remove o poligono
        if (estadoTeclado.IsKeyPressed(Keys.D))
          removerPoligonoSelecionado();

        // remove o vertice do poligono
        if (estadoTeclado.IsKeyPressed(Keys.E))
          removeVerticePoligono();

        // move o vertice do poligono
        if (estadoTeclado.IsKeyDown(Keys.V))
          moveVerticePoligono();

        // muda a primitiva do poligono
        if (estadoTeclado.IsKeyPressed(Keys.P))
          mudarDesenhoPoligono();

        // muda a cor do poligono para vermelho
        if (estadoTeclado.IsKeyPressed(Keys.R))
          mudaCorPoligono('R');

        // muda a cor do poligono para verde          
        if (estadoTeclado.IsKeyPressed(Keys.G))
          mudaCorPoligono('G');

        // muda a cor do poligono para azul
        if (estadoTeclado.IsKeyPressed(Keys.B))
          mudaCorPoligono('B');

        // muda a cor do poligono para branco
        if (estadoTeclado.IsKeyPressed(Keys.W))
          mudaCorPoligono('W');

        // move poligono para cima
        if (estadoTeclado.IsKeyPressed(Keys.Up))
          transacionarPoligono(0, 0.05, 0);

        // move poligono para baixo
        if (estadoTeclado.IsKeyPressed(Keys.Down))
          transacionarPoligono(0, -0.05, 0);

        // move poligono para direita
        if (estadoTeclado.IsKeyPressed(Keys.Right))
          transacionarPoligono(0.05, 0, 0);

        // move poligono para esquerda          
        if (estadoTeclado.IsKeyPressed(Keys.Left))
          transacionarPoligono(-0.05, 0, 0);

        // redimenciona poligono com escala a partir do Bbox
        if (estadoTeclado.IsKeyReleased(Keys.Home))
          escalaBboxPoligono(0.5, 0.5, 0.5);

        // redimenciona poligono com escala a partir do Bbox
        if (estadoTeclado.IsKeyReleased(Keys.End))
          escalaBboxPoligono(2, 2, 2);

        // redimenciona poligono com escala a partir da tela
        if (estadoTeclado.IsKeyReleased(Keys.PageUp))
          escalaPoligono(2, 2, 2);

        // redimenciona poligono com escala a partir da tela
        if (estadoTeclado.IsKeyReleased(Keys.PageDown))
          escalaPoligono(0.5, 0.5, 0.5);

        // rotaciona o poligono
        if (estadoTeclado.IsKeyReleased(Keys.D1) || estadoTeclado.IsKeyReleased(Keys.KeyPad1))
          rotacionarPoligono(10);

        // rotaciona o poligono
        if (estadoTeclado.IsKeyReleased(Keys.D2) || estadoTeclado.IsKeyReleased(Keys.KeyPad2))
          rotacionarPoligono(-10);

        // rotaciona o bbox
        if (estadoTeclado.IsKeyReleased(Keys.D3) || estadoTeclado.IsKeyReleased(Keys.KeyPad3))
          rotacionarBbox(10);

        // rotaciona o bbox
        if (estadoTeclado.IsKeyReleased(Keys.D4) || estadoTeclado.IsKeyReleased(Keys.KeyPad4))
          rotacionarBbox(-10);
      }
      #endregion

      #region  Mouse
      // ao clicar com o botao direito do mouse
      if (MouseState.IsButtonReleased(MouseButton.Right))
        controlePrimeiroClique = true;

      // ao segurar o botao direito no mouse
      if (MouseState.IsButtonDown(MouseButton.Right))
      {
        if (controlePrimeiroClique) {
          desenharPoligono();
          controlePrimeiroClique = false;
        }

        efetuarRastroPoligono();  
      }

      // ao clicar no botao esquerdo do mouse
      if (MouseState.IsButtonPressed(MouseButton.Left))
        selecionaPoligono();
      #endregion
    }

    // desenha o poligono em tela
    private void desenharPoligono()
    {
      pontos.Add(getPosicaoMouse());

      if (pontos.Count == 2)
      {
        //criando o novo objeto e setando direto no selecionado
        objetoSelecionado = novo = new Poligono(objetoSelecionado == null ? mundo : objetoSelecionado, ref rotuloAtual, new List<Ponto4D>(pontos.ToList()));
      }
      
      //se ja tiver 2, então ja criou o objeto e precisa somente adicionar os pontos
      if (pontos.Count > 2)
      {
        novo.PontosAdicionar(pontos.Last());
        novo.ObjetoAtualizar();
      }
    }

    // faz o rastro depois do terceiro ponto
    private void efetuarRastroPoligono()
    {
      // se o objeto selecionado não for nullo e tiver mais que 1 pto adicionado  
      if (objetoSelecionado != null && objetoSelecionado.getTamanhoListaPontos() > 2 && pontos.Count > 2)
      {
        objetoSelecionado.PontosAlterar(getPosicaoMouse(), objetoSelecionado.getTamanhoListaPontos() - 1);
        objetoSelecionado.ObjetoAtualizar();
      }
    }

    // move o vertice do poligono mais proximo da posição do mouse
    private void moveVerticePoligono()
    {
      Ponto4D aux = getPosicaoMouse();  
      objetoSelecionado.PontosAlterar(aux, objetoSelecionado.ObterIndiceMaisProximo(aux));
    }

    // remove o vertice do poligono mais proximo da posição do mouse
    private void removeVerticePoligono()
    {
      objetoSelecionado.removerVerticePoligono(getPosicaoMouse());

      //se após retirar o vertice, não ter 2 pontos, mata o poligono também
      if (objetoSelecionado.getTamanhoListaPontos() < 2)
        removerPoligonoSelecionado();
    }

    // remove o poliogno que está selecionado
    private void removerPoligonoSelecionado()
    {
      //que dizer que esta na criação de um objeto
      if (isCriandoPoligono() || objetoSelecionado == null)
        return;
      
      //se excluiu o objeto, 
      if (mundo.removerObjeto(objetoSelecionado))
      {
        //se não encontrar o proximo objeto, faz referencia
        Objeto aux = mundo.GrafocenaBuscaProximo(objetoSelecionado);
        objetoSelecionado = aux != null ? aux : mundo;
      }
    }

    // muda a primitiva do vertice do poligono aberto - fechado / fechado - aberto
    private void mudarDesenhoPoligono()
    {
      objetoSelecionado.PrimitivaTipo = (objetoSelecionado.PrimitivaTipo == PrimitiveType.LineLoop) ? PrimitiveType.LineStrip : PrimitiveType.LineLoop;
    }

    // muda a cor do poligono R - vervelho / G - verde / B - azul
    private void mudaCorPoligono(char c)
    {
      Shader shader;
      switch (c)
      {
        case 'R':
          shader = _shaderVermelha;
          break;

        case 'G':
          shader = _shaderVerde;
          break;

        case 'B':
          shader = _shaderAzul;
          break;

        case 'W':
          shader = _shaderBranca;
          break;

        default:
          shader = _shaderBranca;
          break;
      }

      objetoSelecionado.ShaderObjeto = shader;

    }

    // busca o poligono por meio da recursão
    private void selecionaPoligono()
    {

      if (isCriandoPoligono()) 
        return;

      objetoSelecionado = mundo.isDentroBbox(getPosicaoMouse());
    }

    // transaciona o poligono
    private void transacionarPoligono(double x, double y, double z)
    {
      // se não tiver objeto selecionado n faz nada
      if (objetoSelecionado == null)
        return;

      objetoSelecionado.MatrizTranslacaoXYZ(x, y, z);
    }

    // escala o bbox so poliogno
    private void escalaBboxPoligono(double x, double y, double z)
    {
      if (objetoSelecionado == null)
        return;

      objetoSelecionado.MatrizEscalaXYZBBox(x, y, z);
    }

    // escala o poligono
    private void escalaPoligono(double x, double y, double z)
    {
      if (objetoSelecionado == null)
        return;

      objetoSelecionado.MatrizEscalaXYZ(x, y, z);
    }

    // rotaciona o bbox do poliogno
    private void rotacionarBbox(int rotacao)
    {
      if (objetoSelecionado == null)
        return;

      objetoSelecionado.MatrizRotacaoZBBox(rotacao);
    }

    //rotaciona o poliogno
    private void rotacionarPoligono(int rotacao)
    {
      if (objetoSelecionado == null)
        return;

      objetoSelecionado.MatrizRotacao(rotacao);
    }

    // pega a posição atual do mouse
    private Ponto4D getPosicaoMouse()
    {
      int janelaLargura = Size.X;
      int janelaAltura = Size.Y;

      Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);

      return Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);
    }

    // verifica se não está criando um objeto
    private bool isCriandoPoligono()
    {
      return pontos.Count != 0 ? true : false;
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);

#if CG_DEBUG      
      Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif
      GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
    }

    protected override void OnUnload()
    {
      mundo.OnUnload();

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
      GL.UseProgram(0);

      GL.DeleteBuffer(_vertexBufferObject_sruEixos);
      GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

      GL.DeleteBuffer(_vertexBufferObject_bbox);
      GL.DeleteVertexArray(_vertexArrayObject_bbox);

      GL.DeleteProgram(_shaderBranca.Handle);
      GL.DeleteProgram(_shaderVermelha.Handle);
      GL.DeleteProgram(_shaderVerde.Handle);
      GL.DeleteProgram(_shaderAzul.Handle);
      GL.DeleteProgram(_shaderCiano.Handle);
      GL.DeleteProgram(_shaderMagenta.Handle);
      GL.DeleteProgram(_shaderAmarela.Handle);

      base.OnUnload();
    }

#if CG_Gizmo
    private void Gizmo_Sru3D()
    {
#if CG_OpenGL && !CG_DirectX
      var transform = Matrix4.Identity;
      GL.BindVertexArray(_vertexArrayObject_sruEixos);

      // EixoX
      _shaderCiano.SetMatrix4("transform", transform);
      _shaderCiano.Use();
      GL.DrawArrays(PrimitiveType.Lines, 0, 2);

      // EixoY
      _shaderMagenta.SetMatrix4("transform", transform);
      _shaderMagenta.Use();
      GL.DrawArrays(PrimitiveType.Lines, 2, 2);

      // EixoZ
      _shaderAzul.SetMatrix4("transform", transform);
      _shaderAzul.Use();
      GL.DrawArrays(PrimitiveType.Lines, 4, 2);

#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
    }
#endif    

#if CG_Gizmo
    private void Gizmo_BBox()
    {
      if (objetoSelecionado != null)
      {

#if CG_OpenGL && !CG_DirectX

        float[] _bbox =
        {
        (float) objetoSelecionado.Bbox().ObterMenorX, (float) objetoSelecionado.Bbox().ObterMenorY, 0.0f, // A
        (float) objetoSelecionado.Bbox().ObterMaiorX, (float) objetoSelecionado.Bbox().ObterMenorY, 0.0f, // B
        (float) objetoSelecionado.Bbox().ObterMaiorX, (float) objetoSelecionado.Bbox().ObterMaiorY, 0.0f, // C
        (float) objetoSelecionado.Bbox().ObterMenorX, (float) objetoSelecionado.Bbox().ObterMaiorY, 0.0f  // D
      };

        _vertexBufferObject_bbox = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_bbox);
        GL.BufferData(BufferTarget.ArrayBuffer, _bbox.Length * sizeof(float), _bbox, BufferUsageHint.StaticDraw);
        _vertexArrayObject_bbox = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject_bbox);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        var transform = Matrix4.Identity;
        GL.BindVertexArray(_vertexArrayObject_bbox);
        _shaderAmarela.SetMatrix4("transform", transform);
        _shaderAmarela.Use();
        GL.DrawArrays(PrimitiveType.LineLoop, 0, (_bbox.Length / 3));

#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
      }
    }
#endif    
  }

}
