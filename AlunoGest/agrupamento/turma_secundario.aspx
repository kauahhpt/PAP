<%@ Page Title="" Language="C#" MasterPageFile="~/agrupamento/modeloAgrupamento.Master"
    AutoEventWireup="true" CodeBehind="turma_secundario.aspx.cs"
    Inherits="AlunoGest.agrupamento.turma_secundario"
    MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">

    <style type="text/css">
        /*
            Caixa visual usada nas zonas de opções.
        */
        .caixa-opcoes {
            border: 1px solid #ced4da;
            border-radius: 6px;
            padding: 12px;
            background-color: #f8f9fa;
        }

        /*
            Pequeno texto de ajuda apresentado acima das opções.
        */
        .texto-ajuda-opcoes {
            display: block;
            margin-bottom: 8px;
        }

        /*
            Faz com que as CheckBoxList fiquem alinhadas à esquerda.
        */
        .checklist-esquerda {
            text-align: left;
        }

        .checklist-esquerda input[type="checkbox"] {
            margin-right: 6px;
        }

        .checklist-esquerda label {
            display: inline-block;
            margin-bottom: 6px;
        }
    </style>

    <script type="text/javascript">

        // =========================================================
        // Vai buscar todas as checkbox de um controlo ASP.NET
        // CheckBoxList.
        // =========================================================
        function obterCheckBoxes(idControlo) {
            var controlo = document.getElementById(idControlo);

            if (!controlo)
                return [];

            return controlo.querySelectorAll('input[type="checkbox"]');
        }

        // =========================================================
        // Conta quantas checkbox estão selecionadas.
        // =========================================================
        function contarSelecionadas(lista) {
            var total = 0;

            for (var i = 0; i < lista.length; i++) {
                if (lista[i].checked)
                    total++;
            }

            return total;
        }

        // =========================================================
        // BLOCO B
        // 10.º e 11.º anos:
        // - podem ser escolhidas apenas 2 disciplinas.
        // - quando já estão 2 selecionadas, as restantes ficam
        //   desativadas.
        // =========================================================
        function atualizarEstadoBlocoB() {
            var checks = obterCheckBoxes('<%= cblB.ClientID %>');
            var total = contarSelecionadas(checks);

            for (var i = 0; i < checks.length; i++) {
                if (checks[i].checked) {
                    checks[i].disabled = false;
                }
                else {
                    if (total >= 2)
                        checks[i].disabled = true;
                    else
                        checks[i].disabled = false;
                }
            }
        }

        // =========================================================
        // Quando o utilizador clica numa checkbox do bloco B:
        // - se tentar escolher uma 3.ª, ela volta a false;
        // - depois atualizamos o estado visual.
        // =========================================================
        function tratarCliqueBlocoB(checkClicada) {
            var checks = obterCheckBoxes('<%= cblB.ClientID %>');
            var total = contarSelecionadas(checks);

            if (total > 2) {
                checkClicada.checked = false;
            }

            atualizarEstadoBlocoB();
        }

        // =========================================================
        // BLOCOS C E D
        // 12.º ano:
        // - no total só podem ficar 2 disciplinas;
        // - pelo menos 1 tem de ser do grupo C.
        //
        // Regra prática no JavaScript:
        // - se já existem 2 escolhidas, bloqueia as restantes;
        // - se existe 1 escolhida e ela é do grupo D, então a 2.ª
        //   obrigatoriamente tem de ser do grupo C, por isso as
        //   restantes do grupo D ficam bloqueadas.
        // =========================================================
        function atualizarEstadoBlocosCD() {
            var checksC = obterCheckBoxes('<%= cblC.ClientID %>');
            var checksD = obterCheckBoxes('<%= cblD.ClientID %>');

            var totalC = contarSelecionadas(checksC);
            var totalD = contarSelecionadas(checksD);
            var total = totalC + totalD;

            // Primeiro desbloqueamos tudo
            for (var i = 0; i < checksC.length; i++) {
                if (!checksC[i].checked)
                    checksC[i].disabled = false;
            }

            for (var j = 0; j < checksD.length; j++) {
                if (!checksD[j].checked)
                    checksD[j].disabled = false;
            }

            // Se já existem 2 selecionadas, bloqueia todas as outras
            if (total >= 2) {
                for (var k = 0; k < checksC.length; k++) {
                    if (!checksC[k].checked)
                        checksC[k].disabled = true;
                }

                for (var l = 0; l < checksD.length; l++) {
                    if (!checksD[l].checked)
                        checksD[l].disabled = true;
                }
            }

            // Se só existe 1 selecionada e essa está no grupo D,
            // a próxima tem de ser obrigatoriamente do grupo C.
            if (total == 1 && totalC == 0 && totalD == 1) {
                for (var m = 0; m < checksD.length; m++) {
                    if (!checksD[m].checked)
                        checksD[m].disabled = true;
                }
            }
        }

        // =========================================================
        // Quando o utilizador clica numa checkbox dos grupos C ou D:
        // - se tentar ficar com mais de 2, a última volta a false;
        // - depois atualizamos o estado visual.
        // =========================================================
        function tratarCliqueBlocosCD(checkClicada) {
            var checksC = obterCheckBoxes('<%= cblC.ClientID %>');
            var checksD = obterCheckBoxes('<%= cblD.ClientID %>');

            var total = contarSelecionadas(checksC) + contarSelecionadas(checksD);

            if (total > 2) {
                checkClicada.checked = false;
            }

            atualizarEstadoBlocosCD();
        }

        // =========================================================
        // Liga os eventos do bloco B.
        // Usamos addEventListener para não estragar o JavaScript que
        // o próprio ASP.NET usa internamente.
        // =========================================================
        function ligarEventosBlocoB() {
            var checks = obterCheckBoxes('<%= cblB.ClientID %>');

            for (var i = 0; i < checks.length; i++) {

                // Evita ligar o mesmo evento várias vezes
                if (checks[i].getAttribute("data-evento-b") != "1") {
                    checks[i].setAttribute("data-evento-b", "1");

                    checks[i].addEventListener("click", function () {
                        tratarCliqueBlocoB(this);
                    });
                }
            }

            atualizarEstadoBlocoB();
        }

        // =========================================================
        // Liga os eventos dos grupos C e D.
        // =========================================================
        function ligarEventosBlocosCD() {
            var checksC = obterCheckBoxes('<%= cblC.ClientID %>');
            var checksD = obterCheckBoxes('<%= cblD.ClientID %>');

            for (var i = 0; i < checksC.length; i++) {
                if (checksC[i].getAttribute("data-evento-c") != "1") {
                    checksC[i].setAttribute("data-evento-c", "1");

                    checksC[i].addEventListener("click", function () {
                        tratarCliqueBlocosCD(this);
                    });
                }
            }

            for (var j = 0; j < checksD.length; j++) {
                if (checksD[j].getAttribute("data-evento-d") != "1") {
                    checksD[j].setAttribute("data-evento-d", "1");

                    checksD[j].addEventListener("click", function () {
                        tratarCliqueBlocosCD(this);
                    });
                }
            }

            atualizarEstadoBlocosCD();
        }

        // =========================================================
        // Prepara a página.
        // =========================================================
        function prepararPagina() {
            ligarEventosBlocoB();
            ligarEventosBlocosCD();
        }

        document.addEventListener("DOMContentLoaded", function () {
            prepararPagina();
        });

        // Em Web Forms, após postback a página é reconstruída.
        // Esta função ajuda a voltar a ligar os eventos.
        function pageLoad() {
            prepararPagina();
        }

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">

    <div class="container">

        <h1>Turma do Ensino Secundário</h1>

        <div class="mb-3">
            <asp:Label ID="lblEscola" runat="server" CssClass="fw-semibold"></asp:Label>
        </div>

        <div class="mb-3">
            <asp:Label ID="lblModo" runat="server" CssClass="fw-semibold text-primary"></asp:Label>
        </div>

        <asp:Label ID="lblMensagem" runat="server" Visible="false"></asp:Label>

        <div id="painelFormulario" runat="server" class="mt-4">

            <div class="row mb-2">
                <label for="ddlAnoLetivo" class="col-sm-2 col-form-label text-end">Ano letivo</label>
                <div class="col-sm-3">
                    <asp:DropDownList ID="ddlAnoLetivo" runat="server" CssClass="form-select border-secondary"></asp:DropDownList>
                </div>
            </div>

            <div class="row mb-2">
                <label for="ddlPlano" class="col-sm-2 col-form-label text-end">Curso</label>
                <div class="col-sm-5">
                    <asp:DropDownList ID="ddlPlano" runat="server"
                        CssClass="form-select border-secondary"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlPlano_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
            </div>

            <div class="row mb-2">
                <label for="ddlAno" class="col-sm-2 col-form-label text-end">Ano</label>
                <div class="col-sm-3">
                    <asp:DropDownList ID="ddlAno" runat="server"
                        CssClass="form-select border-secondary"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlAno_SelectedIndexChanged">
                        <asp:ListItem Text="-- selecionar --" Value=""></asp:ListItem>
                        <asp:ListItem Text="10.º ano" Value="10"></asp:ListItem>
                        <asp:ListItem Text="11.º ano" Value="11"></asp:ListItem>
                        <asp:ListItem Text="12.º ano" Value="12"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div class="row mb-3">
                <label for="txtCodigoTurma" class="col-sm-2 col-form-label text-end">Código da turma</label>
                <div class="col-sm-3">
                    <asp:TextBox ID="txtCodigoTurma" runat="server" CssClass="form-control border-secondary"></asp:TextBox>
                </div>
            </div>

            <div id="painelLE" runat="server" visible="false" class="mb-3">
                <div class="row">
                    <label for="ddlLE" class="col-sm-2 col-form-label text-end">Língua estrangeira</label>
                    <div class="col-sm-6">
                        <div class="caixa-opcoes">
                            <small class="text-muted texto-ajuda-opcoes">
                                Nos 10.º e 11.º anos é necessário escolher uma língua estrangeira do grupo A.
                            </small>

                            <asp:DropDownList ID="ddlLE" runat="server"
                                CssClass="form-select border-secondary"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="Opcoes_Changed">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
            </div>

            <div id="painelB" runat="server" visible="false" class="mb-3">
                <div class="row">
                    <label class="col-sm-2 col-form-label text-end">Grupo de opções B</label>
                    <div class="col-sm-6">
                        <div class="caixa-opcoes">
                            <small class="text-muted texto-ajuda-opcoes">
                                Nos 10.º e 11.º anos é necessário selecionar duas disciplinas do grupo B.
                            </small>

                            <asp:CheckBoxList ID="cblB" runat="server"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="Opcoes_Changed"
                                RepeatDirection="Vertical"
                                CssClass="checklist-esquerda">
                            </asp:CheckBoxList>
                        </div>
                    </div>
                </div>
            </div>

            <div id="painelC" runat="server" visible="false" class="mb-3">
                <div class="row">
                    <label class="col-sm-2 col-form-label text-end">Grupo de opções C</label>
                    <div class="col-sm-6">
                        <div class="caixa-opcoes">
                            <small class="text-muted texto-ajuda-opcoes">
                                No 12.º ano devem ser escolhidas duas disciplinas dos grupos C e D, sendo obrigatório selecionar pelo menos uma do grupo C.
                            </small>

                            <asp:CheckBoxList ID="cblC" runat="server"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="Opcoes_Changed"
                                RepeatDirection="Vertical"
                                CssClass="checklist-esquerda">
                            </asp:CheckBoxList>
                        </div>
                    </div>
                </div>
            </div>

            <div id="painelD" runat="server" visible="false" class="mb-3">
                <div class="row">
                    <label class="col-sm-2 col-form-label text-end">Grupo de opções D</label>
                    <div class="col-sm-6">
                        <div class="caixa-opcoes">
                            <small class="text-muted texto-ajuda-opcoes">
                                Depois de escolheres uma disciplina do grupo D, a outra tem de ficar no grupo C.
                            </small>

                            <asp:CheckBoxList ID="cblD" runat="server"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="Opcoes_Changed"
                                RepeatDirection="Vertical"
                                CssClass="checklist-esquerda">
                            </asp:CheckBoxList>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row mb-2">
                <label for="chkPLNM" class="col-sm-2 col-form-label text-end">PLNM</label>
                <div class="col-sm-6 pt-2">
                    <asp:CheckBox ID="chkPLNM" runat="server"
                        Text="Associar Português Língua Não Materna"
                        AutoPostBack="true"
                        OnCheckedChanged="Opcoes_Changed" />
                </div>
            </div>

            <div class="row mb-3">
                <label for="chkEMR" class="col-sm-2 col-form-label text-end">EMR</label>
                <div class="col-sm-6 pt-2">
                    <asp:CheckBox ID="chkEMR" runat="server"
                        Text="Associar Educação Moral e Religiosa"
                        AutoPostBack="true"
                        OnCheckedChanged="Opcoes_Changed" />
                </div>
            </div>

            <div class="row mb-3">
                <label for="chkAtiva" class="col-sm-2 col-form-label text-end">Ativa</label>
                <div class="col-sm-3 pt-2">
                    <asp:CheckBox ID="chkAtiva" runat="server" Checked="true" />
                </div>
            </div>

            <div class="row mb-4">
                <div class="col-sm-2"></div>
                <div class="col-sm-8 text-end">
                    <asp:Button ID="buttonGuardar" runat="server" Text="Guardar"
                        CssClass="btn btn-primary"
                        OnClick="buttonGuardar_Click" />

                    <asp:Button ID="buttonCancelar" runat="server" Text="Cancelar"
                        CssClass="btn btn-warning ms-3"
                        OnClick="buttonCancelar_Click"
                        CausesValidation="false" />
                </div>
            </div>

            <div id="painelDisciplinas" runat="server" visible="false" class="mt-4">
                <h3>Disciplinas que vão ficar na turma</h3>

                <p class="text-muted">
                    A grelha é atualizada automaticamente quando mudares o curso, o ano ou qualquer opção.
                </p>

                <asp:GridView ID="gridDisciplinas" runat="server"
                    CssClass="table table-striped table-bordered"
                    AutoGenerateColumns="False"
                    EmptyDataText="Seleciona o curso e o ano para veres as disciplinas.">
                    <Columns>
                        <asp:BoundField DataField="GrupoDisciplinar" HeaderText="Grupo disciplinar" />
                        <asp:BoundField DataField="Disciplina" HeaderText="Disciplina" />
                        <asp:BoundField DataField="Natureza" HeaderText="Natureza" />
                        <asp:BoundField DataField="Origem" HeaderText="Origem" />
                    </Columns>
                </asp:GridView>
            </div>

        </div>

    </div>

</asp:Content>