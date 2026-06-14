<%@ Page Title="" Language="C#" MasterPageFile="~/agrupamento/modeloAgrupamento.Master"
    AutoEventWireup="true" CodeBehind="turma_basico.aspx.cs"
    Inherits="AlunoGest.agrupamento.turma_basico"
    MaintainScrollPositionOnPostback="true"%>

<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">

    <style type="text/css">
        .zona-check-esquerda {
            text-align: left;
        }

        .zona-check-esquerda input[type="checkbox"] {
            margin-right: 6px;
        }

        .zona-check-esquerda label {
            display: inline-block;
            margin-bottom: 0;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">

    <div class="container">

        <h1>Turma do 3.º Ciclo</h1>

        <div class="mb-3">
            <asp:Label ID="lblEscola" runat="server" CssClass="fw-semibold"></asp:Label>
        </div>

        <asp:Label ID="lblMensagem" runat="server" Visible="false"></asp:Label>

        <div id="controlos" runat="server" class="mt-4">

            <div class="row mb-2">
                <label for="ddlAnoLetivo" class="col-sm-2 col-form-label text-end">Ano letivo</label>
                <div class="col-sm-3">
                    <asp:DropDownList ID="ddlAnoLetivo" runat="server" CssClass="form-select border-secondary"></asp:DropDownList>
                </div>
            </div>

            <div class="row mb-2">
                <label for="ddlAnoEscolaridade" class="col-sm-2 col-form-label text-end">Ano de escolaridade</label>
                <div class="col-sm-3">
                    <asp:DropDownList ID="ddlAnoEscolaridade" runat="server"
                        CssClass="form-select border-secondary"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="SelecaoCurricular_Changed">
                        <asp:ListItem Text="-- selecionar --" Value=""></asp:ListItem>
                        <asp:ListItem Text="7.º ano" Value="7"></asp:ListItem>
                        <asp:ListItem Text="8.º ano" Value="8"></asp:ListItem>
                        <asp:ListItem Text="9.º ano" Value="9"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div class="row mb-2">
                <label for="txtCodigoTurma" class="col-sm-2 col-form-label text-end">Código da turma</label>
                <div class="col-sm-3">
                    <asp:TextBox ID="txtCodigoTurma" runat="server" CssClass="form-control border-secondary" />
                </div>
            </div>

            <div class="row mb-2">
                <label for="ddlLE2" class="col-sm-2 col-form-label text-end">Língua Estrangeira II</label>
                <div class="col-sm-3">
                    <asp:DropDownList ID="ddlLE2" runat="server"
                        CssClass="form-select border-secondary"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="SelecaoCurricular_Changed">
                    </asp:DropDownList>
                </div>
            </div>

            <div class="row mb-2">
                <label for="chkEMR" class="col-sm-2 col-form-label text-end">Educação Moral e Religiosa</label>
                <div class="col-sm-6 pt-2 zona-check-esquerda">
                    <asp:CheckBox ID="chkEMR" runat="server"
                        AutoPostBack="true"
                        OnCheckedChanged="SelecaoCurricular_Changed" />
                </div>
            </div>

            <div class="row mb-2">
                <label for="chkPLNM" class="col-sm-2 col-form-label text-end">Português Língua Não Materna</label>
                <div class="col-sm-6 pt-2 zona-check-esquerda">
                    <asp:CheckBox ID="chkPLNM" runat="server"
                        AutoPostBack="true"
                        OnCheckedChanged="SelecaoCurricular_Changed" />
                </div>
            </div>

            <div class="row mb-2">
                <label for="chkAtiva" class="col-sm-2 col-form-label text-end">Ativa</label>
                <div class="col-sm-2 pt-2">
                    <asp:CheckBox ID="chkAtiva" runat="server" Checked="true" />
                </div>
            </div>

            <div class="row mb-2">
                <div class="col-sm-2"></div>
                <div class="col-sm-8 text-end">
                    <asp:Button Text="Guardar" runat="server" CssClass="btn btn-primary d-inline-block"
                        ID="buttonGuardar" OnClick="buttonGuardar_Click" />

                    <asp:Button Text="Cancelar" runat="server" CssClass="btn btn-warning d-inline-block ms-4"
                        ID="buttonCancelar" OnClick="buttonCancelar_Click" CausesValidation="false" />
                </div>
            </div>

            <div id="painelDisciplinas" runat="server" class="mt-5" visible="false">
                <h3>Disciplinas que vão ficar na turma</h3>

                <asp:GridView runat="server" ID="gridDisciplinas"
                    CssClass="table table-striped table-bordered"
                    AutoGenerateColumns="False"
                    EmptyDataText="Selecione o ano e a LE II para ver as disciplinas.">
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