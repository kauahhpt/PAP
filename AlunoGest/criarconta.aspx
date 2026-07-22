<%@ Page
    Language="C#"
    AutoEventWireup="true"
    CodeBehind="criarconta.aspx.cs"
    Inherits="AlunoGest.criarconta" %>

<!DOCTYPE html>

<html lang="pt-PT"
    xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <meta charset="utf-8" />

    <meta
        name="viewport"
        content="width=device-width, initial-scale=1" />

    <meta
        name="description"
        content="Pedido de criação de agrupamento escolar" />

    <meta
        name="theme-color"
        content="#123570" />

    <title>
        Inovar Inovado | Pedido de agrupamento
    </title>

    <link
        href="Content/bootstrap.min.css"
        rel="stylesheet" />

    <style>

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
            width: 100%;
            min-height: 100vh;
        }


        /* =====================================================
           LAYOUT
        ===================================================== */

        .register-page {
            width: 100%;
            min-height: 100vh;

            display: grid;

            grid-template-columns:
                minmax(330px, 0.72fr)
                minmax(600px, 1.28fr);
        }


        /* =====================================================
           ÁREA ESQUERDA
        ===================================================== */

        .brand-side {
            position: relative;

            min-height: 100vh;

            display: flex;
            align-items: center;

            padding: 55px;

            overflow: hidden;

            background:
                linear-gradient(
                    145deg,
                    #0f2c61 0%,
                    #123570 45%,
                    #1d4ed8 100%
                );

            color: #ffffff;
        }

        .brand-side::before {
            content: "";

            position: absolute;

            width: 400px;
            height: 400px;

            top: -170px;
            right: -170px;

            border-radius: 50%;

            background:
                rgba(255, 255, 255, 0.07);
        }

        .brand-side::after {
            content: "";

            position: absolute;

            width: 280px;
            height: 280px;

            bottom: -130px;
            left: -120px;

            border-radius: 50%;

            background:
                rgba(255, 255, 255, 0.06);
        }

        .brand-content {
            position: relative;
            z-index: 2;

            width: 100%;
            max-width: 480px;

            margin: 0 auto;
        }

        .brand-logo {
            width: 58px;
            height: 58px;

            display: flex;
            align-items: center;
            justify-content: center;

            margin-bottom: 26px;

            border:
                1px solid rgba(255, 255, 255, 0.22);

            border-radius: 16px;

            background:
                rgba(255, 255, 255, 0.14);

            color: #ffffff;

            font-size: 23px;
            font-weight: 800;
        }

        .brand-title {
            margin: 0 0 14px;

            font-size: 42px;
            font-weight: 800;
            line-height: 1.1;

            letter-spacing: -0.03em;
        }

        .brand-description {
            max-width: 450px;

            margin: 0 0 35px;

            color: #dbeafe;

            font-size: 16px;
            line-height: 1.7;
        }

        .brand-steps {
            display: flex;
            flex-direction: column;

            gap: 15px;
        }

        .brand-step {
            display: flex;
            align-items: center;

            gap: 12px;

            color: #eff6ff;

            font-size: 14px;
        }

        .step-number {
            width: 28px;
            height: 28px;

            flex: 0 0 28px;

            display: flex;
            align-items: center;
            justify-content: center;

            border-radius: 50%;

            background:
                rgba(255, 255, 255, 0.14);

            font-size: 12px;
            font-weight: 800;
        }


        /* =====================================================
           ÁREA DO FORMULÁRIO
        ===================================================== */

        .form-side {
            min-height: 100vh;

            display: flex;
            align-items: flex-start;
            justify-content: center;

            padding: 45px 35px;

            background: #f3f6fb;
        }

        .form-wrapper {
            width: 100%;
            max-width: 900px;

            margin: auto 0;
        }

        .mobile-brand {
            display: none;

            margin-bottom: 25px;

            color: #123570;

            font-size: 24px;
            font-weight: 800;
        }


        /* =====================================================
           CABEÇALHO
        ===================================================== */

        .form-header {
            margin-bottom: 25px;
        }

        .form-header h1 {
            margin: 0 0 7px;

            color: #111827;

            font-size: 31px;
            font-weight: 800;

            letter-spacing: -0.02em;
        }

        .form-header p {
            margin: 0;

            color: #64748b;

            font-size: 15px;
            line-height: 1.6;
        }


        /* =====================================================
           CARD
        ===================================================== */

        .register-card {
            padding: 30px;

            border:
                1px solid rgba(15, 23, 42, 0.08);

            border-radius: 18px;

            background: #ffffff;

            box-shadow:
                0 15px 40px rgba(15, 23, 42, 0.08);
        }


        /* =====================================================
           SECÇÕES
        ===================================================== */

        .form-section {
            margin-bottom: 28px;
        }

        .form-section:last-of-type {
            margin-bottom: 0;
        }

        .section-header {
            display: flex;
            align-items: center;

            gap: 12px;

            margin-bottom: 18px;
            padding-bottom: 12px;

            border-bottom:
                1px solid #edf0f5;
        }

        .section-number {
            width: 31px;
            height: 31px;

            flex: 0 0 31px;

            display: flex;
            align-items: center;
            justify-content: center;

            border-radius: 9px;

            background: #e8f0ff;
            color: #1d4ed8;

            font-size: 13px;
            font-weight: 800;
        }

        .section-title {
            margin: 0;

            color: #1f2937;

            font-size: 16px;
            font-weight: 750;
        }

        .section-description {
            margin: 2px 0 0;

            color: #94a3b8;

            font-size: 12px;
        }


        /* =====================================================
           CAMPOS
        ===================================================== */

        .form-grid {
            display: grid;

            grid-template-columns:
                repeat(2, minmax(0, 1fr));

            gap: 18px;
        }

        .form-grid.three-columns {
            grid-template-columns:
                repeat(3, minmax(0, 1fr));
        }

        .field-full {
            grid-column: 1 / -1;
        }

        .field-group {
            min-width: 0;
        }

        .form-label-custom {
            display: block;

            margin-bottom: 7px;

            color: #334155;

            font-size: 13px;
            font-weight: 650;
        }

        .required {
            color: #dc2626;
        }

        .input-custom {
            width: 100%;
            min-height: 46px;

            padding: 10px 13px;

            border: 1px solid #d8dee9;
            border-radius: 10px;

            background: #f8fafc;
            color: #1f2937;

            font-size: 14px;

            outline: none;

            transition:
                border-color 0.2s,
                box-shadow 0.2s,
                background 0.2s;
        }

        .input-custom:focus {
            border-color: #2563eb;

            background: #ffffff;

            box-shadow:
                0 0 0 3px rgba(37, 99, 235, 0.12);
        }

        .input-custom::placeholder {
            color: #94a3b8;
        }

        textarea.input-custom {
            min-height: 85px;

            resize: vertical;
        }


        /* =====================================================
           VALIDADORES
        ===================================================== */

        .validator-error {
            margin-top: 5px;

            color: #dc2626;

            font-size: 12px;
            font-weight: 600;
        }

        .validation-summary {
            margin-bottom: 20px;

            border-radius: 10px;

            font-size: 13px;
        }


        /* =====================================================
           INFORMAÇÃO
        ===================================================== */

        .information-box {
            margin-top: 24px;
            padding: 14px 16px;

            border: 1px solid #dbeafe;
            border-radius: 11px;

            background: #eff6ff;
            color: #475569;

            font-size: 13px;
            line-height: 1.6;
        }


        /* =====================================================
           MENSAGENS
        ===================================================== */

        .message-success,
        .message-warning,
        .message-error {
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

        .message-warning {
            border: 1px solid #fde68a;

            background: #fffbeb;
            color: #92400e;
        }

        .message-error {
            border: 1px solid #fecaca;

            background: #fef2f2;
            color: #b91c1c;
        }


        /* =====================================================
           BOTÕES
        ===================================================== */

        .button-area {
            display: flex;
            align-items: center;
            justify-content: space-between;

            flex-wrap: wrap;

            gap: 15px;

            margin-top: 28px;
            padding-top: 22px;

            border-top:
                1px solid #edf0f5;
        }

        .btn-send {
            min-height: 46px;

            padding: 10px 21px;

            border: 0;
            border-radius: 10px;

            background:
                linear-gradient(
                    135deg,
                    #123570,
                    #2563eb
                );

            color: #ffffff;

            font-size: 14px;
            font-weight: 700;

            cursor: pointer;

            transition:
                box-shadow 0.2s,
                transform 0.15s;
        }

        .btn-send:hover {
            box-shadow:
                0 8px 20px rgba(37, 99, 235, 0.24);
        }

        .btn-send:active {
            transform: translateY(1px);
        }

        .back-link {
            color: #64748b;

            font-size: 13px;
            font-weight: 700;
            text-decoration: none;
        }

        .back-link:hover {
            color: #2563eb;

            text-decoration: underline;
        }


        /* =====================================================
           FOOTER
        ===================================================== */

        .form-footer {
            margin-top: 20px;

            color: #94a3b8;

            text-align: center;

            font-size: 12px;
        }


        /* =====================================================
           RESPONSIVO
        ===================================================== */

        @media (max-width: 1050px) {

            .register-page {
                grid-template-columns:
                    minmax(300px, 0.65fr)
                    minmax(550px, 1.35fr);
            }

            .brand-side {
                padding: 40px;
            }

            .brand-title {
                font-size: 35px;
            }

            .form-side {
                padding: 30px 25px;
            }
        }

        @media (max-width: 850px) {

            .register-page {
                display: block;
            }

            .brand-side {
                display: none;
            }

            .form-side {
                min-height: 100vh;

                padding: 30px 18px;
            }

            .mobile-brand {
                display: block;
            }
        }

        @media (max-width: 650px) {

            .register-card {
                padding: 22px 18px;
            }

            .form-grid,
            .form-grid.three-columns {
                grid-template-columns: 1fr;
            }

            .field-full {
                grid-column: auto;
            }

            .button-area {
                flex-direction: column-reverse;
                align-items: stretch;
            }

            .btn-send {
                width: 100%;
            }

            .back-link {
                text-align: center;
            }

            .form-header h1 {
                font-size: 27px;
            }
        }

        @media (max-width: 400px) {

            .form-side {
                padding: 20px 10px;
            }

            .register-card {
                padding: 20px 15px;
            }
        }

    </style>

</head>

<body>

    <form
        id="form1"
        runat="server">

        <div class="register-page">


            <!-- ÁREA INSTITUCIONAL -->

            <aside class="brand-side">

                <div class="brand-content">

                    <div class="brand-logo">
                        II
                    </div>

                    <h1 class="brand-title">
                        Registe o seu agrupamento
                    </h1>

                    <p class="brand-description">

                        Envie os dados da instituição para análise
                        pela administração da plataforma.

                    </p>

                    <div class="brand-steps">

                        <div class="brand-step">

                            <span class="step-number">
                                1
                            </span>

                            Preencha e envie o pedido.

                        </div>

                        <div class="brand-step">

                            <span class="step-number">
                                2
                            </span>

                            A administração valida as informações.

                        </div>

                        <div class="brand-step">

                            <span class="step-number">
                                3
                            </span>

                            As credenciais são enviadas após aprovação.

                        </div>

                    </div>

                </div>

            </aside>


            <!-- FORMULÁRIO -->

            <main class="form-side">

                <div class="form-wrapper">

                    <div class="mobile-brand">
                        Inovar Inovado
                    </div>

                    <header class="form-header">

                        <h1>
                            Criar conta de agrupamento
                        </h1>

                        <p>

                            Preencha os dados abaixo para enviar um pedido
                            de criação de conta. A conta só será criada
                            depois da aprovação de um administrador.

                        </p>

                    </header>


                    <div class="register-card">

                        <asp:Label
                            ID="lblMensagem"
                            runat="server"
                            Visible="false" />


                        <asp:ValidationSummary
                            ID="ValidationSummary1"
                            runat="server"
                            ValidationGroup="pedido"
                            CssClass="alert alert-warning validation-summary"
                            HeaderText="Verifique os seguintes campos:"
                            DisplayMode="BulletList" />


                        <!-- 1. AGRUPAMENTO -->

                        <section class="form-section">

                            <div class="section-header">

                                <div class="section-number">
                                    1
                                </div>

                                <div>

                                    <h2 class="section-title">
                                        Informações do agrupamento
                                    </h2>

                                    <p class="section-description">
                                        Dados de identificação da instituição.
                                    </p>

                                </div>

                            </div>


                            <div class="form-grid">

                                <div class="field-group field-full">

                                    <asp:Label
                                        ID="LblNome"
                                        runat="server"
                                        AssociatedControlID="txtNome"
                                        CssClass="form-label-custom">

                                        Nome do agrupamento

                                        <span class="required">
                                            *
                                        </span>

                                    </asp:Label>


                                    <asp:TextBox
                                        ID="txtNome"
                                        runat="server"
                                        CssClass="input-custom"
                                        MaxLength="200"
                                        placeholder="Nome completo do agrupamento" />


                                    <asp:RequiredFieldValidator
                                        ID="RfvNome"
                                        runat="server"
                                        ControlToValidate="txtNome"
                                        ValidationGroup="pedido"
                                        Display="Dynamic"
                                        CssClass="validator-error"
                                        ErrorMessage="O nome do agrupamento é obrigatório." />

                                </div>


                                <div class="field-group">

                                    <asp:Label
                                        ID="LblNIF"
                                        runat="server"
                                        AssociatedControlID="txtNIF"
                                        CssClass="form-label-custom">

                                        NIF

                                        <span class="required">
                                            *
                                        </span>

                                    </asp:Label>


                                    <asp:TextBox
                                        ID="txtNIF"
                                        runat="server"
                                        CssClass="input-custom"
                                        MaxLength="9"
                                        placeholder="123456789" />


                                    <asp:RequiredFieldValidator
                                        ID="RfvNIF"
                                        runat="server"
                                        ControlToValidate="txtNIF"
                                        ValidationGroup="pedido"
                                        Display="Dynamic"
                                        CssClass="validator-error"
                                        ErrorMessage="O NIF é obrigatório." />


                                    <asp:RegularExpressionValidator
                                        ID="RevNIF"
                                        runat="server"
                                        ControlToValidate="txtNIF"
                                        ValidationGroup="pedido"
                                        Display="Dynamic"
                                        CssClass="validator-error"
                                        ValidationExpression="^\d{9}$"
                                        ErrorMessage="O NIF deve conter exatamente 9 algarismos." />

                                </div>


                                <div class="field-group">

                                    <asp:Label
                                        ID="LblCodigoMEC"
                                        runat="server"
                                        AssociatedControlID="txtCodigoMEC"
                                        CssClass="form-label-custom"
                                        Text="Código MEC" />


                                    <asp:TextBox
                                        ID="txtCodigoMEC"
                                        runat="server"
                                        CssClass="input-custom"
                                        MaxLength="20"
                                        placeholder="Opcional" />

                                </div>

                            </div>

                        </section>


                        <!-- 2. RESPONSÁVEL -->

                        <section class="form-section">

                            <div class="section-header">

                                <div class="section-number">
                                    2
                                </div>

                                <div>

                                    <h2 class="section-title">
                                        Responsável pelo pedido
                                    </h2>

                                    <p class="section-description">
                                        Dados da pessoa que acompanhará o processo.
                                    </p>

                                </div>

                            </div>


                            <div class="form-grid three-columns">

                                <div class="field-group">

                                    <asp:Label
                                        ID="LblNomeResponsavel"
                                        runat="server"
                                        AssociatedControlID="txtNomeResponsavel"
                                        CssClass="form-label-custom">

                                        Nome do responsável

                                        <span class="required">
                                            *
                                        </span>

                                    </asp:Label>


                                    <asp:TextBox
                                        ID="txtNomeResponsavel"
                                        runat="server"
                                        CssClass="input-custom"
                                        MaxLength="200"
                                        placeholder="Nome completo" />


                                    <asp:RequiredFieldValidator
                                        ID="RfvNomeResponsavel"
                                        runat="server"
                                        ControlToValidate="txtNomeResponsavel"
                                        ValidationGroup="pedido"
                                        Display="Dynamic"
                                        CssClass="validator-error"
                                        ErrorMessage="O nome do responsável é obrigatório." />

                                </div>


                                <div class="field-group">

                                    <asp:Label
                                        ID="LblEmail"
                                        runat="server"
                                        AssociatedControlID="txtEmail"
                                        CssClass="form-label-custom">

                                        Email

                                        <span class="required">
                                            *
                                        </span>

                                    </asp:Label>


                                    <asp:TextBox
                                        ID="txtEmail"
                                        runat="server"
                                        CssClass="input-custom"
                                        TextMode="Email"
                                        MaxLength="150"
                                        autocomplete="email"
                                        placeholder="email@exemplo.pt" />


                                    <asp:RequiredFieldValidator
                                        ID="RfvEmail"
                                        runat="server"
                                        ControlToValidate="txtEmail"
                                        ValidationGroup="pedido"
                                        Display="Dynamic"
                                        CssClass="validator-error"
                                        ErrorMessage="O email é obrigatório." />


                                    <asp:RegularExpressionValidator
                                        ID="RevEmail"
                                        runat="server"
                                        ControlToValidate="txtEmail"
                                        ValidationGroup="pedido"
                                        Display="Dynamic"
                                        CssClass="validator-error"
                                        ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                                        ErrorMessage="Introduza um email válido." />

                                </div>


                                <div class="field-group">

                                    <asp:Label
                                        ID="LblTelefone"
                                        runat="server"
                                        AssociatedControlID="txtTelefone"
                                        CssClass="form-label-custom">

                                        Telefone

                                        <span class="required">
                                            *
                                        </span>

                                    </asp:Label>


                                    <asp:TextBox
                                        ID="txtTelefone"
                                        runat="server"
                                        CssClass="input-custom"
                                        MaxLength="20"
                                        autocomplete="tel"
                                        placeholder="+351 900 000 000" />


                                    <asp:RequiredFieldValidator
                                        ID="RfvTelefone"
                                        runat="server"
                                        ControlToValidate="txtTelefone"
                                        ValidationGroup="pedido"
                                        Display="Dynamic"
                                        CssClass="validator-error"
                                        ErrorMessage="O telefone é obrigatório." />

                                </div>

                            </div>

                        </section>


                        <!-- 3. MORADA -->

                        <section class="form-section">

                            <div class="section-header">

                                <div class="section-number">
                                    3
                                </div>

                                <div>

                                    <h2 class="section-title">
                                        Localização
                                    </h2>

                                    <p class="section-description">
                                        Morada principal do agrupamento.
                                    </p>

                                </div>

                            </div>


                            <div class="form-grid">

                                <div class="field-group field-full">

                                    <asp:Label
                                        ID="LblMorada"
                                        runat="server"
                                        AssociatedControlID="txtMorada"
                                        CssClass="form-label-custom">

                                        Morada

                                        <span class="required">
                                            *
                                        </span>

                                    </asp:Label>


                                    <asp:TextBox
                                        ID="txtMorada"
                                        runat="server"
                                        CssClass="input-custom"
                                        TextMode="MultiLine"
                                        Rows="3"
                                        MaxLength="300"
                                        placeholder="Rua, número e informações adicionais" />


                                    <asp:RequiredFieldValidator
                                        ID="RfvMorada"
                                        runat="server"
                                        ControlToValidate="txtMorada"
                                        ValidationGroup="pedido"
                                        Display="Dynamic"
                                        CssClass="validator-error"
                                        ErrorMessage="A morada é obrigatória." />

                                </div>


                                <div class="field-group">

                                    <asp:Label
                                        ID="LblCodigoPostal"
                                        runat="server"
                                        AssociatedControlID="txtCodigoPostal"
                                        CssClass="form-label-custom">

                                        Código postal

                                        <span class="required">
                                            *
                                        </span>

                                    </asp:Label>


                                    <asp:TextBox
                                        ID="txtCodigoPostal"
                                        runat="server"
                                        CssClass="input-custom"
                                        MaxLength="8"
                                        placeholder="1234-567" />


                                    <asp:RequiredFieldValidator
                                        ID="RfvCodigoPostal"
                                        runat="server"
                                        ControlToValidate="txtCodigoPostal"
                                        ValidationGroup="pedido"
                                        Display="Dynamic"
                                        CssClass="validator-error"
                                        ErrorMessage="O código postal é obrigatório." />


                                    <asp:RegularExpressionValidator
                                        ID="RevCodigoPostal"
                                        runat="server"
                                        ControlToValidate="txtCodigoPostal"
                                        ValidationGroup="pedido"
                                        Display="Dynamic"
                                        CssClass="validator-error"
                                        ValidationExpression="^\d{4}-\d{3}$"
                                        ErrorMessage="Utilize o formato 1234-567." />

                                </div>


                                <div class="field-group">

                                    <asp:Label
                                        ID="LblLocalidade"
                                        runat="server"
                                        AssociatedControlID="txtLocalidade"
                                        CssClass="form-label-custom">

                                        Localidade

                                        <span class="required">
                                            *
                                        </span>

                                    </asp:Label>


                                    <asp:TextBox
                                        ID="txtLocalidade"
                                        runat="server"
                                        CssClass="input-custom"
                                        MaxLength="150"
                                        placeholder="Localidade" />


                                    <asp:RequiredFieldValidator
                                        ID="RfvLocalidade"
                                        runat="server"
                                        ControlToValidate="txtLocalidade"
                                        ValidationGroup="pedido"
                                        Display="Dynamic"
                                        CssClass="validator-error"
                                        ErrorMessage="A localidade é obrigatória." />

                                </div>

                            </div>

                        </section>


                        <div class="information-box">

                            O envio deste formulário não cria imediatamente
                            uma conta. O pedido será analisado por um
                            administrador e receberá uma confirmação por email.

                            Caso o pedido seja aprovado, as credenciais de
                            acesso serão enviadas posteriormente.

                        </div>


                        <!-- AÇÕES -->

                        <div class="button-area">

                            <a
                                href="login.aspx"
                                class="back-link">

                                Voltar ao início de sessão

                            </a>


                            <asp:Button
                                ID="btnCriarConta"
                                runat="server"
                                Text="Enviar pedido"
                                CssClass="btn-send"
                                ValidationGroup="pedido"
                                CausesValidation="true"
                                OnClick="btnCriarConta_Click" />

                        </div>


                        <asp:Button
                            ID="btnCancelar"
                            runat="server"
                            Text="Cancelar"
                            CausesValidation="false"
                            Style="display: none;" />

                    </div>


                    <div class="form-footer">

                        © 2026 Inovar Inovado —
                        Plataforma de Gestão Escolar

                    </div>

                </div>

            </main>

        </div>

    </form>

</body>

</html>