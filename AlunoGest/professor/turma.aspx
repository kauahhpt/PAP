<%@ Page Title="Gerir turma" Language="C#" MasterPageFile="~/professor/ModeloProfessor.Master"
    AutoEventWireup="true" CodeBehind="turma.aspx.cs" Inherits="AlunoGest.professor.turma" %>
<asp:Content ID="Title" ContentPlaceHolderID="titleContent" runat="server">Gerir turma</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="mainContent" runat="server">
    <div class="mb-3"><a href="dashboard.aspx" class="text-decoration-none">&larr; Voltar às minhas turmas</a></div>
    <div class="d-flex flex-wrap justify-content-between align-items-end gap-3 mb-4">
        <div><h1 class="h3 mb-1"><asp:Label ID="LblTurma" runat="server" /></h1><asp:Label ID="LblDetalhes" runat="server" CssClass="text-muted" /></div>
        <asp:Label ID="LblDisciplinas" runat="server" CssClass="badge text-bg-primary fs-6" />
    </div>
    <asp:Label ID="LblMensagem" runat="server" Visible="false" />

    <div class="card page-card mb-4"><div class="card-body">
        <div class="d-flex flex-wrap justify-content-between gap-2 mb-3">
            <div><h2 class="h5 mb-1">Alunos da turma</h2><p class="text-muted small mb-0">Consulte os alunos atualmente inscritos nesta turma.</p></div>
            <asp:TextBox ID="TxtPesquisaAluno" runat="server" CssClass="form-control" style="max-width:270px" placeholder="Pesquisar aluno"
                AutoPostBack="true" OnTextChanged="TxtPesquisaAluno_TextChanged" />
        </div>
        <div class="row g-2 align-items-end mb-3">
            <div class="col-md-7"><label class="form-label">Adicionar aluno disponível</label>
                <asp:DropDownList ID="DdlAlunosDisponiveis" runat="server" CssClass="form-select" /></div>
            <div class="col-md-auto"><asp:Button ID="BtnAdicionarAluno" runat="server" Text="Adicionar à turma" CssClass="btn btn-success" OnClick="BtnAdicionarAluno_Click" CausesValidation="false" /></div>
        </div>
        <div class="table-responsive">
            <asp:GridView ID="GridAlunos" runat="server" AutoGenerateColumns="false" CssClass="table table-hover align-middle" GridLines="None" EmptyDataText="A turma ainda não tem alunos."
                DataKeyNames="AlunoTurmaId" OnRowCommand="GridAlunos_RowCommand">
                <Columns><asp:BoundField DataField="Numero" HeaderText="N.º" /><asp:BoundField DataField="NomeCompleto" HeaderText="Nome" />
                    <asp:BoundField DataField="NumeroProcesso" HeaderText="Processo" /><asp:BoundField DataField="Email" HeaderText="Email" />
                    <asp:BoundField DataField="Desde" HeaderText="Na turma desde" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:TemplateField><ItemTemplate><asp:LinkButton runat="server" CommandName="RemoverAluno" CommandArgument='<%# Container.DataItemIndex %>' Text="Remover" CssClass="btn btn-outline-danger btn-sm"
                        OnClientClick="return confirm('Remover este aluno da turma?');" CausesValidation="false" /></ItemTemplate></asp:TemplateField></Columns>
            </asp:GridView>
        </div>
    </div></div>

    <div class="card page-card mb-4"><div class="card-body">
        <div class="d-flex justify-content-between align-items-center mb-3"><div><h2 class="h5 mb-1">Testes e trabalhos</h2><p class="text-muted small mb-0">O que publicar aqui fica visível na agenda de todos os alunos da turma.</p></div>
            <asp:Button ID="BtnNova" runat="server" Text="Nova atividade" CssClass="btn btn-primary" OnClick="BtnNova_Click" CausesValidation="false" /></div>
        <asp:Panel ID="PainelFormulario" runat="server" Visible="false" CssClass="border rounded-3 bg-light p-3 mb-4">
            <asp:HiddenField ID="HdnEventoId" runat="server" />
            <div class="row g-3">
                <div class="col-md-3"><label class="form-label">Tipo</label><asp:DropDownList ID="DdlTipo" runat="server" CssClass="form-select"><asp:ListItem>Trabalho</asp:ListItem><asp:ListItem>Teste</asp:ListItem></asp:DropDownList></div>
                <div class="col-md-5"><label class="form-label">Título</label><asp:TextBox ID="TxtTitulo" runat="server" CssClass="form-control" MaxLength="200" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtTitulo" ErrorMessage="Indique um título." CssClass="text-danger small" ValidationGroup="atividade" /></div>
                <div class="col-md-4"><label class="form-label">Data e hora</label><asp:TextBox ID="TxtDataHora" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtDataHora" ErrorMessage="Indique a data e hora." CssClass="text-danger small" ValidationGroup="atividade" /></div>
                <div class="col-12"><label class="form-label">Anexo <span class="text-muted small">(opcional, máximo 10 MB)</span></label><asp:FileUpload ID="FileAnexo" runat="server" CssClass="form-control" /></div>
            </div>
            <asp:Repeater ID="RepeaterAnexos" runat="server" OnItemCommand="RepeaterAnexos_ItemCommand"><HeaderTemplate><div class="mt-3"><strong>Anexos atuais</strong></HeaderTemplate><ItemTemplate>
                <div class="d-flex gap-2 align-items-center mt-1"><a href='<%# Eval("CaminhoFicheiro") %>' target="_blank"><%# Eval("NomeFicheiro") %></a>
                <asp:LinkButton runat="server" CommandName="Remover" CommandArgument='<%# Eval("Id") %>' Text="Remover" CssClass="text-danger small" CausesValidation="false" /></div>
            </ItemTemplate><FooterTemplate></div></FooterTemplate></asp:Repeater>
            <div class="mt-3"><asp:Button ID="BtnGuardar" runat="server" Text="Guardar" CssClass="btn btn-success" OnClick="BtnGuardar_Click" ValidationGroup="atividade" />
                <asp:Button ID="BtnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary" OnClick="BtnCancelar_Click" CausesValidation="false" /></div>
        </asp:Panel>
        <div class="table-responsive"><asp:GridView ID="GridAtividades" runat="server" AutoGenerateColumns="false" CssClass="table table-hover align-middle" GridLines="None"
            DataKeyNames="Id" EmptyDataText="Ainda não existem atividades para esta turma." OnRowCommand="GridAtividades_RowCommand">
            <Columns><asp:BoundField DataField="Tipo" HeaderText="Tipo" /><asp:BoundField DataField="Titulo" HeaderText="Título" />
                <asp:BoundField DataField="DataHora" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy HH:mm}" /><asp:BoundField DataField="TotalAnexos" HeaderText="Anexos" />
                <asp:TemplateField><ItemTemplate><asp:LinkButton runat="server" CommandName="EditarAtividade" CommandArgument='<%# Container.DataItemIndex %>' Text="Editar" CssClass="btn btn-outline-primary btn-sm me-1" CausesValidation="false" />
                    <asp:LinkButton runat="server" CommandName="ApagarAtividade" CommandArgument='<%# Container.DataItemIndex %>' Text="Eliminar" CssClass="btn btn-outline-danger btn-sm"
                        OnClientClick="return confirm('Eliminar esta atividade e os respetivos anexos?');" CausesValidation="false" /></ItemTemplate></asp:TemplateField></Columns>
        </asp:GridView></div>
    </div></div>
</asp:Content>
