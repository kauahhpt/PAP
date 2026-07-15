<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="criarconta.aspx.cs"
    Inherits="AlunoGest.criarconta" %>

<!DOCTYPE html>

<html lang="pt-PT"
xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <meta charset="utf-8" />

    <meta name="viewport"
        content="width=device-width, initial-scale=1" />

    <meta
        name="description"
        content="Inovar Inovado - Criar Agrupamento" />

    <meta
        name="theme-color"
        content="#123570" />

    <title>Inovar Inovado | Criar Agrupamento</title>

    <link
        href="../Content/bootstrap.min.css"
        rel="stylesheet" />


    <style>
        * {
            box-sizing: border-box;
        }


        html,
        body {
            margin: 0;
            width: 100%;
            min-height: 100%;
        }


        body {
            min-height: 100vh;
            background: #f3f6fb;
            color: #1f2937;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
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
            grid-template-columns: minmax(330px, 0.72fr) minmax(600px, 1.28fr);
        }


        /* =====================================================
           ÁREA ESQUERDA
        ===================================================== */

        .brand-side {
            position: relative;
            display: flex;
            flex-direction: column;
            justify-content: center;
            min-height: 100vh;
            padding: 55px;
            overflow: hidden;
            background: linear-gradient( 145deg, #0f2c61 0%, #123570 45%, #1d4ed8 100% );
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
                background: rgba(255, 255, 255, 0.07);
            }


            .brand-side::after {
                content: "";
                position: absolute;
                width: 280px;
                height: 280px;
                bottom: -130px;
                left: -120px;
                border-radius: 50%;
                background: rgba(255, 255, 255, 0.06);
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
            border: 1px solid rgba(255, 255, 255, 0.22);
            border-radius: 16px;
            background: rgba(255, 255, 255, 0.14);
            color: #ffffff;
            font-size: 23px;
            font-weight: 800;
        }


        .brand-title {
            margin: 0 0 14px 0;
            font-size: 42px;
            font-weight: 800;
            line-height: 1.1;
            letter-spacing: -0.03em;
        }


        .brand-description {
            max-width: 450px;
            margin: 0 0 35px 0;
            color: #dbeafe;
            font-size: 16px;
            line-height: 1.7;
        }


        .brand-features {
            display: flex;
            flex-direction: column;
            gap: 14px;
        }


        .brand-feature {
            display: flex;
            align-items: center;
            gap: 12px;
            color: #eff6ff;
            font-size: 14px;
        }


        .feature-check {
            width: 27px;
            height: 27px;
            flex: 0 0 27px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 50%;
            background: rgba(255, 255, 255, 0.14);
            font-size: 13px;
            font-weight: 700;
        }


        /* =====================================================
           ÁREA DO FORMULÁRIO
        ===================================================== */

        .form-side {
            display: flex;
            align-items: flex-start;
            justify-content: center;
            min-height: 100vh;
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
                margin: 0 0 7px 0;
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
            background: #ffffff;
            border: 1px solid rgba(15, 23, 42, 0.08);
            border-radius: 18px;
            box-shadow: 0 15px 40px rgba(15, 23, 42, 0.08);
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
            border-bottom: 1px solid #edf0f5;
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
            margin: 2px 0 0 0;
            color: #94a3b8;
            font-size: 12px;
        }


        /* =====================================================
           GRID DOS CAMPOS
        ===================================================== */

        .form-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 18px;
        }


            .form-grid.three-columns {
                grid-template-columns: repeat(3, minmax(0, 1fr));
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


        /* =====================================================
           INPUTS
        ===================================================== */

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
            transition: border-color 0.2s, box-shadow 0.2s, background 0.2s;
        }


            .input-custom:focus {
                background: #ffffff;
                border-color: #2563eb;
                box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.12);
            }


            .input-custom::placeholder {
                color: #94a3b8;
            }


        textarea.input-custom {
            min-height: 85px;
            resize: vertical;
        }


        /* =====================================================
           CREDENCIAIS
        ===================================================== */

        .credentials-box {
            padding: 20px;
            border: 1px solid #dbeafe;
            border-radius: 12px;
            background: linear-gradient( 145deg, #f8fbff, #eff6ff );
        }


            .credentials-box .section-header {
                border-bottom-color: #dbeafe;
            }


        /* =====================================================
           VALIDADORES
        ===================================================== */

        .validator-error {
            display: block;
            margin-top: 5px;
            color: #dc2626;
            font-size: 12px;
        }


        /* =====================================================
           BOTÕES
        ===================================================== */

        .button-area {
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 15px;
            margin-top: 28px;
            padding-top: 22px;
            border-top: 1px solid #edf0f5;
        }


        .button-group {
            display: flex;
            align-items: center;
            gap: 10px;
            flex-wrap: wrap;
        }


        .btn-custom {
            min-height: 44px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            padding: 10px 18px;
            border-radius: 9px;
            font-size: 14px;
            font-weight: 700;
            text-decoration: none;
            cursor: pointer;
            transition: transform 0.15s, box-shadow 0.2s, opacity 0.2s, background 0.2s;
        }


        .btn-create {
            min-width: 150px;
            border: 0;
            background: linear-gradient( 135deg, #123570, #2563eb );
            color: #ffffff;
        }


            .btn-create:hover {
                color: #ffffff;
                box-shadow: 0 8px 20px rgba(37, 99, 235, 0.24);
                opacity: 0.96;
            }


        .btn-secondary-custom {
            border: 1px solid #cbd5e1;
            background: #ffffff;
            color: #475569;
        }


            .btn-secondary-custom:hover {
                background: #f8fafc;
                color: #1f2937;
            }


        .btn-custom:active {
            transform: translateY(1px);
        }


        /* =====================================================
           LOGIN
        ===================================================== */

        .login-area {
            color: #64748b;
            font-size: 13px;
        }


            .login-area a {
                color: #2563eb;
                font-weight: 700;
                text-decoration: none;
            }


                .login-area a:hover {
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
                grid-template-columns: minmax(300px, 0.65fr) minmax(550px, 1.35fr);
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
                flex-direction: column;
                align-items: stretch;
            }


            .button-group {
                width: 100%;
            }


            .btn-custom {
                width: 100%;
            }


            .login-area {
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

        .message-success {
            display: block;
            margin-bottom: 20px;
            padding: 12px 15px;
            border: 1px solid #bbf7d0;
            border-radius: 10px;
            background: #f0fdf4;
            color: #166534;
            font-size: 14px;
        }

        .message-error {
            display: block;
            margin-bottom: 20px;
            padding: 12px 15px;
            border: 1px solid #fecaca;
            border-radius: 10px;
            background: #fef2f2;
            color: #b91c1c;
            font-size: 14px;
        }
    </style>

</head>


<body>

    <form
        id="form1"
        runat="server">

        <div class="register-page">


            <!-- ==================================================
                 ÁREA INSTITUCIONAL
            =================================================== -->

            <aside class="brand-side">

                <div class="brand-content">

                    <div class="brand-logo">
                        II
                    </div>


                    <h1 class="brand-title">Inovar Inovado

                    </h1>


                    <p class="brand-description">
                        Área reservada à administração da plataforma para
                        criação e gestão dos agrupamentos escolares.
                    </p>


                    <div class="brand-features">


                        <div class="brand-feature">

                            <span class="feature-check">✓
                            </span>

                            Gestão centralizada das instituições.

                        </div>


                        <div class="brand-feature">

                            <span class="feature-check">✓
                            </span>

                            Comunicação entre escolas, professores e alunos.

                        </div>


                        <div class="brand-feature">

                            <span class="feature-check">✓
                            </span>

                            Acesso simples, organizado e seguro.

                        </div>


                    </div>

                </div>

            </aside>


            <!-- ==================================================
                 ÁREA DO FORMULÁRIO
            =================================================== -->

            <main class="form-side">

                <div class="form-wrapper">


                    <div class="mobile-brand">
                        Inovar Inovado

                    </div>


                    <div class="form-header">

                        <h1>Criar agrupamento</h1>

                        <p>
                            Registe um novo agrupamento e defina as respetivas credenciais de acesso.
                        </p>
                    </div>


                    <div class="register-card">
                        <asp:Label
                            ID="lblMensagem"
                            runat="server"
                            Visible="false" />

                        <asp:Panel runat="server">


                            <!-- ==================================================
                                 1. INFORMAÇÕES PESSOAIS
                            =================================================== -->

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
                                            Dados gerais de contacto da instituição.
                                        </p>

                                    </div>

                                </div>


                                <div class="form-grid three-columns">


                                    <!-- NOME -->

                                    <div class="field-group">

                                        <label
                                            class="form-label-custom"
                                            for="txtNome">
                                            Nome do agrupamento
                                            <span class="required">*</span>

                                        </label>


                                        <asp:TextBox
                                            ID="txtNome"
                                            runat="server"
                                            CssClass="input-custom"
                                            placeholder="Nome completo" />


                                        <asp:RequiredFieldValidator
                                            runat="server"
                                            ControlToValidate="txtNome"
                                            ErrorMessage="O nome é obrigatório."
                                            CssClass="validator-error"
                                            Display="Dynamic" />

                                    </div>


                                    <!-- EMAIL -->

                                    <div class="field-group">

                                        <label
                                            class="form-label-custom"
                                            for="txtEmail">
                                            Email
                                            <span class="required">*</span>

                                        </label>


                                        <asp:TextBox
                                            ID="txtEmail"
                                            runat="server"
                                            CssClass="input-custom"
                                            TextMode="Email"
                                            placeholder="email@exemplo.com" />


                                        <asp:RequiredFieldValidator
                                            runat="server"
                                            ControlToValidate="txtEmail"
                                            ErrorMessage="O email é obrigatório."
                                            CssClass="validator-error"
                                            Display="Dynamic" />


                                        <asp:RegularExpressionValidator
                                            runat="server"
                                            ControlToValidate="txtEmail"
                                            ErrorMessage="Introduza um email válido."
                                            CssClass="validator-error"
                                            ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                                            Display="Dynamic" />

                                    </div>


                                    <!-- TELEFONE -->

                                    <div class="field-group">

                                        <label
                                            class="form-label-custom"
                                            for="txtTelefone">
                                            Telefone
                                            <span class="required">*</span>

                                        </label>


                                        <asp:TextBox
                                            ID="txtTelefone"
                                            runat="server"
                                            CssClass="input-custom"
                                            placeholder="+351 9XXXXXXXX" />


                                        <asp:RequiredFieldValidator
                                            runat="server"
                                            ControlToValidate="txtTelefone"
                                            ErrorMessage="O telefone é obrigatório."
                                            CssClass="validator-error"
                                            Display="Dynamic" />

                                    </div>


                                </div>

                            </section>


                            <!-- ==================================================
                                 2. MORADA
                            =================================================== -->

                            <section class="form-section">

                                <div class="section-header">

                                    <div class="section-number">
                                        2
                                    </div>

                                    <div>

                                        <h2 class="section-title">Morada
                                        </h2>

                                        <p class="section-description">
                                            Localização e dados de endereço.
                                        </p>

                                    </div>

                                </div>


                                <div class="form-grid">


                                    <!-- MORADA -->

                                    <div class="field-group field-full">

                                        <label
                                            class="form-label-custom"
                                            for="txtMorada">
                                            Morada
                                            <span class="required">*</span>

                                        </label>


                                        <asp:TextBox
                                            ID="txtMorada"
                                            runat="server"
                                            CssClass="input-custom"
                                            TextMode="MultiLine"
                                            Rows="3"
                                            placeholder="Rua, número e informação adicional" />


                                        <asp:RequiredFieldValidator
                                            runat="server"
                                            ControlToValidate="txtMorada"
                                            ErrorMessage="A morada é obrigatória."
                                            CssClass="validator-error"
                                            Display="Dynamic" />

                                    </div>


                                    <!-- CÓDIGO POSTAL -->

                                    <div class="field-group">

                                        <label
                                            class="form-label-custom"
                                            for="txtCodigoPostal">
                                            Código postal
                                            <span class="required">*</span>

                                        </label>


                                        <asp:TextBox
                                            ID="txtCodigoPostal"
                                            runat="server"
                                            CssClass="input-custom"
                                            placeholder="1234-567" />


                                        <asp:RequiredFieldValidator
                                            runat="server"
                                            ControlToValidate="txtCodigoPostal"
                                            ErrorMessage="O código postal é obrigatório."
                                            CssClass="validator-error"
                                            Display="Dynamic" />

                                    </div>


                                    <!-- LOCALIDADE -->

                                    <div class="field-group">

                                        <label
                                            class="form-label-custom"
                                            for="txtLocalidade">
                                            Localidade
                                            <span class="required">*</span>

                                        </label>


                                        <asp:TextBox
                                            ID="txtLocalidade"
                                            runat="server"
                                            CssClass="input-custom"
                                            placeholder="Localidade" />


                                        <asp:RequiredFieldValidator
                                            runat="server"
                                            ControlToValidate="txtLocalidade"
                                            ErrorMessage="A localidade é obrigatória."
                                            CssClass="validator-error"
                                            Display="Dynamic" />

                                    </div>


                                </div>

                            </section>


                            <!-- ==================================================
                                 3. INFORMAÇÕES DA INSTITUIÇÃO
                            =================================================== -->

                            <section class="form-section">

                                <div class="section-header">

                                    <div class="section-number">
                                        3
                                    </div>

                                    <div>

                                        <h2 class="section-title">Informação institucional
                                        </h2>

                                        <p class="section-description">
                                            Identificação adicional da instituição.
                                        </p>

                                    </div>

                                </div>


                                <div class="form-grid">

                                    <div class="field-group">

                                        <label
                                            class="form-label-custom"
                                            for="txtCodigoMEC">
                                            Código MEC

                                        </label>


                                        <asp:TextBox
                                            ID="txtCodigoMEC"
                                            runat="server"
                                            CssClass="input-custom"
                                            placeholder="Opcional" />

                                    </div>

                                </div>

                            </section>


                            <!-- ==================================================
                                 4. CREDENCIAIS
                            =================================================== -->

                            <section class="form-section credentials-box">

                                <div class="section-header">

                                    <div class="section-number">
                                        4
                                    </div>

                                    <div>

                                        <h2 class="section-title">Credenciais de acesso
                                        </h2>

                                        <p class="section-description">
                                            Dados utilizados para iniciar sessão.
                                        </p>

                                    </div>

                                </div>


                                <div class="form-grid">


                                    <!-- USERNAME -->

                                    <div class="field-group">

                                        <label
                                            class="form-label-custom"
                                            for="txtUsername">
                                            Nome de utilizador
                                            <span class="required">*</span>

                                        </label>


                                        <asp:TextBox
                                            ID="txtUsername"
                                            runat="server"
                                            CssClass="input-custom"
                                            placeholder="Nome de utilizador" />


                                        <asp:RequiredFieldValidator
                                            runat="server"
                                            ControlToValidate="txtUsername"
                                            ErrorMessage="O nome de utilizador é obrigatório."
                                            CssClass="validator-error"
                                            Display="Dynamic" />

                                    </div>


                                    <!-- PASSWORD -->

                                    <div class="field-group">

                                        <label
                                            class="form-label-custom"
                                            for="txtPassword">
                                            Palavra-passe
                                            <span class="required">*</span>

                                        </label>


                                        <asp:TextBox
                                            ID="txtPassword"
                                            runat="server"
                                            CssClass="input-custom"
                                            TextMode="Password"
                                            placeholder="Introduza a palavra-passe" />


                                        <asp:RequiredFieldValidator
                                            runat="server"
                                            ControlToValidate="txtPassword"
                                            ErrorMessage="A palavra-passe é obrigatória."
                                            CssClass="validator-error"
                                            Display="Dynamic" />

                                    </div>


                                </div>

                            </section>


                            <!-- ==================================================
                                 AÇÕES
                            =================================================== -->

                            <div class="button-area">


                                <div class="button-group">


                                    <asp:Button
                                        ID="btnCriarConta"
                                        runat="server"
                                        Text="Criar agrupamento"
                                        CssClass="btn-custom btn-create"
                                        OnClick="btnCriarConta_Click" />





                                </div>
                            </div>


                            <!-- Mantido para compatibilidade com a página atual -->

                            <asp:Button
                                ID="btnCancelar"
                                runat="server"
                                Text="Cancelar"
                                CausesValidation="false"
                                Style="display: none;" />


                        </asp:Panel>

                    </div>


                    <div class="form-footer">
                        © 2026 Inovar Inovado — Plataforma de Gestão Escolar

                    </div>

                </div>

            </main>

        </div>

    </form>

</body>

</html>
