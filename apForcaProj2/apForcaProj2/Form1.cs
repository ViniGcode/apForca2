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
        int tempo;
        private Random random = new Random(); // sorteador
        private Dicionario palavraSorteada;
        private int qtdErros = 0; // contagem de erros
        private int qtdAcertos = 0; // contagem de acertos
        private bool aguardandoNome = true; 

        // array para facilitar as atribuições de cada tag e dos clicks de cada btn
        private string[] teclado = new string[] { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P",
                                                  "A", "S", "D", "F", "G", "H", "J", "K", "L", "Ç",
                                                  "Z", "X", "C", "V", "B", "N", "M" };



        public FrmForca()
        {
            InitializeComponent();
            listaDicionario = new ListaDupla<Dicionario>();
            CarregarFonte();
            timerTempoRestante.Interval = 1000; // define o intervalo do timer que exibe o Tempo Restante para 1000 milissegundos ==> 1 segundo

        }

        // método para utilização da fonte escolhida
        private void CarregarFonte()
        {
            fonteCurlz.AddFontFile("CURLZ__.ttf");

            Font fonteForca = new Font(fonteCurlz.Families[0], 20);

            //utilização nos labels determinados
            lblForca.Font = fonteForca;


        }

        // método para abrir o arquivo .txt contendo as palavras e dicas
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

        // método de configuração das ações do formulário ao ser iniciado 
        private void FrmForca_Load(object sender, EventArgs e)
        {
            // fazer a leitura do arquivo escolhido pelo usuário e armazená-lo na listaDicionario
            // posicionar o ponteiro atual no início da lista duplamente ligada
            // chama ExibirRegistroAtual() para posicionar o ponteiro
            // exibe os dados carregados no DataGridView
            // define as tags e eventos de clique dos botões do teclado a partir de um for que percorre o array teclado[]

            FazerLeitura();
            listaDicionario.PosicionarNoInicio();
            ExibirRegistroAtual();
            ExibirDadosNoDataGridView(listaDicionario, dgDicionario, Direcao.paraFrente);

            btnIniciarJogo.Enabled = false; // só habilita após aguardandoNome = false
            aguardandoNome = true;

            // letras do teclado
            for (int i = 0; i < teclado.Length; i++)
            {
                Button btn = this.Controls.Find("btn" + teclado[i], true).FirstOrDefault() as Button;
                if (btn != null)
                {
                    btn.Tag = teclado[i];
                    btn.Click += btnLetra_Click;
                }
            }

        }

        // método de para salvar o arquivo .txt ao fechar o formulário 
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

        // ------------------------------------------ tabCadastro --------------------------------------------------------------
        private void ExibirDadosNoDataGridView(ListaDupla<Dicionario> aLista, DataGridView dgv, Direcao qualDirecao)
        {
            // limpa os dados das linhas do DataGridView
            // faz a listagem dos dados na lista na direção escolhida
            // adiciona uma nova linha no DataGridView com os dados
            dgv.Rows.Clear();
            var dadosDaLista = aLista.Listagem(qualDirecao);
            foreach (Dicionario dicionario in dadosDaLista)
            {
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

        // ---------------------------------------------------------------------------------------------------------------------

        // --------------------------------------------- tabForca --------------------------------------------------------------

        // método de configuração do timer 
        // exibe o tempo restante em txtTempoRestante 
        private void timerTempoRestante_Tick(object sender, EventArgs e)
        {
            tempo--;
            txtTempoRestante.Text = tempo.ToString(); 

            if (tempo <= 0)
            {
                timerTempoRestante.Stop();
                MessageBox.Show("Tempo esgotado!");
                tabControl1.TabPages.Add(tabCadastro);
            }
        }

        private void btnIniciarJogo_Click(object sender, EventArgs e)
        {
            // verifica se a lista está vazia
            // se estiver, retorna e impossibilita o jogo
            // oculta a aba de cadastro para o jogador não trapacear
            // sorteia uma palavra a partir da listagem crescente do dicionario
            // configura o DataGridView limpando linhas e colunas anteriores
            // adiciona uma coluna para cada letra da palavra 
            // adiciona linha com traços (_)
            // se houver espaço ou hífen, mantém o caractere original
            // habilita novamente todos os botões que possuem tag (os botões do teclado)
            // de modo que evita que continuem desabilitados devido a jogos anteriores
            // limpa a dica exibida, a contagem de erros, acertos e desmarca o rbComDica
            // inicia contagem do tempo e estipula para 90s

            if (listaDicionario.EstaVazia)
            {
                MessageBox.Show("Não há palavras cadastradas!");
                return;
            }

            tabControl1.TabPages.Remove(tabCadastro);

            var lista = listaDicionario.Listagem(Direcao.paraFrente);
            int indiceSorteado = random.Next(lista.Count);
            palavraSorteada = lista[indiceSorteado];

            dgvPalavraForca.Columns.Clear();
            dgvPalavraForca.Rows.Clear();

            int qtdeLetras = palavraSorteada.Palavra.Trim().Length;


            for (int i = 0; i < qtdeLetras; i++)
            {
                var coluna = new DataGridViewTextBoxColumn();
                coluna.Width = 35;
                coluna.HeaderText = ""; // tira texto do cabeçalho
                coluna.ReadOnly = true;
                dgvPalavraForca.Columns.Add(coluna);
            }

            dgvPalavraForca.Rows.Add();
            for (int i = 0; i < qtdeLetras; i++)
            {
                char letra = palavraSorteada.Palavra[i];
                if (letra == ' ' || letra == '-')
                    dgvPalavraForca.Rows[0].Cells[i].Value = letra;
                else
                    dgvPalavraForca.Rows[0].Cells[i].Value = "_";
            }

            // travar o dgv para ter só essa linha e sem cabeçalho de linha
            dgvPalavraForca.RowHeadersVisible = false;
            dgvPalavraForca.AllowUserToAddRows = false;
            dgvPalavraForca.AllowUserToResizeColumns = false;

            // ajustar altura da linha
            dgvPalavraForca.RowTemplate.Height = 40;

            foreach (Control ctrl in tabForca.Controls)
            {
                // percorre todos os botões do Form, mas pega só os que possui Tag
                // verifica se o Tag possui somente uma letra, como forma de assegurar que atinge somente o teclado
                // ativa todos de novo
                if (ctrl is Button btn && btn.Tag != null && btn.Tag.ToString().Length == 1)
                {
                    btn.Enabled = true;
                }
            }

            txtDicaExibida.Clear();
            rbComDica.Checked = false;
            txtErros.Clear();
            txtPontos.Clear();
            tempo = 90;
            timerTempoRestante.Start();
            qtdErros = 0;
            qtdAcertos = 0;
        }

        private void rbComDica_CheckedChanged(object sender, EventArgs e)
        {
            // se o RadioButton for marcado e existir uma palavra sorteada
            // exibe a dica dela no txtDicaExibida
            // senão, limpa o txtDicaExibida

            if (rbComDica.Checked)
            {
                if (palavraSorteada != null)
                    txtDicaExibida.Text = palavraSorteada.Dica.Trim();
                else
                    txtDicaExibida.Text = "Nenhuma palavra sorteada.";
            }
            else
            {
                txtDicaExibida.Clear();
            }
        }

        // método de clique dos botões do teclado da forca
        private void btnLetra_Click(object sender, EventArgs e)
        {
            // obtém o botão clicado e a letra correspondente
            // inicialmente será utilizado para preencher o nome do usuário em txtSeuNome
            // após o envio do nome, aguardandoNome passa a ser false, o que permite o início do jogo
            // durante o jogo, os botões clicados são desativados para evitar clique repetido
            // chama o método da palavra sorteada e atualiza o vetor de acertos e retorna se a letra existe
            // se a letra foi encontrada, atualiza as posições do DataGridView baseadas no vetor de acertos
            // se errou, incrementa o qtdErros e atualiza txtErros
            // faz a contagem de acertos e atualiza txtPontos

            Button btnClicado = sender as Button;
            string letra = btnClicado.Tag.ToString();

            if (aguardandoNome)
            {
                txtSeuNome.Text += letra;
            }
            else
            {
                btnClicado.Enabled = false;

                // usa método VerificarLetra do Dicionario
                bool[] acertos;
                int qtdeAcertosNaPalavra = palavraSorteada.VerificarLetra(letra[0], out acertos);

                if (qtdeAcertosNaPalavra > 0)
                {
                    qtdAcertos += qtdeAcertosNaPalavra;
                    txtPontos.Text = qtdAcertos.ToString();

                    for (int i = 0; i < acertos.Length; i++)
                    {
                        if (acertos[i])
                        {
                            dgvPalavraForca.Rows[0].Cells[i].Value = letra;
                        }
                    }
                }
                else
                {
                    qtdErros++;
                    txtErros.Text = qtdErros.ToString();
                }
            }
        }

        // método que permite o usuário corrigir o nome caso escreva errado
        // verifica se o nome ainda não foi enviado e se o txtSeuNome já possui algum caracter
        private void btnBackSpace_Click(object sender, EventArgs e)
        {
            if (aguardandoNome && txtSeuNome.Text.Length > 0)
            {
                txtSeuNome.Text = txtSeuNome.Text.Substring(0, txtSeuNome.Text.Length - 1);
            }
        }

        // método para enviar o nome do usuário e habilitar o btnIniciarJogo
        private void btnEnter_Click(object sender, EventArgs e)
        {
            if (aguardandoNome)
            {
                if (string.IsNullOrWhiteSpace(txtSeuNome.Text))
                {
                    MessageBox.Show("Digite seu nome antes de continuar.");
                    return;
                }

                aguardandoNome = false;
                btnIniciarJogo.Enabled = true;
                MessageBox.Show($"Bem-vindo, {txtSeuNome.Text.Trim()}! Agora você pode começar o jogo.");
            }
        }
         // método para a barra de espaço (" ")
         // verifica se o jogo ainda está aguardando o envio do nome do usuário para possibilitar o uso da barra
        private void btnSpaceBar_Click(object sender, EventArgs e)
        {
            if (aguardandoNome)
            {
                txtSeuNome.Text += " ";
            }
        }
    }
}
