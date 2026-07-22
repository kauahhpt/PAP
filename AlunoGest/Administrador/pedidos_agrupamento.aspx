<%@ Page
    Language="C#"
    AutoEventWireup="true"
    CodeBehind="pedidos_agrupamento.aspx.cs"
    Inherits="AlunoGest.administrador.pedidos_agrupamento" %>

<!DOCTYPE html>

<html lang="pt-PT"
    xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <meta charset="utf-8" />

    <meta
        name="viewport"
        content="width=device-width, initial-scale=1" />

    <title>
        Inovar Inovado | Pedidos de agrupamento
    </title>

    <link
        href="../Content/bootstrap.min.css"
        rel="stylesheet" />

    <style>

        /* =====================================================
   JANELA DE REJEIÇÃO
===================================================== */

.modal-overlay {
    position: fixed;
    inset: 0;
    z-index: 1000;

    display: flex;
    align-items: center;
    justify-content: center;

    padding: 20px;

    background: rgba(15, 23, 42, 0.58);
    backdrop-filter: blur(3px);
}

.rejection-modal {
    width: 100%;
    max-width: 520px;

    overflow: hidden;

    border: 1px solid rgba(15, 23, 42, 0.10);
    border-radius: 17px;

    background: #ffffff;

    box-shadow:
        0 25px 65px rgba(15, 23, 42, 0.28);
}

.rejection-modal-header {
    padding: 22px 24px 18px;

    border-bottom: 1px solid #edf0f5;
}

.rejection-modal-header h2 {
    margin: 0 0 7px;

    color: #991b1b;

    font-size: 20px;
    font-weight: 800;
}

.rejection-modal-header p {
    margin: 0;

    color: #64748b;

    font-size: 13px;
    line-height: 1.6;
}

.rejection-modal-body {
    padding: 22px 24px;
}

.rejection-label {
    display: block;

    margin-bottom: 8px;

    color: #334155;

    font-size: 13px;
    font-weight: 700;
}

.rejection-textarea {
    width: 100%;
    min-height: 130px;

    padding: 12px 13px;

    border: 1px solid #d8dee9;
    border-radius: 10px;

    background: #f8fafc;
    color: #1f2937;

    font-family: inherit;
    font-size: 14px;
    line-height: 1.5;

    resize: vertical;
    outline: none;
}

.rejection-textarea:focus {
    border-color: #ef4444;
    background: #ffffff;

    box-shadow:
        0 0 0 3px rgba(239, 68, 68, 0.12);
}

.rejection-validator {
    margin-top: 6px;

    color: #dc2626;

    font-size: 12px;
    font-weight: 600;
}

.rejection-counter-note {
    margin-top: 7px;

    color: #94a3b8;

    font-size: 11px;
}

.rejection-modal-actions {
    display: flex;
    justify-content: flex-end;

    gap: 10px;

    padding: 17px 24px;

    border-top: 1px solid #edf0f5;

    background: #f8fafc;
}

.btn-modal-cancel,
.btn-modal-confirm {
    min-height: 41px;

    padding: 9px 16px;

    border-radius: 9px;

    font-size: 13px;
    font-weight: 700;

    cursor: pointer;
}

.btn-modal-cancel {
    border: 1px solid #cbd5e1;

    background: #ffffff;
    color: #475569;
}

.btn-modal-cancel:hover {
    background: #f1f5f9;
    color: #1f2937;
}

.btn-modal-confirm {
    border: 1px solid #dc2626;

    background: #dc2626;
    color: #ffffff;
}

.btn-modal-confirm:hover {
    border-color: #b91c1c;

    background: #b91c1c;
}

@media (max-width: 560px) {
    .rejection-modal-header,
    .rejection-modal-body,
    .rejection-modal-actions {
        padding-right: 18px;
        padding-left: 18px;
    }

    .rejection-modal-actions {
        flex-direction: column-reverse;
    }

    .btn-modal-cancel,
    .btn-modal-confirm {
        width: 100%;
    }
}

        * {
            box-sizing: border-box;
        }

        html,
        body {
            width: 100%;
            min-height: 100%;
            margin: 0;
        }

        body {
            min-height: 100vh;

            background: #f3f6fb;
            color: #1f2937;

            font-family:
                "Segoe UI",
                Tahoma,
                Geneva,
                Verdana,
                sans-serif;
        }

        form {
            min-height: 100vh;
        }


        /* =====================================================
           CABEÇALHO
        ===================================================== */

        .admin-header {
            background:
                linear-gradient(
                    135deg,
                    #0f2c61,
                    #1d4ed8
                );

            color: #ffffff;

            box-shadow:
                0 5px 18px rgba(15, 44, 97, 0.18);
        }

        .admin-header-content {
            width: 100%;
            max-width: 1450px;

            min-height: 76px;

            display: flex;
            align-items: center;
            justify-content: space-between;

            gap: 20px;

            margin: 0 auto;
            padding: 14px 28px;
        }

        .admin-brand {
            display: flex;
            align-items: center;

            gap: 13px;
        }

        .brand-logo {
            width: 44px;
            height: 44px;

            display: flex;
            align-items: center;
            justify-content: center;

            border:
                1px solid rgba(255, 255, 255, 0.22);

            border-radius: 12px;

            background:
                rgba(255, 255, 255, 0.14);

            font-size: 17px;
            font-weight: 800;
        }

        .brand-text strong {
            display: block;

            font-size: 17px;
            font-weight: 800;
        }

        .brand-text span {
            color: #dbeafe;

            font-size: 12px;
        }

        .logout-link {
            padding: 9px 14px;

            border:
                1px solid rgba(255, 255, 255, 0.28);

            border-radius: 9px;

            color: #ffffff;

            font-size: 13px;
            font-weight: 700;
            text-decoration: none;

            transition:
                background-color 0.2s,
                border-color 0.2s;
        }

        .logout-link:hover {
            border-color:
                rgba(255, 255, 255, 0.45);

            background:
                rgba(255, 255, 255, 0.12);

            color: #ffffff;
        }


        /* =====================================================
           CONTEÚDO
        ===================================================== */

        .admin-page {
            width: 100%;
            max-width: 1450px;

            margin: 0 auto;
            padding: 38px 28px;
        }

        .page-heading {
            display: flex;
            align-items: flex-start;
            justify-content: space-between;

            gap: 22px;

            margin-bottom: 27px;
        }

        .page-title-area h1 {
            margin: 0 0 7px;

            color: #111827;

            font-size: 30px;
            font-weight: 800;

            letter-spacing: -0.02em;
        }

        .page-title-area p {
            max-width: 760px;

            margin: 0;

            color: #64748b;

            font-size: 14px;
            line-height: 1.6;
        }

        .status-badge {
            display: inline-flex;
            align-items: center;

            gap: 8px;

            padding: 10px 14px;

            border: 1px solid #fde68a;
            border-radius: 10px;

            background: #fffbeb;
            color: #92400e;

            font-size: 13px;
            font-weight: 700;
        }

        .status-dot {
            width: 8px;
            height: 8px;

            border-radius: 50%;

            background: #f59e0b;
        }


        /* =====================================================
           MENSAGENS
        ===================================================== */

        .message-success,
        .message-error,
        .message-warning {
            display: block;

            margin-bottom: 20px;
            padding: 13px 15px;

            border-radius: 10px;

            font-size: 14px;
            line-height: 1.5;
        }

        .message-success {
            border: 1px solid #bbf7d0;

            background: #f0fdf4;
            color: #166534;
        }

        .message-error {
            border: 1px solid #fecaca;

            background: #fef2f2;
            color: #b91c1c;
        }

        .message-warning {
            border: 1px solid #fde68a;

            background: #fffbeb;
            color: #92400e;
        }


        /* =====================================================
           CARD DA TABELA
        ===================================================== */

        .requests-card {
            overflow: hidden;

            border:
                1px solid rgba(15, 23, 42, 0.08);

            border-radius: 17px;

            background: #ffffff;

            box-shadow:
                0 13px 35px rgba(15, 23, 42, 0.07);
        }

        .card-header-custom {
            display: flex;
            align-items: center;
            justify-content: space-between;

            gap: 15px;

            padding: 21px 23px;

            border-bottom: 1px solid #edf0f5;
        }

        .card-header-custom h2 {
            margin: 0;

            color: #1f2937;

            font-size: 17px;
            font-weight: 800;
        }

        .card-header-custom p {
            margin: 4px 0 0;

            color: #94a3b8;

            font-size: 12px;
        }

        .refresh-button {
            min-height: 38px;

            padding: 8px 14px;

            border: 1px solid #cbd5e1;
            border-radius: 9px;

            background: #ffffff;
            color: #475569;

            font-size: 13px;
            font-weight: 700;

            cursor: pointer;
        }

        .refresh-button:hover {
            border-color: #93c5fd;

            background: #eff6ff;
            color: #1d4ed8;
        }

        .table-container {
            width: 100%;

            overflow-x: auto;
        }


        /* =====================================================
           GRIDVIEW
        ===================================================== */

        .requests-grid {
            width: 100%;

            border-collapse: collapse;

            min-width: 1150px;
        }

        .requests-grid th {
            padding: 13px 15px;

            border-bottom: 1px solid #e5e7eb;

            background: #f8fafc;
            color: #475569;

            font-size: 11px;
            font-weight: 800;
            text-align: left;
            text-transform: uppercase;

            letter-spacing: 0.04em;

            white-space: nowrap;
        }

        .requests-grid td {
            padding: 15px;

            border-bottom: 1px solid #edf0f5;

            color: #334155;

            font-size: 13px;
            vertical-align: middle;
        }

        .requests-grid tr:last-child td {
            border-bottom: 0;
        }

        .requests-grid tr:hover td {
            background: #f8fbff;
        }

        .request-name {
            color: #1f2937;

            font-weight: 750;
        }

        .request-secondary {
            display: block;

            margin-top: 3px;

            color: #94a3b8;

            font-size: 11px;
        }

        .pending-badge {
            display: inline-flex;
            align-items: center;

            padding: 5px 9px;

            border: 1px solid #fde68a;
            border-radius: 999px;

            background: #fffbeb;
            color: #92400e;

            font-size: 11px;
            font-weight: 750;
        }

        .actions-container {
            display: flex;
            align-items: center;

            gap: 7px;

            white-space: nowrap;
        }

        .btn-approve,
        .btn-reject {
            min-height: 35px;

            padding: 7px 11px;

            border-radius: 8px;

            font-size: 12px;
            font-weight: 700;

            cursor: pointer;
        }

        .btn-approve {
            border: 1px solid #86efac;

            background: #f0fdf4;
            color: #166534;
        }

        .btn-approve:hover {
            border-color: #22c55e;

            background: #dcfce7;
        }

        .btn-reject {
            border: 1px solid #fecaca;

            background: #fef2f2;
            color: #b91c1c;
        }

        .btn-reject:hover {
            border-color: #f87171;

            background: #fee2e2;
        }


        /* =====================================================
           GRID VAZIA
        ===================================================== */

        .empty-area {
            padding: 55px 20px;

            color: #64748b;

            text-align: center;
        }

        .empty-icon {
            width: 55px;
            height: 55px;

            display: flex;
            align-items: center;
            justify-content: center;

            margin: 0 auto 15px;

            border-radius: 15px;

            background: #eff6ff;
            color: #2563eb;

            font-size: 25px;
            font-weight: 800;
        }

        .empty-area strong {
            display: block;

            margin-bottom: 5px;

            color: #334155;

            font-size: 15px;
        }

        .empty-area span {
            font-size: 13px;
        }


        /* =====================================================
           RODAPÉ
        ===================================================== */

        .admin-footer {
            margin-top: 22px;

            color: #94a3b8;

            font-size: 12px;
            text-align: center;
        }


        /* =====================================================
           RESPONSIVO
        ===================================================== */

        @media (max-width: 800px) {

            .admin-header-content {
                padding:
                    13px 17px;
            }

            .admin-page {
                padding:
                    28px 17px;
            }

            .page-heading {
                flex-direction: column;
            }

            .page-title-area h1 {
                font-size: 25px;
            }

            .status-badge {
                width: 100%;

                justify-content: center;
            }

            .card-header-custom {
                align-items: flex-start;
                flex-direction: column;
            }

            .refresh-button {
                width: 100%;
            }
        }

    </style>

</head>

<body>

    <form
        id="form1"
        runat="server">


        <!-- CABEÇALHO -->

        <header class="admin-header">

            <div class="admin-header-content">

                <div class="admin-brand">

                    <div class="brand-logo">
                        II
                    </div>

                    <div class="brand-text">

                        <strong>
                            Inovar Inovado
                        </strong>

                        <span>
                            Administração da plataforma
                        </span>

                    </div>

                </div>

                <asp:LoginStatus
                    ID="LoginStatus1"
                    runat="server"
                    LogoutPageUrl="~/login.aspx"
                    LogoutText="Terminar sessão"
                    CssClass="logout-link" />

            </div>

        </header>


        <!-- CONTEÚDO -->

        <main class="admin-page">

            <div class="page-heading">

                <div class="page-title-area">

                    <h1>
                        Pedidos de agrupamento
                    </h1>

                    <p>
                        Analise os pedidos enviados por instituições.
                        A conta do agrupamento só será criada depois
                        da aprovação do administrador.
                    </p>

                </div>

                <div class="status-badge">

                    <span class="status-dot"></span>

                    Pedidos pendentes

                </div>

            </div>


            <asp:Label
                ID="lblMensagem"
                runat="server"
                Visible="false" />


            <section class="requests-card">

                <div class="card-header-custom">

                    <div>

                        <h2>
                            Pedidos recebidos
                        </h2>

                        <p>
                            Apenas os pedidos que aguardam decisão são apresentados.
                        </p>

                    </div>

                    <asp:Button
                        ID="btnAtualizar"
                        runat="server"
                        Text="Atualizar lista"
                        CssClass="refresh-button"
                        CausesValidation="false" />

                </div>


                <div class="table-container">

                    <asp:GridView
                        ID="GridPedidos"
                        runat="server"
                        AutoGenerateColumns="false"
                        CssClass="requests-grid"
                        GridLines="None"
                        DataKeyNames="IdPedido"
                        EmptyDataText="">

                        <Columns>




                            <asp:TemplateField HeaderText="Agrupamento">

                                <ItemTemplate>

                                    <span class="request-name">

                                        <%# Eval("NomeAgrupamento") %>

                                    </span>

                                    <span class="request-secondary">

                                        Pedido n.º
                                        <%# Eval("IdPedido") %>

                                    </span>

                                </ItemTemplate>

                            </asp:TemplateField>




                            <asp:BoundField
                                DataField="NomeResponsavel"
                                HeaderText="Responsável" />



                            <asp:BoundField
                                DataField="Email"
                                HeaderText="Email" />



                            <asp:BoundField
                                DataField="Telefone"
                                HeaderText="Telefone" />



                            <asp:BoundField
                                DataField="NIF"
                                HeaderText="NIF" />



                            <asp:BoundField
                                DataField="Localidade"
                                HeaderText="Localidade" />



                            <asp:BoundField
                                DataField="DataPedido"
                                HeaderText="Data"
                                DataFormatString="{0:dd/MM/yyyy HH:mm}" />



                            <asp:TemplateField HeaderText="Estado">

                                <ItemTemplate>

                                    <span class="pending-badge">

                                        <%# Eval("Estado") %>

                                    </span>

                                </ItemTemplate>

                            </asp:TemplateField>



                            <asp:TemplateField HeaderText="Ações">

                                <ItemTemplate>

                                    <div class="actions-container">

                                        <asp:Button
                                            ID="btnAprovar"
                                            runat="server"
                                            Text="Aprovar"
                                            CommandName="AprovarPedido"
                                            CommandArgument='<%# Eval("IdPedido") %>'
                                            CssClass="btn-approve"
                                            CausesValidation="false" />

                                        <asp:Button
                                            ID="btnRejeitar"
                                            runat="server"
                                            Text="Rejeitar"
                                            CommandName="RejeitarPedido"
                                            CommandArgument='<%# Eval("IdPedido") %>'
                                            CssClass="btn-reject"
                                            CausesValidation="false" />

                                    </div>

                                </ItemTemplate>

                            </asp:TemplateField>


                        </Columns>


                        <EmptyDataTemplate>

                            <div class="empty-area">

                                <div class="empty-icon">
                                    ✓
                                </div>

                                <strong>
                                    Não existem pedidos pendentes
                                </strong>

                                <span>
                                    Os novos pedidos aparecerão aqui.
                                </span>

                            </div>

                        </EmptyDataTemplate>

                    </asp:GridView>

                </div>

            </section>


            <div class="admin-footer">

                © 2026 Inovar Inovado —
                Plataforma de Gestão Escolar

            </div>

        </main>

        <!-- ==================================================
     JANELA DE REJEIÇÃO
=================================================== -->

<asp:Panel
    ID="pnlRejeicao"
    runat="server"
    CssClass="modal-overlay"
    Visible="false">

    <section
        class="rejection-modal"
        role="dialog"
        aria-modal="true"
        aria-labelledby="tituloRejeicao">

        <header class="rejection-modal-header">

            <h2 id="tituloRejeicao">
                Rejeitar pedido
            </h2>

            <p>
                Indique o motivo pelo qual o pedido não pode ser aprovado.
                Esta informação será enviada por email ao responsável.
            </p>

        </header>

        <div class="rejection-modal-body">

            <asp:HiddenField
                ID="hdnPedidoRejeitar"
                runat="server" />

            <asp:ValidationSummary
                ID="ValidationSummaryRejeicao"
                runat="server"
                ValidationGroup="rejeicao"
                CssClass="alert alert-warning"
                HeaderText="Verifique o motivo da rejeição:"
                DisplayMode="BulletList" />

            <asp:Label
                ID="LblMotivoRejeicao"
                runat="server"
                AssociatedControlID="txtMotivoRejeicao"
                CssClass="rejection-label"
                Text="Motivo da rejeição" />

            <asp:TextBox
                ID="txtMotivoRejeicao"
                runat="server"
                TextMode="MultiLine"
                Rows="5"
                MaxLength="500"
                CssClass="rejection-textarea"
                placeholder="Explique de forma clara o motivo da rejeição..." />

            <asp:RequiredFieldValidator
                ID="RfvMotivoRejeicao"
                runat="server"
                ControlToValidate="txtMotivoRejeicao"
                ValidationGroup="rejeicao"
                Display="Dynamic"
                CssClass="rejection-validator"
                ErrorMessage="O motivo da rejeição é obrigatório." />

            <div class="rejection-counter-note">
                Máximo de 500 caracteres.
            </div>

        </div>

        <footer class="rejection-modal-actions">

            <asp:Button
                ID="btnCancelarRejeicao"
                runat="server"
                Text="Cancelar"
                CssClass="btn-modal-cancel"
                CausesValidation="false"
                OnClick="btnCancelarRejeicao_Click" />

            <asp:Button
                ID="btnConfirmarRejeicao"
                runat="server"
                Text="Confirmar rejeição"
                CssClass="btn-modal-confirm"
                ValidationGroup="rejeicao"
                CausesValidation="true"
                OnClick="btnConfirmarRejeicao_Click" />

        </footer>

    </section>

</asp:Panel>

    </form>

</body>

</html>