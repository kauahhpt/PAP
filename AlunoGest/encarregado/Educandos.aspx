<%@ Page
    Title="Educandos"
    Language="C#"
    MasterPageFile="~/encarregado/modeloEncarregado.Master"
    AutoEventWireup="true"
    CodeBehind="Educandos.aspx.cs"
    Inherits="AlunoGest.encarregado.Educandos"
    MaintainScrollPositionOnPostback="true" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="headContent"
    runat="server">

    <style>

        /* =====================================================
           ESTRUTURA
        ===================================================== */

        .educando-page {
            padding-bottom: 40px;
        }

        .page-header {
            display: flex;
            align-items: flex-start;
            justify-content: space-between;

            gap: 20px;

            margin-bottom: 25px;
        }

        .page-header h1 {
            margin: 0 0 6px;

            color: #1f2937;

            font-size: 30px;
            font-weight: 800;
            letter-spacing: -0.02em;
        }

        .page-header p {
            max-width: 760px;

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

        .educando-badge::before {
            width: 8px;
            height: 8px;

            border-radius: 50%;

            background: #2563eb;

            content: "";
        }


        /* =====================================================
           SEM EDUCANDO
        ===================================================== */

        .sem-educando {
            padding: 55px 24px;

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

            font-size: 26px;
            font-weight: 800;
        }

        .sem-educando h2 {
            margin: 0 0 7px;

            color: #334155;

            font-size: 20px;
            font-weight: 800;
        }

        .sem-educando p {
            max-width: 570px;

            margin: 0 auto;

            color: #64748b;

            font-size: 14px;
            line-height: 1.6;
        }


        /* =====================================================
           PERFIL
        ===================================================== */

        .perfil-card {
            display: flex;
            align-items: center;

            gap: 21px;

            margin-bottom: 21px;
            padding: 24px;

            border: 1px solid #dbe3ed;
            border-radius: 15px;

            background:
                linear-gradient(
                    135deg,
                    #ffffff,
                    #f8fbff
                );

            box-shadow:
                0 5px 18px rgba(15, 23, 42, 0.06);
        }

        .foto-educando {
            width: 105px;
            height: 105px;

            flex-shrink: 0;

            border: 4px solid #dbeafe;
            border-radius: 50%;

            background: #eff6ff;

            object-fit: cover;
        }

        .foto-placeholder {
            width: 105px;
            height: 105px;

            flex-shrink: 0;

            display: flex;
            align-items: center;
            justify-content: center;

            border: 4px solid #dbeafe;
            border-radius: 50%;

            background: #eff6ff;
            color: #2563eb;

            font-size: 31px;
            font-weight: 800;
        }

        .perfil-conteudo {
            flex: 1;
        }

        .perfil-conteudo h2 {
            margin: 0 0 5px;

            color: #1f2937;

            font-size: 24px;
            font-weight: 800;
        }

        .perfil-descricao {
            margin: 0 0 13px;

            color: #64748b;

            font-size: 14px;
        }

        .perfil-etiquetas {
            display: flex;
            flex-wrap: wrap;

            gap: 8px;
        }

        .etiqueta {
            display: inline-flex;
            align-items: center;

            padding: 5px 10px;

            border-radius: 999px;

            font-size: 11px;
            font-weight: 700;
        }

        .etiqueta-ativo {
            border: 1px solid #bbf7d0;

            background: #f0fdf4;
            color: #166534;
        }

        .etiqueta-inativo {
            border: 1px solid #e2e8f0;

            background: #f8fafc;
            color: #64748b;
        }

        .etiqueta-principal {
            border: 1px solid #bfdbfe;

            background: #eff6ff;
            color: #1d4ed8;
        }

        .etiqueta-secundario {
            border: 1px solid #e2e8f0;

            background: #f8fafc;
            color: #475569;
        }


        /* =====================================================
           GRID DE CONTEÚDO
        ===================================================== */

        .conteudo-grid {
            display: grid;

            grid-template-columns:
                repeat(2, minmax(0, 1fr));

            gap: 21px;
        }

        .dados-card {
            overflow: hidden;

            border: 1px solid #dbe3ed;
            border-radius: 15px;

            background: #ffffff;

            box-shadow:
                0 5px 18px rgba(15, 23, 42, 0.05);
        }

        .dados-card-full {
            grid-column: 1 / -1;
        }

        .dados-card-header {
            padding: 19px 21px;

            border-bottom: 1px solid #e5e7eb;

            background: #f8fafc;
        }

        .dados-card-header h2 {
            margin: 0 0 4px;

            color: #1f2937;

            font-size: 18px;
            font-weight: 800;
        }

        .dados-card-header p {
            margin: 0;

            color: #64748b;

            font-size: 12px;
            line-height: 1.5;
        }

        .dados-card-body {
            padding: 21px;
        }


        /* =====================================================
           CAMPOS
        ===================================================== */

        .informacoes-grid {
            display: grid;

            grid-template-columns:
                repeat(2, minmax(0, 1fr));

            gap: 14px;
        }

        .informacao-item {
            padding: 14px;

            border: 1px solid #e2e8f0;
            border-radius: 10px;

            background: #f8fafc;
        }

        .informacao-completa {
            grid-column: 1 / -1;
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
            line-height: 1.45;

            word-break: break-word;
        }


        /* =====================================================
           ASSOCIAÇÃO
        ===================================================== */

        .associacao-destaque {
            padding: 17px;

            border: 1px solid #dbeafe;
            border-radius: 11px;

            background: #f8fbff;
        }

        .associacao-destaque strong {
            display: block;

            margin-bottom: 5px;

            color: #1e3a8a;

            font-size: 14px;
        }

        .associacao-destaque p {
            margin: 0;

            color: #64748b;

            font-size: 13px;
            line-height: 1.6;
        }


        /* =====================================================
           RESPONSIVO
        ===================================================== */

        @media (max-width: 900px) {

            .conteudo-grid {
                grid-template-columns: 1fr;
            }

            .dados-card-full {
                grid-column: auto;
            }
        }

        @media (max-width: 650px) {

            .page-header {
                flex-direction: column;
            }

            .educando-badge {
                width: 100%;

                justify-content: center;
            }

            .perfil-card {
                align-items: flex-start;
            }

            .foto-educando,
            .foto-placeholder {
                width: 80px;
                height: 80px;
            }

            .foto-placeholder {
                font-size: 24px;
            }

            .perfil-conteudo h2 {
                font-size: 20px;
            }

            .informacoes-grid {
                grid-template-columns: 1fr;
            }

            .informacao-completa {
                grid-column: auto;
            }
        }

        @media (max-width: 470px) {

            .perfil-card {
                align-items: center;
                flex-direction: column;

                text-align: center;
            }

            .perfil-etiquetas {
                justify-content: center;
            }

            .page-header h1 {
                font-size: 25px;
            }
        }

    </style>

</asp:Content>


<asp:Content
    ID="Content2"
    ContentPlaceHolderID="mainContent"
    runat="server">

    <div class="educando-page">


        <!-- ==================================================
             CABEÇALHO
        =================================================== -->

        <div class="page-header">

            <div>

                <h1>
                    Educandos
                </h1>

                <p>
                    Consulte os dados pessoais, escolares e a relação
                    com o educando selecionado no menu superior.
                </p>

            </div>

            <asp:Label
                ID="LblEducandoSelecionado"
                runat="server"
                Visible="false"
                CssClass="educando-badge" />

        </div>


        <!-- ==================================================
             MENSAGEM
        =================================================== -->

        <asp:Label
            ID="LblMensagem"
            runat="server"
            Visible="false" />


        <!-- ==================================================
             SEM EDUCANDO
        =================================================== -->

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


        <!-- ==================================================
             CONTEÚDO
        =================================================== -->

        <asp:Panel
            ID="PnlConteudo"
            runat="server"
            Visible="false">


            <!-- PERFIL -->

            <section class="perfil-card">

                <asp:Image
                    ID="ImgEducando"
                    runat="server"
                    Visible="false"
                    CssClass="foto-educando"
                    AlternateText="Fotografia do educando" />

                <asp:Panel
                    ID="PnlFotoPlaceholder"
                    runat="server"
                    CssClass="foto-placeholder">

                    <asp:Label
                        ID="LblIniciais"
                        runat="server"
                        Text="?" />

                </asp:Panel>

                <div class="perfil-conteudo">

                    <h2>

                        <asp:Label
                            ID="LblNomeEducando"
                            runat="server" />

                    </h2>

                    <p class="perfil-descricao">

                        <asp:Label
                            ID="LblDescricaoEducando"
                            runat="server"
                            Text="Informações do aluno" />

                    </p>

                    <div class="perfil-etiquetas">

                        <asp:Label
                            ID="LblEstadoAluno"
                            runat="server"
                            Text="Ativo"
                            CssClass="etiqueta etiqueta-ativo" />

                        <asp:Label
                            ID="LblTipoEncarregado"
                            runat="server"
                            Text="Encarregado"
                            CssClass="etiqueta etiqueta-secundario" />

                    </div>

                </div>

            </section>


            <!-- DADOS -->

            <div class="conteudo-grid">


                <!-- DADOS PESSOAIS -->

                <section class="dados-card">

                    <header class="dados-card-header">

                        <h2>
                            Dados pessoais
                        </h2>

                        <p>
                            Informações pessoais e de contacto do aluno.
                        </p>

                    </header>

                    <div class="dados-card-body">

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
                                    NIF
                                </span>

                                <asp:Label
                                    ID="LblNIF"
                                    runat="server"
                                    CssClass="informacao-valor"
                                    Text="—" />

                            </div>

                            <div class="informacao-item informacao-completa">

                                <span class="informacao-label">
                                    Email
                                </span>

                                <asp:Label
                                    ID="LblEmail"
                                    runat="server"
                                    CssClass="informacao-valor"
                                    Text="—" />

                            </div>

                            <div class="informacao-item informacao-completa">

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


                <!-- DADOS ESCOLARES -->

                <section class="dados-card">

                    <header class="dados-card-header">

                        <h2>
                            Dados escolares
                        </h2>

                        <p>
                            Turma, escola e ano letivo atual.
                        </p>

                    </header>

                    <div class="dados-card-body">

                        <div class="informacoes-grid">

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
                                    Ano letivo
                                </span>

                                <asp:Label
                                    ID="LblAnoLetivo"
                                    runat="server"
                                    CssClass="informacao-valor"
                                    Text="—" />

                            </div>

                            <div class="informacao-item informacao-completa">

                                <span class="informacao-label">
                                    Escola
                                </span>

                                <asp:Label
                                    ID="LblEscola"
                                    runat="server"
                                    CssClass="informacao-valor"
                                    Text="—" />

                            </div>

                            <div class="informacao-item informacao-completa">

                                <span class="informacao-label">
                                    Agrupamento
                                </span>

                                <asp:Label
                                    ID="LblAgrupamento"
                                    runat="server"
                                    CssClass="informacao-valor"
                                    Text="—" />

                            </div>

                        </div>

                    </div>

                </section>


                <!-- RELAÇÃO COM O ENCARREGADO -->

                <section class="dados-card dados-card-full">

                    <header class="dados-card-header">

                        <h2>
                            Relação com o educando
                        </h2>

                        <p>
                            Informação registada pelo agrupamento
                            sobre a associação à sua conta.
                        </p>

                    </header>

                    <div class="dados-card-body">

                        <div class="informacoes-grid">

                            <div class="informacao-item">

                                <span class="informacao-label">
                                    Parentesco
                                </span>

                                <asp:Label
                                    ID="LblParentesco"
                                    runat="server"
                                    CssClass="informacao-valor"
                                    Text="Não definido" />

                            </div>

                            <div class="informacao-item">

                                <span class="informacao-label">
                                    Tipo de encarregado
                                </span>

                                <asp:Label
                                    ID="LblPrincipal"
                                    runat="server"
                                    CssClass="informacao-valor"
                                    Text="Encarregado associado" />

                            </div>

                            <div class="informacao-item">

                                <span class="informacao-label">
                                    Associação ativa
                                </span>

                                <asp:Label
                                    ID="LblAssociacaoAtiva"
                                    runat="server"
                                    CssClass="informacao-valor"
                                    Text="Sim" />

                            </div>

                            <div class="informacao-item">

                                <span class="informacao-label">
                                    Associado desde
                                </span>

                                <asp:Label
                                    ID="LblDataAssociacao"
                                    runat="server"
                                    CssClass="informacao-valor"
                                    Text="Não definido" />

                            </div>

                        </div>

                        <div class="associacao-destaque mt-3">

                            <strong>
                                Acesso protegido
                            </strong>

                            <p>
                                Apenas pode consultar os dados dos alunos
                                que se encontram associados à sua conta
                                pelo respetivo agrupamento.
                            </p>

                        </div>

                    </div>

                </section>

            </div>

        </asp:Panel>

    </div>

</asp:Content>