using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace apForcaProj2
{
    public partial class FrmForca : Form
    {
        // Matheus Carletti dos Santos 23532
        // Vinícius Bernardo Guerreiro 24488

        ListaDupla<Dicionario> listaDicionario;
        PrivateFontCollection fonteCurlz = new PrivateFontCollection();

        public FrmForca()
        {
            InitializeComponent();
            listaDicionario = new ListaDupla<Dicionario>();
            CarregarFonte();
        }

        // método para utilização da fonte escolhida
        private void CarregarFonte()
        {
            fonteCurlz.AddFontFile("CURLZ__.ttf");

            Font fonteForca = new Font(fonteCurlz.Families[0], 20);

            //utilização nos labels determinados
            lblForca.Font = fonteForca;


        }

        private void FazerLeitura()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Escolha o arquivo de entrada";
            ofd.Filter = "Arquivos texto (*.txt)|*.txt";

            if (ofd.ShowDialog() == DialogResult.OK)
            {

                using (StreamReader leitor = new StreamReader(ofd.FileName))
                {
                    while (!leitor.EndOfStream)
                    {
                        string linha = leitor.ReadLine();

                        if (linha.Length >= 30)
                        {
                            string palavra = linha.Substring(0, 30).TrimEnd();
                            string dica = linha.Substring(30).Trim();

                            Dicionario dicionario = new Dicionario(palavra, dica);
                            listaDicionario.InserirAposFim(dicionario);
                        }
                    }
                }
            }
        }

        private void FrmForca_Load(object sender, EventArgs e)
        {
            // fazer a leitura do arquivo escolhido pelo usuário e armazená-lo na listaDicionario
            // posicionar o ponteiro atual no início da lista duplamente ligada
            // Exibir o Registro Atual;

            FazerLeitura();
            listaDicionario.PosicionarNoInicio();
            ExibirRegistroAtual();
            ExibirDadosNoDataGridView(listaDicionario, dgDicionario, Direcao.paraFrente);
        }

        private void FrmForca_FormClosing(object sender, FormClosingEventArgs e)
        {
            // salvar o arquivo
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Escolha o arquivo de saída";
            sfd.Filter = "Arquivos texto (*.txt)|*.txt";

            // se o usuário selecionar um arquivo 
            // chama a função GravarDados da ListaDupla, passando o caminho do arquivo escolhido
            // percorre toda a lista e grava os dados de cada nó no arquivo

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                listaDicionario.GravarDados(sfd.FileName);
            }
        }

        private void ExibirRegistroAtual()
        {
            // se a lista não está vazia:
            // acessar o nó atual e exibir seus campos em txtDica e txtPalavra
            // atualizar no status bar o número do registro atual / quantos nós na lista

            if (!listaDicionario.EstaVazia && listaDicionario.Atual != null)
            {
                Dicionario atual = listaDicionario.Atual.Info;
                txtPalavra.Text = atual.Palavra;
                txtDica.Text = atual.Dica;
                slRegistro.Text = $"{listaDicionario.NumeroDoNoAtual + 1}/{listaDicionario.QuantosNos}";
            }
            else
            {
                MessageBox.Show("Nenhum registro selecionado para exibir.");
            }
        }

        private void ExibirDadosNoDataGridView(ListaDupla<Dicionario> aLista, DataGridView dgv, Direcao qualDirecao)
        {
            dgv.Rows.Clear();
            var dadosDaLista = aLista.Listagem(qualDirecao);
            foreach (Dicionario dicionario in dadosDaLista)
            {
                // Adiciona uma nova linha no DataGridView com os dados
                dgv.Rows.Add(dicionario.Palavra.Trim(), dicionario.Dica.Trim());
            }
        }


        private void btnInicio_Click(object sender, EventArgs e)
        {
            // posicionar o ponteiro atual no início da lista duplamente ligada
            // Exibir o Registro Atual;
            listaDicionario.PosicionarNoInicio();
            ExibirRegistroAtual();
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            // Retroceder o ponteiro atual para o nó imediatamente anterior 
            // Exibir o Registro Atual;
            listaDicionario.Retroceder();
            ExibirRegistroAtual();
        }

        private void btnProximo_Click(object sender, EventArgs e)
        {
            // Retroceder o ponteiro atual para o nó seguinte 
            // Exibir o Registro Atual;
            listaDicionario.Avancar();
            ExibirRegistroAtual();
        }

        private void btnFim_Click(object sender, EventArgs e)
        {
            // posicionar o ponteiro atual no último nó da lista 
            // Exibir o Registro Atual;
            listaDicionario.PosicionarNoFinal();
            ExibirRegistroAtual();
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            // verifica se Palavra e Dica foram preenchidos
            // se sim, tenta inseri-lo na lista 
            // senão, atualiza a exibição da lista
            if (!string.IsNullOrWhiteSpace(txtPalavra.Text) && !string.IsNullOrWhiteSpace(txtDica.Text))
            {
                Dicionario novaPalavra = new Dicionario(txtPalavra.Text, txtDica.Text);

                if (!listaDicionario.InserirEmOrdem(novaPalavra))
                    MessageBox.Show("Palavra já cadastrada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                {
                    ExibirDadosNoDataGridView(listaDicionario, dgDicionario, Direcao.paraFrente);
                    MessageBox.Show("Dicionario incluído com sucesso.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else // se os campos estiverem vazios, o usuário recebe uma mensagem 
            {
                MessageBox.Show("Digite RA e Nome para incluir.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            // alterar a dica e guardar seu novo valor no nó exibido
            if (!listaDicionario.EstaVazia && listaDicionario.Atual != null)
            {
                Dicionario atual = listaDicionario.Atual.Info;
                atual.Dica = txtDica.Text;
                ExibirRegistroAtual();
            }
            else
            {
                MessageBox.Show("Nenhum registro selecionado para editar.");
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void btnBuscar_Click(object sender, EventArgs e)
        {

        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {

        }
    }
}
