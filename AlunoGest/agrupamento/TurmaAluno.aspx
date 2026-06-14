<%@ Page Title="" Language="C#" MasterPageFile="~/agrupamento/modeloAgrupamento.Master"
    AutoEventWireup="true" CodeBehind="TurmaAluno.aspx.cs"
    Inherits="AlunoGest.agrupamento.TurmaAlunos"
    MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">

    <div class="container">

        <h1>Alunos da Turma</h1>

        <div class="mb-3">
            <asp:Label ID="LblTurma" runat="server" CssClass="fw-semibold"></asp:Label>
        </div>

        <asp:Label ID="LblMensagem" runat="server" Visible="false"></asp:Label>

        <div class="card mb-4">
            <div class="card-header">
                <h5 class="mb-0">Adicionar Aluno à Turma</h5>
            </div>
            <div class="card-body">
                <div class="row g-3 align-items-end">

                    <div class="col-md-5">
                        <label class="form-label">Aluno</label>
                        <asp:DropDownList ID="DdlAlunos" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>

                    <div class="col-md-2">
                        <label class="form-label">Data de Entrada</label>
                        <asp:TextBox ID="TxtDesde" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="col-md-2 d-flex flex-column gap-1">
                        <div class="form-check">
                            <asp:CheckBox ID="ChkTemPortugues" runat="server" Checked="true"
                                CssClass="form-check-input" ClientIDMode="Static" />
                            <label class="form-check-label" for="ChkTemPortugues">Tem Português</label>
                        </div>
                        <div class="form-check">
                            <asp:CheckBox ID="ChkTemEMRC" runat="server" Checked="false"
                                CssClass="form-check-input" ClientIDMode="Static" />
                            <label class="form-check-label" for="ChkTemEMRC">Tem EMRC</label>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <asp:Button ID="ButtonAdicionarAluno" runat="server"
                            Text="Adicionar Aluno"
                            CssClass="btn btn-primary"
                            OnClick="ButtonAdicionarAluno_Click" />
                    </div>

                </div>
            </div>
        </div>

        <div class="mt-3">
            <h3>Alunos Actuais</h3>

            <asp:GridView ID="GridAlunos" runat="server"
                CssClass="table table-striped table-bordered"
                AutoGenerateColumns="False"
                DataKeyNames="Id"
                EmptyDataText="A turma ainda não tem alunos."
                OnRowCommand="GridAlunos_RowCommand">
                <Columns>
                    <asp:BoundField DataField="NomeCompleto" HeaderText="Nome" />
                    <asp:BoundField DataField="NumeroProcesso" HeaderText="N.º Processo" />
                    <asp:BoundField DataField="Desde" HeaderText="Desde"
                        DataFormatString="{0:dd/MM/yyyy}" />

                    <asp:TemplateField HeaderText="Português">
                        <ItemTemplate>
                            <span class='badge <%# Convert.ToBoolean(Eval("TemPortugues")) ? "bg-success" : "bg-secondary" %>'>
                                <%# Convert.ToBoolean(Eval("TemPortugues")) ? "Sim" : "Não" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="EMRC">
                        <ItemTemplate>
                            <span class='badge <%# Convert.ToBoolean(Eval("TemEMRC")) ? "bg-success" : "bg-secondary" %>'>
                                <%# Convert.ToBoolean(Eval("TemEMRC")) ? "Sim" : "Não" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Ação">
                        <ItemTemplate>
                            <asp:Button ID="ButtonRemover" runat="server"
                                Text="Remover"
                                CommandName="RemoverAluno"
                                CommandArgument='<%# Eval("Id") %>'
                                CssClass="btn btn-outline-danger btn-sm" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

        <div class="mt-4">
            <h3>Histórico de Alunos</h3>

            <asp:GridView ID="GridHistorico" runat="server"
                CssClass="table table-striped table-bordered"
                AutoGenerateColumns="False"
                EmptyDataText="Sem histórico para esta turma.">
                <Columns>
                    <asp:BoundField DataField="NomeCompleto" HeaderText="Nome" />
                    <asp:BoundField DataField="NumeroProcesso" HeaderText="N.º Processo" />
                    <asp:BoundField DataField="Desde" HeaderText="Desde"
                        DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="Ate" HeaderText="Até"
                        DataFormatString="{0:dd/MM/yyyy}" />

                    <asp:TemplateField HeaderText="Português">
                        <ItemTemplate>
                            <span class='badge <%# Convert.ToBoolean(Eval("TemPortugues")) ? "bg-success" : "bg-secondary" %>'>
                                <%# Convert.ToBoolean(Eval("TemPortugues")) ? "Sim" : "Não" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="EMRC">
                        <ItemTemplate>
                            <span class='badge <%# Convert.ToBoolean(Eval("TemEMRC")) ? "bg-success" : "bg-secondary" %>'>
                                <%# Convert.ToBoolean(Eval("TemEMRC")) ? "Sim" : "Não" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

        <div class="mt-4">
            <asp:Button ID="ButtonVoltar" runat="server"
                Text="Voltar às Turmas"
                CssClass="btn btn-outline-secondary"
                OnClick="ButtonVoltar_Click"
                CausesValidation="false" />
        </div>

    </div>

</asp:Content>
