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

        public Ponto pontoControle1;
        public Ponto pontoControle2;
        public Ponto pontoControle3;
        public Ponto pontoControle4;
        public SegReta segReta1;
        public SegReta segReta2;
        public SegReta segReta3;
        protected int indexPontoSelecionado = 1;
        private List<Ponto4D> lista = new List<Ponto4D>(); 
        private int indice = 0;
        
        public Spline(Objeto _paiRef, ref char _rotulo) : base(_paiRef, ref _rotulo)
        {
            PrimitivaTipo = PrimitiveType.LineStrip;

            _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
            _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
            _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
            _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
            _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
            _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");

            pontoControle1 = new Ponto(_paiRef, ref _rotulo, new Ponto4D(0.5 , -0.5));
            pontoControle2 = new Ponto(_paiRef, ref _rotulo, new Ponto4D(0.5 , 0.5));
            pontoControle3 = new Ponto(_paiRef, ref _rotulo, new Ponto4D(-0.5 , 0.5));
            pontoControle4 = new Ponto(_paiRef, ref _rotulo, new Ponto4D(-0.5 , -0.5));

            segReta1 = new SegReta(_paiRef, ref _rotulo, new Ponto4D(0.5 , 0.5), new Ponto4D(-0.5 , 0.5));
            segReta2 = new SegReta(_paiRef, ref _rotulo, new Ponto4D(-0.5 , -0.5), new Ponto4D(-0.5 , 0.5));
            segReta3 = new SegReta(_paiRef, ref _rotulo, new Ponto4D(0.5 , 0.5), new Ponto4D(0.5 , -0.5));
            atualizarSpline();
            
        }

        public void SplineQtdPto() {
            double inc = 0.0;
            while (inc < 1) {
                
                double ABX = pontoControle4.PontosId(0).X + (pontoControle3.PontosId(0).X - pontoControle4.PontosId(0).X) * inc;
                double ABY = pontoControle4.PontosId(0).Y + (pontoControle3.PontosId(0).Y - pontoControle4.PontosId(0).Y) * inc;
                double BCX = pontoControle3.PontosId(0).X + (pontoControle2.PontosId(0).X - pontoControle3.PontosId(0).X) * inc;
                double BCY = pontoControle3.PontosId(0).Y + (pontoControle2.PontosId(0).Y - pontoControle3.PontosId(0).Y) * inc;
                double CDX = pontoControle2.PontosId(0).X + (pontoControle1.PontosId(0).X - pontoControle2.PontosId(0).X) * inc;
                double CDY = pontoControle2.PontosId(0).Y + (pontoControle1.PontosId(0).Y - pontoControle2.PontosId(0).Y) * inc;

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

        public void vinculoObjeto(){
            if(indice >3){
                indice =0;
            }
            indice++;
        }

        public void atualizarSpline(){
            lista.Clear();
            pontosLista.Clear();    
            SplineQtdPto();
            criarPontos();
        }

        public void adicionarY(){
            switch(indice){
                case 0:
                pontoControle1.PontosId(0).Y += 0.2;
                break;
                case 1:
                pontoControle2.PontosId(0).Y += 0.2;
                break;
                case 2:
                pontoControle3.PontosId(0).Y += 0.2;
                break;
                case 3:
                pontoControle4.PontosId(0).Y += 0.2;
                break;  
                default:
                break;               
            }
       
        }
        public void diminuirY(){
           switch(indice){
                case 0:
                pontoControle1.PontosId(0).Y -= 0.2;
                break;
                case 1:
                pontoControle2.PontosId(0).Y -= 0.2;
                break;
                case 2:
                pontoControle3.PontosId(0).Y -= 0.2;
                break;
                case 3:
                pontoControle4.PontosId(0).Y -= 0.2;
                break;  
                default:
                break;               
            }
           
        }

        public void diminuirX(){
           switch(indice){
                case 0:
                pontoControle1.PontosId(0).X -= 0.2;
                break;
                case 1:
                pontoControle2.PontosId(0).X -= 0.2;
                break;
                case 2:
                pontoControle3.PontosId(0).X -= 0.2;
                break;
                case 3:
                pontoControle4.PontosId(0).X -= 0.2;
                break;  
                default:
                break;               
            }
        }

        public void adicionarX(){
           switch(indice){
                case 0:
                pontoControle1.PontosId(0).X += 0.2;
                break;
                case 1:
                pontoControle2.PontosId(0).X += 0.2;
                break;
                case 2:
                pontoControle3.PontosId(0).X += 0.2;
                break;
                case 3:
                pontoControle4.PontosId(0).X += 0.2;
                break;  
                default:
                break;               
            }
            
        }


        public void criarPontos(){
            for(int i = 0; i < lista.Count; i++){
                PontosAdicionar(lista[i]);
            }
        }

        public void selecionaPontoVermelho(Objeto objetoSelecionado) {
            if (indexPontoSelecionado > 3) {
                indexPontoSelecionado = 0;
            }
            switch (indexPontoSelecionado)
            {
                case 0:
                    objetoSelecionado = pontoControle1;
                    objetoSelecionado.shaderObjeto = _shaderVermelha;

                    pontoControle2.shaderObjeto = _shaderBranca;
                    pontoControle3.shaderObjeto = _shaderBranca;
                    pontoControle4.shaderObjeto = _shaderBranca;

                    break;
                case 1:
                    objetoSelecionado = pontoControle2;
                    objetoSelecionado.shaderObjeto = _shaderVermelha;

                    pontoControle1.shaderObjeto = _shaderBranca;
                    pontoControle3.shaderObjeto = _shaderBranca;
                    pontoControle4.shaderObjeto = _shaderBranca;

                    break;

                case 2: 
                    objetoSelecionado = pontoControle3;
                    objetoSelecionado.shaderObjeto = _shaderVermelha;

                    pontoControle1.shaderObjeto = _shaderBranca;
                    pontoControle2.shaderObjeto = _shaderBranca;
                    pontoControle4.shaderObjeto = _shaderBranca;

                    break;

                case 3: 
                    objetoSelecionado = pontoControle4;
                    objetoSelecionado.shaderObjeto = _shaderVermelha;

                    pontoControle1.shaderObjeto = _shaderBranca;
                    pontoControle2.shaderObjeto = _shaderBranca;
                    pontoControle3.shaderObjeto = _shaderBranca;
                    
                    break;
                default:
                    break;

            }
            indexPontoSelecionado++;
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