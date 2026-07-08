<%@ Page Title="As minhas turmas"
    Language="C#"
    MasterPageFile="~/professor/ModeloProfessor.Master"
    AutoEventWireup="true"
    CodeBehind="MinhasTurmas.aspx.cs"
    Inherits="AlunoGest.professor.dashboard" %>

<asp:Content
    ID="Title"
    ContentPlaceHolderID="titleContent"
    runat="server">

    As minhas turmas

</asp:Content>


<asp:Content
    ID="Head"
    ContentPlaceHolderID="headContent"
    runat="server">

    <style>
        .turmas-page {
            width: 100%;
            max-width: 1500px;
            margin: 0 auto;
            text-align: left;
        }

        .turmas-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            flex-wrap: wrap;
            gap: 20px;
            margin-bottom: 24px;
        }

        .turmas-header h1 {
            margin: 0 0 5px 0;
            color: #1f2937;
            font-size: 30px;
            font-weight: 800;
        }

        .turmas-header p {
            margin: 0;
            color: #64748b;
            font-size: 15px;
        }

        .pesquisa-area {
            width: 100%;
            max-width: 320px;
        }

        .pesquisa-area .form-control {
            min-height: 44px;
            border-radius: 10px;
            border-color: #d8dee9;
        }

        .pesquisa-area .form-control:focus {
            border-color: #2563eb;
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.12);
        }

        /* =====================================================
           ESTATÍSTICAS
        ===================================================== */

        .stats-grid {
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            gap: 18px;
            margin-bottom: 24px;
        }

        .stat-card-custom {
            position: relative;
            overflow: hidden;
            padding: 22px;
            background: #ffffff;
            border: 1px solid rgba(15, 23, 42, 0.08);
            border-radius: 14px;
            box-shadow: 0 2px 8px rgba(15, 23, 42, 0.06);
        }

        .stat-card-custom::before {
            content: "";
            position: absolute;
            top: 0;
            left: 0;
            width: 4px;
            height: 100%;
            background: #2563eb;
        }

        .stat-label {
            margin-bottom: 6px;
            color: #64748b;
            font-size: 14px;
            font-weight: 600;
        }

        .stat-value {
            color: #123570;
            font-size: 30px;
            font-weight: 800;
            line-height: 1;
        }

        /* =====================================================
           CARD PRINCIPAL
        ===================================================== */

        .turmas-card {
            background: #ffffff;
            border: 1px solid rgba(15, 23, 42, 0.08);
            border-radius: 14px;
            box-shadow: 0 2px 8px rgba(15, 23, 42, 0.06);
            overflow: hidden;
        }

        .turmas-card-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 15px;
            padding: 20px 22px;
            border-bottom: 1px solid #edf0f5;
        }

        .turmas-card-header h2 {
            margin: 0;
            color: #1f2937;
            font-size: 20px;
            font-weight: 750;
        }

        .turmas-card-header p {
            margin: 4px 0 0 0;
            color: #64748b;
            font-size: 13px;
        }

        /* =====================================================
           TABELA
        ===================================================== */

        .table-wrapper {
            width: 100%;
            overflow-x: auto;
        }

        .turmas-table {
            width: 100%;
            min-width: 900px;
            margin-bottom: 0;
        }

        .turmas-table th {
            padding: 14px 16px;
            background: #f8fafc;
            color: #475569;
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.03em;
            white-space: nowrap;
            border-bottom: 1px solid #e2e8f0;
        }

        .turmas-table td {
            padding: 15px 16px;
            color: #334155;
            font-size: 14px;
            vertical-align: middle;
        }

        .turmas-table tbody tr {
            transition: background 0.2s;
        }

        .turmas-table tbody tr:hover {
            background: #f8fafc;
        }

        .turma-nome {
            font-weight: 700;
            color: #1f2937;
        }

        .actions-area {
            display: flex;
            align-items: center;
            gap: 7px;
            flex-wrap: nowrap;
        }

        .btn-ver-turma {
            background: #2563eb;
            border-color: #2563eb;
            color: #ffffff;
            white-space: nowrap;
        }

        .btn-ver-turma:hover {
            background: #1d4ed8;
            border-color: #1d4ed8;
            color: #ffffff;
        }

        .btn-home-turma {
            white-space: nowrap;
        }

        /* =====================================================
           RESPONSIVO
        ===================================================== */

        @media (max-width: 900px) {
            .stats-grid {
                grid-template-columns: repeat(3, 1fr);
            }
        }

        @media (max-width: 700px) {
            .turmas-header {
                flex-direction: column;
            }

            .pesquisa-area {
                max-width: 100%;
            }

            .stats-grid {
                grid-template-columns: 1fr;
            }

            .turmas-card-header {
                padding: 16px;
            }

            .stat-card-custom {
                padding: 18px;
            }
        }
    </style>

</asp:Content>


<asp:Content
    ID="Main"
    ContentPlaceHolderID="mainContent"
    runat="server">

    <div class="turmas-page">

        <!-- ==================================================
             CABEÇALHO
        =================================================== -->

        <div class="turmas-header">

            <div>

                <h1>
                    As minhas turmas
                </h1>

                <p>
                    Consulte as turmas e disciplinas que lhe estão atribuídas.
                </p>

            </div>


            <div class="pesquisa-area">

                <asp:TextBox
                    ID="TxtPesquisa"
                    runat="server"
                    CssClass="form-control"
                    placeholder="Pesquisar turma ou disciplina"
                    AutoPostBack="true"
                    OnTextChanged="TxtPesquisa_TextChanged" />

            </div>

        </div>


        <!-- ==================================================
             MENSAGEM
        =================================================== -->

        <asp:Label
            ID="LblMensagem"
            runat="server"
            Visible="false"
            CssClass="alert alert-warning d-block" />


        <!-- ==================================================
             ESTATÍSTICAS
        =================================================== -->

        <div class="stats-grid">

            <div class="stat-card-custom">

                <div class="stat-label">
                    Turmas atribuídas
                </div>

                <div class="stat-value">

                    <asp:Label
                        ID="LblTotalTurmas"
                        runat="server" />

                </div>

            </div>


            <div class="stat-card-custom">

                <div class="stat-label">
                    Alunos abrangidos
                </div>

                <div class="stat-value">

                    <asp:Label
                        ID="LblTotalAlunos"
                        runat="server" />

                </div>

            </div>


            <div class="stat-card-custom">

                <div class="stat-label">
                    Atividades futuras
                </div>

                <div class="stat-value">

                    <asp:Label
                        ID="LblTotalAtividades"
                        runat="server" />

                </div>

            </div>

        </div>


        <!-- ==================================================
             LISTA DE TURMAS
        =================================================== -->

        <section class="turmas-card">

            <div class="turmas-card-header">

                <div>

                    <h2>
                        Turmas atribuídas
                    </h2>

                    <p>
                        Aceda aos membros da turma ou ao respetivo feed e calendário.
                    </p>

                </div>

            </div>


            <div class="table-wrapper">

                <asp:GridView
                    ID="GridTurmas"
                    runat="server"
                    AutoGenerateColumns="false"
                    CssClass="table table-hover align-middle turmas-table"
                    GridLines="None"
                    EmptyDataText="Não existem turmas atribuídas."
                    DataKeyNames="TurmaId">

                    <Columns>

                        <asp:TemplateField
                            HeaderText="Turma">

                            <ItemTemplate>

                                <span class="turma-nome">
                                    <%# Eval("Turma") %>
                                </span>

                            </ItemTemplate>

                        </asp:TemplateField>


                        <asp:BoundField
                            DataField="Escola"
                            HeaderText="Escola" />


                        <asp:BoundField
                            DataField="AnoLetivo"
                            HeaderText="Ano letivo" />


                        <asp:BoundField
                            DataField="Disciplinas"
                            HeaderText="Disciplinas" />


                        <asp:BoundField
                            DataField="TotalAlunos"
                            HeaderText="Alunos" />


                        <asp:TemplateField
                            HeaderText="Ações">

                            <ItemTemplate>

                                <div class="actions-area">

                                    <a
                                        class="btn btn-primary btn-sm btn-ver-turma"
                                        href='turma.aspx?id=<%# Eval("TurmaId") %>'>

                                        Ver turma

                                    </a>


                                    <a
                                        class="btn btn-outline-primary btn-sm btn-home-turma"
                                        href='Home.aspx?turma=<%# Eval("TurmaId") %>'>

                                        Feed e calendário

                                    </a>

                                </div>

                            </ItemTemplate>

                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>

        </section>

    </div>

</asp:Content>