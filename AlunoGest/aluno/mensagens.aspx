<%@ Page Title="Amigos e mensagens" Language="C#" MasterPageFile="~/aluno/MasterAluno1.Master"
    AutoEventWireup="true" CodeBehind="mensagens.aspx.cs" Inherits="AlunoGest.aluno.mensagens" %>

<asp:Content ID="Head" ContentPlaceHolderID="headContent" runat="server">
    <style>
        .chat-layout {
            display: grid;
            grid-template-columns: 1fr minmax(280px, 360px);
            gap: 1.25rem;
            align-items: start;
        }

        .chat-sidebar {
            order: 2;
        }

        .chat-main {
            order: 1;
        }

        .chat-stack {
            display: grid;
            gap: 1rem;
        }

        .conversation-list {
            overflow: hidden;
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            background: #fff;
        }

        .conversation-item {
            display: block;
            width: 100%;
            padding: .8rem .9rem;
            border: 0;
            border-bottom: 1px solid #edf2f7;
            background: #fff;
            color: #1f2937;
            text-align: left;
            text-decoration: none;
        }

            .conversation-item:hover,
            .conversation-item.active {
                background: #eef3fb;
                color: #123570;
            }

        .conversation-title {
            display: flex;
            justify-content: space-between;
            gap: .75rem;
            font-weight: 700;
        }

        .conversation-name {
            display: flex;
            min-width: 0;
            align-items: center;
            gap: .45rem;
        }

        .unread-dot {
            width: 10px;
            height: 10px;
            flex: 0 0 auto;
            border-radius: 50%;
            background: #dc3545;
            box-shadow: 0 0 0 3px rgba(220, 53, 69, .12);
        }

        .unread-count {
            display: inline-flex;
            min-width: 22px;
            height: 22px;
            padding: 0 6px;
            align-items: center;
            justify-content: center;
            border-radius: 999px;
            background: #dc3545;
            color: #fff;
            font-size: .78rem;
        }

        .conversation-subtitle {
            margin-top: .15rem;
            overflow: hidden;
            color: #64748b;
            font-size: .85rem;
            text-overflow: ellipsis;
            white-space: nowrap;
        }

        .chat-panel {
            display: flex;
            min-height: 640px;
            flex-direction: column;
        }

        .chat-box {
            min-height: 410px;
            max-height: 520px;
            flex: 1;
            overflow-y: auto;
            padding: 1rem;
            border-radius: 12px;
            background: #f8fafc;
        }

        .message-row {
            display: flex;
            margin-bottom: .75rem;
        }

            .message-row.mine {
                justify-content: flex-end;
            }

        .message-bubble {
            max-width: min(78%, 620px);
            padding: .7rem .85rem;
            border: 1px solid #e2e8f0;
            border-radius: 14px;
            background: #fff;
        }

        .message-row.mine .message-bubble {
            border-color: #123570;
            background: #123570;
            color: #fff;
        }

        .message-meta {
            display: block;
            margin-bottom: .2rem;
            font-size: .78rem;
            opacity: .72;
        }

        .quick-actions {
            display: grid;
            grid-template-columns: repeat(4, minmax(120px, 1fr));
            gap: .65rem;
        }

            .quick-actions .btn {
                display: flex;
                min-height: 54px;
                align-items: center;
                justify-content: center;
                gap: .45rem;
                text-align: center;
                white-space: normal;
            }

        .action-icon {
            display: inline-flex;
            width: 26px;
            height: 26px;
            align-items: center;
            justify-content: center;
            border-radius: 50%;
            background: rgba(18, 53, 112, .12);
            font-weight: 800;
            line-height: 1;
        }

        .btn-primary .action-icon {
            background: rgba(255, 255, 255, .22);
        }

        .compact-list .list-group-item {
            padding: .65rem .8rem;
        }

        /* =====================================================
           PESQUISA DE AMIGOS
        ===================================================== */

        .friend-search-wrapper {
            position: relative;
        }

        .friend-suggestions {
            position: absolute;
            z-index: 1070;
            top: calc(100% + 6px);
            right: 0;
            left: 0;
            max-height: 280px;
            overflow-y: auto;
            border: 1px solid #dbe3ed;
            border-radius: 10px;
            background: #ffffff;
            box-shadow: 0 12px 28px rgba(15, 23, 42, 0.16);
        }

        .friend-suggestion {
            display: block;
            width: 100%;
            padding: 11px 13px;
            border: 0;
            border-bottom: 1px solid #edf2f7;
            background: #ffffff;
            color: #1f2937;
            text-align: left;
            cursor: pointer;
        }

            .friend-suggestion:last-child {
                border-bottom: 0;
            }

            .friend-suggestion:hover,
            .friend-suggestion:focus {
                outline: none;
                background: #eef4ff;
            }

        .friend-suggestion-name {
            display: block;
            margin-bottom: 3px;
            color: #1e293b;
            font-size: 14px;
            font-weight: 700;
        }

        .friend-suggestion-details {
            display: flex;
            justify-content: space-between;
            gap: 10px;
            color: #64748b;
            font-size: 12px;
        }

        .friend-suggestion-username {
            color: #2563eb;
            font-weight: 600;
        }

        .friend-no-results {
            padding: 12px 13px;
            color: #64748b;
            font-size: 13px;
        }

        @media (max-width: 992px) {
            .chat-layout {
                grid-template-columns: 1fr;
            }

            .chat-sidebar,
            .chat-main {
                order: initial;
            }

            .chat-panel {
                min-height: 520px;
            }

            .quick-actions {
                grid-template-columns: 1fr 1fr;
            }
        }

        @media (max-width: 520px) {
            .quick-actions {
                grid-template-columns: 1fr;
            }

            .friend-suggestion-details {
                flex-direction: column;
                gap: 2px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="mainContent" runat="server">
    <asp:ScriptManager
        ID="ScriptManagerMensagens" 
        runat="server"
        EnablePageMethods="true" />

    <div class="d-flex flex-wrap justify-content-between align-items-start gap-3 mb-4">
        <div>
            <h1 class="h3 mb-1">Amigos e mensagens</h1>
            <p class="text-muted mb-0">
                Adiciona colegas, aceita pedidos e conversa em privado ou em grupo.
           
            </p>
        </div>
    </div>

    <asp:Label
        ID="LblMensagem"
        runat="server"
        Visible="false" />

    <div class="chat-layout">

        <aside class="chat-stack chat-sidebar">

            <div class="card page-card">

                <div class="card-body">

                    <div class="d-flex align-items-center justify-content-between gap-2 mb-3">

                        <h2 class="h5 mb-0">Conversas
                        </h2>

                        <span class="badge rounded-pill text-bg-primary">chat
                        </span>

                    </div>

                    <div class="conversation-list">

                        <asp:Repeater
                            ID="RepeaterConversas"
                            runat="server"
                            OnItemCommand="RepeaterConversas_ItemCommand">

                            <ItemTemplate>

                                <asp:LinkButton
                                    runat="server"
                                    CssClass='<%# Convert.ToBoolean(Eval("Ativa")) ? "conversation-item active" : "conversation-item" %>'
                                    CommandName="AbrirConversa"
                                    CommandArgument='<%# Eval("Tipo") + ":" + Eval("Id") %>'>

                                    <span class="conversation-title">

                                        <span class="conversation-name">

                                            <span
                                                class="unread-dot"
                                                runat="server"
                                                visible='<%# Convert.ToInt32(Eval("NaoLidas")) > 0 %>'>
                                            </span>

                                            <span>
                                                <%# Server.HtmlEncode(Eval("Nome").ToString()) %>
                                            </span>

                                        </span>

                                        <span class="d-flex align-items-center gap-2">

                                            <span
                                                class="unread-count"
                                                runat="server"
                                                visible='<%# Convert.ToInt32(Eval("NaoLidas")) > 0 %>'>

                                                <%# Convert.ToInt32(Eval("NaoLidas")) > 9 ? "9+" : Eval("NaoLidas").ToString() %>

                                            </span>

                                            <small>
                                                <%# Server.HtmlEncode(Eval("TipoTexto").ToString()) %>
                                            </small>

                                        </span>

                                    </span>

                                    <span class="conversation-subtitle">
                                        <%# Server.HtmlEncode(Eval("UltimaMensagem").ToString()) %>
                                    </span>

                                </asp:LinkButton>

                            </ItemTemplate>

                        </asp:Repeater>

                        <asp:Panel
                            ID="PainelSemConversas"
                            runat="server"
                            CssClass="p-3 text-muted">
                            Adiciona amigos para começar a conversar.

                       
                        </asp:Panel>

                    </div>

                </div>

            </div>

        </aside>

        <section class="chat-stack chat-main">

            <div class="card page-card">

                <div class="card-body">

                    <div class="quick-actions">

                        <button
                            type="button"
                            class="btn btn-primary"
                            data-bs-toggle="modal"
                            data-bs-target="#modalAdicionarAmigo">

                            <span class="action-icon">+</span>
                            Adicionar amigo

                       
                        </button>

                        <button
                            type="button"
                            class="btn btn-outline-primary"
                            data-bs-toggle="modal"
                            data-bs-target="#modalPedidosAmizade">

                            <span class="action-icon">!</span>
                            Pedidos recebidos

                       
                        </button>

                        <button
                            type="button"
                            class="btn btn-outline-primary"
                            data-bs-toggle="modal"
                            data-bs-target="#modalCriarGrupo">

                            <span class="action-icon">#</span>
                            Criar grupo

                       
                        </button>

                        <button
                            type="button"
                            class="btn btn-outline-primary"
                            data-bs-toggle="modal"
                            data-bs-target="#modalConvitesGrupo">

                            <span class="action-icon">*</span>
                            Convites de grupos

                       
                        </button>

                    </div>

                </div>

            </div>

            <div class="card page-card">

                <div class="card-body chat-panel">

                    <div class="d-flex align-items-center justify-content-between gap-2 mb-3">

                        <h2 class="h5 mb-0">

                            <asp:Label
                                ID="LblConversaAtual"
                                runat="server"
                                Text="Escolhe uma conversa" />

                        </h2>

                        <asp:Label
                            ID="LblTipoConversa"
                            runat="server"
                            CssClass="badge rounded-pill text-bg-light" />

                    </div>

                    <asp:HiddenField
                        ID="HdnTipoConversa"
                        runat="server" />

                    <asp:HiddenField
                        ID="HdnConversaId"
                        runat="server" />

                    <div class="chat-box mb-3">

                        <asp:Repeater
                            ID="RepeaterMensagens"
                            runat="server">

                            <ItemTemplate>

                                <div class='<%# Convert.ToBoolean(Eval("Minha")) ? "message-row mine" : "message-row" %>'>

                                    <div class="message-bubble">

                                        <span class="message-meta">
                                            <%# Server.HtmlEncode(Eval("Autor").ToString()) %>
                                            -
                                           
                                            <%# Eval("CriadoEm", "{0:dd/MM/yyyy HH:mm}") %>
                                        </span>

                                        <%# Server.HtmlEncode(Eval("Texto").ToString()) %>
                                    </div>

                                </div>

                            </ItemTemplate>

                        </asp:Repeater>

                        <asp:Panel
                            ID="PainelSemMensagens"
                            runat="server"
                            CssClass="text-muted">
                            Ainda não existem mensagens nesta conversa.

                       
                        </asp:Panel>

                    </div>

                    <div class="input-group">

                        <asp:TextBox
                            ID="TxtMensagem"
                            runat="server"
                            CssClass="form-control"
                            TextMode="MultiLine"
                            Rows="2"
                            MaxLength="2000" />

                        <asp:Button
                            ID="BtnEnviarMensagem"
                            runat="server"
                            Text="Enviar"
                            CssClass="btn btn-primary"
                            OnClick="BtnEnviarMensagem_Click" />

                    </div>

                </div>

            </div>

        </section>

    </div>

    <!-- =====================================================
         MODAL: ADICIONAR AMIGO
    ====================================================== -->

    <div
        class="modal fade"
        id="modalAdicionarAmigo"
        tabindex="-1"
        aria-labelledby="modalAdicionarAmigoTitulo"
        aria-hidden="true">

        <div class="modal-dialog modal-dialog-centered">

            <div class="modal-content">

                <div class="modal-header">

                    <h2
                        class="modal-title h5"
                        id="modalAdicionarAmigoTitulo">Adicionar amigo

                    </h2>

                    <button
                        type="button"
                        class="btn-close"
                        data-bs-dismiss="modal"
                        aria-label="Fechar">
                    </button>

                </div>

                <div class="modal-body">

                    <label
                        class="form-label"
                        for="<%= TxtUsernameAmigo.ClientID %>">
                        Pesquisar aluno pelo nome

                   
                    </label>

                    <div
                        id="friendSearchWrapper"
                        class="friend-search-wrapper">

                        <asp:TextBox
                            ID="TxtUsernameAmigo"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="256"
                            autocomplete="off"
                            placeholder="Começa a escrever o nome do aluno..." />

                        <asp:HiddenField
                            ID="HdnUsernameAmigo"
                            runat="server" />

                        <div
                            id="listaSugestoesAmigo"
                            class="friend-suggestions d-none"
                            role="listbox">
                        </div>

                    </div>

                </div>

                <div class="modal-footer">

                    <button
                        type="button"
                        class="btn btn-outline-secondary"
                        data-bs-dismiss="modal">
                        Fechar

                   
                    </button>

                    <asp:Button
                        ID="BtnAdicionarAmigo"
                        runat="server"
                        Text="Enviar pedido"
                        CssClass="btn btn-primary"
                        OnClick="BtnAdicionarAmigo_Click" />

                </div>

            </div>

        </div>

    </div>

    <!-- =====================================================
         MODAL: PEDIDOS DE AMIZADE
    ====================================================== -->

    <div
        class="modal fade"
        id="modalPedidosAmizade"
        tabindex="-1"
        aria-labelledby="modalPedidosAmizadeTitulo"
        aria-hidden="true">

        <div class="modal-dialog modal-dialog-centered modal-lg">

            <div class="modal-content">

                <div class="modal-header">

                    <h2
                        class="modal-title h5"
                        id="modalPedidosAmizadeTitulo">Pedidos recebidos

                    </h2>

                    <button
                        type="button"
                        class="btn-close"
                        data-bs-dismiss="modal"
                        aria-label="Fechar">
                    </button>

                </div>

                <div class="modal-body">

                    <asp:GridView
                        ID="GridPedidosAmizade"
                        runat="server"
                        AutoGenerateColumns="false"
                        CssClass="table table-sm table-hover align-middle mb-0"
                        GridLines="None"
                        DataKeyNames="Id"
                        EmptyDataText="Sem pedidos pendentes."
                        OnRowCommand="GridPedidosAmizade_RowCommand">

                        <Columns>

                            <asp:BoundField
                                DataField="NomeCompleto"
                                HeaderText="Aluno" />

                            <asp:BoundField
                                DataField="UserName"
                                HeaderText="Utilizador" />

                            <asp:TemplateField>

                                <ItemTemplate>

                                    <asp:LinkButton
                                        runat="server"
                                        Text="Aceitar"
                                        CommandName="AceitarAmizade"
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

    <!-- =====================================================
         MODAL: CRIAR GRUPO
    ====================================================== -->

    <div
        class="modal fade"
        id="modalCriarGrupo"
        tabindex="-1"
        aria-labelledby="modalCriarGrupoTitulo"
        aria-hidden="true">

        <div class="modal-dialog modal-dialog-centered">

            <div class="modal-content">

                <div class="modal-header">

                    <h2
                        class="modal-title h5"
                        id="modalCriarGrupoTitulo">Criar grupo

                    </h2>

                    <button
                        type="button"
                        class="btn-close"
                        data-bs-dismiss="modal"
                        aria-label="Fechar">
                    </button>

                </div>

                <div class="modal-body">

                    <label
                        class="form-label"
                        for="<%= TxtNomeGrupo.ClientID %>">
                        Nome do grupo

                   
                    </label>

                    <asp:TextBox
                        ID="TxtNomeGrupo"
                        runat="server"
                        CssClass="form-control mb-3"
                        MaxLength="120" />

                    <asp:CheckBoxList
                        ID="CheckAmigosGrupo"
                        runat="server"
                        CssClass="compact-list" />

                </div>

                <div class="modal-footer">

                    <button
                        type="button"
                        class="btn btn-outline-secondary"
                        data-bs-dismiss="modal">
                        Fechar

                   
                    </button>

                    <asp:Button
                        ID="BtnCriarGrupo"
                        runat="server"
                        Text="Criar e convidar"
                        CssClass="btn btn-primary"
                        OnClick="BtnCriarGrupo_Click" />

                </div>

            </div>

        </div>

    </div>

    <!-- =====================================================
         MODAL: CONVITES DE GRUPO
    ====================================================== -->

    <div
        class="modal fade"
        id="modalConvitesGrupo"
        tabindex="-1"
        aria-labelledby="modalConvitesGrupoTitulo"
        aria-hidden="true">

        <div class="modal-dialog modal-dialog-centered modal-lg">

            <div class="modal-content">

                <div class="modal-header">

                    <h2
                        class="modal-title h5"
                        id="modalConvitesGrupoTitulo">Convites para grupos

                    </h2>

                    <button
                        type="button"
                        class="btn-close"
                        data-bs-dismiss="modal"
                        aria-label="Fechar">
                    </button>

                </div>

                <div class="modal-body">

                    <asp:GridView
                        ID="GridConvitesGrupo"
                        runat="server"
                        AutoGenerateColumns="false"
                        CssClass="table table-sm table-hover align-middle mb-0"
                        GridLines="None"
                        DataKeyNames="GrupoId"
                        EmptyDataText="Sem convites pendentes."
                        OnRowCommand="GridConvitesGrupo_RowCommand">

                        <Columns>

                            <asp:BoundField
                                DataField="Nome"
                                HeaderText="Grupo" />

                            <asp:BoundField
                                DataField="Criador"
                                HeaderText="Criado por" />

                            <asp:TemplateField>

                                <ItemTemplate>

                                    <asp:LinkButton
                                        runat="server"
                                        Text="Aceitar"
                                        CommandName="AceitarGrupo"
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

    <!-- =====================================================
         PESQUISA DINÂMICA DE ALUNOS
    ====================================================== -->

    <script>
        document.addEventListener(
            'DOMContentLoaded',
            function () {

                const input =
                    document.getElementById(
                        '<%= TxtUsernameAmigo.ClientID %>'
                    );

                const hiddenUsername =
                    document.getElementById(
                        '<%= HdnUsernameAmigo.ClientID %>'
                    );

                const lista =
                    document.getElementById(
                        'listaSugestoesAmigo'
                    );

                const wrapper =
                    document.getElementById(
                        'friendSearchWrapper'
                    );

                const modal =
                    document.getElementById(
                        'modalAdicionarAmigo'
                    );

                let temporizadorPesquisa = null;
                let numeroPesquisa = 0;

                function esconderSugestoes() {
                    lista.innerHTML = '';
                    lista.classList.add('d-none');
                }

                function mostrarMensagemLista(mensagem) {
                    lista.innerHTML = '';

                    const elemento =
                        document.createElement('div');

                    elemento.className =
                        'friend-no-results';

                    elemento.textContent =
                        mensagem;

                    lista.appendChild(elemento);
                    lista.classList.remove('d-none');
                }

                function selecionarAluno(aluno) {
                    input.value =
                        aluno.UserName;

                    hiddenUsername.value =
                        aluno.UserName;

                    esconderSugestoes();
                }

                function mostrarSugestoes(alunos) {
                    lista.innerHTML = '';

                    if (!alunos || alunos.length === 0) {
                        mostrarMensagemLista(
                            'Nenhum aluno encontrado.'
                        );

                        return;
                    }

                    alunos.forEach(
                        function (aluno) {

                            const botao =
                                document.createElement('button');

                            botao.type =
                                'button';

                            botao.className =
                                'friend-suggestion';

                            botao.setAttribute(
                                'role',
                                'option'
                            );

                            const nome =
                                document.createElement('span');

                            nome.className =
                                'friend-suggestion-name';

                            nome.textContent =
                                aluno.NomeCompleto;

                            const detalhes =
                                document.createElement('span');

                            detalhes.className =
                                'friend-suggestion-details';

                            const processo =
                                document.createElement('span');

                            processo.textContent =
                                'Processo: ' +
                                (
                                    aluno.NumeroProcesso ||
                                    'não indicado'
                                );

                            const username =
                                document.createElement('span');

                            username.className =
                                'friend-suggestion-username';

                            username.textContent =
                                aluno.UserName;

                            detalhes.appendChild(
                                processo
                            );

                            detalhes.appendChild(
                                username
                            );

                            botao.appendChild(
                                nome
                            );

                            botao.appendChild(
                                detalhes
                            );

                            botao.addEventListener(
                                'click',
                                function () {

                                    selecionarAluno(
                                        aluno
                                    );

                                }
                            );

                            lista.appendChild(
                                botao
                            );

                        }
                    );

                    lista.classList.remove(
                        'd-none'
                    );
                }

                function pesquisarAlunos(
                    termo,
                    pesquisaAtual) {

                    PageMethods.PesquisarAlunos(
                        termo,

                        function (alunos) {

                            if (pesquisaAtual !== numeroPesquisa) {
                                return;
                            }

                            mostrarSugestoes(
                                alunos || []
                            );
                        },

                        function (erro) {

                            console.error(
                                'Erro na pesquisa:',
                                erro.get_message()
                            );

                            if (pesquisaAtual === numeroPesquisa) {

                                mostrarMensagemLista(
                                    'Erro: ' +
                                    erro.get_message()
                                );
                            }
                        }
                    );
                }

                input.addEventListener(
                    'input',
                    function () {

                        hiddenUsername.value =
                            '';

                        clearTimeout(
                            temporizadorPesquisa
                        );

                        numeroPesquisa++;

                        const termo =
                            input.value.trim();

                        if (termo.length === 0) {
                            esconderSugestoes();
                            return;
                        }

                        const pesquisaAtual =
                            numeroPesquisa;

                        temporizadorPesquisa =
                            setTimeout(
                                function () {

                                    pesquisarAlunos(
                                        termo,
                                        pesquisaAtual
                                    );

                                },
                                250
                            );

                    }
                );

                input.addEventListener(
                    'focus',
                    function () {

                        const termo =
                            input.value.trim();

                        if (
                            termo.length > 0 &&
                            hiddenUsername.value.length === 0
                        ) {
                            numeroPesquisa++;

                            pesquisarAlunos(
                                termo,
                                numeroPesquisa
                            );
                        }

                    }
                );

                document.addEventListener(
                    'click',
                    function (evento) {

                        if (!wrapper.contains(
                            evento.target)) {
                            esconderSugestoes();
                        }

                    }
                );

                if (modal) {
                    modal.addEventListener(
                        'hidden.bs.modal',
                        function () {

                            input.value =
                                '';

                            hiddenUsername.value =
                                '';

                            esconderSugestoes();

                        }
                    );
                }

            }
        );
    </script>

</asp:Content>
