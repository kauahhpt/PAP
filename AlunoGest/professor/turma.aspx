<%@ Page Title="Ver turma"
    Language="C#"
    MasterPageFile="~/professor/ModeloProfessor.Master"
    AutoEventWireup="true"
    CodeBehind="turma.aspx.cs"
    Inherits="AlunoGest.professor.turma" %>

<asp:Content
    ID="Title"
    ContentPlaceHolderID="titleContent"
    runat="server">

    Ver turma

</asp:Content>

<asp:Content
    ID="Main"
    ContentPlaceHolderID="mainContent"
    runat="server">

    <style>
        .turma-page {
            width: 100%;
            max-width: 1450px;
            margin: 0 auto;
            text-align: left;
        }

        .back-link {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            margin-bottom: 20px;
            color: #475569;
            text-decoration: none;
            font-weight: 600;
        }

        .back-link:hover {
            color: #123570;
        }

        .turma-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            flex-wrap: wrap;
            gap: 20px;
            margin-bottom: 25px;
        }

        .turma-header h1 {
            margin: 0 0 5px 0;
            font-size: 30px;
            font-weight: 800;
            color: #1f2937;
        }

        .turma-detalhes {
            color: #64748b;
            font-size: 15px;
        }

        .disciplinas-badge {
            display: inline-block;
            padding: 8px 14px;
            background: #e8f0ff;
            color: #1d4ed8;
            border-radius: 20px;
            font-size: 13px;
            font-weight: 700;
        }

        .stats-grid {
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 220px));
            gap: 16px;
            margin-bottom: 30px;
        }

        .stat-card {
            background: #ffffff;
            border: 1px solid rgba(15, 23, 42, 0.08);
            border-radius: 14px;
            padding: 20px;
            box-shadow: 0 2px 8px rgba(15, 23, 42, 0.06);
        }

        .stat-value {
            font-size: 28px;
            font-weight: 800;
            color: #123570;
        }

        .stat-label {
            color: #64748b;
            font-size: 14px;
            margin-top: 3px;
        }

        .section-card {
            background: #ffffff;
            border: 1px solid rgba(15, 23, 42, 0.08);
            border-radius: 14px;
            box-shadow: 0 2px 8px rgba(15, 23, 42, 0.06);
            margin-bottom: 25px;
            overflow: hidden;
        }

        .section-body {
            padding: 22px;
        }

        .section-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 15px;
            margin-bottom: 20px;
        }

        .section-title {
            margin: 0;
            font-size: 20px;
            font-weight: 700;
            color: #1f2937;
        }

        .section-description {
            margin: 4px 0 0 0;
            color: #64748b;
            font-size: 14px;
        }

        .pessoas-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
            gap: 16px;
        }

        .pessoa-card {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 17px;
            background: #ffffff;
            border: 1px solid #e5e7eb;
            border-radius: 12px;
            transition:
                transform 0.2s,
                box-shadow 0.2s,
                border-color 0.2s;
        }

        .pessoa-card:hover {
            transform: translateY(-2px);
            border-color: #cbd5e1;
            box-shadow: 0 5px 15px rgba(15, 23, 42, 0.08);
        }

        .pessoa-avatar {
            width: 52px;
            height: 52px;
            min-width: 52px;
            min-height: 52px;
            max-width: 52px;
            max-height: 52px;
            flex: 0 0 52px;
            display: flex;
            align-items: center;
            justify-content: center;
            overflow: hidden;
            border-radius: 50%;
            background: linear-gradient(
                135deg,
                #123570,
                #2563eb
            );
            color: #ffffff;
            font-size: 20px;
            font-weight: 800;
        }

        .pessoa-avatar img {
            width: 52px !important;
            height: 52px !important;
            min-width: 52px;
            min-height: 52px;
            max-width: 52px !important;
            max-height: 52px !important;
            object-fit: cover;
            display: block;
            border-radius: 50%;
        }

        .pessoa-info {
            min-width: 0;
        }

        .pessoa-nome {
            color: #1f2937;
            font-size: 16px;
            font-weight: 700;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        .pessoa-detalhe {
            color: #64748b;
            font-size: 13px;
            margin-top: 3px;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        .empty-state {
            padding: 30px;
            text-align: center;
            border: 1px dashed #cbd5e1;
            border-radius: 12px;
            color: #64748b;
            background: #f8fafc;
        }

        .activity-form {
            margin-bottom: 25px;
            padding: 20px;
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            background: #f8fafc;
        }

        .activity-table {
            width: 100%;
            margin-bottom: 0;
        }

        .activity-table th {
            color: #475569;
            font-size: 13px;
            font-weight: 700;
            white-space: nowrap;
        }

        .activity-table td {
            vertical-align: middle;
            color: #334155;
            font-size: 14px;
        }

        .anexo-item {
            display: flex;
            align-items: center;
            gap: 10px;
            margin-top: 6px;
            padding: 8px 10px;
            background: #ffffff;
            border: 1px solid #e2e8f0;
            border-radius: 8px;
        }

        @media (max-width: 900px) {
            .stats-grid {
                grid-template-columns: repeat(3, 1fr);
            }
        }

        @media (max-width: 700px) {
            .stats-grid {
                grid-template-columns: 1fr;
            }

            .pessoas-grid {
                grid-template-columns: 1fr;
            }

            .turma-header {
                flex-direction: column;
            }

            .section-body {
                padding: 16px;
            }
        }
    </style>

    <div class="turma-page">

        <a
            href="dashboard.aspx"
            class="back-link">

            &larr; Voltar às minhas turmas

        </a>

        <div class="turma-header">

            <div>

                <h1>
                    <asp:Label
                        ID="LblTurma"
                        runat="server" />
                </h1>

                <div class="turma-detalhes">

                    <asp:Label
                        ID="LblDetalhes"
                        runat="server" />

                </div>

            </div>

            <div>

                <asp:Label
                    ID="LblDisciplinas"
                    runat="server"
                    CssClass="disciplinas-badge" />

            </div>

        </div>

        <asp:Label
            ID="LblMensagem"
            runat="server"
            Visible="false" />

        <div class="stats-grid">

            <div class="stat-card">

                <div class="stat-value">

                    <asp:Label
                        ID="LblTotalAlunos"
                        runat="server"
                        Text="0" />

                </div>

                <div class="stat-label">
                    Alunos da turma
                </div>

            </div>

            <div class="stat-card">

                <div class="stat-value">

                    <asp:Label
                        ID="LblTotalProfessores"
                        runat="server"
                        Text="0" />

                </div>

                <div class="stat-label">
                    Professores
                </div>

            </div>

            <div class="stat-card">

                <div class="stat-value">

                    <asp:Label
                        ID="LblTotalAtividades"
                        runat="server"
                        Text="0" />

                </div>

                <div class="stat-label">
                    Atividades futuras
                </div>

            </div>

        </div>

        <section class="section-card">

            <div class="section-body">

                <div class="section-header">

                    <div>

                        <h2 class="section-title">
                            Alunos da turma
                        </h2>

                        <p class="section-description">
                            Consulte os alunos atualmente inscritos nesta turma.
                        </p>

                    </div>

                    <asp:TextBox
                        ID="TxtPesquisaAluno"
                        runat="server"
                        CssClass="form-control"
                        Style="max-width: 280px;"
                        placeholder="Pesquisar aluno"
                        AutoPostBack="true"
                        OnTextChanged="TxtPesquisaAluno_TextChanged" />

                </div>

                <div class="pessoas-grid">

                    <asp:Repeater
                        ID="RepeaterAlunos"
                        runat="server">

                        <ItemTemplate>

                            <div class="pessoa-card">

                                <div class="pessoa-avatar">

                                    <asp:Image
                                        ID="ImgAluno"
                                        runat="server"
                                        ImageUrl='<%# ObterFoto(Eval("Foto")) %>'
                                        Visible='<%# TemFoto(Eval("Foto")) %>' />

                                    <asp:Literal
                                        ID="LitInicialAluno"
                                        runat="server"
                                        Text='<%# ObterInicial(Eval("NomeCompleto")) %>'
                                        Visible='<%# !TemFoto(Eval("Foto")) %>' />

                                </div>

                                <div class="pessoa-info">

                                    <div class="pessoa-nome">
                                        <%# Eval("NomeCompleto") %>
                                    </div>

                                    <div class="pessoa-detalhe">
                                        Processo:
                                        <%# Eval("NumeroProcesso") %>
                                    </div>

                                    <div class="pessoa-detalhe">
                                        <%# Eval("Email") %>
                                    </div>

                                </div>

                            </div>

                        </ItemTemplate>

                    </asp:Repeater>

                </div>

                <asp:Panel
                    ID="PainelSemAlunos"
                    runat="server"
                    Visible="false"
                    CssClass="empty-state">

                    Não foram encontrados alunos nesta turma.

                </asp:Panel>

            </div>

        </section>

        <section class="section-card">

            <div class="section-body">

                <div class="section-header">

                    <div>

                        <h2 class="section-title">
                            Professores da turma
                        </h2>

                        <p class="section-description">
                            Professores associados às disciplinas desta turma.
                        </p>

                    </div>

                </div>

                <div class="pessoas-grid">

                    <asp:Repeater
                        ID="RepeaterProfessores"
                        runat="server">

                        <ItemTemplate>

                            <div class="pessoa-card">

                                <div class="pessoa-avatar">

                                    <asp:Literal
                                        ID="LitInicialProfessor"
                                        runat="server"
                                        Text='<%# ObterInicial(Eval("Nome")) %>' />

                                </div>

                                <div class="pessoa-info">

                                    <div class="pessoa-nome">
                                        <%# Eval("Nome") %>
                                    </div>

                                    <div class="pessoa-detalhe">
                                        <%# Eval("Disciplinas") %>
                                    </div>

                                </div>

                            </div>

                        </ItemTemplate>

                    </asp:Repeater>

                </div>

                <asp:Panel
                    ID="PainelSemProfessores"
                    runat="server"
                    Visible="false"
                    CssClass="empty-state">

                    Não existem professores associados à turma.

                </asp:Panel>

            </div>

        </section>

        <section class="section-card">

            <div class="section-body">

                <div class="section-header">

                    <div>

                        <h2 class="section-title">
                            Testes e trabalhos
                        </h2>

                        <p class="section-description">
                            As atividades publicadas ficam visíveis na agenda dos alunos.
                        </p>

                    </div>

                    <asp:Button
                        ID="BtnNova"
                        runat="server"
                        Text="+ Nova atividade"
                        CssClass="btn btn-primary"
                        OnClick="BtnNova_Click"
                        CausesValidation="false" />

                </div>

                <asp:Panel
                    ID="PainelFormulario"
                    runat="server"
                    Visible="false"
                    CssClass="activity-form">

                    <asp:HiddenField
                        ID="HdnEventoId"
                        runat="server" />

                    <div class="row g-3">

                        <div class="col-md-3">

                            <label class="form-label">
                                Tipo
                            </label>

                            <asp:DropDownList
                                ID="DdlTipo"
                                runat="server"
                                CssClass="form-select">

                                <asp:ListItem
                                    Text="Trabalho"
                                    Value="Trabalho" />

                                <asp:ListItem
                                    Text="Teste"
                                    Value="Teste" />

                            </asp:DropDownList>

                        </div>

                        <div class="col-md-5">

                            <label class="form-label">
                                Título
                            </label>

                            <asp:TextBox
                                ID="TxtTitulo"
                                runat="server"
                                CssClass="form-control"
                                MaxLength="200" />

                            <asp:RequiredFieldValidator
                                runat="server"
                                ControlToValidate="TxtTitulo"
                                ErrorMessage="Indique um título."
                                CssClass="text-danger small"
                                ValidationGroup="atividade" />

                        </div>

                        <div class="col-md-4">

                            <label class="form-label">
                                Data e hora
                            </label>

                            <asp:TextBox
                                ID="TxtDataHora"
                                runat="server"
                                CssClass="form-control"
                                TextMode="DateTimeLocal" />

                            <asp:RequiredFieldValidator
                                runat="server"
                                ControlToValidate="TxtDataHora"
                                ErrorMessage="Indique a data e hora."
                                CssClass="text-danger small"
                                ValidationGroup="atividade" />

                        </div>

                        <div class="col-12">

                            <label class="form-label">

                                Anexo

                                <span class="text-muted small">
                                    (opcional, máximo 10 MB)
                                </span>

                            </label>

                            <asp:FileUpload
                                ID="FileAnexo"
                                runat="server"
                                CssClass="form-control" />

                        </div>

                    </div>

                    <asp:Repeater
                        ID="RepeaterAnexos"
                        runat="server"
                        OnItemCommand="RepeaterAnexos_ItemCommand">

                        <HeaderTemplate>

                            <div class="mt-3">

                                <strong>
                                    Anexos atuais
                                </strong>

                        </HeaderTemplate>

                        <ItemTemplate>

                            <div class="anexo-item">

                                <a
                                    href='<%# Eval("CaminhoFicheiro") %>'
                                    target="_blank">

                                    <%# Eval("NomeFicheiro") %>

                                </a>

                                <asp:LinkButton
                                    runat="server"
                                    CommandName="Remover"
                                    CommandArgument='<%# Eval("Id") %>'
                                    Text="Remover"
                                    CssClass="text-danger small"
                                    CausesValidation="false" />

                            </div>

                        </ItemTemplate>

                        <FooterTemplate>
                            </div>
                        </FooterTemplate>

                    </asp:Repeater>

                    <div class="mt-3">

                        <asp:Button
                            ID="BtnGuardar"
                            runat="server"
                            Text="Guardar atividade"
                            CssClass="btn btn-success"
                            OnClick="BtnGuardar_Click"
                            ValidationGroup="atividade" />

                        <asp:Button
                            ID="BtnCancelar"
                            runat="server"
                            Text="Cancelar"
                            CssClass="btn btn-outline-secondary"
                            OnClick="BtnCancelar_Click"
                            CausesValidation="false" />

                    </div>

                </asp:Panel>

                <div class="table-responsive">

                    <asp:GridView
                        ID="GridAtividades"
                        runat="server"
                        AutoGenerateColumns="false"
                        CssClass="table table-hover align-middle activity-table"
                        GridLines="None"
                        DataKeyNames="Id"
                        EmptyDataText="Ainda não existem atividades para esta turma."
                        OnRowCommand="GridAtividades_RowCommand">

                        <Columns>

                            <asp:BoundField
                                DataField="Tipo"
                                HeaderText="Tipo" />

                            <asp:BoundField
                                DataField="Titulo"
                                HeaderText="Título" />

                            <asp:BoundField
                                DataField="DataHora"
                                HeaderText="Data"
                                DataFormatString="{0:dd/MM/yyyy HH:mm}" />

                            <asp:BoundField
                                DataField="TotalAnexos"
                                HeaderText="Anexos" />

                            <asp:TemplateField>

                                <ItemTemplate>

                                    <asp:LinkButton
                                        runat="server"
                                        CommandName="EditarAtividade"
                                        CommandArgument='<%# Container.DataItemIndex %>'
                                        Text="Editar"
                                        CssClass="btn btn-outline-primary btn-sm me-1"
                                        CausesValidation="false" />

                                    <asp:LinkButton
                                        runat="server"
                                        CommandName="ApagarAtividade"
                                        CommandArgument='<%# Container.DataItemIndex %>'
                                        Text="Eliminar"
                                        CssClass="btn btn-outline-danger btn-sm"
                                        OnClientClick="return confirm('Eliminar esta atividade e os respetivos anexos?');"
                                        CausesValidation="false" />

                                </ItemTemplate>

                            </asp:TemplateField>

                        </Columns>

                    </asp:GridView>

                </div>

            </div>

        </section>

    </div>

</asp:Content>