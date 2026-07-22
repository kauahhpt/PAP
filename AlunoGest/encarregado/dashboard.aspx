<%@ Page
    Title="Dashboard"
    Language="C#"
    MasterPageFile="~/encarregado/modeloEncarregado.Master"
    AutoEventWireup="true"
    CodeBehind="dashboard.aspx.cs"
    Inherits="AlunoGest.encarregado.dashboard"
    MaintainScrollPositionOnPostback="true" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="headContent"
    runat="server">

    <style>

        /* =====================================================
           ESTRUTURA
        ===================================================== */

        .dashboard-encarregado {
            padding-bottom: 40px;
        }

        .dashboard-header {
            display: flex;
            align-items: flex-start;
            justify-content: space-between;

            gap: 20px;

            margin-bottom: 25px;
        }

        .dashboard-header h1 {
            margin: 0 0 6px;

            color: #1f2937;

            font-size: 30px;
            font-weight: 800;
            letter-spacing: -0.02em;
        }

        .dashboard-header p {
            margin: 0;

            color: #64748b;

            font-size: 14px;
            line-height: 1.6;
        }

        .educando-badge {
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

        .educando-badge-dot {
            width: 8px;
            height: 8px;

            border-radius: 50%;

            background: #2563eb;
        }


        /* =====================================================
           MENSAGENS
        ===================================================== */

        .mensagem-erro,
        .mensagem-aviso,
        .mensagem-sucesso {
            display: block;

            margin-bottom: 20px;
            padding: 13px 15px;

            border-radius: 10px;

            font-size: 14px;
            line-height: 1.5;
        }

        .mensagem-erro {
            border: 1px solid #fecaca;

            background: #fef2f2;
            color: #b91c1c;
        }

        .mensagem-aviso {
            border: 1px solid #fde68a;

            background: #fffbeb;
            color: #92400e;
        }

        .mensagem-sucesso {
            border: 1px solid #bbf7d0;

            background: #f0fdf4;
            color: #166534;
        }


        /* =====================================================
           SEM EDUCANDO
        ===================================================== */

        .sem-educando-card {
            padding: 55px 25px;

            border: 1px solid #dbe3ed;
            border-radius: 15px;

            background: #ffffff;

            text-align: center;

            box-shadow:
                0 5px 18px rgba(15, 23, 42, 0.06);
        }

        .sem-educando-icon {
            width: 62px;
            height: 62px;

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

        .sem-educando-card h2 {
            margin: 0 0 7px;

            color: #334155;

            font-size: 20px;
            font-weight: 800;
        }

        .sem-educando-card p {
            max-width: 570px;

            margin: 0 auto;

            color: #64748b;

            font-size: 14px;
            line-height: 1.6;
        }


        /* =====================================================
           CARDS DE RESUMO
        ===================================================== */

        .resumo-grid {
            display: grid;

            grid-template-columns:
                repeat(4, minmax(0, 1fr));

            gap: 17px;

            margin-bottom: 22px;
        }

        .resumo-card {
            min-height: 125px;

            padding: 20px;

            border: 1px solid #dbe3ed;
            border-radius: 14px;

            background: #ffffff;

            box-shadow:
                0 4px 14px rgba(15, 23, 42, 0.05);
        }

        .resumo-label {
            display: block;

            margin-bottom: 9px;

            color: #64748b;

            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;

            letter-spacing: 0.04em;
        }

        .resumo-valor {
            display: block;

            color: #1f2937;

            font-size: 19px;
            font-weight: 800;

            line-height: 1.3;
        }

        .resumo-detalhe {
            display: block;

            margin-top: 5px;

            color: #94a3b8;

            font-size: 12px;
        }


        /* =====================================================
           ÁREA PRINCIPAL
        ===================================================== */

        .dashboard-grid {
            display: grid;

            grid-template-columns:
                minmax(0, 1.7fr)
                minmax(310px, 0.8fr);

            gap: 21px;

            align-items: start;
        }

        .dashboard-card {
            overflow: hidden;

            border: 1px solid #dbe3ed;
            border-radius: 15px;

            background: #ffffff;

            box-shadow:
                0 5px 18px rgba(15, 23, 42, 0.06);
        }

        .dashboard-card-header {
            display: flex;
            align-items: center;
            justify-content: space-between;

            gap: 14px;

            padding: 20px 21px;

            border-bottom: 1px solid #e5e7eb;

            background: #f8fafc;
        }

        .dashboard-card-header h2 {
            margin: 0;

            color: #1f2937;

            font-size: 18px;
            font-weight: 800;
        }

        .dashboard-card-header p {
            margin: 4px 0 0;

            color: #64748b;

            font-size: 12px;
        }

        .dashboard-card-body {
            padding: 21px;
        }


        /* =====================================================
           INFORMAÇÕES DO EDUCANDO
        ===================================================== */

        .perfil-educando {
            display: flex;
            align-items: center;

            gap: 17px;

            margin-bottom: 22px;
            padding-bottom: 21px;

            border-bottom: 1px solid #e5e7eb;
        }

        .educando-foto {
            width: 72px;
            height: 72px;

            flex-shrink: 0;

            border: 3px solid #dbeafe;
            border-radius: 50%;

            background: #eff6ff;

            object-fit: cover;
        }

        .educando-foto-placeholder {
            width: 72px;
            height: 72px;

            flex-shrink: 0;

            display: flex;
            align-items: center;
            justify-content: center;

            border: 3px solid #dbeafe;
            border-radius: 50%;

            background: #eff6ff;
            color: #2563eb;

            font-size: 24px;
            font-weight: 800;
        }

        .perfil-educando h3 {
            margin: 0 0 4px;

            color: #1f2937;

            font-size: 20px;
            font-weight: 800;
        }

        .perfil-educando p {
            margin: 0;

            color: #64748b;

            font-size: 13px;
        }

        .informacoes-grid {
            display: grid;

            grid-template-columns:
                repeat(2, minmax(0, 1fr));

            gap: 15px;
        }

        .informacao-item {
            padding: 14px;

            border: 1px solid #e2e8f0;
            border-radius: 10px;

            background: #f8fafc;
        }

        .informacao-label {
            display: block;

            margin-bottom: 5px;

            color: #64748b;

            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;

            letter-spacing: 0.04em;
        }

        .informacao-valor {
            display: block;

            color: #334155;

            font-size: 14px;
            font-weight: 700;

            word-break: break-word;
        }


        /* =====================================================
           EVENTOS
        ===================================================== */

        .eventos-grid {
            width: 100%;

            border-collapse: collapse;
        }

        .eventos-grid th {
            padding: 12px 13px;

            border-bottom: 1px solid #e5e7eb;

            background: #f8fafc;
            color: #475569;

            font-size: 11px;
            font-weight: 800;
            text-align: left;
            text-transform: uppercase;

            letter-spacing: 0.04em;
        }

        .eventos-grid td {
            padding: 13px;

            border-bottom: 1px solid #edf0f5;

            color: #334155;

            font-size: 13px;
            vertical-align: middle;
        }

        .eventos-grid tr:last-child td {
            border-bottom: 0;
        }

        .evento-titulo {
            color: #1f2937;

            font-weight: 700;
        }

        .evento-tipo {
            display: inline-flex;

            padding: 4px 8px;

            border: 1px solid #bfdbfe;
            border-radius: 999px;

            background: #eff6ff;
            color: #1d4ed8;

            font-size: 11px;
            font-weight: 700;
        }

        .evento-data {
            white-space: nowrap;
        }

        .empty-eventos {
            padding: 35px 20px;

            color: #64748b;

            text-align: center;
        }


        /* =====================================================
           PRÓXIMOS EVENTOS
        ===================================================== */

        .proximo-evento-item {
            padding: 14px 0;

            border-bottom: 1px solid #edf0f5;
        }

        .proximo-evento-item:first-child {
            padding-top: 0;
        }

        .proximo-evento-item:last-child {
            padding-bottom: 0;

            border-bottom: 0;
        }

        .proximo-evento-data {
            display: inline-flex;

            margin-bottom: 6px;
            padding: 4px 8px;

            border-radius: 7px;

            background: #eff6ff;
            color: #1d4ed8;

            font-size: 11px;
            font-weight: 800;
        }

        .proximo-evento-titulo {
            display: block;

            margin-bottom: 3px;

            color: #1f2937;

            font-size: 14px;
            font-weight: 750;
        }

        .proximo-evento-tipo {
            color: #64748b;

            font-size: 12px;
        }


        /* =====================================================
           RESPONSIVO
        ===================================================== */

        @media (max-width: 1100px) {

            .resumo-grid {
                grid-template-columns:
                    repeat(2, minmax(0, 1fr));
            }

            .dashboard-grid {
                grid-template-columns: 1fr;
            }
        }

        @media (max-width: 700px) {

            .dashboard-header {
                flex-direction: column;
            }

            .educando-badge {
                width: 100%;

                justify-content: center;
            }

            .resumo-grid {
                grid-template-columns: 1fr;
            }

            .informacoes-grid {
                grid-template-columns: 1fr;
            }

            .dashboard-card-header {
                align-items: flex-start;
                flex-direction: column;
            }
        }

        @media (max-width: 500px) {

            .dashboard-header h1 {
                font-size: 25px;
            }

            .perfil-educando {
                align-items: flex-start;
            }

            .educando-foto,
            .educando-foto-placeholder {
                width: 60px;
                height: 60px;
            }
        }

    </style>

</asp:Content>


<asp:Content
    ID="Content2"
    ContentPlaceHolderID="mainContent"
    runat="server">

    <div class="dashboard-encarregado">


        <!-- ==================================================
             CABEÇALHO
        =================================================== -->

        <div class="dashboard-header">

            <div>

                <h1>
                    Acompanhamento do educando
                </h1>

                <p>
                    Consulte as informações escolares e os próximos
                    acontecimentos do educando selecionado.
                </p>

            </div>

            <asp:Panel
                ID="PnlBadgeEducando"
                runat="server"
                CssClass="educando-badge"
                Visible="false">

                <span class="educando-badge-dot"></span>

                <asp:Label
                    ID="LblBadgeEducando"
                    runat="server" />

            </asp:Panel>

        </div>


        <!-- ==================================================
             MENSAGEM
        =================================================== -->

        <asp:Label
            ID="LblMensagem"
            runat="server"
            Visible="false" />


        <!-- ==================================================
             SEM EDUCANDO SELECIONADO
        =================================================== -->

        <asp:Panel
            ID="PnlSemEducando"
            runat="server"
            Visible="false"
            CssClass="sem-educando-card">

            <div class="sem-educando-icon">
                !
            </div>

            <h2>
                Nenhum educando disponível
            </h2>

            <p>
                Não existe um aluno ativo associado à sua conta.
                Contacte o agrupamento para verificar a associação.
            </p>

        </asp:Panel>


        <!-- ==================================================
             CONTEÚDO DO DASHBOARD
        =================================================== -->

        <asp:Panel
            ID="PnlDashboard"
            runat="server"
            Visible="false">


            <!-- CARDS DE RESUMO -->

            <div class="resumo-grid">

                <article class="resumo-card">

                    <span class="resumo-label">
                        Educando
                    </span>

                    <asp:Label
                        ID="LblResumoNome"
                        runat="server"
                        CssClass="resumo-valor" />

                    <span class="resumo-detalhe">
                        Aluno selecionado
                    </span>

                </article>


                <article class="resumo-card">

                    <span class="resumo-label">
                        Número de processo
                    </span>

                    <asp:Label
                        ID="LblResumoProcesso"
                        runat="server"
                        CssClass="resumo-valor" />

                    <span class="resumo-detalhe">
                        Identificação escolar
                    </span>

                </article>


                <article class="resumo-card">

                    <span class="resumo-label">
                        Turma atual
                    </span>

                    <asp:Label
                        ID="LblResumoTurma"
                        runat="server"
                        CssClass="resumo-valor"
                        Text="—" />

                    <asp:Label
                        ID="LblResumoAnoLetivo"
                        runat="server"
                        CssClass="resumo-detalhe"
                        Text="Ano letivo não definido" />

                </article>


                <article class="resumo-card">

                    <span class="resumo-label">
                        Próximos eventos
                    </span>

                    <asp:Label
                        ID="LblNumeroEventos"
                        runat="server"
                        CssClass="resumo-valor"
                        Text="0" />

                    <span class="resumo-detalhe">
                        A partir de hoje
                    </span>

                </article>

            </div>


            <!-- ÁREA PRINCIPAL -->

            <div class="dashboard-grid">


                <!-- COLUNA PRINCIPAL -->

                <div>


                    <!-- INFORMAÇÕES DO EDUCANDO -->

                    <section class="dashboard-card mb-4">

                        <header class="dashboard-card-header">

                            <div>

                                <h2>
                                    Informações do educando
                                </h2>

                                <p>
                                    Dados pessoais e escolares do aluno selecionado.
                                </p>

                            </div>

                        </header>

                        <div class="dashboard-card-body">

                            <div class="perfil-educando">

                                <asp:Image
                                    ID="ImgEducando"
                                    runat="server"
                                    CssClass="educando-foto"
                                    Visible="false"
                                    AlternateText="Fotografia do educando" />

                                <asp:Panel
                                    ID="PnlFotoPlaceholder"
                                    runat="server"
                                    CssClass="educando-foto-placeholder">

                                    <asp:Label
                                        ID="LblIniciaisEducando"
                                        runat="server" />

                                </asp:Panel>

                                <div>

                                    <h3>

                                        <asp:Label
                                            ID="LblNomeEducando"
                                            runat="server" />

                                    </h3>

                                    <p>

                                        <asp:Label
                                            ID="LblDescricaoEducando"
                                            runat="server"
                                            Text="Informações do aluno" />

                                    </p>

                                </div>

                            </div>


                            <div class="informacoes-grid">

                                <div class="informacao-item">

                                    <span class="informacao-label">
                                        Número de processo
                                    </span>

                                    <asp:Label
                                        ID="LblNumeroProcesso"
                                        runat="server"
                                        CssClass="informacao-valor"
                                        Text="—" />

                                </div>


                                <div class="informacao-item">

                                    <span class="informacao-label">
                                        Turma
                                    </span>

                                    <asp:Label
                                        ID="LblTurma"
                                        runat="server"
                                        CssClass="informacao-valor"
                                        Text="—" />

                                </div>


                                <div class="informacao-item">

                                    <span class="informacao-label">
                                        Email
                                    </span>

                                    <asp:Label
                                        ID="LblEmail"
                                        runat="server"
                                        CssClass="informacao-valor"
                                        Text="—" />

                                </div>


                                <div class="informacao-item">

                                    <span class="informacao-label">
                                        Telefone
                                    </span>

                                    <asp:Label
                                        ID="LblTelefone"
                                        runat="server"
                                        CssClass="informacao-valor"
                                        Text="—" />

                                </div>

                            </div>

                        </div>

                    </section>


                    <!-- EVENTOS -->

                    <section class="dashboard-card">

                        <header class="dashboard-card-header">

                            <div>

                                <h2>
                                    Trabalhos, testes e eventos
                                </h2>

                                <p>
                                    Próximos acontecimentos associados ao educando.
                                </p>

                            </div>

                        </header>

                        <div class="table-responsive">

                            <asp:GridView
                                ID="GridEventos"
                                runat="server"
                                AutoGenerateColumns="false"
                                CssClass="eventos-grid"
                                GridLines="None"
                                EmptyDataText="">

                                <Columns>

                                    <asp:TemplateField HeaderText="Título">

                                        <ItemTemplate>

                                            <span class="evento-titulo">

                                                <%# Eval("Titulo") %>

                                            </span>

                                        </ItemTemplate>

                                    </asp:TemplateField>


                                    <asp:TemplateField HeaderText="Tipo">

                                        <ItemTemplate>

                                            <span class="evento-tipo">

                                                <%# Eval("Tipo") %>

                                            </span>

                                        </ItemTemplate>

                                    </asp:TemplateField>


                                    <asp:BoundField
                                        DataField="DataHora"
                                        HeaderText="Data"
                                        DataFormatString="{0:dd/MM/yyyy HH:mm}"
                                        ItemStyle-CssClass="evento-data" />

                                </Columns>

                                <EmptyDataTemplate>

                                    <div class="empty-eventos">

                                        Não existem eventos futuros
                                        para o educando selecionado.

                                    </div>

                                </EmptyDataTemplate>

                            </asp:GridView>

                        </div>

                    </section>

                </div>


                <!-- COLUNA LATERAL -->

                <aside class="dashboard-card">

                    <header class="dashboard-card-header">

                        <div>

                            <h2>
                                Mais próximos
                            </h2>

                            <p>
                                Os primeiros acontecimentos agendados.
                            </p>

                        </div>

                    </header>

                    <div class="dashboard-card-body">

                        <asp:Repeater
                            ID="RptProximosEventos"
                            runat="server">

                            <ItemTemplate>

                                <article class="proximo-evento-item">

                                    <span class="proximo-evento-data">

                                        <%# Eval(
                                            "DataHora",
                                            "{0:dd/MM/yyyy HH:mm}"
                                        ) %>

                                    </span>

                                    <span class="proximo-evento-titulo">

                                        <%# Eval("Titulo") %>

                                    </span>

                                    <span class="proximo-evento-tipo">

                                        <%# Eval("Tipo") %>

                                    </span>

                                </article>

                            </ItemTemplate>

                        </asp:Repeater>


                        <asp:Panel
                            ID="PnlSemProximosEventos"
                            runat="server"
                            Visible="false"
                            CssClass="empty-eventos">

                            Não existem eventos agendados.

                        </asp:Panel>

                    </div>

                </aside>

            </div>

        </asp:Panel>

    </div>

</asp:Content>