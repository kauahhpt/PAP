<%@ Page Title="Dashboard do aluno"
    Language="C#"
    MasterPageFile="~/aluno/MasterAluno1.Master"
    AutoEventWireup="true"
    CodeBehind="dashboard.aspx.cs"
    Inherits="AlunoGest.aluno.dashboard" %>

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
           CONTAINER
        ===================================================== */

        .dashboard-page {
            width: 100%;
            max-width: 1500px;
            margin: 0 auto;
            text-align: left;
        }


        /* =====================================================
           CABEÇALHO
        ===================================================== */

        .dashboard-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            gap: 20px;
            margin-bottom: 24px;
        }

            .dashboard-header h1 {
                margin: 0 0 5px 0;
                color: #1f2937;
                font-size: 30px;
                font-weight: 800;
            }

        .dashboard-resumo {
            color: #64748b;
            font-size: 15px;
        }


        /* =====================================================
           LAYOUT PRINCIPAL
        ===================================================== */

        .dashboard-layout {
            display: grid;
            grid-template-columns: minmax(0, 1.35fr) minmax(400px, 0.65fr);
            gap: 24px;
            align-items: start;
        }

        .dashboard-main {
            min-width: 0;
        }

        .dashboard-side {
            min-width: 0;
        }


        /* =====================================================
           CARDS
        ===================================================== */

        .dashboard-card {
            background: #ffffff;
            border: 1px solid rgba(15, 23, 42, 0.08);
            border-radius: 14px;
            box-shadow: 0 2px 8px rgba(15, 23, 42, 0.06);
            overflow: hidden;
        }

        .dashboard-card-body {
            padding: 20px;
        }

        .side-card {
            margin-bottom: 20px;
        }


        /* =====================================================
           CRIAR PUBLICAÇÃO
        ===================================================== */

        .create-post-card {
            margin-bottom: 24px;
        }

        .create-post-top {
            display: flex;
            align-items: center;
            gap: 12px;
        }

        .create-post-icon {
            width: 42px;
            height: 42px;
            flex: 0 0 42px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 50%;
            background: linear-gradient( 135deg, #123570, #2563eb );
            color: white;
            font-size: 18px;
            font-weight: 800;
        }

        .create-post-button {
            flex: 1;
            width: 100%;
            padding: 13px 18px;
            border: 1px solid #d8dee9;
            background: #f8fafc;
            border-radius: 30px;
            color: #64748b;
            text-align: left;
            cursor: pointer;
            transition: background 0.2s, border-color 0.2s;
        }

            .create-post-button:hover {
                background: #f1f5f9;
                border-color: #94a3b8;
            }


        /* =====================================================
           TÍTULOS
        ===================================================== */

        .section-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 15px;
            margin-bottom: 16px;
        }

            .section-header h2 {
                margin: 0;
                color: #1f2937;
                font-size: 20px;
                font-weight: 750;
            }

        .section-description {
            margin: 4px 0 0 0;
            color: #64748b;
            font-size: 13px;
        }

        .side-card-title {
            margin-bottom: 15px;
            color: #1f2937;
            font-size: 18px;
            font-weight: 750;
        }


        /* =====================================================
           FEED
        ===================================================== */

        .feed-section {
            min-width: 0;
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
            width: 44px !important;
            height: 44px !important;
            min-width: 44px;
            min-height: 44px;
            max-width: 44px;
            max-height: 44px;
            flex: 0 0 44px;
            border-radius: 50%;
            overflow: hidden;
            background: linear-gradient( 135deg, #123570, #2563eb );
            color: white;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 700;
            font-size: 18px;
        }

            .post-avatar-img,
            .post-avatar img {
                width: 44px !important;
                height: 44px !important;
                min-width: 44px;
                min-height: 44px;
                max-width: 44px !important;
                max-height: 44px !important;
                object-fit: cover;
                border-radius: 50%;
                display: block;
            }

        .post-user-info {
            min-width: 0;
        }

        .post-author {
            color: #1f2937;
            font-size: 15px;
            font-weight: 700;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        .post-date {
            color: #94a3b8;
            font-size: 13px;
            margin-top: 2px;
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

        .publicacao-conteudo {
            margin-bottom: 18px;
            color: #475569;
            line-height: 1.6;
            white-space: pre-line;
        }

        .post-actions {
            display: flex;
            align-items: center;
            gap: 10px;
            padding-top: 12px;
            border-top: 1px solid #edf0f5;
        }


        /* =====================================================
           CALENDÁRIO COMPACTO
        ===================================================== */

        .calendar-card {
            margin-bottom: 20px;
        }

        .calendar-container {
            padding: 18px;
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
           TABELAS
        ===================================================== */

        .table-wrapper {
            width: 100%;
            overflow-x: auto;
        }

        .dashboard-table {
            width: 100%;
            margin-bottom: 0;
        }

            .dashboard-table th {
                color: #475569;
                font-size: 12px;
                font-weight: 700;
                border-bottom: 1px solid #e2e8f0;
                white-space: nowrap;
            }

            .dashboard-table td {
                color: #334155;
                font-size: 13px;
                vertical-align: middle;
            }

            .dashboard-table .btn {
                white-space: nowrap;
            }


        /* =====================================================
           PAINEL DE ENTREGA
        ===================================================== */

        .entrega-section {
            margin-top: 24px;
        }

        .entrega-header {
            margin-bottom: 15px;
        }

            .entrega-header h2 {
                margin: 0;
                color: #1f2937;
                font-size: 21px;
                font-weight: 750;
            }

        .anexo-professor {
            display: inline-flex;
            padding: 7px 10px;
            margin: 4px 5px 4px 0;
            border-radius: 8px;
            background: #eff6ff;
            color: #1d4ed8;
            text-decoration: none;
            font-size: 13px;
            font-weight: 600;
        }

            .anexo-professor:hover {
                background: #dbeafe;
            }


        /* =====================================================
           RESPONSIVO
        ===================================================== */

        @media (max-width: 1200px) {
            .dashboard-layout {
                grid-template-columns: minmax(0, 1fr) minmax(350px, 0.72fr);
            }
        }

        @media (max-width: 950px) {
            .dashboard-layout {
                grid-template-columns: 1fr;
            }

            .dashboard-side {
                display: grid;
                grid-template-columns: repeat(2, minmax(0, 1fr));
                gap: 20px;
            }

            .calendar-card {
                grid-column: 1 / -1;
                margin-bottom: 0;
            }

            .side-card {
                margin-bottom: 0;
            }
        }

        @media (max-width: 700px) {
            .dashboard-header {
                flex-direction: column;
            }

            .dashboard-side {
                grid-template-columns: 1fr;
            }

            .calendar-card {
                grid-column: auto;
            }

            .dashboard-card-body {
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
        }
    </style>

</asp:Content>


<asp:Content
    ID="Main"
    ContentPlaceHolderID="mainContent"
    runat="server">

    <div class="dashboard-page">

        <!-- ==================================================
             CABEÇALHO
        =================================================== -->

        <div class="dashboard-header">

            <div>

                <h1>O meu dashboard
                </h1>

                <div class="dashboard-resumo">

                    <asp:Label
                        ID="LblResumo"
                        runat="server" />

                </div>

            </div>

        </div>


        <!-- ==================================================
             MENSAGENS
        =================================================== -->

        <asp:Label
            ID="LblMensagem"
            runat="server"
            Visible="false" />


        <!-- ==================================================
             LAYOUT PRINCIPAL
        =================================================== -->

        <div class="dashboard-layout">


            <!-- ==================================================
                 COLUNA PRINCIPAL - FEED
            =================================================== -->

            <div class="dashboard-main">


                <!-- CRIAR PUBLICAÇÃO -->

                <div class="dashboard-card create-post-card">

                    <div class="dashboard-card-body">

                        <div class="create-post-top">

                            <div class="create-post-icon">
                                +
                            </div>

                            <button
                                type="button"
                                class="create-post-button"
                                data-bs-toggle="modal"
                                data-bs-target="#modalPublicacao">
                                Em que estás a pensar?

                            </button>

                        </div>

                    </div>

                </div>


                <!-- FEED -->

                <section class="feed-section">

                    <div class="section-header">

                        <div>

                            <h2>Feed da turma
                            </h2>

                            <p class="section-description">
                                Publicações e mensagens partilhadas com a tua turma.
                            </p>

                        </div>

                    </div>


                    <asp:Repeater
                        ID="RepeaterPublicacoes"
                        runat="server"
                        OnItemCommand="RepeaterPublicacoes_ItemCommand">

                        <ItemTemplate>

                            <article class="dashboard-card post-card">

                                <div class="dashboard-card-body">

                                    <div class="post-header">

                                        <div class="post-user">

                                            <div class="post-avatar">

                                                <asp:Image
                                                    ID="ImgAutorPublicacao"
                                                    runat="server"
                                                    CssClass="post-avatar-img"
                                                    ImageUrl='<%# ObterFotoPublicacao(Eval("Foto")) %>'
                                                    Visible='<%# TemFotoPublicacao(Eval("Foto")) %>' />

                                                <asp:Literal
                                                    ID="LitInicialAutor"
                                                    runat="server"
                                                    Text='<%# Eval("NomeCompleto").ToString().Substring(0, 1).ToUpper() %>'
                                                    Visible='<%# !TemFotoPublicacao(Eval("Foto")) %>' />

                                            </div>

                                            <div class="post-user-info">

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


                                    <div class="publicacao-conteudo">

                                        <%# Eval("Conteudo") %>
                                    </div>


                                    <div class="post-actions">

                                        <asp:LinkButton
                                            runat="server"
                                            CommandName="Like"
                                            CommandArgument='<%# Eval("Id") %>'
                                            CssClass="btn btn-outline-primary btn-sm">

                                            Gosto (<%# Eval("TotalLikes") %>)

                                        </asp:LinkButton>

                                    </div>

                                </div>

                            </article>

                        </ItemTemplate>

                    </asp:Repeater>

                </section>

            </div>


            <!-- ==================================================
                 COLUNA DIREITA
            =================================================== -->

            <aside class="dashboard-side">


                <!-- CALENDÁRIO -->

                <div class="dashboard-card calendar-card">

                    <div class="dashboard-card-body">

                        <div class="section-header">

                            <div>

                                <h2>Calendário
                                </h2>

                                <p class="section-description">
                                    Próximos testes, trabalhos e eventos.
                                </p>

                            </div>

                        </div>

                    </div>

                    <div class="calendar-container">

                        <div id="calendar"></div>

                    </div>

                </div>


                <!-- TRABALHOS E TESTES -->

                <div class="dashboard-card side-card">

                    <div class="dashboard-card-body">

                        <div class="side-card-title">
                            Trabalhos e testes
                        </div>

                        <div class="table-wrapper">

                            <asp:GridView
                                ID="GridEventos"
                                runat="server"
                                AutoGenerateColumns="false"
                                CssClass="table table-hover dashboard-table"
                                GridLines="None"
                                DataKeyNames="Id"
                                EmptyDataText="Ainda não existem eventos da turma."
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
                                        DataFormatString="{0:dd/MM HH:mm}" />

                                    <asp:BoundField
                                        DataField="EstadoEntrega"
                                        HeaderText="Entrega" />

                                    <asp:TemplateField>

                                        <ItemTemplate>

                                            <asp:LinkButton
                                                runat="server"
                                                Text="Abrir"
                                                CommandName="AbrirEvento"
                                                CommandArgument='<%# Container.DataItemIndex %>'
                                                CssClass="btn btn-primary btn-sm" />

                                        </ItemTemplate>

                                    </asp:TemplateField>

                                </Columns>

                            </asp:GridView>

                        </div>

                    </div>

                </div>


                <!-- NOTAS -->

                <div class="dashboard-card side-card">

                    <div class="dashboard-card-body">

                        <div class="side-card-title">
                            Notas e feedback
                        </div>

                        <div class="table-wrapper">

                            <asp:GridView
                                ID="GridNotas"
                                runat="server"
                                AutoGenerateColumns="false"
                                CssClass="table table-hover dashboard-table"
                                GridLines="None"
                                EmptyDataText="Ainda não existem avaliações.">

                                <Columns>

                                    <asp:BoundField
                                        DataField="Evento"
                                        HeaderText="Evento" />

                                    <asp:BoundField
                                        DataField="Nota"
                                        HeaderText="Nota" />

                                    <asp:BoundField
                                        DataField="Feedback"
                                        HeaderText="Feedback" />

                                </Columns>

                            </asp:GridView>

                        </div>

                    </div>

                </div>

            </aside>

        </div>


        <!-- ==================================================
             PAINEL DE ENTREGA
        =================================================== -->

        <asp:Panel
            ID="PainelEntrega"
            runat="server"
            Visible="false"
            CssClass="dashboard-card entrega-section">

            <div class="dashboard-card-body">

                <div class="entrega-header">

                    <h2>

                        <asp:Label
                            ID="LblEventoSelecionado"
                            runat="server" />

                    </h2>

                </div>

                <asp:HiddenField
                    ID="HdnEventoId"
                    runat="server" />


                <p class="text-muted">
                    Anexos publicados pelo professor:
                </p>


                <asp:Repeater
                    ID="RepeaterAnexosProfessor"
                    runat="server">

                    <ItemTemplate>

                        <a
                            class="anexo-professor"
                            href='<%# Eval("CaminhoFicheiro") %>'
                            target="_blank">

                            <%# Eval("NomeFicheiro") %>

                        </a>

                    </ItemTemplate>

                </asp:Repeater>


                <hr />


                <div class="row g-3">

                    <div class="col-md-5">

                        <label class="form-label">
                            Anexo da entrega
                        </label>

                        <asp:FileUpload
                            ID="FileEntrega"
                            runat="server"
                            CssClass="form-control" />

                    </div>


                    <div class="col-md-7">

                        <label class="form-label">
                            Observação
                        </label>

                        <asp:TextBox
                            ID="TxtObservacao"
                            runat="server"
                            CssClass="form-control"
                            TextMode="MultiLine"
                            Rows="2" />

                    </div>

                </div>


                <div class="mt-3">

                    <asp:Button
                        ID="BtnEntregar"
                        runat="server"
                        Text="Enviar entrega"
                        CssClass="btn btn-success"
                        OnClick="BtnEntregar_Click" />

                    <asp:Button
                        ID="BtnFecharEntrega"
                        runat="server"
                        Text="Fechar"
                        CssClass="btn btn-outline-secondary"
                        OnClick="BtnFecharEntrega_Click"
                        CausesValidation="false" />

                </div>

            </div>

        </asp:Panel>


        <asp:HiddenField
            ID="HdnEvents"
            runat="server" />

    </div>


    <!-- ==================================================
         MODAL CRIAR PUBLICAÇÃO
    =================================================== -->

    <div
        class="modal fade"
        id="modalPublicacao"
        tabindex="-1"
        aria-labelledby="modalPublicacaoLabel"
        aria-hidden="true">

        <div class="modal-dialog modal-lg modal-dialog-centered">

            <div class="modal-content">

                <div class="modal-header">

                    <h5
                        class="modal-title"
                        id="modalPublicacaoLabel">Criar publicação

                    </h5>

                    <button
                        type="button"
                        class="btn-close"
                        data-bs-dismiss="modal"
                        aria-label="Fechar">
                    </button>

                </div>


                <div class="modal-body">

                    <div class="mb-3">

                        <label class="form-label">
                            Tipo
                        </label>

                        <asp:DropDownList
                            ID="DdlTipoPublicacao"
                            runat="server"
                            CssClass="form-select">

                            <asp:ListItem
                                Text="Publicação"
                                Value="Publicacao" />

                            <asp:ListItem
                                Text="Dúvida"
                                Value="Duvida" />

                            <asp:ListItem
                                Text="Material"
                                Value="Material" />

                            <asp:ListItem
                                Text="Aviso"
                                Value="Aviso" />

                            <asp:ListItem
                                Text="Dica"
                                Value="Dica" />

                        </asp:DropDownList>

                    </div>


                    <div class="mb-3">

                        <label class="form-label">
                            Título
                        </label>

                        <asp:TextBox
                            ID="TxtTituloPublicacao"
                            runat="server"
                            CssClass="form-control" />

                    </div>


                    <div class="mb-3">

                        <label class="form-label">
                            Conteúdo
                        </label>

                        <asp:TextBox
                            ID="TxtConteudoPublicacao"
                            runat="server"
                            CssClass="form-control"
                            TextMode="MultiLine"
                            Rows="5" />

                    </div>


                    <div class="mb-3">

                        <label class="form-label">
                            Anexo
                        </label>

                        <asp:FileUpload
                            ID="FilePublicacao"
                            runat="server"
                            CssClass="form-control" />

                    </div>


                    <div class="mb-2">

                        <asp:CheckBox
                            ID="ChkPublicaTurma"
                            runat="server"
                            Text=" Tornar visível para a turma" />

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
                        ID="BtnPublicar"
                        runat="server"
                        Text="Publicar"
                        CssClass="btn btn-primary"
                        OnClick="BtnPublicar_Click" />

                </div>

            </div>

        </div>

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
