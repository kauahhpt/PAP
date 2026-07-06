<%@ Page Title="Dashboard do aluno"
    Language="C#"
    MasterPageFile="~/aluno/MasterAluno1.Master"
    AutoEventWireup="true"
    CodeBehind="dashboard.aspx.cs"
    Inherits="AlunoGest.aluno.dashboard" %>


<asp:Content ID="Head"
    ContentPlaceHolderID="headContent"
    runat="server">

    <link href="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.8/index.global.min.css"
        rel="stylesheet" />

    <script src="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.8/index.global.min.js"></script>


    <style>
        /* =====================================================
           CONTAINER GERAL
        ===================================================== */

        .dashboard-page {
            width: calc(100% - 40px);
            max-width: 1500px;
            margin: 0 auto;
            text-align: left;
        }


        /* =====================================================
           CABEÇALHO DO DASHBOARD
        ===================================================== */

        .dashboard-header {
            margin-bottom: 24px;
        }

            .dashboard-header h1 {
                margin: 0 0 5px 0;
                font-size: 30px;
                font-weight: 700;
                color: #1f2937;
            }

            .dashboard-header .dashboard-resumo {
                color: #64748b;
                font-size: 15px;
            }


        /* =====================================================
           GRID PRINCIPAL
        ===================================================== */

        .dashboard-grid {
            display: grid !important;
            grid-template-columns: minmax(0, 1fr) 380px;
            gap: 24px;
            align-items: start;
            width: 100%;
        }


        .dashboard-main {
            min-width: 0;
            width: 100%;
        }


        .dashboard-sidebar {
            min-width: 0;
            width: 100%;
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


        /* =====================================================
           CAIXA CRIAR PUBLICAÇÃO
        ===================================================== */

        .create-post-card {
            margin-bottom: 24px;
        }


        .create-post-button {
            width: 100%;
            padding: 16px 20px;
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
           TÍTULO DAS SECÇÕES
        ===================================================== */

        .section-title {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 15px;
        }


            .section-title h2 {
                margin: 0;
                font-size: 20px;
                font-weight: 700;
                color: #1f2937;
            }


        /* =====================================================
           FEED
        ===================================================== */

        .feed-section {
            margin-bottom: 24px;
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


        .post-author {
            font-weight: 700;
            color: #1f2937;
            font-size: 15px;
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
            font-size: 19px;
            font-weight: 700;
            color: #1f2937;
            margin-bottom: 8px;
        }


        .publicacao-conteudo {
            white-space: pre-line;
            color: #475569;
            line-height: 1.6;
            margin-bottom: 18px;
        }


        .post-actions {
            display: flex;
            align-items: center;
            gap: 10px;
            padding-top: 12px;
            border-top: 1px solid #edf0f5;
        }


        /* =====================================================
           SIDEBAR
        ===================================================== */

        .sidebar-card {
            margin-bottom: 20px;
        }


        .sidebar-card-title {
            font-size: 18px;
            font-weight: 700;
            color: #1f2937;
            margin-bottom: 15px;
        }


        /* =====================================================
           TABELAS
        ===================================================== */

        .dashboard-table {
            margin-bottom: 0;
            width: 100%;
        }


            .dashboard-table th {
                color: #475569;
                font-size: 13px;
                font-weight: 700;
                border-bottom: 1px solid #e2e8f0;
                white-space: nowrap;
            }


            .dashboard-table td {
                font-size: 14px;
                vertical-align: middle;
                color: #334155;
            }


        /* =====================================================
           CALENDÁRIO
        ===================================================== */

        .calendar-section {
            margin-top: 24px;
        }


        #calendar {
            background: #ffffff;
            width: 100%;
            min-height: 620px;
        }


        .calendar-container {
            padding: 20px;
        }


        /* =====================================================
           ENTREGA
        ===================================================== */

        .entrega-section {
            margin-top: 24px;
        }


        /* =====================================================
           RESPONSIVO
        ===================================================== */

        @media (max-width: 1100px) {

            .dashboard-grid {
                grid-template-columns: 1fr;
            }


            .dashboard-sidebar {
                display: grid;
                grid-template-columns: repeat(2, minmax(0, 1fr));
                gap: 20px;
            }


            .sidebar-card {
                margin-bottom: 0;
            }
        }


        @media (max-width: 700px) {

            .dashboard-page {
                width: calc(100% - 20px);
            }


            .dashboard-sidebar {
                grid-template-columns: 1fr;
            }


            .dashboard-card-body {
                padding: 15px;
            }


            .post-header {
                flex-direction: column;
            }
        }
    </style>

</asp:Content>



<asp:Content ID="Main"
    ContentPlaceHolderID="mainContent"
    runat="server">


    <div class="dashboard-page">


        <!-- ==================================================
             CABEÇALHO
        =================================================== -->

        <div class="dashboard-header">

            <h1>O meu dashboard</h1>

            <div class="dashboard-resumo">

                <asp:Label
                    ID="LblResumo"
                    runat="server" />

            </div>

        </div>


        <!-- MENSAGENS -->

        <asp:Label
            ID="LblMensagem"
            runat="server"
            Visible="false" />


        <!-- ==================================================
             GRID PRINCIPAL
        =================================================== -->

        <div class="dashboard-grid">


            <!-- ==================================================
                 COLUNA PRINCIPAL
            =================================================== -->

            <div class="dashboard-main">


                <!-- CRIAR PUBLICAÇÃO -->

                <div class="dashboard-card create-post-card">

                    <div class="dashboard-card-body">

                        <button
                            type="button"
                            class="create-post-button"
                            data-bs-toggle="modal"
                            data-bs-target="#modalPublicacao">
                            Em que estás a pensar?

                        </button>

                    </div>

                </div>


                <!-- ==================================================
                     FEED
                =================================================== -->

                <section class="feed-section">


                    <div class="section-title">

                        <h2>Feed da turma</h2>

                    </div>


                    <asp:Repeater
                        ID="RepeaterPublicacoes"
                        runat="server"
                        OnItemCommand="RepeaterPublicacoes_ItemCommand">


                        <ItemTemplate>


                            <article class="dashboard-card post-card">


                                <div class="dashboard-card-body">


                                    <!-- CABEÇALHO DA PUBLICAÇÃO -->

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



                                    <!-- CONTEÚDO -->

                                    <div class="post-title">

                                        <%# Eval("Titulo") %>
                                    </div>


                                    <div class="publicacao-conteudo">

                                        <%# Eval("Conteudo") %>
                                    </div>



                                    <!-- AÇÕES -->

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



                <!-- ==================================================
                     CALENDÁRIO
                =================================================== -->

                <section class="calendar-section">


                    <div class="section-title">

                        <h2>Calendário</h2>

                    </div>


                    <div class="dashboard-card">


                        <div class="calendar-container">

                            <div id="calendar"></div>

                        </div>


                    </div>


                </section>


            </div>



            <!-- ==================================================
                 SIDEBAR
            =================================================== -->

            <aside class="dashboard-sidebar">


                <!-- TRABALHOS E TESTES -->

                <div class="dashboard-card sidebar-card">


                    <div class="dashboard-card-body">


                        <div class="sidebar-card-title">
                            Trabalhos e testes

                        </div>


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



                <!-- NOTAS -->

                <div class="dashboard-card sidebar-card">


                    <div class="dashboard-card-body">


                        <div class="sidebar-card-title">
                            Notas e feedback

                        </div>


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


                <h2>

                    <asp:Label
                        ID="LblEventoSelecionado"
                        runat="server" />

                </h2>


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


                        <div>

                            <a
                                href='<%# Eval("CaminhoFicheiro") %>'
                                target="_blank">

                                <%# Eval("NomeFicheiro") %>

                            </a>

                        </div>


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

                            headerToolbar:
                            {
                                left:
                                    'prev,next today',

                                center:
                                    'title',

                                right:
                                    'dayGridMonth,timeGridWeek,timeGridDay'
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
