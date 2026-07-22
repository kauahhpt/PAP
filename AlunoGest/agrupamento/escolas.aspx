<%@ Page Title="" Language="C#" MasterPageFile="~/agrupamento/modeloAgrupamento.Master"
    AutoEventWireup="true" CodeBehind="escolas.aspx.cs" Inherits="AlunoGest.agrupamento.escolas"
    MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">

    <div class="container mt-4">

        <div class="row mb-4">
            <div class="col-12">
                <h2 class="mb-0">Gestão de Escolas</h2>
            </div>
        </div>

        <div class="row mb-3">
            <div class="col-12">
                <asp:Label ID="lblMensagem" runat="server" Visible="false"></asp:Label>
            </div>
        </div>

        <div class="card border-secondary shadow-sm mb-4">
            <div class="card-header bg-light border-secondary">
                <strong>Lista de Escolas</strong>
            </div>
            <div class="card-body">

                <asp:GridView ID="gridEscolas" runat="server"
                    CssClass="table table-bordered table-hover table-striped align-middle"
                    AutoGenerateColumns="False"
                    DataKeyNames="Id"
                    GridLines="None"
                    EmptyDataText="Não existem escolas registadas."
                    SelectedRowStyle-CssClass="table-primary">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="ID" ReadOnly="True" />
                        <asp:BoundField DataField="Nome" HeaderText="Nome" />
                        <asp:BoundField DataField="CodigoMEC" HeaderText="Código MEC" />
                        <asp:BoundField DataField="CodigoPostal" HeaderText="Código Postal" />
                        <asp:BoundField DataField="Localidade" HeaderText="Localidade" />
                        <asp:BoundField DataField="Email" HeaderText="Email" />
                        <asp:BoundField DataField="Telefone" HeaderText="Telefone" />
                        <asp:CheckBoxField DataField="Ativa" HeaderText="Ativa" />
                        <asp:CommandField ShowSelectButton="True" SelectText="Selecionar" HeaderText="Ações" />
                    </Columns>
                </asp:GridView>

                <div class="mt-3 d-flex gap-2 flex-wrap">
                    <asp:Button ID="buttonVer" runat="server" Text="Ver"
                        CssClass="btn btn-secondary" OnClick="buttonVer_Click" />

                    <asp:Button ID="buttonCriar" runat="server" Text="Criar"
                        CssClass="btn btn-success" OnClick="buttonCriar_Click" />

                    <asp:Button ID="buttonEditar" runat="server" Text="Editar"
                        CssClass="btn btn-warning text-dark" OnClick="buttonEditar_Click" />

                    <asp:Button Text="Gerir salas" runat="server" ID="buttonSalas"
                        CssClass="btn btn-outline-primary d-inline-block ms-4"
                        OnClick="buttonSalas_Click" CausesValidation="false" />

                    <asp:Button Text="Gerir turmas" runat="server" ID="buttonTurmas"
                        CssClass="btn btn-outline-primary d-inline-block ms-4"
                        OnClick="buttonTurmas_Click" CausesValidation="false" />

                    <asp:Button
                        ID="buttonVoltar"
                        runat="server"
                        Text="Voltar"
                        CssClass="btn btn-outline-secondary d-inline-block ms-4"
                        CausesValidation="false"
                        PostBackUrl="~/agrupamento/dashboard.aspx" />
                </div>


                <div class="my-4 d-flex gap-3 flex-wrap align-items-center">
                    <span class="d-inline-block me-3 fw-bold py-2">Nível de ensino</span>

                    <asp:DropDownList runat="server" CssClass="form-select border-secondary" Width="300" ID="ddlNivelEnsino">
                        <asp:ListItem Text="Ensino Básico - 3º Ciclo" />
                        <asp:ListItem Text="Ensino Secundário" />
                    </asp:DropDownList>

                    <asp:Button ID="buttonCriarTurma" runat="server" Text="Criar turma"
                        CssClass="btn btn-primary" OnClick="buttonCriarTurma_Click" />
                </div>


            </div>
        </div>

        <div id="controlos" runat="server" class="card border-secondary shadow-sm">
            <div class="card-header bg-light border-secondary">
                <strong>Dados da Escola</strong>
            </div>

            <div class="card-body">

                <div class="row mb-3 align-items-center">
                    <div class="col-md-2">
                        <label for="<%= txtNome.ClientID %>" class="col-form-label fw-semibold">Nome</label>
                    </div>
                    <div class="col-md-10">
                        <asp:TextBox ID="txtNome" runat="server" CssClass="form-control border-secondary"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvNome" runat="server"
                            ControlToValidate="txtNome"
                            ErrorMessage="O nome é obrigatório."
                            CssClass="text-danger"
                            Display="Dynamic"
                            ValidationGroup="vgEscola">
                        </asp:RequiredFieldValidator>
                    </div>
                </div>

                <div class="row mb-3 align-items-center">
                    <div class="col-md-2">
                        <label for="<%= txtCodigoMEC.ClientID %>" class="col-form-label fw-semibold">Código MEC</label>
                    </div>
                    <div class="col-md-4">
                        <asp:TextBox ID="txtCodigoMEC" runat="server" CssClass="form-control border-secondary" MaxLength="20"></asp:TextBox>
                    </div>

                    <div class="col-md-2">
                        <label for="<%= chkAtiva.ClientID %>" class="col-form-label fw-semibold">Ativa</label>
                    </div>
                    <div class="col-md-4">
                        <div class="form-check pt-2">
                            <asp:CheckBox ID="chkAtiva" runat="server" CssClass="form-check-input" />
                        </div>
                    </div>
                </div>

                <div class="row mb-3 align-items-center">
                    <div class="col-md-2">
                        <label for="<%= txtMorada.ClientID %>" class="col-form-label fw-semibold">Morada</label>
                    </div>
                    <div class="col-md-10">
                        <asp:TextBox ID="txtMorada" runat="server" CssClass="form-control border-secondary"></asp:TextBox>
                    </div>
                </div>

                <div class="row mb-3 align-items-center">
                    <div class="col-md-2">
                        <label for="<%= txtCodigoPostal.ClientID %>" class="col-form-label fw-semibold">Código Postal</label>
                    </div>
                    <div class="col-md-4">
                        <asp:TextBox ID="txtCodigoPostal" runat="server" CssClass="form-control border-secondary" MaxLength="8"></asp:TextBox>
                    </div>

                    <div class="col-md-2">
                        <label for="<%= txtLocalidade.ClientID %>" class="col-form-label fw-semibold">Localidade</label>
                    </div>
                    <div class="col-md-4">
                        <asp:TextBox ID="txtLocalidade" runat="server" CssClass="form-control border-secondary"></asp:TextBox>
                    </div>
                </div>

                <div class="row mb-3 align-items-center">
                    <div class="col-md-2">
                        <label for="<%= txtEmail.ClientID %>" class="col-form-label fw-semibold">Email</label>
                    </div>
                    <div class="col-md-4">
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control border-secondary"></asp:TextBox>
                    </div>

                    <div class="col-md-2">
                        <label for="<%= txtTelefone.ClientID %>" class="col-form-label fw-semibold">Telefone</label>
                    </div>
                    <div class="col-md-4">
                        <asp:TextBox ID="txtTelefone" runat="server" CssClass="form-control border-secondary"></asp:TextBox>
                    </div>
                </div>

                <div class="row mt-4">
                    <div class="col-12 d-flex gap-2 flex-wrap">
                        <asp:Button ID="buttonGuardar" runat="server" Text="Guardar"
                            CssClass="btn btn-primary"
                            ValidationGroup="vgEscola"
                            OnClick="buttonGuardar_Click" />

                        <asp:Button ID="buttonCancelar" runat="server" Text="Cancelar"
                            CssClass="btn btn-outline-secondary"
                            CausesValidation="false"
                            OnClick="buttonCancelar_Click" />
                    </div>
                </div>

            </div>
        </div>

    </div>

</asp:Content>
