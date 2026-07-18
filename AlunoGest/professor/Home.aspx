<%@ Page Title="Feed e Calendário"
    Language="C#"
    MasterPageFile="~/professor/ModeloProfessor.Master"
    AutoEventWireup="true"
    CodeBehind="Home.aspx.cs"
    Inherits="AlunoGest.professor.calendario" %>

<asp:Content
    ID="Title"
    ContentPlaceHolderID="titleContent"
    runat="server">
    Feed e Calendário

</asp:Content>

<asp:Content
    ID="Head"
    ContentPlaceHolderID="headContent"
    runat="server">

    <link
        href="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.8/index.global.min.css"
        rel="stylesheet" />

    <script src="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.8/index.global.min.js"></script>

    <style>
        /* =====================================================
           ESTRUTURA GERAL
        ===================================================== */

        .turma-page {
            width: 100%;
            max-width: 1500px;
            margin: 0 auto;
            text-align: left;
        }

        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            gap: 20px;
            margin-bottom: 24px;
        }

            .page-header h1 {
                margin: 0 0 5px;
                color: #1f2937;
                font-size: 30px;
                font-weight: 800;
            }

        .page-subtitle {
            color: #64748b;
            font-size: 15px;
        }

        .page-card {
            overflow: hidden;
            border: 1px solid rgba(15, 23, 42, 0.08);
            border-radius: 14px;
            background: #ffffff;
            box-shadow: 0 2px 8px rgba(15, 23, 42, 0.06);
        }

        .page-card-body {
            padding: 20px;
        }

        .turma-selector {
            margin-bottom: 24px;
        }

        .section-title {
            margin-bottom: 16px;
        }

            .section-title h2 {
                margin: 0;
                color: #1f2937;
                font-size: 20px;
                font-weight: 750;
            }

            .section-title p {
                margin: 4px 0 0;
                color: #64748b;
                font-size: 13px;
            }

        .form-label {
            color: #334155;
            font-weight: 600;
        }


        /* =====================================================
           LAYOUT PRINCIPAL
        ===================================================== */

        .professor-layout {
            display: grid;
            grid-template-columns: minmax(0, 1.45fr) minmax(340px, 0.55fr);
            gap: 24px;
            align-items: start;
            margin-bottom: 24px;
        }

        .main-column,
        .feed-column {
            min-width: 0;
        }


        /* =====================================================
           CAIXA PARA ABRIR O MODAL DE PUBLICAÇÃO
        ===================================================== */

        .share-composer {
            margin-bottom: 20px;
        }

        .share-composer-body {
            padding: 18px;
        }

        .share-composer-label {
            margin-bottom: 10px;
            color: #1e293b;
            font-size: 13px;
            font-weight: 800;
        }

        .share-trigger {
            display: flex;
            align-items: center;
            gap: 10px;
            width: 100%;
            padding: 0;
            border: 0;
            background: transparent;
            text-align: left;
        }

        .share-plus {
            width: 42px;
            height: 42px;
            min-width: 42px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            border-radius: 10px;
            background: #1d4ed8;
            color: #ffffff;
            font-size: 24px;
            font-weight: 400;
            line-height: 1;
            box-shadow: 0 5px 12px rgba(29, 78, 216, 0.22);
        }

        .share-placeholder {
            flex: 1;
            min-height: 42px;
            padding: 10px 14px;
            display: flex;
            align-items: center;
            border: 1px solid #dbe4ef;
            border-radius: 11px;
            background: #f8fafc;
            color: #64748b;
            font-size: 14px;
            transition: border-color 0.15s, background 0.15s, box-shadow 0.15s;
        }

        .share-trigger:hover .share-placeholder,
        .share-trigger:focus .share-placeholder {
            border-color: #93b4ef;
            background: #ffffff;
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.09);
        }

        .share-trigger:focus {
            outline: none;
        }


        /* =====================================================
           MODAL DE PUBLICAÇÃO
        ===================================================== */

        .publication-modal .modal-content {
            overflow: hidden;
            border: 0;
            border-radius: 16px;
            box-shadow: 0 24px 60px rgba(15, 23, 42, 0.28);
        }

        .publication-modal .modal-header {
            padding: 18px 22px;
            border-bottom: 1px solid #e5eaf1;
        }

        .publication-modal .modal-title {
            color: #1e293b;
            font-size: 20px;
            font-weight: 800;
        }

        .publication-modal .modal-body {
            padding: 22px;
        }

        .publication-modal .modal-footer {
            padding: 15px 22px;
            border-top: 1px solid #e5eaf1;
        }

        .publication-form-grid {
            display: grid;
            grid-template-columns: 1fr;
            gap: 16px;
        }

        .publication-modal textarea {
            resize: vertical;
            min-height: 120px;
        }


        /* =====================================================
           VISIBILIDADE E DESTINATÁRIOS
        ===================================================== */

        .audience-card {
            padding: 16px;
            border: 1px solid #dce5ef;
            border-radius: 14px;
            background: #f8fafc;
        }

        .audience-main-option {
            display: flex;
            justify-content: center;
            margin-bottom: 6px;
        }

            .audience-main-option span {
                display: inline-flex;
                align-items: center;
                gap: 7px;
            }

            .audience-main-option label {
                color: #1e293b;
                font-size: 14px;
                font-weight: 750;
                cursor: pointer;
            }

        .audience-help {
            margin: 7px 0 15px;
            color: #64748b;
            font-size: 12px;
            line-height: 1.5;
            text-align: center;
        }

        .audience-selector {
            padding-top: 15px;
            border-top: 1px solid #dce5ef;
        }

            .audience-selector.is-hidden {
                display: none;
            }

        .audience-selector-header {
            margin-bottom: 14px;
            text-align: center;
        }

            .audience-selector-header h6 {
                margin: 0 0 5px;
                color: #1e293b;
                font-size: 14px;
                font-weight: 800;
            }

            .audience-selector-header p {
                margin: 0;
                color: #64748b;
                font-size: 12px;
            }

        .recipient-group {
            height: 100%;
            overflow: hidden;
            border: 1px solid #dce4ee;
            border-radius: 12px;
            background: #ffffff;
        }

        .recipient-group-title {
            padding: 10px 13px;
            border-bottom: 1px solid #e8edf3;
            background: #f1f5f9;
            color: #334155;
            font-size: 13px;
            font-weight: 800;
            text-align: center;
        }

        .recipient-list-container {
            max-height: 210px;
            padding: 10px 12px;
            overflow-y: auto;
        }

        .recipient-list {
            width: 100%;
        }

            .recipient-list span {
                display: flex;
                align-items: flex-start;
                gap: 7px;
                margin-bottom: 7px;
                padding: 7px 8px;
                border-radius: 8px;
                transition: background 0.15s;
            }

                .recipient-list span:hover {
                    background: #eef4ff;
                }

            .recipient-list input[type="checkbox"] {
                margin-top: 3px;
            }

            .recipient-list label {
                flex: 1;
                margin: 0;
                color: #334155;
                font-size: 13px;
                line-height: 1.4;
                cursor: pointer;
            }



        /* =====================================================
           SELECIONAR TODOS OS DESTINATÁRIOS
        ===================================================== */

        .recipient-group-header {
            display: flex;
            flex-direction: column;
            align-items: stretch;
            gap: 8px;
            text-align: left;
        }

        .recipient-group-header > span {
            font-size: 13px;
            font-weight: 800;
        }

        .select-all-option {
            display: flex;
            align-items: center;
            gap: 7px;
            margin: 0;
            padding: 7px 9px;
            border: 1px solid #cbd8ea;
            border-radius: 8px;
            background: #ffffff;
            color: #1d4ed8;
            font-size: 11px;
            font-weight: 700;
            cursor: pointer;
        }

        .select-all-option:hover {
            border-color: #2563eb;
            background: #eef4ff;
        }

        .select-all-option input[type="checkbox"] {
            margin: 0;
            cursor: pointer;
        }

        /* =====================================================
           FEED
        ===================================================== */

        .feed-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            gap: 10px;
            margin-bottom: 14px;
        }

            .feed-header h2 {
                margin: 0;
                color: #1f2937;
                font-size: 20px;
                font-weight: 800;
            }

            .feed-header p {
                margin: 3px 0 0;
                color: #64748b;
                font-size: 13px;
            }

        .feed-badge {
            padding: 6px 10px;
            border-radius: 999px;
            background: #ffffff;
            color: #64748b;
            font-size: 11px;
            font-weight: 700;
        }

        .post-card {
            margin-bottom: 16px;
        }

            .post-card:last-child {
                margin-bottom: 0;
            }

        .post-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            gap: 15px;
            margin-bottom: 15px;
        }

        .post-user {
            display: flex;
            align-items: center;
            gap: 12px;
            min-width: 0;
        }

        .post-avatar {
            width: 44px;
            height: 44px;
            min-width: 44px;
            min-height: 44px;
            max-width: 44px;
            max-height: 44px;
            flex: 0 0 44px;
            overflow: hidden;
            border-radius: 50%;
            background: linear-gradient( 135deg, #123570, #2563eb );
            color: #ffffff;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 18px;
            font-weight: 700;
        }

            .post-avatar img {
                width: 44px !important;
                height: 44px !important;
                max-width: 44px !important;
                max-height: 44px !important;
                display: block;
                object-fit: cover;
                border-radius: 50%;
            }

        .post-author {
            color: #1f2937;
            font-size: 15px;
            font-weight: 700;
        }

        .post-author-type {
            margin-left: 5px;
            color: #64748b;
            font-size: 11px;
            font-weight: 600;
        }

        .post-date {
            margin-top: 2px;
            color: #94a3b8;
            font-size: 13px;
        }

        .post-type {
            padding: 5px 10px;
            border-radius: 20px;
            background: #e8f0ff;
            color: #1d4ed8;
            font-size: 12px;
            font-weight: 700;
            white-space: nowrap;
        }

        .post-title {
            margin-bottom: 8px;
            color: #1f2937;
            font-size: 18px;
            font-weight: 750;
        }

        .post-content {
            color: #475569;
            font-size: 14px;
            line-height: 1.6;
            white-space: pre-line;
        }

        .post-footer {
            display: flex;
            align-items: center;
            gap: 10px;
            margin-top: 16px;
            padding-top: 12px;
            border-top: 1px solid #edf0f5;
            color: #64748b;
            font-size: 13px;
        }

        .empty-feed {
            padding: 35px;
            border: 1px dashed #cbd5e1;
            border-radius: 12px;
            background: #ffffff;
            color: #64748b;
            text-align: center;
        }


        /* =====================================================
           CALENDÁRIO
        ===================================================== */

        .calendar-card {
            margin-bottom: 20px;
        }

        .calendar-container {
            padding: 16px;
        }

        #calendar {
            width: 100%;
            background: #ffffff;
        }

        .fc .fc-toolbar {
            gap: 8px;
        }

        .fc .fc-toolbar-title {
            font-size: 1.05rem;
            font-weight: 750;
        }

        .fc .fc-button {
            padding: 0.35rem 0.55rem;
            font-size: 0.78rem;
        }

        .fc .fc-col-header-cell-cushion,
        .fc .fc-daygrid-day-number {
            font-size: 12px;
            text-decoration: none;
        }

        .fc .fc-daygrid-day-number {
            color: #334155;
        }

        .fc .fc-event {
            font-size: 11px;
        }


        /* =====================================================
           EVENTOS
        ===================================================== */

        .event-form-card {
            margin-bottom: 20px;
        }

        .event-form-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 14px;
        }

        .event-buttons {
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
            margin-top: 16px;
        }


        /* =====================================================
           TABELAS E GESTÃO
        ===================================================== */

        .management-grid {
            display: grid;
            grid-template-columns: minmax(0, 1fr) minmax(0, 1fr);
            gap: 24px;
            margin-bottom: 24px;
        }

        .table-wrapper {
            width: 100%;
            overflow-x: auto;
        }

        .table {
            width: 100%;
            margin-bottom: 0;
        }

            .table th {
                color: #475569;
                font-size: 12px;
                font-weight: 700;
                white-space: nowrap;
            }

            .table td {
                color: #334155;
                font-size: 13px;
                vertical-align: middle;
            }

        .avaliacao-card {
            margin-bottom: 24px;
        }


        /* =====================================================
           RESPONSIVIDADE
        ===================================================== */

        @media (max-width: 1000px) {

            .professor-layout,
            .management-grid {
                grid-template-columns: 1fr;
            }

            .feed-column {
                order: -1;
            }
        }

        @media (max-width: 700px) {

            .page-header {
                flex-direction: column;
            }

            .page-card-body {
                padding: 15px;
            }

            .event-form-grid {
                grid-template-columns: 1fr;
            }

            .post-header {
                flex-direction: column;
            }

            .fc .fc-toolbar {
                flex-direction: column;
                align-items: stretch;
            }

            .event-buttons {
                flex-direction: column;
            }

                .event-buttons .btn {
                    width: 100%;
                }

            .share-plus {
                width: 38px;
                height: 38px;
                min-width: 38px;
            }
        }
    </style>

</asp:Content>


<asp:Content
    ID="Main"
    ContentPlaceHolderID="mainContent"
    runat="server">

    <div class="turma-page">


        <!-- ==================================================
             CABEÇALHO
        =================================================== -->

        <div class="page-header">

            <div>

                <h1>Feed e Calendário da Turma
                </h1>

                <div class="page-subtitle">

                    <asp:Label
                        ID="LblResumoTurma"
                        runat="server" />

                </div>

            </div>

        </div>


        <asp:Label
            ID="LblMensagem"
            runat="server"
            Visible="false" />


        <!-- ==================================================
             SELEÇÃO DA TURMA
        =================================================== -->

        <div class="page-card turma-selector">

            <div class="page-card-body">

                <div class="row g-3 align-items-end">

                    <div class="col-md-5">

                        <label
                            class="form-label"
                            for="<%= DdlTurmas.ClientID %>">
                            Turma

                        </label>

                        <asp:DropDownList
                            ID="DdlTurmas"
                            runat="server"
                            CssClass="form-select"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="DdlTurmas_SelectedIndexChanged" />

                    </div>


                    <div class="col-md-7">

                        <div class="alert alert-info mb-0">
                            Estão a ser apresentadas apenas as informações
                            da turma selecionada.

                        </div>

                    </div>

                </div>

            </div>

        </div>


        <!-- ==================================================
             CONTEÚDO PRINCIPAL
        =================================================== -->

        <div class="professor-layout">


            <!-- ==================================================
                 COLUNA PRINCIPAL: CALENDÁRIO
            =================================================== -->

            <section class="main-column">


                <!-- CALENDÁRIO -->

                <div class="page-card calendar-card">

                    <div class="page-card-body">

                        <div class="section-title">

                            <h2>Calendário da turma
                            </h2>

                            <p>
                                Testes, trabalhos e avisos da turma selecionada.
                            </p>

                        </div>

                    </div>

                    <div class="calendar-container">

                        <div id="calendar"></div>

                    </div>

                </div>


                <!-- NOVO EVENTO -->

                <div class="page-card event-form-card">

                    <div class="page-card-body">

                        <div class="section-title">

                            <h2>Novo ou editar evento
                            </h2>

                            <p>
                                Crie testes, trabalhos e avisos.
                            </p>

                        </div>


                        <asp:HiddenField
                            ID="HdnEventoId"
                            runat="server" />


                        <div class="event-form-grid">


                            <div>

                                <label
                                    class="form-label"
                                    for="<%= DdlTipo.ClientID %>">
                                    Tipo

                                </label>

                                <asp:DropDownList
                                    ID="DdlTipo"
                                    runat="server"
                                    CssClass="form-select">

                                    <asp:ListItem
                                        Value="Trabalho"
                                        Text="Trabalho" />

                                    <asp:ListItem
                                        Value="Teste"
                                        Text="Teste" />

                                    <asp:ListItem
                                        Value="Aviso"
                                        Text="Aviso" />

                                </asp:DropDownList>

                            </div>


                            <div>

                                <label
                                    class="form-label"
                                    for="<%= TxtTitulo.ClientID %>">
                                    Título

                                </label>

                                <asp:TextBox
                                    ID="TxtTitulo"
                                    runat="server"
                                    CssClass="form-control"
                                    MaxLength="200" />

                            </div>


                            <div>

                                <label
                                    class="form-label"
                                    for="<%= TxtDataHora.ClientID %>">
                                    Data e hora

                                </label>

                                <asp:TextBox
                                    ID="TxtDataHora"
                                    runat="server"
                                    CssClass="form-control"
                                    TextMode="DateTimeLocal" />

                            </div>


                            <div>

                                <label
                                    class="form-label"
                                    for="<%= FileAnexo.ClientID %>">
                                    Anexo do professor

                                </label>

                                <asp:FileUpload
                                    ID="FileAnexo"
                                    runat="server"
                                    CssClass="form-control" />

                            </div>

                        </div>


                        <div class="event-buttons">

                            <asp:Button
                                ID="BtnGuardar"
                                runat="server"
                                Text="Guardar evento"
                                CssClass="btn btn-primary"
                                OnClick="BtnGuardar_Click" />

                            <asp:Button
                                ID="BtnLimpar"
                                runat="server"
                                Text="Limpar"
                                CssClass="btn btn-outline-secondary"
                                OnClick="BtnLimpar_Click"
                                CausesValidation="false" />

                        </div>

                    </div>

                </div>

            </section>


            <!-- ==================================================
                 COLUNA SECUNDÁRIA: PUBLICAÇÃO + FEED
            =================================================== -->

            <aside class="feed-column">


                <!-- PEQUENA CAIXA PARA CRIAR PUBLICAÇÃO -->

                <div class="page-card share-composer">

                    <div class="share-composer-body">

                        <div class="share-composer-label">
                            Partilhar com a turma
                        </div>

                        <button
                            type="button"
                            class="share-trigger"
                            data-bs-toggle="modal"
                            data-bs-target="#modalCriarPublicacaoProfessor">

                            <span class="share-plus">+
                            </span>

                            <span class="share-placeholder">Em que está a pensar?
                            </span>

                        </button>

                    </div>

                </div>


                <!-- FEED -->

                <div class="feed-header">

                    <div>

                        <h2>Feed da turma
                        </h2>

                        <p>
                            Publicações partilhadas com a turma.
                        </p>

                    </div>

                    <span class="feed-badge">Atualizações
                    </span>

                </div>


                <asp:Repeater
                    ID="RepeaterPublicacoes"
                    runat="server">

                    <ItemTemplate>

                        <article class="page-card post-card">

                            <div class="page-card-body">


                                <div class="post-header">


                                    <div class="post-user">


                                        <div class="post-avatar">

                                            <asp:Image
                                                ID="ImgAutorPublicacao"
                                                runat="server"
                                                ImageUrl='<%# ObterFotoPublicacao(Eval("Foto")) %>'
                                                Visible='<%# TemFotoPublicacao(Eval("Foto")) %>' />

                                            <asp:Literal
                                                ID="LitInicialAutor"
                                                runat="server"
                                                Text='<%# ObterInicial(Eval("NomeCompleto")) %>'
                                                Visible='<%# !TemFotoPublicacao(Eval("Foto")) %>' />

                                        </div>


                                        <div>

                                            <div class="post-author">

                                                <%# Eval("NomeCompleto") %>

                                                <span class="post-author-type">

                                                    <%# Eval("AutorTipo") %>

                                                </span>

                                            </div>

                                            <div class="post-date">

                                                <%#
                                                    Eval(
                                                        "CreatedAt",
                                                        "{0:dd/MM/yyyy HH:mm}"
                                                    )
                                                %>
                                            </div>

                                        </div>


                                    </div>


                                    <span class="post-type">

                                        <%# Eval("Tipo") %>

                                    </span>


                                </div>


                                <div class="post-title">

                                    <%# Eval("Titulo") %>
                                </div>


                                <div class="post-content">

                                    <%# Eval("Conteudo") %>
                                </div>


                                <div class="post-footer">

                                    <span>Gostos: <%# Eval("TotalLikes") %>
                                    </span>

                                </div>


                            </div>

                        </article>

                    </ItemTemplate>

                </asp:Repeater>


                <asp:Panel
                    ID="PainelSemPublicacoes"
                    runat="server"
                    Visible="false"
                    CssClass="empty-feed">
                    Ainda não existem publicações visíveis para si nesta turma.

                </asp:Panel>


            </aside>

        </div>


        <!-- ==================================================
             EVENTOS E ENTREGAS
        =================================================== -->

        <div class="management-grid">


            <!-- EVENTOS -->

            <div class="page-card">

                <div class="page-card-body">

                    <div class="section-title">

                        <h2>Eventos da turma
                        </h2>

                        <p>
                            Consulte, edite ou elimine os eventos publicados.
                        </p>

                    </div>


                    <div class="table-wrapper">

                        <asp:GridView
                            ID="GridEventos"
                            runat="server"
                            AutoGenerateColumns="false"
                            CssClass="table table-hover align-middle"
                            GridLines="None"
                            DataKeyNames="Id"
                            EmptyDataText="Ainda não existem eventos."
                            OnRowCommand="GridEventos_RowCommand">

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

                                <asp:TemplateField>

                                    <ItemTemplate>

                                        <asp:LinkButton
                                            runat="server"
                                            Text="Editar"
                                            CommandName="EditarEvento"
                                            CommandArgument='<%# Container.DataItemIndex %>'
                                            CssClass="btn btn-outline-primary btn-sm me-1" />

                                        <asp:LinkButton
                                            runat="server"
                                            Text="Apagar"
                                            CommandName="ApagarEvento"
                                            CommandArgument='<%# Container.DataItemIndex %>'
                                            CssClass="btn btn-outline-danger btn-sm"
                                            OnClientClick="return confirm('Apagar este evento?');" />

                                    </ItemTemplate>

                                </asp:TemplateField>

                            </Columns>

                        </asp:GridView>

                    </div>

                </div>

            </div>


            <!-- ENTREGAS -->

            <div class="page-card">

                <div class="page-card-body">

                    <div class="section-title">

                        <h2>Entregas dos alunos
                        </h2>

                        <p>
                            Consulte e avalie os trabalhos entregues.
                        </p>

                    </div>


                    <div class="table-wrapper">

                        <asp:GridView
                            ID="GridEntregas"
                            runat="server"
                            AutoGenerateColumns="false"
                            CssClass="table table-hover align-middle"
                            GridLines="None"
                            DataKeyNames="Id"
                            EmptyDataText="Ainda não existem entregas."
                            OnRowCommand="GridEntregas_RowCommand">

                            <Columns>

                                <asp:BoundField
                                    DataField="Aluno"
                                    HeaderText="Aluno" />

                                <asp:BoundField
                                    DataField="Evento"
                                    HeaderText="Evento" />

                                <asp:TemplateField
                                    HeaderText="Ficheiro">

                                    <ItemTemplate>

                                        <a
                                            href='<%# Eval("CaminhoFicheiro") %>'
                                            target="_blank">

                                            <%# Eval("NomeFicheiro") %>

                                        </a>

                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:BoundField
                                    DataField="Nota"
                                    HeaderText="Nota" />

                                <asp:TemplateField>

                                    <ItemTemplate>

                                        <asp:LinkButton
                                            runat="server"
                                            Text="Avaliar"
                                            CommandName="AvaliarEntrega"
                                            CommandArgument='<%# Container.DataItemIndex %>'
                                            CssClass="btn btn-success btn-sm" />

                                    </ItemTemplate>

                                </asp:TemplateField>

                            </Columns>

                        </asp:GridView>

                    </div>

                </div>

            </div>

        </div>


        <!-- ==================================================
             AVALIAÇÃO
        =================================================== -->

        <asp:Panel
            ID="PainelAvaliacao"
            runat="server"
            Visible="false"
            CssClass="page-card avaliacao-card">

            <div class="page-card-body">

                <div class="section-title">

                    <h2>Avaliar entrega
                    </h2>

                    <p>
                        Atribua uma nota e deixe feedback para o aluno.
                    </p>

                </div>


                <asp:HiddenField
                    ID="HdnEntregaId"
                    runat="server" />


                <div class="row g-3">


                    <div class="col-md-2">

                        <label
                            class="form-label"
                            for="<%= TxtNota.ClientID %>">
                            Nota

                        </label>

                        <asp:TextBox
                            ID="TxtNota"
                            runat="server"
                            CssClass="form-control"
                            placeholder="0-20" />

                    </div>


                    <div class="col-md-10">

                        <label
                            class="form-label"
                            for="<%= TxtFeedback.ClientID %>">
                            Feedback

                        </label>

                        <asp:TextBox
                            ID="TxtFeedback"
                            runat="server"
                            CssClass="form-control"
                            TextMode="MultiLine"
                            Rows="3" />

                    </div>


                </div>


                <div class="mt-3">

                    <asp:Button
                        ID="BtnGuardarAvaliacao"
                        runat="server"
                        Text="Guardar avaliação"
                        CssClass="btn btn-success"
                        OnClick="BtnGuardarAvaliacao_Click" />

                    <asp:Button
                        ID="BtnCancelarAvaliacao"
                        runat="server"
                        Text="Cancelar"
                        CssClass="btn btn-outline-secondary"
                        OnClick="BtnCancelarAvaliacao_Click"
                        CausesValidation="false" />

                </div>

            </div>

        </asp:Panel>


        <asp:HiddenField
            ID="HdnEvents"
            runat="server" />


        <!-- ==================================================
             MODAL DE CRIAR PUBLICAÇÃO
        =================================================== -->

        <div
            class="modal fade publication-modal"
            id="modalCriarPublicacaoProfessor"
            tabindex="-1"
            aria-labelledby="modalCriarPublicacaoProfessorTitulo"
            aria-hidden="true">

            <div
                class="modal-dialog modal-dialog-centered modal-lg modal-dialog-scrollable">

                <div class="modal-content">


                    <div class="modal-header">

                        <h2
                            class="modal-title"
                            id="modalCriarPublicacaoProfessorTitulo">Criar publicação

                        </h2>

                        <button
                            type="button"
                            class="btn-close"
                            data-bs-dismiss="modal"
                            aria-label="Fechar">
                        </button>

                    </div>


                    <div class="modal-body">

                        <div class="publication-form-grid">


                            <!-- TIPO -->

                            <div>

                                <label
                                    class="form-label"
                                    for="<%= DdlTipoPublicacaoProfessor.ClientID %>">
                                    Tipo

                                </label>

                                <asp:DropDownList
                                    ID="DdlTipoPublicacaoProfessor"
                                    runat="server"
                                    CssClass="form-select">

                                    <asp:ListItem
                                        Value="Publicacao"
                                        Text="Publicação" />

                                    <asp:ListItem
                                        Value="Aviso"
                                        Text="Aviso" />

                                    <asp:ListItem
                                        Value="Material"
                                        Text="Material" />

                                    <asp:ListItem
                                        Value="Informacao"
                                        Text="Informação" />

                                </asp:DropDownList>

                            </div>


                            <!-- TÍTULO -->

                            <div>

                                <label
                                    class="form-label"
                                    for="<%= TxtTituloPublicacaoProfessor.ClientID %>">
                                    Título

                                </label>

                                <asp:TextBox
                                    ID="TxtTituloPublicacaoProfessor"
                                    runat="server"
                                    CssClass="form-control"
                                    MaxLength="200" />

                            </div>


                            <!-- CONTEÚDO -->

                            <div>

                                <label
                                    class="form-label"
                                    for="<%= TxtConteudoPublicacaoProfessor.ClientID %>">
                                    Conteúdo

                                </label>

                                <asp:TextBox
                                    ID="TxtConteudoPublicacaoProfessor"
                                    runat="server"
                                    CssClass="form-control"
                                    TextMode="MultiLine"
                                    Rows="5"
                                    MaxLength="5000" />

                            </div>


                            <!-- ANEXO -->

                            <div>

                                <label
                                    class="form-label"
                                    for="<%= FilePublicacaoProfessor.ClientID %>">
                                    Anexo

                                </label>

                                <asp:FileUpload
                                    ID="FilePublicacaoProfessor"
                                    runat="server"
                                    CssClass="form-control" />

                                <div class="form-text">
                                    Tamanho máximo permitido: 10 MB.
                                </div>

                            </div>


                            <!-- VISIBILIDADE -->

                            <div class="audience-card">


                                <div class="audience-main-option">

                                    <asp:CheckBox
                                        ID="ChkPublicaTurmaProfessor"
                                        runat="server"
                                        Text=" Visível para toda a turma" />

                                </div>


                                <p class="audience-help">
                                    Ao marcar esta opção, todos os alunos
                                    e professores da turma poderão visualizar
                                    a publicação.

                                </p>


                                <div
                                    id="painelDestinatariosProfessor"
                                    class="audience-selector">


                                    <div class="audience-selector-header">

                                        <h6>Escolher destinatários específicos
                                        </h6>

                                        <p>
                                            Quando a publicação não for para toda
                                            a turma, selecione quem poderá visualizá-la.
                                        </p>

                                    </div>


                                    <div class="row g-3">


                                        <!-- ALUNOS -->

                                        <div class="col-md-6">

                                            <div class="recipient-group">

                                                <div class="recipient-group-title recipient-group-header">

                                                    <span>Alunos
                                                    </span>

                                                    <label class="select-all-option">

                                                        <input
                                                            type="checkbox"
                                                            id="chkTodosAlunosProfessor"
                                                            data-target-list="<%= CblAlunosDestinatariosProfessor.ClientID %>" />

                                                        <span>Selecionar todos os alunos
                                                        </span>

                                                    </label>

                                                </div>

                                                <div class="recipient-list-container">

                                                    <asp:CheckBoxList
                                                        ID="CblAlunosDestinatariosProfessor"
                                                        runat="server"
                                                        CssClass="recipient-list"
                                                        RepeatLayout="Flow"
                                                        RepeatDirection="Vertical" />

                                                    <asp:Label
                                                        ID="LblSemAlunosDestinatariosProfessor"
                                                        runat="server"
                                                        Text="Não existem alunos associados à turma."
                                                        CssClass="text-muted small"
                                                        Visible="false" />

                                                </div>

                                            </div>

                                        </div>


                                        <!-- PROFESSORES -->

                                        <div class="col-md-6">

                                            <div class="recipient-group">

                                                <div class="recipient-group-title recipient-group-header">

                                                    <span>Professores
                                                    </span>

                                                    <label class="select-all-option">

                                                        <input
                                                            type="checkbox"
                                                            id="chkTodosProfessoresProfessor"
                                                            data-target-list="<%= CblProfessoresDestinatariosProfessor.ClientID %>" />

                                                        <span>Selecionar todos os professores
                                                        </span>

                                                    </label>

                                                </div>

                                                <div class="recipient-list-container">

                                                    <asp:CheckBoxList
                                                        ID="CblProfessoresDestinatariosProfessor"
                                                        runat="server"
                                                        CssClass="recipient-list"
                                                        RepeatLayout="Flow"
                                                        RepeatDirection="Vertical" />

                                                    <asp:Label
                                                        ID="LblSemProfessoresDestinatariosProfessor"
                                                        runat="server"
                                                        Text="Não existem outros professores associados à turma."
                                                        CssClass="text-muted small"
                                                        Visible="false" />

                                                </div>

                                            </div>

                                        </div>


                                    </div>

                                </div>

                            </div>

                        </div>

                    </div>


                    <div class="modal-footer">

                        <button
                            type="button"
                            class="btn btn-outline-secondary"
                            data-bs-dismiss="modal">
                            Cancelar

                        </button>

                        <asp:Button
                            ID="BtnPublicarProfessor"
                            runat="server"
                            Text="Publicar"
                            CssClass="btn btn-primary"
                            OnClick="BtnPublicarProfessor_Click" />

                    </div>


                </div>

            </div>

        </div>
    </div>


    <!-- ==================================================
         JAVASCRIPT
    =================================================== -->

    <script>

        document.addEventListener(
            'DOMContentLoaded',
            function () {

                /* =============================================
                   CALENDÁRIO
                ============================================== */

                var calendarElement =
                    document.getElementById(
                        'calendar'
                    );

                if (calendarElement) {

                    var eventsField =
                        document.getElementById(
                            '<%= HdnEvents.ClientID %>'
                        );

                    var eventos =
                        [];

                    try {

                        eventos =
                            JSON.parse(
                                eventsField.value || '[]'
                            );

                    } catch (erro) {

                        console.error(
                            'Não foi possível carregar os eventos:',
                            erro
                        );

                    }

                    var calendar =
                        new FullCalendar.Calendar(
                            calendarElement,
                            {
                                initialView:
                                    'dayGridMonth',

                                locale:
                                    'pt',

                                height:
                                    650,

                                headerToolbar:
                                {
                                    left:
                                        'prev,next',

                                    center:
                                        'title',

                                    right:
                                        'today'
                                },

                                buttonText:
                                {
                                    today:
                                        'Hoje'
                                },

                                events:
                                    eventos
                            }
                        );

                    calendar.render();
                }


                /* =============================================
                   VISIBILIDADE DA PUBLICAÇÃO
                ============================================== */

                var checkboxTurma =
                    document.getElementById(
                        '<%= ChkPublicaTurmaProfessor.ClientID %>'
                    );

                var painelDestinatarios =
                    document.getElementById(
                        'painelDestinatariosProfessor'
                    );

                if (
                    checkboxTurma &&
                    painelDestinatarios
                ) {

                    function atualizarDestinatarios() {

                        if (checkboxTurma.checked) {

                            painelDestinatarios
                                .classList
                                .add(
                                    'is-hidden'
                                );

                        } else {

                            painelDestinatarios
                                .classList
                                .remove(
                                    'is-hidden'
                                );

                        }

                    }

                    checkboxTurma.addEventListener(
                        'change',
                        atualizarDestinatarios
                    );

                    atualizarDestinatarios();
                }


                /* =============================================
                   SELECIONAR TODOS OS ALUNOS OU PROFESSORES
                ============================================== */

                var seletoresTodos =
                    document.querySelectorAll(
                        '[data-target-list]'
                    );

                seletoresTodos.forEach(
                    function (seletorTodos) {

                        var listaId =
                            seletorTodos.getAttribute(
                                'data-target-list'
                            );

                        var lista =
                            document.getElementById(
                                listaId
                            );

                        if (!lista) {
                            return;
                        }

                        function obterCheckboxes() {
                            return Array.from(
                                lista.querySelectorAll(
                                    'input[type="checkbox"]'
                                )
                            );
                        }

                        function atualizarEstadoPrincipal() {
                            var checkboxes =
                                obterCheckboxes();

                            if (checkboxes.length === 0) {
                                seletorTodos.checked = false;
                                seletorTodos.indeterminate = false;
                                seletorTodos.disabled = true;
                                return;
                            }

                            seletorTodos.disabled = false;

                            var quantidadeSelecionada =
                                checkboxes.filter(
                                    function (checkbox) {
                                        return checkbox.checked;
                                    }
                                ).length;

                            seletorTodos.checked =
                                quantidadeSelecionada ===
                                checkboxes.length;

                            seletorTodos.indeterminate =
                                quantidadeSelecionada > 0 &&
                                quantidadeSelecionada <
                                checkboxes.length;
                        }

                        seletorTodos.addEventListener(
                            'change',
                            function () {

                                var selecionarTodos =
                                    seletorTodos.checked;

                                obterCheckboxes().forEach(
                                    function (checkbox) {
                                        checkbox.checked =
                                            selecionarTodos;
                                    }
                                );

                                seletorTodos.indeterminate =
                                    false;
                            }
                        );

                        lista.addEventListener(
                            'change',
                            atualizarEstadoPrincipal
                        );

                        atualizarEstadoPrincipal();
                    }
                );

            }
        );

    </script>

</asp:Content>
