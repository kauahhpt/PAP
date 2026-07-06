<%@ Page Title="Feed e Calendário"
    Language="C#"
    MasterPageFile="~/professor/ModeloProfessor.Master"
    AutoEventWireup="true"
    CodeBehind="calendario.aspx.cs"
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
            max-width: 1450px;
            margin: 0 auto;
            text-align: left;
        }

        .page-header {
            margin-bottom: 25px;
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

        .section-title {
            margin-bottom: 16px;
        }

        .section-title h2 {
            margin: 0;
            font-size: 21px;
            font-weight: 700;
            color: #1f2937;
        }

        .section-title p {
            margin: 4px 0 0 0;
            font-size: 14px;
            color: #64748b;
        }

        .turma-selector {
            margin-bottom: 25px;
        }

        .feed-section {
            margin-bottom: 32px;
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
            border-radius: 50%;
            overflow: hidden;
            background: linear-gradient(
                135deg,
                #123570,
                #2563eb
            );
            color: white;
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
            border-radius: 50%;
            display: block;
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

        .calendar-section {
            margin-bottom: 30px;
        }

        #calendar {
            background: #ffffff;
            width: 100%;
            min-height: 620px;
        }

        .calendar-container {
            padding: 20px;
        }

        .management-grid {
            display: grid;
            grid-template-columns: minmax(0, 1fr) minmax(0, 1fr);
            gap: 24px;
            margin-bottom: 25px;
        }

        .form-section {
            margin-bottom: 25px;
        }

        .table {
            margin-bottom: 0;
        }

        .table th {
            color: #475569;
            font-size: 13px;
            font-weight: 700;
            white-space: nowrap;
        }

        .table td {
            color: #334155;
            font-size: 14px;
            vertical-align: middle;
        }

        @media (max-width: 1000px) {
            .management-grid {
                grid-template-columns: 1fr;
            }
        }

        @media (max-width: 700px) {
            .turma-page {
                padding: 0 5px;
            }

            .page-card-body {
                padding: 15px;
            }

            .post-header {
                flex-direction: column;
            }

            .fc .fc-toolbar {
                flex-direction: column;
                gap: 10px;
            }
        }
    </style>

</asp:Content>

<asp:Content
    ID="Main"
    ContentPlaceHolderID="mainContent"
    runat="server">

    <div class="turma-page">

        <div class="page-header">

            <h1>
                Feed e Calendário da Turma
            </h1>

            <div class="page-subtitle">

                <asp:Label
                    ID="LblResumoTurma"
                    runat="server" />

            </div>

        </div>

        <asp:Label
            ID="LblMensagem"
            runat="server"
            Visible="false" />

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

        <section class="feed-section">

            <div class="section-title">

                <h2>
                    Feed da turma
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

                                            <%# Eval(
                                                "CreatedAt",
                                                "{0:dd/MM/yyyy HH:mm}"
                                            ) %>

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

                                <span>
                                    Gostos: <%# Eval("TotalLikes") %>
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

        <section class="calendar-section">

            <div class="section-title">

                <h2>
                    Calendário
                </h2>

                <p>
                    Testes, trabalhos, avisos e outros eventos da turma.
                </p>

            </div>

            <div class="page-card">

                <div class="calendar-container">

                    <div id="calendar"></div>

                </div>

            </div>

        </section>

        <section class="page-card form-section">

            <div class="page-card-body">

                <div class="section-title">

                    <h2>
                        Novo ou editar evento
                    </h2>

                    <p>
                        Crie testes, trabalhos e avisos para a turma.
                    </p>

                </div>

                <asp:HiddenField
                    ID="HdnEventoId"
                    runat="server" />

                <div class="row g-3 align-items-end">

                    <div class="col-md-2">

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

                    <div class="col-md-4">

                        <label class="form-label">
                            Título
                        </label>

                        <asp:TextBox
                            ID="TxtTitulo"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="200" />

                    </div>

                    <div class="col-md-3">

                        <label class="form-label">
                            Data e hora
                        </label>

                        <asp:TextBox
                            ID="TxtDataHora"
                            runat="server"
                            CssClass="form-control"
                            TextMode="DateTimeLocal" />

                    </div>

                    <div class="col-md-3">

                        <label class="form-label">
                            Anexo do professor
                        </label>

                        <asp:FileUpload
                            ID="FileAnexo"
                            runat="server"
                            CssClass="form-control" />

                    </div>

                </div>

                <div class="mt-3">

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

        </section>

        <div class="management-grid">

            <div class="page-card">

                <div class="page-card-body">

                    <div class="section-title">

                        <h2>
                            Eventos da turma
                        </h2>

                    </div>

                    <div class="table-responsive">

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

            <div class="page-card">

                <div class="page-card-body">

                    <div class="section-title">

                        <h2>
                            Entregas dos alunos
                        </h2>

                    </div>

                    <div class="table-responsive">

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

        <asp:Panel
            ID="PainelAvaliacao"
            runat="server"
            Visible="false"
            CssClass="page-card">

            <div class="page-card-body">

                <div class="section-title">

                    <h2>
                        Avaliar entrega
                    </h2>

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
                            initialView: 'dayGridMonth',

                            locale: 'pt',

                            headerToolbar: {
                                left: 'prev,next today',
                                center: 'title',
                                right: 'dayGridMonth,timeGridWeek,timeGridDay'
                            },

                            buttonText: {
                                today: 'Hoje',
                                month: 'Mês',
                                week: 'Semana',
                                day: 'Dia'
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