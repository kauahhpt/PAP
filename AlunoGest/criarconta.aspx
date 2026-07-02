<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="criarconta.aspx.cs" Inherits="AlunoGest.criarconta" %>

<!DOCTYPE html>

<html lang="pt-PT" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Meta Tags -->
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <meta name="description" content="Inovar Inovado - Criar Conta" />
    <meta name="theme-color" content="#343a40" />
    <title>Inovar Inovado - Criar Conta</title>

    <!-- Bootstrap CSS -->
    <link href="Content/bootstrap.min.css" rel="stylesheet" />

    <style>
        /* ========== VARIÁVEIS E RESET ========== */

        :root {
            --primary-dark: #343a40;
            --primary-light: #007bff;
            --secondary-gray: #6c757d;
            --light-bg: #f8f9fa;
            --border-color: #dee2e6;
            --danger-color: #dc3545;
            --success-color: #28a745;
        }

        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        html, body {
            height: 100%;
            width: 100%;
        }

        body {
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
            font-size: 16px;
            line-height: 1.6;
            color: #212529;
            background: linear-gradient(135deg, var(--primary-dark) 0%, #2c3e50 100%);
            min-height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
            padding: 1rem;
        }

        /* ========== CONTAINER DO FORMULÁRIO ========== */

        form {
            width: 100%;
            height: 100%;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .form-wrapper {
            width: 100%;
            max-width: 650px;
            padding: 2.5rem;
            background: white;
            border-radius: 12px;
            box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
            animation: slideIn 0.4s ease-out;
            max-height: 90vh;
            overflow-y: auto;
        }

        @keyframes slideIn {

            from {
                opacity: 0;
                transform: translateY(-20px);
            }

            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        /* ========== HEADER DO FORMULÁRIO ========== */

        .form-header {
            text-align: center;
            margin-bottom: 2rem;
            border-bottom: 2px solid var(--light-bg);
            padding-bottom: 1.5rem;
        }

            .form-header h1 {
                font-size: 1.75rem;
                font-weight: 700;
                color: var(--primary-dark);
                margin-bottom: 0.5rem;
            }

            .form-header p {
                font-size: 0.875rem;
                color: var(--secondary-gray);
                margin: 0;
            }

        /* ========== SEÇÕES DO FORMULÁRIO ========== */

        .form-section {
            margin-bottom: 2rem;
        }

        .form-section-title {
            font-size: 0.95rem;
            font-weight: 600;
            color: var(--primary-dark);
            margin-bottom: 1rem;
            padding-bottom: 0.5rem;
            border-bottom: 2px solid var(--light-bg);
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        /* ========== LINHAS DO FORMULÁRIO ========== */

        .form-row-custom {
            display: flex;
            flex-direction: column;
            gap: 0.5rem;
            margin-bottom: 1.5rem;
        }

            .form-row-custom.two-cols {
                display: grid;
                grid-template-columns: 1fr 1fr;
                gap: 1.5rem;
                align-items: start;
            }

        .form-label-custom {
            font-weight: 500;
            color: var(--primary-dark);
            font-size: 0.9rem;
        }

        .required {
            color: var(--danger-color);
            font-weight: bold;
        }

        /* ========== INPUTS ========== */

        .form-control {
            padding: 0.75rem 1rem;
            border: 1px solid var(--border-color);
            border-radius: 6px;
            font-size: 1rem;
            transition: all 0.3s ease;
            background-color: var(--light-bg);
            font-family: inherit;
            width: 100%;
        }

            .form-control:focus {
                outline: none;
                border-color: var(--primary-light);
                background-color: white;
                box-shadow: 0 0 0 3px rgba(0, 123, 255, 0.1);
            }

        textarea.form-control {
            resize: vertical;
            min-height: 80px;
        }

        /* ========== VALIDAÇÃO ========== */

        .validator-error {
            color: var(--danger-color);
            font-size: 0.85rem;
            margin-top: 0.25rem;
            display: block;
        }

        /* ========== BOTÕES ========== */

        .button-group {
            display: flex;
            gap: 1rem;
            margin-top: 2rem;
            justify-content: center;
            padding-top: 1.5rem;
            border-top: 1px solid var(--border-color);
        }

        .btn-custom {
            padding: 0.875rem 2rem;
            font-size: 1rem;
            font-weight: 600;
            border: none;
            border-radius: 6px;
            cursor: pointer;
            transition: all 0.3s ease;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 0.5rem;
            min-width: 140px;
        }

        .btn-success-custom {
            background-color: var(--success-color);
            color: white;
        }

            .btn-success-custom:hover,
            .btn-success-custom:focus {
                background-color: #218838;
                box-shadow: 0 4px 12px rgba(40, 167, 69, 0.3);
                transform: translateY(-2px);
                outline: 2px solid rgba(40, 167, 69, 0.4);
                outline-offset: 2px;
            }

            .btn-success-custom:active {
                transform: translateY(0);
            }

        .btn-secondary-custom {
            background-color: var(--secondary-gray);
            color: white;
        }

            .btn-secondary-custom:hover,
            .btn-secondary-custom:focus {
                background-color: #5a6268;
                box-shadow: 0 4px 12px rgba(108, 117, 125, 0.3);
                transform: translateY(-2px);
                outline: 2px solid rgba(108, 117, 125, 0.4);
                outline-offset: 2px;
            }

            .btn-secondary-custom:active {
                transform: translateY(0);
            }

        .btn-primary-custom {
            background-color: var(--primary-light);
            color: white;
        }

            .btn-primary-custom:hover,
            .btn-primary-custom:focus {
                background-color: #0056b3;
                box-shadow: 0 4px 12px rgba(0, 123, 255, 0.3);
                transform: translateY(-2px);
                outline: 2px solid rgba(0, 123, 255, 0.4);
                outline-offset: 2px;
            }

            .btn-primary-custom:active {
                transform: translateY(0);
            }

        /* ========== LINK DE LOGIN ========== */

        .login-link {
            text-align: center;
            margin-top: 1.5rem;
            padding-top: 1.5rem;
            border-top: 1px solid var(--border-color);
        }

            .login-link a {
                color: var(--primary-light);
                text-decoration: none;
                font-weight: 600;
                transition: all 0.3s ease;
            }

                .login-link a:hover,
                .login-link a:focus {
                    color: #0056b3;
                    text-decoration: underline;
                    outline: 2px solid rgba(0, 123, 255, 0.4);
                    outline-offset: 4px;
                    border-radius: 4px;
                }

        /* ========== RESPONSIVIDADE ========== */

        @media (max-width: 768px) {

            .form-wrapper {
                padding: 1.5rem;
            }

            .form-header h1 {
                font-size: 1.5rem;
            }

            .form-row-custom.two-cols {
                grid-template-columns: 1fr;
                gap: 1rem;
            }

            .button-group {
                flex-direction: column;
                gap: 0.75rem;
            }

            .btn-custom {
                width: 100%;
            }
        }

        @media (max-width: 480px) {

            .form-wrapper {
                padding: 1rem;
                max-height: 95vh;
            }

            .form-header h1 {
                font-size: 1.25rem;
            }

            .form-section-title {
                font-size: 0.85rem;
            }

            .form-control {
                font-size: 16px; /* Evita zoom em mobile */
            }

            .form-row-custom {
                margin-bottom: 1rem;
            }

            .button-group {
                flex-direction: column;
            }

            .btn-custom {
                width: 100%;
            }
        }

        /* ========== ACESSIBILIDADE ========== */

        @media (prefers-reduced-motion: reduce) {

            * {
                animation-duration: 0.01ms !important;
                animation-iteration-count: 1 !important;
                transition-duration: 0.01ms !important;
            }

            .form-wrapper {
                animation: none;
            }
        }

        @media (prefers-color-scheme: dark) {

            body {
                background: linear-gradient(135deg, #1a1a1a 0%, #0d1117 100%);
            }

            .form-wrapper {
                background-color: #2d2d2d;
                box-shadow: 0 10px 40px rgba(0, 0, 0, 0.5);
            }

            .form-header {
                border-bottom-color: #444;
            }

                .form-header h1 {
                    color: #e0e0e0;
                }

                .form-header p {
                    color: #b0b0b0;
                }

            .form-section-title {
                color: #e0e0e0;
                border-bottom-color: #444;
            }

            .form-label-custom {
                color: #e0e0e0;
            }

            .form-control {
                background-color: #3a3a3a;
                border-color: #555;
                color: #e0e0e0;
            }

                .form-control:focus {
                    background-color: #444;
                }

            .button-group {
                border-top-color: #555;
            }

            .login-link {
                border-top-color: #555;
            }
        }

        /* ========== IMPRESSÃO ========== */

        @media print {

            body {
                background: white;
            }

            .form-wrapper {
                box-shadow: none;
            }

            .button-group {
                display: none;
            }
        }

        /* ========== FOCUS VISÍVEL ========== */

        :focus-visible {
            outline: 2px solid var(--primary-light);
            outline-offset: 2px;
            border-radius: 2px;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <div class="form-wrapper">

            <!-- Header do Formulário -->
            <div class="form-header">
                <h1>Criar Conta</h1>
                <p>Preencha todos os campos para registar-se</p>
            </div>

            <!-- Painel do Formulário -->
            <asp:Panel runat="server">

                <!-- SEÇÃO: INFORMAÇÕES PESSOAIS -->
                <div class="form-section">
                    <div class="form-section-title">Informações Pessoais</div>

                    <!-- Nome -->
                    <div class="form-row-custom">
                        <label class="form-label-custom" for="txtNome">
                            Nome <span class="required">*</span>
                        </label>
                        <asp:TextBox ID="txtNome" runat="server"
                            CssClass="form-control"
                            placeholder="Digite seu nome completo" />
                        <asp:RequiredFieldValidator
                            runat="server"
                            ControlToValidate="txtNome"
                            ErrorMessage="O nome é obrigatório"
                            CssClass="validator-error" />
                    </div>

                    <!-- Email -->
                    <div class="form-row-custom">
                        <label class="form-label-custom" for="txtEmail">
                            Email <span class="required">*</span>
                        </label>
                        <asp:TextBox ID="txtEmail" runat="server"
                            CssClass="form-control"
                            TextMode="Email"
                            placeholder="seu.email@exemplo.com" />
                        <asp:RequiredFieldValidator
                            runat="server"
                            ControlToValidate="txtEmail"
                            ErrorMessage="O email é obrigatório"
                            CssClass="validator-error"
                            Display="Dynamic" />
                        <asp:RegularExpressionValidator
                            runat="server"
                            ControlToValidate="txtEmail"
                            ErrorMessage="Email inválido"
                            CssClass="validator-error"
                            ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                            Display="Dynamic" />
                    </div>

                    <!-- Telefone -->
                    <div class="form-row-custom">
                        <label class="form-label-custom" for="txtTelefone">
                            Telefone <span class="required">*</span>
                        </label>
                        <asp:TextBox ID="txtTelefone" runat="server"
                            CssClass="form-control"
                            placeholder="+351 9XXXXXXXX" />
                        <asp:RequiredFieldValidator
                            runat="server"
                            ControlToValidate="txtTelefone"
                            ErrorMessage="O telefone é obrigatório"
                            CssClass="validator-error" />
                    </div>
                </div>

                <!-- SEÇÃO: MORADA -->
                <div class="form-section">
                    <div class="form-section-title">Morada</div>

                    <!-- Morada -->
                    <div class="form-row-custom">
                        <label class="form-label-custom" for="txtMorada">
                            Morada <span class="required">*</span>
                        </label>
                        <asp:TextBox ID="txtMorada" runat="server"
                            CssClass="form-control"
                            TextMode="MultiLine"
                            Rows="3"
                            placeholder="Rua, número, complemento..." />
                        <asp:RequiredFieldValidator
                            runat="server"
                            ControlToValidate="txtMorada"
                            ErrorMessage="A morada é obrigatória"
                            CssClass="validator-error" />
                    </div>

                    <!-- Código Postal e Localidade (lado a lado) -->
                    <div class="form-row-custom two-cols">
                        <div>
                            <label class="form-label-custom" for="txtCodigoPostal">
                                Código Postal <span class="required">*</span>
                            </label>
                            <asp:TextBox ID="txtCodigoPostal" runat="server"
                                CssClass="form-control"
                                placeholder="1234-567" />
                            <asp:RequiredFieldValidator
                                runat="server"
                                ControlToValidate="txtCodigoPostal"
                                ErrorMessage="O código postal é obrigatório"
                                CssClass="validator-error" />
                        </div>

                        <div>
                            <label class="form-label-custom" for="txtLocalidade">
                                Localidade <span class="required">*</span>
                            </label>
                            <asp:TextBox ID="txtLocalidade" runat="server"
                                CssClass="form-control"
                                placeholder="Cidade ou município" />
                            <asp:RequiredFieldValidator
                                runat="server"
                                ControlToValidate="txtLocalidade"
                                ErrorMessage="A localidade é obrigatória"
                                CssClass="validator-error" />
                        </div>
                    </div>
                </div>

                <!-- SEÇÃO: INFORMAÇÕES PROFISSIONAIS -->
                <div class="form-section">
                    <div class="form-section-title">Informações Profissionais</div>

                    <!-- Código MEC -->
                    <div class="form-row-custom">
                        <label class="form-label-custom" for="txtCodigoMEC">Código MEC</label>
                        <asp:TextBox ID="txtCodigoMEC" runat="server"
                            CssClass="form-control"
                            placeholder="(Opcional)" />
                    </div>
                </div>

                <!-- SEÇÃO: CREDENCIAIS DE ACESSO -->
                <div class="form-section">
                    <div class="form-section-title">Credenciais de Acesso</div>

                    <!-- Username e Password (lado a lado) -->
                    <div class="form-row-custom two-cols">
                        <div>
                            <label class="form-label-custom" for="txtUsername">
                                Nome de Utilizador <span class="required">*</span>
                            </label>
                            <asp:TextBox ID="txtUsername" runat="server"
                                CssClass="form-control"
                                placeholder="seu_utilizador" />
                            <asp:RequiredFieldValidator
                                runat="server"
                                ControlToValidate="txtUsername"
                                ErrorMessage="O nome de utilizador é obrigatório"
                                CssClass="validator-error" />
                        </div>

                        <div>
                            <label class="form-label-custom" for="txtPassword">
                                Senha <span class="required">*</span>
                            </label>
                            <asp:TextBox ID="txtPassword" runat="server"
                                CssClass="form-control"
                                TextMode="Password"
                                placeholder="Mínimo 6 caracteres" />
                            <asp:RequiredFieldValidator
                                runat="server"
                                ControlToValidate="txtPassword"
                                ErrorMessage="A senha é obrigatória"
                                CssClass="validator-error" />
                        </div>
                    </div>
                </div>

                <!-- BOTÕES -->
                <div class="button-group">
                    <asp:Button ID="btnCriarConta" runat="server"
                        Text="Criar Conta"
                        CssClass="btn-custom btn-success-custom"
                        OnClick="btnCriarConta_Click" />

                    <asp:Button ID="btnCancelar" runat="server"
                        Text="Cancelar"
                        CssClass="btn-custom btn-secondary-custom"
                        CausesValidation="false" />

                    <asp:HyperLink ID="lnkVoltar" runat="server"
                        NavigateUrl="login.aspx"
                        CssClass="btn-custom btn-primary-custom"
                        Text="Voltar para Login" />
                </div>

                <!-- Link para Login -->
                <div class="login-link">
                    <span>Já tem conta? </span>
                    <asp:HyperLink ID="lnkIrLogin" runat="server" NavigateUrl="login.aspx">Iniciar Sessão</asp:HyperLink>
                </div>

            </asp:Panel>

        </div>
    </form>

    <!-- Bootstrap JS -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js" defer></script>
</body>

</html>

