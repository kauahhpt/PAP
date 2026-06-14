<%@ Page Title="" Language="C#" MasterPageFile="~/agrupamento/modeloAgrupamento.Master" 
    AutoEventWireup="true" CodeBehind="professores.aspx.cs" Inherits="AlunoGest.agrupamento.professores" 
    MaintainScrollPositionOnPostback="true"%>

<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">

    <div class="container">

        <h1>Professores</h1>

        <asp:GridView runat="server" ID="gridProfessores" CssClass="table table-striped linha-selecionada"
            DataKeyNames="Id" AutoGenerateColumns="False"
            EmptyDataText="O agrupamento ainda não tem professores definidos."
            SelectedRowCssClass="linha-selecionada"
            AllowPaging="true" PageSize="10"
            OnPageIndexChanging="gridProfessores_PageIndexChanging">

            <Columns>
                <asp:CommandField ShowSelectButton="True" SelectText="Selecionar" />
                <asp:BoundField DataField="Nome" HeaderText="Nome" />
                <asp:BoundField DataField="GrupoRecrutamento" HeaderText="Grupo de recrutamento" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
            </Columns>

        </asp:GridView>

        <div class="row mt-5">
            <div class="col-12" style="text-align: left;">
                <asp:Button Text="Ver dados do professor" runat="server" ID="buttonVer" CssClass="btn btn-outline-primary d-inline-block" OnClick="buttonVer_Click" />
                <asp:Button Text="Criar professor" runat="server" ID="buttonCriar" CssClass="btn btn-outline-primary d-inline-block ms-4" OnClick="buttonCriar_Click" />
                <asp:Button Text="Editar professor" runat="server" ID="buttonEditar" CssClass="btn btn-outline-primary d-inline-block ms-4" OnClick="buttonEditar_Click" />
                <asp:Button Text="Disciplinas do professor" runat="server" ID="buttonDisciplinasProfessor" CssClass="btn btn-outline-primary d-inline-block ms-4" OnClick="buttonDisciplinasProfessor_Click" />
            </div>
        </div>

        <div id="controlos" runat="server" class="mt-4 row" visible="true" style="margin-top: 60px;">

            <div class="container">

                <div class="container">

                    <!-- Nome -->
                    <div class="row mb-2">
                        <label for="txtNome" class="col-sm-2 col-form-label text-end">Nome</label>
                        <div class="col-sm-6">
                            <asp:TextBox ID="txtNome" runat="server" CssClass="form-control border-secondary" />
                            <asp:RequiredFieldValidator
                                ID="rfvNome" runat="server"
                                ControlToValidate="txtNome"
                                ErrorMessage="O Nome é obrigatório"
                                CssClass="text-danger" />
                        </div>
                    </div>

                    <!-- Email -->
                    <div class="row mb-2">
                        <label for="txtEmail" class="col-sm-2 col-form-label text-end">Email</label>
                        <div class="col-sm-6">
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control border-secondary" />
                            <asp:RequiredFieldValidator
                                ID="rfvEmail" runat="server"
                                ControlToValidate="txtEmail"
                                ErrorMessage="O email é obrigatório"
                                CssClass="text-danger" />
                        </div>
                    </div>

                    <!-- Número de processo -->
                    <div class="row mb-2">
                        <label for="txtNumeroProcesso" class="col-sm-2 col-form-label text-end">Número de processo</label>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtNumeroProcesso" runat="server" CssClass="form-control border-secondary" />
                            <asp:RequiredFieldValidator
                                ID="rfvCodigoPostal" runat="server"
                                ControlToValidate="txtNumeroProcesso"
                                ErrorMessage="O número de processo é obrigatório"
                                CssClass="text-danger" />
                        </div>
                    </div>

                    <!-- grupo de recrutamento -->
                    <div class="row mb-2">
                        <label for="ddlGrupoRecrutamento" class="col-sm-2 col-form-label text-end">Grupo de recrutamento</label>
                        <div class="col-sm-3">
                            <asp:DropDownList runat="server" CssClass="form-select border-secondary" ID="ddlGrupoRecrutamento"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-sm-2"></div>
                        <div class="col-sm-8 text-end">
                            <asp:Button Text="Guardar" runat="server" CssClass="btn btn-primary d-inline-block" ID="buttonGuardar" OnClick="buttonGuardar_Click" />
                            <asp:Button Text="Cancelar" runat="server" CssClass="btn btn-warning d-inline-block ms-4"
                                ID="buttonCancelar" OnClick="buttonCancelar_Click" CausesValidation="false" />
                        </div>
                    </div>

                </div>

                <div id="painelDisciplinasProfessor" runat="server" class="container mt-5" visible="false">
                    <h3>Disciplinas que o professor pode lecionar</h3>

                    <asp:GridView runat="server" ID="gridDisciplinasProfessor"
                        CssClass="table table-striped table-bordered"
                        AutoGenerateColumns="False"
                        EmptyDataText="O professor não tem disciplinas associadas.">

                        <Columns>
                            <asp:BoundField DataField="GrupoDisciplinar" HeaderText="Grupo disciplinar" />
                            <asp:BoundField DataField="Disciplina" HeaderText="Disciplina" />
                            <asp:BoundField DataField="Desde" HeaderText="Desde" DataFormatString="{0:yyyy-MM-dd}" HtmlEncode="false" />
                            <asp:BoundField DataField="Ate" HeaderText="Até" DataFormatString="{0:yyyy-MM-dd}" HtmlEncode="false" NullDisplayText="—" />
                            <asp:BoundField DataField="Estado" HeaderText="Estado" />
                        </Columns>

                    </asp:GridView>
                </div>

            </div>
        </div>

    </div>

</asp:Content>