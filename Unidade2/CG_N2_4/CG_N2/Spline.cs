using System;
using System.Collections.Generic;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg {
    internal class Spline : Objeto {

        public Shader _shaderVermelha;
        public Shader _shaderVerde;
        public Shader _shaderAzul;
        public Shader _shaderBranca;
        public Shader _shaderAmarela;
        public Shader _shaderCiano;
        public SegReta segReta1;
        public SegReta segReta2;
        public SegReta segReta3;
        protected int indexPontoSelecionado = 1;
        private List<Ponto4D> lista = new List<Ponto4D>(); 
        public int indice = 0;
        public Ponto[] pontos = new Ponto[4];
        
        public Spline(Objeto _paiRef, ref char _rotulo) : base(_paiRef, ref _rotulo)
        {
            PrimitivaTipo = PrimitiveType.LineStrip;

            _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
            _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
            _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
            _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
            _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
            _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");

            pontos[0] = new Ponto(_paiRef, ref _rotulo, new Ponto4D(0.5 , -0.5));
            pontos[1] = new Ponto(_paiRef, ref _rotulo, new Ponto4D(0.5 , 0.5));
            pontos[2] = new Ponto(_paiRef, ref _rotulo, new Ponto4D(-0.5 , 0.5));
            pontos[3] = new Ponto(_paiRef, ref _rotulo, new Ponto4D(-0.5 , -0.5));

            segReta1 = new SegReta(_paiRef, ref _rotulo, pontos[0].PontosId(0), pontos[1].PontosId(0));
            segReta2 = new SegReta(_paiRef, ref _rotulo, pontos[1].PontosId(0), pontos[2].PontosId(0));
            segReta3 = new SegReta(_paiRef, ref _rotulo, pontos[2].PontosId(0), pontos[3].PontosId(0));

            segReta1.shaderObjeto = _shaderCiano;
            segReta2.shaderObjeto = _shaderCiano;
            segReta3.shaderObjeto = _shaderCiano;

            // segReta1 = new SegReta(_paiRef, ref _rotulo, pontoControle2.PontosId(0), pontoControle3.PontosId(0));
            // segReta2 = new SegReta(_paiRef, ref _rotulo,  pontoControle4.PontosId(0), pontoControle3.PontosId(0));
            // segReta3 = new SegReta(_paiRef, ref _rotulo, pontoControle2.PontosId(0), pontoControle1.PontosId(0));
            selecionaPontoVermelho();
            atualizarSpline();
            
        }

        public void SplineQtdPto() {
            double inc = 0.0;
            
            while (inc < 1) {
                double ABX = pontos[3].PontosId(0).X + (pontos[2].PontosId(0).X - pontos[3].PontosId(0).X) * inc;
                double ABY = pontos[3].PontosId(0).Y + (pontos[2].PontosId(0).Y - pontos[3].PontosId(0).Y) * inc;
                double BCX = pontos[2].PontosId(0).X + (pontos[1].PontosId(0).X - pontos[2].PontosId(0).X) * inc;
                double BCY = pontos[2].PontosId(0).Y + (pontos[1].PontosId(0).Y - pontos[2].PontosId(0).Y) * inc;
                double CDX = pontos[1].PontosId(0).X + (pontos[0].PontosId(0).X - pontos[1].PontosId(0).X) * inc;
                double CDY = pontos[1].PontosId(0).Y + (pontos[0].PontosId(0).Y - pontos[1].PontosId(0).Y) * inc;

                double ABCX = ABX + (BCX - ABX) * inc;
                double ABCY = ABY + (BCY - ABY) * inc;
                double BCDX = BCX + (CDX - BCX) * inc;
                double BCDY = BCY + (CDY - BCY) * inc;

                double ABCDX = ABCX + (BCDX - ABCX) * inc;
                double ABCDY = ABCY + (BCDY - ABCY) * inc;

                lista.Add(new Ponto4D(ABCDX, ABCDY)); 

                inc = inc + 0.1;
            }            
            
        }

        public void vinculoObjeto() {
            if (indice >= 3) {
                indice = 0;
            } else {
                indice++;
            }
        }

        public int getIndice() {
            return indice;
        }

        public void atualizarSpline() {
            lista.Clear();
            pontosLista.Clear();    
            SplineQtdPto();
            criarPontos();
        }

        public void adicionarY() {
            pontos[indice].PontosId(0).Y += 0.1;           
        }
        public void diminuirY() {
            pontos[indice].PontosId(0).Y -= 0.1;
        }
           
        public void diminuirX() {
            pontos[indice].PontosId(0).X -= 0.1;
        }

        public void adicionarX() {
            pontos[indice].PontosId(0).X += 0.1;
        }

        public void criarPontos() {
            for (int i = 0; i < lista.Count; i++) {
                PontosAdicionar(lista[i]);
            }
        }

        public void selecionaPontoVermelho() {
            switch (indice)
            {
                case 0:
                    pontos[indice].shaderObjeto = _shaderVermelha;
                    pontos[1].shaderObjeto = _shaderBranca;
                    pontos[2].shaderObjeto = _shaderBranca;
                    pontos[3].shaderObjeto = _shaderBranca;

                    break;
                case 1:
                    pontos[indice].shaderObjeto = _shaderVermelha;
                    pontos[0].shaderObjeto = _shaderBranca;
                    pontos[2].shaderObjeto = _shaderBranca;
                    pontos[3].shaderObjeto = _shaderBranca;
                    break;

                case 2: 
                    pontos[indice].shaderObjeto = _shaderVermelha;
                    pontos[0].shaderObjeto = _shaderBranca;
                    pontos[1].shaderObjeto = _shaderBranca;
                    pontos[3].shaderObjeto = _shaderBranca;
                    break;

                case 3: 
                    pontos[indice].shaderObjeto = _shaderVermelha;
                    pontos[2].shaderObjeto = _shaderBranca;
                    pontos[1].shaderObjeto = _shaderBranca;
                    pontos[0].shaderObjeto = _shaderBranca;
                    break;
                default:
                    break;

            }
        }

        public void Atualizar() {
            base.ObjetoAtualizar();
        }

        public void AtualizarSpline(Ponto4D ptoInc, bool proximo) {

        }

        #if CG_Debug
        public override string ToString() {
            string retorno;
            retorno = "__ Objeto Spline _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
            retorno += base.ImprimeToString();
            return (retorno);
        }
        #endif
    }
}