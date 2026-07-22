<%@ Page Title="" Language="C#" MasterPageFile="~/agrupamento/modeloAgrupamento.Master"
    AutoEventWireup="true" CodeBehind="turmas.aspx.cs"
    Inherits="AlunoGest.agrupamento.turmas"
    MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">

    <style type="text/css">
        .caixa-resumo-oferta {
            border: 1px solid #ced4da;
            border-radius: 6px;
            background-color: #f8f9fa;
            padding: 12px;
        }

        .checklist-esquerda {
            text-align: left;
        }

            .checklist-esquerda input[type="checkbox"] {
                margin-right: 6px;
            }

            .checklist-esquerda label {
                display: inline-block;
                margin-bottom: 6px;
            }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">

    <div class="container">

        <h1>Turmas</h1>

        <div class="mb-3">
            <asp:Label ID="lblEscola" runat="server" CssClass="fw-semibold"></asp:Label>
        </div>

        <asp:Label ID="lblMensagem" runat="server" Visible="false"></asp:Label>

        <!-- Ano letivo -->
        <div class="row mb-4">
            <label for="ddlAnoLetivo" class="col-sm-2 col-form-label text-end">Ano letivo</label>
            <div class="col-sm-3">
                <asp:DropDownList ID="ddlAnoLetivo" runat="server"
                    CssClass="form-select border-secondary"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="ddlAnoLetivo_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
        </div>

        <!-- Oferta de Escola -->
        <div class="card border-secondary mb-4">
            <div class="card-header d-flex justify-content-between align-items-center">
                <strong>Oferta de Escola do 3.º ciclo</strong>

                <asp:Button ID="buttonDefinirOfertaEscola" runat="server"
                    Text="Definir oferta de escola"
                    CssClass="btn btn-sm btn-primary"
                    OnClick="buttonDefinirOfertaEscola_Click" />
            </div>

            <div class="card-body">

                <!-- Resumo da oferta já definida -->
                <div id="painelResumoOfertaEscola" runat="server" visible="false" class="mb-4">
                    <div class="caixa-resumo-oferta">

                        <div class="row mb-2">
                            <div class="col-sm-3 fw-semibold">Disciplina definida</div>
                            <div class="col-sm-9">
                                <asp:Label ID="lblOfertaEscolaAtual" runat="server"></asp:Label>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-sm-3 fw-semibold">Grupos disciplinares</div>
                            <div class="col-sm-9">
                                <asp:Label ID="lblGruposOfertaEscolaAtual" runat="server"></asp:Label>
                            </div>
                        </div>

                    </div>
                </div>

                <!-- Painel de edição da oferta -->
                <div id="painelEditarOfertaEscola" runat="server" visible="false">

                    <div class="row mb-3">
                        <label for="txtNomeOfertaEscola" class="col-sm-2 col-form-label text-end">
                            Nome da disciplina
                       
                        </label>
                        <div class="col-sm-6">
                            <asp:TextBox ID="txtNomeOfertaEscola" runat="server"
                                CssClass="form-control border-secondary"
                                placeholder="Ex.: Complemento à Educação Artística">
                            </asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-sm-2 col-form-label text-end">
                            Grupos disciplinares
                       
                        </label>

                        <div class="col-sm-6">
                            <asp:CheckBoxList ID="cblGruposOfertaEscola" runat="server"
                                CssClass="checklist-esquerda"
                                RepeatColumns="2"
                                RepeatDirection="Vertical">
                            </asp:CheckBoxList>

                            <div class="form-text">
                                Seleciona os grupos disciplinares que podem lecionar esta disciplina.
                           
                            </div>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-sm-2"></div>
                        <div class="col-sm-6">
                            <asp:Button ID="buttonGuardarOfertaEscola" runat="server"
                                Text="Guardar"
                                CssClass="btn btn-primary"
                                OnClick="buttonGuardarOfertaEscola_Click" />

                            <asp:Button ID="buttonCancelarOfertaEscola" runat="server"
                                Text="Cancelar"
                                CssClass="btn btn-warning ms-3"
                                CausesValidation="false"
                                OnClick="buttonCancelarOfertaEscola_Click" />
                        </div>
                    </div>

                </div>

            </div>
        </div>

        <!-- Zona para criar turma -->
        <div class="row mb-4">
            <label for="ddlTipoTurma" class="col-sm-2 col-form-label text-end">Tipo de turma</label>

            <div class="col-sm-3">
                <asp:DropDownList ID="ddlTipoTurma" runat="server" CssClass="form-select border-secondary">
                    <asp:ListItem Text="-- tipo de turma --" Value=""></asp:ListItem>
                    <asp:ListItem Text="Turma do 3.º ciclo" Value="basico"></asp:ListItem>
                    <asp:ListItem Text="Turma do secundário" Value="secundario"></asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="col-sm-7 text-end">
                <asp:Button ID="buttonCriarTurma" runat="server"
                    Text="Criar turma"
                    CssClass="btn btn-primary d-inline-block"
                    OnClick="buttonCriarTurma_Click" />

                <asp:Button
                    ID="buttonVoltar"
                    runat="server"
                    Text="Voltar"
                    CssClass="btn btn-outline-secondary d-inline-block ms-4"
                    CausesValidation="false"
                    PostBackUrl="~/agrupamento/escolas.aspx" />
            </div>
        </div>

        <!-- Lista de turmas -->
        <asp:GridView ID="gridTurmas" runat="server"
            CssClass="table table-striped table-bordered"
            AutoGenerateColumns="False"
            DataKeyNames="Id"
            EmptyDataText="Não existem turmas para os critérios selecionados."
            OnRowCommand="gridTurmas_RowCommand">

            <Columns>
                <asp:BoundField DataField="CodigoTurma" HeaderText="Código" />
                <asp:BoundField DataField="AnoEscolaridade" HeaderText="Ano" />
                <asp:BoundField DataField="AnoLetivo" HeaderText="Ano letivo" />
                <asp:BoundField DataField="PlanoCurricular" HeaderText="Tipo" />
                <asp:CheckBoxField DataField="Ativa" HeaderText="Ativa" ReadOnly="true" />

                <asp:TemplateField HeaderText="Ações">
                    <ItemTemplate>
                        <asp:Button ID="buttonEditarTurma" runat="server"
                            Text="Editar"
                            CommandName="editarTurma"
                            CommandArgument='<%# Eval("Id") %>'
                            CssClass="btn btn-sm btn-primary" />

                        <asp:Button ID="buttonProfessores" runat="server"
                            Text="Professores"
                            CommandName="professoresTurma"
                            CommandArgument='<%# Eval("Id") %>'
                            CssClass="btn btn-sm btn-secondary ms-2" />


                        <asp:Button ID="buttonAlunos" runat="server"
                            Text="Alunos"
                            CommandName="alunosTurma"
                            CommandArgument='<%# Eval("Id") %>'
                            CssClass="btn btn-sm btn-info ms-2" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>

        </asp:GridView>

    </div>

</asp:Content>
