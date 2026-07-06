<%@ Page Title="Minha Turma"
    Language="C#"
    MasterPageFile="~/aluno/MasterAluno1.Master"
    AutoEventWireup="true"
    CodeBehind="MinhaTurma.aspx.cs"
    Inherits="AlunoGest.aluno.MinhaTurma" %>

<asp:Content
    ID="Head"
    ContentPlaceHolderID="headContent"
    runat="server">

    <style>
        .turma-page {
            width: calc(100% - 40px);
            max-width: 1400px;
            margin: 0 auto;
            text-align: left;
        }

        .turma-header {
            margin-bottom: 24px;
        }

        .turma-header h1 {
            margin: 0 0 5px 0;
            font-size: 30px;
            font-weight: 700;
            color: #1f2937;
        }

        .turma-resumo {
            color: #64748b;
            font-size: 15px;
        }

        .turma-stats {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 220px));
            gap: 16px;
            margin-bottom: 30px;
        }

        .stat-card {
            background: #ffffff;
            border: 1px solid rgba(15, 23, 42, 0.08);
            border-radius: 14px;
            padding: 20px;
            box-shadow: 0 2px 8px rgba(15, 23, 42, 0.06);
        }

        .stat-value {
            font-size: 28px;
            font-weight: 800;
            color: #123570;
        }

        .stat-label {
            font-size: 14px;
            color: #64748b;
            margin-top: 3px;
        }

        .turma-section {
            margin-bottom: 35px;
        }

        .section-title {
            font-size: 21px;
            font-weight: 700;
            color: #1f2937;
            margin-bottom: 16px;
        }

        .pessoas-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
            gap: 16px;
        }

        .pessoa-card {
            background: #ffffff;
            border: 1px solid rgba(15, 23, 42, 0.08);
            border-radius: 14px;
            padding: 18px;
            box-shadow: 0 2px 8px rgba(15, 23, 42, 0.05);

            display: flex;
            align-items: center;
            gap: 14px;

            transition: transform 0.2s, box-shadow 0.2s;
        }

        .pessoa-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 18px rgba(15, 23, 42, 0.10);
        }

        .pessoa-avatar {
            width: 52px;
            height: 52px;

            min-width: 52px;
            min-height: 52px;

            max-width: 52px;
            max-height: 52px;

            border-radius: 50%;
            overflow: hidden;

            display: flex;
            align-items: center;
            justify-content: center;

            background: linear-gradient(
                135deg,
                #123570,
                #2563eb
            );

            color: #ffffff;
            font-size: 20px;
            font-weight: 700;

            flex: 0 0 52px;
        }

        .pessoa-avatar img {
            width: 52px !important;
            height: 52px !important;

            min-width: 52px;
            min-height: 52px;

            max-width: 52px !important;
            max-height: 52px !important;

            object-fit: cover;
            border-radius: 50%;
            display: block;
        }

        .pessoa-info {
            min-width: 0;
        }

        .pessoa-nome {
            font-size: 16px;
            font-weight: 700;
            color: #1f2937;

            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        .pessoa-detalhe {
            color: #64748b;
            font-size: 13px;
            margin-top: 3px;
        }

        .empty-card {
            background: #ffffff;
            border: 1px solid #e2e8f0;
            border-radius: 14px;
            padding: 25px;

            color: #64748b;
            text-align: center;
        }

        @media (max-width: 700px) {
            .turma-page {
                width: calc(100% - 20px);
            }

            .turma-stats {
                grid-template-columns: 1fr 1fr;
            }

            .pessoas-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>

</asp:Content>


<asp:Content
    ID="Main"
    ContentPlaceHolderID="mainContent"
    runat="server">

    <div class="turma-page">

        <div class="turma-header">

            <h1>Minha Turma</h1>

            <div class="turma-resumo">
                <asp:Label
                    ID="LblResumoTurma"
                    runat="server" />
            </div>

        </div>


        <asp:Label
            ID="LblMensagem"
            runat="server"
            Visible="false" />


        <div class="turma-stats">

            <div class="stat-card">

                <div class="stat-value">
                    <asp:Label
                        ID="LblTotalAlunos"
                        runat="server"
                        Text="0" />
                </div>

                <div class="stat-label">
                    Alunos
                </div>

            </div>


            <div class="stat-card">

                <div class="stat-value">
                    <asp:Label
                        ID="LblTotalProfessores"
                        runat="server"
                        Text="0" />
                </div>

                <div class="stat-label">
                    Professores
                </div>

            </div>

        </div>


        <section class="turma-section">

            <div class="section-title">
                Alunos da turma
            </div>


            <div class="pessoas-grid">

                <asp:Repeater
                    ID="RepeaterAlunos"
                    runat="server">

                    <ItemTemplate>

                        <div class="pessoa-card">

                            <div class="pessoa-avatar">

                                <asp:Image
                                    ID="ImgAluno"
                                    runat="server"
                                    ImageUrl='<%# ObterFoto(Eval("Foto")) %>'
                                    Visible='<%# TemFoto(Eval("Foto")) %>' />

                                <asp:Literal
                                    ID="LitInicialAluno"
                                    runat="server"
                                    Text='<%# ObterInicial(Eval("NomeCompleto")) %>'
                                    Visible='<%# !TemFoto(Eval("Foto")) %>' />

                            </div>


                            <div class="pessoa-info">

                                <div class="pessoa-nome">
                                    <%# Eval("NomeCompleto") %>
                                </div>

                                <div class="pessoa-detalhe">
                                    Aluno
                                </div>

                            </div>

                        </div>

                    </ItemTemplate>

                </asp:Repeater>

            </div>


            <asp:Panel
                ID="PainelSemAlunos"
                runat="server"
                Visible="false"
                CssClass="empty-card">

                Não existem alunos associados à turma.

            </asp:Panel>

        </section>


        <section class="turma-section">

            <div class="section-title">
                Professores da turma
            </div>


            <div class="pessoas-grid">

                <asp:Repeater
                    ID="RepeaterProfessores"
                    runat="server">

                    <ItemTemplate>

                        <div class="pessoa-card">

                            <div class="pessoa-avatar">

                                <asp:Image
                                    ID="ImgProfessor"
                                    runat="server"
                                    ImageUrl='<%# ObterFoto(Eval("FotoPerfil")) %>'
                                    Visible='<%# TemFoto(Eval("FotoPerfil")) %>' />

                                <asp:Literal
                                    ID="LitInicialProfessor"
                                    runat="server"
                                    Text='<%# ObterInicial(Eval("Nome")) %>'
                                    Visible='<%# !TemFoto(Eval("FotoPerfil")) %>' />

                            </div>


                            <div class="pessoa-info">

                                <div class="pessoa-nome">
                                    <%# Eval("Nome") %>
                                </div>

                                <div class="pessoa-detalhe">
                                    <%# Eval("Disciplina") %>
                                </div>

                            </div>

                        </div>

                    </ItemTemplate>

                </asp:Repeater>

            </div>


            <asp:Panel
                ID="PainelSemProfessores"
                runat="server"
                Visible="false"
                CssClass="empty-card">

                Não existem professores associados à turma.

            </asp:Panel>

        </section>

    </div>

</asp:Content>