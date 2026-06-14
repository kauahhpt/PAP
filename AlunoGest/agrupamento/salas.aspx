<%@ Page Title="" Language="C#" MasterPageFile="~/agrupamento/modeloAgrupamento.Master" 
    AutoEventWireup="true" CodeBehind="salas.aspx.cs" Inherits="AlunoGest.agrupamento.salas" 
    MaintainScrollPositionOnPostback="true"%>
<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">

    <div class="container">

        <h1>Salas</h1>

        <div class="mb-3">
            <asp:Label ID="lblEscola" runat="server" CssClass="fw-semibold"></asp:Label>
        </div>

        <asp:Label ID="lblMensagem" runat="server" Visible="false"></asp:Label>

        <asp:GridView runat="server" ID="gridSalas" CssClass="table table-striped linha-selecionada"
            DataKeyNames="Id" AutoGenerateColumns="False"
            EmptyDataText="A escola ainda não tem salas definidas."
            SelectedRowCssClass="linha-selecionada">

            <Columns>
                <asp:CommandField ShowSelectButton="True" SelectText="Selecionar" />
                <asp:BoundField DataField="Nome" HeaderText="Nome" />
                <asp:BoundField DataField="Capacidade" HeaderText="Capacidade" />
            </Columns>

        </asp:GridView>

        <div class="row mt-5">
            <div class="col-12" style="text-align: left;">
                <asp:Button Text="Ver dados da sala" runat="server" ID="buttonVer" CssClass="btn btn-outline-primary d-inline-block" OnClick="buttonVer_Click" />
                <asp:Button Text="Criar sala" runat="server" ID="buttonCriar" CssClass="btn btn-outline-primary d-inline-block ms-4" OnClick="buttonCriar_Click" />
                <asp:Button Text="Editar sala" runat="server" ID="buttonEditar" CssClass="btn btn-outline-primary d-inline-block ms-4" OnClick="buttonEditar_Click" />
                <asp:Button Text="Voltar às escolas" runat="server" ID="buttonVoltar" CssClass="btn btn-outline-secondary d-inline-block ms-4" OnClick="buttonVoltar_Click" CausesValidation="false" />
            </div>
        </div>

        <div id="controlos" runat="server" class="mt-4 row" visible="false" style="margin-top: 60px;">

            <div class="container">

                <!-- Nome -->
                <div class="row mb-2">
                    <label for="txtNome" class="col-sm-2 col-form-label text-end">Nome</label>
                    <div class="col-sm-4">
                        <asp:TextBox ID="txtNome" runat="server" CssClass="form-control border-secondary" />
                        <asp:RequiredFieldValidator
                            ID="rfvNome" runat="server"
                            ControlToValidate="txtNome"
                            ErrorMessage="O nome da sala é obrigatório."
                            CssClass="text-danger" />
                    </div>
                </div>

                <!-- Capacidade -->
                <div class="row mb-2">
                    <label for="txtCapacidade" class="col-sm-2 col-form-label text-end">Capacidade</label>
                    <div class="col-sm-2">
                        <asp:TextBox ID="txtCapacidade" runat="server" CssClass="form-control border-secondary" TextMode="Number" />
                        <asp:RangeValidator
                            ID="rvCapacidade" runat="server"
                            ControlToValidate="txtCapacidade"
                            MinimumValue="1"
                            MaximumValue="500"
                            Type="Integer"
                            ErrorMessage="A capacidade deve estar entre 1 e 500."
                            CssClass="text-danger" />
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
        </div>

    </div>

</asp:Content>

