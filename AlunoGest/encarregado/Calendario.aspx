<%@ Page
    Title="Calendário"
    Language="C#"
    MasterPageFile="~/encarregado/modeloEncarregado.Master"
    AutoEventWireup="true"
    CodeBehind="Calendario.aspx.cs"
    Inherits="AlunoGest.encarregado.Calendario"
    MaintainScrollPositionOnPostback="true" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="headContent"
    runat="server">

    <link
        href="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.8/index.global.min.css"
        rel="stylesheet" />

    <style>

        .calendario-page {
            padding-bottom: 40px;
        }

        .page-header {
            display: flex;
            align-items: flex-start;
            justify-content: space-between;

            gap: 20px;

            margin-bottom: 24px;
        }

        .page-header h1 {
            margin: 0 0 6px;

            color: #1f2937;

            font-size: 30px;
            font-weight: 800;
        }

        .page-header p {
            max-width: 730px;

            margin: 0;

            color: #64748b;

            font-size: 14px;
            line-height: 1.6;
        }

        .educando-selecionado {
            display: inline-flex;
            align-items: center;

            gap: 8px;

            padding: 10px 14px;

            border: 1px solid #bfdbfe;
            border-radius: 10px;

            background: #eff6ff;
            color: #1d4ed8;

            font-size: 13px;
            font-weight: 700;

            white-space: nowrap;
        }

        .educando-selecionado::before {
            width: 8px;
            height: 8px;

            border-radius: 50%;

            background: #2563eb;

            content: "";
        }


        /* =====================================================
           RESUMO
        ===================================================== */

        .resumo-grid {
            display: grid;

            grid-template-columns:
                repeat(3, minmax(0, 1fr));

            gap: 16px;

            margin-bottom: 22px;
        }

        .resumo-card {
            padding: 18px;

            border: 1px solid #e2e8f0;
            border-radius: 13px;

            background: #ffffff;

            box-shadow:
                0 4px 14px rgba(15, 23, 42, 0.05);
        }

        .resumo-label {
            display: block;

            margin-bottom: 7px;

            color: #64748b;

            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;

            letter-spacing: 0.04em;
        }

        .resumo-valor {
            display: block;

            color: #1f2937;

            font-size: 25px;
            font-weight: 800;
        }

        .resumo-descricao {
            display: block;

            margin-top: 4px;

            color: #94a3b8;

            font-size: 12px;
        }


        /* =====================================================
           CALENDÁRIO
        ===================================================== */

        .calendario-card {
            overflow: hidden;

            border: 1px solid #dbe3ed;
            border-radius: 15px;

            background: #ffffff;

            box-shadow:
                0 5px 18px rgba(15, 23, 42, 0.06);
        }

        .calendario-card-header {
            display: flex;
            align-items: center;
            justify-content: space-between;

            gap: 18px;

            padding: 19px 22px;

            border-bottom: 1px solid #e5e7eb;

            background: #f8fafc;
        }

        .calendario-card-header h2 {
            margin: 0 0 4px;

            color: #1f2937;

            font-size: 19px;
            font-weight: 800;
        }

        .calendario-card-header p {
            margin: 0;

            color: #64748b;

            font-size: 12px;
        }

        .legenda {
            display: flex;
            flex-wrap: wrap;
            justify-content: flex-end;

            gap: 10px;
        }

        .legenda-item {
            display: inline-flex;
            align-items: center;

            gap: 6px;

            color: #475569;

            font-size: 11px;
            font-weight: 700;
        }

        .legenda-cor {
            width: 10px;
            height: 10px;

            border-radius: 50%;
        }

        .cor-teste {
            background: #dc2626;
        }

        .cor-trabalho {
            background: #2563eb;
        }

        .cor-evento {
            background: #16a34a;
        }

        .cor-outro {
            background: #7c3aed;
        }

        .calendario-card-body {
            padding: 22px;
        }

        #calendario {
            min-height: 650px;
        }


        /* =====================================================
           FULLCALENDAR
        ===================================================== */

        .fc {
            color: #334155;

            font-family: inherit;
        }

        .fc .fc-toolbar-title {
            color: #1f2937;

            font-size: 21px;
            font-weight: 800;
        }

        .fc .fc-button {
            border-color: #2563eb;

            background: #2563eb;

            font-size: 12px;
            font-weight: 700;

            box-shadow: none;
        }

        .fc .fc-button:hover,
        .fc .fc-button:focus {
            border-color: #1d4ed8;

            background: #1d4ed8;
        }

        .fc .fc-button-primary:not(:disabled).fc-button-active {
            border-color: #1e40af;

            background: #1e40af;
        }

        .fc .fc-col-header-cell {
            padding: 10px 4px;

            background: #f8fafc;
        }

        .fc .fc-col-header-cell-cushion {
            color: #475569;

            font-size: 12px;
            font-weight: 800;

            text-decoration: none;
        }

        .fc .fc-daygrid-day-number {
            color: #475569;

            font-size: 12px;
            font-weight: 700;

            text-decoration: none;
        }

        .fc .fc-day-today {
            background: #eff6ff !important;
        }

        .fc .fc-event {
            border: none;
            border-radius: 5px;

            padding: 2px 4px;

            cursor: pointer;

            font-size: 11px;
            font-weight: 700;
        }


        /* =====================================================
           SEM EDUCANDO
        ===================================================== */

        .sem-educando {
            padding: 60px 24px;

            border: 1px solid #dbe3ed;
            border-radius: 15px;

            background: #ffffff;

            text-align: center;

            box-shadow:
                0 5px 18px rgba(15, 23, 42, 0.06);
        }

        .sem-educando-icon {
            width: 64px;
            height: 64px;

            display: flex;
            align-items: center;
            justify-content: center;

            margin: 0 auto 16px;

            border-radius: 17px;

            background: #eff6ff;
            color: #2563eb;

            font-size: 27px;
            font-weight: 800;
        }

        .sem-educando h2 {
            margin: 0 0 7px;

            color: #334155;

            font-size: 21px;
            font-weight: 800;
        }

        .sem-educando p {
            max-width: 560px;

            margin: 0 auto;

            color: #64748b;

            font-size: 14px;
            line-height: 1.6;
        }


        /* =====================================================
           MODAL
        ===================================================== */

        .evento-detalhe {
            margin-bottom: 15px;
        }

        .evento-detalhe:last-child {
            margin-bottom: 0;
        }

        .evento-detalhe-label {
            display: block;

            margin-bottom: 4px;

            color: #64748b;

            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;
        }

        .evento-detalhe-valor {
            color: #334155;

            font-size: 14px;
            font-weight: 600;

            white-space: pre-wrap;
            word-break: break-word;
        }


        /* =====================================================
           RESPONSIVO
        ===================================================== */

        @media (max-width: 900px) {

            .resumo-grid {
                grid-template-columns: 1fr;
            }

            .calendario-card-header {
                align-items: flex-start;
                flex-direction: column;
            }

            .legenda {
                justify-content: flex-start;
            }
        }

        @media (max-width: 700px) {

            .page-header {
                flex-direction: column;
            }

            .educando-selecionado {
                width: 100%;

                justify-content: center;
            }

            .calendario-card-body {
                padding: 12px;
            }

            #calendario {
                min-height: 560px;
            }

            .fc .fc-toolbar {
                align-items: stretch;
                flex-direction: column;

                gap: 10px;
            }

            .fc .fc-toolbar-chunk {
                display: flex;
                justify-content: center;
            }

            .fc .fc-toolbar-title {
                font-size: 18px;
            }
        }

    </style>

</asp:Content>


<asp:Content
    ID="Content2"
    ContentPlaceHolderID="mainContent"
    runat="server">

    <div class="calendario-page">


        <!-- CABEÇALHO -->

        <div class="page-header">

            <div>

                <h1>
                    Calendário
                </h1>

                <p>
                    Consulte os testes, trabalhos e eventos escolares
                    do educando selecionado.
                </p>

            </div>

            <asp:Label
                ID="LblEducandoSelecionado"
                runat="server"
                Visible="false"
                CssClass="educando-selecionado" />

        </div>


        <!-- MENSAGEM -->

        <asp:Label
            ID="LblMensagem"
            runat="server"
            Visible="false" />


        <!-- SEM EDUCANDO -->

        <asp:Panel
            ID="PnlSemEducando"
            runat="server"
            Visible="false"
            CssClass="sem-educando">

            <div class="sem-educando-icon">
                !
            </div>

            <h2>
                Nenhum educando selecionado
            </h2>

            <p>
                Não existe um aluno ativo associado à sua conta.
                Contacte o agrupamento para confirmar a associação.
            </p>

        </asp:Panel>


        <!-- CONTEÚDO -->

        <asp:Panel
            ID="PnlConteudo"
            runat="server"
            Visible="false">


            <!-- RESUMO -->

            <div class="resumo-grid">

                <div class="resumo-card">

                    <span class="resumo-label">
                        Próximos eventos
                    </span>

                    <asp:Label
                        ID="LblTotalProximos"
                        runat="server"
                        Text="0"
                        CssClass="resumo-valor" />

                    <span class="resumo-descricao">
                        A partir de hoje
                    </span>

                </div>

                <div class="resumo-card">

                    <span class="resumo-label">
                        Testes
                    </span>

                    <asp:Label
                        ID="LblTotalTestes"
                        runat="server"
                        Text="0"
                        CssClass="resumo-valor" />

                    <span class="resumo-descricao">
                        Próximos testes
                    </span>

                </div>

                <div class="resumo-card">

                    <span class="resumo-label">
                        Trabalhos
                    </span>

                    <asp:Label
                        ID="LblTotalTrabalhos"
                        runat="server"
                        Text="0"
                        CssClass="resumo-valor" />

                    <span class="resumo-descricao">
                        Próximas entregas
                    </span>

                </div>

            </div>


            <!-- CALENDÁRIO -->

            <section class="calendario-card">

                <header class="calendario-card-header">

                    <div>

                        <h2>
                            Eventos escolares
                        </h2>

                        <p>
                            Clique num evento para consultar os detalhes.
                        </p>

                    </div>

                    <div class="legenda">

                        <span class="legenda-item">

                            <span class="legenda-cor cor-teste"></span>

                            Teste

                        </span>

                        <span class="legenda-item">

                            <span class="legenda-cor cor-trabalho"></span>

                            Trabalho

                        </span>

                        <span class="legenda-item">

                            <span class="legenda-cor cor-evento"></span>

                            Evento

                        </span>

                        <span class="legenda-item">

                            <span class="legenda-cor cor-outro"></span>

                            Outro

                        </span>

                    </div>

                </header>

                <div class="calendario-card-body">

                    <div id="calendario"></div>

                </div>

            </section>

        </asp:Panel>


        <!-- DADOS DOS EVENTOS -->

        <asp:HiddenField
            ID="HfEventosJson"
            runat="server" />

    </div>


    <!-- MODAL DETALHES -->

    <div
        class="modal fade"
        id="modalEvento"
        tabindex="-1"
        aria-labelledby="modalEventoTitulo"
        aria-hidden="true">

        <div class="modal-dialog modal-dialog-centered">

            <div class="modal-content">

                <div class="modal-header">

                    <h5
                        class="modal-title"
                        id="modalEventoTitulo">

                        Detalhes do evento

                    </h5>

                    <button
                        type="button"
                        class="btn-close"
                        data-bs-dismiss="modal"
                        aria-label="Fechar">
                    </button>

                </div>

                <div class="modal-body">

                    <div class="evento-detalhe">

                        <span class="evento-detalhe-label">
                            Tipo
                        </span>

                        <div
                            id="modalEventoTipo"
                            class="evento-detalhe-valor">
                        </div>

                    </div>

                    <div class="evento-detalhe">

                        <span class="evento-detalhe-label">
                            Data e hora
                        </span>

                        <div
                            id="modalEventoData"
                            class="evento-detalhe-valor">
                        </div>

                    </div>

                    <div class="evento-detalhe">

                        <span class="evento-detalhe-label">
                            Descrição
                        </span>

                        <div
                            id="modalEventoDescricao"
                            class="evento-detalhe-valor">
                        </div>

                    </div>

                </div>

                <div class="modal-footer">

                    <button
                        type="button"
                        class="btn btn-secondary"
                        data-bs-dismiss="modal">

                        Fechar

                    </button>

                </div>

            </div>

        </div>

    </div>


    <script src="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.8/index.global.min.js"></script>

    <script>

        document.addEventListener(
            "DOMContentLoaded",
            function () {

                const calendarElement =
                    document.getElementById("calendario");

                if (!calendarElement) {
                    return;
                }

                const hiddenEventos =
                    document.getElementById(
                        "<%= HfEventosJson.ClientID %>"
                    );

                let eventos = [];

                if (
                    hiddenEventos &&
                    hiddenEventos.value
                ) {
                    try {
                        eventos =
                            JSON.parse(
                                hiddenEventos.value
                            );
                    }
                    catch (erro) {
                        console.error(
                            "Erro ao interpretar os eventos:",
                            erro
                        );
                    }
                }

                const calendario =
                    new FullCalendar.Calendar(
                        calendarElement,
                        {
                            locale: "pt",

                            initialView:
                                "dayGridMonth",

                            firstDay: 1,

                            height: "auto",

                            displayEventTime: true,

                            eventTimeFormat: {
                                hour: "2-digit",
                                minute: "2-digit",
                                hour12: false
                            },

                            headerToolbar: {
                                left:
                                    "prev,next today",

                                center:
                                    "title",

                                right:
                                    "dayGridMonth,listMonth"
                            },

                            buttonText: {
                                today: "Hoje",
                                month: "Mês",
                                list: "Lista"
                            },

                            noEventsText:
                                "Não existem eventos neste período.",

                            events: eventos,

                            eventClick: function (info) {

                                info.jsEvent.preventDefault();

                                const evento =
                                    info.event;

                                const propriedades =
                                    evento.extendedProps || {};

                                document
                                    .getElementById(
                                        "modalEventoTitulo"
                                    )
                                    .textContent =
                                        evento.title ||
                                        "Detalhes do evento";

                                document
                                    .getElementById(
                                        "modalEventoTipo"
                                    )
                                    .textContent =
                                        propriedades.tipo ||
                                        "Não definido";

                                document
                                    .getElementById(
                                        "modalEventoDescricao"
                                    )
                                    .textContent =
                                        propriedades.descricao ||
                                        "Sem descrição.";

                                let dataTexto =
                                    "Não definida";

                                if (evento.start) {
                                    dataTexto =
                                        evento.start
                                            .toLocaleString(
                                                "pt-PT",
                                                {
                                                    dateStyle:
                                                        "long",

                                                    timeStyle:
                                                        "short"
                                                }
                                            );
                                }

                                document
                                    .getElementById(
                                        "modalEventoData"
                                    )
                                    .textContent =
                                        dataTexto;

                                const modalElement =
                                    document.getElementById(
                                        "modalEvento"
                                    );

                                const modal =
                                    bootstrap.Modal
                                        .getOrCreateInstance(
                                            modalElement
                                        );

                                modal.show();
                            }
                        }
                    );

                calendario.render();
            }
        );

    </script>

</asp:Content>