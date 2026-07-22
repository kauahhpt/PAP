<%@ Page Title="Disciplinas do Professor" Language="C#" MasterPageFile="~/agrupamento/modeloAgrupamento.Master"
    AutoEventWireup="true" CodeBehind="professor_disciplinas.aspx.cs"
    Inherits="AlunoGest.agrupamento.professor_disciplinas"
    MaintainScrollPositionOnPostback="true"%>

<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">

    <div class="container">

        <h1>Disciplinas que o professor pode lecionar</h1>

        <div class="row mt-4 mb-4">
            <div class="col-md-6">
                <label for="ddlGrupoDisciplinar" class="form-label fw-semibold">Grupo disciplinar</label>
                <asp:DropDownList ID="ddlGrupoDisciplinar" runat="server"
                    CssClass="form-select"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="ddlGrupoDisciplinar_SelectedIndexChanged">
                </asp:DropDownList>
            </div>

            <div class="col-md-6 d-flex align-items-end">
                <asp:Label ID="lblProfessor" runat="server" CssClass="fw-semibold"></asp:Label>
            </div>
        </div>

        <div class="row">
            <!-- Disciplinas disponíveis -->
            <div class="col-md-6">
                <h3>Disciplinas disponíveis</h3>

                <asp:GridView ID="gridDisciplinasDisponiveis" runat="server"
                    CssClass="table table-striped table-bordered"
                    AutoGenerateColumns="False"
                    DataKeyNames="Id"
                    EmptyDataText="Selecione o grupo disciplinar."
                    OnRowCommand="gridDisciplinasDisponiveis_RowCommand">

                    <Columns>
                        <asp:BoundField DataField="Nome" HeaderText="Disciplina" />
                        <asp:ButtonField ButtonType="Button"
                            CommandName="Associar"
                            Text="Associar"
                            ControlStyle-CssClass="btn btn-sm btn-primary" />
                    </Columns>
                </asp:GridView>
            </div>

            <!-- Disciplinas do professor -->
            <div class="col-md-6">
                <h3>Disciplinas associadas ao professor</h3>

                <asp:GridView ID="gridDisciplinasProfessor" runat="server"
                    CssClass="table table-striped table-bordered"
                    AutoGenerateColumns="False"
                    DataKeyNames="ProfessorDisciplinaId,DisciplinaId"
                    EmptyDataText="O professor ainda não tem disciplinas associadas."
                    OnRowCommand="gridDisciplinasProfessor_RowCommand">

                    <Columns>
                        <asp:BoundField DataField="Disciplina" HeaderText="Disciplina" />
                        <asp:BoundField DataField="GrupoDisciplinar" HeaderText="Grupo disciplinar" />
                        <asp:BoundField DataField="Desde" HeaderText="Desde" DataFormatString="{0:yyyy-MM-dd}" HtmlEncode="false" />
                        <asp:ButtonField ButtonType="Button"
                            CommandName="Dissociar"
                            Text="Dissociar"
                            ControlStyle-CssClass="btn btn-sm btn-warning" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
        <asp:Button
    ID="buttonVoltar"
    runat="server"
    Text="Voltar"
    CssClass="btn btn-outline-secondary d-inline-block ms-4"
    CausesValidation="false"
    PostBackUrl="~/agrupamento/professores.aspx" />
    </div>

</asp:Content>