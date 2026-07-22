<%@ Page
    Title="Minha Conta"
    Language="C#"
    MasterPageFile="~/encarregado/modeloEncarregado.Master"
    AutoEventWireup="true"
    CodeBehind="MinhaConta.aspx.cs"
    Inherits="AlunoGest.encarregado.MinhaConta"
    MaintainScrollPositionOnPostback="true" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="headContent"
    runat="server">

    <style>

        .perfil-page {
            padding-bottom: 40px;
        }

        .perfil-card {
            width: 100%;
            max-width: 900px;

            margin: 0 auto;
            padding: 28px;

            border: 1px solid rgba(15, 23, 42, 0.08);
            border-radius: 15px;

            background: #ffffff;

            box-shadow:
                0 5px 18px rgba(15, 23, 42, 0.07);
        }

        .perfil-header {
            margin-bottom: 25px;
        }

        .perfil-header h1 {
            margin: 0 0 6px;

            color: #1f2937;

            font-size: 29px;
            font-weight: 800;
        }

        .perfil-header p {
            margin: 0;

            color: #64748b;

            font-size: 14px;
            line-height: 1.6;
        }


        /* =====================================================
           IDENTIFICAÇÃO
        ===================================================== */

        .perfil-identificacao {
            display: flex;
            align-items: center;

            gap: 18px;

            margin: 24px 0;
            padding: 20px;

            border: 1px solid #dbeafe;
            border-radius: 13px;

            background: #f8fbff;
        }

        .perfil-iniciais {
            width: 72px;
            height: 72px;

            flex-shrink: 0;

            display: flex;
            align-items: center;
            justify-content: center;

            border: 3px solid #bfdbfe;
            border-radius: 50%;

            background: #eff6ff;
            color: #1d4ed8;

            font-size: 23px;
            font-weight: 800;
        }

        .perfil-identificacao h2 {
            margin: 0 0 4px;

            color: #1f2937;

            font-size: 20px;
            font-weight: 800;
        }

        .perfil-identificacao p {
            margin: 0;

            color: #64748b;

            font-size: 13px;
        }


        /* =====================================================
           INFORMAÇÕES
        ===================================================== */

        .info-grid {
            display: grid;

            grid-template-columns:
                repeat(2, minmax(0, 1fr));

            gap: 16px;

            margin-bottom: 25px;
        }

        .info-item {
            padding: 15px 17px;

            border: 1px solid #e2e8f0;
            border-radius: 10px;

            background: #f8fafc;
        }

        .info-label {
            display: block;

            margin-bottom: 5px;

            color: #64748b;

            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;

            letter-spacing: 0.04em;
        }

        .info-value {
            color: #1f2937;

            font-size: 15px;
            font-weight: 650;

            word-break: break-word;
        }

        .estado-ativo,
        .estado-inativo {
            display: inline-flex;

            padding: 4px 9px;

            border-radius: 999px;

            font-size: 11px;
            font-weight: 700;
        }

        .estado-ativo {
            border: 1px solid #bbf7d0;

            background: #f0fdf4;
            color: #166534;
        }

        .estado-inativo {
            border: 1px solid #e2e8f0;

            background: #f8fafc;
            color: #64748b;
        }


        /* =====================================================
           SECÇÕES
        ===================================================== */

        .perfil-section {
            margin-top: 27px;
            padding-top: 26px;

            border-top: 1px solid #e2e8f0;
        }

        .perfil-section h2 {
            margin: 0 0 5px;

            color: #1f2937;

            font-size: 20px;
            font-weight: 800;
        }

        .section-description {
            margin: 0 0 20px;

            color: #64748b;

            font-size: 14px;
            line-height: 1.6;
        }

        .form-label {
            color: #334155;

            font-size: 13px;
            font-weight: 700;
        }

        .validation-message {
            display: block;

            margin-top: 5px;

            color: #dc2626;

            font-size: 12px;
            font-weight: 600;
        }

        .campo-ajuda {
            display: block;

            margin-top: 5px;

            color: #64748b;

            font-size: 12px;
        }

        .dados-grid,
        .password-grid {
            display: grid;

            grid-template-columns:
                repeat(2, minmax(0, 1fr));

            gap: 16px;
        }

        .campo-completo {
            grid-column: 1 / -1;
        }

        .botoes-area {
            display: flex;
            align-items: center;

            gap: 10px;

            margin-top: 20px;
        }


        /* =====================================================
           RESPONSIVO
        ===================================================== */

        @media (max-width: 700px) {

            .perfil-card {
                padding: 20px;
            }

            .info-grid,
            .dados-grid,
            .password-grid {
                grid-template-columns: 1fr;
            }

            .campo-completo {
                grid-column: auto;
            }

            .perfil-identificacao {
                align-items: flex-start;
            }

            .perfil-iniciais {
                width: 60px;
                height: 60px;

                font-size: 19px;
            }

            .botoes-area {
                align-items: stretch;
                flex-direction: column;
            }

            .botoes-area .btn {
                width: 100%;
            }
        }

    </style>

</asp:Content>


<asp:Content
    ID="Content2"
    ContentPlaceHolderID="mainContent"
    runat="server">

    <div class="perfil-page">

        <div class="perfil-card">


            <!-- ==================================================
                 CABEÇALHO
            =================================================== -->

            <div class="perfil-header">

                <h1>
                    Minha Conta
                </h1>

                <p>
                    Consulte os seus dados pessoais e atualize
                    as informações de contacto da sua conta.
                </p>

            </div>


            <!-- ==================================================
                 MENSAGEM
            =================================================== -->

            <asp:Label
                ID="LblMensagem"
                runat="server"
                Visible="false" />


            <!-- ==================================================
                 IDENTIFICAÇÃO
            =================================================== -->

            <div class="perfil-identificacao">

                <div class="perfil-iniciais">

                    

                    <asp:Label
                        ID="LblIniciais"
                        runat="server"
                        Text="?" />

                </div>

                <div>

                    <h2>

                        <asp:Label
                            ID="LblNomeCabecalho"
                            runat="server" />

                    </h2>

                    <p>
                        Encarregado de Educação
                    </p>

                </div>

            </div>


            <!-- ==================================================
                 INFORMAÇÕES DA CONTA
            =================================================== -->

            <div class="info-grid">


                <!-- USERNAME -->

                <div class="info-item">

                    <span class="info-label">
                        Nome de utilizador
                    </span>

                    <div class="info-value">

                        <asp:Label
                            ID="LblUsername"
                            runat="server"
                            Text="Não definido" />

                    </div>

                </div>


                <!-- NIF -->

                <div class="info-item">

                    <span class="info-label">
                        NIF
                    </span>

                    <div class="info-value">

                        <asp:Label
                            ID="LblNIF"
                            runat="server"
                            Text="Não definido" />

                    </div>

                </div>


                <!-- EMAIL -->

                <div class="info-item">

                    <span class="info-label">
                        Email
                    </span>

                    <div class="info-value">

                        <asp:Label
                            ID="LblEmail"
                            runat="server"
                            Text="Não definido" />

                    </div>

                </div>


                <!-- TELEFONE -->

                <div class="info-item">

                    <span class="info-label">
                        Telefone
                    </span>

                    <div class="info-value">

                        <asp:Label
                            ID="LblTelefone"
                            runat="server"
                            Text="Não definido" />

                    </div>

                </div>


                <!-- ESTADO -->

                <div class="info-item">

                    <span class="info-label">
                        Estado da conta
                    </span>

                    <div class="info-value">

                        <asp:Label
                            ID="LblEstado"
                            runat="server"
                            Text="Ativo"
                            CssClass="estado-ativo" />

                    </div>

                </div>


                <!-- DATA DE CRIAÇÃO -->

                <div class="info-item">

                    <span class="info-label">
                        Conta criada em
                    </span>

                    <div class="info-value">

                        <asp:Label
                            ID="LblDataCriacao"
                            runat="server"
                            Text="Não definido" />

                    </div>

                </div>

            </div>


            <!-- ==================================================
                 ALTERAR DADOS
            =================================================== -->

            <section class="perfil-section">

                <h2>
                    Alterar dados pessoais
                </h2>

                <p class="section-description">
                    Pode alterar o nome, o email e o telefone
                    associados à sua conta.
                </p>


                <asp:ValidationSummary
                    ID="ValidationSummaryDados"
                    runat="server"
                    ValidationGroup="dados"
                    HeaderText="Verifique os seguintes campos:"
                    DisplayMode="BulletList"
                    CssClass="alert alert-warning" />


                <div class="dados-grid">


                    <!-- NOME -->

                    <div class="campo-completo">

                        <asp:Label
                            ID="LabelNome"
                            runat="server"
                            AssociatedControlID="TxtNomeCompleto"
                            CssClass="form-label"
                            Text="Nome completo" />

                        <asp:TextBox
                            ID="TxtNomeCompleto"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="200" />

                        <asp:RequiredFieldValidator
                            ID="RfvNomeCompleto"
                            runat="server"
                            ControlToValidate="TxtNomeCompleto"
                            ValidationGroup="dados"
                            Display="Dynamic"
                            CssClass="validation-message"
                            ErrorMessage="O nome completo é obrigatório." />

                    </div>


                    <!-- EMAIL -->

                    <div>

                        <asp:Label
                            ID="LabelEmail"
                            runat="server"
                            AssociatedControlID="TxtEmail"
                            CssClass="form-label"
                            Text="Email" />

                        <asp:TextBox
                            ID="TxtEmail"
                            runat="server"
                            CssClass="form-control"
                            TextMode="Email"
                            MaxLength="150" />

                        <asp:RequiredFieldValidator
                            ID="RfvEmail"
                            runat="server"
                            ControlToValidate="TxtEmail"
                            ValidationGroup="dados"
                            Display="Dynamic"
                            CssClass="validation-message"
                            ErrorMessage="O email é obrigatório." />

                        <asp:RegularExpressionValidator
                            ID="RevEmail"
                            runat="server"
                            ControlToValidate="TxtEmail"
                            ValidationGroup="dados"
                            ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                            Display="Dynamic"
                            CssClass="validation-message"
                            ErrorMessage="Introduza um email válido." />

                    </div>


                    <!-- TELEFONE -->

                    <div>

                        <asp:Label
                            ID="LabelTelefone"
                            runat="server"
                            AssociatedControlID="TxtTelefone"
                            CssClass="form-label"
                            Text="Telefone" />

                        <asp:TextBox
                            ID="TxtTelefone"
                            runat="server"
                            CssClass="form-control"
                            TextMode="Phone"
                            MaxLength="20" />

                        <asp:RequiredFieldValidator
                            ID="RfvTelefone"
                            runat="server"
                            ControlToValidate="TxtTelefone"
                            ValidationGroup="dados"
                            Display="Dynamic"
                            CssClass="validation-message"
                            ErrorMessage="O telefone é obrigatório." />

                        <asp:RegularExpressionValidator
                            ID="RevTelefone"
                            runat="server"
                            ControlToValidate="TxtTelefone"
                            ValidationGroup="dados"
                            ValidationExpression="^[0-9+\s()\-]{7,20}$"
                            Display="Dynamic"
                            CssClass="validation-message"
                            ErrorMessage="Introduza um telefone válido." />

                    </div>

                </div>


                <div class="botoes-area">

                    <asp:Button
                        ID="BtnGuardarDados"
                        runat="server"
                        Text="Guardar alterações"
                        CssClass="btn btn-primary"
                        ValidationGroup="dados"
                        OnClick="BtnGuardarDados_Click" />

                    <asp:Button
                        ID="BtnCancelarDados"
                        runat="server"
                        Text="Cancelar"
                        CssClass="btn btn-outline-secondary"
                        CausesValidation="false"
                        OnClick="BtnCancelarDados_Click" />

                </div>

            </section>


            <!-- ==================================================
                 ALTERAR PALAVRA-PASSE
            =================================================== -->

            <section class="perfil-section">

                <h2>
                    Alterar palavra-passe
                </h2>

                <p class="section-description">
                    Introduza a palavra-passe atual e escolha
                    uma nova palavra-passe segura.
                </p>


                <asp:ValidationSummary
                    ID="ValidationSummarySenha"
                    runat="server"
                    ValidationGroup="senha"
                    HeaderText="Verifique os seguintes campos:"
                    DisplayMode="BulletList"
                    CssClass="alert alert-warning" />


                <div class="password-grid">


                    <!-- SENHA ATUAL -->

                    <div class="campo-completo">

                        <asp:Label
                            ID="LabelSenhaAtual"
                            runat="server"
                            AssociatedControlID="TxtSenhaAtual"
                            CssClass="form-label"
                            Text="Palavra-passe atual" />

                        <asp:TextBox
                            ID="TxtSenhaAtual"
                            runat="server"
                            TextMode="Password"
                            CssClass="form-control" />

                        <asp:RequiredFieldValidator
                            ID="RfvSenhaAtual"
                            runat="server"
                            ControlToValidate="TxtSenhaAtual"
                            ValidationGroup="senha"
                            Display="Dynamic"
                            CssClass="validation-message"
                            ErrorMessage="A palavra-passe atual é obrigatória." />

                    </div>


                    <!-- NOVA SENHA -->

                    <div>

                        <asp:Label
                            ID="LabelNovaSenha"
                            runat="server"
                            AssociatedControlID="TxtNovaSenha"
                            CssClass="form-label"
                            Text="Nova palavra-passe" />

                        <asp:TextBox
                            ID="TxtNovaSenha"
                            runat="server"
                            TextMode="Password"
                            CssClass="form-control" />

                        <asp:RequiredFieldValidator
                            ID="RfvNovaSenha"
                            runat="server"
                            ControlToValidate="TxtNovaSenha"
                            ValidationGroup="senha"
                            Display="Dynamic"
                            CssClass="validation-message"
                            ErrorMessage="A nova palavra-passe é obrigatória." />

                        <span class="campo-ajuda">
                            Utilize pelo menos 8 caracteres, incluindo
                            maiúscula, minúscula, número e carácter especial.
                        </span>

                    </div>


                    <!-- CONFIRMAÇÃO -->

                    <div>

                        <asp:Label
                            ID="LabelConfirmarSenha"
                            runat="server"
                            AssociatedControlID="TxtConfirmarSenha"
                            CssClass="form-label"
                            Text="Confirmar nova palavra-passe" />

                        <asp:TextBox
                            ID="TxtConfirmarSenha"
                            runat="server"
                            TextMode="Password"
                            CssClass="form-control" />

                        <asp:RequiredFieldValidator
                            ID="RfvConfirmarSenha"
                            runat="server"
                            ControlToValidate="TxtConfirmarSenha"
                            ValidationGroup="senha"
                            Display="Dynamic"
                            CssClass="validation-message"
                            ErrorMessage="Confirme a nova palavra-passe." />

                        <asp:CompareValidator
                            ID="CvConfirmarSenha"
                            runat="server"
                            ControlToValidate="TxtConfirmarSenha"
                            ControlToCompare="TxtNovaSenha"
                            ValidationGroup="senha"
                            Display="Dynamic"
                            CssClass="validation-message"
                            ErrorMessage="As palavras-passe não coincidem." />

                    </div>

                </div>


                <div class="botoes-area">

                    <asp:Button
                        ID="BtnAlterarSenha"
                        runat="server"
                        Text="Alterar palavra-passe"
                        CssClass="btn btn-warning"
                        ValidationGroup="senha"
                        OnClick="BtnAlterarSenha_Click" />

                </div>

            </section>

        </div>

    </div>

</asp:Content>