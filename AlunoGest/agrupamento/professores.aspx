<%@ Page Title="" Language="C#"
    MasterPageFile="~/agrupamento/modeloAgrupamento.Master"
    AutoEventWireup="true"
    CodeBehind="professores.aspx.cs"
    Inherits="AlunoGest.agrupamento.professores"
    MaintainScrollPositionOnPostback="true" %>

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

        <h1>Professores</h1>

        <asp:GridView
            ID="gridProfessores"
            runat="server"
            CssClass="table table-striped linha-selecionada"
            DataKeyNames="Id"
            AutoGenerateColumns="False"
            EmptyDataText="O agrupamento ainda não tem professores definidos."
            SelectedRowCssClass="linha-selecionada"
            AllowPaging="true"
            PageSize="10"
            OnPageIndexChanging="gridProfessores_PageIndexChanging">

            <Columns>

                <asp:CommandField
                    ShowSelectButton="True"
                    SelectText="Selecionar" />

                <asp:BoundField
                    DataField="Nome"
                    HeaderText="Nome" />

                <asp:BoundField
                    DataField="GrupoRecrutamento"
                    HeaderText="Grupo de recrutamento" />

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

        <div class="row mt-5">

            <div class="col-12" style="text-align: left;">

                <asp:Button
                    ID="buttonVer"
                    runat="server"
                    Text="Ver dados do professor"
                    CssClass="btn btn-outline-primary d-inline-block"
                    CausesValidation="false"
                    OnClick="buttonVer_Click" />

                <asp:Button
                    ID="buttonCriar"
                    runat="server"
                    Text="Criar professor"
                    CssClass="btn btn-outline-primary d-inline-block ms-4"
                    CausesValidation="false"
                    OnClick="buttonCriar_Click" />

                <asp:Button
                    ID="buttonEditar"
                    runat="server"
                    Text="Editar professor"
                    CssClass="btn btn-outline-primary d-inline-block ms-4"
                    CausesValidation="false"
                    OnClick="buttonEditar_Click" />

                <asp:Button
                    ID="buttonDisciplinasProfessor"
                    runat="server"
                    Text="Disciplinas do professor"
                    CssClass="btn btn-outline-primary d-inline-block ms-4"
                    CausesValidation="false"
                    OnClick="buttonDisciplinasProfessor_Click" />

            </div>

        </div>

        <div
            ID="controlos"
            runat="server"
            class="mt-4 row"
            visible="true"
            style="margin-top: 60px;">

            <div class="container">

                <div class="container">

                    <!-- Nome -->
                    <div class="row mb-2">

                        <label
                            for="txtNome"
                            class="col-sm-2 col-form-label text-end">
                            Nome
                        </label>

                        <div class="col-sm-6">

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
                                CssClass="text-danger"
                                Display="Dynamic" />

                        </div>

                    </div>

                    <!-- Email -->
                    <div class="row mb-2">

                        <label
                            for="txtEmail"
                            class="col-sm-2 col-form-label text-end">
                            Email
                        </label>

                        <div class="col-sm-6">

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
                                CssClass="text-danger"
                                Display="Dynamic" />

                            <asp:RegularExpressionValidator
                                ID="revEmail"
                                runat="server"
                                ControlToValidate="txtEmail"
                                ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                                ErrorMessage="Introduza um endereço de email válido."
                                CssClass="text-danger"
                                Display="Dynamic" />

                        </div>

                    </div>

                    <!-- Telefone -->
                    <div class="row mb-2">

                        <label
                            for="txtTelefone"
                            class="col-sm-2 col-form-label text-end">
                            Telefone
                        </label>

                        <div class="col-sm-3">

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
                                CssClass="text-danger"
                                Display="Dynamic" />

                        </div>

                    </div>

                    <!-- Número de processo -->
                    <div class="row mb-2">

                        <label
                            for="txtNumeroProcesso"
                            class="col-sm-2 col-form-label text-end">
                            Número de processo
                        </label>

                        <div class="col-sm-3">

                            <asp:TextBox
                                ID="txtNumeroProcesso"
                                runat="server"
                                CssClass="form-control border-secondary"
                                MaxLength="50" />

                            <asp:RequiredFieldValidator
                                ID="rfvCodigoPostal"
                                runat="server"
                                ControlToValidate="txtNumeroProcesso"
                                ErrorMessage="O número de processo é obrigatório."
                                CssClass="text-danger"
                                Display="Dynamic" />

                        </div>

                    </div>

                    <!-- Grupo de recrutamento -->
                    <div class="row mb-2">

                        <label
                            for="ddlGrupoRecrutamento"
                            class="col-sm-2 col-form-label text-end">
                            Grupo de recrutamento
                        </label>

                        <div class="col-sm-3">

                            <asp:DropDownList
                                ID="ddlGrupoRecrutamento"
                                runat="server"
                                CssClass="form-select border-secondary">
                            </asp:DropDownList>

                        </div>

                    </div>

                    <!-- Botões -->
                    <div class="row mb-2">

                        <div class="col-sm-2"></div>

                        <div class="col-sm-8 text-end">

                            <asp:Button
                                ID="buttonGuardar"
                                runat="server"
                                Text="Guardar"
                                CssClass="btn btn-primary d-inline-block"
                                OnClick="buttonGuardar_Click" />

                            <asp:Button
                                ID="buttonCancelar"
                                runat="server"
                                Text="Cancelar"
                                CssClass="btn btn-warning d-inline-block ms-4"
                                CausesValidation="false"
                                OnClick="buttonCancelar_Click" />

                        </div>

                    </div>

                </div>

                <div
                    ID="painelDisciplinasProfessor"
                    runat="server"
                    class="container mt-5"
                    visible="false">

                    <h3>Disciplinas que o professor pode lecionar</h3>

                    <asp:GridView
                        ID="gridDisciplinasProfessor"
                        runat="server"
                        CssClass="table table-striped table-bordered"
                        AutoGenerateColumns="False"
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

    </div>

</asp:Content>