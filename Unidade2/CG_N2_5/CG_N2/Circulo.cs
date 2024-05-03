using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace gcgcg
{
    internal class Circulo : Objeto
    {
        public Circulo(Objeto _paiRef, ref char _rotulo, Objeto objetoFilho = null) : base(_paiRef, ref _rotulo, objetoFilho)
        {
            // // vem da classe abstrata Objeto
            // PrimitivaTipo = PrimitiveType.LineLoop;
            // // define o tamanho dos pontos 
            // PrimitivaTamanho = 2;
            // List<Ponto4D> pontosCirculo = CalcularPontosCirculo(0.5, 0);
            // foreach (Ponto4D ponto in pontosCirculo) {
            //     PontosAdicionar(ponto);
            // }

            // Atualizar();
        }

        public Circulo(Objeto _paiRef, ref char _rotulo, double raio, double deslocamento) : base(_paiRef, ref _rotulo)
        {
            // vem da classe abstrata Objeto
            PrimitivaTipo = PrimitiveType.LineLoop;
            // define o tamanho dos pontos 
            PrimitivaTamanho = 2;
            List<Ponto4D> pontosCirculo = CalcularPontosCirculo(raio, deslocamento);
            foreach (Ponto4D ponto in pontosCirculo) {
                PontosAdicionar(ponto);
            }

            Atualizar();
        }

        public void Atualizar()
        {
            base.ObjetoAtualizar();
        }

        public static List<Ponto4D> CalcularPontosCirculo(double raio, double deslocamento)
        {
            List<Ponto4D> pontos = [];
            
            int numPontos = 72;

            double angulo = 360 / numPontos;
            double add = angulo;

            for (int i = 0; i < numPontos; i++)
            {
                Ponto4D ponto = new(Matematica.GerarPtosCirculo(angulo, raio).X + deslocamento, 
                    Matematica.GerarPtosCirculo(angulo, raio).Y + deslocamento);
                pontos.Add(ponto);
                angulo += add;
            }

            return pontos;
        }

        #if CG_Debug
        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto Círculo _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
            retorno += base.ImprimeToString();
            return (retorno);
        }
        #endif
    } 

}