<%@ Page Title="Alunos"
    Language="C#"
    MasterPageFile="~/agrupamento/modeloAgrupamento.Master"
    AutoEventWireup="true"
    CodeBehind="alunos.aspx.cs"
    Inherits="AlunoGest.agrupamento.Alunos"
    MaintainScrollPositionOnPostback="true" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="headContent"
    runat="server">

    <style>
        .validation-message {
            margin-top: 5px;
            color: #dc3545;
            font-size: 12px;
            font-weight: 600;
        }

        .pagina-alunos {
            padding-bottom: 40px;
        }

        .cabecalho-pagina {
            margin-bottom: 25px;
        }

            .cabecalho-pagina h1 {
                margin-bottom: 5px;
                color: #1f2937;
                font-size: 30px;
                font-weight: 800;
            }

            .cabecalho-pagina p {
                margin: 0;
                color: #64748b;
            }

        .formulario-card {
            border: 1px solid #dbe3ed;
            border-radius: 14px;
            background: #ffffff;
            box-shadow: 0 4px 14px rgba(15, 23, 42, 0.06);
        }

        .formulario-titulo {
            margin-bottom: 22px;
            color: #1f2937;
            font-size: 21px;
            font-weight: 800;
        }

        .linha-selecionada {
            background-color: #dbeafe !important;
        }

        .nif-ajuda {
            display: block;
            margin-top: 5px;
            color: #64748b;
            font-size: 12px;
        }

        @media (max-width: 768px) {
            .campo-label {
                text-align: left !important;
            }
        }
    </style>

</asp:Content>

<asp:Content
    ID="Content2"
    ContentPlaceHolderID="mainContent"
    runat="server">

    <div class="container pagina-alunos">

        <div class="cabecalho-pagina">

            <h1>Alunos</h1>

            <p>
                Crie e atualize os alunos associados ao agrupamento.
            </p>

        </div>

        <asp:Label
            ID="LblMensagem"
            runat="server"
            Visible="false" />

        <!-- LISTA DE ALUNOS -->

        <div class="table-responsive mb-3">

            <asp:GridView
                ID="GridAlunos"
                runat="server"
                CssClass="table table-striped table-bordered align-middle"
                AutoGenerateColumns="false"
                DataKeyNames="Id"
                EmptyDataText="Não existem alunos registados."
                SelectedRowCssClass="linha-selecionada">

                <Columns>

                    <asp:CommandField
                        ShowSelectButton="true"
                        SelectText="Selecionar"
                        CausesValidation="false" />

                    <asp:BoundField
                        DataField="NomeCompleto"
                        HeaderText="Nome" />

                    <asp:BoundField
                        DataField="NumeroProcesso"
                        HeaderText="N.º processo"
                        NullDisplayText="—" />

                    <asp:BoundField
                        DataField="NIF"
                        HeaderText="NIF"
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

        </div>

        <!-- BOTÕES DA LISTAGEM -->

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
                Text="Editar aluno"
                CssClass="btn btn-secondary ms-2"
                OnClick="ButtonEditar_Click"
                CausesValidation="false" />

            <asp:Button
                ID="buttonVoltar"
                runat="server"
                Text="Voltar"
                CssClass="btn btn-outline-secondary d-inline-block ms-4"
                CausesValidation="false"
                PostBackUrl="~/agrupamento/dashboard.aspx" />

        </div>

        <!-- FORMULÁRIO -->

        <div
            id="Controlos"
            runat="server"
            visible="false"
            class="formulario-card mb-4">

            <div class="card-body p-4">

                <h2 class="formulario-titulo">Dados do aluno
                </h2>

                <!-- NOME -->

                <div class="row mb-3">

                    <label
                        for="<%= TxtNomeCompleto.ClientID %>"
                        class="col-sm-3 col-form-label text-end campo-label">
                        Nome completo

                    </label>

                    <div class="col-sm-7">

                        <asp:TextBox
                            ID="TxtNomeCompleto"
                            runat="server"
                            CssClass="form-control border-secondary"
                            MaxLength="200" />

                        <asp:RequiredFieldValidator
                            ID="RfvNomeCompleto"
                            runat="server"
                            ControlToValidate="TxtNomeCompleto"
                            ErrorMessage="O nome é obrigatório."
                            CssClass="text-danger small validation-message"
                            Display="Dynamic"
                            ValidationGroup="aluno" />

                    </div>

                </div>

                <!-- NÚMERO DE PROCESSO -->

                <div class="row mb-3">

                    <label
                        for="<%= TxtNumeroProcesso.ClientID %>"
                        class="col-sm-3 col-form-label text-end campo-label">
                        N.º processo

                    </label>

                    <div class="col-sm-5">

                        <asp:TextBox
                            ID="TxtNumeroProcesso"
                            runat="server"
                            CssClass="form-control border-secondary"
                            MaxLength="50" />

                        <asp:RequiredFieldValidator
                            ID="RfvNumeroProcesso"
                            runat="server"
                            ControlToValidate="TxtNumeroProcesso"
                            ErrorMessage="O número de processo é obrigatório."
                            CssClass="text-danger small validation-message"
                            Display="Dynamic"
                            ValidationGroup="aluno" />

                    </div>

                </div>

                <!-- NIF -->

                <div class="row mb-3">

                    <label
                        for="<%= TxtNIF.ClientID %>"
                        class="col-sm-3 col-form-label text-end campo-label">
                        NIF

                    </label>

                    <div class="col-sm-5">

                        <asp:TextBox
                            ID="TxtNIF"
                            runat="server"
                            CssClass="form-control border-secondary"
                            MaxLength="9"
                            inputmode="numeric"
                            placeholder="123456789" />

                        <asp:RequiredFieldValidator
                            ID="RfvNIF"
                            runat="server"
                            ControlToValidate="TxtNIF"
                            ErrorMessage="O NIF é obrigatório."
                            CssClass="text-danger small validation-message"
                            Display="Dynamic"
                            ValidationGroup="aluno" />

                        <asp:RegularExpressionValidator
                            ID="RevNIF"
                            runat="server"
                            ControlToValidate="TxtNIF"
                            ValidationExpression="^\d{9}$"
                            ErrorMessage="O NIF deve conter exatamente 9 algarismos."
                            CssClass="text-danger small validation-message"
                            Display="Dynamic"
                            ValidationGroup="aluno" />



                    </div>

                </div>

                <!-- EMAIL -->

                <div class="row mb-3">

                    <label
                        for="<%= TxtEmail.ClientID %>"
                        class="col-sm-3 col-form-label text-end campo-label">
                        Email

                    </label>

                    <div class="col-sm-7">

                        <asp:TextBox
                            ID="TxtEmail"
                            runat="server"
                            CssClass="form-control border-secondary"
                            TextMode="Email"
                            MaxLength="150" />

                        <asp:RequiredFieldValidator
                            ID="RfvEmail"
                            runat="server"
                            ControlToValidate="TxtEmail"
                            ErrorMessage="O email é obrigatório."
                            CssClass="text-danger small validation-message"
                            Display="Dynamic"
                            ValidationGroup="aluno" />

                        <asp:RegularExpressionValidator
                            ID="RevEmail"
                            runat="server"
                            ControlToValidate="TxtEmail"
                            ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                            ErrorMessage="Introduza um email válido."
                            CssClass="text-danger small validation-message"
                            Display="Dynamic"
                            ValidationGroup="aluno" />

                    </div>

                </div>

                <!-- TELEFONE -->

                <div class="row mb-3">

                    <label
                        for="<%= TxtTelefone.ClientID %>"
                        class="col-sm-3 col-form-label text-end campo-label">
                        Telefone

                    </label>

                    <div class="col-sm-5">

                        <asp:TextBox
                            ID="TxtTelefone"
                            runat="server"
                            CssClass="form-control border-secondary"
                            TextMode="Phone"
                            MaxLength="20"
                            placeholder="+351 912 345 678" />

                        <asp:RequiredFieldValidator
                            ID="RfvTelefone"
                            runat="server"
                            ControlToValidate="TxtTelefone"
                            ErrorMessage="O telefone é obrigatório."
                            CssClass="text-danger small validation-message"
                            Display="Dynamic"
                            ValidationGroup="aluno" />

                        <asp:RegularExpressionValidator
                            ID="RevTelefone"
                            runat="server"
                            ControlToValidate="TxtTelefone"
                            ValidationExpression="^[0-9+\s()\-]{7,20}$"
                            ErrorMessage="Introduza um número de telefone válido."
                            CssClass="text-danger small validation-message"
                            Display="Dynamic"
                            ValidationGroup="aluno" />

                    </div>

                </div>

                <!-- ESTADO -->

                <div class="row mb-3">

                    <div class="col-sm-3"></div>

                    <div class="col-sm-7">

                        <div class="form-check">

                            <asp:CheckBox
                                ID="ChkAtivo"
                                runat="server"
                                CssClass="form-check-input"
                                ClientIDMode="Static" />

                            <label
                                class="form-check-label"
                                for="ChkAtivo">
                                Aluno ativo

                            </label>

                        </div>

                    </div>

                </div>

                <!-- BOTÕES -->

                <div class="row">

                    <div class="col-sm-3"></div>

                    <div class="col-sm-7">

                        <asp:Button
                            ID="ButtonGuardar"
                            runat="server"
                            Text="Guardar"
                            CssClass="btn btn-primary"
                            OnClick="ButtonGuardar_Click"
                            ValidationGroup="aluno" />

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
