using System;
using System.Collections.Generic;
using System.IO;

// Matheus Carletti dos Santos 23532
// Vinícius Bernardo Guerreiro 24488
public enum Direcao { paraFrente, paraTras };

public class ListaDupla<Dado>
             where Dado : IComparable<Dado>, IRegistro
{
    NoDuplo<Dado> primeiro, ultimo,
                  atual;   // é usado para percorrer a lista e mostrar
                           // o nó que está sendo visitado a cada momento
    int quantosNos;
    bool primeiroAcessoDoPercurso;
    int numeroDoNoAtual;

    public void PosicionarNoInicio()
    {
        atual = primeiro;
        numeroDoNoAtual = 0;
    }

    public void PosicionarNoFinal()
    {
        atual = ultimo;
        numeroDoNoAtual = quantosNos - 1;
    }

    public void Avancar()
    {
        if (atual != null && atual.Prox != null)
        {
            atual = atual.Prox;
            numeroDoNoAtual++;
        }
        else if (atual == ultimo)
        {
        }
    }

    public void Retroceder()
    {
        if (atual != null && atual.Ant != null)
        {
            atual = atual.Ant;
            numeroDoNoAtual--;
        }
        else if (atual == primeiro)
        {
        }
    }


    public void PosicionarEm(int indice)
    {
        // fica para vocês fazerem
        // verificar se indice é válido ( >= 0 && < quantosNos)
        // se for valido:
        //    atual aponta o primeiro nó;
        //    percorre "indice" nós com o ponteiro atual sequencial
        //      atualiza a variável numeroDoNoAtual

        if (indice >= 0 && indice < quantosNos)
        {
            atual = primeiro;
            numeroDoNoAtual = 0;

            for (int i = 0; i < indice; i++)
            {
                atual = atual.Prox;
                numeroDoNoAtual++;
            }
        }
    }

    public Dado this[int indice]
    {
        get
        {
            PosicionarEm(indice);
            return atual.Info;
        }
        set
        {
            PosicionarEm(indice);
            atual.Info = value;
        }
    }

    public List<Dado> Listagem(Direcao qualDirecao)
    {
        var dados = new List<Dado>();
        NoDuplo<Dado> aux; // ponteiro de percurso local, para não afetar o 'atual' oficial da lista

        if (qualDirecao == Direcao.paraFrente)
        {
            aux = primeiro;        // posiciona ponteiro de percurso no 1o nó
            while (aux != null)    // enquanto houver nós a visitar
            {
                dados.Add(aux.Info);   // inclui no listbox os dados do nó visitado agora
                aux = aux.Prox;        // avança o ponteiro de percurso para o nó seguinte
            }
        }
        else
        {
            aux = ultimo;          // posiciona ponteiro de percurso no último nó
            while (aux != null)    // enquanto houver nós a visitar
            {
                dados.Add(aux.Info);   // inclui no listbox os dados do nó visitado agora
                aux = aux.Ant;         // retrocede o ponteiro de percurso para o nó anterior
            }
        }

        return dados;
    }


    public ListaDupla()
    {
        primeiro = ultimo = atual = null;
        quantosNos = numeroDoNoAtual = 0;
        primeiroAcessoDoPercurso = false;
    }

    public bool EstaVazia
    {
        get => primeiro == null;
    }
    public NoDuplo<Dado> Primeiro
    {
        get => primeiro;
    }
    public NoDuplo<Dado> Ultimo
    {
        get => ultimo;
    }
    public int QuantosNos
    {
        get => quantosNos;
    }

    public void InserirAntesDoInicio(Dado novoDado)
    {
        var novoNo = new NoDuplo<Dado>(novoDado);

        if (EstaVazia)
            ultimo = novoNo;

        novoNo.Prox = primeiro;
        primeiro = novoNo;
        quantosNos++;
    }

    public void InserirAposFim(Dado novoDado)
    {
        var novoNo = new NoDuplo<Dado>(novoDado);

        if (EstaVazia)
            primeiro = novoNo;
        else
            ultimo.Prox = novoNo;

        ultimo = novoNo;
        quantosNos++;
    }

    public void InserirAposFim(NoDuplo<Dado> noExistente)
    {
        if (noExistente != null)
        {
            if (EstaVazia)
                primeiro = noExistente;
            else
                ultimo.Prox = noExistente;

            ultimo = noExistente;
            noExistente.Prox = null;
            quantosNos++;
        }
    }

    public bool Existe(Dado outroProcurado)
    {
        //anterior = null;
        atual = primeiro;

        //	Em seguida, é verificado se a lista está vazia. Caso esteja, é
        //	retornado false ao local de chamada, indicando que a chave não foi
        //	encontrada, e atual e anterior ficam valendo null
        if (EstaVazia)
            return false;

        // a lista não está vazia, possui nós
        // dado procurado é menor que o primeiro dado da lista:
        // portanto, dado procurado não existe
        if (outroProcurado.CompareTo(primeiro.Info) < 0)
            return false;

        // dado procurado é maior que o último dado da lista:
        // portanto, dado procurado não existe
        if (outroProcurado.CompareTo(ultimo.Info) > 0)
        {
            // anterior = ultimo;
            atual = null;
            return false;
        }

        //	caso não tenha sido definido que a chave está fora dos limites de 
        //	chaves da lista, vamos procurar no seu interior
        //	o apontador atual indica o primeiro nó da lista e consideraremos que
        //	ainda não achou a chave procurada nem chegamos ao final da lista
        bool achou = false;
        bool fim = false;

        //	repete os comandos abaixo enquanto não achou o RA nem chegou ao
        //	final da pesquisa
        while (!achou && !fim)
            // se o apontador atual vale null, indica final físico da lista
            if (atual == null)
                fim = true;
            // se não chegou ao final da lista, verifica o valor da chave atual
            else
              // verifica igualdade entre chave procurada e chave do nó atual
              if (outroProcurado.CompareTo(atual.Info) == 0)
                achou = true;
            else
                // se chave atual é maior que a procurada, significa que
                // a chave procurada não existe na lista ordenada e, assim,
                // termina a pesquisa indicando que não achou. Anterior
                // aponta o nó anterior ao atual, que foi acessado na
                // última repetição
                if (atual.Info.CompareTo(outroProcurado) > 0)
                fim = true;
            else
            {
                // se não achou a chave procurada nem uma chave > que ela,
                // então a pesquisa continua, de maneira que o apontador
                // anterior deve apontar o nó atual e o apontador atual
                // deve seguir para o nó seguinte
                //    anterior = atual;
                atual = atual.Prox;
            }

        // por fim, caso a pesquisa tenha terminado, o apontador atual
        // aponta o nó onde está a chave procurada, caso ela tenha sido
        // encontrada, ou aponta o nó onde ela deveria estar para manter a
        // ordenação da lista. O apontador anterior aponta o nó anterior
        // ao atual
        return achou;   // devolve o valor da variável achou, que indica
    }

    public NoDuplo<Dado> Atual => atual;

    public int NumeroDoNoAtual { get => numeroDoNoAtual; set => numeroDoNoAtual = value; }

    public bool InserirEmOrdem(Dado dados)
    {
        if (Existe(dados))     // Existe() configura anterior e atual
            return false;

        // aqui temos certeza de que a chave não existe
        // guardaremos os dados no novo nó
        if (EstaVazia)                  // se a lista está vazia, então o 	
            InserirAntesDoInicio(dados);  // dado ficará como primeiro da lista
        else
            // testa se nova chave < primeira chave
            if (/*anterior == null && */ atual != null)
            InserirAntesDoInicio(dados); // liga novo nó antes do primeiro
        else
              // testa se nova chave > última chave
              if (/*anterior != null && */ atual == null)
            InserirAposFim(dados);
        else
            InserirNoMeio(dados);  // insere entre os nós anterior e atual

        return true;  // conseguiu incluir pois não é repetido
    }

    private void InserirNoMeio(Dado dados)
    {
        var novo = new NoDuplo<Dado>(dados);

        // liga novo no anterior e no atual
        novo.Prox = atual;
        novo.Ant = atual.Ant;

        if (atual.Ant != null)
            atual.Ant.Prox = novo;
        atual.Ant = novo;

        quantosNos++;
    }


    public bool Remover(Dado dadoARemover)
    {
        if (EstaVazia)
            return false;

        if (!Existe(dadoARemover))
            return false;

        if (atual == primeiro)
        {
            primeiro = primeiro.Prox;
            if (primeiro != null)
                primeiro.Ant = null;
            else
                ultimo = null;
        }
        else if (atual == ultimo)
        {
            ultimo = ultimo.Ant;
            if (ultimo != null)
                ultimo.Prox = null;
            else
                primeiro = null;
        }
        else
        {
            atual.Ant.Prox = atual.Prox;
            atual.Prox.Ant = atual.Ant;
        }

        quantosNos--;
        return true;
    }


    public void GravarDados(string nomeArq)
    {
        var arquivo = new StreamWriter(nomeArq);
        atual = primeiro;
        while (atual != null)
        {
            arquivo.WriteLine(atual.Info.FormatoDeArquivo());
            atual = atual.Prox;
        }
        arquivo.Close();
    }

}
