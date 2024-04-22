#define CG_Gizmo  // debugar gráfico.
#define CG_OpenGL // render OpenGL.

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
    internal class SrPalito: Objeto {

        private double posicao = 0;
        private double angulo = 45;
        private double tamanhoReta = 0.5; // Valor inicial
        private Ponto4D pontoInicial;
        private Ponto4D pontoFim;

        public SrPalito(Objeto _paiRef, ref char _rotulo) : this(_paiRef, ref _rotulo, new Ponto4D(0.0, 0.0))
        {

        }

        public SrPalito(Objeto _paiRef, ref char _rotulo, Ponto4D inicio) : base(_paiRef, ref _rotulo)
        {
            PrimitivaTipo = PrimitiveType.Lines;
            PrimitivaTamanho = 3;

            PontosAdicionar(inicio);
            PontosAdicionar(new Ponto4D(inicio.X + tamanhoReta, inicio.Y));

            ObjetoAtualizar();
        }

#if CG_Debug
        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto SrPalito _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
            retorno += base.ImprimeToString();
            return (retorno);
        }
#endif

        public void moverEsquerda() {
            pontoInicial = base.PontosId(0);
            pontoFim = base.PontosId(1);
            posicao = posicao - 0.5;
            base.PontosAlterar(new Ponto4D(posicao, pontoInicial.Y), 0);
            base.PontosAlterar(new Ponto4D(pontoFim.X - 0.5, pontoFim.Y), 1);
        }

        public void moverDireita() {
            pontoInicial = base.PontosId(0);
            pontoFim = base.PontosId(1);
            posicao = posicao + 0.5;
            base.PontosAlterar(new Ponto4D(posicao, pontoInicial.Y), 0);
            base.PontosAlterar(new Ponto4D(pontoFim.X + 0.5, pontoFim.Y), 1);
        }

        public void diminuir() {
            pontoInicial = base.PontosId(0);
            pontoFim = base.PontosId(1);
            tamanhoReta = tamanhoReta - 0.5;
            
            Ponto4D novoPonto = Matematica.GerarPtosCirculo(angulo, tamanhoReta);
            novoPonto.X = novoPonto.X + posicao;
            base.PontosAlterar(novoPonto, 1);
        }

        public void aumentar() {
            pontoInicial = base.PontosId(0);
            pontoFim = base.PontosId(1);

            tamanhoReta = tamanhoReta + 0.5;
            Ponto4D novoPonto = Matematica.GerarPtosCirculo(angulo, tamanhoReta);
            novoPonto.X = novoPonto.X + posicao;
            base.PontosAlterar(novoPonto, 1);
        }

        public void diminuirAngulo() {
            angulo = angulo - 5;
            Ponto4D novoPonto = Matematica.GerarPtosCirculo(angulo, tamanhoReta);
            novoPonto.X = novoPonto.X + posicao;
            base.PontosAlterar(novoPonto, 1);
        }

        public void aumentarAngulo() {
            angulo = angulo + 5;
            Ponto4D novoPonto = Matematica.GerarPtosCirculo(angulo, tamanhoReta);
            novoPonto.X = novoPonto.X + posicao;
            base.PontosAlterar(novoPonto, 1);
        }
        
    }
}
