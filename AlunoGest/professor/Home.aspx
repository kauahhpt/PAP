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
                margin: 0 0 5px 0;
                font-size: 30px;
                font-weight: 800;
                color: #1f2937;
            }

        .page-subtitle {
            color: #64748b;
            font-size: 15px;
        }

        .page-card {
            background: #ffffff;
            border: 1px solid rgba(15, 23, 42, 0.08);
            border-radius: 14px;
            box-shadow: 0 2px 8px rgba(15, 23, 42, 0.06);
            overflow: hidden;
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
                font-size: 20px;
                font-weight: 750;
                color: #1f2937;
            }

            .section-title p {
                margin: 4px 0 0 0;
                font-size: 13px;
                color: #64748b;
            }

        /* =====================================================
           LAYOUT PRINCIPAL
        ===================================================== */

        .professor-layout {
            display: grid;
            grid-template-columns: minmax(0, 1.35fr) minmax(400px, 0.65fr);
            gap: 24px;
            align-items: start;
            margin-bottom: 24px;
        }

        .feed-column,
        .side-column {
            min-width: 0;
        }

        /* =====================================================
           FEED
        ===================================================== */

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
            border-radius: 50%;
            overflow: hidden;
            background: linear-gradient( 135deg, #123570, #2563eb );
            color: #ffffff;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 700;
            font-size: 18px;
        }

            .post-avatar img {
                width: 44px !important;
                height: 44px !important;
                min-width: 44px;
                min-height: 44px;
                max-width: 44px !important;
                max-height: 44px !important;
                object-fit: cover;
                display: block;
                border-radius: 50%;
            }

        .post-author {
            color: #1f2937;
            font-size: 15px;
            font-weight: 700;
        }

        .post-date {
            margin-top: 2px;
            color: #94a3b8;
            font-size: 13px;
        }

        .post-type {
            padding: 5px 10px;
            background: #e8f0ff;
            color: #1d4ed8;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 700;
            white-space: nowrap;
        }

        .post-title {
            margin-bottom: 8px;
            color: #1f2937;
            font-size: 19px;
            font-weight: 700;
        }

        .post-content {
            color: #475569;
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
            background: #ffffff;
            border: 1px dashed #cbd5e1;
            border-radius: 12px;
            text-align: center;
            color: #64748b;
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

        .fc .fc-col-header-cell-cushion {
            font-size: 12px;
            text-decoration: none;
        }

        .fc .fc-daygrid-day-number {
            color: #334155;
            font-size: 12px;
            text-decoration: none;
        }

        .fc .fc-event {
            font-size: 11px;
        }

        /* =====================================================
           FORMULÁRIO DE EVENTO
        ===================================================== */

        .event-form-card {
            margin-bottom: 20px;
        }

        .event-form-grid {
            display: grid;
            grid-template-columns: 1fr;
            gap: 14px;
        }

        .form-label {
            font-weight: 600;
            color: #334155;
        }

        .event-buttons {
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
            margin-top: 16px;
        }

        /* =====================================================
           GESTÃO
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
            margin-bottom: 0;
            width: 100%;
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

            .table .btn {
                white-space: nowrap;
            }

        .avaliacao-card {
            margin-bottom: 24px;
        }

        /* =====================================================
           RESPONSIVO
        ===================================================== */

        @media (max-width: 1100px) {
            .professor-layout {
                grid-template-columns: minmax(0, 1fr) minmax(350px, 0.75fr);
            }
        }

        @media (max-width: 950px) {
            .professor-layout {
                grid-template-columns: 1fr;
            }

            .side-column {
                display: grid;
                grid-template-columns: repeat(2, minmax(0, 1fr));
                gap: 20px;
            }

            .calendar-card,
            .event-form-card {
                margin-bottom: 0;
            }

            .management-grid {
                grid-template-columns: 1fr;
            }
        }

        @media (max-width: 700px) {
            .page-header {
                flex-direction: column;
            }

            .side-column {
                grid-template-columns: 1fr;
            }

            .page-card-body {
                padding: 15px;
            }

            .post-header {
                flex-direction: column;
            }

            .fc .fc-toolbar {
                flex-direction: column;
                align-items: stretch;
            }

            .fc .fc-toolbar-chunk {
                display: flex;
                justify-content: center;
            }

            .event-buttons {
                flex-direction: column;
            }

                .event-buttons .btn {
                    width: 100%;
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

                        <label class="form-label">
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
                            Estão a ser apresentadas apenas as informações da turma selecionada.

                        </div>

                    </div>

                </div>

            </div>

        </div>


        <!-- ==================================================
             FEED + CALENDÁRIO
        =================================================== -->

        <div class="professor-layout">


            <!-- FEED -->

            <section class="feed-column">

                <div class="section-title">

                    <h2>Feed da turma
                    </h2>

                    <p>
                        Publicações partilhadas pelos alunos desta turma.
                    </p>

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
                    Ainda não existem publicações nesta turma.

                </asp:Panel>

            </section>


            <!-- COLUNA LATERAL -->

            <aside class="side-column">


                <!-- CALENDÁRIO -->

                <div class="page-card calendar-card">

                    <div class="page-card-body">

                        <div class="section-title">

                            <h2>Calendário
                            </h2>

                            <p>
                                Testes, trabalhos e avisos da turma.
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

                                <label class="form-label">
                                    Tipo
                                </label>

                                <asp:DropDownList
                                    ID="DdlTipo"
                                    runat="server"
                                    CssClass="form-select">

                                    <asp:ListItem>
                                        Trabalho
                                    </asp:ListItem>

                                    <asp:ListItem>
                                        Teste
                                    </asp:ListItem>

                                    <asp:ListItem>
                                        Aviso
                                    </asp:ListItem>

                                </asp:DropDownList>

                            </div>


                            <div>

                                <label class="form-label">
                                    Título
                                </label>

                                <asp:TextBox
                                    ID="TxtTitulo"
                                    runat="server"
                                    CssClass="form-control"
                                    MaxLength="200" />

                            </div>


                            <div>

                                <label class="form-label">
                                    Data e hora
                                </label>

                                <asp:TextBox
                                    ID="TxtDataHora"
                                    runat="server"
                                    CssClass="form-control"
                                    TextMode="DateTimeLocal" />

                            </div>


                            <div>

                                <label class="form-label">
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

                        <label class="form-label">
                            Nota
                        </label>

                        <asp:TextBox
                            ID="TxtNota"
                            runat="server"
                            CssClass="form-control"
                            placeholder="0-20" />

                    </div>


                    <div class="col-md-10">

                        <label class="form-label">
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

    </div>


    <!-- ==================================================
         CALENDÁRIO
    =================================================== -->

    <script>
        document.addEventListener(
            'DOMContentLoaded',
            function () {

                var calendarElement =
                    document.getElementById('calendar');

                var calendar =
                    new FullCalendar.Calendar(
                        calendarElement,
                        {
                            initialView:
                                'dayGridMonth',

                            locale:
                                'pt',

                            height:
                                500,

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
                                JSON.parse(
                                    document.getElementById(
                                        '<%= HdnEvents.ClientID %>'
                                    ).value || '[]'
                                )
                        }
                    );

                calendar.render();
            }
        );
    </script>

</asp:Content>
