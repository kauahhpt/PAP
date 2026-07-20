<%@ Page Title="Professores"
    Language="C#"
    MasterPageFile="~/agrupamento/modeloAgrupamento.Master"
    AutoEventWireup="true"
    CodeBehind="professores.aspx.cs"
    Inherits="AlunoGest.agrupamento.professores"
    MaintainScrollPositionOnPostback="true" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="headContent"
    runat="server">

    <style>
        .pagina-professores {
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

        .formulario-professor {
            margin-top: 35px;
            padding: 25px;
            border: 1px solid #dbe3ed;
            border-radius: 14px;
            background: #ffffff;
            box-shadow: 0 4px 14px rgba(15, 23, 42, 0.06);
        }

        .titulo-formulario {
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

    <div class="container pagina-professores">

        <div class="cabecalho-pagina">

            <h1>Professores</h1>

            <p>
                Crie e atualize os professores associados ao agrupamento.
            </p>

        </div>

        <div class="table-responsive">

            <asp:GridView
                ID="gridProfessores"
                runat="server"
                CssClass="table table-striped table-bordered align-middle"
                DataKeyNames="Id"
                AutoGenerateColumns="false"
                EmptyDataText="O agrupamento ainda não tem professores definidos."
                SelectedRowCssClass="linha-selecionada"
                AllowPaging="true"
                PageSize="10"
                OnPageIndexChanging="gridProfessores_PageIndexChanging">

                <Columns>

                    <asp:CommandField
                        ShowSelectButton="true"
                        SelectText="Selecionar"
                        CausesValidation="false" />

                    <asp:BoundField
                        DataField="Nome"
                        HeaderText="Nome" />

                    <asp:BoundField
                        DataField="GrupoRecrutamento"
                        HeaderText="Grupo de recrutamento"
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

                </Columns>

            </asp:GridView>

        </div>

        <div class="mt-4">

            <asp:Button
                ID="buttonVer"
                runat="server"
                Text="Ver dados do professor"
                CssClass="btn btn-outline-primary"
                CausesValidation="false"
                OnClick="buttonVer_Click" />

            <asp:Button
                ID="buttonCriar"
                runat="server"
                Text="Criar professor"
                CssClass="btn btn-outline-primary ms-2"
                CausesValidation="false"
                OnClick="buttonCriar_Click" />

            <asp:Button
                ID="buttonEditar"
                runat="server"
                Text="Editar professor"
                CssClass="btn btn-outline-primary ms-2"
                CausesValidation="false"
                OnClick="buttonEditar_Click" />

            <asp:Button
                ID="buttonDisciplinasProfessor"
                runat="server"
                Text="Disciplinas do professor"
                CssClass="btn btn-outline-primary ms-2"
                CausesValidation="false"
                OnClick="buttonDisciplinasProfessor_Click" />

        </div>

        <!-- FORMULÁRIO -->

        <div
            ID="controlos"
            runat="server"
            Visible="false"
            class="formulario-professor">

            <h2 class="titulo-formulario">
                Dados do professor
            </h2>

            <!-- NOME -->

            <div class="row mb-3">

                <label
                    for="<%= txtNome.ClientID %>"
                    class="col-sm-3 col-form-label text-end campo-label">

                    Nome

                </label>

                <div class="col-sm-7">

                    <asp:TextBox
                        ID="txtNome"
                        runat="server"
                        CssClass="form-control border-secondary"
                        MaxLength="200" />

                    <asp:RequiredFieldValidator
                        ID="rfvNome"
                        runat="server"
                        ControlToValidate="txtNome"
                        ErrorMessage="O nome é obrigatório."
                        CssClass="text-danger small validation-message"
                        Display="Dynamic"
                        ValidationGroup="professor" />

                </div>

            </div>

            <!-- EMAIL -->

            <div class="row mb-3">

                <label
                    for="<%= txtEmail.ClientID %>"
                    class="col-sm-3 col-form-label text-end campo-label">

                    Email

                </label>

                <div class="col-sm-7">

                    <asp:TextBox
                        ID="txtEmail"
                        runat="server"
                        CssClass="form-control border-secondary"
                        TextMode="Email"
                        MaxLength="150" />

                    <asp:RequiredFieldValidator
                        ID="rfvEmail"
                        runat="server"
                        ControlToValidate="txtEmail"
                        ErrorMessage="O email é obrigatório."
                        CssClass="text-danger small validation-message"
                        Display="Dynamic"
                        ValidationGroup="professor" />

                    <asp:RegularExpressionValidator
                        ID="revEmail"
                        runat="server"
                        ControlToValidate="txtEmail"
                        ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                        ErrorMessage="Introduza um endereço de email válido."
                        CssClass="text-danger small validation-message"
                        Display="Dynamic"
                        ValidationGroup="professor" />

                </div>

            </div>

            <!-- TELEFONE -->

            <div class="row mb-3">

                <label
                    for="<%= txtTelefone.ClientID %>"
                    class="col-sm-3 col-form-label text-end campo-label">

                    Telefone

                </label>

                <div class="col-sm-5">

                    <asp:TextBox
                        ID="txtTelefone"
                        runat="server"
                        CssClass="form-control border-secondary"
                        TextMode="Phone"
                        MaxLength="20"
                        placeholder="+351 912 345 678" />

                    <asp:RequiredFieldValidator
                        ID="rfvTelefone"
                        runat="server"
                        ControlToValidate="txtTelefone"
                        ErrorMessage="O telefone é obrigatório."
                        CssClass="text-danger small validation-message"
                        Display="Dynamic"
                        ValidationGroup="professor" />

                    <asp:RegularExpressionValidator
                        ID="revTelefone"
                        runat="server"
                        ControlToValidate="txtTelefone"
                        ValidationExpression="^[0-9+\s()\-]{7,20}$"
                        ErrorMessage="Introduza um número de telefone válido."
                        CssClass="text-danger small validation-message"
                        Display="Dynamic"
                        ValidationGroup="professor" />

                </div>

            </div>

            <!-- NIF -->

            <div class="row mb-3">

                <label
                    for="<%= txtNIF.ClientID %>"
                    class="col-sm-3 col-form-label text-end campo-label">

                    NIF

                </label>

                <div class="col-sm-5">

                    <asp:TextBox
                        ID="txtNIF"
                        runat="server"
                        CssClass="form-control border-secondary"
                        MaxLength="9"
                        inputmode="numeric"
                        placeholder="123456789" />

                    <asp:RequiredFieldValidator
                        ID="rfvNIF"
                        runat="server"
                        ControlToValidate="txtNIF"
                        ErrorMessage="O NIF é obrigatório."
                        CssClass="text-danger small validation-message"
                        Display="Dynamic"
                        ValidationGroup="professor" />

                    <asp:RegularExpressionValidator
                        ID="revNIF"
                        runat="server"
                        ControlToValidate="txtNIF"
                        ValidationExpression="^\d{9}$"
                        ErrorMessage="O NIF deve conter exatamente 9 algarismos."
                        CssClass="text-danger small validation-message"
                        Display="Dynamic"
                        ValidationGroup="professor" />

                </div>

            </div>

            <!-- NÚMERO DE PROCESSO -->

            <div class="row mb-3">

                <label
                    for="<%= txtNumeroProcesso.ClientID %>"
                    class="col-sm-3 col-form-label text-end campo-label">

                    Número de processo

                </label>

                <div class="col-sm-5">

                    <asp:TextBox
                        ID="txtNumeroProcesso"
                        runat="server"
                        CssClass="form-control border-secondary"
                        MaxLength="50" />

                    <asp:RequiredFieldValidator
                        ID="rfvNumeroProcesso"
                        runat="server"
                        ControlToValidate="txtNumeroProcesso"
                        ErrorMessage="O número de processo é obrigatório."
                        CssClass="text-danger small validation-message"
                        Display="Dynamic"
                        ValidationGroup="professor" />

                </div>

            </div>

            <!-- GRUPO -->

            <div class="row mb-3">

                <label
                    for="<%= ddlGrupoRecrutamento.ClientID %>"
                    class="col-sm-3 col-form-label text-end campo-label">

                    Grupo de recrutamento

                </label>

                <div class="col-sm-5">

                    <asp:DropDownList
                        ID="ddlGrupoRecrutamento"
                        runat="server"
                        CssClass="form-select border-secondary" />

                </div>

            </div>

            <!-- BOTÕES -->

            <div class="row">

                <div class="col-sm-3"></div>

                <div class="col-sm-7">

                    <asp:Button
                        ID="buttonGuardar"
                        runat="server"
                        Text="Guardar"
                        CssClass="btn btn-primary"
                        OnClick="buttonGuardar_Click"
                        ValidationGroup="professor" />

                    <asp:Button
                        ID="buttonCancelar"
                        runat="server"
                        Text="Cancelar"
                        CssClass="btn btn-warning ms-3"
                        CausesValidation="false"
                        OnClick="buttonCancelar_Click" />

                </div>

            </div>

        </div>

        <!-- DISCIPLINAS -->

        <div
            ID="painelDisciplinasProfessor"
            runat="server"
            class="mt-5"
            Visible="false">

            <h3>
                Disciplinas que o professor pode lecionar
            </h3>

            <div class="table-responsive">

                <asp:GridView
                    ID="gridDisciplinasProfessor"
                    runat="server"
                    CssClass="table table-striped table-bordered"
                    AutoGenerateColumns="false"
                    EmptyDataText="O professor não tem disciplinas associadas.">

                    <Columns>

                        <asp:BoundField
                            DataField="GrupoDisciplinar"
                            HeaderText="Grupo disciplinar" />

                        <asp:BoundField
                            DataField="Disciplina"
                            HeaderText="Disciplina" />

                        <asp:BoundField
                            DataField="Desde"
                            HeaderText="Desde"
                            DataFormatString="{0:yyyy-MM-dd}"
                            HtmlEncode="false" />

                        <asp:BoundField
                            DataField="Ate"
                            HeaderText="Até"
                            DataFormatString="{0:yyyy-MM-dd}"
                            HtmlEncode="false"
                            NullDisplayText="—" />

                        <asp:BoundField
                            DataField="Estado"
                            HeaderText="Estado" />

                    </Columns>

                </asp:GridView>

            </div>

        </div>

    </div>

</asp:Content>