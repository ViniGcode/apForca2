using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class Dicionario : IComparable<Dicionario>, IRegistro
    {
        // atributos
        string palavra;
        string dica;
        bool[] acertou = new bool[tamanhoPalavra]; // vetor para contagem de letras acertadas 

        // mapeamento dos campos do arquivo texto de dados das palavras e dicas
        const int tamanhoPalavra = 15,
                    tamanhoDica = 50,
                    inicioPalavra = 0,
                    inicioDica = inicioPalavra + tamanhoPalavra;


        // construtor vazio
        // inicializa o bool[] acertou com 15 posições, todas false
        public Dicionario() 
            {
                IniciarContagemDeAcertos(); 
            }

        // construtor que recebe linha de dados e converte
        public Dicionario(string linhaDeDados)
        {
            LerDados(linhaDeDados);
            IniciarContagemDeAcertos();

        }

        // construtor normal
        public Dicionario(string palavra, string dica)
        {
            Palavra = palavra;
            Dica = dica;
            IniciarContagemDeAcertos();

        }

        //´métodos de acesso as propriedades Palavra, Dica e Acertos
        public string Palavra {
            get => palavra;
            set
            {
                if (value == "")
                    throw new Exception("Palavra precisa ter um valor");

                // preenche à direita com espaço até completar tamanhoPalavra
                //ex.:  Palavra1       Dica1                        
                //      Palavra2       Dica2
                palavra = value.PadRight(tamanhoPalavra, ' ').Substring(0, tamanhoPalavra);
            }
        }

        public string Dica {
            get => dica;
            set
            {
                if (value == "")
                    throw new Exception("Dica precisa ter um valor");
                dica = value.PadRight(tamanhoDica, ' ').Substring(0, tamanhoDica);
            }
        }

        public bool[] Acertos
        {
            get => acertou;
        }


        // método para inicializar o vetor acertou com false
        //ex.: false false false false false false false false false false false false false false false
        private void IniciarContagemDeAcertos()
        {
            for(int i = 0; i < tamanhoPalavra; i++)
            {
                acertou[i] = false;
            }
        }


        // método para ler uma linha de dados e armazenar nas propriedades da classe
        public void LerDados(string linhaDeDados)
        {
            Palavra = linhaDeDados.Substring(inicioPalavra, tamanhoPalavra);
            Dica = linhaDeDados.Substring(inicioDica, tamanhoDica);
        }

        // método para formatação dos dados a fim de gravar no arquivo
        public string Formatar()
        {
            return Palavra + Dica;
        }

    // método para verificar se a letra existe na palavra e marcar as posições certas
    // ao acertar, o vetor acertou altera para true a posição da letra adivinhada

    // ex.: ao acertar a letra "O" da palavra COMPUTADOR, o vetor acertou, por acertar as posições acertou[1] e acertou[8], passaria a ser:

    //  c     o     m     p     u     t     a     d    o    r
    // [false true false false false false false false true false false false false false false]
    //   0      1    2     3     4     5     6     7     8     9   10    11    12    13     14
    public int VerificarLetra(char letra, out bool[] acertos)
    {
        int qtdeAcertos = 0;
        acertos = new bool[Palavra.Length];

        for (int i = 0; i < Palavra.Length; i++)
        {
            if (char.ToUpper(Palavra[i]) == char.ToUpper(letra))
            {
                acertos[i] = true;
                qtdeAcertos++;
            }
        }

        return qtdeAcertos;
    }


    public int CompareTo(Dicionario outra)
        {
            return this.Palavra.CompareTo(outra.Palavra);
        }

        public override string ToString()
        {
            return Palavra + " " + Dica;
        }

        public string FormatoDeArquivo()
        {
            return $"{palavra}{dica}";
        }
}

