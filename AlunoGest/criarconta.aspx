<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="criarconta.aspx.cs" Inherits="AlunoGest.criarconta" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Inovar inovando</title>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />

    <style>
        .panel-form {
            width: 900px;
        }

        .form-row-custom {
            display: flex;
            align-items: center;
            margin-bottom: 1.2rem;
        }

        .form-label-custom {
            width: 220px;
        }

        .form-input-area {
            width: 600px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .input-600 {
            width: 100%;
        }

        .input-300 {
            width: 300px;
        }

        .input-200 {
            width: 200px;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <div class="d-flex justify-content-center align-items-center" style="min-height: 100vh;">
            <div class="p-4 bg-light border rounded panel-form">

                <asp:Panel runat="server">

                    <div class="row mb-3">
                        <div class="col-sm-12 text-center">
                            <h1>Criar conta</h1>
                        </div>
                    </div>


                    <!-- Nome -->
                    <div class="form-row-custom">
                        <label class="form-label-custom text-end me-2">Nome</label>
                        <div class="form-input-area">
                            <asp:TextBox ID="txtNome" runat="server"
                                CssClass="form-control border border-dark input-600" />
                            <asp:RequiredFieldValidator
                                runat="server"
                                ControlToValidate="txtNome"
                                ErrorMessage="Obrigatório"
                                CssClass="text-danger" />
                        </div>
                    </div>

                    <!-- Morada -->
                    <div class="form-row-custom">
                        <label class="form-label-custom text-end me-2">Morada</label>
                        <div class="form-input-area">
                            <asp:TextBox ID="txtMorada" runat="server"
                                CssClass="form-control border border-dark input-600"
                                TextMode="MultiLine" Rows="3" />
                            <asp:RequiredFieldValidator
                                runat="server"
                                ControlToValidate="txtMorada"
                                ErrorMessage="Obrigatório"
                                CssClass="text-danger" />
                        </div>
                    </div>

                    <!-- Código Postal -->
                    <div class="form-row-custom">
                        <label class="form-label-custom text-end me-2">Código Postal</label>
                        <div class="form-input-area">
                            <asp:TextBox ID="txtCodigoPostal" runat="server"
                                CssClass="form-control border border-dark input-600" />
                            <asp:RequiredFieldValidator
                                runat="server"
                                ControlToValidate="txtCodigoPostal"
                                ErrorMessage="Obrigatório"
                                CssClass="text-danger" />
                        </div>
                    </div>

                    <!-- Localidade -->
                    <div class="form-row-custom">
                        <label class="form-label-custom text-end me-2">Localidade</label>
                        <div class="form-input-area">
                            <asp:TextBox ID="txtLocalidade" runat="server"
                                CssClass="form-control border border-dark input-600" />
                            <asp:RequiredFieldValidator
                                runat="server"
                                ControlToValidate="txtLocalidade"
                                ErrorMessage="Obrigatório"
                                CssClass="text-danger" />
                        </div>
                    </div>

                    <!-- Telefone -->
                    <div class="form-row-custom">
                        <label class="form-label-custom text-end me-2">Telefone</label>
                        <div class="form-input-area">
                            <asp:TextBox ID="txtTelefone" runat="server"
                                CssClass="form-control border border-dark input-600" />
                            <asp:RequiredFieldValidator
                                runat="server"
                                ControlToValidate="txtTelefone"
                                ErrorMessage="Obrigatório"
                                CssClass="text-danger" />
                        </div>
                    </div>

                    <!-- Email -->
                    <div class="form-row-custom">
                        <label class="form-label-custom text-end me-2">Email</label>
                        <div class="form-input-area">
                            <asp:TextBox ID="txtEmail" runat="server"
                                CssClass="form-control border border-dark input-600" />
                            <asp:RegularExpressionValidator
                                runat="server"
                                ControlToValidate="txtEmail"
                                ErrorMessage="Email inválido"
                                CssClass="text-danger"
                                ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" />
                        </div>
                    </div>

                    <!-- Código MEC -->
                    <div class="form-row-custom">
                        <label class="form-label-custom text-end me-2">Código MEC</label>
                        <div class="form-input-area">
                            <asp:TextBox ID="txtCodigoMEC" runat="server"
                                CssClass="form-control border border-dark input-200" />
                        </div>
                    </div>

                    <!-- Username -->
                    <div class="form-row-custom">
                        <label class="form-label-custom text-end me-2">Username</label>
                        <div class="form-input-area">
                            <asp:TextBox ID="txtUsername" runat="server"
                                CssClass="form-control border border-dark input-300" />
                            <asp:RequiredFieldValidator
                                runat="server"
                                ControlToValidate="txtUsername"
                                ErrorMessage="Obrigatório"
                                CssClass="text-danger" />
                        </div>
                    </div>

                    <!-- Password -->
                    <div class="form-row-custom">
                        <label class="form-label-custom text-end me-2">Password</label>
                        <div class="form-input-area">
                            <asp:TextBox ID="txtPassword" runat="server"
                                TextMode="Password"
                                CssClass="form-control border border-dark input-300" />
                            <asp:RequiredFieldValidator
                                runat="server"
                                ControlToValidate="txtPassword"
                                ErrorMessage="Obrigatório"
                                CssClass="text-danger" />
                        </div>
                    </div>

                    <!-- Botões -->
                    <div class="form-row-custom mt-4">
                        <div class="form-label-custom"></div>
                        <div class="form-input-area">
                            <asp:Button ID="btnCriarConta" runat="server"
                                Text="Criar conta"
                                CssClass="btn btn-primary px-4" OnClick="btnCriarConta_Click" />
                            <asp:Button ID="btnCancelar" runat="server"
                                Text="Cancelar"
                                CssClass="btn btn-primary px-4 ms-4" CausesValidation="false" />
                        </div>
                    </div>

                </asp:Panel>

            </div>
        </div>
    </form>
</body>



</html>

