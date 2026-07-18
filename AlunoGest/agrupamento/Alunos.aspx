<%@ Page Title="" Language="C#"
    MasterPageFile="~/agrupamento/modeloAgrupamento.Master"
    AutoEventWireup="true"
    CodeBehind="alunos.aspx.cs"
    Inherits="AlunoGest.agrupamento.Alunos" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="headContent"
    runat="server">
</asp:Content>

<asp:Content
    ID="Content2"
    ContentPlaceHolderID="mainContent"
    runat="server">

    <div class="container">

        <h1>Alunos</h1>

        <asp:Label
            ID="LblMensagem"
            runat="server"
            Visible="false">
        </asp:Label>

        <!-- Lista de alunos -->
        <asp:GridView
            ID="GridAlunos"
            runat="server"
            CssClass="table table-striped table-bordered"
            AutoGenerateColumns="False"
            DataKeyNames="Id"
            EmptyDataText="Não existem alunos registados."
            SelectedRowCssClass="linha-selecionada">

            <Columns>

                <asp:CommandField
                    ShowSelectButton="True"
                    SelectText="Selecionar"
                    CausesValidation="false" />

                <asp:BoundField
                    DataField="NomeCompleto"
                    HeaderText="Nome" />

                <asp:BoundField
                    DataField="NumeroProcesso"
                    HeaderText="N.º Processo"
                    NullDisplayText="—" />

                <asp:BoundField
                    DataField="Telefone"
                    HeaderText="Telefone"
                    NullDisplayText="—" />

                <asp:BoundField
                    DataField="Email"
                    HeaderText="Email"
                    NullDisplayText="—" />

                <asp:CheckBoxField
                    DataField="Ativo"
                    HeaderText="Ativo"
                    ReadOnly="true" />

            </Columns>

        </asp:GridView>

        <!-- Botões da listagem -->
        <div class="mb-4">

            <asp:Button
                ID="ButtonCriar"
                runat="server"
                Text="Novo aluno"
                CssClass="btn btn-primary"
                OnClick="ButtonCriar_Click"
                CausesValidation="false" />

            <asp:Button
                ID="ButtonEditar"
                runat="server"
                Text="Editar"
                CssClass="btn btn-secondary ms-2"
                OnClick="ButtonEditar_Click"
                CausesValidation="false" />

        </div>

        <!-- Formulário de criação/edição -->
        <div
            ID="Controlos"
            runat="server"
            Visible="false"
            class="card border-secondary mb-4">

            <div class="card-body">

                <!-- Nome completo -->
                <div class="row mb-3">

                    <label
                        for="TxtNomeCompleto"
                        class="col-sm-2 col-form-label text-end">
                        Nome completo
                    </label>

                    <div class="col-sm-6">

                        <asp:TextBox
                            ID="TxtNomeCompleto"
                            runat="server"
                            CssClass="form-control border-secondary"
                            MaxLength="200">
                        </asp:TextBox>

                        <asp:RequiredFieldValidator
                            ID="RfvNomeCompleto"
                            runat="server"
                            ControlToValidate="TxtNomeCompleto"
                            ErrorMessage="O nome é obrigatório."
                            CssClass="text-danger"
                            Display="Dynamic" />

                    </div>

                </div>

                <!-- Número de processo -->
                <div class="row mb-3">

                    <label
                        for="TxtNumeroProcesso"
                        class="col-sm-2 col-form-label text-end">
                        N.º processo
                    </label>

                    <div class="col-sm-4">

                        <asp:TextBox
                            ID="TxtNumeroProcesso"
                            runat="server"
                            CssClass="form-control border-secondary"
                            MaxLength="50">
                        </asp:TextBox>

                        <asp:RequiredFieldValidator
                            ID="RfvNumeroProcesso"
                            runat="server"
                            ControlToValidate="TxtNumeroProcesso"
                            ErrorMessage="O número de processo é obrigatório."
                            CssClass="text-danger"
                            Display="Dynamic" />

                    </div>

                </div>

                <!-- Email -->
                <div class="row mb-3">

                    <label
                        for="TxtEmail"
                        class="col-sm-2 col-form-label text-end">
                        Email
                    </label>

                    <div class="col-sm-6">

                        <asp:TextBox
                            ID="TxtEmail"
                            runat="server"
                            CssClass="form-control border-secondary"
                            TextMode="Email"
                            MaxLength="150">
                        </asp:TextBox>

                        <asp:RequiredFieldValidator
                            ID="RfvEmail"
                            runat="server"
                            ControlToValidate="TxtEmail"
                            ErrorMessage="O email é obrigatório."
                            CssClass="text-danger"
                            Display="Dynamic" />

                        <asp:RegularExpressionValidator
                            ID="RevEmail"
                            runat="server"
                            ControlToValidate="TxtEmail"
                            ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                            ErrorMessage="Introduza um email válido."
                            CssClass="text-danger"
                            Display="Dynamic" />

                    </div>

                </div>

                <!-- Telefone -->
                <div class="row mb-3">

                    <label
                        for="TxtTelefone"
                        class="col-sm-2 col-form-label text-end">
                        Telefone
                    </label>

                    <div class="col-sm-4">

                        <asp:TextBox
                            ID="TxtTelefone"
                            runat="server"
                            CssClass="form-control border-secondary"
                            TextMode="Phone"
                            MaxLength="20"
                            placeholder="+351 912 345 678">
                        </asp:TextBox>

                        <asp:RequiredFieldValidator
                            ID="RfvTelefone"
                            runat="server"
                            ControlToValidate="TxtTelefone"
                            ErrorMessage="O telefone é obrigatório."
                            CssClass="text-danger"
                            Display="Dynamic" />

                        <asp:RegularExpressionValidator
                            ID="RevTelefone"
                            runat="server"
                            ControlToValidate="TxtTelefone"
                            ValidationExpression="^[0-9+\s()\-]{7,20}$"
                            ErrorMessage="Introduza um número de telefone válido."
                            CssClass="text-danger"
                            Display="Dynamic" />

                    </div>

                </div>

                <!-- Estado -->
                <div class="row mb-3">

                    <div class="col-sm-2"></div>

                    <div class="col-sm-6">

                        <div class="form-check">

                            <asp:CheckBox
                                ID="ChkAtivo"
                                runat="server"
                                CssClass="form-check-input"
                                ClientIDMode="Static" />

                            <label
                                class="form-check-label"
                                for="ChkAtivo">
                                Ativo
                            </label>

                        </div>

                    </div>

                </div>

                <!-- Botões -->
                <div class="row mb-2">

                    <div class="col-sm-2"></div>

                    <div class="col-sm-6">

                        <asp:Button
                            ID="ButtonGuardar"
                            runat="server"
                            Text="Guardar"
                            CssClass="btn btn-primary"
                            OnClick="ButtonGuardar_Click" />

                        <asp:Button
                            ID="ButtonCancelar"
                            runat="server"
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