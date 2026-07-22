<%@ Page
    Title="Mensagens"
    Language="C#"
    MasterPageFile="~/professor/ModeloProfessor.Master"
    AutoEventWireup="true"
    CodeBehind="Mensagens.aspx.cs"
    Inherits="AlunoGest.professor.Mensagens"
    MaintainScrollPositionOnPostback="true" %>


<asp:Content
    ID="ContentTitle"
    ContentPlaceHolderID="titleContent"
    runat="server">
    Mensagens

</asp:Content>


<asp:Content
    ID="ContentHead"
    ContentPlaceHolderID="headContent"
    runat="server">

    <style>
        /* =====================================================
           ESTRUTURA
        ===================================================== */

        .mensagens-page {
            width: 100%;
            max-width: 1500px;
            margin: 0 auto;
            padding-bottom: 40px;
            text-align: left;
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
                max-width: 760px;
                margin: 0;
                color: #64748b;
                font-size: 14px;
                line-height: 1.6;
            }

        .professor-badge {
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

            .professor-badge::before {
                width: 8px;
                height: 8px;
                border-radius: 50%;
                background: #2563eb;
                content: "";
            }


        /* =====================================================
           CARDS
        ===================================================== */

        .chat-card {
            overflow: hidden;
            border: 1px solid #dbe3ed;
            border-radius: 15px;
            background: #ffffff;
            box-shadow: 0 5px 18px rgba(15, 23, 42, 0.06);
        }

        .chat-card-header {
            padding: 18px 20px;
            border-bottom: 1px solid #e5e7eb;
            background: #f8fafc;
        }

            .chat-card-header h2 {
                margin: 0 0 4px;
                color: #1f2937;
                font-size: 18px;
                font-weight: 800;
            }

            .chat-card-header p {
                margin: 0;
                color: #64748b;
                font-size: 12px;
                line-height: 1.5;
            }

        .chat-card-body {
            padding: 20px;
        }


        /* =====================================================
           SELEÇÃO
        ===================================================== */

        .selecao-card {
            margin-bottom: 21px;
        }

        .selecao-grid {
            display: grid;
            grid-template-columns: minmax(190px, 0.8fr) minmax(180px, 0.7fr) minmax(260px, 1.2fr) auto;
            gap: 14px;
            align-items: end;
        }

        .form-label {
            display: block;
            margin-bottom: 6px;
            color: #334155;
            font-size: 13px;
            font-weight: 700;
        }

        .campo-ajuda {
            display: block;
            margin-top: 6px;
            color: #64748b;
            font-size: 11px;
            line-height: 1.5;
        }

        .btn-iniciar {
            min-height: 38px;
            white-space: nowrap;
        }


        /* =====================================================
           LAYOUT DO CHAT
        ===================================================== */

        .chat-layout {
            display: grid;
            grid-template-columns: minmax(290px, 360px) minmax(0, 1fr);
            gap: 21px;
            align-items: start;
        }

        .chat-sidebar,
        .chat-main {
            display: grid;
            gap: 16px;
        }


        /* =====================================================
           LISTA DE CONVERSAS
        ===================================================== */

        .conversation-list {
            overflow: hidden;
            border: 1px solid #e2e8f0;
            border-radius: 11px;
            background: #ffffff;
        }

        .conversation-item {
            display: block;
            width: 100%;
            padding: 13px 14px;
            border: 0;
            border-bottom: 1px solid #edf2f7;
            background: #ffffff;
            color: #1f2937;
            text-align: left;
            text-decoration: none;
            transition: background 0.15s, color 0.15s;
        }

            .conversation-item:last-child {
                border-bottom: 0;
            }

            .conversation-item:hover,
            .conversation-item.active {
                background: #eef4ff;
                color: #123570;
            }

        .conversation-title {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 10px;
            font-size: 14px;
            font-weight: 800;
        }

        .conversation-name {
            display: flex;
            align-items: center;
            min-width: 0;
            gap: 7px;
        }

        .conversation-type {
            display: inline-flex;
            padding: 3px 7px;
            border: 1px solid #dbeafe;
            border-radius: 999px;
            background: #eff6ff;
            color: #1d4ed8;
            font-size: 9px;
            font-weight: 800;
            text-transform: uppercase;
        }

        .conversation-context {
            display: block;
            margin-top: 5px;
            color: #64748b;
            font-size: 11px;
            font-weight: 600;
            line-height: 1.4;
        }

        .conversation-subtitle {
            display: block;
            margin-top: 7px;
            overflow: hidden;
            color: #64748b;
            font-size: 12px;
            text-overflow: ellipsis;
            white-space: nowrap;
        }

        .conversation-date {
            display: block;
            margin-top: 4px;
            color: #94a3b8;
            font-size: 10px;
        }

        .unread-dot {
            width: 9px;
            height: 9px;
            flex-shrink: 0;
            border-radius: 50%;
            background: #dc2626;
            box-shadow: 0 0 0 3px rgba(220, 38, 38, 0.12);
        }

        .unread-count {
            display: inline-flex;
            min-width: 22px;
            height: 22px;
            padding: 0 6px;
            align-items: center;
            justify-content: center;
            border-radius: 999px;
            background: #dc2626;
            color: #ffffff;
            font-size: 11px;
            font-weight: 800;
        }

        .sem-conversas {
            padding: 25px 16px;
            color: #64748b;
            font-size: 13px;
            line-height: 1.6;
            text-align: center;
        }


        /* =====================================================
           CONVERSA ATUAL
        ===================================================== */

        .chat-panel {
            display: flex;
            min-height: 660px;
            flex-direction: column;
        }

        .chat-current-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 15px;
            padding: 18px 20px;
            border-bottom: 1px solid #e5e7eb;
            background: #f8fafc;
        }

            .chat-current-header h2 {
                margin: 0 0 3px;
                color: #1f2937;
                font-size: 18px;
                font-weight: 800;
            }

            .chat-current-header p {
                margin: 0;
                color: #64748b;
                font-size: 12px;
                line-height: 1.5;
            }

        .chat-type {
            display: inline-flex;
            padding: 5px 10px;
            border: 1px solid #bfdbfe;
            border-radius: 999px;
            background: #eff6ff;
            color: #1d4ed8;
            font-size: 11px;
            font-weight: 700;
            white-space: nowrap;
        }


        /* =====================================================
           MENSAGENS
        ===================================================== */

        .chat-box {
            min-height: 440px;
            max-height: 570px;
            flex: 1;
            overflow-y: auto;
            padding: 20px;
            background: #f8fafc;
        }

        .message-row {
            display: flex;
            margin-bottom: 12px;
        }

            .message-row.mine {
                justify-content: flex-end;
            }

        .message-bubble {
            display: inline-block;
            width: auto;
            max-width: min(78%, 650px);
            padding: 10px 13px;
            border: 1px solid #e2e8f0;
            border-radius: 14px 14px 14px 4px;
            background: #ffffff;
            color: #334155;
            font-size: 14px;
            line-height: 1.5;
            word-break: break-word;
            box-shadow: 0 2px 7px rgba(15, 23, 42, 0.04);
        }

        .message-text {
            white-space: pre-wrap;
            word-break: break-word;
        }

        .message-row.mine .message-bubble {
            border-color: #2563eb;
            border-radius: 14px 14px 4px 14px;
            background: #2563eb;
            color: #ffffff;
        }

        .message-meta {
            display: block;
            margin-bottom: 4px;
            font-size: 10px;
            font-weight: 600;
            opacity: 0.72;
        }

        .sem-mensagens {
            display: flex;
            align-items: center;
            justify-content: center;
            min-height: 400px;
            padding: 20px;
            color: #64748b;
            font-size: 13px;
            line-height: 1.6;
            text-align: center;
        }


        /* =====================================================
           ENVIO
        ===================================================== */

        .message-compose {
            padding: 16px 20px;
            border-top: 1px solid #e5e7eb;
            background: #ffffff;
        }

        .message-compose-grid {
            display: grid;
            grid-template-columns: minmax(0, 1fr) auto;
            gap: 10px;
        }

        .message-textarea {
            min-height: 54px;
            max-height: 150px;
            resize: vertical;
        }

        .send-button {
            min-width: 95px;
        }


        /* =====================================================
           SEM TURMAS
        ===================================================== */

        .sem-turmas {
            padding: 60px 24px;
            border: 1px solid #dbe3ed;
            border-radius: 15px;
            background: #ffffff;
            text-align: center;
            box-shadow: 0 5px 18px rgba(15, 23, 42, 0.06);
        }

        .sem-turmas-icon {
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

        .sem-turmas h2 {
            margin: 0 0 7px;
            color: #334155;
            font-size: 21px;
            font-weight: 800;
        }

        .sem-turmas p {
            max-width: 590px;
            margin: 0 auto;
            color: #64748b;
            font-size: 14px;
            line-height: 1.6;
        }


        /* =====================================================
           RESPONSIVO
        ===================================================== */

        @media (max-width: 1100px) {

            .selecao-grid {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }

            .btn-iniciar {
                width: 100%;
            }
        }

        @media (max-width: 920px) {

            .chat-layout {
                grid-template-columns: 1fr;
            }

            .chat-panel {
                min-height: 540px;
            }
        }

        @media (max-width: 650px) {

            .page-header {
                flex-direction: column;
            }

            .professor-badge {
                width: 100%;
                justify-content: center;
            }

            .selecao-grid {
                grid-template-columns: 1fr;
            }

            .chat-current-header {
                align-items: flex-start;
                flex-direction: column;
            }

            .message-compose-grid {
                grid-template-columns: 1fr;
            }

            .send-button {
                width: 100%;
            }

            .message-bubble {
                max-width: 90%;
            }
        }
    </style>

</asp:Content>


<asp:Content
    ID="ContentMain"
    ContentPlaceHolderID="mainContent"
    runat="server">

    <div class="mensagens-page">


        <!-- ==================================================
             CABEÇALHO
        =================================================== -->

        <div class="page-header">

            <div>

                <h1>Mensagens
                </h1>

                <p>
                    Converse diretamente com alunos e encarregados
                    de educação das turmas em que leciona.
                </p>

            </div>

            <asp:Label
                ID="LblProfessor"
                runat="server"
                Visible="false"
                CssClass="professor-badge" />

        </div>


        <!-- ==================================================
             MENSAGEM
        =================================================== -->

        <asp:Label
            ID="LblMensagem"
            runat="server"
            Visible="false" />


        <!-- ==================================================
             SEM TURMAS
        =================================================== -->

        <asp:Panel
            ID="PnlSemTurmas"
            runat="server"
            Visible="false"
            CssClass="sem-turmas">

            <div class="sem-turmas-icon">
                !
            </div>

            <h2>Nenhuma turma disponível
            </h2>

            <p>
                Não existem turmas ativas associadas à sua conta.
                Contacte o agrupamento para confirmar as disciplinas
                e turmas atribuídas.
            </p>

        </asp:Panel>


        <!-- ==================================================
             CONTEÚDO
        =================================================== -->

        <asp:Panel
            ID="PnlConteudo"
            runat="server"
            Visible="false">


            <!-- ==================================================
                 SELEÇÃO DO DESTINATÁRIO
            =================================================== -->

            <section class="chat-card selecao-card">

                <header class="chat-card-header">

                    <h2>Nova conversa
                    </h2>

                    <p>
                        Escolha uma turma e um aluno ou encarregado
                        de educação autorizado.
                    </p>

                </header>

                <div class="chat-card-body">

                    <div class="selecao-grid">


                        <!-- TURMA -->

                        <div>

                            <asp:Label
                                ID="LabelTurma"
                                runat="server"
                                AssociatedControlID="DdlTurmas"
                                CssClass="form-label"
                                Text="Turma" />

                            <asp:DropDownList
                                ID="DdlTurmas"
                                runat="server"
                                CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="DdlTurmas_SelectedIndexChanged" />

                        </div>


                        <!-- TIPO -->

                        <div>

                            <asp:Label
                                ID="LabelTipoDestinatario"
                                runat="server"
                                AssociatedControlID="DdlTipoDestinatario"
                                CssClass="form-label"
                                Text="Tipo de destinatário" />

                            <asp:DropDownList
                                ID="DdlTipoDestinatario"
                                runat="server"
                                CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="DdlTipoDestinatario_SelectedIndexChanged">

                                <asp:ListItem
                                    Text="Aluno"
                                    Value="aluno" />

                                <asp:ListItem
                                    Text="Encarregado de Educação"
                                    Value="encarregado" />

                            </asp:DropDownList>

                        </div>


                        <!-- DESTINATÁRIO -->

                        <div>

                            <asp:Label
                                ID="LabelDestinatario"
                                runat="server"
                                AssociatedControlID="DdlDestinatarios"
                                CssClass="form-label"
                                Text="Destinatário" />

                            <asp:DropDownList
                                ID="DdlDestinatarios"
                                runat="server"
                                CssClass="form-select">

                                <asp:ListItem
                                    Text="Selecione um destinatário"
                                    Value="" />

                            </asp:DropDownList>

                            <span class="campo-ajuda">Apenas são apresentados utilizadores
                                relacionados com a turma selecionada.
                            </span>

                        </div>


                        <!-- BOTÃO -->

                        <div>

                            <asp:Button
                                ID="BtnIniciarConversa"
                                runat="server"
                                Text="Iniciar conversa"
                                CssClass="btn btn-primary btn-iniciar"
                                CausesValidation="false"
                                OnClick="BtnIniciarConversa_Click" />

                        </div>

                    </div>

                </div>

            </section>


            <!-- ==================================================
                 CHAT
            =================================================== -->

            <div class="chat-layout">


                <!-- ==================================================
                     CONVERSAS
                =================================================== -->

                <aside class="chat-sidebar">

                    <section class="chat-card">

                        <header class="chat-card-header">

                            <h2>Conversas
                            </h2>

                            <p>
                                Alunos e encarregados com quem já
                                trocou mensagens.
                            </p>

                        </header>

                        <div class="chat-card-body">

                            <div class="conversation-list">

                                <asp:Repeater
                                    ID="RepeaterConversas"
                                    runat="server"
                                    OnItemCommand="RepeaterConversas_ItemCommand">

                                    <ItemTemplate>

                                        <asp:LinkButton
                                            ID="BtnAbrirConversa"
                                            runat="server"
                                            CausesValidation="false"
                                            CommandName="AbrirConversa"
                                            CommandArgument='<%# Eval("Id") %>'
                                            CssClass='<%#
                                                Convert.ToBoolean(
                                                    Eval("Ativa")
                                                )
                                                    ? "conversation-item active"
                                                    : "conversation-item"
                                            %>'>

                                            <span class="conversation-title">

                                                <span class="conversation-name">

                                                    <span
                                                        class="unread-dot"
                                                        runat="server"
                                                        visible='<%#
                                                            Convert.ToInt32(
                                                                Eval("NaoLidas")
                                                            ) > 0
                                                        %>'>
                                                    </span>

                                                    <span>

                                                        <%#
                                                            Server.HtmlEncode(
                                                                Eval("Nome")
                                                                    .ToString()
                                                            )
                                                        %>

                                                    </span>

                                                </span>

                                                <span class="d-flex align-items-center gap-2">

                                                    <span class="conversation-type">

                                                        <%#
                                                            Server.HtmlEncode(
                                                                Eval("TipoTexto")
                                                                    .ToString()
                                                            )
                                                        %>

                                                    </span>

                                                    <span
                                                        class="unread-count"
                                                        runat="server"
                                                        visible='<%#
                                                            Convert.ToInt32(
                                                                Eval("NaoLidas")
                                                            ) > 0
                                                        %>'>

                                                        <%#
                                                            Convert.ToInt32(
                                                                Eval("NaoLidas")
                                                            ) > 9
                                                                ? "9+"
                                                                : Eval("NaoLidas")
                                                                    .ToString()
                                                        %>

                                                    </span>

                                                </span>

                                            </span>

                                            <span class="conversation-context">

                                                <%#
                                                    Server.HtmlEncode(
                                                        Eval("Contexto")
                                                            .ToString()
                                                    )
                                                %>

                                            </span>

                                            <span class="conversation-subtitle">

                                                <%#
                                                    Server.HtmlEncode(
                                                        Eval("UltimaMensagem")
                                                            .ToString()
                                                    )
                                                %>

                                            </span>

                                            <span class="conversation-date">

                                                <%#
                                                    Eval(
                                                        "DataUltima",
                                                        "{0:dd/MM/yyyy HH:mm}"
                                                    )
                                                %>

                                            </span>

                                        </asp:LinkButton>

                                    </ItemTemplate>

                                </asp:Repeater>


                                <asp:Panel
                                    ID="PnlSemConversas"
                                    runat="server"
                                    CssClass="sem-conversas">
                                    Ainda não existem conversas.
                                    Escolha um destinatário para começar.

                                </asp:Panel>

                            </div>

                        </div>

                    </section>

                </aside>


                <!-- ==================================================
                     CONVERSA ATUAL
                =================================================== -->

                <main class="chat-main">

                    <section class="chat-card chat-panel">


                        <!-- CABEÇALHO -->

                        <header class="chat-current-header">

                            <div>

                                <h2>

                                    <asp:Label
                                        ID="LblConversaAtual"
                                        runat="server"
                                        Text="Selecione uma conversa" />

                                </h2>

                                <p>

                                    <asp:Label
                                        ID="LblContextoConversa"
                                        runat="server"
                                        Text="Nenhuma conversa aberta." />

                                </p>

                            </div>

                            <asp:Label
                                ID="LblTipoConversa"
                                runat="server"
                                Visible="false"
                                CssClass="chat-type" />

                        </header>


                        <!-- DADOS DA CONVERSA -->

                        <asp:HiddenField
                            ID="HdnConversaId"
                            runat="server" />

                        <asp:HiddenField
                            ID="HdnDestinatarioUserId"
                            runat="server" />

                        <asp:HiddenField
                            ID="HdnTipoDestinatario"
                            runat="server" />

                        <asp:HiddenField
                            ID="HdnTurmaContextoId"
                            runat="server" />

                        <asp:HiddenField
                            ID="HdnAlunoContextoId"
                            runat="server" />


                        <!-- MENSAGENS -->

                        <div
                            id="ChatBox"
                            runat="server"
                            clientidmode="Static"
                            class="chat-box">

                            <asp:Repeater
                                ID="RepeaterMensagens"
                                runat="server">

                                <ItemTemplate>

                                    <div class='<%#
                                        Convert.ToBoolean(
                                            Eval("Minha")
                                        )
                                            ? "message-row mine"
                                            : "message-row"
                                    %>'>

                                        <div class="message-bubble">

                                            <span class="message-meta">

                                                <%#
                                                    Server.HtmlEncode(
                                                        Eval("Autor")
                                                            .ToString()
                                                    )
                                                %>

                                                ·

                                                <%#
                                                    Eval(
                                                        "CriadoEm",
                                                        "{0:dd/MM/yyyy HH:mm}"
                                                    )
                                                %>

                                            </span>

                                            <%#
                                                Server.HtmlEncode(
                                                    Eval("Texto")
                                                        .ToString()
                                                )
                                            %>
                                        </div>

                                    </div>

                                </ItemTemplate>

                            </asp:Repeater>


                            <asp:Panel
                                ID="PnlSemMensagens"
                                runat="server"
                                CssClass="sem-mensagens">
                                Selecione uma conversa ou inicie uma nova
                                conversa com um aluno ou encarregado.

                            </asp:Panel>

                        </div>


                        <!-- ENVIAR -->

                        <div class="message-compose">

                            <div class="message-compose-grid">

                                <asp:TextBox
                                    ID="TxtMensagem"
                                    runat="server"
                                    TextMode="MultiLine"
                                    Rows="2"
                                    MaxLength="2000"
                                    CssClass="form-control message-textarea"
                                    placeholder="Escreva uma mensagem..." />

                                <asp:Button
                                    ID="BtnEnviarMensagem"
                                    runat="server"
                                    Text="Enviar"
                                    CssClass="btn btn-primary send-button"
                                    CausesValidation="false"
                                    OnClick="BtnEnviarMensagem_Click" />

                            </div>

                        </div>

                    </section>

                </main>

            </div>

        </asp:Panel>

    </div>


    <!-- ==================================================
         DESLOCAR PARA A ÚLTIMA MENSAGEM
    =================================================== -->

    <script>

                 document.addEventListener(
                     "DOMContentLoaded",
                     function () {

                         const chatBox =
                             document.getElementById(
                                 "ChatBox"
                             );

                         if (chatBox) {

                             chatBox.scrollTop =
                                 chatBox.scrollHeight;

                         }

                     }
                 );

    </script>

</asp:Content>
