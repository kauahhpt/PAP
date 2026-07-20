<%@ Page
    Language="C#"
    AutoEventWireup="true"
    CodeBehind="definir_password.aspx.cs"
    Inherits="AlunoGest.definir_password" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <meta charset="utf-8" />

    <meta
        name="viewport"
        content="width=device-width, initial-scale=1" />

    <title>Definir nova palavra-passe</title>

    <link
        href="Content/bootstrap.min.css"
        rel="stylesheet" />

    <asp:Literal
        ID="ltlRedirect"
        runat="server"
        EnableViewState="false" />

    <style>
        * {
            box-sizing: border-box;
        }

        body {
            min-height: 100vh;
            margin: 0;

            background:
                linear-gradient(
                    135deg,
                    #eef4ff,
                    #f8fafc,
                    #e8f0ff
                );

            color: #1e293b;

            font-family:
                "Segoe UI",
                Tahoma,
                Geneva,
                Verdana,
                sans-serif;
        }

        .reset-page {
            min-height: 100vh;

            display: flex;
            align-items: center;
            justify-content: center;

            padding: 30px 18px;
        }

        .reset-card {
            width: 100%;
            max-width: 540px;

            overflow: hidden;

            border: 1px solid rgba(15, 23, 42, 0.09);
            border-radius: 20px;

            background: #ffffff;

            box-shadow:
                0 24px 60px rgba(15, 23, 42, 0.12),
                0 4px 12px rgba(15, 23, 42, 0.06);
        }

        .reset-header {
            padding: 30px 32px 22px;

            border-bottom: 1px solid #e8edf4;

            text-align: center;
        }

        .reset-icon {
            width: 62px;
            height: 62px;

            margin: 0 auto 17px;

            display: flex;
            align-items: center;
            justify-content: center;

            border-radius: 18px;

            background:
                linear-gradient(
                    135deg,
                    #173b70,
                    #2563eb
                );

            color: #ffffff;

            font-size: 27px;
            font-weight: 800;

            box-shadow:
                0 10px 24px rgba(37, 99, 235, 0.25);
        }

        .reset-header h1 {
            margin: 0 0 8px;

            color: #172033;

            font-size: 27px;
            font-weight: 800;
        }

        .reset-header p {
            margin: 0;

            color: #64748b;

            font-size: 14px;
            line-height: 1.6;
        }

        .reset-body {
            padding: 27px 32px 32px;
        }

        .form-label {
            display: block;

            margin-bottom: 7px;

            color: #334155;

            font-size: 14px;
            font-weight: 700;
        }

        .form-control {
            min-height: 48px;

            border-color: #d5deea;
            border-radius: 11px;

            transition:
                border-color 0.2s ease,
                box-shadow 0.2s ease;
        }

        .form-control:focus {
            border-color: #5b8def;

            box-shadow:
                0 0 0 3px rgba(37, 99, 235, 0.12);
        }

        /* Caixa que contém o campo e o olho */

        .password-input-wrapper {
            position: relative;
        }

        .password-input-wrapper .form-control {
            padding-right: 52px;
        }

        /* Botão do olho */

        .password-toggle {
            position: absolute;

            top: 50%;
            right: 10px;

            width: 36px;
            height: 36px;

            display: flex;
            align-items: center;
            justify-content: center;

            padding: 0;

            border: none;
            border-radius: 9px;

            background: transparent;
            color: #64748b;

            cursor: pointer;

            transform: translateY(-50%);

            transition:
                color 0.2s ease,
                background-color 0.2s ease,
                box-shadow 0.2s ease;
        }

        .password-toggle:hover {
            background-color: #eff6ff;
            color: #1d4ed8;
        }

        .password-toggle:focus {
            outline: none;

            box-shadow:
                0 0 0 3px rgba(37, 99, 235, 0.15);
        }

        .password-toggle svg {
            width: 21px;
            height: 21px;

            pointer-events: none;
        }

        .password-toggle .icon-hidden {
            display: none;
        }

        /* Regras da password */

        .password-rules {
            margin: 16px 0 20px;
            padding: 13px 15px;

            border: 1px solid #e8edf4;
            border-radius: 11px;

            background: #f8fafc;
            color: #64748b;

            font-size: 12px;
            line-height: 1.6;
        }

        .password-rules strong {
            color: #475569;
        }

        /* Botão principal */

        .reset-button {
            width: 100%;
            min-height: 48px;

            margin-top: 8px;

            border: none;
            border-radius: 11px;

            background: #1d4ed8;

            font-weight: 700;

            transition:
                background-color 0.2s ease,
                transform 0.2s ease,
                box-shadow 0.2s ease;
        }

        .reset-button:hover {
            background: #1e40af;

            transform: translateY(-1px);

            box-shadow:
                0 8px 18px rgba(37, 99, 235, 0.20);
        }

        .reset-button:disabled {
            background: #94a3b8;

            cursor: not-allowed;
            transform: none;
            box-shadow: none;
        }

        /* Validadores */

        .validation-message {
            display: block;

            margin-top: 6px;

            color: #dc2626;

            font-size: 12px;
            font-weight: 600;
        }

        .validation-summary {
            margin-bottom: 20px;

            border-radius: 11px;

            font-size: 13px;
        }

        .validation-summary ul {
            margin-top: 8px;
            margin-bottom: 0;
        }

        /* Mensagens do servidor */

        .message-area {
            margin-top: 20px;
        }

        .message-area .alert {
            margin-bottom: 0;

            border-radius: 11px;

            font-size: 13px;
            line-height: 1.5;
        }

        /* Voltar */

        .back-link {
            display: block;

            margin-top: 21px;

            color: #2563eb;

            font-size: 14px;
            font-weight: 700;
            text-align: center;
            text-decoration: none;
        }

        .back-link:hover {
            color: #1e40af;
            text-decoration: underline;
        }

        @media (max-width: 540px) {
            .reset-header,
            .reset-body {
                padding-right: 21px;
                padding-left: 21px;
            }

            .reset-header h1 {
                font-size: 23px;
            }
        }
    </style>

</head>

<body>

    <form
        id="form1"
        runat="server">

        <div class="reset-page">

            <main class="reset-card">

                <header class="reset-header">

                    <div class="reset-icon">
                        ✓
                    </div>

                    <h1>
                        Definir nova palavra-passe
                    </h1>

                    <p>
                        Escolha uma palavra-passe segura
                        para voltar a aceder à sua conta.
                    </p>

                </header>

                <div class="reset-body">

                    <asp:ValidationSummary
                        ID="ValidationSummary1"
                        runat="server"
                        ValidationGroup="password"
                        CssClass="alert alert-warning validation-summary"
                        HeaderText="Verifique os seguintes campos:"
                        DisplayMode="BulletList"
                        ShowSummary="true" />


                    <!-- NOVA PALAVRA-PASSE -->

                    <div class="mb-3">

                        <asp:Label
                            ID="LblPassword"
                            runat="server"
                            AssociatedControlID="textPassword"
                            CssClass="form-label"
                            Text="Nova palavra-passe" />

                        <div class="password-input-wrapper">

                            <asp:TextBox
                                ID="textPassword"
                                runat="server"
                                ClientIDMode="Static"
                                CssClass="form-control"
                                TextMode="Password"
                                MaxLength="100"
                                autocomplete="new-password"
                                placeholder="Introduza a nova palavra-passe" />

                            <button
                                type="button"
                                class="password-toggle"
                                data-target="textPassword"
                                aria-label="Mostrar palavra-passe"
                                aria-pressed="false"
                                title="Mostrar palavra-passe">

                                <!-- Olho aberto -->

                                <svg
                                    class="eye-open"
                                    viewBox="0 0 24 24"
                                    fill="none"
                                    stroke="currentColor"
                                    stroke-width="2"
                                    stroke-linecap="round"
                                    stroke-linejoin="round"
                                    aria-hidden="true">

                                    <path
                                        d="M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7S2 12 2 12" />

                                    <circle
                                        cx="12"
                                        cy="12"
                                        r="3" />

                                </svg>

                                <!-- Olho fechado -->

                                <svg
                                    class="eye-closed icon-hidden"
                                    viewBox="0 0 24 24"
                                    fill="none"
                                    stroke="currentColor"
                                    stroke-width="2"
                                    stroke-linecap="round"
                                    stroke-linejoin="round"
                                    aria-hidden="true">

                                    <path d="M3 3l18 18" />

                                    <path
                                        d="M10.6 10.7a2 2 0 0 0 2.7 2.7" />

                                    <path
                                        d="M9.9 4.2A10.8 10.8 0 0 1 12 4c6.5 0 10 8 10 8a18 18 0 0 1-2.1 3.2" />

                                    <path
                                        d="M6.6 6.6C3.6 8.5 2 12 2 12s3.5 8 10 8a10.5 10.5 0 0 0 5.4-1.5" />

                                </svg>

                            </button>

                        </div>

                        <asp:RequiredFieldValidator
                            ID="RfvPassword"
                            runat="server"
                            ControlToValidate="textPassword"
                            ValidationGroup="password"
                            Display="Dynamic"
                            CssClass="validation-message"
                            ErrorMessage="Introduza a nova palavra-passe." />

                    </div>


                    <!-- CONFIRMAR PALAVRA-PASSE -->

                    <div class="mb-3">

                        <asp:Label
                            ID="LblPasswordRepeticao"
                            runat="server"
                            AssociatedControlID="textPasswordRepeticao"
                            CssClass="form-label"
                            Text="Confirmar palavra-passe" />

                        <div class="password-input-wrapper">

                            <asp:TextBox
                                ID="textPasswordRepeticao"
                                runat="server"
                                ClientIDMode="Static"
                                CssClass="form-control"
                                TextMode="Password"
                                MaxLength="100"
                                autocomplete="new-password"
                                placeholder="Introduza novamente a palavra-passe" />

                            <button
                                type="button"
                                class="password-toggle"
                                data-target="textPasswordRepeticao"
                                aria-label="Mostrar palavra-passe"
                                aria-pressed="false"
                                title="Mostrar palavra-passe">

                                <!-- Olho aberto -->

                                <svg
                                    class="eye-open"
                                    viewBox="0 0 24 24"
                                    fill="none"
                                    stroke="currentColor"
                                    stroke-width="2"
                                    stroke-linecap="round"
                                    stroke-linejoin="round"
                                    aria-hidden="true">

                                    <path
                                        d="M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7S2 12 2 12" />

                                    <circle
                                        cx="12"
                                        cy="12"
                                        r="3" />

                                </svg>

                                <!-- Olho fechado -->

                                <svg
                                    class="eye-closed icon-hidden"
                                    viewBox="0 0 24 24"
                                    fill="none"
                                    stroke="currentColor"
                                    stroke-width="2"
                                    stroke-linecap="round"
                                    stroke-linejoin="round"
                                    aria-hidden="true">

                                    <path d="M3 3l18 18" />

                                    <path
                                        d="M10.6 10.7a2 2 0 0 0 2.7 2.7" />

                                    <path
                                        d="M9.9 4.2A10.8 10.8 0 0 1 12 4c6.5 0 10 8 10 8a18 18 0 0 1-2.1 3.2" />

                                    <path
                                        d="M6.6 6.6C3.6 8.5 2 12 2 12s3.5 8 10 8a10.5 10.5 0 0 0 5.4-1.5" />

                                </svg>

                            </button>

                        </div>

                        <asp:RequiredFieldValidator
                            ID="RfvPasswordRepeticao"
                            runat="server"
                            ControlToValidate="textPasswordRepeticao"
                            ValidationGroup="password"
                            Display="Dynamic"
                            CssClass="validation-message"
                            ErrorMessage="Confirme a nova palavra-passe." />

                        <asp:CompareValidator
                            ID="ComparePasswords"
                            runat="server"
                            ControlToValidate="textPasswordRepeticao"
                            ControlToCompare="textPassword"
                            ValidationGroup="password"
                            Operator="Equal"
                            Type="String"
                            Display="Dynamic"
                            CssClass="validation-message"
                            ErrorMessage="As palavras-passe não coincidem." />

                    </div>


                    <!-- REGRAS -->

                    <div class="password-rules">

                        <strong>
                            A palavra-passe deve possuir:
                        </strong>

                        <br />

                        Pelo menos oito caracteres, uma letra maiúscula,
                        uma letra minúscula, um número e um carácter especial.

                    </div>


                    <!-- BOTÃO -->

                    <asp:Button
                        ID="btnReset"
                        runat="server"
                        Text="Guardar nova palavra-passe"
                        CssClass="btn btn-primary reset-button"
                        ValidationGroup="password"
                        OnClick="btnReset_Click" />


                    <!-- MENSAGEM -->

                    <div class="message-area">

                        <asp:Label
                            ID="lblMessage"
                            runat="server"
                            Visible="false" />

                    </div>


                    <!-- VOLTAR -->

                    <a
                        class="back-link"
                        href="<%= ResolveUrl("~/login.aspx") %>">

                        Voltar ao início de sessão

                    </a>

                </div>

            </main>

        </div>

    </form>


    <script>
        document.addEventListener(
            "DOMContentLoaded",
            function () {

                var botoes =
                    document.querySelectorAll(
                        ".password-toggle"
                    );

                botoes.forEach(
                    function (botao) {

                        botao.addEventListener(
                            "click",
                            function () {

                                var targetId =
                                    botao.getAttribute(
                                        "data-target"
                                    );

                                var campo =
                                    document.getElementById(
                                        targetId
                                    );

                                if (!campo) {
                                    return;
                                }

                                var passwordEstaOculta =
                                    campo.type === "password";

                                campo.type =
                                    passwordEstaOculta
                                        ? "text"
                                        : "password";

                                var olhoAberto =
                                    botao.querySelector(
                                        ".eye-open"
                                    );

                                var olhoFechado =
                                    botao.querySelector(
                                        ".eye-closed"
                                    );

                                if (passwordEstaOculta) {
                                    olhoAberto.classList.add(
                                        "icon-hidden"
                                    );

                                    olhoFechado.classList.remove(
                                        "icon-hidden"
                                    );
                                }
                                else {
                                    olhoAberto.classList.remove(
                                        "icon-hidden"
                                    );

                                    olhoFechado.classList.add(
                                        "icon-hidden"
                                    );
                                }

                                var textoBotao =
                                    passwordEstaOculta
                                        ? "Ocultar palavra-passe"
                                        : "Mostrar palavra-passe";

                                botao.setAttribute(
                                    "aria-label",
                                    textoBotao
                                );

                                botao.setAttribute(
                                    "title",
                                    textoBotao
                                );

                                botao.setAttribute(
                                    "aria-pressed",
                                    passwordEstaOculta
                                        ? "true"
                                        : "false"
                                );

                                campo.focus();
                            }
                        );

                    }
                );

            }
        );
    </script>

</body>

</html>