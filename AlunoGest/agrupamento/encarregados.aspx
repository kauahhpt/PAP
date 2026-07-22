<%@ Page
    Title="Encarregados de Educação"
    Language="C#"
    MasterPageFile="~/agrupamento/modeloAgrupamento.Master"
    AutoEventWireup="true"
    CodeBehind="encarregados.aspx.cs"
    Inherits="AlunoGest.agrupamento.Encarregados"
    MaintainScrollPositionOnPostback="true" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="headContent"
    runat="server">

    <style>
        /* =====================================================
           ESTRUTURA DA PÁGINA
        ===================================================== */

        .pagina-encarregados {
            padding-bottom: 45px;
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
                max-width: 780px;
                margin: 0;
                color: #64748b;
                line-height: 1.6;
            }


        /* =====================================================
           CARDS
        ===================================================== */

        .pagina-card {
            overflow: hidden;
            border: 1px solid #dbe3ed;
            border-radius: 14px;
            background: #ffffff;
            box-shadow: 0 4px 14px rgba(15, 23, 42, 0.06);
        }

        .pagina-card-header {
            padding: 20px 22px;
            border-bottom: 1px solid #e5e7eb;
            background: #f8fafc;
        }

            .pagina-card-header h2 {
                margin: 0 0 5px;
                color: #1f2937;
                font-size: 20px;
                font-weight: 800;
            }

            .pagina-card-header p {
                margin: 0;
                color: #64748b;
                font-size: 13px;
            }

        .pagina-card-body {
            padding: 24px;
        }


        /* =====================================================
           GRIDVIEW
        ===================================================== */

        .linha-selecionada {
            background-color: #dbeafe !important;
        }

        .nome-encarregado {
            color: #1f2937;
            font-weight: 700;
        }

        .texto-secundario {
            display: block;
            margin-top: 3px;
            color: #94a3b8;
            font-size: 11px;
        }

        .estado-ativo,
        .estado-inativo,
        .estado-principal {
            display: inline-flex;
            align-items: center;
            padding: 4px 9px;
            border-radius: 999px;
            font-size: 11px;
            font-weight: 700;
        }

        .estado-ativo {
            border: 1px solid #bbf7d0;
            background: #f0fdf4;
            color: #166534;
        }

        .estado-inativo {
            border: 1px solid #e2e8f0;
            background: #f8fafc;
            color: #64748b;
        }

        .estado-principal {
            border: 1px solid #bfdbfe;
            background: #eff6ff;
            color: #1d4ed8;
        }


        /* =====================================================
           FORMULÁRIOS
        ===================================================== */

        .formulario-titulo {
            margin-bottom: 22px;
            color: #1f2937;
            font-size: 21px;
            font-weight: 800;
        }

        .formulario-subtitulo {
            margin: 26px 0 18px;
            padding-top: 22px;
            border-top: 1px solid #e5e7eb;
            color: #334155;
            font-size: 16px;
            font-weight: 800;
        }

        .campo-obrigatorio::after {
            content: " *";
            color: #dc2626;
        }

        .validation-message {
            display: block;
            margin-top: 5px;
            color: #dc3545;
            font-size: 12px;
            font-weight: 600;
        }

        .campo-ajuda {
            display: block;
            margin-top: 5px;
            color: #64748b;
            font-size: 12px;
            line-height: 1.5;
        }

        .form-check-area {
            min-height: 38px;
            display: flex;
            align-items: center;
        }


        /* =====================================================
           EDUCANDOS ASSOCIADOS
        ===================================================== */

        .associacao-area {
            border: 1px solid #dbeafe;
            border-radius: 12px;
            background: #f8fbff;
        }

        .associacao-formulario {
            padding: 20px;
            border-bottom: 1px solid #dbeafe;
        }

            .associacao-formulario h3 {
                margin: 0 0 5px;
                color: #1e3a8a;
                font-size: 17px;
                font-weight: 800;
            }

            .associacao-formulario > p {
                margin: 0 0 20px;
                color: #64748b;
                font-size: 13px;
            }

        .educandos-lista {
            padding: 20px;
        }

            .educandos-lista h3 {
                margin: 0 0 15px;
                color: #334155;
                font-size: 16px;
                font-weight: 800;
            }


        /* =====================================================
           RESPONSIVO
        ===================================================== */

        @media (max-width: 768px) {

            .campo-label {
                margin-bottom: 5px;
                text-align: left !important;
            }

            .pagina-card-body {
                padding: 18px;
            }

            .cabecalho-pagina h1 {
                font-size: 25px;
            }

            .botoes-listagem .btn {
                width: 100%;
                margin: 0 0 9px !important;
            }

            .botoes-formulario .btn {
                width: 100%;
                margin: 0 0 9px !important;
            }
        }
    </style>

</asp:Content>


<asp:Content
    ID="Content2"
    ContentPlaceHolderID="mainContent"
    runat="server">

    <div class="container pagina-encarregados">


        <!-- ==================================================
             CABEÇALHO
        =================================================== -->

        <div class="cabecalho-pagina">

            <h1>Encarregados de Educação
            </h1>

            <p>
                Crie as contas dos encarregados de educação e associe
                cada encarregado aos respetivos alunos do agrupamento.
            </p>

        </div>


        <!-- ==================================================
             MENSAGEM
        =================================================== -->

        <asp:Label
            ID="LblMensagem"
            runat="server"
            Visible="false" />


        <!-- ==================================================
             LISTA DE ENCARREGADOS
        =================================================== -->

        <section class="pagina-card mb-4">

            <header class="pagina-card-header">

                <h2>Encarregados registados
                </h2>

                <p>
                    Selecione um encarregado para editar os seus dados
                    ou gerir os educandos associados.
                </p>

            </header>

            <div class="table-responsive">

                <asp:GridView
                    ID="GridEncarregados"
                    runat="server"
                    CssClass="table table-striped table-bordered align-middle mb-0"
                    AutoGenerateColumns="false"
                    DataKeyNames="Id"
                    GridLines="None"
                    EmptyDataText="Não existem encarregados registados."
                    SelectedRowCssClass="linha-selecionada">

                    <Columns>

                        <asp:CommandField
                            ShowSelectButton="true"
                            SelectText="Selecionar"
                            CausesValidation="false" />



                        <asp:TemplateField HeaderText="Encarregado">

                            <ItemTemplate>

                                <span class="nome-encarregado">

                                    <%# Eval("NomeCompleto") %>

                                </span>

                                <span class="texto-secundario">Registo n.º
                                    <%# Eval("Id") %>

                                </span>

                            </ItemTemplate>

                        </asp:TemplateField>



                        <asp:BoundField
                            DataField="NIF"
                            HeaderText="NIF"
                            NullDisplayText="—" />



                        <asp:BoundField
                            DataField="Email"
                            HeaderText="Email" />



                        <asp:BoundField
                            DataField="Telefone"
                            HeaderText="Telefone"
                            NullDisplayText="—" />



                        <asp:BoundField
                            DataField="NumeroEducandos"
                            HeaderText="Educandos"
                            NullDisplayText="0" />



                        <asp:TemplateField HeaderText="Estado">

                            <ItemTemplate>

                                <asp:Label
                                    ID="LblEstadoEncarregado"
                                    runat="server"
                                    Text='<%# Convert.ToBoolean(Eval("Ativo"))
                                        ? "Ativo"
                                        : "Inativo" %>'
                                    CssClass='<%# Convert.ToBoolean(Eval("Ativo"))
                                        ? "estado-ativo"
                                        : "estado-inativo" %>' />

                            </ItemTemplate>

                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>

        </section>




        <div class="mb-4 botoes-listagem">

            <asp:Button
                ID="ButtonCriar"
                runat="server"
                Text="Novo encarregado"
                CssClass="btn btn-primary"
                CausesValidation="false" />

            <asp:Button
                ID="ButtonEditar"
                runat="server"
                Text="Editar encarregado"
                CssClass="btn btn-secondary ms-2"
                CausesValidation="false" />

            <asp:Button
                ID="ButtonGerirEducandos"
                runat="server"
                Text="Gerir educandos"
                CssClass="btn btn-outline-primary ms-2"
                CausesValidation="false" />

            <asp:Button
                ID="buttonVoltar"
                runat="server"
                Text="Voltar"
                CssClass="btn btn-outline-secondary d-inline-block ms-4"
                CausesValidation="false"
                PostBackUrl="~/agrupamento/dashboard.aspx" />

        </div>



        <asp:Panel
            ID="PnlFormulario"
            runat="server"
            Visible="false"
            CssClass="pagina-card mb-4">

            <div class="pagina-card-body">

                <h2 class="formulario-titulo">Dados do encarregado
                </h2>


                <asp:ValidationSummary
                    ID="ValidationSummaryEncarregado"
                    runat="server"
                    ValidationGroup="encarregado"
                    HeaderText="Verifique os seguintes campos:"
                    DisplayMode="BulletList"
                    CssClass="alert alert-warning" />



                <div class="row mb-3">

                    <label
                        for="<%= TxtNomeCompleto.ClientID %>"
                        class="
                            col-sm-3
                            col-form-label
                            text-end
                            campo-label
                            campo-obrigatorio">
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
                            ValidationGroup="encarregado"
                            Display="Dynamic"
                            CssClass="validation-message"
                            ErrorMessage="O nome completo é obrigatório." />

                    </div>

                </div>



                <div class="row mb-3">

                    <label
                        for="<%= TxtNIF.ClientID %>"
                        class="
                            col-sm-3
                            col-form-label
                            text-end
                            campo-label
                            campo-obrigatorio">
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
                            ValidationGroup="encarregado"
                            Display="Dynamic"
                            CssClass="validation-message"
                            ErrorMessage="O NIF é obrigatório." />

                        <asp:RegularExpressionValidator
                            ID="RevNIF"
                            runat="server"
                            ControlToValidate="TxtNIF"
                            ValidationGroup="encarregado"
                            ValidationExpression="^\d{9}$"
                            Display="Dynamic"
                            CssClass="validation-message"
                            ErrorMessage="O NIF deve conter exatamente 9 algarismos." />

                    </div>

                </div>



                <div class="row mb-3">

                    <label
                        for="<%= TxtEmail.ClientID %>"
                        class="
                            col-sm-3
                            col-form-label
                            text-end
                            campo-label
                            campo-obrigatorio">
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
                            ValidationGroup="encarregado"
                            Display="Dynamic"
                            CssClass="validation-message"
                            ErrorMessage="O email é obrigatório." />

                        <asp:RegularExpressionValidator
                            ID="RevEmail"
                            runat="server"
                            ControlToValidate="TxtEmail"
                            ValidationGroup="encarregado"
                            ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                            Display="Dynamic"
                            CssClass="validation-message"
                            ErrorMessage="Introduza um endereço de email válido." />

                        <span class="campo-ajuda">As credenciais de acesso serão enviadas para este email.
                        </span>

                    </div>

                </div>



                <div class="row mb-3">

                    <label
                        for="<%= TxtTelefone.ClientID %>"
                        class="
                            col-sm-3
                            col-form-label
                            text-end
                            campo-label
                            campo-obrigatorio">
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
                            ValidationGroup="encarregado"
                            Display="Dynamic"
                            CssClass="validation-message"
                            ErrorMessage="O telefone é obrigatório." />

                        <asp:RegularExpressionValidator
                            ID="RevTelefone"
                            runat="server"
                            ControlToValidate="TxtTelefone"
                            ValidationGroup="encarregado"
                            ValidationExpression="^[0-9+\s()\-]{7,20}$"
                            Display="Dynamic"
                            CssClass="validation-message"
                            ErrorMessage="Introduza um número de telefone válido." />

                    </div>

                </div>



                <div class="row mb-3">

                    <div class="col-sm-3"></div>

                    <div class="col-sm-7">

                        <div class="form-check-area">

                            <div class="form-check">

                                <asp:CheckBox
                                    ID="ChkAtivo"
                                    runat="server"
                                    CssClass="form-check-input"
                                    ClientIDMode="Static" />

                                <label
                                    class="form-check-label"
                                    for="ChkAtivo">
                                    Encarregado ativo

                                </label>

                            </div>

                        </div>

                    </div>

                </div>



                <asp:Panel
                    ID="PnlAssociacaoInicial"
                    runat="server"
                    Visible="false">

                    <h3 class="formulario-subtitulo">Associar o primeiro educando
                    </h3>



                    <div class="row mb-3">

                        <label
                            for="<%= DdlAlunoInicial.ClientID %>"
                            class="
                                col-sm-3
                                col-form-label
                                text-end
                                campo-label
                                campo-obrigatorio">
                            Aluno

                        </label>

                        <div class="col-sm-7">

                            <asp:DropDownList
                                ID="DdlAlunoInicial"
                                runat="server"
                                CssClass="form-select border-secondary"
                                AppendDataBoundItems="true">

                                <asp:ListItem
                                    Text="Selecione um aluno..."
                                    Value="" />

                            </asp:DropDownList>

                            <asp:RequiredFieldValidator
                                ID="RfvAlunoInicial"
                                runat="server"
                                ControlToValidate="DdlAlunoInicial"
                                InitialValue=""
                                ValidationGroup="encarregado"
                                Display="Dynamic"
                                CssClass="validation-message"
                                ErrorMessage="Selecione o aluno associado." />

                        </div>

                    </div>



                    <div class="row mb-3">

                        <label
                            for="<%= TxtParentescoInicial.ClientID %>"
                            class="
                                col-sm-3
                                col-form-label
                                text-end
                                campo-label">
                            Parentesco

                        </label>

                        <div class="col-sm-5">

                            <asp:TextBox
                                ID="TxtParentescoInicial"
                                runat="server"
                                CssClass="form-control border-secondary"
                                MaxLength="50"
                                placeholder="Ex.: Mãe, Pai, Avó, Tutor..." />

                        </div>

                    </div>



                    <div class="row mb-3">

                        <div class="col-sm-3"></div>

                        <div class="col-sm-7">

                            <div class="form-check">

                                <asp:CheckBox
                                    ID="ChkPrincipalInicial"
                                    runat="server"
                                    CssClass="form-check-input"
                                    ClientIDMode="Static" />

                                <label
                                    class="form-check-label"
                                    for="ChkPrincipalInicial">
                                    Definir como encarregado principal deste aluno

                                </label>

                            </div>

                        </div>

                    </div>

                </asp:Panel>



                <div class="row mt-4">

                    <div class="col-sm-3"></div>

                    <div class="col-sm-7 botoes-formulario">

                        <asp:Button
                            ID="ButtonGuardar"
                            runat="server"
                            Text="Guardar"
                            CssClass="btn btn-primary"
                            ValidationGroup="encarregado" />

                        <asp:Button
                            ID="ButtonCancelar"
                            runat="server"
                            Text="Cancelar"
                            CssClass="btn btn-warning ms-3"
                            CausesValidation="false" />

                    </div>

                </div>

            </div>

        </asp:Panel>


        <asp:Panel
            ID="PnlEducandos"
            runat="server"
            Visible="false"
            CssClass="pagina-card mb-4">

            <header class="pagina-card-header">

                <h2>Educandos associados
                </h2>

                <p>
                    Associe ou remova alunos do encarregado selecionado.
                </p>

            </header>

            <div class="pagina-card-body">

                <asp:Label
                    ID="LblEncarregadoSelecionado"
                    runat="server"
                    CssClass="alert alert-info d-block mb-4" />


                <div class="associacao-area mb-4">

                    <div class="associacao-formulario">

                        <h3>Nova associação
                        </h3>

                        <p>
                            Selecione um aluno que pertence ao agrupamento.
                        </p>


                        <asp:ValidationSummary
                            ID="ValidationSummaryAssociacao"
                            runat="server"
                            ValidationGroup="associacao"
                            HeaderText="Verifique os seguintes campos:"
                            DisplayMode="BulletList"
                            CssClass="alert alert-warning" />



                        <div class="row mb-3">

                            <label
                                for="<%= DdlAlunoAssociar.ClientID %>"
                                class="
                                    col-sm-3
                                    col-form-label
                                    text-end
                                    campo-label
                                    campo-obrigatorio">
                                Aluno

                            </label>

                            <div class="col-sm-7">

                                <asp:DropDownList
                                    ID="DdlAlunoAssociar"
                                    runat="server"
                                    CssClass="form-select border-secondary"
                                    AppendDataBoundItems="true">

                                    <asp:ListItem
                                        Text="Selecione um aluno..."
                                        Value="" />

                                </asp:DropDownList>

                                <asp:RequiredFieldValidator
                                    ID="RfvAlunoAssociar"
                                    runat="server"
                                    ControlToValidate="DdlAlunoAssociar"
                                    InitialValue=""
                                    ValidationGroup="associacao"
                                    Display="Dynamic"
                                    CssClass="validation-message"
                                    ErrorMessage="Selecione um aluno." />

                            </div>

                        </div>



                        <div class="row mb-3">

                            <label
                                for="<%= TxtParentesco.ClientID %>"
                                class="
                                    col-sm-3
                                    col-form-label
                                    text-end
                                    campo-label">
                                Parentesco

                            </label>

                            <div class="col-sm-5">

                                <asp:TextBox
                                    ID="TxtParentesco"
                                    runat="server"
                                    CssClass="form-control border-secondary"
                                    MaxLength="50"
                                    placeholder="Ex.: Mãe, Pai, Avó, Tutor..." />

                            </div>

                        </div>



                        <div class="row mb-3">

                            <div class="col-sm-3"></div>

                            <div class="col-sm-7">

                                <div class="form-check">

                                    <asp:CheckBox
                                        ID="ChkPrincipal"
                                        runat="server"
                                        CssClass="form-check-input"
                                        ClientIDMode="Static" />

                                    <label
                                        class="form-check-label"
                                        for="ChkPrincipal">
                                        Definir como encarregado principal deste aluno

                                    </label>

                                </div>

                            </div>

                        </div>



                        <div class="row">

                            <div class="col-sm-3"></div>

                            <div class="col-sm-7">

                                <asp:Button
                                    ID="ButtonAssociarAluno"
                                    runat="server"
                                    Text="Associar aluno"
                                    CssClass="btn btn-primary"
                                    ValidationGroup="associacao" />

                            </div>

                        </div>

                    </div>



                    <div class="educandos-lista">

                        <h3>Alunos deste encarregado
                        </h3>

                        <div class="table-responsive">

                            <asp:GridView
                                ID="GridEducandos"
                                runat="server"
                                CssClass="
                                    table
                                    table-striped
                                    table-bordered
                                    align-middle
                                    mb-0"
                                AutoGenerateColumns="false"
                                DataKeyNames="Id"
                                GridLines="None"
                                EmptyDataText="Este encarregado ainda não possui educandos associados.">

                                <Columns>

                                    <asp:BoundField
                                        DataField="NomeCompleto"
                                        HeaderText="Aluno" />

                                    <asp:BoundField
                                        DataField="NumeroProcesso"
                                        HeaderText="N.º processo"
                                        NullDisplayText="—" />

                                    <asp:BoundField
                                        DataField="Parentesco"
                                        HeaderText="Parentesco"
                                        NullDisplayText="—" />

                                    <asp:TemplateField HeaderText="Principal">

                                        <ItemTemplate>

                                            <asp:Label
                                                ID="LblPrincipal"
                                                runat="server"
                                                Text='<%# Convert.ToBoolean(Eval("Principal"))
                                                    ? "Principal"
                                                    : "Não" %>'
                                                CssClass='<%# Convert.ToBoolean(Eval("Principal"))
                                                    ? "estado-principal"
                                                    : "estado-inativo" %>' />

                                        </ItemTemplate>

                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Ações">

                                        <ItemTemplate>

                                            <asp:Button
                                                ID="ButtonRemoverAssociacao"
                                                runat="server"
                                                Text="Remover"
                                                CommandName="RemoverAssociacao"
                                                CommandArgument='<%# Eval("Id") %>'
                                                CssClass="btn btn-sm btn-outline-danger"
                                                CausesValidation="false"
                                                OnClientClick="
                                                    return confirm(
                                                        'Tem a certeza de que pretende remover esta associação?'
                                                    );" />

                                        </ItemTemplate>

                                    </asp:TemplateField>

                                </Columns>

                            </asp:GridView>

                        </div>

                    </div>

                </div>


                <asp:Button
                    ID="ButtonFecharEducandos"
                    runat="server"
                    Text="Fechar"
                    CssClass="btn btn-secondary"
                    CausesValidation="false" />

            </div>

        </asp:Panel>

    </div>

</asp:Content>
