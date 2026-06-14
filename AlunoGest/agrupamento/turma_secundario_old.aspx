<%@ Page Title="" Language="C#" MasterPageFile="~/agrupamento/modeloAgrupamento.Master"
    AutoEventWireup="true" CodeBehind="turma_secundario_old.aspx.cs"
    Inherits="AlunoGest.agrupamento.turma_secundario_old"
    MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">

    <style type="text/css">
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

        .texto-ajuda-opcoes {
            display: block;
            margin-bottom: 8px;
        }
    </style>

    <script type="text/javascript">

        // ---------------------------------------------------------
        // Conta quantas checkboxes estão selecionadas dentro de um bloco
        // ---------------------------------------------------------
        function contarSelecionadas(idControlo) {
            var controlo = document.getElementById(idControlo);

            if (!controlo)
                return 0;

            var checks = controlo.querySelectorAll('input[type="checkbox"]');
            var total = 0;

            for (var i = 0; i < checks.length; i++) {
                if (checks[i].checked)
                    total++;
            }

            return total;
        }

        // ---------------------------------------------------------
        // BLOCO B
        // Máximo 2 selecionadas
        // ---------------------------------------------------------
        function atualizarBlocoB() {
            var cbl = document.getElementById('<%= cblB.ClientID %>');

            if (!cbl)
                return;

            var checks = cbl.querySelectorAll('input[type="checkbox"]');
            var total = 0;

            for (var i = 0; i < checks.length; i++) {
                if (checks[i].checked)
                    total++;
            }

            for (var j = 0; j < checks.length; j++) {
                if (checks[j].checked) {
                    checks[j].disabled = false;
                }
                else {
                    if (total >= 2)
                        checks[j].disabled = true;
                    else
                        checks[j].disabled = false;
                }
            }
        }

        // ---------------------------------------------------------
        // BLOCOS C E D
        // No total só podem ficar 2 selecionadas
        // ---------------------------------------------------------
        function atualizarBlocosCD() {
            var cblC = document.getElementById('<%= cblC.ClientID %>');
            var cblD = document.getElementById('<%= cblD.ClientID %>');

            var totalC = contarSelecionadas('<%= cblC.ClientID %>');
            var totalD = contarSelecionadas('<%= cblD.ClientID %>');
            var total = totalC + totalD;

            if (cblC) {
                var checksC = cblC.querySelectorAll('input[type="checkbox"]');

                for (var i = 0; i < checksC.length; i++) {
                    if (checksC[i].checked) {
                        checksC[i].disabled = false;
                    }
                    else {
                        // Mantém as não selecionadas sem visto e bloqueia quando já existem 2
                        checksC[i].checked = false;
                        checksC[i].disabled = (total >= 2);
                    }
                }
            }

            if (cblD) {
                var checksD = cblD.querySelectorAll('input[type="checkbox"]');

                for (var j = 0; j < checksD.length; j++) {
                    if (checksD[j].checked) {
                        checksD[j].disabled = false;
                    }
                    else {
                        // Mantém as não selecionadas sem visto e bloqueia quando já existem 2
                        checksD[j].checked = false;
                        checksD[j].disabled = (total >= 2);
                    }
                }
            }
        }

        // ---------------------------------------------------------
        // Associa eventos ao bloco B
        // ---------------------------------------------------------
        function associarEventosBlocoB() {
            var cbl = document.getElementById('<%= cblB.ClientID %>');

            if (!cbl)
                return;

            var checks = cbl.querySelectorAll('input[type="checkbox"]');

            for (var i = 0; i < checks.length; i++) {
                checks[i].onclick = function () {
                    atualizarBlocoB();
                };
            }

            atualizarBlocoB();
        }

        // ---------------------------------------------------------
        // Associa eventos aos blocos C e D
        // ---------------------------------------------------------
        function associarEventosBlocosCD() {
            var cblC = document.getElementById('<%= cblC.ClientID %>');
            var cblD = document.getElementById('<%= cblD.ClientID %>');

            if (cblC) {
                var checksC = cblC.querySelectorAll('input[type="checkbox"]');

                for (var i = 0; i < checksC.length; i++) {
                    checksC[i].onclick = function () {
                        atualizarBlocosCD();
                    };
                }
            }

            if (cblD) {
                var checksD = cblD.querySelectorAll('input[type="checkbox"]');

                for (var j = 0; j < checksD.length; j++) {
                    checksD[j].onclick = function () {
                        atualizarBlocosCD();
                    };
                }
            }

            atualizarBlocosCD();
        }

        // ---------------------------------------------------------
        // Prepara os controlos quando a página carrega
        // ---------------------------------------------------------
        function prepararCheckBoxes() {
            associarEventosBlocoB();
            associarEventosBlocosCD();
        }

        document.addEventListener("DOMContentLoaded", function () {
            prepararCheckBoxes();
        });

        function pageLoad() {
            prepararCheckBoxes();
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
                <div class="col-sm-4">
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

            <div class="row mb-2">
                <label for="txtCodigoTurma" class="col-sm-2 col-form-label text-end">Código da turma</label>
                <div class="col-sm-3">
                    <asp:TextBox ID="txtCodigoTurma" runat="server" CssClass="form-control border-secondary"></asp:TextBox>
                </div>
            </div>

            <div id="painelLE" runat="server" visible="false">
                <div class="row mb-2">
                    <label for="ddlLE" class="col-sm-2 col-form-label text-end">Língua estrangeira</label>
                    <div class="col-sm-4">
                        <asp:DropDownList ID="ddlLE" runat="server"
                            CssClass="form-select border-secondary"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="Opcoes_Changed">
                        </asp:DropDownList>
                        <small class="text-muted">Seleciona 1 disciplina.</small>
                    </div>
                </div>
            </div>

            <div id="painelB" runat="server" visible="false">
                <div class="row mb-2">
                    <label class="col-sm-2 col-form-label text-end">Opções B</label>
                    <div class="col-sm-6">
                        <asp:CheckBoxList ID="cblB" runat="server"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="Opcoes_Changed"
                            RepeatDirection="Vertical"
                            CssClass="checklist-esquerda">
                        </asp:CheckBoxList>
                        <small class="text-muted">Seleciona 2 disciplinas.</small>
                    </div>
                </div>
            </div>

            <div id="painelC" runat="server" visible="false">
                <div class="row mb-2">
                    <label class="col-sm-2 col-form-label text-end">Opções C</label>
                    <div class="col-sm-6">
                        <small class="text-muted texto-ajuda-opcoes">
                            No total dos blocos C e D, seleciona 2 disciplinas. Pelo menos uma tem de ser do bloco C.
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

            <div id="painelD" runat="server" visible="false">
                <div class="row mb-2">
                    <label class="col-sm-2 col-form-label text-end">Opções D</label>
                    <div class="col-sm-6">
                        <asp:CheckBoxList ID="cblD" runat="server"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="Opcoes_Changed"
                            RepeatDirection="Vertical"
                            CssClass="checklist-esquerda">
                        </asp:CheckBoxList>
                    </div>
                </div>
            </div>

            <div class="row mb-2">
                <label for="chkEMR" class="col-sm-2 col-form-label text-end">EMR</label>
                <div class="col-sm-5 pt-2">
                    <asp:CheckBox ID="chkEMR" runat="server"
                        Text="Associar Educação Moral e Religiosa"
                        AutoPostBack="true"
                        OnCheckedChanged="Opcoes_Changed" />
                </div>
            </div>

            <div class="row mb-2">
                <label for="chkPLNM" class="col-sm-2 col-form-label text-end">PLNM</label>
                <div class="col-sm-5 pt-2">
                    <asp:CheckBox ID="chkPLNM" runat="server"
                        Text="Associar Português Língua Não Materna"
                        AutoPostBack="true"
                        OnCheckedChanged="Opcoes_Changed" />
                </div>
            </div>

            <div class="row mb-2">
                <label for="chkAtiva" class="col-sm-2 col-form-label text-end">Ativa</label>
                <div class="col-sm-2 pt-2">
                    <asp:CheckBox ID="chkAtiva" runat="server" Checked="true" />
                </div>
            </div>

            <div class="row mb-3">
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