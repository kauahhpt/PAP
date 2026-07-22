<%@ Page
    Title="Dashboard do Agrupamento"
    Language="C#"
    MasterPageFile="~/agrupamento/modeloAgrupamento.Master"
    AutoEventWireup="true"
    CodeBehind="dashboard.aspx.cs"
    Inherits="AlunoGest.agrupamento.dashboard" %>


<asp:Content
    ID="ContentHead"
    ContentPlaceHolderID="headContent"
    runat="server">

    <style>

        /* =====================================================
           ESTRUTURA
        ===================================================== */

        .dashboard-page {
            width: 100%;
            max-width: 1500px;
            margin: 0 auto;
            padding-bottom: 40px;
        }

        .dashboard-header {
            display: flex;
            align-items: flex-start;
            justify-content: space-between;
            gap: 20px;
            margin-bottom: 24px;
            padding: 25px;
            border: 1px solid #dbe3ed;
            border-radius: 16px;
            background: linear-gradient( 135deg, #ffffff, #f4f8ff );
            box-shadow: 0 5px 20px rgba(15, 23, 42, 0.06);
        }

        .dashboard-kicker {
            display: block;
            margin-bottom: 7px;
            color: #2563eb;
            font-size: 12px;
            font-weight: 800;
            letter-spacing: 0.08em;
            text-transform: uppercase;
        }

        .dashboard-header h1 {
            margin: 0 0 7px;
            color: #1e293b;
            font-size: 29px;
            font-weight: 800;
        }

        .dashboard-header p {
            max-width: 730px;
            margin: 0;
            color: #64748b;
            font-size: 14px;
            line-height: 1.6;
        }

        .ano-letivo-box {
            min-width: 185px;
            padding: 13px 16px;
            border: 1px solid #bfdbfe;
            border-radius: 11px;
            background: #eff6ff;
        }

        .ano-letivo-label {
            display: block;
            margin-bottom: 4px;
            color: #64748b;
            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;
        }

        .ano-letivo-value {
            color: #1d4ed8;
            font-size: 15px;
            font-weight: 800;
        }


        /* =====================================================
           ESTATÍSTICAS
        ===================================================== */

        .stats-grid {
            display: grid;
            grid-template-columns: repeat(5, minmax(0, 1fr));
            gap: 16px;
            margin-bottom: 22px;
        }

        .stat-card {
            position: relative;
            overflow: hidden;
            min-height: 145px;
            padding: 20px;
            border: 1px solid #dbe3ed;
            border-radius: 15px;
            background: #ffffff;
            text-decoration: none;
            box-shadow: 0 4px 16px rgba(15, 23, 42, 0.06);
            transition: transform 0.18s, box-shadow 0.18s, border-color 0.18s;
        }

            .stat-card:hover {
                transform: translateY(-3px);
                border-color: #93c5fd;
                box-shadow: 0 9px 24px rgba(15, 23, 42, 0.1);
            }

        .stat-icon {
            width: 42px;
            height: 42px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            margin-bottom: 16px;
            border-radius: 11px;
            background: #eff6ff;
            color: #2563eb;
            font-size: 13px;
            font-weight: 900;
        }

        .stat-value {
            display: block;
            margin-bottom: 4px;
            color: #1e293b;
            font-size: 28px;
            font-weight: 900;
            line-height: 1;
        }

        .stat-label {
            display: block;
            color: #64748b;
            font-size: 13px;
            font-weight: 700;
        }

        .stat-arrow {
            position: absolute;
            right: 17px;
            bottom: 16px;
            color: #94a3b8;
            font-size: 18px;
        }


        /* =====================================================
           CONTEÚDO PRINCIPAL
        ===================================================== */

        .dashboard-content-grid {
            display: grid;
            grid-template-columns: minmax(0, 1.25fr) minmax(330px, 0.75fr);
            gap: 21px;
            align-items: start;
        }

        .dashboard-column {
            display: grid;
            gap: 21px;
        }

        .dashboard-card {
            overflow: hidden;
            border: 1px solid #dbe3ed;
            border-radius: 15px;
            background: #ffffff;
            box-shadow: 0 4px 16px rgba(15, 23, 42, 0.06);
        }

        .dashboard-card-header {
            display: flex;
            align-items: flex-start;
            justify-content: space-between;
            gap: 15px;
            padding: 19px 21px;
            border-bottom: 1px solid #e5e7eb;
            background: #f8fafc;
        }

            .dashboard-card-header h2 {
                margin: 0 0 4px;
                color: #1e293b;
                font-size: 18px;
                font-weight: 800;
            }

            .dashboard-card-header p {
                margin: 0;
                color: #64748b;
                font-size: 12px;
                line-height: 1.5;
            }

        .dashboard-card-body {
            padding: 20px;
        }


        /* =====================================================
           ATENÇÃO NECESSÁRIA
        ===================================================== */

        .attention-list {
            display: grid;
            gap: 11px;
        }

        .attention-item {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 15px;
            padding: 15px;
            border: 1px solid #e2e8f0;
            border-radius: 11px;
            background: #ffffff;
            color: #334155;
            text-decoration: none;
            transition: border-color 0.16s, background 0.16s;
        }

            .attention-item:hover {
                border-color: #fbbf24;
                background: #fffbeb;
            }

        .attention-information {
            display: flex;
            align-items: center;
            gap: 12px;
        }

        .attention-icon {
            width: 37px;
            height: 37px;
            display: flex;
            align-items: center;
            justify-content: center;
            flex-shrink: 0;
            border-radius: 10px;
            background: #fff7ed;
            color: #ea580c;
            font-size: 17px;
            font-weight: 900;
        }

        .attention-title {
            display: block;
            margin-bottom: 2px;
            color: #334155;
            font-size: 13px;
            font-weight: 800;
        }

        .attention-description {
            display: block;
            color: #64748b;
            font-size: 11px;
            line-height: 1.4;
        }

        .attention-count {
            min-width: 38px;
            height: 30px;
            padding: 0 9px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            border-radius: 999px;
            background: #fff7ed;
            color: #c2410c;
            font-size: 13px;
            font-weight: 900;
        }


        /* =====================================================
           AÇÕES RÁPIDAS
        ===================================================== */

        .quick-actions {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 12px;
        }

        .quick-action {
            display: flex;
            align-items: center;
            gap: 12px;
            min-height: 72px;
            padding: 14px;
            border: 1px solid #e2e8f0;
            border-radius: 11px;
            background: #ffffff;
            color: #334155;
            text-decoration: none;
            transition: border-color 0.16s, background 0.16s, transform 0.16s;
        }

            .quick-action:hover {
                transform: translateY(-2px);
                border-color: #93c5fd;
                background: #f8fbff;
            }

        .quick-action-icon {
            width: 39px;
            height: 39px;
            display: flex;
            align-items: center;
            justify-content: center;
            flex-shrink: 0;
            border-radius: 10px;
            background: #eff6ff;
            color: #2563eb;
            font-size: 20px;
            font-weight: 800;
        }

        .quick-action-title {
            display: block;
            margin-bottom: 2px;
            color: #334155;
            font-size: 13px;
            font-weight: 800;
        }

        .quick-action-description {
            display: block;
            color: #64748b;
            font-size: 10px;
            line-height: 1.4;
        }


        /* =====================================================
           RESUMO
        ===================================================== */

        .summary-list {
            display: grid;
            gap: 13px;
        }

        .summary-row {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 15px;
            padding-bottom: 13px;
            border-bottom: 1px solid #edf2f7;
        }

            .summary-row:last-child {
                padding-bottom: 0;
                border-bottom: 0;
            }

        .summary-label {
            color: #64748b;
            font-size: 12px;
            font-weight: 600;
        }

        .summary-value {
            color: #1e293b;
            font-size: 13px;
            font-weight: 800;
        }


        /* =====================================================
           RESPONSIVO
        ===================================================== */

        @media (max-width: 1200px) {

            .stats-grid {
                grid-template-columns: repeat(3, minmax(0, 1fr));
            }
        }

        @media (max-width: 950px) {

            .dashboard-content-grid {
                grid-template-columns: 1fr;
            }
        }

        @media (max-width: 750px) {

            .dashboard-header {
                flex-direction: column;
            }

            .ano-letivo-box {
                width: 100%;
            }

            .stats-grid {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }
        }

        @media (max-width: 520px) {

            .stats-grid {
                grid-template-columns: 1fr;
            }

            .quick-actions {
                grid-template-columns: 1fr;
            }

            .dashboard-header {
                padding: 20px;
            }

            .dashboard-card-body {
                padding: 15px;
            }
        }

    </style>

</asp:Content>


<asp:Content
    ID="ContentMain"
    ContentPlaceHolderID="mainContent"
    runat="server">

    <div class="dashboard-page">


        <!-- ==================================================
             CABEÇALHO
        =================================================== -->

        <section class="dashboard-header">

            <div>

                <span class="dashboard-kicker">Administração do agrupamento
                </span>

                <h1>Bem-vindo,
                    <asp:Label
                        ID="LblAgrupamento"
                        runat="server"
                        Text="Agrupamento" />

                </h1>

                <p>
                    Consulte o resumo das escolas, turmas e utilizadores
                    associados ao agrupamento e aceda rapidamente às
                    principais áreas de gestão.
                </p>

            </div>

            <div class="ano-letivo-box">

                <span class="ano-letivo-label">Ano letivo ativo
                </span>

                <asp:Label
                    ID="LblAnoLetivo"
                    runat="server"
                    Text="Não definido"
                    CssClass="ano-letivo-value" />

            </div>

        </section>


        <!-- ==================================================
             CARTÕES DE ESTATÍSTICAS
        =================================================== -->

        <section class="stats-grid">


            <!-- ESCOLAS -->

            <asp:HyperLink
                ID="LnkEstatisticaEscolas"
                runat="server"
                NavigateUrl="~/agrupamento/escolas.aspx"
                CssClass="stat-card">

                <span class="stat-icon">ESC
                </span>

                <asp:Label
                    ID="LblTotalEscolas"
                    runat="server"
                    Text="0"
                    CssClass="stat-value" />

                <span class="stat-label">Escolas ativas
                </span>

                <span class="stat-arrow">→
                </span>

            </asp:HyperLink>


            <!-- TURMAS -->

            <asp:HyperLink
                ID="LnkEstatisticaTurmas"
                runat="server"
                NavigateUrl="~/agrupamento/turmas.aspx"
                CssClass="stat-card">

                <span class="stat-icon">TUR
                </span>

                <asp:Label
                    ID="LblTotalTurmas"
                    runat="server"
                    Text="0"
                    CssClass="stat-value" />

                <span class="stat-label">Turmas ativas
                </span>

                <span class="stat-arrow">→
                </span>

            </asp:HyperLink>


            <!-- ALUNOS -->

            <asp:HyperLink
                ID="LnkEstatisticaAlunos"
                runat="server"
                NavigateUrl="~/agrupamento/alunos.aspx"
                CssClass="stat-card">

                <span class="stat-icon">AL
                </span>

                <asp:Label
                    ID="LblTotalAlunos"
                    runat="server"
                    Text="0"
                    CssClass="stat-value" />

                <span class="stat-label">Alunos ativos
                </span>

                <span class="stat-arrow">→
                </span>

            </asp:HyperLink>


            <!-- PROFESSORES -->

            <asp:HyperLink
                ID="LnkEstatisticaProfessores"
                runat="server"
                NavigateUrl="~/agrupamento/professores.aspx"
                CssClass="stat-card">

                <span class="stat-icon">PROF
                </span>

                <asp:Label
                    ID="LblTotalProfessores"
                    runat="server"
                    Text="0"
                    CssClass="stat-value" />

                <span class="stat-label">Professores ativos
                </span>

                <span class="stat-arrow">→
                </span>

            </asp:HyperLink>


            <!-- ENCARREGADOS -->

            <asp:HyperLink
                ID="LnkEstatisticaEncarregados"
                runat="server"
                NavigateUrl="~/agrupamento/encarregados.aspx"
                CssClass="stat-card">

                <span class="stat-icon">EE
                </span>

                <asp:Label
                    ID="LblTotalEncarregados"
                    runat="server"
                    Text="0"
                    CssClass="stat-value" />

                <span class="stat-label">Encarregados ativos
                </span>

                <span class="stat-arrow">→
                </span>

            </asp:HyperLink>

        </section>


        <!-- ==================================================
             CONTEÚDO
        =================================================== -->

        <div class="dashboard-content-grid">


            <!-- COLUNA PRINCIPAL -->

            <div class="dashboard-column">


                <!-- ATENÇÃO NECESSÁRIA -->

                <asp:Panel
                    ID="PnlAtencaoNecessaria"
                    runat="server"
                    Visible="false"
                    CssClass="dashboard-card">

                    <header class="dashboard-card-header">

                        <div>

                            <h2>Atenção necessária
                            </h2>

                            <p>
                                Situações administrativas que poderão
                                precisar de intervenção.
                            </p>

                        </div>

                    </header>

                    <div class="dashboard-card-body">

                        <div class="attention-list">


                            <!-- ALUNOS SEM ENCARREGADO -->

                            <asp:HyperLink
                                ID="LnkAlunosSemEncarregado"
                                runat="server"
                                NavigateUrl="~/agrupamento/alunos.aspx"
                                CssClass="attention-item">

                                <span class="attention-information">

                                    <span class="attention-icon">!
                                    </span>

                                    <span>

                                        <span class="attention-title">Alunos sem encarregado associado
                                        </span>

                                        <span class="attention-description">Alunos ativos sem qualquer associação
                                            ativa a um encarregado.
                                        </span>

                                    </span>

                                </span>

                                <asp:Label
                                    ID="LblAlunosSemEncarregado"
                                    runat="server"
                                    Text="0"
                                    CssClass="attention-count" />

                            </asp:HyperLink>


                            <!-- ALUNOS SEM TURMA -->

                            <asp:HyperLink
                                ID="LnkAlunosSemTurma"
                                runat="server"
                                NavigateUrl="~/agrupamento/alunos.aspx"
                                CssClass="attention-item">

                                <span class="attention-information">

                                    <span class="attention-icon">!
                                    </span>

                                    <span>

                                        <span class="attention-title">Alunos sem turma ativa
                                        </span>

                                        <span class="attention-description">Alunos ativos que ainda não possuem
                                            uma matrícula ativa numa turma.
                                        </span>

                                    </span>

                                </span>

                                <asp:Label
                                    ID="LblAlunosSemTurma"
                                    runat="server"
                                    Text="0"
                                    CssClass="attention-count" />

                            </asp:HyperLink>


                            <!-- PROFESSORES SEM DISCIPLINA -->

                            <asp:HyperLink
                                ID="LnkProfessoresSemDisciplina"
                                runat="server"
                                NavigateUrl="~/agrupamento/professores.aspx"
                                CssClass="attention-item">

                                <span class="attention-information">

                                    <span class="attention-icon">!
                                    </span>

                                    <span>

                                        <span class="attention-title">Professores sem disciplina atribuída
                                        </span>

                                        <span class="attention-description">Professores ativos sem atribuições
                                            atuais a turmas e disciplinas.
                                        </span>

                                    </span>

                                </span>

                                <asp:Label
                                    ID="LblProfessoresSemDisciplina"
                                    runat="server"
                                    Text="0"
                                    CssClass="attention-count" />

                            </asp:HyperLink>


                            <!-- TURMAS SEM ALUNOS -->

                            <asp:HyperLink
                                ID="LnkTurmasSemAlunos"
                                runat="server"
                                NavigateUrl="~/agrupamento/turmas.aspx"
                                CssClass="attention-item">

                                <span class="attention-information">

                                    <span class="attention-icon">!
                                    </span>

                                    <span>

                                        <span class="attention-title">Turmas sem alunos
                                        </span>

                                        <span class="attention-description">Turmas ativas sem qualquer aluno
                                            atualmente matriculado.
                                        </span>

                                    </span>

                                </span>

                                <asp:Label
                                    ID="LblTurmasSemAlunos"
                                    runat="server"
                                    Text="0"
                                    CssClass="attention-count" />

                            </asp:HyperLink>

                        </div>

                    </div>

</asp:Panel>


                <!-- AÇÕES RÁPIDAS -->

                    <section class="dashboard-card">

                        <header class="dashboard-card-header">

                            <div>

                                <h2>Ações rápidas
                                </h2>

                                <p>
                                    Aceda diretamente às tarefas administrativas
                                mais utilizadas.
                                </p>

                            </div>

                        </header>

                        <div class="dashboard-card-body">

                            <div class="quick-actions">

                                <asp:HyperLink
                                    runat="server"
                                    NavigateUrl="~/agrupamento/escolas.aspx"
                                    CssClass="quick-action">

                                <span class="quick-action-icon">
                                    +
                                </span>

                                <span>

                                    <span class="quick-action-title">
                                        Gerir escolas
                                    </span>

                                    <span class="quick-action-description">
                                        Criar ou consultar escolas.
                                    </span>

                                </span>

                                </asp:HyperLink>


                                <asp:HyperLink
                                    runat="server"
                                    NavigateUrl="~/agrupamento/professores.aspx"
                                    CssClass="quick-action">

                                <span class="quick-action-icon">
                                    +
                                </span>

                                <span>

                                    <span class="quick-action-title">
                                        Adicionar professor
                                    </span>

                                    <span class="quick-action-description">
                                        Criar e gerir professores.
                                    </span>

                                </span>

                                </asp:HyperLink>


                                <asp:HyperLink
                                    runat="server"
                                    NavigateUrl="~/agrupamento/alunos.aspx"
                                    CssClass="quick-action">

                                <span class="quick-action-icon">
                                    +
                                </span>

                                <span>

                                    <span class="quick-action-title">
                                        Adicionar aluno
                                    </span>

                                    <span class="quick-action-description">
                                        Registar e gerir alunos.
                                    </span>

                                </span>

                                </asp:HyperLink>





                                <asp:HyperLink
                                    runat="server"
                                    NavigateUrl="~/agrupamento/encarregados.aspx"
                                    CssClass="quick-action">

                                <span class="quick-action-icon">
                                    +
                                </span>

                                <span>

                                    <span class="quick-action-title">
                                        Adicionar encarregado
                                    </span>

                                    <span class="quick-action-description">
                                        Registar e associar encarregados.
                                    </span>

                                </span>

                                </asp:HyperLink>

                            </div>

                        </div>

                    </section>
            </div>


            <!-- COLUNA LATERAL -->

            <aside class="dashboard-column">


                <!-- RESUMO -->

                <section class="dashboard-card">

                    <header class="dashboard-card-header">

                        <div>

                            <h2>Resumo do agrupamento
                            </h2>

                            <p>
                                Informação geral da estrutura atualmente ativa.
                            </p>

                        </div>

                    </header>

                    <div class="dashboard-card-body">

                        <div class="summary-list">

                            <div class="summary-row">

                                <span class="summary-label">Total de utilizadores
                                </span>

                                <asp:Label
                                    ID="LblTotalUtilizadores"
                                    runat="server"
                                    Text="0"
                                    CssClass="summary-value" />

                            </div>

                            <div class="summary-row">

                                <span class="summary-label">Escolas ativas
                                </span>

                                <asp:Label
                                    ID="LblResumoEscolas"
                                    runat="server"
                                    Text="0"
                                    CssClass="summary-value" />

                            </div>

                            <div class="summary-row">

                                <span class="summary-label">Turmas ativas
                                </span>

                                <asp:Label
                                    ID="LblResumoTurmas"
                                    runat="server"
                                    Text="0"
                                    CssClass="summary-value" />

                            </div>

                            <div class="summary-row">

                                <span class="summary-label">Alunos ativos
                                </span>

                                <asp:Label
                                    ID="LblResumoAlunos"
                                    runat="server"
                                    Text="0"
                                    CssClass="summary-value" />

                            </div>

                            <div class="summary-row">

                                <span class="summary-label">Professores ativos
                                </span>

                                <asp:Label
                                    ID="LblResumoProfessores"
                                    runat="server"
                                    Text="0"
                                    CssClass="summary-value" />

                            </div>

                            <div class="summary-row">

                                <span class="summary-label">Encarregados ativos
                                </span>

                                <asp:Label
                                    ID="LblResumoEncarregados"
                                    runat="server"
                                    Text="0"
                                    CssClass="summary-value" />

                            </div>

                        </div>

                    </div>

                </section>

            </aside>

        </div>

    </div>

</asp:Content>
