<%@ Page Title="A minha conta"
    Language="C#"
    MasterPageFile="~/professor/ModeloProfessor.Master"
    AutoEventWireup="true"
    CodeBehind="conta.aspx.cs"
    Inherits="AlunoGest.professor.conta" %>

<asp:Content
    ID="Title"
    ContentPlaceHolderID="titleContent"
    runat="server">
    A minha conta

</asp:Content>


<asp:Content
    ID="Head"
    ContentPlaceHolderID="headContent"
    runat="server">

    <style>
        .perfil-card {
            width: 100%;
            max-width: 850px;
            margin: 0 auto;
            background: #ffffff;
            padding: 28px;
            border-radius: 14px;
            border: 1px solid rgba(15, 23, 42, 0.08);
            box-shadow: 0 4px 18px rgba(15, 23, 42, 0.08);
            text-align: left;
        }

        .perfil-header {
            margin-bottom: 25px;
        }

            .perfil-header h1 {
                margin: 0 0 5px 0;
                font-size: 28px;
                font-weight: 800;
                color: #1f2937;
            }

            .perfil-header p {
                margin: 0;
                color: #64748b;
                font-size: 14px;
            }

        .foto-area {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            margin: 25px 0;
        }

        .foto-perfil {
            width: 150px !important;
            height: 150px !important;
            min-width: 150px;
            min-height: 150px;
            max-width: 150px !important;
            max-height: 150px !important;
            object-fit: cover;
            border-radius: 50%;
            border: 4px solid #2563eb;
            box-shadow: 0 5px 15px rgba(15, 23, 42, 0.12);
            display: block;
        }

        .info-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 16px;
            margin-bottom: 25px;
        }

        .info-item {
            padding: 15px 17px;
            background: #f8fafc;
            border: 1px solid #e2e8f0;
            border-radius: 10px;
        }

        .info-label {
            display: block;
            margin-bottom: 4px;
            color: #64748b;
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.04em;
        }

        .info-value {
            color: #1f2937;
            font-size: 15px;
            font-weight: 600;
            word-break: break-word;
        }

        .perfil-section {
            padding-top: 25px;
            margin-top: 25px;
            border-top: 1px solid #e2e8f0;
        }

            .perfil-section h2 {
                margin: 0 0 5px 0;
                color: #1f2937;
                font-size: 20px;
                font-weight: 700;
            }

        .section-description {
            margin-bottom: 20px;
            color: #64748b;
            font-size: 14px;
        }

        .form-label {
            font-weight: 600;
            color: #334155;
        }

        .primeiro-acesso {
            margin-bottom: 20px;
        }

        .botoes-area {
            display: flex;
            align-items: center;
            gap: 10px;
            margin-top: 20px;
        }

        @media (max-width: 700px) {
            .perfil-card {
                padding: 20px;
            }

            .info-grid {
                grid-template-columns: 1fr;
            }

            .botoes-area {
                flex-direction: column;
                align-items: stretch;
            }
        }
    </style>

</asp:Content>


<asp:Content
    ID="Main"
    ContentPlaceHolderID="mainContent"
    runat="server">

    <div class="perfil-card">


        <div class="perfil-header">

            <h1>A minha conta
            </h1>

            <p>
                Consulte os seus dados e altere a fotografia ou as credenciais da conta.
            </p>

        </div>


        <asp:Panel
            ID="PainelPrimeiroAcesso"
            runat="server"
            Visible="false"
            CssClass="alert alert-info primeiro-acesso">
            Este é o primeiro acesso.
            Por segurança, escolha um novo utilizador e uma nova palavra-passe antes de continuar.

        </asp:Panel>


        <asp:Label
            ID="LblMensagem"
            runat="server"
            Visible="false" />


        <div class="foto-area">

            <asp:Image
                ID="ImgFotoPerfil"
                runat="server"
                CssClass="foto-perfil" />

        </div>


        <div class="info-grid">


            <div class="info-item">

                <span class="info-label">Nome completo
                </span>

                <div class="info-value">

                    <asp:Label
                        ID="LblNomeCompleto"
                        runat="server" />

                </div>

            </div>


            <div class="info-item">

                <span class="info-label">Número de processo
                </span>

                <div class="info-value">

                    <asp:Label
                        ID="LblNumeroProcesso"
                        runat="server" />

                </div>
                <div class="info-item">

                    <span class="info-label">Número de telemóvel
                    </span>

                    <div class="info-value">

                        <asp:Label
                            ID="LblTelefone"
                            runat="server" />

                    </div>

                </div>


                <div class="info-item">

                    <span class="info-label">NIF
                    </span>

                    <div class="info-value">

                        <asp:Label
                            ID="LblNIF"
                            runat="server" />

                    </div>

                </div>

            </div>


            <div class="info-item">

                <span class="info-label">Utilizador
                </span>

                <div class="info-value">

                    <asp:Label
                        ID="LblUsername"
                        runat="server" />

                </div>

            </div>


            <div class="info-item">

                <span class="info-label">Email
                </span>

                <div class="info-value">

                    <asp:Label
                        ID="LblEmail"
                        runat="server" />

                </div>

            </div>


        </div>


        <section class="perfil-section">

            <h2>Alterar fotografia de perfil
            </h2>

            <p class="section-description">
                Escolha uma imagem JPG, JPEG ou PNG com um máximo de 5 MB.
            </p>


            <div class="mb-3">

                <asp:FileUpload
                    ID="FileFoto"
                    runat="server"
                    CssClass="form-control" />

            </div>


            <asp:Button
                ID="BtnAlterarFoto"
                runat="server"
                Text="Alterar fotografia"
                CssClass="btn btn-primary"
                OnClick="BtnAlterarFoto_Click"
                CausesValidation="false" />

        </section>


        <section class="perfil-section">

            <h2>Alterar credenciais
            </h2>

            <p class="section-description">
                Pode alterar o seu nome de utilizador e a palavra-passe.
            </p>


            <div class="mb-3">

                <label class="form-label">
                    Novo utilizador
                </label>

                <asp:TextBox
                    ID="TxtUsername"
                    runat="server"
                    CssClass="form-control"
                    MaxLength="50" />

                <asp:RequiredFieldValidator
                    runat="server"
                    ControlToValidate="TxtUsername"
                    ErrorMessage="Indique o utilizador."
                    CssClass="text-danger small d-block"
                    ValidationGroup="conta" />

                <asp:RegularExpressionValidator
                    runat="server"
                    ControlToValidate="TxtUsername"
                    ValidationExpression="^[A-Za-z0-9._-]{4,50}$"
                    ErrorMessage="Use entre 4 e 50 letras, números, ponto, hífen ou underscore."
                    CssClass="text-danger small d-block"
                    ValidationGroup="conta" />

            </div>


            <div class="mb-3">

                <label class="form-label">
                    Palavra-passe atual
                </label>

                <asp:TextBox
                    ID="TxtPasswordAtual"
                    runat="server"
                    CssClass="form-control"
                    TextMode="Password" />

                <asp:RequiredFieldValidator
                    runat="server"
                    ControlToValidate="TxtPasswordAtual"
                    ErrorMessage="Indique a palavra-passe atual."
                    CssClass="text-danger small d-block"
                    ValidationGroup="conta" />

            </div>


            <div class="row g-3">


                <div class="col-md-6">

                    <label class="form-label">
                        Nova palavra-passe
                    </label>

                    <asp:TextBox
                        ID="TxtNovaPassword"
                        runat="server"
                        CssClass="form-control"
                        TextMode="Password" />

                    <asp:RequiredFieldValidator
                        runat="server"
                        ControlToValidate="TxtNovaPassword"
                        ErrorMessage="Indique a nova palavra-passe."
                        CssClass="text-danger small d-block"
                        ValidationGroup="conta" />

                </div>


                <div class="col-md-6">

                    <label class="form-label">
                        Confirmar nova palavra-passe
                    </label>

                    <asp:TextBox
                        ID="TxtConfirmarPassword"
                        runat="server"
                        CssClass="form-control"
                        TextMode="Password" />

                    <asp:RequiredFieldValidator
                        runat="server"
                        ControlToValidate="TxtConfirmarPassword"
                        ErrorMessage="Confirme a nova palavra-passe."
                        CssClass="text-danger small d-block"
                        ValidationGroup="conta" />

                    <asp:CompareValidator
                        runat="server"
                        ControlToValidate="TxtConfirmarPassword"
                        ControlToCompare="TxtNovaPassword"
                        ErrorMessage="As palavras-passe não coincidem."
                        CssClass="text-danger small d-block"
                        ValidationGroup="conta" />

                </div>


            </div>


            <div class="botoes-area">

                <asp:Button
                    ID="BtnGuardar"
                    runat="server"
                    Text="Guardar novas credenciais"
                    CssClass="btn btn-warning"
                    OnClick="BtnGuardar_Click"
                    ValidationGroup="conta" />

            </div>

        </section>


    </div>

</asp:Content>
