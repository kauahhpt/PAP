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
           PÁGINA
        ===================================================== */

        .dashboard-page {
            width: 100%;
            max-width: 1500px;
            margin: 0 auto;
            padding-bottom: 35px;
            text-align: left;
        }

        /* =====================================================
           CABEÇALHO DO DASHBOARD DO ALUNO
        ===================================================== */

        .dashboard-page > .student-dashboard-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 24px;
            margin: 0 0 24px 0;
            padding: 24px 26px;
            border: 1px solid #e2e8f0;
            border-left: 5px solid #2563eb;
            border-radius: 16px;
            background: #ffffff !important;
            color: #1e293b !important;
            box-shadow:
                0 4px 14px rgba(15, 23, 42, 0.06),
                0 1px 3px rgba(15, 23, 42, 0.04);
        }

        .dashboard-page > .student-dashboard-header h1 {
            margin: 0 0 6px 0 !important;
            background: transparent !important;
            color: #1e293b !important;
            font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif !important;
            font-size: 30px !important;
            font-style: normal !important;
            font-weight: 800 !important;
            line-height: 1.2 !important;
            letter-spacing: -0.03em !important;
            text-shadow: none !important;
        }

        .dashboard-page > .student-dashboard-header .dashboard-kicker {
            display: block;
            margin: 0 0 6px 0;
            background: transparent !important;
            color: #2563eb !important;
            font-size: 12px !important;
            font-style: normal !important;
            font-weight: 800 !important;
            letter-spacing: 0.08em;
            text-transform: uppercase;
        }

        .dashboard-page > .student-dashboard-header .dashboard-resumo {
            background: transparent !important;
            color: #64748b !important;
            font-size: 14px !important;
            font-style: normal !important;
            font-weight: 400 !important;
        }

        .dashboard-page > .student-dashboard-header .dashboard-header-badge {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 9px 14px;
            border: 1px solid #dbe4f0;
            border-radius: 999px;
            background: #f1f5f9 !important;
            color: #334155 !important;
            font-size: 13px !important;
            font-style: normal !important;
            font-weight: 700 !important;
            white-space: nowrap;
        }

        .dashboard-page > .student-dashboard-header
        .dashboard-header-badge::before {
            width: 8px;
            height: 8px;
            border-radius: 50%;
            background: #22c55e;
            content: "";
        }

        /* =====================================================
           LAYOUT PRINCIPAL
        ===================================================== */

        .dashboard-layout {
            display: grid;
            grid-template-columns:
                minmax(0, 1.65fr)
                minmax(350px, 0.75fr);
            gap: 24px;
            align-items: start;
        }

        .dashboard-main,
        .dashboard-side {
            min-width: 0;
        }

        /* =====================================================
           CARDS
        ===================================================== */

        .dashboard-card {
            overflow: hidden;
            border: 1px solid rgba(15, 23, 42, 0.08);
            border-radius: 18px;
            background: #ffffff;
            box-shadow:
                0 4px 14px rgba(15, 23, 42, 0.06),
                0 1px 3px rgba(15, 23, 42, 0.04);
        }

        .dashboard-card-body {
            padding: 22px;
        }

        .dashboard-card:hover {
            border-color: rgba(37, 99, 235, 0.15);
        }

        /* =====================================================
           TÍTULOS
        ===================================================== */

        .section-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 16px;
        }

        .section-header h2 {
            margin: 0;
            color: #182235;
            font-size: 21px;
            font-weight: 800;
            letter-spacing: -0.02em;
        }

        .section-description {
            margin: 5px 0 0;
            color: #64748b;
            font-size: 13px;
            line-height: 1.45;
        }

        .section-icon {
            display: flex;
            width: 43px;
            height: 43px;
            flex: 0 0 43px;
            align-items: center;
            justify-content: center;
            border-radius: 13px;
            background: #eef4ff;
            color: #1d4ed8;
            font-size: 20px;
            font-weight: 800;
        }

        .section-heading-group {
            display: flex;
            align-items: center;
            gap: 13px;
        }

        /* =====================================================
           CALENDÁRIO PRINCIPAL
        ===================================================== */

        .calendar-card {
            position: relative;
            margin-bottom: 22px;
        }

        .calendar-card::before {
            position: absolute;
            z-index: 2;
            top: 0;
            right: 0;
            left: 0;
            height: 4px;
            background: linear-gradient(
                90deg,
                #173b70,
                #2563eb,
                #60a5fa
            );
            content: "";
        }

        .calendar-header {
            padding: 25px 25px 8px;
        }

        .calendar-status {
            display: inline-flex;
            align-items: center;
            gap: 7px;
            padding: 7px 11px;
            border: 1px solid #dbeafe;
            border-radius: 999px;
            background: #eff6ff;
            color: #1d4ed8;
            font-size: 12px;
            font-weight: 700;
            white-space: nowrap;
        }

        .calendar-status::before {
            width: 7px;
            height: 7px;
            border-radius: 50%;
            background: #3b82f6;
            content: "";
        }

        .calendar-container {
            padding: 16px 25px 25px;
        }

        #calendar {
            width: 100%;
            min-height: 500px;
            background: #ffffff;
        }

        .fc .fc-toolbar.fc-header-toolbar {
            margin-bottom: 20px;
        }

        .fc .fc-toolbar-title {
            color: #182235;
            font-size: 1.2rem;
            font-weight: 800;
            text-transform: capitalize;
        }

        .fc .fc-button-primary {
            border-color: #1e3a5f;
            background: #1e3a5f;
            box-shadow: none;
            font-weight: 700;
        }

        .fc .fc-button-primary:hover,
        .fc .fc-button-primary:focus {
            border-color: #163250;
            background: #163250;
            box-shadow: none;
        }

        .fc .fc-button-primary:disabled {
            border-color: #64748b;
            background: #64748b;
        }

        .fc .fc-button {
            padding: 0.48rem 0.7rem;
            border-radius: 8px;
            font-size: 0.8rem;
        }

        .fc-theme-standard td,
        .fc-theme-standard th {
            border-color: #e5eaf1;
        }

        .fc-theme-standard .fc-scrollgrid {
            overflow: hidden;
            border: 1px solid #e2e8f0;
            border-radius: 12px;
        }

        .fc .fc-col-header-cell {
            background: #f8fafc;
        }

        .fc .fc-col-header-cell-cushion {
            padding: 10px 5px;
            color: #1d4ed8;
            font-size: 12px;
            font-weight: 800;
            text-decoration: none;
            text-transform: capitalize;
        }

        .fc .fc-daygrid-day-frame {
            min-height: 90px;
        }

        .fc .fc-daygrid-day-number {
            padding: 8px;
            color: #334155;
            font-size: 12px;
            font-weight: 600;
            text-decoration: none;
        }

        .fc .fc-day-today {
            background: #fffbe8 !important;
        }

        .fc .fc-day-today .fc-daygrid-day-number {
            display: inline-flex;
            width: 27px;
            height: 27px;
            margin: 5px;
            padding: 0;
            align-items: center;
            justify-content: center;
            border-radius: 50%;
            background: #2563eb;
            color: #ffffff;
        }

        .fc .fc-event {
            padding: 2px 4px;
            border: none;
            border-radius: 5px;
            font-size: 11px;
            font-weight: 600;
            cursor: pointer;
        }

        .fc .fc-daygrid-more-link {
            color: #2563eb;
            font-size: 11px;
            font-weight: 700;
        }

        /* =====================================================
           TRABALHOS E NOTAS
        ===================================================== */

        .dashboard-secondary-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 22px;
        }

        .information-card {
            min-width: 0;
        }

        .information-card-header {
            display: flex;
            align-items: center;
            gap: 12px;
            margin-bottom: 18px;
        }

        .information-card-icon {
            display: flex;
            width: 39px;
            height: 39px;
            flex: 0 0 39px;
            align-items: center;
            justify-content: center;
            border-radius: 11px;
            background: #f1f5f9;
            color: #334155;
            font-size: 18px;
            font-weight: 800;
        }

        .information-card-title {
            margin: 0;
            color: #1e293b;
            font-size: 17px;
            font-weight: 800;
        }

        .information-card-description {
            margin: 2px 0 0;
            color: #94a3b8;
            font-size: 12px;
        }

        /* =====================================================
           CRIAR PUBLICAÇÃO
        ===================================================== */

        .create-post-card {
            margin-bottom: 22px;
        }

        .create-post-card .dashboard-card-body {
            padding: 18px;
        }

        .create-post-label {
            display: block;
            margin-bottom: 12px;
            color: #334155;
            font-size: 13px;
            font-weight: 800;
        }

        .create-post-top {
            display: flex;
            align-items: center;
            gap: 11px;
        }

        .create-post-icon {
            display: flex;
            width: 40px;
            height: 40px;
            flex: 0 0 40px;
            align-items: center;
            justify-content: center;
            border-radius: 12px;
            background: linear-gradient(
                135deg,
                #173b70,
                #2563eb
            );
            color: #ffffff;
            font-size: 21px;
            font-weight: 500;
            box-shadow: 0 5px 12px rgba(37, 99, 235, 0.2);
        }

        .create-post-button {
            width: 100%;
            min-width: 0;
            padding: 12px 15px;
            overflow: hidden;
            border: 1px solid #dde4ed;
            border-radius: 12px;
            background: #f8fafc;
            color: #64748b;
            text-align: left;
            text-overflow: ellipsis;
            white-space: nowrap;
            cursor: pointer;
            transition:
                border-color 0.2s,
                background 0.2s,
                color 0.2s;
        }

        .create-post-button:hover {
            border-color: #93b4e8;
            background: #f1f6ff;
            color: #334155;
        }

        /* =====================================================
           FEED SECUNDÁRIO
        ===================================================== */

        .feed-section {
            min-width: 0;
        }

        .feed-heading {
            display: flex;
            justify-content: space-between;
            align-items: flex-end;
            gap: 15px;
            margin-bottom: 15px;
        }

        .feed-heading h2 {
            margin: 0;
            color: #182235;
            font-size: 20px;
            font-weight: 800;
        }

        .feed-heading .section-description {
            margin-top: 3px;
        }

        .feed-label {
            padding: 6px 10px;
            border-radius: 999px;
            background: rgba(255, 255, 255, 0.7);
            color: #64748b;
            font-size: 11px;
            font-weight: 700;
            white-space: nowrap;
        }

        .post-card {
            margin-bottom: 16px;
            transition:
                transform 0.18s ease,
                box-shadow 0.18s ease;
        }

        .post-card:hover {
            transform: translateY(-2px);
            box-shadow:
                0 8px 22px rgba(15, 23, 42, 0.09),
                0 2px 4px rgba(15, 23, 42, 0.04);
        }

        .post-card:last-child {
            margin-bottom: 0;
        }

        .post-card .dashboard-card-body {
            padding: 18px;
        }

        .post-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            gap: 10px;
            margin-bottom: 15px;
        }

        .post-user {
            display: flex;
            min-width: 0;
            align-items: center;
            gap: 10px;
        }

        .post-avatar {
            display: flex;
            width: 42px !important;
            height: 42px !important;
            min-width: 42px;
            min-height: 42px;
            max-width: 42px;
            max-height: 42px;
            flex: 0 0 42px;
            align-items: center;
            justify-content: center;
            overflow: hidden;
            border-radius: 50%;
            background: linear-gradient(
                135deg,
                #173b70,
                #2563eb
            );
            color: #ffffff;
            font-size: 17px;
            font-weight: 800;
        }

        .post-avatar-img,
        .post-avatar img {
            display: block;
            width: 42px !important;
            height: 42px !important;
            min-width: 42px;
            min-height: 42px;
            max-width: 42px !important;
            max-height: 42px !important;
            border-radius: 50%;
            object-fit: cover;
        }

        .post-user-info {
            min-width: 0;
        }

        .post-author {
            overflow: hidden;
            color: #1f2937;
            font-size: 14px;
            font-weight: 800;
            text-overflow: ellipsis;
            white-space: nowrap;
        }

        .post-date {
            margin-top: 2px;
            color: #94a3b8;
            font-size: 11px;
        }

        .post-type {
            padding: 5px 9px;
            border-radius: 999px;
            background: #eaf1ff;
            color: #1d4ed8;
            font-size: 10px;
            font-weight: 800;
            white-space: nowrap;
        }

        .post-title {
            margin-bottom: 8px;
            color: #1e293b;
            font-size: 16px;
            font-weight: 800;
            line-height: 1.35;
        }

        .publicacao-conteudo {
            margin-bottom: 16px;
            color: #526176;
            font-size: 13px;
            line-height: 1.65;
            overflow-wrap: anywhere;
            white-space: pre-line;
        }

        .post-actions {
            display: flex;
            align-items: center;
            gap: 10px;
            padding-top: 12px;
            border-top: 1px solid #edf1f5;
        }

        .post-actions .btn {
            border-radius: 8px;
            font-size: 12px;
            font-weight: 700;
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
            padding-top: 9px;
            padding-bottom: 9px;
            border-bottom: 1px solid #e2e8f0;
            color: #64748b;
            font-size: 11px;
            font-weight: 800;
            white-space: nowrap;
            text-transform: uppercase;
        }

        .dashboard-table td {
            padding-top: 11px;
            padding-bottom: 11px;
            border-bottom-color: #edf1f5;
            color: #334155;
            font-size: 12px;
            vertical-align: middle;
        }

        .dashboard-table tr:last-child td {
            border-bottom: 0;
        }

        .dashboard-table .btn {
            border-radius: 7px;
            font-size: 11px;
            font-weight: 700;
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
            font-weight: 800;
        }

        .anexo-professor {
            display: inline-flex;
            margin: 4px 5px 4px 0;
            padding: 7px 10px;
            border-radius: 8px;
            background: #eff6ff;
            color: #1d4ed8;
            font-size: 13px;
            font-weight: 600;
            text-decoration: none;
        }

        .anexo-professor:hover {
            background: #dbeafe;
        }

        /* =====================================================
           MODAL
        ===================================================== */

        .modal-content {
            overflow: hidden;
            border: 0;
            border-radius: 18px;
            box-shadow: 0 22px 55px rgba(15, 23, 42, 0.22);
        }

        .modal-header {
            padding: 20px 22px;
            border-bottom-color: #edf1f5;
        }

        .modal-title {
            color: #1e293b;
            font-weight: 800;
        }

        .modal-body {
            padding: 22px;
        }

        .modal-footer {
            padding: 16px 22px;
            border-top-color: #edf1f5;
        }

        /* =====================================================
           DESTINATÁRIOS DA PUBLICAÇÃO
        ===================================================== */

        .audience-card {
            padding: 16px;
            border: 1px solid #e2e8f0;
            border-radius: 14px;
            background: #f8fafc;
        }

        .audience-card .form-check {
            margin: 0;
        }

        .audience-card .form-check label {
            color: #1e293b;
            font-size: 14px;
            font-weight: 700;
        }

        .audience-help {
            margin: 6px 0 15px;
            color: #64748b;
            font-size: 12px;
            line-height: 1.5;
        }

        .audience-selector {
            padding-top: 15px;
            border-top: 1px solid #e2e8f0;
        }

        .audience-selector.is-hidden {
            display: none;
        }

        .audience-selector-header {
            margin-bottom: 13px;
        }

        .audience-selector-header h6 {
            margin: 0 0 4px;
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
            margin-bottom: 8px;
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
           SELECIONAR TODOS
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
           RESPONSIVO
        ===================================================== */

        @media (max-width: 1200px) {
            .dashboard-layout {
                grid-template-columns:
                    minmax(0, 1.45fr)
                    minmax(320px, 0.75fr);
            }

            .dashboard-secondary-grid {
                grid-template-columns: 1fr;
            }
        }

        @media (max-width: 980px) {
            .dashboard-layout {
                grid-template-columns: 1fr;
            }

            .dashboard-main {
                order: 1;
            }

            .dashboard-side {
                order: 2;
            }

            .dashboard-secondary-grid {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }
        }

        @media (max-width: 720px) {
            .student-dashboard-header {
                flex-direction: column;
                align-items: flex-start;
            }

            .student-dashboard-header h1 {
                font-size: 27px;
            }

            .dashboard-secondary-grid {
                grid-template-columns: 1fr;
            }

            .calendar-header,
            .calendar-container {
                padding-right: 15px;
                padding-left: 15px;
            }

            .calendar-status {
                display: none;
            }

            .fc .fc-toolbar {
                flex-direction: column;
                align-items: stretch;
                gap: 12px;
            }

            .fc .fc-toolbar-chunk {
                display: flex;
                justify-content: center;
            }

            .fc .fc-daygrid-day-frame {
                min-height: 70px;
            }

            .dashboard-card-body {
                padding: 16px;
            }

            .post-header {
                flex-wrap: wrap;
            }
        }

        @media (max-width: 480px) {
            .dashboard-header-badge {
                display: none;
            }

            .section-heading-group {
                align-items: flex-start;
            }

            .section-icon {
                width: 38px;
                height: 38px;
                flex-basis: 38px;
            }

            .calendar-header {
                padding-top: 22px;
            }

            .fc .fc-toolbar-title {
                font-size: 1rem;
            }

            .fc .fc-daygrid-day-number {
                padding: 5px;
                font-size: 10px;
            }

            .fc .fc-col-header-cell-cushion {
                font-size: 10px;
            }

            .create-post-icon {
                display: none;
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

        <header class="student-dashboard-header">

            <div>

                <span class="dashboard-kicker">
                    Área do aluno
                </span>

                <h1>O meu dashboard</h1>

                <div class="dashboard-resumo">

                    <asp:Label
                        ID="LblResumo"
                        runat="server" />

                </div>

            </div>

            <div class="dashboard-header-badge">
                Visão geral da turma
            </div>

        </header>

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
                 ÁREA PRINCIPAL
            =================================================== -->

            <main class="dashboard-main">

                <!-- CALENDÁRIO -->

                <section class="dashboard-card calendar-card">

                    <div class="calendar-header">

                        <div class="section-header">

                            <div class="section-heading-group">

                                <div class="section-icon">
                                    ◫
                                </div>

                                <div>

                                    <h2>Calendário da turma</h2>

                                    <p class="section-description">
                                        Consulta testes, trabalhos, entregas
                                        e outros eventos escolares.
                                    </p>

                                </div>

                            </div>

                            <span class="calendar-status">
                                Calendário mensal
                            </span>

                        </div>

                    </div>

                    <div class="calendar-container">
                        <div id="calendar"></div>
                    </div>

                </section>

                <!-- TRABALHOS E NOTAS -->

                <div class="dashboard-secondary-grid">

                    <!-- TRABALHOS E TESTES -->

                    <section class="dashboard-card information-card">

                        <div class="dashboard-card-body">

                            <div class="information-card-header">

                                <div class="information-card-icon">
                                    ✓
                                </div>

                                <div>

                                    <h2 class="information-card-title">
                                        Trabalhos e testes
                                    </h2>

                                    <p class="information-card-description">
                                        Atividades e entregas da turma
                                    </p>

                                </div>

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

                    </section>

                    <!-- NOTAS -->

                    <section class="dashboard-card information-card">

                        <div class="dashboard-card-body">

                            <div class="information-card-header">

                                <div class="information-card-icon">
                                    ★
                                </div>

                                <div>

                                    <h2 class="information-card-title">
                                        Notas e feedback
                                    </h2>

                                    <p class="information-card-description">
                                        Avaliações e comentários recebidos
                                    </p>

                                </div>

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

                    </section>

                </div>

            </main>

            <!-- ==================================================
                 ÁREA SECUNDÁRIA
            =================================================== -->

            <aside class="dashboard-side">

                <!-- CRIAR PUBLICAÇÃO -->

                <section class="dashboard-card create-post-card">

                    <div class="dashboard-card-body">

                        <span class="create-post-label">
                            Partilhar com a turma
                        </span>

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

                </section>

                <!-- FEED -->

                <section class="feed-section">

                    <div class="feed-heading">

                        <div>

                            <h2>Feed da turma</h2>

                            <p class="section-description">
                                Publicações partilhadas pela turma.
                            </p>

                        </div>

                        <span class="feed-label">
                            Atualizações
                        </span>

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

                    <asp:Label
                        ID="LblPrazoEntrega"
                        runat="server"
                        CssClass="d-inline-block mt-2" />

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

                <!-- HISTÓRICO DE SUBMISSÕES -->

                <div class="mb-4">

                    <h3
                        style="
                            margin-bottom: 4px;
                            color: #1e293b;
                            font-size: 18px;
                            font-weight: 800;">

                        As minhas submissões

                    </h3>

                    <p class="text-muted mb-3">

                        Podes enviar mais do que uma versão
                        enquanto o prazo estiver aberto.

                    </p>

                    <div class="table-wrapper">

                        <asp:GridView
                            ID="GridSubmissoes"
                            runat="server"
                            AutoGenerateColumns="false"
                            DataKeyNames="Id"
                            GridLines="None"
                            CssClass="table table-hover dashboard-table"
                            EmptyDataText="Ainda não fizeste nenhuma submissão."
                            OnRowCommand="GridSubmissoes_RowCommand">

                            <Columns>

                                <asp:BoundField
                                    DataField="CreatedAt"
                                    HeaderText="Enviado em"
                                    DataFormatString="{0:dd/MM/yyyy HH:mm}"
                                    HtmlEncode="false" />

                                <asp:TemplateField HeaderText="Ficheiro">

                                    <ItemTemplate>

                                        <asp:HyperLink
                                            ID="LinkFicheiroSubmissao"
                                            runat="server"
                                            Text='<%# Eval("NomeFicheiro") %>'
                                            NavigateUrl='<%# Eval("CaminhoFicheiro") %>'
                                            Target="_blank"
                                            CssClass="btn btn-outline-primary btn-sm" />

                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:BoundField
                                    DataField="Observacao"
                                    HeaderText="Observação"
                                    NullDisplayText="—" />

                                <asp:TemplateField HeaderText="Ações">

                                    <ItemTemplate>

                                        <asp:LinkButton
                                            ID="BtnEliminarSubmissao"
                                            runat="server"
                                            Text="Eliminar"
                                            CommandName="EliminarSubmissao"
                                            CommandArgument='<%# Eval("Id") %>'
                                            CssClass="btn btn-outline-danger btn-sm"
                                            CausesValidation="false"
                                            Visible='<%# Convert.ToBoolean(Eval("PodeEliminar")) %>'
                                            OnClientClick="return confirm('Tens a certeza de que queres eliminar esta submissão?');" />

                                    </ItemTemplate>

                                </asp:TemplateField>

                            </Columns>

                        </asp:GridView>

                    </div>

                </div>

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
                        id="modalPublicacaoLabel">

                        Criar publicação

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

                        <label
                            class="form-label"
                            for="<%= DdlTipoPublicacao.ClientID %>">

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

                        <label
                            class="form-label"
                            for="<%= TxtTituloPublicacao.ClientID %>">

                            Título

                        </label>

                        <asp:TextBox
                            ID="TxtTituloPublicacao"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="200" />

                    </div>

                    <div class="mb-3">

                        <label
                            class="form-label"
                            for="<%= TxtConteudoPublicacao.ClientID %>">

                            Conteúdo

                        </label>

                        <asp:TextBox
                            ID="TxtConteudoPublicacao"
                            runat="server"
                            CssClass="form-control"
                            TextMode="MultiLine"
                            Rows="5"
                            MaxLength="5000" />

                    </div>

                    <div class="mb-3">

                        <label
                            class="form-label"
                            for="<%= FilePublicacao.ClientID %>">

                            Anexo

                        </label>

                        <asp:FileUpload
                            ID="FilePublicacao"
                            runat="server"
                            CssClass="form-control" />

                    </div>

                    <!-- VISIBILIDADE DA PUBLICAÇÃO -->

                    <div class="audience-card">

                        <div class="form-check mb-3">

                            <asp:CheckBox
                                ID="ChkPublicaTurma"
                                runat="server"
                                Text=" Visível para toda a turma" />

                        </div>

                        <p class="audience-help">

                            Ao marcar esta opção, todos os alunos
                            e professores da turma poderão visualizar
                            a publicação.

                        </p>

                        <div
                            id="painelDestinatariosPublicacao"
                            class="audience-selector">

                            <div class="audience-selector-header">

                                <h6>
                                    Escolher destinatários específicos
                                </h6>

                                <p>

                                    Quando a publicação não for para toda
                                    a turma, seleciona quem poderá visualizá-la.

                                </p>

                            </div>

                            <div class="row g-3">

                                <!-- ALUNOS -->

                                <div class="col-md-6">

                                    <div class="recipient-group">

                                        <div class="recipient-group-title recipient-group-header">

                                            <span>
                                                Alunos
                                            </span>

                                            <label class="select-all-option">

                                                <input
                                                    type="checkbox"
                                                    id="chkTodosAlunos"
                                                    data-target-list="<%= CblAlunosDestinatarios.ClientID %>" />

                                                <span>
                                                    Selecionar todos os alunos
                                                </span>

                                            </label>

                                        </div>

                                        <div class="recipient-list-container">

                                            <asp:CheckBoxList
                                                ID="CblAlunosDestinatarios"
                                                runat="server"
                                                CssClass="recipient-list"
                                                RepeatLayout="Flow"
                                                RepeatDirection="Vertical">
                                            </asp:CheckBoxList>

                                            <asp:Label
                                                ID="LblSemAlunosDestinatarios"
                                                runat="server"
                                                Text="Não existem outros alunos na turma."
                                                CssClass="text-muted small"
                                                Visible="false" />

                                        </div>

                                    </div>

                                </div>

                                <!-- PROFESSORES -->

                                <div class="col-md-6">

                                    <div class="recipient-group">

                                        <div class="recipient-group-title recipient-group-header">

                                            <span>
                                                Professores
                                            </span>

                                            <label class="select-all-option">

                                                <input
                                                    type="checkbox"
                                                    id="chkTodosProfessores"
                                                    data-target-list="<%= CblProfessoresDestinatarios.ClientID %>" />

                                                <span>
                                                    Selecionar todos os professores
                                                </span>

                                            </label>

                                        </div>

                                        <div class="recipient-list-container">

                                            <asp:CheckBoxList
                                                ID="CblProfessoresDestinatarios"
                                                runat="server"
                                                CssClass="recipient-list"
                                                RepeatLayout="Flow"
                                                RepeatDirection="Vertical">
                                            </asp:CheckBoxList>

                                            <asp:Label
                                                ID="LblSemProfessoresDestinatarios"
                                                runat="server"
                                                Text="Não existem professores associados à turma."
                                                CssClass="text-muted small"
                                                Visible="false" />

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
         JAVASCRIPT DO CALENDÁRIO
    =================================================== -->

    <script>
        document.addEventListener(
            "DOMContentLoaded",
            function () {

                var calendarElement =
                    document.getElementById(
                        "calendar"
                    );

                if (!calendarElement) {
                    return;
                }

                var eventsField =
                    document.getElementById(
                        "<%= HdnEvents.ClientID %>"
                    );

                var events = [];

                try {

                    events =
                        JSON.parse(
                            eventsField.value || "[]"
                        );

                } catch (error) {

                    console.error(
                        "Erro ao carregar os eventos do calendário:",
                        error
                    );

                }

                var calendar =
                    new FullCalendar.Calendar(
                        calendarElement,
                        {
                            initialView:
                                "dayGridMonth",

                            locale:
                                "pt",

                            height:
                                "auto",

                            aspectRatio:
                                1.5,

                            dayMaxEventRows:
                                3,

                            navLinks:
                                false,

                            fixedWeekCount:
                                true,

                            headerToolbar:
                            {
                                left:
                                    "prev,next",

                                center:
                                    "title",

                                right:
                                    "today"
                            },

                            buttonText:
                            {
                                today:
                                    "Hoje"
                            },

                            eventTimeFormat:
                            {
                                hour:
                                    "2-digit",

                                minute:
                                    "2-digit",

                                hour12:
                                    false
                            },

                            events:
                                events
                        }
                    );

                calendar.render();
            }
        );
    </script>

    <!-- ==================================================
         VISIBILIDADE E SELEÇÃO DOS DESTINATÁRIOS
    =================================================== -->

    <script>
        document.addEventListener(
            "DOMContentLoaded",
            function () {

                var checkboxTurma =
                    document.getElementById(
                        "<%= ChkPublicaTurma.ClientID %>"
                    );

                var painelDestinatarios =
                    document.getElementById(
                        "painelDestinatariosPublicacao"
                    );

                function atualizarDestinatarios() {

                    if (
                        !checkboxTurma ||
                        !painelDestinatarios
                    ) {
                        return;
                    }

                    if (checkboxTurma.checked) {

                        painelDestinatarios
                            .classList
                            .add(
                                "is-hidden"
                            );

                    } else {

                        painelDestinatarios
                            .classList
                            .remove(
                                "is-hidden"
                            );

                    }

                }

                if (checkboxTurma) {

                    checkboxTurma.addEventListener(
                        "change",
                        atualizarDestinatarios
                    );

                    atualizarDestinatarios();

                }

                var seletoresTodos =
                    document.querySelectorAll(
                        "[data-target-list]"
                    );

                seletoresTodos.forEach(
                    function (seletorTodos) {

                        var listaId =
                            seletorTodos.getAttribute(
                                "data-target-list"
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

                                seletorTodos.checked =
                                    false;

                                seletorTodos.indeterminate =
                                    false;

                                seletorTodos.disabled =
                                    true;

                                return;
                            }

                            seletorTodos.disabled =
                                false;

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
                            "change",
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
                            "change",
                            atualizarEstadoPrincipal
                        );

                        atualizarEstadoPrincipal();

                    }
                );

            }
        );
    </script>

</asp:Content>