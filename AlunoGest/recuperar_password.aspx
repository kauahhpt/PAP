<%@ Page
    Language="C#"
    AutoEventWireup="true"
    CodeBehind="recuperar_password.aspx.cs"
    Inherits="AlunoGest.recuperar_password" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <meta charset="utf-8" />

    <meta
        name="viewport"
        content="width=device-width, initial-scale=1" />

    <title>Recuperar palavra-passe</title>

    <link
        href="Content/bootstrap.min.css"
        rel="stylesheet" />

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
                    #eef4ff 0%,
                    #f8fafc 50%,
                    #e8f0ff 100%
                );

            color: #1e293b;

            font-family:
                "Segoe UI",
                Tahoma,
                Geneva,
                Verdana,
                sans-serif;
        }

        .recovery-page {
            min-height: 100vh;

            display: flex;
            align-items: center;
            justify-content: center;

            padding: 30px 18px;
        }

        .recovery-card {
            width: 100%;
            max-width: 500px;

            overflow: hidden;

            border: 1px solid rgba(15, 23, 42, 0.09);
            border-radius: 20px;

            background: #ffffff;

            box-shadow:
                0 24px 60px rgba(15, 23, 42, 0.12),
                0 4px 12px rgba(15, 23, 42, 0.06);
        }

        .recovery-header {
            padding: 30px 32px 22px;

            border-bottom: 1px solid #e8edf4;

            text-align: center;
        }

        .recovery-icon {
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

            font-size: 28px;
            font-weight: 700;

            box-shadow:
                0 10px 24px rgba(37, 99, 235, 0.25);
        }

        .recovery-header h1 {
            margin: 0 0 8px;

            color: #172033;

            font-size: 27px;
            font-weight: 800;
        }

        .recovery-header p {
            margin: 0;

            color: #64748b;

            font-size: 14px;
            line-height: 1.6;
        }

        .recovery-body {
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
            width: 100%;
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

        /*
         * Não colocar display:block ou d-block nesta classe.
         * O ASP.NET precisa de conseguir esconder os validadores.
         */

        .validation-message {
            margin-top: 5px;

            color: #dc3545;

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

        .send-button {
            width: 100%;
            min-height: 48px;

            margin-top: 8px;

            border: 0;
            border-radius: 11px;

            background: #1d4ed8;

            font-weight: 700;

            transition:
                background-color 0.2s ease,
                transform 0.2s ease,
                box-shadow 0.2s ease;
        }

        .send-button:hover {
            background: #1e40af;

            transform: translateY(-1px);

            box-shadow:
                0 8px 18px rgba(37, 99, 235, 0.20);
        }

        .send-button:focus {
            box-shadow:
                0 0 0 3px rgba(37, 99, 235, 0.18);
        }

        .message-area {
            margin-top: 20px;
        }

        .message-area .alert {
            margin-bottom: 0;

            border-radius: 11px;

            font-size: 13px;
            line-height: 1.5;
        }

        .security-note {
            margin-top: 20px;
            padding: 12px 14px;

            border-radius: 10px;

            background: #f8fafc;
            color: #64748b;

            font-size: 12px;
            line-height: 1.5;
        }

        .back-link {
            display: block;

            margin-top: 22px;

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
            .recovery-header,
            .recovery-body {
                padding-right: 21px;
                padding-left: 21px;
            }

            .recovery-header h1 {
                font-size: 23px;
            }
        }
    </style>

</head>

<body>

    <form
        id="form1"
        runat="server">

        <div class="recovery-page">

            <main class="recovery-card">

                <!-- CABEÇALHO -->

                <header class="recovery-header">

                    <div class="recovery-icon">
                        ↻
                    </div>

                    <h1>
                        Recuperar palavra-passe
                    </h1>

                    <p>
                        Introduza o nome de utilizador e o email
                        associados à sua conta. Será enviado um
                        link válido durante 30 minutos.
                    </p>

                </header>


                <!-- CONTEÚDO DO FORMULÁRIO -->

                <div class="recovery-body">

                    <asp:ValidationSummary
                        ID="ValidationSummary1"
                        runat="server"
                        ValidationGroup="recuperar"
                        CssClass="alert alert-warning validation-summary"
                        HeaderText="Verifique os seguintes campos:"
                        DisplayMode="BulletList"
                        ShowSummary="true" />


                    <!-- NOME DE UTILIZADOR -->

                    <div class="mb-3">

                        <asp:Label
                            ID="LblUsername"
                            runat="server"
                            AssociatedControlID="textUsername"
                            CssClass="form-label"
                            Text="Nome de utilizador" />

                        <asp:TextBox
                            ID="textUsername"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="100"
                            placeholder="Introduza o seu nome de utilizador"
                            autocomplete="username" />

                        <asp:RequiredFieldValidator
                            ID="RfvUsername"
                            runat="server"
                            ControlToValidate="textUsername"
                            ValidationGroup="recuperar"
                            Display="Dynamic"
                            EnableClientScript="true"
                            SetFocusOnError="true"
                            CssClass="text-danger small validation-message"
                            ErrorMessage="O nome de utilizador é obrigatório." />

                    </div>


                    <!-- EMAIL -->

                    <div class="mb-3">

                        <asp:Label
                            ID="LblEmail"
                            runat="server"
                            AssociatedControlID="textEmail"
                            CssClass="form-label"
                            Text="Email" />

                        <asp:TextBox
                            ID="textEmail"
                            runat="server"
                            CssClass="form-control"
                            TextMode="Email"
                            MaxLength="150"
                            placeholder="nome@exemplo.pt"
                            autocomplete="email" />

                        <asp:RequiredFieldValidator
                            ID="RfvEmail"
                            runat="server"
                            ControlToValidate="textEmail"
                            ValidationGroup="recuperar"
                            Display="Dynamic"
                            EnableClientScript="true"
                            SetFocusOnError="true"
                            CssClass="text-danger small validation-message"
                            ErrorMessage="O email é obrigatório." />

                        <asp:RegularExpressionValidator
                            ID="RevEmail"
                            runat="server"
                            ControlToValidate="textEmail"
                            ValidationGroup="recuperar"
                            Display="Dynamic"
                            EnableClientScript="true"
                            SetFocusOnError="true"
                            CssClass="text-danger small validation-message"
                            ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                            ErrorMessage="Introduza um endereço de email válido." />

                    </div>


                    <!-- BOTÃO -->

                    <asp:Button
                        ID="btnSend"
                        runat="server"
                        Text="Enviar link de recuperação"
                        CssClass="btn btn-primary send-button"
                        ValidationGroup="recuperar"
                        CausesValidation="true"
                        OnClick="btnSend_Click" />


                    <!-- MENSAGEM DO SERVIDOR -->

                    <div class="message-area">

                        <asp:Label
                            ID="lblMessage"
                            runat="server"
                            Visible="false" />

                    </div>


                    <!-- NOTA DE SEGURANÇA -->

                    <div class="security-note">

                        Por segurança, a mensagem apresentada será
                        a mesma independentemente de o nome de utilizador
                        e o email estarem ou não registados.

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

</body>

</html>