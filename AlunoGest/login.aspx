<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="login.aspx.cs"
    Inherits="AlunoGest.login" %>

<!DOCTYPE html>

<html lang="pt-PT">

<head runat="server">

    <meta charset="utf-8" />

    <meta name="viewport"
        content="width=device-width, initial-scale=1" />

    <title>Inovar Inovado | Iniciar sessão</title>

    <link
        href="Content/bootstrap.min.css"
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
            min-height: 100vh;
        }


        /* =====================================================
           LAYOUT
        ===================================================== */

        .login-page {
            width: 100%;
            min-height: 100vh;
            display: grid;
            grid-template-columns: minmax(420px, 1fr) minmax(500px, 1fr);
        }


        /* =====================================================
           LADO ESQUERDO
        ===================================================== */

        .login-brand-side {
            position: relative;
            display: flex;
            flex-direction: column;
            justify-content: center;
            padding: 70px;
            overflow: hidden;
            background: linear-gradient( 145deg, #0f2c61 0%, #123570 45%, #1d4ed8 100% );
            color: #ffffff;
        }

            .login-brand-side::before {
                content: "";
                position: absolute;
                width: 420px;
                height: 420px;
                border-radius: 50%;
                right: -180px;
                top: -140px;
                background: rgba(255, 255, 255, 0.07);
            }

            .login-brand-side::after {
                content: "";
                position: absolute;
                width: 300px;
                height: 300px;
                border-radius: 50%;
                left: -130px;
                bottom: -130px;
                background: rgba(255, 255, 255, 0.06);
            }

        .brand-content {
            position: relative;
            z-index: 2;
            width: 100%;
            max-width: 580px;
            margin: 0 auto;
        }

        .brand-logo {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            width: 58px;
            height: 58px;
            margin-bottom: 25px;
            border-radius: 16px;
            background: rgba(255, 255, 255, 0.14);
            border: 1px solid rgba(255, 255, 255, 0.22);
            backdrop-filter: blur(8px);
            color: #ffffff;
            font-size: 23px;
            font-weight: 800;
        }

        .brand-title {
            margin: 0 0 14px 0;
            font-size: 46px;
            line-height: 1.1;
            font-weight: 800;
            letter-spacing: -0.03em;
        }

        .brand-description {
            max-width: 500px;
            margin: 0 0 35px 0;
            color: #dbeafe;
            font-size: 17px;
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
           LADO DO LOGIN
        ===================================================== */

        .login-form-side {
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 45px;
            background: #f3f6fb;
        }

        .login-wrapper {
            width: 100%;
            max-width: 440px;
        }

        .mobile-brand {
            display: none;
            margin-bottom: 30px;
            color: #123570;
            font-size: 24px;
            font-weight: 800;
        }

        .login-header {
            margin-bottom: 28px;
        }

            .login-header h1 {
                margin: 0 0 8px 0;
                color: #111827;
                font-size: 31px;
                font-weight: 800;
                letter-spacing: -0.02em;
            }

            .login-header p {
                margin: 0;
                color: #64748b;
                font-size: 15px;
                line-height: 1.6;
            }

        .login-card {
            padding: 32px;
            background: #ffffff;
            border: 1px solid rgba(15, 23, 42, 0.08);
            border-radius: 18px;
            box-shadow: 0 15px 40px rgba(15, 23, 42, 0.08);
        }


        /* =====================================================
           CAMPOS
        ===================================================== */

        .login-field {
            margin-bottom: 20px;
        }

        .login-label {
            display: block;
            margin-bottom: 7px;
            color: #334155;
            font-size: 14px;
            font-weight: 650;
        }

        .login-input {
            width: 100%;
            height: 48px;
            padding: 10px 14px;
            border: 1px solid #d8dee9;
            border-radius: 10px;
            background: #f8fafc;
            color: #1f2937;
            font-size: 15px;
            outline: none;
            transition: border-color 0.2s, box-shadow 0.2s, background 0.2s;
        }

            .login-input:focus {
                background: #ffffff;
                border-color: #2563eb;
                box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.12);
            }

            .login-input::placeholder {
                color: #94a3b8;
            }

        .remember-area {
            display: flex;
            align-items: center;
            margin-bottom: 22px;
            color: #64748b;
            font-size: 14px;
        }

            .remember-area input {
                margin-right: 7px;
                accent-color: #2563eb;
            }


        /* =====================================================
           BOTÃO
        ===================================================== */

        .login-button {
            width: 100%;
            min-height: 48px;
            border: 0;
            border-radius: 10px;
            background: linear-gradient( 135deg, #123570, #2563eb );
            color: #ffffff;
            font-size: 15px;
            font-weight: 700;
            cursor: pointer;
            transition: transform 0.15s, box-shadow 0.2s, opacity 0.2s;
        }

            .login-button:hover {
                opacity: 0.95;
                box-shadow: 0 8px 20px rgba(37, 99, 235, 0.24);
            }

            .login-button:active {
                transform: translateY(1px);
            }


        /* =====================================================
           ERROS
        ===================================================== */

        .login-error {
            display: block;
            margin-bottom: 18px;
            padding: 10px 12px;
            border: 1px solid #fecaca;
            border-radius: 8px;
            background: #fef2f2;
            color: #b91c1c;
            font-size: 13px;
        }

        .field-error {
            display: block;
            margin-top: 5px;
            color: #dc2626;
            font-size: 12px;
        }


        /* =====================================================
           RODAPÉ DO LOGIN
        ===================================================== */

        .login-footer {
            margin-top: 25px;
            color: #94a3b8;
            text-align: center;
            font-size: 12px;
        }


        /* =====================================================
           RESPONSIVO
        ===================================================== */

        @media (max-width: 1000px) {

            .login-page {
                grid-template-columns: 0.85fr 1fr;
            }

            .login-brand-side {
                padding: 45px;
            }

            .brand-title {
                font-size: 38px;
            }

            .login-form-side {
                padding: 30px;
            }
        }

        @media (max-width: 780px) {

            .login-page {
                display: block;
            }

            .login-brand-side {
                display: none;
            }

            .login-form-side {
                min-height: 100vh;
                padding: 25px 18px;
            }

            .mobile-brand {
                display: block;
            }

            .login-card {
                padding: 25px;
            }

            .login-header h1 {
                font-size: 27px;
            }
        }

        @media (max-width: 400px) {

            .login-card {
                padding: 22px 18px;
            }
        }

        .admin-access {
            margin-top: 16px;
            text-align: center;
        }

        .admin-access-link {
            color: #94a3b8;
            font-size: 12px;
            text-decoration: none;
            transition: color 0.2s;
        }

            .admin-access-link:hover {
                color: #2563eb;
                text-decoration: underline;
            }
    </style>

    <style>
        .password-recovery-area {
    margin-top: 11px;
    text-align: center;
}

.password-recovery-link {
    display: inline-block;
    padding: 2px 4px;

    color: #7b8ba3;

    font-size: 12px;
    font-weight: 600;
    text-decoration: none;

    transition:
        color 0.2s ease,
        text-decoration-color 0.2s ease;
}

.password-recovery-link:hover {
    color: #2457d6;
    text-decoration: underline;
}

.password-recovery-link:focus {
    color: #2457d6;
    outline: none;
    text-decoration: underline;
}

/* =====================================================
   MOSTRAR/OCULTAR PALAVRA-PASSE
===================================================== */

.password-input-wrapper {
    position: relative;
}

.password-input-wrapper .login-input {
    padding-right: 52px;
}

.password-toggle {
    position: absolute;
    top: 50%;
    right: 9px;

    width: 36px;
    height: 36px;

    display: flex;
    align-items: center;
    justify-content: center;

    padding: 0;

    border: none;
    border-radius: 8px;

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
    background: #eff6ff;
    color: #2563eb;
}

.password-toggle:focus {
    outline: none;

    box-shadow:
        0 0 0 3px rgba(37, 99, 235, 0.14);
}

.password-toggle svg {
    width: 21px;
    height: 21px;

    pointer-events: none;
}

.password-toggle .icon-hidden {
    display: none;
}
    </style>

</head>


<body>

    <form
        id="form1"
        runat="server">

        <div class="login-page">


            <!-- ==================================================
                 ÁREA INSTITUCIONAL
            =================================================== -->

            <section class="login-brand-side">

                <div class="brand-content">

                    <div class="brand-logo">
                        II
                    </div>

                    <h1 class="brand-title">Inovar Inovado
                    </h1>

                    <p class="brand-description">
                        Uma plataforma criada para aproximar alunos,
                        professores e instituições num único espaço digital.
                    </p>

                    <div class="brand-features">

                        <div class="brand-feature">

                            <span class="feature-check">✓
                            </span>

                            Consulte eventos e atividades escolares.

                        </div>

                        <div class="brand-feature">

                            <span class="feature-check">✓
                            </span>

                            Partilhe informações com a sua turma.

                        </div>

                        <div class="brand-feature">

                            <span class="feature-check">✓
                            </span>

                            Acompanhe avaliações e atividades.

                        </div>

                    </div>

                </div>

            </section>


            <!-- ==================================================
     FORMULÁRIO
=================================================== -->

            <section class="login-form-side">

                <div class="login-wrapper">

                    <div class="mobile-brand">
                        Inovar Inovado
                    </div>

                    <div class="login-header">

                        <h1>Bem-vindo
                        </h1>

                        <p>
                            Introduza as suas credenciais para aceder à plataforma.
                        </p>

                    </div>

                    <div class="login-card">

                        <asp:Login
                            ID="loginUtilizador"
                            runat="server"
                            DisplayRememberMe="true"
                            FailureText="Nome de utilizador ou palavra-passe incorretos."
                            OnLoggedIn="loginUtilizador_LoggedIn">



                            <LayoutTemplate>


                                <asp:Literal
                                    ID="FailureText"
                                    runat="server"
                                    EnableViewState="false" />

                                <div class="login-field">

                                    <asp:Label
                                        ID="UserNameLabel"
                                        runat="server"
                                        AssociatedControlID="UserName"
                                        CssClass="login-label">

                            Nome de utilizador

                                    </asp:Label>

                                    <asp:TextBox
                                        ID="UserName"
                                        runat="server"
                                        CssClass="login-input"
                                        placeholder="Introduza o seu utilizador" />

                                    <asp:RequiredFieldValidator
                                        ID="UserNameRequired"
                                        runat="server"
                                        ControlToValidate="UserName"
                                        ErrorMessage="Introduza o nome de utilizador."
                                        ValidationGroup="loginUtilizador"
                                        CssClass="field-error"
                                        Display="Dynamic">

                            Introduza o nome de utilizador.

                                    </asp:RequiredFieldValidator>

                                </div>

                                <div class="login-field">

                                    <asp:Label
                                        ID="PasswordLabel"
                                        runat="server"
                                        AssociatedControlID="Password"
                                        CssClass="login-label">

                            Palavra-passe

                                    </asp:Label>

                                    <div class="password-input-wrapper">

    <asp:TextBox
        ID="Password"
        runat="server"
        ClientIDMode="Static"
        CssClass="login-input"
        TextMode="Password"
        autocomplete="current-password"
        placeholder="Introduza a sua palavra-passe" />

    <button
        type="button"
        class="password-toggle"
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
                                        ID="PasswordRequired"
                                        runat="server"
                                        ControlToValidate="Password"
                                        ErrorMessage="Introduza a palavra-passe."
                                        ValidationGroup="loginUtilizador"
                                        CssClass="field-error"
                                        Display="Dynamic">

                            Introduza a palavra-passe.

                                    </asp:RequiredFieldValidator>

                                </div>

                                <div class="remember-area">

                                    <asp:CheckBox
                                        ID="RememberMe"
                                        runat="server"
                                        Text=" Manter sessão iniciada" />

                                </div>

                                <asp:Button
                                    ID="LoginButton"
                                    runat="server"
                                    CommandName="Login"
                                    Text="Iniciar sessão"
                                    ValidationGroup="loginUtilizador"
                                    CssClass="login-button" />

                            </LayoutTemplate>



                        </asp:Login>

                        <div class="password-recovery-area">

                            <asp:HyperLink
                                ID="LnkRecuperarPassword"
                                runat="server"
                                NavigateUrl="~/recuperar_password.aspx"
                                CssClass="password-recovery-link">

<span class="password-recovery-icon">↻</span>
Esqueci-me da palavra-passe

                            </asp:HyperLink>

                        </div>

                    </div>

                    <div class="admin-access">

                        <asp:HyperLink
                            ID="lnkAdministracao"
                            runat="server"
                            NavigateUrl="~/administrador/criarconta.aspx"
                            CssClass="admin-access-link"
                            Text="Acesso administrativo" />

                    </div>

                    <div class="login-footer">
                        © 2026 Inovar Inovado — Plataforma de Gestão Escolar
                    </div>

                </div>

            </section>

        </div>

    </form>

</body>
    <script>
    document.addEventListener(
        "DOMContentLoaded",
        function () {

            var botao =
                document.querySelector(
                    ".password-toggle"
                );

            var campo =
                document.getElementById(
                    "Password"
                );

            if (!botao || !campo) {
                return;
            }

            botao.addEventListener(
                "click",
                function () {

                    var estaOculta =
                        campo.type === "password";

                    campo.type =
                        estaOculta
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

                    if (estaOculta) {
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

                    var texto =
                        estaOculta
                            ? "Ocultar palavra-passe"
                            : "Mostrar palavra-passe";

                    botao.setAttribute(
                        "aria-label",
                        texto
                    );

                    botao.setAttribute(
                        "title",
                        texto
                    );

                    botao.setAttribute(
                        "aria-pressed",
                        estaOculta
                            ? "true"
                            : "false"
                    );

                    campo.focus();
                }
            );

        }
    );
    </script>
</html>
