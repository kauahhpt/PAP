<%@ Page Title="" Language="C#" MasterPageFile="~/agrupamento/modeloAgrupamento.Master"
    AutoEventWireup="true" CodeBehind="alunos.aspx.cs"
    Inherits="AlunoGest.agrupamento.Alunos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">

    <div class="container">

        <h1>Alunos</h1>

        <asp:Label ID="LblMensagem" runat="server" Visible="false"></asp:Label>

        <!-- Lista de alunos -->
        <asp:GridView ID="GridAlunos" runat="server"
            CssClass="table table-striped table-bordered"
            AutoGenerateColumns="False"
            DataKeyNames="Id"
            EmptyDataText="Não existem alunos registados."
            SelectedRowCssClass="linha-selecionada">
            <Columns>
                <asp:CommandField ShowSelectButton="True" SelectText="Selecionar" />
                <asp:BoundField DataField="NomeCompleto"   HeaderText="Nome" />
                <asp:BoundField DataField="NumeroProcesso" HeaderText="N.º Processo" />
                <asp:BoundField DataField="Email"          HeaderText="Email" />
                <asp:CheckBoxField DataField="Ativo"       HeaderText="Ativo" ReadOnly="true" />
            </Columns>
        </asp:GridView>

        <!-- Botões da listagem -->
        <div class="mb-4">
            <asp:Button ID="ButtonCriar" runat="server"
                Text="Novo Aluno"
                CssClass="btn btn-primary"
                OnClick="ButtonCriar_Click"
                CausesValidation="false" />

            <asp:Button ID="ButtonEditar" runat="server"
                Text="Editar"
                CssClass="btn btn-secondary ms-2"
                OnClick="ButtonEditar_Click"
                CausesValidation="false" />
        </div>

        <!-- Formulário de criação/edição -->
        <div id="Controlos" runat="server" visible="false" class="card border-secondary mb-4">
            <div class="card-body">

                <div class="row mb-3">
                    <label for="TxtNomeCompleto" class="col-sm-2 col-form-label text-end">Nome completo</label>
                    <div class="col-sm-6">
                        <asp:TextBox ID="TxtNomeCompleto" runat="server" CssClass="form-control border-secondary"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RfvNomeCompleto" runat="server"
                            ControlToValidate="TxtNomeCompleto"
                            ErrorMessage="O nome é obrigatório."
                            CssClass="text-danger"
                            Display="Dynamic" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label for="TxtNumeroProcesso" class="col-sm-2 col-form-label text-end">N.º processo</label>
                    <div class="col-sm-4">
                        <asp:TextBox ID="TxtNumeroProcesso" runat="server" CssClass="form-control border-secondary"></asp:TextBox>
                    </div>
                </div>

                <div class="row mb-3">
                    <label for="TxtEmail" class="col-sm-2 col-form-label text-end">Email</label>
                    <div class="col-sm-6">
                        <asp:TextBox ID="TxtEmail" runat="server" CssClass="form-control border-secondary"></asp:TextBox>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-sm-2"></div>
                    <div class="col-sm-6">
                        <div class="form-check">
                            <asp:CheckBox ID="ChkAtivo" runat="server"
                                CssClass="form-check-input" ClientIDMode="Static" />
                            <label class="form-check-label" for="ChkAtivo">Ativo</label>
                        </div>
                    </div>
                </div>

                <div class="row mb-2">
                    <div class="col-sm-2"></div>
                    <div class="col-sm-6">
                        <asp:Button ID="ButtonGuardar" runat="server"
                            Text="Guardar"
                            CssClass="btn btn-primary"
                            OnClick="ButtonGuardar_Click" />

                        <asp:Button ID="ButtonCancelar" runat="server"
                            Text="Cancelar"
                            CssClass="btn btn-warning ms-3"
                            CausesValidation="false"
                            OnClick="ButtonCancelar_Click" />
                    </div>
                </div>

            </div>
        </div>

    </div>

</asp:Content>