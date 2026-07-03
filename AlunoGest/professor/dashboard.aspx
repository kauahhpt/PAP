<%@ Page Title="As minhas turmas" Language="C#" MasterPageFile="~/professor/ModeloProfessor.Master"
    AutoEventWireup="true" CodeBehind="dashboard.aspx.cs" Inherits="AlunoGest.professor.dashboard" %>
<asp:Content ID="Title" ContentPlaceHolderID="titleContent" runat="server">As minhas turmas</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="mainContent" runat="server">
    <div class="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-4">
        <div><h1 class="h3 mb-1">As minhas turmas</h1><p class="text-muted mb-0">Turmas e disciplinas que lhe estão atribuídas.</p></div>
        <asp:TextBox ID="TxtPesquisa" runat="server" CssClass="form-control" style="max-width:280px"
            placeholder="Pesquisar turma ou disciplina" AutoPostBack="true" OnTextChanged="TxtPesquisa_TextChanged" />
    </div>
    <asp:Label ID="LblMensagem" runat="server" Visible="false" CssClass="alert alert-warning d-block" />
    <div class="row g-3 mb-4">
        <div class="col-md-4"><div class="card stat-card"><div class="card-body"><div class="text-muted">Turmas atribuídas</div><asp:Label ID="LblTotalTurmas" runat="server" CssClass="fs-2 fw-bold" /></div></div></div>
        <div class="col-md-4"><div class="card stat-card"><div class="card-body"><div class="text-muted">Alunos abrangidos</div><asp:Label ID="LblTotalAlunos" runat="server" CssClass="fs-2 fw-bold" /></div></div></div>
        <div class="col-md-4"><div class="card stat-card"><div class="card-body"><div class="text-muted">Atividades futuras</div><asp:Label ID="LblTotalAtividades" runat="server" CssClass="fs-2 fw-bold" /></div></div></div>
    </div>
    <div class="card page-card"><div class="card-body p-0">
        <asp:GridView ID="GridTurmas" runat="server" AutoGenerateColumns="false" CssClass="table table-hover align-middle mb-0"
            GridLines="None" EmptyDataText="Não existem turmas atribuídas." DataKeyNames="TurmaId">
            <Columns>
                <asp:BoundField DataField="Turma" HeaderText="Turma" />
                <asp:BoundField DataField="Escola" HeaderText="Escola" />
                <asp:BoundField DataField="AnoLetivo" HeaderText="Ano letivo" />
                <asp:BoundField DataField="Disciplinas" HeaderText="Disciplinas" />
                <asp:BoundField DataField="TotalAlunos" HeaderText="Alunos" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <a class="btn btn-primary btn-sm me-1" href='turma.aspx?id=<%# Eval("TurmaId") %>'>Gerir turma</a>
                        <a class="btn btn-outline-primary btn-sm" href='calendario.aspx?turma=<%# Eval("TurmaId") %>'>Calendário</a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div></div>
</asp:Content>
