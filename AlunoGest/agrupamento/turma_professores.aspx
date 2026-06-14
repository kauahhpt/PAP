<%@ Page Title="" Language="C#" MasterPageFile="~/agrupamento/modeloAgrupamento.Master"
    AutoEventWireup="true" CodeBehind="turma_professores.aspx.cs"
    Inherits="AlunoGest.agrupamento.turma_professores"
    MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">

    <div class="container">

        <h1>Professores da turma</h1>

        <div class="mb-3">
            <asp:Label ID="lblTurma" runat="server" CssClass="fw-semibold"></asp:Label>
        </div>

        <asp:Label ID="lblMensagem" runat="server" Visible="false"></asp:Label>

        <div class="mt-4">
            <h3>Disciplinas da turma</h3>

            <asp:GridView ID="GridTurmaDisciplinas" runat="server"
                CssClass="table table-striped linha-selecionada"
                AutoGenerateColumns="False"
                DataKeyNames="TurmaDisciplinaId,DisciplinaId,OfertaEscola"
                EmptyDataText="A turma não tem disciplinas."
                SelectedRowCssClass="linha-selecionada"
                OnSelectedIndexChanged="GridTurmaDisciplinas_SelectedIndexChanged">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" SelectText="Selecionar" />
                    <asp:BoundField DataField="GrupoDisciplinar" HeaderText="Grupo disciplinar" />
                    <asp:BoundField DataField="Disciplina" HeaderText="Disciplina" />
                    <asp:BoundField DataField="Natureza" HeaderText="Natureza" />
                    <asp:BoundField DataField="ProfessorAtual" HeaderText="Professor atual" />
                </Columns>
            </asp:GridView>
        </div>

        <div id="painelDisciplina" runat="server" visible="false" class="mt-4">

            <div class="d-flex justify-content-between align-items-center mb-3">
                <asp:Label ID="lblDisciplinaSelecionada" runat="server" CssClass="fw-semibold text-primary"></asp:Label>

                <asp:Button ID="buttonOcultarPainelDisciplina" runat="server"
                    Text="Ocultar"
                    CssClass="btn btn-outline-secondary"
                    CausesValidation="false"
                    OnClick="buttonOcultarPainelDisciplina_Click" />
            </div>

            <div class="row">

                <div class="col-md-6">
                    <h4>Professores disponíveis</h4>

                    <asp:GridView ID="GridProfessores" runat="server"
                        CssClass="table table-striped linha-selecionada"
                        AutoGenerateColumns="False"
                        DataKeyNames="ProfessorId"
                        EmptyDataText="Não há professores disponíveis para esta disciplina."
                        SelectedRowCssClass="linha-selecionada">
                        <Columns>
                            <asp:CommandField ShowSelectButton="True" SelectText="Selecionar" />
                            <asp:BoundField DataField="Nome" HeaderText="Professor" />
                            <asp:BoundField DataField="NumeroProcesso" HeaderText="N.º processo" />
                            <asp:BoundField DataField="Email" HeaderText="Email" />
                        </Columns>
                    </asp:GridView>

                    <div class="mt-3">
                        <asp:Button ID="buttonAssociarProfessor" runat="server"
                            Text="Associar professor à disciplina"
                            CssClass="btn btn-primary"
                            OnClick="buttonAssociarProfessor_Click" />
                    </div>
                </div>

                <div class="col-md-6">
                    <h4>Professor atual da disciplina</h4>

                    <asp:GridView ID="GridTurmaProfessores" runat="server"
                        CssClass="table table-striped table-bordered"
                        AutoGenerateColumns="False"
                        DataKeyNames="Id,ProfessorId"
                        EmptyDataText="Esta disciplina não tem professor atual."
                        OnRowCommand="GridTurmaProfessores_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="Nome" HeaderText="Professor" />
                            <asp:BoundField DataField="NumeroProcesso" HeaderText="N.º processo" />
                            <asp:BoundField DataField="Desde" HeaderText="Desde" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:ButtonField Text="Terminar"
                                CommandName="TerminarProfessor"
                                ButtonType="Button"
                                ControlStyle-CssClass="btn btn-outline-danger btn-sm" />
                        </Columns>
                    </asp:GridView>
                </div>

            </div>

            <div class="mt-4">
                <h4>Histórico de professores da disciplina</h4>

                <asp:GridView ID="GridHistoricoProfessores" runat="server"
                    CssClass="table table-striped table-bordered"
                    AutoGenerateColumns="False"
                    EmptyDataText="Sem histórico para esta disciplina.">
                    <Columns>
                        <asp:BoundField DataField="Nome" HeaderText="Professor" />
                        <asp:BoundField DataField="NumeroProcesso" HeaderText="N.º processo" />
                        <asp:BoundField DataField="Desde" HeaderText="Desde" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField DataField="Ate" HeaderText="Até" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField DataField="Estado" HeaderText="Estado" />
                    </Columns>
                </asp:GridView>
            </div>

        </div>

        <div class="mt-5">
            <h3>Diretor de turma</h3>

            <div class="row">
                <div class="col-md-6">

                    <div class="mb-2">
                        <label for="ddlDiretorTurma" class="form-label">Escolher diretor de turma</label>
                        <asp:DropDownList ID="ddlDiretorTurma" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>

                    <asp:Button ID="buttonDefinirDiretorTurma" runat="server"
                        Text="Definir diretor de turma"
                        CssClass="btn btn-primary"
                        OnClick="buttonDefinirDiretorTurma_Click" />
                </div>

                <div class="col-md-6">
                    <h4>Diretor de turma atual</h4>

                    <asp:GridView ID="GridDiretorAtual" runat="server"
                        CssClass="table table-striped table-bordered"
                        AutoGenerateColumns="False"
                        DataKeyNames="Id,ProfessorId"
                        EmptyDataText="A turma não tem diretor de turma atual."
                        OnRowCommand="GridDiretorAtual_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="Nome" HeaderText="Professor" />
                            <asp:BoundField DataField="NumeroProcesso" HeaderText="N.º processo" />
                            <asp:BoundField DataField="Desde" HeaderText="Desde" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:ButtonField Text="Terminar"
                                CommandName="TerminarDiretor"
                                ButtonType="Button"
                                ControlStyle-CssClass="btn btn-outline-danger btn-sm" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

            <div class="mt-4">
                <h4>Histórico de diretores de turma</h4>

                <asp:GridView ID="GridHistoricoDiretores" runat="server"
                    CssClass="table table-striped table-bordered"
                    AutoGenerateColumns="False"
                    EmptyDataText="Sem histórico de diretores de turma.">
                    <Columns>
                        <asp:BoundField DataField="Nome" HeaderText="Professor" />
                        <asp:BoundField DataField="NumeroProcesso" HeaderText="N.º processo" />
                        <asp:BoundField DataField="Desde" HeaderText="Desde" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField DataField="Ate" HeaderText="Até" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField DataField="Estado" HeaderText="Estado" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <div class="mt-4">
            <asp:Button ID="buttonVoltar" runat="server"
                Text="Voltar às turmas"
                CssClass="btn btn-outline-secondary"
                OnClick="buttonVoltar_Click"
                CausesValidation="false" />
        </div>

    </div>

</asp:Content>